using System;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x02000143 RID: 323
	[Serializable]
	public class PlayerStatsSiege : PlayerStatsBase
	{
		// Token: 0x170002CF RID: 719
		// (get) Token: 0x0600080B RID: 2059 RVA: 0x0000CE50 File Offset: 0x0000B050
		// (set) Token: 0x0600080C RID: 2060 RVA: 0x0000CE58 File Offset: 0x0000B058
		public int WallsBreached { get; set; }

		// Token: 0x170002D0 RID: 720
		// (get) Token: 0x0600080D RID: 2061 RVA: 0x0000CE61 File Offset: 0x0000B061
		// (set) Token: 0x0600080E RID: 2062 RVA: 0x0000CE69 File Offset: 0x0000B069
		public int SiegeEngineKills { get; set; }

		// Token: 0x170002D1 RID: 721
		// (get) Token: 0x0600080F RID: 2063 RVA: 0x0000CE72 File Offset: 0x0000B072
		// (set) Token: 0x06000810 RID: 2064 RVA: 0x0000CE7A File Offset: 0x0000B07A
		public int SiegeEnginesDestroyed { get; set; }

		// Token: 0x170002D2 RID: 722
		// (get) Token: 0x06000811 RID: 2065 RVA: 0x0000CE83 File Offset: 0x0000B083
		// (set) Token: 0x06000812 RID: 2066 RVA: 0x0000CE8B File Offset: 0x0000B08B
		public int ObjectiveGoldGained { get; set; }

		// Token: 0x170002D3 RID: 723
		// (get) Token: 0x06000813 RID: 2067 RVA: 0x0000CE94 File Offset: 0x0000B094
		// (set) Token: 0x06000814 RID: 2068 RVA: 0x0000CE9C File Offset: 0x0000B09C
		public int Score { get; set; }

		// Token: 0x170002D4 RID: 724
		// (get) Token: 0x06000815 RID: 2069 RVA: 0x0000CEA5 File Offset: 0x0000B0A5
		public int AverageScore
		{
			get
			{
				return this.Score / ((base.WinCount + base.LoseCount != 0) ? (base.WinCount + base.LoseCount) : 1);
			}
		}

		// Token: 0x170002D5 RID: 725
		// (get) Token: 0x06000816 RID: 2070 RVA: 0x0000CECD File Offset: 0x0000B0CD
		public int AverageKillCount
		{
			get
			{
				return base.KillCount / ((base.WinCount + base.LoseCount != 0) ? (base.WinCount + base.LoseCount) : 1);
			}
		}

		// Token: 0x06000817 RID: 2071 RVA: 0x0000CEF5 File Offset: 0x0000B0F5
		public PlayerStatsSiege()
		{
			base.GameType = "Siege";
		}

		// Token: 0x06000818 RID: 2072 RVA: 0x0000CF08 File Offset: 0x0000B108
		public void FillWith(PlayerId playerId, int killCount, int deathCount, int assistCount, int winCount, int loseCount, int forfeitCount, int wallsBreached, int siegeEngineKills, int siegeEnginesDestroyed, int objectiveGoldGained, int score)
		{
			base.FillWith(playerId, killCount, deathCount, assistCount, winCount, loseCount, forfeitCount);
			this.WallsBreached = wallsBreached;
			this.SiegeEngineKills = siegeEngineKills;
			this.SiegeEnginesDestroyed = siegeEnginesDestroyed;
			this.ObjectiveGoldGained = objectiveGoldGained;
			this.Score = score;
		}

		// Token: 0x06000819 RID: 2073 RVA: 0x0000CF44 File Offset: 0x0000B144
		public void FillWithNewPlayer(PlayerId playerId)
		{
			this.FillWith(playerId, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
		}

		// Token: 0x0600081A RID: 2074 RVA: 0x0000CF64 File Offset: 0x0000B164
		public void Update(BattlePlayerStatsSiege stats, bool won)
		{
			base.Update(stats, won);
			this.WallsBreached += stats.WallsBreached;
			this.SiegeEngineKills += stats.SiegeEngineKills;
			this.SiegeEnginesDestroyed += stats.SiegeEnginesDestroyed;
			this.ObjectiveGoldGained += stats.ObjectiveGoldGained;
			this.Score += stats.Score;
		}
	}
}
