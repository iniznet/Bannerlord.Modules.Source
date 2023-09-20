using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace TaleWorlds.MountAndBlade.GauntletUI.Widgets.Multiplayer.Lobby.Armory
{
	// Token: 0x020000A6 RID: 166
	public class MultiplayerLobbyArmoryCosmeticItemBrushWidget : BrushWidget
	{
		// Token: 0x060008A9 RID: 2217 RVA: 0x00018EB1 File Offset: 0x000170B1
		public MultiplayerLobbyArmoryCosmeticItemBrushWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060008AA RID: 2218 RVA: 0x00018EBC File Offset: 0x000170BC
		private void OnRarityChanged()
		{
			switch (this.Rarity)
			{
			case 0:
			case 1:
				base.Brush = base.Context.GetBrush("MPLobby.Armory.CosmeticButton.Common");
				return;
			case 2:
				base.Brush = base.Context.GetBrush("MPLobby.Armory.CosmeticButton.Rare");
				return;
			case 3:
				base.Brush = base.Context.GetBrush("MPLobby.Armory.CosmeticButton.Unique");
				return;
			default:
				return;
			}
		}

		// Token: 0x17000309 RID: 777
		// (get) Token: 0x060008AB RID: 2219 RVA: 0x00018F2B File Offset: 0x0001712B
		// (set) Token: 0x060008AC RID: 2220 RVA: 0x00018F33 File Offset: 0x00017133
		[Editor(false)]
		public int Rarity
		{
			get
			{
				return this._rarity;
			}
			set
			{
				if (value != this._rarity)
				{
					this._rarity = value;
					base.OnPropertyChanged(value, "Rarity");
					this.OnRarityChanged();
				}
			}
		}

		// Token: 0x040003F3 RID: 1011
		private const string BaseBrushName = "MPLobby.Armory.CosmeticButton";

		// Token: 0x040003F4 RID: 1012
		private int _rarity;
	}
}
