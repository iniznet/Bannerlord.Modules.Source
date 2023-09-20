using System;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	// Token: 0x020000BC RID: 188
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class Test_AddChatRoomUser : Message
	{
		// Token: 0x17000108 RID: 264
		// (get) Token: 0x060002AE RID: 686 RVA: 0x00003E49 File Offset: 0x00002049
		// (set) Token: 0x060002AF RID: 687 RVA: 0x00003E51 File Offset: 0x00002051
		public string Name { get; private set; }

		// Token: 0x060002B0 RID: 688 RVA: 0x00003E5A File Offset: 0x0000205A
		public Test_AddChatRoomUser(string name)
		{
			this.Name = name;
		}
	}
}
