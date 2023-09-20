using System;
using TaleWorlds.Diamond;
using TaleWorlds.MountAndBlade.Diamond;

namespace Messages.FromLobbyServer.ToClient
{
	// Token: 0x0200003E RID: 62
	[MessageDescription("LobbyServer", "Client")]
	[Serializable]
	public class LobbyNotificationsMessage : Message
	{
		// Token: 0x1700005A RID: 90
		// (get) Token: 0x060000EB RID: 235 RVA: 0x00002A6D File Offset: 0x00000C6D
		public LobbyNotification[] Notifications { get; }

		// Token: 0x060000EC RID: 236 RVA: 0x00002A75 File Offset: 0x00000C75
		public LobbyNotificationsMessage(LobbyNotification[] notifications)
		{
			this.Notifications = notifications;
		}
	}
}
