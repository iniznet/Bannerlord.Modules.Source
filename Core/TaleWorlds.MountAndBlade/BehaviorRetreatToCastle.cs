using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	public class BehaviorRetreatToCastle : BehaviorComponent
	{
		public BehaviorRetreatToCastle(Formation formation)
			: base(formation)
		{
			WorldPosition worldPosition = Mission.Current.DeploymentPlan.GetFormationPlan(formation.Team.Side, FormationClass.Cavalry, DeploymentPlanType.Initial).CreateNewDeploymentWorldPosition(WorldPosition.WorldPositionEnforcedCache.GroundVec3);
			base.CurrentOrder = MovementOrder.MovementOrderMove(worldPosition);
			base.BehaviorCoherence = 0f;
		}

		public override void TickOccasionally()
		{
			base.TickOccasionally();
			if (base.Formation.AI.ActiveBehavior == this)
			{
				base.Formation.SetMovementOrder(base.CurrentOrder);
			}
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
			return 1f;
		}
	}
}
