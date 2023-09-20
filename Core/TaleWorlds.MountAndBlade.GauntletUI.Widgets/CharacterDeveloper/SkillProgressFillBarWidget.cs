using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.ExtraWidgets;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.CharacterDeveloper
{
	// Token: 0x02000165 RID: 357
	public class SkillProgressFillBarWidget : FillBarWidget
	{
		// Token: 0x0600125A RID: 4698 RVA: 0x00032BB4 File Offset: 0x00030DB4
		public SkillProgressFillBarWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x0600125B RID: 4699 RVA: 0x00032BC0 File Offset: 0x00030DC0
		protected override void OnRender(TwoDimensionContext twoDimensionContext, TwoDimensionDrawContext drawContext)
		{
			if (this.PercentageIndicatorWidget != null)
			{
				base.ScaledPositionXOffset = Mathf.Clamp((this.PercentageIndicatorWidget.ScaledPositionXOffset - base.Size.X / 2f) * base._scaleToUse, 0f, 600f * base._scaleToUse);
			}
			base.OnRender(twoDimensionContext, drawContext);
		}

		// Token: 0x1700067A RID: 1658
		// (get) Token: 0x0600125C RID: 4700 RVA: 0x00032C1D File Offset: 0x00030E1D
		// (set) Token: 0x0600125D RID: 4701 RVA: 0x00032C25 File Offset: 0x00030E25
		public Widget PercentageIndicatorWidget
		{
			get
			{
				return this._percentageIndicatorWidget;
			}
			set
			{
				if (this._percentageIndicatorWidget != value)
				{
					this._percentageIndicatorWidget = value;
					base.OnPropertyChanged<Widget>(value, "PercentageIndicatorWidget");
				}
			}
		}

		// Token: 0x04000869 RID: 2153
		private Widget _percentageIndicatorWidget;
	}
}
