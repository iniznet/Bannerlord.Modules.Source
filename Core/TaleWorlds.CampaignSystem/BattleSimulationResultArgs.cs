using System;
using System.Collections.Generic;

namespace TaleWorlds.CampaignSystem
{
	public class BattleSimulationResultArgs
	{
		public BattleSimulationResultArgs()
		{
			this.RoundResults = new List<BattleSimulationResult>();
		}

		public List<BattleSimulationResult> RoundResults;
	}
}
