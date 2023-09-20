using System;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Quests
{
	// Token: 0x0200001F RID: 31
	public class QuestMarkerVM : ViewModel
	{
		// Token: 0x1700005F RID: 95
		// (get) Token: 0x060001DC RID: 476 RVA: 0x0001011C File Offset: 0x0000E31C
		public TextObject QuestTitle { get; }

		// Token: 0x17000060 RID: 96
		// (get) Token: 0x060001DD RID: 477 RVA: 0x00010124 File Offset: 0x0000E324
		public TextObject QuestHintText { get; }

		// Token: 0x17000061 RID: 97
		// (get) Token: 0x060001DE RID: 478 RVA: 0x0001012C File Offset: 0x0000E32C
		public CampaignUIHelper.IssueQuestFlags IssueQuestFlag { get; }

		// Token: 0x060001DF RID: 479 RVA: 0x00010134 File Offset: 0x0000E334
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

		// Token: 0x060001E0 RID: 480 RVA: 0x0001019E File Offset: 0x0000E39E
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

		// Token: 0x17000062 RID: 98
		// (get) Token: 0x060001E1 RID: 481 RVA: 0x000101D1 File Offset: 0x0000E3D1
		// (set) Token: 0x060001E2 RID: 482 RVA: 0x000101D9 File Offset: 0x0000E3D9
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

		// Token: 0x17000063 RID: 99
		// (get) Token: 0x060001E3 RID: 483 RVA: 0x000101F7 File Offset: 0x0000E3F7
		// (set) Token: 0x060001E4 RID: 484 RVA: 0x000101FF File Offset: 0x0000E3FF
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

		// Token: 0x17000064 RID: 100
		// (get) Token: 0x060001E5 RID: 485 RVA: 0x0001021D File Offset: 0x0000E41D
		// (set) Token: 0x060001E6 RID: 486 RVA: 0x00010225 File Offset: 0x0000E425
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

		// Token: 0x040000DC RID: 220
		private bool _isTrackMarker;

		// Token: 0x040000DD RID: 221
		private int _questMarkerType;

		// Token: 0x040000DE RID: 222
		private HintViewModel _questHint;
	}
}
