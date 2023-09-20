using System;
using SandBox.View.Map;
using SandBox.ViewModelCollection.MapSiege;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View;

namespace SandBox.GauntletUI.Map
{
	// Token: 0x02000033 RID: 51
	[OverrideView(typeof(MapSiegeOverlayView))]
	public class GauntletMapSiegeOverlayView : MapView
	{
		// Token: 0x060001CF RID: 463 RVA: 0x0000CE38 File Offset: 0x0000B038
		protected override void CreateLayout()
		{
			base.CreateLayout();
			GauntletMapBasicView mapView = base.MapScreen.GetMapView<GauntletMapBasicView>();
			base.Layer = mapView.GauntletNameplateLayer;
			this._layerAsGauntletLayer = base.Layer as GauntletLayer;
			this._dataSource = new MapSiegeVM(base.MapScreen._mapCameraView.Camera);
			this._movie = this._layerAsGauntletLayer.LoadMovie("MapSiegeOverlay", this._dataSource);
		}

		// Token: 0x060001D0 RID: 464 RVA: 0x0000CEAB File Offset: 0x0000B0AB
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

		// Token: 0x060001D1 RID: 465 RVA: 0x0000CED4 File Offset: 0x0000B0D4
		protected override void OnFinalize()
		{
			this._layerAsGauntletLayer.ReleaseMovie(this._movie);
			this._movie = null;
			this._dataSource = null;
			base.Layer = null;
			this._layerAsGauntletLayer = null;
			base.OnFinalize();
		}

		// Token: 0x060001D2 RID: 466 RVA: 0x0000CF09 File Offset: 0x0000B109
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

		// Token: 0x060001D3 RID: 467 RVA: 0x0000CF30 File Offset: 0x0000B130
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

		// Token: 0x040000ED RID: 237
		private GauntletLayer _layerAsGauntletLayer;

		// Token: 0x040000EE RID: 238
		private MapSiegeVM _dataSource;

		// Token: 0x040000EF RID: 239
		private IGauntletMovie _movie;
	}
}
