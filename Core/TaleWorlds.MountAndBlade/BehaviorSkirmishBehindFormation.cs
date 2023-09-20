using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
	public class BehaviorSkirmishBehindFormation : BehaviorComponent
	{
		public BehaviorSkirmishBehindFormation(Formation formation)
			: base(formation)
		{
			this._behaviorSide = formation.AI.Side;
			this.CalculateCurrentOrder();
		}

		protected override void CalculateCurrentOrder()
		{
			Vec2 vec;
			if (base.Formation.QuerySystem.ClosestEnemyFormation != null)
			{
				vec = ((base.Formation.Direction.DotProduct((base.Formation.QuerySystem.ClosestEnemyFormation.MedianPosition.AsVec2 - base.Formation.QuerySystem.AveragePosition).Normalized()) > 0.5f) ? (base.Formation.QuerySystem.ClosestEnemyFormation.MedianPosition.AsVec2 - base.Formation.QuerySystem.AveragePosition) : base.Formation.Direction).Normalized();
			}
			else
			{
				vec = base.Formation.Direction;
			}
			WorldPosition worldPosition;
			if (this.ReferenceFormation == null)
			{
				worldPosition = base.Formation.QuerySystem.MedianPosition;
				worldPosition.SetVec2(base.Formation.QuerySystem.AveragePosition);
			}
			else
			{
				worldPosition = this.ReferenceFormation.QuerySystem.MedianPosition;
				worldPosition.SetVec2(worldPosition.AsVec2 - vec * ((this.ReferenceFormation.Depth + base.Formation.Depth) * 0.5f));
			}
			if (base.CurrentOrder.GetPosition(base.Formation).IsValid)
			{
				base.CurrentOrder = MovementOrder.MovementOrderMove(worldPosition);
			}
			else
			{
				FormationQuerySystem closestSignificantlyLargeEnemyFormation = base.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation;
				if ((closestSignificantlyLargeEnemyFormation != null && (!closestSignificantlyLargeEnemyFormation.IsRangedCavalryFormation || base.Formation.QuerySystem.AveragePosition.DistanceSquared(base.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.MedianPosition.GetNavMeshVec3().AsVec2) >= closestSignificantlyLargeEnemyFormation.MissileRangeAdjusted * closestSignificantlyLargeEnemyFormation.MissileRangeAdjusted)) || base.CurrentOrder.CreateNewOrderWorldPosition(base.Formation, WorldPosition.WorldPositionEnforcedCache.NavMeshVec3).GetNavMeshVec3().DistanceSquared(worldPosition.GetNavMeshVec3()) >= base.Formation.Depth * base.Formation.Depth)
				{
					base.CurrentOrder = MovementOrder.MovementOrderMove(worldPosition);
				}
			}
			if (!this.CurrentFacingOrder.GetDirection(base.Formation, null).IsValid || this.CurrentFacingOrder.OrderEnum == FacingOrder.FacingOrderEnum.LookAtEnemy || base.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation == null || base.Formation.QuerySystem.AveragePosition.DistanceSquared(base.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.MedianPosition.GetNavMeshVec3().AsVec2) >= base.Formation.QuerySystem.MissileRangeAdjusted * base.Formation.QuerySystem.MissileRangeAdjusted || (!base.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.IsRangedCavalryFormation && this.CurrentFacingOrder.GetDirection(base.Formation, null).DotProduct(vec) <= MBMath.Lerp(0.5f, 1f, 1f - MBMath.ClampFloat(base.Formation.Width, 1f, 20f) * 0.05f, 1E-05f)))
			{
				this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(vec);
			}
		}

		public override void TickOccasionally()
		{
			this.CalculateCurrentOrder();
			bool flag = base.Formation.QuerySystem.ClosestEnemyFormation == null || this.ReferenceFormation.QuerySystem.MedianPosition.AsVec2.DistanceSquared(base.Formation.QuerySystem.ClosestEnemyFormation.MedianPosition.AsVec2) <= base.Formation.QuerySystem.AveragePosition.DistanceSquared(base.Formation.QuerySystem.ClosestEnemyFormation.MedianPosition.AsVec2) || base.Formation.QuerySystem.AveragePosition.DistanceSquared(base.CurrentOrder.GetPosition(base.Formation)) <= (this.ReferenceFormation.Depth + base.Formation.Depth) * (this.ReferenceFormation.Depth + base.Formation.Depth) * 0.25f;
			if (flag != this._isFireAtWill)
			{
				this._isFireAtWill = flag;
				if (this._isFireAtWill)
				{
					base.Formation.FiringOrder = FiringOrder.FiringOrderFireAtWill;
				}
				else
				{
					base.Formation.FiringOrder = FiringOrder.FiringOrderHoldYourFire;
				}
			}
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
			base.Formation.FormOrder = FormOrder.FormOrderWider;
		}

		public override TextObject GetBehaviorString()
		{
			TextObject behaviorString = base.GetBehaviorString();
			if (this.ReferenceFormation != null)
			{
				behaviorString.SetTextVariable("AI_SIDE", GameTexts.FindText("str_formation_ai_side_strings", this.ReferenceFormation.AI.Side.ToString()));
				behaviorString.SetTextVariable("CLASS", GameTexts.FindText("str_formation_class_string", this.ReferenceFormation.PhysicalClass.GetName()));
			}
			return behaviorString;
		}

		protected override float GetAiWeight()
		{
			return 10f;
		}

		public Formation ReferenceFormation;

		private bool _isFireAtWill = true;
	}
}
