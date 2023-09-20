using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	// Token: 0x02000082 RID: 130
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class CreatePremadeGameMessage : Message
	{
		// Token: 0x170000C4 RID: 196
		// (get) Token: 0x060001F3 RID: 499 RVA: 0x00003635 File Offset: 0x00001835
		// (set) Token: 0x060001F4 RID: 500 RVA: 0x0000363D File Offset: 0x0000183D
		public string PremadeGameName { get; private set; }

		// Token: 0x170000C5 RID: 197
		// (get) Token: 0x060001F5 RID: 501 RVA: 0x00003646 File Offset: 0x00001846
		// (set) Token: 0x060001F6 RID: 502 RVA: 0x0000364E File Offset: 0x0000184E
		public string GameType { get; private set; }

		// Token: 0x170000C6 RID: 198
		// (get) Token: 0x060001F7 RID: 503 RVA: 0x00003657 File Offset: 0x00001857
		// (set) Token: 0x060001F8 RID: 504 RVA: 0x0000365F File Offset: 0x0000185F
		public string MapName { get; private set; }

		// Token: 0x170000C7 RID: 199
		// (get) Token: 0x060001F9 RID: 505 RVA: 0x00003668 File Offset: 0x00001868
		// (set) Token: 0x060001FA RID: 506 RVA: 0x00003670 File Offset: 0x00001870
		public string FactionA { get; private set; }

		// Token: 0x170000C8 RID: 200
		// (get) Token: 0x060001FB RID: 507 RVA: 0x00003679 File Offset: 0x00001879
		// (set) Token: 0x060001FC RID: 508 RVA: 0x00003681 File Offset: 0x00001881
		public string FactionB { get; private set; }

		// Token: 0x170000C9 RID: 201
		// (get) Token: 0x060001FD RID: 509 RVA: 0x0000368A File Offset: 0x0000188A
		// (set) Token: 0x060001FE RID: 510 RVA: 0x00003692 File Offset: 0x00001892
		public string Password { get; private set; }

		// Token: 0x170000CA RID: 202
		// (get) Token: 0x060001FF RID: 511 RVA: 0x0000369B File Offset: 0x0000189B
		// (set) Token: 0x06000200 RID: 512 RVA: 0x000036A3 File Offset: 0x000018A3
		public PremadeGameType PremadeGameType { get; private set; }

		// Token: 0x06000201 RID: 513 RVA: 0x000036AC File Offset: 0x000018AC
		public CreatePremadeGameMessage(string premadeGameName, string gameType, string mapName, string factionA, string factionB, string password, PremadeGameType premadeGameType)
		{
			this.PremadeGameName = premadeGameName;
			this.GameType = gameType;
			this.MapName = mapName;
			this.FactionA = factionA;
			this.FactionB = factionB;
			this.Password = password;
			this.PremadeGameType = premadeGameType;
		}
	}
}
