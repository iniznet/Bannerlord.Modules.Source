using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;
using TaleWorlds.MountAndBlade.Objects;

namespace TaleWorlds.MountAndBlade
{
	public class BehaviorSergeantMPRanged : BehaviorComponent
	{
		public BehaviorSergeantMPRanged(Formation formation)
			: base(formation)
		{
			this._flagpositions = base.Formation.Team.Mission.ActiveMissionObjects.FindAllWithType<FlagCapturePoint>().ToList<FlagCapturePoint>();
			this._flagDominationGameMode = base.Formation.Team.Mission.GetMissionBehavior<MissionMultiplayerFlagDomination>();
			this.CalculateCurrentOrder();
		}

		protected override void CalculateCurrentOrder()
		{
			bool flag = false;
			Formation formation = null;
			float num = float.MaxValue;
			foreach (Team team in base.Formation.Team.Mission.Teams)
			{
				if (team.IsEnemyOf(base.Formation.Team))
				{
					for (int i = 0; i < Math.Min(team.FormationsIncludingSpecialAndEmpty.Count, 8); i++)
					{
						Formation formation2 = team.FormationsIncludingSpecialAndEmpty[i];
						if (formation2.CountOfUnits > 0)
						{
							flag = true;
							if (formation2.QuerySystem.IsCavalryFormation || formation2.QuerySystem.IsRangedCavalryFormation)
							{
								float num2 = formation2.QuerySystem.MedianPosition.AsVec2.DistanceSquared(base.Formation.QuerySystem.AveragePosition);
								if (num2 < num)
								{
									num = num2;
									formation = formation2;
								}
							}
						}
					}
				}
			}
			if (base.Formation.Team.FormationsIncludingEmpty.AnyQ((Formation f) => f.CountOfUnits > 0 && f != base.Formation && f.QuerySystem.IsInfantryFormation))
			{
				this._attachedInfantry = base.Formation.Team.FormationsIncludingEmpty.Where((Formation f) => f.CountOfUnits > 0 && f != base.Formation && f.QuerySystem.IsInfantryFormation).MinBy((Formation f) => f.QuerySystem.MedianPosition.AsVec2.DistanceSquared(base.Formation.QuerySystem.AveragePosition));
				Formation formation3 = null;
				if (flag)
				{
					if (base.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.MedianPosition.AsVec2.DistanceSquared(base.Formation.QuerySystem.AveragePosition) <= 4900f)
					{
						formation3 = base.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.Formation;
					}
					else if (formation != null)
					{
						formation3 = formation;
					}
				}
				Vec2 vec = ((formation3 == null) ? this._attachedInfantry.Direction : (formation3.QuerySystem.MedianPosition.AsVec2 - this._attachedInfantry.QuerySystem.MedianPosition.AsVec2).Normalized());
				WorldPosition medianPosition = this._attachedInfantry.QuerySystem.MedianPosition;
				medianPosition.SetVec2(medianPosition.AsVec2 - vec * ((this._attachedInfantry.Depth + base.Formation.Depth) / 2f));
				base.CurrentOrder = MovementOrder.MovementOrderMove(medianPosition);
				this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(vec);
				return;
			}
			if (base.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation != null && base.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.MedianPosition.AsVec2.DistanceSquared(base.Formation.QuerySystem.AveragePosition) <= 4900f)
			{
				Vec2 vec2 = (base.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.MedianPosition.AsVec2 - base.Formation.QuerySystem.AveragePosition).Normalized();
				float num3 = base.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.MedianPosition.AsVec2.Distance(base.Formation.QuerySystem.AveragePosition);
				WorldPosition medianPosition2 = base.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.MedianPosition;
				if (num3 > base.Formation.QuerySystem.MissileRangeAdjusted)
				{
					medianPosition2.SetVec2(medianPosition2.AsVec2 - vec2 * (base.Formation.QuerySystem.MissileRangeAdjusted - base.Formation.Depth * 0.5f));
				}
				else if (num3 < base.Formation.QuerySystem.MissileRangeAdjusted * 0.4f)
				{
					medianPosition2.SetVec2(medianPosition2.AsVec2 - vec2 * (base.Formation.QuerySystem.MissileRangeAdjusted * 0.4f));
				}
				else
				{
					medianPosition2.SetVec2(base.Formation.QuerySystem.AveragePosition);
				}
				base.CurrentOrder = MovementOrder.MovementOrderMove(medianPosition2);
				this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(vec2);
				return;
			}
			if (this._flagpositions.Any((FlagCapturePoint fp) => this._flagDominationGameMode.GetFlagOwnerTeam(fp) != base.Formation.Team))
			{
				Vec3 position = this._flagpositions.Where((FlagCapturePoint fp) => this._flagDominationGameMode.GetFlagOwnerTeam(fp) != base.Formation.Team).MinBy((FlagCapturePoint fp) => fp.Position.AsVec2.DistanceSquared(base.Formation.QuerySystem.AveragePosition)).Position;
				if (base.CurrentOrder.OrderEnum == MovementOrder.MovementOrderEnum.Invalid || base.CurrentOrder.GetPosition(base.Formation) != position.AsVec2)
				{
					Vec2 vec3;
					if (base.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation == null)
					{
						vec3 = base.Formation.Direction;
					}
					else
					{
						vec3 = (base.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.MedianPosition.AsVec2 - base.Formation.QuerySystem.AveragePosition).Normalized();
					}
					WorldPosition worldPosition = new WorldPosition(base.Formation.Team.Mission.Scene, UIntPtr.Zero, position, false);
					base.CurrentOrder = MovementOrder.MovementOrderMove(worldPosition);
					this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(vec3);
					return;
				}
			}
			else
			{
				if (this._flagpositions.Any((FlagCapturePoint fp) => this._flagDominationGameMode.GetFlagOwnerTeam(fp) == base.Formation.Team))
				{
					Vec3 position2 = this._flagpositions.Where((FlagCapturePoint fp) => this._flagDominationGameMode.GetFlagOwnerTeam(fp) == base.Formation.Team).MinBy((FlagCapturePoint fp) => fp.Position.AsVec2.DistanceSquared(base.Formation.QuerySystem.AveragePosition)).Position;
					base.CurrentOrder = MovementOrder.MovementOrderMove(new WorldPosition(base.Formation.Team.Mission.Scene, UIntPtr.Zero, position2, false));
					this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtEnemy;
					return;
				}
				WorldPosition medianPosition3 = base.Formation.QuerySystem.MedianPosition;
				medianPosition3.SetVec2(base.Formation.QuerySystem.AveragePosition);
				base.CurrentOrder = MovementOrder.MovementOrderMove(medianPosition3);
				this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtEnemy;
			}
		}

		public override void TickOccasionally()
		{
			this._flagpositions.RemoveAll((FlagCapturePoint fp) => fp.IsDeactivated);
			this.CalculateCurrentOrder();
			base.Formation.SetMovementOrder(base.CurrentOrder);
			base.Formation.FacingOrder = this.CurrentFacingOrder;
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

		protected override float GetAiWeight()
		{
			if (base.Formation.QuerySystem.IsRangedFormation)
			{
				return 1.2f;
			}
			return 0f;
		}

		private List<FlagCapturePoint> _flagpositions;

		private Formation _attachedInfantry;

		private MissionMultiplayerFlagDomination _flagDominationGameMode;
	}
}
