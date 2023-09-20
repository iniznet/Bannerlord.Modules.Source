using System;
using StoryMode.StoryModePhases;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Party;

namespace StoryMode.GameComponents
{
	// Token: 0x0200003C RID: 60
	public class StoryModeBattleRewardModel : DefaultBattleRewardModel
	{
		// Token: 0x060003B0 RID: 944 RVA: 0x000171EC File Offset: 0x000153EC
		public override ExplainedNumber CalculateRenownGain(PartyBase party, float renownValueOfBattle, float contributionShare)
		{
			if (TutorialPhase.Instance != null && !TutorialPhase.Instance.IsCompleted && party == PartyBase.MainParty)
			{
				return default(ExplainedNumber);
			}
			return base.CalculateRenownGain(party, renownValueOfBattle, contributionShare);
		}
	}
}
