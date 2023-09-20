using System;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	// Token: 0x02000076 RID: 118
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class ChangeClanFactionMessage : Message
	{
		// Token: 0x170000B5 RID: 181
		// (get) Token: 0x060001CA RID: 458 RVA: 0x00003475 File Offset: 0x00001675
		// (set) Token: 0x060001CB RID: 459 RVA: 0x0000347D File Offset: 0x0000167D
		public string NewFaction { get; private set; }

		// Token: 0x060001CC RID: 460 RVA: 0x00003486 File Offset: 0x00001686
		public ChangeClanFactionMessage(string newFaction)
		{
			this.NewFaction = newFaction;
		}
	}
}
