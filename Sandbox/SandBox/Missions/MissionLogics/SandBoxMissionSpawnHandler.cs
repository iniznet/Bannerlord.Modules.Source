using System;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.MissionLogics
{
	// Token: 0x02000052 RID: 82
	public class SandBoxMissionSpawnHandler : MissionLogic
	{
		// Token: 0x060003B5 RID: 949 RVA: 0x0001B4AB File Offset: 0x000196AB
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this._missionAgentSpawnLogic = base.Mission.GetMissionBehavior<MissionAgentSpawnLogic>();
			this._mapEvent = MapEvent.PlayerMapEvent;
		}

		// Token: 0x060003B6 RID: 950 RVA: 0x0001B4D0 File Offset: 0x000196D0
		protected static MissionSpawnSettings CreateSandBoxBattleWaveSpawnSettings()
		{
			int reinforcementWaveCount = BannerlordConfig.GetReinforcementWaveCount();
			return new MissionSpawnSettings(0, 0, 1, 3f, 0f, 0f, 0.5f, reinforcementWaveCount, 0f, 0f, 1f, 0.75f);
		}

		// Token: 0x040001C0 RID: 448
		protected MissionAgentSpawnLogic _missionAgentSpawnLogic;

		// Token: 0x040001C1 RID: 449
		protected MapEvent _mapEvent;
	}
}
