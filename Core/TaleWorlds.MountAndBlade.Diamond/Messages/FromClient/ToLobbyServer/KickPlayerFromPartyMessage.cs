using System;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromClient.ToLobbyServer
{
	// Token: 0x020000A9 RID: 169
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class KickPlayerFromPartyMessage : Message
	{
		// Token: 0x170000EA RID: 234
		// (get) Token: 0x06000265 RID: 613 RVA: 0x00003B02 File Offset: 0x00001D02
		// (set) Token: 0x06000266 RID: 614 RVA: 0x00003B0A File Offset: 0x00001D0A
		public PlayerId KickedPlayerId { get; private set; }

		// Token: 0x06000267 RID: 615 RVA: 0x00003B13 File Offset: 0x00001D13
		public KickPlayerFromPartyMessage(PlayerId kickedPlayerId)
		{
			this.KickedPlayerId = kickedPlayerId;
		}
	}
}
