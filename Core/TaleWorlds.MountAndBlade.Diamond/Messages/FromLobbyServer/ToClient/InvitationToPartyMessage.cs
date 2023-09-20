using System;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x02000035 RID: 53
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class InvitationToPartyMessage : Message
	{
		// Token: 0x17000049 RID: 73
		// (get) Token: 0x060000BC RID: 188 RVA: 0x00002855 File Offset: 0x00000A55
		// (set) Token: 0x060000BD RID: 189 RVA: 0x0000285D File Offset: 0x00000A5D
		public string InviterPlayerName { get; private set; }

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x060000BE RID: 190 RVA: 0x00002866 File Offset: 0x00000A66
		// (set) Token: 0x060000BF RID: 191 RVA: 0x0000286E File Offset: 0x00000A6E
		public PlayerId InviterPlayerId { get; private set; }

		// Token: 0x060000C0 RID: 192 RVA: 0x00002877 File Offset: 0x00000A77
		public InvitationToPartyMessage(string inviterPlayerName, PlayerId inviterPlayerId)
		{
			this.InviterPlayerName = inviterPlayerName;
			this.InviterPlayerId = inviterPlayerId;
		}
	}
}
