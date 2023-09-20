using System;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class InviteToClanMessage : Message
	{
		public PlayerId InvitedPlayerId { get; private set; }

		public bool DontUseNameForUnknownPlayer { get; private set; }

		public InviteToClanMessage(PlayerId invitedPlayerId, bool dontUseNameForUnknownPlayer)
		{
			this.InvitedPlayerId = invitedPlayerId;
			this.DontUseNameForUnknownPlayer = dontUseNameForUnknownPlayer;
		}
	}
}
