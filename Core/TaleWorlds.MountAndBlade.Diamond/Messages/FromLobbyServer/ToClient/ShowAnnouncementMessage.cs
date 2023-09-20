using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[Serializable]
	public class ShowAnnouncementMessage : Message
	{
		public Announcement Announcement { get; private set; }

		public ShowAnnouncementMessage(Announcement announcement)
		{
			this.Announcement = announcement;
		}
	}
}
