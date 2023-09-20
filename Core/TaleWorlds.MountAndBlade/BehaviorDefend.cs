using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class BehaviorDefend : BehaviorComponent
	{
		public BehaviorDefend(Formation formation)
			: base(formation)
		{
			this.CalculateCurrentOrder();
		}

		protected override void CalculateCurrentOrder()
		{
			Vec2 vec;
			if (this.TacticalDefendPosition != null)
			{
				vec = ((!this.TacticalDefendPosition.IsInsurmountable) ? this.TacticalDefendPosition.Direction : (base.Formation.Team.QuerySystem.AverageEnemyPosition - this.TacticalDefendPosition.Position.AsVec2).Normalized());
			}
			else if (base.Formation.QuerySystem.ClosestEnemyFormation == null)
			{
				vec = base.Formation.Direction;
			}
			else
			{
				vec = ((base.Formation.Direction.DotProduct((base.Formation.QuerySystem.ClosestEnemyFormation.MedianPosition.AsVec2 - base.Formation.QuerySystem.AveragePosition).Normalized()) < 0.5f) ? (base.Formation.QuerySystem.ClosestEnemyFormation.MedianPosition.AsVec2 - base.Formation.QuerySystem.AveragePosition) : base.Formation.Direction).Normalized();
			}
			if (this.TacticalDefendPosition != null)
			{
				if (!this.TacticalDefendPosition.IsInsurmountable)
				{
					base.CurrentOrder = MovementOrder.MovementOrderMove(this.TacticalDefendPosition.Position);
				}
				else
				{
					Vec2 vec2 = this.TacticalDefendPosition.Position.AsVec2 + this.TacticalDefendPosition.Width * 0.5f * vec;
					WorldPosition position = this.TacticalDefendPosition.Position;
					position.SetVec2(vec2);
					base.CurrentOrder = MovementOrder.MovementOrderMove(position);
				}
				if (!this.TacticalDefendPosition.IsInsurmountable)
				{
					this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(vec);
					return;
				}
				this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtEnemy;
				return;
			}
			else
			{
				if (this.DefensePosition.IsValid)
				{
					base.CurrentOrder = MovementOrder.MovementOrderMove(this.DefensePosition);
					this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(vec);
					return;
				}
				WorldPosition medianPosition = base.Formation.QuerySystem.MedianPosition;
				medianPosition.SetVec2(base.Formation.QuerySystem.AveragePosition);
				base.CurrentOrder = MovementOrder.MovementOrderMove(medianPosition);
				this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(vec);
				return;
			}
		}

		public override void TickOccasionally()
		{
			this.CalculateCurrentOrder();
			base.Formation.SetMovementOrder(base.CurrentOrder);
			base.Formation.FacingOrder = this.CurrentFacingOrder;
			if (base.Formation.QuerySystem.AveragePosition.DistanceSquared(base.CurrentOrder.GetPosition(base.Formation)) < 100f)
			{
				if (base.Formation.QuerySystem.HasShield)
				{
					base.Formation.ArrangementOrder = ArrangementOrder.ArrangementOrderShieldWall;
				}
				else if (base.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation != null && base.Formation.QuerySystem.AveragePosition.DistanceSquared(base.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.MedianPosition.AsVec2) > 100f && base.Formation.QuerySystem.UnderRangedAttackRatio > 0.2f - ((base.Formation.ArrangementOrder.OrderEnum == ArrangementOrder.ArrangementOrderEnum.Loose) ? 0.1f : 0f))
				{
					base.Formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLoose;
				}
				if (this.TacticalDefendPosition != null)
				{
					float num;
					if (this.TacticalDefendPosition.TacticalPositionType == TacticalPosition.TacticalPositionTypeEnum.ChokePoint)
					{
						num = this.TacticalDefendPosition.Width;
					}
					else
					{
						int countOfUnits = base.Formation.CountOfUnits;
						float num2 = base.Formation.Interval * (float)(countOfUnits - 1) + base.Formation.UnitDiameter * (float)countOfUnits;
						num = MathF.Min(this.TacticalDefendPosition.Width, num2 / 3f);
					}
					base.Formation.FormOrder = FormOrder.FormOrderCustom(num);
					return;
				}
			}
			else
			{
				base.Formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLoose;
			}
		}

		protected override void OnBehaviorActivatedAux()
		{
			this.CalculateCurrentOrder();
			base.Formation.SetMovementOrder(base.CurrentOrder);
			base.Formation.FacingOrder = this.CurrentFacingOrder;
			base.Formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLoose;
			base.Formation.FiringOrder = FiringOrder.FiringOrderFireAtWill;
			base.Formation.FormOrder = FormOrder.FormOrderWide;
		}

		public override void ResetBehavior()
		{
			base.ResetBehavior();
			this.DefensePosition = WorldPosition.Invalid;
			this.TacticalDefendPosition = null;
		}

		protected override float GetAiWeight()
		{
			return 1f;
		}

		public WorldPosition DefensePosition = WorldPosition.Invalid;

		public TacticalPosition TacticalDefendPosition;
	}
}
