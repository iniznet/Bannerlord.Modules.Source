using System;
using System.Linq;
using SandBox.View.Map;
using SandBox.View.Menu;
using TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.TownManagement;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.ScreenSystem;

namespace SandBox.GauntletUI.Menu
{
	[OverrideView(typeof(MenuTownManagementView))]
	public class GauntletMenuTownManagementView : MenuView
	{
		protected override void OnInitialize()
		{
			base.OnInitialize();
			this._dataSource = new TownManagementVM();
			base.Layer = new GauntletLayer(206, "GauntletLayer", false)
			{
				Name = "TownManagementLayer"
			};
			this._layerAsGauntletLayer = base.Layer as GauntletLayer;
			base.Layer.InputRestrictions.SetInputRestrictions(true, 7);
			base.MenuViewContext.AddLayer(base.Layer);
			if (!base.Layer.Input.IsCategoryRegistered(HotKeyManager.GetCategory("GenericPanelGameKeyCategory")))
			{
				base.Layer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
			}
			this._dataSource.SetDoneInputKey(HotKeyManager.GetCategory("GenericPanelGameKeyCategory").GetHotKey("Confirm"));
			this._layerAsGauntletLayer.LoadMovie("TownManagement", this._dataSource);
			base.Layer.IsFocusLayer = true;
			ScreenManager.TrySetFocus(base.Layer);
			this._dataSource.Show = true;
			MapScreen mapScreen;
			if ((mapScreen = ScreenManager.TopScreen as MapScreen) != null)
			{
				mapScreen.SetIsInTownManagement(true);
			}
		}

		protected override void OnFinalize()
		{
			base.OnFinalize();
			base.MenuViewContext.RemoveLayer(base.Layer);
			base.Layer.IsFocusLayer = false;
			ScreenManager.TryLoseFocus(base.Layer);
			MapScreen mapScreen;
			if ((mapScreen = ScreenManager.TopScreen as MapScreen) != null)
			{
				mapScreen.SetIsInTownManagement(false);
			}
			this._dataSource.OnFinalize();
			this._dataSource = null;
			this._layerAsGauntletLayer = null;
			base.Layer = null;
			base.OnFinalize();
		}

		protected override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
			if (base.Layer.Input.IsHotKeyReleased("Confirm") || base.Layer.Input.IsHotKeyReleased("Exit"))
			{
				if (this._dataSource.ReserveControl.IsEnabled)
				{
					this._dataSource.ReserveControl.ExecuteUpdateReserve();
				}
				else
				{
					this._dataSource.ExecuteDone();
				}
			}
			else if (base.Layer.Input.IsHotKeyReleased("Exit"))
			{
				if (this._dataSource.IsSelectingGovernor)
				{
					this._dataSource.IsSelectingGovernor = false;
				}
				else if (this._dataSource.ReserveControl.IsEnabled)
				{
					this._dataSource.ReserveControl.IsEnabled = false;
				}
				else
				{
					SettlementBuildingProjectVM settlementBuildingProjectVM = this._dataSource.ProjectSelection.AvailableProjects.FirstOrDefault((SettlementBuildingProjectVM x) => x.IsSelected);
					if (settlementBuildingProjectVM != null)
					{
						settlementBuildingProjectVM.IsSelected = false;
					}
					else
					{
						this._dataSource.ExecuteDone();
					}
				}
			}
			if (!this._dataSource.Show)
			{
				base.MenuViewContext.CloseTownManagement();
			}
		}

		private GauntletLayer _layerAsGauntletLayer;

		private TownManagementVM _dataSource;
	}
}
