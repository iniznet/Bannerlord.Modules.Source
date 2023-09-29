using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromCustomBattleServerManager.ToCustomBattleServer
{
	[MessageDescription("CustomBattleServerManager", "CustomBattleServer")]
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
