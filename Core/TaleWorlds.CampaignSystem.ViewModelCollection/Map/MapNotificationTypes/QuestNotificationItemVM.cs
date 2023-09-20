using System;
using TaleWorlds.CampaignSystem.Issues;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapNotificationTypes
{
	public class QuestNotificationItemVM : MapNotificationItemBaseVM
	{
		public QuestNotificationItemVM(QuestBase quest, InformationData data, Action<QuestBase> onQuestNotificationInspect, Action<MapNotificationItemBaseVM> onRemove)
			: base(data)
		{
			this._quest = quest;
			this._onQuestNotificationInspect = onQuestNotificationInspect;
			this._onInspect = (this._onInspectAction = delegate
			{
				this._onQuestNotificationInspect(this._quest);
			});
			base.NotificationIdentifier = "quest";
		}

		public QuestNotificationItemVM(IssueBase issue, InformationData data, Action<IssueBase> onIssueNotificationInspect, Action<MapNotificationItemBaseVM> onRemove)
			: base(data)
		{
			this._issue = issue;
			this._onIssueNotificationInspect = onIssueNotificationInspect;
			this._onInspect = (this._onInspectAction = delegate
			{
				this._onIssueNotificationInspect(this._issue);
			});
			base.NotificationIdentifier = "quest";
		}

		public override void ManualRefreshRelevantStatus()
		{
			base.ManualRefreshRelevantStatus();
		}

		private QuestBase _quest;

		private IssueBase _issue;

		private Action<QuestBase> _onQuestNotificationInspect;

		private Action<IssueBase> _onIssueNotificationInspect;

		protected Action _onInspectAction;
	}
}
