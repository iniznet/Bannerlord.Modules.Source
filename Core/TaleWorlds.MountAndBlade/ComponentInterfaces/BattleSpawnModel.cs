using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade.ComponentInterfaces
{
	// Token: 0x0200040A RID: 1034
	public abstract class BattleSpawnModel : GameModel
	{
		// Token: 0x06003571 RID: 13681 RVA: 0x000DE3FE File Offset: 0x000DC5FE
		public virtual void OnMissionStart()
		{
		}

		// Token: 0x06003572 RID: 13682 RVA: 0x000DE400 File Offset: 0x000DC600
		public virtual void OnMissionEnd()
		{
		}

		// Token: 0x06003573 RID: 13683
		[return: TupleElementNames(new string[] { "origin", "formationIndex" })]
		public abstract List<ValueTuple<IAgentOriginBase, int>> GetInitialSpawnAssignments(BattleSideEnum battleSide, List<IAgentOriginBase> troopOrigins);

		// Token: 0x06003574 RID: 13684
		[return: TupleElementNames(new string[] { "origin", "formationIndex" })]
		public abstract List<ValueTuple<IAgentOriginBase, int>> GetReinforcementAssignments(BattleSideEnum battleSide, List<IAgentOriginBase> troopOrigins);
	}
}
