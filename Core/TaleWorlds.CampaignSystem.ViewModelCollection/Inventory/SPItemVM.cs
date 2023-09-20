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
	// Token: 0x02000083 RID: 131
	public class SPItemVM : ItemVM
	{
		// Token: 0x1700042B RID: 1067
		// (get) Token: 0x06000CCF RID: 3279 RVA: 0x000341C7 File Offset: 0x000323C7
		// (set) Token: 0x06000CD0 RID: 3280 RVA: 0x000341CF File Offset: 0x000323CF
		public InventoryLogic.InventorySide InventorySide { get; private set; }

		// Token: 0x06000CD1 RID: 3281 RVA: 0x000341D8 File Offset: 0x000323D8
		public SPItemVM()
		{
			base.StringId = "";
			base.ImageIdentifier = new ImageIdentifierVM(ImageIdentifierType.Null);
			this._itemType = EquipmentIndex.None;
		}

		// Token: 0x06000CD2 RID: 3282 RVA: 0x00034208 File Offset: 0x00032408
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

		// Token: 0x06000CD3 RID: 3283 RVA: 0x000344E4 File Offset: 0x000326E4
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

		// Token: 0x06000CD4 RID: 3284 RVA: 0x00034548 File Offset: 0x00032748
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

		// Token: 0x06000CD5 RID: 3285 RVA: 0x0003467C File Offset: 0x0003287C
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

		// Token: 0x06000CD6 RID: 3286 RVA: 0x00034728 File Offset: 0x00032928
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

		// Token: 0x06000CD7 RID: 3287 RVA: 0x000348B0 File Offset: 0x00032AB0
		public void ExecuteBuySingle()
		{
			this.TransactionCount = 1;
			ItemVM.ProcessBuyItem(this, false);
		}

		// Token: 0x06000CD8 RID: 3288 RVA: 0x000348C5 File Offset: 0x00032AC5
		public void ExecuteSellSingle()
		{
			this.TransactionCount = 1;
			SPItemVM.ProcessSellItem(this, false);
		}

		// Token: 0x06000CD9 RID: 3289 RVA: 0x000348DA File Offset: 0x00032ADA
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

		// Token: 0x06000CDA RID: 3290 RVA: 0x000348FF File Offset: 0x00032AFF
		public void ExecuteSellItem()
		{
			SPItemVM.ProcessSellItem(this, false);
		}

		// Token: 0x06000CDB RID: 3291 RVA: 0x00034910 File Offset: 0x00032B10
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

		// Token: 0x06000CDC RID: 3292 RVA: 0x00034987 File Offset: 0x00032B87
		public void ExecuteResetTrade()
		{
			this.TradeData.ExecuteReset();
		}

		// Token: 0x06000CDD RID: 3293 RVA: 0x00034994 File Offset: 0x00032B94
		private void UpdateTotalCost()
		{
			if (this.TransactionCount <= 0 || this._inventoryLogic == null || this.InventorySide == InventoryLogic.InventorySide.Equipment)
			{
				return;
			}
			int num;
			this.TotalCost = this._inventoryLogic.GetItemTotalPrice(this.ItemRosterElement, this.TransactionCount, out num, this.InventorySide == InventoryLogic.InventorySide.OtherInventory);
		}

		// Token: 0x06000CDE RID: 3294 RVA: 0x000349E4 File Offset: 0x00032BE4
		public void UpdateTradeData(bool forceUpdateAmounts)
		{
			InventoryTradeVM tradeData = this.TradeData;
			if (tradeData != null)
			{
				tradeData.UpdateItemData(this.ItemRosterElement, this.InventorySide, forceUpdateAmounts);
			}
			this.UpdateProfitType();
		}

		// Token: 0x06000CDF RID: 3295 RVA: 0x00034A0A File Offset: 0x00032C0A
		public void ExecuteSlaughterItem()
		{
			if (this.CanBeSlaughtered)
			{
				SPItemVM.ProcessItemSlaughter(this);
			}
		}

		// Token: 0x06000CE0 RID: 3296 RVA: 0x00034A1F File Offset: 0x00032C1F
		public void ExecuteDonateItem()
		{
			if (this.CanBeDonated)
			{
				SPItemVM.ProcessItemDonate(this);
			}
		}

		// Token: 0x06000CE1 RID: 3297 RVA: 0x00034A34 File Offset: 0x00032C34
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

		// Token: 0x06000CE2 RID: 3298 RVA: 0x00034A4D File Offset: 0x00032C4D
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

		// Token: 0x06000CE3 RID: 3299 RVA: 0x00034A68 File Offset: 0x00032C68
		public void UpdateCanBeSlaughtered()
		{
			InventoryLogic inventoryLogic = this._inventoryLogic;
			this.CanBeSlaughtered = inventoryLogic != null && inventoryLogic.CanSlaughterItem(this.ItemRosterElement, this.InventorySide) && !this.ItemRosterElement.EquipmentElement.IsQuestItem;
		}

		// Token: 0x06000CE4 RID: 3300 RVA: 0x00034AB4 File Offset: 0x00032CB4
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

		// Token: 0x06000CE5 RID: 3301 RVA: 0x00034BC8 File Offset: 0x00032DC8
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

		// Token: 0x06000CE6 RID: 3302 RVA: 0x00034D92 File Offset: 0x00032F92
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

		// Token: 0x1700042C RID: 1068
		// (get) Token: 0x06000CE7 RID: 3303 RVA: 0x00034DD0 File Offset: 0x00032FD0
		// (set) Token: 0x06000CE8 RID: 3304 RVA: 0x00034DD8 File Offset: 0x00032FD8
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

		// Token: 0x1700042D RID: 1069
		// (get) Token: 0x06000CE9 RID: 3305 RVA: 0x00034DF6 File Offset: 0x00032FF6
		// (set) Token: 0x06000CEA RID: 3306 RVA: 0x00034DFE File Offset: 0x00032FFE
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

		// Token: 0x1700042E RID: 1070
		// (get) Token: 0x06000CEB RID: 3307 RVA: 0x00034E1C File Offset: 0x0003301C
		// (set) Token: 0x06000CEC RID: 3308 RVA: 0x00034E24 File Offset: 0x00033024
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

		// Token: 0x1700042F RID: 1071
		// (get) Token: 0x06000CED RID: 3309 RVA: 0x00034E42 File Offset: 0x00033042
		// (set) Token: 0x06000CEE RID: 3310 RVA: 0x00034E4A File Offset: 0x0003304A
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

		// Token: 0x17000430 RID: 1072
		// (get) Token: 0x06000CEF RID: 3311 RVA: 0x00034E68 File Offset: 0x00033068
		// (set) Token: 0x06000CF0 RID: 3312 RVA: 0x00034E70 File Offset: 0x00033070
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

		// Token: 0x17000431 RID: 1073
		// (get) Token: 0x06000CF1 RID: 3313 RVA: 0x00034E8E File Offset: 0x0003308E
		// (set) Token: 0x06000CF2 RID: 3314 RVA: 0x00034E96 File Offset: 0x00033096
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

		// Token: 0x17000432 RID: 1074
		// (get) Token: 0x06000CF3 RID: 3315 RVA: 0x00034EB4 File Offset: 0x000330B4
		// (set) Token: 0x06000CF4 RID: 3316 RVA: 0x00034EBC File Offset: 0x000330BC
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

		// Token: 0x17000433 RID: 1075
		// (get) Token: 0x06000CF5 RID: 3317 RVA: 0x00034EDA File Offset: 0x000330DA
		// (set) Token: 0x06000CF6 RID: 3318 RVA: 0x00034EE2 File Offset: 0x000330E2
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

		// Token: 0x17000434 RID: 1076
		// (get) Token: 0x06000CF7 RID: 3319 RVA: 0x00034F00 File Offset: 0x00033100
		// (set) Token: 0x06000CF8 RID: 3320 RVA: 0x00034F08 File Offset: 0x00033108
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

		// Token: 0x17000435 RID: 1077
		// (get) Token: 0x06000CF9 RID: 3321 RVA: 0x00034F26 File Offset: 0x00033126
		// (set) Token: 0x06000CFA RID: 3322 RVA: 0x00034F2E File Offset: 0x0003312E
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

		// Token: 0x17000436 RID: 1078
		// (get) Token: 0x06000CFB RID: 3323 RVA: 0x00034F4C File Offset: 0x0003314C
		// (set) Token: 0x06000CFC RID: 3324 RVA: 0x00034F54 File Offset: 0x00033154
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

		// Token: 0x17000437 RID: 1079
		// (get) Token: 0x06000CFD RID: 3325 RVA: 0x00034F72 File Offset: 0x00033172
		// (set) Token: 0x06000CFE RID: 3326 RVA: 0x00034F7A File Offset: 0x0003317A
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

		// Token: 0x17000438 RID: 1080
		// (get) Token: 0x06000CFF RID: 3327 RVA: 0x00034F98 File Offset: 0x00033198
		// (set) Token: 0x06000D00 RID: 3328 RVA: 0x00034FA0 File Offset: 0x000331A0
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

		// Token: 0x17000439 RID: 1081
		// (get) Token: 0x06000D01 RID: 3329 RVA: 0x00034FCA File Offset: 0x000331CA
		// (set) Token: 0x06000D02 RID: 3330 RVA: 0x00034FD2 File Offset: 0x000331D2
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

		// Token: 0x1700043A RID: 1082
		// (get) Token: 0x06000D03 RID: 3331 RVA: 0x00034FFD File Offset: 0x000331FD
		// (set) Token: 0x06000D04 RID: 3332 RVA: 0x00035005 File Offset: 0x00033205
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

		// Token: 0x1700043B RID: 1083
		// (get) Token: 0x06000D05 RID: 3333 RVA: 0x00035023 File Offset: 0x00033223
		// (set) Token: 0x06000D06 RID: 3334 RVA: 0x0003502B File Offset: 0x0003322B
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

		// Token: 0x1700043C RID: 1084
		// (get) Token: 0x06000D07 RID: 3335 RVA: 0x00035049 File Offset: 0x00033249
		// (set) Token: 0x06000D08 RID: 3336 RVA: 0x00035051 File Offset: 0x00033251
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

		// Token: 0x1700043D RID: 1085
		// (get) Token: 0x06000D09 RID: 3337 RVA: 0x00035075 File Offset: 0x00033275
		// (set) Token: 0x06000D0A RID: 3338 RVA: 0x0003507D File Offset: 0x0003327D
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

		// Token: 0x1700043E RID: 1086
		// (get) Token: 0x06000D0B RID: 3339 RVA: 0x0003509B File Offset: 0x0003329B
		// (set) Token: 0x06000D0C RID: 3340 RVA: 0x000350A3 File Offset: 0x000332A3
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

		// Token: 0x040005E3 RID: 1507
		public static Action<SPItemVM> OnFocus;

		// Token: 0x040005E4 RID: 1508
		public static Action<SPItemVM, bool> ProcessSellItem;

		// Token: 0x040005E5 RID: 1509
		public static Action<SPItemVM> ProcessItemSlaughter;

		// Token: 0x040005E6 RID: 1510
		public static Action<SPItemVM> ProcessItemDonate;

		// Token: 0x040005E7 RID: 1511
		public static Action<SPItemVM, bool> ProcessLockItem;

		// Token: 0x040005E8 RID: 1512
		private readonly string _fiveStackShortcutKeyText;

		// Token: 0x040005E9 RID: 1513
		private readonly string _entireStackShortcutKeyText;

		// Token: 0x040005EA RID: 1514
		private readonly InventoryMode _usageType;

		// Token: 0x040005EB RID: 1515
		private Concept _tradeGoodConceptObj;

		// Token: 0x040005EC RID: 1516
		private Concept _itemConceptObj;

		// Token: 0x040005EE RID: 1518
		private InventoryLogic _inventoryLogic;

		// Token: 0x040005EF RID: 1519
		private bool _isFocused;

		// Token: 0x040005F0 RID: 1520
		private int _level;

		// Token: 0x040005F1 RID: 1521
		private bool _isTransferable;

		// Token: 0x040005F2 RID: 1522
		private bool _isCivilianItem;

		// Token: 0x040005F3 RID: 1523
		private bool _isGenderDifferent;

		// Token: 0x040005F4 RID: 1524
		private bool _isEquipableItem;

		// Token: 0x040005F5 RID: 1525
		private bool _canCharacterUseItem;

		// Token: 0x040005F6 RID: 1526
		private bool _isLocked;

		// Token: 0x040005F7 RID: 1527
		private bool _isArtifact;

		// Token: 0x040005F8 RID: 1528
		private bool _canBeSlaughtered;

		// Token: 0x040005F9 RID: 1529
		private bool _canBeDonated;

		// Token: 0x040005FA RID: 1530
		private int _count;

		// Token: 0x040005FB RID: 1531
		private int _profitType = -5;

		// Token: 0x040005FC RID: 1532
		private int _transactionCount;

		// Token: 0x040005FD RID: 1533
		private int _totalCost;

		// Token: 0x040005FE RID: 1534
		private bool _isTransferButtonHighlighted;

		// Token: 0x040005FF RID: 1535
		private bool _isItemHighlightEnabled;

		// Token: 0x04000600 RID: 1536
		private bool _isNew;

		// Token: 0x04000601 RID: 1537
		private InventoryTradeVM _tradeData;

		// Token: 0x020001C5 RID: 453
		public enum ProfitTypes
		{
			// Token: 0x04000F99 RID: 3993
			HighLoss = -2,
			// Token: 0x04000F9A RID: 3994
			Loss,
			// Token: 0x04000F9B RID: 3995
			Default,
			// Token: 0x04000F9C RID: 3996
			Profit,
			// Token: 0x04000F9D RID: 3997
			HighProfit
		}
	}
}
