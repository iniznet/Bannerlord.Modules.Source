using System;

namespace TaleWorlds.MountAndBlade
{
	public class TacticSergeantMPBotTactic : TacticComponent
	{
		public TacticSergeantMPBotTactic(Team team)
			: base(team)
		{
		}

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
