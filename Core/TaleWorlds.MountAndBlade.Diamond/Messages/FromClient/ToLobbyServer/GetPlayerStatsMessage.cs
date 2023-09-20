using System;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromClient.ToLobbyServer
{
	// Token: 0x0200009D RID: 157
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class GetPlayerStatsMessage : Message
	{
		// Token: 0x170000DA RID: 218
		// (get) Token: 0x06000239 RID: 569 RVA: 0x00003919 File Offset: 0x00001B19
		// (set) Token: 0x0600023A RID: 570 RVA: 0x00003921 File Offset: 0x00001B21
		public PlayerId PlayerId { get; private set; }

		// Token: 0x0600023B RID: 571 RVA: 0x0000392A File Offset: 0x00001B2A
		public GetPlayerStatsMessage(PlayerId playerId)
		{
			this.PlayerId = playerId;
		}
	}
}
