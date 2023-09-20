using System;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.ExtraWidgets
{
	// Token: 0x0200000D RID: 13
	public class ScrollingRichTextWidget : RichTextWidget
	{
		// Token: 0x17000051 RID: 81
		// (get) Token: 0x060000C5 RID: 197 RVA: 0x000054C9 File Offset: 0x000036C9
		// (set) Token: 0x060000C6 RID: 198 RVA: 0x000054D1 File Offset: 0x000036D1
		public string ActualText { get; private set; }

		// Token: 0x060000C7 RID: 199 RVA: 0x000054DC File Offset: 0x000036DC
		public ScrollingRichTextWidget(UIContext context)
			: base(context)
		{
			this.ScrollOnHoverWidget = this;
			this.DefaultTextHorizontalAlignment = base.Brush.TextHorizontalAlignment;
			base.ClipContents = true;
		}

		// Token: 0x060000C8 RID: 200 RVA: 0x0000552C File Offset: 0x0000372C
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (base.Size != this._currentSize)
			{
				this._currentSize = base.Size;
				this.UpdateScrollable();
			}
			if (this._shouldScroll)
			{
				this._scrollTimeElapsed += dt;
				if (this._scrollTimeElapsed < this.InbetweenScrollDuration)
				{
					this._currentScrollAmount = 0f;
				}
				else if (this._scrollTimeElapsed >= this.InbetweenScrollDuration && this._currentScrollAmount < this._totalScrollAmount)
				{
					this._currentScrollAmount += dt * this.ScrollPerTick;
				}
				else if (this._currentScrollAmount >= this._totalScrollAmount)
				{
					if (this._scrollTimeNeeded.ApproximatelyEqualsTo(0f, 1E-05f))
					{
						this._scrollTimeNeeded = this._scrollTimeElapsed;
					}
					if (this._scrollTimeElapsed < this._scrollTimeNeeded + this.InbetweenScrollDuration)
					{
						this._currentScrollAmount = this._totalScrollAmount;
					}
					else
					{
						this._scrollTimeNeeded = 0f;
						this._scrollTimeElapsed = 0f;
					}
				}
			}
			if (base.EventManager.HoveredView == this.ScrollOnHoverWidget && !this._isHovering)
			{
				this._isHovering = true;
				if (!this.IsAutoScrolling)
				{
					base.Text = this.ActualText;
					this._shouldScroll = this._wordWidth > base.Size.X;
				}
			}
			else if (base.EventManager.HoveredView != this.ScrollOnHoverWidget && this._isHovering)
			{
				this._isHovering = false;
				this.ResetScroll();
				this.UpdateScrollable();
			}
			this._renderXOffset = -this._currentScrollAmount;
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x000056CF File Offset: 0x000038CF
		public override void OnBrushChanged()
		{
			base.OnBrushChanged();
			this.DefaultTextHorizontalAlignment = base.Brush.TextHorizontalAlignment;
			this.UpdateScrollable();
		}

		// Token: 0x060000CA RID: 202 RVA: 0x000056EE File Offset: 0x000038EE
		protected override void SetText(string value)
		{
			base.SetText(value);
			this._richText.SkipLineOnContainerExceeded = false;
			this.ActualText = this._richText.Value;
			this.UpdateScrollable();
		}

		// Token: 0x060000CB RID: 203 RVA: 0x0000571C File Offset: 0x0000391C
		private void UpdateScrollable()
		{
			this.UpdateWordWidth();
			if (this._wordWidth > base.Size.X)
			{
				this._shouldScroll = this.IsAutoScrolling;
				this._totalScrollAmount = this._wordWidth - base.Size.X;
				base.Brush.TextHorizontalAlignment = TextHorizontalAlignment.Left;
				FontFactory fontFactory = base.Context.FontFactory;
				Brush brush = base.Brush;
				string text;
				if (brush == null)
				{
					text = null;
				}
				else
				{
					Font font = brush.Font;
					text = ((font != null) ? font.Name : null);
				}
				Font mappedFontForLocalization = fontFactory.GetMappedFontForLocalization(text);
				if (!this.IsAutoScrolling && !this._isHovering)
				{
					bool flag = false;
					for (int i = this._richText.Value.Length; i > 3; i--)
					{
						if (this._richText.Value[i - 1] == '>')
						{
							flag = true;
						}
						else if (this._richText.Value[i - 1] == '<')
						{
							flag = false;
						}
						if (!flag && mappedFontForLocalization.GetWordWidth(this._richText.Value.Substring(0, i - 3) + "...", 0.25f) * ((float)base.Brush.FontSize / (float)mappedFontForLocalization.Size) * base._scaleToUse < base.Size.X)
						{
							this._richText.Value = this._richText.Value.Substring(0, i - 3) + "...";
							return;
						}
					}
					return;
				}
			}
			else
			{
				this.ResetScroll();
			}
		}

		// Token: 0x060000CC RID: 204 RVA: 0x00005898 File Offset: 0x00003A98
		private void UpdateWordWidth()
		{
			FontFactory fontFactory = base.Context.FontFactory;
			Brush brush = base.Brush;
			string text;
			if (brush == null)
			{
				text = null;
			}
			else
			{
				Font font = brush.Font;
				text = ((font != null) ? font.Name : null);
			}
			Font mappedFontForLocalization = fontFactory.GetMappedFontForLocalization(text);
			this._wordWidth = mappedFontForLocalization.GetWordWidth(this._richText.Value, 0.5f) * ((float)base.Brush.FontSize / (float)mappedFontForLocalization.Size) * base._scaleToUse;
		}

		// Token: 0x060000CD RID: 205 RVA: 0x0000590C File Offset: 0x00003B0C
		private void ResetScroll()
		{
			this._shouldScroll = false;
			this._scrollTimeElapsed = 0f;
			this._currentScrollAmount = 0f;
			base.Brush.TextHorizontalAlignment = this.DefaultTextHorizontalAlignment;
		}

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x060000CE RID: 206 RVA: 0x0000593C File Offset: 0x00003B3C
		// (set) Token: 0x060000CF RID: 207 RVA: 0x00005944 File Offset: 0x00003B44
		[Editor(false)]
		public Widget ScrollOnHoverWidget
		{
			get
			{
				return this._scrollOnHoverWidget;
			}
			set
			{
				if (value != this._scrollOnHoverWidget)
				{
					this._scrollOnHoverWidget = value;
					base.OnPropertyChanged<Widget>(value, "ScrollOnHoverWidget");
				}
			}
		}

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x060000D0 RID: 208 RVA: 0x00005962 File Offset: 0x00003B62
		// (set) Token: 0x060000D1 RID: 209 RVA: 0x0000596A File Offset: 0x00003B6A
		[Editor(false)]
		public bool IsAutoScrolling
		{
			get
			{
				return this._isAutoScrolling;
			}
			set
			{
				if (value != this._isAutoScrolling)
				{
					this._isAutoScrolling = value;
					base.OnPropertyChanged(value, "IsAutoScrolling");
				}
			}
		}

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x060000D2 RID: 210 RVA: 0x00005988 File Offset: 0x00003B88
		// (set) Token: 0x060000D3 RID: 211 RVA: 0x00005990 File Offset: 0x00003B90
		[Editor(false)]
		public float ScrollPerTick
		{
			get
			{
				return this._scrollPerTick;
			}
			set
			{
				if (value != this._scrollPerTick)
				{
					this._scrollPerTick = value;
					base.OnPropertyChanged(value, "ScrollPerTick");
				}
			}
		}

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x060000D4 RID: 212 RVA: 0x000059AE File Offset: 0x00003BAE
		// (set) Token: 0x060000D5 RID: 213 RVA: 0x000059B6 File Offset: 0x00003BB6
		[Editor(false)]
		public float InbetweenScrollDuration
		{
			get
			{
				return this._inbetweenScrollDuration;
			}
			set
			{
				if (value != this._inbetweenScrollDuration)
				{
					this._inbetweenScrollDuration = value;
					base.OnPropertyChanged(value, "InbetweenScrollDuration");
				}
			}
		}

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x060000D6 RID: 214 RVA: 0x000059D4 File Offset: 0x00003BD4
		// (set) Token: 0x060000D7 RID: 215 RVA: 0x000059DC File Offset: 0x00003BDC
		[Editor(false)]
		public TextHorizontalAlignment DefaultTextHorizontalAlignment
		{
			get
			{
				return this._defaultTextHorizontalAlignment;
			}
			set
			{
				if (value != this._defaultTextHorizontalAlignment)
				{
					this._defaultTextHorizontalAlignment = value;
					base.OnPropertyChanged<string>(Enum.GetName(typeof(TextHorizontalAlignment), value), "DefaultTextHorizontalAlignment");
				}
			}
		}

		// Token: 0x04000054 RID: 84
		private bool _shouldScroll;

		// Token: 0x04000055 RID: 85
		private float _scrollTimeNeeded;

		// Token: 0x04000056 RID: 86
		private float _scrollTimeElapsed;

		// Token: 0x04000057 RID: 87
		private float _totalScrollAmount;

		// Token: 0x04000058 RID: 88
		private float _currentScrollAmount;

		// Token: 0x04000059 RID: 89
		private Vec2 _currentSize;

		// Token: 0x0400005B RID: 91
		private bool _isHovering;

		// Token: 0x0400005C RID: 92
		private float _wordWidth;

		// Token: 0x0400005D RID: 93
		private Widget _scrollOnHoverWidget;

		// Token: 0x0400005E RID: 94
		private bool _isAutoScrolling = true;

		// Token: 0x0400005F RID: 95
		private float _scrollPerTick = 30f;

		// Token: 0x04000060 RID: 96
		private float _inbetweenScrollDuration = 1f;

		// Token: 0x04000061 RID: 97
		private TextHorizontalAlignment _defaultTextHorizontalAlignment;
	}
}
