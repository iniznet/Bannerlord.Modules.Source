using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020000FA RID: 250
	public class BehaviorAssaultWalls : BehaviorComponent
	{
		// Token: 0x06000C5D RID: 3165 RVA: 0x000186C8 File Offset: 0x000168C8
		private void ResetOrderPositions()
		{
			this._primarySiegeWeapons = this._teamAISiegeComponent.PrimarySiegeWeapons.ToList<IPrimarySiegeWeapon>();
			this._primarySiegeWeapons.RemoveAll((IPrimarySiegeWeapon uM) => uM.WeaponSide != this._behaviorSide);
			IEnumerable<ICastleKeyPosition> enumerable = TeamAISiegeComponent.SiegeLanes.Where((SiegeLane sl) => sl.LaneSide == this._behaviorSide).SelectMany((SiegeLane sila) => sila.DefensePoints);
			this._innerGate = this._teamAISiegeComponent.InnerGate;
			this._isGateLane = this._teamAISiegeComponent.OuterGate.DefenseSide == this._behaviorSide;
			if (this._isGateLane)
			{
				this._wallSegment = null;
			}
			else
			{
				WallSegment wallSegment = enumerable.FirstOrDefault((ICastleKeyPosition dp) => dp is WallSegment && (dp as WallSegment).IsBreachedWall) as WallSegment;
				if (wallSegment != null)
				{
					this._wallSegment = wallSegment;
				}
				else
				{
					IPrimarySiegeWeapon primarySiegeWeapon = this._primarySiegeWeapons.MaxBy((IPrimarySiegeWeapon psw) => psw.SiegeWeaponPriority);
					this._wallSegment = primarySiegeWeapon.TargetCastlePosition as WallSegment;
				}
			}
			this._stopOrder = MovementOrder.MovementOrderStop;
			this._chargeOrder = MovementOrder.MovementOrderCharge;
			bool flag = this._teamAISiegeComponent.OuterGate != null && this._behaviorSide == this._teamAISiegeComponent.OuterGate.DefenseSide;
			this._attackEntityOrderOuterGate = ((flag && !this._teamAISiegeComponent.OuterGate.IsDeactivated && this._teamAISiegeComponent.OuterGate.State != CastleGate.GateState.Open) ? MovementOrder.MovementOrderAttackEntity(this._teamAISiegeComponent.OuterGate.GameEntity, false) : MovementOrder.MovementOrderStop);
			this._attackEntityOrderInnerGate = ((flag && this._teamAISiegeComponent.InnerGate != null && !this._teamAISiegeComponent.InnerGate.IsDeactivated && this._teamAISiegeComponent.InnerGate.State != CastleGate.GateState.Open) ? MovementOrder.MovementOrderAttackEntity(this._teamAISiegeComponent.InnerGate.GameEntity, false) : MovementOrder.MovementOrderStop);
			WorldPosition origin = this._teamAISiegeComponent.OuterGate.MiddleFrame.Origin;
			this._castleGateMoveOrder = MovementOrder.MovementOrderMove(origin);
			if (this._isGateLane)
			{
				this._wallSegmentMoveOrder = this._castleGateMoveOrder;
			}
			else
			{
				WorldPosition origin2 = this._wallSegment.MiddleFrame.Origin;
				this._wallSegmentMoveOrder = MovementOrder.MovementOrderMove(origin2);
			}
			this._facingOrder = FacingOrder.FacingOrderLookAtEnemy;
		}

		// Token: 0x06000C5E RID: 3166 RVA: 0x00018938 File Offset: 0x00016B38
		public BehaviorAssaultWalls(Formation formation)
			: base(formation)
		{
			base.BehaviorCoherence = 0f;
			this._behaviorSide = formation.AI.Side;
			this._teamAISiegeComponent = (TeamAISiegeComponent)formation.Team.TeamAI;
			this._behaviorState = BehaviorAssaultWalls.BehaviorState.Deciding;
			this.ResetOrderPositions();
			base.CurrentOrder = this._stopOrder;
		}

		// Token: 0x06000C5F RID: 3167 RVA: 0x00018998 File Offset: 0x00016B98
		public override TextObject GetBehaviorString()
		{
			TextObject behaviorString = base.GetBehaviorString();
			TextObject textObject = GameTexts.FindText("str_formation_ai_side_strings", base.Formation.AI.Side.ToString());
			behaviorString.SetTextVariable("SIDE_STRING", textObject);
			behaviorString.SetTextVariable("IS_GENERAL_SIDE", "0");
			return behaviorString;
		}

		// Token: 0x06000C60 RID: 3168 RVA: 0x000189F4 File Offset: 0x00016BF4
		private BehaviorAssaultWalls.BehaviorState CheckAndChangeState()
		{
			switch (this._behaviorState)
			{
			case BehaviorAssaultWalls.BehaviorState.Deciding:
				if (!this._isGateLane && this._wallSegment == null)
				{
					return BehaviorAssaultWalls.BehaviorState.Charging;
				}
				if (!this._isGateLane)
				{
					return BehaviorAssaultWalls.BehaviorState.ClimbWall;
				}
				if (this._teamAISiegeComponent.OuterGate.IsGateOpen && this._teamAISiegeComponent.InnerGate.IsGateOpen)
				{
					return BehaviorAssaultWalls.BehaviorState.Charging;
				}
				return BehaviorAssaultWalls.BehaviorState.AttackEntity;
			case BehaviorAssaultWalls.BehaviorState.ClimbWall:
			{
				if (this._wallSegment == null)
				{
					return BehaviorAssaultWalls.BehaviorState.Charging;
				}
				bool flag = false;
				if (this._behaviorSide < FormationAI.BehaviorSide.BehaviorSideNotSet)
				{
					SiegeLane siegeLane = TeamAISiegeComponent.SiegeLanes[(int)this._behaviorSide];
					flag = siegeLane.IsUnderAttack() && !siegeLane.IsDefended();
				}
				flag = flag || base.Formation.QuerySystem.MedianPosition.GetNavMeshVec3().DistanceSquared(this._wallSegment.MiddleFrame.Origin.GetNavMeshVec3()) < base.Formation.Depth * base.Formation.Depth;
				if (flag)
				{
					return BehaviorAssaultWalls.BehaviorState.TakeControl;
				}
				return BehaviorAssaultWalls.BehaviorState.ClimbWall;
			}
			case BehaviorAssaultWalls.BehaviorState.AttackEntity:
				if (this._teamAISiegeComponent.OuterGate.IsGateOpen && this._teamAISiegeComponent.InnerGate.IsGateOpen)
				{
					return BehaviorAssaultWalls.BehaviorState.Charging;
				}
				return BehaviorAssaultWalls.BehaviorState.AttackEntity;
			case BehaviorAssaultWalls.BehaviorState.TakeControl:
				if (base.Formation.QuerySystem.ClosestEnemyFormation == null)
				{
					return BehaviorAssaultWalls.BehaviorState.Deciding;
				}
				if (TeamAISiegeComponent.SiegeLanes.FirstOrDefault((SiegeLane sl) => sl.LaneSide == this._behaviorSide).IsDefended())
				{
					return BehaviorAssaultWalls.BehaviorState.TakeControl;
				}
				if (!this._teamAISiegeComponent.OuterGate.IsGateOpen || !this._teamAISiegeComponent.InnerGate.IsGateOpen)
				{
					return BehaviorAssaultWalls.BehaviorState.MoveToGate;
				}
				return BehaviorAssaultWalls.BehaviorState.Charging;
			case BehaviorAssaultWalls.BehaviorState.MoveToGate:
				if (this._teamAISiegeComponent.OuterGate.IsGateOpen && this._teamAISiegeComponent.InnerGate.IsGateOpen)
				{
					return BehaviorAssaultWalls.BehaviorState.Charging;
				}
				return BehaviorAssaultWalls.BehaviorState.MoveToGate;
			case BehaviorAssaultWalls.BehaviorState.Charging:
				if ((!this._isGateLane || !this._teamAISiegeComponent.OuterGate.IsGateOpen || !this._teamAISiegeComponent.InnerGate.IsGateOpen) && this._behaviorSide < FormationAI.BehaviorSide.BehaviorSideNotSet)
				{
					if (!TeamAISiegeComponent.SiegeLanes[(int)this._behaviorSide].IsOpen && !TeamAISiegeComponent.IsFormationInsideCastle(base.Formation, true, 0.4f))
					{
						return BehaviorAssaultWalls.BehaviorState.Deciding;
					}
					if (base.Formation.QuerySystem.ClosestEnemyFormation == null)
					{
						return BehaviorAssaultWalls.BehaviorState.Stop;
					}
				}
				return BehaviorAssaultWalls.BehaviorState.Charging;
			default:
				if (base.Formation.QuerySystem.ClosestEnemyFormation != null)
				{
					return BehaviorAssaultWalls.BehaviorState.Deciding;
				}
				return BehaviorAssaultWalls.BehaviorState.Stop;
			}
		}

		// Token: 0x06000C61 RID: 3169 RVA: 0x00018C44 File Offset: 0x00016E44
		protected override void CalculateCurrentOrder()
		{
			switch (this._behaviorState)
			{
			case BehaviorAssaultWalls.BehaviorState.Deciding:
				base.CurrentOrder = this._stopOrder;
				return;
			case BehaviorAssaultWalls.BehaviorState.ClimbWall:
			{
				base.CurrentOrder = this._wallSegmentMoveOrder;
				WorldFrame worldFrame = this._wallSegment.MiddleFrame;
				this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(-worldFrame.Rotation.f.AsVec2.Normalized());
				this.CurrentArrangementOrder = ArrangementOrder.ArrangementOrderLine;
				return;
			}
			case BehaviorAssaultWalls.BehaviorState.AttackEntity:
				if (!this._teamAISiegeComponent.OuterGate.IsGateOpen)
				{
					base.CurrentOrder = this._attackEntityOrderOuterGate;
				}
				else
				{
					base.CurrentOrder = this._attackEntityOrderInnerGate;
				}
				this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtEnemy;
				this.CurrentArrangementOrder = ArrangementOrder.ArrangementOrderLine;
				return;
			case BehaviorAssaultWalls.BehaviorState.TakeControl:
			{
				if (base.Formation.QuerySystem.ClosestEnemyFormation != null)
				{
					base.CurrentOrder = MovementOrder.MovementOrderChargeToTarget(base.Formation.QuerySystem.ClosestEnemyFormation.Formation);
				}
				else
				{
					base.CurrentOrder = MovementOrder.MovementOrderCharge;
				}
				WorldFrame worldFrame = this._wallSegment.MiddleFrame;
				this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(-worldFrame.Rotation.f.AsVec2.Normalized());
				this.CurrentArrangementOrder = ArrangementOrder.ArrangementOrderLine;
				return;
			}
			case BehaviorAssaultWalls.BehaviorState.MoveToGate:
			{
				base.CurrentOrder = this._castleGateMoveOrder;
				WorldFrame worldFrame = this._innerGate.MiddleFrame;
				this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(-worldFrame.Rotation.f.AsVec2.Normalized());
				this.CurrentArrangementOrder = ArrangementOrder.ArrangementOrderLine;
				return;
			}
			case BehaviorAssaultWalls.BehaviorState.Charging:
				base.CurrentOrder = this._chargeOrder;
				this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtEnemy;
				this.CurrentArrangementOrder = ArrangementOrder.ArrangementOrderLoose;
				return;
			case BehaviorAssaultWalls.BehaviorState.Stop:
				base.CurrentOrder = this._stopOrder;
				return;
			default:
				return;
			}
		}

		// Token: 0x06000C62 RID: 3170 RVA: 0x00018E14 File Offset: 0x00017014
		public override void OnValidBehaviorSideChanged()
		{
			base.OnValidBehaviorSideChanged();
			this.ResetOrderPositions();
			this._behaviorState = BehaviorAssaultWalls.BehaviorState.Deciding;
		}

		// Token: 0x06000C63 RID: 3171 RVA: 0x00018E2C File Offset: 0x0001702C
		public override void TickOccasionally()
		{
			BehaviorAssaultWalls.BehaviorState behaviorState = this.CheckAndChangeState();
			this._behaviorState = behaviorState;
			this.CalculateCurrentOrder();
			foreach (IPrimarySiegeWeapon primarySiegeWeapon in this._primarySiegeWeapons)
			{
				UsableMachine usableMachine = primarySiegeWeapon as UsableMachine;
				if (!usableMachine.IsDeactivated && !primarySiegeWeapon.HasCompletedAction() && !usableMachine.IsUsedByFormation(base.Formation))
				{
					base.Formation.StartUsingMachine(primarySiegeWeapon as UsableMachine, false);
				}
			}
			if (this._behaviorState == BehaviorAssaultWalls.BehaviorState.MoveToGate)
			{
				CastleGate castleGate = this._teamAISiegeComponent.InnerGate;
				if (castleGate != null && !castleGate.IsGateOpen && !castleGate.IsDestroyed)
				{
					if (!castleGate.IsUsedByFormation(base.Formation))
					{
						base.Formation.StartUsingMachine(castleGate, false);
					}
				}
				else
				{
					castleGate = this._teamAISiegeComponent.OuterGate;
					if (castleGate != null && !castleGate.IsGateOpen && !castleGate.IsDestroyed && !castleGate.IsUsedByFormation(base.Formation))
					{
						base.Formation.StartUsingMachine(castleGate, false);
					}
				}
			}
			else
			{
				if (base.Formation.Detachments.Contains(this._teamAISiegeComponent.OuterGate))
				{
					base.Formation.StopUsingMachine(this._teamAISiegeComponent.OuterGate, false);
				}
				if (base.Formation.Detachments.Contains(this._teamAISiegeComponent.InnerGate))
				{
					base.Formation.StopUsingMachine(this._teamAISiegeComponent.InnerGate, false);
				}
			}
			base.Formation.SetMovementOrder(base.CurrentOrder);
			base.Formation.FacingOrder = this.CurrentFacingOrder;
			base.Formation.ArrangementOrder = this.CurrentArrangementOrder;
		}

		// Token: 0x06000C64 RID: 3172 RVA: 0x00019000 File Offset: 0x00017200
		protected override void OnBehaviorActivatedAux()
		{
			this.CalculateCurrentOrder();
			base.Formation.SetMovementOrder(base.CurrentOrder);
			base.Formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
			base.Formation.FacingOrder = this.CurrentFacingOrder;
			base.Formation.FiringOrder = FiringOrder.FiringOrderHoldYourFire;
			base.Formation.FormOrder = FormOrder.FormOrderDeep;
			base.Formation.WeaponUsageOrder = WeaponUsageOrder.WeaponUsageOrderUseAny;
		}

		// Token: 0x1700030A RID: 778
		// (get) Token: 0x06000C65 RID: 3173 RVA: 0x00019075 File Offset: 0x00017275
		public override float NavmeshlessTargetPositionPenalty
		{
			get
			{
				return 1f;
			}
		}

		// Token: 0x06000C66 RID: 3174 RVA: 0x0001907C File Offset: 0x0001727C
		protected override float GetAiWeight()
		{
			float num = 0f;
			if (this._teamAISiegeComponent != null)
			{
				if (this._primarySiegeWeapons.Any((IPrimarySiegeWeapon psw) => psw.HasCompletedAction()) || this._wallSegment != null)
				{
					if (this._teamAISiegeComponent.IsCastleBreached())
					{
						num = 0.75f;
					}
					else
					{
						num = 0.25f;
					}
				}
				else if (this._teamAISiegeComponent.OuterGate.DefenseSide == this._behaviorSide)
				{
					num = 0.1f;
				}
			}
			return num;
		}

		// Token: 0x040002CF RID: 719
		private BehaviorAssaultWalls.BehaviorState _behaviorState;

		// Token: 0x040002D0 RID: 720
		private List<IPrimarySiegeWeapon> _primarySiegeWeapons;

		// Token: 0x040002D1 RID: 721
		private WallSegment _wallSegment;

		// Token: 0x040002D2 RID: 722
		private CastleGate _innerGate;

		// Token: 0x040002D3 RID: 723
		private TeamAISiegeComponent _teamAISiegeComponent;

		// Token: 0x040002D4 RID: 724
		private MovementOrder _attackEntityOrderInnerGate;

		// Token: 0x040002D5 RID: 725
		private MovementOrder _attackEntityOrderOuterGate;

		// Token: 0x040002D6 RID: 726
		private MovementOrder _chargeOrder;

		// Token: 0x040002D7 RID: 727
		private MovementOrder _stopOrder;

		// Token: 0x040002D8 RID: 728
		private MovementOrder _castleGateMoveOrder;

		// Token: 0x040002D9 RID: 729
		private MovementOrder _wallSegmentMoveOrder;

		// Token: 0x040002DA RID: 730
		private FacingOrder _facingOrder;

		// Token: 0x040002DB RID: 731
		protected ArrangementOrder CurrentArrangementOrder;

		// Token: 0x040002DC RID: 732
		private bool _isGateLane;

		// Token: 0x0200043A RID: 1082
		private enum BehaviorState
		{
			// Token: 0x04001833 RID: 6195
			Deciding,
			// Token: 0x04001834 RID: 6196
			ClimbWall,
			// Token: 0x04001835 RID: 6197
			AttackEntity,
			// Token: 0x04001836 RID: 6198
			TakeControl,
			// Token: 0x04001837 RID: 6199
			MoveToGate,
			// Token: 0x04001838 RID: 6200
			Charging,
			// Token: 0x04001839 RID: 6201
			Stop
		}
	}
}
