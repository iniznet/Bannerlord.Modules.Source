using System;
using TaleWorlds.Library.EventSystem;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.CharacterDeveloper.PerkSelection
{
	// Token: 0x02000120 RID: 288
	public class PerkSelectionToggleEvent : EventBase
	{
		// Token: 0x17000995 RID: 2453
		// (get) Token: 0x06001BF6 RID: 7158 RVA: 0x00064A20 File Offset: 0x00062C20
		// (set) Token: 0x06001BF7 RID: 7159 RVA: 0x00064A28 File Offset: 0x00062C28
		public bool IsCurrentlyActive { get; private set; }

		// Token: 0x06001BF8 RID: 7160 RVA: 0x00064A31 File Offset: 0x00062C31
		public PerkSelectionToggleEvent(bool isCurrentlyActive)
		{
			this.IsCurrentlyActive = isCurrentlyActive;
		}
	}
}
