using System;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class FriendRequestResponseMessage : Message
	{
		public PlayerId PlayerId { get; private set; }

		public bool DontUseNameForUnknownPlayer { get; private set; }

		public bool IsAccepted { get; private set; }

		public bool IsBlocked { get; private set; }

		public FriendRequestResponseMessage(PlayerId playerId, bool dontUseNameForUnknownPlayer, bool isAccepted, bool isBlocked)
		{
			this.PlayerId = playerId;
			this.DontUseNameForUnknownPlayer = dontUseNameForUnknownPlayer;
			this.IsAccepted = isAccepted;
			this.IsBlocked = isBlocked;
		}
	}
}
