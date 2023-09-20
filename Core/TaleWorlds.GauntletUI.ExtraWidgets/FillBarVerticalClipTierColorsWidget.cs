using System;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace TaleWorlds.GauntletUI.ExtraWidgets
{
	public class FillBarVerticalClipTierColorsWidget : FillBarVerticalWidget
	{
		public FillBarVerticalClipTierColorsWidget(UIContext context)
			: base(context)
		{
		}

		protected override void OnRender(TwoDimensionContext twoDimensionContext, TwoDimensionDrawContext drawContext)
		{
			base.OnRender(twoDimensionContext, drawContext);
			float num = (float)base.InitialAmount / base.MaxAmountAsFloat;
			Color color = new Color(0f, 0f, 0f, 0f);
			if (num == 1f)
			{
				base.FillWidget.Color = Color.ConvertStringToColor(this.MaxedColor);
				return;
			}
			float num2 = this._maxThreshold;
			float num3 = this._maxThreshold;
			Color color2 = Color.ConvertStringToColor(this.MaxedColor);
			Color color3 = Color.ConvertStringToColor(this.MaxedColor);
			if (num >= this._highThreshold && num < this._maxThreshold)
			{
				num2 = this._highThreshold;
				num3 = this._maxThreshold;
				color2 = Color.ConvertStringToColor(this.HighColor);
				color3 = Color.ConvertStringToColor(this.MaxedColor);
			}
			else if (num >= this._mediumThreshold && num < this._highThreshold)
			{
				num2 = this._mediumThreshold;
				num3 = this._highThreshold;
				color2 = Color.ConvertStringToColor(this.MediumColor);
				color3 = Color.ConvertStringToColor(this.HighColor);
			}
			else if (num >= this._lowThreshold && num < this._mediumThreshold)
			{
				num2 = this._lowThreshold;
				num3 = this._mediumThreshold;
				color2 = Color.ConvertStringToColor(this.LowColor);
				color3 = Color.ConvertStringToColor(this.MediumColor);
			}
			float num4 = (num - num2) / (num3 - num2);
			color = Color.Lerp(color2, color3, num4);
			base.FillWidget.Color = color;
		}

		[Editor(false)]
		public string MaxedColor
		{
			get
			{
				return this._maxedColor;
			}
			set
			{
				if (value != this._maxedColor)
				{
					this._maxedColor = value;
					base.OnPropertyChanged<string>(value, "MaxedColor");
				}
			}
		}

		[Editor(false)]
		public string HighColor
		{
			get
			{
				return this._highColor;
			}
			set
			{
				if (value != this._highColor)
				{
					this._highColor = value;
					base.OnPropertyChanged<string>(value, "HighColor");
				}
			}
		}

		[Editor(false)]
		public string MediumColor
		{
			get
			{
				return this._mediumColor;
			}
			set
			{
				if (value != this._mediumColor)
				{
					this._mediumColor = value;
					base.OnPropertyChanged<string>(value, "MediumColor");
				}
			}
		}

		[Editor(false)]
		public string LowColor
		{
			get
			{
				return this._lowColor;
			}
			set
			{
				if (value != this._lowColor)
				{
					this._lowColor = value;
					base.OnPropertyChanged<string>(value, "LowColor");
				}
			}
		}

		private readonly float _maxThreshold = 1f;

		private readonly float _highThreshold = 0.6f;

		private readonly float _mediumThreshold = 0.35f;

		private readonly float _lowThreshold;

		private string _maxedColor;

		private string _highColor;

		private string _mediumColor;

		private string _lowColor;
	}
}
