﻿using System;
using System.Collections.Generic;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.LogEntries
{
	public class IssueQuestLogEntry : LogEntry
	{
		internal static void AutoGeneratedStaticCollectObjectsIssueQuestLogEntry(object o, List<object> collectedObjects)
		{
			((IssueQuestLogEntry)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
		}

		protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
		{
			base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			collectedObjects.Add(this.IssueGiver);
			collectedObjects.Add(this.Antagonist);
		}

		internal static object AutoGeneratedGetMemberValueIssueGiver(object o)
		{
			return ((IssueQuestLogEntry)o).IssueGiver;
		}

		internal static object AutoGeneratedGetMemberValueAntagonist(object o)
		{
			return ((IssueQuestLogEntry)o).Antagonist;
		}

		internal static object AutoGeneratedGetMemberValueDetails(object o)
		{
			return ((IssueQuestLogEntry)o).Details;
		}

		public override CampaignTime KeepInHistoryTime
		{
			get
			{
				return CampaignTime.Weeks(1f);
			}
		}

		public IssueQuestLogEntry(Hero questGiver, Hero antagonist, QuestBase.QuestCompleteDetails status)
		{
			this.IssueGiver = questGiver;
			this.Antagonist = antagonist;
			this.Details = status;
		}

		public override void GetConversationScoreAndComment(Hero talkTroop, bool findString, out string comment, out ImportanceEnum score)
		{
			score = ImportanceEnum.Zero;
			comment = "";
			if (this.IssueGiver == talkTroop)
			{
				if (this.Details == QuestBase.QuestCompleteDetails.FailWithBetrayal)
				{
					score = ImportanceEnum.MatterOfLifeAndDeath;
					if (findString)
					{
						comment = "str_comment_quest_betrayed";
						return;
					}
				}
				else if (this.Details == QuestBase.QuestCompleteDetails.Success)
				{
					score = ImportanceEnum.VeryImportant;
					if (findString)
					{
						comment = "str_comment_quest_succeeded";
						return;
					}
				}
				else if (this.Details == QuestBase.QuestCompleteDetails.Fail || this.Details == QuestBase.QuestCompleteDetails.Timeout || this.Details == QuestBase.QuestCompleteDetails.Cancel)
				{
					score = ImportanceEnum.Important;
					if (findString)
					{
						comment = "str_comment_quest_failed";
						return;
					}
				}
				else if (this.Details == QuestBase.QuestCompleteDetails.Invalid)
				{
					score = ImportanceEnum.ReasonablyImportant;
					if (findString)
					{
						comment = "str_comment_quest_invalid";
						return;
					}
				}
			}
			else if (this.Antagonist == talkTroop && this.Details == QuestBase.QuestCompleteDetails.FailWithBetrayal)
			{
				score = ImportanceEnum.MatterOfLifeAndDeath;
				if (findString)
				{
					comment = "str_comment_quest_counteroffer_accepted";
				}
			}
		}

		[SaveableField(10)]
		public readonly Hero IssueGiver;

		[SaveableField(20)]
		public readonly Hero Antagonist;

		[SaveableField(30)]
		public QuestBase.QuestCompleteDetails Details;
	}
}