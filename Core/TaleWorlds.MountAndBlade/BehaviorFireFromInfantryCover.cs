using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;

namespace TaleWorlds.MountAndBlade
{
	public class BehaviorFireFromInfantryCover : BehaviorComponent
	{
		public BehaviorFireFromInfantryCover(Formation formation)
			: base(formation)
		{
			this._mainFormation = formation.Team.FormationsIncludingEmpty.FirstOrDefaultQ((Formation f) => f.CountOfUnits > 0 && f.AI.IsMainFormation);
			this.CalculateCurrentOrder();
		}

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

		private Formation _mainFormation;

		private bool _isFireAtWill = true;
	}
}
