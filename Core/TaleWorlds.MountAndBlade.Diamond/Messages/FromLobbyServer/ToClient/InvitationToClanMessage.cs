using System;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x02000034 RID: 52
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class InvitationToClanMessage : Message
	{
		// Token: 0x17000045 RID: 69
		// (get) Token: 0x060000B3 RID: 179 RVA: 0x000027EC File Offset: 0x000009EC
		// (set) Token: 0x060000B4 RID: 180 RVA: 0x000027F4 File Offset: 0x000009F4
		public PlayerId InviterId { get; private set; }

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x060000B5 RID: 181 RVA: 0x000027FD File Offset: 0x000009FD
		// (set) Token: 0x060000B6 RID: 182 RVA: 0x00002805 File Offset: 0x00000A05
		public string ClanName { get; private set; }

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x060000B7 RID: 183 RVA: 0x0000280E File Offset: 0x00000A0E
		// (set) Token: 0x060000B8 RID: 184 RVA: 0x00002816 File Offset: 0x00000A16
		public string ClanTag { get; private set; }

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x060000B9 RID: 185 RVA: 0x0000281F File Offset: 0x00000A1F
		// (set) Token: 0x060000BA RID: 186 RVA: 0x00002827 File Offset: 0x00000A27
		public int ClanPlayerCount { get; private set; }

		// Token: 0x060000BB RID: 187 RVA: 0x00002830 File Offset: 0x00000A30
		public InvitationToClanMessage(PlayerId inviterId, string clanName, string clanTag, int clanPlayerCount)
		{
			this.InviterId = inviterId;
			this.ClanName = clanName;
			this.ClanTag = clanTag;
			this.ClanPlayerCount = clanPlayerCount;
		}
	}
}
