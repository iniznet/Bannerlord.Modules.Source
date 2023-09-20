using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby.Armory
{
	// Token: 0x020000AA RID: 170
	public class MultiplayerLobbyClassFilterClassItemWidget : ToggleStateButtonWidget
	{
		// Token: 0x060008C5 RID: 2245 RVA: 0x000192AA File Offset: 0x000174AA
		public MultiplayerLobbyClassFilterClassItemWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060008C6 RID: 2246 RVA: 0x000192B4 File Offset: 0x000174B4
		private void SetFactionColor()
		{
			if (this.FactionColorWidget == null || string.IsNullOrEmpty(this.Culture))
			{
				return;
			}
			string factionColorCode = WidgetsMultiplayerHelper.GetFactionColorCode(this.Culture.ToLower(), false);
			this.FactionColorWidget.Color = Color.ConvertStringToColor(factionColorCode);
		}

		// Token: 0x060008C7 RID: 2247 RVA: 0x000192FC File Offset: 0x000174FC
		private void UpdateIcon()
		{
			if (string.IsNullOrEmpty(this.TroopType) || this._iconWidget == null)
			{
				return;
			}
			this.IconWidget.Sprite = base.Context.SpriteData.GetSprite("General\\compass\\" + this.TroopType);
		}

		// Token: 0x17000313 RID: 787
		// (get) Token: 0x060008C8 RID: 2248 RVA: 0x0001934A File Offset: 0x0001754A
		// (set) Token: 0x060008C9 RID: 2249 RVA: 0x00019352 File Offset: 0x00017552
		[Editor(false)]
		public Widget FactionColorWidget
		{
			get
			{
				return this._factionColorWidget;
			}
			set
			{
				if (this._factionColorWidget != value)
				{
					this._factionColorWidget = value;
					base.OnPropertyChanged<Widget>(value, "FactionColorWidget");
					this.SetFactionColor();
				}
			}
		}

		// Token: 0x17000314 RID: 788
		// (get) Token: 0x060008CA RID: 2250 RVA: 0x00019376 File Offset: 0x00017576
		// (set) Token: 0x060008CB RID: 2251 RVA: 0x0001937E File Offset: 0x0001757E
		[Editor(false)]
		public string Culture
		{
			get
			{
				return this._culture;
			}
			set
			{
				if (this._culture != value)
				{
					this._culture = value;
					base.OnPropertyChanged<string>(value, "Culture");
					this.SetFactionColor();
				}
			}
		}

		// Token: 0x17000315 RID: 789
		// (get) Token: 0x060008CC RID: 2252 RVA: 0x000193A7 File Offset: 0x000175A7
		// (set) Token: 0x060008CD RID: 2253 RVA: 0x000193AF File Offset: 0x000175AF
		[DataSourceProperty]
		public string TroopType
		{
			get
			{
				return this._troopType;
			}
			set
			{
				if (value != this._troopType)
				{
					this._troopType = value;
					base.OnPropertyChanged<string>(value, "TroopType");
					this.UpdateIcon();
				}
			}
		}

		// Token: 0x17000316 RID: 790
		// (get) Token: 0x060008CE RID: 2254 RVA: 0x000193D8 File Offset: 0x000175D8
		// (set) Token: 0x060008CF RID: 2255 RVA: 0x000193E0 File Offset: 0x000175E0
		[DataSourceProperty]
		public Widget IconWidget
		{
			get
			{
				return this._iconWidget;
			}
			set
			{
				if (value != this._iconWidget)
				{
					this._iconWidget = value;
					base.OnPropertyChanged<Widget>(value, "IconWidget");
					this.UpdateIcon();
				}
			}
		}

		// Token: 0x040003FE RID: 1022
		private Widget _factionColorWidget;

		// Token: 0x040003FF RID: 1023
		private string _culture;

		// Token: 0x04000400 RID: 1024
		private string _troopType;

		// Token: 0x04000401 RID: 1025
		private Widget _iconWidget;
	}
}
