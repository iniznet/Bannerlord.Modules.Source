using System;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	// Token: 0x020000BD RID: 189
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class Test_CreateChatRoomMessage : Message
	{
		// Token: 0x17000109 RID: 265
		// (get) Token: 0x060002B1 RID: 689 RVA: 0x00003E69 File Offset: 0x00002069
		// (set) Token: 0x060002B2 RID: 690 RVA: 0x00003E71 File Offset: 0x00002071
		public string Name { get; private set; }

		// Token: 0x060002B3 RID: 691 RVA: 0x00003E7A File Offset: 0x0000207A
		public Test_CreateChatRoomMessage(string name)
		{
			this.Name = name;
		}
	}
}
