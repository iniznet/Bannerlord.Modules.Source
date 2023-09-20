using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000175 RID: 373
	public class TeamAISallyOutAttacker : TeamAISiegeComponent
	{
		// Token: 0x06001344 RID: 4932 RVA: 0x0004AE68 File Offset: 0x00049068
		public TeamAISallyOutAttacker(Mission currentMission, Team currentTeam, float thinkTimerTime, float applyTimerTime)
			: base(currentMission, currentTeam, thinkTimerTime, applyTimerTime)
		{
			this.ArcherPositions = currentMission.Scene.FindEntitiesWithTag("archer_position");
			this.BesiegerRangedSiegeWeapons = new List<UsableMachine>(from w in currentMission.ActiveMissionObjects.FindAllWithType<RangedSiegeWeapon>()
				where w.Side == BattleSideEnum.Attacker && !w.IsDisabled
				select w);
		}

		// Token: 0x06001345 RID: 4933 RVA: 0x0004AED0 File Offset: 0x000490D0
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
				formation.AI.AddAiBehavior(new BehaviorShootFromCastleWalls(formation));
				formation.AI.AddAiBehavior(new BehaviorDestroySiegeWeapons(formation));
				formation.AI.AddAiBehavior(new BehaviorSparseSkirmish(formation));
				formation.AI.AddAiBehavior(new BehaviorDefend(formation));
				formation.AI.AddAiBehavior(new BehaviorRetreatToCastle(formation));
				formation.AI.AddAiBehavior(new BehaviorRetreatToKeep(formation));
				formation.AI.AddAiBehavior(new BehaviorDefendCastleKeyPosition(formation));
			}
		}

		// Token: 0x06001346 RID: 4934 RVA: 0x0004B012 File Offset: 0x00049212
		public override void OnDeploymentFinished()
		{
			base.OnDeploymentFinished();
			if (base.CurrentTactic != null)
			{
				base.CurrentTactic.ResetTactic();
			}
		}

		// Token: 0x04000578 RID: 1400
		public IEnumerable<GameEntity> ArcherPositions;

		// Token: 0x04000579 RID: 1401
		public readonly List<UsableMachine> BesiegerRangedSiegeWeapons;
	}
}
