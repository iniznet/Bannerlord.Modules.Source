using System;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	// Token: 0x020000AD RID: 173
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class PSPlayerJoinedToPlayerSessionMessage : Message
	{
		// Token: 0x170000EE RID: 238
		// (get) Token: 0x06000271 RID: 625 RVA: 0x00003B82 File Offset: 0x00001D82
		public ulong InviterPlayerAccountId { get; }

		// Token: 0x06000272 RID: 626 RVA: 0x00003B8A File Offset: 0x00001D8A
		public PSPlayerJoinedToPlayerSessionMessage(ulong inviterPlayerAccountId)
		{
			this.InviterPlayerAccountId = inviterPlayerAccountId;
		}
	}
}
