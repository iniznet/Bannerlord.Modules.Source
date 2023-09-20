using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.GameOver
{
	public class GameOverScreenWidget : Widget
	{
		public BrushWidget ConceptVisualWidget { get; set; }

		public BrushWidget BannerBrushWidget { get; set; }

		public BrushWidget BannerFrameBrushWidget1 { get; set; }

		public BrushWidget BannerFrameBrushWidget2 { get; set; }

		public string GameOverReason { get; set; }

		public GameOverScreenWidget(UIContext context)
			: base(context)
		{
			base.EventManager.AddLateUpdateAction(this, new Action<float>(this.OnManualLateUpdate), 4);
		}

		private void OnManualLateUpdate(float obj)
		{
			if (this.ConceptVisualWidget != null)
			{
				this.ConceptVisualWidget.Brush = base.Context.GetBrush("GameOver.Mask." + this.GameOverReason);
			}
			if (this.BannerBrushWidget != null)
			{
				this.BannerBrushWidget.Brush = base.Context.GetBrush("GameOver.Banner." + this.GameOverReason);
			}
			if (this.BannerFrameBrushWidget1 != null)
			{
				this.BannerFrameBrushWidget1.Brush = base.Context.GetBrush("GameOver.Banner.Frame." + this.GameOverReason);
			}
			if (this.BannerFrameBrushWidget2 != null)
			{
				this.BannerFrameBrushWidget2.Brush = base.Context.GetBrush("GameOver.Banner.Frame." + this.GameOverReason);
			}
		}
	}
}
