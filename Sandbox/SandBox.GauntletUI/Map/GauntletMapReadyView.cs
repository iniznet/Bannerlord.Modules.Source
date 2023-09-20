using System;
using SandBox.View.Map;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.MountAndBlade.View;

namespace SandBox.GauntletUI.Map
{
	// Token: 0x02000030 RID: 48
	[OverrideView(typeof(MapReadyView))]
	public class GauntletMapReadyView : MapReadyView
	{
		// Token: 0x060001BC RID: 444 RVA: 0x0000C87C File Offset: 0x0000AA7C
		protected override void CreateLayout()
		{
			base.CreateLayout();
			this._dataSource = new BoolItemWithActionVM(null, true, null);
			base.Layer = new GauntletLayer(9999, "GauntletLayer", false);
			(base.Layer as GauntletLayer).LoadMovie("MapReadyBlocker", this._dataSource);
			base.MapScreen.AddLayer(base.Layer);
		}

		// Token: 0x060001BD RID: 445 RVA: 0x0000C8E0 File Offset: 0x0000AAE0
		protected override void OnFinalize()
		{
			base.OnFinalize();
			this._dataSource.OnFinalize();
			base.MapScreen.RemoveLayer(base.Layer);
			base.Layer = null;
			this._dataSource = null;
		}

		// Token: 0x060001BE RID: 446 RVA: 0x0000C912 File Offset: 0x0000AB12
		public override void SetIsMapSceneReady(bool isReady)
		{
			base.SetIsMapSceneReady(isReady);
			this._dataSource.IsActive = !isReady;
		}

		// Token: 0x040000E8 RID: 232
		private BoolItemWithActionVM _dataSource;
	}
}
