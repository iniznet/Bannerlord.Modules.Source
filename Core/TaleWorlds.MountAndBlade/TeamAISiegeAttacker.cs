using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class TeamAISiegeAttacker : TeamAISiegeComponent
	{
		public MBReadOnlyList<ArcherPosition> ArcherPositions
		{
			get
			{
				return this._archerPositions;
			}
		}

		public TeamAISiegeAttacker(Mission currentMission, Team currentTeam, float thinkTimerTime, float applyTimerTime)
			: base(currentMission, currentTeam, thinkTimerTime, applyTimerTime)
		{
			IEnumerable<GameEntity> enumerable = currentMission.Scene.FindEntitiesWithTag("archer_position_attacker");
			this._archerPositions = enumerable.Select((GameEntity ap) => new ArcherPosition(ap, TeamAISiegeComponent.QuerySystem, BattleSideEnum.Attacker)).ToMBList<ArcherPosition>();
		}

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

		private readonly MBList<ArcherPosition> _archerPositions;
	}
}
