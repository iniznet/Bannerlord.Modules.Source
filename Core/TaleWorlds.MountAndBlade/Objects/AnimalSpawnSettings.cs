using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade.Objects
{
	// Token: 0x0200039E RID: 926
	public class AnimalSpawnSettings : ScriptComponentBehavior
	{
		// Token: 0x06003294 RID: 12948 RVA: 0x000D160C File Offset: 0x000CF80C
		public static void CheckAndSetAnimalAgentFlags(GameEntity spawnEntity, Agent animalAgent)
		{
			if (spawnEntity.HasScriptOfType<AnimalSpawnSettings>() && spawnEntity.GetFirstScriptOfType<AnimalSpawnSettings>().DisableWandering)
			{
				animalAgent.SetAgentFlags(animalAgent.GetAgentFlags() & ~AgentFlag.CanWander);
			}
		}

		// Token: 0x04001552 RID: 5458
		public bool DisableWandering = true;
	}
}
