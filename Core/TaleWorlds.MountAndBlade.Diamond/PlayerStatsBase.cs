using System;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x0200013E RID: 318
	[Serializable]
	public class PlayerStatsBase
	{
		// Token: 0x170002B8 RID: 696
		// (get) Token: 0x060007CD RID: 1997 RVA: 0x0000C988 File Offset: 0x0000AB88
		// (set) Token: 0x060007CE RID: 1998 RVA: 0x0000C990 File Offset: 0x0000AB90
		public PlayerId PlayerId { get; private set; }

		// Token: 0x170002B9 RID: 697
		// (get) Token: 0x060007CF RID: 1999 RVA: 0x0000C999 File Offset: 0x0000AB99
		// (set) Token: 0x060007D0 RID: 2000 RVA: 0x0000C9A1 File Offset: 0x0000ABA1
		public int KillCount { get; set; }

		// Token: 0x170002BA RID: 698
		// (get) Token: 0x060007D1 RID: 2001 RVA: 0x0000C9AA File Offset: 0x0000ABAA
		// (set) Token: 0x060007D2 RID: 2002 RVA: 0x0000C9B2 File Offset: 0x0000ABB2
		public int DeathCount { get; set; }

		// Token: 0x170002BB RID: 699
		// (get) Token: 0x060007D3 RID: 2003 RVA: 0x0000C9BB File Offset: 0x0000ABBB
		// (set) Token: 0x060007D4 RID: 2004 RVA: 0x0000C9C3 File Offset: 0x0000ABC3
		public int AssistCount { get; set; }

		// Token: 0x170002BC RID: 700
		// (get) Token: 0x060007D5 RID: 2005 RVA: 0x0000C9CC File Offset: 0x0000ABCC
		// (set) Token: 0x060007D6 RID: 2006 RVA: 0x0000C9D4 File Offset: 0x0000ABD4
		public int WinCount { get; set; }

		// Token: 0x170002BD RID: 701
		// (get) Token: 0x060007D7 RID: 2007 RVA: 0x0000C9DD File Offset: 0x0000ABDD
		// (set) Token: 0x060007D8 RID: 2008 RVA: 0x0000C9E5 File Offset: 0x0000ABE5
		public int LoseCount { get; set; }

		// Token: 0x170002BE RID: 702
		// (get) Token: 0x060007D9 RID: 2009 RVA: 0x0000C9EE File Offset: 0x0000ABEE
		// (set) Token: 0x060007DA RID: 2010 RVA: 0x0000C9F6 File Offset: 0x0000ABF6
		public int ForfeitCount { get; set; }

		// Token: 0x170002BF RID: 703
		// (get) Token: 0x060007DB RID: 2011 RVA: 0x0000C9FF File Offset: 0x0000ABFF
		public float AverageKillPerDeath
		{
			get
			{
				return (float)this.KillCount / (float)((this.DeathCount != 0) ? this.DeathCount : 1);
			}
		}

		// Token: 0x170002C0 RID: 704
		// (get) Token: 0x060007DC RID: 2012 RVA: 0x0000CA1B File Offset: 0x0000AC1B
		// (set) Token: 0x060007DD RID: 2013 RVA: 0x0000CA23 File Offset: 0x0000AC23
		public string GameType { get; set; }

		// Token: 0x060007DF RID: 2015 RVA: 0x0000CA34 File Offset: 0x0000AC34
		public void FillWith(PlayerId playerId, int killCount, int deathCount, int assistCount, int winCount, int loseCount, int forfeitCount)
		{
			this.PlayerId = playerId;
			this.KillCount = killCount;
			this.DeathCount = deathCount;
			this.AssistCount = assistCount;
			this.WinCount = winCount;
			this.LoseCount = loseCount;
			this.ForfeitCount = forfeitCount;
		}

		// Token: 0x060007E0 RID: 2016 RVA: 0x0000CA6C File Offset: 0x0000AC6C
		public virtual void Update(BattlePlayerStatsBase battleStats, bool won)
		{
			this.KillCount += battleStats.Kills;
			this.DeathCount += battleStats.Deaths;
			this.AssistCount += battleStats.Assists;
			int num;
			if (won)
			{
				num = this.WinCount;
				this.WinCount = num + 1;
				return;
			}
			num = this.LoseCount;
			this.LoseCount = num + 1;
		}
	}
}
