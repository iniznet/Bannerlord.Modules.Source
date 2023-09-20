using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200014F RID: 335
	public sealed class SiegeTowerAI : UsableMachineAIBase
	{
		// Token: 0x1700039D RID: 925
		// (get) Token: 0x060010E5 RID: 4325 RVA: 0x00037585 File Offset: 0x00035785
		private SiegeTower SiegeTower
		{
			get
			{
				return this.UsableMachine as SiegeTower;
			}
		}

		// Token: 0x060010E6 RID: 4326 RVA: 0x00037592 File Offset: 0x00035792
		public SiegeTowerAI(SiegeTower siegeTower)
			: base(siegeTower)
		{
		}

		// Token: 0x1700039E RID: 926
		// (get) Token: 0x060010E7 RID: 4327 RVA: 0x0003759B File Offset: 0x0003579B
		public override bool HasActionCompleted
		{
			get
			{
				return this.SiegeTower.MovementComponent.HasArrivedAtTarget && this.SiegeTower.State == SiegeTower.GateState.Open;
			}
		}

		// Token: 0x1700039F RID: 927
		// (get) Token: 0x060010E8 RID: 4328 RVA: 0x000375BF File Offset: 0x000357BF
		protected override MovementOrder NextOrder
		{
			get
			{
				return MovementOrder.MovementOrderCharge;
			}
		}
	}
}
