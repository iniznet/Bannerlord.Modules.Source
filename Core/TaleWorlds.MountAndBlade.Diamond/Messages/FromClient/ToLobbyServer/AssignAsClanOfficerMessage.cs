using System;
using TaleWorlds.Diamond;
using TaleWorlds.PlayerServices;

namespace Messages.FromClient.ToLobbyServer
{
	// Token: 0x02000071 RID: 113
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class AssignAsClanOfficerMessage : Message
	{
		// Token: 0x170000B2 RID: 178
		// (get) Token: 0x060001C0 RID: 448 RVA: 0x0000340E File Offset: 0x0000160E
		// (set) Token: 0x060001C1 RID: 449 RVA: 0x00003416 File Offset: 0x00001616
		public PlayerId AssignedPlayerId { get; private set; }

		// Token: 0x170000B3 RID: 179
		// (get) Token: 0x060001C2 RID: 450 RVA: 0x0000341F File Offset: 0x0000161F
		// (set) Token: 0x060001C3 RID: 451 RVA: 0x00003427 File Offset: 0x00001627
		public bool DontUseNameForUnknownPlayer { get; private set; }

		// Token: 0x060001C4 RID: 452 RVA: 0x00003430 File Offset: 0x00001630
		public AssignAsClanOfficerMessage(PlayerId assignedPlayerId, bool dontUseNameForUnknownPlayer)
		{
			this.AssignedPlayerId = assignedPlayerId;
			this.DontUseNameForUnknownPlayer = dontUseNameForUnknownPlayer;
		}
	}
}
