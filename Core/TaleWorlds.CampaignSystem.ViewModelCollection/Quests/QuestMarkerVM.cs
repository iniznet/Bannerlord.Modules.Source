using System;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Quests
{
	public class QuestMarkerVM : ViewModel
	{
		public TextObject QuestTitle { get; }

		public TextObject QuestHintText { get; }

		public CampaignUIHelper.IssueQuestFlags IssueQuestFlag { get; }

		public QuestMarkerVM(CampaignUIHelper.IssueQuestFlags issueQuestFlag, TextObject questTitle = null, TextObject questHintText = null)
		{
			this.IssueQuestFlag = issueQuestFlag;
			this.QuestMarkerType = (int)issueQuestFlag;
			this.QuestTitle = questTitle ?? TextObject.Empty;
			this.QuestHintText = questHintText;
			if (this.QuestHintText != null)
			{
				this.QuestHint = new HintViewModel(this.QuestHintText, null);
			}
			this.IsTrackMarker = issueQuestFlag == CampaignUIHelper.IssueQuestFlags.TrackedIssue || issueQuestFlag == CampaignUIHelper.IssueQuestFlags.TrackedStoryQuest;
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			if (!TextObject.IsNullOrEmpty(this.QuestHintText))
			{
				this.QuestHint = new HintViewModel(this.QuestHintText, null);
				return;
			}
			this.QuestHint = new HintViewModel();
		}

		[DataSourceProperty]
		public bool IsTrackMarker
		{
			get
			{
				return this._isTrackMarker;
			}
			set
			{
				if (value != this._isTrackMarker)
				{
					this._isTrackMarker = value;
					base.OnPropertyChangedWithValue(value, "IsTrackMarker");
				}
			}
		}

		[DataSourceProperty]
		public int QuestMarkerType
		{
			get
			{
				return this._questMarkerType;
			}
			set
			{
				if (value != this._questMarkerType)
				{
					this._questMarkerType = value;
					base.OnPropertyChangedWithValue(value, "QuestMarkerType");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel QuestHint
		{
			get
			{
				return this._questHint;
			}
			set
			{
				if (value != this._questHint)
				{
					this._questHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "QuestHint");
				}
			}
		}

		private bool _isTrackMarker;

		private int _questMarkerType;

		private HintViewModel _questHint;
	}
}
