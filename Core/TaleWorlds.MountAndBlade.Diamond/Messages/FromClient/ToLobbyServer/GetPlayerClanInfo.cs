using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class GetPlayerClanInfo : Message
	{
		[JsonProperty]
		public PlayerId PlayerId { get; private set; }

		public GetPlayerClanInfo()
		{
		}

		public GetPlayerClanInfo(PlayerId playerId)
		{
			this.PlayerId = playerId;
		}
	}
}
