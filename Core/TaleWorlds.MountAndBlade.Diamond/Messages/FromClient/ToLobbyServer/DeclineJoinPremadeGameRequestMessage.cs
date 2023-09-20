using System;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	// Token: 0x02000085 RID: 133
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class DeclineJoinPremadeGameRequestMessage : Message
	{
		// Token: 0x170000CB RID: 203
		// (get) Token: 0x06000204 RID: 516 RVA: 0x000036F9 File Offset: 0x000018F9
		// (set) Token: 0x06000205 RID: 517 RVA: 0x00003701 File Offset: 0x00001901
		public Guid PartyId { get; private set; }

		// Token: 0x06000206 RID: 518 RVA: 0x0000370A File Offset: 0x0000190A
		public DeclineJoinPremadeGameRequestMessage(Guid partyId)
		{
			this.PartyId = partyId;
		}
	}
}
