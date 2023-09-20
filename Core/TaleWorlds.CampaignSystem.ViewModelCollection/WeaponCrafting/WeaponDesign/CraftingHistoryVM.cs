using System;
using System.Linq;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.CraftingSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.Input;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.WeaponDesign
{
	// Token: 0x020000DB RID: 219
	public class CraftingHistoryVM : ViewModel
	{
		// Token: 0x06001475 RID: 5237 RVA: 0x0004D990 File Offset: 0x0004BB90
		public CraftingHistoryVM(Crafting crafting, ICraftingCampaignBehavior craftingBehavior, Func<CraftingOrder> getActiveOrder, Action<WeaponDesignSelectorVM> onDone)
		{
			this._crafting = crafting;
			this._craftingBehavior = craftingBehavior;
			this._getActiveOrder = getActiveOrder;
			this._onDone = onDone;
			this.CraftingHistory = new MBBindingList<WeaponDesignSelectorVM>();
			this.HistoryHint = new HintViewModel();
			this.RefreshValues();
		}

		// Token: 0x06001476 RID: 5238 RVA: 0x0004D9DC File Offset: 0x0004BBDC
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.TitleText = CraftingHistoryVM._craftingHistoryText.ToString();
			this.DoneText = GameTexts.FindText("str_done", null).ToString();
			this.CancelText = GameTexts.FindText("str_cancel", null).ToString();
			this.RefreshAvailability();
		}

		// Token: 0x06001477 RID: 5239 RVA: 0x0004DA34 File Offset: 0x0004BC34
		private void RefreshCraftingHistory()
		{
			this.FinalizeHistory();
			CraftingOrder craftingOrder = this._getActiveOrder();
			foreach (WeaponDesign weaponDesign in this._craftingBehavior.CraftingHistory)
			{
				if (craftingOrder == null || weaponDesign.Template.TemplateName.ToString() == craftingOrder.PreCraftedWeaponDesignItem.WeaponDesign.Template.TemplateName.ToString())
				{
					this.CraftingHistory.Add(new WeaponDesignSelectorVM(weaponDesign, new Action<WeaponDesignSelectorVM>(this.ExecuteSelect)));
				}
			}
			this.HasItemsInHistory = this.CraftingHistory.Count > 0;
			this.ExecuteSelect(null);
		}

		// Token: 0x06001478 RID: 5240 RVA: 0x0004DB00 File Offset: 0x0004BD00
		private void FinalizeHistory()
		{
			if (this.CraftingHistory.Count > 0)
			{
				foreach (WeaponDesignSelectorVM weaponDesignSelectorVM in this.CraftingHistory)
				{
					weaponDesignSelectorVM.OnFinalize();
				}
			}
			this.CraftingHistory.Clear();
		}

		// Token: 0x06001479 RID: 5241 RVA: 0x0004DB64 File Offset: 0x0004BD64
		public override void OnFinalize()
		{
			base.OnFinalize();
			this.FinalizeHistory();
			this.DoneKey.OnFinalize();
			this.CancelKey.OnFinalize();
		}

		// Token: 0x0600147A RID: 5242 RVA: 0x0004DB88 File Offset: 0x0004BD88
		public void RefreshAvailability()
		{
			CraftingOrder activeOrder = this._getActiveOrder();
			this.HasItemsInHistory = ((activeOrder == null) ? (this._craftingBehavior.CraftingHistory.Count > 0) : this._craftingBehavior.CraftingHistory.Any((WeaponDesign x) => x.Template.StringId == activeOrder.PreCraftedWeaponDesignItem.WeaponDesign.Template.StringId));
			this.HistoryHint.HintText = (this.HasItemsInHistory ? CraftingHistoryVM._craftingHistoryText : CraftingHistoryVM._noItemsHint);
		}

		// Token: 0x0600147B RID: 5243 RVA: 0x0004DC0A File Offset: 0x0004BE0A
		public void ExecuteOpen()
		{
			this.RefreshCraftingHistory();
			this.IsVisible = true;
		}

		// Token: 0x0600147C RID: 5244 RVA: 0x0004DC19 File Offset: 0x0004BE19
		public void ExecuteCancel()
		{
			this.IsVisible = false;
		}

		// Token: 0x0600147D RID: 5245 RVA: 0x0004DC22 File Offset: 0x0004BE22
		public void ExecuteDone()
		{
			Action<WeaponDesignSelectorVM> onDone = this._onDone;
			if (onDone != null)
			{
				onDone(this.SelectedDesign);
			}
			this.ExecuteCancel();
		}

		// Token: 0x0600147E RID: 5246 RVA: 0x0004DC41 File Offset: 0x0004BE41
		private void ExecuteSelect(WeaponDesignSelectorVM selector)
		{
			this.IsDoneAvailable = selector != null;
			if (this.SelectedDesign != null)
			{
				this.SelectedDesign.IsSelected = false;
			}
			this.SelectedDesign = selector;
			if (this.SelectedDesign != null)
			{
				this.SelectedDesign.IsSelected = true;
			}
		}

		// Token: 0x170006E0 RID: 1760
		// (get) Token: 0x0600147F RID: 5247 RVA: 0x0004DC7C File Offset: 0x0004BE7C
		// (set) Token: 0x06001480 RID: 5248 RVA: 0x0004DC84 File Offset: 0x0004BE84
		[DataSourceProperty]
		public bool IsDoneAvailable
		{
			get
			{
				return this._isDoneAvailable;
			}
			set
			{
				if (value != this._isDoneAvailable)
				{
					this._isDoneAvailable = value;
					base.OnPropertyChangedWithValue(value, "IsDoneAvailable");
				}
			}
		}

		// Token: 0x170006E1 RID: 1761
		// (get) Token: 0x06001481 RID: 5249 RVA: 0x0004DCA2 File Offset: 0x0004BEA2
		// (set) Token: 0x06001482 RID: 5250 RVA: 0x0004DCAA File Offset: 0x0004BEAA
		[DataSourceProperty]
		public bool IsVisible
		{
			get
			{
				return this._isVisible;
			}
			set
			{
				if (value != this._isVisible)
				{
					this._isVisible = value;
					base.OnPropertyChangedWithValue(value, "IsVisible");
				}
			}
		}

		// Token: 0x170006E2 RID: 1762
		// (get) Token: 0x06001483 RID: 5251 RVA: 0x0004DCC8 File Offset: 0x0004BEC8
		// (set) Token: 0x06001484 RID: 5252 RVA: 0x0004DCD0 File Offset: 0x0004BED0
		[DataSourceProperty]
		public bool HasItemsInHistory
		{
			get
			{
				return this._hasItemsInHistory;
			}
			set
			{
				if (value != this._hasItemsInHistory)
				{
					this._hasItemsInHistory = value;
					base.OnPropertyChangedWithValue(value, "HasItemsInHistory");
				}
			}
		}

		// Token: 0x170006E3 RID: 1763
		// (get) Token: 0x06001485 RID: 5253 RVA: 0x0004DCEE File Offset: 0x0004BEEE
		// (set) Token: 0x06001486 RID: 5254 RVA: 0x0004DCF6 File Offset: 0x0004BEF6
		[DataSourceProperty]
		public HintViewModel HistoryHint
		{
			get
			{
				return this._historyHint;
			}
			set
			{
				if (value != this._historyHint)
				{
					this._historyHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "HistoryHint");
				}
			}
		}

		// Token: 0x170006E4 RID: 1764
		// (get) Token: 0x06001487 RID: 5255 RVA: 0x0004DD14 File Offset: 0x0004BF14
		// (set) Token: 0x06001488 RID: 5256 RVA: 0x0004DD1C File Offset: 0x0004BF1C
		[DataSourceProperty]
		public MBBindingList<WeaponDesignSelectorVM> CraftingHistory
		{
			get
			{
				return this._craftingHistory;
			}
			set
			{
				if (value != this._craftingHistory)
				{
					this._craftingHistory = value;
					base.OnPropertyChangedWithValue<MBBindingList<WeaponDesignSelectorVM>>(value, "CraftingHistory");
				}
			}
		}

		// Token: 0x170006E5 RID: 1765
		// (get) Token: 0x06001489 RID: 5257 RVA: 0x0004DD3A File Offset: 0x0004BF3A
		// (set) Token: 0x0600148A RID: 5258 RVA: 0x0004DD42 File Offset: 0x0004BF42
		[DataSourceProperty]
		public WeaponDesignSelectorVM SelectedDesign
		{
			get
			{
				return this._selectedDesign;
			}
			set
			{
				if (value != this._selectedDesign)
				{
					this._selectedDesign = value;
					base.OnPropertyChangedWithValue<WeaponDesignSelectorVM>(value, "SelectedDesign");
				}
			}
		}

		// Token: 0x170006E6 RID: 1766
		// (get) Token: 0x0600148B RID: 5259 RVA: 0x0004DD60 File Offset: 0x0004BF60
		// (set) Token: 0x0600148C RID: 5260 RVA: 0x0004DD68 File Offset: 0x0004BF68
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

		// Token: 0x170006E7 RID: 1767
		// (get) Token: 0x0600148D RID: 5261 RVA: 0x0004DD8B File Offset: 0x0004BF8B
		// (set) Token: 0x0600148E RID: 5262 RVA: 0x0004DD93 File Offset: 0x0004BF93
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

		// Token: 0x170006E8 RID: 1768
		// (get) Token: 0x0600148F RID: 5263 RVA: 0x0004DDB6 File Offset: 0x0004BFB6
		// (set) Token: 0x06001490 RID: 5264 RVA: 0x0004DDBE File Offset: 0x0004BFBE
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

		// Token: 0x06001491 RID: 5265 RVA: 0x0004DDE1 File Offset: 0x0004BFE1
		public void SetDoneKey(HotKey hotkey)
		{
			this.DoneKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
		}

		// Token: 0x06001492 RID: 5266 RVA: 0x0004DDF0 File Offset: 0x0004BFF0
		public void SetCancelKey(HotKey hotkey)
		{
			this.CancelKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
		}

		// Token: 0x170006E9 RID: 1769
		// (get) Token: 0x06001493 RID: 5267 RVA: 0x0004DDFF File Offset: 0x0004BFFF
		// (set) Token: 0x06001494 RID: 5268 RVA: 0x0004DE07 File Offset: 0x0004C007
		public InputKeyItemVM CancelKey
		{
			get
			{
				return this._cancelKey;
			}
			set
			{
				if (value != this._cancelKey)
				{
					this._cancelKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "CancelKey");
				}
			}
		}

		// Token: 0x170006EA RID: 1770
		// (get) Token: 0x06001495 RID: 5269 RVA: 0x0004DE25 File Offset: 0x0004C025
		// (set) Token: 0x06001496 RID: 5270 RVA: 0x0004DE2D File Offset: 0x0004C02D
		public InputKeyItemVM DoneKey
		{
			get
			{
				return this._doneKey;
			}
			set
			{
				if (value != this._doneKey)
				{
					this._doneKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "DoneKey");
				}
			}
		}

		// Token: 0x04000987 RID: 2439
		private static TextObject _noItemsHint = new TextObject("{=saHYZKLt}There are no available items in history", null);

		// Token: 0x04000988 RID: 2440
		private static TextObject _craftingHistoryText = new TextObject("{=xW4BPVLX}Crafting History", null);

		// Token: 0x04000989 RID: 2441
		private ICraftingCampaignBehavior _craftingBehavior;

		// Token: 0x0400098A RID: 2442
		private Func<CraftingOrder> _getActiveOrder;

		// Token: 0x0400098B RID: 2443
		private Action<WeaponDesignSelectorVM> _onDone;

		// Token: 0x0400098C RID: 2444
		private Crafting _crafting;

		// Token: 0x0400098D RID: 2445
		private bool _isDoneAvailable;

		// Token: 0x0400098E RID: 2446
		private bool _isVisible;

		// Token: 0x0400098F RID: 2447
		private bool _hasItemsInHistory;

		// Token: 0x04000990 RID: 2448
		private HintViewModel _historyHint;

		// Token: 0x04000991 RID: 2449
		private MBBindingList<WeaponDesignSelectorVM> _craftingHistory;

		// Token: 0x04000992 RID: 2450
		private WeaponDesignSelectorVM _selectedDesign;

		// Token: 0x04000993 RID: 2451
		private string _titleText;

		// Token: 0x04000994 RID: 2452
		private string _doneText;

		// Token: 0x04000995 RID: 2453
		private string _cancelText;

		// Token: 0x04000996 RID: 2454
		private InputKeyItemVM _cancelKey;

		// Token: 0x04000997 RID: 2455
		private InputKeyItemVM _doneKey;
	}
}
