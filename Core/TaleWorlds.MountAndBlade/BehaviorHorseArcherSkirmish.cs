using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class BehaviorHorseArcherSkirmish : BehaviorComponent
	{
		public BehaviorHorseArcherSkirmish(Formation formation)
			: base(formation)
		{
			this.CalculateCurrentOrder();
			base.BehaviorCoherence = 0.5f;
		}

		protected override float GetAiWeight()
		{
			if (!this._isEnemyReachable)
			{
				return 0.09f;
			}
			return 0.9f;
		}

		protected override void OnBehaviorActivatedAux()
		{
			this.CalculateCurrentOrder();
			base.Formation.SetMovementOrder(base.CurrentOrder);
			base.Formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
			base.Formation.FacingOrder = FacingOrder.FacingOrderLookAtEnemy;
			base.Formation.FiringOrder = FiringOrder.FiringOrderFireAtWill;
			base.Formation.FormOrder = FormOrder.FormOrderDeep;
			base.Formation.WeaponUsageOrder = WeaponUsageOrder.WeaponUsageOrderUseAny;
		}

		protected override void CalculateCurrentOrder()
		{
			WorldPosition worldPosition = base.Formation.QuerySystem.MedianPosition;
			this._isEnemyReachable = base.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation != null && (!(base.Formation.Team.TeamAI is TeamAISiegeComponent) || !TeamAISiegeComponent.IsFormationInsideCastle(base.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.Formation, false, 0.4f));
			Vec2 averagePosition = base.Formation.QuerySystem.AveragePosition;
			if (!this._isEnemyReachable)
			{
				worldPosition.SetVec2(averagePosition);
			}
			else
			{
				WorldPosition medianPosition = base.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.MedianPosition;
				int num = 0;
				Vec2 vec = Vec2.Zero;
				foreach (Formation formation in base.Formation.Team.FormationsIncludingSpecialAndEmpty)
				{
					if (formation != base.Formation && formation.CountOfUnits > 0)
					{
						num++;
						vec += formation.QuerySystem.MedianPosition.AsVec2;
					}
				}
				if (num > 0)
				{
					vec /= (float)num;
				}
				else
				{
					vec = averagePosition;
				}
				WorldPosition medianTargetFormationPosition = base.Formation.QuerySystem.Team.MedianTargetFormationPosition;
				Vec2 vec2 = (medianTargetFormationPosition.AsVec2 - vec).Normalized();
				float missileRangeAdjusted = base.Formation.QuerySystem.MissileRangeAdjusted;
				if (this._rushMode)
				{
					float num2 = averagePosition.DistanceSquared(medianPosition.AsVec2);
					if (num2 > base.Formation.QuerySystem.MissileRangeAdjusted * base.Formation.QuerySystem.MissileRangeAdjusted)
					{
						worldPosition = medianTargetFormationPosition;
						worldPosition.SetVec2(worldPosition.AsVec2 - vec2 * (missileRangeAdjusted - (10f + base.Formation.Depth * 0.5f)));
					}
					else if (base.Formation.QuerySystem.ClosestEnemyFormation.IsCavalryFormation || num2 <= 400f || base.Formation.QuerySystem.UnderRangedAttackRatio >= 0.4f)
					{
						worldPosition = base.Formation.QuerySystem.Team.MedianPosition;
						worldPosition.SetVec2(vec - (((num > 0) ? 30f : 80f) + base.Formation.Depth) * vec2);
						this._rushMode = false;
					}
					else
					{
						worldPosition = base.Formation.QuerySystem.Team.MedianPosition;
						Vec2 vec3 = (medianPosition.AsVec2 - averagePosition).Normalized();
						worldPosition.SetVec2(medianPosition.AsVec2 - vec3 * (missileRangeAdjusted - (10f + base.Formation.Depth * 0.5f)));
					}
				}
				else
				{
					if (num > 0)
					{
						worldPosition = base.Formation.QuerySystem.Team.MedianPosition;
						worldPosition.SetVec2(vec - (30f + base.Formation.Depth) * vec2);
					}
					else
					{
						worldPosition = base.Formation.QuerySystem.ClosestEnemyFormation.MedianPosition;
						worldPosition.SetVec2(worldPosition.AsVec2 - 80f * vec2);
					}
					if (worldPosition.AsVec2.DistanceSquared(averagePosition) <= 400f)
					{
						worldPosition = medianTargetFormationPosition;
						worldPosition.SetVec2(worldPosition.AsVec2 - vec2 * (missileRangeAdjusted - (10f + base.Formation.Depth * 0.5f)));
						this._rushMode = true;
					}
				}
			}
			base.CurrentOrder = MovementOrder.MovementOrderMove(worldPosition);
		}

		public override void TickOccasionally()
		{
			this.CalculateCurrentOrder();
			base.Formation.SetMovementOrder(base.CurrentOrder);
		}

		private bool _rushMode;

		private bool _isEnemyReachable = true;
	}
}
