using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class GetPlayerStatsMessage : Message
	{
		[JsonProperty]
		public PlayerId PlayerId { get; private set; }

		public GetPlayerStatsMessage()
		{
		}

		public GetPlayerStatsMessage(PlayerId playerId)
		{
			this.PlayerId = playerId;
		}
	}
}
