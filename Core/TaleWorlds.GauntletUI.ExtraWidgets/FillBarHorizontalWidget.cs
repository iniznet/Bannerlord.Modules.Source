using System;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.ExtraWidgets
{
	// Token: 0x02000006 RID: 6
	public class FillBarHorizontalWidget : Widget
	{
		// Token: 0x0600003B RID: 59 RVA: 0x00002913 File Offset: 0x00000B13
		public FillBarHorizontalWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x0600003C RID: 60 RVA: 0x0000291C File Offset: 0x00000B1C
		protected override void OnRender(TwoDimensionContext twoDimensionContext, TwoDimensionDrawContext drawContext)
		{
			if (this.FillWidget != null)
			{
				float x = this.FillWidget.ParentWidget.Size.X;
				float num = Mathf.Clamp(Mathf.Clamp((float)this.InitialAmount, 0f, (float)this.MaxAmount) / (float)this.MaxAmount, 0f, 1f);
				float num2 = (this._isCurrentValueSet ? Mathf.Clamp((float)(this.CurrentAmount - this.InitialAmount), (float)(-(float)this.MaxAmount), (float)this.MaxAmount) : 0f);
				float num3 = (this._isCurrentValueSet ? Mathf.Clamp(num2 / (float)this.MaxAmount, -1f, 1f) : 0f);
				if (this.IsDirectionRightward)
				{
					this.FillWidget.HorizontalAlignment = HorizontalAlignment.Left;
					this.FillWidget.ScaledSuggestedWidth = num * x;
					if (this.ChangeWidget != null)
					{
						this.ChangeWidget.ScaledSuggestedWidth = num3 * x;
						if (num3 >= 0f)
						{
							this.ChangeWidget.ScaledPositionXOffset = -this.FillWidget.ScaledSuggestedWidth;
							this.ChangeWidget.Color = new Color(1f, 1f, 1f, 1f);
						}
						else
						{
							this.ChangeWidget.ScaledPositionXOffset = -this.FillWidget.ScaledSuggestedWidth + this.ChangeWidget.ScaledSuggestedWidth;
							this.ChangeWidget.Color = new Color(1f, 0f, 0f, 1f);
						}
					}
				}
				else
				{
					this.FillWidget.HorizontalAlignment = HorizontalAlignment.Right;
					this.FillWidget.ScaledSuggestedWidth = num * x;
					if (this.ChangeWidget != null)
					{
						this.ChangeWidget.ScaledSuggestedWidth = num3 * x;
						this.ChangeWidget.HorizontalAlignment = HorizontalAlignment.Right;
						if (num3 >= 0f)
						{
							this.ChangeWidget.ScaledPositionXOffset = -this.FillWidget.ScaledSuggestedWidth;
							this.ChangeWidget.Color = new Color(1f, 1f, 1f, 1f);
						}
						else
						{
							this.ChangeWidget.ScaledPositionXOffset = -this.FillWidget.ScaledSuggestedWidth + this.ChangeWidget.ScaledSuggestedWidth;
							this.ChangeWidget.Color = new Color(1f, 0f, 0f, 1f);
						}
					}
				}
				if (this.ChangeWidget != null && this.DividerWidget != null)
				{
					this.DividerWidget.IsVisible = this.ChangeWidget != null && num3 != 0f;
				}
			}
			base.OnRender(twoDimensionContext, drawContext);
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x0600003D RID: 61 RVA: 0x00002BAA File Offset: 0x00000DAA
		// (set) Token: 0x0600003E RID: 62 RVA: 0x00002BB2 File Offset: 0x00000DB2
		[Editor(false)]
		public bool IsDirectionRightward
		{
			get
			{
				return this._isDirectionRightward;
			}
			set
			{
				if (this._isDirectionRightward != value)
				{
					this._isDirectionRightward = value;
					base.OnPropertyChanged(value, "IsDirectionRightward");
				}
			}
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x0600003F RID: 63 RVA: 0x00002BD0 File Offset: 0x00000DD0
		// (set) Token: 0x06000040 RID: 64 RVA: 0x00002BD9 File Offset: 0x00000DD9
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

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000041 RID: 65 RVA: 0x00002C00 File Offset: 0x00000E00
		// (set) Token: 0x06000042 RID: 66 RVA: 0x00002C09 File Offset: 0x00000E09
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

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000043 RID: 67 RVA: 0x00002C29 File Offset: 0x00000E29
		// (set) Token: 0x06000044 RID: 68 RVA: 0x00002C32 File Offset: 0x00000E32
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

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000045 RID: 69 RVA: 0x00002C52 File Offset: 0x00000E52
		// (set) Token: 0x06000046 RID: 70 RVA: 0x00002C5A File Offset: 0x00000E5A
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

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000047 RID: 71 RVA: 0x00002C78 File Offset: 0x00000E78
		// (set) Token: 0x06000048 RID: 72 RVA: 0x00002C80 File Offset: 0x00000E80
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

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x06000049 RID: 73 RVA: 0x00002C9E File Offset: 0x00000E9E
		// (set) Token: 0x0600004A RID: 74 RVA: 0x00002CA6 File Offset: 0x00000EA6
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

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x0600004B RID: 75 RVA: 0x00002CC4 File Offset: 0x00000EC4
		// (set) Token: 0x0600004C RID: 76 RVA: 0x00002CCC File Offset: 0x00000ECC
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

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x0600004D RID: 77 RVA: 0x00002CEA File Offset: 0x00000EEA
		// (set) Token: 0x0600004E RID: 78 RVA: 0x00002CF2 File Offset: 0x00000EF2
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

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x0600004F RID: 79 RVA: 0x00002D10 File Offset: 0x00000F10
		// (set) Token: 0x06000050 RID: 80 RVA: 0x00002D18 File Offset: 0x00000F18
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

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000051 RID: 81 RVA: 0x00002D36 File Offset: 0x00000F36
		// (set) Token: 0x06000052 RID: 82 RVA: 0x00002D3E File Offset: 0x00000F3E
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

		// Token: 0x0400001B RID: 27
		private bool _isCurrentValueSet;

		// Token: 0x0400001C RID: 28
		private Widget _fillWidget;

		// Token: 0x0400001D RID: 29
		private Widget _changeWidget;

		// Token: 0x0400001E RID: 30
		private Widget _containerWidget;

		// Token: 0x0400001F RID: 31
		private Widget _dividerWidget;

		// Token: 0x04000020 RID: 32
		private float _maxAmount;

		// Token: 0x04000021 RID: 33
		private float _currentAmount;

		// Token: 0x04000022 RID: 34
		private float _initialAmount;

		// Token: 0x04000023 RID: 35
		private bool _isDirectionRightward;
	}
}
