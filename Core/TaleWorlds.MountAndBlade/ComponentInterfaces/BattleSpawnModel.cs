using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade.ComponentInterfaces
{
	public abstract class BattleSpawnModel : GameModel
	{
		public virtual void OnMissionStart()
		{
		}

		public virtual void OnMissionEnd()
		{
		}

		[return: TupleElementNames(new string[] { "origin", "formationIndex" })]
		public abstract List<ValueTuple<IAgentOriginBase, int>> GetInitialSpawnAssignments(BattleSideEnum battleSide, List<IAgentOriginBase> troopOrigins);

		[return: TupleElementNames(new string[] { "origin", "formationIndex" })]
		public abstract List<ValueTuple<IAgentOriginBase, int>> GetReinforcementAssignments(BattleSideEnum battleSide, List<IAgentOriginBase> troopOrigins);
	}
}
