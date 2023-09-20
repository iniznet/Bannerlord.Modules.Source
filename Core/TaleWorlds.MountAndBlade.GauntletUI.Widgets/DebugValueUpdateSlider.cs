using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.ExtraWidgets;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	public class DebugValueUpdateSlider : SliderWidget
	{
		public DebugValueUpdateSlider(UIContext context)
			: base(context)
		{
		}

		protected override void OnValueIntChanged(int value)
		{
			base.OnValueIntChanged(value);
			this.OnValueChanged((float)value);
		}

		protected override void OnValueFloatChanged(float value)
		{
			base.OnValueFloatChanged(value);
			this.OnValueChanged(value);
		}

		private void OnValueChanged(float value)
		{
			if (this.WidgetToUpdate != null)
			{
				this.WidgetToUpdate.Text = this.WidgetToUpdate.GlobalPosition.Y.ToString("F0");
			}
			if (this.ValueToUpdate != null)
			{
				this.ValueToUpdate.InitialAmount = (int)value;
			}
		}

		public TextWidget WidgetToUpdate { get; set; }

		public FillBarVerticalWidget ValueToUpdate { get; set; }
	}
}
