using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class BehaviorHoldHighGround : BehaviorComponent
	{
		public BehaviorHoldHighGround(Formation formation)
			: base(formation)
		{
			this._isAllowedToChangePosition = true;
			this.RangedAllyFormation = null;
			this.CalculateCurrentOrder();
		}

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

		public override void TickOccasionally()
		{
			this.CalculateCurrentOrder();
			base.Formation.SetMovementOrder(base.CurrentOrder);
			base.Formation.FacingOrder = this.CurrentFacingOrder;
		}

		protected override void OnBehaviorActivatedAux()
		{
			this.CalculateCurrentOrder();
			base.Formation.SetMovementOrder(base.CurrentOrder);
			base.Formation.FacingOrder = this.CurrentFacingOrder;
			base.Formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
			base.Formation.FiringOrder = FiringOrder.FiringOrderFireAtWill;
			base.Formation.FormOrder = FormOrder.FormOrderDeep;
		}

		protected override float GetAiWeight()
		{
			if (base.Formation.QuerySystem.ClosestEnemyFormation == null)
			{
				return 0f;
			}
			return 1f;
		}

		public Formation RangedAllyFormation;

		private bool _isAllowedToChangePosition;

		private WorldPosition _lastChosenPosition;
	}
}
