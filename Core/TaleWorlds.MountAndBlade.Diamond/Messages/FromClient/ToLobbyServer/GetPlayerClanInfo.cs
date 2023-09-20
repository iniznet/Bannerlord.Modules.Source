using System;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromClient.ToLobbyServer
{
	// Token: 0x0200009A RID: 154
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class GetPlayerClanInfo : Message
	{
		// Token: 0x170000D8 RID: 216
		// (get) Token: 0x06000232 RID: 562 RVA: 0x000038D1 File Offset: 0x00001AD1
		// (set) Token: 0x06000233 RID: 563 RVA: 0x000038D9 File Offset: 0x00001AD9
		public PlayerId PlayerId { get; private set; }

		// Token: 0x06000234 RID: 564 RVA: 0x000038E2 File Offset: 0x00001AE2
		public GetPlayerClanInfo(PlayerId playerId)
		{
			this.PlayerId = playerId;
		}
	}
}
