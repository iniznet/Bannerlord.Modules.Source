using System;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	// Token: 0x0200007A RID: 122
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class ChangeRegionMessage : Message
	{
		// Token: 0x170000B9 RID: 185
		// (get) Token: 0x060001D6 RID: 470 RVA: 0x000034F5 File Offset: 0x000016F5
		// (set) Token: 0x060001D7 RID: 471 RVA: 0x000034FD File Offset: 0x000016FD
		public string Region { get; private set; }

		// Token: 0x060001D8 RID: 472 RVA: 0x00003506 File Offset: 0x00001706
		public ChangeRegionMessage(string region)
		{
			this.Region = region;
		}
	}
}
