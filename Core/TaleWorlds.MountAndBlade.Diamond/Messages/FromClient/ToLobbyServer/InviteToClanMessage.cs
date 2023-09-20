using System;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromClient.ToLobbyServer
{
	// Token: 0x020000A4 RID: 164
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class InviteToClanMessage : Message
	{
		// Token: 0x170000E3 RID: 227
		// (get) Token: 0x06000252 RID: 594 RVA: 0x00003A32 File Offset: 0x00001C32
		// (set) Token: 0x06000253 RID: 595 RVA: 0x00003A3A File Offset: 0x00001C3A
		public PlayerId InvitedPlayerId { get; private set; }

		// Token: 0x170000E4 RID: 228
		// (get) Token: 0x06000254 RID: 596 RVA: 0x00003A43 File Offset: 0x00001C43
		// (set) Token: 0x06000255 RID: 597 RVA: 0x00003A4B File Offset: 0x00001C4B
		public bool DontUseNameForUnknownPlayer { get; private set; }

		// Token: 0x06000256 RID: 598 RVA: 0x00003A54 File Offset: 0x00001C54
		public InviteToClanMessage(PlayerId invitedPlayerId, bool dontUseNameForUnknownPlayer)
		{
			this.InvitedPlayerId = invitedPlayerId;
			this.DontUseNameForUnknownPlayer = dontUseNameForUnknownPlayer;
		}
	}
}
