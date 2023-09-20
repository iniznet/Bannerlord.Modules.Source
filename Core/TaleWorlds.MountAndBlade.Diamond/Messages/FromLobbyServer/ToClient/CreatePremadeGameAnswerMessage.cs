using System;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x02000017 RID: 23
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class CreatePremadeGameAnswerMessage : Message
	{
		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000050 RID: 80 RVA: 0x000023A2 File Offset: 0x000005A2
		// (set) Token: 0x06000051 RID: 81 RVA: 0x000023AA File Offset: 0x000005AA
		public bool Successful { get; private set; }

		// Token: 0x06000052 RID: 82 RVA: 0x000023B3 File Offset: 0x000005B3
		public CreatePremadeGameAnswerMessage(bool successful)
		{
			this.Successful = successful;
		}
	}
}
