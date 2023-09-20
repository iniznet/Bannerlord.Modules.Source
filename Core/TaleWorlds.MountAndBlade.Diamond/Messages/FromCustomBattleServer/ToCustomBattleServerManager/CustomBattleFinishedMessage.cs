using System;
using System.Collections.Generic;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromCustomBattleServer.ToCustomBattleServerManager
{
	[MessageDescription("CustomBattleServer", "CustomBattleServerManager")]
	[Serializable]
	public class CustomBattleFinishedMessage : Message
	{
		public BattleResult BattleResult { get; }

		public Dictionary<int, int> TeamScores { get; }

		public Dictionary<PlayerId, int> PlayerScores { get; }

		public CustomBattleFinishedMessage(BattleResult battleResult, Dictionary<int, int> teamScores, Dictionary<PlayerId, int> playerScores)
		{
			this.BattleResult = battleResult;
			this.TeamScores = teamScores;
			this.PlayerScores = playerScores;
		}
	}
}
