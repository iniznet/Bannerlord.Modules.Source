using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;

namespace TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer
{
	public class MPChatVM : ViewModel, IChatHandler
	{
		public ChatChannelType ActiveChannelType
		{
			get
			{
				return this._activeChannelType;
			}
			set
			{
				if ((value == ChatChannelType.All || value == ChatChannelType.Team) && !GameNetwork.IsClient)
				{
					this._activeChannelType = ChatChannelType.NaN;
					this.IsChatDisabled = true;
					return;
				}
				if (value != this._activeChannelType)
				{
					this._activeChannelType = value;
					this.RefreshActiveChannelNameData();
					this.IsChatDisabled = false;
				}
			}
		}

		private string _playerName
		{
			get
			{
				string text = ((NetworkMain.GameClient.PlayerData != null) ? NetworkMain.GameClient.Name : new TextObject("{=!}ERROR: MISSING PLAYERDATA", null).ToString());
				NetworkCommunicator myPeer = GameNetwork.MyPeer;
				MissionPeer missionPeer = ((myPeer != null) ? myPeer.GetComponent<MissionPeer>() : null);
				if (missionPeer != null && !missionPeer.IsAgentAliveForChatting)
				{
					GameTexts.SetVariable("PLAYER_NAME", "{=!}" + text);
					text = GameTexts.FindText("str_chat_message_dead_player", null).ToString();
				}
				return text;
			}
		}

		public MPChatVM()
		{
			this._allMessages = new List<MPChatLineVM>();
			this._requestedMessages = new Queue<MPChatLineVM>();
			this.MessageHistory = new MBBindingList<MPChatLineVM>();
			this.CombatLogHint = new HintViewModel();
			this.IncludeCombatLog = BannerlordConfig.ReportDamage;
			this.IncludeBark = BannerlordConfig.ReportBark;
			InformationManager.DisplayMessageInternal += this.OnDisplayMessageReceived;
			InformationManager.ClearAllMessagesInternal += this.ClearAllMessages;
			ManagedOptions.OnManagedOptionChanged = (ManagedOptions.OnManagedOptionChangedDelegate)Delegate.Combine(ManagedOptions.OnManagedOptionChanged, new ManagedOptions.OnManagedOptionChangedDelegate(this.OnOptionChange));
			this.MaxMessageLength = 100;
			this._recentlySentMessagesTimes = new List<float>();
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.CombatLogHint.HintText = new TextObject("{=FRSGOfUJ}Toggle include Combat Log", null);
			this.ToggleCombatLogText = new TextObject("{=rx18kyZb}Combat Log", null).ToString();
			this.ToggleBarkText = new TextObject("{=NuMQvQxg}Shouts", null).ToString();
			this.UpdateHideShowText(this._isInspectingMessages);
			this.UpdateShortcutTexts();
			this.RefreshActiveChannelNameData();
		}

		private void RefreshActiveChannelNameData()
		{
			if (this.ActiveChannelType == ChatChannelType.NaN)
			{
				this.ActiveChannelNameText = string.Empty;
				this.ActiveChannelColor = Color.White;
				return;
			}
			if (this.ActiveChannelType == ChatChannelType.Custom)
			{
				this.ActiveChannelNameText = "(" + this._currentCustomChatChannel.Name + ")";
				this.ActiveChannelColor = Color.ConvertStringToColor(this._currentCustomChatChannel.RoomColor);
				return;
			}
			string text = GameTexts.FindText("str_multiplayer_chat_channel", this.ActiveChannelType.ToString()).ToString();
			GameTexts.SetVariable("STR", text);
			this.ActiveChannelNameText = GameTexts.FindText("str_STR_in_parentheses", null).ToString();
			this.ActiveChannelColor = this.GetChannelColor(this.ActiveChannelType);
		}

		private void OnOptionChange(ManagedOptions.ManagedOptionsType changedManagedOptionsType)
		{
			if (changedManagedOptionsType == ManagedOptions.ManagedOptionsType.ReportDamage)
			{
				this.IncludeCombatLog = BannerlordConfig.ReportDamage;
				return;
			}
			if (changedManagedOptionsType == ManagedOptions.ManagedOptionsType.ReportBark)
			{
				this.IncludeBark = BannerlordConfig.ReportBark;
			}
		}

		public void ToggleIncludeCombatLog()
		{
			this.IncludeCombatLog = !this.IncludeCombatLog;
		}

		public void ExecuteToggleIncludeShouts()
		{
			this.IncludeBark = !this.IncludeBark;
		}

		private void UpdateHideShowText(bool isInspecting)
		{
			TextObject textObject = TextObject.Empty;
			if (this._game != null && isInspecting)
			{
				textObject = this._hideText;
				textObject.SetTextVariable("KEY", this._getToggleChatKeyText() ?? TextObject.Empty);
			}
			this.HideShowText = textObject.ToString();
		}

		private void UpdateShortcutTexts()
		{
			TextObject cycleChannelsText = this._cycleChannelsText;
			string text = "KEY";
			Func<TextObject> getCycleChannelsKeyText = this._getCycleChannelsKeyText;
			cycleChannelsText.SetTextVariable(text, ((getCycleChannelsKeyText != null) ? getCycleChannelsKeyText() : null) ?? TextObject.Empty);
			this.CycleThroughChannelsText = this._cycleChannelsText.ToString();
			if (Input.IsGamepadActive)
			{
				TextObject sendMessageTextObject = this._sendMessageTextObject;
				string text2 = "KEY";
				Func<TextObject> getSendMessageKeyText = this._getSendMessageKeyText;
				sendMessageTextObject.SetTextVariable(text2, ((getSendMessageKeyText != null) ? getSendMessageKeyText() : null) ?? TextObject.Empty);
				this.SendMessageText = this._sendMessageTextObject.ToString();
				TextObject cancelSendingTextObject = this._cancelSendingTextObject;
				string text3 = "KEY";
				Func<TextObject> getCancelSendingKeyText = this._getCancelSendingKeyText;
				cancelSendingTextObject.SetTextVariable(text3, ((getCancelSendingKeyText != null) ? getCancelSendingKeyText() : null) ?? TextObject.Empty);
				this.CancelSendingText = this._cancelSendingTextObject.ToString();
				return;
			}
			this.SendMessageText = string.Empty;
			this.CancelSendingText = string.Empty;
		}

		public void Tick(float dt)
		{
			while (this._requestedMessages.Count > 0)
			{
				this.AddChatLine(this._requestedMessages.Dequeue());
			}
			float applicationTime = Time.ApplicationTime;
			for (int i = 0; i < this._recentlySentMessagesTimes.Count; i++)
			{
				if (applicationTime - this._recentlySentMessagesTimes[i] >= 15f)
				{
					this._recentlySentMessagesTimes.RemoveAt(i);
				}
			}
			this.CheckChatFading(dt);
		}

		public void Clear()
		{
			this._allMessages.ForEach(delegate(MPChatLineVM l)
			{
				l.ForceInvisible();
			});
			this.MessageHistory.ToList<MPChatLineVM>().ForEach(delegate(MPChatLineVM l)
			{
				l.ForceInvisible();
			});
			this._allMessages.Clear();
			this.MessageHistory.Clear();
		}

		private void OnDisplayMessageReceived(InformationMessage informationMessage)
		{
			if (this.IsChatAllowedByOptions())
			{
				this.HandleAddChatLineRequest(informationMessage);
			}
		}

		private void ClearAllMessages()
		{
			this.Clear();
		}

		public void UpdateObjects(Game game, Mission mission)
		{
			if (this._game != game)
			{
				if (this._game != null)
				{
					this.ClearGame();
				}
				this._game = game;
				if (this._game != null)
				{
					this.SetGame();
				}
			}
			if (this._mission != mission)
			{
				if (this._mission != null)
				{
					this.ClearMission();
				}
				this._mission = mission;
				if (this._mission != null)
				{
					this.SetMission();
				}
			}
			if (this._game != null)
			{
				ChatBox gameHandler = this._game.GetGameHandler<ChatBox>();
				if (this._chatBox != gameHandler)
				{
					if (this._chatBox != null)
					{
						this.ClearChatBox();
					}
					this._chatBox = gameHandler;
					if (this._chatBox != null)
					{
						this.SetChatBox();
					}
				}
			}
			this.IsOptionsAvailable = this.IsInspectingMessages && this.IsTypingText;
		}

		private void ClearGame()
		{
			this._game = null;
		}

		private void ClearChatBox()
		{
			if (this._chatBox != null)
			{
				this._chatBox.PlayerMessageReceived -= this.OnPlayerMessageReceived;
				this._chatBox.WhisperMessageSent -= this.OnWhisperMessageSent;
				this._chatBox.WhisperMessageReceived -= this.OnWhisperMessageReceived;
				this._chatBox.ErrorWhisperMessageReceived -= this.OnErrorWhisperMessageReceived;
				this._chatBox.ServerMessage -= this.OnServerMessage;
				this._chatBox = null;
			}
		}

		private void SetGame()
		{
			this.UpdateHideShowText(this.IsInspectingMessages);
		}

		private void SetChatBox()
		{
			this._chatBox.PlayerMessageReceived += this.OnPlayerMessageReceived;
			this._chatBox.WhisperMessageSent += this.OnWhisperMessageSent;
			this._chatBox.WhisperMessageReceived += this.OnWhisperMessageReceived;
			this._chatBox.ErrorWhisperMessageReceived += this.OnErrorWhisperMessageReceived;
			this._chatBox.ServerMessage += this.OnServerMessage;
		}

		private void SetMission()
		{
			Game game = Game.Current;
			bool flag;
			if (game == null)
			{
				flag = false;
			}
			else
			{
				ChatBox gameHandler = game.GetGameHandler<ChatBox>();
				bool? flag2 = ((gameHandler != null) ? new bool?(gameHandler.IsContentRestricted) : null);
				bool flag3 = true;
				flag = (flag2.GetValueOrDefault() == flag3) & (flag2 != null);
			}
			this.IsChatDisabled = flag;
		}

		private void ClearMission()
		{
			this._mission = null;
			this.ActiveChannelType = ChatChannelType.NaN;
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			if (this._game != null)
			{
				this.ClearGame();
			}
			if (this._mission != null)
			{
				this.ClearMission();
			}
		}

		private void ExecuteSendMessage()
		{
			string text = this.WrittenText;
			if (string.IsNullOrEmpty(text))
			{
				this.WrittenText = string.Empty;
				return;
			}
			if (text.Length > this.MaxMessageLength)
			{
				text = this.WrittenText.Substring(0, this.MaxMessageLength);
			}
			text = Regex.Replace(text.Trim(), "\\s+", " ");
			if (text.StartsWith("/"))
			{
				string[] array = text.Split(new char[] { ' ' });
				ChatChannelType chatChannelType = ChatChannelType.NaN;
				LobbyClient gameClient = NetworkMain.GameClient;
				if (gameClient != null && gameClient.Connected)
				{
					LobbyClient gameClient2 = NetworkMain.GameClient;
					ChatManager.GetChatRoomResult getChatRoomResult = ((gameClient2 != null) ? gameClient2.ChatManager.TryGetChatRoom(array[0]) : null);
					if (getChatRoomResult.Successful)
					{
						chatChannelType = ChatChannelType.Custom;
						this._currentCustomChatChannel = getChatRoomResult.Room;
					}
					else
					{
						string text2 = array[0].ToLower();
						if (!(text2 == "/all") && !(text2 == "/a"))
						{
							if (!(text2 == "/team") && !(text2 == "/t"))
							{
								MPChatLineVM mpchatLineVM = new MPChatLineVM(getChatRoomResult.ErrorMessage.ToString(), Color.White, "Social");
								this.AddChatLine(mpchatLineVM);
							}
							else
							{
								chatChannelType = ChatChannelType.Team;
							}
						}
						else
						{
							chatChannelType = ChatChannelType.All;
						}
					}
				}
				this.ActiveChannelType = chatChannelType;
			}
			else
			{
				switch (this.ActiveChannelType)
				{
				case ChatChannelType.Private:
				case ChatChannelType.Custom:
					this.SendMessageToLobbyChannel(text);
					goto IL_191;
				case ChatChannelType.All:
				case ChatChannelType.Team:
				case ChatChannelType.Party:
					this.CheckSpamAndSendMessage(this.ActiveChannelType, text);
					goto IL_191;
				}
				Debug.FailedAssert("Player in invalid channel", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.ViewModelCollection\\Multiplayer\\MPChatVM.cs", "ExecuteSendMessage", 461);
			}
			IL_191:
			this.WrittenText = "";
		}

		private void CheckSpamAndSendMessage(ChatChannelType channelType, string textToSend)
		{
			if (this._recentlySentMessagesTimes.Count >= 5)
			{
				GameTexts.SetVariable("SECONDS", (15f - (Time.ApplicationTime - this._recentlySentMessagesTimes[0])).ToString("0.0"));
				this.AddChatLine(new MPChatLineVM(new TextObject("{=76VR5o8h}You must wait {SECONDS} seconds before sending another message.", null).ToString(), this.GetChannelColor(ChatChannelType.System), "Default"));
				return;
			}
			this._recentlySentMessagesTimes.Add(Time.ApplicationTime);
			this.SendMessageToChannel(this.ActiveChannelType, textToSend);
		}

		private void HandleAddChatLineRequest(InformationMessage informationMessage)
		{
			string information = informationMessage.Information;
			string text = (string.IsNullOrEmpty(informationMessage.Category) ? "Default" : informationMessage.Category);
			Color color = informationMessage.Color;
			MPChatLineVM mpchatLineVM = new MPChatLineVM(information, color, text);
			this._requestedMessages.Enqueue(mpchatLineVM);
		}

		public void SendMessageToChannel(ChatChannelType channel, string message)
		{
			LobbyClient gameClient = NetworkMain.GameClient;
			if (gameClient != null && gameClient.Connected)
			{
				switch (channel)
				{
				case ChatChannelType.All:
					this._chatBox.SendMessageToAll(message);
					return;
				case ChatChannelType.Team:
					this._chatBox.SendMessageToTeam(message);
					return;
				}
				throw new NotImplementedException();
			}
		}

		public void SendMessageToLobbyChannel(string message)
		{
			LobbyClient gameClient = NetworkMain.GameClient;
			if (gameClient != null && gameClient.Connected)
			{
				NetworkMain.GameClient.SendChannelMessage(this._currentCustomChatChannel.RoomId, message);
			}
		}

		void IChatHandler.ReceiveChatMessage(ChatChannelType channel, string sender, string message)
		{
			TextObject textObject = TextObject.Empty;
			if (channel == ChatChannelType.Private)
			{
				textObject = new TextObject("{=6syoutpV}From {WHISPER_TARGET}", null);
				textObject.SetTextVariable("WHISPER_TARGET", sender);
			}
			this.AddMessage(message, sender, channel, textObject);
		}

		private void AddMessage(string msg, string author, ChatChannelType type, TextObject customChannelName = null)
		{
			Color channelColor = this.GetChannelColor(type);
			string text = ((!TextObject.IsNullOrEmpty(customChannelName)) ? customChannelName.ToString() : type.ToString());
			MPChatLineVM mpchatLineVM = new MPChatLineVM(string.Concat(new string[] { "(", text, ") ", author, ": ", msg }), channelColor, "Social");
			this.AddChatLine(mpchatLineVM);
		}

		private void AddChatLine(MPChatLineVM chatLine)
		{
			if (NativeConfig.DisableGuiMessages || chatLine == null)
			{
				return;
			}
			this._allMessages.Add(chatLine);
			if (this._allMessages.Count > this._maxHistoryCount * 5)
			{
				this._allMessages.RemoveAt(0);
			}
			if (this.IsMessageIncluded(chatLine))
			{
				this.MessageHistory.Add(chatLine);
				if (this.MessageHistory.Count > this._maxHistoryCount)
				{
					this.MessageHistory.RemoveAt(0);
				}
			}
			this.RefreshVisibility();
		}

		public void CheckChatFading(float dt)
		{
			foreach (MPChatLineVM mpchatLineVM in this._allMessages)
			{
				mpchatLineVM.HandleFading(dt);
			}
		}

		[Conditional("DEBUG")]
		private void CheckFadingOutOrder()
		{
			for (int i = 0; i < this._allMessages.Count - 1; i++)
			{
				MPChatLineVM mpchatLineVM = this._allMessages[i];
				MPChatLineVM mpchatLineVM2 = this._allMessages[i + 1];
			}
		}

		private void ChatHistoryFilterToggled()
		{
			this.MessageHistory.Clear();
			int num = 0;
			while (num < this._allMessages.Count && this.MessageHistory.Count < this._maxHistoryCount)
			{
				MPChatLineVM mpchatLineVM = this._allMessages[num];
				if (this.IsMessageIncluded(mpchatLineVM))
				{
					this.MessageHistory.Add(mpchatLineVM);
				}
				num++;
			}
			this.RefreshVisibility();
		}

		private bool IsMessageIncluded(MPChatLineVM chatLine)
		{
			if (chatLine.Category == "Combat")
			{
				return this.IncludeCombatLog;
			}
			return !(chatLine.Category == "Bark") || this.IncludeBark;
		}

		public void SetGetKeyTextFromKeyIDFunc(Func<TextObject> getToggleChatKeyText)
		{
			this._getToggleChatKeyText = getToggleChatKeyText;
		}

		public void SetGetCycleChannelKeyTextFunc(Func<TextObject> getCycleChannelsKeyText)
		{
			this._getCycleChannelsKeyText = getCycleChannelsKeyText;
		}

		public void SetGetSendMessageKeyTextFunc(Func<TextObject> getSendMessageKeyText)
		{
			this._getSendMessageKeyText = getSendMessageKeyText;
		}

		public void SetGetCancelSendingKeyTextFunc(Func<TextObject> getCancelSendingKeyText)
		{
			this._getCancelSendingKeyText = getCancelSendingKeyText;
		}

		private void OnPlayerMessageReceived(NetworkCommunicator player, string message, bool toTeamOnly)
		{
			MissionPeer component = player.GetComponent<MissionPeer>();
			string text = ((component != null) ? component.DisplayedName : null) ?? player.UserName;
			if (component != null && !component.IsAgentAliveForChatting)
			{
				GameTexts.SetVariable("PLAYER_NAME", text);
				text = GameTexts.FindText("str_chat_message_dead_player", null).ToString();
			}
			this.AddMessage(message, text, toTeamOnly ? ChatChannelType.Team : ChatChannelType.All, null);
		}

		private void OnWhisperMessageReceived(string fromUserName, string message)
		{
			this.AddMessage(message, fromUserName, ChatChannelType.Private, null);
		}

		private void OnErrorWhisperMessageReceived(string toUserName)
		{
			TextObject textObject = new TextObject("{=61isYVW0}Player {USER_NAME} is not found", null);
			textObject.SetTextVariable("USER_NAME", toUserName);
			MPChatLineVM mpchatLineVM = new MPChatLineVM(textObject.ToString(), Color.White, "Social");
			this.AddChatLine(mpchatLineVM);
		}

		private void OnWhisperMessageSent(string message, string whisperTarget)
		{
			this.AddMessage(message, whisperTarget, ChatChannelType.Private, null);
		}

		private void OnServerMessage(string message)
		{
			MPChatLineVM mpchatLineVM = new MPChatLineVM(message, Color.White, "Social");
			this.AddChatLine(mpchatLineVM);
		}

		private Color GetChannelColor(ChatChannelType type)
		{
			string text;
			switch (type)
			{
			case ChatChannelType.Private:
				text = "#8C1ABDFF";
				break;
			case ChatChannelType.All:
				text = "#EC943EFF";
				break;
			case ChatChannelType.Team:
				text = "#05C5F7FF";
				break;
			case ChatChannelType.Party:
				text = "#05C587FF";
				break;
			case ChatChannelType.System:
				text = "#FF0000FF";
				break;
			case ChatChannelType.Custom:
				text = "#FF0000FF";
				break;
			default:
				text = "#FFFFFFFF";
				break;
			}
			return Color.ConvertStringToColor(text);
		}

		public bool IsChatAllowedByOptions()
		{
			if (GameNetwork.IsMultiplayer)
			{
				return BannerlordConfig.EnableMultiplayerChatBox;
			}
			return BannerlordConfig.EnableSingleplayerChatBox && (Mission.Current == null || !BannerlordConfig.HideBattleUI);
		}

		public void TypeToChannelAll(bool startTyping = false)
		{
			this.ActiveChannelType = ChatChannelType.All;
			if (startTyping)
			{
				this.StartTyping();
			}
		}

		public void TypeToChannelTeam(bool startTyping = false)
		{
			this.ActiveChannelType = ChatChannelType.Team;
			if (startTyping)
			{
				this.StartTyping();
			}
		}

		public void StartInspectingMessages()
		{
			this.IsInspectingMessages = true;
			this.IsTypingText = false;
			this.WrittenText = "";
		}

		public void StopInspectingMessages()
		{
			this.IsInspectingMessages = false;
			this.IsTypingText = false;
			this.WrittenText = "";
		}

		public void StartTyping()
		{
			this.IsTypingText = true;
			this.IsInspectingMessages = true;
		}

		public void StopTyping(bool resetWrittenText = false)
		{
			this.IsTypingText = false;
			this.IsInspectingMessages = false;
			if (resetWrittenText)
			{
				this.WrittenText = "";
			}
		}

		public void SendCurrentlyTypedMessage()
		{
			this.ExecuteSendMessage();
		}

		private void RefreshVisibility()
		{
			foreach (MPChatLineVM mpchatLineVM in this._allMessages)
			{
				mpchatLineVM.ToggleForceVisible(this.IsTypingText || this.IsInspectingMessages);
			}
		}

		public void ExecuteSaveSizes()
		{
			BannerlordConfig.ChatBoxSizeX = this.ChatBoxSizeX;
			BannerlordConfig.ChatBoxSizeY = this.ChatBoxSizeY;
			BannerlordConfig.Save();
		}

		public void SetMessageHistoryCapacity(int capacity)
		{
			this._maxHistoryCount = capacity;
			MBBindingList<MPChatLineVM> messageHistory = this.MessageHistory;
			if (messageHistory == null)
			{
				return;
			}
			messageHistory.Clear();
		}

		[DataSourceProperty]
		public float ChatBoxSizeX
		{
			get
			{
				return this._chatBoxSizeX;
			}
			set
			{
				if (value != this._chatBoxSizeX)
				{
					this._chatBoxSizeX = value;
					base.OnPropertyChangedWithValue(value, "ChatBoxSizeX");
				}
			}
		}

		[DataSourceProperty]
		public float ChatBoxSizeY
		{
			get
			{
				return this._chatBoxSizeY;
			}
			set
			{
				if (value != this._chatBoxSizeY)
				{
					this._chatBoxSizeY = value;
					base.OnPropertyChangedWithValue(value, "ChatBoxSizeY");
				}
			}
		}

		[DataSourceProperty]
		public int MaxMessageLength
		{
			get
			{
				return this._maxMessageLength;
			}
			set
			{
				if (value != this._maxMessageLength)
				{
					this._maxMessageLength = value;
					base.OnPropertyChangedWithValue(value, "MaxMessageLength");
				}
			}
		}

		[DataSourceProperty]
		public bool IsTypingText
		{
			get
			{
				return this._isTypingText;
			}
			set
			{
				if (value != this._isTypingText)
				{
					this._isTypingText = value;
					base.OnPropertyChangedWithValue(value, "IsTypingText");
					this.RefreshVisibility();
				}
			}
		}

		[DataSourceProperty]
		public bool IsInspectingMessages
		{
			get
			{
				return this._isInspectingMessages;
			}
			set
			{
				if (value != this._isInspectingMessages)
				{
					this._isInspectingMessages = value;
					this.UpdateHideShowText(this._isInspectingMessages);
					this.UpdateShortcutTexts();
					base.OnPropertyChangedWithValue(value, "IsInspectingMessages");
					this.RefreshVisibility();
				}
			}
		}

		[DataSourceProperty]
		public bool IsChatDisabled
		{
			get
			{
				return this._isChatDisabled;
			}
			set
			{
				if (value != this._isChatDisabled)
				{
					this._isChatDisabled = value;
					base.OnPropertyChangedWithValue(value, "IsChatDisabled");
				}
			}
		}

		[DataSourceProperty]
		public bool ShowHideShowHint
		{
			get
			{
				return this._showHideShowHint;
			}
			set
			{
				if (value != this._showHideShowHint)
				{
					this._showHideShowHint = value;
					base.OnPropertyChangedWithValue(value, "ShowHideShowHint");
				}
			}
		}

		[DataSourceProperty]
		public bool IsOptionsAvailable
		{
			get
			{
				return this._isOptionsAvailable;
			}
			set
			{
				if (value != this._isOptionsAvailable)
				{
					this._isOptionsAvailable = value;
					base.OnPropertyChangedWithValue(value, "IsOptionsAvailable");
				}
			}
		}

		[DataSourceProperty]
		public string WrittenText
		{
			get
			{
				return this._writtenText;
			}
			set
			{
				if (value != this._writtenText)
				{
					this._writtenText = value;
					base.OnPropertyChangedWithValue<string>(value, "WrittenText");
				}
			}
		}

		[DataSourceProperty]
		public Color ActiveChannelColor
		{
			get
			{
				return this._activeChannelColor;
			}
			set
			{
				if (value != this._activeChannelColor)
				{
					this._activeChannelColor = value;
					base.OnPropertyChangedWithValue(value, "ActiveChannelColor");
				}
			}
		}

		[DataSourceProperty]
		public string ActiveChannelNameText
		{
			get
			{
				return this._activeChannelNameText;
			}
			set
			{
				if (value != this._activeChannelNameText)
				{
					this._activeChannelNameText = value;
					base.OnPropertyChangedWithValue<string>(value, "ActiveChannelNameText");
				}
			}
		}

		[DataSourceProperty]
		public string HideShowText
		{
			get
			{
				return this._hideShowText;
			}
			set
			{
				if (value != this._hideShowText)
				{
					this._hideShowText = value;
					base.OnPropertyChangedWithValue<string>(value, "HideShowText");
				}
			}
		}

		[DataSourceProperty]
		public string ToggleCombatLogText
		{
			get
			{
				return this._toggleCombatLogText;
			}
			set
			{
				if (value != this._toggleCombatLogText)
				{
					this._toggleCombatLogText = value;
					base.OnPropertyChangedWithValue<string>(value, "ToggleCombatLogText");
				}
			}
		}

		[DataSourceProperty]
		public string ToggleBarkText
		{
			get
			{
				return this._toggleBarkText;
			}
			set
			{
				if (value != this._toggleBarkText)
				{
					this._toggleBarkText = value;
					base.OnPropertyChangedWithValue<string>(value, "ToggleBarkText");
				}
			}
		}

		[DataSourceProperty]
		public string CycleThroughChannelsText
		{
			get
			{
				return this._cycleThroughChannelsText;
			}
			set
			{
				if (value != this._cycleThroughChannelsText)
				{
					this._cycleThroughChannelsText = value;
					base.OnPropertyChangedWithValue<string>(value, "CycleThroughChannelsText");
				}
			}
		}

		[DataSourceProperty]
		public string SendMessageText
		{
			get
			{
				return this._sendMessageText;
			}
			set
			{
				if (value != this._sendMessageText)
				{
					this._sendMessageText = value;
					base.OnPropertyChangedWithValue<string>(value, "SendMessageText");
				}
			}
		}

		[DataSourceProperty]
		public string CancelSendingText
		{
			get
			{
				return this._cancelSendingText;
			}
			set
			{
				if (value != this._cancelSendingText)
				{
					this._cancelSendingText = value;
					base.OnPropertyChangedWithValue<string>(value, "CancelSendingText");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<MPChatLineVM> MessageHistory
		{
			get
			{
				return this._messageHistory;
			}
			set
			{
				if (value != this._messageHistory)
				{
					this._messageHistory = value;
					base.OnPropertyChangedWithValue<MBBindingList<MPChatLineVM>>(value, "MessageHistory");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel CombatLogHint
		{
			get
			{
				return this._combatLogHint;
			}
			set
			{
				if (value != this._combatLogHint)
				{
					this._combatLogHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "CombatLogHint");
				}
			}
		}

		[DataSourceProperty]
		public bool IncludeCombatLog
		{
			get
			{
				return this._includeCombatLog;
			}
			set
			{
				if (value != this._includeCombatLog)
				{
					this._includeCombatLog = value;
					base.OnPropertyChangedWithValue(value, "IncludeCombatLog");
					this.ChatHistoryFilterToggled();
					BannerlordConfig.ReportDamage = value;
				}
			}
		}

		[DataSourceProperty]
		public bool IncludeBark
		{
			get
			{
				return this._includeBark;
			}
			set
			{
				if (value != this._includeBark)
				{
					this._includeBark = value;
					base.OnPropertyChangedWithValue(value, "IncludeBark");
					this.ChatHistoryFilterToggled();
					BannerlordConfig.ReportBark = value;
				}
			}
		}

		private readonly TextObject _hideText = new TextObject("{=ou5KJERr}Press '{KEY}' to hide", null);

		private readonly TextObject _cycleChannelsText = new TextObject("{=Dhb2N5JD}Press '{KEY}' to cycle through channels", null);

		private readonly TextObject _sendMessageTextObject = new TextObject("{=f64QfbTO}'{KEY}' to send", null);

		private readonly TextObject _cancelSendingTextObject = new TextObject("{=U1rHNqOk}'{KEY}' to cancel", null);

		public const string DefaultCategory = "Default";

		public const string CombatCategory = "Combat";

		public const string SocialCategory = "Social";

		public const string BarkCategory = "Bark";

		private int _maxHistoryCount = 100;

		private const int _spamDetectionInterval = 15;

		private const int _maxMessagesAllowedPerInterval = 5;

		private List<float> _recentlySentMessagesTimes;

		private readonly List<MPChatLineVM> _allMessages;

		private readonly Queue<MPChatLineVM> _requestedMessages;

		private Func<TextObject> _getToggleChatKeyText;

		private Func<TextObject> _getCycleChannelsKeyText;

		private Func<TextObject> _getSendMessageKeyText;

		private Func<TextObject> _getCancelSendingKeyText;

		private ChatBox _chatBox;

		private Game _game;

		private Mission _mission;

		private ChatRoomInformationForClient _currentCustomChatChannel;

		private ChatChannelType _activeChannelType = ChatChannelType.NaN;

		private float _chatBoxSizeX;

		private float _chatBoxSizeY;

		private int _maxMessageLength;

		private string _writtenText = "";

		private string _activeChannelNameText;

		private string _hideShowText;

		private string _toggleCombatLogText;

		private string _toggleBarkText;

		private string _cycleThroughChannelsText;

		private string _sendMessageText;

		private string _cancelSendingText;

		private MBBindingList<MPChatLineVM> _messageHistory;

		private bool _includeCombatLog;

		private bool _includeBark;

		private bool _isTypingText;

		private bool _isInspectingMessages;

		private bool _isChatDisabled;

		private bool _showHideShowHint;

		private bool _isOptionsAvailable;

		private HintViewModel _combatLogHint;

		private Color _activeChannelColor;
	}
}
