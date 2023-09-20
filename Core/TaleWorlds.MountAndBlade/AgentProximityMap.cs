using System;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020000F2 RID: 242
	public class AgentProximityMap
	{
		// Token: 0x06000C03 RID: 3075 RVA: 0x000162E0 File Offset: 0x000144E0
		public static bool CanSearchRadius(float searchRadius)
		{
			float num = Mission.Current.ProximityMapMaxSearchRadius();
			return searchRadius <= num;
		}

		// Token: 0x06000C04 RID: 3076 RVA: 0x00016300 File Offset: 0x00014500
		public static AgentProximityMap.ProximityMapSearchStruct BeginSearch(Mission mission, Vec2 searchPos, float searchRadius, bool extendRangeByBiggestAgentCollisionPadding = false)
		{
			if (extendRangeByBiggestAgentCollisionPadding)
			{
				searchRadius += mission.GetBiggestAgentCollisionPadding() + 1f;
			}
			AgentProximityMap.ProximityMapSearchStruct proximityMapSearchStruct = default(AgentProximityMap.ProximityMapSearchStruct);
			float num = mission.ProximityMapMaxSearchRadius();
			proximityMapSearchStruct.LoopAllAgents = searchRadius > num;
			if (proximityMapSearchStruct.LoopAllAgents)
			{
				proximityMapSearchStruct.SearchStructInternal.SearchPos = searchPos;
				proximityMapSearchStruct.SearchStructInternal.SearchDistSq = searchRadius * searchRadius;
				proximityMapSearchStruct.LastAgentLoopIndex = 0;
				proximityMapSearchStruct.LastFoundAgent = null;
				MBReadOnlyList<Agent> agents = mission.Agents;
				while (agents.Count > proximityMapSearchStruct.LastAgentLoopIndex)
				{
					Agent agent = agents[proximityMapSearchStruct.LastAgentLoopIndex];
					if (agent.Position.AsVec2.DistanceSquared(searchPos) <= proximityMapSearchStruct.SearchStructInternal.SearchDistSq)
					{
						proximityMapSearchStruct.LastFoundAgent = agent;
						break;
					}
					proximityMapSearchStruct.LastAgentLoopIndex++;
				}
			}
			else
			{
				proximityMapSearchStruct.SearchStructInternal = mission.ProximityMapBeginSearch(searchPos, searchRadius);
				proximityMapSearchStruct.RefreshLastFoundAgent(mission);
			}
			return proximityMapSearchStruct;
		}

		// Token: 0x06000C05 RID: 3077 RVA: 0x000163EC File Offset: 0x000145EC
		public static void FindNext(Mission mission, ref AgentProximityMap.ProximityMapSearchStruct searchStruct)
		{
			if (searchStruct.LoopAllAgents)
			{
				searchStruct.LastAgentLoopIndex++;
				searchStruct.LastFoundAgent = null;
				MBReadOnlyList<Agent> agents = mission.Agents;
				while (agents.Count > searchStruct.LastAgentLoopIndex)
				{
					Agent agent = agents[searchStruct.LastAgentLoopIndex];
					if (agent.Position.AsVec2.DistanceSquared(searchStruct.SearchStructInternal.SearchPos) <= searchStruct.SearchStructInternal.SearchDistSq)
					{
						searchStruct.LastFoundAgent = agent;
						return;
					}
					searchStruct.LastAgentLoopIndex++;
				}
				return;
			}
			mission.ProximityMapFindNext(ref searchStruct.SearchStructInternal);
			searchStruct.RefreshLastFoundAgent(mission);
		}

		// Token: 0x02000434 RID: 1076
		public struct ProximityMapSearchStruct
		{
			// Token: 0x17000945 RID: 2373
			// (get) Token: 0x060035CC RID: 13772 RVA: 0x000DF023 File Offset: 0x000DD223
			// (set) Token: 0x060035CD RID: 13773 RVA: 0x000DF02B File Offset: 0x000DD22B
			public Agent LastFoundAgent { get; internal set; }

			// Token: 0x060035CE RID: 13774 RVA: 0x000DF034 File Offset: 0x000DD234
			internal void RefreshLastFoundAgent(Mission mission)
			{
				this.LastFoundAgent = this.SearchStructInternal.GetCurrentAgent(mission);
			}

			// Token: 0x04001813 RID: 6163
			internal AgentProximityMap.ProximityMapSearchStructInternal SearchStructInternal;

			// Token: 0x04001814 RID: 6164
			internal bool LoopAllAgents;

			// Token: 0x04001815 RID: 6165
			internal int LastAgentLoopIndex;
		}

		// Token: 0x02000435 RID: 1077
		[EngineStruct("Managed_proximity_map_search_struct")]
		[Serializable]
		internal struct ProximityMapSearchStructInternal
		{
			// Token: 0x060035CF RID: 13775 RVA: 0x000DF048 File Offset: 0x000DD248
			internal Agent GetCurrentAgent(Mission mission)
			{
				return mission.FindAgentWithIndex(this.CurrentElementIndex);
			}

			// Token: 0x04001817 RID: 6167
			internal int CurrentElementIndex;

			// Token: 0x04001818 RID: 6168
			internal Vec2i Loc;

			// Token: 0x04001819 RID: 6169
			internal Vec2i GridMin;

			// Token: 0x0400181A RID: 6170
			internal Vec2i GridMax;

			// Token: 0x0400181B RID: 6171
			internal Vec2 SearchPos;

			// Token: 0x0400181C RID: 6172
			internal float SearchDistSq;
		}
	}
}
