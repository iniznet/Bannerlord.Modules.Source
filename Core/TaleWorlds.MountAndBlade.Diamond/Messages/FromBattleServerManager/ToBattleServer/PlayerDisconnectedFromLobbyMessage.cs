using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromBattleServerManager.ToBattleServer
{
	[MessageDescription("BattleServerManager", "BattleServer")]
	[Serializable]
	public class PlayerDisconnectedFromLobbyMessage : Message
	{
		[JsonProperty]
		public PlayerId PlayerId { get; private set; }

		public PlayerDisconnectedFromLobbyMessage()
		{
		}

		public PlayerDisconnectedFromLobbyMessage(PlayerId playerId)
		{
			this.PlayerId = playerId;
		}
	}
}
