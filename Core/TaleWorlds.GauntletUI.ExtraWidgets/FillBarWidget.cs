using System;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.ExtraWidgets
{
	public class FillBarWidget : Widget
	{
		public FillBarWidget(UIContext context)
			: base(context)
		{
		}

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

		private Widget _fillWidget;

		private Widget _changeWidget;

		private Widget _dividerWidget;

		private float _maxAmount;

		private float _currentAmount;

		private float _initialAmount;

		private bool _completelyFillChange;

		private bool _showNegativeChange;
	}
}
