using System;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	// Token: 0x0200006C RID: 108
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class AcceptJoinPremadeGameRequestMessage : Message
	{
		// Token: 0x170000AB RID: 171
		// (get) Token: 0x060001AF RID: 431 RVA: 0x00003350 File Offset: 0x00001550
		// (set) Token: 0x060001B0 RID: 432 RVA: 0x00003358 File Offset: 0x00001558
		public Guid PartyId { get; private set; }

		// Token: 0x060001B1 RID: 433 RVA: 0x00003361 File Offset: 0x00001561
		public AcceptJoinPremadeGameRequestMessage(Guid partyId)
		{
			this.PartyId = partyId;
		}
	}
}
