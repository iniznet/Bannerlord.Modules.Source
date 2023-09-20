using System;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class PromotePlayerToPartyLeaderMessage : Message
	{
		public PlayerId PromotedPlayerId { get; private set; }

		public PromotePlayerToPartyLeaderMessage(PlayerId promotedPlayerId)
		{
			this.PromotedPlayerId = promotedPlayerId;
		}
	}
}
