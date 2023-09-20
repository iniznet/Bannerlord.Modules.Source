using System;
using TaleWorlds.CampaignSystem.ViewModelCollection.Input;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Party.PartyTroopManagerPopUp
{
	public abstract class PartyTroopManagerVM : ViewModel
	{
		public abstract void ExecuteItemPrimaryAction();

		public abstract void ExecuteItemSecondaryAction();

		public PartyTroopManagerVM(PartyVM partyVM)
		{
			this._partyVM = partyVM;
			this.Troops = new MBBindingList<PartyTroopManagerItemVM>();
			this.OpenButtonHint = new HintViewModel();
			this.RefreshValues();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.AvatarText = new TextObject("{=5tbWdY1j}Avatar", null).ToString();
			this.NameText = new TextObject("{=PDdh1sBj}Name", null).ToString();
			this.CountText = new TextObject("{=zFDoDbNj}Count", null).ToString();
			this.DoneLbl = GameTexts.FindText("str_done", null).ToString();
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			this.DoneInputKey.OnFinalize();
			InputKeyItemVM primaryActionInputKey = this.PrimaryActionInputKey;
			if (primaryActionInputKey != null)
			{
				primaryActionInputKey.OnFinalize();
			}
			InputKeyItemVM secondaryActionInputKey = this.SecondaryActionInputKey;
			if (secondaryActionInputKey == null)
			{
				return;
			}
			secondaryActionInputKey.OnFinalize();
		}

		public virtual void OpenPopUp()
		{
			this._partyVM.PartyScreenLogic.SavePartyScreenData();
			this._initialGoldChange = this._partyVM.PartyScreenLogic.CurrentData.PartyGoldChangeAmount;
			this._initialHorseChange = this._partyVM.PartyScreenLogic.CurrentData.PartyHorseChangeAmount;
			this._initialMoraleChange = this._partyVM.PartyScreenLogic.CurrentData.PartyMoraleChangeAmount;
			this.UpdateLabels();
			this._hasMadeChanges = false;
			this.IsOpen = true;
		}

		public virtual void ExecuteDone()
		{
			this.IsOpen = false;
		}

		protected virtual void ConfirmCancel()
		{
			this._partyVM.PartyScreenLogic.ResetToLastSavedPartyScreenData(false);
			this.IsOpen = false;
		}

		public void UpdateOpenButtonHint(bool isDisabled, bool isIrrelevant, bool isUpgradesDisabled)
		{
			TextObject textObject;
			if (isIrrelevant)
			{
				textObject = this._openButtonIrrelevantScreenHint;
			}
			else if (isUpgradesDisabled)
			{
				textObject = this._openButtonUpgradesDisabledHint;
			}
			else if (isDisabled)
			{
				textObject = this._openButtonNoTroopsHint;
			}
			else
			{
				textObject = this._openButtonEnabledHint;
			}
			this.OpenButtonHint.HintText = textObject;
		}

		public abstract void ExecuteCancel();

		protected void ShowCancelInquiry(Action confirmCancel)
		{
			if (this._hasMadeChanges)
			{
				string text = new TextObject("{=a8NoW1Q2}Are you sure you want to cancel your changes?", null).ToString();
				InformationManager.ShowInquiry(new InquiryData("", text, true, true, GameTexts.FindText("str_yes", null).ToString(), GameTexts.FindText("str_no", null).ToString(), delegate
				{
					confirmCancel();
				}, null, "", 0f, null, null, null), false, false);
				return;
			}
			confirmCancel();
		}

		protected void UpdateLabels()
		{
			MBTextManager.SetTextVariable("PAY_OR_GET", 0);
			int num = this._partyVM.PartyScreenLogic.CurrentData.PartyGoldChangeAmount - this._initialGoldChange;
			int num2 = this._partyVM.PartyScreenLogic.CurrentData.PartyHorseChangeAmount - this._initialHorseChange;
			int num3 = this._partyVM.PartyScreenLogic.CurrentData.PartyMoraleChangeAmount - this._initialMoraleChange;
			MBTextManager.SetTextVariable("LABEL_ICON", "{=!}<img src=\"General\\Icons\\Coin@2x\" extend=\"8\">", false);
			MBTextManager.SetTextVariable("TRADE_AMOUNT", MathF.Abs(num));
			this.GoldChangeText = ((num == 0) ? "" : GameTexts.FindText("str_party_generic_label", null).ToString());
			MBTextManager.SetTextVariable("LABEL_ICON", "{=!}<img src=\"StdAssets\\ItemIcons\\Mount\" extend=\"16\">", false);
			MBTextManager.SetTextVariable("TRADE_AMOUNT", MathF.Abs(num2));
			this.HorseChangeText = ((num2 == 0) ? "" : GameTexts.FindText("str_party_generic_label", null).ToString());
			MBTextManager.SetTextVariable("LABEL_ICON", "{=!}<img src=\"General\\Icons\\Morale@2x\" extend=\"8\">", false);
			MBTextManager.SetTextVariable("TRADE_AMOUNT", MathF.Abs(num3));
			this.MoraleChangeText = ((num3 == 0) ? "" : GameTexts.FindText("str_party_generic_label", null).ToString());
		}

		protected void SetFocusedCharacter(PartyTroopManagerItemVM troop)
		{
			this.FocusedTroop = troop;
			this.IsFocusedOnACharacter = troop != null;
			if (this.FocusedTroop == null)
			{
				this.IsPrimaryActionAvailable = false;
				this.IsSecondaryActionAvailable = false;
				return;
			}
			if (this.IsUpgradePopUp)
			{
				MBBindingList<UpgradeTargetVM> upgrades = this.FocusedTroop.PartyCharacter.Upgrades;
				this.IsPrimaryActionAvailable = upgrades.Count > 0 && upgrades[0].IsAvailable;
				this.IsSecondaryActionAvailable = upgrades.Count > 1 && upgrades[1].IsAvailable;
				return;
			}
			this.IsPrimaryActionAvailable = false;
			this.IsSecondaryActionAvailable = this.FocusedTroop.IsTroopRecruitable;
		}

		public void SetDoneInputKey(HotKey hotKey)
		{
			this.DoneInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		public void SetPrimaryActionInputKey(HotKey hotKey)
		{
			this.PrimaryActionInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		public void SetSecondaryActionInputKey(HotKey hotKey)
		{
			this.SecondaryActionInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
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
		public InputKeyItemVM PrimaryActionInputKey
		{
			get
			{
				return this._primaryActionInputKey;
			}
			set
			{
				if (value != this._primaryActionInputKey)
				{
					this._primaryActionInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "PrimaryActionInputKey");
				}
			}
		}

		[DataSourceProperty]
		public InputKeyItemVM SecondaryActionInputKey
		{
			get
			{
				return this._secondaryActionInputKey;
			}
			set
			{
				if (value != this._secondaryActionInputKey)
				{
					this._secondaryActionInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "SecondaryActionInputKey");
				}
			}
		}

		[DataSourceProperty]
		public bool IsFocusedOnACharacter
		{
			get
			{
				return this._isFocusedOnACharacter;
			}
			set
			{
				if (value != this._isFocusedOnACharacter)
				{
					this._isFocusedOnACharacter = value;
					base.OnPropertyChangedWithValue(value, "IsFocusedOnACharacter");
				}
			}
		}

		[DataSourceProperty]
		public bool IsOpen
		{
			get
			{
				return this._isOpen;
			}
			set
			{
				if (value != this._isOpen)
				{
					this._isOpen = value;
					base.OnPropertyChangedWithValue(value, "IsOpen");
				}
			}
		}

		[DataSourceProperty]
		public bool IsUpgradePopUp
		{
			get
			{
				return this._isUpgradePopUp;
			}
			set
			{
				if (value != this._isUpgradePopUp)
				{
					this._isUpgradePopUp = value;
					base.OnPropertyChangedWithValue(value, "IsUpgradePopUp");
				}
			}
		}

		[DataSourceProperty]
		public bool IsPrimaryActionAvailable
		{
			get
			{
				return this._isPrimaryActionAvailable;
			}
			set
			{
				if (value != this._isPrimaryActionAvailable)
				{
					this._isPrimaryActionAvailable = value;
					base.OnPropertyChangedWithValue(value, "IsPrimaryActionAvailable");
				}
			}
		}

		[DataSourceProperty]
		public bool IsSecondaryActionAvailable
		{
			get
			{
				return this._isSecondaryActionAvailable;
			}
			set
			{
				if (value != this._isSecondaryActionAvailable)
				{
					this._isSecondaryActionAvailable = value;
					base.OnPropertyChangedWithValue(value, "IsSecondaryActionAvailable");
				}
			}
		}

		[DataSourceProperty]
		public PartyTroopManagerItemVM FocusedTroop
		{
			get
			{
				return this._focusedTroop;
			}
			set
			{
				if (value != this._focusedTroop)
				{
					this._focusedTroop = value;
					base.OnPropertyChangedWithValue<PartyTroopManagerItemVM>(value, "FocusedTroop");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<PartyTroopManagerItemVM> Troops
		{
			get
			{
				return this._troops;
			}
			set
			{
				if (value != this._troops)
				{
					this._troops = value;
					base.OnPropertyChangedWithValue<MBBindingList<PartyTroopManagerItemVM>>(value, "Troops");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel OpenButtonHint
		{
			get
			{
				return this._openButtonHint;
			}
			set
			{
				if (value != this._openButtonHint)
				{
					this._openButtonHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "OpenButtonHint");
				}
			}
		}

		[DataSourceProperty]
		public string TitleText
		{
			get
			{
				return this._titleText;
			}
			set
			{
				if (value != this._titleText)
				{
					this._titleText = value;
					base.OnPropertyChangedWithValue<string>(value, "TitleText");
				}
			}
		}

		[DataSourceProperty]
		public string AvatarText
		{
			get
			{
				return this._avatarText;
			}
			set
			{
				if (value != this._avatarText)
				{
					this._avatarText = value;
					base.OnPropertyChangedWithValue<string>(value, "AvatarText");
				}
			}
		}

		[DataSourceProperty]
		public string NameText
		{
			get
			{
				return this._nameText;
			}
			set
			{
				if (value != this._nameText)
				{
					this._nameText = value;
					base.OnPropertyChangedWithValue<string>(value, "NameText");
				}
			}
		}

		[DataSourceProperty]
		public string CountText
		{
			get
			{
				return this._countText;
			}
			set
			{
				if (value != this._countText)
				{
					this._countText = value;
					base.OnPropertyChangedWithValue<string>(value, "CountText");
				}
			}
		}

		[DataSourceProperty]
		public string GoldChangeText
		{
			get
			{
				return this._goldChangeText;
			}
			set
			{
				if (value != this._goldChangeText)
				{
					this._goldChangeText = value;
					base.OnPropertyChangedWithValue<string>(value, "GoldChangeText");
				}
			}
		}

		[DataSourceProperty]
		public string HorseChangeText
		{
			get
			{
				return this._horseChangeText;
			}
			set
			{
				if (value != this._horseChangeText)
				{
					this._horseChangeText = value;
					base.OnPropertyChangedWithValue<string>(value, "HorseChangeText");
				}
			}
		}

		[DataSourceProperty]
		public string MoraleChangeText
		{
			get
			{
				return this._moraleChangeText;
			}
			set
			{
				if (value != this._moraleChangeText)
				{
					this._moraleChangeText = value;
					base.OnPropertyChangedWithValue<string>(value, "MoraleChangeText");
				}
			}
		}

		[DataSourceProperty]
		public string DoneLbl
		{
			get
			{
				return this._doneLbl;
			}
			set
			{
				if (value != this._doneLbl)
				{
					this._doneLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "DoneLbl");
				}
			}
		}

		protected PartyVM _partyVM;

		protected bool _hasMadeChanges;

		protected TextObject _openButtonEnabledHint = TextObject.Empty;

		protected TextObject _openButtonNoTroopsHint = TextObject.Empty;

		protected TextObject _openButtonIrrelevantScreenHint = TextObject.Empty;

		protected TextObject _openButtonUpgradesDisabledHint = TextObject.Empty;

		private int _initialGoldChange;

		private int _initialHorseChange;

		private int _initialMoraleChange;

		private InputKeyItemVM _doneInputKey;

		private InputKeyItemVM _primaryActionInputKey;

		private InputKeyItemVM _secondaryActionInputKey;

		private bool _isFocusedOnACharacter;

		private bool _isOpen;

		private bool _isUpgradePopUp;

		private bool _isPrimaryActionAvailable;

		private bool _isSecondaryActionAvailable;

		private PartyTroopManagerItemVM _focusedTroop;

		private MBBindingList<PartyTroopManagerItemVM> _troops;

		private HintViewModel _openButtonHint;

		private string _titleText;

		private string _avatarText;

		private string _nameText;

		private string _countText;

		private string _goldChangeText;

		private string _horseChangeText;

		private string _moraleChangeText;

		private string _doneLbl;
	}
}
