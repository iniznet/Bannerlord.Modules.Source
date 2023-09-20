using System;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x02000183 RID: 387
	public abstract class MinorFactionsModel : GameModel
	{
		// Token: 0x1700068E RID: 1678
		// (get) Token: 0x0600196C RID: 6508
		public abstract float DailyMinorFactionHeroSpawnChance { get; }

		// Token: 0x1700068F RID: 1679
		// (get) Token: 0x0600196D RID: 6509
		public abstract int MinorFactionHeroLimit { get; }

		// Token: 0x0600196E RID: 6510
		public abstract int GetMercenaryAwardFactorToJoinKingdom(Clan mercenaryClan, Kingdom kingdom, bool neededAmountForClanToJoinCalculation = false);
	}
}
