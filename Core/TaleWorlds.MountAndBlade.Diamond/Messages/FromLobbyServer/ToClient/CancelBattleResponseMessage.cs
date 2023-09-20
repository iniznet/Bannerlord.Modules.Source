using System;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x02000007 RID: 7
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class CancelBattleResponseMessage : Message
	{
		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000016 RID: 22 RVA: 0x00002129 File Offset: 0x00000329
		// (set) Token: 0x06000017 RID: 23 RVA: 0x00002131 File Offset: 0x00000331
		public bool Successful { get; private set; }

		// Token: 0x06000018 RID: 24 RVA: 0x0000213A File Offset: 0x0000033A
		public CancelBattleResponseMessage(bool successful)
		{
			this.Successful = successful;
		}
	}
}
