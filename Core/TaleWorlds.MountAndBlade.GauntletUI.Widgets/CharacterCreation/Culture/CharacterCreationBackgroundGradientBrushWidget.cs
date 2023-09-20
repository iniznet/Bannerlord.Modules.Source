using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.CharacterCreation.Culture
{
	public class CharacterCreationBackgroundGradientBrushWidget : BrushWidget
	{
		public CharacterCreationBackgroundGradientBrushWidget(UIContext context)
			: base(context)
		{
		}

		private void SetCultureBackground(Color cultureColor1)
		{
			foreach (Style style in base.Brush.Styles)
			{
				foreach (StyleLayer styleLayer in style.Layers)
				{
					styleLayer.Color = cultureColor1;
				}
			}
		}

		[Editor(false)]
		public Color CultureColor1
		{
			get
			{
				return this._cultureColor1;
			}
			set
			{
				if (this._cultureColor1 != value)
				{
					this._cultureColor1 = value;
					base.OnPropertyChanged(value, "CultureColor1");
					this.SetCultureBackground(value);
				}
			}
		}

		private Color _cultureColor1;
	}
}
