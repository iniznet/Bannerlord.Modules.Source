using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class GetRecentPlayersStatusMessage : Message
	{
		[JsonProperty]
		public PlayerId[] RecentPlayers { get; private set; }

		public GetRecentPlayersStatusMessage()
		{
		}

		public GetRecentPlayersStatusMessage(PlayerId[] recentPlayers)
		{
			this.RecentPlayers = recentPlayers;
		}
	}
}
