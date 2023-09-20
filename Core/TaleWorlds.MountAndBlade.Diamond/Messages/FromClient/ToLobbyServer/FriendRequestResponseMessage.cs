using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class FriendRequestResponseMessage : Message
	{
		[JsonProperty]
		public PlayerId PlayerId { get; private set; }

		[JsonProperty]
		public bool DontUseNameForUnknownPlayer { get; private set; }

		[JsonProperty]
		public bool IsAccepted { get; private set; }

		[JsonProperty]
		public bool IsBlocked { get; private set; }

		public FriendRequestResponseMessage()
		{
		}

		public FriendRequestResponseMessage(PlayerId playerId, bool dontUseNameForUnknownPlayer, bool isAccepted, bool isBlocked)
		{
			this.PlayerId = playerId;
			this.DontUseNameForUnknownPlayer = dontUseNameForUnknownPlayer;
			this.IsAccepted = isAccepted;
			this.IsBlocked = isBlocked;
		}
	}
}
