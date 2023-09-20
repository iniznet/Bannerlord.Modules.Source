using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class InviteToPartyMessage : Message
	{
		[JsonProperty]
		public PlayerId InvitedPlayerId { get; private set; }

		[JsonProperty]
		public bool DontUseNameForUnknownPlayer { get; private set; }

		public InviteToPartyMessage()
		{
		}

		public InviteToPartyMessage(PlayerId invitedPlayerId, bool dontUseNameForUnknownPlayer)
		{
			this.InvitedPlayerId = invitedPlayerId;
			this.DontUseNameForUnknownPlayer = dontUseNameForUnknownPlayer;
		}
	}
}
