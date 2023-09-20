using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.MissionLogics.Towns
{
	public class TownCenterMissionController : MissionLogic
	{
		public override void OnCreated()
		{
			base.OnCreated();
			base.Mission.DoesMissionRequireCivilianEquipment = true;
		}

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
