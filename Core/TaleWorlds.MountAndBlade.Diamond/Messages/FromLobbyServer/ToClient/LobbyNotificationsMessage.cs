using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class LobbyNotificationsMessage : Message
	{
		public LobbyNotification[] Notifications { get; }

		public LobbyNotificationsMessage(LobbyNotification[] notifications)
		{
			this.Notifications = notifications;
		}
	}
}
