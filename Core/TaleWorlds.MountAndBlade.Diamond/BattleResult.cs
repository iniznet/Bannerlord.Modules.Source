using System;
using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Diamond
{
	// Token: 0x020000EE RID: 238
	[Serializable]
	public class BattleResult
	{
		// Token: 0x060003D0 RID: 976 RVA: 0x00004B1F File Offset: 0x00002D1F
		public BattleResult()
		{
			this.PlayerEntries = new Dictionary<PlayerId, BattlePlayerEntry>();
			this.IsCancelled = false;
		}

		// Token: 0x060003D1 RID: 977 RVA: 0x00004B3C File Offset: 0x00002D3C
		public void AddOrUpdatePlayerEntry(PlayerId playerId, int teamNo, string gameMode, Guid party)
		{
			BattlePlayerEntry battlePlayerEntry;
			if (this.PlayerEntries.TryGetValue(playerId, out battlePlayerEntry))
			{
				battlePlayerEntry.TeamNo = teamNo;
				battlePlayerEntry.Party = party;
				if (battlePlayerEntry.Disconnected)
				{
					battlePlayerEntry.Disconnected = false;
					battlePlayerEntry.LastJoinTime = DateTime.Now;
					return;
				}
			}
			else
			{
				BattlePlayerStatsBase battlePlayerStatsBase = this.CreatePlayerBattleStats(gameMode);
				battlePlayerEntry = new BattlePlayerEntry();
				battlePlayerEntry.PlayerId = playerId;
				battlePlayerEntry.TeamNo = teamNo;
				battlePlayerEntry.Party = party;
				battlePlayerEntry.GameType = gameMode;
				battlePlayerEntry.PlayerStats = battlePlayerStatsBase;
				battlePlayerEntry.LastJoinTime = DateTime.Now;
				battlePlayerEntry.PlayTime = 0;
				battlePlayerEntry.Disconnected = false;
				this.PlayerEntries.Add(playerId, battlePlayerEntry);
			}
		}

		// Token: 0x060003D2 RID: 978 RVA: 0x00004BDB File Offset: 0x00002DDB
		public bool TryGetPlayerEntry(PlayerId playerId, out BattlePlayerEntry battlePlayerEntry)
		{
			return this.PlayerEntries.TryGetValue(playerId, out battlePlayerEntry);
		}

		// Token: 0x060003D3 RID: 979 RVA: 0x00004BEC File Offset: 0x00002DEC
		public void HandlePlayerDisconnect(PlayerId playerId)
		{
			BattlePlayerEntry battlePlayerEntry;
			if (this.PlayerEntries.TryGetValue(playerId, out battlePlayerEntry))
			{
				battlePlayerEntry.Disconnected = true;
				battlePlayerEntry.PlayTime += (int)(DateTime.Now - battlePlayerEntry.LastJoinTime).TotalSeconds;
			}
		}

		// Token: 0x060003D4 RID: 980 RVA: 0x00004C38 File Offset: 0x00002E38
		public void DebugPrint()
		{
			Debug.Print("-----PRINTING BATTLE RESULT-----", 0, Debug.DebugColor.White, 17592186044416UL);
			foreach (BattlePlayerEntry battlePlayerEntry in this.PlayerEntries.Values)
			{
				Debug.Print("Player: " + battlePlayerEntry.PlayerId + "[DEBUG] ", 0, Debug.DebugColor.White, 17592186044416UL);
				Debug.Print("Kill: " + battlePlayerEntry.PlayerStats.Kills + "[DEBUG] ", 0, Debug.DebugColor.White, 17592186044416UL);
				Debug.Print("Death: " + battlePlayerEntry.PlayerStats.Deaths + "[DEBUG] ", 0, Debug.DebugColor.White, 17592186044416UL);
				Debug.Print("----", 0, Debug.DebugColor.White, 17592186044416UL);
			}
			Debug.Print("-----PRINTING OVER-----", 0, Debug.DebugColor.White, 17592186044416UL);
		}

		// Token: 0x060003D5 RID: 981 RVA: 0x00004D5C File Offset: 0x00002F5C
		public void SetBattleFinished(int winnerTeamNo, bool isPremadeGame, PremadeGameType premadeGameType)
		{
			this.WinnerTeamNo = winnerTeamNo;
			this.IsPremadeGame = isPremadeGame;
			this.PremadeGameType = premadeGameType;
			foreach (BattlePlayerEntry battlePlayerEntry in this.PlayerEntries.Values)
			{
				battlePlayerEntry.Won = battlePlayerEntry.TeamNo == winnerTeamNo;
				if (!battlePlayerEntry.Disconnected)
				{
					battlePlayerEntry.PlayTime += (int)(DateTime.Now - battlePlayerEntry.LastJoinTime).TotalSeconds;
				}
			}
		}

		// Token: 0x060003D6 RID: 982 RVA: 0x00004E00 File Offset: 0x00003000
		public void SetBattleCancelled()
		{
			this.IsCancelled = true;
		}

		// Token: 0x060003D7 RID: 983 RVA: 0x00004E0C File Offset: 0x0000300C
		private BattlePlayerStatsBase CreatePlayerBattleStats(string gameType)
		{
			if (gameType == "Skirmish")
			{
				return new BattlePlayerStatsSkirmish();
			}
			if (gameType == "Captain")
			{
				return new BattlePlayerStatsCaptain();
			}
			if (gameType == "Siege")
			{
				return new BattlePlayerStatsSiege();
			}
			if (gameType == "TeamDeathmatch")
			{
				return new BattlePlayerStatsTeamDeathmatch();
			}
			if (gameType == "Duel")
			{
				return new BattlePlayerStatsDuel();
			}
			if (gameType == "Battle")
			{
				return new BattlePlayerStatsBattle();
			}
			return new BattlePlayerStatsBase();
		}

		// Token: 0x17000184 RID: 388
		// (get) Token: 0x060003D8 RID: 984 RVA: 0x00004E90 File Offset: 0x00003090
		// (set) Token: 0x060003D9 RID: 985 RVA: 0x00004E98 File Offset: 0x00003098
		public bool IsCancelled { get; private set; }

		// Token: 0x17000185 RID: 389
		// (get) Token: 0x060003DA RID: 986 RVA: 0x00004EA1 File Offset: 0x000030A1
		// (set) Token: 0x060003DB RID: 987 RVA: 0x00004EA9 File Offset: 0x000030A9
		public int WinnerTeamNo { get; private set; }

		// Token: 0x17000186 RID: 390
		// (get) Token: 0x060003DC RID: 988 RVA: 0x00004EB2 File Offset: 0x000030B2
		// (set) Token: 0x060003DD RID: 989 RVA: 0x00004EBA File Offset: 0x000030BA
		public bool IsPremadeGame { get; private set; }

		// Token: 0x17000187 RID: 391
		// (get) Token: 0x060003DE RID: 990 RVA: 0x00004EC3 File Offset: 0x000030C3
		// (set) Token: 0x060003DF RID: 991 RVA: 0x00004ECB File Offset: 0x000030CB
		public PremadeGameType PremadeGameType { get; private set; }

		// Token: 0x17000188 RID: 392
		// (get) Token: 0x060003E0 RID: 992 RVA: 0x00004ED4 File Offset: 0x000030D4
		// (set) Token: 0x060003E1 RID: 993 RVA: 0x00004EDC File Offset: 0x000030DC
		public Dictionary<PlayerId, BattlePlayerEntry> PlayerEntries { get; private set; }
	}
}
