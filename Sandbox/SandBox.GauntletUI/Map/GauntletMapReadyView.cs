using System;
using SandBox.View.Map;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.MountAndBlade.View;

namespace SandBox.GauntletUI.Map
{
	[OverrideView(typeof(MapReadyView))]
	public class GauntletMapReadyView : MapReadyView
	{
		protected override void CreateLayout()
		{
			base.CreateLayout();
			this._dataSource = new BoolItemWithActionVM(null, true, null);
			base.Layer = new GauntletLayer(9999, "GauntletLayer", false);
			(base.Layer as GauntletLayer).LoadMovie("MapReadyBlocker", this._dataSource);
			base.MapScreen.AddLayer(base.Layer);
		}

		protected override void OnFinalize()
		{
			base.OnFinalize();
			this._dataSource.OnFinalize();
			base.MapScreen.RemoveLayer(base.Layer);
			base.Layer = null;
			this._dataSource = null;
		}

		public override void SetIsMapSceneReady(bool isReady)
		{
			base.SetIsMapSceneReady(isReady);
			this._dataSource.IsActive = !isReady;
		}

		private BoolItemWithActionVM _dataSource;
	}
}
