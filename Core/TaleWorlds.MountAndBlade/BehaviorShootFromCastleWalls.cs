using System;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200011C RID: 284
	public class BehaviorShootFromCastleWalls : BehaviorComponent
	{
		// Token: 0x1700031E RID: 798
		// (get) Token: 0x06000D7F RID: 3455 RVA: 0x00023253 File Offset: 0x00021453
		// (set) Token: 0x06000D80 RID: 3456 RVA: 0x0002325B File Offset: 0x0002145B
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
					this.OnArcherPositionSet(value);
				}
			}
		}

		// Token: 0x06000D81 RID: 3457 RVA: 0x00023272 File Offset: 0x00021472
		public BehaviorShootFromCastleWalls(Formation formation)
			: base(formation)
		{
			this.OnArcherPositionSet(this._archerPosition);
			base.BehaviorCoherence = 0f;
		}

		// Token: 0x06000D82 RID: 3458 RVA: 0x00023294 File Offset: 0x00021494
		private void OnArcherPositionSet(GameEntity value)
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
			this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(this._archerPosition.GetGlobalFrame().rotation.f.AsVec2);
		}

		// Token: 0x06000D83 RID: 3459 RVA: 0x00023384 File Offset: 0x00021584
		public override void TickOccasionally()
		{
			base.Formation.SetMovementOrder(base.CurrentOrder);
			base.Formation.FacingOrder = this.CurrentFacingOrder;
			if (this._tacticalArcherPosition != null)
			{
				base.Formation.FormOrder = FormOrder.FormOrderCustom(this._tacticalArcherPosition.Width);
			}
		}

		// Token: 0x06000D84 RID: 3460 RVA: 0x000233D8 File Offset: 0x000215D8
		protected override void OnBehaviorActivatedAux()
		{
			base.Formation.SetMovementOrder(base.CurrentOrder);
			base.Formation.FacingOrder = this.CurrentFacingOrder;
			base.Formation.ArrangementOrder = ArrangementOrder.ArrangementOrderScatter;
			base.Formation.FiringOrder = FiringOrder.FiringOrderFireAtWill;
			base.Formation.FormOrder = FormOrder.FormOrderWide;
			base.Formation.WeaponUsageOrder = WeaponUsageOrder.WeaponUsageOrderUseAny;
		}

		// Token: 0x1700031F RID: 799
		// (get) Token: 0x06000D85 RID: 3461 RVA: 0x00023447 File Offset: 0x00021647
		public override float NavmeshlessTargetPositionPenalty
		{
			get
			{
				return 1f;
			}
		}

		// Token: 0x06000D86 RID: 3462 RVA: 0x0002344E File Offset: 0x0002164E
		protected override float GetAiWeight()
		{
			return 10f * (base.Formation.QuerySystem.RangedCavalryUnitRatio + base.Formation.QuerySystem.RangedUnitRatio);
		}

		// Token: 0x04000343 RID: 835
		private GameEntity _archerPosition;

		// Token: 0x04000344 RID: 836
		private TacticalPosition _tacticalArcherPosition;
	}
}
