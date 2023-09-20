using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000103 RID: 259
	public class BehaviorDefensiveRing : BehaviorComponent
	{
		// Token: 0x06000CBF RID: 3263 RVA: 0x0001C56E File Offset: 0x0001A76E
		public BehaviorDefensiveRing(Formation formation)
			: base(formation)
		{
			this.CalculateCurrentOrder();
		}

		// Token: 0x06000CC0 RID: 3264 RVA: 0x0001C580 File Offset: 0x0001A780
		protected override void CalculateCurrentOrder()
		{
			Vec2 vec;
			if (this.TacticalDefendPosition != null)
			{
				vec = this.TacticalDefendPosition.Direction;
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
				base.CurrentOrder = MovementOrder.MovementOrderMove(this.TacticalDefendPosition.Position);
				this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(vec);
				return;
			}
			WorldPosition medianPosition = base.Formation.QuerySystem.MedianPosition;
			medianPosition.SetVec2(base.Formation.QuerySystem.AveragePosition);
			base.CurrentOrder = MovementOrder.MovementOrderMove(medianPosition);
			this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(vec);
		}

		// Token: 0x06000CC1 RID: 3265 RVA: 0x0001C6D4 File Offset: 0x0001A8D4
		public override void TickOccasionally()
		{
			this.CalculateCurrentOrder();
			base.Formation.SetMovementOrder(base.CurrentOrder);
			base.Formation.FacingOrder = this.CurrentFacingOrder;
			if (base.Formation.QuerySystem.AveragePosition.Distance(base.CurrentOrder.GetPosition(base.Formation)) - base.Formation.Arrangement.Depth * 0.5f < 10f)
			{
				base.Formation.ArrangementOrder = ArrangementOrder.ArrangementOrderCircle;
				if (base.Formation.Team.FormationsIncludingEmpty.AnyQ((Formation f) => f.CountOfUnits > 0 && f.QuerySystem.IsRangedFormation))
				{
					Formation formation = base.Formation.Team.FormationsIncludingEmpty.WhereQ((Formation f) => f.CountOfUnits > 0 && f.QuerySystem.IsRangedFormation).MaxBy((Formation f) => f.CountOfUnits);
					int num = (int)MathF.Sqrt((float)formation.CountOfUnits);
					float num2 = ((float)num * formation.UnitDiameter + (float)(num - 1) * formation.Interval) * 0.5f * 1.414213f;
					int i = base.Formation.Arrangement.UnitCount;
					int num3 = 0;
					while (i > 0)
					{
						double num4 = (double)(num2 + base.Formation.Distance * (float)num3 + base.Formation.UnitDiameter * (float)(num3 + 1)) * 3.141592653589793 * 2.0 / (double)(base.Formation.UnitDiameter + base.Formation.Interval);
						i -= (int)Math.Ceiling(num4);
						num3++;
					}
					float num5 = num2 + (float)num3 * base.Formation.UnitDiameter + (float)(num3 - 1) * base.Formation.Distance;
					base.Formation.FormOrder = FormOrder.FormOrderCustom(num5 * 2f);
					return;
				}
			}
			else
			{
				base.Formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLoose;
			}
		}

		// Token: 0x06000CC2 RID: 3266 RVA: 0x0001C8FC File Offset: 0x0001AAFC
		protected override void OnBehaviorActivatedAux()
		{
			this.CalculateCurrentOrder();
			base.Formation.SetMovementOrder(base.CurrentOrder);
			base.Formation.FacingOrder = this.CurrentFacingOrder;
			base.Formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLoose;
			base.Formation.FiringOrder = FiringOrder.FiringOrderFireAtWill;
			base.Formation.FormOrder = FormOrder.FormOrderDeep;
			base.Formation.WeaponUsageOrder = WeaponUsageOrder.WeaponUsageOrderUseAny;
		}

		// Token: 0x06000CC3 RID: 3267 RVA: 0x0001C971 File Offset: 0x0001AB71
		public override void ResetBehavior()
		{
			base.ResetBehavior();
			this.TacticalDefendPosition = null;
		}

		// Token: 0x06000CC4 RID: 3268 RVA: 0x0001C980 File Offset: 0x0001AB80
		protected override float GetAiWeight()
		{
			if (this.TacticalDefendPosition == null)
			{
				return 0f;
			}
			return 1f;
		}

		// Token: 0x0400030E RID: 782
		public TacticalPosition TacticalDefendPosition;
	}
}
