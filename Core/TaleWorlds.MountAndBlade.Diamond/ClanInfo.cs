using System;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x020000FF RID: 255
	[Serializable]
	public class ClanInfo
	{
		// Token: 0x170001BD RID: 445
		// (get) Token: 0x06000481 RID: 1153 RVA: 0x000067E9 File Offset: 0x000049E9
		// (set) Token: 0x06000482 RID: 1154 RVA: 0x000067F1 File Offset: 0x000049F1
		public Guid ClanId { get; private set; }

		// Token: 0x170001BE RID: 446
		// (get) Token: 0x06000483 RID: 1155 RVA: 0x000067FA File Offset: 0x000049FA
		// (set) Token: 0x06000484 RID: 1156 RVA: 0x00006802 File Offset: 0x00004A02
		public string Name { get; private set; }

		// Token: 0x170001BF RID: 447
		// (get) Token: 0x06000485 RID: 1157 RVA: 0x0000680B File Offset: 0x00004A0B
		// (set) Token: 0x06000486 RID: 1158 RVA: 0x00006813 File Offset: 0x00004A13
		public string Tag { get; private set; }

		// Token: 0x170001C0 RID: 448
		// (get) Token: 0x06000487 RID: 1159 RVA: 0x0000681C File Offset: 0x00004A1C
		// (set) Token: 0x06000488 RID: 1160 RVA: 0x00006824 File Offset: 0x00004A24
		public string Faction { get; private set; }

		// Token: 0x170001C1 RID: 449
		// (get) Token: 0x06000489 RID: 1161 RVA: 0x0000682D File Offset: 0x00004A2D
		// (set) Token: 0x0600048A RID: 1162 RVA: 0x00006835 File Offset: 0x00004A35
		public string Sigil { get; private set; }

		// Token: 0x170001C2 RID: 450
		// (get) Token: 0x0600048B RID: 1163 RVA: 0x0000683E File Offset: 0x00004A3E
		// (set) Token: 0x0600048C RID: 1164 RVA: 0x00006846 File Offset: 0x00004A46
		public string InformationText { get; private set; }

		// Token: 0x170001C3 RID: 451
		// (get) Token: 0x0600048D RID: 1165 RVA: 0x0000684F File Offset: 0x00004A4F
		// (set) Token: 0x0600048E RID: 1166 RVA: 0x00006857 File Offset: 0x00004A57
		public ClanPlayer[] Players { get; private set; }

		// Token: 0x170001C4 RID: 452
		// (get) Token: 0x0600048F RID: 1167 RVA: 0x00006860 File Offset: 0x00004A60
		// (set) Token: 0x06000490 RID: 1168 RVA: 0x00006868 File Offset: 0x00004A68
		public ClanAnnouncement[] Announcements { get; private set; }

		// Token: 0x06000491 RID: 1169 RVA: 0x00006874 File Offset: 0x00004A74
		public ClanInfo(Guid clanId, string name, string tag, string faction, string sigil, string information, ClanPlayer[] players, ClanAnnouncement[] announcements)
		{
			this.ClanId = clanId;
			this.Name = name;
			this.Tag = tag;
			this.Faction = faction;
			this.Sigil = sigil;
			this.Players = players;
			this.InformationText = information;
			this.Announcements = announcements;
		}

		// Token: 0x06000492 RID: 1170 RVA: 0x000068C4 File Offset: 0x00004AC4
		public static ClanInfo CreateUnavailableClanInfo()
		{
			return new ClanInfo(Guid.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, new ClanPlayer[0], new ClanAnnouncement[0]);
		}
	}
}
