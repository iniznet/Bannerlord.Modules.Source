using System;
using SandBox.View.Map;
using SandBox.ViewModelCollection.MapSiege;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View;

namespace SandBox.GauntletUI.Map
{
	[OverrideView(typeof(MapSiegeOverlayView))]
	public class GauntletMapSiegeOverlayView : MapView
	{
		protected override void CreateLayout()
		{
			base.CreateLayout();
			GauntletMapBasicView mapView = base.MapScreen.GetMapView<GauntletMapBasicView>();
			base.Layer = mapView.GauntletNameplateLayer;
			this._layerAsGauntletLayer = base.Layer as GauntletLayer;
			PartyVisual visualOfParty = PartyVisualManager.Current.GetVisualOfParty(PlayerSiege.PlayerSiegeEvent.BesiegedSettlement.Party);
			this._dataSource = new MapSiegeVM(base.MapScreen._mapCameraView.Camera, visualOfParty.GetAttackerBatteringRamSiegeEngineFrames(), visualOfParty.GetAttackerRangedSiegeEngineFrames(), visualOfParty.GetAttackerTowerSiegeEngineFrames(), visualOfParty.GetDefenderRangedSiegeEngineFrames(), visualOfParty.GetBreachableWallFrames());
			CampaignEvents.SiegeEngineBuiltEvent.AddNonSerializedListener(this, new Action<SiegeEvent, BattleSideEnum, SiegeEngineType>(this.OnSiegeEngineBuilt));
			this._movie = this._layerAsGauntletLayer.LoadMovie("MapSiegeOverlay", this._dataSource);
		}

		protected override void OnMapScreenUpdate(float dt)
		{
			base.OnMapScreenUpdate(dt);
			MapSiegeVM dataSource = this._dataSource;
			if (dataSource == null)
			{
				return;
			}
			dataSource.Update(base.MapScreen._mapCameraView.CameraDistance);
		}

		protected override void OnFinalize()
		{
			this._layerAsGauntletLayer.ReleaseMovie(this._movie);
			this._movie = null;
			this._dataSource = null;
			base.Layer = null;
			this._layerAsGauntletLayer = null;
			CampaignEvents.SiegeEngineBuiltEvent.ClearListeners(this);
			base.OnFinalize();
		}

		protected override void OnSiegeEngineClick(MatrixFrame siegeEngineFrame)
		{
			base.OnSiegeEngineClick(siegeEngineFrame);
			UISoundsHelper.PlayUISound("event:/ui/panels/siege/engine_click");
			MapSiegeVM dataSource = this._dataSource;
			if (dataSource != null && dataSource.ProductionController.IsEnabled && this._dataSource.ProductionController.LatestSelectedPOI.MapSceneLocationFrame.NearlyEquals(siegeEngineFrame, 1E-05f))
			{
				this._dataSource.ProductionController.ExecuteDisable();
				return;
			}
			MapSiegeVM dataSource2 = this._dataSource;
			if (dataSource2 != null)
			{
				dataSource2.OnSelectionFromScene(siegeEngineFrame);
			}
			base.MapState.OnSiegeEngineClick(siegeEngineFrame);
		}

		protected override void OnMapTerrainClick()
		{
			base.OnMapTerrainClick();
			MapSiegeVM dataSource = this._dataSource;
			if (dataSource == null)
			{
				return;
			}
			dataSource.ProductionController.ExecuteDisable();
		}

		private void OnSiegeEngineBuilt(SiegeEvent siegeEvent, BattleSideEnum side, SiegeEngineType siegeEngineType)
		{
			if (siegeEvent.IsPlayerSiegeEvent && side == PlayerSiege.PlayerSide)
			{
				UISoundsHelper.PlayUISound("event:/ui/panels/siege/engine_build_complete");
			}
		}

		private GauntletLayer _layerAsGauntletLayer;

		private MapSiegeVM _dataSource;

		private IGauntletMovie _movie;
	}
}
