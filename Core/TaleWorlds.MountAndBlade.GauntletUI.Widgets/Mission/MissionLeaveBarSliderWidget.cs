using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission
{
	public class MissionLeaveBarSliderWidget : SliderWidget
	{
		private float CurrentAlpha
		{
			get
			{
				return base.ReadOnlyBrush.GlobalAlphaFactor;
			}
		}

		public float FadeInMultiplier { get; set; } = 1f;

		public float FadeOutMultiplier { get; set; } = 1f;

		public MissionLeaveBarSliderWidget(UIContext context)
			: base(context)
		{
		}

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

		private bool _initialized;
	}
}
