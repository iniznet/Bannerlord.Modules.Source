using System;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x02000014 RID: 20
	[Serializable]
	public class ClientQuitFromCustomGameMessage : Message
	{
		// Token: 0x1700001B RID: 27
		// (get) Token: 0x06000045 RID: 69 RVA: 0x0000232A File Offset: 0x0000052A
		// (set) Token: 0x06000046 RID: 70 RVA: 0x00002332 File Offset: 0x00000532
		public PlayerId PlayerId { get; private set; }

		// Token: 0x06000047 RID: 71 RVA: 0x0000233B File Offset: 0x0000053B
		public ClientQuitFromCustomGameMessage(PlayerId playerId)
		{
			this.PlayerId = playerId;
		}
	}
}
