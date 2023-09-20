using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.CampaignSystem.ViewModelCollection.Input;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.CharacterCreation.OptionsStage
{
	public class CharacterCreationOptionsStageVM : CharacterCreationStageBaseVM
	{
		public CharacterCreationOptionsStageVM(CharacterCreation characterCreation, Action affirmativeAction, TextObject affirmativeActionText, Action negativeAction, TextObject negativeActionText, int currentStageIndex, int totalStagesCount, int furthestIndex, Action<int> goToIndex)
			: base(characterCreation, affirmativeAction, affirmativeActionText, negativeAction, negativeActionText, currentStageIndex, totalStagesCount, furthestIndex, goToIndex)
		{
			base.Title = GameTexts.FindText("str_difficulty", null).ToString();
			base.Description = GameTexts.FindText("str_determine_difficulty", null).ToString();
			MBBindingList<CampaignOptionItemVM> mbbindingList = new MBBindingList<CampaignOptionItemVM>();
			List<ICampaignOptionData> characterCreationCampaignOptions = CampaignOptionsManager.GetCharacterCreationCampaignOptions();
			for (int i = 0; i < characterCreationCampaignOptions.Count; i++)
			{
				mbbindingList.Add(new CampaignOptionItemVM(characterCreationCampaignOptions[i]));
			}
			this.OptionsController = new CampaignOptionsControllerVM(mbbindingList);
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.OptionsController.RefreshValues();
		}

		private void OnOptionChange(string identifier)
		{
		}

		public override bool CanAdvanceToNextStage()
		{
			return true;
		}

		public override void OnNextStage()
		{
			this._affirmativeAction();
		}

		public override void OnPreviousStage()
		{
			this._negativeAction();
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
		public CampaignOptionsControllerVM OptionsController
		{
			get
			{
				return this._optionsController;
			}
			set
			{
				if (value != this._optionsController)
				{
					this._optionsController = value;
					base.OnPropertyChangedWithValue<CampaignOptionsControllerVM>(value, "OptionsController");
				}
			}
		}

		private InputKeyItemVM _cancelInputKey;

		private InputKeyItemVM _doneInputKey;

		private CampaignOptionsControllerVM _optionsController;
	}
}
