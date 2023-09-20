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
	// Token: 0x0200012E RID: 302
	public class CharacterCreationOptionsStageVM : CharacterCreationStageBaseVM
	{
		// Token: 0x06001CBC RID: 7356 RVA: 0x00066F6C File Offset: 0x0006516C
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

		// Token: 0x06001CBD RID: 7357 RVA: 0x00066FF5 File Offset: 0x000651F5
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.OptionsController.RefreshValues();
		}

		// Token: 0x06001CBE RID: 7358 RVA: 0x00067008 File Offset: 0x00065208
		private void OnOptionChange(string identifier)
		{
		}

		// Token: 0x06001CBF RID: 7359 RVA: 0x0006700A File Offset: 0x0006520A
		public override bool CanAdvanceToNextStage()
		{
			return true;
		}

		// Token: 0x06001CC0 RID: 7360 RVA: 0x0006700D File Offset: 0x0006520D
		public override void OnNextStage()
		{
			this._affirmativeAction();
		}

		// Token: 0x06001CC1 RID: 7361 RVA: 0x0006701A File Offset: 0x0006521A
		public override void OnPreviousStage()
		{
			this._negativeAction();
		}

		// Token: 0x06001CC2 RID: 7362 RVA: 0x00067027 File Offset: 0x00065227
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

		// Token: 0x06001CC3 RID: 7363 RVA: 0x00067050 File Offset: 0x00065250
		public void SetCancelInputKey(HotKey hotKey)
		{
			this.CancelInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x06001CC4 RID: 7364 RVA: 0x0006705F File Offset: 0x0006525F
		public void SetDoneInputKey(HotKey hotKey)
		{
			this.DoneInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x170009D9 RID: 2521
		// (get) Token: 0x06001CC5 RID: 7365 RVA: 0x0006706E File Offset: 0x0006526E
		// (set) Token: 0x06001CC6 RID: 7366 RVA: 0x00067076 File Offset: 0x00065276
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

		// Token: 0x170009DA RID: 2522
		// (get) Token: 0x06001CC7 RID: 7367 RVA: 0x00067094 File Offset: 0x00065294
		// (set) Token: 0x06001CC8 RID: 7368 RVA: 0x0006709C File Offset: 0x0006529C
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

		// Token: 0x170009DB RID: 2523
		// (get) Token: 0x06001CC9 RID: 7369 RVA: 0x000670BA File Offset: 0x000652BA
		// (set) Token: 0x06001CCA RID: 7370 RVA: 0x000670C2 File Offset: 0x000652C2
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

		// Token: 0x04000D8C RID: 3468
		private InputKeyItemVM _cancelInputKey;

		// Token: 0x04000D8D RID: 3469
		private InputKeyItemVM _doneInputKey;

		// Token: 0x04000D8E RID: 3470
		private CampaignOptionsControllerVM _optionsController;
	}
}
