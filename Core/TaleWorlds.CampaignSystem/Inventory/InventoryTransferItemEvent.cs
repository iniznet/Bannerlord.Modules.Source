using System;
using TaleWorlds.Core;
using TaleWorlds.Library.EventSystem;

namespace TaleWorlds.CampaignSystem.Inventory
{
	// Token: 0x020000D6 RID: 214
	public class InventoryTransferItemEvent : EventBase
	{
		// Token: 0x17000579 RID: 1401
		// (get) Token: 0x06001337 RID: 4919 RVA: 0x000564F6 File Offset: 0x000546F6
		// (set) Token: 0x06001338 RID: 4920 RVA: 0x000564FE File Offset: 0x000546FE
		public ItemObject Item { get; private set; }

		// Token: 0x1700057A RID: 1402
		// (get) Token: 0x06001339 RID: 4921 RVA: 0x00056507 File Offset: 0x00054707
		// (set) Token: 0x0600133A RID: 4922 RVA: 0x0005650F File Offset: 0x0005470F
		public bool IsBuyForPlayer { get; private set; }

		// Token: 0x0600133B RID: 4923 RVA: 0x00056518 File Offset: 0x00054718
		public InventoryTransferItemEvent(ItemObject item, bool isBuyForPlayer)
		{
			this.Item = item;
			this.IsBuyForPlayer = isBuyForPlayer;
		}
	}
}
