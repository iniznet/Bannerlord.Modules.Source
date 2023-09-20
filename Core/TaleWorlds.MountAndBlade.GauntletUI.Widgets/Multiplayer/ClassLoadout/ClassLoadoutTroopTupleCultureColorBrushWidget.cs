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
			if (string.IsNullOrEmpty(this.FactionCode))
			{
				return;
			}
			string factionColorCode = WidgetsMultiplayerHelper.GetFactionColorCode(this.FactionCode.ToLower(), this.UseSecondary);
			foreach (Style style in base.Brush.Styles)
			{
				foreach (StyleLayer styleLayer in style.Layers)
				{
					styleLayer.Color = Color.ConvertStringToColor(factionColorCode);
				}
			}
		}

		public string FactionCode
		{
			get
			{
				return this._factionCode;
			}
			set
			{
				if (value != this._factionCode)
				{
					this._factionCode = value;
					base.OnPropertyChanged<string>(value, "FactionCode");
					this.UpdateColor();
				}
			}
		}

		public bool UseSecondary
		{
			get
			{
				return this._useSecondary;
			}
			set
			{
				if (value != this._useSecondary)
				{
					this._useSecondary = value;
					base.OnPropertyChanged(value, "UseSecondary");
					this.UpdateColor();
				}
			}
		}

		private string _factionCode;

		private bool _useSecondary;
	}
}
