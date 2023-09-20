using System;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class AgentProximityMap
	{
		public static bool CanSearchRadius(float searchRadius)
		{
			float num = Mission.Current.ProximityMapMaxSearchRadius();
			return searchRadius <= num;
		}

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

		public struct ProximityMapSearchStruct
		{
			public Agent LastFoundAgent { get; internal set; }

			internal void RefreshLastFoundAgent(Mission mission)
			{
				this.LastFoundAgent = this.SearchStructInternal.GetCurrentAgent(mission);
			}

			internal AgentProximityMap.ProximityMapSearchStructInternal SearchStructInternal;

			internal bool LoopAllAgents;

			internal int LastAgentLoopIndex;
		}

		[EngineStruct("Managed_proximity_map_search_struct", false)]
		[Serializable]
		internal struct ProximityMapSearchStructInternal
		{
			internal Agent GetCurrentAgent(Mission mission)
			{
				return mission.FindAgentWithIndex(this.CurrentElementIndex);
			}

			internal int CurrentElementIndex;

			internal Vec2i Loc;

			internal Vec2i GridMin;

			internal Vec2i GridMax;

			internal Vec2 SearchPos;

			internal float SearchDistSq;
		}
	}
}
