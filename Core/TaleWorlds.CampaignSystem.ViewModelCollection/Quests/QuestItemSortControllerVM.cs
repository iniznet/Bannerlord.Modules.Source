using System;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Quests
{
	// Token: 0x0200001D RID: 29
	public class QuestItemSortControllerVM : ViewModel
	{
		// Token: 0x1700004A RID: 74
		// (get) Token: 0x060001A7 RID: 423 RVA: 0x0000F771 File Offset: 0x0000D971
		// (set) Token: 0x060001A8 RID: 424 RVA: 0x0000F779 File Offset: 0x0000D979
		public QuestItemSortControllerVM.QuestItemSortOption? CurrentSortOption { get; private set; }

		// Token: 0x060001A9 RID: 425 RVA: 0x0000F784 File Offset: 0x0000D984
		public QuestItemSortControllerVM(ref MBBindingList<QuestItemVM> listToControl)
		{
			this._listToControl = listToControl;
			this._dateStartedComparer = new QuestItemSortControllerVM.QuestItemDateStartedComparer();
			this._lastUpdatedComparer = new QuestItemSortControllerVM.QuestItemLastUpdatedComparer();
			this._timeDueComparer = new QuestItemSortControllerVM.QuestItemTimeDueComparer();
			this.IsThereAnyQuest = this._listToControl.Count > 0;
		}

		// Token: 0x060001AA RID: 426 RVA: 0x0000F7D4 File Offset: 0x0000D9D4
		private void ExecuteSortByDateStarted()
		{
			this._listToControl.Sort(this._dateStartedComparer);
			this.CurrentSortOption = new QuestItemSortControllerVM.QuestItemSortOption?(QuestItemSortControllerVM.QuestItemSortOption.DateStarted);
		}

		// Token: 0x060001AB RID: 427 RVA: 0x0000F7F3 File Offset: 0x0000D9F3
		private void ExecuteSortByLastUpdated()
		{
			this._listToControl.Sort(this._lastUpdatedComparer);
			this.CurrentSortOption = new QuestItemSortControllerVM.QuestItemSortOption?(QuestItemSortControllerVM.QuestItemSortOption.LastUpdated);
		}

		// Token: 0x060001AC RID: 428 RVA: 0x0000F812 File Offset: 0x0000DA12
		private void ExecuteSortByTimeDue()
		{
			this._listToControl.Sort(this._timeDueComparer);
			this.CurrentSortOption = new QuestItemSortControllerVM.QuestItemSortOption?(QuestItemSortControllerVM.QuestItemSortOption.TimeDue);
		}

		// Token: 0x060001AD RID: 429 RVA: 0x0000F831 File Offset: 0x0000DA31
		public void SortByOption(QuestItemSortControllerVM.QuestItemSortOption sortOption)
		{
			if (sortOption == QuestItemSortControllerVM.QuestItemSortOption.DateStarted)
			{
				this.ExecuteSortByDateStarted();
				return;
			}
			if (sortOption == QuestItemSortControllerVM.QuestItemSortOption.LastUpdated)
			{
				this.ExecuteSortByLastUpdated();
				return;
			}
			if (sortOption == QuestItemSortControllerVM.QuestItemSortOption.TimeDue)
			{
				this.ExecuteSortByTimeDue();
			}
		}

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x060001AE RID: 430 RVA: 0x0000F852 File Offset: 0x0000DA52
		// (set) Token: 0x060001AF RID: 431 RVA: 0x0000F85A File Offset: 0x0000DA5A
		[DataSourceProperty]
		public bool IsThereAnyQuest
		{
			get
			{
				return this._isThereAnyQuest;
			}
			set
			{
				if (value != this._isThereAnyQuest)
				{
					this._isThereAnyQuest = value;
					base.OnPropertyChangedWithValue(value, "IsThereAnyQuest");
				}
			}
		}

		// Token: 0x040000BE RID: 190
		private MBBindingList<QuestItemVM> _listToControl;

		// Token: 0x040000BF RID: 191
		private QuestItemSortControllerVM.QuestItemDateStartedComparer _dateStartedComparer;

		// Token: 0x040000C0 RID: 192
		private QuestItemSortControllerVM.QuestItemLastUpdatedComparer _lastUpdatedComparer;

		// Token: 0x040000C1 RID: 193
		private QuestItemSortControllerVM.QuestItemTimeDueComparer _timeDueComparer;

		// Token: 0x040000C3 RID: 195
		private bool _isThereAnyQuest;

		// Token: 0x02000157 RID: 343
		public enum QuestItemSortOption
		{
			// Token: 0x04000EAF RID: 3759
			DateStarted,
			// Token: 0x04000EB0 RID: 3760
			LastUpdated,
			// Token: 0x04000EB1 RID: 3761
			TimeDue
		}

		// Token: 0x02000158 RID: 344
		private abstract class QuestItemComparerBase : IComparer<QuestItemVM>
		{
			// Token: 0x06001EBC RID: 7868
			public abstract int Compare(QuestItemVM x, QuestItemVM y);

			// Token: 0x06001EBD RID: 7869 RVA: 0x0006DA88 File Offset: 0x0006BC88
			protected JournalLog GetJournalLogAt(QuestItemVM questItem, QuestItemSortControllerVM.QuestItemComparerBase.JournalLogIndex logIndex)
			{
				if (questItem.Quest == null && questItem.Stages.Count > 0)
				{
					int num = ((logIndex == QuestItemSortControllerVM.QuestItemComparerBase.JournalLogIndex.First) ? 0 : (questItem.Stages.Count - 1));
					return questItem.Stages[num].Log;
				}
				if (questItem.Quest != null && questItem.Quest.JournalEntries.Count > 0)
				{
					int num2 = ((logIndex == QuestItemSortControllerVM.QuestItemComparerBase.JournalLogIndex.First) ? 0 : (questItem.Quest.JournalEntries.Count - 1));
					return questItem.Quest.JournalEntries[num2];
				}
				return null;
			}

			// Token: 0x02000289 RID: 649
			protected enum JournalLogIndex
			{
				// Token: 0x040011CD RID: 4557
				First,
				// Token: 0x040011CE RID: 4558
				Last
			}
		}

		// Token: 0x02000159 RID: 345
		private class QuestItemDateStartedComparer : QuestItemSortControllerVM.QuestItemComparerBase
		{
			// Token: 0x06001EBF RID: 7871 RVA: 0x0006DB20 File Offset: 0x0006BD20
			public override int Compare(QuestItemVM first, QuestItemVM second)
			{
				JournalLog journalLogAt = base.GetJournalLogAt(first, QuestItemSortControllerVM.QuestItemComparerBase.JournalLogIndex.First);
				JournalLog journalLogAt2 = base.GetJournalLogAt(second, QuestItemSortControllerVM.QuestItemComparerBase.JournalLogIndex.First);
				if (journalLogAt != null && journalLogAt2 != null)
				{
					return journalLogAt.LogTime.CompareTo(journalLogAt2.LogTime);
				}
				if (journalLogAt == null && journalLogAt2 != null)
				{
					return -1;
				}
				if (journalLogAt != null && journalLogAt2 == null)
				{
					return 1;
				}
				return 0;
			}
		}

		// Token: 0x0200015A RID: 346
		private class QuestItemLastUpdatedComparer : QuestItemSortControllerVM.QuestItemComparerBase
		{
			// Token: 0x06001EC1 RID: 7873 RVA: 0x0006DB74 File Offset: 0x0006BD74
			public override int Compare(QuestItemVM first, QuestItemVM second)
			{
				JournalLog journalLogAt = base.GetJournalLogAt(first, QuestItemSortControllerVM.QuestItemComparerBase.JournalLogIndex.Last);
				JournalLog journalLogAt2 = base.GetJournalLogAt(second, QuestItemSortControllerVM.QuestItemComparerBase.JournalLogIndex.Last);
				if (journalLogAt != null && journalLogAt2 != null)
				{
					return journalLogAt2.LogTime.CompareTo(journalLogAt.LogTime);
				}
				if (journalLogAt == null && journalLogAt2 != null)
				{
					return -1;
				}
				if (journalLogAt != null && journalLogAt2 == null)
				{
					return 1;
				}
				return 0;
			}
		}

		// Token: 0x0200015B RID: 347
		private class QuestItemTimeDueComparer : QuestItemSortControllerVM.QuestItemComparerBase
		{
			// Token: 0x06001EC3 RID: 7875 RVA: 0x0006DBC8 File Offset: 0x0006BDC8
			public override int Compare(QuestItemVM first, QuestItemVM second)
			{
				CampaignTime campaignTime = CampaignTime.Now;
				CampaignTime campaignTime2 = CampaignTime.Now;
				if (first.Quest != null)
				{
					campaignTime = first.Quest.QuestDueTime;
				}
				if (second.Quest != null)
				{
					campaignTime2 = second.Quest.QuestDueTime;
				}
				return campaignTime.CompareTo(campaignTime2);
			}
		}
	}
}
