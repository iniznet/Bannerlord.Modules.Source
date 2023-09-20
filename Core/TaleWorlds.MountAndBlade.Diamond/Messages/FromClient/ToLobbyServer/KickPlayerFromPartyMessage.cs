using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class KickPlayerFromPartyMessage : Message
	{
		[JsonProperty]
		public PlayerId KickedPlayerId { get; private set; }

		public KickPlayerFromPartyMessage()
		{
		}

		public KickPlayerFromPartyMessage(PlayerId kickedPlayerId)
		{
			this.KickedPlayerId = kickedPlayerId;
		}
	}
}
