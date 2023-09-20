using System;
using System.Collections.Generic;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromClient.ToLobbyServer
{
	// Token: 0x02000097 RID: 151
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class GetOtherPlayersStateMessage : Message
	{
		// Token: 0x170000D5 RID: 213
		// (get) Token: 0x0600022A RID: 554 RVA: 0x0000387A File Offset: 0x00001A7A
		public List<PlayerId> Players { get; }

		// Token: 0x0600022B RID: 555 RVA: 0x00003882 File Offset: 0x00001A82
		public GetOtherPlayersStateMessage(List<PlayerId> players)
		{
			this.Players = players;
		}
	}
}
