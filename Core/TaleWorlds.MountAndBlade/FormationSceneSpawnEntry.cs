using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020001FC RID: 508
	public struct FormationSceneSpawnEntry
	{
		// Token: 0x06001C42 RID: 7234 RVA: 0x00064C35 File Offset: 0x00062E35
		public FormationSceneSpawnEntry(FormationClass formationClass, GameEntity spawnEntity, GameEntity reinforcementSpawnEntity)
		{
			this.FormationClass = formationClass;
			this.SpawnEntity = spawnEntity;
			this.ReinforcementSpawnEntity = reinforcementSpawnEntity;
		}

		// Token: 0x04000937 RID: 2359
		public readonly FormationClass FormationClass;

		// Token: 0x04000938 RID: 2360
		public readonly GameEntity SpawnEntity;

		// Token: 0x04000939 RID: 2361
		public readonly GameEntity ReinforcementSpawnEntity;
	}
}
