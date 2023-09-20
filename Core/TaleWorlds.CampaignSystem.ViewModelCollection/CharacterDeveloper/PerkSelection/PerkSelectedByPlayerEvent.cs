using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Library.EventSystem;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.CharacterDeveloper.PerkSelection
{
	// Token: 0x0200011F RID: 287
	public class PerkSelectedByPlayerEvent : EventBase
	{
		// Token: 0x17000994 RID: 2452
		// (get) Token: 0x06001BF3 RID: 7155 RVA: 0x00064A00 File Offset: 0x00062C00
		// (set) Token: 0x06001BF4 RID: 7156 RVA: 0x00064A08 File Offset: 0x00062C08
		public PerkObject SelectedPerk { get; private set; }

		// Token: 0x06001BF5 RID: 7157 RVA: 0x00064A11 File Offset: 0x00062C11
		public PerkSelectedByPlayerEvent(PerkObject selectedPerk)
		{
			this.SelectedPerk = selectedPerk;
		}
	}
}
