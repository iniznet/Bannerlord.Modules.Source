using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000138 RID: 312
	public interface IDetachment
	{
		// Token: 0x17000379 RID: 889
		// (get) Token: 0x06000FA3 RID: 4003
		MBReadOnlyList<Formation> UserFormations { get; }

		// Token: 0x1700037A RID: 890
		// (get) Token: 0x06000FA4 RID: 4004
		bool IsLoose { get; }

		// Token: 0x06000FA5 RID: 4005
		bool IsAgentUsingOrInterested(Agent agent);

		// Token: 0x06000FA6 RID: 4006
		float? GetWeightOfNextSlot(BattleSideEnum side);

		// Token: 0x06000FA7 RID: 4007
		float GetDetachmentWeight(BattleSideEnum side);

		// Token: 0x06000FA8 RID: 4008
		float ComputeAndCacheDetachmentWeight(BattleSideEnum side);

		// Token: 0x06000FA9 RID: 4009
		float GetDetachmentWeightFromCache();

		// Token: 0x06000FAA RID: 4010
		void GetSlotIndexWeightTuples(List<ValueTuple<int, float>> slotIndexWeightTuples);

		// Token: 0x06000FAB RID: 4011
		bool IsSlotAtIndexAvailableForAgent(int slotIndex, Agent agent);

		// Token: 0x06000FAC RID: 4012
		bool IsAgentEligible(Agent agent);

		// Token: 0x06000FAD RID: 4013
		void AddAgentAtSlotIndex(Agent agent, int slotIndex);

		// Token: 0x06000FAE RID: 4014
		Agent GetMovingAgentAtSlotIndex(int slotIndex);

		// Token: 0x06000FAF RID: 4015
		void MarkSlotAtIndex(int slotIndex);

		// Token: 0x06000FB0 RID: 4016
		bool IsDetachmentRecentlyEvaluated();

		// Token: 0x06000FB1 RID: 4017
		void UnmarkDetachment();

		// Token: 0x06000FB2 RID: 4018
		float? GetWeightOfAgentAtNextSlot(List<Agent> candidates, out Agent match);

		// Token: 0x06000FB3 RID: 4019
		float? GetWeightOfAgentAtNextSlot(List<ValueTuple<Agent, float>> agentTemplateScores, out Agent match);

		// Token: 0x06000FB4 RID: 4020
		float GetTemplateWeightOfAgent(Agent candidate);

		// Token: 0x06000FB5 RID: 4021
		List<float> GetTemplateCostsOfAgent(Agent candidate, List<float> oldValue);

		// Token: 0x06000FB6 RID: 4022
		float GetExactCostOfAgentAtSlot(Agent candidate, int slotIndex);

		// Token: 0x06000FB7 RID: 4023
		float GetWeightOfOccupiedSlot(Agent detachedAgent);

		// Token: 0x06000FB8 RID: 4024
		float? GetWeightOfAgentAtOccupiedSlot(Agent detachedAgent, List<Agent> candidates, out Agent match);

		// Token: 0x06000FB9 RID: 4025
		bool IsStandingPointAvailableForAgent(Agent agent);

		// Token: 0x06000FBA RID: 4026
		void AddAgent(Agent agent, int slotIndex = -1);

		// Token: 0x06000FBB RID: 4027
		void RemoveAgent(Agent detachedAgent);

		// Token: 0x06000FBC RID: 4028
		void FormationStartUsing(Formation formation);

		// Token: 0x06000FBD RID: 4029
		void FormationStopUsing(Formation formation);

		// Token: 0x06000FBE RID: 4030
		bool IsUsedByFormation(Formation formation);

		// Token: 0x06000FBF RID: 4031
		WorldFrame? GetAgentFrame(Agent detachedAgent);

		// Token: 0x06000FC0 RID: 4032
		void ResetEvaluation();

		// Token: 0x06000FC1 RID: 4033
		bool IsEvaluated();

		// Token: 0x06000FC2 RID: 4034
		void SetAsEvaluated();

		// Token: 0x06000FC3 RID: 4035
		void OnFormationLeave(Formation formation);
	}
}
