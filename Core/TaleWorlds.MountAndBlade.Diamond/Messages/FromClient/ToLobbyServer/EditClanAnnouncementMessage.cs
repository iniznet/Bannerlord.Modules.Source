using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class EditClanAnnouncementMessage : Message
	{
		[JsonProperty]
		public int AnnouncementId { get; private set; }

		[JsonProperty]
		public string Text { get; private set; }

		public EditClanAnnouncementMessage()
		{
		}

		public EditClanAnnouncementMessage(int announcementId, string text)
		{
			this.AnnouncementId = announcementId;
			this.Text = text;
		}
	}
}
