using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x02000052 RID: 82
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class ServerStatusMessage : Message
	{
		// Token: 0x17000074 RID: 116
		// (get) Token: 0x06000130 RID: 304 RVA: 0x00002D8A File Offset: 0x00000F8A
		// (set) Token: 0x06000131 RID: 305 RVA: 0x00002D92 File Offset: 0x00000F92
		public ServerStatus ServerStatus { get; private set; }

		// Token: 0x06000132 RID: 306 RVA: 0x00002D9B File Offset: 0x00000F9B
		public ServerStatusMessage(ServerStatus serverStatus)
		{
			this.ServerStatus = serverStatus;
		}
	}
}
