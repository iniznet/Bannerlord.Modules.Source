using System;
using StoryMode.StoryModePhases;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Party;

namespace StoryMode.GameComponents
{
	public class StoryModeBattleRewardModel : DefaultBattleRewardModel
	{
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
