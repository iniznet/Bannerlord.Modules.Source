﻿using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade.Objects
{
	public class AnimalSpawnSettings : ScriptComponentBehavior
	{
		public static void CheckAndSetAnimalAgentFlags(GameEntity spawnEntity, Agent animalAgent)
		{
			if (spawnEntity.HasScriptOfType<AnimalSpawnSettings>() && spawnEntity.GetFirstScriptOfType<AnimalSpawnSettings>().DisableWandering)
			{
				animalAgent.SetAgentFlags(animalAgent.GetAgentFlags() & ~AgentFlag.CanWander);
			}
		}

		public bool DisableWandering = true;
	}
}
