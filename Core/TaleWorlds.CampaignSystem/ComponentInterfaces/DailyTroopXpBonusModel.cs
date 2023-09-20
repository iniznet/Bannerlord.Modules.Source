using System;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x020001B0 RID: 432
	public abstract class DailyTroopXpBonusModel : GameModel
	{
		// Token: 0x06001ACA RID: 6858
		public abstract int CalculateDailyTroopXpBonus(Town town);

		// Token: 0x06001ACB RID: 6859
		public abstract float CalculateGarrisonXpBonusMultiplier(Town town);
	}
}
