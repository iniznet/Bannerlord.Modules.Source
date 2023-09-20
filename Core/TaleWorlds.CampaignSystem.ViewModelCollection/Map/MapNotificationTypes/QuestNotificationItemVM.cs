using System;
using TaleWorlds.CampaignSystem.Issues;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapNotificationTypes
{
	// Token: 0x02000044 RID: 68
	public class QuestNotificationItemVM : MapNotificationItemBaseVM
	{
		// Token: 0x06000554 RID: 1364 RVA: 0x0001ACE4 File Offset: 0x00018EE4
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

		// Token: 0x06000555 RID: 1365 RVA: 0x0001AD2C File Offset: 0x00018F2C
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

		// Token: 0x06000556 RID: 1366 RVA: 0x0001AD74 File Offset: 0x00018F74
		public override void ManualRefreshRelevantStatus()
		{
			base.ManualRefreshRelevantStatus();
		}

		// Token: 0x04000243 RID: 579
		private QuestBase _quest;

		// Token: 0x04000244 RID: 580
		private IssueBase _issue;

		// Token: 0x04000245 RID: 581
		private Action<QuestBase> _onQuestNotificationInspect;

		// Token: 0x04000246 RID: 582
		private Action<IssueBase> _onIssueNotificationInspect;

		// Token: 0x04000247 RID: 583
		protected Action _onInspectAction;
	}
}
