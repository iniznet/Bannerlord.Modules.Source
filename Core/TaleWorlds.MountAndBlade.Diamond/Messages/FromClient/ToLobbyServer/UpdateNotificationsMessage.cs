using System;
using TaleWorlds.Diamond;

namespace Messages.FromClient.ToLobbyServer
{
	// Token: 0x020000C1 RID: 193
	[MessageDescription("Client", "LobbyServer")]
	[Serializable]
	public class UpdateNotificationsMessage : Message
	{
		// Token: 0x1700010E RID: 270
		// (get) Token: 0x060002BF RID: 703 RVA: 0x00003F01 File Offset: 0x00002101
		public int[] SeenNotificationIds { get; }

		// Token: 0x060002C0 RID: 704 RVA: 0x00003F09 File Offset: 0x00002109
		public UpdateNotificationsMessage(int[] seenNotificationIds)
		{
			this.SeenNotificationIds = seenNotificationIds;
		}
	}
}
