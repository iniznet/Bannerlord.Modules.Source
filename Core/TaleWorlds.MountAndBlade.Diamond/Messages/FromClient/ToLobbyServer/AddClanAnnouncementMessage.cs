using System;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	// Token: 0x0200006E RID: 110
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class AddClanAnnouncementMessage : Message
	{
		// Token: 0x170000AC RID: 172
		// (get) Token: 0x060001B3 RID: 435 RVA: 0x00003378 File Offset: 0x00001578
		// (set) Token: 0x060001B4 RID: 436 RVA: 0x00003380 File Offset: 0x00001580
		public string Announcement { get; private set; }

		// Token: 0x060001B5 RID: 437 RVA: 0x00003389 File Offset: 0x00001589
		public AddClanAnnouncementMessage(string announcement)
		{
			this.Announcement = announcement;
		}
	}
}
