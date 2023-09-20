using System;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.ExtraWidgets
{
	public class FillBarHorizontalWidget : Widget
	{
		public FillBarHorizontalWidget(UIContext context)
			: base(context)
		{
		}

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

		private bool _isCurrentValueSet;

		private Widget _fillWidget;

		private Widget _changeWidget;

		private Widget _containerWidget;

		private Widget _dividerWidget;

		private float _maxAmount;

		private float _currentAmount;

		private float _initialAmount;

		private bool _isDirectionRightward;
	}
}
