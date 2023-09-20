using System;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	// Token: 0x0200007F RID: 127
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class ClanMessage : Message
	{
		// Token: 0x170000BF RID: 191
		// (get) Token: 0x060001E6 RID: 486 RVA: 0x000035A4 File Offset: 0x000017A4
		// (set) Token: 0x060001E7 RID: 487 RVA: 0x000035AC File Offset: 0x000017AC
		public string Message { get; private set; }

		// Token: 0x060001E8 RID: 488 RVA: 0x000035B5 File Offset: 0x000017B5
		public ClanMessage(string message)
		{
			this.Message = message;
		}
	}
}
