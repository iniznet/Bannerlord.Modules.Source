using System;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x02000124 RID: 292
	[Serializable]
	public class PlayerJoinGameResponseDataFromHost
	{
		// Token: 0x17000250 RID: 592
		// (get) Token: 0x06000691 RID: 1681 RVA: 0x0000B083 File Offset: 0x00009283
		// (set) Token: 0x06000692 RID: 1682 RVA: 0x0000B08B File Offset: 0x0000928B
		public PlayerId PlayerId { get; set; }

		// Token: 0x17000251 RID: 593
		// (get) Token: 0x06000693 RID: 1683 RVA: 0x0000B094 File Offset: 0x00009294
		// (set) Token: 0x06000694 RID: 1684 RVA: 0x0000B09C File Offset: 0x0000929C
		public int PeerIndex { get; set; }

		// Token: 0x17000252 RID: 594
		// (get) Token: 0x06000695 RID: 1685 RVA: 0x0000B0A5 File Offset: 0x000092A5
		// (set) Token: 0x06000696 RID: 1686 RVA: 0x0000B0AD File Offset: 0x000092AD
		public int SessionKey { get; set; }

		// Token: 0x17000253 RID: 595
		// (get) Token: 0x06000697 RID: 1687 RVA: 0x0000B0B6 File Offset: 0x000092B6
		// (set) Token: 0x06000698 RID: 1688 RVA: 0x0000B0BE File Offset: 0x000092BE
		public bool IsAdmin { get; set; }

		// Token: 0x17000254 RID: 596
		// (get) Token: 0x06000699 RID: 1689 RVA: 0x0000B0C7 File Offset: 0x000092C7
		// (set) Token: 0x0600069A RID: 1690 RVA: 0x0000B0CF File Offset: 0x000092CF
		public CustomGameJoinResponse CustomGameJoinResponse { get; set; }
	}
}
