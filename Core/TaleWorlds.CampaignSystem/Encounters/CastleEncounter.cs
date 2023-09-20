using System;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.Encounters
{
	// Token: 0x02000298 RID: 664
	public class CastleEncounter : LocationEncounter
	{
		// Token: 0x06002407 RID: 9223 RVA: 0x00099054 File Offset: 0x00097254
		public CastleEncounter(Settlement settlement)
			: base(settlement)
		{
		}

		// Token: 0x06002408 RID: 9224 RVA: 0x00099060 File Offset: 0x00097260
		public override IMission CreateAndOpenMissionController(Location nextLocation, Location previousLocation = null, CharacterObject talkToChar = null, string playerSpecialSpawnTag = null)
		{
			int num = base.Settlement.Town.GetWallLevel();
			IMission mission;
			if (nextLocation.StringId == "center")
			{
				mission = CampaignMission.OpenCastleCourtyardMission(nextLocation.GetSceneName(num), nextLocation, talkToChar, num);
			}
			else if (nextLocation.StringId == "lordshall")
			{
				nextLocation.GetSceneName(num);
				mission = CampaignMission.OpenIndoorMission(nextLocation.GetSceneName(num), num, nextLocation, talkToChar);
			}
			else
			{
				num = Campaign.Current.Models.LocationModel.GetSettlementUpgradeLevel(PlayerEncounter.LocationEncounter);
				nextLocation.GetSceneName(num);
				mission = CampaignMission.OpenIndoorMission(nextLocation.GetSceneName(num), num, nextLocation, talkToChar);
			}
			return mission;
		}
	}
}
