using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class RecentPlayerStatusesMessage : Message
	{
		public FriendInfo[] Friends { get; private set; }

		public RecentPlayerStatusesMessage(FriendInfo[] friends)
		{
			this.Friends = friends;
		}
	}
}
