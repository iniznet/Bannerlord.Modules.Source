using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	public class RelationTextWidget : TextWidget
	{
		public RelationTextWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this._isVisualsDirty)
			{
				base.Text = ((this.Amount > 0) ? ("+" + this.Amount.ToString()) : this.Amount.ToString());
				if (this.Amount > 0)
				{
					base.Brush.FontColor = this.PositiveColor;
				}
				else if (this.Amount < 0)
				{
					base.Brush.FontColor = this.NegativeColor;
				}
				else
				{
					base.Brush.FontColor = this.ZeroColor;
				}
				this._isVisualsDirty = false;
			}
		}

		[Editor(false)]
		public int Amount
		{
			get
			{
				return this._amount;
			}
			set
			{
				if (this._amount != value)
				{
					this._amount = value;
					base.OnPropertyChanged(value, "Amount");
					this._isVisualsDirty = true;
				}
			}
		}

		[Editor(false)]
		public Color ZeroColor
		{
			get
			{
				return this._zeroColor;
			}
			set
			{
				if (value != this._zeroColor)
				{
					this._zeroColor = value;
					base.OnPropertyChanged(value, "ZeroColor");
				}
			}
		}

		[Editor(false)]
		public Color PositiveColor
		{
			get
			{
				return this._positiveColor;
			}
			set
			{
				if (value != this._positiveColor)
				{
					this._positiveColor = value;
					base.OnPropertyChanged(value, "PositiveColor");
				}
			}
		}

		[Editor(false)]
		public Color NegativeColor
		{
			get
			{
				return this._negativeColor;
			}
			set
			{
				if (value != this._negativeColor)
				{
					this._negativeColor = value;
					base.OnPropertyChanged(value, "NegativeColor");
				}
			}
		}

		private bool _isVisualsDirty = true;

		private int _amount;

		private Color _zeroColor;

		private Color _positiveColor;

		private Color _negativeColor;
	}
}
