using System;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class UpdateNotificationsMessage : Message
	{
		public int[] SeenNotificationIds { get; }

		public UpdateNotificationsMessage(int[] seenNotificationIds)
		{
			this.SeenNotificationIds = seenNotificationIds;
		}
	}
}
