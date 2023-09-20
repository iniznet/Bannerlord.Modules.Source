using System;
using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade.Diamond
{
	[Serializable]
	public class BattleResult
	{
		public BattleResult()
		{
			this.PlayerEntries = new Dictionary<string, BattlePlayerEntry>();
			this.IsCancelled = false;
		}

		public void AddOrUpdatePlayerEntry(PlayerId playerId, int teamNo, string gameMode, Guid party)
		{
			BattlePlayerEntry battlePlayerEntry;
			if (this.PlayerEntries.TryGetValue(playerId.ToString(), out battlePlayerEntry))
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
				this.PlayerEntries.Add(playerId.ToString(), battlePlayerEntry);
			}
		}

		public bool TryGetPlayerEntry(PlayerId playerId, out BattlePlayerEntry battlePlayerEntry)
		{
			return this.PlayerEntries.TryGetValue(playerId.ToString(), out battlePlayerEntry);
		}

		public void HandlePlayerDisconnect(PlayerId playerId)
		{
			BattlePlayerEntry battlePlayerEntry;
			if (this.PlayerEntries.TryGetValue(playerId.ToString(), out battlePlayerEntry))
			{
				battlePlayerEntry.Disconnected = true;
				battlePlayerEntry.PlayTime += (int)(DateTime.Now - battlePlayerEntry.LastJoinTime).TotalSeconds;
			}
		}

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

		public void SetBattleCancelled()
		{
			this.IsCancelled = true;
		}

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

		public bool IsCancelled { get; private set; }

		public int WinnerTeamNo { get; private set; }

		public bool IsPremadeGame { get; private set; }

		public PremadeGameType PremadeGameType { get; private set; }

		public Dictionary<string, BattlePlayerEntry> PlayerEntries { get; private set; }
	}
}
