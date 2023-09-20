using System;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.CharacterCreationContent
{
	// Token: 0x020001D8 RID: 472
	public class FaceGenMount
	{
		// Token: 0x17000727 RID: 1831
		// (get) Token: 0x06001BC4 RID: 7108 RVA: 0x0007DA4E File Offset: 0x0007BC4E
		// (set) Token: 0x06001BC5 RID: 7109 RVA: 0x0007DA56 File Offset: 0x0007BC56
		public MountCreationKey MountKey { get; private set; }

		// Token: 0x17000728 RID: 1832
		// (get) Token: 0x06001BC6 RID: 7110 RVA: 0x0007DA5F File Offset: 0x0007BC5F
		// (set) Token: 0x06001BC7 RID: 7111 RVA: 0x0007DA67 File Offset: 0x0007BC67
		public ItemObject HorseItem { get; private set; }

		// Token: 0x17000729 RID: 1833
		// (get) Token: 0x06001BC8 RID: 7112 RVA: 0x0007DA70 File Offset: 0x0007BC70
		// (set) Token: 0x06001BC9 RID: 7113 RVA: 0x0007DA78 File Offset: 0x0007BC78
		public ItemObject HarnessItem { get; private set; }

		// Token: 0x1700072A RID: 1834
		// (get) Token: 0x06001BCA RID: 7114 RVA: 0x0007DA81 File Offset: 0x0007BC81
		// (set) Token: 0x06001BCB RID: 7115 RVA: 0x0007DA89 File Offset: 0x0007BC89
		public string ActionName { get; set; }

		// Token: 0x06001BCC RID: 7116 RVA: 0x0007DA92 File Offset: 0x0007BC92
		public FaceGenMount(MountCreationKey mountKey, ItemObject horseItem, ItemObject harnessItem, string actionName = "act_inventory_idle_start")
		{
			this.MountKey = mountKey;
			this.HorseItem = horseItem;
			this.HarnessItem = harnessItem;
			this.ActionName = actionName;
		}
	}
}
