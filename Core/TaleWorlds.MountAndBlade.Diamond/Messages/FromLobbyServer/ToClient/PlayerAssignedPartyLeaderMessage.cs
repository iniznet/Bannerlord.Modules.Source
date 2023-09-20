using System;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class PlayerAssignedPartyLeaderMessage : Message
	{
		public PlayerId PartyLeaderId { get; private set; }

		public PlayerAssignedPartyLeaderMessage(PlayerId partyLeaderId)
		{
			this.PartyLeaderId = partyLeaderId;
		}
	}
}
