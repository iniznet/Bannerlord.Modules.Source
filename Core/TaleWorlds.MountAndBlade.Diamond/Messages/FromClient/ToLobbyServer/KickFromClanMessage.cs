using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class KickFromClanMessage : Message
	{
		[JsonProperty]
		public PlayerId PlayerId { get; private set; }

		public KickFromClanMessage()
		{
		}

		public KickFromClanMessage(PlayerId playerId)
		{
			this.PlayerId = playerId;
		}
	}
}
