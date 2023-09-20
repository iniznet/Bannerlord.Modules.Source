using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class DeclinePartyJoinRequestMessage : Message
	{
		[JsonProperty]
		public PlayerId RequesterPlayerId { get; private set; }

		[JsonProperty]
		public PartyJoinDeclineReason Reason { get; private set; }

		public DeclinePartyJoinRequestMessage()
		{
		}

		public DeclinePartyJoinRequestMessage(PlayerId requesterPlayerId, PartyJoinDeclineReason reason)
		{
			this.RequesterPlayerId = requesterPlayerId;
			this.Reason = reason;
		}
	}
}
