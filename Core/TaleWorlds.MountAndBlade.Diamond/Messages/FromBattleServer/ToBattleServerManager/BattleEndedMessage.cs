using System;
using System.Collections.Generic;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromBattleServer.ToBattleServerManager
{
	[MessageDescription("BattleServer", "BattleServerManager")]
	[Serializable]
	public class BattleEndedMessage : Message
	{
		public BattleResult BattleResult { get; }

		public GameLog[] GameLogs { get; }

		public Dictionary<ValueTuple<PlayerId, string, string>, int> BadgeDataDictionary { get; }

		public Dictionary<int, int> TeamScores { get; }

		public Dictionary<PlayerId, int> PlayerScores { get; }

		public int GameTime { get; }

		public BattleEndedMessage(BattleResult battleResult, GameLog[] gameLogs, Dictionary<ValueTuple<PlayerId, string, string>, int> badgeDataDictionary, int gameTime, Dictionary<int, int> teamScores, Dictionary<PlayerId, int> playerScores)
		{
			this.BattleResult = battleResult;
			this.GameLogs = gameLogs;
			this.BadgeDataDictionary = badgeDataDictionary;
			this.TeamScores = teamScores;
			this.PlayerScores = playerScores;
			this.GameTime = gameTime;
		}
	}
}
