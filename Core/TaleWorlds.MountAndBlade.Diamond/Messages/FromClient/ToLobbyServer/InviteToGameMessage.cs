using System;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class InviteToGameMessage : Message
	{
		public PlayerId InvitedPlayerId { get; private set; }

		public InviteToGameMessage(PlayerId invitedPlayerId)
		{
			this.InvitedPlayerId = invitedPlayerId;
		}
	}
}
