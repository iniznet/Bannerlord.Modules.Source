using System;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class KickPlayerFromPartyMessage : Message
	{
		public PlayerId KickedPlayerId { get; private set; }

		public KickPlayerFromPartyMessage(PlayerId kickedPlayerId)
		{
			this.KickedPlayerId = kickedPlayerId;
		}
	}
}
