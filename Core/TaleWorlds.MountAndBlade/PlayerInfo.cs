using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000322 RID: 802
	public class PlayerInfo
	{
		// Token: 0x170007BC RID: 1980
		// (get) Token: 0x06002B74 RID: 11124 RVA: 0x000A9AA5 File Offset: 0x000A7CA5
		// (set) Token: 0x06002B75 RID: 11125 RVA: 0x000A9AAD File Offset: 0x000A7CAD
		public string PlayerId { get; set; }

		// Token: 0x170007BD RID: 1981
		// (get) Token: 0x06002B76 RID: 11126 RVA: 0x000A9AB6 File Offset: 0x000A7CB6
		// (set) Token: 0x06002B77 RID: 11127 RVA: 0x000A9ABE File Offset: 0x000A7CBE
		public string Username { get; set; }

		// Token: 0x170007BE RID: 1982
		// (get) Token: 0x06002B78 RID: 11128 RVA: 0x000A9AC7 File Offset: 0x000A7CC7
		// (set) Token: 0x06002B79 RID: 11129 RVA: 0x000A9ACF File Offset: 0x000A7CCF
		public int ForcedIndex { get; set; }

		// Token: 0x170007BF RID: 1983
		// (get) Token: 0x06002B7A RID: 11130 RVA: 0x000A9AD8 File Offset: 0x000A7CD8
		// (set) Token: 0x06002B7B RID: 11131 RVA: 0x000A9AE0 File Offset: 0x000A7CE0
		public int TeamNo { get; set; }

		// Token: 0x170007C0 RID: 1984
		// (get) Token: 0x06002B7C RID: 11132 RVA: 0x000A9AE9 File Offset: 0x000A7CE9
		// (set) Token: 0x06002B7D RID: 11133 RVA: 0x000A9AF1 File Offset: 0x000A7CF1
		public int Kill { get; set; }

		// Token: 0x170007C1 RID: 1985
		// (get) Token: 0x06002B7E RID: 11134 RVA: 0x000A9AFA File Offset: 0x000A7CFA
		// (set) Token: 0x06002B7F RID: 11135 RVA: 0x000A9B02 File Offset: 0x000A7D02
		public int Death { get; set; }

		// Token: 0x170007C2 RID: 1986
		// (get) Token: 0x06002B80 RID: 11136 RVA: 0x000A9B0B File Offset: 0x000A7D0B
		// (set) Token: 0x06002B81 RID: 11137 RVA: 0x000A9B13 File Offset: 0x000A7D13
		public int Assist { get; set; }
	}
}
