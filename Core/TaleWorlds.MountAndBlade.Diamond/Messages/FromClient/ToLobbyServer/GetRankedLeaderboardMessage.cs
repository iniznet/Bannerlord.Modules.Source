using System;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class GetRankedLeaderboardMessage : Message
	{
		public string GameType { get; private set; }

		public GetRankedLeaderboardMessage(string gameType)
		{
			this.GameType = gameType;
		}
	}
}
