using System;
using TaleWorlds.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x0200000A RID: 10
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class ChatRoomClosedMessage : Message
	{
		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000021 RID: 33 RVA: 0x000021A1 File Offset: 0x000003A1
		// (set) Token: 0x06000022 RID: 34 RVA: 0x000021A9 File Offset: 0x000003A9
		public Guid ChatRoomId { get; private set; }

		// Token: 0x06000023 RID: 35 RVA: 0x000021B2 File Offset: 0x000003B2
		public ChatRoomClosedMessage(Guid chatRoomId)
		{
			this.ChatRoomId = chatRoomId;
		}
	}
}
