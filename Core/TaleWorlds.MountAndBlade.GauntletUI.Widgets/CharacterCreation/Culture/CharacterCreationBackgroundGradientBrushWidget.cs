using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.CharacterCreation.Culture
{
	public class CharacterCreationBackgroundGradientBrushWidget : BrushWidget
	{
		public CharacterCreationBackgroundGradientBrushWidget(UIContext context)
			: base(context)
		{
		}

		private void SetCultureBackground(string value)
		{
			string factionColorCode = WidgetsMultiplayerHelper.GetFactionColorCode(value, false);
			foreach (Style style in base.Brush.Styles)
			{
				foreach (StyleLayer styleLayer in style.Layers)
				{
					styleLayer.Color = Color.ConvertStringToColor(factionColorCode);
				}
			}
		}

		[Editor(false)]
		public string CurrentCultureId
		{
			get
			{
				return this._currentCultureId;
			}
			set
			{
				if (this._currentCultureId != value)
				{
					this._currentCultureId = value;
					base.OnPropertyChanged<string>(value, "CurrentCultureId");
					this.SetCultureBackground(value);
				}
			}
		}

		private string _currentCultureId;
	}
}
