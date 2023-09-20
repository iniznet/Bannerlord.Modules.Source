using System;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x02000013 RID: 19
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class ClanMessageReceivedMessage : Message
	{
		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000040 RID: 64 RVA: 0x000022F2 File Offset: 0x000004F2
		// (set) Token: 0x06000041 RID: 65 RVA: 0x000022FA File Offset: 0x000004FA
		public string PlayerName { get; private set; }

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000042 RID: 66 RVA: 0x00002303 File Offset: 0x00000503
		// (set) Token: 0x06000043 RID: 67 RVA: 0x0000230B File Offset: 0x0000050B
		public string Message { get; private set; }

		// Token: 0x06000044 RID: 68 RVA: 0x00002314 File Offset: 0x00000514
		public ClanMessageReceivedMessage(string playerName, string message)
		{
			this.PlayerName = playerName;
			this.Message = message;
		}
	}
}
