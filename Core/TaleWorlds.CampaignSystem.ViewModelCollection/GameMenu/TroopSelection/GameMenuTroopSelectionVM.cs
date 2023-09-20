using System;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.ViewModelCollection.Input;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.TroopSelection
{
	// Token: 0x02000089 RID: 137
	public class GameMenuTroopSelectionVM : ViewModel
	{
		// Token: 0x06000D75 RID: 3445 RVA: 0x00036B80 File Offset: 0x00034D80
		public GameMenuTroopSelectionVM(TroopRoster fullRoster, TroopRoster initialSelections, Func<CharacterObject, bool> canChangeChangeStatusOfTroop, Action<TroopRoster> onDone, int maxSelectableTroopCount, int minSelectableTroopCount)
		{
			this._canChangeChangeStatusOfTroop = canChangeChangeStatusOfTroop;
			this._onDone = onDone;
			this._fullRoster = fullRoster;
			this._initialSelections = initialSelections;
			this._maxSelectableTroopCount = maxSelectableTroopCount;
			this._minSelectableTroopCount = minSelectableTroopCount;
			this.InitList();
			this.RefreshValues();
			this.OnCurrentSelectedAmountChange();
		}

		// Token: 0x06000D76 RID: 3446 RVA: 0x00036BF4 File Offset: 0x00034DF4
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.TitleText = this._titleTextObject.ToString();
			this.CurrentSelectedAmountTitle = this._chosenTitleTextObject.ToString();
			this.DoneText = GameTexts.FindText("str_done", null).ToString();
			this.CancelText = GameTexts.FindText("str_cancel", null).ToString();
			this.ClearSelectionText = new TextObject("{=QMNWbmao}Clear Selection", null).ToString();
		}

		// Token: 0x06000D77 RID: 3447 RVA: 0x00036C6C File Offset: 0x00034E6C
		private void InitList()
		{
			this.Troops = new MBBindingList<TroopSelectionItemVM>();
			this._currentTotalSelectedTroopCount = 0;
			foreach (TroopRosterElement troopRosterElement in this._fullRoster.GetTroopRoster())
			{
				TroopSelectionItemVM troopSelectionItemVM = new TroopSelectionItemVM(troopRosterElement, new Action<TroopSelectionItemVM>(this.OnAddCount), new Action<TroopSelectionItemVM>(this.OnRemoveCount));
				troopSelectionItemVM.IsLocked = !this._canChangeChangeStatusOfTroop(troopRosterElement.Character) || troopRosterElement.Number - troopRosterElement.WoundedNumber <= 0;
				this.Troops.Add(troopSelectionItemVM);
				int troopCount = this._initialSelections.GetTroopCount(troopRosterElement.Character);
				if (troopCount > 0)
				{
					troopSelectionItemVM.CurrentAmount = troopCount;
					this._currentTotalSelectedTroopCount += troopCount;
				}
			}
			this.Troops.Sort(new TroopItemComparer());
		}

		// Token: 0x06000D78 RID: 3448 RVA: 0x00036D6C File Offset: 0x00034F6C
		private void OnRemoveCount(TroopSelectionItemVM troopItem)
		{
			if (troopItem.CurrentAmount > 0)
			{
				int num = 1;
				if (this.IsEntireStackModifierActive)
				{
					num = troopItem.CurrentAmount;
				}
				else if (this.IsFiveStackModifierActive)
				{
					num = MathF.Min(troopItem.CurrentAmount, 5);
				}
				troopItem.CurrentAmount -= num;
				this._currentTotalSelectedTroopCount -= num;
			}
			this.OnCurrentSelectedAmountChange();
		}

		// Token: 0x06000D79 RID: 3449 RVA: 0x00036DCC File Offset: 0x00034FCC
		private void OnAddCount(TroopSelectionItemVM troopItem)
		{
			if (troopItem.CurrentAmount < troopItem.MaxAmount && this._currentTotalSelectedTroopCount < this._maxSelectableTroopCount)
			{
				int num = 1;
				if (this.IsEntireStackModifierActive)
				{
					num = MathF.Min(troopItem.MaxAmount - troopItem.CurrentAmount, this._maxSelectableTroopCount - this._currentTotalSelectedTroopCount);
				}
				else if (this.IsFiveStackModifierActive)
				{
					num = MathF.Min(MathF.Min(troopItem.MaxAmount - troopItem.CurrentAmount, this._maxSelectableTroopCount - this._currentTotalSelectedTroopCount), 5);
				}
				troopItem.CurrentAmount += num;
				this._currentTotalSelectedTroopCount += num;
			}
			this.OnCurrentSelectedAmountChange();
		}

		// Token: 0x06000D7A RID: 3450 RVA: 0x00036E74 File Offset: 0x00035074
		private void OnCurrentSelectedAmountChange()
		{
			foreach (TroopSelectionItemVM troopSelectionItemVM in this.Troops)
			{
				troopSelectionItemVM.IsRosterFull = this._currentTotalSelectedTroopCount >= this._maxSelectableTroopCount;
			}
			GameTexts.SetVariable("LEFT", this._currentTotalSelectedTroopCount);
			GameTexts.SetVariable("RIGHT", this._maxSelectableTroopCount);
			this.CurrentSelectedAmountText = GameTexts.FindText("str_LEFT_over_RIGHT_in_paranthesis", null).ToString();
			this.IsDoneEnabled = this._currentTotalSelectedTroopCount <= this._maxSelectableTroopCount && this._currentTotalSelectedTroopCount >= this._minSelectableTroopCount;
		}

		// Token: 0x06000D7B RID: 3451 RVA: 0x00036F30 File Offset: 0x00035130
		private void OnDone()
		{
			TroopRoster troopRoster = TroopRoster.CreateDummyTroopRoster();
			foreach (TroopSelectionItemVM troopSelectionItemVM in this.Troops)
			{
				if (troopSelectionItemVM.CurrentAmount > 0)
				{
					troopRoster.AddToCounts(troopSelectionItemVM.Troop.Character, troopSelectionItemVM.CurrentAmount, false, 0, 0, true, -1);
				}
			}
			this.IsEnabled = false;
			this._onDone.DynamicInvokeWithLog(new object[] { troopRoster });
		}

		// Token: 0x06000D7C RID: 3452 RVA: 0x00036FC0 File Offset: 0x000351C0
		public void ExecuteDone()
		{
			if (this._currentTotalSelectedTroopCount < this._maxSelectableTroopCount)
			{
				string text = new TextObject("{=z2Slmx4N}There are still some room for more soldiers. Do you want to proceed?", null).ToString();
				InformationManager.ShowInquiry(new InquiryData(this.TitleText, text, true, true, GameTexts.FindText("str_yes", null).ToString(), GameTexts.FindText("str_no", null).ToString(), new Action(this.OnDone), null, "", 0f, null, null, null), false, false);
				return;
			}
			this.OnDone();
		}

		// Token: 0x06000D7D RID: 3453 RVA: 0x00037042 File Offset: 0x00035242
		public void ExecuteCancel()
		{
			this.IsEnabled = false;
		}

		// Token: 0x06000D7E RID: 3454 RVA: 0x0003704B File Offset: 0x0003524B
		public void ExecuteReset()
		{
			this.InitList();
			this.OnCurrentSelectedAmountChange();
		}

		// Token: 0x06000D7F RID: 3455 RVA: 0x00037059 File Offset: 0x00035259
		public void ExecuteClearSelection()
		{
			this.Troops.ApplyActionOnAllItems(delegate(TroopSelectionItemVM troopItem)
			{
				if (this._canChangeChangeStatusOfTroop(troopItem.Troop.Character))
				{
					int currentAmount = troopItem.CurrentAmount;
					for (int i = 0; i < currentAmount; i++)
					{
						troopItem.ExecuteRemove();
					}
				}
			});
		}

		// Token: 0x06000D80 RID: 3456 RVA: 0x00037072 File Offset: 0x00035272
		public override void OnFinalize()
		{
			base.OnFinalize();
			InputKeyItemVM cancelInputKey = this.CancelInputKey;
			if (cancelInputKey != null)
			{
				cancelInputKey.OnFinalize();
			}
			InputKeyItemVM doneInputKey = this.DoneInputKey;
			if (doneInputKey != null)
			{
				doneInputKey.OnFinalize();
			}
			InputKeyItemVM resetInputKey = this.ResetInputKey;
			if (resetInputKey == null)
			{
				return;
			}
			resetInputKey.OnFinalize();
		}

		// Token: 0x06000D81 RID: 3457 RVA: 0x000370AC File Offset: 0x000352AC
		public void SetCancelInputKey(HotKey hotkey)
		{
			this.CancelInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
		}

		// Token: 0x06000D82 RID: 3458 RVA: 0x000370BB File Offset: 0x000352BB
		public void SetDoneInputKey(HotKey hotkey)
		{
			this.DoneInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
		}

		// Token: 0x06000D83 RID: 3459 RVA: 0x000370CA File Offset: 0x000352CA
		public void SetResetInputKey(HotKey hotkey)
		{
			this.ResetInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
		}

		// Token: 0x1700045E RID: 1118
		// (get) Token: 0x06000D84 RID: 3460 RVA: 0x000370D9 File Offset: 0x000352D9
		// (set) Token: 0x06000D85 RID: 3461 RVA: 0x000370E1 File Offset: 0x000352E1
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

		// Token: 0x1700045F RID: 1119
		// (get) Token: 0x06000D86 RID: 3462 RVA: 0x000370FF File Offset: 0x000352FF
		// (set) Token: 0x06000D87 RID: 3463 RVA: 0x00037107 File Offset: 0x00035307
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

		// Token: 0x17000460 RID: 1120
		// (get) Token: 0x06000D88 RID: 3464 RVA: 0x00037125 File Offset: 0x00035325
		// (set) Token: 0x06000D89 RID: 3465 RVA: 0x0003712D File Offset: 0x0003532D
		[DataSourceProperty]
		public InputKeyItemVM ResetInputKey
		{
			get
			{
				return this._resetInputKey;
			}
			set
			{
				if (value != this._resetInputKey)
				{
					this._resetInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "ResetInputKey");
				}
			}
		}

		// Token: 0x17000461 RID: 1121
		// (get) Token: 0x06000D8A RID: 3466 RVA: 0x0003714B File Offset: 0x0003534B
		// (set) Token: 0x06000D8B RID: 3467 RVA: 0x00037153 File Offset: 0x00035353
		[DataSourceProperty]
		public bool IsEnabled
		{
			get
			{
				return this._isEnabled;
			}
			set
			{
				if (value != this._isEnabled)
				{
					this._isEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsEnabled");
				}
			}
		}

		// Token: 0x17000462 RID: 1122
		// (get) Token: 0x06000D8C RID: 3468 RVA: 0x00037171 File Offset: 0x00035371
		// (set) Token: 0x06000D8D RID: 3469 RVA: 0x00037179 File Offset: 0x00035379
		[DataSourceProperty]
		public bool IsDoneEnabled
		{
			get
			{
				return this._isDoneEnabled;
			}
			set
			{
				if (value != this._isDoneEnabled)
				{
					this._isDoneEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsDoneEnabled");
				}
			}
		}

		// Token: 0x17000463 RID: 1123
		// (get) Token: 0x06000D8E RID: 3470 RVA: 0x00037197 File Offset: 0x00035397
		// (set) Token: 0x06000D8F RID: 3471 RVA: 0x0003719F File Offset: 0x0003539F
		[DataSourceProperty]
		public MBBindingList<TroopSelectionItemVM> Troops
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
					base.OnPropertyChangedWithValue<MBBindingList<TroopSelectionItemVM>>(value, "Troops");
				}
			}
		}

		// Token: 0x17000464 RID: 1124
		// (get) Token: 0x06000D90 RID: 3472 RVA: 0x000371BD File Offset: 0x000353BD
		// (set) Token: 0x06000D91 RID: 3473 RVA: 0x000371C5 File Offset: 0x000353C5
		[DataSourceProperty]
		public string DoneText
		{
			get
			{
				return this._doneText;
			}
			set
			{
				if (value != this._doneText)
				{
					this._doneText = value;
					base.OnPropertyChangedWithValue<string>(value, "DoneText");
				}
			}
		}

		// Token: 0x17000465 RID: 1125
		// (get) Token: 0x06000D92 RID: 3474 RVA: 0x000371E8 File Offset: 0x000353E8
		// (set) Token: 0x06000D93 RID: 3475 RVA: 0x000371F0 File Offset: 0x000353F0
		[DataSourceProperty]
		public string CancelText
		{
			get
			{
				return this._cancelText;
			}
			set
			{
				if (value != this._cancelText)
				{
					this._cancelText = value;
					base.OnPropertyChangedWithValue<string>(value, "CancelText");
				}
			}
		}

		// Token: 0x17000466 RID: 1126
		// (get) Token: 0x06000D94 RID: 3476 RVA: 0x00037213 File Offset: 0x00035413
		// (set) Token: 0x06000D95 RID: 3477 RVA: 0x0003721B File Offset: 0x0003541B
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

		// Token: 0x17000467 RID: 1127
		// (get) Token: 0x06000D96 RID: 3478 RVA: 0x0003723E File Offset: 0x0003543E
		// (set) Token: 0x06000D97 RID: 3479 RVA: 0x00037246 File Offset: 0x00035446
		[DataSourceProperty]
		public string ClearSelectionText
		{
			get
			{
				return this._clearSelectionText;
			}
			set
			{
				if (value != this._clearSelectionText)
				{
					this._clearSelectionText = value;
					base.OnPropertyChangedWithValue<string>(value, "ClearSelectionText");
				}
			}
		}

		// Token: 0x17000468 RID: 1128
		// (get) Token: 0x06000D98 RID: 3480 RVA: 0x00037269 File Offset: 0x00035469
		// (set) Token: 0x06000D99 RID: 3481 RVA: 0x00037271 File Offset: 0x00035471
		[DataSourceProperty]
		public string CurrentSelectedAmountText
		{
			get
			{
				return this._currentSelectedAmountText;
			}
			set
			{
				if (value != this._currentSelectedAmountText)
				{
					this._currentSelectedAmountText = value;
					base.OnPropertyChangedWithValue<string>(value, "CurrentSelectedAmountText");
				}
			}
		}

		// Token: 0x17000469 RID: 1129
		// (get) Token: 0x06000D9A RID: 3482 RVA: 0x00037294 File Offset: 0x00035494
		// (set) Token: 0x06000D9B RID: 3483 RVA: 0x0003729C File Offset: 0x0003549C
		[DataSourceProperty]
		public string CurrentSelectedAmountTitle
		{
			get
			{
				return this._currentSelectedAmountTitle;
			}
			set
			{
				if (value != this._currentSelectedAmountTitle)
				{
					this._currentSelectedAmountTitle = value;
					base.OnPropertyChangedWithValue<string>(value, "CurrentSelectedAmountTitle");
				}
			}
		}

		// Token: 0x0400063F RID: 1599
		private readonly Action<TroopRoster> _onDone;

		// Token: 0x04000640 RID: 1600
		private readonly TroopRoster _fullRoster;

		// Token: 0x04000641 RID: 1601
		private readonly TroopRoster _initialSelections;

		// Token: 0x04000642 RID: 1602
		private readonly Func<CharacterObject, bool> _canChangeChangeStatusOfTroop;

		// Token: 0x04000643 RID: 1603
		private readonly int _maxSelectableTroopCount;

		// Token: 0x04000644 RID: 1604
		private readonly int _minSelectableTroopCount;

		// Token: 0x04000645 RID: 1605
		private readonly TextObject _titleTextObject = new TextObject("{=uQgNPJnc}Manage Troops", null);

		// Token: 0x04000646 RID: 1606
		private readonly TextObject _chosenTitleTextObject = new TextObject("{=InqmgBiF}Chosen Crew", null);

		// Token: 0x04000647 RID: 1607
		private int _currentTotalSelectedTroopCount;

		// Token: 0x04000648 RID: 1608
		public bool IsFiveStackModifierActive;

		// Token: 0x04000649 RID: 1609
		public bool IsEntireStackModifierActive;

		// Token: 0x0400064A RID: 1610
		private InputKeyItemVM _doneInputKey;

		// Token: 0x0400064B RID: 1611
		private InputKeyItemVM _cancelInputKey;

		// Token: 0x0400064C RID: 1612
		private InputKeyItemVM _resetInputKey;

		// Token: 0x0400064D RID: 1613
		private bool _isEnabled;

		// Token: 0x0400064E RID: 1614
		private bool _isDoneEnabled;

		// Token: 0x0400064F RID: 1615
		private string _doneText;

		// Token: 0x04000650 RID: 1616
		private string _cancelText;

		// Token: 0x04000651 RID: 1617
		private string _titleText;

		// Token: 0x04000652 RID: 1618
		private string _clearSelectionText;

		// Token: 0x04000653 RID: 1619
		private string _currentSelectedAmountText;

		// Token: 0x04000654 RID: 1620
		private string _currentSelectedAmountTitle;

		// Token: 0x04000655 RID: 1621
		private MBBindingList<TroopSelectionItemVM> _troops;
	}
}
