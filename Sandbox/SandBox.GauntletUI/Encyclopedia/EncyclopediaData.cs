using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Encyclopedia;
using TaleWorlds.CampaignSystem.Encyclopedia.Pages;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.List;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Pages;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.ScreenSystem;

namespace SandBox.GauntletUI.Encyclopedia
{
	public class EncyclopediaData
	{
		public EncyclopediaData(GauntletMapEncyclopediaView manager, ScreenBase screen, EncyclopediaHomeVM homeDatasource, EncyclopediaNavigatorVM navigatorDatasource)
		{
			this._manager = manager;
			this._screen = screen;
			this._pages = new Dictionary<string, EncyclopediaPage>();
			foreach (EncyclopediaPage encyclopediaPage in Campaign.Current.EncyclopediaManager.GetEncyclopediaPages())
			{
				foreach (string text in encyclopediaPage.GetIdentifierNames())
				{
					if (!this._pages.ContainsKey(text))
					{
						this._pages.Add(text, encyclopediaPage);
					}
				}
			}
			this._homeDatasource = homeDatasource;
			this._lists = new Dictionary<EncyclopediaPage, EncyclopediaListVM>();
			foreach (EncyclopediaPage encyclopediaPage2 in Campaign.Current.EncyclopediaManager.GetEncyclopediaPages())
			{
				if (!this._lists.ContainsKey(encyclopediaPage2))
				{
					EncyclopediaListVM encyclopediaListVM = new EncyclopediaListVM(new EncyclopediaPageArgs(encyclopediaPage2));
					this._manager.ListViewDataController.LoadListData(encyclopediaListVM);
					this._lists.Add(encyclopediaPage2, encyclopediaListVM);
				}
			}
			this._navigatorDatasource = navigatorDatasource;
			this._navigatorDatasource.SetCancelInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Exit"));
			this._navigatorDatasource.SetPreviousPageInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("SwitchToPreviousTab"));
			this._navigatorDatasource.SetNextPageInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("SwitchToNextTab"));
			Game.Current.EventManager.RegisterEvent<TutorialContextChangedEvent>(new Action<TutorialContextChangedEvent>(this.OnTutorialContextChanged));
		}

		private void OnTutorialContextChanged(TutorialContextChangedEvent obj)
		{
			if (obj.NewContext != 9)
			{
				this._prevContext = obj.NewContext;
			}
		}

		internal void OnTick()
		{
			this._navigatorDatasource.CanSwitchTabs = !Input.IsGamepadActive || !InformationManager.GetIsAnyTooltipActiveAndExtended();
			if (this._activeGauntletLayer.Input.IsHotKeyDownAndReleased("Exit") || (this._activeGauntletLayer.Input.IsGameKeyDownAndReleased(39) && !this._activeGauntletLayer.IsFocusedOnInput()))
			{
				if (this._navigatorDatasource.IsSearchResultsShown)
				{
					this._navigatorDatasource.SearchText = string.Empty;
				}
				else
				{
					this._manager.CloseEncyclopedia();
					UISoundsHelper.PlayUISound("event:/ui/default");
				}
			}
			else if (!this._activeGauntletLayer.IsFocusedOnInput() && this._navigatorDatasource.CanSwitchTabs)
			{
				if ((Input.IsKeyPressed(14) && this._navigatorDatasource.IsBackEnabled) || this._activeGauntletLayer.Input.IsHotKeyReleased("SwitchToPreviousTab"))
				{
					this._navigatorDatasource.ExecuteBack();
				}
				else if (this._activeGauntletLayer.Input.IsHotKeyReleased("SwitchToNextTab"))
				{
					this._navigatorDatasource.ExecuteForward();
				}
			}
			if (this._activeGauntletLayer != null)
			{
				object initialState = this._initialState;
				Game game = Game.Current;
				object obj;
				if (game == null)
				{
					obj = null;
				}
				else
				{
					GameStateManager gameStateManager = game.GameStateManager;
					obj = ((gameStateManager != null) ? gameStateManager.ActiveState : null);
				}
				if (initialState != obj)
				{
					this._manager.CloseEncyclopedia();
				}
			}
			EncyclopediaPageVM activeDatasource = this._activeDatasource;
			if (activeDatasource == null)
			{
				return;
			}
			activeDatasource.OnTick();
		}

		private void SetEncyclopediaPage(string pageId, object obj)
		{
			GauntletLayer activeGauntletLayer = this._activeGauntletLayer;
			if (this._activeGauntletLayer != null && this._activeGauntletMovie != null)
			{
				this._activeGauntletLayer.ReleaseMovie(this._activeGauntletMovie);
			}
			EncyclopediaListVM encyclopediaListVM;
			if ((encyclopediaListVM = this._activeDatasource as EncyclopediaListVM) != null)
			{
				EncyclopediaListItemVM encyclopediaListItemVM = encyclopediaListVM.Items.FirstOrDefault((EncyclopediaListItemVM x) => x.Object == obj);
				this._manager.ListViewDataController.SaveListData(encyclopediaListVM, (encyclopediaListItemVM != null) ? encyclopediaListItemVM.Id : encyclopediaListVM.LastSelectedItemId);
			}
			if (this._activeGauntletLayer == null)
			{
				this._activeGauntletLayer = new GauntletLayer(310, "GauntletLayer", false);
				this._navigatorActiveGauntletMovie = this._activeGauntletLayer.LoadMovie("EncyclopediaBar", this._navigatorDatasource);
				this._navigatorDatasource.PageName = this._homeDatasource.GetName();
				this._activeGauntletLayer.IsFocusLayer = true;
				ScreenManager.TrySetFocus(this._activeGauntletLayer);
				this._activeGauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
				this._activeGauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericCampaignPanelsGameKeyCategory"));
				Game.Current.GameStateManager.RegisterActiveStateDisableRequest(this);
				Game.Current.EventManager.TriggerEvent<TutorialContextChangedEvent>(new TutorialContextChangedEvent(9));
				this._initialState = Game.Current.GameStateManager.ActiveState;
			}
			if (pageId == "Home")
			{
				this._activeGauntletMovie = this._activeGauntletLayer.LoadMovie("EncyclopediaHome", this._homeDatasource);
				this._homeGauntletMovie = this._activeGauntletMovie;
				this._activeDatasource = this._homeDatasource;
				this._activeDatasource.Refresh();
				Game.Current.EventManager.TriggerEvent<EncyclopediaPageChangedEvent>(new EncyclopediaPageChangedEvent(1, false));
			}
			else if (pageId == "ListPage")
			{
				EncyclopediaPage encyclopediaPage = obj as EncyclopediaPage;
				this._activeDatasource = this._lists[encyclopediaPage];
				this._activeGauntletMovie = this._activeGauntletLayer.LoadMovie("EncyclopediaItemList", this._activeDatasource);
				this._activeDatasource.Refresh();
				this._manager.ListViewDataController.LoadListData(this._activeDatasource as EncyclopediaListVM);
				this.SetTutorialListPageContext(encyclopediaPage);
			}
			else
			{
				EncyclopediaPage encyclopediaPage2 = this._pages[pageId];
				this._activeDatasource = this.GetEncyclopediaPageInstance(encyclopediaPage2, obj);
				EncyclopediaContentPageVM encyclopediaContentPageVM = this._activeDatasource as EncyclopediaContentPageVM;
				if (encyclopediaContentPageVM != null)
				{
					encyclopediaContentPageVM.InitializeQuickNavigation(this._lists[encyclopediaPage2]);
				}
				this._activeGauntletMovie = this._activeGauntletLayer.LoadMovie(this._pages[pageId].GetViewFullyQualifiedName(), this._activeDatasource);
				this.SetTutorialPageContext(this._activeDatasource);
			}
			this._navigatorDatasource.NavBarString = this._activeDatasource.GetNavigationBarURL();
			if (activeGauntletLayer != null && activeGauntletLayer != this._activeGauntletLayer)
			{
				this._screen.RemoveLayer(activeGauntletLayer);
				this._screen.AddLayer(this._activeGauntletLayer);
			}
			else if (activeGauntletLayer == null && this._activeGauntletLayer != null)
			{
				this._screen.AddLayer(this._activeGauntletLayer);
			}
			this._activeGauntletLayer.InputRestrictions.SetInputRestrictions(true, 7);
			this._previousPageID = pageId;
		}

		internal EncyclopediaPageVM ExecuteLink(string pageId, object obj, bool needsRefresh)
		{
			this.SetEncyclopediaPage(pageId, obj);
			return this._activeDatasource;
		}

		private EncyclopediaPageVM GetEncyclopediaPageInstance(EncyclopediaPage page, object o)
		{
			EncyclopediaPageArgs encyclopediaPageArgs;
			encyclopediaPageArgs..ctor(o);
			foreach (Type type in Extensions.GetTypesSafe(typeof(EncyclopediaHomeVM).Assembly, null))
			{
				if (typeof(EncyclopediaPageVM).IsAssignableFrom(type))
				{
					object[] customAttributes = type.GetCustomAttributes(typeof(EncyclopediaViewModel), false);
					for (int i = 0; i < customAttributes.Length; i++)
					{
						EncyclopediaViewModel encyclopediaViewModel;
						if ((encyclopediaViewModel = customAttributes[i] as EncyclopediaViewModel) != null && page.HasIdentifierType(encyclopediaViewModel.PageTargetType))
						{
							return Activator.CreateInstance(type, new object[] { encyclopediaPageArgs }) as EncyclopediaPageVM;
						}
					}
				}
			}
			return null;
		}

		public void OnFinalize()
		{
			Game.Current.GameStateManager.UnregisterActiveStateDisableRequest(this);
			this._pages = null;
			this._homeDatasource = null;
			this._lists = null;
			this._activeGauntletMovie = null;
			this._activeDatasource = null;
			this._activeGauntletLayer = null;
			this._navigatorActiveGauntletMovie = null;
			this._navigatorDatasource = null;
			this._initialState = null;
			Game.Current.EventManager.UnregisterEvent<TutorialContextChangedEvent>(new Action<TutorialContextChangedEvent>(this.OnTutorialContextChanged));
		}

		public void CloseEncyclopedia()
		{
			EncyclopediaListVM encyclopediaListVM;
			if ((encyclopediaListVM = this._activeDatasource as EncyclopediaListVM) != null)
			{
				this._manager.ListViewDataController.SaveListData(encyclopediaListVM, encyclopediaListVM.LastSelectedItemId);
			}
			this.ResetPageFilters();
			this._activeGauntletLayer.ReleaseMovie(this._activeGauntletMovie);
			this._screen.RemoveLayer(this._activeGauntletLayer);
			this._activeGauntletLayer.InputRestrictions.ResetInputRestrictions();
			this.OnFinalize();
			Game.Current.EventManager.TriggerEvent<EncyclopediaPageChangedEvent>(new EncyclopediaPageChangedEvent(0, false));
			Game.Current.EventManager.TriggerEvent<TutorialContextChangedEvent>(new TutorialContextChangedEvent(this._prevContext));
		}

		private void ResetPageFilters()
		{
			foreach (EncyclopediaListVM encyclopediaListVM in this._lists.Values)
			{
				foreach (EncyclopediaFilterGroupVM encyclopediaFilterGroupVM in encyclopediaListVM.FilterGroups)
				{
					foreach (EncyclopediaListFilterVM encyclopediaListFilterVM in encyclopediaFilterGroupVM.Filters)
					{
						encyclopediaListFilterVM.IsSelected = false;
					}
				}
			}
		}

		private void SetTutorialPageContext(EncyclopediaPageVM _page)
		{
			if (_page is EncyclopediaClanPageVM)
			{
				Game.Current.EventManager.TriggerEvent<EncyclopediaPageChangedEvent>(new EncyclopediaPageChangedEvent(8, false));
				return;
			}
			if (_page is EncyclopediaConceptPageVM)
			{
				Game.Current.EventManager.TriggerEvent<EncyclopediaPageChangedEvent>(new EncyclopediaPageChangedEvent(13, false));
				return;
			}
			if (_page is EncyclopediaFactionPageVM)
			{
				Game.Current.EventManager.TriggerEvent<EncyclopediaPageChangedEvent>(new EncyclopediaPageChangedEvent(11, false));
				return;
			}
			if (_page is EncyclopediaUnitPageVM)
			{
				Game.Current.EventManager.TriggerEvent<EncyclopediaPageChangedEvent>(new EncyclopediaPageChangedEvent(10, false));
				return;
			}
			if (_page is EncyclopediaHeroPageVM)
			{
				Game.Current.EventManager.TriggerEvent<EncyclopediaPageChangedEvent>(new EncyclopediaPageChangedEvent(12, false));
				return;
			}
			if (_page is EncyclopediaSettlementPageVM)
			{
				Game.Current.EventManager.TriggerEvent<EncyclopediaPageChangedEvent>(new EncyclopediaPageChangedEvent(9, false));
			}
		}

		private void SetTutorialListPageContext(EncyclopediaPage _page)
		{
			if (_page is DefaultEncyclopediaClanPage)
			{
				Game.Current.EventManager.TriggerEvent<EncyclopediaPageChangedEvent>(new EncyclopediaPageChangedEvent(4, false));
				return;
			}
			if (_page is DefaultEncyclopediaConceptPage)
			{
				Game.Current.EventManager.TriggerEvent<EncyclopediaPageChangedEvent>(new EncyclopediaPageChangedEvent(7, false));
				return;
			}
			if (_page is DefaultEncyclopediaFactionPage)
			{
				Game.Current.EventManager.TriggerEvent<EncyclopediaPageChangedEvent>(new EncyclopediaPageChangedEvent(5, false));
				return;
			}
			if (_page is DefaultEncyclopediaUnitPage)
			{
				Game.Current.EventManager.TriggerEvent<EncyclopediaPageChangedEvent>(new EncyclopediaPageChangedEvent(3, false));
				return;
			}
			if (_page is DefaultEncyclopediaHeroPage)
			{
				Game.Current.EventManager.TriggerEvent<EncyclopediaPageChangedEvent>(new EncyclopediaPageChangedEvent(6, false));
				return;
			}
			if (_page is DefaultEncyclopediaSettlementPage)
			{
				Game.Current.EventManager.TriggerEvent<EncyclopediaPageChangedEvent>(new EncyclopediaPageChangedEvent(2, false));
			}
		}

		private Dictionary<string, EncyclopediaPage> _pages;

		private string _previousPageID;

		private EncyclopediaHomeVM _homeDatasource;

		private IGauntletMovie _homeGauntletMovie;

		private Dictionary<EncyclopediaPage, EncyclopediaListVM> _lists;

		private EncyclopediaPageVM _activeDatasource;

		private GauntletLayer _activeGauntletLayer;

		private IGauntletMovie _activeGauntletMovie;

		private EncyclopediaNavigatorVM _navigatorDatasource;

		private IGauntletMovie _navigatorActiveGauntletMovie;

		private readonly ScreenBase _screen;

		private TutorialContexts _prevContext;

		private readonly GauntletMapEncyclopediaView _manager;

		private object _initialState;
	}
}
