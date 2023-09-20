using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class UpdateNotificationsMessage : Message
	{
		[JsonProperty]
		public int[] SeenNotificationIds { get; private set; }

		public UpdateNotificationsMessage()
		{
		}

		public UpdateNotificationsMessage(int[] seenNotificationIds)
		{
			this.SeenNotificationIds = seenNotificationIds;
		}
	}
}
