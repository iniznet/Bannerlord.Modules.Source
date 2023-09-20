using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
	public class BehaviorSkirmishLine : BehaviorComponent
	{
		public BehaviorSkirmishLine(Formation formation)
			: base(formation)
		{
			this._behaviorSide = FormationAI.BehaviorSide.BehaviorSideNotSet;
			this._mainFormation = formation.Team.FormationsIncludingEmpty.FirstOrDefaultQ((Formation f) => f.CountOfUnits > 0 && f.AI.IsMainFormation);
			this.CalculateCurrentOrder();
		}

		protected override void CalculateCurrentOrder()
		{
			this._targetFormation = base.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation ?? base.Formation.QuerySystem.ClosestEnemyFormation;
			Vec2 vec;
			WorldPosition worldPosition;
			if (this._targetFormation == null || this._mainFormation == null)
			{
				vec = base.Formation.Direction;
				worldPosition = base.Formation.QuerySystem.MedianPosition;
				worldPosition.SetVec2(base.Formation.QuerySystem.AveragePosition);
			}
			else
			{
				if (this._mainFormation.AI.ActiveBehavior is BehaviorCautiousAdvance)
				{
					vec = this._mainFormation.Direction;
				}
				else
				{
					vec = ((base.Formation.Direction.DotProduct((this._targetFormation.MedianPosition.AsVec2 - this._mainFormation.QuerySystem.MedianPosition.AsVec2).Normalized()) < 0.5f) ? (this._targetFormation.MedianPosition.AsVec2 - this._mainFormation.QuerySystem.MedianPosition.AsVec2) : base.Formation.Direction).Normalized();
				}
				Vec2 vec2 = this._mainFormation.OrderPosition - this._mainFormation.QuerySystem.MedianPosition.AsVec2;
				float num = this._mainFormation.QuerySystem.MovementSpeed * 7f;
				float length = vec2.Length;
				if (length > 0f)
				{
					float num2 = num / length;
					if (num2 < 1f)
					{
						vec2 *= num2;
					}
				}
				worldPosition = this._mainFormation.QuerySystem.MedianPosition;
				worldPosition.SetVec2(worldPosition.AsVec2 + vec * 8f + vec2);
			}
			base.CurrentOrder = MovementOrder.MovementOrderMove(worldPosition);
			if (!this.CurrentFacingOrder.GetDirection(base.Formation, null).IsValid || this.CurrentFacingOrder.OrderEnum == FacingOrder.FacingOrderEnum.LookAtEnemy || (this._targetFormation != null && (base.Formation.QuerySystem.AveragePosition.DistanceSquared(this._targetFormation.MedianPosition.GetNavMeshVec3().AsVec2) >= base.Formation.QuerySystem.MissileRangeAdjusted * base.Formation.QuerySystem.MissileRangeAdjusted || (!this._targetFormation.IsRangedCavalryFormation && this.CurrentFacingOrder.GetDirection(base.Formation, null).DotProduct(vec) <= MBMath.Lerp(0.5f, 1f, 1f - MBMath.ClampFloat(base.Formation.Width, 1f, 20f) * 0.05f, 1E-05f)))))
			{
				this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(vec);
			}
		}

		public override TextObject GetBehaviorString()
		{
			TextObject behaviorString = base.GetBehaviorString();
			if (this._mainFormation != null)
			{
				behaviorString.SetTextVariable("AI_SIDE", GameTexts.FindText("str_formation_ai_side_strings", this._mainFormation.AI.Side.ToString()));
				behaviorString.SetTextVariable("CLASS", GameTexts.FindText("str_formation_class_string", this._mainFormation.PhysicalClass.GetName()));
			}
			return behaviorString;
		}

		public override void OnValidBehaviorSideChanged()
		{
			base.OnValidBehaviorSideChanged();
			this._mainFormation = base.Formation.Team.FormationsIncludingEmpty.FirstOrDefaultQ((Formation f) => f.CountOfUnits > 0 && f.AI.IsMainFormation);
		}

		public override void TickOccasionally()
		{
			this.CalculateCurrentOrder();
			base.Formation.SetMovementOrder(base.CurrentOrder);
			base.Formation.FacingOrder = this.CurrentFacingOrder;
			if (this._mainFormation != null && base.Formation.Width > this._mainFormation.Width * 1.5f)
			{
				base.Formation.FormOrder = FormOrder.FormOrderCustom(this._mainFormation.Width * 1.2f);
			}
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

		protected override float GetAiWeight()
		{
			if (this._mainFormation == null || !this._mainFormation.AI.IsMainFormation)
			{
				this._mainFormation = base.Formation.Team.FormationsIncludingEmpty.FirstOrDefaultQ((Formation f) => f.CountOfUnits > 0 && f.AI.IsMainFormation);
			}
			if (this._behaviorSide != base.Formation.AI.Side)
			{
				this._behaviorSide = base.Formation.AI.Side;
			}
			if (this._mainFormation == null || base.Formation.AI.IsMainFormation || base.Formation.QuerySystem.ClosestEnemyFormation == null)
			{
				return 0f;
			}
			FormationQuerySystem querySystem = base.Formation.QuerySystem;
			float num = MBMath.Lerp(0.1f, 1f, MBMath.ClampFloat(querySystem.RangedUnitRatio + querySystem.RangedCavalryUnitRatio, 0f, 0.5f) * 2f, 1E-05f);
			float num2 = base.Formation.QuerySystem.AveragePosition.Distance((querySystem.ClosestSignificantlyLargeEnemyFormation ?? querySystem.ClosestEnemyFormation).MedianPosition.AsVec2) / (querySystem.ClosestSignificantlyLargeEnemyFormation ?? querySystem.ClosestEnemyFormation).MovementSpeedMaximum;
			float num3 = MBMath.Lerp(0.5f, 1.2f, (MBMath.ClampFloat(num2, 4f, 8f) - 4f) / 4f, 1E-05f);
			return num * querySystem.MainFormationReliabilityFactor * num3;
		}

		private Formation _mainFormation;

		private FormationQuerySystem _targetFormation;
	}
}
