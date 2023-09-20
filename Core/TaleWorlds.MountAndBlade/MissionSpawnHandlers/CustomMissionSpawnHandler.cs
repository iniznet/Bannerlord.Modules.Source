using System;

namespace TaleWorlds.MountAndBlade.MissionSpawnHandlers
{
	// Token: 0x020003E8 RID: 1000
	public class CustomMissionSpawnHandler : MissionLogic
	{
		// Token: 0x06003491 RID: 13457 RVA: 0x000DA221 File Offset: 0x000D8421
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this._missionAgentSpawnLogic = base.Mission.GetMissionBehavior<MissionAgentSpawnLogic>();
		}

		// Token: 0x06003492 RID: 13458 RVA: 0x000DA23C File Offset: 0x000D843C
		protected static MissionSpawnSettings CreateCustomBattleWaveSpawnSettings()
		{
			return new MissionSpawnSettings(MissionSpawnSettings.InitialSpawnMethod.BattleSizeAllocating, MissionSpawnSettings.ReinforcementTimingMethod.GlobalTimer, MissionSpawnSettings.ReinforcementSpawnMethod.Wave, 3f, 0f, 0f, 0.5f, 0, 0f, 0f, 1f, 0.75f);
		}

		// Token: 0x04001672 RID: 5746
		protected MissionAgentSpawnLogic _missionAgentSpawnLogic;
	}
}
