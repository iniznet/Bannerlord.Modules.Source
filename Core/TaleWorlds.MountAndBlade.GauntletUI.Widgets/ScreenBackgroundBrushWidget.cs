using System;
using System.Linq;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	public class ScreenBackgroundBrushWidget : BrushWidget
	{
		public bool IsParticleVisible { get; set; }

		public bool IsSmokeVisible { get; set; }

		public bool IsFullscreenImageEnabled { get; set; }

		public bool AnimEnabled { get; set; }

		public Widget ParticleWidget1 { get; set; }

		public Widget ParticleWidget2 { get; set; }

		public Widget SmokeWidget1 { get; set; }

		public Widget SmokeWidget2 { get; set; }

		public float SmokeSpeedModifier { get; set; } = 1f;

		public float ParticleSpeedModifier { get; set; } = 1f;

		public ScreenBackgroundBrushWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this._firstFrame)
			{
				this.UpdateBackgroundImage();
				this._firstFrame = false;
			}
			this.ParticleWidget1.IsVisible = this.IsParticleVisible;
			this.ParticleWidget2.IsVisible = this.IsParticleVisible;
			this.SmokeWidget1.IsVisible = this.IsSmokeVisible;
			this.SmokeWidget2.IsVisible = this.IsSmokeVisible;
			if (this.AnimEnabled)
			{
				if (this.IsParticleVisible)
				{
					this.ParticleWidget1.PositionXOffset = this._totalParticleXOffset;
					this.ParticleWidget2.PositionXOffset = this.ParticleWidget1.PositionXOffset + this.ParticleWidget1.SuggestedWidth;
					this._totalParticleXOffset -= dt * 10f * this.ParticleSpeedModifier;
					if (Math.Abs(this._totalParticleXOffset) >= this.ParticleWidget1.SuggestedWidth)
					{
						this._totalParticleXOffset = 0f;
					}
				}
				if (this.IsSmokeVisible)
				{
					this.SmokeWidget1.PositionXOffset = this._totalSmokeXOffset;
					this.SmokeWidget2.PositionXOffset = this.SmokeWidget1.PositionXOffset - this.SmokeWidget1.SuggestedWidth;
					if (Math.Abs(this._totalSmokeXOffset) >= this.SmokeWidget1.SuggestedWidth)
					{
						this._totalSmokeXOffset = 0f;
					}
					this._totalSmokeXOffset += dt * 10f * this.SmokeSpeedModifier;
				}
			}
		}

		private void UpdateBackgroundImage()
		{
			if (this.IsFullscreenImageEnabled)
			{
				int num = base.Context.UIRandom.Next(base.Brush.Styles.Count);
				base.Brush.Sprite = base.ReadOnlyBrush.Styles.ElementAt(num).Layers[0].Sprite;
				return;
			}
			base.Brush.Sprite = null;
		}

		private bool _firstFrame = true;

		private float _totalSmokeXOffset;

		private float _totalParticleXOffset;
	}
}
