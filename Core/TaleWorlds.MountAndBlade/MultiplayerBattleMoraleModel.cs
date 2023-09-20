using System;
using System.Runtime.CompilerServices;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.ComponentInterfaces;

namespace TaleWorlds.MountAndBlade
{
	public class MultiplayerBattleMoraleModel : BattleMoraleModel
	{
		[return: TupleElementNames(new string[] { "affectedSideMaxMoraleLoss", "affectorSideMaxMoraleGain" })]
		public override ValueTuple<float, float> CalculateMaxMoraleChangeDueToAgentIncapacitated(Agent affectedAgent, AgentState affectedAgentState, Agent affectorAgent, in KillingBlow killingBlow)
		{
			return new ValueTuple<float, float>(0f, 0f);
		}

		[return: TupleElementNames(new string[] { "affectedSideMaxMoraleLoss", "affectorSideMaxMoraleGain" })]
		public override ValueTuple<float, float> CalculateMaxMoraleChangeDueToAgentPanicked(Agent agent)
		{
			return new ValueTuple<float, float>(0f, 0f);
		}

		public override float CalculateMoraleChangeToCharacter(Agent agent, float maxMoraleChange)
		{
			return 0f;
		}

		public override float GetEffectiveInitialMorale(Agent agent, float baseMorale)
		{
			return baseMorale;
		}

		public override bool CanPanicDueToMorale(Agent agent)
		{
			return true;
		}

		public override float CalculateCasualtiesFactor(BattleSideEnum battleSide)
		{
			return 1f;
		}

		public override float GetAverageMorale(Formation formation)
		{
			return 0f;
		}
	}
}
