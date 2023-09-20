using System;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.GauntletUI.ExtraWidgets
{
	public class TwoWaySliderWidget : SliderWidget
	{
		public TwoWaySliderWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnValueIntChanged(int value)
		{
			base.OnValueIntChanged(value);
			if (this.ChangeFillWidget == null || base.MaxValueInt == 0)
			{
				return;
			}
			float num = base.Size.X / base._scaleToUse;
			float num2 = (float)this.BaseValueInt / base.MaxValueFloat * num;
			if (value < this.BaseValueInt)
			{
				this.ChangeFillWidget.SetState("Positive");
				this.ChangeFillWidget.SuggestedWidth = (float)(this.BaseValueInt - value) / base.MaxValueFloat * num;
				this.ChangeFillWidget.PositionXOffset = num2 - this.ChangeFillWidget.SuggestedWidth;
			}
			else if (value > this.BaseValueInt)
			{
				this.ChangeFillWidget.SetState("Negative");
				this.ChangeFillWidget.SuggestedWidth = (float)(value - this.BaseValueInt) / base.MaxValueFloat * num;
				this.ChangeFillWidget.PositionXOffset = num2;
			}
			else
			{
				this.ChangeFillWidget.SetState("Default");
				this.ChangeFillWidget.SuggestedWidth = 0f;
			}
			if (this._handleClicked || this._valueChangedByMouse || this._manuallyIncreased)
			{
				this._manuallyIncreased = false;
				base.OnPropertyChanged(base.ValueInt, "ValueInt");
			}
		}

		private void ChangeFillWidgetUpdated()
		{
			if (this.ChangeFillWidget != null)
			{
				this.ChangeFillWidget.AddState("Negative");
				this.ChangeFillWidget.AddState("Positive");
				this.ChangeFillWidget.HorizontalAlignment = HorizontalAlignment.Left;
			}
		}

		private void BaseValueIntUpdated()
		{
			this.OnValueIntChanged(base.ValueInt);
		}

		[Editor(false)]
		public BrushWidget ChangeFillWidget
		{
			get
			{
				return this._changeFillWidget;
			}
			set
			{
				if (this._changeFillWidget != value)
				{
					this._changeFillWidget = value;
					base.OnPropertyChanged<BrushWidget>(value, "ChangeFillWidget");
					this.ChangeFillWidgetUpdated();
				}
			}
		}

		[Editor(false)]
		public int BaseValueInt
		{
			get
			{
				return this._baseValueInt;
			}
			set
			{
				if (this._baseValueInt != value)
				{
					this._baseValueInt = value;
					base.OnPropertyChanged(value, "BaseValueInt");
					this.BaseValueIntUpdated();
				}
			}
		}

		protected bool _manuallyIncreased;

		private BrushWidget _changeFillWidget;

		private int _baseValueInt;
	}
}
