using System;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromClient.ToLobbyServer
{
	// Token: 0x020000AC RID: 172
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class PromotePlayerToPartyLeaderMessage : Message
	{
		// Token: 0x170000ED RID: 237
		// (get) Token: 0x0600026E RID: 622 RVA: 0x00003B62 File Offset: 0x00001D62
		// (set) Token: 0x0600026F RID: 623 RVA: 0x00003B6A File Offset: 0x00001D6A
		public PlayerId PromotedPlayerId { get; private set; }

		// Token: 0x06000270 RID: 624 RVA: 0x00003B73 File Offset: 0x00001D73
		public PromotePlayerToPartyLeaderMessage(PlayerId promotedPlayerId)
		{
			this.PromotedPlayerId = promotedPlayerId;
		}
	}
}
