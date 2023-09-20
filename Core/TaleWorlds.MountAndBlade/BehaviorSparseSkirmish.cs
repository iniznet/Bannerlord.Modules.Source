using System;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000121 RID: 289
	public class BehaviorSparseSkirmish : BehaviorComponent
	{
		// Token: 0x17000320 RID: 800
		// (get) Token: 0x06000D9E RID: 3486 RVA: 0x00024BEA File Offset: 0x00022DEA
		// (set) Token: 0x06000D9F RID: 3487 RVA: 0x00024BF2 File Offset: 0x00022DF2
		public GameEntity ArcherPosition
		{
			get
			{
				return this._archerPosition;
			}
			set
			{
				if (this._archerPosition != value)
				{
					this.SetArcherPosition(value);
				}
			}
		}

		// Token: 0x06000DA0 RID: 3488 RVA: 0x00024C0C File Offset: 0x00022E0C
		private void SetArcherPosition(GameEntity value)
		{
			this._archerPosition = value;
			if (!(this._archerPosition != null))
			{
				this._tacticalArcherPosition = null;
				WorldPosition medianPosition = base.Formation.QuerySystem.MedianPosition;
				medianPosition.SetVec2(base.Formation.CurrentPosition);
				base.CurrentOrder = MovementOrder.MovementOrderMove(medianPosition);
				this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtEnemy;
				return;
			}
			this._tacticalArcherPosition = this._archerPosition.GetFirstScriptOfType<TacticalPosition>();
			if (this._tacticalArcherPosition != null)
			{
				base.CurrentOrder = MovementOrder.MovementOrderMove(this._tacticalArcherPosition.Position);
				this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(this._tacticalArcherPosition.Direction);
				return;
			}
			base.CurrentOrder = MovementOrder.MovementOrderMove(this._archerPosition.GlobalPosition.ToWorldPosition());
			this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtEnemy;
		}

		// Token: 0x06000DA1 RID: 3489 RVA: 0x00024CDC File Offset: 0x00022EDC
		public BehaviorSparseSkirmish(Formation formation)
			: base(formation)
		{
			this.SetArcherPosition(this._archerPosition);
			base.BehaviorCoherence = 0f;
		}

		// Token: 0x06000DA2 RID: 3490 RVA: 0x00024CFC File Offset: 0x00022EFC
		public override void TickOccasionally()
		{
			base.Formation.SetMovementOrder(base.CurrentOrder);
			base.Formation.FacingOrder = this.CurrentFacingOrder;
			if (this._tacticalArcherPosition != null)
			{
				base.Formation.FormOrder = FormOrder.FormOrderCustom(this._tacticalArcherPosition.Width);
			}
		}

		// Token: 0x06000DA3 RID: 3491 RVA: 0x00024D50 File Offset: 0x00022F50
		protected override void OnBehaviorActivatedAux()
		{
			base.Formation.SetMovementOrder(base.CurrentOrder);
			base.Formation.FacingOrder = this.CurrentFacingOrder;
			base.Formation.ArrangementOrder = ArrangementOrder.ArrangementOrderScatter;
			base.Formation.FiringOrder = FiringOrder.FiringOrderFireAtWill;
			base.Formation.FormOrder = FormOrder.FormOrderWider;
			base.Formation.WeaponUsageOrder = WeaponUsageOrder.WeaponUsageOrderUseAny;
		}

		// Token: 0x17000321 RID: 801
		// (get) Token: 0x06000DA4 RID: 3492 RVA: 0x00024DBF File Offset: 0x00022FBF
		public override float NavmeshlessTargetPositionPenalty
		{
			get
			{
				return 1f;
			}
		}

		// Token: 0x06000DA5 RID: 3493 RVA: 0x00024DC6 File Offset: 0x00022FC6
		protected override float GetAiWeight()
		{
			return 2f;
		}

		// Token: 0x04000351 RID: 849
		private GameEntity _archerPosition;

		// Token: 0x04000352 RID: 850
		private TacticalPosition _tacticalArcherPosition;
	}
}
