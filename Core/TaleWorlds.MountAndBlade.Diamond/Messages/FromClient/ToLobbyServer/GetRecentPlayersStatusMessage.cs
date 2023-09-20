using System;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromClient.ToLobbyServer
{
	// Token: 0x020000A0 RID: 160
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class GetRecentPlayersStatusMessage : Message
	{
		// Token: 0x170000DC RID: 220
		// (get) Token: 0x06000240 RID: 576 RVA: 0x00003961 File Offset: 0x00001B61
		// (set) Token: 0x06000241 RID: 577 RVA: 0x00003969 File Offset: 0x00001B69
		public PlayerId[] RecentPlayers { get; private set; }

		// Token: 0x06000242 RID: 578 RVA: 0x00003972 File Offset: 0x00001B72
		public GetRecentPlayersStatusMessage(PlayerId[] recentPlayers)
		{
			this.RecentPlayers = recentPlayers;
		}
	}
}
