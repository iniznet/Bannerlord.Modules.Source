using System;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x02000044 RID: 68
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class PlayerInvitedToPartyMessage : Message
	{
		// Token: 0x1700005F RID: 95
		// (get) Token: 0x060000F9 RID: 249 RVA: 0x00002B03 File Offset: 0x00000D03
		// (set) Token: 0x060000FA RID: 250 RVA: 0x00002B0B File Offset: 0x00000D0B
		public PlayerId PlayerId { get; private set; }

		// Token: 0x17000060 RID: 96
		// (get) Token: 0x060000FB RID: 251 RVA: 0x00002B14 File Offset: 0x00000D14
		// (set) Token: 0x060000FC RID: 252 RVA: 0x00002B1C File Offset: 0x00000D1C
		public string PlayerName { get; private set; }

		// Token: 0x060000FD RID: 253 RVA: 0x00002B25 File Offset: 0x00000D25
		public PlayerInvitedToPartyMessage(PlayerId playerId, string playerName)
		{
			this.PlayerId = playerId;
			this.PlayerName = playerName;
		}
	}
}
