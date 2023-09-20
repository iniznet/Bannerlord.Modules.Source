using System;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	// Token: 0x02000089 RID: 137
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class EditClanAnnouncementMessage : Message
	{
		// Token: 0x170000CC RID: 204
		// (get) Token: 0x0600020A RID: 522 RVA: 0x00003731 File Offset: 0x00001931
		// (set) Token: 0x0600020B RID: 523 RVA: 0x00003739 File Offset: 0x00001939
		public int AnnouncementId { get; private set; }

		// Token: 0x170000CD RID: 205
		// (get) Token: 0x0600020C RID: 524 RVA: 0x00003742 File Offset: 0x00001942
		// (set) Token: 0x0600020D RID: 525 RVA: 0x0000374A File Offset: 0x0000194A
		public string Text { get; private set; }

		// Token: 0x0600020E RID: 526 RVA: 0x00003753 File Offset: 0x00001953
		public EditClanAnnouncementMessage(int announcementId, string text)
		{
			this.AnnouncementId = announcementId;
			this.Text = text;
		}
	}
}
