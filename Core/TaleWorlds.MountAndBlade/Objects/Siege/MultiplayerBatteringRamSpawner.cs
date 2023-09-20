using System;

namespace TaleWorlds.MountAndBlade.Objects.Siege
{
	// Token: 0x020003A9 RID: 937
	public class MultiplayerBatteringRamSpawner : BatteringRamSpawner
	{
		// Token: 0x060032DA RID: 13018 RVA: 0x000D2758 File Offset: 0x000D0958
		public override void AssignParameters(SpawnerEntityMissionHelper _spawnerMissionHelper)
		{
			base.AssignParameters(_spawnerMissionHelper);
			_spawnerMissionHelper.SpawnedEntity.GetFirstScriptOfType<DestructableComponent>().MaxHitPoint = 12000f;
			BatteringRam firstScriptOfType = _spawnerMissionHelper.SpawnedEntity.GetFirstScriptOfType<BatteringRam>();
			firstScriptOfType.MaxSpeed *= 1f;
			firstScriptOfType.MinSpeed *= 1f;
		}

		// Token: 0x04001585 RID: 5509
		private const float MaxHitPoint = 12000f;

		// Token: 0x04001586 RID: 5510
		private const float SpeedMultiplier = 1f;
	}
}
