using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class RemoveClanAnnouncementMessage : Message
	{
		[JsonProperty]
		public int AnnouncementId { get; private set; }

		public RemoveClanAnnouncementMessage()
		{
		}

		public RemoveClanAnnouncementMessage(int announcementId)
		{
			this.AnnouncementId = announcementId;
		}
	}
}
