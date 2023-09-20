using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.MissionLogics
{
	// Token: 0x02000057 RID: 87
	public class VillageMissionController : MissionLogic
	{
		// Token: 0x060003CA RID: 970 RVA: 0x0001BA07 File Offset: 0x00019C07
		public override void OnCreated()
		{
			base.OnCreated();
			base.Mission.DoesMissionRequireCivilianEquipment = false;
		}

		// Token: 0x060003CB RID: 971 RVA: 0x0001BA1C File Offset: 0x00019C1C
		public override void AfterStart()
		{
			base.AfterStart();
			bool isNight = Campaign.Current.IsNight;
			base.Mission.IsInventoryAccessible = true;
			base.Mission.IsQuestScreenAccessible = true;
			MissionAgentHandler missionBehavior = base.Mission.GetMissionBehavior<MissionAgentHandler>();
			missionBehavior.SpawnPlayer(base.Mission.DoesMissionRequireCivilianEquipment, false, false, false, false, "");
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
