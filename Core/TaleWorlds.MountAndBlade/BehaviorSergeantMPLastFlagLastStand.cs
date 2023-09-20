using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade.Objects;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000118 RID: 280
	public class BehaviorSergeantMPLastFlagLastStand : BehaviorComponent
	{
		// Token: 0x06000D4E RID: 3406 RVA: 0x00021B7B File Offset: 0x0001FD7B
		public BehaviorSergeantMPLastFlagLastStand(Formation formation)
			: base(formation)
		{
			this._flagpositions = Mission.Current.ActiveMissionObjects.FindAllWithType<FlagCapturePoint>().ToList<FlagCapturePoint>();
			this._flagDominationGameMode = Mission.Current.GetMissionBehavior<MissionMultiplayerFlagDomination>();
			this.CalculateCurrentOrder();
		}

		// Token: 0x06000D4F RID: 3407 RVA: 0x00021BB4 File Offset: 0x0001FDB4
		protected override void CalculateCurrentOrder()
		{
			base.CurrentOrder = ((this._flagpositions.Count > 0) ? MovementOrder.MovementOrderMove(new WorldPosition(Mission.Current.Scene, UIntPtr.Zero, this._flagpositions[0].Position, false)) : MovementOrder.MovementOrderStop);
		}

		// Token: 0x06000D50 RID: 3408 RVA: 0x00021C08 File Offset: 0x0001FE08
		public override void TickOccasionally()
		{
			this._flagpositions.RemoveAll((FlagCapturePoint fp) => fp.IsDeactivated);
			this.CalculateCurrentOrder();
			base.Formation.SetMovementOrder(base.CurrentOrder);
			base.Formation.FiringOrder = FiringOrder.FiringOrderHoldYourFire;
		}

		// Token: 0x06000D51 RID: 3409 RVA: 0x00021C68 File Offset: 0x0001FE68
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

		// Token: 0x06000D52 RID: 3410 RVA: 0x00021D08 File Offset: 0x0001FF08
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

		// Token: 0x04000339 RID: 825
		private List<FlagCapturePoint> _flagpositions;

		// Token: 0x0400033A RID: 826
		private bool _lastEffort;

		// Token: 0x0400033B RID: 827
		private MissionMultiplayerFlagDomination _flagDominationGameMode;
	}
}
