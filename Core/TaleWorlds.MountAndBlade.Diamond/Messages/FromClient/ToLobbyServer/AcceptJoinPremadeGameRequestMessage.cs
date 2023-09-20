using System;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class AcceptJoinPremadeGameRequestMessage : Message
	{
		public Guid PartyId { get; private set; }

		public AcceptJoinPremadeGameRequestMessage(Guid partyId)
		{
			this.PartyId = partyId;
		}
	}
}
