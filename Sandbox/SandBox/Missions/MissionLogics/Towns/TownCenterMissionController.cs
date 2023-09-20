using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.MissionLogics.Towns
{
	// Token: 0x0200005D RID: 93
	public class TownCenterMissionController : MissionLogic
	{
		// Token: 0x0600040F RID: 1039 RVA: 0x0001D4FC File Offset: 0x0001B6FC
		public override void OnCreated()
		{
			base.OnCreated();
			base.Mission.DoesMissionRequireCivilianEquipment = true;
		}

		// Token: 0x06000410 RID: 1040 RVA: 0x0001D510 File Offset: 0x0001B710
		public override void AfterStart()
		{
			bool isNight = Campaign.Current.IsNight;
			base.Mission.SetMissionMode(0, true);
			base.Mission.IsInventoryAccessible = !Campaign.Current.IsMainHeroDisguised;
			base.Mission.IsQuestScreenAccessible = true;
			MissionAgentHandler missionBehavior = base.Mission.GetMissionBehavior<MissionAgentHandler>();
			missionBehavior.SpawnPlayer(base.Mission.DoesMissionRequireCivilianEquipment, true, false, false, false, "");
			missionBehavior.SpawnLocationCharacters(null);
			MissionAgentHandler.SpawnHorses();
			MissionAgentHandler.SpawnCats();
			MissionAgentHandler.SpawnDogs();
			if (!isNight)
			{
				MissionAgentHandler.SpawnSheeps();
				MissionAgentHandler.SpawnCows();
				MissionAgentHandler.SpawnHogs();
				MissionAgentHandler.SpawnGeese();
				MissionAgentHandler.SpawnChicken();
			}
		}
	}
}
