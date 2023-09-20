using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
	public class BehaviorFlank : BehaviorComponent
	{
		public BehaviorFlank(Formation formation)
			: base(formation)
		{
			base.BehaviorCoherence = 0.5f;
			this.CalculateCurrentOrder();
		}

		protected override void CalculateCurrentOrder()
		{
			WorldPosition worldPosition = ((base.Formation.AI.Side == FormationAI.BehaviorSide.Right) ? base.Formation.QuerySystem.Team.RightFlankEdgePosition : base.Formation.QuerySystem.Team.LeftFlankEdgePosition);
			Vec2 vec = (worldPosition.AsVec2 - base.Formation.QuerySystem.AveragePosition).Normalized();
			base.CurrentOrder = MovementOrder.MovementOrderMove(worldPosition);
			this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(vec);
		}

		public override void TickOccasionally()
		{
			this.CalculateCurrentOrder();
			base.Formation.SetMovementOrder(base.CurrentOrder);
			base.Formation.FacingOrder = this.CurrentFacingOrder;
		}

		public override TextObject GetBehaviorString()
		{
			TextObject behaviorString = base.GetBehaviorString();
			behaviorString.SetTextVariable("IS_GENERAL_SIDE", "0");
			TextObject textObject = GameTexts.FindText("str_formation_ai_side_strings", base.Formation.AI.Side.ToString());
			behaviorString.SetTextVariable("SIDE_STRING", textObject);
			return behaviorString;
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
			FormationQuerySystem querySystem = base.Formation.QuerySystem;
			if (querySystem.ClosestEnemyFormation == null || querySystem.ClosestEnemyFormation.ClosestEnemyFormation == querySystem)
			{
				return 0f;
			}
			Vec2 vec = (querySystem.ClosestEnemyFormation.MedianPosition.AsVec2 - querySystem.AveragePosition).Normalized();
			Vec2 vec2 = (querySystem.ClosestEnemyFormation.ClosestEnemyFormation.MedianPosition.AsVec2 - querySystem.ClosestEnemyFormation.MedianPosition.AsVec2).Normalized();
			if (vec.DotProduct(vec2) > -0.5f)
			{
				return 0f;
			}
			if (Mission.Current.MissionTeamAIType != Mission.MissionTeamAITypeEnum.FieldBattle)
			{
				int num = -1;
				Vec3 navMeshVec = ((base.Formation.AI.Side == FormationAI.BehaviorSide.Right) ? base.Formation.QuerySystem.Team.RightFlankEdgePosition : base.Formation.QuerySystem.Team.LeftFlankEdgePosition).GetNavMeshVec3();
				Mission.Current.Scene.GetNavigationMeshForPosition(ref navMeshVec, out num, 1.5f);
				if (num >= 0)
				{
					Agent medianAgent = base.Formation.GetMedianAgent(true, true, base.Formation.QuerySystem.AveragePosition);
					if ((medianAgent != null && medianAgent.GetCurrentNavigationFaceId() % 10 == 1) == (num % 10 == 1))
					{
						goto IL_158;
					}
				}
				return 0f;
			}
			IL_158:
			return 1.2f;
		}
	}
}
