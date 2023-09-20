using System;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x02000101 RID: 257
	[Serializable]
	public class ClanLeaderboardEntry
	{
		// Token: 0x170001C7 RID: 455
		// (get) Token: 0x06000499 RID: 1177 RVA: 0x00006936 File Offset: 0x00004B36
		// (set) Token: 0x0600049A RID: 1178 RVA: 0x0000693E File Offset: 0x00004B3E
		public Guid ClanId { get; private set; }

		// Token: 0x170001C8 RID: 456
		// (get) Token: 0x0600049B RID: 1179 RVA: 0x00006947 File Offset: 0x00004B47
		// (set) Token: 0x0600049C RID: 1180 RVA: 0x0000694F File Offset: 0x00004B4F
		public string Name { get; private set; }

		// Token: 0x170001C9 RID: 457
		// (get) Token: 0x0600049D RID: 1181 RVA: 0x00006958 File Offset: 0x00004B58
		// (set) Token: 0x0600049E RID: 1182 RVA: 0x00006960 File Offset: 0x00004B60
		public string Tag { get; private set; }

		// Token: 0x170001CA RID: 458
		// (get) Token: 0x0600049F RID: 1183 RVA: 0x00006969 File Offset: 0x00004B69
		// (set) Token: 0x060004A0 RID: 1184 RVA: 0x00006971 File Offset: 0x00004B71
		public string Sigil { get; private set; }

		// Token: 0x170001CB RID: 459
		// (get) Token: 0x060004A1 RID: 1185 RVA: 0x0000697A File Offset: 0x00004B7A
		// (set) Token: 0x060004A2 RID: 1186 RVA: 0x00006982 File Offset: 0x00004B82
		public int WinCount { get; private set; }

		// Token: 0x170001CC RID: 460
		// (get) Token: 0x060004A3 RID: 1187 RVA: 0x0000698B File Offset: 0x00004B8B
		// (set) Token: 0x060004A4 RID: 1188 RVA: 0x00006993 File Offset: 0x00004B93
		public int LossCount { get; private set; }

		// Token: 0x170001CD RID: 461
		// (get) Token: 0x060004A5 RID: 1189 RVA: 0x0000699C File Offset: 0x00004B9C
		// (set) Token: 0x060004A6 RID: 1190 RVA: 0x000069A4 File Offset: 0x00004BA4
		public float Score { get; private set; }

		// Token: 0x060004A7 RID: 1191 RVA: 0x000069AD File Offset: 0x00004BAD
		public ClanLeaderboardEntry(Guid clanId, string name, string tag, string sigil, int winCount, int lossCount, float score)
		{
			this.ClanId = clanId;
			this.Name = name;
			this.Tag = tag;
			this.Sigil = sigil;
			this.WinCount = winCount;
			this.LossCount = lossCount;
			this.Score = score;
		}
	}
}
