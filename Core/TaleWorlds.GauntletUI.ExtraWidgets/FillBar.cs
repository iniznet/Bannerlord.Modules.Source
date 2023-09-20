using System;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.ExtraWidgets
{
	public class FillBar : BrushWidget
	{
		public FillBar(UIContext context)
			: base(context)
		{
		}

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

		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			this._localDt = dt * 10f;
		}

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

		private float _localDt;

		private float _maxAmount;

		private float _currentAmount;

		private float _initialAmount;

		private bool _isVertical;

		private bool _isSmoothFillEnabled;
	}
}
