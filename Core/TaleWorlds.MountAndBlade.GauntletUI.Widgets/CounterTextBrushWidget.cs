using System;
using System.Numerics;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.Layout;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	// Token: 0x02000014 RID: 20
	public class CounterTextBrushWidget : BrushWidget
	{
		// Token: 0x1700004E RID: 78
		// (get) Token: 0x060000E9 RID: 233 RVA: 0x00004687 File Offset: 0x00002887
		// (set) Token: 0x060000EA RID: 234 RVA: 0x0000468F File Offset: 0x0000288F
		public float CounterTime { get; set; } = 0.5f;

		// Token: 0x060000EB RID: 235 RVA: 0x00004698 File Offset: 0x00002898
		public CounterTextBrushWidget(UIContext context)
			: base(context)
		{
			FontFactory fontFactory = context.FontFactory;
			this._text = new Text((int)base.Size.X, (int)base.Size.Y, fontFactory.DefaultFont, new Func<int, Font>(fontFactory.GetUsableFontForCharacter));
			base.LayoutImp = new TextLayout(this._text);
		}

		// Token: 0x060000EC RID: 236 RVA: 0x0000471C File Offset: 0x0000291C
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

		// Token: 0x060000ED RID: 237 RVA: 0x00004828 File Offset: 0x00002A28
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

		// Token: 0x060000EE RID: 238 RVA: 0x00004895 File Offset: 0x00002A95
		public void SetInitialValue(float value)
		{
			this._initialValue = value;
			this._currentValue = value;
			this._initValueSet = true;
		}

		// Token: 0x060000EF RID: 239 RVA: 0x000048AC File Offset: 0x00002AAC
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

		// Token: 0x060000F0 RID: 240 RVA: 0x00004908 File Offset: 0x00002B08
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

		// Token: 0x060000F1 RID: 241 RVA: 0x000049C0 File Offset: 0x00002BC0
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

		// Token: 0x060000F2 RID: 242 RVA: 0x00004A4C File Offset: 0x00002C4C
		public void ForceSetValue(float value)
		{
			this.SetText(this.ShowFloatingPoint ? value.ToString("F2") : MathF.Floor(value).ToString());
		}

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x060000F3 RID: 243 RVA: 0x00004A83 File Offset: 0x00002C83
		// (set) Token: 0x060000F4 RID: 244 RVA: 0x00004A92 File Offset: 0x00002C92
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

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x060000F5 RID: 245 RVA: 0x00004AA6 File Offset: 0x00002CA6
		// (set) Token: 0x060000F6 RID: 246 RVA: 0x00004AAE File Offset: 0x00002CAE
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

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x060000F7 RID: 247 RVA: 0x00004AC0 File Offset: 0x00002CC0
		// (set) Token: 0x060000F8 RID: 248 RVA: 0x00004AC8 File Offset: 0x00002CC8
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

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x060000F9 RID: 249 RVA: 0x00004AE6 File Offset: 0x00002CE6
		// (set) Token: 0x060000FA RID: 250 RVA: 0x00004AEE File Offset: 0x00002CEE
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

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x060000FB RID: 251 RVA: 0x00004B0C File Offset: 0x00002D0C
		// (set) Token: 0x060000FC RID: 252 RVA: 0x00004B14 File Offset: 0x00002D14
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

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x060000FD RID: 253 RVA: 0x00004B26 File Offset: 0x00002D26
		// (set) Token: 0x060000FE RID: 254 RVA: 0x00004B2E File Offset: 0x00002D2E
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

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x060000FF RID: 255 RVA: 0x00004B40 File Offset: 0x00002D40
		// (set) Token: 0x06000100 RID: 256 RVA: 0x00004B48 File Offset: 0x00002D48
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

		// Token: 0x04000075 RID: 117
		private readonly Text _text;

		// Token: 0x04000076 RID: 118
		private float _currentValue;

		// Token: 0x04000077 RID: 119
		private float _initialValue;

		// Token: 0x04000078 RID: 120
		private float _startTime;

		// Token: 0x04000079 RID: 121
		private bool _initValueSet;

		// Token: 0x0400007A RID: 122
		private float _targetValue;

		// Token: 0x0400007B RID: 123
		private float _minValue = float.MinValue;

		// Token: 0x0400007C RID: 124
		private float _maxValue = float.MaxValue;

		// Token: 0x0400007D RID: 125
		private bool _showSign;

		// Token: 0x0400007E RID: 126
		public bool _clamped;

		// Token: 0x0400007F RID: 127
		private bool _showFloatingPoint;
	}
}
