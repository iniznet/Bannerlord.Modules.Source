using System;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.Encounters
{
	// Token: 0x0200029B RID: 667
	public class RetirementEncounter : LocationEncounter
	{
		// Token: 0x06002417 RID: 9239 RVA: 0x000992FE File Offset: 0x000974FE
		public RetirementEncounter(Settlement settlement)
			: base(settlement)
		{
		}

		// Token: 0x06002418 RID: 9240 RVA: 0x00099308 File Offset: 0x00097508
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
