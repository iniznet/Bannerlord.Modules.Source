using System;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromClient.ToLobbyServer
{
	// Token: 0x0200008D RID: 141
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class GetAnotherPlayerDataMessage : Message
	{
		// Token: 0x170000D2 RID: 210
		// (get) Token: 0x0600021A RID: 538 RVA: 0x000037E2 File Offset: 0x000019E2
		// (set) Token: 0x0600021B RID: 539 RVA: 0x000037EA File Offset: 0x000019EA
		public PlayerId PlayerId { get; private set; }

		// Token: 0x0600021C RID: 540 RVA: 0x000037F3 File Offset: 0x000019F3
		public GetAnotherPlayerDataMessage(PlayerId playerId)
		{
			this.PlayerId = playerId;
		}
	}
}
