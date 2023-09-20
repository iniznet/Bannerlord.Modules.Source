using System;
using TaleWorlds.Library.EventSystem;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Inventory
{
	// Token: 0x0200007F RID: 127
	public class InventoryEquipmentTypeChangedEvent : EventBase
	{
		// Token: 0x17000425 RID: 1061
		// (get) Token: 0x06000CBF RID: 3263 RVA: 0x00034116 File Offset: 0x00032316
		// (set) Token: 0x06000CC0 RID: 3264 RVA: 0x0003411E File Offset: 0x0003231E
		public bool IsCurrentlyWarSet { get; private set; }

		// Token: 0x06000CC1 RID: 3265 RVA: 0x00034127 File Offset: 0x00032327
		public InventoryEquipmentTypeChangedEvent(bool isCurrentlyWarSet)
		{
			this.IsCurrentlyWarSet = isCurrentlyWarSet;
		}
	}
}
