using System;
using SandBox.View.Map;
using SandBox.ViewModelCollection.SaveLoad;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.ScreenSystem;

namespace SandBox.GauntletUI.Map
{
	// Token: 0x02000031 RID: 49
	[OverrideView(typeof(MapSaveView))]
	public class GauntletMapSaveView : MapView
	{
		// Token: 0x060001C0 RID: 448 RVA: 0x0000C934 File Offset: 0x0000AB34
		protected override void CreateLayout()
		{
			base.CreateLayout();
			this._dataSource = new MapSaveVM(new Action<bool>(this.OnStateChange));
			base.Layer = new GauntletLayer(10000, "GauntletLayer", false);
			(base.Layer as GauntletLayer).LoadMovie("MapSave", this._dataSource);
			base.Layer.InputRestrictions.SetInputRestrictions(false, 5);
			base.MapScreen.AddLayer(base.Layer);
		}

		// Token: 0x060001C1 RID: 449 RVA: 0x0000C9B4 File Offset: 0x0000ABB4
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

		// Token: 0x060001C2 RID: 450 RVA: 0x0000CA15 File Offset: 0x0000AC15
		protected override void OnFinalize()
		{
			base.OnFinalize();
			this._dataSource.OnFinalize();
			base.MapScreen.RemoveLayer(base.Layer);
			base.Layer = null;
			this._dataSource = null;
		}

		// Token: 0x040000E9 RID: 233
		private MapSaveVM _dataSource;
	}
}
