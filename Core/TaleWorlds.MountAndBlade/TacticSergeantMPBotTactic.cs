using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000164 RID: 356
	public class TacticSergeantMPBotTactic : TacticComponent
	{
		// Token: 0x06001211 RID: 4625 RVA: 0x000461F0 File Offset: 0x000443F0
		public TacticSergeantMPBotTactic(Team team)
			: base(team)
		{
		}

		// Token: 0x06001212 RID: 4626 RVA: 0x000461FC File Offset: 0x000443FC
		protected internal override void TickOccasionally()
		{
			foreach (Formation formation in base.FormationsIncludingEmpty)
			{
				if (formation.CountOfUnits > 0)
				{
					formation.AI.ResetBehaviorWeights();
					formation.AI.SetBehaviorWeight<BehaviorCharge>(1f);
					formation.AI.SetBehaviorWeight<BehaviorTacticalCharge>(1f);
					formation.AI.SetBehaviorWeight<BehaviorSergeantMPInfantry>(1f);
					formation.AI.SetBehaviorWeight<BehaviorSergeantMPRanged>(1f);
					formation.AI.SetBehaviorWeight<BehaviorSergeantMPMounted>(1f);
					formation.AI.SetBehaviorWeight<BehaviorSergeantMPMountedRanged>(1f);
					formation.AI.SetBehaviorWeight<BehaviorSergeantMPLastFlagLastStand>(1f);
				}
			}
		}
	}
}
