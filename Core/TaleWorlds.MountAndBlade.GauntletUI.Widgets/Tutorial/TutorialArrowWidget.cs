using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Tutorial
{
	// Token: 0x0200003F RID: 63
	public class TutorialArrowWidget : Widget
	{
		// Token: 0x17000125 RID: 293
		// (get) Token: 0x06000347 RID: 839 RVA: 0x0000A840 File Offset: 0x00008A40
		// (set) Token: 0x06000348 RID: 840 RVA: 0x0000A848 File Offset: 0x00008A48
		public bool IsArrowEnabled { get; set; }

		// Token: 0x17000126 RID: 294
		// (get) Token: 0x06000349 RID: 841 RVA: 0x0000A851 File Offset: 0x00008A51
		// (set) Token: 0x0600034A RID: 842 RVA: 0x0000A859 File Offset: 0x00008A59
		public float FadeInTime { get; set; } = 1f;

		// Token: 0x17000127 RID: 295
		// (get) Token: 0x0600034B RID: 843 RVA: 0x0000A862 File Offset: 0x00008A62
		// (set) Token: 0x0600034C RID: 844 RVA: 0x0000A86A File Offset: 0x00008A6A
		public float BigCircleRadius { get; set; } = 2f;

		// Token: 0x17000128 RID: 296
		// (get) Token: 0x0600034D RID: 845 RVA: 0x0000A873 File Offset: 0x00008A73
		// (set) Token: 0x0600034E RID: 846 RVA: 0x0000A87B File Offset: 0x00008A7B
		public float SmallCircleRadius { get; set; } = 2f;

		// Token: 0x0600034F RID: 847 RVA: 0x0000A884 File Offset: 0x00008A84
		public TutorialArrowWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000350 RID: 848 RVA: 0x0000A8B0 File Offset: 0x00008AB0
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

		// Token: 0x06000351 RID: 849 RVA: 0x0000A958 File Offset: 0x00008B58
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

		// Token: 0x06000352 RID: 850 RVA: 0x0000AAA1 File Offset: 0x00008CA1
		public void ResetFade()
		{
			this._startTime = base.EventManager.Time;
		}

		// Token: 0x06000353 RID: 851 RVA: 0x0000AAB4 File Offset: 0x00008CB4
		public void DisableFade()
		{
			this._startTime = base.EventManager.Time;
		}

		// Token: 0x06000354 RID: 852 RVA: 0x0000AAC7 File Offset: 0x00008CC7
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

		// Token: 0x04000159 RID: 345
		private float _localWidth;

		// Token: 0x0400015A RID: 346
		private float _localHeight;

		// Token: 0x0400015B RID: 347
		private bool _isDirectionDown;

		// Token: 0x0400015C RID: 348
		private bool _isDirectionRight;

		// Token: 0x0400015D RID: 349
		private float _startTime;
	}
}
