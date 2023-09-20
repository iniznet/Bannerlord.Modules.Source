using System;
using System.Numerics;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.ExtraWidgets
{
	// Token: 0x0200000E RID: 14
	public class ScrollingTextWidget : TextWidget
	{
		// Token: 0x17000057 RID: 87
		// (get) Token: 0x060000D8 RID: 216 RVA: 0x00005A0E File Offset: 0x00003C0E
		// (set) Token: 0x060000D9 RID: 217 RVA: 0x00005A16 File Offset: 0x00003C16
		public string ActualText { get; private set; }

		// Token: 0x060000DA RID: 218 RVA: 0x00005A20 File Offset: 0x00003C20
		public ScrollingTextWidget(UIContext context)
			: base(context)
		{
			this.ScrollOnHoverWidget = this;
			this.DefaultTextHorizontalAlignment = base.Brush.TextHorizontalAlignment;
			base.ClipHorizontalContent = true;
		}

		// Token: 0x060000DB RID: 219 RVA: 0x00005A70 File Offset: 0x00003C70
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
				if (!this.IsAutoScrolling)
				{
					this._text.Value = this.ActualText;
					this._shouldScroll = this._wordWidth > base.Size.X;
				}
				this._isHovering = true;
				return;
			}
			if (base.EventManager.HoveredView != this.ScrollOnHoverWidget && this._isHovering)
			{
				this.ResetScroll();
				this.UpdateScrollable();
				this._isHovering = false;
			}
		}

		// Token: 0x060000DC RID: 220 RVA: 0x00005C0A File Offset: 0x00003E0A
		public override void OnBrushChanged()
		{
			this.DefaultTextHorizontalAlignment = base.Brush.TextHorizontalAlignment;
			this.UpdateScrollable();
		}

		// Token: 0x060000DD RID: 221 RVA: 0x00005C23 File Offset: 0x00003E23
		protected override void SetText(string value)
		{
			base.SetText(value);
			this._text.SkipLineOnContainerExceeded = false;
			this.ActualText = this._text.Value;
			this.UpdateScrollable();
		}

		// Token: 0x060000DE RID: 222 RVA: 0x00005C50 File Offset: 0x00003E50
		protected override void OnRender(TwoDimensionContext twoDimensionContext, TwoDimensionDrawContext drawContext)
		{
			base.RefreshTextParameters();
			TextMaterial textMaterial = base.BrushRenderer.CreateTextMaterial(drawContext);
			textMaterial.AlphaFactor *= base.Context.ContextAlpha;
			Vector2 cachedGlobalPosition = this._cachedGlobalPosition;
			drawContext.Draw(this._text, textMaterial, cachedGlobalPosition.X - this._currentScrollAmount, cachedGlobalPosition.Y, base.Size.X, base.Size.Y);
		}

		// Token: 0x060000DF RID: 223 RVA: 0x00005CC8 File Offset: 0x00003EC8
		private void UpdateScrollable()
		{
			this.UpdateWordWidth();
			if (this._wordWidth > base.Size.X)
			{
				this._shouldScroll = this.IsAutoScrolling;
				this._totalScrollAmount = this._wordWidth - base.Size.X;
				base.Brush.TextHorizontalAlignment = TextHorizontalAlignment.Left;
				if (!this.IsAutoScrolling)
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
					for (int i = this._text.Value.Length; i > 3; i--)
					{
						if (mappedFontForLocalization.GetWordWidth(this._text.Value.Substring(0, i - 3) + "...", 0.25f) * ((float)base.Brush.FontSize / (float)mappedFontForLocalization.Size) * base._scaleToUse < base.Size.X)
						{
							this._text.Value = this._text.Value.Substring(0, i - 3) + "...";
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

		// Token: 0x060000E0 RID: 224 RVA: 0x00005E00 File Offset: 0x00004000
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
			this._wordWidth = mappedFontForLocalization.GetWordWidth(this._text.Value, 0.5f) * ((float)base.Brush.FontSize / (float)mappedFontForLocalization.Size) * base._scaleToUse;
		}

		// Token: 0x060000E1 RID: 225 RVA: 0x00005E74 File Offset: 0x00004074
		private void ResetScroll()
		{
			this._shouldScroll = false;
			this._scrollTimeElapsed = 0f;
			this._currentScrollAmount = 0f;
			base.Brush.TextHorizontalAlignment = this.DefaultTextHorizontalAlignment;
		}

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x060000E2 RID: 226 RVA: 0x00005EA4 File Offset: 0x000040A4
		// (set) Token: 0x060000E3 RID: 227 RVA: 0x00005EAC File Offset: 0x000040AC
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

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x060000E4 RID: 228 RVA: 0x00005ECA File Offset: 0x000040CA
		// (set) Token: 0x060000E5 RID: 229 RVA: 0x00005ED2 File Offset: 0x000040D2
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

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x060000E6 RID: 230 RVA: 0x00005EF0 File Offset: 0x000040F0
		// (set) Token: 0x060000E7 RID: 231 RVA: 0x00005EF8 File Offset: 0x000040F8
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

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x060000E8 RID: 232 RVA: 0x00005F16 File Offset: 0x00004116
		// (set) Token: 0x060000E9 RID: 233 RVA: 0x00005F1E File Offset: 0x0000411E
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

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x060000EA RID: 234 RVA: 0x00005F3C File Offset: 0x0000413C
		// (set) Token: 0x060000EB RID: 235 RVA: 0x00005F44 File Offset: 0x00004144
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

		// Token: 0x04000062 RID: 98
		private bool _shouldScroll;

		// Token: 0x04000063 RID: 99
		private float _scrollTimeNeeded;

		// Token: 0x04000064 RID: 100
		private float _scrollTimeElapsed;

		// Token: 0x04000065 RID: 101
		private float _totalScrollAmount;

		// Token: 0x04000066 RID: 102
		private float _currentScrollAmount;

		// Token: 0x04000067 RID: 103
		private Vec2 _currentSize;

		// Token: 0x04000069 RID: 105
		private bool _isHovering;

		// Token: 0x0400006A RID: 106
		private float _wordWidth;

		// Token: 0x0400006B RID: 107
		private Widget _scrollOnHoverWidget;

		// Token: 0x0400006C RID: 108
		private bool _isAutoScrolling = true;

		// Token: 0x0400006D RID: 109
		private float _scrollPerTick = 30f;

		// Token: 0x0400006E RID: 110
		private float _inbetweenScrollDuration = 1f;

		// Token: 0x0400006F RID: 111
		private TextHorizontalAlignment _defaultTextHorizontalAlignment;
	}
}
