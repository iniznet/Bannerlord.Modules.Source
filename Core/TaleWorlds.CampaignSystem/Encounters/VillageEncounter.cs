﻿using System;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.Encounters
{
	public class VillageEncounter : LocationEncounter
	{
		public VillageEncounter(Settlement settlement)
			: base(settlement)
		{
		}

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
