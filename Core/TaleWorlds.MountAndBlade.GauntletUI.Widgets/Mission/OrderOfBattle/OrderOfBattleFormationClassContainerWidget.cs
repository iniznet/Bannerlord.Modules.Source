using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission.OrderOfBattle
{
	public class OrderOfBattleFormationClassContainerWidget : Widget
	{
		public OrderOfBattleFormationClassContainerWidget(UIContext context)
			: base(context)
		{
		}

		[Editor(false)]
		public SliderWidget WeightSlider
		{
			get
			{
				return this._weightSlider;
			}
			set
			{
				if (value != this._weightSlider)
				{
					this._weightSlider = value;
					base.OnPropertyChanged<SliderWidget>(value, "WeightSlider");
				}
			}
		}

		private SliderWidget _weightSlider;
	}
}
