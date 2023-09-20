using System;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	// Token: 0x020000B1 RID: 177
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class RegisterCustomGameMessage : Message
	{
		// Token: 0x170000F0 RID: 240
		// (get) Token: 0x06000278 RID: 632 RVA: 0x00003BC9 File Offset: 0x00001DC9
		// (set) Token: 0x06000279 RID: 633 RVA: 0x00003BD1 File Offset: 0x00001DD1
		public string GameModule { get; private set; }

		// Token: 0x170000F1 RID: 241
		// (get) Token: 0x0600027A RID: 634 RVA: 0x00003BDA File Offset: 0x00001DDA
		// (set) Token: 0x0600027B RID: 635 RVA: 0x00003BE2 File Offset: 0x00001DE2
		public string GameType { get; private set; }

		// Token: 0x170000F2 RID: 242
		// (get) Token: 0x0600027C RID: 636 RVA: 0x00003BEB File Offset: 0x00001DEB
		// (set) Token: 0x0600027D RID: 637 RVA: 0x00003BF3 File Offset: 0x00001DF3
		public string ServerName { get; private set; }

		// Token: 0x170000F3 RID: 243
		// (get) Token: 0x0600027E RID: 638 RVA: 0x00003BFC File Offset: 0x00001DFC
		// (set) Token: 0x0600027F RID: 639 RVA: 0x00003C04 File Offset: 0x00001E04
		public int MaxPlayerCount { get; private set; }

		// Token: 0x170000F4 RID: 244
		// (get) Token: 0x06000280 RID: 640 RVA: 0x00003C0D File Offset: 0x00001E0D
		// (set) Token: 0x06000281 RID: 641 RVA: 0x00003C15 File Offset: 0x00001E15
		public string Map { get; private set; }

		// Token: 0x170000F5 RID: 245
		// (get) Token: 0x06000282 RID: 642 RVA: 0x00003C1E File Offset: 0x00001E1E
		// (set) Token: 0x06000283 RID: 643 RVA: 0x00003C26 File Offset: 0x00001E26
		public string UniqueMapId { get; private set; }

		// Token: 0x170000F6 RID: 246
		// (get) Token: 0x06000284 RID: 644 RVA: 0x00003C2F File Offset: 0x00001E2F
		// (set) Token: 0x06000285 RID: 645 RVA: 0x00003C37 File Offset: 0x00001E37
		public int Port { get; private set; }

		// Token: 0x170000F7 RID: 247
		// (get) Token: 0x06000286 RID: 646 RVA: 0x00003C40 File Offset: 0x00001E40
		// (set) Token: 0x06000287 RID: 647 RVA: 0x00003C48 File Offset: 0x00001E48
		public string GamePassword { get; private set; }

		// Token: 0x170000F8 RID: 248
		// (get) Token: 0x06000288 RID: 648 RVA: 0x00003C51 File Offset: 0x00001E51
		// (set) Token: 0x06000289 RID: 649 RVA: 0x00003C59 File Offset: 0x00001E59
		public string AdminPassword { get; private set; }

		// Token: 0x0600028A RID: 650 RVA: 0x00003C64 File Offset: 0x00001E64
		public RegisterCustomGameMessage(string gameModule, string gameType, string serverName, int maxPlayerCount, string map, string uniqueMapId, string gamePassword, string adminPassword, int port)
		{
			this.GameModule = gameModule;
			this.GameType = gameType;
			this.ServerName = serverName;
			this.MaxPlayerCount = maxPlayerCount;
			this.Map = map;
			this.UniqueMapId = uniqueMapId;
			this.GamePassword = gamePassword;
			this.AdminPassword = adminPassword;
			this.Port = port;
		}
	}
}
