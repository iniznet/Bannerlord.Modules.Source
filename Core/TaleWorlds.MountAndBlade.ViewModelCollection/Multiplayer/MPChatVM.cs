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
	// Token: 0x02000039 RID: 57
	public class MPChatVM : ViewModel, IChatHandler
	{
		// Token: 0x17000163 RID: 355
		// (get) Token: 0x060004B5 RID: 1205 RVA: 0x00015216 File Offset: 0x00013416
		// (set) Token: 0x060004B6 RID: 1206 RVA: 0x0001521E File Offset: 0x0001341E
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

		// Token: 0x17000164 RID: 356
		// (get) Token: 0x060004B7 RID: 1207 RVA: 0x0001525C File Offset: 0x0001345C
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

		// Token: 0x060004B8 RID: 1208 RVA: 0x000152DC File Offset: 0x000134DC
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

		// Token: 0x060004B9 RID: 1209 RVA: 0x000153EC File Offset: 0x000135EC
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

		// Token: 0x060004BA RID: 1210 RVA: 0x0001545C File Offset: 0x0001365C
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

		// Token: 0x060004BB RID: 1211 RVA: 0x00015520 File Offset: 0x00013720
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

		// Token: 0x060004BC RID: 1212 RVA: 0x00015543 File Offset: 0x00013743
		public void ToggleIncludeCombatLog()
		{
			this.IncludeCombatLog = !this.IncludeCombatLog;
		}

		// Token: 0x060004BD RID: 1213 RVA: 0x00015554 File Offset: 0x00013754
		public void ExecuteToggleIncludeShouts()
		{
			this.IncludeBark = !this.IncludeBark;
		}

		// Token: 0x060004BE RID: 1214 RVA: 0x00015568 File Offset: 0x00013768
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

		// Token: 0x060004BF RID: 1215 RVA: 0x000155BC File Offset: 0x000137BC
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

		// Token: 0x060004C0 RID: 1216 RVA: 0x000156A0 File Offset: 0x000138A0
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

		// Token: 0x060004C1 RID: 1217 RVA: 0x00015714 File Offset: 0x00013914
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

		// Token: 0x060004C2 RID: 1218 RVA: 0x00015790 File Offset: 0x00013990
		private void OnDisplayMessageReceived(InformationMessage informationMessage)
		{
			if (this.IsChatAllowedByOptions())
			{
				this.HandleAddChatLineRequest(informationMessage);
			}
		}

		// Token: 0x060004C3 RID: 1219 RVA: 0x000157A1 File Offset: 0x000139A1
		private void ClearAllMessages()
		{
			this.Clear();
		}

		// Token: 0x060004C4 RID: 1220 RVA: 0x000157AC File Offset: 0x000139AC
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

		// Token: 0x060004C5 RID: 1221 RVA: 0x00015868 File Offset: 0x00013A68
		private void ClearGame()
		{
			this._game = null;
		}

		// Token: 0x060004C6 RID: 1222 RVA: 0x00015874 File Offset: 0x00013A74
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

		// Token: 0x060004C7 RID: 1223 RVA: 0x00015903 File Offset: 0x00013B03
		private void SetGame()
		{
			this.UpdateHideShowText(this.IsInspectingMessages);
		}

		// Token: 0x060004C8 RID: 1224 RVA: 0x00015914 File Offset: 0x00013B14
		private void SetChatBox()
		{
			this._chatBox.PlayerMessageReceived += this.OnPlayerMessageReceived;
			this._chatBox.WhisperMessageSent += this.OnWhisperMessageSent;
			this._chatBox.WhisperMessageReceived += this.OnWhisperMessageReceived;
			this._chatBox.ErrorWhisperMessageReceived += this.OnErrorWhisperMessageReceived;
			this._chatBox.ServerMessage += this.OnServerMessage;
		}

		// Token: 0x060004C9 RID: 1225 RVA: 0x00015994 File Offset: 0x00013B94
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

		// Token: 0x060004CA RID: 1226 RVA: 0x000159E6 File Offset: 0x00013BE6
		private void ClearMission()
		{
			this._mission = null;
			this.ActiveChannelType = ChatChannelType.NaN;
		}

		// Token: 0x060004CB RID: 1227 RVA: 0x000159F6 File Offset: 0x00013BF6
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

		// Token: 0x060004CC RID: 1228 RVA: 0x00015A1C File Offset: 0x00013C1C
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

		// Token: 0x060004CD RID: 1229 RVA: 0x00015BC8 File Offset: 0x00013DC8
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

		// Token: 0x060004CE RID: 1230 RVA: 0x00015C58 File Offset: 0x00013E58
		private void HandleAddChatLineRequest(InformationMessage informationMessage)
		{
			string information = informationMessage.Information;
			string text = (string.IsNullOrEmpty(informationMessage.Category) ? "Default" : informationMessage.Category);
			Color color = informationMessage.Color;
			MPChatLineVM mpchatLineVM = new MPChatLineVM(information, color, text);
			this._requestedMessages.Enqueue(mpchatLineVM);
		}

		// Token: 0x060004CF RID: 1231 RVA: 0x00015CA4 File Offset: 0x00013EA4
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

		// Token: 0x060004D0 RID: 1232 RVA: 0x00015CF8 File Offset: 0x00013EF8
		public void SendMessageToLobbyChannel(string message)
		{
			LobbyClient gameClient = NetworkMain.GameClient;
			if (gameClient != null && gameClient.Connected)
			{
				NetworkMain.GameClient.SendChannelMessage(this._currentCustomChatChannel.RoomId, message);
			}
		}

		// Token: 0x060004D1 RID: 1233 RVA: 0x00015D24 File Offset: 0x00013F24
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

		// Token: 0x060004D2 RID: 1234 RVA: 0x00015D60 File Offset: 0x00013F60
		private void AddMessage(string msg, string author, ChatChannelType type, TextObject customChannelName = null)
		{
			Color channelColor = this.GetChannelColor(type);
			string text = ((!TextObject.IsNullOrEmpty(customChannelName)) ? customChannelName.ToString() : type.ToString());
			MPChatLineVM mpchatLineVM = new MPChatLineVM(string.Concat(new string[] { "(", text, ") ", author, ": ", msg }), channelColor, "Social");
			this.AddChatLine(mpchatLineVM);
		}

		// Token: 0x060004D3 RID: 1235 RVA: 0x00015DD8 File Offset: 0x00013FD8
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

		// Token: 0x060004D4 RID: 1236 RVA: 0x00015E58 File Offset: 0x00014058
		public void CheckChatFading(float dt)
		{
			foreach (MPChatLineVM mpchatLineVM in this._allMessages)
			{
				mpchatLineVM.HandleFading(dt);
			}
		}

		// Token: 0x060004D5 RID: 1237 RVA: 0x00015EAC File Offset: 0x000140AC
		[Conditional("DEBUG")]
		private void CheckFadingOutOrder()
		{
			for (int i = 0; i < this._allMessages.Count - 1; i++)
			{
				MPChatLineVM mpchatLineVM = this._allMessages[i];
				MPChatLineVM mpchatLineVM2 = this._allMessages[i + 1];
			}
		}

		// Token: 0x060004D6 RID: 1238 RVA: 0x00015EF0 File Offset: 0x000140F0
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

		// Token: 0x060004D7 RID: 1239 RVA: 0x00015F59 File Offset: 0x00014159
		private bool IsMessageIncluded(MPChatLineVM chatLine)
		{
			if (chatLine.Category == "Combat")
			{
				return this.IncludeCombatLog;
			}
			return !(chatLine.Category == "Bark") || this.IncludeBark;
		}

		// Token: 0x060004D8 RID: 1240 RVA: 0x00015F8E File Offset: 0x0001418E
		public void SetGetKeyTextFromKeyIDFunc(Func<TextObject> getToggleChatKeyText)
		{
			this._getToggleChatKeyText = getToggleChatKeyText;
		}

		// Token: 0x060004D9 RID: 1241 RVA: 0x00015F97 File Offset: 0x00014197
		public void SetGetCycleChannelKeyTextFunc(Func<TextObject> getCycleChannelsKeyText)
		{
			this._getCycleChannelsKeyText = getCycleChannelsKeyText;
		}

		// Token: 0x060004DA RID: 1242 RVA: 0x00015FA0 File Offset: 0x000141A0
		public void SetGetSendMessageKeyTextFunc(Func<TextObject> getSendMessageKeyText)
		{
			this._getSendMessageKeyText = getSendMessageKeyText;
		}

		// Token: 0x060004DB RID: 1243 RVA: 0x00015FA9 File Offset: 0x000141A9
		public void SetGetCancelSendingKeyTextFunc(Func<TextObject> getCancelSendingKeyText)
		{
			this._getCancelSendingKeyText = getCancelSendingKeyText;
		}

		// Token: 0x060004DC RID: 1244 RVA: 0x00015FB4 File Offset: 0x000141B4
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

		// Token: 0x060004DD RID: 1245 RVA: 0x0001601C File Offset: 0x0001421C
		private void OnWhisperMessageReceived(string fromUserName, string message)
		{
			this.AddMessage(message, fromUserName, ChatChannelType.Private, null);
		}

		// Token: 0x060004DE RID: 1246 RVA: 0x00016028 File Offset: 0x00014228
		private void OnErrorWhisperMessageReceived(string toUserName)
		{
			TextObject textObject = new TextObject("{=61isYVW0}Player {USER_NAME} is not found", null);
			textObject.SetTextVariable("USER_NAME", toUserName);
			MPChatLineVM mpchatLineVM = new MPChatLineVM(textObject.ToString(), Color.White, "Social");
			this.AddChatLine(mpchatLineVM);
		}

		// Token: 0x060004DF RID: 1247 RVA: 0x00016069 File Offset: 0x00014269
		private void OnWhisperMessageSent(string message, string whisperTarget)
		{
			this.AddMessage(message, whisperTarget, ChatChannelType.Private, null);
		}

		// Token: 0x060004E0 RID: 1248 RVA: 0x00016078 File Offset: 0x00014278
		private void OnServerMessage(string message)
		{
			MPChatLineVM mpchatLineVM = new MPChatLineVM(message, Color.White, "Social");
			this.AddChatLine(mpchatLineVM);
		}

		// Token: 0x060004E1 RID: 1249 RVA: 0x000160A0 File Offset: 0x000142A0
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

		// Token: 0x060004E2 RID: 1250 RVA: 0x00016109 File Offset: 0x00014309
		public bool IsChatAllowedByOptions()
		{
			if (GameNetwork.IsMultiplayer)
			{
				return BannerlordConfig.EnableMultiplayerChatBox;
			}
			return BannerlordConfig.EnableSingleplayerChatBox && (Mission.Current == null || !BannerlordConfig.HideBattleUI);
		}

		// Token: 0x060004E3 RID: 1251 RVA: 0x00016132 File Offset: 0x00014332
		public void TypeToChannelAll(bool startTyping = false)
		{
			this.ActiveChannelType = ChatChannelType.All;
			if (startTyping)
			{
				this.StartTyping();
			}
		}

		// Token: 0x060004E4 RID: 1252 RVA: 0x00016144 File Offset: 0x00014344
		public void TypeToChannelTeam(bool startTyping = false)
		{
			this.ActiveChannelType = ChatChannelType.Team;
			if (startTyping)
			{
				this.StartTyping();
			}
		}

		// Token: 0x060004E5 RID: 1253 RVA: 0x00016156 File Offset: 0x00014356
		public void StartInspectingMessages()
		{
			this.IsInspectingMessages = true;
			this.IsTypingText = false;
			this.WrittenText = "";
		}

		// Token: 0x060004E6 RID: 1254 RVA: 0x00016171 File Offset: 0x00014371
		public void StopInspectingMessages()
		{
			this.IsInspectingMessages = false;
			this.IsTypingText = false;
			this.WrittenText = "";
		}

		// Token: 0x060004E7 RID: 1255 RVA: 0x0001618C File Offset: 0x0001438C
		public void StartTyping()
		{
			this.IsTypingText = true;
			this.IsInspectingMessages = true;
		}

		// Token: 0x060004E8 RID: 1256 RVA: 0x0001619C File Offset: 0x0001439C
		public void StopTyping(bool resetWrittenText = false)
		{
			this.IsTypingText = false;
			this.IsInspectingMessages = false;
			if (resetWrittenText)
			{
				this.WrittenText = "";
			}
		}

		// Token: 0x060004E9 RID: 1257 RVA: 0x000161BA File Offset: 0x000143BA
		public void SendCurrentlyTypedMessage()
		{
			this.ExecuteSendMessage();
		}

		// Token: 0x060004EA RID: 1258 RVA: 0x000161C4 File Offset: 0x000143C4
		private void RefreshVisibility()
		{
			foreach (MPChatLineVM mpchatLineVM in this._allMessages)
			{
				mpchatLineVM.ToggleForceVisible(this.IsTypingText || this.IsInspectingMessages);
			}
		}

		// Token: 0x060004EB RID: 1259 RVA: 0x00016228 File Offset: 0x00014428
		public void ExecuteSaveSizes()
		{
			BannerlordConfig.ChatBoxSizeX = this.ChatBoxSizeX;
			BannerlordConfig.ChatBoxSizeY = this.ChatBoxSizeY;
			BannerlordConfig.Save();
		}

		// Token: 0x060004EC RID: 1260 RVA: 0x00016246 File Offset: 0x00014446
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

		// Token: 0x17000165 RID: 357
		// (get) Token: 0x060004ED RID: 1261 RVA: 0x0001625F File Offset: 0x0001445F
		// (set) Token: 0x060004EE RID: 1262 RVA: 0x00016267 File Offset: 0x00014467
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

		// Token: 0x17000166 RID: 358
		// (get) Token: 0x060004EF RID: 1263 RVA: 0x00016285 File Offset: 0x00014485
		// (set) Token: 0x060004F0 RID: 1264 RVA: 0x0001628D File Offset: 0x0001448D
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

		// Token: 0x17000167 RID: 359
		// (get) Token: 0x060004F1 RID: 1265 RVA: 0x000162AB File Offset: 0x000144AB
		// (set) Token: 0x060004F2 RID: 1266 RVA: 0x000162B3 File Offset: 0x000144B3
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

		// Token: 0x17000168 RID: 360
		// (get) Token: 0x060004F3 RID: 1267 RVA: 0x000162D1 File Offset: 0x000144D1
		// (set) Token: 0x060004F4 RID: 1268 RVA: 0x000162D9 File Offset: 0x000144D9
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

		// Token: 0x17000169 RID: 361
		// (get) Token: 0x060004F5 RID: 1269 RVA: 0x000162FD File Offset: 0x000144FD
		// (set) Token: 0x060004F6 RID: 1270 RVA: 0x00016305 File Offset: 0x00014505
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

		// Token: 0x1700016A RID: 362
		// (get) Token: 0x060004F7 RID: 1271 RVA: 0x0001633B File Offset: 0x0001453B
		// (set) Token: 0x060004F8 RID: 1272 RVA: 0x00016343 File Offset: 0x00014543
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

		// Token: 0x1700016B RID: 363
		// (get) Token: 0x060004F9 RID: 1273 RVA: 0x00016361 File Offset: 0x00014561
		// (set) Token: 0x060004FA RID: 1274 RVA: 0x00016369 File Offset: 0x00014569
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

		// Token: 0x1700016C RID: 364
		// (get) Token: 0x060004FB RID: 1275 RVA: 0x00016387 File Offset: 0x00014587
		// (set) Token: 0x060004FC RID: 1276 RVA: 0x0001638F File Offset: 0x0001458F
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

		// Token: 0x1700016D RID: 365
		// (get) Token: 0x060004FD RID: 1277 RVA: 0x000163AD File Offset: 0x000145AD
		// (set) Token: 0x060004FE RID: 1278 RVA: 0x000163B5 File Offset: 0x000145B5
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

		// Token: 0x1700016E RID: 366
		// (get) Token: 0x060004FF RID: 1279 RVA: 0x000163D8 File Offset: 0x000145D8
		// (set) Token: 0x06000500 RID: 1280 RVA: 0x000163E0 File Offset: 0x000145E0
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

		// Token: 0x1700016F RID: 367
		// (get) Token: 0x06000501 RID: 1281 RVA: 0x00016403 File Offset: 0x00014603
		// (set) Token: 0x06000502 RID: 1282 RVA: 0x0001640B File Offset: 0x0001460B
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

		// Token: 0x17000170 RID: 368
		// (get) Token: 0x06000503 RID: 1283 RVA: 0x0001642E File Offset: 0x0001462E
		// (set) Token: 0x06000504 RID: 1284 RVA: 0x00016436 File Offset: 0x00014636
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

		// Token: 0x17000171 RID: 369
		// (get) Token: 0x06000505 RID: 1285 RVA: 0x00016459 File Offset: 0x00014659
		// (set) Token: 0x06000506 RID: 1286 RVA: 0x00016461 File Offset: 0x00014661
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

		// Token: 0x17000172 RID: 370
		// (get) Token: 0x06000507 RID: 1287 RVA: 0x00016484 File Offset: 0x00014684
		// (set) Token: 0x06000508 RID: 1288 RVA: 0x0001648C File Offset: 0x0001468C
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

		// Token: 0x17000173 RID: 371
		// (get) Token: 0x06000509 RID: 1289 RVA: 0x000164AF File Offset: 0x000146AF
		// (set) Token: 0x0600050A RID: 1290 RVA: 0x000164B7 File Offset: 0x000146B7
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

		// Token: 0x17000174 RID: 372
		// (get) Token: 0x0600050B RID: 1291 RVA: 0x000164DA File Offset: 0x000146DA
		// (set) Token: 0x0600050C RID: 1292 RVA: 0x000164E2 File Offset: 0x000146E2
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

		// Token: 0x17000175 RID: 373
		// (get) Token: 0x0600050D RID: 1293 RVA: 0x00016505 File Offset: 0x00014705
		// (set) Token: 0x0600050E RID: 1294 RVA: 0x0001650D File Offset: 0x0001470D
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

		// Token: 0x17000176 RID: 374
		// (get) Token: 0x0600050F RID: 1295 RVA: 0x00016530 File Offset: 0x00014730
		// (set) Token: 0x06000510 RID: 1296 RVA: 0x00016538 File Offset: 0x00014738
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

		// Token: 0x17000177 RID: 375
		// (get) Token: 0x06000511 RID: 1297 RVA: 0x00016556 File Offset: 0x00014756
		// (set) Token: 0x06000512 RID: 1298 RVA: 0x0001655E File Offset: 0x0001475E
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

		// Token: 0x17000178 RID: 376
		// (get) Token: 0x06000513 RID: 1299 RVA: 0x0001657C File Offset: 0x0001477C
		// (set) Token: 0x06000514 RID: 1300 RVA: 0x00016584 File Offset: 0x00014784
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

		// Token: 0x17000179 RID: 377
		// (get) Token: 0x06000515 RID: 1301 RVA: 0x000165AE File Offset: 0x000147AE
		// (set) Token: 0x06000516 RID: 1302 RVA: 0x000165B6 File Offset: 0x000147B6
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

		// Token: 0x04000266 RID: 614
		private readonly TextObject _hideText = new TextObject("{=ou5KJERr}Press '{KEY}' to hide", null);

		// Token: 0x04000267 RID: 615
		private readonly TextObject _cycleChannelsText = new TextObject("{=Dhb2N5JD}Press '{KEY}' to cycle through channels", null);

		// Token: 0x04000268 RID: 616
		private readonly TextObject _sendMessageTextObject = new TextObject("{=f64QfbTO}'{KEY}' to send", null);

		// Token: 0x04000269 RID: 617
		private readonly TextObject _cancelSendingTextObject = new TextObject("{=U1rHNqOk}'{KEY}' to cancel", null);

		// Token: 0x0400026A RID: 618
		public const string DefaultCategory = "Default";

		// Token: 0x0400026B RID: 619
		public const string CombatCategory = "Combat";

		// Token: 0x0400026C RID: 620
		public const string SocialCategory = "Social";

		// Token: 0x0400026D RID: 621
		public const string BarkCategory = "Bark";

		// Token: 0x0400026E RID: 622
		private int _maxHistoryCount = 100;

		// Token: 0x0400026F RID: 623
		private const int _spamDetectionInterval = 15;

		// Token: 0x04000270 RID: 624
		private const int _maxMessagesAllowedPerInterval = 5;

		// Token: 0x04000271 RID: 625
		private List<float> _recentlySentMessagesTimes;

		// Token: 0x04000272 RID: 626
		private readonly List<MPChatLineVM> _allMessages;

		// Token: 0x04000273 RID: 627
		private readonly Queue<MPChatLineVM> _requestedMessages;

		// Token: 0x04000274 RID: 628
		private Func<TextObject> _getToggleChatKeyText;

		// Token: 0x04000275 RID: 629
		private Func<TextObject> _getCycleChannelsKeyText;

		// Token: 0x04000276 RID: 630
		private Func<TextObject> _getSendMessageKeyText;

		// Token: 0x04000277 RID: 631
		private Func<TextObject> _getCancelSendingKeyText;

		// Token: 0x04000278 RID: 632
		private ChatBox _chatBox;

		// Token: 0x04000279 RID: 633
		private Game _game;

		// Token: 0x0400027A RID: 634
		private Mission _mission;

		// Token: 0x0400027B RID: 635
		private ChatRoomInformationForClient _currentCustomChatChannel;

		// Token: 0x0400027C RID: 636
		private ChatChannelType _activeChannelType = ChatChannelType.NaN;

		// Token: 0x0400027D RID: 637
		private float _chatBoxSizeX;

		// Token: 0x0400027E RID: 638
		private float _chatBoxSizeY;

		// Token: 0x0400027F RID: 639
		private int _maxMessageLength;

		// Token: 0x04000280 RID: 640
		private string _writtenText = "";

		// Token: 0x04000281 RID: 641
		private string _activeChannelNameText;

		// Token: 0x04000282 RID: 642
		private string _hideShowText;

		// Token: 0x04000283 RID: 643
		private string _toggleCombatLogText;

		// Token: 0x04000284 RID: 644
		private string _toggleBarkText;

		// Token: 0x04000285 RID: 645
		private string _cycleThroughChannelsText;

		// Token: 0x04000286 RID: 646
		private string _sendMessageText;

		// Token: 0x04000287 RID: 647
		private string _cancelSendingText;

		// Token: 0x04000288 RID: 648
		private MBBindingList<MPChatLineVM> _messageHistory;

		// Token: 0x04000289 RID: 649
		private bool _includeCombatLog;

		// Token: 0x0400028A RID: 650
		private bool _includeBark;

		// Token: 0x0400028B RID: 651
		private bool _isTypingText;

		// Token: 0x0400028C RID: 652
		private bool _isInspectingMessages;

		// Token: 0x0400028D RID: 653
		private bool _isChatDisabled;

		// Token: 0x0400028E RID: 654
		private bool _showHideShowHint;

		// Token: 0x0400028F RID: 655
		private bool _isOptionsAvailable;

		// Token: 0x04000290 RID: 656
		private HintViewModel _combatLogHint;

		// Token: 0x04000291 RID: 657
		private Color _activeChannelColor;
	}
}
