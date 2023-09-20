using System;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x020001A1 RID: 417
	public abstract class SettlementLoyaltyModel : GameModel
	{
		// Token: 0x170006BA RID: 1722
		// (get) Token: 0x06001A4F RID: 6735
		public abstract int SettlementLoyaltyChangeDueToSecurityThreshold { get; }

		// Token: 0x170006BB RID: 1723
		// (get) Token: 0x06001A50 RID: 6736
		public abstract int MaximumLoyaltyInSettlement { get; }

		// Token: 0x170006BC RID: 1724
		// (get) Token: 0x06001A51 RID: 6737
		public abstract int LoyaltyDriftMedium { get; }

		// Token: 0x170006BD RID: 1725
		// (get) Token: 0x06001A52 RID: 6738
		public abstract float HighLoyaltyProsperityEffect { get; }

		// Token: 0x170006BE RID: 1726
		// (get) Token: 0x06001A53 RID: 6739
		public abstract int LowLoyaltyProsperityEffect { get; }

		// Token: 0x170006BF RID: 1727
		// (get) Token: 0x06001A54 RID: 6740
		public abstract int MilitiaBoostPercentage { get; }

		// Token: 0x170006C0 RID: 1728
		// (get) Token: 0x06001A55 RID: 6741
		public abstract float HighSecurityLoyaltyEffect { get; }

		// Token: 0x170006C1 RID: 1729
		// (get) Token: 0x06001A56 RID: 6742
		public abstract float LowSecurityLoyaltyEffect { get; }

		// Token: 0x170006C2 RID: 1730
		// (get) Token: 0x06001A57 RID: 6743
		public abstract float GovernorSameCultureLoyaltyEffect { get; }

		// Token: 0x170006C3 RID: 1731
		// (get) Token: 0x06001A58 RID: 6744
		public abstract float GovernorDifferentCultureLoyaltyEffect { get; }

		// Token: 0x170006C4 RID: 1732
		// (get) Token: 0x06001A59 RID: 6745
		public abstract float SettlementOwnerDifferentCultureLoyaltyEffect { get; }

		// Token: 0x170006C5 RID: 1733
		// (get) Token: 0x06001A5A RID: 6746
		public abstract int ThresholdForTaxBoost { get; }

		// Token: 0x170006C6 RID: 1734
		// (get) Token: 0x06001A5B RID: 6747
		public abstract int RebellionStartLoyaltyThreshold { get; }

		// Token: 0x170006C7 RID: 1735
		// (get) Token: 0x06001A5C RID: 6748
		public abstract int ThresholdForTaxCorruption { get; }

		// Token: 0x170006C8 RID: 1736
		// (get) Token: 0x06001A5D RID: 6749
		public abstract int ThresholdForHigherTaxCorruption { get; }

		// Token: 0x170006C9 RID: 1737
		// (get) Token: 0x06001A5E RID: 6750
		public abstract int ThresholdForProsperityBoost { get; }

		// Token: 0x170006CA RID: 1738
		// (get) Token: 0x06001A5F RID: 6751
		public abstract int ThresholdForProsperityPenalty { get; }

		// Token: 0x170006CB RID: 1739
		// (get) Token: 0x06001A60 RID: 6752
		public abstract int AdditionalStarvationPenaltyStartDay { get; }

		// Token: 0x170006CC RID: 1740
		// (get) Token: 0x06001A61 RID: 6753
		public abstract int AdditionalStarvationLoyaltyEffect { get; }

		// Token: 0x170006CD RID: 1741
		// (get) Token: 0x06001A62 RID: 6754
		public abstract int RebelliousStateStartLoyaltyThreshold { get; }

		// Token: 0x170006CE RID: 1742
		// (get) Token: 0x06001A63 RID: 6755
		public abstract int LoyaltyBoostAfterRebellionStartValue { get; }

		// Token: 0x170006CF RID: 1743
		// (get) Token: 0x06001A64 RID: 6756
		public abstract float ThresholdForNotableRelationBonus { get; }

		// Token: 0x170006D0 RID: 1744
		// (get) Token: 0x06001A65 RID: 6757
		public abstract int DailyNotableRelationBonus { get; }

		// Token: 0x06001A66 RID: 6758
		public abstract ExplainedNumber CalculateLoyaltyChange(Town town, bool includeDescriptions = false);

		// Token: 0x06001A67 RID: 6759
		public abstract void CalculateGoldGainDueToHighLoyalty(Town town, ref ExplainedNumber explainedNumber);

		// Token: 0x06001A68 RID: 6760
		public abstract void CalculateGoldCutDueToLowLoyalty(Town town, ref ExplainedNumber explainedNumber);
	}
}
