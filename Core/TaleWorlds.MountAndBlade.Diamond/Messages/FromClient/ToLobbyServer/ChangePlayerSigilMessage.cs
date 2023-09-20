using System;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	// Token: 0x02000079 RID: 121
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class ChangePlayerSigilMessage : Message
	{
		// Token: 0x170000B8 RID: 184
		// (get) Token: 0x060001D3 RID: 467 RVA: 0x000034D5 File Offset: 0x000016D5
		// (set) Token: 0x060001D4 RID: 468 RVA: 0x000034DD File Offset: 0x000016DD
		public string SigilId { get; private set; }

		// Token: 0x060001D5 RID: 469 RVA: 0x000034E6 File Offset: 0x000016E6
		public ChangePlayerSigilMessage(string sigilId)
		{
			this.SigilId = sigilId;
		}
	}
}
