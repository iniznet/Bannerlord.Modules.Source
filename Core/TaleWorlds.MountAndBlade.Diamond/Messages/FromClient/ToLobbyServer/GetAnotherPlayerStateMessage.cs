using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class GetAnotherPlayerStateMessage : Message
	{
		[JsonProperty]
		public PlayerId PlayerId { get; private set; }

		public GetAnotherPlayerStateMessage()
		{
		}

		public GetAnotherPlayerStateMessage(PlayerId playerId)
		{
			this.PlayerId = playerId;
		}
	}
}
