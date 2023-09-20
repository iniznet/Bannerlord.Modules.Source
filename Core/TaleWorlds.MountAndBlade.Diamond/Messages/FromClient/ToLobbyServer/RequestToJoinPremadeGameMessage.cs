using System;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	// Token: 0x020000B9 RID: 185
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class RequestToJoinPremadeGameMessage : Message
	{
		// Token: 0x17000104 RID: 260
		// (get) Token: 0x060002A3 RID: 675 RVA: 0x00003DD1 File Offset: 0x00001FD1
		// (set) Token: 0x060002A4 RID: 676 RVA: 0x00003DD9 File Offset: 0x00001FD9
		public Guid GameId { get; private set; }

		// Token: 0x17000105 RID: 261
		// (get) Token: 0x060002A5 RID: 677 RVA: 0x00003DE2 File Offset: 0x00001FE2
		// (set) Token: 0x060002A6 RID: 678 RVA: 0x00003DEA File Offset: 0x00001FEA
		public string Password { get; private set; }

		// Token: 0x060002A7 RID: 679 RVA: 0x00003DF3 File Offset: 0x00001FF3
		public RequestToJoinPremadeGameMessage(Guid gameId, string password)
		{
			this.GameId = gameId;
			this.Password = password;
		}
	}
}
