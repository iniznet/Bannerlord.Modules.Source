using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class RequestJoinPlayerPartyMessage : Message
	{
		[JsonProperty]
		public PlayerId TargetPlayer { get; private set; }

		[JsonProperty]
		public bool InviteRequest { get; private set; }

		public RequestJoinPlayerPartyMessage()
		{
		}

		public RequestJoinPlayerPartyMessage(PlayerId targetPlayer, bool inviteRequest)
		{
			this.TargetPlayer = targetPlayer;
			this.InviteRequest = inviteRequest;
		}
	}
}
