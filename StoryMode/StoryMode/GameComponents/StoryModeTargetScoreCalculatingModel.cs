using System;
using StoryMode.StoryModePhases;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;

namespace StoryMode.GameComponents
{
	public class StoryModeTargetScoreCalculatingModel : DefaultTargetScoreCalculatingModel
	{
		public override float GetTargetScoreForFaction(Settlement targetSettlement, Army.ArmyTypes missionType, MobileParty mobileParty, float ourStrength, int numberOfEnemyFactionSettlements, float totalEnemyMobilePartyStrength)
		{
			if (missionType == 1 && targetSettlement != null && targetSettlement.StringId == "village_ES3_2" && TutorialPhase.Instance != null && !TutorialPhase.Instance.IsCompleted)
			{
				return 0f;
			}
			return base.GetTargetScoreForFaction(targetSettlement, missionType, mobileParty, ourStrength, numberOfEnemyFactionSettlements, totalEnemyMobilePartyStrength);
		}
	}
}
