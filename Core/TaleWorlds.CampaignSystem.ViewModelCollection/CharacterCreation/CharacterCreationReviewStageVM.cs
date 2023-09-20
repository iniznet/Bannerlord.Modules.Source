using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ViewModelCollection.Input;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.CharacterCreation
{
	// Token: 0x0200012C RID: 300
	public class CharacterCreationReviewStageVM : CharacterCreationStageBaseVM
	{
		// Token: 0x06001C8C RID: 7308 RVA: 0x000666E4 File Offset: 0x000648E4
		public CharacterCreationReviewStageVM(CharacterCreation characterCreation, Action affirmativeAction, TextObject affirmativeActionText, Action negativeAction, TextObject negativeActionText, int currentStageIndex, int totalStagesCount, int furthestIndex, Action<int> goToIndex, bool isBannerAndClanNameSet)
			: base(characterCreation, affirmativeAction, affirmativeActionText, negativeAction, negativeActionText, currentStageIndex, totalStagesCount, furthestIndex, goToIndex)
		{
			this.ReviewList = new MBBindingList<CharacterCreationReviewStageItemVM>();
			base.Title = new TextObject("{=txjiykNa}Review", null).ToString();
			base.Description = CharacterCreationContentBase.Instance.ReviewPageDescription.ToString();
			this._currentContent = (GameStateManager.Current.ActiveState as CharacterCreationState).CurrentCharacterCreationContent;
			this._isBannerAndClanNameSet = isBannerAndClanNameSet;
			this.Name = this._characterCreation.Name;
			this.NameTextQuestion = new TextObject("{=mHVmrwRQ}Enter your name", null).ToString();
			this.AddReviewedItems();
			this.GainedPropertiesController = new CharacterCreationGainedPropertiesVM(this._characterCreation, -1);
			this.ClanBanner = new ImageIdentifierVM(Clan.PlayerClan.Banner);
			this.CannotAdvanceReasonHint = new HintViewModel();
		}

		// Token: 0x06001C8D RID: 7309 RVA: 0x000667D4 File Offset: 0x000649D4
		private void AddReviewedItems()
		{
			string text = string.Empty;
			CultureObject selectedCulture = this._currentContent.GetSelectedCulture();
			IEnumerable<FeatObject> culturalFeats = selectedCulture.GetCulturalFeats((FeatObject x) => x.IsPositive);
			IEnumerable<FeatObject> culturalFeats2 = selectedCulture.GetCulturalFeats((FeatObject x) => !x.IsPositive);
			foreach (FeatObject featObject in culturalFeats)
			{
				GameTexts.SetVariable("STR1", text);
				GameTexts.SetVariable("STR2", featObject.Description);
				text = GameTexts.FindText("str_string_newline_string", null).ToString();
			}
			foreach (FeatObject featObject2 in culturalFeats2)
			{
				GameTexts.SetVariable("STR1", text);
				GameTexts.SetVariable("STR2", featObject2.Description);
				text = GameTexts.FindText("str_string_newline_string", null).ToString();
			}
			CharacterCreationReviewStageItemVM characterCreationReviewStageItemVM = new CharacterCreationReviewStageItemVM(new TextObject("{=K6GYskvJ}Culture:", null).ToString(), this._currentContent.GetSelectedCulture().Name.ToString(), text);
			this.ReviewList.Add(characterCreationReviewStageItemVM);
			for (int i = 0; i < this._characterCreation.CharacterCreationMenuCount; i++)
			{
				IEnumerable<int> selectedOptions = this._characterCreation.GetSelectedOptions(i);
				IEnumerable<CharacterCreationOption> currentMenuOptions = this._characterCreation.GetCurrentMenuOptions(i);
				Func<CharacterCreationOption, bool> func;
				Func<CharacterCreationOption, bool> <>9__2;
				if ((func = <>9__2) == null)
				{
					func = (<>9__2 = (CharacterCreationOption s) => selectedOptions.Contains(s.Id));
				}
				foreach (CharacterCreationOption characterCreationOption in currentMenuOptions.Where(func))
				{
					characterCreationReviewStageItemVM = new CharacterCreationReviewStageItemVM(this._characterCreation.GetCurrentMenuTitle(i).ToString(), characterCreationOption.Text.ToString(), characterCreationOption.PositiveEffectText.ToString());
					this.ReviewList.Add(characterCreationReviewStageItemVM);
				}
			}
			if (this._isBannerAndClanNameSet)
			{
				CharacterCreationReviewStageItemVM characterCreationReviewStageItemVM2 = new CharacterCreationReviewStageItemVM(new ImageIdentifierVM(BannerCode.CreateFrom(Clan.PlayerClan.Banner), true), GameTexts.FindText("str_clan", null).ToString(), Clan.PlayerClan.Name.ToString(), null);
				this.ReviewList.Add(characterCreationReviewStageItemVM2);
			}
		}

		// Token: 0x06001C8E RID: 7310 RVA: 0x00066A74 File Offset: 0x00064C74
		public void ExecuteRandomizeName()
		{
			CharacterCreationContentBase currentCharacterCreationContent = (GameStateManager.Current.ActiveState as CharacterCreationState).CurrentCharacterCreationContent;
			this.Name = NameGenerator.Current.GenerateFirstNameForPlayer(currentCharacterCreationContent.GetSelectedCulture(), Hero.MainHero.IsFemale).ToString();
		}

		// Token: 0x06001C8F RID: 7311 RVA: 0x00066ABC File Offset: 0x00064CBC
		private void OnRefresh()
		{
			TextObject textObject = GameTexts.FindText("str_generic_character_firstname", null);
			textObject.SetTextVariable("CHARACTER_FIRSTNAME", new TextObject(this.Name, null));
			TextObject textObject2 = GameTexts.FindText("str_generic_character_name", null);
			textObject2.SetTextVariable("CHARACTER_NAME", new TextObject(this.Name, null));
			textObject2.SetTextVariable("CHARACTER_GENDER", Hero.MainHero.IsFemale ? 1 : 0);
			textObject.SetTextVariable("CHARACTER_GENDER", Hero.MainHero.IsFemale ? 1 : 0);
			Hero.MainHero.SetName(textObject2, textObject);
			base.OnPropertyChanged("CanAdvance");
		}

		// Token: 0x06001C90 RID: 7312 RVA: 0x00066B60 File Offset: 0x00064D60
		public override void OnNextStage()
		{
			this._affirmativeAction();
		}

		// Token: 0x06001C91 RID: 7313 RVA: 0x00066B6D File Offset: 0x00064D6D
		public override void OnPreviousStage()
		{
			this._negativeAction();
		}

		// Token: 0x06001C92 RID: 7314 RVA: 0x00066B7C File Offset: 0x00064D7C
		public override bool CanAdvanceToNextStage()
		{
			TextObject textObject = TextObject.Empty;
			bool flag = true;
			if (string.IsNullOrEmpty(this.Name) || string.IsNullOrWhiteSpace(this.Name))
			{
				textObject = new TextObject("{=IRcy3pWJ}Name cannot be empty", null);
				flag = false;
			}
			Tuple<bool, string> tuple = CampaignUIHelper.IsStringApplicableForHeroName(this.Name);
			if (!tuple.Item1)
			{
				if (!string.IsNullOrEmpty(tuple.Item2))
				{
					textObject = new TextObject("{=!}" + tuple.Item2, null);
				}
				flag = false;
			}
			this.CannotAdvanceReasonHint.HintText = textObject;
			return flag;
		}

		// Token: 0x06001C93 RID: 7315 RVA: 0x00066C00 File Offset: 0x00064E00
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

		// Token: 0x06001C94 RID: 7316 RVA: 0x00066C29 File Offset: 0x00064E29
		public void SetCancelInputKey(HotKey hotKey)
		{
			this.CancelInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x06001C95 RID: 7317 RVA: 0x00066C38 File Offset: 0x00064E38
		public void SetDoneInputKey(HotKey hotKey)
		{
			this.DoneInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x170009C7 RID: 2503
		// (get) Token: 0x06001C96 RID: 7318 RVA: 0x00066C47 File Offset: 0x00064E47
		// (set) Token: 0x06001C97 RID: 7319 RVA: 0x00066C4F File Offset: 0x00064E4F
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

		// Token: 0x170009C8 RID: 2504
		// (get) Token: 0x06001C98 RID: 7320 RVA: 0x00066C6D File Offset: 0x00064E6D
		// (set) Token: 0x06001C99 RID: 7321 RVA: 0x00066C75 File Offset: 0x00064E75
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

		// Token: 0x170009C9 RID: 2505
		// (get) Token: 0x06001C9A RID: 7322 RVA: 0x00066C93 File Offset: 0x00064E93
		// (set) Token: 0x06001C9B RID: 7323 RVA: 0x00066C9B File Offset: 0x00064E9B
		[DataSourceProperty]
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (value != this._name)
				{
					this._name = value;
					this._characterCreation.Name = value;
					base.OnPropertyChangedWithValue<string>(value, "Name");
					this.OnRefresh();
				}
			}
		}

		// Token: 0x170009CA RID: 2506
		// (get) Token: 0x06001C9C RID: 7324 RVA: 0x00066CD0 File Offset: 0x00064ED0
		// (set) Token: 0x06001C9D RID: 7325 RVA: 0x00066CD8 File Offset: 0x00064ED8
		[DataSourceProperty]
		public string NameTextQuestion
		{
			get
			{
				return this._nameTextQuestion;
			}
			set
			{
				if (value != this._nameTextQuestion)
				{
					this._nameTextQuestion = value;
					base.OnPropertyChangedWithValue<string>(value, "NameTextQuestion");
				}
			}
		}

		// Token: 0x170009CB RID: 2507
		// (get) Token: 0x06001C9E RID: 7326 RVA: 0x00066CFB File Offset: 0x00064EFB
		// (set) Token: 0x06001C9F RID: 7327 RVA: 0x00066D03 File Offset: 0x00064F03
		[DataSourceProperty]
		public MBBindingList<CharacterCreationReviewStageItemVM> ReviewList
		{
			get
			{
				return this._reviewList;
			}
			set
			{
				if (value != this._reviewList)
				{
					this._reviewList = value;
					base.OnPropertyChangedWithValue<MBBindingList<CharacterCreationReviewStageItemVM>>(value, "ReviewList");
				}
			}
		}

		// Token: 0x170009CC RID: 2508
		// (get) Token: 0x06001CA0 RID: 7328 RVA: 0x00066D21 File Offset: 0x00064F21
		// (set) Token: 0x06001CA1 RID: 7329 RVA: 0x00066D29 File Offset: 0x00064F29
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

		// Token: 0x170009CD RID: 2509
		// (get) Token: 0x06001CA2 RID: 7330 RVA: 0x00066D47 File Offset: 0x00064F47
		// (set) Token: 0x06001CA3 RID: 7331 RVA: 0x00066D4F File Offset: 0x00064F4F
		[DataSourceProperty]
		public ImageIdentifierVM ClanBanner
		{
			get
			{
				return this._clanBanner;
			}
			set
			{
				if (value != this._clanBanner)
				{
					this._clanBanner = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "ClanBanner");
				}
			}
		}

		// Token: 0x170009CE RID: 2510
		// (get) Token: 0x06001CA4 RID: 7332 RVA: 0x00066D6D File Offset: 0x00064F6D
		// (set) Token: 0x06001CA5 RID: 7333 RVA: 0x00066D75 File Offset: 0x00064F75
		[DataSourceProperty]
		public HintViewModel CannotAdvanceReasonHint
		{
			get
			{
				return this._cannotAdvanceReasonHint;
			}
			set
			{
				if (value != this._cannotAdvanceReasonHint)
				{
					this._cannotAdvanceReasonHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "CannotAdvanceReasonHint");
				}
			}
		}

		// Token: 0x04000D75 RID: 3445
		private readonly CharacterCreationContentBase _currentContent;

		// Token: 0x04000D76 RID: 3446
		private bool _isBannerAndClanNameSet;

		// Token: 0x04000D77 RID: 3447
		private InputKeyItemVM _cancelInputKey;

		// Token: 0x04000D78 RID: 3448
		private InputKeyItemVM _doneInputKey;

		// Token: 0x04000D79 RID: 3449
		private string _name = "";

		// Token: 0x04000D7A RID: 3450
		private string _nameTextQuestion = "";

		// Token: 0x04000D7B RID: 3451
		private MBBindingList<CharacterCreationReviewStageItemVM> _reviewList;

		// Token: 0x04000D7C RID: 3452
		private CharacterCreationGainedPropertiesVM _gainedPropertiesController;

		// Token: 0x04000D7D RID: 3453
		private ImageIdentifierVM _clanBanner;

		// Token: 0x04000D7E RID: 3454
		private HintViewModel _cannotAdvanceReasonHint;
	}
}
