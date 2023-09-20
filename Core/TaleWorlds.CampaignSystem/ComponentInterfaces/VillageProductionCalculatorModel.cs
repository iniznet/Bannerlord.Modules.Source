using System;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x02000189 RID: 393
	public abstract class VillageProductionCalculatorModel : GameModel
	{
		// Token: 0x060019BB RID: 6587
		public abstract float CalculateProductionSpeedOfItemCategory(ItemCategory item);

		// Token: 0x060019BC RID: 6588
		public abstract float CalculateDailyProductionAmount(Village village, ItemObject item);

		// Token: 0x060019BD RID: 6589
		public abstract float CalculateDailyFoodProductionAmount(Village village);
	}
}
