using System;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x020000F1 RID: 241
	[Serializable]
	public struct BattleServerInformationForClient
	{
		// Token: 0x1700019A RID: 410
		// (get) Token: 0x06000429 RID: 1065 RVA: 0x00005FAD File Offset: 0x000041AD
		// (set) Token: 0x0600042A RID: 1066 RVA: 0x00005FB5 File Offset: 0x000041B5
		public string MatchId { get; set; }

		// Token: 0x1700019B RID: 411
		// (get) Token: 0x0600042B RID: 1067 RVA: 0x00005FBE File Offset: 0x000041BE
		// (set) Token: 0x0600042C RID: 1068 RVA: 0x00005FC6 File Offset: 0x000041C6
		public string ServerAddress { get; set; }

		// Token: 0x1700019C RID: 412
		// (get) Token: 0x0600042D RID: 1069 RVA: 0x00005FCF File Offset: 0x000041CF
		// (set) Token: 0x0600042E RID: 1070 RVA: 0x00005FD7 File Offset: 0x000041D7
		public ushort ServerPort { get; set; }

		// Token: 0x1700019D RID: 413
		// (get) Token: 0x0600042F RID: 1071 RVA: 0x00005FE0 File Offset: 0x000041E0
		// (set) Token: 0x06000430 RID: 1072 RVA: 0x00005FE8 File Offset: 0x000041E8
		public int PeerIndex { get; set; }

		// Token: 0x1700019E RID: 414
		// (get) Token: 0x06000431 RID: 1073 RVA: 0x00005FF1 File Offset: 0x000041F1
		// (set) Token: 0x06000432 RID: 1074 RVA: 0x00005FF9 File Offset: 0x000041F9
		public int TeamNo { get; set; }

		// Token: 0x1700019F RID: 415
		// (get) Token: 0x06000433 RID: 1075 RVA: 0x00006002 File Offset: 0x00004202
		// (set) Token: 0x06000434 RID: 1076 RVA: 0x0000600A File Offset: 0x0000420A
		public int SessionKey { get; set; }

		// Token: 0x170001A0 RID: 416
		// (get) Token: 0x06000435 RID: 1077 RVA: 0x00006013 File Offset: 0x00004213
		// (set) Token: 0x06000436 RID: 1078 RVA: 0x0000601B File Offset: 0x0000421B
		public string SceneName { get; set; }

		// Token: 0x170001A1 RID: 417
		// (get) Token: 0x06000437 RID: 1079 RVA: 0x00006024 File Offset: 0x00004224
		// (set) Token: 0x06000438 RID: 1080 RVA: 0x0000602C File Offset: 0x0000422C
		public string GameType { get; set; }
	}
}
