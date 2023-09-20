using System;
using System.Collections.Generic;
using StoryMode.Missions;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace StoryMode.ViewModelCollection.Missions
{
	public class TrainingFieldObjectivesVM : ViewModel
	{
		public TrainingFieldObjectivesVM()
		{
			this.ObjectiveItems = new MBBindingList<TrainingFieldObjectiveItemVM>();
			this.RefreshValues();
			this.UpdateIsGamepadActive();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.CurrentObjectiveText = ((this._objectiveText == null) ? "" : this._objectiveText.ToString());
			string keyHyperlinkText = HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("Generic", 4));
			GameTexts.SetVariable("LEAVE_KEY", keyHyperlinkText);
			GameTexts.SetVariable("newline", "\n");
			this.LeaveAnyTimeText = GameTexts.FindText("str_leave_training_field", null).ToString();
			this.RightStickAbbreviatedText = new TextObject("{=rightstickabbreviated}RS", null).ToString();
			this.ObjectiveItems.ApplyActionOnAllItems(delegate(TrainingFieldObjectiveItemVM o)
			{
				o.RefreshValues();
			});
		}

		public void UpdateObjectivesWith(List<TrainingFieldMissionController.TutorialObjective> objectives)
		{
			this.ObjectiveItems.Clear();
			foreach (TrainingFieldMissionController.TutorialObjective tutorialObjective in objectives)
			{
				this.ObjectiveItems.Add(TrainingFieldObjectiveItemVM.CreateFromObjective(tutorialObjective));
			}
		}

		public void UpdateCurrentObjectiveText(TextObject currentObjectiveText)
		{
			this._objectiveText = currentObjectiveText;
			this.CurrentObjectiveText = ((this._objectiveText == null) ? "" : this._objectiveText.ToString());
		}

		public void UpdateCurrentMouseObjective(TrainingFieldMissionController.MouseObjectives currentMouseObjective)
		{
			this.CurrentMouseObjective = (int)currentMouseObjective;
		}

		public void UpdateTimerText(string timerText)
		{
			this.TimerText = (string.IsNullOrEmpty(timerText) ? "" : timerText);
		}

		public void UpdateIsGamepadActive()
		{
			this.IsGamepadActive = Input.IsGamepadActive;
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

		[DataSourceProperty]
		public string CurrentObjectiveText
		{
			get
			{
				return this._currentObjectiveText;
			}
			set
			{
				if (value != this._currentObjectiveText)
				{
					this._currentObjectiveText = value;
					base.OnPropertyChangedWithValue<string>(value, "CurrentObjectiveText");
				}
			}
		}

		[DataSourceProperty]
		public string TimerText
		{
			get
			{
				return this._timerText;
			}
			set
			{
				if (value != this._timerText)
				{
					this._timerText = value;
					base.OnPropertyChangedWithValue<string>(value, "TimerText");
				}
			}
		}

		[DataSourceProperty]
		public string LeaveAnyTimeText
		{
			get
			{
				return this._leaveAnyTimeText;
			}
			set
			{
				if (value != this._leaveAnyTimeText)
				{
					this._leaveAnyTimeText = value;
					base.OnPropertyChangedWithValue<string>(value, "LeaveAnyTimeText");
				}
			}
		}

		[DataSourceProperty]
		public int CurrentMouseObjective
		{
			get
			{
				return this._currentMouseObjective;
			}
			set
			{
				if (value != this._currentMouseObjective)
				{
					this._currentMouseObjective = value;
					base.OnPropertyChangedWithValue(value, "CurrentMouseObjective");
				}
			}
		}

		[DataSourceProperty]
		public string RightStickAbbreviatedText
		{
			get
			{
				return this._rightStickAbbreviatedText;
			}
			set
			{
				if (value != this._rightStickAbbreviatedText)
				{
					this._rightStickAbbreviatedText = value;
					base.OnPropertyChangedWithValue<string>(value, "RightStickAbbreviatedText");
				}
			}
		}

		[DataSourceProperty]
		public bool IsGamepadActive
		{
			get
			{
				return this._isGamepadActive;
			}
			set
			{
				if (value != this._isGamepadActive)
				{
					this._isGamepadActive = value;
					base.OnPropertyChangedWithValue(value, "IsGamepadActive");
				}
			}
		}

		private TextObject _objectiveText;

		private MBBindingList<TrainingFieldObjectiveItemVM> _objectiveItems;

		private string _currentObjectiveText;

		private string _leaveAnyTimeText;

		private string _timerText;

		private string _rightStickAbbreviatedText;

		private int _currentMouseObjective;

		private bool _isGamepadActive;
	}
}
