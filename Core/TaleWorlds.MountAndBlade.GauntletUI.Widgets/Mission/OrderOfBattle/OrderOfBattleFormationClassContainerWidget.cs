using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission.OrderOfBattle
{
	// Token: 0x020000D0 RID: 208
	public class OrderOfBattleFormationClassContainerWidget : Widget
	{
		// Token: 0x06000A9F RID: 2719 RVA: 0x0001DC2D File Offset: 0x0001BE2D
		public OrderOfBattleFormationClassContainerWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x170003C0 RID: 960
		// (get) Token: 0x06000AA0 RID: 2720 RVA: 0x0001DC36 File Offset: 0x0001BE36
		// (set) Token: 0x06000AA1 RID: 2721 RVA: 0x0001DC3E File Offset: 0x0001BE3E
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

		// Token: 0x040004D8 RID: 1240
		private SliderWidget _weightSlider;
	}
}
