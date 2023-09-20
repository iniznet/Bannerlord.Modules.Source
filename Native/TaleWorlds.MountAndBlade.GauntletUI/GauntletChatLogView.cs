using System;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.MountAndBlade.GauntletUI
{
	public class GauntletChatLogView : GlobalLayer
	{
		public static GauntletChatLogView Current { get; private set; }

		public GauntletChatLogView()
		{
			this._dataSource = new MPChatVM();
			this._dataSource.SetGetKeyTextFromKeyIDFunc(new Func<TextObject>(this.GetToggleChatKeyText));
			this._dataSource.SetGetCycleChannelKeyTextFunc(new Func<TextObject>(this.GetCycleChannelsKeyText));
			this._dataSource.SetGetSendMessageKeyTextFunc(new Func<TextObject>(this.GetSendMessageKeyText));
			this._dataSource.SetGetCancelSendingKeyTextFunc(new Func<TextObject>(this.GetCancelSendingKeyText));
			GauntletLayer gauntletLayer = new GauntletLayer(300, "GauntletLayer", false);
			this._movie = gauntletLayer.LoadMovie("SPChatLog", this._dataSource);
			gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("Generic"));
			gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
			gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("ChatLogHotKeyCategory"));
			base.Layer = gauntletLayer;
			this._chatLogMessageManager = new ChatLogMessageManager(this._dataSource);
			MessageManager.SetMessageManager(this._chatLogMessageManager);
			ManagedOptions.OnManagedOptionChanged = (ManagedOptions.OnManagedOptionChangedDelegate)Delegate.Combine(ManagedOptions.OnManagedOptionChanged, new ManagedOptions.OnManagedOptionChangedDelegate(this.OnManagedOptionsChanged));
		}

		public static void Initialize()
		{
			if (GauntletChatLogView.Current == null)
			{
				GauntletChatLogView.Current = new GauntletChatLogView();
				ScreenManager.AddGlobalLayer(GauntletChatLogView.Current, false);
			}
		}

		private void OnManagedOptionsChanged(ManagedOptions.ManagedOptionsType changedManagedOptionsType)
		{
			bool flag = changedManagedOptionsType == 43 && Mission.Current != null && BannerlordConfig.HideBattleUI;
			bool flag2 = changedManagedOptionsType == 45 && !GameNetwork.IsMultiplayer && !BannerlordConfig.EnableSingleplayerChatBox;
			bool flag3 = changedManagedOptionsType == 46 && GameNetwork.IsMultiplayer && !BannerlordConfig.EnableMultiplayerChatBox;
			if (flag || flag2 || flag3)
			{
				this._dataSource.Clear();
				this.CloseChat();
			}
		}

		private void CloseChat()
		{
			if (this._dataSource.IsInspectingMessages)
			{
				this._dataSource.StopInspectingMessages();
				ScreenManager.TryLoseFocus(base.Layer);
				return;
			}
			if (this._dataSource.IsTypingText)
			{
				this._dataSource.StopTyping(true);
				ScreenManager.TryLoseFocus(base.Layer);
			}
		}

		protected override void OnTick(float dt)
		{
			if (!this._isEnabled)
			{
				this.CloseChat();
				return;
			}
			base.OnTick(dt);
			if (this._dataSource.IsChatAllowedByOptions())
			{
				this._chatLogMessageManager.Update();
			}
			this._dataSource.UpdateObjects(Game.Current, Mission.Current);
			this._dataSource.Tick(dt);
		}

		protected override void OnLateTick(float dt)
		{
			base.OnLateTick(dt);
			MPChatVM dataSource = this._dataSource;
			if (dataSource != null && dataSource.IsChatAllowedByOptions())
			{
				this.HandleInput();
			}
		}

		private void HandleInput()
		{
			bool flag = false;
			bool flag2 = true;
			this._isTeamChatAvailable = true;
			InputContext inputContext = null;
			if (ScreenManager.TopScreen is MissionScreen)
			{
				MissionScreen missionScreen = (MissionScreen)ScreenManager.TopScreen;
				if (missionScreen.SceneLayer != null)
				{
					flag = true;
					inputContext = missionScreen.SceneLayer.Input;
				}
			}
			else if (ScreenManager.TopScreen is IGauntletChatLogHandlerScreen)
			{
				((IGauntletChatLogHandlerScreen)ScreenManager.TopScreen).TryUpdateChatLogLayerParameters(ref this._isTeamChatAvailable, ref flag, ref inputContext);
			}
			else if (ScreenManager.TopScreen is GauntletInitialScreen)
			{
				flag = false;
			}
			else
			{
				ScreenLayer screenLayer = null;
				ScreenBase topScreen = ScreenManager.TopScreen;
				if (((topScreen != null) ? topScreen.Layers : null) != null)
				{
					for (int i = 0; i < ScreenManager.TopScreen.Layers.Count; i++)
					{
						if (ScreenManager.TopScreen.Layers[i]._categoryId == "SceneLayer")
						{
							screenLayer = ScreenManager.TopScreen.Layers[i];
							break;
						}
					}
				}
				if (screenLayer != null)
				{
					flag = true;
					flag2 = true;
					inputContext = screenLayer.Input;
				}
				this._dataSource.ShowHideShowHint = screenLayer != null;
			}
			GauntletLayer gauntletLayer;
			if ((gauntletLayer = ScreenManager.FocusedLayer as GauntletLayer) != null && gauntletLayer != base.Layer && gauntletLayer._gauntletUIContext.EventManager.FocusedWidget is EditableTextWidget)
			{
				flag = false;
			}
			bool flag3 = false;
			bool flag4 = false;
			if (flag)
			{
				if (inputContext != null && !inputContext.IsCategoryRegistered(HotKeyManager.GetCategory("ChatLogHotKeyCategory")))
				{
					inputContext.RegisterHotKeyCategory(HotKeyManager.GetCategory("ChatLogHotKeyCategory"));
				}
				if (flag2)
				{
					if (inputContext != null && inputContext.IsGameKeyReleased(6) && this._canFocusWhileInMission)
					{
						this._dataSource.TypeToChannelAll(true);
						flag3 = true;
					}
					else if (inputContext != null && inputContext.IsGameKeyReleased(7) && this._canFocusWhileInMission && this._isTeamChatAvailable)
					{
						this._dataSource.TypeToChannelTeam(true);
						flag3 = true;
					}
					if (base.Layer.Input.IsHotKeyReleased("ToggleEscapeMenu") || base.Layer.Input.IsHotKeyReleased("Exit"))
					{
						bool isGamepadActive = Input.IsGamepadActive;
						this._dataSource.StopTyping(isGamepadActive);
						flag4 = true;
					}
					else if (base.Layer.Input.IsGameKeyReleased(8) || base.Layer.Input.IsHotKeyReleased("FinalizeChatAlternative") || base.Layer.Input.IsHotKeyReleased("SendMessage"))
					{
						if ((Input.IsGamepadActive && base.Layer.Input.IsHotKeyReleased("SendMessage")) || !Input.IsGamepadActive)
						{
							this._dataSource.SendCurrentlyTypedMessage();
						}
						this._dataSource.StopTyping(false);
						flag4 = true;
					}
					if (inputContext != null && (inputContext.IsGameKeyDownAndReleased(8) || inputContext.IsHotKeyDownAndReleased("FinalizeChatAlternative")) && this._canFocusWhileInMission)
					{
						if (this._dataSource.ActiveChannelType == -1)
						{
							this._dataSource.TypeToChannelAll(true);
						}
						else
						{
							this._dataSource.StartTyping();
						}
						flag3 = true;
					}
					if (base.Layer.Input.IsHotKeyReleased("CycleChatTypes"))
					{
						if (this._dataSource.ActiveChannelType == 2)
						{
							this._dataSource.TypeToChannelAll(false);
						}
						else if (this._dataSource.ActiveChannelType == 1 && this._isTeamChatAvailable)
						{
							this._dataSource.TypeToChannelTeam(false);
						}
					}
				}
				else if (inputContext != null && (inputContext.IsGameKeyReleased(8) || inputContext.IsHotKeyReleased("FinalizeChatAlternative")) && this._canFocusWhileInMission)
				{
					if (!this._dataSource.IsInspectingMessages)
					{
						this._dataSource.StartInspectingMessages();
						flag3 = true;
					}
					else
					{
						this._dataSource.StopInspectingMessages();
						flag4 = true;
					}
				}
			}
			else
			{
				bool flag5 = this._dataSource.IsTypingText || this._dataSource.IsInspectingMessages;
				if (this._dataSource.IsTypingText)
				{
					this._dataSource.StopTyping(false);
				}
				else if (this._dataSource.IsInspectingMessages)
				{
					this._dataSource.StopInspectingMessages();
				}
				if (flag5)
				{
					base.Layer.InputRestrictions.ResetInputRestrictions();
					flag4 = true;
				}
			}
			if (flag3)
			{
				this.UpdateFocusLayer();
				ScreenManager.TrySetFocus(base.Layer);
			}
			else if (flag4)
			{
				this.UpdateFocusLayer();
				ScreenManager.TryLoseFocus(base.Layer);
			}
			if (flag3 || flag4)
			{
				MissionScreen missionScreen2 = ScreenManager.TopScreen as MissionScreen;
				if (missionScreen2 != null && missionScreen2.SceneLayer != null)
				{
					missionScreen2.Mission.GetMissionBehavior<MissionMainAgentController>().IsChatOpen = flag3 && !flag4;
				}
			}
		}

		private void UpdateFocusLayer()
		{
			if (this._dataSource.IsTypingText || this._dataSource.IsInspectingMessages)
			{
				if (this._dataSource.IsTypingText && !base.Layer.IsFocusLayer)
				{
					base.Layer.IsFocusLayer = true;
				}
				base.Layer.InputRestrictions.SetInputRestrictions(true, 7);
				return;
			}
			base.Layer.IsFocusLayer = false;
			base.Layer.InputRestrictions.ResetInputRestrictions();
		}

		public void SetCanFocusWhileInMission(bool canFocusInMission)
		{
			this._canFocusWhileInMission = canFocusInMission;
		}

		public void OnSupportedFeaturesReceived(SupportedFeatures supportedFeatures)
		{
			this.SetEnabled(supportedFeatures.SupportsFeatures(32));
		}

		public void SetEnabled(bool isEnabled)
		{
			if (this._isEnabled != isEnabled)
			{
				this._isEnabled = isEnabled;
			}
		}

		public void LoadMovie(bool forMultiplayer)
		{
			if (this._movie != null)
			{
				GauntletLayer gauntletLayer = base.Layer as GauntletLayer;
				if (gauntletLayer != null)
				{
					gauntletLayer.ReleaseMovie(this._movie);
				}
			}
			if (forMultiplayer)
			{
				Game game = Game.Current;
				if (game != null)
				{
					game.GetGameHandler<ChatBox>().InitializeForMultiplayer();
				}
				GauntletLayer gauntletLayer2 = base.Layer as GauntletLayer;
				this._movie = ((gauntletLayer2 != null) ? gauntletLayer2.LoadMovie("MPChatLog", this._dataSource) : null);
				this._dataSource.SetMessageHistoryCapacity(100);
				return;
			}
			this.SetEnabled(true);
			Game game2 = Game.Current;
			if (game2 != null)
			{
				game2.GetGameHandler<ChatBox>().InitializeForSinglePlayer();
			}
			GauntletLayer gauntletLayer3 = base.Layer as GauntletLayer;
			this._movie = ((gauntletLayer3 != null) ? gauntletLayer3.LoadMovie("SPChatLog", this._dataSource) : null);
			this._dataSource.ChatBoxSizeX = BannerlordConfig.ChatBoxSizeX;
			this._dataSource.ChatBoxSizeY = BannerlordConfig.ChatBoxSizeY;
			this._dataSource.SetMessageHistoryCapacity(250);
		}

		private TextObject GetToggleChatKeyText()
		{
			if (Input.IsGamepadActive)
			{
				Game game = Game.Current;
				if (game == null)
				{
					return null;
				}
				GameTextManager gameTextManager = game.GameTextManager;
				if (gameTextManager == null)
				{
					return null;
				}
				return GameKeyTextExtensions.GetHotKeyGameTextFromKeyID(gameTextManager, "controllerloption");
			}
			else
			{
				Game game2 = Game.Current;
				if (game2 == null)
				{
					return null;
				}
				GameTextManager gameTextManager2 = game2.GameTextManager;
				if (gameTextManager2 == null)
				{
					return null;
				}
				return GameKeyTextExtensions.GetHotKeyGameTextFromKeyID(gameTextManager2, "enter");
			}
		}

		private TextObject GetCycleChannelsKeyText()
		{
			Game game = Game.Current;
			TextObject textObject;
			if (game == null)
			{
				textObject = null;
			}
			else
			{
				GameTextManager gameTextManager = game.GameTextManager;
				textObject = ((gameTextManager != null) ? GameKeyTextExtensions.GetHotKeyGameText(gameTextManager, "ChatLogHotKeyCategory", "CycleChatTypes") : null);
			}
			return textObject ?? TextObject.Empty;
		}

		private TextObject GetSendMessageKeyText()
		{
			Game game = Game.Current;
			TextObject textObject;
			if (game == null)
			{
				textObject = null;
			}
			else
			{
				GameTextManager gameTextManager = game.GameTextManager;
				textObject = ((gameTextManager != null) ? GameKeyTextExtensions.GetHotKeyGameText(gameTextManager, "ChatLogHotKeyCategory", "SendMessage") : null);
			}
			return textObject ?? TextObject.Empty;
		}

		private TextObject GetCancelSendingKeyText()
		{
			Game game = Game.Current;
			TextObject textObject;
			if (game == null)
			{
				textObject = null;
			}
			else
			{
				GameTextManager gameTextManager = game.GameTextManager;
				textObject = ((gameTextManager != null) ? GameKeyTextExtensions.GetHotKeyGameText(gameTextManager, "GenericPanelGameKeyCategory", "Exit") : null);
			}
			return textObject ?? TextObject.Empty;
		}

		private MPChatVM _dataSource;

		private ChatLogMessageManager _chatLogMessageManager;

		private bool _canFocusWhileInMission = true;

		private bool _isTeamChatAvailable;

		private IGauntletMovie _movie;

		private bool _isEnabled = true;

		private const int MaxHistoryCountForSingleplayer = 250;

		private const int MaxHistoryCountForMultiplayer = 100;
	}
}
