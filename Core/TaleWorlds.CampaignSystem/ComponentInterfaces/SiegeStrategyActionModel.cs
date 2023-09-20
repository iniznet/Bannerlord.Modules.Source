using System;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x020001BA RID: 442
	public abstract class SiegeStrategyActionModel : GameModel
	{
		// Token: 0x06001B06 RID: 6918
		public abstract void GetLogicalActionForStrategy(ISiegeEventSide side, out SiegeStrategyActionModel.SiegeAction siegeAction, out SiegeEngineType siegeEngineType, out int deploymentIndex, out int reserveIndex);

		// Token: 0x0200055D RID: 1373
		public enum SiegeAction
		{
			// Token: 0x040016AB RID: 5803
			ConstructNewSiegeEngine,
			// Token: 0x040016AC RID: 5804
			DeploySiegeEngineFromReserve,
			// Token: 0x040016AD RID: 5805
			MoveSiegeEngineToReserve,
			// Token: 0x040016AE RID: 5806
			RemoveDeployedSiegeEngine,
			// Token: 0x040016AF RID: 5807
			Hold
		}
	}
}
