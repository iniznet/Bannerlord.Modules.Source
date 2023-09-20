using System;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	// Token: 0x02000077 RID: 119
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class ChangeClanSigilMessage : Message
	{
		// Token: 0x170000B6 RID: 182
		// (get) Token: 0x060001CD RID: 461 RVA: 0x00003495 File Offset: 0x00001695
		// (set) Token: 0x060001CE RID: 462 RVA: 0x0000349D File Offset: 0x0000169D
		public string NewSigil { get; private set; }

		// Token: 0x060001CF RID: 463 RVA: 0x000034A6 File Offset: 0x000016A6
		public ChangeClanSigilMessage(string newSigil)
		{
			this.NewSigil = newSigil;
		}
	}
}
