using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000179 RID: 377
	public class TeamAISiegeDefender : TeamAISiegeComponent
	{
		// Token: 0x1700043B RID: 1083
		// (get) Token: 0x06001368 RID: 4968 RVA: 0x0004C104 File Offset: 0x0004A304
		public List<ArcherPosition> ArcherPositions { get; }

		// Token: 0x06001369 RID: 4969 RVA: 0x0004C10C File Offset: 0x0004A30C
		public TeamAISiegeDefender(Mission currentMission, Team currentTeam, float thinkTimerTime, float applyTimerTime)
			: base(currentMission, currentTeam, thinkTimerTime, applyTimerTime)
		{
			TeamAISiegeComponent.QuerySystem = new SiegeQuerySystem(this.Team, TeamAISiegeComponent.SiegeLanes);
			TeamAISiegeComponent.QuerySystem.Expire();
			IEnumerable<GameEntity> enumerable = from ap in currentMission.Scene.FindEntitiesWithTag("archer_position")
				where ap.Parent == null || ap.Parent.IsVisibleIncludeParents()
				select ap;
			this.ArcherPositions = enumerable.Select((GameEntity ap) => new ArcherPosition(ap, TeamAISiegeComponent.QuerySystem, BattleSideEnum.Defender)).ToList<ArcherPosition>();
		}

		// Token: 0x0600136A RID: 4970 RVA: 0x0004C1A8 File Offset: 0x0004A3A8
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
				formation.AI.AddAiBehavior(new BehaviorDefendCastleKeyPosition(formation));
				formation.AI.AddAiBehavior(new BehaviorEliminateEnemyInsideCastle(formation));
				formation.AI.AddAiBehavior(new BehaviorRetakeCastleKeyPosition(formation));
				formation.AI.AddAiBehavior(new BehaviorRetreatToKeep(formation));
				formation.AI.AddAiBehavior(new BehaviorSallyOut(formation));
				formation.AI.AddAiBehavior(new BehaviorUseMurderHole(formation));
				formation.AI.AddAiBehavior(new BehaviorShootFromCastleWalls(formation));
				formation.AI.AddAiBehavior(new BehaviorSparseSkirmish(formation));
			}
		}

		// Token: 0x0600136B RID: 4971 RVA: 0x0004C2FC File Offset: 0x0004A4FC
		public override void OnDeploymentFinished()
		{
			base.OnDeploymentFinished();
			foreach (SiegeTower siegeTower in this.SiegeTowers)
			{
				base.DifficultNavmeshIDs.AddRange(siegeTower.CollectGetDifficultNavmeshIDsForDefenders());
			}
			List<SiegeLane> list = TeamAISiegeComponent.SiegeLanes.ToList<SiegeLane>();
			TeamAISiegeComponent.SiegeLanes.Clear();
			int i;
			int i2;
			for (i = 0; i < 3; i = i2 + 1)
			{
				TeamAISiegeComponent.SiegeLanes.Add(new SiegeLane((FormationAI.BehaviorSide)i, TeamAISiegeComponent.QuerySystem));
				SiegeLane siegeLane = TeamAISiegeComponent.SiegeLanes[i];
				siegeLane.SetPrimarySiegeWeapons(base.PrimarySiegeWeapons.Where((IPrimarySiegeWeapon psw) => psw.WeaponSide == (FormationAI.BehaviorSide)i).ToList<IPrimarySiegeWeapon>());
				siegeLane.SetDefensePoints((from ckp in this.CastleKeyPositions
					where (ckp as ICastleKeyPosition).DefenseSide == (FormationAI.BehaviorSide)i
					select ckp into dp
					select dp as ICastleKeyPosition).ToList<ICastleKeyPosition>());
				siegeLane.RefreshLane();
				siegeLane.DetermineLaneState();
				siegeLane.DetermineOrigins();
				if (i < list.Count)
				{
					for (int j = 0; j < Mission.Current.Teams.Count; j++)
					{
						siegeLane.SetLastAssignedFormation(j, list[i].GetLastAssignedFormation(j));
					}
				}
				i2 = i;
			}
			TeamAISiegeComponent.QuerySystem = new SiegeQuerySystem(this.Team, TeamAISiegeComponent.SiegeLanes);
			TeamAISiegeComponent.QuerySystem.Expire();
			TeamAISiegeComponent.SiegeLanes.ForEach(delegate(SiegeLane sl)
			{
				sl.SetSiegeQuerySystem(TeamAISiegeComponent.QuerySystem);
			});
			this.ArcherPositions.ForEach(delegate(ArcherPosition ap)
			{
				ap.OnDeploymentFinished(TeamAISiegeComponent.QuerySystem, BattleSideEnum.Defender);
			});
		}

		// Token: 0x04000592 RID: 1426
		public Vec3 MurderHolePosition;
	}
}
