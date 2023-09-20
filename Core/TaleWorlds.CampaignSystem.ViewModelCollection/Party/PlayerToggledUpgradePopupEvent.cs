using System;
using TaleWorlds.Library.EventSystem;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Party
{
	// Token: 0x0200002A RID: 42
	public class PlayerToggledUpgradePopupEvent : EventBase
	{
		// Token: 0x17000136 RID: 310
		// (get) Token: 0x06000442 RID: 1090 RVA: 0x00017E5F File Offset: 0x0001605F
		// (set) Token: 0x06000443 RID: 1091 RVA: 0x00017E67 File Offset: 0x00016067
		public bool IsOpened { get; private set; }

		// Token: 0x06000444 RID: 1092 RVA: 0x00017E70 File Offset: 0x00016070
		public PlayerToggledUpgradePopupEvent(bool isOpened)
		{
			this.IsOpened = isOpened;
		}
	}
}
