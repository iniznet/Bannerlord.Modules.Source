using System;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class InvitedPlayerOnlineMessage : Message
	{
		public PlayerId PlayerId { get; private set; }

		public InvitedPlayerOnlineMessage(PlayerId playerId)
		{
			this.PlayerId = playerId;
		}
	}
}
