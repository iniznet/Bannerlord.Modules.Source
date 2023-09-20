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
	public class MenuViewContext : IMenuContextHandler
	{
		internal GameMenu CurGameMenu
		{
			get
			{
				return this._menuContext.GameMenu;
			}
		}

		public MenuContext MenuContext
		{
			get
			{
				return this._menuContext;
			}
		}

		public List<MenuView> MenuViews { get; private set; }

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

		public void AddLayer(ScreenLayer layer)
		{
			this._screen.AddLayer(layer);
		}

		public void RemoveLayer(ScreenLayer layer)
		{
			this._screen.RemoveLayer(layer);
		}

		public T FindLayer<T>() where T : ScreenLayer
		{
			return this._screen.FindLayer<T>();
		}

		public T FindLayer<T>(string name) where T : ScreenLayer
		{
			return this._screen.FindLayer<T>(name);
		}

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

		public void OnResume()
		{
			for (int i = 0; i < this.MenuViews.Count; i++)
			{
				this.MenuViews[i].OnResume();
			}
		}

		public void OnHourlyTick()
		{
			for (int i = 0; i < this.MenuViews.Count; i++)
			{
				this.MenuViews[i].OnHourlyTick();
			}
		}

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

		public void OnDeactivate()
		{
			this.StopAllSounds();
		}

		public void OnInitialize()
		{
		}

		public void OnFinalize()
		{
			this.ClearMenuViews();
			MBInformationManager.HideInformations();
			this._menuContext = null;
		}

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

		void IMenuContextHandler.OnAmbientSoundIDSet(string ambientSoundID)
		{
			this.PlayAmbientSound(ambientSoundID);
		}

		void IMenuContextHandler.OnPanelSoundIDSet(string panelSoundID)
		{
			this.PlayPanelSound(panelSoundID);
		}

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

		void IMenuContextHandler.OnMenuActivate()
		{
			foreach (MenuView menuView in this.MenuViews)
			{
				menuView.OnActivate();
			}
		}

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

		public void OnGameStateDeactivate()
		{
		}

		public void OnGameStateInitialize()
		{
		}

		public void OnGameStateFinalize()
		{
		}

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

		public void CloseCharacterDeveloper()
		{
			this.RemoveMenuView(this._menuCharacterDeveloper);
			this._menuCharacterDeveloper = null;
			foreach (MenuView menuView in this.MenuViews)
			{
				menuView.OnCharacterDeveloperClosed();
			}
		}

		public MenuView AddMenuView<T>(params object[] parameters) where T : MenuView, new()
		{
			MenuView menuView = SandBoxViewCreator.CreateMenuView<T>(parameters);
			menuView.MenuViewContext = this;
			menuView.MenuContext = this._menuContext;
			this.MenuViews.Add(menuView);
			menuView.OnInitialize();
			return menuView;
		}

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

		void IMenuContextHandler.OnBackgroundMeshNameSet(string name)
		{
			foreach (MenuView menuView in this.MenuViews)
			{
				menuView.OnBackgroundMeshNameSet(name);
			}
		}

		void IMenuContextHandler.OnOpenTownManagement()
		{
			if (this._menuTownManagement == null)
			{
				this._menuTownManagement = this.AddMenuView<MenuTownManagementView>(Array.Empty<object>());
			}
		}

		public void CloseTownManagement()
		{
			this.RemoveMenuView(this._menuTownManagement);
			this._menuTownManagement = null;
		}

		void IMenuContextHandler.OnOpenRecruitVolunteers()
		{
			if (this._menuRecruitVolunteers == null)
			{
				this._menuRecruitVolunteers = this.AddMenuView<MenuRecruitVolunteersView>(Array.Empty<object>());
			}
		}

		public void CloseRecruitVolunteers()
		{
			this.RemoveMenuView(this._menuRecruitVolunteers);
			this._menuRecruitVolunteers = null;
		}

		void IMenuContextHandler.OnOpenTournamentLeaderboard()
		{
			if (this._menuTournamentLeaderboard == null)
			{
				this._menuTournamentLeaderboard = this.AddMenuView<MenuTournamentLeaderboardView>(Array.Empty<object>());
			}
		}

		public void CloseTournamentLeaderboard()
		{
			this.RemoveMenuView(this._menuTournamentLeaderboard);
			this._menuTournamentLeaderboard = null;
		}

		void IMenuContextHandler.OnOpenTroopSelection(TroopRoster fullRoster, TroopRoster initialSelections, Func<CharacterObject, bool> canChangeStatusOfTroop, Action<TroopRoster> onDone, int maxSelectableTroopCount, int minSelectableTroopCount)
		{
			if (this._menuTroopSelection == null)
			{
				this._menuTroopSelection = this.AddMenuView<MenuTroopSelectionView>(new object[] { fullRoster, initialSelections, canChangeStatusOfTroop, onDone, maxSelectableTroopCount, minSelectableTroopCount });
			}
		}

		public void CloseTroopSelection()
		{
			this.RemoveMenuView(this._menuTroopSelection);
			this._menuTroopSelection = null;
		}

		private MenuContext _menuContext;

		private MenuView _currentMenuBase;

		private MenuView _currentMenuBackground;

		private MenuView _menuCharacterDeveloper;

		private MenuView _menuOverlayBase;

		private MenuView _menuRecruitVolunteers;

		private MenuView _menuTournamentLeaderboard;

		private MenuView _menuTroopSelection;

		private MenuView _menuTownManagement;

		private SoundEvent _panelSound;

		private SoundEvent _ambientSound;

		private GameOverlays.MenuOverlayType _currentOverlayType;

		private ScreenBase _screen;
	}
}
