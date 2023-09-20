using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
	public class BehaviorVanguard : BehaviorComponent
	{
		public BehaviorVanguard(Formation formation)
			: base(formation)
		{
			this._behaviorSide = formation.AI.Side;
			this._mainFormation = formation.Team.FormationsIncludingEmpty.FirstOrDefaultQ((Formation f) => f.CountOfUnits > 0 && f.AI.IsMainFormation);
			this.CalculateCurrentOrder();
		}

		protected override void CalculateCurrentOrder()
		{
			if (this._mainFormation != null && this._mainFormation.CountOfUnits == 0)
			{
				this._mainFormation = base.Formation.Team.FormationsIncludingEmpty.FirstOrDefaultQ((Formation f) => f.CountOfUnits > 0 && f.AI.IsMainFormation);
			}
			Vec2 vec;
			WorldPosition worldPosition;
			if (this._mainFormation != null)
			{
				vec = this._mainFormation.Direction;
				Vec2 vec2 = (base.Formation.QuerySystem.Team.MedianTargetFormationPosition.AsVec2 - this._mainFormation.QuerySystem.MedianPosition.AsVec2).Normalized();
				worldPosition = this._mainFormation.QuerySystem.MedianPosition;
				worldPosition.SetVec2(this._mainFormation.CurrentPosition + vec2 * ((this._mainFormation.Depth + base.Formation.Depth) * 0.5f + 10f));
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
			if (base.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation != null && base.Formation.QuerySystem.AveragePosition.DistanceSquared(base.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.MedianPosition.AsVec2) > 1600f && base.Formation.QuerySystem.UnderRangedAttackRatio > 0.2f - ((base.Formation.ArrangementOrder.OrderEnum == ArrangementOrder.ArrangementOrderEnum.Loose) ? 0.1f : 0f))
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
			base.Formation.FormOrder = FormOrder.FormOrderWide;
			base.Formation.WeaponUsageOrder = WeaponUsageOrder.WeaponUsageOrderUseAny;
		}

		public override TextObject GetBehaviorString()
		{
			TextObject behaviorString = base.GetBehaviorString();
			TextObject textObject = GameTexts.FindText("str_formation_ai_side_strings", base.Formation.AI.Side.ToString());
			behaviorString.SetTextVariable("SIDE_STRING", textObject);
			if (this._mainFormation != null)
			{
				behaviorString.SetTextVariable("AI_SIDE", GameTexts.FindText("str_formation_ai_side_strings", this._mainFormation.AI.Side.ToString()));
				behaviorString.SetTextVariable("CLASS", GameTexts.FindText("str_formation_class_string", this._mainFormation.PrimaryClass.GetName()));
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
	}
}
