using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Crafting
{
	// Token: 0x02000142 RID: 322
	public class CraftingCardHighlightBrushWidget : BrushWidget
	{
		// Token: 0x060010F1 RID: 4337 RVA: 0x0002F394 File Offset: 0x0002D594
		public CraftingCardHighlightBrushWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060010F2 RID: 4338 RVA: 0x0002F3A4 File Offset: 0x0002D5A4
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

		// Token: 0x040007C7 RID: 1991
		private bool _playingAnimation;

		// Token: 0x040007C8 RID: 1992
		private bool _firstFrame = true;
	}
}
