using System;
using System.Linq;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000115 RID: 277
	public class BehaviorSallyOut : BehaviorComponent
	{
		// Token: 0x1700031B RID: 795
		// (get) Token: 0x06000D34 RID: 3380 RVA: 0x000209A0 File Offset: 0x0001EBA0
		private bool _calculateAreGatesOutsideOpen
		{
			get
			{
				return (this._teamAISiegeDefender.OuterGate == null || this._teamAISiegeDefender.OuterGate.IsGateOpen) && (this._teamAISiegeDefender.InnerGate == null || this._teamAISiegeDefender.InnerGate.IsGateOpen);
			}
		}

		// Token: 0x1700031C RID: 796
		// (get) Token: 0x06000D35 RID: 3381 RVA: 0x000209ED File Offset: 0x0001EBED
		private bool _calculateShouldStartAttacking
		{
			get
			{
				return this._calculateAreGatesOutsideOpen || !TeamAISiegeComponent.IsFormationInsideCastle(base.Formation, true, 0.4f);
			}
		}

		// Token: 0x06000D36 RID: 3382 RVA: 0x00020A0D File Offset: 0x0001EC0D
		public BehaviorSallyOut(Formation formation)
			: base(formation)
		{
			this._teamAISiegeDefender = formation.Team.TeamAI as TeamAISiegeDefender;
			this._behaviorSide = formation.AI.Side;
			this.ResetOrderPositions();
		}

		// Token: 0x06000D37 RID: 3383 RVA: 0x00020A43 File Offset: 0x0001EC43
		protected override void CalculateCurrentOrder()
		{
			base.CalculateCurrentOrder();
			base.CurrentOrder = (this._calculateShouldStartAttacking ? this._attackOrder : this._gatherOrder);
		}

		// Token: 0x06000D38 RID: 3384 RVA: 0x00020A68 File Offset: 0x0001EC68
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

		// Token: 0x06000D39 RID: 3385 RVA: 0x00020BE0 File Offset: 0x0001EDE0
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

		// Token: 0x06000D3A RID: 3386 RVA: 0x00020C68 File Offset: 0x0001EE68
		protected override void OnBehaviorActivatedAux()
		{
			this._behaviorSide = base.Formation.AI.Side;
			this.ResetOrderPositions();
			base.Formation.SetMovementOrder(base.CurrentOrder);
			base.Formation.FacingOrder = FacingOrder.FacingOrderLookAtEnemy;
			base.Formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
			base.Formation.FiringOrder = FiringOrder.FiringOrderFireAtWill;
			base.Formation.FormOrder = FormOrder.FormOrderWide;
			base.Formation.WeaponUsageOrder = WeaponUsageOrder.WeaponUsageOrderUseAny;
		}

		// Token: 0x1700031D RID: 797
		// (get) Token: 0x06000D3B RID: 3387 RVA: 0x00020CF2 File Offset: 0x0001EEF2
		public override float NavmeshlessTargetPositionPenalty
		{
			get
			{
				return 1f;
			}
		}

		// Token: 0x06000D3C RID: 3388 RVA: 0x00020CF9 File Offset: 0x0001EEF9
		protected override float GetAiWeight()
		{
			return 10f;
		}

		// Token: 0x04000330 RID: 816
		private readonly TeamAISiegeDefender _teamAISiegeDefender;

		// Token: 0x04000331 RID: 817
		private MovementOrder _gatherOrder;

		// Token: 0x04000332 RID: 818
		private MovementOrder _attackOrder;

		// Token: 0x04000333 RID: 819
		private TacticalPosition _gatheringTacticalPos;
	}
}
