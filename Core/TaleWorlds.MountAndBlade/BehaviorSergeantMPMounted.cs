using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Objects;

namespace TaleWorlds.MountAndBlade
{
	public class BehaviorSergeantMPMounted : BehaviorComponent
	{
		public BehaviorSergeantMPMounted(Formation formation)
			: base(formation)
		{
			this._flagpositions = base.Formation.Team.Mission.ActiveMissionObjects.FindAllWithType<FlagCapturePoint>().ToList<FlagCapturePoint>();
			this._flagDominationGameMode = base.Formation.Team.Mission.GetMissionBehavior<MissionMultiplayerFlagDomination>();
			this.CalculateCurrentOrder();
		}

		private MovementOrder UncapturedFlagMoveOrder()
		{
			if (this._flagpositions.Any((FlagCapturePoint fp) => this._flagDominationGameMode.GetFlagOwnerTeam(fp) != base.Formation.Team))
			{
				FlagCapturePoint flagCapturePoint = this._flagpositions.Where((FlagCapturePoint fp) => this._flagDominationGameMode.GetFlagOwnerTeam(fp) != base.Formation.Team).MinBy((FlagCapturePoint fp) => base.Formation.Team.QuerySystem.GetLocalEnemyPower(fp.Position.AsVec2));
				return MovementOrder.MovementOrderMove(new WorldPosition(base.Formation.Team.Mission.Scene, UIntPtr.Zero, flagCapturePoint.Position, false));
			}
			if (this._flagpositions.Any((FlagCapturePoint fp) => this._flagDominationGameMode.GetFlagOwnerTeam(fp) == base.Formation.Team))
			{
				Vec3 position = this._flagpositions.Where((FlagCapturePoint fp) => this._flagDominationGameMode.GetFlagOwnerTeam(fp) == base.Formation.Team).MinBy((FlagCapturePoint fp) => fp.Position.AsVec2.DistanceSquared(base.Formation.QuerySystem.AveragePosition)).Position;
				return MovementOrder.MovementOrderMove(new WorldPosition(base.Formation.Team.Mission.Scene, UIntPtr.Zero, position, false));
			}
			return MovementOrder.MovementOrderStop;
		}

		protected override void CalculateCurrentOrder()
		{
			if (base.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation == null || base.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.MedianPosition.AsVec2.DistanceSquared(base.Formation.QuerySystem.AveragePosition) > 2500f)
			{
				base.CurrentOrder = this.UncapturedFlagMoveOrder();
				return;
			}
			FlagCapturePoint flagCapturePoint = null;
			if (this._flagpositions.Any((FlagCapturePoint fp) => this._flagDominationGameMode.GetFlagOwnerTeam(fp) != base.Formation.Team && !fp.IsContested))
			{
				flagCapturePoint = this._flagpositions.Where((FlagCapturePoint fp) => this._flagDominationGameMode.GetFlagOwnerTeam(fp) != base.Formation.Team && !fp.IsContested).MinBy((FlagCapturePoint fp) => base.Formation.QuerySystem.AveragePosition.DistanceSquared(fp.Position.AsVec2));
			}
			if ((!base.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.IsRangedFormation || base.Formation.QuerySystem.FormationPower / base.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.FormationPower / base.Formation.Team.QuerySystem.RemainingPowerRatio <= 0.7f) && flagCapturePoint != null)
			{
				base.CurrentOrder = MovementOrder.MovementOrderMove(new WorldPosition(base.Formation.Team.Mission.Scene, UIntPtr.Zero, flagCapturePoint.Position, false));
				return;
			}
			base.CurrentOrder = MovementOrder.MovementOrderChargeToTarget(base.Formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.Formation);
		}

		public override void TickOccasionally()
		{
			this._flagpositions.RemoveAll((FlagCapturePoint fp) => fp.IsDeactivated);
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
			if (base.Formation.QuerySystem.IsCavalryFormation)
			{
				return 1.2f;
			}
			return 0f;
		}

		private List<FlagCapturePoint> _flagpositions;

		private MissionMultiplayerFlagDomination _flagDominationGameMode;
	}
}
