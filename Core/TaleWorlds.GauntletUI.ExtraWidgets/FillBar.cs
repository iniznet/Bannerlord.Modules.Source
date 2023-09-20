using System;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.ExtraWidgets
{
	// Token: 0x02000005 RID: 5
	public class FillBar : BrushWidget
	{
		// Token: 0x06000028 RID: 40 RVA: 0x00002524 File Offset: 0x00000724
		public FillBar(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00002530 File Offset: 0x00000730
		protected override void OnRender(TwoDimensionContext twoDimensionContext, TwoDimensionDrawContext drawContext)
		{
			if (base.IsVisible)
			{
				StyleLayer layer = base.Brush.DefaultStyle.GetLayer("DefaultFill");
				StyleLayer layer2 = base.Brush.DefaultStyle.GetLayer("ChangeFill");
				layer.WidthPolicy = BrushLayerSizePolicy.Overriden;
				layer2.WidthPolicy = BrushLayerSizePolicy.Overriden;
				layer.HeightPolicy = BrushLayerSizePolicy.Overriden;
				layer2.HeightPolicy = BrushLayerSizePolicy.Overriden;
				float num = Mathf.Clamp(this._initialAmount / (float)this.MaxAmount, 0f, 1f);
				float num2 = Mathf.Clamp(Mathf.Clamp((float)(this.CurrentAmount - this.InitialAmount), 0f, (float)(this.MaxAmount - this.InitialAmount)) / (float)this.MaxAmount, 0f, 1f);
				if (this.IsVertical)
				{
					if (!this.IsSmoothFillEnabled)
					{
						this._localDt = 1f;
					}
					float num3 = base.Size.Y * num * base._inverseScaleToUse;
					layer.OverridenHeight = Mathf.Lerp(layer.OverridenHeight, num3, this._localDt);
					num3 = base.Size.Y - layer.OverridenHeight;
					layer.YOffset = Mathf.Lerp(layer.YOffset, num3, this._localDt);
					num3 = base.Size.Y * num2 * base._inverseScaleToUse;
					layer2.OverridenHeight = Mathf.Lerp(layer2.OverridenHeight, num3, this._localDt);
					num3 = base.Size.Y - (layer.OverridenHeight + layer2.OverridenHeight);
					layer2.YOffset = Mathf.Lerp(layer2.YOffset, num3, this._localDt);
					layer.OverridenWidth = base.Size.X * base._inverseScaleToUse;
					layer2.OverridenWidth = base.Size.X * base._inverseScaleToUse;
				}
				else
				{
					if (!this.IsSmoothFillEnabled)
					{
						this._localDt = 1f;
					}
					float num4 = base.Size.X * num * base._inverseScaleToUse;
					layer.OverridenWidth = Mathf.Lerp(layer.OverridenWidth, num4, this._localDt);
					num4 = layer.OverridenWidth;
					layer2.XOffset = Mathf.Lerp(layer2.XOffset, num4, this._localDt);
					num4 = base.Size.X * num2 * base._inverseScaleToUse;
					layer2.OverridenWidth = Mathf.Lerp(layer2.OverridenWidth, num4, this._localDt);
					layer.OverridenHeight = base.Size.Y * base._inverseScaleToUse;
					layer2.OverridenHeight = base.ScaledSuggestedHeight * base._inverseScaleToUse;
				}
				base.OnRender(twoDimensionContext, drawContext);
			}
		}

		// Token: 0x0600002A RID: 42 RVA: 0x000027C4 File Offset: 0x000009C4
		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			this._localDt = dt * 10f;
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600002B RID: 43 RVA: 0x000027DA File Offset: 0x000009DA
		// (set) Token: 0x0600002C RID: 44 RVA: 0x000027E3 File Offset: 0x000009E3
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

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600002D RID: 45 RVA: 0x00002803 File Offset: 0x00000A03
		// (set) Token: 0x0600002E RID: 46 RVA: 0x0000280C File Offset: 0x00000A0C
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

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x0600002F RID: 47 RVA: 0x0000282C File Offset: 0x00000A2C
		// (set) Token: 0x06000030 RID: 48 RVA: 0x00002835 File Offset: 0x00000A35
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

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000031 RID: 49 RVA: 0x00002855 File Offset: 0x00000A55
		// (set) Token: 0x06000032 RID: 50 RVA: 0x0000285D File Offset: 0x00000A5D
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

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000033 RID: 51 RVA: 0x0000287B File Offset: 0x00000A7B
		// (set) Token: 0x06000034 RID: 52 RVA: 0x00002883 File Offset: 0x00000A83
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

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000035 RID: 53 RVA: 0x000028A1 File Offset: 0x00000AA1
		// (set) Token: 0x06000036 RID: 54 RVA: 0x000028A9 File Offset: 0x00000AA9
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

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000037 RID: 55 RVA: 0x000028C7 File Offset: 0x00000AC7
		// (set) Token: 0x06000038 RID: 56 RVA: 0x000028CF File Offset: 0x00000ACF
		[Editor(false)]
		public bool IsVertical
		{
			get
			{
				return this._isVertical;
			}
			set
			{
				if (this._isVertical != value)
				{
					this._isVertical = value;
					base.OnPropertyChanged(value, "IsVertical");
				}
			}
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000039 RID: 57 RVA: 0x000028ED File Offset: 0x00000AED
		// (set) Token: 0x0600003A RID: 58 RVA: 0x000028F5 File Offset: 0x00000AF5
		[Editor(false)]
		public bool IsSmoothFillEnabled
		{
			get
			{
				return this._isSmoothFillEnabled;
			}
			set
			{
				if (this._isSmoothFillEnabled != value)
				{
					this._isSmoothFillEnabled = value;
					base.OnPropertyChanged(value, "IsSmoothFillEnabled");
				}
			}
		}

		// Token: 0x04000015 RID: 21
		private float _localDt;

		// Token: 0x04000016 RID: 22
		private float _maxAmount;

		// Token: 0x04000017 RID: 23
		private float _currentAmount;

		// Token: 0x04000018 RID: 24
		private float _initialAmount;

		// Token: 0x04000019 RID: 25
		private bool _isVertical;

		// Token: 0x0400001A RID: 26
		private bool _isSmoothFillEnabled;
	}
}
