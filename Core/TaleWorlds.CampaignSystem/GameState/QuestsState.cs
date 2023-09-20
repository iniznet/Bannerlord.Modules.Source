using System;
using TaleWorlds.CampaignSystem.Issues;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.GameState
{
	public class QuestsState : GameState
	{
		public IssueBase InitialSelectedIssue { get; private set; }

		public QuestBase InitialSelectedQuest { get; private set; }

		public JournalLogEntry InitialSelectedLog { get; private set; }

		public override bool IsMenuState
		{
			get
			{
				return true;
			}
		}

		public IQuestsStateHandler Handler
		{
			get
			{
				return this._handler;
			}
			set
			{
				this._handler = value;
			}
		}

		public QuestsState()
		{
		}

		public QuestsState(IssueBase initialSelectedIssue)
		{
			this.InitialSelectedIssue = initialSelectedIssue;
		}

		public QuestsState(QuestBase initialSelectedQuest)
		{
			this.InitialSelectedQuest = initialSelectedQuest;
		}

		public QuestsState(JournalLogEntry initialSelectedLog)
		{
			this.InitialSelectedLog = initialSelectedLog;
		}

		private IQuestsStateHandler _handler;
	}
}
