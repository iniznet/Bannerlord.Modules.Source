using System;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x02000043 RID: 67
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class PlayerAssignedPartyLeaderMessage : Message
	{
		// Token: 0x1700005E RID: 94
		// (get) Token: 0x060000F6 RID: 246 RVA: 0x00002AE3 File Offset: 0x00000CE3
		// (set) Token: 0x060000F7 RID: 247 RVA: 0x00002AEB File Offset: 0x00000CEB
		public PlayerId PartyLeaderId { get; private set; }

		// Token: 0x060000F8 RID: 248 RVA: 0x00002AF4 File Offset: 0x00000CF4
		public PlayerAssignedPartyLeaderMessage(PlayerId partyLeaderId)
		{
			this.PartyLeaderId = partyLeaderId;
		}
	}
}
