using System;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x020001A8 RID: 424
	public abstract class ClanTierModel : GameModel
	{
		// Token: 0x170006E7 RID: 1767
		// (get) Token: 0x06001A9A RID: 6810
		public abstract int MinClanTier { get; }

		// Token: 0x170006E8 RID: 1768
		// (get) Token: 0x06001A9B RID: 6811
		public abstract int MaxClanTier { get; }

		// Token: 0x170006E9 RID: 1769
		// (get) Token: 0x06001A9C RID: 6812
		public abstract int MercenaryEligibleTier { get; }

		// Token: 0x170006EA RID: 1770
		// (get) Token: 0x06001A9D RID: 6813
		public abstract int VassalEligibleTier { get; }

		// Token: 0x170006EB RID: 1771
		// (get) Token: 0x06001A9E RID: 6814
		public abstract int BannerEligibleTier { get; }

		// Token: 0x170006EC RID: 1772
		// (get) Token: 0x06001A9F RID: 6815
		public abstract int RebelClanStartingTier { get; }

		// Token: 0x170006ED RID: 1773
		// (get) Token: 0x06001AA0 RID: 6816
		public abstract int CompanionToLordClanStartingTier { get; }

		// Token: 0x06001AA1 RID: 6817
		public abstract int CalculateInitialRenown(Clan clan);

		// Token: 0x06001AA2 RID: 6818
		public abstract int CalculateInitialInfluence(Clan clan);

		// Token: 0x06001AA3 RID: 6819
		public abstract int CalculateTier(Clan clan);

		// Token: 0x06001AA4 RID: 6820
		public abstract ValueTuple<ExplainedNumber, bool> HasUpcomingTier(Clan clan, out TextObject extraExplanation, bool includeDescriptions = false);

		// Token: 0x06001AA5 RID: 6821
		public abstract int GetRequiredRenownForTier(int tier);

		// Token: 0x06001AA6 RID: 6822
		public abstract int GetPartyLimitForTier(Clan clan, int clanTierToCheck);

		// Token: 0x06001AA7 RID: 6823
		public abstract int GetCompanionLimit(Clan clan);
	}
}
