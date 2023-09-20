using System;
using SandBox.View.Menu;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.MountAndBlade.View;

namespace SandBox.GauntletUI.Menu
{
	[OverrideView(typeof(MenuBackgroundView))]
	public class GauntlerMenuBackground : MenuView
	{
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

		private IGauntletMovie _movie;
	}
}
