using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class TeamAISallyOutDefender : TeamAISiegeComponent
	{
		public List<ArcherPosition> ArcherPositions { get; }

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

		public readonly Func<WorldPosition> DefensePosition;
	}
}
