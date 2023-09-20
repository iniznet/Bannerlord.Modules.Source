using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
	public class BehaviorProtectFlank : BehaviorComponent
	{
		public BehaviorProtectFlank(Formation formation)
			: base(formation)
		{
			this._protectFlankState = BehaviorProtectFlank.BehaviorState.HoldingFlank;
			this._behaviorSide = formation.AI.Side;
			this._mainFormation = formation.Team.FormationsIncludingEmpty.FirstOrDefaultQ((Formation f) => f.CountOfUnits > 0 && f.AI.IsMainFormation);
			this.CalculateCurrentOrder();
			base.CurrentOrder = this._movementOrder;
		}

		protected override void CalculateCurrentOrder()
		{
			if (this._mainFormation == null || base.Formation.AI.IsMainFormation || base.Formation.QuerySystem.ClosestEnemyFormation == null)
			{
				base.CurrentOrder = MovementOrder.MovementOrderStop;
				this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtEnemy;
				return;
			}
			if (this._protectFlankState == BehaviorProtectFlank.BehaviorState.HoldingFlank || this._protectFlankState == BehaviorProtectFlank.BehaviorState.Returning)
			{
				Vec2 direction = this._mainFormation.Direction;
				Vec2 vec = (base.Formation.QuerySystem.Team.MedianTargetFormationPosition.AsVec2 - this._mainFormation.QuerySystem.MedianPosition.AsVec2).Normalized();
				Vec2 vec2;
				if (this._behaviorSide == FormationAI.BehaviorSide.Right || this.FlankSide == FormationAI.BehaviorSide.Right)
				{
					vec2 = this._mainFormation.CurrentPosition + vec.RightVec().Normalized() * (this._mainFormation.Width + base.Formation.Width + 10f);
					vec2 -= vec * (this._mainFormation.Depth + base.Formation.Depth);
				}
				else if (this._behaviorSide == FormationAI.BehaviorSide.Left || this.FlankSide == FormationAI.BehaviorSide.Left)
				{
					vec2 = this._mainFormation.CurrentPosition + vec.LeftVec().Normalized() * (this._mainFormation.Width + base.Formation.Width + 10f);
					vec2 -= vec * (this._mainFormation.Depth + base.Formation.Depth);
				}
				else
				{
					vec2 = this._mainFormation.CurrentPosition + vec * ((this._mainFormation.Depth + base.Formation.Depth) * 0.5f + 10f);
				}
				WorldPosition medianPosition = this._mainFormation.QuerySystem.MedianPosition;
				medianPosition.SetVec2(vec2);
				this._movementOrder = MovementOrder.MovementOrderMove(medianPosition);
				base.CurrentOrder = this._movementOrder;
				this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(direction);
			}
		}

		private void CheckAndChangeState()
		{
			Vec2 position = this._movementOrder.GetPosition(base.Formation);
			switch (this._protectFlankState)
			{
			case BehaviorProtectFlank.BehaviorState.HoldingFlank:
				if (base.Formation.QuerySystem.ClosestEnemyFormation != null)
				{
					float num = 50f + (base.Formation.Depth + base.Formation.QuerySystem.ClosestEnemyFormation.Formation.Depth) / 2f;
					if (base.Formation.QuerySystem.ClosestEnemyFormation.MedianPosition.AsVec2.DistanceSquared(position) < num * num)
					{
						this._chargeToTargetOrder = MovementOrder.MovementOrderChargeToTarget(base.Formation.QuerySystem.ClosestEnemyFormation.Formation);
						base.CurrentOrder = this._chargeToTargetOrder;
						this._protectFlankState = BehaviorProtectFlank.BehaviorState.Charging;
						return;
					}
				}
				break;
			case BehaviorProtectFlank.BehaviorState.Charging:
			{
				if (base.Formation.QuerySystem.ClosestEnemyFormation == null)
				{
					base.CurrentOrder = this._movementOrder;
					this._protectFlankState = BehaviorProtectFlank.BehaviorState.Returning;
					return;
				}
				float num2 = 60f + (base.Formation.Depth + base.Formation.QuerySystem.ClosestEnemyFormation.Formation.Depth) / 2f;
				if (base.Formation.QuerySystem.AveragePosition.DistanceSquared(position) > num2 * num2)
				{
					base.CurrentOrder = this._movementOrder;
					this._protectFlankState = BehaviorProtectFlank.BehaviorState.Returning;
					return;
				}
				break;
			}
			case BehaviorProtectFlank.BehaviorState.Returning:
				if (base.Formation.QuerySystem.AveragePosition.DistanceSquared(position) < 400f)
				{
					this._protectFlankState = BehaviorProtectFlank.BehaviorState.HoldingFlank;
				}
				break;
			default:
				return;
			}
		}

		public override void OnValidBehaviorSideChanged()
		{
			base.OnValidBehaviorSideChanged();
			this._mainFormation = base.Formation.Team.FormationsIncludingEmpty.FirstOrDefaultQ((Formation f) => f.CountOfUnits > 0 && f.AI.IsMainFormation);
		}

		public override void TickOccasionally()
		{
			this.CheckAndChangeState();
			this.CalculateCurrentOrder();
			base.Formation.SetMovementOrder(base.CurrentOrder);
			base.Formation.FacingOrder = this.CurrentFacingOrder;
			if (this._protectFlankState == BehaviorProtectFlank.BehaviorState.HoldingFlank && base.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation != null && base.Formation.QuerySystem.AveragePosition.DistanceSquared(base.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.MedianPosition.AsVec2) > 1600f && base.Formation.QuerySystem.UnderRangedAttackRatio > 0.2f - ((base.Formation.ArrangementOrder.OrderEnum == ArrangementOrder.ArrangementOrderEnum.Loose) ? 0.1f : 0f))
			{
				base.Formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLoose;
				return;
			}
			base.Formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
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

		public override TextObject GetBehaviorString()
		{
			TextObject behaviorString = base.GetBehaviorString();
			TextObject textObject = GameTexts.FindText("str_formation_ai_side_strings", base.Formation.AI.Side.ToString());
			behaviorString.SetTextVariable("IS_GENERAL_SIDE", "0");
			behaviorString.SetTextVariable("SIDE_STRING", textObject);
			if (this._mainFormation != null)
			{
				behaviorString.SetTextVariable("AI_SIDE", GameTexts.FindText("str_formation_ai_side_strings", this._mainFormation.AI.Side.ToString()));
				behaviorString.SetTextVariable("CLASS", GameTexts.FindText("str_formation_class_string", this._mainFormation.PhysicalClass.GetName()));
			}
			return behaviorString;
		}

		protected override float GetAiWeight()
		{
			if (this._mainFormation == null || !this._mainFormation.AI.IsMainFormation)
			{
				this._mainFormation = base.Formation.Team.FormationsIncludingEmpty.FirstOrDefaultQ((Formation f) => f.CountOfUnits > 0 && f.AI.IsMainFormation);
			}
			if (this._mainFormation == null || base.Formation.AI.IsMainFormation)
			{
				return 0f;
			}
			return 1.2f;
		}

		private Formation _mainFormation;

		public FormationAI.BehaviorSide FlankSide;

		private BehaviorProtectFlank.BehaviorState _protectFlankState;

		private MovementOrder _movementOrder;

		private MovementOrder _chargeToTargetOrder;

		private enum BehaviorState
		{
			HoldingFlank,
			Charging,
			Returning
		}
	}
}
