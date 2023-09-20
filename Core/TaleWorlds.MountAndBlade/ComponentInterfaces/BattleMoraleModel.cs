using System;
using System.Runtime.CompilerServices;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade.ComponentInterfaces
{
	// Token: 0x02000408 RID: 1032
	public abstract class BattleMoraleModel : GameModel
	{
		// Token: 0x06003562 RID: 13666
		[return: TupleElementNames(new string[] { "affectedSideMaxMoraleLoss", "affectorSideMaxMoraleGain" })]
		public abstract ValueTuple<float, float> CalculateMaxMoraleChangeDueToAgentIncapacitated(Agent affectedAgent, AgentState affectedAgentState, Agent affectorAgent, in KillingBlow killingBlow);

		// Token: 0x06003563 RID: 13667
		[return: TupleElementNames(new string[] { "affectedSideMaxMoraleLoss", "affectorSideMaxMoraleGain" })]
		public abstract ValueTuple<float, float> CalculateMaxMoraleChangeDueToAgentPanicked(Agent agent);

		// Token: 0x06003564 RID: 13668
		public abstract float CalculateMoraleChangeToCharacter(Agent agent, float maxMoraleChange);

		// Token: 0x06003565 RID: 13669
		public abstract float GetEffectiveInitialMorale(Agent agent, float baseMorale);

		// Token: 0x06003566 RID: 13670
		public abstract bool CanPanicDueToMorale(Agent agent);

		// Token: 0x06003567 RID: 13671
		public abstract float CalculateCasualtiesFactor(BattleSideEnum battleSide);

		// Token: 0x06003568 RID: 13672
		public abstract float GetAverageMorale(Formation formation);

		// Token: 0x040016DD RID: 5853
		public const float BaseMoraleGainOnKill = 3f;

		// Token: 0x040016DE RID: 5854
		public const float BaseMoraleLossOnKill = 4f;

		// Token: 0x040016DF RID: 5855
		public const float BaseMoraleGainOnPanic = 2f;

		// Token: 0x040016E0 RID: 5856
		public const float BaseMoraleLossOnPanic = 1.1f;

		// Token: 0x040016E1 RID: 5857
		public const float MeleeWeaponMoraleMultiplier = 0.75f;

		// Token: 0x040016E2 RID: 5858
		public const float RangedWeaponMoraleMultiplier = 0.5f;

		// Token: 0x040016E3 RID: 5859
		public const float SiegeWeaponMoraleMultiplier = 0.25f;

		// Token: 0x040016E4 RID: 5860
		public const float BurningSiegeWeaponMoraleBonus = 0.25f;

		// Token: 0x040016E5 RID: 5861
		public const float CasualtyFactorRate = 2f;
	}
}
