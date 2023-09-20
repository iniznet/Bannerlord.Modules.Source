using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.ExtraWidgets;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.CharacterDeveloper
{
	public class SkillProgressFillBarWidget : FillBarWidget
	{
		public SkillProgressFillBarWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnRender(TwoDimensionContext twoDimensionContext, TwoDimensionDrawContext drawContext)
		{
			if (this.PercentageIndicatorWidget != null)
			{
				base.ScaledPositionXOffset = Mathf.Clamp((this.PercentageIndicatorWidget.ScaledPositionXOffset - base.Size.X / 2f) * base._scaleToUse, 0f, 600f * base._scaleToUse);
			}
			base.OnRender(twoDimensionContext, drawContext);
		}

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

		private Widget _percentageIndicatorWidget;
	}
}
