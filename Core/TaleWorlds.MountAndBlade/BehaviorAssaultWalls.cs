using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
	public class BehaviorAssaultWalls : BehaviorComponent
	{
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

		public override TextObject GetBehaviorString()
		{
			TextObject behaviorString = base.GetBehaviorString();
			TextObject textObject = GameTexts.FindText("str_formation_ai_side_strings", base.Formation.AI.Side.ToString());
			behaviorString.SetTextVariable("SIDE_STRING", textObject);
			behaviorString.SetTextVariable("IS_GENERAL_SIDE", "0");
			return behaviorString;
		}

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

		public override void OnValidBehaviorSideChanged()
		{
			base.OnValidBehaviorSideChanged();
			this.ResetOrderPositions();
			this._behaviorState = BehaviorAssaultWalls.BehaviorState.Deciding;
		}

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

		protected override void OnBehaviorActivatedAux()
		{
			this.CalculateCurrentOrder();
			base.Formation.SetMovementOrder(base.CurrentOrder);
			base.Formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
			base.Formation.FacingOrder = this.CurrentFacingOrder;
			base.Formation.FiringOrder = FiringOrder.FiringOrderHoldYourFire;
			base.Formation.FormOrder = FormOrder.FormOrderDeep;
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

		private BehaviorAssaultWalls.BehaviorState _behaviorState;

		private List<IPrimarySiegeWeapon> _primarySiegeWeapons;

		private WallSegment _wallSegment;

		private CastleGate _innerGate;

		private TeamAISiegeComponent _teamAISiegeComponent;

		private MovementOrder _attackEntityOrderInnerGate;

		private MovementOrder _attackEntityOrderOuterGate;

		private MovementOrder _chargeOrder;

		private MovementOrder _stopOrder;

		private MovementOrder _castleGateMoveOrder;

		private MovementOrder _wallSegmentMoveOrder;

		private FacingOrder _facingOrder;

		protected ArrangementOrder CurrentArrangementOrder;

		private bool _isGateLane;

		private enum BehaviorState
		{
			Deciding,
			ClimbWall,
			AttackEntity,
			TakeControl,
			MoveToGate,
			Charging,
			Stop
		}
	}
}
