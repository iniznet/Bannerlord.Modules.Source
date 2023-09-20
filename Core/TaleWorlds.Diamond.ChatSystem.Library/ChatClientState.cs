using System;

namespace TaleWorlds.Diamond.ChatSystem.Library
{
	// Token: 0x02000004 RID: 4
	public enum ChatClientState
	{
		// Token: 0x0400000C RID: 12
		Created,
		// Token: 0x0400000D RID: 13
		Connecting,
		// Token: 0x0400000E RID: 14
		Connected,
		// Token: 0x0400000F RID: 15
		Disconnected,
		// Token: 0x04000010 RID: 16
		WaitingForReconnect,
		// Token: 0x04000011 RID: 17
		Stopped
	}
}
