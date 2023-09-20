using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200014E RID: 334
	public sealed class SiegeLadderAI : UsableMachineAIBase
	{
		// Token: 0x060010E1 RID: 4321 RVA: 0x00037565 File Offset: 0x00035765
		public SiegeLadderAI(SiegeLadder ladder)
			: base(ladder)
		{
		}

		// Token: 0x1700039A RID: 922
		// (get) Token: 0x060010E2 RID: 4322 RVA: 0x0003756E File Offset: 0x0003576E
		public SiegeLadder Ladder
		{
			get
			{
				return this.UsableMachine as SiegeLadder;
			}
		}

		// Token: 0x1700039B RID: 923
		// (get) Token: 0x060010E3 RID: 4323 RVA: 0x0003757B File Offset: 0x0003577B
		public override bool HasActionCompleted
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700039C RID: 924
		// (get) Token: 0x060010E4 RID: 4324 RVA: 0x0003757E File Offset: 0x0003577E
		protected override MovementOrder NextOrder
		{
			get
			{
				return MovementOrder.MovementOrderCharge;
			}
		}
	}
}
