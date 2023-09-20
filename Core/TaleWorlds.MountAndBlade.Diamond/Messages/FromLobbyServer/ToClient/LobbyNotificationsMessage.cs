using System;
using Newtonsoft.Json;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class LobbyNotificationsMessage : Message
	{
		[JsonProperty]
		public LobbyNotification[] Notifications { get; private set; }

		public LobbyNotificationsMessage()
		{
		}

		public LobbyNotificationsMessage(LobbyNotification[] notifications)
		{
			this.Notifications = notifications;
		}
	}
}
