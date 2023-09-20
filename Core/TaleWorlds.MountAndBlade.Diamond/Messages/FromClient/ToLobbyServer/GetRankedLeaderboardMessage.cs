using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class GetRankedLeaderboardMessage : Message
	{
		[JsonProperty]
		public string GameType { get; private set; }

		public GetRankedLeaderboardMessage()
		{
		}

		public GetRankedLeaderboardMessage(string gameType)
		{
			this.GameType = gameType;
		}
	}
}
