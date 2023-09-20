using System;
using SandBox.View.Map;
using SandBox.ViewModelCollection.MapSiege;
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
			this._dataSource = new MapSiegeVM(base.MapScreen._mapCameraView.Camera);
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
			base.OnFinalize();
		}

		protected override void OnSiegeEngineClick(MatrixFrame siegeEngineFrame)
		{
			base.OnSiegeEngineClick(siegeEngineFrame);
			MapSiegeVM dataSource = this._dataSource;
			if (dataSource != null)
			{
				dataSource.OnSelectionFromScene(siegeEngineFrame);
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

		private GauntletLayer _layerAsGauntletLayer;

		private MapSiegeVM _dataSource;

		private IGauntletMovie _movie;
	}
}
