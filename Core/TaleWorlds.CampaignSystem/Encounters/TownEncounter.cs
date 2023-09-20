using System;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.Encounters
{
	// Token: 0x0200029C RID: 668
	public class TownEncounter : LocationEncounter
	{
		// Token: 0x06002419 RID: 9241 RVA: 0x00099358 File Offset: 0x00097558
		public TownEncounter(Settlement settlement)
			: base(settlement)
		{
		}

		// Token: 0x0600241A RID: 9242 RVA: 0x00099364 File Offset: 0x00097564
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
