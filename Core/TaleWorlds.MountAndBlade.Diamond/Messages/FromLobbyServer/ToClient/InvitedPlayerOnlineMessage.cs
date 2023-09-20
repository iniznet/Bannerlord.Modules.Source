using System;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x02000036 RID: 54
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class InvitedPlayerOnlineMessage : Message
	{
		// Token: 0x1700004B RID: 75
		// (get) Token: 0x060000C1 RID: 193 RVA: 0x0000288D File Offset: 0x00000A8D
		// (set) Token: 0x060000C2 RID: 194 RVA: 0x00002895 File Offset: 0x00000A95
		public PlayerId PlayerId { get; private set; }

		// Token: 0x060000C3 RID: 195 RVA: 0x0000289E File Offset: 0x00000A9E
		public InvitedPlayerOnlineMessage(PlayerId playerId)
		{
			this.PlayerId = playerId;
		}
	}
}
