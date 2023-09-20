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
	[OverrideView(typeof(MapEventVisualsView))]
	public class GauntletMapEventVisualsView : MapView, IGauntletMapEventVisualHandler
	{
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

		protected override void OnMapScreenUpdate(float dt)
		{
			base.OnMapScreenUpdate(dt);
			this._dataSource.Update(dt);
		}

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

		private Vec3 GetRealPositionOfMapEvent(Vec2 mapEventPosition)
		{
			float num = 0f;
			((MapScene)Campaign.Current.MapSceneWrapper).Scene.GetHeightAtPoint(mapEventPosition, 2208137, ref num);
			return new Vec3(mapEventPosition.x, mapEventPosition.y, num, -1f);
		}

		void IGauntletMapEventVisualHandler.OnNewEventStarted(GauntletMapEventVisual newEvent)
		{
			this._dataSource.OnMapEventStarted(newEvent.MapEvent);
		}

		void IGauntletMapEventVisualHandler.OnInitialized(GauntletMapEventVisual newEvent)
		{
			this._dataSource.OnMapEventStarted(newEvent.MapEvent);
		}

		void IGauntletMapEventVisualHandler.OnEventEnded(GauntletMapEventVisual newEvent)
		{
			this._dataSource.OnMapEventEnded(newEvent.MapEvent);
		}

		void IGauntletMapEventVisualHandler.OnEventVisibilityChanged(GauntletMapEventVisual visibilityChangedEvent)
		{
			this._dataSource.OnMapEventVisibilityChanged(visibilityChangedEvent.MapEvent);
		}

		private GauntletLayer _layerAsGauntletLayer;

		private IGauntletMovie _movie;

		private MapEventVisualsVM _dataSource;
	}
}
