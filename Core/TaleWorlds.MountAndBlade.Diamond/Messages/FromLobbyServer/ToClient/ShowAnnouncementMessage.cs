using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x02000053 RID: 83
	[Serializable]
	public class ShowAnnouncementMessage : Message
	{
		// Token: 0x17000075 RID: 117
		// (get) Token: 0x06000133 RID: 307 RVA: 0x00002DAA File Offset: 0x00000FAA
		// (set) Token: 0x06000134 RID: 308 RVA: 0x00002DB2 File Offset: 0x00000FB2
		public Announcement Announcement { get; private set; }

		// Token: 0x06000135 RID: 309 RVA: 0x00002DBB File Offset: 0x00000FBB
		public ShowAnnouncementMessage(Announcement announcement)
		{
			this.Announcement = announcement;
		}
	}
}
