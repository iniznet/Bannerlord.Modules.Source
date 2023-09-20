using System;
using TaleWorlds.Library.EventSystem;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.WeaponDesign
{
	// Token: 0x020000E6 RID: 230
	public class CraftingWeaponResultPopupToggledEvent : EventBase
	{
		// Token: 0x17000722 RID: 1826
		// (get) Token: 0x06001524 RID: 5412 RVA: 0x0004EF6D File Offset: 0x0004D16D
		public bool IsOpen { get; }

		// Token: 0x06001525 RID: 5413 RVA: 0x0004EF75 File Offset: 0x0004D175
		public CraftingWeaponResultPopupToggledEvent(bool isOpen)
		{
			this.IsOpen = isOpen;
		}
	}
}
