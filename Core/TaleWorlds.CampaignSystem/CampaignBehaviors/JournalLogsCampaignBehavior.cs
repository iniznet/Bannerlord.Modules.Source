using System;
using System.Linq;
using TaleWorlds.CampaignSystem.Issues;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	public class JournalLogsCampaignBehavior : CampaignBehaviorBase
	{
		public override void RegisterEvents()
		{
			CampaignEvents.OnQuestStartedEvent.AddNonSerializedListener(this, new Action<QuestBase>(this.OnQuestStarted));
			CampaignEvents.OnQuestCompletedEvent.AddNonSerializedListener(this, new Action<QuestBase, QuestBase.QuestCompleteDetails>(this.OnQuestCompleted));
			CampaignEvents.OnIssueUpdatedEvent.AddNonSerializedListener(this, new Action<IssueBase, IssueBase.IssueUpdateDetails, Hero>(this.OnIssueUpdated));
			CampaignEvents.IssueLogAddedEvent.AddNonSerializedListener(this, new Action<IssueBase, bool>(this.OnIssueLogAdded));
			CampaignEvents.QuestLogAddedEvent.AddNonSerializedListener(this, new Action<QuestBase, bool>(this.OnQuestLogAdded));
		}

		public override void SyncData(IDataStore dataStore)
		{
		}

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

		private JournalLogEntry CreateRelatedLog(IssueBase issue)
		{
			if (issue.IssueQuest != null)
			{
				return new JournalLogEntry(issue.IssueQuest.Title, issue.IssueQuest.QuestGiver, null, issue.IssueQuest.IsSpecialQuest, new MBObjectBase[] { issue, issue.IssueQuest });
			}
			return new JournalLogEntry(issue.Title, issue.IssueOwner, issue.CounterOfferHero, false, new MBObjectBase[] { issue });
		}

		private JournalLogEntry CreateRelatedLog(QuestBase quest)
		{
			IssueBase issueOfQuest = IssueManager.GetIssueOfQuest(quest);
			if (issueOfQuest != null)
			{
				return this.CreateRelatedLog(issueOfQuest);
			}
			return new JournalLogEntry(quest.Title, quest.QuestGiver, null, quest.IsSpecialQuest, new MBObjectBase[] { quest });
		}

		private JournalLogEntry GetRelatedLog(IssueBase issue)
		{
			return Campaign.Current.LogEntryHistory.FindLastGameActionLog<JournalLogEntry>((JournalLogEntry x) => x.IsRelatedTo(issue));
		}

		private JournalLogEntry GetRelatedLog(QuestBase quest)
		{
			IssueBase issueOfQuest = IssueManager.GetIssueOfQuest(quest);
			if (issueOfQuest != null)
			{
				return this.GetRelatedLog(issueOfQuest);
			}
			return Campaign.Current.LogEntryHistory.FindLastGameActionLog<JournalLogEntry>((JournalLogEntry x) => x.IsRelatedTo(quest));
		}

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
