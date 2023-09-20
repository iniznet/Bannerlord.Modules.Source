using System;
using System.Collections.Generic;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class GetOtherPlayersStateMessage : Message
	{
		public List<PlayerId> Players { get; }

		public GetOtherPlayersStateMessage(List<PlayerId> players)
		{
			this.Players = players;
		}
	}
}
