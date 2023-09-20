using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class BehaviorRegroup : BehaviorComponent
	{
		public BehaviorRegroup(Formation formation)
			: base(formation)
		{
			base.BehaviorCoherence = 1f;
			this.CalculateCurrentOrder();
		}

		protected override void CalculateCurrentOrder()
		{
			Vec2 vec;
			if (base.Formation.QuerySystem.ClosestEnemyFormation != null)
			{
				vec = (base.Formation.QuerySystem.ClosestEnemyFormation.MedianPosition.AsVec2 - base.Formation.QuerySystem.AveragePosition).Normalized();
			}
			else
			{
				vec = base.Formation.Direction;
			}
			WorldPosition medianPosition = base.Formation.QuerySystem.MedianPosition;
			medianPosition.SetVec2(base.Formation.QuerySystem.AveragePosition);
			base.CurrentOrder = MovementOrder.MovementOrderMove(medianPosition);
			this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(vec);
		}

		public override void TickOccasionally()
		{
			this.CalculateCurrentOrder();
			base.Formation.SetMovementOrder(base.CurrentOrder);
			base.Formation.FacingOrder = this.CurrentFacingOrder;
		}

		protected override float GetAiWeight()
		{
			FormationQuerySystem querySystem = base.Formation.QuerySystem;
			if (base.Formation.AI.ActiveBehavior == null)
			{
				return 0f;
			}
			float behaviorCoherence = base.Formation.AI.ActiveBehavior.BehaviorCoherence;
			return MBMath.Lerp(0.1f, 1.2f, MBMath.ClampFloat(behaviorCoherence * (querySystem.FormationIntegrityData.DeviationOfPositionsExcludeFarAgents + 1f) / (querySystem.IdealAverageDisplacement + 1f), 0f, 3f) / 3f, 1E-05f);
		}
	}
}
