using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.ExtraWidgets;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	// Token: 0x02000015 RID: 21
	public class DebugValueUpdateSlider : SliderWidget
	{
		// Token: 0x06000101 RID: 257 RVA: 0x00004B5A File Offset: 0x00002D5A
		public DebugValueUpdateSlider(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000102 RID: 258 RVA: 0x00004B63 File Offset: 0x00002D63
		protected override void OnValueIntChanged(int value)
		{
			base.OnValueIntChanged(value);
			this.OnValueChanged((float)value);
		}

		// Token: 0x06000103 RID: 259 RVA: 0x00004B74 File Offset: 0x00002D74
		protected override void OnValueFloatChanged(float value)
		{
			base.OnValueFloatChanged(value);
			this.OnValueChanged(value);
		}

		// Token: 0x06000104 RID: 260 RVA: 0x00004B84 File Offset: 0x00002D84
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

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x06000105 RID: 261 RVA: 0x00004BD6 File Offset: 0x00002DD6
		// (set) Token: 0x06000106 RID: 262 RVA: 0x00004BDE File Offset: 0x00002DDE
		public TextWidget WidgetToUpdate { get; set; }

		// Token: 0x17000057 RID: 87
		// (get) Token: 0x06000107 RID: 263 RVA: 0x00004BE7 File Offset: 0x00002DE7
		// (set) Token: 0x06000108 RID: 264 RVA: 0x00004BEF File Offset: 0x00002DEF
		public FillBarVerticalWidget ValueToUpdate { get; set; }
	}
}
