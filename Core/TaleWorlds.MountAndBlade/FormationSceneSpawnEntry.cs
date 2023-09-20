using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	public struct FormationSceneSpawnEntry
	{
		public FormationSceneSpawnEntry(FormationClass formationClass, GameEntity spawnEntity, GameEntity reinforcementSpawnEntity)
		{
			this.FormationClass = formationClass;
			this.SpawnEntity = spawnEntity;
			this.ReinforcementSpawnEntity = reinforcementSpawnEntity;
		}

		public readonly FormationClass FormationClass;

		public readonly GameEntity SpawnEntity;

		public readonly GameEntity ReinforcementSpawnEntity;
	}
}
