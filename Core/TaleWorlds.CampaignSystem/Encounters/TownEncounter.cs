using System;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.Encounters
{
	public class TownEncounter : LocationEncounter
	{
		public TownEncounter(Settlement settlement)
			: base(settlement)
		{
		}

		public override IMission CreateAndOpenMissionController(Location nextLocation, Location previousLocation = null, CharacterObject talkToChar = null, string playerSpecialSpawnTag = null)
		{
			int num = base.Settlement.Town.GetWallLevel();
			string sceneName = nextLocation.GetSceneName(num);
			IMission mission;
			if (nextLocation.StringId == "center")
			{
				mission = CampaignMission.OpenTownCenterMission(sceneName, nextLocation, talkToChar, num, playerSpecialSpawnTag);
			}
			else if (nextLocation.StringId == "arena")
			{
				mission = CampaignMission.OpenArenaStartMission(sceneName, nextLocation, talkToChar);
			}
			else
			{
				num = Campaign.Current.Models.LocationModel.GetSettlementUpgradeLevel(PlayerEncounter.LocationEncounter);
				mission = CampaignMission.OpenIndoorMission(sceneName, num, nextLocation, talkToChar);
			}
			return mission;
		}
	}
}
