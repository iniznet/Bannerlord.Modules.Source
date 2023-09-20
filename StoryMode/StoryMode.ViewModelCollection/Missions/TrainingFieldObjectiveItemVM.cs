using System;
using StoryMode.Missions;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace StoryMode.ViewModelCollection.Missions
{
	public class TrainingFieldObjectiveItemVM : ViewModel
	{
		private TrainingFieldObjectiveItemVM()
		{
		}

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

		public static TrainingFieldObjectiveItemVM CreateFromObjective(TrainingFieldMissionController.TutorialObjective objective)
		{
			return new TrainingFieldObjectiveItemVM(objective);
		}

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

		private string _textObjectString;

		private string _objectiveText;

		private bool _isCompleted;

		private bool _isActive;

		private float _score;

		private MBBindingList<TrainingFieldObjectiveItemVM> _objectiveItems;
	}
}
