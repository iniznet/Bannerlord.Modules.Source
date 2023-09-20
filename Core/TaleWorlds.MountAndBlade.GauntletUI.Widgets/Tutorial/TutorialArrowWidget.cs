using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Tutorial
{
	public class TutorialArrowWidget : Widget
	{
		public bool IsArrowEnabled { get; set; }

		public float FadeInTime { get; set; } = 1f;

		public float BigCircleRadius { get; set; } = 2f;

		public float SmallCircleRadius { get; set; } = 2f;

		public TutorialArrowWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (!this.IsArrowEnabled)
			{
				base.IsVisible = false;
				this.SetGlobalAlphaRecursively(0f);
				return;
			}
			base.IsVisible = true;
			base.ScaledSuggestedWidth = this._localWidth;
			base.ScaledSuggestedHeight = this._localHeight;
			if (this._startTime > -1f)
			{
				float num = Mathf.Lerp(0f, 1f, Mathf.Clamp((base.EventManager.Time - this._startTime) / this.FadeInTime, 0f, 1f));
				this.SetGlobalAlphaRecursively(num);
				return;
			}
			this.SetGlobalAlphaRecursively(0f);
		}

		public void SetArrowProperties(float width, float height, bool isDirectionDown, bool isDirectionRight)
		{
			if (this._localWidth != width || this._localHeight != height || this._isDirectionDown != isDirectionDown || this._isDirectionRight != isDirectionRight)
			{
				base.RemoveAllChildren();
				float num = (float)Math.Sqrt((double)(width * width + height * height));
				float num2 = (this.BigCircleRadius + this.SmallCircleRadius) / 2f;
				int num3 = (int)(num / num2);
				float num4 = 0f;
				float num5 = 0f;
				float num6;
				float num7;
				if (isDirectionDown)
				{
					num6 = width;
					num7 = height;
				}
				else
				{
					num6 = width;
					num5 = height;
					num7 = 0f;
				}
				float num8 = (isDirectionRight ? this.BigCircleRadius : this.SmallCircleRadius);
				float num9 = (isDirectionRight ? this.SmallCircleRadius : this.BigCircleRadius);
				for (int i = 0; i < num3; i++)
				{
					Widget defaultCircleWidgetTemplate = this.GetDefaultCircleWidgetTemplate();
					base.AddChild(defaultCircleWidgetTemplate);
					float num10 = num2 * (float)i / MathF.Abs(num4 - num6);
					float num11 = Mathf.Lerp(num8, num9, num10);
					defaultCircleWidgetTemplate.PositionXOffset = Mathf.Lerp(num4, num6, num10);
					defaultCircleWidgetTemplate.PositionYOffset = Mathf.Lerp(num5, num7, num10);
					defaultCircleWidgetTemplate.SuggestedHeight = num11;
					defaultCircleWidgetTemplate.SuggestedWidth = num11;
				}
				this._localWidth = width;
				this._localHeight = height;
				this._isDirectionDown = isDirectionDown;
				this._isDirectionRight = isDirectionRight;
			}
		}

		public void ResetFade()
		{
			this._startTime = base.EventManager.Time;
		}

		public void DisableFade()
		{
			this._startTime = base.EventManager.Time;
		}

		private Widget GetDefaultCircleWidgetTemplate()
		{
			return new Widget(base.Context)
			{
				WidthSizePolicy = SizePolicy.Fixed,
				HeightSizePolicy = SizePolicy.Fixed,
				Sprite = base.Context.SpriteData.GetSprite("BlankWhiteCircle"),
				IsEnabled = false
			};
		}

		private float _localWidth;

		private float _localHeight;

		private bool _isDirectionDown;

		private bool _isDirectionRight;

		private float _startTime;
	}
}
