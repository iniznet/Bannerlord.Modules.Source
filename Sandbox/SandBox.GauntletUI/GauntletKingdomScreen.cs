using System;
using SandBox.View.Map;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.ViewModelCollection.ArmyManagement;
using TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement;
using TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement.Decisions.ItemTypes;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.ScreenSystem;
using TaleWorlds.TwoDimension;

namespace SandBox.GauntletUI
{
	// Token: 0x0200000B RID: 11
	[GameStateScreen(typeof(KingdomState))]
	public class GauntletKingdomScreen : ScreenBase, IGameStateListener
	{
		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600007F RID: 127 RVA: 0x00005689 File Offset: 0x00003889
		// (set) Token: 0x06000080 RID: 128 RVA: 0x00005691 File Offset: 0x00003891
		public KingdomManagementVM DataSource { get; private set; }

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000081 RID: 129 RVA: 0x0000569A File Offset: 0x0000389A
		public bool IsMakingDecision
		{
			get
			{
				return this.DataSource.Decision.IsActive;
			}
		}

		// Token: 0x06000082 RID: 130 RVA: 0x000056AC File Offset: 0x000038AC
		public GauntletKingdomScreen(KingdomState kingdomState)
		{
			this._kingdomState = kingdomState;
		}

		// Token: 0x06000083 RID: 131 RVA: 0x000056BC File Offset: 0x000038BC
		protected override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
			LoadingWindow.DisableGlobalLoadingWindow();
			this.DataSource.CanSwitchTabs = !InformationManager.GetIsAnyTooltipActiveAndExtended();
			if (this.DataSource.Decision.IsActive)
			{
				if (this._gauntletLayer.Input.IsHotKeyDownAndReleased("Confirm"))
				{
					DecisionItemBaseVM currentDecision = this.DataSource.Decision.CurrentDecision;
					if (currentDecision != null)
					{
						currentDecision.ExecuteFinalSelection();
					}
				}
			}
			else if (this._armyManagementDatasource != null)
			{
				if (this._armyManagementLayer.Input.IsHotKeyDownAndReleased("Exit"))
				{
					this._armyManagementDatasource.ExecuteCancel();
				}
				else if (this._armyManagementLayer.Input.IsHotKeyDownAndReleased("Confirm"))
				{
					this._armyManagementDatasource.ExecuteDone();
				}
				else if (this._armyManagementLayer.Input.IsHotKeyDownAndReleased("Reset"))
				{
					this._armyManagementDatasource.ExecuteReset();
				}
				else if (this._armyManagementLayer.Input.IsHotKeyReleased("RemoveParty") && this._armyManagementDatasource.FocusedItem != null)
				{
					this._armyManagementDatasource.FocusedItem.ExecuteAction();
				}
			}
			else if (this._gauntletLayer.Input.IsHotKeyReleased("Exit") || this._gauntletLayer.Input.IsGameKeyPressed(40) || this._gauntletLayer.Input.IsHotKeyReleased("Confirm"))
			{
				this.CloseKingdomScreen();
			}
			else if (this.DataSource.CanSwitchTabs)
			{
				if (this._gauntletLayer.Input.IsHotKeyReleased("SwitchToPreviousTab"))
				{
					this.DataSource.SelectPreviousCategory();
				}
				else if (this._gauntletLayer.Input.IsHotKeyReleased("SwitchToNextTab"))
				{
					this.DataSource.SelectNextCategory();
				}
			}
			KingdomManagementVM dataSource = this.DataSource;
			if (dataSource == null)
			{
				return;
			}
			dataSource.OnFrameTick();
		}

		// Token: 0x06000084 RID: 132 RVA: 0x000058A4 File Offset: 0x00003AA4
		void IGameStateListener.OnActivate()
		{
			base.OnActivate();
			SpriteData spriteData = UIResourceManager.SpriteData;
			TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
			ResourceDepot uiresourceDepot = UIResourceManager.UIResourceDepot;
			this._kingdomCategory = spriteData.SpriteCategories["ui_kingdom"];
			this._kingdomCategory.Load(resourceContext, uiresourceDepot);
			this._gauntletLayer = new GauntletLayer(1, "GauntletLayer", true);
			this._gauntletLayer.InputRestrictions.SetInputRestrictions(true, 7);
			this._gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
			this._gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericCampaignPanelsGameKeyCategory"));
			this._gauntletLayer.IsFocusLayer = true;
			ScreenManager.TrySetFocus(this._gauntletLayer);
			base.AddLayer(this._gauntletLayer);
			this.DataSource = new KingdomManagementVM(new Action(this.CloseKingdomScreen), new Action(this.OpenArmyManagement), new Action<Army>(this.ShowArmyOnMap));
			this.DataSource.SetDoneInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Confirm"));
			this.DataSource.SetPreviousTabInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("SwitchToPreviousTab"));
			this.DataSource.SetNextTabInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("SwitchToNextTab"));
			if (this._kingdomState.InitialSelectedArmy != null)
			{
				this.DataSource.SelectArmy(this._kingdomState.InitialSelectedArmy);
			}
			else if (this._kingdomState.InitialSelectedSettlement != null)
			{
				this.DataSource.SelectSettlement(this._kingdomState.InitialSelectedSettlement);
			}
			else if (this._kingdomState.InitialSelectedClan != null)
			{
				this.DataSource.SelectClan(this._kingdomState.InitialSelectedClan);
			}
			else if (this._kingdomState.InitialSelectedPolicy != null)
			{
				this.DataSource.SelectPolicy(this._kingdomState.InitialSelectedPolicy);
			}
			else if (this._kingdomState.InitialSelectedKingdom != null)
			{
				this.DataSource.SelectKingdom(this._kingdomState.InitialSelectedKingdom);
			}
			this._gauntletLayer.LoadMovie("KingdomManagement", this.DataSource);
			Game.Current.EventManager.TriggerEvent<TutorialContextChangedEvent>(new TutorialContextChangedEvent(7));
			SoundEvent.PlaySound2D("event:/ui/panels/panel_kingdom_open");
			this._gauntletLayer._gauntletUIContext.EventManager.GainNavigationAfterFrames(2, null);
		}

		// Token: 0x06000085 RID: 133 RVA: 0x00005AFB File Offset: 0x00003CFB
		void IGameStateListener.OnDeactivate()
		{
			base.OnDeactivate();
			base.RemoveLayer(this._gauntletLayer);
			this._gauntletLayer.IsFocusLayer = false;
			ScreenManager.TryLoseFocus(this._gauntletLayer);
			Game.Current.EventManager.TriggerEvent<TutorialContextChangedEvent>(new TutorialContextChangedEvent(0));
		}

		// Token: 0x06000086 RID: 134 RVA: 0x00005B3B File Offset: 0x00003D3B
		void IGameStateListener.OnInitialize()
		{
		}

		// Token: 0x06000087 RID: 135 RVA: 0x00005B3D File Offset: 0x00003D3D
		void IGameStateListener.OnFinalize()
		{
			this._kingdomCategory.Unload();
			this.DataSource.OnFinalize();
			this.DataSource = null;
			this._gauntletLayer = null;
		}

		// Token: 0x06000088 RID: 136 RVA: 0x00005B64 File Offset: 0x00003D64
		private void ShowArmyOnMap(Army army)
		{
			Vec2 position2D = army.LeaderParty.Position2D;
			this.CloseKingdomScreen();
			MapScreen.Instance.FastMoveCameraToPosition(position2D);
		}

		// Token: 0x06000089 RID: 137 RVA: 0x00005B90 File Offset: 0x00003D90
		private void OpenArmyManagement()
		{
			if (this._gauntletLayer != null)
			{
				this._armyManagementDatasource = new ArmyManagementVM(new Action(this.CloseArmyManagement));
				this._armyManagementDatasource.SetCancelInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Exit"));
				this._armyManagementDatasource.SetDoneInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Confirm"));
				this._armyManagementDatasource.SetResetInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Reset"));
				this._armyManagementDatasource.SetRemoveInputKey(HotKeyManager.GetCategory("ArmyManagementHotkeyCategory").GetHotKey("RemoveParty"));
				SpriteData spriteData = UIResourceManager.SpriteData;
				TwoDimensionEngineResourceContext resourceContext = UIResourceManager.ResourceContext;
				ResourceDepot uiresourceDepot = UIResourceManager.UIResourceDepot;
				this._armyManagementCategory = spriteData.SpriteCategories["ui_armymanagement"];
				this._armyManagementCategory.Load(resourceContext, uiresourceDepot);
				this._armyManagementLayer = new GauntletLayer(2, "GauntletLayer", false);
				this._armyManagementLayer.LoadMovie("ArmyManagement", this._armyManagementDatasource);
				this._armyManagementLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericCampaignPanelsGameKeyCategory"));
				this._armyManagementLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
				this._armyManagementLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("ArmyManagementHotkeyCategory"));
				this._armyManagementLayer.InputRestrictions.SetInputRestrictions(true, 7);
				this._armyManagementLayer.IsFocusLayer = true;
				base.AddLayer(this._armyManagementLayer);
				ScreenManager.TrySetFocus(this._armyManagementLayer);
			}
		}

		// Token: 0x0600008A RID: 138 RVA: 0x00005D1C File Offset: 0x00003F1C
		private void CloseArmyManagement()
		{
			this._armyManagementLayer.InputRestrictions.ResetInputRestrictions();
			this._armyManagementLayer.IsFocusLayer = false;
			ScreenManager.TryLoseFocus(this._armyManagementLayer);
			base.RemoveLayer(this._armyManagementLayer);
			this._armyManagementLayer = null;
			this._armyManagementDatasource.OnFinalize();
			this._armyManagementDatasource = null;
			this._armyManagementCategory.Unload();
			Game.Current.EventManager.TriggerEvent<TutorialContextChangedEvent>(new TutorialContextChangedEvent(7));
			this.DataSource.OnRefresh();
		}

		// Token: 0x0600008B RID: 139 RVA: 0x00005DA0 File Offset: 0x00003FA0
		private void CloseKingdomScreen()
		{
			Game.Current.GameStateManager.PopState(0);
		}

		// Token: 0x0400003E RID: 62
		private const string _panelOpenSound = "event:/ui/panels/panel_kingdom_open";

		// Token: 0x0400003F RID: 63
		private GauntletLayer _gauntletLayer;

		// Token: 0x04000040 RID: 64
		private readonly KingdomState _kingdomState;

		// Token: 0x04000041 RID: 65
		private GauntletLayer _armyManagementLayer;

		// Token: 0x04000042 RID: 66
		private ArmyManagementVM _armyManagementDatasource;

		// Token: 0x04000043 RID: 67
		private SpriteCategory _kingdomCategory;

		// Token: 0x04000044 RID: 68
		private SpriteCategory _armyManagementCategory;
	}
}
