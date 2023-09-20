using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Objects;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000117 RID: 279
	public class BehaviorSergeantMPInfantry : BehaviorComponent
	{
		// Token: 0x06000D43 RID: 3395 RVA: 0x000215AC File Offset: 0x0001F7AC
		public BehaviorSergeantMPInfantry(Formation formation)
			: base(formation)
		{
			this._behaviorState = BehaviorSergeantMPInfantry.BehaviorState.Unset;
			this._flagpositions = base.Formation.Team.Mission.ActiveMissionObjects.FindAllWithType<FlagCapturePoint>().ToList<FlagCapturePoint>();
			this._flagDominationGameMode = base.Formation.Team.Mission.GetMissionBehavior<MissionMultiplayerFlagDomination>();
			this.CalculateCurrentOrder();
		}

		// Token: 0x06000D44 RID: 3396 RVA: 0x00021610 File Offset: 0x0001F810
		protected override void CalculateCurrentOrder()
		{
			BehaviorSergeantMPInfantry.BehaviorState behaviorState;
			if (base.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation != null && ((base.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.IsRangedFormation && base.Formation.QuerySystem.AveragePosition.DistanceSquared(base.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.MedianPosition.AsVec2) <= ((this._behaviorState == BehaviorSergeantMPInfantry.BehaviorState.Attacking) ? 3600f : 2500f)) || (base.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.IsInfantryFormation && base.Formation.QuerySystem.AveragePosition.DistanceSquared(base.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.MedianPosition.AsVec2) <= ((this._behaviorState == BehaviorSergeantMPInfantry.BehaviorState.Attacking) ? 900f : 400f))))
			{
				behaviorState = BehaviorSergeantMPInfantry.BehaviorState.Attacking;
			}
			else
			{
				behaviorState = BehaviorSergeantMPInfantry.BehaviorState.GoingToFlag;
			}
			if (behaviorState == BehaviorSergeantMPInfantry.BehaviorState.Attacking && (this._behaviorState != BehaviorSergeantMPInfantry.BehaviorState.Attacking || base.CurrentOrder.OrderEnum != MovementOrder.MovementOrderEnum.ChargeToTarget || base.CurrentOrder.TargetFormation.QuerySystem != base.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation))
			{
				this._behaviorState = BehaviorSergeantMPInfantry.BehaviorState.Attacking;
				base.CurrentOrder = MovementOrder.MovementOrderChargeToTarget(base.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.Formation);
			}
			if (behaviorState == BehaviorSergeantMPInfantry.BehaviorState.GoingToFlag)
			{
				this._behaviorState = behaviorState;
				WorldPosition medianPosition;
				if (this._flagpositions.Any((FlagCapturePoint fp) => this._flagDominationGameMode.GetFlagOwnerTeam(fp) != base.Formation.Team))
				{
					medianPosition = new WorldPosition(base.Formation.Team.Mission.Scene, UIntPtr.Zero, this._flagpositions.Where((FlagCapturePoint fp) => this._flagDominationGameMode.GetFlagOwnerTeam(fp) != base.Formation.Team).MinBy((FlagCapturePoint fp) => fp.Position.AsVec2.DistanceSquared(base.Formation.QuerySystem.AveragePosition)).Position, false);
				}
				else if (this._flagpositions.Any((FlagCapturePoint fp) => this._flagDominationGameMode.GetFlagOwnerTeam(fp) == base.Formation.Team))
				{
					medianPosition = new WorldPosition(base.Formation.Team.Mission.Scene, UIntPtr.Zero, this._flagpositions.Where((FlagCapturePoint fp) => this._flagDominationGameMode.GetFlagOwnerTeam(fp) == base.Formation.Team).MinBy((FlagCapturePoint fp) => fp.Position.AsVec2.DistanceSquared(base.Formation.QuerySystem.AveragePosition)).Position, false);
				}
				else
				{
					medianPosition = base.Formation.QuerySystem.MedianPosition;
					medianPosition.SetVec2(base.Formation.QuerySystem.AveragePosition);
				}
				if (base.CurrentOrder.OrderEnum == MovementOrder.MovementOrderEnum.Invalid || base.CurrentOrder.GetPosition(base.Formation) != medianPosition.AsVec2)
				{
					Vec2 vec;
					if (base.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation == null)
					{
						vec = base.Formation.Direction;
					}
					else
					{
						vec = (base.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.MedianPosition.AsVec2 - base.Formation.QuerySystem.AveragePosition).Normalized();
					}
					base.CurrentOrder = MovementOrder.MovementOrderMove(medianPosition);
					this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(vec);
				}
			}
		}

		// Token: 0x06000D45 RID: 3397 RVA: 0x00021914 File Offset: 0x0001FB14
		public override void TickOccasionally()
		{
			this._flagpositions.RemoveAll((FlagCapturePoint fp) => fp.IsDeactivated);
			this.CalculateCurrentOrder();
			base.Formation.SetMovementOrder(base.CurrentOrder);
			base.Formation.FacingOrder = this.CurrentFacingOrder;
			if (base.Formation.QuerySystem.HasShield && (this._behaviorState == BehaviorSergeantMPInfantry.BehaviorState.Attacking || (this._behaviorState == BehaviorSergeantMPInfantry.BehaviorState.GoingToFlag && base.CurrentOrder.GetPosition(base.Formation).IsValid && base.Formation.QuerySystem.AveragePosition.DistanceSquared(base.CurrentOrder.GetPosition(base.Formation)) <= 225f)))
			{
				base.Formation.ArrangementOrder = ArrangementOrder.ArrangementOrderShieldWall;
				return;
			}
			base.Formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
		}

		// Token: 0x06000D46 RID: 3398 RVA: 0x00021A0C File Offset: 0x0001FC0C
		protected override void OnBehaviorActivatedAux()
		{
			this.CalculateCurrentOrder();
			base.Formation.SetMovementOrder(base.CurrentOrder);
			base.Formation.FacingOrder = this.CurrentFacingOrder;
			base.Formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
			base.Formation.FiringOrder = FiringOrder.FiringOrderFireAtWill;
			base.Formation.FormOrder = FormOrder.FormOrderDeep;
			base.Formation.WeaponUsageOrder = WeaponUsageOrder.WeaponUsageOrderUseAny;
		}

		// Token: 0x06000D47 RID: 3399 RVA: 0x00021A81 File Offset: 0x0001FC81
		protected override float GetAiWeight()
		{
			if (base.Formation.QuerySystem.IsInfantryFormation)
			{
				return 1.2f;
			}
			return 0f;
		}

		// Token: 0x04000336 RID: 822
		private BehaviorSergeantMPInfantry.BehaviorState _behaviorState;

		// Token: 0x04000337 RID: 823
		private List<FlagCapturePoint> _flagpositions;

		// Token: 0x04000338 RID: 824
		private MissionMultiplayerFlagDomination _flagDominationGameMode;

		// Token: 0x02000451 RID: 1105
		private enum BehaviorState
		{
			// Token: 0x04001883 RID: 6275
			GoingToFlag,
			// Token: 0x04001884 RID: 6276
			Attacking,
			// Token: 0x04001885 RID: 6277
			Unset
		}
	}
}
