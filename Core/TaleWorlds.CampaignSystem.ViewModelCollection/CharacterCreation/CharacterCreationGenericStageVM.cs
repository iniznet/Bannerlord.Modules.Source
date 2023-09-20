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
	// Token: 0x02000129 RID: 297
	public class CharacterCreationGenericStageVM : CharacterCreationStageBaseVM
	{
		// Token: 0x06001C66 RID: 7270 RVA: 0x00066178 File Offset: 0x00064378
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

		// Token: 0x06001C67 RID: 7271 RVA: 0x00066298 File Offset: 0x00064498
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

		// Token: 0x06001C68 RID: 7272 RVA: 0x00066378 File Offset: 0x00064578
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

		// Token: 0x06001C69 RID: 7273 RVA: 0x000663F4 File Offset: 0x000645F4
		public override void OnNextStage()
		{
			this._affirmativeAction();
		}

		// Token: 0x06001C6A RID: 7274 RVA: 0x00066401 File Offset: 0x00064601
		public override void OnPreviousStage()
		{
			this._negativeAction();
		}

		// Token: 0x06001C6B RID: 7275 RVA: 0x0006640E File Offset: 0x0006460E
		public override bool CanAdvanceToNextStage()
		{
			if (this.SelectionList.Count != 0)
			{
				return this.SelectionList.Any((CharacterCreationOptionVM s) => s.IsSelected);
			}
			return true;
		}

		// Token: 0x06001C6C RID: 7276 RVA: 0x00066449 File Offset: 0x00064649
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

		// Token: 0x06001C6D RID: 7277 RVA: 0x00066472 File Offset: 0x00064672
		public void SetCancelInputKey(HotKey hotKey)
		{
			this.CancelInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x06001C6E RID: 7278 RVA: 0x00066481 File Offset: 0x00064681
		public void SetDoneInputKey(HotKey hotKey)
		{
			this.DoneInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x170009BA RID: 2490
		// (get) Token: 0x06001C6F RID: 7279 RVA: 0x00066490 File Offset: 0x00064690
		// (set) Token: 0x06001C70 RID: 7280 RVA: 0x00066498 File Offset: 0x00064698
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

		// Token: 0x170009BB RID: 2491
		// (get) Token: 0x06001C71 RID: 7281 RVA: 0x000664B6 File Offset: 0x000646B6
		// (set) Token: 0x06001C72 RID: 7282 RVA: 0x000664BE File Offset: 0x000646BE
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

		// Token: 0x170009BC RID: 2492
		// (get) Token: 0x06001C73 RID: 7283 RVA: 0x000664DC File Offset: 0x000646DC
		// (set) Token: 0x06001C74 RID: 7284 RVA: 0x000664E4 File Offset: 0x000646E4
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

		// Token: 0x170009BD RID: 2493
		// (get) Token: 0x06001C75 RID: 7285 RVA: 0x00066502 File Offset: 0x00064702
		// (set) Token: 0x06001C76 RID: 7286 RVA: 0x0006650A File Offset: 0x0006470A
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

		// Token: 0x170009BE RID: 2494
		// (get) Token: 0x06001C77 RID: 7287 RVA: 0x00066528 File Offset: 0x00064728
		// (set) Token: 0x06001C78 RID: 7288 RVA: 0x00066530 File Offset: 0x00064730
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

		// Token: 0x170009BF RID: 2495
		// (get) Token: 0x06001C79 RID: 7289 RVA: 0x00066553 File Offset: 0x00064753
		// (set) Token: 0x06001C7A RID: 7290 RVA: 0x0006655B File Offset: 0x0006475B
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

		// Token: 0x170009C0 RID: 2496
		// (get) Token: 0x06001C7B RID: 7291 RVA: 0x0006657E File Offset: 0x0006477E
		// (set) Token: 0x06001C7C RID: 7292 RVA: 0x00066586 File Offset: 0x00064786
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

		// Token: 0x04000D64 RID: 3428
		private readonly int _stageIndex;

		// Token: 0x04000D65 RID: 3429
		public Action OnOptionSelection;

		// Token: 0x04000D66 RID: 3430
		private CharacterCreationOption _selectedOption;

		// Token: 0x04000D67 RID: 3431
		private bool _isRefreshing;

		// Token: 0x04000D68 RID: 3432
		private InputKeyItemVM _cancelInputKey;

		// Token: 0x04000D69 RID: 3433
		private InputKeyItemVM _doneInputKey;

		// Token: 0x04000D6A RID: 3434
		private MBBindingList<CharacterCreationOptionVM> _selectionList;

		// Token: 0x04000D6B RID: 3435
		private CharacterCreationGainedPropertiesVM _gainedPropertiesController;

		// Token: 0x04000D6C RID: 3436
		private string _positiveEffectText;

		// Token: 0x04000D6D RID: 3437
		private string _negativeEffectText;

		// Token: 0x04000D6E RID: 3438
		private string _descriptionText;
	}
}
