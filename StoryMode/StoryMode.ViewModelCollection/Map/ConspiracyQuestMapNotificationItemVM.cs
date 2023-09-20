using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapNotificationTypes;

namespace StoryMode.ViewModelCollection.Map
{
	public class ConspiracyQuestMapNotificationItemVM : MapNotificationItemBaseVM
	{
		public QuestBase Quest { get; }

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
