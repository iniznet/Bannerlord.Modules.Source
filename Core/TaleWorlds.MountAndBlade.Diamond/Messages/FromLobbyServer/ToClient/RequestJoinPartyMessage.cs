using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class RequestJoinPartyMessage : Message
	{
		[JsonProperty]
		public PlayerId PlayerId { get; private set; }

		[JsonProperty]
		public string PlayerName { get; private set; }

		[JsonProperty]
		public PlayerId ViaPlayerId { get; private set; }

		[JsonProperty]
		public string ViaPlayerName { get; private set; }

		public RequestJoinPartyMessage()
		{
		}

		public RequestJoinPartyMessage(PlayerId playerId, string playerName, PlayerId viaPlayerId, string viaPlayerName)
		{
			this.PlayerId = playerId;
			this.PlayerName = playerName;
			this.ViaPlayerId = viaPlayerId;
			this.ViaPlayerName = viaPlayerName;
		}
	}
}
