using System;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.ExtraWidgets
{
	// Token: 0x0200000A RID: 10
	public class FillBarWidget : Widget
	{
		// Token: 0x06000090 RID: 144 RVA: 0x00003708 File Offset: 0x00001908
		public FillBarWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000091 RID: 145 RVA: 0x00003714 File Offset: 0x00001914
		protected override void OnRender(TwoDimensionContext twoDimensionContext, TwoDimensionDrawContext drawContext)
		{
			if (this.FillWidget != null)
			{
				float x = this.FillWidget.ParentWidget.Size.X;
				float num = Mathf.Clamp(Mathf.Clamp(this._initialAmount, 0f, this._maxAmount) / this._maxAmount, 0f, 1f);
				this.FillWidget.ScaledSuggestedWidth = num * x;
				if (this.ChangeWidget != null)
				{
					float num2 = Mathf.Clamp(Mathf.Clamp(this._currentAmount - this._initialAmount, -this._maxAmount, this._maxAmount) / this._maxAmount, -1f, 1f);
					if (num2 > 0f)
					{
						if (this.CompletelyFillChange)
						{
							float num3 = Mathf.Clamp(Mathf.Clamp(this._currentAmount, 0f, this._maxAmount) / this._maxAmount, 0f, 1f);
							this.ChangeWidget.ScaledSuggestedWidth = num3 * x;
						}
						else
						{
							this.ChangeWidget.ScaledSuggestedWidth = Mathf.Clamp(num2 * x, 0f, x - this.FillWidget.ScaledSuggestedWidth);
							this.ChangeWidget.ScaledPositionXOffset = this.FillWidget.ScaledSuggestedWidth;
						}
						this.ChangeWidget.Color = new Color(1f, 1f, 1f, 1f);
					}
					else if (num2 < 0f && this.ShowNegativeChange)
					{
						this.ChangeWidget.ScaledSuggestedWidth = num2 * x * -1f;
						this.ChangeWidget.ScaledPositionXOffset = this.FillWidget.ScaledSuggestedWidth - this.ChangeWidget.ScaledSuggestedWidth;
						this.ChangeWidget.Color = new Color(1f, 0f, 0f, 1f);
					}
					else
					{
						this.ChangeWidget.ScaledSuggestedWidth = 0f;
					}
					if (this.DividerWidget != null)
					{
						if (num2 > 0f)
						{
							this.DividerWidget.ScaledPositionXOffset = this.ChangeWidget.ScaledPositionXOffset - this.DividerWidget.Size.X;
						}
						else if (num2 < 0f)
						{
							this.DividerWidget.ScaledPositionXOffset = this.FillWidget.ScaledSuggestedWidth - this.DividerWidget.Size.X;
						}
						this.DividerWidget.IsVisible = this.ChangeWidget != null && num2 != 0f;
					}
				}
			}
			base.OnRender(twoDimensionContext, drawContext);
		}

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x06000092 RID: 146 RVA: 0x00003981 File Offset: 0x00001B81
		// (set) Token: 0x06000093 RID: 147 RVA: 0x0000398A File Offset: 0x00001B8A
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
				}
			}
		}

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x06000094 RID: 148 RVA: 0x000039AA File Offset: 0x00001BAA
		// (set) Token: 0x06000095 RID: 149 RVA: 0x000039B3 File Offset: 0x00001BB3
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

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x06000096 RID: 150 RVA: 0x000039D3 File Offset: 0x00001BD3
		// (set) Token: 0x06000097 RID: 151 RVA: 0x000039DC File Offset: 0x00001BDC
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

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x06000098 RID: 152 RVA: 0x000039FC File Offset: 0x00001BFC
		// (set) Token: 0x06000099 RID: 153 RVA: 0x00003A04 File Offset: 0x00001C04
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

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x0600009A RID: 154 RVA: 0x00003A22 File Offset: 0x00001C22
		// (set) Token: 0x0600009B RID: 155 RVA: 0x00003A2A File Offset: 0x00001C2A
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

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x0600009C RID: 156 RVA: 0x00003A48 File Offset: 0x00001C48
		// (set) Token: 0x0600009D RID: 157 RVA: 0x00003A50 File Offset: 0x00001C50
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

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x0600009E RID: 158 RVA: 0x00003A6E File Offset: 0x00001C6E
		// (set) Token: 0x0600009F RID: 159 RVA: 0x00003A76 File Offset: 0x00001C76
		[Editor(false)]
		public bool CompletelyFillChange
		{
			get
			{
				return this._completelyFillChange;
			}
			set
			{
				if (this._completelyFillChange != value)
				{
					this._completelyFillChange = value;
					base.OnPropertyChanged(value, "CompletelyFillChange");
				}
			}
		}

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x060000A0 RID: 160 RVA: 0x00003A94 File Offset: 0x00001C94
		// (set) Token: 0x060000A1 RID: 161 RVA: 0x00003A9C File Offset: 0x00001C9C
		[Editor(false)]
		public bool ShowNegativeChange
		{
			get
			{
				return this._showNegativeChange;
			}
			set
			{
				if (this._showNegativeChange != value)
				{
					this._showNegativeChange = value;
					base.OnPropertyChanged(value, "ShowNegativeChange");
				}
			}
		}

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x060000A2 RID: 162 RVA: 0x00003ABA File Offset: 0x00001CBA
		// (set) Token: 0x060000A3 RID: 163 RVA: 0x00003AC2 File Offset: 0x00001CC2
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

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x060000A4 RID: 164 RVA: 0x00003AE0 File Offset: 0x00001CE0
		// (set) Token: 0x060000A5 RID: 165 RVA: 0x00003AE8 File Offset: 0x00001CE8
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

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x060000A6 RID: 166 RVA: 0x00003B06 File Offset: 0x00001D06
		// (set) Token: 0x060000A7 RID: 167 RVA: 0x00003B0E File Offset: 0x00001D0E
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

		// Token: 0x0400003F RID: 63
		private Widget _fillWidget;

		// Token: 0x04000040 RID: 64
		private Widget _changeWidget;

		// Token: 0x04000041 RID: 65
		private Widget _dividerWidget;

		// Token: 0x04000042 RID: 66
		private float _maxAmount;

		// Token: 0x04000043 RID: 67
		private float _currentAmount;

		// Token: 0x04000044 RID: 68
		private float _initialAmount;

		// Token: 0x04000045 RID: 69
		private bool _completelyFillChange;

		// Token: 0x04000046 RID: 70
		private bool _showNegativeChange;
	}
}
