using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.MissionLogics
{
	// Token: 0x0200003D RID: 61
	public class HouseMissionController : MissionLogic
	{
		// Token: 0x06000302 RID: 770 RVA: 0x00013ECB File Offset: 0x000120CB
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this._missionAgentHandler = base.Mission.GetMissionBehavior<MissionAgentHandler>();
		}

		// Token: 0x06000303 RID: 771 RVA: 0x00013EE4 File Offset: 0x000120E4
		public override void OnCreated()
		{
			base.OnCreated();
			base.Mission.DoesMissionRequireCivilianEquipment = true;
		}

		// Token: 0x06000304 RID: 772 RVA: 0x00013EF8 File Offset: 0x000120F8
		public override void EarlyStart()
		{
		}

		// Token: 0x06000305 RID: 773 RVA: 0x00013EFC File Offset: 0x000120FC
		public override void AfterStart()
		{
			base.AfterStart();
			base.Mission.SetMissionMode(0, true);
			base.Mission.IsInventoryAccessible = !Campaign.Current.IsMainHeroDisguised;
			base.Mission.IsQuestScreenAccessible = true;
			this._missionAgentHandler.SpawnPlayer(base.Mission.DoesMissionRequireCivilianEquipment, true, true, false, false, "");
			this._missionAgentHandler.SpawnLocationCharacters(null);
		}

		// Token: 0x0400018C RID: 396
		private MissionAgentHandler _missionAgentHandler;
	}
}
