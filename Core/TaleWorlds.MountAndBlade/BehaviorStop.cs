using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000122 RID: 290
	public class BehaviorStop : BehaviorComponent
	{
		// Token: 0x06000DA6 RID: 3494 RVA: 0x00024DCD File Offset: 0x00022FCD
		public BehaviorStop(Formation formation)
			: base(formation)
		{
			base.CurrentOrder = MovementOrder.MovementOrderStop;
			base.BehaviorCoherence = 0f;
		}

		// Token: 0x06000DA7 RID: 3495 RVA: 0x00024DEC File Offset: 0x00022FEC
		public override void TickOccasionally()
		{
			base.Formation.SetMovementOrder(base.CurrentOrder);
		}

		// Token: 0x06000DA8 RID: 3496 RVA: 0x00024E00 File Offset: 0x00023000
		protected override void OnBehaviorActivatedAux()
		{
			base.Formation.ArrangementOrder = (base.Formation.QuerySystem.HasShield ? ArrangementOrder.ArrangementOrderShieldWall : ArrangementOrder.ArrangementOrderLine);
			base.Formation.FiringOrder = FiringOrder.FiringOrderFireAtWill;
			base.Formation.WeaponUsageOrder = WeaponUsageOrder.WeaponUsageOrderUseAny;
			this._lastPlayerInformTime = Mission.Current.CurrentTime;
		}

		// Token: 0x17000322 RID: 802
		// (get) Token: 0x06000DA9 RID: 3497 RVA: 0x00024E66 File Offset: 0x00023066
		public override float NavmeshlessTargetPositionPenalty
		{
			get
			{
				return 1f;
			}
		}

		// Token: 0x06000DAA RID: 3498 RVA: 0x00024E6D File Offset: 0x0002306D
		protected override float GetAiWeight()
		{
			return 0.01f;
		}
	}
}
