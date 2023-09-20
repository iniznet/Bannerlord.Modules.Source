using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class AddClanAnnouncementMessage : Message
	{
		[JsonProperty]
		public string Announcement { get; private set; }

		public AddClanAnnouncementMessage()
		{
		}

		public AddClanAnnouncementMessage(string announcement)
		{
			this.Announcement = announcement;
		}
	}
}
