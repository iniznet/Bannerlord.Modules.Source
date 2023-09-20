using System;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromClient.ToLobbyServer
{
	// Token: 0x0200008E RID: 142
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class GetAnotherPlayerStateMessage : Message
	{
		// Token: 0x170000D3 RID: 211
		// (get) Token: 0x0600021D RID: 541 RVA: 0x00003802 File Offset: 0x00001A02
		// (set) Token: 0x0600021E RID: 542 RVA: 0x0000380A File Offset: 0x00001A0A
		public PlayerId PlayerId { get; private set; }

		// Token: 0x0600021F RID: 543 RVA: 0x00003813 File Offset: 0x00001A13
		public GetAnotherPlayerStateMessage(PlayerId playerId)
		{
			this.PlayerId = playerId;
		}
	}
}
