using System;
using SandBox.View.Menu;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.MountAndBlade.View;

namespace SandBox.GauntletUI.Menu
{
	// Token: 0x02000018 RID: 24
	[OverrideView(typeof(MenuBackgroundView))]
	public class GauntlerMenuBackground : MenuView
	{
		// Token: 0x06000107 RID: 263 RVA: 0x000087E8 File Offset: 0x000069E8
		protected override void OnInitialize()
		{
			base.OnInitialize();
			base.Layer = base.MenuViewContext.FindLayer<GauntletLayer>("BasicLayer");
			if (base.Layer == null)
			{
				base.Layer = new GauntletLayer(100, "GauntletLayer", false)
				{
					Name = "BasicLayer"
				};
				base.MenuViewContext.AddLayer(base.Layer);
			}
			GauntletLayer gauntletLayer = base.Layer as GauntletLayer;
			this._movie = gauntletLayer.LoadMovie("GameMenuBackground", null);
			base.Layer.InputRestrictions.SetInputRestrictions(true, 7);
		}

		// Token: 0x06000108 RID: 264 RVA: 0x00008878 File Offset: 0x00006A78
		protected override void OnFinalize()
		{
			GauntletLayer gauntletLayer = base.Layer as GauntletLayer;
			if (gauntletLayer != null)
			{
				gauntletLayer.ReleaseMovie(this._movie);
			}
			base.Layer = null;
			this._movie = null;
			base.OnFinalize();
		}

		// Token: 0x04000079 RID: 121
		private IGauntletMovie _movie;
	}
}
