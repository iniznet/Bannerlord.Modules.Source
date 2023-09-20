using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class RequestToJoinPremadeGameMessage : Message
	{
		[JsonProperty]
		public Guid GameId { get; private set; }

		[JsonProperty]
		public string Password { get; private set; }

		public RequestToJoinPremadeGameMessage()
		{
		}

		public RequestToJoinPremadeGameMessage(Guid gameId, string password)
		{
			this.GameId = gameId;
			this.Password = password;
		}
	}
}
