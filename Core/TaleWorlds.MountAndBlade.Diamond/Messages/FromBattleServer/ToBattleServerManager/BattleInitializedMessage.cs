using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromBattleServer.ToBattleServerManager
{
	[MessageDescription("BattleServer", "BattleServerManager")]
	[Serializable]
	public class BattleInitializedMessage : Message
	{
		[JsonProperty]
		public string GameType { get; }

		[JsonProperty]
		public List<PlayerId> AssignedPlayers { get; }

		[JsonProperty]
		public string Faction1 { get; }

		[JsonProperty]
		public string Faction2 { get; }

		public BattleInitializedMessage()
		{
		}

		public BattleInitializedMessage(string gameType, List<PlayerId> assignedPlayers, string faction1, string faction2)
		{
			this.GameType = gameType;
			this.AssignedPlayers = assignedPlayers;
			this.Faction1 = faction1;
			this.Faction2 = faction2;
		}
	}
}
