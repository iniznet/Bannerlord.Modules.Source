using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000106 RID: 262
	public class BehaviorFireFromInfantryCover : BehaviorComponent
	{
		// Token: 0x06000CDC RID: 3292 RVA: 0x0001D3F4 File Offset: 0x0001B5F4
		public BehaviorFireFromInfantryCover(Formation formation)
			: base(formation)
		{
			this._mainFormation = formation.Team.FormationsIncludingEmpty.FirstOrDefaultQ((Formation f) => f.CountOfUnits > 0 && f.AI.IsMainFormation);
			this.CalculateCurrentOrder();
		}

		// Token: 0x06000CDD RID: 3293 RVA: 0x0001D44C File Offset: 0x0001B64C
		protected unsafe override void CalculateCurrentOrder()
		{
			WorldPosition medianPosition = base.Formation.QuerySystem.MedianPosition;
			Vec2 vec = base.Formation.Direction;
			if (this._mainFormation == null)
			{
				medianPosition.SetVec2(base.Formation.QuerySystem.AveragePosition);
			}
			else
			{
				MovementOrder movementOrder = *this._mainFormation.GetReadonlyMovementOrderReference();
				Vec2 position = movementOrder.GetPosition(this._mainFormation);
				if (position.IsValid)
				{
					vec = (position - this._mainFormation.QuerySystem.AveragePosition).Normalized();
					Vec2 vec2 = position - vec * this._mainFormation.Depth * 0.33f;
					medianPosition.SetVec2(vec2);
				}
				else
				{
					medianPosition.SetVec2(base.Formation.QuerySystem.AveragePosition);
				}
			}
			base.CurrentOrder = MovementOrder.MovementOrderMove(medianPosition);
			this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(vec);
		}

		// Token: 0x06000CDE RID: 3294 RVA: 0x0001D540 File Offset: 0x0001B740
		public override void TickOccasionally()
		{
			this.CalculateCurrentOrder();
			base.Formation.SetMovementOrder(base.CurrentOrder);
			base.Formation.FacingOrder = this.CurrentFacingOrder;
			if (base.Formation.QuerySystem.AveragePosition.DistanceSquared(base.CurrentOrder.GetPosition(base.Formation)) < 100f)
			{
				base.Formation.ArrangementOrder = ArrangementOrder.ArrangementOrderSquare;
			}
			Vec2 position = base.CurrentOrder.GetPosition(base.Formation);
			bool flag = base.Formation.QuerySystem.ClosestEnemyFormation == null || this._mainFormation.QuerySystem.AveragePosition.DistanceSquared(base.Formation.QuerySystem.AveragePosition) <= base.Formation.Depth * base.Formation.Width || base.Formation.QuerySystem.AveragePosition.DistanceSquared(position) <= (this._mainFormation.Depth + base.Formation.Depth) * (this._mainFormation.Depth + base.Formation.Depth) * 0.25f;
			if (flag != this._isFireAtWill)
			{
				this._isFireAtWill = flag;
				base.Formation.FiringOrder = (this._isFireAtWill ? FiringOrder.FiringOrderFireAtWill : FiringOrder.FiringOrderHoldYourFire);
			}
		}

		// Token: 0x06000CDF RID: 3295 RVA: 0x0001D6AC File Offset: 0x0001B8AC
		protected override void OnBehaviorActivatedAux()
		{
			this.CalculateCurrentOrder();
			base.Formation.SetMovementOrder(base.CurrentOrder);
			base.Formation.FacingOrder = this.CurrentFacingOrder;
			base.Formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
			base.Formation.FiringOrder = FiringOrder.FiringOrderFireAtWill;
			int num = (int)MathF.Sqrt((float)base.Formation.CountOfUnits);
			float num2 = (float)num * base.Formation.UnitDiameter + (float)(num - 1) * base.Formation.Interval;
			base.Formation.FormOrder = FormOrder.FormOrderCustom(num2);
			base.Formation.WeaponUsageOrder = WeaponUsageOrder.WeaponUsageOrderUseAny;
		}

		// Token: 0x06000CE0 RID: 3296 RVA: 0x0001D758 File Offset: 0x0001B958
		protected override float GetAiWeight()
		{
			if (this._mainFormation == null || !this._mainFormation.AI.IsMainFormation)
			{
				this._mainFormation = base.Formation.Team.FormationsIncludingEmpty.FirstOrDefaultQ((Formation f) => f.CountOfUnits > 0 && f.AI.IsMainFormation);
			}
			if (this._mainFormation == null || base.Formation.AI.IsMainFormation || base.Formation.QuerySystem.ClosestEnemyFormation == null || !base.Formation.QuerySystem.IsRangedFormation)
			{
				return 0f;
			}
			return 2f;
		}

		// Token: 0x0400031A RID: 794
		private Formation _mainFormation;

		// Token: 0x0400031B RID: 795
		private bool _isFireAtWill = true;
	}
}
