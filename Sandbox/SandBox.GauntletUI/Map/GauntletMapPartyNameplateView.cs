using System;
using SandBox.View.Map;
using SandBox.ViewModelCollection.Nameplate;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.MountAndBlade.View;

namespace SandBox.GauntletUI.Map
{
	// Token: 0x0200002F RID: 47
	[OverrideView(typeof(MapPartyNameplateView))]
	public class GauntletMapPartyNameplateView : MapView
	{
		// Token: 0x060001B6 RID: 438 RVA: 0x0000C714 File Offset: 0x0000A914
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

		// Token: 0x060001B7 RID: 439 RVA: 0x0000C7B0 File Offset: 0x0000A9B0
		protected override void OnMapScreenUpdate(float dt)
		{
			base.OnMapScreenUpdate(dt);
			this._dataSource.Update();
		}

		// Token: 0x060001B8 RID: 440 RVA: 0x0000C7C4 File Offset: 0x0000A9C4
		protected override void OnResume()
		{
			base.OnResume();
			foreach (PartyNameplateVM partyNameplateVM in this._dataSource.Nameplates)
			{
				partyNameplateVM.RefreshDynamicProperties(true);
			}
		}

		// Token: 0x060001B9 RID: 441 RVA: 0x0000C81C File Offset: 0x0000AA1C
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

		// Token: 0x060001BA RID: 442 RVA: 0x0000C85C File Offset: 0x0000AA5C
		private bool IsShowPartyNamesEnabled()
		{
			return base.MapScreen.SceneLayer.Input.IsGameKeyDown(5);
		}

		// Token: 0x040000E5 RID: 229
		private GauntletLayer _layerAsGauntletLayer;

		// Token: 0x040000E6 RID: 230
		private PartyNameplatesVM _dataSource;

		// Token: 0x040000E7 RID: 231
		private IGauntletMovie _movie;
	}
}
