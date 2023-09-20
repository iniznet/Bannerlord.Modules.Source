using System;
using TaleWorlds.Library.EventSystem;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.WeaponDesign
{
	// Token: 0x020000E3 RID: 227
	public class CraftingOrderSelectionOpenedEvent : EventBase
	{
		// Token: 0x17000707 RID: 1799
		// (get) Token: 0x060014E2 RID: 5346 RVA: 0x0004E826 File Offset: 0x0004CA26
		// (set) Token: 0x060014E3 RID: 5347 RVA: 0x0004E82E File Offset: 0x0004CA2E
		public bool IsOpen { get; private set; }

		// Token: 0x060014E4 RID: 5348 RVA: 0x0004E837 File Offset: 0x0004CA37
		public CraftingOrderSelectionOpenedEvent(bool isOpen)
		{
			this.IsOpen = isOpen;
		}
	}
}
