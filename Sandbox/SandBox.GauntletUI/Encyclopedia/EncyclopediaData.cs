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
using TaleWorlds.ScreenSystem;

namespace SandBox.GauntletUI.Encyclopedia
{
	// Token: 0x02000034 RID: 52
	public class EncyclopediaData
	{
		// Token: 0x060001D4 RID: 468 RVA: 0x0000CF50 File Offset: 0x0000B150
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
			this._navigatorDatasource.SetPreviousPageInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("SwitchToPreviousTab"));
			this._navigatorDatasource.SetNextPageInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("SwitchToNextTab"));
			Game.Current.EventManager.RegisterEvent<TutorialContextChangedEvent>(new Action<TutorialContextChangedEvent>(this.OnTutorialContextChanged));
		}

		// Token: 0x060001D5 RID: 469 RVA: 0x0000D0E8 File Offset: 0x0000B2E8
		private void OnTutorialContextChanged(TutorialContextChangedEvent obj)
		{
			if (obj.NewContext != 9)
			{
				this._prevContext = obj.NewContext;
			}
		}

		// Token: 0x060001D6 RID: 470 RVA: 0x0000D100 File Offset: 0x0000B300
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

		// Token: 0x060001D7 RID: 471 RVA: 0x0000D250 File Offset: 0x0000B450
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

		// Token: 0x060001D8 RID: 472 RVA: 0x0000D583 File Offset: 0x0000B783
		internal EncyclopediaPageVM ExecuteLink(string pageId, object obj, bool needsRefresh)
		{
			this.SetEncyclopediaPage(pageId, obj);
			return this._activeDatasource;
		}

		// Token: 0x060001D9 RID: 473 RVA: 0x0000D594 File Offset: 0x0000B794
		private EncyclopediaPageVM GetEncyclopediaPageInstance(EncyclopediaPage page, object o)
		{
			EncyclopediaPageArgs encyclopediaPageArgs;
			encyclopediaPageArgs..ctor(o);
			foreach (Type type in typeof(EncyclopediaHomeVM).Assembly.GetTypes())
			{
				if (typeof(EncyclopediaPageVM).IsAssignableFrom(type))
				{
					object[] customAttributes = type.GetCustomAttributes(typeof(EncyclopediaViewModel), false);
					for (int j = 0; j < customAttributes.Length; j++)
					{
						EncyclopediaViewModel encyclopediaViewModel;
						if ((encyclopediaViewModel = customAttributes[j] as EncyclopediaViewModel) != null && page.HasIdentifierType(encyclopediaViewModel.PageTargetType))
						{
							return Activator.CreateInstance(type, new object[] { encyclopediaPageArgs }) as EncyclopediaPageVM;
						}
					}
				}
			}
			return null;
		}

		// Token: 0x060001DA RID: 474 RVA: 0x0000D644 File Offset: 0x0000B844
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

		// Token: 0x060001DB RID: 475 RVA: 0x0000D6BC File Offset: 0x0000B8BC
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

		// Token: 0x060001DC RID: 476 RVA: 0x0000D760 File Offset: 0x0000B960
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

		// Token: 0x060001DD RID: 477 RVA: 0x0000D81C File Offset: 0x0000BA1C
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

		// Token: 0x060001DE RID: 478 RVA: 0x0000D8E8 File Offset: 0x0000BAE8
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

		// Token: 0x040000F0 RID: 240
		private Dictionary<string, EncyclopediaPage> _pages;

		// Token: 0x040000F1 RID: 241
		private string _previousPageID;

		// Token: 0x040000F2 RID: 242
		private EncyclopediaHomeVM _homeDatasource;

		// Token: 0x040000F3 RID: 243
		private IGauntletMovie _homeGauntletMovie;

		// Token: 0x040000F4 RID: 244
		private Dictionary<EncyclopediaPage, EncyclopediaListVM> _lists;

		// Token: 0x040000F5 RID: 245
		private EncyclopediaPageVM _activeDatasource;

		// Token: 0x040000F6 RID: 246
		private GauntletLayer _activeGauntletLayer;

		// Token: 0x040000F7 RID: 247
		private IGauntletMovie _activeGauntletMovie;

		// Token: 0x040000F8 RID: 248
		private EncyclopediaNavigatorVM _navigatorDatasource;

		// Token: 0x040000F9 RID: 249
		private IGauntletMovie _navigatorActiveGauntletMovie;

		// Token: 0x040000FA RID: 250
		private readonly ScreenBase _screen;

		// Token: 0x040000FB RID: 251
		private TutorialContexts _prevContext;

		// Token: 0x040000FC RID: 252
		private readonly GauntletMapEncyclopediaView _manager;

		// Token: 0x040000FD RID: 253
		private object _initialState;
	}
}
