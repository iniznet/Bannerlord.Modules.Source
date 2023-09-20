using System;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class InvitationToPartyMessage : Message
	{
		public string InviterPlayerName { get; private set; }

		public PlayerId InviterPlayerId { get; private set; }

		public InvitationToPartyMessage(string inviterPlayerName, PlayerId inviterPlayerId)
		{
			this.InviterPlayerName = inviterPlayerName;
			this.InviterPlayerId = inviterPlayerId;
		}
	}
}
