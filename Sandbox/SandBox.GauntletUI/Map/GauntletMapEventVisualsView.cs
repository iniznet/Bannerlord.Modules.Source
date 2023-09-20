using System;
using SandBox.View.Map;
using SandBox.ViewModelCollection.Map;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View;

namespace SandBox.GauntletUI.Map
{
	// Token: 0x0200002B RID: 43
	[OverrideView(typeof(MapEventVisualsView))]
	public class GauntletMapEventVisualsView : MapView, IGauntletMapEventVisualHandler
	{
		// Token: 0x06000190 RID: 400 RVA: 0x0000BA94 File Offset: 0x00009C94
		protected override void CreateLayout()
		{
			base.CreateLayout();
			this._dataSource = new MapEventVisualsVM(base.MapScreen._mapCameraView.Camera, new Func<Vec2, Vec3>(this.GetRealPositionOfMapEvent));
			GauntletMapBasicView mapView = base.MapScreen.GetMapView<GauntletMapBasicView>();
			base.Layer = mapView.GauntletNameplateLayer;
			this._layerAsGauntletLayer = base.Layer as GauntletLayer;
			this._movie = this._layerAsGauntletLayer.LoadMovie("MapEventVisuals", this._dataSource);
			GauntletMapEventVisualCreator gauntletMapEventVisualCreator;
			if ((gauntletMapEventVisualCreator = Campaign.Current.VisualCreator.MapEventVisualCreator as GauntletMapEventVisualCreator) != null)
			{
				gauntletMapEventVisualCreator.Handlers.Add(this);
				foreach (GauntletMapEventVisual gauntletMapEventVisual in gauntletMapEventVisualCreator.GetCurrentEvents())
				{
					this._dataSource.OnMapEventStarted(gauntletMapEventVisual.MapEvent);
				}
			}
		}

		// Token: 0x06000191 RID: 401 RVA: 0x0000BB84 File Offset: 0x00009D84
		protected override void OnMapScreenUpdate(float dt)
		{
			base.OnMapScreenUpdate(dt);
			this._dataSource.Update(dt);
		}

		// Token: 0x06000192 RID: 402 RVA: 0x0000BB9C File Offset: 0x00009D9C
		protected override void OnFinalize()
		{
			GauntletMapEventVisualCreator gauntletMapEventVisualCreator;
			if ((gauntletMapEventVisualCreator = Campaign.Current.VisualCreator.MapEventVisualCreator as GauntletMapEventVisualCreator) != null)
			{
				gauntletMapEventVisualCreator.Handlers.Remove(this);
			}
			this._dataSource.OnFinalize();
			this._layerAsGauntletLayer.ReleaseMovie(this._movie);
			base.Layer = null;
			this._layerAsGauntletLayer = null;
			this._movie = null;
			this._dataSource = null;
			base.OnFinalize();
		}

		// Token: 0x06000193 RID: 403 RVA: 0x0000BC0C File Offset: 0x00009E0C
		private Vec3 GetRealPositionOfMapEvent(Vec2 mapEventPosition)
		{
			float num = 0f;
			((MapScene)Campaign.Current.MapSceneWrapper).Scene.GetHeightAtPoint(mapEventPosition, 2208137, ref num);
			return new Vec3(mapEventPosition.x, mapEventPosition.y, num, -1f);
		}

		// Token: 0x06000194 RID: 404 RVA: 0x0000BC58 File Offset: 0x00009E58
		void IGauntletMapEventVisualHandler.OnNewEventStarted(GauntletMapEventVisual newEvent)
		{
			this._dataSource.OnMapEventStarted(newEvent.MapEvent);
		}

		// Token: 0x06000195 RID: 405 RVA: 0x0000BC6B File Offset: 0x00009E6B
		void IGauntletMapEventVisualHandler.OnInitialized(GauntletMapEventVisual newEvent)
		{
			this._dataSource.OnMapEventStarted(newEvent.MapEvent);
		}

		// Token: 0x06000196 RID: 406 RVA: 0x0000BC7E File Offset: 0x00009E7E
		void IGauntletMapEventVisualHandler.OnEventEnded(GauntletMapEventVisual newEvent)
		{
			this._dataSource.OnMapEventEnded(newEvent.MapEvent);
		}

		// Token: 0x06000197 RID: 407 RVA: 0x0000BC91 File Offset: 0x00009E91
		void IGauntletMapEventVisualHandler.OnEventVisibilityChanged(GauntletMapEventVisual visibilityChangedEvent)
		{
			this._dataSource.OnMapEventVisibilityChanged(visibilityChangedEvent.MapEvent);
		}

		// Token: 0x040000D0 RID: 208
		private GauntletLayer _layerAsGauntletLayer;

		// Token: 0x040000D1 RID: 209
		private IGauntletMovie _movie;

		// Token: 0x040000D2 RID: 210
		private MapEventVisualsVM _dataSource;
	}
}
