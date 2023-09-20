using System;
using StoryMode.Missions;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace StoryMode.ViewModelCollection.Missions
{
	// Token: 0x02000005 RID: 5
	public class TrainingFieldObjectiveItemVM : ViewModel
	{
		// Token: 0x06000051 RID: 81 RVA: 0x000029DA File Offset: 0x00000BDA
		private TrainingFieldObjectiveItemVM()
		{
		}

		// Token: 0x06000052 RID: 82 RVA: 0x000029E4 File Offset: 0x00000BE4
		private TrainingFieldObjectiveItemVM(TrainingFieldMissionController.TutorialObjective objective)
		{
			this._textObjectString = objective.GetNameString();
			this.IsActive = objective.IsActive;
			this.IsCompleted = objective.IsFinished;
			this._score = objective.Score;
			this.ObjectiveItems = new MBBindingList<TrainingFieldObjectiveItemVM>();
			if (objective.SubTasks != null)
			{
				foreach (TrainingFieldMissionController.TutorialObjective tutorialObjective in objective.SubTasks)
				{
					this.ObjectiveItems.Add(TrainingFieldObjectiveItemVM.CreateFromObjective(tutorialObjective));
				}
			}
			this.RefreshValues();
		}

		// Token: 0x06000053 RID: 83 RVA: 0x00002A90 File Offset: 0x00000C90
		public override void RefreshValues()
		{
			base.RefreshValues();
			if (this._textObjectString != "")
			{
				this.ObjectiveText = this._textObjectString;
				if (this._score != 0f)
				{
					TextObject textObject = GameTexts.FindText("str_tutorial_time_score", null);
					textObject.SetTextVariable("TIME_SCORE", this._score.ToString("0.0"));
					this.ObjectiveText += textObject.ToString();
				}
			}
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00002B0D File Offset: 0x00000D0D
		public static TrainingFieldObjectiveItemVM CreateFromObjective(TrainingFieldMissionController.TutorialObjective objective)
		{
			return new TrainingFieldObjectiveItemVM(objective);
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000055 RID: 85 RVA: 0x00002B15 File Offset: 0x00000D15
		// (set) Token: 0x06000056 RID: 86 RVA: 0x00002B1D File Offset: 0x00000D1D
		[DataSourceProperty]
		public string ObjectiveText
		{
			get
			{
				return this._objectiveText;
			}
			set
			{
				if (value != this._objectiveText)
				{
					this._objectiveText = value;
					base.OnPropertyChangedWithValue<string>(value, "ObjectiveText");
				}
			}
		}

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000057 RID: 87 RVA: 0x00002B40 File Offset: 0x00000D40
		// (set) Token: 0x06000058 RID: 88 RVA: 0x00002B48 File Offset: 0x00000D48
		[DataSourceProperty]
		public bool IsCompleted
		{
			get
			{
				return this._isCompleted;
			}
			set
			{
				if (value != this._isCompleted)
				{
					this._isCompleted = value;
					base.OnPropertyChangedWithValue(value, "IsCompleted");
				}
			}
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06000059 RID: 89 RVA: 0x00002B66 File Offset: 0x00000D66
		// (set) Token: 0x0600005A RID: 90 RVA: 0x00002B6E File Offset: 0x00000D6E
		[DataSourceProperty]
		public bool IsActive
		{
			get
			{
				return this._isActive;
			}
			set
			{
				if (value != this._isActive)
				{
					this._isActive = value;
					base.OnPropertyChangedWithValue(value, "IsActive");
				}
			}
		}

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x0600005B RID: 91 RVA: 0x00002B8C File Offset: 0x00000D8C
		// (set) Token: 0x0600005C RID: 92 RVA: 0x00002B94 File Offset: 0x00000D94
		[DataSourceProperty]
		public MBBindingList<TrainingFieldObjectiveItemVM> ObjectiveItems
		{
			get
			{
				return this._objectiveItems;
			}
			set
			{
				if (value != this._objectiveItems)
				{
					this._objectiveItems = value;
					base.OnPropertyChangedWithValue<MBBindingList<TrainingFieldObjectiveItemVM>>(value, "ObjectiveItems");
				}
			}
		}

		// Token: 0x04000026 RID: 38
		private string _textObjectString;

		// Token: 0x04000027 RID: 39
		private string _objectiveText;

		// Token: 0x04000028 RID: 40
		private bool _isCompleted;

		// Token: 0x04000029 RID: 41
		private bool _isActive;

		// Token: 0x0400002A RID: 42
		private float _score;

		// Token: 0x0400002B RID: 43
		private MBBindingList<TrainingFieldObjectiveItemVM> _objectiveItems;
	}
}
