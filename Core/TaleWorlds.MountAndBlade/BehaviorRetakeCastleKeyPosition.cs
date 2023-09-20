using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	public class BehaviorRetakeCastleKeyPosition : BehaviorComponent
	{
		public BehaviorRetakeCastleKeyPosition(Formation formation)
			: base(formation)
		{
			this._behaviorState = BehaviorRetakeCastleKeyPosition.BehaviorState.UnSet;
			this._behaviorSide = formation.AI.Side;
			this.ResetOrderPositions();
		}

		protected override void CalculateCurrentOrder()
		{
			base.CalculateCurrentOrder();
			base.CurrentOrder = ((this._behaviorState == BehaviorRetakeCastleKeyPosition.BehaviorState.Attacking) ? this._attackOrder : this._gatherOrder);
		}

		private FormationAI.BehaviorSide DetermineGatheringSide()
		{
			IEnumerable<SiegeLane> enumerable = TeamAISiegeComponent.SiegeLanes.Where((SiegeLane sl) => sl.LaneSide != this._behaviorSide && sl.LaneState != SiegeLane.LaneStateEnum.Conceited && sl.DefenderOrigin.IsValid);
			if (enumerable.Any<SiegeLane>())
			{
				int nearestSafeSideDistance = enumerable.Min((SiegeLane pgl) => SiegeQuerySystem.SideDistance(1 << (int)this._behaviorSide, 1 << (int)pgl.LaneSide));
				return enumerable.Where((SiegeLane pgl) => SiegeQuerySystem.SideDistance(1 << (int)this._behaviorSide, 1 << (int)pgl.LaneSide) == nearestSafeSideDistance).MinBy((SiegeLane pgl) => pgl.DefenderOrigin.GetGroundVec3().DistanceSquared(base.Formation.QuerySystem.MedianPosition.GetGroundVec3())).LaneSide;
			}
			return this._behaviorSide;
		}

		private void ConfirmGatheringSide()
		{
			SiegeLane siegeLane = TeamAISiegeComponent.SiegeLanes.FirstOrDefault((SiegeLane sl) => sl.LaneSide == this._gatheringSide);
			if (siegeLane == null || siegeLane.LaneState >= SiegeLane.LaneStateEnum.Conceited)
			{
				this.ResetOrderPositions();
			}
		}

		private void ResetOrderPositions()
		{
			this._behaviorState = BehaviorRetakeCastleKeyPosition.BehaviorState.UnSet;
			this._gatheringSide = this.DetermineGatheringSide();
			SiegeLane siegeLane = TeamAISiegeComponent.SiegeLanes.FirstOrDefault((SiegeLane sl) => sl.LaneSide == this._gatheringSide);
			ICastleKeyPosition castleKeyPosition = siegeLane.DefensePoints.FirstOrDefault((ICastleKeyPosition dp) => dp.AttackerSiegeWeapon is UsableMachine && !(dp.AttackerSiegeWeapon as UsableMachine).IsDisabled);
			WorldFrame worldFrame = ((castleKeyPosition != null) ? castleKeyPosition.DefenseWaitFrame : siegeLane.DefensePoints.FirstOrDefault<ICastleKeyPosition>().DefenseWaitFrame);
			ICastleKeyPosition castleKeyPosition2 = siegeLane.DefensePoints.FirstOrDefault((ICastleKeyPosition dp) => dp.AttackerSiegeWeapon is UsableMachine && !(dp.AttackerSiegeWeapon as UsableMachine).IsDisabled);
			this._gatheringTacticalPos = ((castleKeyPosition2 != null) ? castleKeyPosition2.WaitPosition : null);
			if (this._gatheringTacticalPos != null)
			{
				this._gatherOrder = MovementOrder.MovementOrderMove(this._gatheringTacticalPos.Position);
				this._gatheringFacingOrder = FacingOrder.FacingOrderLookAtEnemy;
			}
			else if (worldFrame.Origin.IsValid)
			{
				worldFrame.Rotation.f.Normalize();
				this._gatherOrder = MovementOrder.MovementOrderMove(worldFrame.Origin);
				this._gatheringFacingOrder = FacingOrder.FacingOrderLookAtEnemy;
			}
			else
			{
				this._gatherOrder = MovementOrder.MovementOrderMove(base.Formation.QuerySystem.MedianPosition);
				this._gatheringFacingOrder = FacingOrder.FacingOrderLookAtEnemy;
			}
			SiegeLane siegeLane2 = TeamAISiegeComponent.SiegeLanes.FirstOrDefault((SiegeLane sl) => sl.LaneSide == this._behaviorSide);
			ICastleKeyPosition castleKeyPosition3 = siegeLane2.DefensePoints.FirstOrDefault((ICastleKeyPosition dp) => dp.AttackerSiegeWeapon is UsableMachine && !(dp.AttackerSiegeWeapon as UsableMachine).IsDisabled);
			this._attackOrder = MovementOrder.MovementOrderMove((castleKeyPosition3 != null) ? castleKeyPosition3.MiddleFrame.Origin : siegeLane2.DefensePoints.FirstOrDefault<ICastleKeyPosition>().MiddleFrame.Origin);
			this._attackFacingOrder = FacingOrder.FacingOrderLookAtEnemy;
			base.CurrentOrder = ((this._behaviorState == BehaviorRetakeCastleKeyPosition.BehaviorState.Attacking) ? this._attackOrder : this._gatherOrder);
			this.CurrentFacingOrder = ((this._behaviorState == BehaviorRetakeCastleKeyPosition.BehaviorState.Attacking) ? this._attackFacingOrder : this._gatheringFacingOrder);
		}

		public override void OnValidBehaviorSideChanged()
		{
			base.OnValidBehaviorSideChanged();
			this.ResetOrderPositions();
		}

		public override void TickOccasionally()
		{
			base.TickOccasionally();
			if (this._behaviorState != BehaviorRetakeCastleKeyPosition.BehaviorState.Attacking)
			{
				this.ConfirmGatheringSide();
			}
			bool flag = true;
			if (this._behaviorState != BehaviorRetakeCastleKeyPosition.BehaviorState.Attacking)
			{
				flag = base.Formation.QuerySystem.MedianPosition.GetNavMeshVec3().DistanceSquared(this._gatherOrder.CreateNewOrderWorldPosition(base.Formation, WorldPosition.WorldPositionEnforcedCache.NavMeshVec3).GetNavMeshVec3()) < 100f || base.Formation.QuerySystem.FormationIntegrityData.DeviationOfPositionsExcludeFarAgents / ((base.Formation.QuerySystem.IdealAverageDisplacement != 0f) ? base.Formation.QuerySystem.IdealAverageDisplacement : 1f) <= 3f;
			}
			BehaviorRetakeCastleKeyPosition.BehaviorState behaviorState = (flag ? BehaviorRetakeCastleKeyPosition.BehaviorState.Attacking : BehaviorRetakeCastleKeyPosition.BehaviorState.Gathering);
			if (behaviorState != this._behaviorState)
			{
				this._behaviorState = behaviorState;
				base.CurrentOrder = ((this._behaviorState == BehaviorRetakeCastleKeyPosition.BehaviorState.Attacking) ? this._attackOrder : this._gatherOrder);
				this.CurrentFacingOrder = ((this._behaviorState == BehaviorRetakeCastleKeyPosition.BehaviorState.Attacking) ? this._attackFacingOrder : this._gatheringFacingOrder);
			}
			base.Formation.SetMovementOrder(base.CurrentOrder);
			base.Formation.FacingOrder = this.CurrentFacingOrder;
			if (this._behaviorState == BehaviorRetakeCastleKeyPosition.BehaviorState.Gathering && this._gatheringTacticalPos != null)
			{
				base.Formation.FormOrder = FormOrder.FormOrderCustom(this._gatheringTacticalPos.Width);
			}
		}

		protected override void OnBehaviorActivatedAux()
		{
			this._behaviorState = BehaviorRetakeCastleKeyPosition.BehaviorState.UnSet;
			this._behaviorSide = base.Formation.AI.Side;
			this.ResetOrderPositions();
			base.Formation.SetMovementOrder(base.CurrentOrder);
			base.Formation.FacingOrder = this.CurrentFacingOrder;
			base.Formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
			base.Formation.FiringOrder = FiringOrder.FiringOrderFireAtWill;
			base.Formation.FormOrder = FormOrder.FormOrderWide;
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

		private BehaviorRetakeCastleKeyPosition.BehaviorState _behaviorState;

		private MovementOrder _gatherOrder;

		private MovementOrder _attackOrder;

		private FacingOrder _gatheringFacingOrder;

		private FacingOrder _attackFacingOrder;

		private TacticalPosition _gatheringTacticalPos;

		private FormationAI.BehaviorSide _gatheringSide;

		private enum BehaviorState
		{
			UnSet,
			Gathering,
			Attacking
		}
	}
}
