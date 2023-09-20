using System;
using TaleWorlds.Library.EventSystem;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia
{
	// Token: 0x020000A7 RID: 167
	public class EncyclopediaPageChangedEvent : EventBase
	{
		// Token: 0x1700058D RID: 1421
		// (get) Token: 0x060010D3 RID: 4307 RVA: 0x000430B7 File Offset: 0x000412B7
		// (set) Token: 0x060010D4 RID: 4308 RVA: 0x000430BF File Offset: 0x000412BF
		public EncyclopediaPages NewPage { get; private set; }

		// Token: 0x1700058E RID: 1422
		// (get) Token: 0x060010D5 RID: 4309 RVA: 0x000430C8 File Offset: 0x000412C8
		// (set) Token: 0x060010D6 RID: 4310 RVA: 0x000430D0 File Offset: 0x000412D0
		public bool NewPageHasHiddenInformation { get; private set; }

		// Token: 0x060010D7 RID: 4311 RVA: 0x000430D9 File Offset: 0x000412D9
		public EncyclopediaPageChangedEvent(EncyclopediaPages newPage, bool hasHiddenInformation = false)
		{
			this.NewPage = newPage;
			this.NewPageHasHiddenInformation = hasHiddenInformation;
		}
	}
}
