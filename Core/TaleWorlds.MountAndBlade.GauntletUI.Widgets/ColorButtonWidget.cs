using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets
{
	public class ColorButtonWidget : ButtonWidget
	{
		public ColorButtonWidget(UIContext context)
			: base(context)
		{
		}

		private void ApplyStringColorToBrush(string color)
		{
			Color color2 = Color.ConvertStringToColor(color);
			foreach (Style style in base.Brush.Styles)
			{
				foreach (StyleLayer styleLayer in style.Layers)
				{
					styleLayer.Color = color2;
				}
			}
		}

		[Editor(false)]
		public string ColorToApply
		{
			get
			{
				return this._colorToApply;
			}
			set
			{
				if (this._colorToApply != value)
				{
					this._colorToApply = value;
					base.OnPropertyChanged<string>(value, "ColorToApply");
					if (!string.IsNullOrEmpty(value))
					{
						this.ApplyStringColorToBrush(value);
					}
				}
			}
		}

		private string _colorToApply;
	}
}
