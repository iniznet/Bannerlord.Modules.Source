using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class DeclineJoinPremadeGameRequestMessage : Message
	{
		[JsonProperty]
		public Guid PartyId { get; private set; }

		public DeclineJoinPremadeGameRequestMessage()
		{
		}

		public DeclineJoinPremadeGameRequestMessage(Guid partyId)
		{
			this.PartyId = partyId;
		}
	}
}
