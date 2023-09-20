using System;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.ExtraWidgets
{
	// Token: 0x02000008 RID: 8
	public class FillBarVerticalClipWidget : Widget
	{
		// Token: 0x0600005D RID: 93 RVA: 0x00002F8D File Offset: 0x0000118D
		public FillBarVerticalClipWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x0600005E RID: 94 RVA: 0x00002F98 File Offset: 0x00001198
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this.FillWidget != null && this.ClipWidget != null)
			{
				float y = base.Size.Y;
				float num = Mathf.Clamp(Mathf.Clamp(this._initialAmount, 0f, (float)this.MaxAmount) / (float)this.MaxAmount, 0f, 1f);
				float num2 = (this._isCurrentValueSet ? Mathf.Clamp((float)(this.CurrentAmount - this.InitialAmount), (float)(-(float)this.MaxAmount), (float)this.MaxAmount) : 0f);
				float num3 = (this._isCurrentValueSet ? Mathf.Clamp(num2 / (float)this.MaxAmount, -1f, 1f) : 0f);
				this.ClipWidget.VerticalAlignment = VerticalAlignment.Bottom;
				this.ClipWidget.ClipContents = true;
				this.FillWidget.VerticalAlignment = VerticalAlignment.Bottom;
				if (this.IsDirectionUpward)
				{
					this.ClipWidget.VerticalAlignment = VerticalAlignment.Bottom;
				}
				else
				{
					this.ClipWidget.VerticalAlignment = VerticalAlignment.Top;
				}
				this.ClipWidget.ScaledSuggestedHeight = y * num;
				if (this.ChangeWidget != null && this.DividerWidget != null)
				{
					this.DividerWidget.IsVisible = this.ChangeWidget != null && num3 != 0f;
				}
			}
		}

		// Token: 0x0600005F RID: 95 RVA: 0x000030DC File Offset: 0x000012DC
		protected override void OnRender(TwoDimensionContext twoDimensionContext, TwoDimensionDrawContext drawContext)
		{
			base.OnRender(twoDimensionContext, drawContext);
		}

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x06000060 RID: 96 RVA: 0x000030E6 File Offset: 0x000012E6
		// (set) Token: 0x06000061 RID: 97 RVA: 0x000030EE File Offset: 0x000012EE
		[Editor(false)]
		public bool IsDirectionUpward
		{
			get
			{
				return this._isDirectionUpward;
			}
			set
			{
				if (this._isDirectionUpward != value)
				{
					this._isDirectionUpward = value;
					base.OnPropertyChanged(value, "IsDirectionUpward");
				}
			}
		}

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x06000062 RID: 98 RVA: 0x0000310C File Offset: 0x0000130C
		// (set) Token: 0x06000063 RID: 99 RVA: 0x00003115 File Offset: 0x00001315
		[Editor(false)]
		public int CurrentAmount
		{
			get
			{
				return (int)this._currentAmount;
			}
			set
			{
				if (this._currentAmount != (float)value)
				{
					this._currentAmount = (float)value;
					base.OnPropertyChanged(value, "CurrentAmount");
					this._isCurrentValueSet = true;
				}
			}
		}

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x06000064 RID: 100 RVA: 0x0000313C File Offset: 0x0000133C
		// (set) Token: 0x06000065 RID: 101 RVA: 0x00003145 File Offset: 0x00001345
		[Editor(false)]
		public int MaxAmount
		{
			get
			{
				return (int)this._maxAmount;
			}
			set
			{
				if (this._maxAmount != (float)value)
				{
					this._maxAmount = (float)value;
					base.OnPropertyChanged(value, "MaxAmount");
				}
			}
		}

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x06000066 RID: 102 RVA: 0x00003165 File Offset: 0x00001365
		// (set) Token: 0x06000067 RID: 103 RVA: 0x0000316E File Offset: 0x0000136E
		[Editor(false)]
		public int InitialAmount
		{
			get
			{
				return (int)this._initialAmount;
			}
			set
			{
				if (this._initialAmount != (float)value)
				{
					this._initialAmount = (float)value;
					base.OnPropertyChanged(value, "InitialAmount");
				}
			}
		}

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x06000068 RID: 104 RVA: 0x0000318E File Offset: 0x0000138E
		// (set) Token: 0x06000069 RID: 105 RVA: 0x00003196 File Offset: 0x00001396
		[Editor(false)]
		public float MaxAmountAsFloat
		{
			get
			{
				return this._maxAmount;
			}
			set
			{
				if (this._maxAmount != value)
				{
					this._maxAmount = value;
					base.OnPropertyChanged(value, "MaxAmountAsFloat");
				}
			}
		}

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x0600006A RID: 106 RVA: 0x000031B4 File Offset: 0x000013B4
		// (set) Token: 0x0600006B RID: 107 RVA: 0x000031BC File Offset: 0x000013BC
		[Editor(false)]
		public float CurrentAmountAsFloat
		{
			get
			{
				return this._currentAmount;
			}
			set
			{
				if (this._currentAmount != value)
				{
					this._currentAmount = value;
					base.OnPropertyChanged(value, "CurrentAmountAsFloat");
				}
			}
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x0600006C RID: 108 RVA: 0x000031DA File Offset: 0x000013DA
		// (set) Token: 0x0600006D RID: 109 RVA: 0x000031E2 File Offset: 0x000013E2
		[Editor(false)]
		public float InitialAmountAsFloat
		{
			get
			{
				return this._initialAmount;
			}
			set
			{
				if (this._initialAmount != value)
				{
					this._initialAmount = value;
					base.OnPropertyChanged(value, "InitialAmountAsFloat");
				}
			}
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x0600006E RID: 110 RVA: 0x00003200 File Offset: 0x00001400
		// (set) Token: 0x0600006F RID: 111 RVA: 0x00003208 File Offset: 0x00001408
		public Widget FillWidget
		{
			get
			{
				return this._fillWidget;
			}
			set
			{
				if (this._fillWidget != value)
				{
					this._fillWidget = value;
					base.OnPropertyChanged<Widget>(value, "FillWidget");
				}
			}
		}

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x06000070 RID: 112 RVA: 0x00003226 File Offset: 0x00001426
		// (set) Token: 0x06000071 RID: 113 RVA: 0x0000322E File Offset: 0x0000142E
		public Widget ChangeWidget
		{
			get
			{
				return this._changeWidget;
			}
			set
			{
				if (this._changeWidget != value)
				{
					this._changeWidget = value;
					base.OnPropertyChanged<Widget>(value, "ChangeWidget");
				}
			}
		}

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x06000072 RID: 114 RVA: 0x0000324C File Offset: 0x0000144C
		// (set) Token: 0x06000073 RID: 115 RVA: 0x00003254 File Offset: 0x00001454
		public Widget DividerWidget
		{
			get
			{
				return this._dividerWidget;
			}
			set
			{
				if (this._dividerWidget != value)
				{
					this._dividerWidget = value;
					base.OnPropertyChanged<Widget>(value, "DividerWidget");
				}
			}
		}

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x06000074 RID: 116 RVA: 0x00003272 File Offset: 0x00001472
		// (set) Token: 0x06000075 RID: 117 RVA: 0x0000327A File Offset: 0x0000147A
		public Widget ContainerWidget
		{
			get
			{
				return this._containerWidget;
			}
			set
			{
				if (this._containerWidget != value)
				{
					this._containerWidget = value;
					base.OnPropertyChanged<Widget>(value, "ContainerWidget");
				}
			}
		}

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x06000076 RID: 118 RVA: 0x00003298 File Offset: 0x00001498
		// (set) Token: 0x06000077 RID: 119 RVA: 0x000032A0 File Offset: 0x000014A0
		public Widget ClipWidget
		{
			get
			{
				return this._clipWidget;
			}
			set
			{
				if (this._clipWidget != value)
				{
					this._clipWidget = value;
					base.OnPropertyChanged<Widget>(value, "ClipWidget");
				}
			}
		}

		// Token: 0x0400002C RID: 44
		private bool _isCurrentValueSet;

		// Token: 0x0400002D RID: 45
		private Widget _fillWidget;

		// Token: 0x0400002E RID: 46
		private Widget _changeWidget;

		// Token: 0x0400002F RID: 47
		private Widget _containerWidget;

		// Token: 0x04000030 RID: 48
		private Widget _dividerWidget;

		// Token: 0x04000031 RID: 49
		private Widget _clipWidget;

		// Token: 0x04000032 RID: 50
		private float _maxAmount;

		// Token: 0x04000033 RID: 51
		private float _currentAmount;

		// Token: 0x04000034 RID: 52
		private float _initialAmount;

		// Token: 0x04000035 RID: 53
		private bool _isDirectionUpward;
	}
}
