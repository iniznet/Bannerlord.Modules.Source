using System;
using System.Linq;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	public class BehaviorSallyOut : BehaviorComponent
	{
		private bool _calculateAreGatesOutsideOpen
		{
			get
			{
				return (this._teamAISiegeDefender.OuterGate == null || this._teamAISiegeDefender.OuterGate.IsGateOpen) && (this._teamAISiegeDefender.InnerGate == null || this._teamAISiegeDefender.InnerGate.IsGateOpen);
			}
		}

		private bool _calculateShouldStartAttacking
		{
			get
			{
				return this._calculateAreGatesOutsideOpen || !TeamAISiegeComponent.IsFormationInsideCastle(base.Formation, true, 0.4f);
			}
		}

		public BehaviorSallyOut(Formation formation)
			: base(formation)
		{
			this._teamAISiegeDefender = formation.Team.TeamAI as TeamAISiegeDefender;
			this._behaviorSide = formation.AI.Side;
			this.ResetOrderPositions();
		}

		protected override void CalculateCurrentOrder()
		{
			base.CalculateCurrentOrder();
			base.CurrentOrder = (this._calculateShouldStartAttacking ? this._attackOrder : this._gatherOrder);
		}

		private void ResetOrderPositions()
		{
			SiegeLane siegeLane = TeamAISiegeComponent.SiegeLanes.FirstOrDefault((SiegeLane sl) => sl.LaneSide == FormationAI.BehaviorSide.Middle);
			WorldFrame? worldFrame;
			if (siegeLane == null)
			{
				worldFrame = null;
			}
			else
			{
				ICastleKeyPosition castleKeyPosition = siegeLane.DefensePoints.FirstOrDefault((ICastleKeyPosition dp) => dp.AttackerSiegeWeapon is UsableMachine && !(dp.AttackerSiegeWeapon as UsableMachine).IsDisabled);
				worldFrame = ((castleKeyPosition != null) ? new WorldFrame?(castleKeyPosition.DefenseWaitFrame) : null);
			}
			WorldFrame worldFrame2 = worldFrame ?? WorldFrame.Invalid;
			TacticalPosition tacticalPosition;
			if (siegeLane == null)
			{
				tacticalPosition = null;
			}
			else
			{
				ICastleKeyPosition castleKeyPosition2 = siegeLane.DefensePoints.FirstOrDefault((ICastleKeyPosition dp) => dp.AttackerSiegeWeapon is UsableMachine && !(dp.AttackerSiegeWeapon as UsableMachine).IsDisabled);
				tacticalPosition = ((castleKeyPosition2 != null) ? castleKeyPosition2.WaitPosition : null);
			}
			this._gatheringTacticalPos = tacticalPosition;
			if (this._gatheringTacticalPos != null)
			{
				this._gatherOrder = MovementOrder.MovementOrderMove(this._gatheringTacticalPos.Position);
			}
			else if (worldFrame2.Origin.IsValid)
			{
				worldFrame2.Rotation.f.Normalize();
				this._gatherOrder = MovementOrder.MovementOrderMove(worldFrame2.Origin);
			}
			else
			{
				this._gatherOrder = MovementOrder.MovementOrderMove(base.Formation.QuerySystem.MedianPosition);
			}
			this._attackOrder = MovementOrder.MovementOrderCharge;
			base.CurrentOrder = (this._calculateShouldStartAttacking ? this._attackOrder : this._gatherOrder);
		}

		public override void TickOccasionally()
		{
			base.TickOccasionally();
			this.CalculateCurrentOrder();
			base.Formation.SetMovementOrder(base.CurrentOrder);
			if (!this._calculateAreGatesOutsideOpen)
			{
				CastleGate castleGate = ((this._teamAISiegeDefender.InnerGate != null && !this._teamAISiegeDefender.InnerGate.IsGateOpen) ? this._teamAISiegeDefender.InnerGate : this._teamAISiegeDefender.OuterGate);
				if (!castleGate.IsUsedByFormation(base.Formation))
				{
					base.Formation.StartUsingMachine(castleGate, false);
				}
			}
		}

		protected override void OnBehaviorActivatedAux()
		{
			this._behaviorSide = base.Formation.AI.Side;
			this.ResetOrderPositions();
			base.Formation.SetMovementOrder(base.CurrentOrder);
			base.Formation.FacingOrder = FacingOrder.FacingOrderLookAtEnemy;
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
			return 10f;
		}

		private readonly TeamAISiegeDefender _teamAISiegeDefender;

		private MovementOrder _gatherOrder;

		private MovementOrder _attackOrder;

		private TacticalPosition _gatheringTacticalPos;
	}
}
