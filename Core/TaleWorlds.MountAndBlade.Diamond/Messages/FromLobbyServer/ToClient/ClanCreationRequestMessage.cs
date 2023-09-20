using System;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x0200000F RID: 15
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class ClanCreationRequestMessage : Message
	{
		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000032 RID: 50 RVA: 0x00002259 File Offset: 0x00000459
		// (set) Token: 0x06000033 RID: 51 RVA: 0x00002261 File Offset: 0x00000461
		public string CreatorPlayerName { get; private set; }

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000034 RID: 52 RVA: 0x0000226A File Offset: 0x0000046A
		// (set) Token: 0x06000035 RID: 53 RVA: 0x00002272 File Offset: 0x00000472
		public PlayerId CreatorPlayerId { get; private set; }

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000036 RID: 54 RVA: 0x0000227B File Offset: 0x0000047B
		// (set) Token: 0x06000037 RID: 55 RVA: 0x00002283 File Offset: 0x00000483
		public string ClanName { get; private set; }

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000038 RID: 56 RVA: 0x0000228C File Offset: 0x0000048C
		// (set) Token: 0x06000039 RID: 57 RVA: 0x00002294 File Offset: 0x00000494
		public string ClanTag { get; private set; }

		// Token: 0x0600003A RID: 58 RVA: 0x0000229D File Offset: 0x0000049D
		public ClanCreationRequestMessage(PlayerId creatorPlayerId, string creatorPlayerName, string clanName, string clanTag)
		{
			this.CreatorPlayerId = creatorPlayerId;
			this.CreatorPlayerName = creatorPlayerName;
			this.ClanName = clanName;
			this.ClanTag = clanTag;
		}
	}
}
