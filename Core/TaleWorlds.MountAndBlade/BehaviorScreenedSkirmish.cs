using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000116 RID: 278
	public class BehaviorScreenedSkirmish : BehaviorComponent
	{
		// Token: 0x06000D3D RID: 3389 RVA: 0x00020D00 File Offset: 0x0001EF00
		public BehaviorScreenedSkirmish(Formation formation)
			: base(formation)
		{
			this._behaviorSide = formation.AI.Side;
			this._mainFormation = formation.Team.FormationsIncludingEmpty.FirstOrDefaultQ((Formation f) => f.CountOfUnits > 0 && f.AI.IsMainFormation);
			this.CalculateCurrentOrder();
		}

		// Token: 0x06000D3E RID: 3390 RVA: 0x00020D68 File Offset: 0x0001EF68
		protected override void CalculateCurrentOrder()
		{
			Vec2 vec3;
			if (base.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation != null && this._mainFormation != null)
			{
				Vec2 vec = (base.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.MedianPosition.AsVec2 - base.Formation.QuerySystem.AveragePosition).Normalized();
				Vec2 vec2 = (this._mainFormation.QuerySystem.MedianPosition.AsVec2 - base.Formation.QuerySystem.AveragePosition).Normalized();
				if (vec.DotProduct(vec2) > 0.5f)
				{
					vec3 = this._mainFormation.FacingOrder.GetDirection(this._mainFormation, null);
				}
				else
				{
					vec3 = vec;
				}
			}
			else
			{
				vec3 = base.Formation.Direction;
			}
			WorldPosition worldPosition;
			if (this._mainFormation == null)
			{
				worldPosition = base.Formation.QuerySystem.MedianPosition;
				worldPosition.SetVec2(base.Formation.QuerySystem.AveragePosition);
			}
			else
			{
				worldPosition = this._mainFormation.QuerySystem.MedianPosition;
				worldPosition.SetVec2(worldPosition.AsVec2 - vec3 * ((this._mainFormation.Depth + base.Formation.Depth) * 0.5f));
			}
			if (!base.CurrentOrder.CreateNewOrderWorldPosition(base.Formation, WorldPosition.WorldPositionEnforcedCache.None).IsValid || (base.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation != null && (!base.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.IsRangedCavalryFormation || base.Formation.QuerySystem.AveragePosition.DistanceSquared(base.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.MedianPosition.GetNavMeshVec3().AsVec2) >= base.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.MissileRangeAdjusted * base.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.MissileRangeAdjusted || base.CurrentOrder.CreateNewOrderWorldPosition(base.Formation, WorldPosition.WorldPositionEnforcedCache.NavMeshVec3).GetNavMeshVec3().DistanceSquared(worldPosition.GetNavMeshVec3()) >= base.Formation.Depth * base.Formation.Depth)))
			{
				base.CurrentOrder = MovementOrder.MovementOrderMove(worldPosition);
			}
			if (!this.CurrentFacingOrder.GetDirection(base.Formation, null).IsValid || this.CurrentFacingOrder.OrderEnum == FacingOrder.FacingOrderEnum.LookAtEnemy || base.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation == null || base.Formation.QuerySystem.AveragePosition.DistanceSquared(base.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.MedianPosition.GetNavMeshVec3().AsVec2) >= base.Formation.QuerySystem.MissileRangeAdjusted * base.Formation.QuerySystem.MissileRangeAdjusted || (!base.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.IsRangedCavalryFormation && this.CurrentFacingOrder.GetDirection(base.Formation, null).DotProduct(vec3) <= MBMath.Lerp(0.5f, 1f, 1f - MBMath.ClampFloat(base.Formation.Width, 1f, 20f) * 0.05f, 1E-05f)))
			{
				this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(vec3);
			}
		}

		// Token: 0x06000D3F RID: 3391 RVA: 0x000210F8 File Offset: 0x0001F2F8
		public override void TickOccasionally()
		{
			this.CalculateCurrentOrder();
			bool flag = base.Formation.QuerySystem.ClosestEnemyFormation == null || this._mainFormation.QuerySystem.MedianPosition.AsVec2.DistanceSquared(base.Formation.QuerySystem.ClosestEnemyFormation.MedianPosition.AsVec2) <= base.Formation.QuerySystem.AveragePosition.DistanceSquared(base.Formation.QuerySystem.ClosestEnemyFormation.MedianPosition.AsVec2) || base.Formation.QuerySystem.AveragePosition.DistanceSquared(base.CurrentOrder.GetPosition(base.Formation)) <= (this._mainFormation.Depth + base.Formation.Depth) * (this._mainFormation.Depth + base.Formation.Depth) * 0.25f;
			if (flag != this._isFireAtWill)
			{
				this._isFireAtWill = flag;
				base.Formation.FiringOrder = (this._isFireAtWill ? FiringOrder.FiringOrderFireAtWill : FiringOrder.FiringOrderHoldYourFire);
			}
			if (this._mainFormation != null && MathF.Abs(this._mainFormation.Width - base.Formation.Width) > 10f)
			{
				base.Formation.FormOrder = FormOrder.FormOrderCustom(this._mainFormation.Width);
			}
			base.Formation.SetMovementOrder(base.CurrentOrder);
			base.Formation.FacingOrder = this.CurrentFacingOrder;
		}

		// Token: 0x06000D40 RID: 3392 RVA: 0x00021298 File Offset: 0x0001F498
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

		// Token: 0x06000D41 RID: 3393 RVA: 0x00021310 File Offset: 0x0001F510
		public override TextObject GetBehaviorString()
		{
			TextObject behaviorString = base.GetBehaviorString();
			if (this._mainFormation != null)
			{
				behaviorString.SetTextVariable("AI_SIDE", GameTexts.FindText("str_formation_ai_side_strings", this._mainFormation.AI.Side.ToString()));
				behaviorString.SetTextVariable("CLASS", GameTexts.FindText("str_formation_class_string", this._mainFormation.PrimaryClass.GetName()));
			}
			return behaviorString;
		}

		// Token: 0x06000D42 RID: 3394 RVA: 0x00021388 File Offset: 0x0001F588
		protected override float GetAiWeight()
		{
			MovementOrder currentOrder = base.CurrentOrder;
			if ((currentOrder) == MovementOrder.MovementOrderStop)
			{
				this.CalculateCurrentOrder();
			}
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
			float num2 = this._mainFormation.Direction.Normalized().DotProduct((base.Formation.QuerySystem.ClosestEnemyFormation.MedianPosition.AsVec2 - this._mainFormation.QuerySystem.MedianPosition.AsVec2).Normalized());
			float num3 = MBMath.LinearExtrapolation(0.5f, 1.1f, (num2 + 1f) / 2f);
			float num4 = base.Formation.QuerySystem.AveragePosition.Distance(querySystem.ClosestEnemyFormation.MedianPosition.AsVec2) / querySystem.ClosestEnemyFormation.MovementSpeedMaximum;
			float num5 = MBMath.Lerp(0.5f, 1.2f, (8f - MBMath.ClampFloat(num4, 4f, 8f)) / 4f, 1E-05f);
			return num * base.Formation.QuerySystem.MainFormationReliabilityFactor * num3 * num5;
		}

		// Token: 0x04000334 RID: 820
		private Formation _mainFormation;

		// Token: 0x04000335 RID: 821
		private bool _isFireAtWill = true;
	}
}
