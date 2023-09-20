using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000165 RID: 357
	public class TacticStop : TacticComponent
	{
		// Token: 0x06001213 RID: 4627 RVA: 0x000462DC File Offset: 0x000444DC
		public TacticStop(Team team)
			: base(team)
		{
		}

		// Token: 0x06001214 RID: 4628 RVA: 0x000462E8 File Offset: 0x000444E8
		protected internal override void TickOccasionally()
		{
			foreach (Formation formation in base.FormationsIncludingEmpty)
			{
				if (formation.CountOfUnits > 0)
				{
					formation.AI.ResetBehaviorWeights();
					formation.AI.SetBehaviorWeight<BehaviorStop>(1f);
				}
			}
			base.TickOccasionally();
		}
	}
}
