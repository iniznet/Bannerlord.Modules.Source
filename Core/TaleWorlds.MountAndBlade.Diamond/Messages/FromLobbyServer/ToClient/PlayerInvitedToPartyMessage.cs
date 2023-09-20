using System;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class PlayerInvitedToPartyMessage : Message
	{
		public PlayerId PlayerId { get; private set; }

		public string PlayerName { get; private set; }

		public PlayerInvitedToPartyMessage(PlayerId playerId, string playerName)
		{
			this.PlayerId = playerId;
			this.PlayerName = playerName;
		}
	}
}
