using System;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.MountAndBlade;

namespace SandBox.Missions.MissionLogics
{
	public class SandBoxMissionSpawnHandler : MissionLogic
	{
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this._missionAgentSpawnLogic = base.Mission.GetMissionBehavior<MissionAgentSpawnLogic>();
			this._mapEvent = MapEvent.PlayerMapEvent;
		}

		protected static MissionSpawnSettings CreateSandBoxBattleWaveSpawnSettings()
		{
			int reinforcementWaveCount = BannerlordConfig.GetReinforcementWaveCount();
			return new MissionSpawnSettings(0, 0, 1, 3f, 0f, 0f, 0.5f, reinforcementWaveCount, 0f, 0f, 1f, 0.75f);
		}

		protected MissionAgentSpawnLogic _missionAgentSpawnLogic;

		protected MapEvent _mapEvent;
	}
}
