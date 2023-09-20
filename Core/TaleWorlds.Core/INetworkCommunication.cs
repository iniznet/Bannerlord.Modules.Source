using System;

namespace TaleWorlds.Core
{
	// Token: 0x0200008D RID: 141
	public interface INetworkCommunication
	{
		// Token: 0x1700029A RID: 666
		// (get) Token: 0x060007CA RID: 1994
		VirtualPlayer MyPeer { get; }

		// Token: 0x1700029B RID: 667
		// (get) Token: 0x060007CB RID: 1995
		bool IsServer { get; }

		// Token: 0x1700029C RID: 668
		// (get) Token: 0x060007CC RID: 1996
		bool IsClient { get; }

		// Token: 0x1700029D RID: 669
		// (get) Token: 0x060007CD RID: 1997
		bool MultiplayerDisabled { get; }
	}
}
