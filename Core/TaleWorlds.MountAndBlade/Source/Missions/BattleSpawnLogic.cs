using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade.Source.Missions
{
	public class BattleSpawnLogic : MissionLogic
	{
		public BattleSpawnLogic(string selectedSpawnPointSetTag)
		{
			this._selectedSpawnPointSetTag = selectedSpawnPointSetTag;
		}

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

		public const string BattleTag = "battle_set";

		public const string SallyOutTag = "sally_out_set";

		public const string ReliefForceAttackTag = "relief_force_attack_set";

		private const string SpawnPointSetCommonTag = "spawnpoint_set";

		private readonly string _selectedSpawnPointSetTag;

		private bool _isScenePrepared;
	}
}
