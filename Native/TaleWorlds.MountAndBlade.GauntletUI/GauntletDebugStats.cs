using System;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.ScreenSystem;

namespace TaleWorlds.MountAndBlade.GauntletUI
{
	public class GauntletDebugStats : GlobalLayer
	{
		public void Initialize()
		{
			this._dataSource = new DebugStatsVM();
			GauntletLayer gauntletLayer = new GauntletLayer(15000, "GauntletLayer", false);
			gauntletLayer.LoadMovie("DebugStats", this._dataSource);
			gauntletLayer.InputRestrictions.SetInputRestrictions(false, 0);
			base.Layer = gauntletLayer;
			ScreenManager.AddGlobalLayer(this, true);
		}

		private DebugStatsVM _dataSource;
	}
}
