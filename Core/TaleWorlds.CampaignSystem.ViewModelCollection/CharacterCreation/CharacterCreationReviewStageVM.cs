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
	public class CharacterCreationReviewStageVM : CharacterCreationStageBaseVM
	{
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

		public void ExecuteRandomizeName()
		{
			CharacterCreationContentBase currentCharacterCreationContent = (GameStateManager.Current.ActiveState as CharacterCreationState).CurrentCharacterCreationContent;
			this.Name = NameGenerator.Current.GenerateFirstNameForPlayer(currentCharacterCreationContent.GetSelectedCulture(), Hero.MainHero.IsFemale).ToString();
		}

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

		private readonly CharacterCreationContentBase _currentContent;

		private bool _isBannerAndClanNameSet;

		private InputKeyItemVM _cancelInputKey;

		private InputKeyItemVM _doneInputKey;

		private string _name = "";

		private string _nameTextQuestion = "";

		private MBBindingList<CharacterCreationReviewStageItemVM> _reviewList;

		private CharacterCreationGainedPropertiesVM _gainedPropertiesController;

		private ImageIdentifierVM _clanBanner;

		private HintViewModel _cannotAdvanceReasonHint;
	}
}
