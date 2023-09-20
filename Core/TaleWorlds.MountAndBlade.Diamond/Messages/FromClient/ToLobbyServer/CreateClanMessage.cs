using System;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	// Token: 0x02000081 RID: 129
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class CreateClanMessage : Message
	{
		// Token: 0x170000C0 RID: 192
		// (get) Token: 0x060001EA RID: 490 RVA: 0x000035CC File Offset: 0x000017CC
		// (set) Token: 0x060001EB RID: 491 RVA: 0x000035D4 File Offset: 0x000017D4
		public string ClanName { get; private set; }

		// Token: 0x170000C1 RID: 193
		// (get) Token: 0x060001EC RID: 492 RVA: 0x000035DD File Offset: 0x000017DD
		// (set) Token: 0x060001ED RID: 493 RVA: 0x000035E5 File Offset: 0x000017E5
		public string ClanTag { get; private set; }

		// Token: 0x170000C2 RID: 194
		// (get) Token: 0x060001EE RID: 494 RVA: 0x000035EE File Offset: 0x000017EE
		// (set) Token: 0x060001EF RID: 495 RVA: 0x000035F6 File Offset: 0x000017F6
		public string ClanFaction { get; private set; }

		// Token: 0x170000C3 RID: 195
		// (get) Token: 0x060001F0 RID: 496 RVA: 0x000035FF File Offset: 0x000017FF
		// (set) Token: 0x060001F1 RID: 497 RVA: 0x00003607 File Offset: 0x00001807
		public string ClanSigil { get; private set; }

		// Token: 0x060001F2 RID: 498 RVA: 0x00003610 File Offset: 0x00001810
		public CreateClanMessage(string clanName, string clanTag, string clanFaction, string clanSigil)
		{
			this.ClanName = clanName;
			this.ClanTag = clanTag;
			this.ClanFaction = clanFaction;
			this.ClanSigil = clanSigil;
		}
	}
}
