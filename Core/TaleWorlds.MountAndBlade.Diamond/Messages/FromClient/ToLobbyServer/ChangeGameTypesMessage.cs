using System;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	// Token: 0x02000078 RID: 120
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class ChangeGameTypesMessage : Message
	{
		// Token: 0x170000B7 RID: 183
		// (get) Token: 0x060001D0 RID: 464 RVA: 0x000034B5 File Offset: 0x000016B5
		// (set) Token: 0x060001D1 RID: 465 RVA: 0x000034BD File Offset: 0x000016BD
		public string[] GameTypes { get; private set; }

		// Token: 0x060001D2 RID: 466 RVA: 0x000034C6 File Offset: 0x000016C6
		public ChangeGameTypesMessage(string[] gameTypes)
		{
			this.GameTypes = gameTypes;
		}
	}
}
