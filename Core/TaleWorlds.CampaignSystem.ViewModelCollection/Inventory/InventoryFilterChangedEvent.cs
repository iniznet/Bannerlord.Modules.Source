using System;
using TaleWorlds.Library.EventSystem;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Inventory
{
	// Token: 0x02000080 RID: 128
	public class InventoryFilterChangedEvent : EventBase
	{
		// Token: 0x17000426 RID: 1062
		// (get) Token: 0x06000CC2 RID: 3266 RVA: 0x00034136 File Offset: 0x00032336
		// (set) Token: 0x06000CC3 RID: 3267 RVA: 0x0003413E File Offset: 0x0003233E
		public SPInventoryVM.Filters NewFilter { get; private set; }

		// Token: 0x06000CC4 RID: 3268 RVA: 0x00034147 File Offset: 0x00032347
		public InventoryFilterChangedEvent(SPInventoryVM.Filters newFilter)
		{
			this.NewFilter = newFilter;
		}
	}
}
