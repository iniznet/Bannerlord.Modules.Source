using System;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x0200013D RID: 317
	[Serializable]
	public class PlayerLeaderboardData
	{
		// Token: 0x170002B4 RID: 692
		// (get) Token: 0x060007C4 RID: 1988 RVA: 0x0000C91F File Offset: 0x0000AB1F
		// (set) Token: 0x060007C5 RID: 1989 RVA: 0x0000C927 File Offset: 0x0000AB27
		public PlayerId PlayerId { get; set; }

		// Token: 0x170002B5 RID: 693
		// (get) Token: 0x060007C6 RID: 1990 RVA: 0x0000C930 File Offset: 0x0000AB30
		// (set) Token: 0x060007C7 RID: 1991 RVA: 0x0000C938 File Offset: 0x0000AB38
		public string RankId { get; set; }

		// Token: 0x170002B6 RID: 694
		// (get) Token: 0x060007C8 RID: 1992 RVA: 0x0000C941 File Offset: 0x0000AB41
		// (set) Token: 0x060007C9 RID: 1993 RVA: 0x0000C949 File Offset: 0x0000AB49
		public int Rating { get; set; }

		// Token: 0x170002B7 RID: 695
		// (get) Token: 0x060007CA RID: 1994 RVA: 0x0000C952 File Offset: 0x0000AB52
		// (set) Token: 0x060007CB RID: 1995 RVA: 0x0000C95A File Offset: 0x0000AB5A
		public string Name { get; set; }

		// Token: 0x060007CC RID: 1996 RVA: 0x0000C963 File Offset: 0x0000AB63
		public PlayerLeaderboardData(PlayerId playerId, string rankId, int rating, string name)
		{
			this.PlayerId = playerId;
			this.RankId = rankId;
			this.Rating = rating;
			this.Name = name;
		}
	}
}
