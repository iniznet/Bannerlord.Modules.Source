using System;
using System.Linq;
using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.CampaignSystem.ViewModelCollection.Input;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.CharacterCreation
{
	// Token: 0x02000123 RID: 291
	public class CharacterCreationCultureStageVM : CharacterCreationStageBaseVM
	{
		// Token: 0x06001C19 RID: 7193 RVA: 0x00064E20 File Offset: 0x00063020
		public CharacterCreationCultureStageVM(CharacterCreation characterCreation, Action affirmativeAction, TextObject affirmativeActionText, Action negativeAction, TextObject negativeActionText, int currentStageIndex, int totalStagesCount, int furthestIndex, Action<int> goToIndex, Action<CultureObject> onCultureSelected)
			: base(characterCreation, affirmativeAction, affirmativeActionText, negativeAction, negativeActionText, currentStageIndex, totalStagesCount, furthestIndex, goToIndex)
		{
			this._onCultureSelected = onCultureSelected;
			CharacterCreationContentBase currentContent = (GameStateManager.Current.ActiveState as CharacterCreationState).CurrentCharacterCreationContent;
			this.Cultures = new MBBindingList<CharacterCreationCultureVM>();
			base.Title = GameTexts.FindText("str_culture", null).ToString();
			base.Description = new TextObject("{=fz2kQjFS}Choose your character's culture:", null).ToString();
			base.SelectionText = new TextObject("{=MaHMOzL2}Character Culture", null).ToString();
			foreach (CultureObject cultureObject in currentContent.GetCultures())
			{
				CharacterCreationCultureVM characterCreationCultureVM = new CharacterCreationCultureVM(cultureObject, new Action<CharacterCreationCultureVM>(this.OnCultureSelection));
				this.Cultures.Add(characterCreationCultureVM);
			}
			this.SortCultureList(this.Cultures);
			if (currentContent.GetSelectedCulture() != null)
			{
				CharacterCreationCultureVM characterCreationCultureVM2 = this.Cultures.FirstOrDefault((CharacterCreationCultureVM c) => c.Culture == currentContent.GetSelectedCulture());
				if (characterCreationCultureVM2 != null)
				{
					this.OnCultureSelection(characterCreationCultureVM2);
				}
			}
		}

		// Token: 0x06001C1A RID: 7194 RVA: 0x00064F50 File Offset: 0x00063150
		private void SortCultureList(MBBindingList<CharacterCreationCultureVM> listToWorkOn)
		{
			int num = listToWorkOn.IndexOf(listToWorkOn.Single((CharacterCreationCultureVM i) => i.CultureID.Contains("vlan")));
			this.Swap(listToWorkOn, num, 0);
			int num2 = listToWorkOn.IndexOf(listToWorkOn.Single((CharacterCreationCultureVM i) => i.CultureID.Contains("stur")));
			this.Swap(listToWorkOn, num2, 1);
			int num3 = listToWorkOn.IndexOf(listToWorkOn.Single((CharacterCreationCultureVM i) => i.CultureID.Contains("empi")));
			this.Swap(listToWorkOn, num3, 2);
			int num4 = listToWorkOn.IndexOf(listToWorkOn.Single((CharacterCreationCultureVM i) => i.CultureID.Contains("aser")));
			this.Swap(listToWorkOn, num4, 3);
			int num5 = listToWorkOn.IndexOf(listToWorkOn.Single((CharacterCreationCultureVM i) => i.CultureID.Contains("khuz")));
			this.Swap(listToWorkOn, num5, 4);
		}

		// Token: 0x06001C1B RID: 7195 RVA: 0x00065068 File Offset: 0x00063268
		public void OnCultureSelection(CharacterCreationCultureVM selectedCulture)
		{
			this.InitializePlayersFaceKeyAccordingToCultureSelection(selectedCulture);
			foreach (CharacterCreationCultureVM characterCreationCultureVM in this.Cultures.Where((CharacterCreationCultureVM c) => c.IsSelected))
			{
				characterCreationCultureVM.IsSelected = false;
			}
			selectedCulture.IsSelected = true;
			this.CurrentSelectedCulture = selectedCulture;
			base.AnyItemSelected = true;
			(GameStateManager.Current.ActiveState as CharacterCreationState).CurrentCharacterCreationContent.SetSelectedCulture(selectedCulture.Culture, this._characterCreation);
			base.OnPropertyChanged("CanAdvance");
			Action<CultureObject> onCultureSelected = this._onCultureSelected;
			if (onCultureSelected == null)
			{
				return;
			}
			onCultureSelected(selectedCulture.Culture);
		}

		// Token: 0x06001C1C RID: 7196 RVA: 0x0006513C File Offset: 0x0006333C
		private void InitializePlayersFaceKeyAccordingToCultureSelection(CharacterCreationCultureVM selectedCulture)
		{
			string text = "<BodyProperties version='4' age='25.84' weight='0.5000' build='0.5000'  key='000BAC088000100DB976648E6774B835537D86629511323BDCB177278A84F667017776140748B49500000000000000000000000000000000000000003EFC5002'/>";
			string text2 = "<BodyProperties version='4' age='25.84' weight='0.5000' build='0.5000'  key='000500000000000D797664884754DCBAA35E866295A0967774414A498C8336860F7776F20BA7B7A500000000000000000000000000000000000000003CFC2002'/>";
			string text3 = "<BodyProperties version='4' age='25.84' weight='0.5000' build='0.5000'  key='001CB80CC000300D7C7664876753888A7577866254C69643C4B647398C95A0370077760307A7497300000000000000000000000000000000000000003AF47002'/>";
			string text4 = "<BodyProperties version='4' age='25.84' weight='0.5000' build='0.5000'  key='0028C80FC000100DBA756445533377873CD1833B3101B44A21C3C5347CA32C260F7776F20BBC35E8000000000000000000000000000000000000000042F41002'/>";
			string text5 = "<BodyProperties version='4' age='25.84' weight='0.5000' build='0.5000'   key='0016F80E4000200EB8708BD6CDC85229D3698B3ABDFE344CD22D3DD5388988680F7776F20B96723B00000000000000000000000000000000000000003EF41002'/>";
			string text6 = "<BodyProperties version='4' age='25.84' weight='0.5000' build='0.5000'  key='000000058000200D79766434475CDCBAC34E866255A096777441DA49838BF6A50F7776F20BA7B7A500000000000000000000000000000000000000003CFC0002'/>";
			string text7;
			if (selectedCulture.Culture.StringId == "aserai")
			{
				text7 = text4;
			}
			else if (selectedCulture.Culture.StringId == "khuzait")
			{
				text7 = text5;
			}
			else if (selectedCulture.Culture.StringId == "vlandia")
			{
				text7 = text;
			}
			else if (selectedCulture.Culture.StringId == "sturgia")
			{
				text7 = text2;
			}
			else if (selectedCulture.Culture.StringId == "battania")
			{
				text7 = text6;
			}
			else if (selectedCulture.Culture.StringId == "empire")
			{
				text7 = text3;
			}
			else
			{
				text7 = text3;
			}
			BodyProperties bodyProperties;
			if (BodyProperties.FromString(text7, out bodyProperties))
			{
				CharacterObject.PlayerCharacter.UpdatePlayerCharacterBodyProperties(bodyProperties, CharacterObject.PlayerCharacter.Race, CharacterObject.PlayerCharacter.IsFemale);
			}
			CharacterObject.PlayerCharacter.Culture = selectedCulture.Culture;
		}

		// Token: 0x06001C1D RID: 7197 RVA: 0x00065264 File Offset: 0x00063464
		private void Swap(MBBindingList<CharacterCreationCultureVM> listToWorkOn, int swapFromIndex, int swapToIndex)
		{
			if (swapFromIndex != swapToIndex)
			{
				CharacterCreationCultureVM characterCreationCultureVM = listToWorkOn[swapToIndex];
				listToWorkOn[swapToIndex] = listToWorkOn[swapFromIndex];
				listToWorkOn[swapFromIndex] = characterCreationCultureVM;
			}
		}

		// Token: 0x06001C1E RID: 7198 RVA: 0x00065293 File Offset: 0x00063493
		public override void OnNextStage()
		{
			this._affirmativeAction();
		}

		// Token: 0x06001C1F RID: 7199 RVA: 0x000652A0 File Offset: 0x000634A0
		public override void OnPreviousStage()
		{
			this._negativeAction();
		}

		// Token: 0x06001C20 RID: 7200 RVA: 0x000652AD File Offset: 0x000634AD
		public override bool CanAdvanceToNextStage()
		{
			return this.Cultures.Any((CharacterCreationCultureVM s) => s.IsSelected);
		}

		// Token: 0x06001C21 RID: 7201 RVA: 0x000652D9 File Offset: 0x000634D9
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

		// Token: 0x06001C22 RID: 7202 RVA: 0x00065302 File Offset: 0x00063502
		public void SetCancelInputKey(HotKey hotKey)
		{
			this.CancelInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x06001C23 RID: 7203 RVA: 0x00065311 File Offset: 0x00063511
		public void SetDoneInputKey(HotKey hotKey)
		{
			this.DoneInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x170009A1 RID: 2465
		// (get) Token: 0x06001C24 RID: 7204 RVA: 0x00065320 File Offset: 0x00063520
		// (set) Token: 0x06001C25 RID: 7205 RVA: 0x00065328 File Offset: 0x00063528
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

		// Token: 0x170009A2 RID: 2466
		// (get) Token: 0x06001C26 RID: 7206 RVA: 0x00065346 File Offset: 0x00063546
		// (set) Token: 0x06001C27 RID: 7207 RVA: 0x0006534E File Offset: 0x0006354E
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

		// Token: 0x170009A3 RID: 2467
		// (get) Token: 0x06001C28 RID: 7208 RVA: 0x0006536C File Offset: 0x0006356C
		// (set) Token: 0x06001C29 RID: 7209 RVA: 0x00065374 File Offset: 0x00063574
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

		// Token: 0x170009A4 RID: 2468
		// (get) Token: 0x06001C2A RID: 7210 RVA: 0x00065392 File Offset: 0x00063592
		// (set) Token: 0x06001C2B RID: 7211 RVA: 0x0006539A File Offset: 0x0006359A
		[DataSourceProperty]
		public MBBindingList<CharacterCreationCultureVM> Cultures
		{
			get
			{
				return this._cultures;
			}
			set
			{
				if (value != this._cultures)
				{
					this._cultures = value;
					base.OnPropertyChangedWithValue<MBBindingList<CharacterCreationCultureVM>>(value, "Cultures");
				}
			}
		}

		// Token: 0x170009A5 RID: 2469
		// (get) Token: 0x06001C2C RID: 7212 RVA: 0x000653B8 File Offset: 0x000635B8
		// (set) Token: 0x06001C2D RID: 7213 RVA: 0x000653C0 File Offset: 0x000635C0
		[DataSourceProperty]
		public CharacterCreationCultureVM CurrentSelectedCulture
		{
			get
			{
				return this._currentSelectedCulture;
			}
			set
			{
				if (value != this._currentSelectedCulture)
				{
					this._currentSelectedCulture = value;
					base.OnPropertyChangedWithValue<CharacterCreationCultureVM>(value, "CurrentSelectedCulture");
				}
			}
		}

		// Token: 0x04000D42 RID: 3394
		private Action<CultureObject> _onCultureSelected;

		// Token: 0x04000D43 RID: 3395
		private InputKeyItemVM _cancelInputKey;

		// Token: 0x04000D44 RID: 3396
		private InputKeyItemVM _doneInputKey;

		// Token: 0x04000D45 RID: 3397
		private bool _isActive;

		// Token: 0x04000D46 RID: 3398
		private MBBindingList<CharacterCreationCultureVM> _cultures;

		// Token: 0x04000D47 RID: 3399
		private CharacterCreationCultureVM _currentSelectedCulture;
	}
}
