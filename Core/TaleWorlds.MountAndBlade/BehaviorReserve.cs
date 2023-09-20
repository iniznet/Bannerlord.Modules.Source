using System;
using System.Collections.Generic;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;

namespace TaleWorlds.MountAndBlade
{
	public class BehaviorReserve : BehaviorComponent
	{
		public BehaviorReserve(Formation formation)
			: base(formation)
		{
			this._behaviorSide = formation.AI.Side;
			this.CalculateCurrentOrder();
		}

		protected override void CalculateCurrentOrder()
		{
			Formation formation = base.Formation.Team.FormationsIncludingEmpty.FirstOrDefaultQ((Formation f) => f.CountOfUnits > 0 && f != base.Formation && f.AI.IsMainFormation);
			WorldPosition worldPosition;
			if (formation != null)
			{
				worldPosition = formation.QuerySystem.MedianPosition;
				Vec2 vec = (base.Formation.QuerySystem.Team.AverageEnemyPosition - formation.QuerySystem.MedianPosition.AsVec2).Normalized();
				worldPosition.SetVec2(worldPosition.AsVec2 - vec * (40f + base.Formation.Depth));
			}
			else
			{
				Vec2 vec2 = Vec2.Zero;
				int num = 0;
				foreach (Formation formation2 in base.Formation.Team.FormationsIncludingSpecialAndEmpty)
				{
					if (formation2.CountOfUnits > 0 && formation2 != base.Formation)
					{
						vec2 += formation2.QuerySystem.MedianPosition.AsVec2;
						num++;
					}
				}
				if (num <= 0)
				{
					base.CurrentOrder = MovementOrder.MovementOrderStop;
					return;
				}
				WorldPosition worldPosition2 = WorldPosition.Invalid;
				float num2 = float.MaxValue;
				vec2 *= 1f / (float)num;
				foreach (Formation formation3 in base.Formation.Team.FormationsIncludingSpecialAndEmpty)
				{
					if (formation3.CountOfUnits > 0 && formation3 != base.Formation)
					{
						float num3 = vec2.DistanceSquared(formation3.QuerySystem.MedianPosition.AsVec2);
						if (num3 < num2)
						{
							num2 = num3;
							worldPosition2 = formation3.QuerySystem.MedianPosition;
						}
					}
				}
				Vec2 vec3 = (base.Formation.QuerySystem.Team.AverageEnemyPosition - worldPosition2.AsVec2).Normalized();
				worldPosition = worldPosition2;
				worldPosition.SetVec2(worldPosition.AsVec2 - vec3 * (20f + base.Formation.Depth));
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
			base.Formation.FormOrder = FormOrder.FormOrderWider;
			base.Formation.WeaponUsageOrder = WeaponUsageOrder.WeaponUsageOrderUseAny;
		}

		protected override float GetAiWeight()
		{
			if (!base.Formation.AI.IsMainFormation)
			{
				foreach (Formation formation in base.Formation.Team.FormationsIncludingSpecialAndEmpty)
				{
					if (base.Formation != formation && formation.CountOfUnits > 0)
					{
						using (List<Team>.Enumerator enumerator2 = Mission.Current.Teams.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								Team team = enumerator2.Current;
								if (team.IsEnemyOf(base.Formation.Team))
								{
									using (List<Formation>.Enumerator enumerator3 = team.FormationsIncludingSpecialAndEmpty.GetEnumerator())
									{
										while (enumerator3.MoveNext())
										{
											if (enumerator3.Current.CountOfUnits > 0)
											{
												return 0.04f;
											}
										}
									}
								}
							}
							break;
						}
					}
				}
			}
			return 0f;
		}
	}
}
