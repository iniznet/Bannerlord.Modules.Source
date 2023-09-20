using System;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.MountAndBlade.GauntletUI
{
	// Token: 0x02000005 RID: 5
	public class GauntletDebugStats : GlobalLayer
	{
		// Token: 0x0600001F RID: 31 RVA: 0x00002BFC File Offset: 0x00000DFC
		public void Initialize()
		{
			this._dataSource = new DebugStatsVM();
			GauntletLayer gauntletLayer = new GauntletLayer(15000, "GauntletLayer", false);
			gauntletLayer.LoadMovie("DebugStats", this._dataSource);
			gauntletLayer.InputRestrictions.SetInputRestrictions(false, 0);
			base.Layer = gauntletLayer;
			ScreenManager.AddGlobalLayer(this, true);
		}

		// Token: 0x04000012 RID: 18
		private DebugStatsVM _dataSource;
	}
}
