using System;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromClient.ToLobbyServer
{
	// Token: 0x020000A5 RID: 165
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class InviteToGameMessage : Message
	{
		// Token: 0x170000E5 RID: 229
		// (get) Token: 0x06000257 RID: 599 RVA: 0x00003A6A File Offset: 0x00001C6A
		// (set) Token: 0x06000258 RID: 600 RVA: 0x00003A72 File Offset: 0x00001C72
		public PlayerId InvitedPlayerId { get; private set; }

		// Token: 0x06000259 RID: 601 RVA: 0x00003A7B File Offset: 0x00001C7B
		public InviteToGameMessage(PlayerId invitedPlayerId)
		{
			this.InvitedPlayerId = invitedPlayerId;
		}
	}
}
