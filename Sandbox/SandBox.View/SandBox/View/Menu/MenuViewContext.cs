using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Overlay;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.ScreenSystem;

namespace SandBox.View.Menu
{
	// Token: 0x02000036 RID: 54
	public class MenuViewContext : IMenuContextHandler
	{
		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000192 RID: 402 RVA: 0x00011703 File Offset: 0x0000F903
		internal GameMenu CurGameMenu
		{
			get
			{
				return this._menuContext.GameMenu;
			}
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000193 RID: 403 RVA: 0x00011710 File Offset: 0x0000F910
		public MenuContext MenuContext
		{
			get
			{
				return this._menuContext;
			}
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000194 RID: 404 RVA: 0x00011718 File Offset: 0x0000F918
		// (set) Token: 0x06000195 RID: 405 RVA: 0x00011720 File Offset: 0x0000F920
		public List<MenuView> MenuViews { get; private set; }

		// Token: 0x06000196 RID: 406 RVA: 0x0001172C File Offset: 0x0000F92C
		public MenuViewContext(ScreenBase screen, MenuContext menuContext)
		{
			this._screen = screen;
			this._menuContext = menuContext;
			this.MenuViews = new List<MenuView>();
			this._menuContext.Handler = this;
			if (Campaign.Current.GameMode != 2 && this.CurGameMenu.StringId != "siege_test_menu")
			{
				this.OnMenuCreate();
				this.OnMenuActivate();
			}
		}

		// Token: 0x06000197 RID: 407 RVA: 0x00011794 File Offset: 0x0000F994
		public void UpdateMenuContext(MenuContext menuContext)
		{
			this._menuContext = menuContext;
			this._menuContext.Handler = this;
			this.MenuViews.ForEach(delegate(MenuView m)
			{
				m.MenuContext = menuContext;
			});
			this.MenuViews.ForEach(delegate(MenuView m)
			{
				m.OnMenuContextUpdated(menuContext);
			});
			this.CheckAndInitializeOverlay();
		}

		// Token: 0x06000198 RID: 408 RVA: 0x000117FA File Offset: 0x0000F9FA
		public void AddLayer(ScreenLayer layer)
		{
			this._screen.AddLayer(layer);
		}

		// Token: 0x06000199 RID: 409 RVA: 0x00011808 File Offset: 0x0000FA08
		public void RemoveLayer(ScreenLayer layer)
		{
			this._screen.RemoveLayer(layer);
		}

		// Token: 0x0600019A RID: 410 RVA: 0x00011816 File Offset: 0x0000FA16
		public T FindLayer<T>() where T : ScreenLayer
		{
			return this._screen.FindLayer<T>();
		}

		// Token: 0x0600019B RID: 411 RVA: 0x00011823 File Offset: 0x0000FA23
		public T FindLayer<T>(string name) where T : ScreenLayer
		{
			return this._screen.FindLayer<T>(name);
		}

		// Token: 0x0600019C RID: 412 RVA: 0x00011834 File Offset: 0x0000FA34
		public void OnFrameTick(float dt)
		{
			for (int i = 0; i < this.MenuViews.Count; i++)
			{
				MenuView menuView = this.MenuViews[i];
				menuView.OnFrameTick(dt);
				if (menuView.Removed)
				{
					i--;
				}
			}
		}

		// Token: 0x0600019D RID: 413 RVA: 0x00011878 File Offset: 0x0000FA78
		public void OnResume()
		{
			for (int i = 0; i < this.MenuViews.Count; i++)
			{
				this.MenuViews[i].OnResume();
			}
		}

		// Token: 0x0600019E RID: 414 RVA: 0x000118AC File Offset: 0x0000FAAC
		public void OnHourlyTick()
		{
			for (int i = 0; i < this.MenuViews.Count; i++)
			{
				this.MenuViews[i].OnHourlyTick();
			}
		}

		// Token: 0x0600019F RID: 415 RVA: 0x000118E0 File Offset: 0x0000FAE0
		public void OnActivate()
		{
			MenuContext menuContext = this.MenuContext;
			if (!string.IsNullOrEmpty((menuContext != null) ? menuContext.CurrentAmbientSoundID : null))
			{
				this.PlayAmbientSound(this.MenuContext.CurrentAmbientSoundID);
			}
			MenuContext menuContext2 = this.MenuContext;
			if (!string.IsNullOrEmpty((menuContext2 != null) ? menuContext2.CurrentPanelSoundID : null))
			{
				this.PlayPanelSound(this.MenuContext.CurrentPanelSoundID);
			}
		}

		// Token: 0x060001A0 RID: 416 RVA: 0x00011941 File Offset: 0x0000FB41
		public void OnDeactivate()
		{
			this.StopAllSounds();
		}

		// Token: 0x060001A1 RID: 417 RVA: 0x00011949 File Offset: 0x0000FB49
		public void OnInitialize()
		{
		}

		// Token: 0x060001A2 RID: 418 RVA: 0x0001194B File Offset: 0x0000FB4B
		public void OnFinalize()
		{
			this.ClearMenuViews();
			MBInformationManager.HideInformations();
			this._menuContext = null;
		}

		// Token: 0x060001A3 RID: 419 RVA: 0x00011960 File Offset: 0x0000FB60
		private void ClearMenuViews()
		{
			foreach (MenuView menuView in this.MenuViews.ToArray())
			{
				this.RemoveMenuView(menuView);
			}
			this._menuCharacterDeveloper = null;
			this._menuOverlayBase = null;
			this._menuRecruitVolunteers = null;
			this._menuTownManagement = null;
			this._menuTroopSelection = null;
		}

		// Token: 0x060001A4 RID: 420 RVA: 0x000119B5 File Offset: 0x0000FBB5
		public void StopAllSounds()
		{
			SoundEvent ambientSound = this._ambientSound;
			if (ambientSound != null)
			{
				ambientSound.Release();
			}
			SoundEvent panelSound = this._panelSound;
			if (panelSound == null)
			{
				return;
			}
			panelSound.Release();
		}

		// Token: 0x060001A5 RID: 421 RVA: 0x000119D8 File Offset: 0x0000FBD8
		private void PlayAmbientSound(string ambientSoundID)
		{
			SoundEvent ambientSound = this._ambientSound;
			if (ambientSound != null)
			{
				ambientSound.Release();
			}
			this._ambientSound = SoundEvent.CreateEventFromString(ambientSoundID, null);
			this._ambientSound.Play();
		}

		// Token: 0x060001A6 RID: 422 RVA: 0x00011A04 File Offset: 0x0000FC04
		private void PlayPanelSound(string panelSoundID)
		{
			SoundEvent panelSound = this._panelSound;
			if (panelSound != null)
			{
				panelSound.Release();
			}
			this._panelSound = SoundEvent.CreateEventFromString(panelSoundID, null);
			this._panelSound.Play();
		}

		// Token: 0x060001A7 RID: 423 RVA: 0x00011A30 File Offset: 0x0000FC30
		void IMenuContextHandler.OnAmbientSoundIDSet(string ambientSoundID)
		{
			this.PlayAmbientSound(ambientSoundID);
		}

		// Token: 0x060001A8 RID: 424 RVA: 0x00011A39 File Offset: 0x0000FC39
		void IMenuContextHandler.OnPanelSoundIDSet(string panelSoundID)
		{
			this.PlayPanelSound(panelSoundID);
		}

		// Token: 0x060001A9 RID: 425 RVA: 0x00011A44 File Offset: 0x0000FC44
		void IMenuContextHandler.OnMenuCreate()
		{
			bool flag = Campaign.Current.GameMode == 2 || this.CurGameMenu.StringId == "siege_test_menu";
			if (flag && this._currentMenuBackground == null)
			{
				this._currentMenuBackground = this.AddMenuView<MenuBackgroundView>(Array.Empty<object>());
			}
			if (this._currentMenuBase == null)
			{
				this._currentMenuBase = this.AddMenuView<MenuBaseView>(Array.Empty<object>());
			}
			if (!flag)
			{
				this.CheckAndInitializeOverlay();
			}
		}

		// Token: 0x060001AA RID: 426 RVA: 0x00011AB4 File Offset: 0x0000FCB4
		void IMenuContextHandler.OnMenuActivate()
		{
			foreach (MenuView menuView in this.MenuViews)
			{
				menuView.OnActivate();
			}
		}

		// Token: 0x060001AB RID: 427 RVA: 0x00011B04 File Offset: 0x0000FD04
		public void OnMapConversationActivated()
		{
			for (int i = 0; i < this.MenuViews.Count; i++)
			{
				MenuView menuView = this.MenuViews[i];
				menuView.OnMapConversationActivated();
				if (menuView.Removed)
				{
					i--;
				}
			}
		}

		// Token: 0x060001AC RID: 428 RVA: 0x00011B44 File Offset: 0x0000FD44
		public void OnMapConversationDeactivated()
		{
			for (int i = 0; i < this.MenuViews.Count; i++)
			{
				MenuView menuView = this.MenuViews[i];
				menuView.OnMapConversationDeactivated();
				if (menuView.Removed)
				{
					i--;
				}
			}
		}

		// Token: 0x060001AD RID: 429 RVA: 0x00011B84 File Offset: 0x0000FD84
		public void OnGameStateDeactivate()
		{
		}

		// Token: 0x060001AE RID: 430 RVA: 0x00011B86 File Offset: 0x0000FD86
		public void OnGameStateInitialize()
		{
		}

		// Token: 0x060001AF RID: 431 RVA: 0x00011B88 File Offset: 0x0000FD88
		public void OnGameStateFinalize()
		{
		}

		// Token: 0x060001B0 RID: 432 RVA: 0x00011B8C File Offset: 0x0000FD8C
		private void CheckAndInitializeOverlay()
		{
			GameOverlays.MenuOverlayType menuOverlayType = Campaign.Current.GameMenuManager.GetMenuOverlayType(this._menuContext);
			if (menuOverlayType != null)
			{
				if (menuOverlayType != this._currentOverlayType)
				{
					if (this._menuOverlayBase != null && ((this._currentOverlayType != 4 && menuOverlayType == 4) || (this._currentOverlayType == 4 && (menuOverlayType == 3 || menuOverlayType == 2 || menuOverlayType == 1))))
					{
						this.RemoveMenuView(this._menuOverlayBase);
						this._menuOverlayBase = null;
					}
					if (this._menuOverlayBase == null)
					{
						this._menuOverlayBase = this.AddMenuView<MenuOverlayBaseView>(Array.Empty<object>());
					}
					else
					{
						this._menuOverlayBase.OnOverlayTypeChange(menuOverlayType);
					}
				}
				else
				{
					MenuView menuOverlayBase = this._menuOverlayBase;
					if (menuOverlayBase != null)
					{
						menuOverlayBase.OnOverlayTypeChange(menuOverlayType);
					}
				}
			}
			else
			{
				if (this._menuOverlayBase != null)
				{
					this.RemoveMenuView(this._menuOverlayBase);
					this._menuOverlayBase = null;
				}
				if (this._currentMenuBackground != null)
				{
					this.RemoveMenuView(this._currentMenuBackground);
					this._currentMenuBackground = null;
				}
			}
			this._currentOverlayType = menuOverlayType;
		}

		// Token: 0x060001B1 RID: 433 RVA: 0x00011C78 File Offset: 0x0000FE78
		public void CloseCharacterDeveloper()
		{
			this.RemoveMenuView(this._menuCharacterDeveloper);
			this._menuCharacterDeveloper = null;
			foreach (MenuView menuView in this.MenuViews)
			{
				menuView.OnCharacterDeveloperClosed();
			}
		}

		// Token: 0x060001B2 RID: 434 RVA: 0x00011CDC File Offset: 0x0000FEDC
		public MenuView AddMenuView<T>(params object[] parameters) where T : MenuView, new()
		{
			MenuView menuView = SandBoxViewCreator.CreateMenuView<T>(parameters);
			menuView.MenuViewContext = this;
			menuView.MenuContext = this._menuContext;
			this.MenuViews.Add(menuView);
			menuView.OnInitialize();
			return menuView;
		}

		// Token: 0x060001B3 RID: 435 RVA: 0x00011D18 File Offset: 0x0000FF18
		public T GetMenuView<T>() where T : MenuView
		{
			foreach (MenuView menuView in this.MenuViews)
			{
				T t = menuView as T;
				if (t != null)
				{
					return t;
				}
			}
			return default(T);
		}

		// Token: 0x060001B4 RID: 436 RVA: 0x00011D88 File Offset: 0x0000FF88
		public void RemoveMenuView(MenuView menuView)
		{
			menuView.OnFinalize();
			menuView.Removed = true;
			this.MenuViews.Remove(menuView);
			if (menuView.ShouldUpdateMenuAfterRemoved)
			{
				this.MenuViews.ForEach(delegate(MenuView m)
				{
					m.OnMenuContextUpdated(this._menuContext);
				});
			}
		}

		// Token: 0x060001B5 RID: 437 RVA: 0x00011DC4 File Offset: 0x0000FFC4
		void IMenuContextHandler.OnBackgroundMeshNameSet(string name)
		{
			foreach (MenuView menuView in this.MenuViews)
			{
				menuView.OnBackgroundMeshNameSet(name);
			}
		}

		// Token: 0x060001B6 RID: 438 RVA: 0x00011E18 File Offset: 0x00010018
		void IMenuContextHandler.OnOpenTownManagement()
		{
			if (this._menuTownManagement == null)
			{
				this._menuTownManagement = this.AddMenuView<MenuTownManagementView>(Array.Empty<object>());
			}
		}

		// Token: 0x060001B7 RID: 439 RVA: 0x00011E33 File Offset: 0x00010033
		public void CloseTownManagement()
		{
			this.RemoveMenuView(this._menuTownManagement);
			this._menuTownManagement = null;
		}

		// Token: 0x060001B8 RID: 440 RVA: 0x00011E48 File Offset: 0x00010048
		void IMenuContextHandler.OnOpenRecruitVolunteers()
		{
			if (this._menuRecruitVolunteers == null)
			{
				this._menuRecruitVolunteers = this.AddMenuView<MenuRecruitVolunteersView>(Array.Empty<object>());
			}
		}

		// Token: 0x060001B9 RID: 441 RVA: 0x00011E63 File Offset: 0x00010063
		public void CloseRecruitVolunteers()
		{
			this.RemoveMenuView(this._menuRecruitVolunteers);
			this._menuRecruitVolunteers = null;
		}

		// Token: 0x060001BA RID: 442 RVA: 0x00011E78 File Offset: 0x00010078
		void IMenuContextHandler.OnOpenTournamentLeaderboard()
		{
			if (this._menuTournamentLeaderboard == null)
			{
				this._menuTournamentLeaderboard = this.AddMenuView<MenuTournamentLeaderboardView>(Array.Empty<object>());
			}
		}

		// Token: 0x060001BB RID: 443 RVA: 0x00011E93 File Offset: 0x00010093
		public void CloseTournamentLeaderboard()
		{
			this.RemoveMenuView(this._menuTournamentLeaderboard);
			this._menuTournamentLeaderboard = null;
		}

		// Token: 0x060001BC RID: 444 RVA: 0x00011EA8 File Offset: 0x000100A8
		void IMenuContextHandler.OnOpenTroopSelection(TroopRoster fullRoster, TroopRoster initialSelections, Func<CharacterObject, bool> canChangeStatusOfTroop, Action<TroopRoster> onDone, int maxSelectableTroopCount, int minSelectableTroopCount)
		{
			if (this._menuTroopSelection == null)
			{
				this._menuTroopSelection = this.AddMenuView<MenuTroopSelectionView>(new object[] { fullRoster, initialSelections, canChangeStatusOfTroop, onDone, maxSelectableTroopCount, minSelectableTroopCount });
			}
		}

		// Token: 0x060001BD RID: 445 RVA: 0x00011EF4 File Offset: 0x000100F4
		public void CloseTroopSelection()
		{
			this.RemoveMenuView(this._menuTroopSelection);
			this._menuTroopSelection = null;
		}

		// Token: 0x040000EC RID: 236
		private MenuContext _menuContext;

		// Token: 0x040000ED RID: 237
		private MenuView _currentMenuBase;

		// Token: 0x040000EE RID: 238
		private MenuView _currentMenuBackground;

		// Token: 0x040000EF RID: 239
		private MenuView _menuCharacterDeveloper;

		// Token: 0x040000F0 RID: 240
		private MenuView _menuOverlayBase;

		// Token: 0x040000F1 RID: 241
		private MenuView _menuRecruitVolunteers;

		// Token: 0x040000F2 RID: 242
		private MenuView _menuTournamentLeaderboard;

		// Token: 0x040000F3 RID: 243
		private MenuView _menuTroopSelection;

		// Token: 0x040000F4 RID: 244
		private MenuView _menuTownManagement;

		// Token: 0x040000F5 RID: 245
		private SoundEvent _panelSound;

		// Token: 0x040000F6 RID: 246
		private SoundEvent _ambientSound;

		// Token: 0x040000F7 RID: 247
		private GameOverlays.MenuOverlayType _currentOverlayType;

		// Token: 0x040000F9 RID: 249
		private ScreenBase _screen;
	}
}
