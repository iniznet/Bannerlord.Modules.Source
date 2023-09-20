using System;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x020000F5 RID: 245
	[Serializable]
	public class ClanAnnouncement
	{
		// Token: 0x170001A7 RID: 423
		// (get) Token: 0x0600044B RID: 1099 RVA: 0x00006521 File Offset: 0x00004721
		// (set) Token: 0x0600044C RID: 1100 RVA: 0x00006529 File Offset: 0x00004729
		public int Id { get; private set; }

		// Token: 0x170001A8 RID: 424
		// (get) Token: 0x0600044D RID: 1101 RVA: 0x00006532 File Offset: 0x00004732
		// (set) Token: 0x0600044E RID: 1102 RVA: 0x0000653A File Offset: 0x0000473A
		public string Announcement { get; private set; }

		// Token: 0x170001A9 RID: 425
		// (get) Token: 0x0600044F RID: 1103 RVA: 0x00006543 File Offset: 0x00004743
		// (set) Token: 0x06000450 RID: 1104 RVA: 0x0000654B File Offset: 0x0000474B
		public PlayerId AuthorId { get; private set; }

		// Token: 0x170001AA RID: 426
		// (get) Token: 0x06000451 RID: 1105 RVA: 0x00006554 File Offset: 0x00004754
		// (set) Token: 0x06000452 RID: 1106 RVA: 0x0000655C File Offset: 0x0000475C
		public DateTime CreationTime { get; private set; }

		// Token: 0x06000453 RID: 1107 RVA: 0x00006565 File Offset: 0x00004765
		public ClanAnnouncement(int id, string announcement, PlayerId authorId, DateTime creationTime)
		{
			this.Id = id;
			this.Announcement = announcement;
			this.AuthorId = authorId;
			this.CreationTime = creationTime;
		}
	}
}
