using System;
using SandBox.View.Map;
using SandBox.ViewModelCollection.Map;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View;

namespace SandBox.GauntletUI.Map
{
	[OverrideView(typeof(MapMobilePartyTrackerView))]
	public class GauntletMapMobilePartyTrackerView : MapView
	{
		protected override void CreateLayout()
		{
			base.CreateLayout();
			this._dataSource = new MapMobilePartyTrackerVM(base.MapScreen._mapCameraView.Camera, new Action<Vec2>(base.MapScreen.FastMoveCameraToPosition));
			GauntletMapBasicView mapView = base.MapScreen.GetMapView<GauntletMapBasicView>();
			base.Layer = mapView.GauntletNameplateLayer;
			this._layerAsGauntletLayer = base.Layer as GauntletLayer;
			this._movie = this._layerAsGauntletLayer.LoadMovie("MapMobilePartyTracker", this._dataSource);
		}

		protected override void OnResume()
		{
			base.OnResume();
			this._dataSource.UpdateProperties();
		}

		protected override void OnMapScreenUpdate(float dt)
		{
			base.OnMapScreenUpdate(dt);
			this._dataSource.Update();
		}

		protected override void OnFinalize()
		{
			this._dataSource.OnFinalize();
			this._layerAsGauntletLayer.ReleaseMovie(this._movie);
			this._layerAsGauntletLayer = null;
			base.Layer = null;
			this._movie = null;
			this._dataSource = null;
			base.OnFinalize();
		}

		private GauntletLayer _layerAsGauntletLayer;

		private IGauntletMovie _movie;

		private MapMobilePartyTrackerVM _dataSource;
	}
}
