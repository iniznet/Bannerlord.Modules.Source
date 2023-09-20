using System;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class DeclineJoinPremadeGameRequestMessage : Message
	{
		public Guid PartyId { get; private set; }

		public DeclineJoinPremadeGameRequestMessage(Guid partyId)
		{
			this.PartyId = partyId;
		}
	}
}
