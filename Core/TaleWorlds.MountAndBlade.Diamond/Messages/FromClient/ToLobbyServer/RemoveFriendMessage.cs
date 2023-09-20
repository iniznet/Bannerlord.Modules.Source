using System;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class RemoveFriendMessage : Message
	{
		public PlayerId FriendId { get; private set; }

		public RemoveFriendMessage(PlayerId friendId)
		{
			this.FriendId = friendId;
		}
	}
}
