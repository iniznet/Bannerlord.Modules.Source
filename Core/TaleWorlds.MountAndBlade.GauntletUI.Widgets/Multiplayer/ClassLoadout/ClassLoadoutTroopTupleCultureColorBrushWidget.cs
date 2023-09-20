using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.ClassLoadout
{
	public class ClassLoadoutTroopTupleCultureColorBrushWidget : BrushWidget
	{
		public ClassLoadoutTroopTupleCultureColorBrushWidget(UIContext context)
			: base(context)
		{
		}

		private void UpdateColor()
		{
			foreach (Style style in base.Brush.Styles)
			{
				foreach (StyleLayer styleLayer in style.Layers)
				{
					styleLayer.Color = this.CultureColor;
				}
			}
		}

		public Color CultureColor
		{
			get
			{
				return this._cultureColor;
			}
			set
			{
				if (value != this._cultureColor)
				{
					this._cultureColor = value;
					base.OnPropertyChanged(value, "CultureColor");
					this.UpdateColor();
				}
			}
		}

		private Color _cultureColor;
	}
}
