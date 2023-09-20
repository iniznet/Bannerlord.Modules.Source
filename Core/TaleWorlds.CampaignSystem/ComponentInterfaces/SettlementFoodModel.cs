using System;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x0200019C RID: 412
	public abstract class SettlementFoodModel : GameModel
	{
		// Token: 0x170006B5 RID: 1717
		// (get) Token: 0x06001A33 RID: 6707
		public abstract int FoodStocksUpperLimit { get; }

		// Token: 0x170006B6 RID: 1718
		// (get) Token: 0x06001A34 RID: 6708
		public abstract int NumberOfProsperityToEatOneFood { get; }

		// Token: 0x170006B7 RID: 1719
		// (get) Token: 0x06001A35 RID: 6709
		public abstract int NumberOfMenOnGarrisonToEatOneFood { get; }

		// Token: 0x170006B8 RID: 1720
		// (get) Token: 0x06001A36 RID: 6710
		public abstract int CastleFoodStockUpperLimitBonus { get; }

		// Token: 0x06001A37 RID: 6711
		public abstract ExplainedNumber CalculateTownFoodStocksChange(Town town, bool includeMarketStocks = true, bool includeDescriptions = false);
	}
}
