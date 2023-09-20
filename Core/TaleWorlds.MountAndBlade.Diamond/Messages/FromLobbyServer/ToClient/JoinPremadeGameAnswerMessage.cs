using System;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x0200003A RID: 58
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class JoinPremadeGameAnswerMessage : Message
	{
		// Token: 0x17000052 RID: 82
		// (get) Token: 0x060000D5 RID: 213 RVA: 0x0000296C File Offset: 0x00000B6C
		// (set) Token: 0x060000D6 RID: 214 RVA: 0x00002974 File Offset: 0x00000B74
		public bool Successful { get; private set; }

		// Token: 0x060000D7 RID: 215 RVA: 0x0000297D File Offset: 0x00000B7D
		public JoinPremadeGameAnswerMessage(bool successful)
		{
			this.Successful = successful;
		}
	}
}
