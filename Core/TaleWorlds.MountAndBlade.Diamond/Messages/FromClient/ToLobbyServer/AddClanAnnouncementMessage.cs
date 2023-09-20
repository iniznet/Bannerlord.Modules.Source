using System;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class AddClanAnnouncementMessage : Message
	{
		public string Announcement { get; private set; }

		public AddClanAnnouncementMessage(string announcement)
		{
			this.Announcement = announcement;
		}
	}
}
