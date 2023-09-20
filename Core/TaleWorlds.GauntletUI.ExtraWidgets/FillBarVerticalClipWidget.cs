using System;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.ExtraWidgets
{
	public class FillBarVerticalClipWidget : Widget
	{
		public FillBarVerticalClipWidget(UIContext context)
			: base(context)
		{
		}

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

		protected override void OnRender(TwoDimensionContext twoDimensionContext, TwoDimensionDrawContext drawContext)
		{
			base.OnRender(twoDimensionContext, drawContext);
		}

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

		private bool _isCurrentValueSet;

		private Widget _fillWidget;

		private Widget _changeWidget;

		private Widget _containerWidget;

		private Widget _dividerWidget;

		private Widget _clipWidget;

		private float _maxAmount;

		private float _currentAmount;

		private float _initialAmount;

		private bool _isDirectionUpward;
	}
}
