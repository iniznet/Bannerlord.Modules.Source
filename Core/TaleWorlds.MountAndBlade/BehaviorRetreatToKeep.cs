using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000114 RID: 276
	public class BehaviorRetreatToKeep : BehaviorComponent
	{
		// Token: 0x06000D30 RID: 3376 RVA: 0x00020944 File Offset: 0x0001EB44
		public BehaviorRetreatToKeep(Formation formation)
			: base(formation)
		{
			base.CurrentOrder = MovementOrder.MovementOrderRetreat;
			base.BehaviorCoherence = 0f;
		}

		// Token: 0x06000D31 RID: 3377 RVA: 0x00020963 File Offset: 0x0001EB63
		public override void TickOccasionally()
		{
			base.TickOccasionally();
			if (base.Formation.AI.ActiveBehavior == this)
			{
				base.Formation.SetMovementOrder(base.CurrentOrder);
			}
		}

		// Token: 0x1700031A RID: 794
		// (get) Token: 0x06000D32 RID: 3378 RVA: 0x0002098F File Offset: 0x0001EB8F
		public override float NavmeshlessTargetPositionPenalty
		{
			get
			{
				return 1f;
			}
		}

		// Token: 0x06000D33 RID: 3379 RVA: 0x00020996 File Offset: 0x0001EB96
		protected override float GetAiWeight()
		{
			return 1f;
		}
	}
}
