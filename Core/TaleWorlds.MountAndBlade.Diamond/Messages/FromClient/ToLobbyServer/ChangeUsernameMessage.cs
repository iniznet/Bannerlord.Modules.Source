using System;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class ChangeUsernameMessage : Message
	{
		public string Username { get; }

		public ChangeUsernameMessage(string username)
		{
			this.Username = username;
		}
	}
}
