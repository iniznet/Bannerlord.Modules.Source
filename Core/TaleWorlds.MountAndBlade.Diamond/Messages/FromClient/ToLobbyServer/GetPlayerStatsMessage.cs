using System;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class GetPlayerStatsMessage : Message
	{
		public PlayerId PlayerId { get; private set; }

		public GetPlayerStatsMessage(PlayerId playerId)
		{
			this.PlayerId = playerId;
		}
	}
}
