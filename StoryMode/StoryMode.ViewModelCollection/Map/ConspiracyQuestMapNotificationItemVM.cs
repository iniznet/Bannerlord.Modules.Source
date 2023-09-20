using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapNotificationTypes;

namespace StoryMode.ViewModelCollection.Map
{
	// Token: 0x02000006 RID: 6
	public class ConspiracyQuestMapNotificationItemVM : MapNotificationItemBaseVM
	{
		// Token: 0x17000023 RID: 35
		// (get) Token: 0x0600005D RID: 93 RVA: 0x00002BB2 File Offset: 0x00000DB2
		public QuestBase Quest { get; }

		// Token: 0x0600005E RID: 94 RVA: 0x00002BBC File Offset: 0x00000DBC
		public ConspiracyQuestMapNotificationItemVM(ConspiracyQuestMapNotification data)
			: base(data)
		{
			ConspiracyQuestMapNotificationItemVM <>4__this = this;
			base.NotificationIdentifier = "conspiracyquest";
			this.Quest = data.ConspiracyQuest;
			this._onInspect = delegate
			{
				INavigationHandler navigationHandler = <>4__this.NavigationHandler;
				if (navigationHandler == null)
				{
					return;
				}
				navigationHandler.OpenQuests(data.ConspiracyQuest);
			};
		}
	}
}
