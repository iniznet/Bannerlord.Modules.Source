using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class ChangeUsernameMessage : Message
	{
		[JsonProperty]
		public string Username { get; private set; }

		public ChangeUsernameMessage()
		{
		}

		public ChangeUsernameMessage(string username)
		{
			this.Username = username;
		}
	}
}
