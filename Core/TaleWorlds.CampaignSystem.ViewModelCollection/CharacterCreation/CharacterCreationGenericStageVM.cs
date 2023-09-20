using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.CampaignSystem.ViewModelCollection.Input;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.CharacterCreation
{
	public class CharacterCreationGenericStageVM : CharacterCreationStageBaseVM
	{
		public CharacterCreationGenericStageVM(CharacterCreation characterCreationMenu, Action affirmativeAction, TextObject affirmativeActionText, Action negativeAction, TextObject negativeActionText, int stageIndex, int currentStageIndex, int totalStagesCount, int furthestIndex, Action<int> goToIndex)
			: base(characterCreationMenu, affirmativeAction, affirmativeActionText, negativeAction, negativeActionText, currentStageIndex, totalStagesCount, furthestIndex, goToIndex)
		{
			this._stageIndex = stageIndex;
			this.SelectionList = new MBBindingList<CharacterCreationOptionVM>();
			this._characterCreation.OnInit(stageIndex);
			base.Title = this._characterCreation.GetCurrentMenuTitle(stageIndex).ToString();
			base.Description = this._characterCreation.GetCurrentMenuText(stageIndex).ToString();
			GameTexts.SetVariable("SELECTION", base.Title);
			base.SelectionText = GameTexts.FindText("str_char_creation_generic_selection", null).ToString();
			foreach (CharacterCreationOption characterCreationOption in this._characterCreation.GetCurrentMenuOptions(stageIndex))
			{
				CharacterCreationOptionVM characterCreationOptionVM = new CharacterCreationOptionVM(new Action<object>(this.ApplySelection), characterCreationOption.Text.ToString(), characterCreationOption);
				this.SelectionList.Add(characterCreationOptionVM);
			}
			this.RefreshSelectedOptions();
			this.GainedPropertiesController = new CharacterCreationGainedPropertiesVM(this._characterCreation, this._stageIndex);
		}

		public void RefreshSelectedOptions()
		{
			this._isRefreshing = true;
			IEnumerable<int> selectedOptions = this._characterCreation.GetSelectedOptions(this._stageIndex);
			foreach (CharacterCreationOptionVM characterCreationOptionVM in this.SelectionList)
			{
				CharacterCreationOption characterCreationOption = (CharacterCreationOption)characterCreationOptionVM.Identifier;
				characterCreationOptionVM.IsSelected = selectedOptions.Contains(characterCreationOption.Id);
				if (characterCreationOptionVM.IsSelected)
				{
					this.PositiveEffectText = characterCreationOption.PositiveEffectText.ToString();
					this.DescriptionText = characterCreationOption.DescriptionText.ToString();
					base.AnyItemSelected = true;
					this._selectedOption = characterCreationOption;
					this._characterCreation.RunConsequence(this._selectedOption, this._stageIndex, false);
				}
			}
			this._isRefreshing = false;
			base.OnPropertyChanged("CanAdvance");
		}

		public void ApplySelection(object optionObject)
		{
			CharacterCreationOption characterCreationOption = optionObject as CharacterCreationOption;
			if (characterCreationOption == null || this._isRefreshing || this._selectedOption == characterCreationOption)
			{
				return;
			}
			this._selectedOption = characterCreationOption;
			this._characterCreation.RunConsequence(this._selectedOption, this._stageIndex, false);
			this.RefreshSelectedOptions();
			Action onOptionSelection = this.OnOptionSelection;
			if (onOptionSelection != null)
			{
				onOptionSelection();
			}
			base.AnyItemSelected = true;
			base.OnPropertyChanged("CanAdvance");
			this.GainedPropertiesController.UpdateValues();
		}

		public override void OnNextStage()
		{
			this._affirmativeAction();
		}

		public override void OnPreviousStage()
		{
			this._negativeAction();
		}

		public override bool CanAdvanceToNextStage()
		{
			if (this.SelectionList.Count != 0)
			{
				return this.SelectionList.Any((CharacterCreationOptionVM s) => s.IsSelected);
			}
			return true;
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			InputKeyItemVM cancelInputKey = this.CancelInputKey;
			if (cancelInputKey != null)
			{
				cancelInputKey.OnFinalize();
			}
			InputKeyItemVM doneInputKey = this.DoneInputKey;
			if (doneInputKey == null)
			{
				return;
			}
			doneInputKey.OnFinalize();
		}

		public void SetCancelInputKey(HotKey hotKey)
		{
			this.CancelInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		public void SetDoneInputKey(HotKey hotKey)
		{
			this.DoneInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		[DataSourceProperty]
		public InputKeyItemVM CancelInputKey
		{
			get
			{
				return this._cancelInputKey;
			}
			set
			{
				if (value != this._cancelInputKey)
				{
					this._cancelInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "CancelInputKey");
				}
			}
		}

		[DataSourceProperty]
		public InputKeyItemVM DoneInputKey
		{
			get
			{
				return this._doneInputKey;
			}
			set
			{
				if (value != this._doneInputKey)
				{
					this._doneInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "DoneInputKey");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<CharacterCreationOptionVM> SelectionList
		{
			get
			{
				return this._selectionList;
			}
			set
			{
				if (value != this._selectionList)
				{
					this._selectionList = value;
					base.OnPropertyChangedWithValue<MBBindingList<CharacterCreationOptionVM>>(value, "SelectionList");
				}
			}
		}

		[DataSourceProperty]
		public CharacterCreationGainedPropertiesVM GainedPropertiesController
		{
			get
			{
				return this._gainedPropertiesController;
			}
			set
			{
				if (value != this._gainedPropertiesController)
				{
					this._gainedPropertiesController = value;
					base.OnPropertyChangedWithValue<CharacterCreationGainedPropertiesVM>(value, "GainedPropertiesController");
				}
			}
		}

		[DataSourceProperty]
		public string PositiveEffectText
		{
			get
			{
				return this._positiveEffectText;
			}
			set
			{
				if (value != this._positiveEffectText)
				{
					this._positiveEffectText = value;
					base.OnPropertyChangedWithValue<string>(value, "PositiveEffectText");
				}
			}
		}

		[DataSourceProperty]
		public string NegativeEffectText
		{
			get
			{
				return this._negativeEffectText;
			}
			set
			{
				if (value != this._negativeEffectText)
				{
					this._negativeEffectText = value;
					base.OnPropertyChangedWithValue<string>(value, "NegativeEffectText");
				}
			}
		}

		[DataSourceProperty]
		public string DescriptionText
		{
			get
			{
				return this._descriptionText;
			}
			set
			{
				if (value != this._descriptionText)
				{
					this._descriptionText = value;
					base.OnPropertyChangedWithValue<string>(value, "DescriptionText");
				}
			}
		}

		private readonly int _stageIndex;

		public Action OnOptionSelection;

		private CharacterCreationOption _selectedOption;

		private bool _isRefreshing;

		private InputKeyItemVM _cancelInputKey;

		private InputKeyItemVM _doneInputKey;

		private MBBindingList<CharacterCreationOptionVM> _selectionList;

		private CharacterCreationGainedPropertiesVM _gainedPropertiesController;

		private string _positiveEffectText;

		private string _negativeEffectText;

		private string _descriptionText;
	}
}
