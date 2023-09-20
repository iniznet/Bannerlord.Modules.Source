using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade.ComponentInterfaces
{
	// Token: 0x02000406 RID: 1030
	public abstract class MissionDifficultyModel : GameModel
	{
		// Token: 0x0600355C RID: 13660
		public abstract float GetDamageMultiplierOfCombatDifficulty(Agent victimAgent, Agent attackerAgent = null);
	}
}
