using System;
using System.Collections.Generic;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromCustomBattleServerManager.ToCustomBattleServer
{
	[MessageDescription("CustomBattleServer", "CustomBattleServerManager")]
	[Serializable]
	public class CustomBattleServerStatsUpdateMessage : Message
	{
		public BattleResult BattleResult { get; set; }

		public Dictionary<int, int> TeamScores { get; set; }

		public Dictionary<PlayerId, int> PlayerScores { get; set; }

		public CustomBattleServerStatsUpdateMessage(BattleResult battleResult, Dictionary<int, int> teamScores, Dictionary<PlayerId, int> playerScores)
		{
			this.BattleResult = battleResult;
			this.TeamScores = teamScores;
			this.PlayerScores = playerScores;
		}
	}
}
