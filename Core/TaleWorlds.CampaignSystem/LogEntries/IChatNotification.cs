using System;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.LogEntries
{
	// Token: 0x020002F7 RID: 759
	public interface IChatNotification
	{
		// Token: 0x17000A8C RID: 2700
		// (get) Token: 0x06002B97 RID: 11159
		bool IsVisibleNotification { get; }

		// Token: 0x17000A8D RID: 2701
		// (get) Token: 0x06002B98 RID: 11160
		ChatNotificationType NotificationType { get; }

		// Token: 0x06002B99 RID: 11161
		TextObject GetNotificationText();
	}
}
