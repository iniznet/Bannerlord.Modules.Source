using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000109 RID: 265
	public class BehaviorHoldHighGround : BehaviorComponent
	{
		// Token: 0x06000CEC RID: 3308 RVA: 0x0001DF1A File Offset: 0x0001C11A
		public BehaviorHoldHighGround(Formation formation)
			: base(formation)
		{
			this._isAllowedToChangePosition = true;
			this.RangedAllyFormation = null;
			this.CalculateCurrentOrder();
		}

		// Token: 0x06000CED RID: 3309 RVA: 0x0001DF38 File Offset: 0x0001C138
		protected override void CalculateCurrentOrder()
		{
			WorldPosition worldPosition;
			Vec2 vec;
			if (base.Formation.QuerySystem.ClosestEnemyFormation != null)
			{
				worldPosition = base.Formation.QuerySystem.MedianPosition;
				if (base.Formation.AI.ActiveBehavior != this)
				{
					this._isAllowedToChangePosition = true;
				}
				else
				{
					float num = Math.Max((this.RangedAllyFormation != null) ? (this.RangedAllyFormation.QuerySystem.MissileRangeAdjusted * 0.8f) : 0f, 30f);
					this._isAllowedToChangePosition = base.Formation.QuerySystem.AveragePosition.DistanceSquared(base.Formation.QuerySystem.ClosestEnemyFormation.MedianPosition.AsVec2) > num * num;
				}
				if (this._isAllowedToChangePosition)
				{
					worldPosition.SetVec2(base.Formation.QuerySystem.HighGroundCloseToForeseenBattleGround);
					this._lastChosenPosition = worldPosition;
				}
				else
				{
					worldPosition = this._lastChosenPosition;
				}
				vec = ((base.Formation.QuerySystem.AveragePosition.DistanceSquared(base.Formation.QuerySystem.HighGroundCloseToForeseenBattleGround) > 25f) ? (base.Formation.QuerySystem.Team.MedianTargetFormationPosition.AsVec2 - worldPosition.AsVec2).Normalized() : ((base.Formation.Direction.DotProduct((base.Formation.QuerySystem.ClosestEnemyFormation.MedianPosition.AsVec2 - base.Formation.QuerySystem.AveragePosition).Normalized()) < 0.5f) ? (base.Formation.QuerySystem.ClosestEnemyFormation.MedianPosition.AsVec2 - base.Formation.QuerySystem.AveragePosition) : base.Formation.Direction).Normalized());
			}
			else
			{
				vec = base.Formation.Direction;
				worldPosition = base.Formation.QuerySystem.MedianPosition;
				worldPosition.SetVec2(base.Formation.QuerySystem.AveragePosition);
			}
			base.CurrentOrder = MovementOrder.MovementOrderMove(worldPosition);
			this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(vec);
		}

		// Token: 0x06000CEE RID: 3310 RVA: 0x0001E17A File Offset: 0x0001C37A
		public override void TickOccasionally()
		{
			this.CalculateCurrentOrder();
			base.Formation.SetMovementOrder(base.CurrentOrder);
			base.Formation.FacingOrder = this.CurrentFacingOrder;
		}

		// Token: 0x06000CEF RID: 3311 RVA: 0x0001E1A4 File Offset: 0x0001C3A4
		protected override void OnBehaviorActivatedAux()
		{
			this.CalculateCurrentOrder();
			base.Formation.SetMovementOrder(base.CurrentOrder);
			base.Formation.FacingOrder = this.CurrentFacingOrder;
			base.Formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
			base.Formation.FiringOrder = FiringOrder.FiringOrderFireAtWill;
			base.Formation.FormOrder = FormOrder.FormOrderDeep;
			base.Formation.WeaponUsageOrder = WeaponUsageOrder.WeaponUsageOrderUseAny;
		}

		// Token: 0x06000CF0 RID: 3312 RVA: 0x0001E219 File Offset: 0x0001C419
		protected override float GetAiWeight()
		{
			if (base.Formation.QuerySystem.ClosestEnemyFormation == null)
			{
				return 0f;
			}
			return 1f;
		}

		// Token: 0x0400031D RID: 797
		public Formation RangedAllyFormation;

		// Token: 0x0400031E RID: 798
		private bool _isAllowedToChangePosition;

		// Token: 0x0400031F RID: 799
		private WorldPosition _lastChosenPosition;
	}
}
