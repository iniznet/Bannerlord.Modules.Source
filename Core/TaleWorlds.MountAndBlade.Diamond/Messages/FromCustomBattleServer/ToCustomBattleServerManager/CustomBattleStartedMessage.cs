using System;
using System.Collections.Generic;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromCustomBattleServer.ToCustomBattleServerManager
{
	[MessageDescription("CustomBattleServer", "CustomBattleServerManager")]
	[Serializable]
	public class CustomBattleStartedMessage : Message
	{
		public string GameType { get; set; }

		public Dictionary<PlayerId, int> PlayerTeams { get; set; }

		public List<string> FactionNames { get; set; }

		public CustomBattleStartedMessage(string gameType, Dictionary<PlayerId, int> playerTeams, List<string> factionNames)
		{
			this.GameType = gameType;
			this.PlayerTeams = playerTeams;
			this.FactionNames = factionNames;
		}
	}
}
