using System;

namespace TaleWorlds.MountAndBlade
{
	public class TacticStop : TacticComponent
	{
		public TacticStop(Team team)
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
					formation.AI.SetBehaviorWeight<BehaviorStop>(1f);
				}
			}
			base.TickOccasionally();
		}
	}
}
