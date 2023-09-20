using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromClient.ToLobbyServer
{
	// Token: 0x020000B6 RID: 182
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class ReportPlayerMessage : Message
	{
		// Token: 0x170000FD RID: 253
		// (get) Token: 0x06000297 RID: 663 RVA: 0x00003D3C File Offset: 0x00001F3C
		public Guid GameId { get; }

		// Token: 0x170000FE RID: 254
		// (get) Token: 0x06000298 RID: 664 RVA: 0x00003D44 File Offset: 0x00001F44
		public PlayerId ReportedPlayerId { get; }

		// Token: 0x170000FF RID: 255
		// (get) Token: 0x06000299 RID: 665 RVA: 0x00003D4C File Offset: 0x00001F4C
		public string ReportedPlayerName { get; }

		// Token: 0x17000100 RID: 256
		// (get) Token: 0x0600029A RID: 666 RVA: 0x00003D54 File Offset: 0x00001F54
		public PlayerReportType Type { get; }

		// Token: 0x17000101 RID: 257
		// (get) Token: 0x0600029B RID: 667 RVA: 0x00003D5C File Offset: 0x00001F5C
		public string Message { get; }

		// Token: 0x0600029C RID: 668 RVA: 0x00003D64 File Offset: 0x00001F64
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
