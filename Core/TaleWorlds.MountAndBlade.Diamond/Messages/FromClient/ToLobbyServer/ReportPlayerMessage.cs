using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class ReportPlayerMessage : Message
	{
		public Guid GameId { get; }

		public PlayerId ReportedPlayerId { get; }

		public string ReportedPlayerName { get; }

		public PlayerReportType Type { get; }

		public string Message { get; }

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
