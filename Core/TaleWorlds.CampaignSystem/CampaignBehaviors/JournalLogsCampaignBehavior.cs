using System;
using System.Linq;
using TaleWorlds.CampaignSystem.Issues;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	// Token: 0x020003A8 RID: 936
	public class JournalLogsCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x060037C7 RID: 14279 RVA: 0x000FB2A0 File Offset: 0x000F94A0
		public override void RegisterEvents()
		{
			CampaignEvents.OnQuestStartedEvent.AddNonSerializedListener(this, new Action<QuestBase>(this.OnQuestStarted));
			CampaignEvents.OnQuestCompletedEvent.AddNonSerializedListener(this, new Action<QuestBase, QuestBase.QuestCompleteDetails>(this.OnQuestCompleted));
			CampaignEvents.OnIssueUpdatedEvent.AddNonSerializedListener(this, new Action<IssueBase, IssueBase.IssueUpdateDetails, Hero>(this.OnIssueUpdated));
			CampaignEvents.IssueLogAddedEvent.AddNonSerializedListener(this, new Action<IssueBase, bool>(this.OnIssueLogAdded));
			CampaignEvents.QuestLogAddedEvent.AddNonSerializedListener(this, new Action<QuestBase, bool>(this.OnQuestLogAdded));
		}

		// Token: 0x060037C8 RID: 14280 RVA: 0x000FB320 File Offset: 0x000F9520
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x060037C9 RID: 14281 RVA: 0x000FB324 File Offset: 0x000F9524
		private void OnIssueLogAdded(IssueBase issue, bool hideInformation)
		{
			JournalLogEntry journalLogEntry = this.GetRelatedLog(issue);
			if (journalLogEntry == null)
			{
				journalLogEntry = this.CreateRelatedLog(issue);
				LogEntry.AddLogEntry(journalLogEntry);
			}
			journalLogEntry.Update(this.GetEntries(issue), IssueBase.IssueUpdateDetails.None);
		}

		// Token: 0x060037CA RID: 14282 RVA: 0x000FB358 File Offset: 0x000F9558
		private void OnQuestLogAdded(QuestBase quest, bool hideInformation)
		{
			JournalLogEntry journalLogEntry = this.GetRelatedLog(quest);
			if (journalLogEntry == null)
			{
				journalLogEntry = this.CreateRelatedLog(quest);
				LogEntry.AddLogEntry(journalLogEntry);
			}
			journalLogEntry.Update(this.GetEntries(quest), IssueBase.IssueUpdateDetails.None);
		}

		// Token: 0x060037CB RID: 14283 RVA: 0x000FB38C File Offset: 0x000F958C
		private void OnQuestStarted(QuestBase quest)
		{
			JournalLogEntry journalLogEntry = this.GetRelatedLog(quest);
			if (journalLogEntry == null)
			{
				journalLogEntry = this.CreateRelatedLog(quest);
				LogEntry.AddLogEntry(journalLogEntry);
			}
			journalLogEntry.Update(this.GetEntries(quest), IssueBase.IssueUpdateDetails.None);
			LogEntry.AddLogEntry(new IssueQuestStartLogEntry(journalLogEntry.RelatedHero));
		}

		// Token: 0x060037CC RID: 14284 RVA: 0x000FB3D0 File Offset: 0x000F95D0
		private void OnQuestCompleted(QuestBase quest, QuestBase.QuestCompleteDetails detail)
		{
			JournalLogEntry journalLogEntry = this.GetRelatedLog(quest);
			if (journalLogEntry == null)
			{
				journalLogEntry = this.CreateRelatedLog(quest);
				LogEntry.AddLogEntry(journalLogEntry);
			}
			journalLogEntry.Update(this.GetEntries(quest), detail);
			LogEntry.AddLogEntry(new IssueQuestLogEntry(journalLogEntry.RelatedHero, journalLogEntry.Antagonist, detail));
		}

		// Token: 0x060037CD RID: 14285 RVA: 0x000FB41C File Offset: 0x000F961C
		private void OnIssueUpdated(IssueBase issue, IssueBase.IssueUpdateDetails details, Hero issueSolver)
		{
			if (issueSolver == Hero.MainHero)
			{
				JournalLogEntry journalLogEntry = this.GetRelatedLog(issue);
				if (journalLogEntry == null)
				{
					journalLogEntry = this.CreateRelatedLog(issue);
					LogEntry.AddLogEntry(journalLogEntry);
				}
				journalLogEntry.Update(this.GetEntries(issue), details);
			}
		}

		// Token: 0x060037CE RID: 14286 RVA: 0x000FB458 File Offset: 0x000F9658
		private JournalLogEntry CreateRelatedLog(IssueBase issue)
		{
			if (issue.IssueQuest != null)
			{
				return new JournalLogEntry(issue.IssueQuest.Title, issue.IssueQuest.QuestGiver, null, issue.IssueQuest.IsSpecialQuest, new MBObjectBase[] { issue, issue.IssueQuest });
			}
			return new JournalLogEntry(issue.Title, issue.IssueOwner, issue.CounterOfferHero, false, new MBObjectBase[] { issue });
		}

		// Token: 0x060037CF RID: 14287 RVA: 0x000FB4CC File Offset: 0x000F96CC
		private JournalLogEntry CreateRelatedLog(QuestBase quest)
		{
			IssueBase issueOfQuest = IssueManager.GetIssueOfQuest(quest);
			if (issueOfQuest != null)
			{
				return this.CreateRelatedLog(issueOfQuest);
			}
			return new JournalLogEntry(quest.Title, quest.QuestGiver, null, quest.IsSpecialQuest, new MBObjectBase[] { quest });
		}

		// Token: 0x060037D0 RID: 14288 RVA: 0x000FB510 File Offset: 0x000F9710
		private JournalLogEntry GetRelatedLog(IssueBase issue)
		{
			return Campaign.Current.LogEntryHistory.FindLastGameActionLog<JournalLogEntry>((JournalLogEntry x) => x.IsRelatedTo(issue));
		}

		// Token: 0x060037D1 RID: 14289 RVA: 0x000FB548 File Offset: 0x000F9748
		private JournalLogEntry GetRelatedLog(QuestBase quest)
		{
			IssueBase issueOfQuest = IssueManager.GetIssueOfQuest(quest);
			if (issueOfQuest != null)
			{
				return this.GetRelatedLog(issueOfQuest);
			}
			return Campaign.Current.LogEntryHistory.FindLastGameActionLog<JournalLogEntry>((JournalLogEntry x) => x.IsRelatedTo(quest));
		}

		// Token: 0x060037D2 RID: 14290 RVA: 0x000FB594 File Offset: 0x000F9794
		private MBReadOnlyList<JournalLog> GetEntries(IssueBase issue)
		{
			if (issue.IssueQuest == null)
			{
				return issue.JournalEntries;
			}
			MBList<JournalLog> mblist = issue.JournalEntries.ToMBList<JournalLog>();
			JournalLog journalLog = issue.IssueQuest.JournalEntries.FirstOrDefault<JournalLog>();
			if (journalLog != null)
			{
				int i;
				for (i = 0; i < mblist.Count; i++)
				{
					if (mblist[i].LogTime > journalLog.LogTime)
					{
						i--;
						break;
					}
				}
				mblist.InsertRange(i, issue.IssueQuest.JournalEntries);
			}
			return mblist;
		}

		// Token: 0x060037D3 RID: 14291 RVA: 0x000FB614 File Offset: 0x000F9814
		private MBReadOnlyList<JournalLog> GetEntries(QuestBase quest)
		{
			IssueBase issueOfQuest = IssueManager.GetIssueOfQuest(quest);
			if (issueOfQuest != null)
			{
				return this.GetEntries(issueOfQuest);
			}
			return quest.JournalEntries;
		}
	}
}
