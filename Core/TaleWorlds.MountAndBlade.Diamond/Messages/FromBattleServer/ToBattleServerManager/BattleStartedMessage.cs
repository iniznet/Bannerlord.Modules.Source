using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromBattleServer.ToBattleServerManager
{
	[MessageDescription("BattleServer", "BattleServerManager")]
	[Serializable]
	public class BattleStartedMessage : Message
	{
		public Dictionary<string, int> PlayerTeams { get; set; }

		public BattleStartedMessage()
		{
		}

		public BattleStartedMessage(Dictionary<PlayerId, int> playerTeams)
		{
			this.PlayerTeams = playerTeams.ToDictionary((KeyValuePair<PlayerId, int> kvp) => kvp.Key.ToString(), (KeyValuePair<PlayerId, int> kvp) => kvp.Value);
		}
	}
}
