using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class GetPlayerByUsernameAndIdMessage : Message
	{
		[JsonProperty]
		public string Username { get; private set; }

		[JsonProperty]
		public int UserId { get; private set; }

		public GetPlayerByUsernameAndIdMessage()
		{
		}

		public GetPlayerByUsernameAndIdMessage(string username, int userId)
		{
			this.Username = username;
			this.UserId = userId;
		}
	}
}
