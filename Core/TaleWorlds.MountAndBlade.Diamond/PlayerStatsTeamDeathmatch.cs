using System;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x02000145 RID: 325
	[Serializable]
	public class PlayerStatsTeamDeathmatch : PlayerStatsBase
	{
		// Token: 0x170002D9 RID: 729
		// (get) Token: 0x06000824 RID: 2084 RVA: 0x0000D0C5 File Offset: 0x0000B2C5
		// (set) Token: 0x06000825 RID: 2085 RVA: 0x0000D0CD File Offset: 0x0000B2CD
		public int Score { get; set; }

		// Token: 0x170002DA RID: 730
		// (get) Token: 0x06000826 RID: 2086 RVA: 0x0000D0D6 File Offset: 0x0000B2D6
		public float AverageScore
		{
			get
			{
				return (float)this.Score / (float)((base.WinCount + base.LoseCount != 0) ? (base.WinCount + base.LoseCount) : 1);
			}
		}

		// Token: 0x06000827 RID: 2087 RVA: 0x0000D100 File Offset: 0x0000B300
		public PlayerStatsTeamDeathmatch()
		{
			base.GameType = "TeamDeathmatch";
		}

		// Token: 0x06000828 RID: 2088 RVA: 0x0000D113 File Offset: 0x0000B313
		public void FillWith(PlayerId playerId, int killCount, int deathCount, int assistCount, int winCount, int loseCount, int forfeitCount, int score)
		{
			base.FillWith(playerId, killCount, deathCount, assistCount, winCount, loseCount, forfeitCount);
			this.Score = score;
		}

		// Token: 0x06000829 RID: 2089 RVA: 0x0000D130 File Offset: 0x0000B330
		public void FillWithNewPlayer(PlayerId playerId)
		{
			this.FillWith(playerId, 0, 0, 0, 0, 0, 0, 0);
		}

		// Token: 0x0600082A RID: 2090 RVA: 0x0000D14B File Offset: 0x0000B34B
		public void Update(BattlePlayerStatsTeamDeathmatch stats, bool won)
		{
			base.Update(stats, won);
			this.Score += stats.Score;
		}
	}
}
