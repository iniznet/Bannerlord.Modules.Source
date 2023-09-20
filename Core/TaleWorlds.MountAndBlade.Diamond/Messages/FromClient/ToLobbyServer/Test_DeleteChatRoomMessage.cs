using System;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	// Token: 0x020000BE RID: 190
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class Test_DeleteChatRoomMessage : Message
	{
		// Token: 0x1700010A RID: 266
		// (get) Token: 0x060002B4 RID: 692 RVA: 0x00003E89 File Offset: 0x00002089
		// (set) Token: 0x060002B5 RID: 693 RVA: 0x00003E91 File Offset: 0x00002091
		public Guid ChatRoomId { get; private set; }

		// Token: 0x060002B6 RID: 694 RVA: 0x00003E9A File Offset: 0x0000209A
		public Test_DeleteChatRoomMessage(Guid chatRoomId)
		{
			this.ChatRoomId = chatRoomId;
		}
	}
}
