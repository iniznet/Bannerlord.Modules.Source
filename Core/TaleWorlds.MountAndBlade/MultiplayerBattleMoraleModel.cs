using System;
using System.Runtime.CompilerServices;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.ComponentInterfaces;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020001EB RID: 491
	public class MultiplayerBattleMoraleModel : BattleMoraleModel
	{
		// Token: 0x06001B8B RID: 7051 RVA: 0x00061BDA File Offset: 0x0005FDDA
		[return: TupleElementNames(new string[] { "affectedSideMaxMoraleLoss", "affectorSideMaxMoraleGain" })]
		public override ValueTuple<float, float> CalculateMaxMoraleChangeDueToAgentIncapacitated(Agent affectedAgent, AgentState affectedAgentState, Agent affectorAgent, in KillingBlow killingBlow)
		{
			return new ValueTuple<float, float>(0f, 0f);
		}

		// Token: 0x06001B8C RID: 7052 RVA: 0x00061BEB File Offset: 0x0005FDEB
		[return: TupleElementNames(new string[] { "affectedSideMaxMoraleLoss", "affectorSideMaxMoraleGain" })]
		public override ValueTuple<float, float> CalculateMaxMoraleChangeDueToAgentPanicked(Agent agent)
		{
			return new ValueTuple<float, float>(0f, 0f);
		}

		// Token: 0x06001B8D RID: 7053 RVA: 0x00061BFC File Offset: 0x0005FDFC
		public override float CalculateMoraleChangeToCharacter(Agent agent, float maxMoraleChange)
		{
			return 0f;
		}

		// Token: 0x06001B8E RID: 7054 RVA: 0x00061C03 File Offset: 0x0005FE03
		public override float GetEffectiveInitialMorale(Agent agent, float baseMorale)
		{
			return baseMorale;
		}

		// Token: 0x06001B8F RID: 7055 RVA: 0x00061C06 File Offset: 0x0005FE06
		public override bool CanPanicDueToMorale(Agent agent)
		{
			return true;
		}

		// Token: 0x06001B90 RID: 7056 RVA: 0x00061C09 File Offset: 0x0005FE09
		public override float CalculateCasualtiesFactor(BattleSideEnum battleSide)
		{
			return 1f;
		}

		// Token: 0x06001B91 RID: 7057 RVA: 0x00061C10 File Offset: 0x0005FE10
		public override float GetAverageMorale(Formation formation)
		{
			return 0f;
		}
	}
}
