using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200014B RID: 331
	public sealed class BatteringRamAI : UsableMachineAIBase
	{
		// Token: 0x060010D6 RID: 4310 RVA: 0x00037183 File Offset: 0x00035383
		public BatteringRamAI(BatteringRam batteringRam)
			: base(batteringRam)
		{
		}

		// Token: 0x17000397 RID: 919
		// (get) Token: 0x060010D7 RID: 4311 RVA: 0x0003718C File Offset: 0x0003538C
		private BatteringRam BatteringRam
		{
			get
			{
				return this.UsableMachine as BatteringRam;
			}
		}

		// Token: 0x17000398 RID: 920
		// (get) Token: 0x060010D8 RID: 4312 RVA: 0x00037199 File Offset: 0x00035399
		public override bool HasActionCompleted
		{
			get
			{
				return this.BatteringRam.IsDeactivated;
			}
		}

		// Token: 0x17000399 RID: 921
		// (get) Token: 0x060010D9 RID: 4313 RVA: 0x000371A8 File Offset: 0x000353A8
		protected override MovementOrder NextOrder
		{
			get
			{
				TeamAISiegeComponent teamAISiegeComponent;
				if ((teamAISiegeComponent = Mission.Current.Teams[0].TeamAI as TeamAISiegeComponent) != null && teamAISiegeComponent.InnerGate != null && !teamAISiegeComponent.InnerGate.IsDestroyed)
				{
					return MovementOrder.MovementOrderAttackEntity(teamAISiegeComponent.InnerGate.GameEntity, false);
				}
				return MovementOrder.MovementOrderCharge;
			}
		}
	}
}
