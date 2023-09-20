using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromCustomBattleServer.ToCustomBattleServerManager
{
	[MessageDescription("CustomBattleServer", "CustomBattleServerManager")]
	[Serializable]
	public class CustomBattleFinishedMessage : Message
	{
		[JsonProperty]
		public BattleResult BattleResult { get; private set; }

		[JsonProperty]
		public Dictionary<int, int> TeamScores { get; private set; }

		[JsonProperty]
		public Dictionary<string, int> PlayerScores { get; private set; }

		public CustomBattleFinishedMessage()
		{
		}

		public CustomBattleFinishedMessage(BattleResult battleResult, Dictionary<int, int> teamScores, Dictionary<PlayerId, int> playerScores)
		{
			this.BattleResult = battleResult;
			this.TeamScores = teamScores;
			this.PlayerScores = playerScores.ToDictionary((KeyValuePair<PlayerId, int> kvp) => kvp.Key.ToString(), (KeyValuePair<PlayerId, int> kvp) => kvp.Value);
		}
	}
}
