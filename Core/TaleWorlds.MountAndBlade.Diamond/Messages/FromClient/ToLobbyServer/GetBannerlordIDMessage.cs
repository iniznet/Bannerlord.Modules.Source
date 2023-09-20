using System;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromClient.ToLobbyServer
{
	// Token: 0x02000091 RID: 145
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class GetBannerlordIDMessage : Message
	{
		// Token: 0x170000D4 RID: 212
		// (get) Token: 0x06000222 RID: 546 RVA: 0x00003832 File Offset: 0x00001A32
		// (set) Token: 0x06000223 RID: 547 RVA: 0x0000383A File Offset: 0x00001A3A
		public PlayerId PlayerId { get; private set; }

		// Token: 0x06000224 RID: 548 RVA: 0x00003843 File Offset: 0x00001A43
		public GetBannerlordIDMessage(PlayerId playerId)
		{
			this.PlayerId = playerId;
		}
	}
}
