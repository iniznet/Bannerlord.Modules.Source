using System;
using StoryMode.StoryModePhases;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;

namespace StoryMode.GameComponents
{
	// Token: 0x02000047 RID: 71
	public class StoryModeTargetScoreCalculatingModel : DefaultTargetScoreCalculatingModel
	{
		// Token: 0x060003C7 RID: 967 RVA: 0x000175C0 File Offset: 0x000157C0
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
