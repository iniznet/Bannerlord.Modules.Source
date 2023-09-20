using System;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class GetPlayerByUsernameAndIdMessage : Message
	{
		public string Username { get; private set; }

		public int UserId { get; private set; }

		public GetPlayerByUsernameAndIdMessage(string username, int userId)
		{
			this.Username = username;
			this.UserId = userId;
		}
	}
}
