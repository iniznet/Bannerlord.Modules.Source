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
using TaleWorlds.ScreenSystem;
using TaleWorlds.TwoDimension;

namespace SandBox.GauntletUI.Map
{
	// Token: 0x02000020 RID: 32
	public class GauntletMapBarGlobalLayer : GlobalLayer
	{
		// Token: 0x06000138 RID: 312 RVA: 0x000099B0 File Offset: 0x00007BB0
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

		// Token: 0x06000139 RID: 313 RVA: 0x00009A8C File Offset: 0x00007C8C
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

		// Token: 0x0600013A RID: 314 RVA: 0x00009AFF File Offset: 0x00007CFF
		public void Refresh()
		{
			MapBarVM mapDataSource = this._mapDataSource;
			if (mapDataSource == null)
			{
				return;
			}
			mapDataSource.OnRefresh();
		}

		// Token: 0x0600013B RID: 315 RVA: 0x00009B14 File Offset: 0x00007D14
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

		// Token: 0x0600013C RID: 316 RVA: 0x00009C84 File Offset: 0x00007E84
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

		// Token: 0x0600013D RID: 317 RVA: 0x00009EBC File Offset: 0x000080BC
		private void HandleArmyManagementInput()
		{
			if (this._armyManagementLayer.Input.IsHotKeyReleased("Exit"))
			{
				this._armyManagementVM.ExecuteCancel();
				return;
			}
			if (this._armyManagementLayer.Input.IsHotKeyReleased("Confirm"))
			{
				this._armyManagementVM.ExecuteDone();
				return;
			}
			if (this._armyManagementLayer.Input.IsHotKeyReleased("Reset"))
			{
				this._armyManagementVM.ExecuteReset();
				return;
			}
			if (this._armyManagementLayer.Input.IsHotKeyReleased("RemoveParty") && this._armyManagementVM.FocusedItem != null)
			{
				this._armyManagementVM.FocusedItem.ExecuteAction();
			}
		}

		// Token: 0x0600013E RID: 318 RVA: 0x00009F68 File Offset: 0x00008168
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

		// Token: 0x0600013F RID: 319 RVA: 0x0000A070 File Offset: 0x00008270
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

		// Token: 0x06000140 RID: 320 RVA: 0x0000A260 File Offset: 0x00008460
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

		// Token: 0x06000141 RID: 321 RVA: 0x0000A2FF File Offset: 0x000084FF
		internal bool IsEscaped()
		{
			if (this._armyManagementVM != null)
			{
				this._armyManagementVM.ExecuteCancel();
				return true;
			}
			return false;
		}

		// Token: 0x06000142 RID: 322 RVA: 0x0000A317 File Offset: 0x00008517
		internal void OnMapConversationStart()
		{
			this._contextAlphaTarget = 0f;
		}

		// Token: 0x06000143 RID: 323 RVA: 0x0000A324 File Offset: 0x00008524
		internal void OnMapConversationEnd()
		{
			this._contextAlphaTarget = 1f;
		}

		// Token: 0x04000092 RID: 146
		private MapBarVM _mapDataSource;

		// Token: 0x04000093 RID: 147
		private GauntletLayer _gauntletLayer;

		// Token: 0x04000094 RID: 148
		private IGauntletMovie _movie;

		// Token: 0x04000095 RID: 149
		private SpriteCategory _mapBarCategory;

		// Token: 0x04000096 RID: 150
		private MapScreen _mapScreen;

		// Token: 0x04000097 RID: 151
		private MapNavigationHandler _mapNavigationHandler;

		// Token: 0x04000098 RID: 152
		private INavigationHandler _mapNavigationHandlerAsInterface;

		// Token: 0x04000099 RID: 153
		private MapEncyclopediaView _encyclopediaManager;

		// Token: 0x0400009A RID: 154
		private float _contextAlphaTarget = 1f;

		// Token: 0x0400009B RID: 155
		private float _contextAlphaModifider;

		// Token: 0x0400009C RID: 156
		private GauntletLayer _armyManagementLayer;

		// Token: 0x0400009D RID: 157
		private SpriteCategory _armyManagementCategory;

		// Token: 0x0400009E RID: 158
		private ArmyManagementVM _armyManagementVM;

		// Token: 0x0400009F RID: 159
		private IGauntletMovie _gauntletArmyManagementMovie;

		// Token: 0x040000A0 RID: 160
		private CampaignTimeControlMode _timeControlModeBeforeArmyManagementOpened;
	}
}
