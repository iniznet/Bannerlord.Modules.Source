using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	// Token: 0x0200002D RID: 45
	public class NumericUpDownWidget : Widget
	{
		// Token: 0x06000284 RID: 644 RVA: 0x00008703 File Offset: 0x00006903
		public NumericUpDownWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000285 RID: 645 RVA: 0x00008722 File Offset: 0x00006922
		private void OnUpButtonClicked(Widget widget)
		{
			this.ChangeValue(1);
		}

		// Token: 0x06000286 RID: 646 RVA: 0x0000872B File Offset: 0x0000692B
		private void OnDownButtonClicked(Widget widget)
		{
			this.ChangeValue(-1);
		}

		// Token: 0x06000287 RID: 647 RVA: 0x00008734 File Offset: 0x00006934
		private void ChangeValue(int changeAmount)
		{
			int num = this.IntValue + changeAmount;
			if ((float)num <= this.MaxValue && (float)num >= this.MinValue)
			{
				this.IntValue = num;
			}
		}

		// Token: 0x06000288 RID: 648 RVA: 0x00008768 File Offset: 0x00006968
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

		// Token: 0x170000E2 RID: 226
		// (get) Token: 0x06000289 RID: 649 RVA: 0x000087C3 File Offset: 0x000069C3
		// (set) Token: 0x0600028A RID: 650 RVA: 0x000087CB File Offset: 0x000069CB
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

		// Token: 0x170000E3 RID: 227
		// (get) Token: 0x0600028B RID: 651 RVA: 0x000087E9 File Offset: 0x000069E9
		// (set) Token: 0x0600028C RID: 652 RVA: 0x000087F4 File Offset: 0x000069F4
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

		// Token: 0x170000E4 RID: 228
		// (get) Token: 0x0600028D RID: 653 RVA: 0x00008853 File Offset: 0x00006A53
		// (set) Token: 0x0600028E RID: 654 RVA: 0x0000885B File Offset: 0x00006A5B
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

		// Token: 0x170000E5 RID: 229
		// (get) Token: 0x0600028F RID: 655 RVA: 0x00008886 File Offset: 0x00006A86
		// (set) Token: 0x06000290 RID: 656 RVA: 0x0000888E File Offset: 0x00006A8E
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

		// Token: 0x170000E6 RID: 230
		// (get) Token: 0x06000291 RID: 657 RVA: 0x000088B2 File Offset: 0x00006AB2
		// (set) Token: 0x06000292 RID: 658 RVA: 0x000088BA File Offset: 0x00006ABA
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

		// Token: 0x170000E7 RID: 231
		// (get) Token: 0x06000293 RID: 659 RVA: 0x000088DE File Offset: 0x00006ADE
		// (set) Token: 0x06000294 RID: 660 RVA: 0x000088E6 File Offset: 0x00006AE6
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

		// Token: 0x170000E8 RID: 232
		// (get) Token: 0x06000295 RID: 661 RVA: 0x00008904 File Offset: 0x00006B04
		// (set) Token: 0x06000296 RID: 662 RVA: 0x0000890C File Offset: 0x00006B0C
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

		// Token: 0x170000E9 RID: 233
		// (get) Token: 0x06000297 RID: 663 RVA: 0x00008972 File Offset: 0x00006B72
		// (set) Token: 0x06000298 RID: 664 RVA: 0x0000897C File Offset: 0x00006B7C
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

		// Token: 0x04000107 RID: 263
		private bool _showOneAdded;

		// Token: 0x04000108 RID: 264
		private float _minValue;

		// Token: 0x04000109 RID: 265
		private float _maxValue;

		// Token: 0x0400010A RID: 266
		private int _intValue = int.MinValue;

		// Token: 0x0400010B RID: 267
		private float _value = float.MinValue;

		// Token: 0x0400010C RID: 268
		private TextWidget _textWidget;

		// Token: 0x0400010D RID: 269
		private ButtonWidget _upButton;

		// Token: 0x0400010E RID: 270
		private ButtonWidget _downButton;
	}
}
