using System;
using SandBox.View.Map;
using SandBox.ViewModelCollection.SaveLoad;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.ScreenSystem;

namespace SandBox.GauntletUI.Map
{
	[OverrideView(typeof(MapSaveView))]
	public class GauntletMapSaveView : MapView
	{
		protected override void CreateLayout()
		{
			base.CreateLayout();
			this._dataSource = new MapSaveVM(new Action<bool>(this.OnStateChange));
			base.Layer = new GauntletLayer(10000, "GauntletLayer", false);
			(base.Layer as GauntletLayer).LoadMovie("MapSave", this._dataSource);
			base.Layer.InputRestrictions.SetInputRestrictions(false, 5);
			base.MapScreen.AddLayer(base.Layer);
		}

		private void OnStateChange(bool isActive)
		{
			if (isActive)
			{
				base.Layer.IsFocusLayer = true;
				ScreenManager.TrySetFocus(base.Layer);
				base.Layer.InputRestrictions.SetInputRestrictions(false, 7);
				return;
			}
			base.Layer.IsFocusLayer = false;
			ScreenManager.TryLoseFocus(base.Layer);
			base.Layer.InputRestrictions.ResetInputRestrictions();
		}

		protected override void OnFinalize()
		{
			base.OnFinalize();
			this._dataSource.OnFinalize();
			base.MapScreen.RemoveLayer(base.Layer);
			base.Layer = null;
			this._dataSource = null;
		}

		private MapSaveVM _dataSource;
	}
}
