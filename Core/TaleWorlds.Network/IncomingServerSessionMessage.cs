using System;

namespace TaleWorlds.Network
{
	// Token: 0x02000024 RID: 36
	internal class IncomingServerSessionMessage
	{
		// Token: 0x1700002B RID: 43
		// (get) Token: 0x0600010D RID: 269 RVA: 0x0000451C File Offset: 0x0000271C
		// (set) Token: 0x0600010E RID: 270 RVA: 0x00004524 File Offset: 0x00002724
		internal NetworkMessage NetworkMessage { get; set; }

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x0600010F RID: 271 RVA: 0x0000452D File Offset: 0x0000272D
		// (set) Token: 0x06000110 RID: 272 RVA: 0x00004535 File Offset: 0x00002735
		internal ServersideSession Peer { get; set; }
	}
}
