using System;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x02000054 RID: 84
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class SigilChangeAnswerMessage : Message
	{
		// Token: 0x17000076 RID: 118
		// (get) Token: 0x06000136 RID: 310 RVA: 0x00002DCA File Offset: 0x00000FCA
		// (set) Token: 0x06000137 RID: 311 RVA: 0x00002DD2 File Offset: 0x00000FD2
		public bool Successful { get; private set; }

		// Token: 0x06000138 RID: 312 RVA: 0x00002DDB File Offset: 0x00000FDB
		public SigilChangeAnswerMessage(bool answer)
		{
			this.Successful = answer;
		}
	}
}
