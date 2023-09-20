using System;
using TaleWorlds.CampaignSystem.ViewModelCollection.Input;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Party.PartyTroopManagerPopUp
{
	// Token: 0x02000030 RID: 48
	public abstract class PartyTroopManagerVM : ViewModel
	{
		// Token: 0x0600049F RID: 1183
		public abstract void ExecuteItemPrimaryAction();

		// Token: 0x060004A0 RID: 1184
		public abstract void ExecuteItemSecondaryAction();

		// Token: 0x060004A1 RID: 1185 RVA: 0x0001888C File Offset: 0x00016A8C
		public PartyTroopManagerVM(PartyVM partyVM)
		{
			this._partyVM = partyVM;
			this.Troops = new MBBindingList<PartyTroopManagerItemVM>();
			this.OpenButtonHint = new HintViewModel();
			this.RefreshValues();
		}

		// Token: 0x060004A2 RID: 1186 RVA: 0x000188F0 File Offset: 0x00016AF0
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.AvatarText = new TextObject("{=5tbWdY1j}Avatar", null).ToString();
			this.NameText = new TextObject("{=PDdh1sBj}Name", null).ToString();
			this.CountText = new TextObject("{=zFDoDbNj}Count", null).ToString();
			this.DoneLbl = GameTexts.FindText("str_done", null).ToString();
		}

		// Token: 0x060004A3 RID: 1187 RVA: 0x0001895B File Offset: 0x00016B5B
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

		// Token: 0x060004A4 RID: 1188 RVA: 0x00018990 File Offset: 0x00016B90
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

		// Token: 0x060004A5 RID: 1189 RVA: 0x00018A12 File Offset: 0x00016C12
		public virtual void ExecuteDone()
		{
			this.IsOpen = false;
		}

		// Token: 0x060004A6 RID: 1190 RVA: 0x00018A1B File Offset: 0x00016C1B
		protected virtual void ConfirmCancel()
		{
			this._partyVM.PartyScreenLogic.ResetToLastSavedPartyScreenData(false);
			this.IsOpen = false;
		}

		// Token: 0x060004A7 RID: 1191 RVA: 0x00018A38 File Offset: 0x00016C38
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

		// Token: 0x060004A8 RID: 1192
		public abstract void ExecuteCancel();

		// Token: 0x060004A9 RID: 1193 RVA: 0x00018A80 File Offset: 0x00016C80
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

		// Token: 0x060004AA RID: 1194 RVA: 0x00018B10 File Offset: 0x00016D10
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

		// Token: 0x060004AB RID: 1195 RVA: 0x00018C40 File Offset: 0x00016E40
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

		// Token: 0x060004AC RID: 1196 RVA: 0x00018CE3 File Offset: 0x00016EE3
		public void SetDoneInputKey(HotKey hotKey)
		{
			this.DoneInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x060004AD RID: 1197 RVA: 0x00018CF2 File Offset: 0x00016EF2
		public void SetPrimaryActionInputKey(HotKey hotKey)
		{
			this.PrimaryActionInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x060004AE RID: 1198 RVA: 0x00018D01 File Offset: 0x00016F01
		public void SetSecondaryActionInputKey(HotKey hotKey)
		{
			this.SecondaryActionInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x17000155 RID: 341
		// (get) Token: 0x060004AF RID: 1199 RVA: 0x00018D10 File Offset: 0x00016F10
		// (set) Token: 0x060004B0 RID: 1200 RVA: 0x00018D18 File Offset: 0x00016F18
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

		// Token: 0x17000156 RID: 342
		// (get) Token: 0x060004B1 RID: 1201 RVA: 0x00018D36 File Offset: 0x00016F36
		// (set) Token: 0x060004B2 RID: 1202 RVA: 0x00018D3E File Offset: 0x00016F3E
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

		// Token: 0x17000157 RID: 343
		// (get) Token: 0x060004B3 RID: 1203 RVA: 0x00018D5C File Offset: 0x00016F5C
		// (set) Token: 0x060004B4 RID: 1204 RVA: 0x00018D64 File Offset: 0x00016F64
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

		// Token: 0x17000158 RID: 344
		// (get) Token: 0x060004B5 RID: 1205 RVA: 0x00018D82 File Offset: 0x00016F82
		// (set) Token: 0x060004B6 RID: 1206 RVA: 0x00018D8A File Offset: 0x00016F8A
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

		// Token: 0x17000159 RID: 345
		// (get) Token: 0x060004B7 RID: 1207 RVA: 0x00018DA8 File Offset: 0x00016FA8
		// (set) Token: 0x060004B8 RID: 1208 RVA: 0x00018DB0 File Offset: 0x00016FB0
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

		// Token: 0x1700015A RID: 346
		// (get) Token: 0x060004B9 RID: 1209 RVA: 0x00018DCE File Offset: 0x00016FCE
		// (set) Token: 0x060004BA RID: 1210 RVA: 0x00018DD6 File Offset: 0x00016FD6
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

		// Token: 0x1700015B RID: 347
		// (get) Token: 0x060004BB RID: 1211 RVA: 0x00018DF4 File Offset: 0x00016FF4
		// (set) Token: 0x060004BC RID: 1212 RVA: 0x00018DFC File Offset: 0x00016FFC
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

		// Token: 0x1700015C RID: 348
		// (get) Token: 0x060004BD RID: 1213 RVA: 0x00018E1A File Offset: 0x0001701A
		// (set) Token: 0x060004BE RID: 1214 RVA: 0x00018E22 File Offset: 0x00017022
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

		// Token: 0x1700015D RID: 349
		// (get) Token: 0x060004BF RID: 1215 RVA: 0x00018E40 File Offset: 0x00017040
		// (set) Token: 0x060004C0 RID: 1216 RVA: 0x00018E48 File Offset: 0x00017048
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

		// Token: 0x1700015E RID: 350
		// (get) Token: 0x060004C1 RID: 1217 RVA: 0x00018E66 File Offset: 0x00017066
		// (set) Token: 0x060004C2 RID: 1218 RVA: 0x00018E6E File Offset: 0x0001706E
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

		// Token: 0x1700015F RID: 351
		// (get) Token: 0x060004C3 RID: 1219 RVA: 0x00018E8C File Offset: 0x0001708C
		// (set) Token: 0x060004C4 RID: 1220 RVA: 0x00018E94 File Offset: 0x00017094
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

		// Token: 0x17000160 RID: 352
		// (get) Token: 0x060004C5 RID: 1221 RVA: 0x00018EB2 File Offset: 0x000170B2
		// (set) Token: 0x060004C6 RID: 1222 RVA: 0x00018EBA File Offset: 0x000170BA
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

		// Token: 0x17000161 RID: 353
		// (get) Token: 0x060004C7 RID: 1223 RVA: 0x00018EDD File Offset: 0x000170DD
		// (set) Token: 0x060004C8 RID: 1224 RVA: 0x00018EE5 File Offset: 0x000170E5
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

		// Token: 0x17000162 RID: 354
		// (get) Token: 0x060004C9 RID: 1225 RVA: 0x00018F08 File Offset: 0x00017108
		// (set) Token: 0x060004CA RID: 1226 RVA: 0x00018F10 File Offset: 0x00017110
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

		// Token: 0x17000163 RID: 355
		// (get) Token: 0x060004CB RID: 1227 RVA: 0x00018F33 File Offset: 0x00017133
		// (set) Token: 0x060004CC RID: 1228 RVA: 0x00018F3B File Offset: 0x0001713B
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

		// Token: 0x17000164 RID: 356
		// (get) Token: 0x060004CD RID: 1229 RVA: 0x00018F5E File Offset: 0x0001715E
		// (set) Token: 0x060004CE RID: 1230 RVA: 0x00018F66 File Offset: 0x00017166
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

		// Token: 0x17000165 RID: 357
		// (get) Token: 0x060004CF RID: 1231 RVA: 0x00018F89 File Offset: 0x00017189
		// (set) Token: 0x060004D0 RID: 1232 RVA: 0x00018F91 File Offset: 0x00017191
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

		// Token: 0x17000166 RID: 358
		// (get) Token: 0x060004D1 RID: 1233 RVA: 0x00018FB4 File Offset: 0x000171B4
		// (set) Token: 0x060004D2 RID: 1234 RVA: 0x00018FBC File Offset: 0x000171BC
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

		// Token: 0x17000167 RID: 359
		// (get) Token: 0x060004D3 RID: 1235 RVA: 0x00018FDF File Offset: 0x000171DF
		// (set) Token: 0x060004D4 RID: 1236 RVA: 0x00018FE7 File Offset: 0x000171E7
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

		// Token: 0x040001F6 RID: 502
		protected PartyVM _partyVM;

		// Token: 0x040001F7 RID: 503
		protected bool _hasMadeChanges;

		// Token: 0x040001F8 RID: 504
		protected TextObject _openButtonEnabledHint = TextObject.Empty;

		// Token: 0x040001F9 RID: 505
		protected TextObject _openButtonNoTroopsHint = TextObject.Empty;

		// Token: 0x040001FA RID: 506
		protected TextObject _openButtonIrrelevantScreenHint = TextObject.Empty;

		// Token: 0x040001FB RID: 507
		protected TextObject _openButtonUpgradesDisabledHint = TextObject.Empty;

		// Token: 0x040001FC RID: 508
		private int _initialGoldChange;

		// Token: 0x040001FD RID: 509
		private int _initialHorseChange;

		// Token: 0x040001FE RID: 510
		private int _initialMoraleChange;

		// Token: 0x040001FF RID: 511
		private InputKeyItemVM _doneInputKey;

		// Token: 0x04000200 RID: 512
		private InputKeyItemVM _primaryActionInputKey;

		// Token: 0x04000201 RID: 513
		private InputKeyItemVM _secondaryActionInputKey;

		// Token: 0x04000202 RID: 514
		private bool _isFocusedOnACharacter;

		// Token: 0x04000203 RID: 515
		private bool _isOpen;

		// Token: 0x04000204 RID: 516
		private bool _isUpgradePopUp;

		// Token: 0x04000205 RID: 517
		private bool _isPrimaryActionAvailable;

		// Token: 0x04000206 RID: 518
		private bool _isSecondaryActionAvailable;

		// Token: 0x04000207 RID: 519
		private PartyTroopManagerItemVM _focusedTroop;

		// Token: 0x04000208 RID: 520
		private MBBindingList<PartyTroopManagerItemVM> _troops;

		// Token: 0x04000209 RID: 521
		private HintViewModel _openButtonHint;

		// Token: 0x0400020A RID: 522
		private string _titleText;

		// Token: 0x0400020B RID: 523
		private string _avatarText;

		// Token: 0x0400020C RID: 524
		private string _nameText;

		// Token: 0x0400020D RID: 525
		private string _countText;

		// Token: 0x0400020E RID: 526
		private string _goldChangeText;

		// Token: 0x0400020F RID: 527
		private string _horseChangeText;

		// Token: 0x04000210 RID: 528
		private string _moraleChangeText;

		// Token: 0x04000211 RID: 529
		private string _doneLbl;
	}
}
