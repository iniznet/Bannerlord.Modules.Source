using System;
using System.Collections.Generic;

namespace TaleWorlds.CampaignSystem
{
	// Token: 0x02000023 RID: 35
	public class BattleSimulationResultArgs
	{
		// Token: 0x06000154 RID: 340 RVA: 0x0000EA63 File Offset: 0x0000CC63
		public BattleSimulationResultArgs()
		{
			this.RoundResults = new List<BattleSimulationResult>();
		}

		// Token: 0x04000038 RID: 56
		public List<BattleSimulationResult> RoundResults;
	}
}
