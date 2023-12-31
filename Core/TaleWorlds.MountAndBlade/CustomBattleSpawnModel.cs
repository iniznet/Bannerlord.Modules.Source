﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.ComponentInterfaces;

namespace TaleWorlds.MountAndBlade
{
	public class CustomBattleSpawnModel : BattleSpawnModel
	{
		public override void OnMissionStart()
		{
			MissionReinforcementsHelper.OnMissionStart();
		}

		public override void OnMissionEnd()
		{
			MissionReinforcementsHelper.OnMissionEnd();
		}

		[return: TupleElementNames(new string[] { "origin", "formationIndex" })]
		public override List<ValueTuple<IAgentOriginBase, int>> GetInitialSpawnAssignments(BattleSideEnum battleSide, List<IAgentOriginBase> troopOrigins)
		{
			List<ValueTuple<IAgentOriginBase, int>> list = new List<ValueTuple<IAgentOriginBase, int>>();
			foreach (IAgentOriginBase agentOriginBase in troopOrigins)
			{
				ValueTuple<IAgentOriginBase, int> valueTuple = new ValueTuple<IAgentOriginBase, int>(agentOriginBase, (int)Mission.Current.GetAgentTroopClass(battleSide, agentOriginBase.Troop));
				list.Add(valueTuple);
			}
			return list;
		}

		[return: TupleElementNames(new string[] { "origin", "formationIndex" })]
		public override List<ValueTuple<IAgentOriginBase, int>> GetReinforcementAssignments(BattleSideEnum battleSide, List<IAgentOriginBase> troopOrigins)
		{
			return MissionReinforcementsHelper.GetReinforcementAssignments(battleSide, troopOrigins);
		}
	}
}
