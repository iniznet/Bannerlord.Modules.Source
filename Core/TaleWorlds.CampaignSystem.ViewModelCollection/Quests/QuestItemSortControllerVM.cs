using System;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Quests
{
	public class QuestItemSortControllerVM : ViewModel
	{
		public QuestItemSortControllerVM.QuestItemSortOption? CurrentSortOption { get; private set; }

		public QuestItemSortControllerVM(ref MBBindingList<QuestItemVM> listToControl)
		{
			this._listToControl = listToControl;
			this._dateStartedComparer = new QuestItemSortControllerVM.QuestItemDateStartedComparer();
			this._lastUpdatedComparer = new QuestItemSortControllerVM.QuestItemLastUpdatedComparer();
			this._timeDueComparer = new QuestItemSortControllerVM.QuestItemTimeDueComparer();
			this.IsThereAnyQuest = this._listToControl.Count > 0;
		}

		private void ExecuteSortByDateStarted()
		{
			this._listToControl.Sort(this._dateStartedComparer);
			this.CurrentSortOption = new QuestItemSortControllerVM.QuestItemSortOption?(QuestItemSortControllerVM.QuestItemSortOption.DateStarted);
		}

		private void ExecuteSortByLastUpdated()
		{
			this._listToControl.Sort(this._lastUpdatedComparer);
			this.CurrentSortOption = new QuestItemSortControllerVM.QuestItemSortOption?(QuestItemSortControllerVM.QuestItemSortOption.LastUpdated);
		}

		private void ExecuteSortByTimeDue()
		{
			this._listToControl.Sort(this._timeDueComparer);
			this.CurrentSortOption = new QuestItemSortControllerVM.QuestItemSortOption?(QuestItemSortControllerVM.QuestItemSortOption.TimeDue);
		}

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

		private MBBindingList<QuestItemVM> _listToControl;

		private QuestItemSortControllerVM.QuestItemDateStartedComparer _dateStartedComparer;

		private QuestItemSortControllerVM.QuestItemLastUpdatedComparer _lastUpdatedComparer;

		private QuestItemSortControllerVM.QuestItemTimeDueComparer _timeDueComparer;

		private bool _isThereAnyQuest;

		public enum QuestItemSortOption
		{
			DateStarted,
			LastUpdated,
			TimeDue
		}

		private abstract class QuestItemComparerBase : IComparer<QuestItemVM>
		{
			public abstract int Compare(QuestItemVM x, QuestItemVM y);

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

			protected enum JournalLogIndex
			{
				First,
				Last
			}
		}

		private class QuestItemDateStartedComparer : QuestItemSortControllerVM.QuestItemComparerBase
		{
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

		private class QuestItemLastUpdatedComparer : QuestItemSortControllerVM.QuestItemComparerBase
		{
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

		private class QuestItemTimeDueComparer : QuestItemSortControllerVM.QuestItemComparerBase
		{
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
