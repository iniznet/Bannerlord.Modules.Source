using System;
using TaleWorlds.ScreenSystem;

namespace SandBox.GauntletUI.Map
{
	public readonly struct PanelScreenStatus
	{
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

		public readonly bool IsCharacterScreenOpen;

		public readonly bool IsPartyScreenOpen;

		public readonly bool IsQuestsScreenOpen;

		public readonly bool IsInventoryScreenOpen;

		public readonly bool IsClanScreenOpen;

		public readonly bool IsKingdomScreenOpen;

		public readonly bool IsAnyPanelScreenOpen;

		public readonly bool IsCurrentScreenLocksNavigation;
	}
}
