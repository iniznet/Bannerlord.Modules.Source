using System;
using TaleWorlds.Library.EventSystem;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.WeaponDesign
{
	public class CraftingOrderSelectionOpenedEvent : EventBase
	{
		public bool IsOpen { get; private set; }

		public CraftingOrderSelectionOpenedEvent(bool isOpen)
		{
			this.IsOpen = isOpen;
		}
	}
}
