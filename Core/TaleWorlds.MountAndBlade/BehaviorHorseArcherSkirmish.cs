using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200010A RID: 266
	public class BehaviorHorseArcherSkirmish : BehaviorComponent
	{
		// Token: 0x06000CF1 RID: 3313 RVA: 0x0001E238 File Offset: 0x0001C438
		public BehaviorHorseArcherSkirmish(Formation formation)
			: base(formation)
		{
			this.CalculateCurrentOrder();
			base.BehaviorCoherence = 0.5f;
		}

		// Token: 0x06000CF2 RID: 3314 RVA: 0x0001E259 File Offset: 0x0001C459
		protected override float GetAiWeight()
		{
			if (!this._isEnemyReachable)
			{
				return 0.09f;
			}
			return 0.9f;
		}

		// Token: 0x06000CF3 RID: 3315 RVA: 0x0001E270 File Offset: 0x0001C470
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

		// Token: 0x06000CF4 RID: 3316 RVA: 0x0001E2E4 File Offset: 0x0001C4E4
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

		// Token: 0x06000CF5 RID: 3317 RVA: 0x0001E6BC File Offset: 0x0001C8BC
		public override void TickOccasionally()
		{
			this.CalculateCurrentOrder();
			base.Formation.SetMovementOrder(base.CurrentOrder);
		}

		// Token: 0x04000320 RID: 800
		private bool _rushMode;

		// Token: 0x04000321 RID: 801
		private bool _isEnemyReachable = true;
	}
}
