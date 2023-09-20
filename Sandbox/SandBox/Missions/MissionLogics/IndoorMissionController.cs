using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.MissionLogics
{
	// Token: 0x0200003E RID: 62
	public class IndoorMissionController : MissionLogic
	{
		// Token: 0x06000306 RID: 774 RVA: 0x00013F6B File Offset: 0x0001216B
		public override void OnCreated()
		{
			base.OnCreated();
			base.Mission.DoesMissionRequireCivilianEquipment = true;
		}

		// Token: 0x06000307 RID: 775 RVA: 0x00013F7F File Offset: 0x0001217F
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this._missionAgentHandler = base.Mission.GetMissionBehavior<MissionAgentHandler>();
		}

		// Token: 0x06000308 RID: 776 RVA: 0x00013F98 File Offset: 0x00012198
		public override void AfterStart()
		{
			base.AfterStart();
			base.Mission.SetMissionMode(0, true);
			base.Mission.IsInventoryAccessible = !Campaign.Current.IsMainHeroDisguised;
			base.Mission.IsQuestScreenAccessible = true;
			this._missionAgentHandler.SpawnPlayer(base.Mission.DoesMissionRequireCivilianEquipment, true, false, false, false, "");
			this._missionAgentHandler.SpawnLocationCharacters(null);
		}

		// Token: 0x0400018D RID: 397
		private MissionAgentHandler _missionAgentHandler;
	}
}
