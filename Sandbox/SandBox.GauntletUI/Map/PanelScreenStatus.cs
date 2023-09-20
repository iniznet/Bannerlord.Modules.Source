using System;
using TaleWorlds.ScreenSystem;

namespace SandBox.GauntletUI.Map
{
	// Token: 0x02000021 RID: 33
	public readonly struct PanelScreenStatus
	{
		// Token: 0x06000145 RID: 325 RVA: 0x0000A344 File Offset: 0x00008544
		public PanelScreenStatus(ScreenBase screen)
		{
			this.IsCharacterScreenOpen = false;
			this.IsPartyScreenOpen = false;
			this.IsQuestsScreenOpen = false;
			this.IsInventoryScreenOpen = false;
			this.IsClanScreenOpen = false;
			this.IsKingdomScreenOpen = false;
			this.IsAnyPanelScreenOpen = true;
			this.IsCurrentScreenLocksNavigation = false;
			if (screen is GauntletCharacterDeveloperScreen)
			{
				this.IsCharacterScreenOpen = true;
				return;
			}
			if (screen is GauntletPartyScreen)
			{
				this.IsPartyScreenOpen = true;
				return;
			}
			if (screen is GauntletQuestsScreen)
			{
				this.IsQuestsScreenOpen = true;
				return;
			}
			if (screen is GauntletInventoryScreen)
			{
				this.IsInventoryScreenOpen = true;
				return;
			}
			if (screen is GauntletClanScreen)
			{
				this.IsClanScreenOpen = true;
				return;
			}
			GauntletKingdomScreen gauntletKingdomScreen;
			if ((gauntletKingdomScreen = screen as GauntletKingdomScreen) != null)
			{
				this.IsKingdomScreenOpen = true;
				this.IsCurrentScreenLocksNavigation = gauntletKingdomScreen != null && gauntletKingdomScreen.IsMakingDecision;
				return;
			}
			this.IsAnyPanelScreenOpen = false;
		}

		// Token: 0x040000A1 RID: 161
		public readonly bool IsCharacterScreenOpen;

		// Token: 0x040000A2 RID: 162
		public readonly bool IsPartyScreenOpen;

		// Token: 0x040000A3 RID: 163
		public readonly bool IsQuestsScreenOpen;

		// Token: 0x040000A4 RID: 164
		public readonly bool IsInventoryScreenOpen;

		// Token: 0x040000A5 RID: 165
		public readonly bool IsClanScreenOpen;

		// Token: 0x040000A6 RID: 166
		public readonly bool IsKingdomScreenOpen;

		// Token: 0x040000A7 RID: 167
		public readonly bool IsAnyPanelScreenOpen;

		// Token: 0x040000A8 RID: 168
		public readonly bool IsCurrentScreenLocksNavigation;
	}
}
