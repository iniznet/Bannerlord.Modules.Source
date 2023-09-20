using System;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.CraftingSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.Input;
using TaleWorlds.CampaignSystem.ViewModelCollection.Inventory;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.WeaponDesign
{
	// Token: 0x020000E5 RID: 229
	public class WeaponDesignResultPopupVM : ViewModel
	{
		// Token: 0x060014FB RID: 5371 RVA: 0x0004EA94 File Offset: 0x0004CC94
		public WeaponDesignResultPopupVM(Action onFinalize, Crafting crafting, CraftingOrder completedOrder, MBBindingList<ItemFlagVM> weaponFlagIconsList, ItemObject craftedItem, MBBindingList<WeaponDesignResultPropertyItemVM> designResultPropertyList, string itemName, ItemCollectionElementViewModel itemVisualModel)
		{
			this._craftedItem = craftedItem;
			this._onFinalize = onFinalize;
			this._crafting = crafting;
			this._completedOrder = completedOrder;
			this._craftingBehavior = Campaign.Current.GetCampaignBehavior<ICraftingCampaignBehavior>();
			this.WeaponFlagIconsList = weaponFlagIconsList;
			this.DesignResultPropertyList = designResultPropertyList;
			this.ItemName = itemName;
			this.ItemVisualModel = itemVisualModel;
			Game game = Game.Current;
			if (game != null)
			{
				game.EventManager.TriggerEvent<CraftingWeaponResultPopupToggledEvent>(new CraftingWeaponResultPopupToggledEvent(true));
			}
			this.RefreshValues();
		}

		// Token: 0x060014FC RID: 5372 RVA: 0x0004EB18 File Offset: 0x0004CD18
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.IsInOrderMode = this._completedOrder != null;
			this.WeaponCraftedText = new TextObject("{=0mqdFC2x}Weapon Crafted!", null).ToString();
			this.DoneLbl = GameTexts.FindText("str_done", null).ToString();
			if (this._isInOrderMode)
			{
				bool flag;
				TextObject textObject;
				TextObject textObject2;
				int num;
				this._craftingBehavior.GetOrderResult(this._completedOrder, this._craftedItem, out flag, out textObject, out textObject2, out num);
				this.CraftedWeaponInitialWorth = this._completedOrder.BaseGoldReward;
				this.CraftedWeaponFinalWorth = num;
				this.IsOrderSuccessful = flag;
				this.CraftedWeaponWorthText = new TextObject("{=ZIn8W5ZG}Worth", null).ToString();
				this.DesignResultPropertyList.Add(new WeaponDesignResultPropertyItemVM(new TextObject("{=QmfZjCo1}Worth: ", null), (float)this.CraftedWeaponInitialWorth, (float)this.CraftedWeaponInitialWorth, (float)(this.CraftedWeaponFinalWorth - this.CraftedWeaponInitialWorth), false, true, false));
				this.OrderOwnerRemarkText = textObject.ToString();
				this.OrderResultText = textObject2.ToString();
			}
		}

		// Token: 0x060014FD RID: 5373 RVA: 0x0004EC17 File Offset: 0x0004CE17
		private void UpdateConfirmAvailability()
		{
			this.CanConfirm = true;
			if (string.IsNullOrEmpty(this.ItemName))
			{
				this.CanConfirm = false;
				this.ConfirmDisabledReasonHint = new HintViewModel(new TextObject("{=QQ03J6sf}Item name can not be empty.", null), null);
			}
		}

		// Token: 0x060014FE RID: 5374 RVA: 0x0004EC4B File Offset: 0x0004CE4B
		public override void OnFinalize()
		{
			base.OnFinalize();
			this.DoneInputKey.OnFinalize();
		}

		// Token: 0x060014FF RID: 5375 RVA: 0x0004EC5E File Offset: 0x0004CE5E
		public void ExecuteFinalizeCrafting()
		{
			this._crafting.SetCraftedWeaponName(this.ItemName);
			Action onFinalize = this._onFinalize;
			if (onFinalize != null)
			{
				onFinalize();
			}
			Game game = Game.Current;
			if (game == null)
			{
				return;
			}
			game.EventManager.TriggerEvent<CraftingWeaponResultPopupToggledEvent>(new CraftingWeaponResultPopupToggledEvent(false));
		}

		// Token: 0x06001500 RID: 5376 RVA: 0x0004EC9C File Offset: 0x0004CE9C
		public void ExecuteRandomCraftName()
		{
			this.ItemName = this._crafting.GetRandomCraftName().ToString();
		}

		// Token: 0x17000711 RID: 1809
		// (get) Token: 0x06001501 RID: 5377 RVA: 0x0004ECB4 File Offset: 0x0004CEB4
		// (set) Token: 0x06001502 RID: 5378 RVA: 0x0004ECBC File Offset: 0x0004CEBC
		[DataSourceProperty]
		public MBBindingList<ItemFlagVM> WeaponFlagIconsList
		{
			get
			{
				return this._weaponFlagIconsList;
			}
			set
			{
				if (value != this._weaponFlagIconsList)
				{
					this._weaponFlagIconsList = value;
					base.OnPropertyChangedWithValue<MBBindingList<ItemFlagVM>>(value, "WeaponFlagIconsList");
				}
			}
		}

		// Token: 0x17000712 RID: 1810
		// (get) Token: 0x06001503 RID: 5379 RVA: 0x0004ECDA File Offset: 0x0004CEDA
		// (set) Token: 0x06001504 RID: 5380 RVA: 0x0004ECE2 File Offset: 0x0004CEE2
		[DataSourceProperty]
		public bool IsInOrderMode
		{
			get
			{
				return this._isInOrderMode;
			}
			set
			{
				if (value != this._isInOrderMode)
				{
					this._isInOrderMode = value;
					base.OnPropertyChangedWithValue(value, "IsInOrderMode");
				}
			}
		}

		// Token: 0x17000713 RID: 1811
		// (get) Token: 0x06001505 RID: 5381 RVA: 0x0004ED00 File Offset: 0x0004CF00
		// (set) Token: 0x06001506 RID: 5382 RVA: 0x0004ED08 File Offset: 0x0004CF08
		[DataSourceProperty]
		public int CraftedWeaponFinalWorth
		{
			get
			{
				return this._craftedWeaponFinalWorth;
			}
			set
			{
				if (value != this._craftedWeaponFinalWorth)
				{
					this._craftedWeaponFinalWorth = value;
					base.OnPropertyChangedWithValue(value, "CraftedWeaponFinalWorth");
				}
			}
		}

		// Token: 0x17000714 RID: 1812
		// (get) Token: 0x06001507 RID: 5383 RVA: 0x0004ED26 File Offset: 0x0004CF26
		// (set) Token: 0x06001508 RID: 5384 RVA: 0x0004ED2E File Offset: 0x0004CF2E
		[DataSourceProperty]
		public int CraftedWeaponPriceDifference
		{
			get
			{
				return this._craftedWeaponPriceDifference;
			}
			set
			{
				if (value != this._craftedWeaponPriceDifference)
				{
					this._craftedWeaponPriceDifference = value;
					base.OnPropertyChangedWithValue(value, "CraftedWeaponPriceDifference");
				}
			}
		}

		// Token: 0x17000715 RID: 1813
		// (get) Token: 0x06001509 RID: 5385 RVA: 0x0004ED4C File Offset: 0x0004CF4C
		// (set) Token: 0x0600150A RID: 5386 RVA: 0x0004ED54 File Offset: 0x0004CF54
		[DataSourceProperty]
		public int CraftedWeaponInitialWorth
		{
			get
			{
				return this._craftedWeaponInitialWorth;
			}
			set
			{
				if (value != this._craftedWeaponInitialWorth)
				{
					this._craftedWeaponInitialWorth = value;
					base.OnPropertyChangedWithValue(value, "CraftedWeaponInitialWorth");
				}
			}
		}

		// Token: 0x17000716 RID: 1814
		// (get) Token: 0x0600150B RID: 5387 RVA: 0x0004ED72 File Offset: 0x0004CF72
		// (set) Token: 0x0600150C RID: 5388 RVA: 0x0004ED7A File Offset: 0x0004CF7A
		[DataSourceProperty]
		public string CraftedWeaponWorthText
		{
			get
			{
				return this._craftedWeaponWorthText;
			}
			set
			{
				if (value != this._craftedWeaponWorthText)
				{
					this._craftedWeaponWorthText = value;
					base.OnPropertyChangedWithValue<string>(value, "CraftedWeaponWorthText");
				}
			}
		}

		// Token: 0x17000717 RID: 1815
		// (get) Token: 0x0600150D RID: 5389 RVA: 0x0004ED9D File Offset: 0x0004CF9D
		// (set) Token: 0x0600150E RID: 5390 RVA: 0x0004EDA5 File Offset: 0x0004CFA5
		[DataSourceProperty]
		public bool IsOrderSuccessful
		{
			get
			{
				return this._isOrderSuccessful;
			}
			set
			{
				if (value != this._isOrderSuccessful)
				{
					this._isOrderSuccessful = value;
					base.OnPropertyChangedWithValue(value, "IsOrderSuccessful");
				}
			}
		}

		// Token: 0x17000718 RID: 1816
		// (get) Token: 0x0600150F RID: 5391 RVA: 0x0004EDC3 File Offset: 0x0004CFC3
		// (set) Token: 0x06001510 RID: 5392 RVA: 0x0004EDCB File Offset: 0x0004CFCB
		[DataSourceProperty]
		public bool CanConfirm
		{
			get
			{
				return this._canConfirm;
			}
			set
			{
				if (value != this._canConfirm)
				{
					this._canConfirm = value;
					base.OnPropertyChangedWithValue(value, "CanConfirm");
				}
			}
		}

		// Token: 0x17000719 RID: 1817
		// (get) Token: 0x06001511 RID: 5393 RVA: 0x0004EDE9 File Offset: 0x0004CFE9
		// (set) Token: 0x06001512 RID: 5394 RVA: 0x0004EDF1 File Offset: 0x0004CFF1
		[DataSourceProperty]
		public string OrderResultText
		{
			get
			{
				return this._orderResultText;
			}
			set
			{
				if (value != this._orderResultText)
				{
					this._orderResultText = value;
					base.OnPropertyChangedWithValue<string>(value, "OrderResultText");
				}
			}
		}

		// Token: 0x1700071A RID: 1818
		// (get) Token: 0x06001513 RID: 5395 RVA: 0x0004EE14 File Offset: 0x0004D014
		// (set) Token: 0x06001514 RID: 5396 RVA: 0x0004EE1C File Offset: 0x0004D01C
		[DataSourceProperty]
		public string OrderOwnerRemarkText
		{
			get
			{
				return this._orderOwnerRemarkText;
			}
			set
			{
				if (value != this._orderOwnerRemarkText)
				{
					this._orderOwnerRemarkText = value;
					base.OnPropertyChangedWithValue<string>(value, "OrderOwnerRemarkText");
				}
			}
		}

		// Token: 0x1700071B RID: 1819
		// (get) Token: 0x06001515 RID: 5397 RVA: 0x0004EE3F File Offset: 0x0004D03F
		// (set) Token: 0x06001516 RID: 5398 RVA: 0x0004EE47 File Offset: 0x0004D047
		[DataSourceProperty]
		public string WeaponCraftedText
		{
			get
			{
				return this._weaponCraftedText;
			}
			set
			{
				if (value != this._weaponCraftedText)
				{
					this._weaponCraftedText = value;
					base.OnPropertyChangedWithValue<string>(value, "WeaponCraftedText");
				}
			}
		}

		// Token: 0x1700071C RID: 1820
		// (get) Token: 0x06001517 RID: 5399 RVA: 0x0004EE6A File Offset: 0x0004D06A
		// (set) Token: 0x06001518 RID: 5400 RVA: 0x0004EE72 File Offset: 0x0004D072
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

		// Token: 0x1700071D RID: 1821
		// (get) Token: 0x06001519 RID: 5401 RVA: 0x0004EE95 File Offset: 0x0004D095
		// (set) Token: 0x0600151A RID: 5402 RVA: 0x0004EE9D File Offset: 0x0004D09D
		[DataSourceProperty]
		public MBBindingList<WeaponDesignResultPropertyItemVM> DesignResultPropertyList
		{
			get
			{
				return this._designResultPropertyList;
			}
			set
			{
				if (value != this._designResultPropertyList)
				{
					this._designResultPropertyList = value;
					base.OnPropertyChangedWithValue<MBBindingList<WeaponDesignResultPropertyItemVM>>(value, "DesignResultPropertyList");
				}
			}
		}

		// Token: 0x1700071E RID: 1822
		// (get) Token: 0x0600151B RID: 5403 RVA: 0x0004EEBB File Offset: 0x0004D0BB
		// (set) Token: 0x0600151C RID: 5404 RVA: 0x0004EEC3 File Offset: 0x0004D0C3
		[DataSourceProperty]
		public string ItemName
		{
			get
			{
				return this._itemName;
			}
			set
			{
				if (value != this._itemName)
				{
					this._itemName = value;
					this.UpdateConfirmAvailability();
					base.OnPropertyChangedWithValue<string>(value, "ItemName");
				}
			}
		}

		// Token: 0x1700071F RID: 1823
		// (get) Token: 0x0600151D RID: 5405 RVA: 0x0004EEEC File Offset: 0x0004D0EC
		// (set) Token: 0x0600151E RID: 5406 RVA: 0x0004EEF4 File Offset: 0x0004D0F4
		[DataSourceProperty]
		public ItemCollectionElementViewModel ItemVisualModel
		{
			get
			{
				return this._itemVisualModel;
			}
			set
			{
				if (value != this._itemVisualModel)
				{
					this._itemVisualModel = value;
					base.OnPropertyChangedWithValue<ItemCollectionElementViewModel>(value, "ItemVisualModel");
				}
			}
		}

		// Token: 0x17000720 RID: 1824
		// (get) Token: 0x0600151F RID: 5407 RVA: 0x0004EF12 File Offset: 0x0004D112
		// (set) Token: 0x06001520 RID: 5408 RVA: 0x0004EF1A File Offset: 0x0004D11A
		[DataSourceProperty]
		public HintViewModel ConfirmDisabledReasonHint
		{
			get
			{
				return this._confirmDisabledReasonHint;
			}
			set
			{
				if (value != this._confirmDisabledReasonHint)
				{
					this._confirmDisabledReasonHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "ConfirmDisabledReasonHint");
				}
			}
		}

		// Token: 0x06001521 RID: 5409 RVA: 0x0004EF38 File Offset: 0x0004D138
		public void SetDoneInputKey(HotKey hotkey)
		{
			this.DoneInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
		}

		// Token: 0x17000721 RID: 1825
		// (get) Token: 0x06001522 RID: 5410 RVA: 0x0004EF47 File Offset: 0x0004D147
		// (set) Token: 0x06001523 RID: 5411 RVA: 0x0004EF4F File Offset: 0x0004D14F
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

		// Token: 0x040009CA RID: 2506
		private Action _onFinalize;

		// Token: 0x040009CB RID: 2507
		private Crafting _crafting;

		// Token: 0x040009CC RID: 2508
		private CraftingOrder _completedOrder;

		// Token: 0x040009CD RID: 2509
		private ItemObject _craftedItem;

		// Token: 0x040009CE RID: 2510
		private readonly ICraftingCampaignBehavior _craftingBehavior;

		// Token: 0x040009CF RID: 2511
		private MBBindingList<ItemFlagVM> _weaponFlagIconsList;

		// Token: 0x040009D0 RID: 2512
		private bool _isInOrderMode;

		// Token: 0x040009D1 RID: 2513
		private string _orderResultText;

		// Token: 0x040009D2 RID: 2514
		private string _orderOwnerRemarkText;

		// Token: 0x040009D3 RID: 2515
		private bool _isOrderSuccessful;

		// Token: 0x040009D4 RID: 2516
		private bool _canConfirm;

		// Token: 0x040009D5 RID: 2517
		private string _craftedWeaponWorthText;

		// Token: 0x040009D6 RID: 2518
		private int _craftedWeaponInitialWorth;

		// Token: 0x040009D7 RID: 2519
		private int _craftedWeaponPriceDifference;

		// Token: 0x040009D8 RID: 2520
		private int _craftedWeaponFinalWorth;

		// Token: 0x040009D9 RID: 2521
		private string _weaponCraftedText;

		// Token: 0x040009DA RID: 2522
		private string _doneLbl;

		// Token: 0x040009DB RID: 2523
		private MBBindingList<WeaponDesignResultPropertyItemVM> _designResultPropertyList;

		// Token: 0x040009DC RID: 2524
		private string _itemName;

		// Token: 0x040009DD RID: 2525
		private ItemCollectionElementViewModel _itemVisualModel;

		// Token: 0x040009DE RID: 2526
		private HintViewModel _confirmDisabledReasonHint;

		// Token: 0x040009DF RID: 2527
		private InputKeyItemVM _doneInputKey;
	}
}
