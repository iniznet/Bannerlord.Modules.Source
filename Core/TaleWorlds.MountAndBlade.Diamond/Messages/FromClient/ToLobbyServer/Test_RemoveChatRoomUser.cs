using System;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	// Token: 0x020000BF RID: 191
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class Test_RemoveChatRoomUser : Message
	{
		// Token: 0x1700010B RID: 267
		// (get) Token: 0x060002B7 RID: 695 RVA: 0x00003EA9 File Offset: 0x000020A9
		// (set) Token: 0x060002B8 RID: 696 RVA: 0x00003EB1 File Offset: 0x000020B1
		public string Name { get; private set; }

		// Token: 0x060002B9 RID: 697 RVA: 0x00003EBA File Offset: 0x000020BA
		public Test_RemoveChatRoomUser(string name)
		{
			this.Name = name;
		}
	}
}
