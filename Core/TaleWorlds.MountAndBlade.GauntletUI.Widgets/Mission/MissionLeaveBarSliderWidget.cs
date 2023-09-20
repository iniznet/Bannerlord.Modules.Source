using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission
{
	// Token: 0x020000CA RID: 202
	public class MissionLeaveBarSliderWidget : SliderWidget
	{
		// Token: 0x1700039D RID: 925
		// (get) Token: 0x06000A49 RID: 2633 RVA: 0x0001D30E File Offset: 0x0001B50E
		private float CurrentAlpha
		{
			get
			{
				return base.ReadOnlyBrush.GlobalAlphaFactor;
			}
		}

		// Token: 0x1700039E RID: 926
		// (get) Token: 0x06000A4A RID: 2634 RVA: 0x0001D31B File Offset: 0x0001B51B
		// (set) Token: 0x06000A4B RID: 2635 RVA: 0x0001D323 File Offset: 0x0001B523
		public float FadeInMultiplier { get; set; } = 1f;

		// Token: 0x1700039F RID: 927
		// (get) Token: 0x06000A4C RID: 2636 RVA: 0x0001D32C File Offset: 0x0001B52C
		// (set) Token: 0x06000A4D RID: 2637 RVA: 0x0001D334 File Offset: 0x0001B534
		public float FadeOutMultiplier { get; set; } = 1f;

		// Token: 0x06000A4E RID: 2638 RVA: 0x0001D33D File Offset: 0x0001B53D
		public MissionLeaveBarSliderWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000A4F RID: 2639 RVA: 0x0001D35C File Offset: 0x0001B55C
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (!this._initialized)
			{
				this.SetGlobalAlphaRecursively(0f);
				this._initialized = true;
			}
			float num = ((base.ValueFloat > 0f) ? this.FadeInMultiplier : this.FadeOutMultiplier);
			float num2 = (float)((base.ValueFloat > 0f) ? 1 : 0);
			float num3 = Mathf.Clamp(Mathf.Lerp(this.CurrentAlpha, num2, num * 0.2f), 0f, 1f);
			this.SetGlobalAlphaRecursively(num3);
		}

		// Token: 0x040004AF RID: 1199
		private bool _initialized;
	}
}
