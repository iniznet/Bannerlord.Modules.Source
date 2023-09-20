using System;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class AddFriendByUsernameAndIdMessage : Message
	{
		public string Username { get; private set; }

		public int UserId { get; private set; }

		public bool DontUseNameForUnknownPlayer { get; }

		public AddFriendByUsernameAndIdMessage(string username, int userId, bool dontUseNameForUnknownPlayer)
		{
			this.Username = username;
			this.UserId = userId;
			this.DontUseNameForUnknownPlayer = dontUseNameForUnknownPlayer;
		}
	}
}
