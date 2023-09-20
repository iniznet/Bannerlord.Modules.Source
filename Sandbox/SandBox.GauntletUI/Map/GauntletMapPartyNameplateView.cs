using System;
using SandBox.View.Map;
using SandBox.ViewModelCollection.Nameplate;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.MountAndBlade.View;

namespace SandBox.GauntletUI.Map
{
	[OverrideView(typeof(MapPartyNameplateView))]
	public class GauntletMapPartyNameplateView : MapView
	{
		protected override void CreateLayout()
		{
			base.CreateLayout();
			this._dataSource = new PartyNameplatesVM(base.MapScreen._mapCameraView.Camera, new Action(base.MapScreen.FastMoveCameraToMainParty), new Func<bool>(this.IsShowPartyNamesEnabled));
			GauntletMapBasicView mapView = base.MapScreen.GetMapView<GauntletMapBasicView>();
			base.Layer = mapView.GauntletNameplateLayer;
			this._layerAsGauntletLayer = base.Layer as GauntletLayer;
			this._movie = this._layerAsGauntletLayer.LoadMovie("PartyNameplate", this._dataSource);
			this._dataSource.Initialize();
		}

		protected override void OnMapScreenUpdate(float dt)
		{
			base.OnMapScreenUpdate(dt);
			this._dataSource.Update();
		}

		protected override void OnResume()
		{
			base.OnResume();
			foreach (PartyNameplateVM partyNameplateVM in this._dataSource.Nameplates)
			{
				partyNameplateVM.RefreshDynamicProperties(true);
			}
		}

		protected override void OnFinalize()
		{
			this._layerAsGauntletLayer.ReleaseMovie(this._movie);
			this._dataSource.OnFinalize();
			this._layerAsGauntletLayer = null;
			base.Layer = null;
			this._movie = null;
			this._dataSource = null;
			base.OnFinalize();
		}

		private bool IsShowPartyNamesEnabled()
		{
			return base.MapScreen.SceneLayer.Input.IsGameKeyDown(5);
		}

		private GauntletLayer _layerAsGauntletLayer;

		private PartyNameplatesVM _dataSource;

		private IGauntletMovie _movie;
	}
}
