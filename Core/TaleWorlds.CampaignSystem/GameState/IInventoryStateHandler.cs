using System;
using TaleWorlds.CampaignSystem.Inventory;

namespace TaleWorlds.CampaignSystem.GameState
{
	// Token: 0x0200033B RID: 827
	public interface IInventoryStateHandler
	{
		// Token: 0x06002E55 RID: 11861
		void FilterInventoryAtOpening(InventoryManager.InventoryCategoryType inventoryCategoryType);

		// Token: 0x06002E56 RID: 11862
		void ExecuteLootingScript();

		// Token: 0x06002E57 RID: 11863
		void ExecuteSellAllLoot();

		// Token: 0x06002E58 RID: 11864
		void ExecuteBuyConsumableItem();
	}
}
