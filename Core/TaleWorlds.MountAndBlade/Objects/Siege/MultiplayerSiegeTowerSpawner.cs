using System;

namespace TaleWorlds.MountAndBlade.Objects.Siege
{
	// Token: 0x020003AE RID: 942
	public class MultiplayerSiegeTowerSpawner : SiegeTowerSpawner
	{
		// Token: 0x060032E4 RID: 13028 RVA: 0x000D2813 File Offset: 0x000D0A13
		public override void AssignParameters(SpawnerEntityMissionHelper _spawnerMissionHelper)
		{
			base.AssignParameters(_spawnerMissionHelper);
			_spawnerMissionHelper.SpawnedEntity.GetFirstScriptOfType<DestructableComponent>().MaxHitPoint = 15000f;
			SiegeTower firstScriptOfType = _spawnerMissionHelper.SpawnedEntity.GetFirstScriptOfType<SiegeTower>();
			firstScriptOfType.MaxSpeed = 1f;
			firstScriptOfType.MinSpeed = 0.5f;
		}

		// Token: 0x04001587 RID: 5511
		private const float MaxHitPoint = 15000f;

		// Token: 0x04001588 RID: 5512
		private const float MinimumSpeed = 0.5f;

		// Token: 0x04001589 RID: 5513
		private const float MaximumSpeed = 1f;
	}
}
