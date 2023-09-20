using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.CharacterCreation.Culture
{
	public class CharacterCreationFirstStageFadeOutWidget : Widget
	{
		public float StayTime { get; set; } = 1.5f;

		public float FadeOutTime { get; set; } = 1.5f;

		public CharacterCreationFirstStageFadeOutWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this._totalTime < this.StayTime)
			{
				this.SetGlobalAlphaRecursively(1f);
				base.IsEnabled = true;
			}
			else if (this._totalTime > this.StayTime && this._totalTime < this.StayTime + this.FadeOutTime)
			{
				float num = Mathf.Lerp(1f, 0f, (this._totalTime - this.StayTime) / this.FadeOutTime);
				this.SetGlobalAlphaRecursively(num);
				base.IsEnabled = num > 0.2f;
			}
			else
			{
				this.SetGlobalAlphaRecursively(0f);
				base.IsEnabled = false;
			}
			this._totalTime += dt;
		}

		private float _totalTime;
	}
}
