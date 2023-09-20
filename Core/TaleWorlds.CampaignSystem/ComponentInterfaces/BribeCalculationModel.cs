using System;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x0200017B RID: 379
	public abstract class BribeCalculationModel : GameModel
	{
		// Token: 0x06001931 RID: 6449
		public abstract int GetBribeToEnterLordsHall(Settlement settlement);

		// Token: 0x06001932 RID: 6450
		public abstract int GetBribeToEnterDungeon(Settlement settlement);

		// Token: 0x06001933 RID: 6451
		public abstract bool IsBribeNotNeededToEnterKeep(Settlement settlement);

		// Token: 0x06001934 RID: 6452
		public abstract bool IsBribeNotNeededToEnterDungeon(Settlement settlement);
	}
}
