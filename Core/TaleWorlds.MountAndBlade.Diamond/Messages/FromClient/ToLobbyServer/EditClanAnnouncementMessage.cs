using System;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class EditClanAnnouncementMessage : Message
	{
		public int AnnouncementId { get; private set; }

		public string Text { get; private set; }

		public EditClanAnnouncementMessage(int announcementId, string text)
		{
			this.AnnouncementId = announcementId;
			this.Text = text;
		}
	}
}
