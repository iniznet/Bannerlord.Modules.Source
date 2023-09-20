using System;

namespace TaleWorlds.Network
{
	// Token: 0x02000028 RID: 40
	internal enum TcpStatus
	{
		// Token: 0x0400006D RID: 109
		Connecting,
		// Token: 0x0400006E RID: 110
		WaitingDataLength,
		// Token: 0x0400006F RID: 111
		WaitingData,
		// Token: 0x04000070 RID: 112
		SocketClosed,
		// Token: 0x04000071 RID: 113
		DataReady,
		// Token: 0x04000072 RID: 114
		ConnectionClosed
	}
}
