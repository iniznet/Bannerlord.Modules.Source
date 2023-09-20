using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class GetOtherPlayersStateMessage : Message
	{
		[JsonProperty]
		public List<PlayerId> Players { get; private set; }

		public GetOtherPlayersStateMessage()
		{
		}

		public GetOtherPlayersStateMessage(List<PlayerId> players)
		{
			this.Players = players;
		}
	}
}
