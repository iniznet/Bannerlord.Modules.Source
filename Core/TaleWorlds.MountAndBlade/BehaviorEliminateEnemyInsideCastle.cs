using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	public class BehaviorEliminateEnemyInsideCastle : BehaviorComponent
	{
		public BehaviorEliminateEnemyInsideCastle(Formation formation)
			: base(formation)
		{
			this._behaviorState = BehaviorEliminateEnemyInsideCastle.BehaviorState.UnSet;
			this._behaviorSide = formation.AI.Side;
			this.ResetOrderPositions();
		}

		protected override void CalculateCurrentOrder()
		{
			base.CalculateCurrentOrder();
			base.CurrentOrder = ((this._behaviorState == BehaviorEliminateEnemyInsideCastle.BehaviorState.Attacking) ? this._attackOrder : this._gatherOrder);
		}

		private void DetermineMostImportantInvadingEnemyFormation()
		{
			float num = float.MinValue;
			this._targetEnemyFormation = null;
			foreach (Team team in base.Formation.Team.Mission.Teams)
			{
				if (team.IsEnemyOf(base.Formation.Team))
				{
					for (int i = 0; i < Math.Min(team.FormationsIncludingSpecialAndEmpty.Count, 8); i++)
					{
						Formation formation = team.FormationsIncludingSpecialAndEmpty[i];
						if (formation.CountOfUnits > 0 && TeamAISiegeComponent.IsFormationInsideCastle(formation, true, 0.4f))
						{
							float formationPower = formation.QuerySystem.FormationPower;
							if (formationPower > num)
							{
								num = formationPower;
								this._targetEnemyFormation = formation;
							}
						}
					}
				}
			}
		}

		private void ConfirmGatheringSide()
		{
			SiegeLane siegeLane = TeamAISiegeComponent.SiegeLanes.FirstOrDefault((SiegeLane sl) => sl.LaneSide == this._behaviorSide);
			if (siegeLane == null || siegeLane.LaneState >= SiegeLane.LaneStateEnum.Conceited)
			{
				this.ResetOrderPositions();
			}
		}

		private FormationAI.BehaviorSide DetermineGatheringSide()
		{
			this.DetermineMostImportantInvadingEnemyFormation();
			if (this._targetEnemyFormation == null)
			{
				if (this._behaviorState == BehaviorEliminateEnemyInsideCastle.BehaviorState.Attacking)
				{
					this._behaviorState = BehaviorEliminateEnemyInsideCastle.BehaviorState.UnSet;
				}
				return this._behaviorSide;
			}
			int connectedSides = TeamAISiegeComponent.QuerySystem.DeterminePositionAssociatedSide(this._targetEnemyFormation.QuerySystem.MedianPosition.GetNavMeshVec3());
			IEnumerable<SiegeLane> enumerable = TeamAISiegeComponent.SiegeLanes.Where((SiegeLane sl) => sl.LaneState != SiegeLane.LaneStateEnum.Conceited && !SiegeQuerySystem.AreSidesRelated(sl.LaneSide, connectedSides));
			FormationAI.BehaviorSide behaviorSide = this._behaviorSide;
			if (enumerable.Any<SiegeLane>())
			{
				if (enumerable.Count<SiegeLane>() > 1)
				{
					int leastDangerousLaneState = enumerable.Min((SiegeLane pgl) => (int)pgl.LaneState);
					IEnumerable<SiegeLane> enumerable2 = enumerable.Where((SiegeLane pgl) => pgl.LaneState == (SiegeLane.LaneStateEnum)leastDangerousLaneState);
					behaviorSide = ((enumerable2.Count<SiegeLane>() > 1) ? enumerable2.MinBy((SiegeLane ldl) => SiegeQuerySystem.SideDistance(1 << connectedSides, 1 << (int)ldl.LaneSide)).LaneSide : enumerable2.First<SiegeLane>().LaneSide);
				}
				else
				{
					behaviorSide = enumerable.First<SiegeLane>().LaneSide;
				}
			}
			return behaviorSide;
		}

		private void ResetOrderPositions()
		{
			this._behaviorSide = this.DetermineGatheringSide();
			SiegeLane siegeLane = TeamAISiegeComponent.SiegeLanes.FirstOrDefault((SiegeLane sl) => sl.LaneSide == this._behaviorSide);
			WorldFrame? worldFrame;
			if (siegeLane == null)
			{
				worldFrame = null;
			}
			else
			{
				List<ICastleKeyPosition> defensePoints = siegeLane.DefensePoints;
				if (defensePoints == null)
				{
					worldFrame = null;
				}
				else
				{
					ICastleKeyPosition castleKeyPosition = defensePoints.FirstOrDefault((ICastleKeyPosition dp) => dp.AttackerSiegeWeapon is UsableMachine && !(dp.AttackerSiegeWeapon as UsableMachine).IsDisabled);
					worldFrame = ((castleKeyPosition != null) ? new WorldFrame?(castleKeyPosition.DefenseWaitFrame) : null);
				}
			}
			WorldFrame worldFrame2 = worldFrame ?? WorldFrame.Invalid;
			object obj;
			if (siegeLane == null)
			{
				obj = null;
			}
			else
			{
				List<ICastleKeyPosition> defensePoints2 = siegeLane.DefensePoints;
				if (defensePoints2 == null)
				{
					obj = null;
				}
				else
				{
					ICastleKeyPosition castleKeyPosition2 = defensePoints2.FirstOrDefault((ICastleKeyPosition dp) => dp.AttackerSiegeWeapon is UsableMachine && !(dp.AttackerSiegeWeapon as UsableMachine).IsDisabled);
					obj = ((castleKeyPosition2 != null) ? castleKeyPosition2.WaitPosition : null);
				}
			}
			object obj2;
			if ((obj2 = obj) == null)
			{
				if (siegeLane == null)
				{
					obj2 = null;
				}
				else
				{
					List<ICastleKeyPosition> defensePoints3 = siegeLane.DefensePoints;
					obj2 = ((defensePoints3 != null) ? defensePoints3.FirstOrDefault<ICastleKeyPosition>().WaitPosition : null);
				}
			}
			this._gatheringTacticalPos = obj2;
			if (this._gatheringTacticalPos != null)
			{
				this._gatherOrder = MovementOrder.MovementOrderMove(this._gatheringTacticalPos.Position);
				this._gatheringFacingOrder = FacingOrder.FacingOrderLookAtEnemy;
			}
			else if (worldFrame2.Origin.IsValid)
			{
				worldFrame2.Rotation.f.Normalize();
				this._gatherOrder = MovementOrder.MovementOrderMove(worldFrame2.Origin);
				this._gatheringFacingOrder = FacingOrder.FacingOrderLookAtEnemy;
			}
			else
			{
				this._gatherOrder = MovementOrder.MovementOrderMove(base.Formation.QuerySystem.MedianPosition);
				this._gatheringFacingOrder = FacingOrder.FacingOrderLookAtEnemy;
			}
			this._attackOrder = MovementOrder.MovementOrderChargeToTarget(this._targetEnemyFormation);
			this._attackFacingOrder = FacingOrder.FacingOrderLookAtEnemy;
			base.CurrentOrder = ((this._behaviorState == BehaviorEliminateEnemyInsideCastle.BehaviorState.Attacking) ? this._attackOrder : this._gatherOrder);
			this.CurrentFacingOrder = ((this._behaviorState == BehaviorEliminateEnemyInsideCastle.BehaviorState.Attacking) ? this._attackFacingOrder : this._gatheringFacingOrder);
		}

		public override void OnValidBehaviorSideChanged()
		{
			base.OnValidBehaviorSideChanged();
			this.ResetOrderPositions();
		}

		public override void TickOccasionally()
		{
			base.TickOccasionally();
			if (this._behaviorState != BehaviorEliminateEnemyInsideCastle.BehaviorState.Attacking)
			{
				this.ConfirmGatheringSide();
			}
			bool flag;
			if (this._behaviorState == BehaviorEliminateEnemyInsideCastle.BehaviorState.Attacking)
			{
				flag = this._targetEnemyFormation != null;
			}
			else
			{
				flag = this._targetEnemyFormation != null && (base.Formation.QuerySystem.MedianPosition.GetNavMeshVec3().DistanceSquared(this._gatherOrder.CreateNewOrderWorldPosition(base.Formation, WorldPosition.WorldPositionEnforcedCache.NavMeshVec3).GetNavMeshVec3()) < 100f || base.Formation.QuerySystem.FormationIntegrityData.DeviationOfPositionsExcludeFarAgents / ((base.Formation.QuerySystem.IdealAverageDisplacement != 0f) ? base.Formation.QuerySystem.IdealAverageDisplacement : 1f) <= 3f);
			}
			BehaviorEliminateEnemyInsideCastle.BehaviorState behaviorState = (flag ? BehaviorEliminateEnemyInsideCastle.BehaviorState.Attacking : BehaviorEliminateEnemyInsideCastle.BehaviorState.Gathering);
			if (behaviorState != this._behaviorState)
			{
				this._behaviorState = behaviorState;
				base.CurrentOrder = ((this._behaviorState == BehaviorEliminateEnemyInsideCastle.BehaviorState.Attacking) ? this._attackOrder : this._gatherOrder);
				this.CurrentFacingOrder = ((this._behaviorState == BehaviorEliminateEnemyInsideCastle.BehaviorState.Attacking) ? this._attackFacingOrder : this._gatheringFacingOrder);
			}
			base.Formation.SetMovementOrder(base.CurrentOrder);
			base.Formation.FacingOrder = this.CurrentFacingOrder;
			if (this._behaviorState == BehaviorEliminateEnemyInsideCastle.BehaviorState.Gathering && this._gatheringTacticalPos != null)
			{
				base.Formation.FormOrder = FormOrder.FormOrderCustom(this._gatheringTacticalPos.Width);
			}
		}

		protected override void OnBehaviorActivatedAux()
		{
			this._behaviorState = BehaviorEliminateEnemyInsideCastle.BehaviorState.UnSet;
			this._behaviorSide = base.Formation.AI.Side;
			this.ResetOrderPositions();
			base.Formation.SetMovementOrder(base.CurrentOrder);
			base.Formation.FacingOrder = this.CurrentFacingOrder;
			base.Formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
			base.Formation.FiringOrder = FiringOrder.FiringOrderFireAtWill;
			base.Formation.FormOrder = FormOrder.FormOrderWide;
			base.Formation.WeaponUsageOrder = WeaponUsageOrder.WeaponUsageOrderUseAny;
		}

		public override float NavmeshlessTargetPositionPenalty
		{
			get
			{
				return 1f;
			}
		}

		protected override float GetAiWeight()
		{
			return 1f;
		}

		private BehaviorEliminateEnemyInsideCastle.BehaviorState _behaviorState;

		private MovementOrder _gatherOrder;

		private MovementOrder _attackOrder;

		private FacingOrder _gatheringFacingOrder;

		private FacingOrder _attackFacingOrder;

		private TacticalPosition _gatheringTacticalPos;

		private Formation _targetEnemyFormation;

		private enum BehaviorState
		{
			UnSet,
			Gathering,
			Attacking
		}
	}
}
