using System;
using TaleWorlds.Library.EventSystem;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Inventory
{
	public class InventoryEquipmentTypeChangedEvent : EventBase
	{
		public bool IsCurrentlyWarSet { get; private set; }

		public InventoryEquipmentTypeChangedEvent(bool isCurrentlyWarSet)
		{
			this.IsCurrentlyWarSet = isCurrentlyWarSet;
		}
	}
}
