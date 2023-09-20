using System;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.Encounters
{
	// Token: 0x0200029D RID: 669
	public class VillageEncounter : LocationEncounter
	{
		// Token: 0x0600241B RID: 9243 RVA: 0x000993EC File Offset: 0x000975EC
		public VillageEncounter(Settlement settlement)
			: base(settlement)
		{
		}

		// Token: 0x0600241C RID: 9244 RVA: 0x000993F8 File Offset: 0x000975F8
		public override IMission CreateAndOpenMissionController(Location nextLocation, Location previousLocation = null, CharacterObject talkToChar = null, string playerSpecialSpawnTag = null)
		{
			IMission mission = null;
			if (nextLocation.StringId == "village_center")
			{
				mission = CampaignMission.OpenVillageMission(nextLocation.GetSceneName(1), nextLocation, talkToChar);
			}
			return mission;
		}
	}
}
