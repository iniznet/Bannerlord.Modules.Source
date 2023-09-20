using System;
using TaleWorlds.Core;
using TaleWorlds.Library.EventSystem;

namespace TaleWorlds.CampaignSystem.Inventory
{
	public class InventoryTransferItemEvent : EventBase
	{
		public ItemObject Item { get; private set; }

		public bool IsBuyForPlayer { get; private set; }

		public InventoryTransferItemEvent(ItemObject item, bool isBuyForPlayer)
		{
			this.Item = item;
			this.IsBuyForPlayer = isBuyForPlayer;
		}
	}
}
