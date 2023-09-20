using System;
using System.Collections.Generic;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromBattleServer.ToBattleServerManager
{
	[MessageDescription("BattleServer", "BattleServerManager")]
	[Serializable]
	public class BattleInitializedMessage : Message
	{
		public string GameType { get; }

		public List<PlayerId> AssignedPlayers { get; }

		public string Faction1 { get; }

		public string Faction2 { get; }

		public BattleInitializedMessage(string gameType, List<PlayerId> assignedPlayers, string faction1, string faction2)
		{
			this.GameType = gameType;
			this.AssignedPlayers = assignedPlayers;
			this.Faction1 = faction1;
			this.Faction2 = faction2;
		}
	}
}
