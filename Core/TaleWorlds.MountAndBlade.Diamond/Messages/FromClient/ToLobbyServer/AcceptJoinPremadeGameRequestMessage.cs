using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class AcceptJoinPremadeGameRequestMessage : Message
	{
		[JsonProperty]
		public Guid PartyId { get; private set; }

		public AcceptJoinPremadeGameRequestMessage()
		{
		}

		public AcceptJoinPremadeGameRequestMessage(Guid partyId)
		{
			this.PartyId = partyId;
		}
	}
}
