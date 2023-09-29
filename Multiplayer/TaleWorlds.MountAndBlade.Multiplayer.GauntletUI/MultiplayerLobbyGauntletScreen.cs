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
using TaleWorlds.MountAndBlade.GauntletUI;
using TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby;
using TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.Armory.CosmeticItem;
using TaleWorlds.MountAndBlade.Multiplayer.ViewModelCollection.Lobby.CustomGame;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions;
using TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions.AuxiliaryKeys;
using TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions.GameKeys;
using TaleWorlds.PlayerServices;
using TaleWorlds.ScreenSystem;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.Multiplayer.GauntletUI
{
	[GameStateScreen(typeof(LobbyState))]
	public class MultiplayerLobbyGauntletScreen : ScreenBase, IGameStateListener, ILobbyStateHandler, IGauntletChatLogHandlerScreen
	{
		public MPLobbyVM.LobbyPage CurrentPage
		{
			get
			{
				if (this._lobbyDataSource != null)
				{
					return this._lobbyDataSource.CurrentPage;
				}
				return MPLobbyVM.LobbyPage.NotAssigned;
			}
		}

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

		private void OnManagedOptionChanged(ManagedOptions.ManagedOptionsType changedManagedOptionsType)
		{
			if (changedManagedOptionsType == 31)
			{
				MPLobbyVM lobbyDataSource = this._lobbyDataSource;
				if (lobbyDataSource != null)
				{
					lobbyDataSource.OnEnableGenericAvatarsChanged();
				}
			}
			if (changedManagedOptionsType == 32)
			{
				MPLobbyVM lobbyDataSource2 = this._lobbyDataSource;
				if (lobbyDataSource2 == null)
				{
					return;
				}
				lobbyDataSource2.OnEnableGenericNamesChanged();
			}
		}

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
			GameKeyContext category = HotKeyManager.GetCategory("MultiplayerHotkeyCategory");
			GameKeyContext geneircPanelCategory = HotKeyManager.GetCategory("GenericPanelGameKeyCategory");
			this._lobbyDataSource.BadgeSelectionPopup.RefreshKeyBindings(category.GetHotKey("InspectBadgeProgression"));
			this._lobbyDataSource.Armory.Cosmetics.RefreshKeyBindings(category.GetHotKey("PerformActionOnCosmeticItem"), category.GetHotKey("PreviewCosmeticItem"));
			this._lobbyDataSource.Armory.Cosmetics.TauntSlots.ApplyActionOnAllItems(delegate(MPArmoryCosmeticTauntSlotVM t)
			{
				t.SetSelectKeyVisual(geneircPanelCategory.GetHotKey("GiveAll"));
			});
			this._lobbyDataSource.Armory.Cosmetics.TauntSlots.ApplyActionOnAllItems(delegate(MPArmoryCosmeticTauntSlotVM t)
			{
				t.SetEmptySlotKeyVisual(geneircPanelCategory.GetHotKey("TakeAll"));
			});
			this._lobbyDataSource.Friends.SetToggleFriendListKey(category.RegisteredHotKeys.FirstOrDefault((HotKey g) => ((g != null) ? g.Id : null) == "ToggleFriendsList"));
			this._lobbyDataSource.Matchmaking.CustomServer.SortController.SetSortOption(this._cachedCustomServerSortOption);
			this._lobbyDataSource.Matchmaking.PremadeMatches.SortController.SetSortOption(this._cachedPremadeGameSortOption);
			this._lobbyDataSource.Options.SetDoneInputKey(geneircPanelCategory.GetHotKey("Confirm"));
			this._lobbyDataSource.Options.SetCancelInputKey(geneircPanelCategory.GetHotKey("Exit"));
			this._lobbyDataSource.Options.SetResetInputKey(geneircPanelCategory.GetHotKey("Reset"));
			this._lobbyDataSource.Options.SetPreviousTabInputKey(geneircPanelCategory.GetHotKey("TakeAll"));
			this._lobbyDataSource.Options.SetNextTabInputKey(geneircPanelCategory.GetHotKey("GiveAll"));
			this._lobbyDataSource.Matchmaking.CustomServer.SetRefreshInputKey(geneircPanelCategory.GetHotKey("Reset"));
			if (NativeOptions.GetConfig(69) < 2f)
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

		private void OnCloseBrightness(bool isConfirm)
		{
			this._gauntletBrightnessLayer.ReleaseMovie(this._brightnessOptionMovie);
			base.RemoveLayer(this._gauntletBrightnessLayer);
			this._brightnessOptionDataSource = null;
			this._gauntletBrightnessLayer = null;
			NativeOptions.SaveConfig();
		}

		protected override void OnInitialize()
		{
			base.OnInitialize();
			LoadingWindow.DisableGlobalLoadingWindow();
			this._keybindingPopup = new KeybindingPopup(new Action<Key>(this.SetHotKey), this);
			MPLobbyVM lobbyDataSource = this._lobbyDataSource;
			if (lobbyDataSource != null)
			{
				lobbyDataSource.RefreshPlayerData(this._lobbyState.LobbyClient.PlayerData);
			}
			ManagedOptions.OnManagedOptionChanged = (ManagedOptions.OnManagedOptionChangedDelegate)Delegate.Combine(ManagedOptions.OnManagedOptionChanged, new ManagedOptions.OnManagedOptionChangedDelegate(this.OnManagedOptionChanged));
		}

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

		protected override void OnActivate()
		{
			if (this._lobbyDataSource != null && this._isFacegenOpen)
			{
				this.OnFacegenClosed(true);
			}
			MPLobbyVM lobbyDataSource = this._lobbyDataSource;
			if (lobbyDataSource != null)
			{
				lobbyDataSource.OnActivate();
			}
			MPLobbyVM lobbyDataSource2 = this._lobbyDataSource;
			if (lobbyDataSource2 != null)
			{
				lobbyDataSource2.RefreshPlayerData(this._lobbyState.LobbyClient.PlayerData);
			}
			LoadingWindow.DisableGlobalLoadingWindow();
		}

		protected override void OnDeactivate()
		{
			base.OnDeactivate();
			MPLobbyVM lobbyDataSource = this._lobbyDataSource;
			if (lobbyDataSource == null)
			{
				return;
			}
			lobbyDataSource.OnDeactivate();
		}

		private void OnOpenFacegen(BasicCharacterObject character)
		{
			this._isFacegenOpen = true;
			this._playerCharacter = character;
			LoadingWindow.EnableGlobalLoadingWindow();
			ScreenManager.PushScreen(ViewCreator.CreateMBFaceGeneratorScreen(character, true, null));
		}

		private void OnForceCloseFacegen()
		{
			if (this._isFacegenOpen)
			{
				this.OnFacegenClosed(false);
				ScreenManager.PopScreen();
			}
		}

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

		private string GetContinueKeyText()
		{
			if (Input.IsGamepadActive)
			{
				GameTexts.SetVariable("CONSOLE_KEY_NAME", GameKeyTextExtensions.GetHotKeyGameText(Game.Current.GameTextManager, "GenericPanelGameKeyCategory", "Exit"));
				return GameTexts.FindText("str_click_to_exit_console", null).ToString();
			}
			return GameTexts.FindText("str_click_to_exit", null).ToString();
		}

		private void SetNavigationRestriction(bool isRestricted)
		{
			if (this._isNavigationRestricted != isRestricted)
			{
				this._isNavigationRestricted = isRestricted;
			}
		}

		protected override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
			this.TickInternal(dt);
		}

		private void TickInternal(float dt)
		{
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
					this.HandleInput(dt);
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

		private void HandleInput(float dt)
		{
			bool flag = this._lobbyLayer.Input.IsHotKeyPressed("Confirm");
			bool flag2 = this._lobbyLayer.Input.IsHotKeyPressed("ToggleFriendsList");
			if (flag || flag2)
			{
				if (this._lobbyDataSource.Login.IsEnabled && flag)
				{
					this._lobbyDataSource.Login.ExecuteLogin();
					UISoundsHelper.PlayUISound("event:/ui/default");
					return;
				}
				if (this._lobbyDataSource.Options.IsEnabled && flag)
				{
					this._lobbyDataSource.Options.ExecuteApply();
					UISoundsHelper.PlayUISound("event:/ui/default");
					return;
				}
				if (!this._lobbyDataSource.HasNoPopupOpen() && flag)
				{
					this._lobbyDataSource.OnConfirm();
					UISoundsHelper.PlayUISound("event:/ui/default");
					return;
				}
				if (flag2)
				{
					UISoundsHelper.PlayUISound("event:/ui/default");
					this._lobbyDataSource.Friends.IsListEnabled = !this._lobbyDataSource.Friends.IsListEnabled;
					this._lobbyDataSource.ForceCloseContextMenus();
					return;
				}
			}
			else
			{
				if (this._lobbyLayer.Input.IsHotKeyReleased("Exit"))
				{
					UISoundsHelper.PlayUISound("event:/ui/sort");
					this._lobbyDataSource.OnEscape();
					return;
				}
				if (this._lobbyLayer.Input.IsHotKeyPressed("TakeAll"))
				{
					if (this._lobbyDataSource.Armory.IsEnabled)
					{
						if (Input.IsGamepadActive && this._lobbyDataSource.Armory.IsManagingTaunts)
						{
							this._lobbyDataSource.Armory.ExecuteEmptyFocusedSlot();
							return;
						}
					}
					else if (this._lobbyDataSource.Options.IsEnabled)
					{
						UISoundsHelper.PlayUISound("event:/ui/default");
						this._lobbyDataSource.Options.SelectPreviousCategory();
						return;
					}
				}
				else if (this._lobbyLayer.Input.IsHotKeyPressed("GiveAll"))
				{
					if (this._lobbyDataSource.Armory.IsEnabled)
					{
						if (Input.IsGamepadActive && this._lobbyDataSource.Armory.IsManagingTaunts)
						{
							this._lobbyDataSource.Armory.ExecuteSelectFocusedSlot();
							return;
						}
					}
					else if (this._lobbyDataSource.Options.IsEnabled)
					{
						UISoundsHelper.PlayUISound("event:/ui/default");
						this._lobbyDataSource.Options.SelectNextCategory();
						return;
					}
				}
				else if (this._lobbyLayer.Input.IsHotKeyPressed("SwitchToPreviousTab"))
				{
					if (!this._isNavigationRestricted)
					{
						this.SelectPreviousPage(MPLobbyVM.LobbyPage.NotAssigned);
						return;
					}
				}
				else if (this._lobbyLayer.Input.IsHotKeyPressed("SwitchToNextTab"))
				{
					if (!this._isNavigationRestricted)
					{
						this.SelectNextPage(MPLobbyVM.LobbyPage.NotAssigned);
						return;
					}
				}
				else if (this._lobbyLayer.Input.IsHotKeyReleased("Reset"))
				{
					if (this._lobbyDataSource.HasNoPopupOpen() && this._lobbyDataSource.Options.IsEnabled)
					{
						UISoundsHelper.PlayUISound("event:/ui/default");
						this._lobbyDataSource.Options.ExecuteCancel();
						return;
					}
					if (this._lobbyDataSource.Matchmaking.CustomServer.IsEnabled && !this._lobbyDataSource.Matchmaking.CustomServer.HostGame.IsEnabled)
					{
						this._lobbyDataSource.Matchmaking.CustomServer.ExecuteRefresh();
					}
				}
			}
		}

		private void ShowNextFeedback()
		{
			KeyValuePair<string, InquiryData> keyValuePair = this._feedbackInquiries[0];
			this._feedbackInquiries.Remove(keyValuePair);
			this._activeFeedbackId = keyValuePair.Key;
			InformationManager.ShowInquiry(keyValuePair.Value, false, false);
		}

		[Conditional("DEBUG")]
		private void TickDebug(float dt)
		{
		}

		void ILobbyStateHandler.SetConnectionState(bool isAuthenticated)
		{
			if (this._lobbyDataSource == null)
			{
				this.CreateView();
			}
			if (isAuthenticated && this._lobbyState.LobbyClient.PlayerData != null)
			{
				this._lobbyDataSource.RefreshPlayerData(this._lobbyState.LobbyClient.PlayerData);
				if (this._lobbyDataSource.CurrentPage == MPLobbyVM.LobbyPage.NotAssigned || this._lobbyDataSource.CurrentPage == MPLobbyVM.LobbyPage.Authentication)
				{
					this._lobbyDataSource.SetPage(MPLobbyVM.LobbyPage.Home, MPMatchmakingVM.MatchmakingSubPages.Default);
				}
			}
			else
			{
				if (this._isFacegenOpen)
				{
					this.OnForceCloseFacegen();
				}
				this._lobbyDataSource.SetPage(MPLobbyVM.LobbyPage.Authentication, MPMatchmakingVM.MatchmakingSubPages.Default);
			}
			this._lobbyDataSource.ConnectionStateUpdated(isAuthenticated);
		}

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

		void ILobbyStateHandler.OnUpdateFindingGame(MatchmakingWaitTimeStats matchmakingWaitTimeStats, string[] gameTypeInfo)
		{
			MPLobbyVM lobbyDataSource = this._lobbyDataSource;
			if (lobbyDataSource == null)
			{
				return;
			}
			lobbyDataSource.OnUpdateFindingGame(matchmakingWaitTimeStats, gameTypeInfo);
		}

		void ILobbyStateHandler.OnRequestedToCancelSearchBattle()
		{
			MPLobbyVM lobbyDataSource = this._lobbyDataSource;
			if (lobbyDataSource == null)
			{
				return;
			}
			lobbyDataSource.OnRequestedToCancelSearchBattle();
		}

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

		void ILobbyStateHandler.OnPause()
		{
		}

		void ILobbyStateHandler.OnResume()
		{
			MPLobbyVM lobbyDataSource = this._lobbyDataSource;
			if (lobbyDataSource == null)
			{
				return;
			}
			lobbyDataSource.RefreshPlayerData(this._lobbyState.LobbyClient.PlayerData);
		}

		void ILobbyStateHandler.OnDisconnected()
		{
			MPLobbyVM lobbyDataSource = this._lobbyDataSource;
			if (lobbyDataSource == null)
			{
				return;
			}
			lobbyDataSource.OnDisconnected();
		}

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

		void ILobbyStateHandler.OnPendingRejoin()
		{
			this._lobbyDataSource.SetPage(MPLobbyVM.LobbyPage.Rejoin, MPMatchmakingVM.MatchmakingSubPages.Default);
		}

		void ILobbyStateHandler.OnEnterBattleWithParty(string[] selectedGameTypes)
		{
		}

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

		void ILobbyStateHandler.OnPartyJoinRequestReceived(PlayerId joingPlayerId, PlayerId viaPlayerId, string viaPlayerName, bool newParty)
		{
			if (this._lobbyState.LobbyClient.SupportedFeatures.SupportsFeatures(4))
			{
				if (this._lobbyDataSource != null)
				{
					if (newParty)
					{
						this._lobbyDataSource.PartyJoinRequestPopup.OpenWithNewParty(joingPlayerId);
						return;
					}
					this._lobbyDataSource.PartyJoinRequestPopup.OpenWith(joingPlayerId, viaPlayerId, viaPlayerName);
					return;
				}
			}
			else
			{
				this._lobbyState.LobbyClient.DeclinePartyJoinRequest(joingPlayerId, 0);
			}
		}

		void ILobbyStateHandler.OnPartyInvitationInvalidated()
		{
			MPLobbyVM lobbyDataSource = this._lobbyDataSource;
			if (lobbyDataSource == null)
			{
				return;
			}
			lobbyDataSource.PartyInvitationPopup.Close();
		}

		void ILobbyStateHandler.OnPlayerInvitedToParty(PlayerId playerId)
		{
			MPLobbyVM lobbyDataSource = this._lobbyDataSource;
			if (lobbyDataSource == null)
			{
				return;
			}
			lobbyDataSource.Friends.OnPlayerInvitedToParty(playerId);
		}

		void ILobbyStateHandler.OnPlayerAddedToParty(PlayerId playerId, string playerName, bool isPartyLeader)
		{
			MPLobbyVM lobbyDataSource = this._lobbyDataSource;
			if (lobbyDataSource == null)
			{
				return;
			}
			lobbyDataSource.OnPlayerAddedToParty(playerId);
		}

		void ILobbyStateHandler.OnPlayerRemovedFromParty(PlayerId playerId, PartyRemoveReason reason)
		{
			MPLobbyVM lobbyDataSource = this._lobbyDataSource;
			if (lobbyDataSource == null)
			{
				return;
			}
			lobbyDataSource.OnPlayerRemovedFromParty(playerId, reason);
		}

		void ILobbyStateHandler.OnGameClientStateChange(LobbyClient.State state)
		{
		}

		void ILobbyStateHandler.OnAdminMessageReceived(string message)
		{
			InformationManager.AddSystemNotification(message);
		}

		public void OnBattleServerInformationReceived(BattleServerInformationForClient battleServerInformation)
		{
			UISoundsHelper.PlayUISound("event:/ui/multiplayer/match_ready");
			this._lobbyDataSource.Matchmaking.IsFindingMatch = false;
		}

		string ILobbyStateHandler.ShowFeedback(string title, string feedbackText)
		{
			string id = Guid.NewGuid().ToString();
			InquiryData inquiryData = new InquiryData(title, feedbackText, false, true, "", new TextObject("{=dismissnotification}Dismiss", null).ToString(), null, delegate
			{
				((ILobbyStateHandler)this).DismissFeedback(id);
			}, "", 0f, null, null, null);
			this._feedbackInquiries.Add(new KeyValuePair<string, InquiryData>(id, inquiryData));
			return id;
		}

		string ILobbyStateHandler.ShowFeedback(InquiryData inquiryData)
		{
			string text = Guid.NewGuid().ToString();
			this._feedbackInquiries.Add(new KeyValuePair<string, InquiryData>(text, inquiryData));
			return text;
		}

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

		private void SelectPreviousPage(MPLobbyVM.LobbyPage currentPage = MPLobbyVM.LobbyPage.NotAssigned)
		{
			MPLobbyVM lobbyDataSource = this._lobbyDataSource;
			if (lobbyDataSource != null && lobbyDataSource.HasNoPopupOpen())
			{
				if (currentPage == MPLobbyVM.LobbyPage.NotAssigned)
				{
					currentPage = this._lobbyDataSource.CurrentPage;
				}
				if (currentPage < MPLobbyVM.LobbyPage.Options || currentPage > MPLobbyVM.LobbyPage.Profile)
				{
					return;
				}
				MPLobbyVM.LobbyPage lobbyPage = ((currentPage == MPLobbyVM.LobbyPage.Options) ? MPLobbyVM.LobbyPage.Profile : (currentPage - 1));
				if (this._lobbyDataSource.DisallowedPages.Contains(lobbyPage))
				{
					this.SelectPreviousPage(lobbyPage);
					return;
				}
				if (lobbyPage == MPLobbyVM.LobbyPage.Options)
				{
					UISoundsHelper.PlayUISound("event:/ui/checkbox");
				}
				else
				{
					UISoundsHelper.PlayUISound("event:/ui/tab");
				}
				this._lobbyDataSource.SetPage(lobbyPage, MPMatchmakingVM.MatchmakingSubPages.Default);
			}
		}

		private void SelectNextPage(MPLobbyVM.LobbyPage currentPage = MPLobbyVM.LobbyPage.NotAssigned)
		{
			MPLobbyVM lobbyDataSource = this._lobbyDataSource;
			if (lobbyDataSource != null && lobbyDataSource.HasNoPopupOpen())
			{
				if (currentPage == MPLobbyVM.LobbyPage.NotAssigned)
				{
					currentPage = this._lobbyDataSource.CurrentPage;
				}
				if (currentPage < MPLobbyVM.LobbyPage.Options || currentPage > MPLobbyVM.LobbyPage.Profile)
				{
					return;
				}
				MPLobbyVM.LobbyPage lobbyPage = ((currentPage == MPLobbyVM.LobbyPage.Profile) ? MPLobbyVM.LobbyPage.Options : (currentPage + 1));
				if (this._lobbyDataSource.DisallowedPages.Contains(lobbyPage))
				{
					this.SelectNextPage(lobbyPage);
					return;
				}
				if (lobbyPage == MPLobbyVM.LobbyPage.Options)
				{
					UISoundsHelper.PlayUISound("event:/ui/checkbox");
				}
				else
				{
					UISoundsHelper.PlayUISound("event:/ui/tab");
				}
				this._lobbyDataSource.SetPage(lobbyPage, MPMatchmakingVM.MatchmakingSubPages.Default);
			}
		}

		void ILobbyStateHandler.OnActivateCustomServer()
		{
			this._lobbyDataSource.SetPage(MPLobbyVM.LobbyPage.Matchmaking, MPMatchmakingVM.MatchmakingSubPages.CustomGameList);
		}

		void ILobbyStateHandler.OnActivateHome()
		{
			this._lobbyDataSource.SetPage(MPLobbyVM.LobbyPage.Home, MPMatchmakingVM.MatchmakingSubPages.Default);
		}

		void ILobbyStateHandler.OnActivateMatchmaking()
		{
			this._lobbyDataSource.SetPage(MPLobbyVM.LobbyPage.Matchmaking, MPMatchmakingVM.MatchmakingSubPages.Default);
		}

		void ILobbyStateHandler.OnActivateArmory()
		{
			this._lobbyDataSource.SetPage(MPLobbyVM.LobbyPage.Armory, MPMatchmakingVM.MatchmakingSubPages.Default);
		}

		void ILobbyStateHandler.OnActivateProfile()
		{
			this._lobbyDataSource.SetPage(MPLobbyVM.LobbyPage.Profile, MPMatchmakingVM.MatchmakingSubPages.Default);
		}

		void ILobbyStateHandler.OnClanInvitationReceived(string clanName, string clanTag, bool isCreation)
		{
			this._lobbyDataSource.ClanInvitationPopup.Open(clanName, clanTag, isCreation);
		}

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

		void ILobbyStateHandler.OnClanCreationSuccessful()
		{
			MPLobbyVM lobbyDataSource = this._lobbyDataSource;
			if (lobbyDataSource == null)
			{
				return;
			}
			lobbyDataSource.OnClanCreationFinished();
		}

		void ILobbyStateHandler.OnClanCreationFailed()
		{
			MPLobbyVM lobbyDataSource = this._lobbyDataSource;
			if (lobbyDataSource == null)
			{
				return;
			}
			lobbyDataSource.OnClanCreationFinished();
		}

		void ILobbyStateHandler.OnClanCreationStarted()
		{
			this._lobbyDataSource.ClanCreationPopup.ExecuteSwitchToWaiting();
		}

		void ILobbyStateHandler.OnClanInfoChanged()
		{
			MPLobbyVM lobbyDataSource = this._lobbyDataSource;
			if (lobbyDataSource == null)
			{
				return;
			}
			lobbyDataSource.OnClanInfoChanged();
		}

		void ILobbyStateHandler.OnPremadeGameEligibilityStatusReceived(bool isEligible)
		{
			MPLobbyVM lobbyDataSource = this._lobbyDataSource;
			if (lobbyDataSource == null)
			{
				return;
			}
			lobbyDataSource.Matchmaking.OnPremadeGameEligibilityStatusReceived(isEligible);
		}

		void ILobbyStateHandler.OnPremadeGameCreated()
		{
			MPLobbyVM lobbyDataSource = this._lobbyDataSource;
			if (lobbyDataSource == null)
			{
				return;
			}
			lobbyDataSource.OnPremadeGameCreated();
		}

		void ILobbyStateHandler.OnPremadeGameListReceived()
		{
			MPLobbyVM lobbyDataSource = this._lobbyDataSource;
			if (lobbyDataSource == null)
			{
				return;
			}
			lobbyDataSource.Matchmaking.PremadeMatches.RefreshPremadeGameList();
		}

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

		void ILobbyStateHandler.OnJoinPremadeGameRequested(string clanName, string clanSigilCode, Guid partyId, PlayerId[] challengerPlayerIDs, PlayerId challengerPartyLeaderID, PremadeGameType premadeGameType)
		{
			this._lobbyDataSource.ClanMatchmakingRequestPopup.OpenWith(clanName, clanSigilCode, partyId, challengerPlayerIDs, challengerPartyLeaderID, premadeGameType);
		}

		void ILobbyStateHandler.OnJoinPremadeGameRequestSuccessful()
		{
			if (!this._lobbyDataSource.GameSearch.IsEnabled)
			{
				this._lobbyDataSource.OnPremadeGameCreated();
			}
			this._lobbyDataSource.GameSearch.OnJoinPremadeGameRequestSuccessful();
		}

		void ILobbyStateHandler.OnSigilChanged()
		{
			if (this._lobbyDataSource != null)
			{
				this._lobbyDataSource.OnSigilChanged(this._lobbyDataSource.ChangeSigilPopup.SelectedSigil.IconID);
			}
		}

		void ILobbyStateHandler.OnActivateOptions()
		{
			this._lobbyDataSource.SetPage(MPLobbyVM.LobbyPage.Options, MPMatchmakingVM.MatchmakingSubPages.Default);
		}

		void ILobbyStateHandler.OnDeactivateOptions()
		{
			this._lobbyDataSource.SetPage(MPLobbyVM.LobbyPage.Home, MPMatchmakingVM.MatchmakingSubPages.Default);
		}

		void ILobbyStateHandler.OnCustomGameServerListReceived(AvailableCustomGames customGameServerList)
		{
			this._lobbyDataSource.Matchmaking.CustomServer.RefreshCustomGameServerList(customGameServerList);
		}

		void ILobbyStateHandler.OnMatchmakerGameOver(int oldExperience, int newExperience, List<string> badgesEarned, int lootGained, RankBarInfo oldRankBarInfo, RankBarInfo newRankBarInfo)
		{
			MPLobbyVM lobbyDataSource = this._lobbyDataSource;
			if (lobbyDataSource == null)
			{
				return;
			}
			lobbyDataSource.AfterBattlePopup.OpenWith(oldExperience, newExperience, badgesEarned, lootGained, oldRankBarInfo, newRankBarInfo);
		}

		void ILobbyStateHandler.OnBattleServerLost()
		{
			TextObject textObject = new TextObject("{=wLpJEkKY}Battle Server Crashed", null);
			TextObject textObject2 = new TextObject("{=EzeFJo65}You have been disconnected from server!", null);
			this._lobbyDataSource.Popup.ShowMessage(textObject, textObject2);
		}

		void ILobbyStateHandler.OnRemovedFromMatchmakerGame(DisconnectType disconnectType)
		{
			this.ShowDisconnectMessage(disconnectType);
		}

		void ILobbyStateHandler.OnRemovedFromCustomGame(DisconnectType disconnectType)
		{
			this.ShowDisconnectMessage(disconnectType);
		}

		void ILobbyStateHandler.OnPlayerAssignedPartyLeader(PlayerId partyLeaderId)
		{
			MPLobbyVM lobbyDataSource = this._lobbyDataSource;
			if (lobbyDataSource == null)
			{
				return;
			}
			lobbyDataSource.OnPlayerAssignedPartyLeader(partyLeaderId);
		}

		void ILobbyStateHandler.OnPlayerSuggestedToParty(PlayerId playerId, string playerName, PlayerId suggestingPlayerId, string suggestingPlayerName)
		{
			MPLobbyVM lobbyDataSource = this._lobbyDataSource;
			if (lobbyDataSource == null)
			{
				return;
			}
			lobbyDataSource.OnPlayerSuggestedToParty(playerId, playerName, suggestingPlayerId, suggestingPlayerName);
		}

		void ILobbyStateHandler.OnNotificationsReceived(LobbyNotification[] notifications)
		{
			MPLobbyVM lobbyDataSource = this._lobbyDataSource;
			if (lobbyDataSource == null)
			{
				return;
			}
			lobbyDataSource.OnNotificationsReceived(notifications);
		}

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
					InformationManager.AddSystemNotification(new TextObject(serverStatus.Announcement.Text, null).ToString());
					return;
				}
				if (serverStatus.Announcement.Type == null)
				{
					InformationManager.DisplayMessage(new InformationMessage(new TextObject(serverStatus.Announcement.Text, null).ToString()));
				}
			}
		}

		void ILobbyStateHandler.OnRejoinBattleRequestAnswered(bool isSuccessful)
		{
			MPLobbyVM lobbyDataSource = this._lobbyDataSource;
			if (lobbyDataSource == null)
			{
				return;
			}
			lobbyDataSource.OnRejoinBattleRequestAnswered(isSuccessful);
		}

		void ILobbyStateHandler.OnFriendListUpdated()
		{
			MPLobbyVM lobbyDataSource = this._lobbyDataSource;
			if (lobbyDataSource == null)
			{
				return;
			}
			lobbyDataSource.OnFriendListUpdated(false);
		}

		void ILobbyStateHandler.OnPlayerNameUpdated(string playerName)
		{
			MPLobbyVM lobbyDataSource = this._lobbyDataSource;
			if (lobbyDataSource == null)
			{
				return;
			}
			lobbyDataSource.OnPlayerNameUpdated(playerName);
		}

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

		public MPLobbyVM DataSource
		{
			get
			{
				return this._lobbyDataSource;
			}
		}

		public GauntletLayer LobbyLayer
		{
			get
			{
				return this._lobbyLayer;
			}
		}

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

		private void OnKeybindRequest(KeyOptionVM requestedHotKeyToChange)
		{
			this._currentKey = requestedHotKeyToChange;
			this._keybindingPopup.OnToggle(true);
		}

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

		void IGameStateListener.OnActivate()
		{
			this._musicSoundEvent = SoundEvent.CreateEventFromString("event:/multiplayer/lobby_music", null);
			this._musicSoundEvent.Play();
			if (this._lobbyDataSource == null)
			{
				this.CreateView();
				this._lobbyDataSource.SetPage(MPLobbyVM.LobbyPage.Authentication, MPMatchmakingVM.MatchmakingSubPages.Default);
				return;
			}
			this._spriteCategory = UIResourceManager.SpriteData.SpriteCategories["ui_options"];
			this._spriteCategory.Load(UIResourceManager.ResourceContext, UIResourceManager.UIResourceDepot);
		}

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

		void IGameStateListener.OnInitialize()
		{
		}

		void IGameStateListener.OnFinalize()
		{
		}

		void IGauntletChatLogHandlerScreen.TryUpdateChatLogLayerParameters(ref bool isTeamChatAvailable, ref bool inputEnabled, ref InputContext inputContext)
		{
			if (this.LobbyLayer != null)
			{
				inputEnabled = true;
				inputContext = this.LobbyLayer.Input;
			}
		}

		private List<KeyValuePair<string, InquiryData>> _feedbackInquiries;

		private string _activeFeedbackId;

		private KeybindingPopup _keybindingPopup;

		private KeyOptionVM _currentKey;

		private SpriteCategory _spriteCategory;

		private GauntletLayer _gauntletBrightnessLayer;

		private BrightnessOptionVM _brightnessOptionDataSource;

		private IGauntletMovie _brightnessOptionMovie;

		private LobbyState _lobbyState;

		private BasicCharacterObject _playerCharacter;

		private bool _isFacegenOpen;

		private SoundEvent _musicSoundEvent;

		private bool _isNavigationRestricted;

		private MPCustomGameSortControllerVM.CustomServerSortOption? _cachedCustomServerSortOption;

		private MPCustomGameSortControllerVM.CustomServerSortOption? _cachedPremadeGameSortOption;

		private bool _isLobbyActive;

		private GauntletLayer _lobbyLayer;

		private MPLobbyVM _lobbyDataSource;

		private SpriteCategory _mplobbyCategory;

		private SpriteCategory _bannerIconsCategory;

		private SpriteCategory _badgesCategory;
	}
}
