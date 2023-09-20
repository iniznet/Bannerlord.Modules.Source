using System;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x020001A2 RID: 418
	public abstract class SettlementSecurityModel : GameModel
	{
		// Token: 0x170006D1 RID: 1745
		// (get) Token: 0x06001A6A RID: 6762
		public abstract int MaximumSecurityInSettlement { get; }

		// Token: 0x170006D2 RID: 1746
		// (get) Token: 0x06001A6B RID: 6763
		public abstract int SecurityDriftMedium { get; }

		// Token: 0x170006D3 RID: 1747
		// (get) Token: 0x06001A6C RID: 6764
		public abstract float MapEventSecurityEffectRadius { get; }

		// Token: 0x170006D4 RID: 1748
		// (get) Token: 0x06001A6D RID: 6765
		public abstract float HideoutClearedSecurityEffectRadius { get; }

		// Token: 0x170006D5 RID: 1749
		// (get) Token: 0x06001A6E RID: 6766
		public abstract int HideoutClearedSecurityGain { get; }

		// Token: 0x170006D6 RID: 1750
		// (get) Token: 0x06001A6F RID: 6767
		public abstract int ThresholdForTaxCorruption { get; }

		// Token: 0x170006D7 RID: 1751
		// (get) Token: 0x06001A70 RID: 6768
		public abstract int ThresholdForHigherTaxCorruption { get; }

		// Token: 0x170006D8 RID: 1752
		// (get) Token: 0x06001A71 RID: 6769
		public abstract int ThresholdForTaxBoost { get; }

		// Token: 0x170006D9 RID: 1753
		// (get) Token: 0x06001A72 RID: 6770
		public abstract int SettlementTaxBoostPercentage { get; }

		// Token: 0x170006DA RID: 1754
		// (get) Token: 0x06001A73 RID: 6771
		public abstract int SettlementTaxPenaltyPercentage { get; }

		// Token: 0x06001A74 RID: 6772
		public abstract float GetLootedNearbyPartySecurityEffect(Town town, float sumOfAttackedPartyStrengths);

		// Token: 0x170006DB RID: 1755
		// (get) Token: 0x06001A75 RID: 6773
		public abstract int ThresholdForNotableRelationBonus { get; }

		// Token: 0x170006DC RID: 1756
		// (get) Token: 0x06001A76 RID: 6774
		public abstract int ThresholdForNotableRelationPenalty { get; }

		// Token: 0x170006DD RID: 1757
		// (get) Token: 0x06001A77 RID: 6775
		public abstract int DailyNotableRelationBonus { get; }

		// Token: 0x170006DE RID: 1758
		// (get) Token: 0x06001A78 RID: 6776
		public abstract int DailyNotableRelationPenalty { get; }

		// Token: 0x170006DF RID: 1759
		// (get) Token: 0x06001A79 RID: 6777
		public abstract int DailyNotablePowerBonus { get; }

		// Token: 0x170006E0 RID: 1760
		// (get) Token: 0x06001A7A RID: 6778
		public abstract int DailyNotablePowerPenalty { get; }

		// Token: 0x06001A7B RID: 6779
		public abstract ExplainedNumber CalculateSecurityChange(Town town, bool includeDescriptions = false);

		// Token: 0x06001A7C RID: 6780
		public abstract float GetNearbyBanditPartyDefeatedSecurityEffect(Town town, float sumOfAttackedPartyStrengths);

		// Token: 0x06001A7D RID: 6781
		public abstract void CalculateGoldGainDueToHighSecurity(Town town, ref ExplainedNumber explainedNumber);

		// Token: 0x06001A7E RID: 6782
		public abstract void CalculateGoldCutDueToLowSecurity(Town town, ref ExplainedNumber explainedNumber);
	}
}
