using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[Serializable]
	public class JoinCustomGameResultMessage : Message
	{
		public JoinGameData JoinGameData { get; private set; }

		public bool Success { get; private set; }

		public CustomGameJoinResponse Response { get; private set; }

		public string MatchId { get; private set; }

		private JoinCustomGameResultMessage(JoinGameData joinGameData, bool success, CustomGameJoinResponse response, string matchId)
		{
			this.JoinGameData = joinGameData;
			this.Success = success;
			this.Response = response;
			this.MatchId = matchId;
		}

		public static JoinCustomGameResultMessage CreateSuccess(JoinGameData joinGameData, string matchId)
		{
			return new JoinCustomGameResultMessage(joinGameData, true, CustomGameJoinResponse.Success, matchId);
		}

		public static JoinCustomGameResultMessage CreateFailed(CustomGameJoinResponse response)
		{
			return new JoinCustomGameResultMessage(null, false, response, null);
		}
	}
}
