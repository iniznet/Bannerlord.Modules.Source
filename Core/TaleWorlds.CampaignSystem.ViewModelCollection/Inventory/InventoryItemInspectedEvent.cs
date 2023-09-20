using System;
using TaleWorlds.CampaignSystem.Inventory;
using TaleWorlds.Core;
using TaleWorlds.Library.EventSystem;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Inventory
{
	// Token: 0x02000081 RID: 129
	public class InventoryItemInspectedEvent : EventBase
	{
		// Token: 0x17000427 RID: 1063
		// (get) Token: 0x06000CC5 RID: 3269 RVA: 0x00034156 File Offset: 0x00032356
		// (set) Token: 0x06000CC6 RID: 3270 RVA: 0x0003415E File Offset: 0x0003235E
		public ItemRosterElement Item { get; private set; }

		// Token: 0x17000428 RID: 1064
		// (get) Token: 0x06000CC7 RID: 3271 RVA: 0x00034167 File Offset: 0x00032367
		// (set) Token: 0x06000CC8 RID: 3272 RVA: 0x0003416F File Offset: 0x0003236F
		public InventoryLogic.InventorySide ItemSide { get; private set; }

		// Token: 0x06000CC9 RID: 3273 RVA: 0x00034178 File Offset: 0x00032378
		public InventoryItemInspectedEvent(ItemRosterElement item, InventoryLogic.InventorySide itemSide)
		{
			this.ItemSide = itemSide;
			this.Item = item;
		}
	}
}
