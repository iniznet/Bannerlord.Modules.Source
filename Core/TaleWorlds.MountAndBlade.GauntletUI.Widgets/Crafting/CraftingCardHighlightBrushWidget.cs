using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Crafting
{
	public class CraftingCardHighlightBrushWidget : BrushWidget
	{
		public CraftingCardHighlightBrushWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnParallelUpdate(float dt)
		{
			base.OnParallelUpdate(dt);
			if (this._firstFrame && base.IsVisible)
			{
				this._firstFrame = false;
				return;
			}
			if (!this._playingAnimation && !this._firstFrame)
			{
				base.BrushRenderer.RestartAnimation();
				this._playingAnimation = true;
			}
		}

		private bool _playingAnimation;

		private bool _firstFrame = true;
	}
}
