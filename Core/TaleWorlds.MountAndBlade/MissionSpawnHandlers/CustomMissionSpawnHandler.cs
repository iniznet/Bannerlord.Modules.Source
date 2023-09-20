using System;

namespace TaleWorlds.MountAndBlade.MissionSpawnHandlers
{
	public class CustomMissionSpawnHandler : MissionLogic
	{
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this._missionAgentSpawnLogic = base.Mission.GetMissionBehavior<MissionAgentSpawnLogic>();
		}

		protected static MissionSpawnSettings CreateCustomBattleWaveSpawnSettings()
		{
			return new MissionSpawnSettings(MissionSpawnSettings.InitialSpawnMethod.BattleSizeAllocating, MissionSpawnSettings.ReinforcementTimingMethod.GlobalTimer, MissionSpawnSettings.ReinforcementSpawnMethod.Wave, 3f, 0f, 0f, 0.5f, 0, 0f, 0f, 1f, 0.75f);
		}

		protected MissionAgentSpawnLogic _missionAgentSpawnLogic;
	}
}
