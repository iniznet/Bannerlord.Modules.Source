using System;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class GetRecentPlayersStatusMessage : Message
	{
		public PlayerId[] RecentPlayers { get; private set; }

		public GetRecentPlayersStatusMessage(PlayerId[] recentPlayers)
		{
			this.RecentPlayers = recentPlayers;
		}
	}
}
