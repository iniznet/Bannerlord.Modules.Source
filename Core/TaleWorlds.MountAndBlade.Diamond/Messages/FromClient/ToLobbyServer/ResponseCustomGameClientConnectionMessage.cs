using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	// Token: 0x020000BA RID: 186
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class ResponseCustomGameClientConnectionMessage : Message
	{
		// Token: 0x17000106 RID: 262
		// (get) Token: 0x060002A8 RID: 680 RVA: 0x00003E09 File Offset: 0x00002009
		// (set) Token: 0x060002A9 RID: 681 RVA: 0x00003E11 File Offset: 0x00002011
		public PlayerJoinGameResponseDataFromHost[] PlayerJoinData { get; private set; }

		// Token: 0x060002AA RID: 682 RVA: 0x00003E1A File Offset: 0x0000201A
		public ResponseCustomGameClientConnectionMessage(PlayerJoinGameResponseDataFromHost[] playerJoinData)
		{
			this.PlayerJoinData = playerJoinData;
		}
	}
}
