using System;
using TaleWorlds.Library.EventSystem;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.WeaponDesign
{
	// Token: 0x020000E2 RID: 226
	public class CraftingWeaponClassSelectionOpenedEvent : EventBase
	{
		// Token: 0x17000706 RID: 1798
		// (get) Token: 0x060014DF RID: 5343 RVA: 0x0004E806 File Offset: 0x0004CA06
		// (set) Token: 0x060014E0 RID: 5344 RVA: 0x0004E80E File Offset: 0x0004CA0E
		public bool IsOpen { get; private set; }

		// Token: 0x060014E1 RID: 5345 RVA: 0x0004E817 File Offset: 0x0004CA17
		public CraftingWeaponClassSelectionOpenedEvent(bool isOpen)
		{
			this.IsOpen = isOpen;
		}
	}
}
