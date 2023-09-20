using System;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.Encounters
{
	public class RetirementEncounter : LocationEncounter
	{
		public RetirementEncounter(Settlement settlement)
			: base(settlement)
		{
		}

		public override IMission CreateAndOpenMissionController(Location nextLocation, Location previousLocation = null, CharacterObject talkToChar = null, string playerSpecialSpawnTag = null)
		{
			IMission mission = null;
			if (Settlement.CurrentSettlement.SettlementComponent is RetirementSettlementComponent)
			{
				int num = (Settlement.CurrentSettlement.IsTown ? Settlement.CurrentSettlement.Town.GetWallLevel() : 1);
				mission = CampaignMission.OpenRetirementMission(nextLocation.GetSceneName(num), nextLocation, null, null);
			}
			return mission;
		}
	}
}
