using System;
using SandBox.View.Map;
using SandBox.ViewModelCollection.Map;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View;

namespace SandBox.GauntletUI.Map
{
	// Token: 0x0200002C RID: 44
	[OverrideView(typeof(MapMobilePartyTrackerView))]
	public class GauntletMapMobilePartyTrackerView : MapView
	{
		// Token: 0x06000199 RID: 409 RVA: 0x0000BCAC File Offset: 0x00009EAC
		protected override void CreateLayout()
		{
			base.CreateLayout();
			this._dataSource = new MapMobilePartyTrackerVM(base.MapScreen._mapCameraView.Camera, new Action<Vec2>(base.MapScreen.FastMoveCameraToPosition));
			GauntletMapBasicView mapView = base.MapScreen.GetMapView<GauntletMapBasicView>();
			base.Layer = mapView.GauntletNameplateLayer;
			this._layerAsGauntletLayer = base.Layer as GauntletLayer;
			this._movie = this._layerAsGauntletLayer.LoadMovie("MapMobilePartyTracker", this._dataSource);
		}

		// Token: 0x0600019A RID: 410 RVA: 0x0000BD30 File Offset: 0x00009F30
		protected override void OnResume()
		{
			base.OnResume();
			this._dataSource.UpdateProperties();
		}

		// Token: 0x0600019B RID: 411 RVA: 0x0000BD43 File Offset: 0x00009F43
		protected override void OnMapScreenUpdate(float dt)
		{
			base.OnMapScreenUpdate(dt);
			this._dataSource.Update();
		}

		// Token: 0x0600019C RID: 412 RVA: 0x0000BD57 File Offset: 0x00009F57
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

		// Token: 0x040000D3 RID: 211
		private GauntletLayer _layerAsGauntletLayer;

		// Token: 0x040000D4 RID: 212
		private IGauntletMovie _movie;

		// Token: 0x040000D5 RID: 213
		private MapMobilePartyTrackerVM _dataSource;
	}
}
