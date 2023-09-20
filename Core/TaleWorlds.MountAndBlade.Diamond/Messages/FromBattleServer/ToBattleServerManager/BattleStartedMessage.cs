using System;
using System.Collections.Generic;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromBattleServer.ToBattleServerManager
{
	[MessageDescription("BattleServer", "BattleServerManager")]
	[Serializable]
	public class BattleStartedMessage : Message
	{
		public Dictionary<PlayerId, int> PlayerTeams { get; set; }

		public BattleStartedMessage(Dictionary<PlayerId, int> playerTeams)
		{
			this.PlayerTeams = playerTeams;
		}
	}
}
