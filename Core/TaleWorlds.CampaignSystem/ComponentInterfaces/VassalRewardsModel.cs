using System;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x020001A7 RID: 423
	public abstract class VassalRewardsModel : GameModel
	{
		// Token: 0x170006E5 RID: 1765
		// (get) Token: 0x06001A95 RID: 6805
		public abstract float InfluenceReward { get; }

		// Token: 0x170006E6 RID: 1766
		// (get) Token: 0x06001A96 RID: 6806
		public abstract int RelationRewardWithLeader { get; }

		// Token: 0x06001A97 RID: 6807
		public abstract TroopRoster GetTroopRewardsForJoiningKingdom(Kingdom kingdom);

		// Token: 0x06001A98 RID: 6808
		public abstract ItemRoster GetEquipmentRewardsForJoiningKingdom(Kingdom kingdom);
	}
}
