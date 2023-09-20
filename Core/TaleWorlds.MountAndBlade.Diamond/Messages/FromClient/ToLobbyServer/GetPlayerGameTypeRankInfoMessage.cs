using System;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromClient.ToLobbyServer
{
	// Token: 0x0200009C RID: 156
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class GetPlayerGameTypeRankInfoMessage : Message
	{
		// Token: 0x170000D9 RID: 217
		// (get) Token: 0x06000236 RID: 566 RVA: 0x000038F9 File Offset: 0x00001AF9
		// (set) Token: 0x06000237 RID: 567 RVA: 0x00003901 File Offset: 0x00001B01
		public PlayerId PlayerId { get; private set; }

		// Token: 0x06000238 RID: 568 RVA: 0x0000390A File Offset: 0x00001B0A
		public GetPlayerGameTypeRankInfoMessage(PlayerId playerId)
		{
			this.PlayerId = playerId;
		}
	}
}
