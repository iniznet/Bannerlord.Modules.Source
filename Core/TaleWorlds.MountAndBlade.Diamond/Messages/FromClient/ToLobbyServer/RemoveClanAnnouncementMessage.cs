using System;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	// Token: 0x020000B3 RID: 179
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class RemoveClanAnnouncementMessage : Message
	{
		// Token: 0x170000FA RID: 250
		// (get) Token: 0x0600028E RID: 654 RVA: 0x00003CDC File Offset: 0x00001EDC
		// (set) Token: 0x0600028F RID: 655 RVA: 0x00003CE4 File Offset: 0x00001EE4
		public int AnnouncementId { get; private set; }

		// Token: 0x06000290 RID: 656 RVA: 0x00003CED File Offset: 0x00001EED
		public RemoveClanAnnouncementMessage(int announcementId)
		{
			this.AnnouncementId = announcementId;
		}
	}
}
