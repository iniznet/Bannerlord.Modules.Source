using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Options
{
	public class OptionsBrightnessImageSliderWidget : SliderWidget
	{
		public bool IsMax { get; set; }

		public Widget ImageWidget { get; set; }

		public OptionsBrightnessImageSliderWidget(UIContext context)
			: base(context)
		{
		}

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

		private void SetColorOfImage(float value)
		{
			this.ImageWidget.Color = new Color(value, value, value, 1f);
		}

		private bool _isInitialized;
	}
}
