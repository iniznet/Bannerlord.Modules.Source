using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade.Source.Missions
{
	// Token: 0x020003EF RID: 1007
	public class BattleSpawnLogic : MissionLogic
	{
		// Token: 0x060034C3 RID: 13507 RVA: 0x000DB226 File Offset: 0x000D9426
		public BattleSpawnLogic(string selectedSpawnPointSetTag)
		{
			this._selectedSpawnPointSetTag = selectedSpawnPointSetTag;
		}

		// Token: 0x060034C4 RID: 13508 RVA: 0x000DB238 File Offset: 0x000D9438
		public override void OnPreMissionTick(float dt)
		{
			if (this._isScenePrepared)
			{
				return;
			}
			GameEntity gameEntity = base.Mission.Scene.FindEntityWithTag(this._selectedSpawnPointSetTag);
			if (gameEntity != null)
			{
				List<GameEntity> list = base.Mission.Scene.FindEntitiesWithTag("spawnpoint_set").ToList<GameEntity>();
				list.Remove(gameEntity);
				foreach (GameEntity gameEntity2 in list)
				{
					gameEntity2.Remove(76);
				}
			}
			this._isScenePrepared = true;
		}

		// Token: 0x04001691 RID: 5777
		public const string BattleTag = "battle_set";

		// Token: 0x04001692 RID: 5778
		public const string SallyOutTag = "sally_out_set";

		// Token: 0x04001693 RID: 5779
		public const string ReliefForceAttackTag = "relief_force_attack_set";

		// Token: 0x04001694 RID: 5780
		private const string SpawnPointSetCommonTag = "spawnpoint_set";

		// Token: 0x04001695 RID: 5781
		private readonly string _selectedSpawnPointSetTag;

		// Token: 0x04001696 RID: 5782
		private bool _isScenePrepared;
	}
}
