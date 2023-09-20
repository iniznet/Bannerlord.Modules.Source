using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000176 RID: 374
	public class TeamAISallyOutDefender : TeamAISiegeComponent
	{
		// Token: 0x17000431 RID: 1073
		// (get) Token: 0x06001347 RID: 4935 RVA: 0x0004B02D File Offset: 0x0004922D
		public List<ArcherPosition> ArcherPositions { get; }

		// Token: 0x06001348 RID: 4936 RVA: 0x0004B038 File Offset: 0x00049238
		public TeamAISallyOutDefender(Mission currentMission, Team currentTeam, float thinkTimerTime, float applyTimerTime)
			: base(currentMission, currentTeam, thinkTimerTime, applyTimerTime)
		{
			TeamAISallyOutDefender <>4__this = this;
			TeamAISiegeComponent.QuerySystem = new SiegeQuerySystem(this.Team, TeamAISiegeComponent.SiegeLanes);
			TeamAISiegeComponent.QuerySystem.Expire();
			this.DefensePosition = () => new WorldPosition(currentMission.Scene, UIntPtr.Zero, <>4__this.Ram.GameEntity.GlobalPosition, false);
			IEnumerable<GameEntity> enumerable = from ap in currentMission.Scene.FindEntitiesWithTag("archer_position")
				where ap.Parent == null || ap.Parent.IsVisibleIncludeParents()
				select ap;
			this.ArcherPositions = enumerable.Select((GameEntity ap) => new ArcherPosition(ap, TeamAISiegeComponent.QuerySystem, BattleSideEnum.Defender)).ToList<ArcherPosition>();
		}

		// Token: 0x06001349 RID: 4937 RVA: 0x0004B104 File Offset: 0x00049304
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
				formation.AI.AddAiBehavior(new BehaviorDefendSiegeWeapon(formation));
				formation.AI.AddAiBehavior(new BehaviorSparseSkirmish(formation));
				formation.AI.AddAiBehavior(new BehaviorSkirmishLine(formation));
				formation.AI.AddAiBehavior(new BehaviorScreenedSkirmish(formation));
				formation.AI.AddAiBehavior(new BehaviorSkirmish(formation));
				formation.AI.AddAiBehavior(new BehaviorProtectFlank(formation));
				formation.AI.AddAiBehavior(new BehaviorFlank(formation));
				formation.AI.AddAiBehavior(new BehaviorHorseArcherSkirmish(formation));
				formation.AI.AddAiBehavior(new BehaviorDefend(formation));
			}
		}

		// Token: 0x0600134A RID: 4938 RVA: 0x0004B268 File Offset: 0x00049468
		public Vec3 CalculateSallyOutReferencePosition(FormationAI.BehaviorSide side)
		{
			if (side != FormationAI.BehaviorSide.Left)
			{
				if (side != FormationAI.BehaviorSide.Right)
				{
					return this.Ram.GameEntity.GlobalPosition;
				}
				SiegeTower siegeTower = this.SiegeTowers.FirstOrDefault((SiegeTower st) => st.WeaponSide == FormationAI.BehaviorSide.Right);
				if (siegeTower == null)
				{
					return this.Ram.GameEntity.GlobalPosition;
				}
				return siegeTower.GameEntity.GlobalPosition;
			}
			else
			{
				SiegeTower siegeTower2 = this.SiegeTowers.FirstOrDefault((SiegeTower st) => st.WeaponSide == FormationAI.BehaviorSide.Left);
				if (siegeTower2 == null)
				{
					return this.Ram.GameEntity.GlobalPosition;
				}
				return siegeTower2.GameEntity.GlobalPosition;
			}
		}

		// Token: 0x0600134B RID: 4939 RVA: 0x0004B328 File Offset: 0x00049528
		public override void OnDeploymentFinished()
		{
			TeamAISiegeComponent.SiegeLanes.Clear();
			int i;
			int j;
			for (i = 0; i < 3; i = j + 1)
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
				j = i;
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

		// Token: 0x0400057A RID: 1402
		public readonly Func<WorldPosition> DefensePosition;
	}
}
