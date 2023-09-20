using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.GameOver
{
	// Token: 0x02000130 RID: 304
	public class GameOverScreenWidget : Widget
	{
		// Token: 0x170005A4 RID: 1444
		// (get) Token: 0x06000FFD RID: 4093 RVA: 0x0002D40B File Offset: 0x0002B60B
		// (set) Token: 0x06000FFE RID: 4094 RVA: 0x0002D413 File Offset: 0x0002B613
		public BrushWidget ConceptVisualWidget { get; set; }

		// Token: 0x170005A5 RID: 1445
		// (get) Token: 0x06000FFF RID: 4095 RVA: 0x0002D41C File Offset: 0x0002B61C
		// (set) Token: 0x06001000 RID: 4096 RVA: 0x0002D424 File Offset: 0x0002B624
		public BrushWidget BannerBrushWidget { get; set; }

		// Token: 0x170005A6 RID: 1446
		// (get) Token: 0x06001001 RID: 4097 RVA: 0x0002D42D File Offset: 0x0002B62D
		// (set) Token: 0x06001002 RID: 4098 RVA: 0x0002D435 File Offset: 0x0002B635
		public BrushWidget BannerFrameBrushWidget1 { get; set; }

		// Token: 0x170005A7 RID: 1447
		// (get) Token: 0x06001003 RID: 4099 RVA: 0x0002D43E File Offset: 0x0002B63E
		// (set) Token: 0x06001004 RID: 4100 RVA: 0x0002D446 File Offset: 0x0002B646
		public BrushWidget BannerFrameBrushWidget2 { get; set; }

		// Token: 0x170005A8 RID: 1448
		// (get) Token: 0x06001005 RID: 4101 RVA: 0x0002D44F File Offset: 0x0002B64F
		// (set) Token: 0x06001006 RID: 4102 RVA: 0x0002D457 File Offset: 0x0002B657
		public string GameOverReason { get; set; }

		// Token: 0x06001007 RID: 4103 RVA: 0x0002D460 File Offset: 0x0002B660
		public GameOverScreenWidget(UIContext context)
			: base(context)
		{
			base.EventManager.AddLateUpdateAction(this, new Action<float>(this.OnManualLateUpdate), 4);
		}

		// Token: 0x06001008 RID: 4104 RVA: 0x0002D484 File Offset: 0x0002B684
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
