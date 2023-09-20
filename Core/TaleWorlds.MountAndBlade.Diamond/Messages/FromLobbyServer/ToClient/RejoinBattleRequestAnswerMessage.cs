using System;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x02000050 RID: 80
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class RejoinBattleRequestAnswerMessage : Message
	{
		// Token: 0x17000072 RID: 114
		// (get) Token: 0x0600012A RID: 298 RVA: 0x00002D4A File Offset: 0x00000F4A
		// (set) Token: 0x0600012B RID: 299 RVA: 0x00002D52 File Offset: 0x00000F52
		public bool IsRejoinAccepted { get; private set; }

		// Token: 0x17000073 RID: 115
		// (get) Token: 0x0600012C RID: 300 RVA: 0x00002D5B File Offset: 0x00000F5B
		// (set) Token: 0x0600012D RID: 301 RVA: 0x00002D63 File Offset: 0x00000F63
		public bool IsSuccessful { get; private set; }

		// Token: 0x0600012E RID: 302 RVA: 0x00002D6C File Offset: 0x00000F6C
		public RejoinBattleRequestAnswerMessage(bool isRejoinAccepted, bool isSuccessful)
		{
			this.IsRejoinAccepted = isRejoinAccepted;
			this.IsSuccessful = isSuccessful;
		}
	}
}
