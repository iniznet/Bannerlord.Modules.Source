using System;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem
{
	// Token: 0x0200008E RID: 142
	public struct NavigationPermissionItem
	{
		// Token: 0x170004B9 RID: 1209
		// (get) Token: 0x060010D7 RID: 4311 RVA: 0x0004AA78 File Offset: 0x00048C78
		// (set) Token: 0x060010D8 RID: 4312 RVA: 0x0004AA80 File Offset: 0x00048C80
		public bool IsAuthorized { get; private set; }

		// Token: 0x170004BA RID: 1210
		// (get) Token: 0x060010D9 RID: 4313 RVA: 0x0004AA89 File Offset: 0x00048C89
		// (set) Token: 0x060010DA RID: 4314 RVA: 0x0004AA91 File Offset: 0x00048C91
		public TextObject ReasonString { get; private set; }

		// Token: 0x060010DB RID: 4315 RVA: 0x0004AA9A File Offset: 0x00048C9A
		public NavigationPermissionItem(bool isAuthorized, TextObject reasonString)
		{
			this.IsAuthorized = isAuthorized;
			this.ReasonString = reasonString;
		}
	}
}
