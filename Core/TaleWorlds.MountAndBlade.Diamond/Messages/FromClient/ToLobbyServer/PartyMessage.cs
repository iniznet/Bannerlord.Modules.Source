using System;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	// Token: 0x020000AB RID: 171
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class PartyMessage : Message
	{
		// Token: 0x170000EC RID: 236
		// (get) Token: 0x0600026B RID: 619 RVA: 0x00003B42 File Offset: 0x00001D42
		// (set) Token: 0x0600026C RID: 620 RVA: 0x00003B4A File Offset: 0x00001D4A
		public string Message { get; private set; }

		// Token: 0x0600026D RID: 621 RVA: 0x00003B53 File Offset: 0x00001D53
		public PartyMessage(string message)
		{
			this.Message = message;
		}
	}
}
