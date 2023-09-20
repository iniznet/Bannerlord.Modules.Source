using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	public class NumericUpDownWidget : Widget
	{
		public NumericUpDownWidget(UIContext context)
			: base(context)
		{
		}

		private void OnUpButtonClicked(Widget widget)
		{
			this.ChangeValue(1);
		}

		private void OnDownButtonClicked(Widget widget)
		{
			this.ChangeValue(-1);
		}

		private void ChangeValue(int changeAmount)
		{
			int num = this.IntValue + changeAmount;
			if ((float)num <= this.MaxValue && (float)num >= this.MinValue)
			{
				this.IntValue = num;
			}
		}

		private void UpdateControlButtonsEnabled()
		{
			if (this.UpButton != null)
			{
				this.UpButton.IsEnabled = (float)(this._intValue + 1) <= this.MaxValue;
			}
			if (this.DownButton != null)
			{
				this.DownButton.IsEnabled = (float)(this._intValue - 1) >= this.MinValue;
			}
		}

		[Editor(false)]
		public bool ShowOneAdded
		{
			get
			{
				return this._showOneAdded;
			}
			set
			{
				if (this._showOneAdded != value)
				{
					this._showOneAdded = value;
					base.OnPropertyChanged(value, "ShowOneAdded");
				}
			}
		}

		[Editor(false)]
		public int IntValue
		{
			get
			{
				return this._intValue;
			}
			set
			{
				if (this._intValue != value)
				{
					this._intValue = value;
					this.Value = (float)this._intValue;
					base.OnPropertyChanged(value, "IntValue");
					this._textWidget.IntText = (this.ShowOneAdded ? (this.IntValue + 1) : this.IntValue);
					this.UpdateControlButtonsEnabled();
				}
			}
		}

		[Editor(false)]
		public float Value
		{
			get
			{
				return this._value;
			}
			set
			{
				if (this._value != value)
				{
					this._value = value;
					this.IntValue = (int)this._value;
					base.OnPropertyChanged(value, "Value");
				}
			}
		}

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
					this.UpdateControlButtonsEnabled();
				}
			}
		}

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
					this.UpdateControlButtonsEnabled();
				}
			}
		}

		[Editor(false)]
		public TextWidget TextWidget
		{
			get
			{
				return this._textWidget;
			}
			set
			{
				if (this._textWidget != value)
				{
					this._textWidget = value;
					base.OnPropertyChanged<TextWidget>(value, "TextWidget");
				}
			}
		}

		[Editor(false)]
		public ButtonWidget UpButton
		{
			get
			{
				return this._upButton;
			}
			set
			{
				if (this._upButton != value)
				{
					this._upButton = value;
					base.OnPropertyChanged<ButtonWidget>(value, "UpButton");
					if (value != null && !this._upButton.ClickEventHandlers.Contains(new Action<Widget>(this.OnUpButtonClicked)))
					{
						this._upButton.ClickEventHandlers.Add(new Action<Widget>(this.OnUpButtonClicked));
					}
				}
			}
		}

		[Editor(false)]
		public ButtonWidget DownButton
		{
			get
			{
				return this._downButton;
			}
			set
			{
				if (this._downButton != value)
				{
					this._downButton = value;
					base.OnPropertyChanged<ButtonWidget>(value, "DownButton");
					if (value != null && !this._downButton.ClickEventHandlers.Contains(new Action<Widget>(this.OnDownButtonClicked)))
					{
						this._downButton.ClickEventHandlers.Add(new Action<Widget>(this.OnDownButtonClicked));
					}
				}
			}
		}

		private bool _showOneAdded;

		private float _minValue;

		private float _maxValue;

		private int _intValue = int.MinValue;

		private float _value = float.MinValue;

		private TextWidget _textWidget;

		private ButtonWidget _upButton;

		private ButtonWidget _downButton;
	}
}
