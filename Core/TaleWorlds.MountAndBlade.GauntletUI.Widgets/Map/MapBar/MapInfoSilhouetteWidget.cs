using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Map.MapBar
{
	// Token: 0x0200010E RID: 270
	public class MapInfoSilhouetteWidget : Widget
	{
		// Token: 0x06000DCC RID: 3532 RVA: 0x00026AE4 File Offset: 0x00024CE4
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

		// Token: 0x170004E8 RID: 1256
		// (get) Token: 0x06000DCD RID: 3533 RVA: 0x00026B3A File Offset: 0x00024D3A
		// (set) Token: 0x06000DCE RID: 3534 RVA: 0x00026B44 File Offset: 0x00024D44
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

		// Token: 0x0400065A RID: 1626
		private string _currentScreen;
	}
}
