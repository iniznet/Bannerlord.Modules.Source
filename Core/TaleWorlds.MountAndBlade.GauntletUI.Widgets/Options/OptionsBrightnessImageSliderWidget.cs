using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Options
{
	// Token: 0x02000069 RID: 105
	public class OptionsBrightnessImageSliderWidget : SliderWidget
	{
		// Token: 0x170001F9 RID: 505
		// (get) Token: 0x06000597 RID: 1431 RVA: 0x00010C84 File Offset: 0x0000EE84
		// (set) Token: 0x06000598 RID: 1432 RVA: 0x00010C8C File Offset: 0x0000EE8C
		public bool IsMax { get; set; }

		// Token: 0x170001FA RID: 506
		// (get) Token: 0x06000599 RID: 1433 RVA: 0x00010C95 File Offset: 0x0000EE95
		// (set) Token: 0x0600059A RID: 1434 RVA: 0x00010C9D File Offset: 0x0000EE9D
		public Widget ImageWidget { get; set; }

		// Token: 0x0600059B RID: 1435 RVA: 0x00010CA6 File Offset: 0x0000EEA6
		public OptionsBrightnessImageSliderWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x0600059C RID: 1436 RVA: 0x00010CB0 File Offset: 0x0000EEB0
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (!this._isInitialized)
			{
				float num;
				if (this.IsMax)
				{
					num = (float)(base.ValueInt - 1) * 0.003f + 1f;
				}
				else
				{
					num = (float)(base.ValueInt + 1) * 0.003f;
				}
				this.SetColorOfImage(MBMath.ClampFloat(num, 0f, 1f));
				this._isInitialized = true;
			}
		}

		// Token: 0x0600059D RID: 1437 RVA: 0x00010D20 File Offset: 0x0000EF20
		protected override void OnValueFloatChanged(float value)
		{
			base.OnValueFloatChanged(value);
			float num;
			if (this.IsMax)
			{
				num = (value - 1f) * 0.003f + 1f;
			}
			else
			{
				num = (value + 1f) * 0.003f;
			}
			this.SetColorOfImage(MBMath.ClampFloat(num, 0f, 1f));
		}

		// Token: 0x0600059E RID: 1438 RVA: 0x00010D7C File Offset: 0x0000EF7C
		private void SetColorOfImage(float value)
		{
			this.ImageWidget.Color = new Color(value, value, value, 1f);
		}

		// Token: 0x0400026B RID: 619
		private bool _isInitialized;
	}
}
