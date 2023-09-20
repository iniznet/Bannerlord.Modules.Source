using System;
using TaleWorlds.Library.EventSystem;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Inventory
{
	public class InventoryFilterChangedEvent : EventBase
	{
		public SPInventoryVM.Filters NewFilter { get; private set; }

		public InventoryFilterChangedEvent(SPInventoryVM.Filters newFilter)
		{
			this.NewFilter = newFilter;
		}
	}
}
