using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000111 RID: 273
	public class BehaviorRetakeCastleKeyPosition : BehaviorComponent
	{
		// Token: 0x06000D17 RID: 3351 RVA: 0x00020219 File Offset: 0x0001E419
		public BehaviorRetakeCastleKeyPosition(Formation formation)
			: base(formation)
		{
			this._behaviorState = BehaviorRetakeCastleKeyPosition.BehaviorState.UnSet;
			this._behaviorSide = formation.AI.Side;
			this.ResetOrderPositions();
		}

		// Token: 0x06000D18 RID: 3352 RVA: 0x00020240 File Offset: 0x0001E440
		protected override void CalculateCurrentOrder()
		{
			base.CalculateCurrentOrder();
			base.CurrentOrder = ((this._behaviorState == BehaviorRetakeCastleKeyPosition.BehaviorState.Attacking) ? this._attackOrder : this._gatherOrder);
		}

		// Token: 0x06000D19 RID: 3353 RVA: 0x00020268 File Offset: 0x0001E468
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

		// Token: 0x06000D1A RID: 3354 RVA: 0x000202E8 File Offset: 0x0001E4E8
		private void ConfirmGatheringSide()
		{
			SiegeLane siegeLane = TeamAISiegeComponent.SiegeLanes.FirstOrDefault((SiegeLane sl) => sl.LaneSide == this._gatheringSide);
			if (siegeLane == null || siegeLane.LaneState >= SiegeLane.LaneStateEnum.Conceited)
			{
				this.ResetOrderPositions();
			}
		}

		// Token: 0x06000D1B RID: 3355 RVA: 0x00020320 File Offset: 0x0001E520
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

		// Token: 0x06000D1C RID: 3356 RVA: 0x0002051E File Offset: 0x0001E71E
		public override void OnValidBehaviorSideChanged()
		{
			base.OnValidBehaviorSideChanged();
			this.ResetOrderPositions();
		}

		// Token: 0x06000D1D RID: 3357 RVA: 0x0002052C File Offset: 0x0001E72C
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

		// Token: 0x06000D1E RID: 3358 RVA: 0x00020690 File Offset: 0x0001E890
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
			base.Formation.WeaponUsageOrder = WeaponUsageOrder.WeaponUsageOrderUseAny;
		}

		// Token: 0x17000317 RID: 791
		// (get) Token: 0x06000D1F RID: 3359 RVA: 0x00020722 File Offset: 0x0001E922
		public override float NavmeshlessTargetPositionPenalty
		{
			get
			{
				return 1f;
			}
		}

		// Token: 0x06000D20 RID: 3360 RVA: 0x00020729 File Offset: 0x0001E929
		protected override float GetAiWeight()
		{
			return 1f;
		}

		// Token: 0x04000329 RID: 809
		private BehaviorRetakeCastleKeyPosition.BehaviorState _behaviorState;

		// Token: 0x0400032A RID: 810
		private MovementOrder _gatherOrder;

		// Token: 0x0400032B RID: 811
		private MovementOrder _attackOrder;

		// Token: 0x0400032C RID: 812
		private FacingOrder _gatheringFacingOrder;

		// Token: 0x0400032D RID: 813
		private FacingOrder _attackFacingOrder;

		// Token: 0x0400032E RID: 814
		private TacticalPosition _gatheringTacticalPos;

		// Token: 0x0400032F RID: 815
		private FormationAI.BehaviorSide _gatheringSide;

		// Token: 0x0200044C RID: 1100
		private enum BehaviorState
		{
			// Token: 0x04001872 RID: 6258
			UnSet,
			// Token: 0x04001873 RID: 6259
			Gathering,
			// Token: 0x04001874 RID: 6260
			Attacking
		}
	}
}
