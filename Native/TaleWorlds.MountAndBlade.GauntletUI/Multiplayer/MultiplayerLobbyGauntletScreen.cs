using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Options;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.Diamond.Ranked;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions;
using TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions.AuxiliaryKeys;
using TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions.GameKeys;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.Lobby.CustomGame;
using TaleWorlds.PlayerServices;
using TaleWorlds.ScreenSystem;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Multiplayer
{
	// Token: 0x02000020 RID: 32
	[GameStateScreen(typeof(LobbyState))]
	public class MultiplayerLobbyGauntletScreen : ScreenBase, IGameStateListener, ILobbyStateHandler
	{
		// Token: 0x17000041 RID: 65
		// (get) Token: 0x06000128 RID: 296 RVA: 0x00007105 File Offset: 0x00005305
		public MPLobbyVM.LobbyPage CurrentPage
		{
			get
			{
				if (this._lobbyDataSource != null)
				{
					return this._lobbyDataSource.CurrentPage;
				}
				return 0;
			}
		}

		// Token: 0x06000129 RID: 297 RVA: 0x0000711C File Offset: 0x0000531C
		public MultiplayerLobbyGauntletScreen(LobbyState lobbyState)
		{
			this._feedbackInquiries = new List<KeyValuePair<string, InquiryData>>();
			this._lobbyState = lobbyState;
			this._lobbyState.Handler = this;
			GauntletGameNotification gauntletGameNotification = GauntletGameNotification.Current;
			if (gauntletGameNotification != null)
			{
				gauntletGameNotification.LoadMovie(true);
			}
			GauntletChatLogView gauntletChatLogView = GauntletChatLogView.Current;
			if (gauntletChatLogView != null)
			{
				gauntletChatLogView.LoadMovie(true);
			}
			GauntletChatLogView gauntletChatLogView2 = GauntletChatLogView.Current;
			if (gauntletChatLogView2 != null)
			{
				gauntletChatLogView2.SetEnabled(false);
			}
			MultiplayerAdminInformationScreen.OnInitialize();
			MultiplayerReportPlayerScreen.OnInitialize();
		}

		// Token: 0x0600012A RID: 298 RVA: 0x0000718A File Offset: 0x0000538A
		private void OnManagedOptionChanged(ManagedOptions.ManagedOptionsType changedManagedOptionsType)
		{
			if (changedManagedOptionsType == 29)
			{
				MPLobbyVM lobbyDataSource = this._lobbyDataSource;
				if (lobbyDataSource != null)
				{
					lobbyDataSource.OnEnableGenericAvatarsChanged();
				}
			}
			if (changedManagedOptionsType == 30)
			{
				MPLobbyVM lobbyDataSource2 = this._lobbyDataSource;
				if (lobbyDataSource2 == null)
				{
					return;
				}
				lobbyDataSource2.OnEnableGenericNamesChanged();
			}
		}

		// Token: 0x0600012B RID: 299 RVA: 0x000071B8 File Offset: 0x000053B8
		private void CreateView()
		{
			if (!(GameStateManager.Current.ActiveState is MissionState))
			{
				LoadingWindow.DisableGlobalLoadingWindow();
			}
			SpriteData spriteData = UIResourceManager.SpriteData;
			TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
			ResourceDepot uiresourceDepot = UIResourceManager.UIResourceDepot;
			this._mplobbyCategory = spriteData.SpriteCategories["ui_mplobby"];
			this._mplobbyCategory.Load(resourceContext, uiresourceDepot);
			this._bannerIconsCategory = spriteData.SpriteCategories["ui_bannericons"];
			this._bannerIconsCategory.Load(resourceContext, uiresourceDepot);
			this._badgesCategory = spriteData.SpriteCategories["ui_mpbadges"];
			this._badgesCategory.Load(resourceContext, uiresourceDepot);
			this._lobbyDataSource = new MPLobbyVM(this._lobbyState, new Action<BasicCharacterObject>(this.OnOpenFacegen), new Action(this.OnForceCloseFacegen), new Action<KeyOptionVM>(this.OnKeybindRequest), new Func<string>(this.GetContinueKeyText), new Action<bool>(this.SetNavigationRestriction));
			this._lobbyDataSource.CreateInputKeyVisuals(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Exit"), HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Confirm"));
			this._lobbyLayer = new GauntletLayer(10, "GauntletLayer", true);
			this._lobbyLayer.LoadMovie("Lobby", this._lobbyDataSource);
			this._lobbyLayer.InputRestrictions.SetInputRestrictions(true, 7);
			this._lobbyLayer.IsFocusLayer = true;
			base.AddLayer(this._lobbyLayer);
			ScreenManager.TrySetFocus(this._lobbyLayer);
			this._lobbyLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
			this._lobbyLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("MultiplayerHotkeyCategory"));
			MPLobbyVM lobbyDataSource = this._lobbyDataSource;
			if (lobbyDataSource != null)
			{
				lobbyDataSource.BadgeSelectionPopup.RefreshKeyBindings(HotKeyManager.GetCategory("MultiplayerHotkeyCategory").GetHotKey("InspectBadgeProgression"));
			}
			MPLobbyVM lobbyDataSource2 = this._lobbyDataSource;
			if (lobbyDataSource2 != null)
			{
				lobbyDataSource2.Armory.Cosmetics.RefreshKeyBindings(HotKeyManager.GetCategory("MultiplayerHotkeyCategory").GetHotKey("PerformActionOnCosmeticItem"), HotKeyManager.GetCategory("MultiplayerHotkeyCategory").GetHotKey("PreviewCosmeticItem"));
			}
			MPLobbyVM lobbyDataSource3 = this._lobbyDataSource;
			if (lobbyDataSource3 != null)
			{
				lobbyDataSource3.Friends.SetToggleFriendListKey(HotKeyManager.GetCategory("MultiplayerHotkeyCategory").RegisteredHotKeys.FirstOrDefault((HotKey g) => ((g != null) ? g.Id : null) == "ToggleFriendsList"));
			}
			this._lobbyDataSource.Matchmaking.CustomServer.SortController.SetSortOption(this._cachedCustomServerSortOption);
			this._lobbyDataSource.Matchmaking.PremadeMatches.SortController.SetSortOption(this._cachedPremadeGameSortOption);
			this._lobbyDataSource.Options.SetPreviousTabInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("TakeAll"));
			this._lobbyDataSource.Options.SetNextTabInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("GiveAll"));
			this._lobbyDataSource.Matchmaking.CustomServer.SetRefreshInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Reset"));
			if (NativeOptions.GetConfig(65) < 2f)
			{
				this._brightnessOptionDataSource = new BrightnessOptionVM(new Action<bool>(this.OnCloseBrightness))
				{
					Visible = true
				};
				this._gauntletBrightnessLayer = new GauntletLayer(11, "GauntletLayer", false);
				this._gauntletBrightnessLayer.InputRestrictions.SetInputRestrictions(true, 3);
				this._brightnessOptionMovie = this._gauntletBrightnessLayer.LoadMovie("BrightnessOption", this._brightnessOptionDataSource);
				base.AddLayer(this._gauntletBrightnessLayer);
			}
			this._spriteCategory = spriteData.SpriteCategories["ui_options"];
			this._spriteCategory.Load(resourceContext, uiresourceDepot);
		}

		// Token: 0x0600012C RID: 300 RVA: 0x0000756C File Offset: 0x0000576C
		private void OnCloseBrightness(bool isConfirm)
		{
			this._gauntletBrightnessLayer.ReleaseMovie(this._brightnessOptionMovie);
			base.RemoveLayer(this._gauntletBrightnessLayer);
			this._brightnessOptionDataSource = null;
			NativeOptions.SaveConfig();
		}

		// Token: 0x0600012D RID: 301 RVA: 0x00007598 File Offset: 0x00005798
		protected override void OnInitialize()
		{
			base.OnInitialize();
			LoadingWindow.DisableGlobalLoadingWindow();
			this._keybindingPopup = new KeybindingPopup(new Action<Key>(this.SetHotKey), this);
			ManagedOptions.OnManagedOptionChanged = (ManagedOptions.OnManagedOptionChangedDelegate)Delegate.Combine(ManagedOptions.OnManagedOptionChanged, new ManagedOptions.OnManagedOptionChangedDelegate(this.OnManagedOptionChanged));
		}

		// Token: 0x0600012E RID: 302 RVA: 0x000075E8 File Offset: 0x000057E8
		protected override void OnFinalize()
		{
			if (this._lobbyDataSource != null)
			{
				this._lobbyDataSource.OnFinalize();
				this._lobbyDataSource = null;
			}
			SpriteCategory mplobbyCategory = this._mplobbyCategory;
			if (mplobbyCategory != null)
			{
				mplobbyCategory.Unload();
			}
			this._spriteCategory.Unload();
			SpriteCategory badgesCategory = this._badgesCategory;
			if (badgesCategory != null)
			{
				badgesCategory.Unload();
			}
			MultiplayerReportPlayerScreen.OnFinalize();
			MultiplayerAdminInformationScreen.OnRemove();
			ManagedOptions.OnManagedOptionChanged = (ManagedOptions.OnManagedOptionChangedDelegate)Delegate.Remove(ManagedOptions.OnManagedOptionChanged, new ManagedOptions.OnManagedOptionChangedDelegate(this.OnManagedOptionChanged));
			this._lobbyState.Handler = null;
			this._lobbyState = null;
			base.OnFinalize();
		}

		// Token: 0x0600012F RID: 303 RVA: 0x0000767F File Offset: 0x0000587F
		protected override void OnActivate()
		{
			if (this._lobbyDataSource != null && this._isFacegenOpen)
			{
				this.OnFacegenClosed(true);
			}
			LoadingWindow.DisableGlobalLoadingWindow();
		}

		// Token: 0x06000130 RID: 304 RVA: 0x0000769D File Offset: 0x0000589D
		protected override void OnDeactivate()
		{
			base.OnDeactivate();
		}

		// Token: 0x06000131 RID: 305 RVA: 0x000076A5 File Offset: 0x000058A5
		private void OnOpenFacegen(BasicCharacterObject character)
		{
			this._isFacegenOpen = true;
			this._playerCharacter = character;
			LoadingWindow.EnableGlobalLoadingWindow();
			ScreenManager.PushScreen(ViewCreator.CreateMBFaceGeneratorScreen(character, true, null));
		}

		// Token: 0x06000132 RID: 306 RVA: 0x000076C7 File Offset: 0x000058C7
		private void OnForceCloseFacegen()
		{
			if (this._isFacegenOpen)
			{
				this.OnFacegenClosed(false);
				ScreenManager.PopScreen();
			}
		}

		// Token: 0x06000133 RID: 307 RVA: 0x000076E0 File Offset: 0x000058E0
		private void OnFacegenClosed(bool updateCharacter)
		{
			if (updateCharacter)
			{
				NetworkMain.GameClient.UpdateCharacter(this._playerCharacter.GetBodyPropertiesMin(false), this._playerCharacter.IsFemale);
			}
			ScreenManager.TrySetFocus(this._lobbyLayer);
			this._lobbyDataSource.RefreshPlayerData(this._lobbyState.LobbyClient.PlayerData);
			this._isFacegenOpen = false;
			this._playerCharacter = null;
		}

		// Token: 0x06000134 RID: 308 RVA: 0x00007748 File Offset: 0x00005948
		private string GetContinueKeyText()
		{
			if (Input.IsGamepadActive)
			{
				GameTexts.SetVariable("CONSOLE_KEY_NAME", GameKeyTextExtensions.GetHotKeyGameText(Game.Current.GameTextManager, "GenericPanelGameKeyCategory", "Exit"));
				return GameTexts.FindText("str_click_to_exit_console", null).ToString();
			}
			return GameTexts.FindText("str_click_to_exit", null).ToString();
		}

		// Token: 0x06000135 RID: 309 RVA: 0x000077A0 File Offset: 0x000059A0
		private void SetNavigationRestriction(bool isRestricted)
		{
			if (this._isNavigationRestricted != isRestricted)
			{
				this._isNavigationRestricted = isRestricted;
			}
		}

		// Token: 0x06000136 RID: 310 RVA: 0x000077B4 File Offset: 0x000059B4
		protected override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
			MPLobbyVM lobbyDataSource = this._lobbyDataSource;
			if (lobbyDataSource != null)
			{
				lobbyDataSource.OnTick(dt);
			}
			if (this._activeFeedbackId == null && this._feedbackInquiries.Count > 0)
			{
				this.ShowNextFeedback();
			}
			if (this._lobbyLayer == null)
			{
				return;
			}
			KeybindingPopup keybindingPopup = this._keybindingPopup;
			if (keybindingPopup == null || !keybindingPopup.IsActive)
			{
				if (this._lobbyDataSource != null && !this._keybindingPopup.IsActive && !this._lobbyLayer.IsFocusedOnInput())
				{
					if (this._lobbyDataSource.Options.IsEnabled && this._lobbyLayer.Input.IsHotKeyPressed("TakeAll"))
					{
						this._lobbyDataSource.Options.SelectPreviousCategory();
						return;
					}
					if (this._lobbyDataSource.Options.IsEnabled && this._lobbyLayer.Input.IsHotKeyPressed("GiveAll"))
					{
						this._lobbyDataSource.Options.SelectNextCategory();
						return;
					}
					if (!this._lobbyDataSource.HasNoPopupOpen() && this._lobbyLayer.Input.IsHotKeyPressed("Confirm"))
					{
						this._lobbyDataSource.OnConfirm();
						return;
					}
					if (this._lobbyLayer.Input.IsHotKeyPressed("ToggleFriendsList"))
					{
						this._lobbyDataSource.Friends.IsListEnabled = !this._lobbyDataSource.Friends.IsListEnabled;
						return;
					}
					if (this._lobbyLayer.Input.IsHotKeyPressed("SwitchToPreviousTab") && !this._isNavigationRestricted)
					{
						if (this._lobbyDataSource.Options.IsEnabled && this._lobbyDataSource.Options.IsOptionsChanged())
						{
							this._lobbyDataSource.ShowCustomOptionsChangedInquiry(delegate
							{
								this.SelectPreviousPage(0);
							}, delegate
							{
								this.SelectPreviousPage(0);
							});
							return;
						}
						this.SelectPreviousPage(0);
						return;
					}
					else if (this._lobbyLayer.Input.IsHotKeyPressed("SwitchToNextTab") && !this._isNavigationRestricted)
					{
						if (this._lobbyDataSource.Options.IsEnabled && this._lobbyDataSource.Options.IsOptionsChanged())
						{
							this._lobbyDataSource.ShowCustomOptionsChangedInquiry(delegate
							{
								this.SelectNextPage(0);
							}, delegate
							{
								this.SelectNextPage(0);
							});
							return;
						}
						this.SelectNextPage(0);
						return;
					}
					else if (this._lobbyLayer.Input.IsHotKeyReleased("Reset"))
					{
						if (this._lobbyDataSource.Matchmaking.CustomServer.IsEnabled && !this._lobbyDataSource.Matchmaking.CustomServer.HostGame.IsEnabled)
						{
							this._lobbyDataSource.Matchmaking.CustomServer.ExecuteRefresh();
							return;
						}
					}
					else if (this._lobbyLayer.Input.IsHotKeyReleased("Exit"))
					{
						this._lobbyDataSource.OnEscape();
					}
				}
				return;
			}
			KeybindingPopup keybindingPopup2 = this._keybindingPopup;
			if (keybindingPopup2 == null)
			{
				return;
			}
			keybindingPopup2.Tick();
		}

		// Token: 0x06000137 RID: 311 RVA: 0x00007A90 File Offset: 0x00005C90
		private void ShowNextFeedback()
		{
			KeyValuePair<string, InquiryData> keyValuePair = this._feedbackInquiries[0];
			this._feedbackInquiries.Remove(keyValuePair);
			this._activeFeedbackId = keyValuePair.Key;
			InformationManager.ShowInquiry(keyValuePair.Value, false, false);
		}

		// Token: 0x06000138 RID: 312 RVA: 0x00007AD2 File Offset: 0x00005CD2
		[Conditional("DEBUG")]
		private void TickDebug(float dt)
		{
		}

		// Token: 0x06000139 RID: 313 RVA: 0x00007AD4 File Offset: 0x00005CD4
		void ILobbyStateHandler.SetConnectionState(bool isAuthenticated)
		{
			if (this._lobbyDataSource == null)
			{
				this.CreateView();
			}
			if (isAuthenticated && this._lobbyState.LobbyClient.PlayerData != null)
			{
				this._lobbyDataSource.RefreshPlayerData(this._lobbyState.LobbyClient.PlayerData);
				if (this._lobbyDataSource.CurrentPage == null || this._lobbyDataSource.CurrentPage == 1)
				{
					this._lobbyDataSource.SetPage(4, 4);
				}
			}
			else
			{
				if (this._isFacegenOpen)
				{
					this.OnForceCloseFacegen();
				}
				this._lobbyDataSource.SetPage(1, 4);
			}
			this._lobbyDataSource.ConnectionStateUpdated(isAuthenticated);
		}

		// Token: 0x0600013A RID: 314 RVA: 0x00007B70 File Offset: 0x00005D70
		void ILobbyStateHandler.OnRequestedToSearchBattle()
		{
			this._musicSoundEvent.SetParameter("mpMusicSwitcher", 1f);
			MPLobbyVM lobbyDataSource = this._lobbyDataSource;
			if (lobbyDataSource == null)
			{
				return;
			}
			lobbyDataSource.OnRequestedToSearchBattle();
		}

		// Token: 0x0600013B RID: 315 RVA: 0x00007B97 File Offset: 0x00005D97
		void ILobbyStateHandler.OnUpdateFindingGame(MatchmakingWaitTimeStats matchmakingWaitTimeStats, string[] gameTypeInfo)
		{
			MPLobbyVM lobbyDataSource = this._lobbyDataSource;
			if (lobbyDataSource == null)
			{
				return;
			}
			lobbyDataSource.OnUpdateFindingGame(matchmakingWaitTimeStats, gameTypeInfo);
		}

		// Token: 0x0600013C RID: 316 RVA: 0x00007BAB File Offset: 0x00005DAB
		void ILobbyStateHandler.OnRequestedToCancelSearchBattle()
		{
			MPLobbyVM lobbyDataSource = this._lobbyDataSource;
			if (lobbyDataSource == null)
			{
				return;
			}
			lobbyDataSource.OnRequestedToCancelSearchBattle();
		}

		// Token: 0x0600013D RID: 317 RVA: 0x00007BBD File Offset: 0x00005DBD
		void ILobbyStateHandler.OnSearchBattleCanceled()
		{
			this._musicSoundEvent.SetParameter("mpMusicSwitcher", 0f);
			MPLobbyVM lobbyDataSource = this._lobbyDataSource;
			if (lobbyDataSource == null)
			{
				return;
			}
			lobbyDataSource.OnSearchBattleCanceled();
		}

		// Token: 0x0600013E RID: 318 RVA: 0x00007BE4 File Offset: 0x00005DE4
		void ILobbyStateHandler.OnPause()
		{
		}

		// Token: 0x0600013F RID: 319 RVA: 0x00007BE6 File Offset: 0x00005DE6
		void ILobbyStateHandler.OnResume()
		{
		}

		// Token: 0x06000140 RID: 320 RVA: 0x00007BE8 File Offset: 0x00005DE8
		void ILobbyStateHandler.OnDisconnected()
		{
			MPLobbyVM lobbyDataSource = this._lobbyDataSource;
			if (lobbyDataSource == null)
			{
				return;
			}
			lobbyDataSource.OnDisconnected();
		}

		// Token: 0x06000141 RID: 321 RVA: 0x00007BFA File Offset: 0x00005DFA
		void ILobbyStateHandler.OnPlayerDataReceived(PlayerData playerData)
		{
			MPLobbyVM lobbyDataSource = this._lobbyDataSource;
			if (lobbyDataSource != null)
			{
				lobbyDataSource.RefreshPlayerData(playerData);
			}
			GauntletChatLogView gauntletChatLogView = GauntletChatLogView.Current;
			if (gauntletChatLogView == null)
			{
				return;
			}
			gauntletChatLogView.OnSupportedFeaturesReceived(this._lobbyState.LobbyClient.SupportedFeatures);
		}

		// Token: 0x06000142 RID: 322 RVA: 0x00007C2D File Offset: 0x00005E2D
		void ILobbyStateHandler.OnPendingRejoin()
		{
			this._lobbyDataSource.SetPage(2, 4);
		}

		// Token: 0x06000143 RID: 323 RVA: 0x00007C3C File Offset: 0x00005E3C
		void ILobbyStateHandler.OnEnterBattleWithParty(string[] selectedGameTypes)
		{
		}

		// Token: 0x06000144 RID: 324 RVA: 0x00007C40 File Offset: 0x00005E40
		void ILobbyStateHandler.OnPartyInvitationReceived(PlayerId playerID)
		{
			if (!this._lobbyState.LobbyClient.SupportedFeatures.SupportsFeatures(4))
			{
				this._lobbyState.LobbyClient.DeclinePartyInvitation();
				return;
			}
			MPLobbyVM lobbyDataSource = this._lobbyDataSource;
			if (lobbyDataSource == null)
			{
				return;
			}
			lobbyDataSource.PartyInvitationPopup.OpenWith(playerID);
		}

		// Token: 0x06000145 RID: 325 RVA: 0x00007C8C File Offset: 0x00005E8C
		void ILobbyStateHandler.OnPartyInvitationInvalidated()
		{
			MPLobbyVM lobbyDataSource = this._lobbyDataSource;
			if (lobbyDataSource == null)
			{
				return;
			}
			lobbyDataSource.PartyInvitationPopup.Close();
		}

		// Token: 0x06000146 RID: 326 RVA: 0x00007CA3 File Offset: 0x00005EA3
		void ILobbyStateHandler.OnPlayerInvitedToParty(PlayerId playerId)
		{
			MPLobbyVM lobbyDataSource = this._lobbyDataSource;
			if (lobbyDataSource == null)
			{
				return;
			}
			lobbyDataSource.Friends.OnPlayerInvitedToParty(playerId);
		}

		// Token: 0x06000147 RID: 327 RVA: 0x00007CBB File Offset: 0x00005EBB
		void ILobbyStateHandler.OnPlayerAddedToParty(PlayerId playerId, string playerName, bool isPartyLeader)
		{
			MPLobbyVM lobbyDataSource = this._lobbyDataSource;
			if (lobbyDataSource == null)
			{
				return;
			}
			lobbyDataSource.OnPlayerAddedToParty(playerId);
		}

		// Token: 0x06000148 RID: 328 RVA: 0x00007CCE File Offset: 0x00005ECE
		void ILobbyStateHandler.OnPlayerRemovedFromParty(PlayerId playerId, PartyRemoveReason reason)
		{
			MPLobbyVM lobbyDataSource = this._lobbyDataSource;
			if (lobbyDataSource == null)
			{
				return;
			}
			lobbyDataSource.OnPlayerRemovedFromParty(playerId, reason);
		}

		// Token: 0x06000149 RID: 329 RVA: 0x00007CE2 File Offset: 0x00005EE2
		void ILobbyStateHandler.OnGameClientStateChange(LobbyClient.State state)
		{
		}

		// Token: 0x0600014A RID: 330 RVA: 0x00007CE4 File Offset: 0x00005EE4
		void ILobbyStateHandler.OnAdminMessageReceived(string message)
		{
			InformationManager.AddSystemNotification(message);
		}

		// Token: 0x0600014B RID: 331 RVA: 0x00007CEC File Offset: 0x00005EEC
		public void OnBattleServerInformationReceived(BattleServerInformationForClient battleServerInformation)
		{
			SoundEvent.PlaySound2D("event:/ui/multiplayer/match_ready");
			this._lobbyDataSource.Matchmaking.IsFindingMatch = false;
		}

		// Token: 0x0600014C RID: 332 RVA: 0x00007D0C File Offset: 0x00005F0C
		string ILobbyStateHandler.ShowFeedback(string title, string feedbackText)
		{
			string id = Guid.NewGuid().ToString();
			InquiryData inquiryData = new InquiryData(title, feedbackText, false, true, "", new TextObject("{=dismissnotification}Dismiss", null).ToString(), null, delegate
			{
				this.DismissFeedback(id);
			}, "", 0f, null, null, null);
			this._feedbackInquiries.Add(new KeyValuePair<string, InquiryData>(id, inquiryData));
			return id;
		}

		// Token: 0x0600014D RID: 333 RVA: 0x00007D98 File Offset: 0x00005F98
		string ILobbyStateHandler.ShowFeedback(InquiryData inquiryData)
		{
			string text = Guid.NewGuid().ToString();
			this._feedbackInquiries.Add(new KeyValuePair<string, InquiryData>(text, inquiryData));
			return text;
		}

		// Token: 0x0600014E RID: 334 RVA: 0x00007DCC File Offset: 0x00005FCC
		void ILobbyStateHandler.DismissFeedback(string feedbackId)
		{
			if (this._activeFeedbackId != null && this._activeFeedbackId.Equals(feedbackId))
			{
				InformationManager.HideInquiry();
				this._activeFeedbackId = null;
				return;
			}
			KeyValuePair<string, InquiryData> keyValuePair = this._feedbackInquiries.FirstOrDefault((KeyValuePair<string, InquiryData> q) => q.Key.Equals(feedbackId));
			if (keyValuePair.Key != null)
			{
				this._feedbackInquiries.Remove(keyValuePair);
			}
		}

		// Token: 0x0600014F RID: 335 RVA: 0x00007E3C File Offset: 0x0000603C
		private void SelectPreviousPage(MPLobbyVM.LobbyPage currentPage = 0)
		{
			MPLobbyVM lobbyDataSource = this._lobbyDataSource;
			if (lobbyDataSource != null && lobbyDataSource.HasNoPopupOpen())
			{
				if (currentPage == null)
				{
					currentPage = this._lobbyDataSource.CurrentPage;
				}
				if (currentPage < 3 || currentPage > 7)
				{
					return;
				}
				MPLobbyVM.LobbyPage lobbyPage = ((currentPage == 3) ? 7 : (currentPage - 1));
				if (this._lobbyDataSource.DisallowedPages.Contains(lobbyPage))
				{
					this.SelectPreviousPage(lobbyPage);
					return;
				}
				this._lobbyDataSource.SetPage(lobbyPage, 4);
			}
		}

		// Token: 0x06000150 RID: 336 RVA: 0x00007EAC File Offset: 0x000060AC
		private void SelectNextPage(MPLobbyVM.LobbyPage currentPage = 0)
		{
			MPLobbyVM lobbyDataSource = this._lobbyDataSource;
			if (lobbyDataSource != null && lobbyDataSource.HasNoPopupOpen())
			{
				if (currentPage == null)
				{
					currentPage = this._lobbyDataSource.CurrentPage;
				}
				if (currentPage < 3 || currentPage > 7)
				{
					return;
				}
				MPLobbyVM.LobbyPage lobbyPage = ((currentPage == 7) ? 3 : (currentPage + 1));
				if (this._lobbyDataSource.DisallowedPages.Contains(lobbyPage))
				{
					this.SelectNextPage(lobbyPage);
					return;
				}
				this._lobbyDataSource.SetPage(lobbyPage, 4);
			}
		}

		// Token: 0x06000151 RID: 337 RVA: 0x00007F19 File Offset: 0x00006119
		void ILobbyStateHandler.OnActivateCustomServer()
		{
			this._lobbyDataSource.SetPage(6, 2);
		}

		// Token: 0x06000152 RID: 338 RVA: 0x00007F28 File Offset: 0x00006128
		void ILobbyStateHandler.OnActivateHome()
		{
			this._lobbyDataSource.SetPage(4, 4);
		}

		// Token: 0x06000153 RID: 339 RVA: 0x00007F37 File Offset: 0x00006137
		void ILobbyStateHandler.OnActivateMatchmaking()
		{
			this._lobbyDataSource.SetPage(6, 4);
		}

		// Token: 0x06000154 RID: 340 RVA: 0x00007F46 File Offset: 0x00006146
		void ILobbyStateHandler.OnActivateArmory()
		{
			this._lobbyDataSource.SetPage(5, 4);
		}

		// Token: 0x06000155 RID: 341 RVA: 0x00007F55 File Offset: 0x00006155
		void ILobbyStateHandler.OnActivateProfile()
		{
			this._lobbyDataSource.SetPage(7, 4);
		}

		// Token: 0x06000156 RID: 342 RVA: 0x00007F64 File Offset: 0x00006164
		void ILobbyStateHandler.OnClanInvitationReceived(string clanName, string clanTag, bool isCreation)
		{
			this._lobbyDataSource.ClanInvitationPopup.Open(clanName, clanTag, isCreation);
		}

		// Token: 0x06000157 RID: 343 RVA: 0x00007F79 File Offset: 0x00006179
		void ILobbyStateHandler.OnClanInvitationAnswered(PlayerId playerId, ClanCreationAnswer answer)
		{
			MPLobbyVM lobbyDataSource = this._lobbyDataSource;
			if (lobbyDataSource != null)
			{
				lobbyDataSource.ClanCreationPopup.UpdateConfirmation(playerId, answer);
			}
			MPLobbyVM lobbyDataSource2 = this._lobbyDataSource;
			if (lobbyDataSource2 == null)
			{
				return;
			}
			lobbyDataSource2.ClanInvitationPopup.UpdateConfirmation(playerId, answer);
		}

		// Token: 0x06000158 RID: 344 RVA: 0x00007FAA File Offset: 0x000061AA
		void ILobbyStateHandler.OnClanCreationSuccessful()
		{
			MPLobbyVM lobbyDataSource = this._lobbyDataSource;
			if (lobbyDataSource == null)
			{
				return;
			}
			lobbyDataSource.OnClanCreationFinished();
		}

		// Token: 0x06000159 RID: 345 RVA: 0x00007FBC File Offset: 0x000061BC
		void ILobbyStateHandler.OnClanCreationFailed()
		{
			MPLobbyVM lobbyDataSource = this._lobbyDataSource;
			if (lobbyDataSource == null)
			{
				return;
			}
			lobbyDataSource.OnClanCreationFinished();
		}

		// Token: 0x0600015A RID: 346 RVA: 0x00007FCE File Offset: 0x000061CE
		void ILobbyStateHandler.OnClanCreationStarted()
		{
			this._lobbyDataSource.ClanCreationPopup.ExecuteSwitchToWaiting();
		}

		// Token: 0x0600015B RID: 347 RVA: 0x00007FE0 File Offset: 0x000061E0
		void ILobbyStateHandler.OnClanInfoChanged()
		{
			MPLobbyVM lobbyDataSource = this._lobbyDataSource;
			if (lobbyDataSource == null)
			{
				return;
			}
			lobbyDataSource.OnClanInfoChanged();
		}

		// Token: 0x0600015C RID: 348 RVA: 0x00007FF2 File Offset: 0x000061F2
		void ILobbyStateHandler.OnPremadeGameEligibilityStatusReceived(bool isEligible)
		{
			MPLobbyVM lobbyDataSource = this._lobbyDataSource;
			if (lobbyDataSource == null)
			{
				return;
			}
			lobbyDataSource.Matchmaking.OnPremadeGameEligibilityStatusReceived(isEligible);
		}

		// Token: 0x0600015D RID: 349 RVA: 0x0000800A File Offset: 0x0000620A
		void ILobbyStateHandler.OnPremadeGameCreated()
		{
			MPLobbyVM lobbyDataSource = this._lobbyDataSource;
			if (lobbyDataSource == null)
			{
				return;
			}
			lobbyDataSource.OnPremadeGameCreated();
		}

		// Token: 0x0600015E RID: 350 RVA: 0x0000801C File Offset: 0x0000621C
		void ILobbyStateHandler.OnPremadeGameListReceived()
		{
			MPLobbyVM lobbyDataSource = this._lobbyDataSource;
			if (lobbyDataSource == null)
			{
				return;
			}
			lobbyDataSource.Matchmaking.PremadeMatches.RefreshPremadeGameList();
		}

		// Token: 0x0600015F RID: 351 RVA: 0x00008038 File Offset: 0x00006238
		void ILobbyStateHandler.OnPremadeGameCreationCancelled()
		{
			this._musicSoundEvent.SetParameter("mpMusicSwitcher", 0f);
			MPLobbyVM lobbyDataSource = this._lobbyDataSource;
			if (lobbyDataSource == null)
			{
				return;
			}
			lobbyDataSource.OnSearchBattleCanceled();
		}

		// Token: 0x06000160 RID: 352 RVA: 0x0000805F File Offset: 0x0000625F
		void ILobbyStateHandler.OnJoinPremadeGameRequested(string clanName, string clanSigilCode, Guid partyId, PlayerId[] challengerPlayerIDs, PlayerId challengerPartyLeaderID, PremadeGameType premadeGameType)
		{
			this._lobbyDataSource.ClanMatchmakingRequestPopup.OpenWith(clanName, clanSigilCode, partyId, challengerPlayerIDs, challengerPartyLeaderID, premadeGameType);
		}

		// Token: 0x06000161 RID: 353 RVA: 0x0000807A File Offset: 0x0000627A
		void ILobbyStateHandler.OnJoinPremadeGameRequestSuccessful()
		{
			if (!this._lobbyDataSource.GameSearch.IsEnabled)
			{
				this._lobbyDataSource.OnPremadeGameCreated();
			}
			this._lobbyDataSource.GameSearch.OnJoinPremadeGameRequestSuccessful();
		}

		// Token: 0x06000162 RID: 354 RVA: 0x000080A9 File Offset: 0x000062A9
		void ILobbyStateHandler.OnSigilChanged()
		{
			if (this._lobbyDataSource != null)
			{
				this._lobbyDataSource.OnSigilChanged(this._lobbyDataSource.ChangeSigilPopup.SelectedSigil.IconID);
			}
		}

		// Token: 0x06000163 RID: 355 RVA: 0x000080D3 File Offset: 0x000062D3
		void ILobbyStateHandler.OnActivateOptions()
		{
			this._lobbyDataSource.SetPage(3, 4);
		}

		// Token: 0x06000164 RID: 356 RVA: 0x000080E2 File Offset: 0x000062E2
		void ILobbyStateHandler.OnDeactivateOptions()
		{
			this._lobbyDataSource.SetPage(4, 4);
		}

		// Token: 0x06000165 RID: 357 RVA: 0x000080F1 File Offset: 0x000062F1
		void ILobbyStateHandler.OnCustomGameServerListReceived(AvailableCustomGames customGameServerList)
		{
			this._lobbyDataSource.Matchmaking.CustomServer.RefreshCustomGameServerList(customGameServerList);
		}

		// Token: 0x06000166 RID: 358 RVA: 0x00008109 File Offset: 0x00006309
		void ILobbyStateHandler.OnMatchmakerGameOver(int oldExperience, int newExperience, List<string> badgesEarned, int lootGained, RankBarInfo oldRankBarInfo, RankBarInfo newRankBarInfo)
		{
			MPLobbyVM lobbyDataSource = this._lobbyDataSource;
			if (lobbyDataSource == null)
			{
				return;
			}
			lobbyDataSource.AfterBattlePopup.OpenWith(oldExperience, newExperience, badgesEarned, lootGained, oldRankBarInfo, newRankBarInfo);
		}

		// Token: 0x06000167 RID: 359 RVA: 0x0000812C File Offset: 0x0000632C
		void ILobbyStateHandler.OnBattleServerLost()
		{
			TextObject textObject = new TextObject("{=wLpJEkKY}Battle Server Crashed", null);
			TextObject textObject2 = new TextObject("{=EzeFJo65}You have been disconnected from server!", null);
			this._lobbyDataSource.Popup.ShowMessage(textObject, textObject2);
		}

		// Token: 0x06000168 RID: 360 RVA: 0x00008163 File Offset: 0x00006363
		void ILobbyStateHandler.OnRemovedFromMatchmakerGame(DisconnectType disconnectType)
		{
			this.ShowDisconnectMessage(disconnectType);
		}

		// Token: 0x06000169 RID: 361 RVA: 0x0000816C File Offset: 0x0000636C
		void ILobbyStateHandler.OnRemovedFromCustomGame(DisconnectType disconnectType)
		{
			this.ShowDisconnectMessage(disconnectType);
		}

		// Token: 0x0600016A RID: 362 RVA: 0x00008175 File Offset: 0x00006375
		void ILobbyStateHandler.OnPlayerAssignedPartyLeader(PlayerId partyLeaderId)
		{
			MPLobbyVM lobbyDataSource = this._lobbyDataSource;
			if (lobbyDataSource == null)
			{
				return;
			}
			lobbyDataSource.OnPlayerAssignedPartyLeader(partyLeaderId);
		}

		// Token: 0x0600016B RID: 363 RVA: 0x00008188 File Offset: 0x00006388
		void ILobbyStateHandler.OnPlayerSuggestedToParty(PlayerId playerId, string playerName, PlayerId suggestingPlayerId, string suggestingPlayerName)
		{
			MPLobbyVM lobbyDataSource = this._lobbyDataSource;
			if (lobbyDataSource == null)
			{
				return;
			}
			lobbyDataSource.OnPlayerSuggestedToParty(playerId, playerName, suggestingPlayerId, suggestingPlayerName);
		}

		// Token: 0x0600016C RID: 364 RVA: 0x0000819F File Offset: 0x0000639F
		void ILobbyStateHandler.OnNotificationsReceived(LobbyNotification[] notifications)
		{
			MPLobbyVM lobbyDataSource = this._lobbyDataSource;
			if (lobbyDataSource == null)
			{
				return;
			}
			lobbyDataSource.OnNotificationsReceived(notifications);
		}

		// Token: 0x0600016D RID: 365 RVA: 0x000081B4 File Offset: 0x000063B4
		void ILobbyStateHandler.OnJoinCustomGameFailureResponse(CustomGameJoinResponse response)
		{
			TextObject textObject = new TextObject("{=4mMySbxI}Unspecified error", null);
			switch (response)
			{
			case 1:
				textObject = new TextObject("{=KO2adj2I}You need to be in Lobby to join a custom game", null);
				break;
			case 2:
				textObject = new TextObject("{=IzJ7f5SQ}Server capacity is full", null);
				break;
			case 3:
				textObject = new TextObject("{=vkpMgobZ}Game server error", null);
				break;
			case 4:
				textObject = new TextObject("{=JQVixeQs}Couldn't access game server", null);
				break;
			case 5:
				textObject = new TextObject("{=T8IniCKU}Game server is not available", null);
				break;
			case 6:
				textObject = new TextObject("{=KRNdlbkq}Custom game is ending", null);
				break;
			case 7:
				textObject = new TextObject("{=Mm1Kb1bS}Incorrect password", null);
				break;
			case 8:
				textObject = new TextObject("{=srAJw3Tg}Player is banned from server", null);
				break;
			case 13:
				textObject = new TextObject("{=ivKntfNA}Already requested to join, waiting for server response", null);
				break;
			case 14:
				textObject = new TextObject("{=KQrpWV1n}You need be the party leader to join a custom game", null);
				break;
			case 15:
				textObject = new TextObject("{=tlsmbvQX}Not all players are ready to join", null);
				break;
			case 16:
				textObject = new TextObject("{=LCzAvLUB}Not all players' modules match with the server", null);
				break;
			}
			TextObject textObject2 = new TextObject("{=mO9bh5sy}Couldn't join custom game", null);
			this._lobbyDataSource.Popup.ShowMessage(textObject2, textObject);
		}

		// Token: 0x0600016E RID: 366 RVA: 0x000082E4 File Offset: 0x000064E4
		void ILobbyStateHandler.OnServerStatusReceived(ServerStatus serverStatus)
		{
			MPLobbyVM lobbyDataSource = this._lobbyDataSource;
			if (lobbyDataSource != null)
			{
				lobbyDataSource.OnServerStatusReceived(serverStatus);
			}
			if (serverStatus.Announcement != null)
			{
				if (serverStatus.Announcement.Type == 1)
				{
					InformationManager.AddSystemNotification(serverStatus.Announcement.Text.ToString());
					return;
				}
				if (serverStatus.Announcement.Type == null)
				{
					InformationManager.DisplayMessage(new InformationMessage(serverStatus.Announcement.Text.ToString()));
				}
			}
		}

		// Token: 0x0600016F RID: 367 RVA: 0x00008356 File Offset: 0x00006556
		void ILobbyStateHandler.OnRejoinBattleRequestAnswered(bool isSuccessful)
		{
			MPLobbyVM lobbyDataSource = this._lobbyDataSource;
			if (lobbyDataSource == null)
			{
				return;
			}
			lobbyDataSource.OnRejoinBattleRequestAnswered(isSuccessful);
		}

		// Token: 0x06000170 RID: 368 RVA: 0x00008369 File Offset: 0x00006569
		void ILobbyStateHandler.OnFriendListUpdated()
		{
			MPLobbyVM lobbyDataSource = this._lobbyDataSource;
			if (lobbyDataSource == null)
			{
				return;
			}
			lobbyDataSource.OnFriendListUpdated(false);
		}

		// Token: 0x06000171 RID: 369 RVA: 0x0000837C File Offset: 0x0000657C
		void ILobbyStateHandler.OnPlayerNameUpdated(string playerName)
		{
			MPLobbyVM lobbyDataSource = this._lobbyDataSource;
			if (lobbyDataSource == null)
			{
				return;
			}
			lobbyDataSource.OnPlayerNameUpdated(playerName);
		}

		// Token: 0x06000172 RID: 370 RVA: 0x00008390 File Offset: 0x00006590
		private void ShowDisconnectMessage(DisconnectType disconnectType)
		{
			if (disconnectType != null && disconnectType != 7)
			{
				TextObject textObject = new TextObject("{=JluTW3Qw}Game Ended", null);
				TextObject textObject2 = new TextObject("{=aKjpbRP5}Unknown reason", null);
				switch (disconnectType)
				{
				case 1:
					textObject2 = new TextObject("{=WvGviFgt}Your connection with the server timed out", null);
					break;
				case 2:
					textObject2 = new TextObject("{=a0IHtkoa}You are kicked by game host", null);
					break;
				case 3:
					textObject2 = new TextObject("{=wbFB3N72}You are kicked from game by poll", null);
					break;
				case 4:
					textObject2 = new TextObject("{=OhF7NqSb}You are banned from game by poll", null);
					break;
				case 5:
					textObject2 = new TextObject("{=074YAjOk}You are kicked due to inactivity", null);
					break;
				case 8:
					textObject2 = new TextObject("{=tKSxGy5p}Server not responding", null);
					break;
				case 9:
					textObject2 = new TextObject("{=InUAmnX4}You are kicked due to friendly damage", null);
					break;
				case 10:
					textObject2 = new TextObject("{=O1bGoaE8}Server state could not be retrieved. Please try again.", null);
					break;
				}
				this._lobbyDataSource.Popup.ShowMessage(textObject, textObject2);
			}
		}

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x06000173 RID: 371 RVA: 0x00008474 File Offset: 0x00006674
		public MPLobbyVM DataSource
		{
			get
			{
				return this._lobbyDataSource;
			}
		}

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x06000174 RID: 372 RVA: 0x0000847C File Offset: 0x0000667C
		public GauntletLayer LobbyLayer
		{
			get
			{
				return this._lobbyLayer;
			}
		}

		// Token: 0x06000175 RID: 373 RVA: 0x00008484 File Offset: 0x00006684
		private void DisableLobby()
		{
			if (!this._isLobbyActive)
			{
				return;
			}
			this._isLobbyActive = false;
			SpriteCategory mplobbyCategory = this._mplobbyCategory;
			if (mplobbyCategory != null)
			{
				mplobbyCategory.Unload();
			}
			SpriteCategory bannerIconsCategory = this._bannerIconsCategory;
			if (bannerIconsCategory != null)
			{
				bannerIconsCategory.Unload();
			}
			base.RemoveLayer(this._lobbyLayer);
			this._lobbyDataSource = null;
			this._lobbyLayer = null;
		}

		// Token: 0x06000176 RID: 374 RVA: 0x000084DD File Offset: 0x000066DD
		private void OnKeybindRequest(KeyOptionVM requestedHotKeyToChange)
		{
			this._currentKey = requestedHotKeyToChange;
			this._keybindingPopup.OnToggle(true);
		}

		// Token: 0x06000177 RID: 375 RVA: 0x000084F4 File Offset: 0x000066F4
		private void SetHotKey(Key key)
		{
			GameKeyOptionVM gameKey;
			if ((gameKey = this._currentKey as GameKeyOptionVM) == null)
			{
				AuxiliaryKeyOptionVM auxiliaryKey;
				if ((auxiliaryKey = this._currentKey as AuxiliaryKeyOptionVM) != null)
				{
					AuxiliaryKeyGroupVM auxiliaryKeyGroupVM = this._lobbyDataSource.Options.GameKeyOptionGroups.AuxiliaryKeyGroups.FirstOrDefault((AuxiliaryKeyGroupVM g) => g.HotKeys.Contains(auxiliaryKey));
					GauntletLayer lobbyLayer = this._lobbyLayer;
					if (lobbyLayer != null && lobbyLayer.Input.IsHotKeyReleased("Exit"))
					{
						this._keybindingPopup.OnToggle(false);
						return;
					}
					if (auxiliaryKeyGroupVM != null && auxiliaryKeyGroupVM.HotKeys.Any((AuxiliaryKeyOptionVM k) => k.CurrentKey.InputKey == key.InputKey && k.CurrentHotKey.HasSameModifiers(auxiliaryKey.CurrentHotKey)))
					{
						InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=n4UUrd1p}Already in use", null).ToString()));
						return;
					}
					AuxiliaryKeyOptionVM auxiliaryKey2 = auxiliaryKey;
					if (auxiliaryKey2 != null)
					{
						auxiliaryKey2.Set(key.InputKey);
					}
					auxiliaryKey = null;
					this._keybindingPopup.OnToggle(false);
				}
				return;
			}
			MPLobbyVM lobbyDataSource = this._lobbyDataSource;
			GameKeyGroupVM gameKeyGroupVM = ((lobbyDataSource != null) ? lobbyDataSource.Options.GameKeyOptionGroups.GameKeyGroups.FirstOrDefault((GameKeyGroupVM g) => g.GameKeys.Contains(gameKey)) : null);
			GauntletLayer lobbyLayer2 = this._lobbyLayer;
			if (lobbyLayer2 != null && lobbyLayer2.Input.IsHotKeyReleased("Exit"))
			{
				this._keybindingPopup.OnToggle(false);
				return;
			}
			if (gameKeyGroupVM != null && gameKeyGroupVM.GameKeys.Any((GameKeyOptionVM k) => k.CurrentKey.InputKey == key.InputKey))
			{
				InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=n4UUrd1p}Already in use", null).ToString()));
				return;
			}
			GameKeyOptionVM gameKey2 = gameKey;
			if (gameKey2 != null)
			{
				gameKey2.Set(key.InputKey);
			}
			gameKey = null;
			this._keybindingPopup.OnToggle(false);
		}

		// Token: 0x06000178 RID: 376 RVA: 0x000086D0 File Offset: 0x000068D0
		void IGameStateListener.OnActivate()
		{
			this._musicSoundEvent = SoundEvent.CreateEventFromString("event:/multiplayer/lobby_music", null);
			this._musicSoundEvent.Play();
			if (this._lobbyDataSource == null)
			{
				this.CreateView();
				this._lobbyDataSource.SetPage(1, 4);
				return;
			}
			this._spriteCategory = UIResourceManager.SpriteData.SpriteCategories["ui_options"];
			this._spriteCategory.Load(UIResourceManager.ResourceContext, UIResourceManager.UIResourceDepot);
		}

		// Token: 0x06000179 RID: 377 RVA: 0x00008748 File Offset: 0x00006948
		void IGameStateListener.OnDeactivate()
		{
			base.RemoveLayer(this._lobbyLayer);
			if (this._lobbyDataSource != null)
			{
				this._cachedCustomServerSortOption = this._lobbyDataSource.Matchmaking.CustomServer.SortController.CurrentSortOption;
				this._cachedPremadeGameSortOption = this._lobbyDataSource.Matchmaking.PremadeMatches.SortController.CurrentSortOption;
				this._lobbyDataSource.OnFinalize();
				this._lobbyDataSource = null;
			}
			this._lobbyLayer = null;
			SpriteCategory mplobbyCategory = this._mplobbyCategory;
			if (mplobbyCategory != null)
			{
				mplobbyCategory.Unload();
			}
			SpriteCategory bannerIconsCategory = this._bannerIconsCategory;
			if (bannerIconsCategory != null)
			{
				bannerIconsCategory.Unload();
			}
			this._musicSoundEvent.Stop();
			this._musicSoundEvent = null;
		}

		// Token: 0x0600017A RID: 378 RVA: 0x000087F6 File Offset: 0x000069F6
		void IGameStateListener.OnInitialize()
		{
		}

		// Token: 0x0600017B RID: 379 RVA: 0x000087F8 File Offset: 0x000069F8
		void IGameStateListener.OnFinalize()
		{
		}

		// Token: 0x040000A7 RID: 167
		private List<KeyValuePair<string, InquiryData>> _feedbackInquiries;

		// Token: 0x040000A8 RID: 168
		private string _activeFeedbackId;

		// Token: 0x040000A9 RID: 169
		private KeybindingPopup _keybindingPopup;

		// Token: 0x040000AA RID: 170
		private KeyOptionVM _currentKey;

		// Token: 0x040000AB RID: 171
		private SpriteCategory _spriteCategory;

		// Token: 0x040000AC RID: 172
		private GauntletLayer _gauntletBrightnessLayer;

		// Token: 0x040000AD RID: 173
		private BrightnessOptionVM _brightnessOptionDataSource;

		// Token: 0x040000AE RID: 174
		private IGauntletMovie _brightnessOptionMovie;

		// Token: 0x040000AF RID: 175
		private LobbyState _lobbyState;

		// Token: 0x040000B0 RID: 176
		private BasicCharacterObject _playerCharacter;

		// Token: 0x040000B1 RID: 177
		private bool _isFacegenOpen;

		// Token: 0x040000B2 RID: 178
		private SoundEvent _musicSoundEvent;

		// Token: 0x040000B3 RID: 179
		private bool _isNavigationRestricted;

		// Token: 0x040000B4 RID: 180
		private MPCustomGameSortControllerVM.CustomServerSortOption? _cachedCustomServerSortOption;

		// Token: 0x040000B5 RID: 181
		private MPCustomGameSortControllerVM.CustomServerSortOption? _cachedPremadeGameSortOption;

		// Token: 0x040000B6 RID: 182
		private bool _isLobbyActive;

		// Token: 0x040000B7 RID: 183
		private GauntletLayer _lobbyLayer;

		// Token: 0x040000B8 RID: 184
		private MPLobbyVM _lobbyDataSource;

		// Token: 0x040000B9 RID: 185
		private SpriteCategory _mplobbyCategory;

		// Token: 0x040000BA RID: 186
		private SpriteCategory _bannerIconsCategory;

		// Token: 0x040000BB RID: 187
		private SpriteCategory _badgesCategory;
	}
}
