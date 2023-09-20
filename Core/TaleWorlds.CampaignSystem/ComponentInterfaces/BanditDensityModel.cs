using System;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x0200018D RID: 397
	public abstract class BanditDensityModel : GameModel
	{
		// Token: 0x170006A1 RID: 1697
		// (get) Token: 0x060019D0 RID: 6608
		public abstract int NumberOfMaximumLooterParties { get; }

		// Token: 0x170006A2 RID: 1698
		// (get) Token: 0x060019D1 RID: 6609
		public abstract int NumberOfMinimumBanditPartiesInAHideoutToInfestIt { get; }

		// Token: 0x170006A3 RID: 1699
		// (get) Token: 0x060019D2 RID: 6610
		public abstract int NumberOfMaximumBanditPartiesInEachHideout { get; }

		// Token: 0x170006A4 RID: 1700
		// (get) Token: 0x060019D3 RID: 6611
		public abstract int NumberOfMaximumBanditPartiesAroundEachHideout { get; }

		// Token: 0x170006A5 RID: 1701
		// (get) Token: 0x060019D4 RID: 6612
		public abstract int NumberOfMaximumHideoutsAtEachBanditFaction { get; }

		// Token: 0x170006A6 RID: 1702
		// (get) Token: 0x060019D5 RID: 6613
		public abstract int NumberOfInitialHideoutsAtEachBanditFaction { get; }

		// Token: 0x170006A7 RID: 1703
		// (get) Token: 0x060019D6 RID: 6614
		public abstract int NumberOfMinimumBanditTroopsInHideoutMission { get; }

		// Token: 0x170006A8 RID: 1704
		// (get) Token: 0x060019D7 RID: 6615
		public abstract int NumberOfMaximumTroopCountForFirstFightInHideout { get; }

		// Token: 0x170006A9 RID: 1705
		// (get) Token: 0x060019D8 RID: 6616
		public abstract int NumberOfMaximumTroopCountForBossFightInHideout { get; }

		// Token: 0x170006AA RID: 1706
		// (get) Token: 0x060019D9 RID: 6617
		public abstract float SpawnPercentageForFirstFightInHideoutMission { get; }

		// Token: 0x060019DA RID: 6618
		public abstract int GetPlayerMaximumTroopCountForHideoutMission(MobileParty party);
	}
}
