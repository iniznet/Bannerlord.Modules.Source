using System;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	// Token: 0x0200009F RID: 159
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class GetRankedLeaderboardMessage : Message
	{
		// Token: 0x170000DB RID: 219
		// (get) Token: 0x0600023D RID: 573 RVA: 0x00003941 File Offset: 0x00001B41
		// (set) Token: 0x0600023E RID: 574 RVA: 0x00003949 File Offset: 0x00001B49
		public string GameType { get; private set; }

		// Token: 0x0600023F RID: 575 RVA: 0x00003952 File Offset: 0x00001B52
		public GetRankedLeaderboardMessage(string gameType)
		{
			this.GameType = gameType;
		}
	}
}
