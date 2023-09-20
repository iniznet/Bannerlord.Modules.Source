using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby.Armory
{
	// Token: 0x020000A9 RID: 169
	public class MultiplayerLobbyArmoryCosmeticTierVisualBrushWidget : BrushWidget
	{
		// Token: 0x060008C1 RID: 2241 RVA: 0x0001921F File Offset: 0x0001741F
		public MultiplayerLobbyArmoryCosmeticTierVisualBrushWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060008C2 RID: 2242 RVA: 0x00019230 File Offset: 0x00017430
		private void UpdateVisual()
		{
			switch (this._rarity)
			{
			case 0:
			case 1:
				this.SetState("Common");
				return;
			case 2:
				this.SetState("Rare");
				return;
			case 3:
				this.SetState("Unique");
				return;
			default:
				return;
			}
		}

		// Token: 0x17000312 RID: 786
		// (get) Token: 0x060008C3 RID: 2243 RVA: 0x0001927E File Offset: 0x0001747E
		// (set) Token: 0x060008C4 RID: 2244 RVA: 0x00019286 File Offset: 0x00017486
		[Editor(false)]
		public int Rarity
		{
			get
			{
				return this._rarity;
			}
			set
			{
				if (this._rarity != value)
				{
					this._rarity = value;
					base.OnPropertyChanged(value, "Rarity");
					this.UpdateVisual();
				}
			}
		}

		// Token: 0x040003FD RID: 1021
		private int _rarity = -1;
	}
}
