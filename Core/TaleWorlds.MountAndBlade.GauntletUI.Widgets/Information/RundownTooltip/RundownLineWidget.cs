using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Information.RundownTooltip
{
	public class RundownLineWidget : ListPanel
	{
		public TextWidget NameTextWidget { get; set; }

		public TextWidget ValueTextWidget { get; set; }

		public float Value { get; set; }

		public RundownLineWidget(UIContext context)
			: base(context)
		{
		}

		public void RefreshValueOffset(float columnWidth)
		{
			if (columnWidth >= 0f && this.NameTextWidget.Size.X > 1E-05f && this.ValueTextWidget.Size.X > 1E-05f)
			{
				this.ValueTextWidget.ScaledPositionXOffset = columnWidth - (this.NameTextWidget.Size.X + this.ValueTextWidget.Size.X + base.ScaledMarginLeft + base.ScaledMarginRight);
			}
		}
	}
}
