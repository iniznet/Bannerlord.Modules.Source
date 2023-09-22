using System;
using SandBox.View.Map;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.ViewModelCollection.ArmyManagement;
using TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapBar;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.ScreenSystem;
using TaleWorlds.TwoDimension;

namespace SandBox.GauntletUI.Map
{
	public class GauntletMapBarGlobalLayer : GlobalLayer
	{
		public void Initialize(MapScreen mapScreen, float contextAlphaModifider)
		{
			this._mapScreen = mapScreen;
			this._contextAlphaModifider = contextAlphaModifider;
			this._mapNavigationHandler = new MapNavigationHandler();
			this._mapNavigationHandlerAsInterface = this._mapNavigationHandler;
			this._mapDataSource = new MapBarVM(this._mapNavigationHandler, this._mapScreen, new Func<MapBarShortcuts>(this.GetMapBarShortcuts), new Action(this.OpenArmyManagement));
			this._gauntletLayer = new GauntletLayer(202, "GauntletLayer", false);
			base.Layer = this._gauntletLayer;
			SpriteData spriteData = UIResourceManager.SpriteData;
			this._mapBarCategory = spriteData.SpriteCategories["ui_mapbar"];
			this._mapBarCategory.Load(UIResourceManager.ResourceContext, UIResourceManager.UIResourceDepot);
			this._movie = this._gauntletLayer.LoadMovie("MapBar", this._mapDataSource);
			this._encyclopediaManager = mapScreen.EncyclopediaScreenManager;
		}

		public void OnFinalize()
		{
			ArmyManagementVM armyManagementVM = this._armyManagementVM;
			if (armyManagementVM != null)
			{
				armyManagementVM.OnFinalize();
			}
			this._mapDataSource.OnFinalize();
			IGauntletMovie gauntletArmyManagementMovie = this._gauntletArmyManagementMovie;
			if (gauntletArmyManagementMovie != null)
			{
				gauntletArmyManagementMovie.Release();
			}
			this._movie.Release();
			this._mapBarCategory.Unload();
			this._armyManagementVM = null;
			this._gauntletLayer = null;
			this._mapDataSource = null;
			this._encyclopediaManager = null;
			this._mapScreen = null;
		}

		public void Refresh()
		{
			MapBarVM mapDataSource = this._mapDataSource;
			if (mapDataSource == null)
			{
				return;
			}
			mapDataSource.OnRefresh();
		}

		private MapBarShortcuts GetMapBarShortcuts()
		{
			MapBarShortcuts mapBarShortcuts = default(MapBarShortcuts);
			mapBarShortcuts.EscapeMenuHotkey = GameKeyTextExtensions.GetHotKeyGameText(Game.Current.GameTextManager, "GenericPanelGameKeyCategory", "ToggleEscapeMenu").ToString();
			mapBarShortcuts.CharacterHotkey = GameKeyTextExtensions.GetHotKeyGameText(Game.Current.GameTextManager, "GenericCampaignPanelsGameKeyCategory", 37).ToString();
			mapBarShortcuts.QuestHotkey = GameKeyTextExtensions.GetHotKeyGameText(Game.Current.GameTextManager, "GenericCampaignPanelsGameKeyCategory", 42).ToString();
			mapBarShortcuts.PartyHotkey = GameKeyTextExtensions.GetHotKeyGameText(Game.Current.GameTextManager, "GenericCampaignPanelsGameKeyCategory", 43).ToString();
			mapBarShortcuts.KingdomHotkey = GameKeyTextExtensions.GetHotKeyGameText(Game.Current.GameTextManager, "GenericCampaignPanelsGameKeyCategory", 40).ToString();
			mapBarShortcuts.ClanHotkey = GameKeyTextExtensions.GetHotKeyGameText(Game.Current.GameTextManager, "GenericCampaignPanelsGameKeyCategory", 41).ToString();
			mapBarShortcuts.InventoryHotkey = GameKeyTextExtensions.GetHotKeyGameText(Game.Current.GameTextManager, "GenericCampaignPanelsGameKeyCategory", 38).ToString();
			mapBarShortcuts.FastForwardHotkey = GameKeyTextExtensions.GetHotKeyGameText(Game.Current.GameTextManager, "MapHotKeyCategory", 61).ToString();
			mapBarShortcuts.PauseHotkey = GameKeyTextExtensions.GetHotKeyGameText(Game.Current.GameTextManager, "MapHotKeyCategory", 59).ToString();
			mapBarShortcuts.PlayHotkey = GameKeyTextExtensions.GetHotKeyGameText(Game.Current.GameTextManager, "MapHotKeyCategory", 60).ToString();
			return mapBarShortcuts;
		}

		protected override void OnTick(float dt)
		{
			base.OnTick(dt);
			this._gauntletLayer._gauntletUIContext.ContextAlpha = MathF.Lerp(this._gauntletLayer._gauntletUIContext.ContextAlpha, this._contextAlphaTarget, dt * this._contextAlphaModifider, 1E-05f);
			GameState activeState = Game.Current.GameStateManager.ActiveState;
			ScreenBase topScreen = ScreenManager.TopScreen;
			PanelScreenStatus panelScreenStatus = new PanelScreenStatus(topScreen);
			if (this._mapNavigationHandler != null)
			{
				this._mapNavigationHandler.IsNavigationLocked = panelScreenStatus.IsCurrentScreenLocksNavigation;
			}
			if (topScreen is MapScreen || panelScreenStatus.IsAnyPanelScreenOpen)
			{
				this._mapDataSource.IsEnabled = true;
				this._mapDataSource.CurrentScreen = topScreen.GetType().Name;
				bool flag = ScreenManager.TopScreen is MapScreen;
				this._mapDataSource.MapTimeControl.IsInMap = flag;
				base.Layer.InputRestrictions.SetInputRestrictions(false, 7);
				if (!(activeState is MapState))
				{
					this._mapDataSource.MapTimeControl.IsCenterPanelEnabled = false;
					if (panelScreenStatus.IsAnyPanelScreenOpen)
					{
						this.HandlePanelSwitching(panelScreenStatus);
					}
				}
				else
				{
					MapState mapState = (MapState)activeState;
					if (flag)
					{
						MapScreen mapScreen = ScreenManager.TopScreen as MapScreen;
						mapScreen.SetIsBarExtended(this._mapDataSource.MapInfo.IsInfoBarExtended);
						this._mapDataSource.MapTimeControl.IsInRecruitment = mapScreen.IsInRecruitment;
						this._mapDataSource.MapTimeControl.IsInBattleSimulation = mapScreen.IsInBattleSimulation;
						this._mapDataSource.MapTimeControl.IsEncyclopediaOpen = this._encyclopediaManager.IsEncyclopediaOpen;
						this._mapDataSource.MapTimeControl.IsInArmyManagement = mapScreen.IsInArmyManagement;
						this._mapDataSource.MapTimeControl.IsInTownManagement = mapScreen.IsInTownManagement;
						this._mapDataSource.MapTimeControl.IsInHideoutTroopManage = mapScreen.IsInHideoutTroopManage;
						this._mapDataSource.MapTimeControl.IsInCampaignOptions = mapScreen.IsInCampaignOptions;
						this._mapDataSource.MapTimeControl.IsEscapeMenuOpened = mapScreen.IsEscapeMenuOpened;
						this._mapDataSource.MapTimeControl.IsMarriageOfferPopupActive = mapScreen.IsMarriageOfferPopupActive;
						this._mapDataSource.MapTimeControl.IsMapCheatsActive = mapScreen.IsMapCheatsActive;
						if (this._armyManagementVM != null)
						{
							this.HandleArmyManagementInput();
						}
					}
					else
					{
						this._mapDataSource.MapTimeControl.IsCenterPanelEnabled = false;
					}
				}
				this._mapDataSource.Tick(dt);
				return;
			}
			this._mapDataSource.IsEnabled = false;
			base.Layer.InputRestrictions.ResetInputRestrictions();
		}

		private void HandleArmyManagementInput()
		{
			if (this._armyManagementLayer.Input.IsHotKeyReleased("Exit"))
			{
				UISoundsHelper.PlayUISound("event:/ui/default");
				this._armyManagementVM.ExecuteCancel();
				return;
			}
			if (this._armyManagementLayer.Input.IsHotKeyReleased("Confirm"))
			{
				UISoundsHelper.PlayUISound("event:/ui/default");
				this._armyManagementVM.ExecuteDone();
				return;
			}
			if (this._armyManagementLayer.Input.IsHotKeyReleased("Reset"))
			{
				UISoundsHelper.PlayUISound("event:/ui/default");
				this._armyManagementVM.ExecuteReset();
				return;
			}
			if (this._armyManagementLayer.Input.IsHotKeyReleased("RemoveParty") && this._armyManagementVM.FocusedItem != null)
			{
				UISoundsHelper.PlayUISound("event:/ui/default");
				this._armyManagementVM.FocusedItem.ExecuteAction();
			}
		}

		private void HandlePanelSwitching(PanelScreenStatus screenStatus)
		{
			GauntletLayer gauntletLayer = ScreenManager.TopScreen.FindLayer<GauntletLayer>();
			if (((gauntletLayer != null) ? gauntletLayer.Input : null) == null || gauntletLayer.IsFocusedOnInput())
			{
				return;
			}
			InputContext input = gauntletLayer.Input;
			if (input.IsGameKeyReleased(37) && !screenStatus.IsCharacterScreenOpen)
			{
				INavigationHandler mapNavigationHandlerAsInterface = this._mapNavigationHandlerAsInterface;
				if (mapNavigationHandlerAsInterface == null)
				{
					return;
				}
				mapNavigationHandlerAsInterface.OpenCharacterDeveloper();
				return;
			}
			else if (input.IsGameKeyReleased(43) && !screenStatus.IsPartyScreenOpen)
			{
				INavigationHandler mapNavigationHandlerAsInterface2 = this._mapNavigationHandlerAsInterface;
				if (mapNavigationHandlerAsInterface2 == null)
				{
					return;
				}
				mapNavigationHandlerAsInterface2.OpenParty();
				return;
			}
			else if (input.IsGameKeyReleased(42) && !screenStatus.IsQuestsScreenOpen)
			{
				INavigationHandler mapNavigationHandlerAsInterface3 = this._mapNavigationHandlerAsInterface;
				if (mapNavigationHandlerAsInterface3 == null)
				{
					return;
				}
				mapNavigationHandlerAsInterface3.OpenQuests();
				return;
			}
			else if (input.IsGameKeyReleased(38) && !screenStatus.IsInventoryScreenOpen)
			{
				INavigationHandler mapNavigationHandlerAsInterface4 = this._mapNavigationHandlerAsInterface;
				if (mapNavigationHandlerAsInterface4 == null)
				{
					return;
				}
				mapNavigationHandlerAsInterface4.OpenInventory();
				return;
			}
			else
			{
				if (!input.IsGameKeyReleased(41) || screenStatus.IsClanScreenOpen)
				{
					if (input.IsGameKeyReleased(40) && !screenStatus.IsKingdomScreenOpen)
					{
						INavigationHandler mapNavigationHandlerAsInterface5 = this._mapNavigationHandlerAsInterface;
						if (mapNavigationHandlerAsInterface5 == null)
						{
							return;
						}
						mapNavigationHandlerAsInterface5.OpenKingdom();
					}
					return;
				}
				INavigationHandler mapNavigationHandlerAsInterface6 = this._mapNavigationHandlerAsInterface;
				if (mapNavigationHandlerAsInterface6 == null)
				{
					return;
				}
				mapNavigationHandlerAsInterface6.OpenClan();
				return;
			}
		}

		private void OpenArmyManagement()
		{
			if (this._gauntletLayer != null)
			{
				SpriteData spriteData = UIResourceManager.SpriteData;
				TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
				ResourceDepot uiresourceDepot = UIResourceManager.UIResourceDepot;
				this._armyManagementLayer = new GauntletLayer(300, "GauntletLayer", false);
				this._armyManagementCategory = spriteData.SpriteCategories["ui_armymanagement"];
				this._armyManagementCategory.Load(resourceContext, uiresourceDepot);
				this._armyManagementVM = new ArmyManagementVM(new Action(this.CloseArmyManagement));
				this._gauntletArmyManagementMovie = this._armyManagementLayer.LoadMovie("ArmyManagement", this._armyManagementVM);
				this._armyManagementLayer.InputRestrictions.SetInputRestrictions(true, 7);
				this._armyManagementLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("Generic"));
				this._armyManagementLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
				this._armyManagementLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericCampaignPanelsGameKeyCategory"));
				this._armyManagementLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("ArmyManagementHotkeyCategory"));
				this._armyManagementLayer.IsFocusLayer = true;
				ScreenManager.TrySetFocus(this._armyManagementLayer);
				this._mapScreen.AddLayer(this._armyManagementLayer);
				this._armyManagementVM.SetCancelInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Exit"));
				this._armyManagementVM.SetDoneInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Confirm"));
				this._armyManagementVM.SetResetInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Reset"));
				this._armyManagementVM.SetRemoveInputKey(HotKeyManager.GetCategory("ArmyManagementHotkeyCategory").GetHotKey("RemoveParty"));
				this._timeControlModeBeforeArmyManagementOpened = Campaign.Current.TimeControlMode;
				Campaign.Current.TimeControlMode = 0;
				Campaign.Current.SetTimeControlModeLock(true);
				MapScreen mapScreen;
				if ((mapScreen = ScreenManager.TopScreen as MapScreen) != null)
				{
					mapScreen.SetIsInArmyManagement(true);
				}
			}
		}

		private void CloseArmyManagement()
		{
			this._armyManagementVM.OnFinalize();
			this._armyManagementLayer.ReleaseMovie(this._gauntletArmyManagementMovie);
			this._mapScreen.RemoveLayer(this._armyManagementLayer);
			this._armyManagementCategory.Unload();
			Game.Current.EventManager.TriggerEvent<TutorialContextChangedEvent>(new TutorialContextChangedEvent(4));
			this._gauntletArmyManagementMovie = null;
			this._armyManagementVM = null;
			this._armyManagementLayer = null;
			Campaign.Current.SetTimeControlModeLock(false);
			Campaign.Current.TimeControlMode = this._timeControlModeBeforeArmyManagementOpened;
			MapScreen mapScreen;
			if ((mapScreen = ScreenManager.TopScreen as MapScreen) != null)
			{
				mapScreen.SetIsInArmyManagement(false);
			}
		}

		internal bool IsEscaped()
		{
			if (this._armyManagementVM != null)
			{
				this._armyManagementVM.ExecuteCancel();
				return true;
			}
			return false;
		}

		internal void OnMapConversationStart()
		{
			this._contextAlphaTarget = 0f;
		}

		internal void OnMapConversationEnd()
		{
			this._contextAlphaTarget = 1f;
		}

		private MapBarVM _mapDataSource;

		private GauntletLayer _gauntletLayer;

		private IGauntletMovie _movie;

		private SpriteCategory _mapBarCategory;

		private MapScreen _mapScreen;

		private MapNavigationHandler _mapNavigationHandler;

		private INavigationHandler _mapNavigationHandlerAsInterface;

		private MapEncyclopediaView _encyclopediaManager;

		private float _contextAlphaTarget = 1f;

		private float _contextAlphaModifider;

		private GauntletLayer _armyManagementLayer;

		private SpriteCategory _armyManagementCategory;

		private ArmyManagementVM _armyManagementVM;

		private IGauntletMovie _gauntletArmyManagementMovie;

		private CampaignTimeControlMode _timeControlModeBeforeArmyManagementOpened;
	}
}
