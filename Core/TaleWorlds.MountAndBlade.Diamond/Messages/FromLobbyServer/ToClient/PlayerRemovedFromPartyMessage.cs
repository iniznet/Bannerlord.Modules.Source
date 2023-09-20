using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x02000049 RID: 73
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class PlayerRemovedFromPartyMessage : Message
	{
		// Token: 0x17000065 RID: 101
		// (get) Token: 0x0600010A RID: 266 RVA: 0x00002BBB File Offset: 0x00000DBB
		// (set) Token: 0x0600010B RID: 267 RVA: 0x00002BC3 File Offset: 0x00000DC3
		public PlayerId PlayerId { get; private set; }

		// Token: 0x17000066 RID: 102
		// (get) Token: 0x0600010C RID: 268 RVA: 0x00002BCC File Offset: 0x00000DCC
		// (set) Token: 0x0600010D RID: 269 RVA: 0x00002BD4 File Offset: 0x00000DD4
		public PartyRemoveReason Reason { get; private set; }

		// Token: 0x0600010E RID: 270 RVA: 0x00002BDD File Offset: 0x00000DDD
		public PlayerRemovedFromPartyMessage(PlayerId playerId, PartyRemoveReason reason)
		{
			this.PlayerId = playerId;
			this.Reason = reason;
		}
	}
}
