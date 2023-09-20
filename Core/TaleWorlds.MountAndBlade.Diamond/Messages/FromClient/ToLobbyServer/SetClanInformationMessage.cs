using System;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	// Token: 0x020000BB RID: 187
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class SetClanInformationMessage : Message
	{
		// Token: 0x17000107 RID: 263
		// (get) Token: 0x060002AB RID: 683 RVA: 0x00003E29 File Offset: 0x00002029
		// (set) Token: 0x060002AC RID: 684 RVA: 0x00003E31 File Offset: 0x00002031
		public string Information { get; private set; }

		// Token: 0x060002AD RID: 685 RVA: 0x00003E3A File Offset: 0x0000203A
		public SetClanInformationMessage(string information)
		{
			this.Information = information;
		}
	}
}
