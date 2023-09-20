using System;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	// Token: 0x020000B2 RID: 178
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class RejoinBattleRequestMessage : Message
	{
		// Token: 0x170000F9 RID: 249
		// (get) Token: 0x0600028B RID: 651 RVA: 0x00003CBC File Offset: 0x00001EBC
		// (set) Token: 0x0600028C RID: 652 RVA: 0x00003CC4 File Offset: 0x00001EC4
		public bool IsRejoinAccepted { get; private set; }

		// Token: 0x0600028D RID: 653 RVA: 0x00003CCD File Offset: 0x00001ECD
		public RejoinBattleRequestMessage(bool isRejoinAccepted)
		{
			this.IsRejoinAccepted = isRejoinAccepted;
		}
	}
}
