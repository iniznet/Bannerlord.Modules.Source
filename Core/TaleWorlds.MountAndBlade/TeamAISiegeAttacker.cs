using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000177 RID: 375
	public class TeamAISiegeAttacker : TeamAISiegeComponent
	{
		// Token: 0x17000432 RID: 1074
		// (get) Token: 0x0600134C RID: 4940 RVA: 0x0004B482 File Offset: 0x00049682
		public MBReadOnlyList<ArcherPosition> ArcherPositions
		{
			get
			{
				return this._archerPositions;
			}
		}

		// Token: 0x0600134D RID: 4941 RVA: 0x0004B48C File Offset: 0x0004968C
		public TeamAISiegeAttacker(Mission currentMission, Team currentTeam, float thinkTimerTime, float applyTimerTime)
			: base(currentMission, currentTeam, thinkTimerTime, applyTimerTime)
		{
			IEnumerable<GameEntity> enumerable = currentMission.Scene.FindEntitiesWithTag("archer_position_attacker");
			this._archerPositions = enumerable.Select((GameEntity ap) => new ArcherPosition(ap, TeamAISiegeComponent.QuerySystem, BattleSideEnum.Attacker)).ToMBList<ArcherPosition>();
		}

		// Token: 0x0600134E RID: 4942 RVA: 0x0004B4E8 File Offset: 0x000496E8
		public override void OnUnitAddedToFormationForTheFirstTime(Formation formation)
		{
			if (formation.AI.GetBehavior<BehaviorCharge>() == null)
			{
				if (formation.FormationIndex == FormationClass.NumberOfRegularFormations)
				{
					formation.AI.AddAiBehavior(new BehaviorGeneral(formation));
				}
				else if (formation.FormationIndex == FormationClass.Bodyguard)
				{
					formation.AI.AddAiBehavior(new BehaviorProtectGeneral(formation));
				}
				formation.AI.AddAiBehavior(new BehaviorCharge(formation));
				formation.AI.AddAiBehavior(new BehaviorPullBack(formation));
				formation.AI.AddAiBehavior(new BehaviorRegroup(formation));
				formation.AI.AddAiBehavior(new BehaviorReserve(formation));
				formation.AI.AddAiBehavior(new BehaviorRetreat(formation));
				formation.AI.AddAiBehavior(new BehaviorStop(formation));
				formation.AI.AddAiBehavior(new BehaviorTacticalCharge(formation));
				formation.AI.AddAiBehavior(new BehaviorAssaultWalls(formation));
				formation.AI.AddAiBehavior(new BehaviorShootFromSiegeTower(formation));
				formation.AI.AddAiBehavior(new BehaviorUseSiegeMachines(formation));
				formation.AI.AddAiBehavior(new BehaviorWaitForLadders(formation));
				formation.AI.AddAiBehavior(new BehaviorSparseSkirmish(formation));
				formation.AI.AddAiBehavior(new BehaviorSkirmish(formation));
				formation.AI.AddAiBehavior(new BehaviorRetreatToKeep(formation));
			}
		}

		// Token: 0x0600134F RID: 4943 RVA: 0x0004B62C File Offset: 0x0004982C
		public override void OnDeploymentFinished()
		{
			base.OnDeploymentFinished();
			foreach (SiegeTower siegeTower in this.SiegeTowers)
			{
				base.DifficultNavmeshIDs.AddRange(siegeTower.CollectGetDifficultNavmeshIDsForAttackers());
			}
			foreach (ArcherPosition archerPosition in this._archerPositions)
			{
				archerPosition.OnDeploymentFinished(TeamAISiegeComponent.QuerySystem, BattleSideEnum.Attacker);
			}
		}

		// Token: 0x0400057C RID: 1404
		private readonly MBList<ArcherPosition> _archerPositions;
	}
}
