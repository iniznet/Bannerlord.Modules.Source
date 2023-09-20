using System;
using TaleWorlds.Engine;
using TaleWorlds.LinQuick;

namespace TaleWorlds.MountAndBlade
{
	public class BehaviorGeneral : BehaviorComponent
	{
		public BehaviorGeneral(Formation formation)
			: base(formation)
		{
			this._mainFormation = formation.Team.FormationsIncludingEmpty.FirstOrDefaultQ((Formation f) => f.CountOfUnits > 0 && f.AI.IsMainFormation);
			this.CalculateCurrentOrder();
		}

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

		public override void TickOccasionally()
		{
			this.CalculateCurrentOrder();
			base.Formation.SetMovementOrder(base.CurrentOrder);
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

		protected override float GetAiWeight()
		{
			if (this._mainFormation == null || !this._mainFormation.AI.IsMainFormation)
			{
				this._mainFormation = base.Formation.Team.FormationsIncludingEmpty.FirstOrDefaultQ((Formation f) => f.CountOfUnits > 0 && f.AI.IsMainFormation);
			}
			return 1.2f;
		}

		private Formation _mainFormation;
	}
}
