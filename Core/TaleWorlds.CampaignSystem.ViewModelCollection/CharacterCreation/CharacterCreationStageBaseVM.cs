using System;
using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.CharacterCreation
{
	public abstract class CharacterCreationStageBaseVM : ViewModel
	{
		protected CharacterCreationStageBaseVM(CharacterCreation characterCreation, Action affirmativeAction, TextObject affirmativeActionText, Action negativeAction, TextObject negativeActionText, int currentStageIndex, int totalStagesCount, int furthestIndex, Action<int> goToIndex)
		{
			this._characterCreation = characterCreation;
			this._goToIndex = goToIndex;
			this._affirmativeAction = affirmativeAction;
			this._negativeAction = negativeAction;
			this._affirmativeActionText = affirmativeActionText;
			this._negativeActionText = negativeActionText;
			this.TotalStageCount = totalStagesCount;
			this.CurrentStageIndex = currentStageIndex;
			this.FurthestIndex = furthestIndex;
		}

		public abstract void OnNextStage();

		public abstract void OnPreviousStage();

		public abstract bool CanAdvanceToNextStage();

		public virtual void ExecuteGoToIndex(int index)
		{
			this._goToIndex(index);
		}

		public bool CanAdvance
		{
			get
			{
				return this.CanAdvanceToNextStage();
			}
		}

		public string NextStageText
		{
			get
			{
				return this._affirmativeActionText.ToString();
			}
		}

		public string PreviousStageText
		{
			get
			{
				return this._negativeActionText.ToString();
			}
		}

		[DataSourceProperty]
		public string Title
		{
			get
			{
				return this._title;
			}
			set
			{
				if (value != this._title)
				{
					this._title = value;
					base.OnPropertyChangedWithValue<string>(value, "Title");
				}
			}
		}

		[DataSourceProperty]
		public string Description
		{
			get
			{
				return this._description;
			}
			set
			{
				if (value != this._description)
				{
					this._description = value;
					base.OnPropertyChangedWithValue<string>(value, "Description");
				}
			}
		}

		[DataSourceProperty]
		public string SelectionText
		{
			get
			{
				return this._selectionText;
			}
			set
			{
				if (value != this._selectionText)
				{
					this._selectionText = value;
					base.OnPropertyChangedWithValue<string>(value, "SelectionText");
				}
			}
		}

		[DataSourceProperty]
		public int TotalStageCount
		{
			get
			{
				return this._totalStageCount;
			}
			set
			{
				if (value != this._totalStageCount)
				{
					this._totalStageCount = value;
					base.OnPropertyChangedWithValue(value, "TotalStageCount");
				}
			}
		}

		[DataSourceProperty]
		public int FurthestIndex
		{
			get
			{
				return this._furthestIndex;
			}
			set
			{
				if (value != this._furthestIndex)
				{
					this._furthestIndex = value;
					base.OnPropertyChangedWithValue(value, "FurthestIndex");
				}
			}
		}

		[DataSourceProperty]
		public int CurrentStageIndex
		{
			get
			{
				return this._currentStageIndex;
			}
			set
			{
				if (value != this._currentStageIndex)
				{
					this._currentStageIndex = value;
					base.OnPropertyChangedWithValue(value, "CurrentStageIndex");
				}
			}
		}

		[DataSourceProperty]
		public bool AnyItemSelected
		{
			get
			{
				return this._anyItemSelected;
			}
			set
			{
				if (value != this._anyItemSelected)
				{
					this._anyItemSelected = value;
					base.OnPropertyChangedWithValue(value, "AnyItemSelected");
				}
			}
		}

		protected readonly CharacterCreation _characterCreation;

		protected readonly Action _affirmativeAction;

		protected readonly Action _negativeAction;

		protected readonly TextObject _affirmativeActionText;

		protected readonly TextObject _negativeActionText;

		private readonly Action<int> _goToIndex;

		private string _title = "";

		private string _description = "";

		private string _selectionText = "";

		private int _totalStageCount = -1;

		private int _currentStageIndex = -1;

		private int _furthestIndex = -1;

		private bool _anyItemSelected;
	}
}
