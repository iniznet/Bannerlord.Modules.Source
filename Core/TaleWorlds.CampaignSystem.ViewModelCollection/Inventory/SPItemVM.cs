using System;
using System.Linq;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Inventory;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Inventory
{
	public class SPItemVM : ItemVM
	{
		public InventoryLogic.InventorySide InventorySide { get; private set; }

		public SPItemVM()
		{
			base.StringId = "";
			base.ImageIdentifier = new ImageIdentifierVM(ImageIdentifierType.Null);
			this._itemType = EquipmentIndex.None;
		}

		public SPItemVM(InventoryLogic inventoryLogic, bool isHeroFemale, bool canCharacterUseItem, InventoryMode usageType, ItemRosterElement newItem, InventoryLogic.InventorySide inventorySide, string fiveStackShortcutKeyText, string entireStackShortcutKeyText, int itemCost = 0, EquipmentIndex? itemType = -1)
		{
			if (newItem.EquipmentElement.Item == null)
			{
				return;
			}
			this._fiveStackShortcutKeyText = fiveStackShortcutKeyText;
			this._entireStackShortcutKeyText = entireStackShortcutKeyText;
			this._usageType = usageType;
			this._tradeGoodConceptObj = Concept.All.SingleOrDefault((Concept c) => c.StringId == "str_game_objects_trade_goods");
			this._itemConceptObj = Concept.All.SingleOrDefault((Concept c) => c.StringId == "str_game_objects_item");
			this._inventoryLogic = inventoryLogic;
			this.ItemRosterElement = new ItemRosterElement(newItem.EquipmentElement, newItem.Amount);
			base.ItemCost = itemCost;
			this.ItemCount = newItem.Amount;
			this.TransactionCount = 1;
			this.ItemLevel = newItem.EquipmentElement.Item.Difficulty;
			this.InventorySide = inventorySide;
			if (itemType != null)
			{
				EquipmentIndex? equipmentIndex = itemType;
				EquipmentIndex equipmentIndex2 = EquipmentIndex.None;
				if (!((equipmentIndex.GetValueOrDefault() == equipmentIndex2) & (equipmentIndex != null)))
				{
					this._itemType = itemType.Value;
				}
			}
			base.SetItemTypeId();
			base.ItemDescription = newItem.EquipmentElement.GetModifiedItemName().ToString();
			base.StringId = CampaignUIHelper.GetItemLockStringID(newItem.EquipmentElement);
			ItemObject item = newItem.EquipmentElement.Item;
			Clan playerClan = Clan.PlayerClan;
			base.ImageIdentifier = new ImageIdentifierVM(item, (playerClan != null) ? playerClan.Banner.Serialize() : null);
			this.IsCivilianItem = newItem.EquipmentElement.Item.ItemFlags.HasAnyFlag(ItemFlags.Civilian);
			this.IsGenderDifferent = (isHeroFemale && this.ItemRosterElement.EquipmentElement.Item.ItemFlags.HasAnyFlag(ItemFlags.NotUsableByFemale)) || (!isHeroFemale && this.ItemRosterElement.EquipmentElement.Item.ItemFlags.HasAnyFlag(ItemFlags.NotUsableByMale));
			this.CanCharacterUseItem = canCharacterUseItem;
			this.IsArtifact = newItem.EquipmentElement.Item.IsUniqueItem;
			this.UpdateCanBeSlaughtered();
			this.UpdateHintTexts();
			InventoryLogic inventoryLogic2 = this._inventoryLogic;
			this.CanBeDonated = inventoryLogic2 != null && inventoryLogic2.CanDonateItem(this.ItemRosterElement, this.InventorySide);
			this.TradeData = new InventoryTradeVM(this._inventoryLogic, this.ItemRosterElement, inventorySide, new Action<int, bool>(this.OnTradeApplyTransaction));
			this.IsTransferable = !this.ItemRosterElement.EquipmentElement.IsQuestItem;
			this.TradeData.IsTradeable = this.IsTransferable;
			this.IsEquipableItem = (InventoryManager.GetInventoryItemTypeOfItem(newItem.EquipmentElement.Item) & InventoryItemType.Equipable) > InventoryItemType.None;
			this.UpdateProfitType();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			if (this.ItemRosterElement.EquipmentElement.Item != null)
			{
				TextObject modifiedItemName = this.ItemRosterElement.EquipmentElement.GetModifiedItemName();
				base.ItemDescription = ((modifiedItemName != null) ? modifiedItemName.ToString() : null) ?? "";
				return;
			}
			base.ItemDescription = "";
		}

		public void RefreshWith(SPItemVM itemVM, InventoryLogic.InventorySide inventorySide)
		{
			this.InventorySide = inventorySide;
			if (itemVM == null)
			{
				this.Reset();
				return;
			}
			base.ItemDescription = itemVM.ItemDescription;
			base.ItemCost = itemVM.ItemCost;
			base.TypeId = itemVM.TypeId;
			this._itemType = itemVM.ItemType;
			this.ItemCount = itemVM.ItemCount;
			this.TransactionCount = itemVM.TransactionCount;
			this.ItemLevel = itemVM.ItemLevel;
			base.StringId = itemVM.StringId;
			base.ImageIdentifier = itemVM.ImageIdentifier.Clone();
			this.ItemRosterElement = itemVM.ItemRosterElement;
			this.IsCivilianItem = itemVM.IsCivilianItem;
			this.IsGenderDifferent = itemVM.IsGenderDifferent;
			this.IsEquipableItem = itemVM.IsEquipableItem;
			this.CanCharacterUseItem = this.CanCharacterUseItem;
			this.IsArtifact = itemVM.IsArtifact;
			this.UpdateCanBeSlaughtered();
			this.UpdateHintTexts();
			InventoryLogic inventoryLogic = this._inventoryLogic;
			this.CanBeDonated = inventoryLogic != null && inventoryLogic.CanDonateItem(this.ItemRosterElement, this.InventorySide);
			this.TradeData = new InventoryTradeVM(this._inventoryLogic, itemVM.ItemRosterElement, inventorySide, new Action<int, bool>(this.OnTradeApplyTransaction));
			this.UpdateProfitType();
		}

		private void Reset()
		{
			base.ItemDescription = "";
			base.ItemCost = 0;
			base.TypeId = 0;
			this._itemType = EquipmentIndex.None;
			this.ItemCount = 0;
			this.TransactionCount = 0;
			this.ItemLevel = 0;
			base.StringId = "";
			base.ImageIdentifier = new ImageIdentifierVM(ImageIdentifierType.Null);
			this.ItemRosterElement = default(ItemRosterElement);
			this.ProfitType = 0;
			this.IsCivilianItem = true;
			this.IsGenderDifferent = false;
			this.IsEquipableItem = true;
			this.IsArtifact = false;
			this.TradeData = new InventoryTradeVM(this._inventoryLogic, this.ItemRosterElement, InventoryLogic.InventorySide.None, new Action<int, bool>(this.OnTradeApplyTransaction));
		}

		private void UpdateProfitType()
		{
			this.ProfitType = 0;
			if (Campaign.Current != null)
			{
				if (this.InventorySide == InventoryLogic.InventorySide.PlayerInventory)
				{
					Hero mainHero = Hero.MainHero;
					if (mainHero == null || !mainHero.GetPerkValue(DefaultPerks.Trade.Appraiser))
					{
						Hero mainHero2 = Hero.MainHero;
						if (mainHero2 == null || !mainHero2.GetPerkValue(DefaultPerks.Trade.WholeSeller))
						{
							return;
						}
					}
					IPlayerTradeBehavior campaignBehavior = Campaign.Current.GetCampaignBehavior<IPlayerTradeBehavior>();
					if (campaignBehavior != null)
					{
						int num = -campaignBehavior.GetProjectedProfit(this.ItemRosterElement, base.ItemCost) + base.ItemCost;
						this.ProfitType = (int)SPItemVM.GetProfitTypeFromDiff((float)num, (float)base.ItemCost);
						return;
					}
				}
				else if (this.InventorySide == InventoryLogic.InventorySide.OtherInventory && Settlement.CurrentSettlement != null && (Settlement.CurrentSettlement.IsFortification || Settlement.CurrentSettlement.IsVillage))
				{
					Hero mainHero3 = Hero.MainHero;
					if (mainHero3 == null || !mainHero3.GetPerkValue(DefaultPerks.Trade.CaravanMaster))
					{
						Hero mainHero4 = Hero.MainHero;
						if (mainHero4 == null || !mainHero4.GetPerkValue(DefaultPerks.Trade.MarketDealer))
						{
							return;
						}
					}
					float averagePriceFactorItemCategory = this._inventoryLogic.GetAveragePriceFactorItemCategory(this.ItemRosterElement.EquipmentElement.Item.ItemCategory);
					Town town = (Settlement.CurrentSettlement.IsVillage ? Settlement.CurrentSettlement.Village.Bound.Town : Settlement.CurrentSettlement.Town);
					if (averagePriceFactorItemCategory != -99f)
					{
						this.ProfitType = (int)SPItemVM.GetProfitTypeFromDiff(town.MarketData.GetPriceFactor(this.ItemRosterElement.EquipmentElement.Item.ItemCategory), averagePriceFactorItemCategory);
					}
				}
			}
		}

		public void ExecuteBuySingle()
		{
			this.TransactionCount = 1;
			ItemVM.ProcessBuyItem(this, false);
		}

		public void ExecuteSellSingle()
		{
			this.TransactionCount = 1;
			SPItemVM.ProcessSellItem(this, false);
		}

		private void OnTradeApplyTransaction(int amount, bool isBuying)
		{
			this.TransactionCount = amount;
			if (isBuying)
			{
				ItemVM.ProcessBuyItem(this, true);
				return;
			}
			SPItemVM.ProcessSellItem(this, true);
		}

		public void ExecuteSellItem()
		{
			SPItemVM.ProcessSellItem(this, false);
		}

		public void ExecuteConcept()
		{
			if (this._tradeGoodConceptObj != null)
			{
				ItemObject item = this.ItemRosterElement.EquipmentElement.Item;
				if (item != null && item.Type == ItemObject.ItemTypeEnum.Goods)
				{
					Campaign.Current.EncyclopediaManager.GoToLink(this._tradeGoodConceptObj.EncyclopediaLink);
					return;
				}
			}
			if (this._itemConceptObj != null)
			{
				Campaign.Current.EncyclopediaManager.GoToLink(this._itemConceptObj.EncyclopediaLink);
			}
		}

		public void ExecuteResetTrade()
		{
			this.TradeData.ExecuteReset();
		}

		private void UpdateTotalCost()
		{
			if (this.TransactionCount <= 0 || this._inventoryLogic == null || this.InventorySide == InventoryLogic.InventorySide.Equipment)
			{
				return;
			}
			int num;
			this.TotalCost = this._inventoryLogic.GetItemTotalPrice(this.ItemRosterElement, this.TransactionCount, out num, this.InventorySide == InventoryLogic.InventorySide.OtherInventory);
		}

		public void UpdateTradeData(bool forceUpdateAmounts)
		{
			InventoryTradeVM tradeData = this.TradeData;
			if (tradeData != null)
			{
				tradeData.UpdateItemData(this.ItemRosterElement, this.InventorySide, forceUpdateAmounts);
			}
			this.UpdateProfitType();
		}

		public void ExecuteSlaughterItem()
		{
			if (this.CanBeSlaughtered)
			{
				SPItemVM.ProcessItemSlaughter(this);
			}
		}

		public void ExecuteDonateItem()
		{
			if (this.CanBeDonated)
			{
				SPItemVM.ProcessItemDonate(this);
			}
		}

		public void ExecuteSetFocused()
		{
			this.IsFocused = true;
			Action<SPItemVM> onFocus = SPItemVM.OnFocus;
			if (onFocus == null)
			{
				return;
			}
			onFocus(this);
		}

		public void ExecuteSetUnfocused()
		{
			this.IsFocused = false;
			Action<SPItemVM> onFocus = SPItemVM.OnFocus;
			if (onFocus == null)
			{
				return;
			}
			onFocus(null);
		}

		public void UpdateCanBeSlaughtered()
		{
			InventoryLogic inventoryLogic = this._inventoryLogic;
			this.CanBeSlaughtered = inventoryLogic != null && inventoryLogic.CanSlaughterItem(this.ItemRosterElement, this.InventorySide) && !this.ItemRosterElement.EquipmentElement.IsQuestItem;
		}

		private string GetStackModifierString()
		{
			GameTexts.SetVariable("newline", "\n");
			GameTexts.SetVariable("STR1", "");
			GameTexts.SetVariable("STR2", "");
			if (!string.IsNullOrEmpty(this._entireStackShortcutKeyText))
			{
				GameTexts.SetVariable("KEY_NAME", this._entireStackShortcutKeyText);
				string text = ((this.InventorySide == InventoryLogic.InventorySide.PlayerInventory) ? GameTexts.FindText("str_entire_stack_shortcut_discard_items", null).ToString() : GameTexts.FindText("str_entire_stack_shortcut_take_items", null).ToString());
				GameTexts.SetVariable("STR1", text);
				GameTexts.SetVariable("STR2", "");
				if (this.ItemCount >= 5 && !string.IsNullOrEmpty(this._fiveStackShortcutKeyText))
				{
					GameTexts.SetVariable("KEY_NAME", this._fiveStackShortcutKeyText);
					string text2 = ((this.InventorySide == InventoryLogic.InventorySide.PlayerInventory) ? GameTexts.FindText("str_five_stack_shortcut_discard_items", null).ToString() : GameTexts.FindText("str_five_stack_shortcut_take_items", null).ToString());
					GameTexts.SetVariable("STR2", text2);
				}
			}
			return GameTexts.FindText("str_string_newline_string", null).ToString();
		}

		public void UpdateHintTexts()
		{
			base.SlaughterHint = new BasicTooltipViewModel(delegate
			{
				string stackModifierString = this.GetStackModifierString();
				GameTexts.SetVariable("STR1", GameTexts.FindText("str_inventory_slaughter", null));
				GameTexts.SetVariable("STR2", stackModifierString);
				return GameTexts.FindText("str_string_newline_string", null).ToString();
			});
			base.DonateHint = new BasicTooltipViewModel(delegate
			{
				string stackModifierString2 = this.GetStackModifierString();
				GameTexts.SetVariable("STR1", GameTexts.FindText("str_inventory_donate", null));
				GameTexts.SetVariable("STR2", stackModifierString2);
				return GameTexts.FindText("str_string_newline_string", null).ToString();
			});
			base.PreviewHint = new HintViewModel(GameTexts.FindText("str_inventory_preview", null), null);
			base.EquipHint = new HintViewModel(GameTexts.FindText("str_inventory_equip", null), null);
			base.LockHint = new HintViewModel(GameTexts.FindText("str_inventory_lock", null), null);
			if (this._usageType == InventoryMode.Loot || this._usageType == InventoryMode.Stash)
			{
				base.BuyAndEquipHint = new BasicTooltipViewModel(() => GameTexts.FindText("str_inventory_take_and_equip", null).ToString());
				base.SellHint = new BasicTooltipViewModel(delegate
				{
					string stackModifierString3 = this.GetStackModifierString();
					GameTexts.SetVariable("STR1", GameTexts.FindText("str_inventory_give", null));
					GameTexts.SetVariable("STR2", stackModifierString3);
					return GameTexts.FindText("str_string_newline_string", null).ToString();
				});
				base.BuyHint = new BasicTooltipViewModel(delegate
				{
					string stackModifierString4 = this.GetStackModifierString();
					GameTexts.SetVariable("STR1", GameTexts.FindText("str_inventory_take", null));
					GameTexts.SetVariable("STR2", stackModifierString4);
					return GameTexts.FindText("str_string_newline_string", null).ToString();
				});
				return;
			}
			if (this._usageType == InventoryMode.Default)
			{
				base.BuyAndEquipHint = new BasicTooltipViewModel(() => GameTexts.FindText("str_inventory_take_and_equip", null).ToString());
				base.SellHint = new BasicTooltipViewModel(delegate
				{
					string stackModifierString5 = this.GetStackModifierString();
					GameTexts.SetVariable("STR1", GameTexts.FindText("str_inventory_discard", null));
					GameTexts.SetVariable("STR2", stackModifierString5);
					return GameTexts.FindText("str_string_newline_string", null).ToString();
				});
				base.BuyHint = new BasicTooltipViewModel(delegate
				{
					string stackModifierString6 = this.GetStackModifierString();
					GameTexts.SetVariable("STR1", GameTexts.FindText("str_inventory_take", null));
					GameTexts.SetVariable("STR2", stackModifierString6);
					return GameTexts.FindText("str_string_newline_string", null).ToString();
				});
				return;
			}
			base.BuyAndEquipHint = new BasicTooltipViewModel(() => GameTexts.FindText("str_inventory_buy_and_equip", null).ToString());
			base.SellHint = new BasicTooltipViewModel(delegate
			{
				GameTexts.SetVariable("STR1", GameTexts.FindText("str_inventory_sell", null));
				GameTexts.SetVariable("STR2", string.Empty);
				return GameTexts.FindText("str_string_newline_string", null).ToString();
			});
			base.BuyHint = new BasicTooltipViewModel(delegate
			{
				GameTexts.SetVariable("STR1", GameTexts.FindText("str_inventory_buy", null));
				GameTexts.SetVariable("STR2", string.Empty);
				return GameTexts.FindText("str_string_newline_string", null).ToString();
			});
		}

		public static SPItemVM.ProfitTypes GetProfitTypeFromDiff(float averageValue, float currentValue)
		{
			if (averageValue == 0f)
			{
				return SPItemVM.ProfitTypes.Default;
			}
			if (averageValue < currentValue * 0.8f)
			{
				return SPItemVM.ProfitTypes.HighProfit;
			}
			if (averageValue < currentValue * 0.95f)
			{
				return SPItemVM.ProfitTypes.Profit;
			}
			if (averageValue > currentValue * 1.05f)
			{
				return SPItemVM.ProfitTypes.Loss;
			}
			if (averageValue > currentValue * 1.2f)
			{
				return SPItemVM.ProfitTypes.HighLoss;
			}
			return SPItemVM.ProfitTypes.Default;
		}

		[DataSourceProperty]
		public bool IsFocused
		{
			get
			{
				return this._isFocused;
			}
			set
			{
				if (value != this._isFocused)
				{
					this._isFocused = value;
					base.OnPropertyChangedWithValue(value, "IsFocused");
				}
			}
		}

		[DataSourceProperty]
		public bool IsArtifact
		{
			get
			{
				return this._isArtifact;
			}
			set
			{
				if (value != this._isArtifact)
				{
					this._isArtifact = value;
					base.OnPropertyChangedWithValue(value, "IsArtifact");
				}
			}
		}

		[DataSourceProperty]
		public bool IsTransferable
		{
			get
			{
				return this._isTransferable;
			}
			set
			{
				if (value != this._isTransferable)
				{
					this._isTransferable = value;
					base.OnPropertyChangedWithValue(value, "IsTransferable");
				}
			}
		}

		[DataSourceProperty]
		public bool IsTransferButtonHighlighted
		{
			get
			{
				return this._isTransferButtonHighlighted;
			}
			set
			{
				if (value != this._isTransferButtonHighlighted)
				{
					this._isTransferButtonHighlighted = value;
					base.OnPropertyChangedWithValue(value, "IsTransferButtonHighlighted");
				}
			}
		}

		[DataSourceProperty]
		public bool IsItemHighlightEnabled
		{
			get
			{
				return this._isItemHighlightEnabled;
			}
			set
			{
				if (value != this._isItemHighlightEnabled)
				{
					this._isItemHighlightEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsItemHighlightEnabled");
				}
			}
		}

		[DataSourceProperty]
		public bool IsCivilianItem
		{
			get
			{
				return this._isCivilianItem;
			}
			set
			{
				if (value != this._isCivilianItem)
				{
					this._isCivilianItem = value;
					base.OnPropertyChangedWithValue(value, "IsCivilianItem");
				}
			}
		}

		[DataSourceProperty]
		public bool IsNew
		{
			get
			{
				return this._isNew;
			}
			set
			{
				if (value != this._isNew)
				{
					this._isNew = value;
					base.OnPropertyChangedWithValue(value, "IsNew");
				}
			}
		}

		[DataSourceProperty]
		public bool IsGenderDifferent
		{
			get
			{
				return this._isGenderDifferent;
			}
			set
			{
				if (value != this._isGenderDifferent)
				{
					this._isGenderDifferent = value;
					base.OnPropertyChangedWithValue(value, "IsGenderDifferent");
				}
			}
		}

		[DataSourceProperty]
		public bool CanBeSlaughtered
		{
			get
			{
				return this._canBeSlaughtered;
			}
			set
			{
				if (value != this._canBeSlaughtered)
				{
					this._canBeSlaughtered = value;
					base.OnPropertyChangedWithValue(value, "CanBeSlaughtered");
				}
			}
		}

		[DataSourceProperty]
		public bool CanBeDonated
		{
			get
			{
				return this._canBeDonated;
			}
			set
			{
				if (value != this._canBeDonated)
				{
					this._canBeDonated = value;
					base.OnPropertyChangedWithValue(value, "CanBeDonated");
				}
			}
		}

		[DataSourceProperty]
		public bool IsEquipableItem
		{
			get
			{
				return this._isEquipableItem;
			}
			set
			{
				if (value != this._isEquipableItem)
				{
					this._isEquipableItem = value;
					base.OnPropertyChangedWithValue(value, "IsEquipableItem");
				}
			}
		}

		[DataSourceProperty]
		public bool CanCharacterUseItem
		{
			get
			{
				return this._canCharacterUseItem;
			}
			set
			{
				if (value != this._canCharacterUseItem)
				{
					this._canCharacterUseItem = value;
					base.OnPropertyChangedWithValue(value, "CanCharacterUseItem");
				}
			}
		}

		[DataSourceProperty]
		public bool IsLocked
		{
			get
			{
				return this._isLocked;
			}
			set
			{
				if (value != this._isLocked)
				{
					this._isLocked = value;
					base.OnPropertyChangedWithValue(value, "IsLocked");
					SPItemVM.ProcessLockItem(this, value);
				}
			}
		}

		[DataSourceProperty]
		public int ItemCount
		{
			get
			{
				return this._count;
			}
			set
			{
				if (value != this._count)
				{
					this._count = value;
					base.OnPropertyChangedWithValue(value, "ItemCount");
					this.UpdateTotalCost();
					this.UpdateTradeData(false);
				}
			}
		}

		[DataSourceProperty]
		public int ItemLevel
		{
			get
			{
				return this._level;
			}
			set
			{
				if (value != this._level)
				{
					this._level = value;
					base.OnPropertyChangedWithValue(value, "ItemLevel");
				}
			}
		}

		[DataSourceProperty]
		public int ProfitType
		{
			get
			{
				return this._profitType;
			}
			set
			{
				if (value != this._profitType)
				{
					this._profitType = value;
					base.OnPropertyChangedWithValue(value, "ProfitType");
				}
			}
		}

		[DataSourceProperty]
		public int TransactionCount
		{
			get
			{
				return this._transactionCount;
			}
			set
			{
				if (value != this._transactionCount)
				{
					this._transactionCount = value;
					base.OnPropertyChangedWithValue(value, "TransactionCount");
					this.UpdateTotalCost();
				}
			}
		}

		[DataSourceProperty]
		public int TotalCost
		{
			get
			{
				return this._totalCost;
			}
			set
			{
				if (value != this._totalCost)
				{
					this._totalCost = value;
					base.OnPropertyChangedWithValue(value, "TotalCost");
				}
			}
		}

		[DataSourceProperty]
		public InventoryTradeVM TradeData
		{
			get
			{
				return this._tradeData;
			}
			set
			{
				if (value != this._tradeData)
				{
					this._tradeData = value;
					base.OnPropertyChangedWithValue<InventoryTradeVM>(value, "TradeData");
				}
			}
		}

		public static Action<SPItemVM> OnFocus;

		public static Action<SPItemVM, bool> ProcessSellItem;

		public static Action<SPItemVM> ProcessItemSlaughter;

		public static Action<SPItemVM> ProcessItemDonate;

		public static Action<SPItemVM, bool> ProcessLockItem;

		private readonly string _fiveStackShortcutKeyText;

		private readonly string _entireStackShortcutKeyText;

		private readonly InventoryMode _usageType;

		private Concept _tradeGoodConceptObj;

		private Concept _itemConceptObj;

		private InventoryLogic _inventoryLogic;

		private bool _isFocused;

		private int _level;

		private bool _isTransferable;

		private bool _isCivilianItem;

		private bool _isGenderDifferent;

		private bool _isEquipableItem;

		private bool _canCharacterUseItem;

		private bool _isLocked;

		private bool _isArtifact;

		private bool _canBeSlaughtered;

		private bool _canBeDonated;

		private int _count;

		private int _profitType = -5;

		private int _transactionCount;

		private int _totalCost;

		private bool _isTransferButtonHighlighted;

		private bool _isItemHighlightEnabled;

		private bool _isNew;

		private InventoryTradeVM _tradeData;

		public enum ProfitTypes
		{
			HighLoss = -2,
			Loss,
			Default,
			Profit,
			HighProfit
		}
	}
}
