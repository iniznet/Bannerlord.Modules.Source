using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Map.MapBar
{
	public class MapInfoSilhouetteWidget : Widget
	{
		public MapInfoSilhouetteWidget(UIContext context)
			: base(context)
		{
			base.AddState("MapScreen");
			base.AddState("InventoryGauntletScreen");
			base.AddState("GauntletPartyScreen");
			base.AddState("GauntletCharacterDeveloperScreen");
			base.AddState("GauntletClanScreen");
			base.AddState("GauntletQuestsScreen");
		}

		[Editor(false)]
		public string CurrentScreen
		{
			get
			{
				return this._currentScreen;
			}
			set
			{
				if (this._currentScreen != value)
				{
					this._currentScreen = value;
					if (base.ContainsState(this._currentScreen))
					{
						this.SetState(this._currentScreen);
					}
					else
					{
						this.SetState("Default");
					}
					base.OnPropertyChanged<string>(value, "CurrentScreen");
				}
			}
		}

		private string _currentScreen;
	}
}
