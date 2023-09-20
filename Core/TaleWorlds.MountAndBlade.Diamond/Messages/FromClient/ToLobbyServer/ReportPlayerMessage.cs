using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class ReportPlayerMessage : Message
	{
		[JsonProperty]
		public Guid GameId { get; private set; }

		[JsonProperty]
		public PlayerId ReportedPlayerId { get; private set; }

		[JsonProperty]
		public string ReportedPlayerName { get; private set; }

		[JsonProperty]
		public PlayerReportType Type { get; private set; }

		[JsonProperty]
		public string Message { get; private set; }

		public ReportPlayerMessage()
		{
		}

		public ReportPlayerMessage(Guid gameId, PlayerId reportedPlayerId, string reportedPlayerName, PlayerReportType type, string message)
		{
			this.GameId = gameId;
			this.ReportedPlayerId = reportedPlayerId;
			this.ReportedPlayerName = reportedPlayerName;
			this.Type = type;
			this.Message = message;
		}
	}
}
