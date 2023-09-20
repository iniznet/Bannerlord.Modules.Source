using System;
using System.Numerics;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.Layout;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	public class CounterTextBrushWidget : BrushWidget
	{
		public float CounterTime { get; set; } = 0.5f;

		public CounterTextBrushWidget(UIContext context)
			: base(context)
		{
			FontFactory fontFactory = context.FontFactory;
			this._text = new Text((int)base.Size.X, (int)base.Size.Y, fontFactory.DefaultFont, new Func<int, Font>(fontFactory.GetUsableFontForCharacter));
			base.LayoutImp = new TextLayout(this._text);
		}

		protected override void OnRender(TwoDimensionContext twoDimensionContext, TwoDimensionDrawContext drawContext)
		{
			if (MathF.Abs(this._targetValue - this._currentValue) > 1E-45f && MathF.Abs(base.Context.EventManager.Time - this._startTime) < this.CounterTime)
			{
				this._currentValue = Mathf.Lerp(this._currentValue, this._targetValue, (base.Context.EventManager.Time - this._startTime) / this.CounterTime);
				if (this.Clamped)
				{
					this._currentValue = Mathf.Clamp(this._currentValue, this.MinValue, this.MaxValue);
				}
				this.ForceSetValue(this._currentValue);
			}
			else
			{
				this.ForceSetValue(this._targetValue);
			}
			this.RefreshTextParameters();
			TextMaterial textMaterial = base.BrushRenderer.CreateTextMaterial(drawContext);
			Vector2 globalPosition = base.GlobalPosition;
			drawContext.Draw(this._text, textMaterial, globalPosition.X, globalPosition.Y, base.Size.X, base.Size.Y);
		}

		private void SetText(string value)
		{
			base.SetMeasureAndLayoutDirty();
			this._text.CurrentLanguage = base.Context.FontFactory.GetCurrentLanguage();
			if (this.ShowSign && this._currentValue > 0f)
			{
				this._text.Value = "+" + value;
			}
			else
			{
				this._text.Value = value;
			}
			this.RefreshTextParameters();
		}

		public void SetInitialValue(float value)
		{
			this._initialValue = value;
			this._currentValue = value;
			this._initValueSet = true;
		}

		private void SetTargetValue(float targetValue)
		{
			if (!this._initValueSet)
			{
				this._currentValue = targetValue;
				this._initValueSet = true;
			}
			this._initialValue = this._currentValue;
			this._startTime = base.Context.EventManager.Time;
			this.RefreshTextAnimation(targetValue - this._targetValue);
			this._targetValue = targetValue;
		}

		private void RefreshTextParameters()
		{
			float num = (float)base.ReadOnlyBrush.FontSize * base._scaleToUse;
			this._text.HorizontalAlignment = base.ReadOnlyBrush.TextHorizontalAlignment;
			this._text.VerticalAlignment = base.ReadOnlyBrush.TextVerticalAlignment;
			this._text.FontSize = num;
			this._text.CurrentLanguage = base.Context.FontFactory.GetCurrentLanguage();
			if (base.ReadOnlyBrush.Font != null)
			{
				this._text.Font = base.ReadOnlyBrush.Font;
				return;
			}
			FontFactory fontFactory = base.Context.FontFactory;
			this._text.Font = fontFactory.DefaultFont;
		}

		private void RefreshTextAnimation(float valueDifference)
		{
			if (valueDifference > 0f)
			{
				if (base.CurrentState == "Positive")
				{
					base.BrushRenderer.RestartAnimation();
					return;
				}
				this.SetState("Positive");
				return;
			}
			else
			{
				if (valueDifference >= 0f)
				{
					Debug.FailedAssert("Value change in party label cannot be 0", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.GauntletUI.Widgets\\CounterTextBrushWidget.cs", "RefreshTextAnimation", 142);
					return;
				}
				if (base.CurrentState == "Negative")
				{
					base.BrushRenderer.RestartAnimation();
					return;
				}
				this.SetState("Negative");
				return;
			}
		}

		public void ForceSetValue(float value)
		{
			this.SetText(this.ShowFloatingPoint ? value.ToString("F2") : MathF.Floor(value).ToString());
		}

		[Editor(false)]
		public int IntTarget
		{
			get
			{
				return (int)Math.Round((double)this._targetValue);
			}
			set
			{
				if (this._targetValue != (float)value)
				{
					this.SetTargetValue((float)value);
				}
			}
		}

		[Editor(false)]
		public float FloatTarget
		{
			get
			{
				return this._targetValue;
			}
			set
			{
				if (this._targetValue != value)
				{
					this.SetTargetValue(value);
				}
			}
		}

		[Editor(false)]
		public float MinValue
		{
			get
			{
				return this._minValue;
			}
			set
			{
				if (value != this._minValue)
				{
					this._minValue = value;
					base.OnPropertyChanged(value, "MinValue");
				}
			}
		}

		[Editor(false)]
		public float MaxValue
		{
			get
			{
				return this._maxValue;
			}
			set
			{
				if (value != this._maxValue)
				{
					this._maxValue = value;
					base.OnPropertyChanged(value, "MaxValue");
				}
			}
		}

		[Editor(false)]
		public bool ShowSign
		{
			get
			{
				return this._showSign;
			}
			set
			{
				if (this._showSign != value)
				{
					this._showSign = value;
				}
			}
		}

		[Editor(false)]
		public bool Clamped
		{
			get
			{
				return this._clamped;
			}
			set
			{
				if (this._clamped != value)
				{
					this._clamped = value;
				}
			}
		}

		[Editor(false)]
		public bool ShowFloatingPoint
		{
			get
			{
				return this._showFloatingPoint;
			}
			set
			{
				if (value != this._showFloatingPoint)
				{
					this._showFloatingPoint = value;
				}
			}
		}

		private readonly Text _text;

		private float _currentValue;

		private float _initialValue;

		private float _startTime;

		private bool _initValueSet;

		private float _targetValue;

		private float _minValue = float.MinValue;

		private float _maxValue = float.MaxValue;

		private bool _showSign;

		public bool _clamped;

		private bool _showFloatingPoint;
	}
}
