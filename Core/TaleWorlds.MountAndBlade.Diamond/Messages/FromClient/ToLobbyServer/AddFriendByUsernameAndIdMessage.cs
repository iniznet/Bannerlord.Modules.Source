using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class AddFriendByUsernameAndIdMessage : Message
	{
		[JsonProperty]
		public string Username { get; private set; }

		[JsonProperty]
		public int UserId { get; private set; }

		[JsonProperty]
		public bool DontUseNameForUnknownPlayer { get; private set; }

		public AddFriendByUsernameAndIdMessage()
		{
		}

		public AddFriendByUsernameAndIdMessage(string username, int userId, bool dontUseNameForUnknownPlayer)
		{
			this.Username = username;
			this.UserId = userId;
			this.DontUseNameForUnknownPlayer = dontUseNameForUnknownPlayer;
		}
	}
}
