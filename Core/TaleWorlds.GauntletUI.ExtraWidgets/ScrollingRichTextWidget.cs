using System;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.ExtraWidgets
{
	public class ScrollingRichTextWidget : RichTextWidget
	{
		public string ActualText { get; private set; }

		public ScrollingRichTextWidget(UIContext context)
			: base(context)
		{
			this.ScrollOnHoverWidget = this;
			this.DefaultTextHorizontalAlignment = base.Brush.TextHorizontalAlignment;
			base.ClipContents = true;
		}

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

		public override void OnBrushChanged()
		{
			base.OnBrushChanged();
			this.DefaultTextHorizontalAlignment = base.Brush.TextHorizontalAlignment;
			this.UpdateScrollable();
		}

		protected override void SetText(string value)
		{
			base.SetText(value);
			this._richText.SkipLineOnContainerExceeded = false;
			this.ActualText = this._richText.Value;
			this.UpdateScrollable();
		}

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

		private void ResetScroll()
		{
			this._shouldScroll = false;
			this._scrollTimeElapsed = 0f;
			this._currentScrollAmount = 0f;
			base.Brush.TextHorizontalAlignment = this.DefaultTextHorizontalAlignment;
		}

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

		private bool _shouldScroll;

		private float _scrollTimeNeeded;

		private float _scrollTimeElapsed;

		private float _totalScrollAmount;

		private float _currentScrollAmount;

		private Vec2 _currentSize;

		private bool _isHovering;

		private float _wordWidth;

		private Widget _scrollOnHoverWidget;

		private bool _isAutoScrolling = true;

		private float _scrollPerTick = 30f;

		private float _inbetweenScrollDuration = 1f;

		private TextHorizontalAlignment _defaultTextHorizontalAlignment;
	}
}
