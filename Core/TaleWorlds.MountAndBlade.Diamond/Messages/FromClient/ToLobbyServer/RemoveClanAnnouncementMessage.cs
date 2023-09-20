using System;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class RemoveClanAnnouncementMessage : Message
	{
		public int AnnouncementId { get; private set; }

		public RemoveClanAnnouncementMessage(int announcementId)
		{
			this.AnnouncementId = announcementId;
		}
	}
}
