using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade.Objects;

namespace TaleWorlds.MountAndBlade
{
	public class BehaviorSergeantMPLastFlagLastStand : BehaviorComponent
	{
		public BehaviorSergeantMPLastFlagLastStand(Formation formation)
			: base(formation)
		{
			this._flagpositions = Mission.Current.ActiveMissionObjects.FindAllWithType<FlagCapturePoint>().ToList<FlagCapturePoint>();
			this._flagDominationGameMode = Mission.Current.GetMissionBehavior<MissionMultiplayerFlagDomination>();
			this.CalculateCurrentOrder();
		}

		protected override void CalculateCurrentOrder()
		{
			base.CurrentOrder = ((this._flagpositions.Count > 0) ? MovementOrder.MovementOrderMove(new WorldPosition(Mission.Current.Scene, UIntPtr.Zero, this._flagpositions[0].Position, false)) : MovementOrder.MovementOrderStop);
		}

		public override void TickOccasionally()
		{
			this._flagpositions.RemoveAll((FlagCapturePoint fp) => fp.IsDeactivated);
			this.CalculateCurrentOrder();
			base.Formation.SetMovementOrder(base.CurrentOrder);
			base.Formation.FiringOrder = FiringOrder.FiringOrderHoldYourFire;
		}

		protected override void OnBehaviorActivatedAux()
		{
			this._flagpositions.RemoveAll((FlagCapturePoint fp) => fp.IsDeactivated);
			this.CalculateCurrentOrder();
			base.Formation.SetMovementOrder(base.CurrentOrder);
			base.Formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
			base.Formation.FacingOrder = FacingOrder.FacingOrderLookAtEnemy;
			base.Formation.FiringOrder = FiringOrder.FiringOrderHoldYourFire;
			base.Formation.FormOrder = FormOrder.FormOrderDeep;
			base.Formation.WeaponUsageOrder = WeaponUsageOrder.WeaponUsageOrderUseAny;
		}

		protected override float GetAiWeight()
		{
			if (this._lastEffort)
			{
				return 10f;
			}
			this._flagpositions.RemoveAll((FlagCapturePoint fp) => fp.IsDeactivated);
			FlagCapturePoint flagCapturePoint = this._flagpositions.FirstOrDefault<FlagCapturePoint>();
			if (this._flagpositions.Count != 1 || this._flagDominationGameMode.GetFlagOwnerTeam(flagCapturePoint) == null || !this._flagDominationGameMode.GetFlagOwnerTeam(flagCapturePoint).IsEnemyOf(base.Formation.Team))
			{
				return 0f;
			}
			float timeUntilBattleSideVictory = this._flagDominationGameMode.GetTimeUntilBattleSideVictory(this._flagDominationGameMode.GetFlagOwnerTeam(flagCapturePoint).Side);
			if (timeUntilBattleSideVictory <= 60f)
			{
				return 10f;
			}
			float num = base.Formation.QuerySystem.AveragePosition.Distance(flagCapturePoint.Position.AsVec2);
			float movementSpeedMaximum = base.Formation.QuerySystem.MovementSpeedMaximum;
			if (num / movementSpeedMaximum * 8f > timeUntilBattleSideVictory)
			{
				this._lastEffort = true;
				return 10f;
			}
			return 0f;
		}

		private List<FlagCapturePoint> _flagpositions;

		private bool _lastEffort;

		private MissionMultiplayerFlagDomination _flagDominationGameMode;
	}
}
