using System;
using TaleWorlds.Engine;
using TaleWorlds.LinQuick;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000108 RID: 264
	public class BehaviorGeneral : BehaviorComponent
	{
		// Token: 0x06000CE7 RID: 3303 RVA: 0x0001DB10 File Offset: 0x0001BD10
		public BehaviorGeneral(Formation formation)
			: base(formation)
		{
			this._mainFormation = formation.Team.FormationsIncludingEmpty.FirstOrDefaultQ((Formation f) => f.CountOfUnits > 0 && f.AI.IsMainFormation);
			this.CalculateCurrentOrder();
		}

		// Token: 0x06000CE8 RID: 3304 RVA: 0x0001DB60 File Offset: 0x0001BD60
		protected override void CalculateCurrentOrder()
		{
			bool flag = false;
			bool flag2 = false;
			foreach (Formation formation in base.Formation.Team.FormationsIncludingEmpty)
			{
				if (formation.CountOfUnits > 0)
				{
					flag = true;
					if (formation.GetReadonlyMovementOrderReference().OrderEnum != MovementOrder.MovementOrderEnum.Retreat)
					{
						flag2 = false;
						break;
					}
					flag2 = true;
				}
			}
			if (!flag)
			{
				base.CurrentOrder = MovementOrder.MovementOrderCharge;
				return;
			}
			if (flag2)
			{
				base.CurrentOrder = MovementOrder.MovementOrderRetreat;
				return;
			}
			bool flag3 = false;
			foreach (Team team in Mission.Current.Teams)
			{
				if (team.IsEnemyOf(base.Formation.Team) && team.HasAnyFormationsIncludingSpecialThatIsNotEmpty())
				{
					flag3 = true;
					break;
				}
			}
			WorldPosition worldPosition;
			if (flag3 && base.Formation.Team.HasAnyFormationsIncludingSpecialThatIsNotEmpty())
			{
				float num = ((base.Formation.IsMounted() && base.Formation.Team.QuerySystem.CavalryRatio + base.Formation.Team.QuerySystem.RangedCavalryRatio >= 33.3f) ? 40f : 3f);
				if (this._mainFormation != null && this._mainFormation.CountOfUnits > 0)
				{
					float num2 = this._mainFormation.Depth + num;
					worldPosition = this._mainFormation.QuerySystem.MedianPosition;
					worldPosition.SetVec2(worldPosition.AsVec2 - (base.Formation.QuerySystem.Team.MedianTargetFormationPosition.AsVec2 - this._mainFormation.QuerySystem.MedianPosition.AsVec2).Normalized() * num2);
				}
				else
				{
					worldPosition = base.Formation.QuerySystem.Team.MedianPosition;
					worldPosition.SetVec2(base.Formation.QuerySystem.Team.AveragePosition - (base.Formation.QuerySystem.Team.MedianTargetFormationPosition.AsVec2 - base.Formation.QuerySystem.Team.AveragePosition).Normalized() * num);
				}
			}
			else
			{
				worldPosition = base.Formation.QuerySystem.MedianPosition;
				worldPosition.SetVec2(base.Formation.QuerySystem.AveragePosition);
			}
			base.CurrentOrder = MovementOrder.MovementOrderMove(worldPosition);
		}

		// Token: 0x06000CE9 RID: 3305 RVA: 0x0001DE24 File Offset: 0x0001C024
		public override void TickOccasionally()
		{
			this.CalculateCurrentOrder();
			base.Formation.SetMovementOrder(base.CurrentOrder);
		}

		// Token: 0x06000CEA RID: 3306 RVA: 0x0001DE40 File Offset: 0x0001C040
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

		// Token: 0x06000CEB RID: 3307 RVA: 0x0001DEB4 File Offset: 0x0001C0B4
		protected override float GetAiWeight()
		{
			if (this._mainFormation == null || !this._mainFormation.AI.IsMainFormation)
			{
				this._mainFormation = base.Formation.Team.FormationsIncludingEmpty.FirstOrDefaultQ((Formation f) => f.CountOfUnits > 0 && f.AI.IsMainFormation);
			}
			return 1.2f;
		}

		// Token: 0x0400031C RID: 796
		private Formation _mainFormation;
	}
}
