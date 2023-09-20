using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[Serializable]
	public class JoinCustomGameResultMessage : Message
	{
		[JsonProperty]
		public JoinGameData JoinGameData { get; private set; }

		[JsonProperty]
		public bool Success { get; private set; }

		[JsonProperty]
		public CustomGameJoinResponse Response { get; private set; }

		[JsonProperty]
		public string MatchId { get; private set; }

		public JoinCustomGameResultMessage()
		{
		}

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
