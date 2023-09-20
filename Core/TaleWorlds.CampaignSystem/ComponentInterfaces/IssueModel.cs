using System;
using TaleWorlds.CampaignSystem.Issues;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x020001C0 RID: 448
	public abstract class IssueModel : GameModel
	{
		// Token: 0x06001B2F RID: 6959
		public abstract float GetIssueDifficultyMultiplier();

		// Token: 0x17000709 RID: 1801
		// (get) Token: 0x06001B30 RID: 6960
		public abstract int IssueOwnerCoolDownInDays { get; }

		// Token: 0x06001B31 RID: 6961
		public abstract void GetIssueEffectsOfSettlement(IssueEffect issueEffect, Settlement settlement, ref ExplainedNumber explainedNumber);

		// Token: 0x06001B32 RID: 6962
		public abstract void GetIssueEffectOfHero(IssueEffect issueEffect, Hero hero, ref ExplainedNumber explainedNumber);

		// Token: 0x06001B33 RID: 6963
		public abstract void GetIssueEffectOfClan(IssueEffect issueEffect, Clan clan, ref ExplainedNumber explainedNumber);

		// Token: 0x06001B34 RID: 6964
		public abstract ValueTuple<int, int> GetCausalityForHero(Hero alternativeSolutionHero, IssueBase issue);

		// Token: 0x06001B35 RID: 6965
		public abstract float GetFailureRiskForHero(Hero alternativeSolutionHero, IssueBase issue);

		// Token: 0x06001B36 RID: 6966
		public abstract CampaignTime GetDurationOfResolutionForHero(Hero alternativeSolutionHero, IssueBase issue);

		// Token: 0x06001B37 RID: 6967
		public abstract int GetTroopsRequiredForHero(Hero alternativeSolutionHero, IssueBase issue);

		// Token: 0x06001B38 RID: 6968
		public abstract ValueTuple<SkillObject, int> GetIssueAlternativeSolutionSkill(Hero hero, IssueBase issue);
	}
}
