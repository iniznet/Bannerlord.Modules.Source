using System;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromClient.ToLobbyServer
{
	// Token: 0x020000A6 RID: 166
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class InviteToPartyMessage : Message
	{
		// Token: 0x170000E6 RID: 230
		// (get) Token: 0x0600025A RID: 602 RVA: 0x00003A8A File Offset: 0x00001C8A
		// (set) Token: 0x0600025B RID: 603 RVA: 0x00003A92 File Offset: 0x00001C92
		public PlayerId InvitedPlayerId { get; private set; }

		// Token: 0x170000E7 RID: 231
		// (get) Token: 0x0600025C RID: 604 RVA: 0x00003A9B File Offset: 0x00001C9B
		// (set) Token: 0x0600025D RID: 605 RVA: 0x00003AA3 File Offset: 0x00001CA3
		public bool DontUseNameForUnknownPlayer { get; private set; }

		// Token: 0x0600025E RID: 606 RVA: 0x00003AAC File Offset: 0x00001CAC
		public InviteToPartyMessage(PlayerId invitedPlayerId, bool dontUseNameForUnknownPlayer)
		{
			this.InvitedPlayerId = invitedPlayerId;
			this.DontUseNameForUnknownPlayer = dontUseNameForUnknownPlayer;
		}
	}
}
