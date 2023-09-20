using System;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x02000016 RID: 22
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class CreateClanAnswerMessage : Message
	{
		// Token: 0x1700001E RID: 30
		// (get) Token: 0x0600004D RID: 77 RVA: 0x00002382 File Offset: 0x00000582
		// (set) Token: 0x0600004E RID: 78 RVA: 0x0000238A File Offset: 0x0000058A
		public bool Successful { get; private set; }

		// Token: 0x0600004F RID: 79 RVA: 0x00002393 File Offset: 0x00000593
		public CreateClanAnswerMessage(bool successful)
		{
			this.Successful = successful;
		}
	}
}
