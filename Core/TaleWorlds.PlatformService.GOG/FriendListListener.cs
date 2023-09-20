using System;
using Galaxy.Api;

namespace TaleWorlds.PlatformService.GOG
{
	// Token: 0x02000006 RID: 6
	public class FriendListListener : IFriendListListener
	{
		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000020 RID: 32 RVA: 0x00002396 File Offset: 0x00000596
		// (set) Token: 0x06000021 RID: 33 RVA: 0x0000239E File Offset: 0x0000059E
		public bool GotResult { get; private set; }

		// Token: 0x06000022 RID: 34 RVA: 0x000023A7 File Offset: 0x000005A7
		public override void OnFriendListRetrieveSuccess()
		{
			this.GotResult = true;
		}

		// Token: 0x06000023 RID: 35 RVA: 0x000023B0 File Offset: 0x000005B0
		public override void OnFriendListRetrieveFailure(IFriendListListener.FailureReason failureReason)
		{
			this.GotResult = true;
		}
	}
}
