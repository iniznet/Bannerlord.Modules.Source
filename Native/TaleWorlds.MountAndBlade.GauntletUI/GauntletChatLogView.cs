using System;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.GauntletUI.Multiplayer;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.MountAndBlade.GauntletUI
{
	// Token: 0x02000003 RID: 3
	public class GauntletChatLogView : GlobalLayer
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000008 RID: 8 RVA: 0x000020D6 File Offset: 0x000002D6
		// (set) Token: 0x06000009 RID: 9 RVA: 0x000020DD File Offset: 0x000002DD
		public static GauntletChatLogView Current { get; private set; }

		// Token: 0x0600000A RID: 10 RVA: 0x000020E8 File Offset: 0x000002E8
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

		// Token: 0x0600000B RID: 11 RVA: 0x0000221A File Offset: 0x0000041A
		public static void Initialize()
		{
			if (GauntletChatLogView.Current == null)
			{
				GauntletChatLogView.Current = new GauntletChatLogView();
				ScreenManager.AddGlobalLayer(GauntletChatLogView.Current, false);
			}
		}

		// Token: 0x0600000C RID: 12 RVA: 0x00002238 File Offset: 0x00000438
		private void OnManagedOptionsChanged(ManagedOptions.ManagedOptionsType changedManagedOptionsType)
		{
			bool flag = changedManagedOptionsType == 41 && Mission.Current != null && BannerlordConfig.HideBattleUI;
			bool flag2 = changedManagedOptionsType == 43 && !GameNetwork.IsMultiplayer && !BannerlordConfig.EnableSingleplayerChatBox;
			bool flag3 = changedManagedOptionsType == 44 && GameNetwork.IsMultiplayer && !BannerlordConfig.EnableMultiplayerChatBox;
			if (flag || flag2 || flag3)
			{
				this._dataSource.Clear();
				this.CloseChat();
			}
		}

		// Token: 0x0600000D RID: 13 RVA: 0x000022A0 File Offset: 0x000004A0
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

		// Token: 0x0600000E RID: 14 RVA: 0x000022F8 File Offset: 0x000004F8
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

		// Token: 0x0600000F RID: 15 RVA: 0x00002354 File Offset: 0x00000554
		protected override void OnLateTick(float dt)
		{
			base.OnLateTick(dt);
			MPChatVM dataSource = this._dataSource;
			if (dataSource != null && dataSource.IsChatAllowedByOptions())
			{
				this.HandleInput();
			}
		}

		// Token: 0x06000010 RID: 16 RVA: 0x00002378 File Offset: 0x00000578
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
			else if (ScreenManager.TopScreen is MultiplayerIntermissionScreen)
			{
				MultiplayerIntermissionScreen multiplayerIntermissionScreen = (MultiplayerIntermissionScreen)ScreenManager.TopScreen;
				if (multiplayerIntermissionScreen.Layer != null)
				{
					this._isTeamChatAvailable = false;
					flag = true;
					inputContext = multiplayerIntermissionScreen.Layer.Input;
				}
			}
			else if (ScreenManager.TopScreen is MultiplayerLobbyGauntletScreen)
			{
				MultiplayerLobbyGauntletScreen multiplayerLobbyGauntletScreen = (MultiplayerLobbyGauntletScreen)ScreenManager.TopScreen;
				if (multiplayerLobbyGauntletScreen.LobbyLayer != null)
				{
					flag = true;
					inputContext = multiplayerLobbyGauntletScreen.LobbyLayer.Input;
				}
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
				if (!inputContext.IsCategoryRegistered(HotKeyManager.GetCategory("ChatLogHotKeyCategory")))
				{
					inputContext.RegisterHotKeyCategory(HotKeyManager.GetCategory("ChatLogHotKeyCategory"));
				}
				if (flag2)
				{
					if (inputContext.IsGameKeyReleased(6) && this._canFocusWhileInMission)
					{
						this._dataSource.TypeToChannelAll(true);
						flag3 = true;
					}
					else if (inputContext.IsGameKeyReleased(7) && this._canFocusWhileInMission && this._isTeamChatAvailable)
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
					if ((inputContext.IsGameKeyDownAndReleased(8) || inputContext.IsHotKeyDownAndReleased("FinalizeChatAlternative")) && this._canFocusWhileInMission)
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
				else if ((inputContext.IsGameKeyReleased(8) || inputContext.IsHotKeyReleased("FinalizeChatAlternative")) && this._canFocusWhileInMission)
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

		// Token: 0x06000011 RID: 17 RVA: 0x00002824 File Offset: 0x00000A24
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

		// Token: 0x06000012 RID: 18 RVA: 0x000028A0 File Offset: 0x00000AA0
		public void SetCanFocusWhileInMission(bool canFocusInMission)
		{
			this._canFocusWhileInMission = canFocusInMission;
		}

		// Token: 0x06000013 RID: 19 RVA: 0x000028A9 File Offset: 0x00000AA9
		public void OnSupportedFeaturesReceived(SupportedFeatures supportedFeatures)
		{
			this.SetEnabled(supportedFeatures.SupportsFeatures(32));
		}

		// Token: 0x06000014 RID: 20 RVA: 0x000028B9 File Offset: 0x00000AB9
		public void SetEnabled(bool isEnabled)
		{
			if (this._isEnabled != isEnabled)
			{
				this._isEnabled = isEnabled;
			}
		}

		// Token: 0x06000015 RID: 21 RVA: 0x000028CC File Offset: 0x00000ACC
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

		// Token: 0x06000016 RID: 22 RVA: 0x000029C0 File Offset: 0x00000BC0
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

		// Token: 0x06000017 RID: 23 RVA: 0x00002A15 File Offset: 0x00000C15
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

		// Token: 0x06000018 RID: 24 RVA: 0x00002A47 File Offset: 0x00000C47
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

		// Token: 0x06000019 RID: 25 RVA: 0x00002A79 File Offset: 0x00000C79
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

		// Token: 0x04000006 RID: 6
		private MPChatVM _dataSource;

		// Token: 0x04000007 RID: 7
		private ChatLogMessageManager _chatLogMessageManager;

		// Token: 0x04000008 RID: 8
		private bool _canFocusWhileInMission = true;

		// Token: 0x04000009 RID: 9
		private bool _isTeamChatAvailable;

		// Token: 0x0400000A RID: 10
		private IGauntletMovie _movie;

		// Token: 0x0400000B RID: 11
		private bool _isEnabled = true;

		// Token: 0x0400000C RID: 12
		private const int MaxHistoryCountForSingleplayer = 250;

		// Token: 0x0400000D RID: 13
		private const int MaxHistoryCountForMultiplayer = 100;
	}
}
