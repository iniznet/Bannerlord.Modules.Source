using System;
using System.Runtime.CompilerServices;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade.ComponentInterfaces
{
	public abstract class BattleMoraleModel : GameModel
	{
		[return: TupleElementNames(new string[] { "affectedSideMaxMoraleLoss", "affectorSideMaxMoraleGain" })]
		public abstract ValueTuple<float, float> CalculateMaxMoraleChangeDueToAgentIncapacitated(Agent affectedAgent, AgentState affectedAgentState, Agent affectorAgent, in KillingBlow killingBlow);

		[return: TupleElementNames(new string[] { "affectedSideMaxMoraleLoss", "affectorSideMaxMoraleGain" })]
		public abstract ValueTuple<float, float> CalculateMaxMoraleChangeDueToAgentPanicked(Agent agent);

		public abstract float CalculateMoraleChangeToCharacter(Agent agent, float maxMoraleChange);

		public abstract float GetEffectiveInitialMorale(Agent agent, float baseMorale);

		public abstract bool CanPanicDueToMorale(Agent agent);

		public abstract float CalculateCasualtiesFactor(BattleSideEnum battleSide);

		public abstract float GetAverageMorale(Formation formation);

		public const float BaseMoraleGainOnKill = 3f;

		public const float BaseMoraleLossOnKill = 4f;

		public const float BaseMoraleGainOnPanic = 2f;

		public const float BaseMoraleLossOnPanic = 1.1f;

		public const float MeleeWeaponMoraleMultiplier = 0.75f;

		public const float RangedWeaponMoraleMultiplier = 0.5f;

		public const float SiegeWeaponMoraleMultiplier = 0.25f;

		public const float BurningSiegeWeaponMoraleBonus = 0.25f;

		public const float CasualtyFactorRate = 2f;
	}
}
