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
	public class CraftingHistoryVM : ViewModel
	{
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

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.TitleText = CraftingHistoryVM._craftingHistoryText.ToString();
			this.DoneText = GameTexts.FindText("str_done", null).ToString();
			this.CancelText = GameTexts.FindText("str_cancel", null).ToString();
			this.RefreshAvailability();
		}

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

		public override void OnFinalize()
		{
			base.OnFinalize();
			this.FinalizeHistory();
			this.DoneKey.OnFinalize();
			this.CancelKey.OnFinalize();
		}

		public void RefreshAvailability()
		{
			CraftingOrder activeOrder = this._getActiveOrder();
			this.HasItemsInHistory = ((activeOrder == null) ? (this._craftingBehavior.CraftingHistory.Count > 0) : this._craftingBehavior.CraftingHistory.Any((WeaponDesign x) => x.Template.StringId == activeOrder.PreCraftedWeaponDesignItem.WeaponDesign.Template.StringId));
			this.HistoryHint.HintText = (this.HasItemsInHistory ? CraftingHistoryVM._craftingHistoryText : CraftingHistoryVM._noItemsHint);
		}

		public void ExecuteOpen()
		{
			this.RefreshCraftingHistory();
			this.IsVisible = true;
		}

		public void ExecuteCancel()
		{
			this.IsVisible = false;
		}

		public void ExecuteDone()
		{
			Action<WeaponDesignSelectorVM> onDone = this._onDone;
			if (onDone != null)
			{
				onDone(this.SelectedDesign);
			}
			this.ExecuteCancel();
		}

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

		public void SetDoneKey(HotKey hotkey)
		{
			this.DoneKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
		}

		public void SetCancelKey(HotKey hotkey)
		{
			this.CancelKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
		}

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

		private static TextObject _noItemsHint = new TextObject("{=saHYZKLt}There are no available items in history", null);

		private static TextObject _craftingHistoryText = new TextObject("{=xW4BPVLX}Crafting History", null);

		private ICraftingCampaignBehavior _craftingBehavior;

		private Func<CraftingOrder> _getActiveOrder;

		private Action<WeaponDesignSelectorVM> _onDone;

		private Crafting _crafting;

		private bool _isDoneAvailable;

		private bool _isVisible;

		private bool _hasItemsInHistory;

		private HintViewModel _historyHint;

		private MBBindingList<WeaponDesignSelectorVM> _craftingHistory;

		private WeaponDesignSelectorVM _selectedDesign;

		private string _titleText;

		private string _doneText;

		private string _cancelText;

		private InputKeyItemVM _cancelKey;

		private InputKeyItemVM _doneKey;
	}
}
