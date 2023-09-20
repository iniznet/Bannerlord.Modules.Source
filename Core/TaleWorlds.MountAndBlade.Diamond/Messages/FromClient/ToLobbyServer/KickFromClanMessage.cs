using System;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromClient.ToLobbyServer
{
	// Token: 0x020000A8 RID: 168
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class KickFromClanMessage : Message
	{
		// Token: 0x170000E9 RID: 233
		// (get) Token: 0x06000262 RID: 610 RVA: 0x00003AE2 File Offset: 0x00001CE2
		// (set) Token: 0x06000263 RID: 611 RVA: 0x00003AEA File Offset: 0x00001CEA
		public PlayerId PlayerId { get; private set; }

		// Token: 0x06000264 RID: 612 RVA: 0x00003AF3 File Offset: 0x00001CF3
		public KickFromClanMessage(PlayerId playerId)
		{
			this.PlayerId = playerId;
		}
	}
}
