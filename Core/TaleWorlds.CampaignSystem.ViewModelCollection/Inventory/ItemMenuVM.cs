using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Inventory;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Core.ViewModelCollection.Generic;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Inventory
{
	// Token: 0x0200007B RID: 123
	public class ItemMenuVM : ViewModel
	{
		// Token: 0x06000B01 RID: 2817 RVA: 0x0002AEBC File Offset: 0x000290BC
		public ItemMenuVM(Action<ItemVM, int> resetComparedItems, InventoryLogic inventoryLogic, Func<WeaponComponentData, ItemObject.ItemUsageSetFlags> getItemUsageSetFlags, Func<EquipmentIndex, SPItemVM> getEquipmentAtIndex)
		{
			this._resetComparedItems = resetComparedItems;
			this._inventoryLogic = inventoryLogic;
			this._comparedItemProperties = new MBBindingList<ItemMenuTooltipPropertyVM>();
			this._targetItemProperties = new MBBindingList<ItemMenuTooltipPropertyVM>();
			this._getItemUsageSetFlags = getItemUsageSetFlags;
			this._getEquipmentAtIndex = getEquipmentAtIndex;
			this.TargetItemFlagList = new MBBindingList<ItemFlagVM>();
			this.ComparedItemFlagList = new MBBindingList<ItemFlagVM>();
			this.AlternativeUsages = new MBBindingList<StringItemWithHintVM>();
			this._tradeRumorsBehavior = Campaign.Current.GetCampaignBehavior<ITradeRumorCampaignBehavior>();
		}

		// Token: 0x17000395 RID: 917
		// (get) Token: 0x06000B02 RID: 2818 RVA: 0x0002B1A7 File Offset: 0x000293A7
		// (set) Token: 0x06000B03 RID: 2819 RVA: 0x0002B1AF File Offset: 0x000293AF
		[DataSourceProperty]
		public bool IsComparing
		{
			get
			{
				return this._isComparing;
			}
			set
			{
				if (value != this._isComparing)
				{
					this._isComparing = value;
					base.OnPropertyChangedWithValue(value, "IsComparing");
				}
			}
		}

		// Token: 0x17000396 RID: 918
		// (get) Token: 0x06000B04 RID: 2820 RVA: 0x0002B1CD File Offset: 0x000293CD
		// (set) Token: 0x06000B05 RID: 2821 RVA: 0x0002B1D5 File Offset: 0x000293D5
		[DataSourceProperty]
		public bool IsPlayerItem
		{
			get
			{
				return this._isPlayerItem;
			}
			set
			{
				if (value != this._isPlayerItem)
				{
					this._isPlayerItem = value;
					base.OnPropertyChangedWithValue(value, "IsPlayerItem");
				}
			}
		}

		// Token: 0x17000397 RID: 919
		// (get) Token: 0x06000B06 RID: 2822 RVA: 0x0002B1F3 File Offset: 0x000293F3
		// (set) Token: 0x06000B07 RID: 2823 RVA: 0x0002B1FB File Offset: 0x000293FB
		[DataSourceProperty]
		public ImageIdentifierVM ImageIdentifier
		{
			get
			{
				return this._imageIdentifier;
			}
			set
			{
				if (value != this._imageIdentifier)
				{
					this._imageIdentifier = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "ImageIdentifier");
				}
			}
		}

		// Token: 0x17000398 RID: 920
		// (get) Token: 0x06000B08 RID: 2824 RVA: 0x0002B219 File Offset: 0x00029419
		// (set) Token: 0x06000B09 RID: 2825 RVA: 0x0002B221 File Offset: 0x00029421
		[DataSourceProperty]
		public ImageIdentifierVM ComparedImageIdentifier
		{
			get
			{
				return this._comparedImageIdentifier;
			}
			set
			{
				if (value != this._comparedImageIdentifier)
				{
					this._comparedImageIdentifier = value;
					base.OnPropertyChangedWithValue<ImageIdentifierVM>(value, "ComparedImageIdentifier");
				}
			}
		}

		// Token: 0x17000399 RID: 921
		// (get) Token: 0x06000B0A RID: 2826 RVA: 0x0002B23F File Offset: 0x0002943F
		// (set) Token: 0x06000B0B RID: 2827 RVA: 0x0002B247 File Offset: 0x00029447
		[DataSourceProperty]
		public int TransactionTotalCost
		{
			get
			{
				return this._transactionTotalCost;
			}
			set
			{
				if (value != this._transactionTotalCost)
				{
					this._transactionTotalCost = value;
					base.OnPropertyChangedWithValue(value, "TransactionTotalCost");
				}
			}
		}

		// Token: 0x1700039A RID: 922
		// (get) Token: 0x06000B0C RID: 2828 RVA: 0x0002B265 File Offset: 0x00029465
		// (set) Token: 0x06000B0D RID: 2829 RVA: 0x0002B26D File Offset: 0x0002946D
		[DataSourceProperty]
		public bool IsInitializationOver
		{
			get
			{
				return this._isInitializationOver;
			}
			set
			{
				if (value != this._isInitializationOver)
				{
					this._isInitializationOver = value;
					base.OnPropertyChangedWithValue(value, "IsInitializationOver");
				}
			}
		}

		// Token: 0x1700039B RID: 923
		// (get) Token: 0x06000B0E RID: 2830 RVA: 0x0002B28B File Offset: 0x0002948B
		// (set) Token: 0x06000B0F RID: 2831 RVA: 0x0002B293 File Offset: 0x00029493
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
					base.OnPropertyChangedWithValue<string>(value, "ItemName");
				}
			}
		}

		// Token: 0x1700039C RID: 924
		// (get) Token: 0x06000B10 RID: 2832 RVA: 0x0002B2B6 File Offset: 0x000294B6
		// (set) Token: 0x06000B11 RID: 2833 RVA: 0x0002B2BE File Offset: 0x000294BE
		[DataSourceProperty]
		public string ComparedItemName
		{
			get
			{
				return this._comparedItemName;
			}
			set
			{
				if (value != this._comparedItemName)
				{
					this._comparedItemName = value;
					base.OnPropertyChangedWithValue<string>(value, "ComparedItemName");
				}
			}
		}

		// Token: 0x1700039D RID: 925
		// (get) Token: 0x06000B12 RID: 2834 RVA: 0x0002B2E1 File Offset: 0x000294E1
		// (set) Token: 0x06000B13 RID: 2835 RVA: 0x0002B2E9 File Offset: 0x000294E9
		[DataSourceProperty]
		public MBBindingList<ItemMenuTooltipPropertyVM> TargetItemProperties
		{
			get
			{
				return this._targetItemProperties;
			}
			set
			{
				if (value != this._targetItemProperties)
				{
					this._targetItemProperties = value;
					base.OnPropertyChangedWithValue<MBBindingList<ItemMenuTooltipPropertyVM>>(value, "TargetItemProperties");
				}
			}
		}

		// Token: 0x1700039E RID: 926
		// (get) Token: 0x06000B14 RID: 2836 RVA: 0x0002B307 File Offset: 0x00029507
		// (set) Token: 0x06000B15 RID: 2837 RVA: 0x0002B30F File Offset: 0x0002950F
		[DataSourceProperty]
		public MBBindingList<ItemMenuTooltipPropertyVM> ComparedItemProperties
		{
			get
			{
				return this._comparedItemProperties;
			}
			set
			{
				if (value != this._comparedItemProperties)
				{
					this._comparedItemProperties = value;
					base.OnPropertyChangedWithValue<MBBindingList<ItemMenuTooltipPropertyVM>>(value, "ComparedItemProperties");
				}
			}
		}

		// Token: 0x1700039F RID: 927
		// (get) Token: 0x06000B16 RID: 2838 RVA: 0x0002B32D File Offset: 0x0002952D
		// (set) Token: 0x06000B17 RID: 2839 RVA: 0x0002B335 File Offset: 0x00029535
		[DataSourceProperty]
		public MBBindingList<ItemFlagVM> TargetItemFlagList
		{
			get
			{
				return this._targetItemFlagList;
			}
			set
			{
				if (value != this._targetItemFlagList)
				{
					this._targetItemFlagList = value;
					base.OnPropertyChangedWithValue<MBBindingList<ItemFlagVM>>(value, "TargetItemFlagList");
				}
			}
		}

		// Token: 0x170003A0 RID: 928
		// (get) Token: 0x06000B18 RID: 2840 RVA: 0x0002B353 File Offset: 0x00029553
		// (set) Token: 0x06000B19 RID: 2841 RVA: 0x0002B35B File Offset: 0x0002955B
		[DataSourceProperty]
		public MBBindingList<ItemFlagVM> ComparedItemFlagList
		{
			get
			{
				return this._comparedItemFlagList;
			}
			set
			{
				if (value != this._comparedItemFlagList)
				{
					this._comparedItemFlagList = value;
					base.OnPropertyChangedWithValue<MBBindingList<ItemFlagVM>>(value, "ComparedItemFlagList");
				}
			}
		}

		// Token: 0x170003A1 RID: 929
		// (get) Token: 0x06000B1A RID: 2842 RVA: 0x0002B379 File Offset: 0x00029579
		// (set) Token: 0x06000B1B RID: 2843 RVA: 0x0002B381 File Offset: 0x00029581
		[DataSourceProperty]
		public int AlternativeUsageIndex
		{
			get
			{
				return this._alternativeUsageIndex;
			}
			set
			{
				if (value != this._alternativeUsageIndex)
				{
					this._alternativeUsageIndex = value;
					base.OnPropertyChangedWithValue(value, "AlternativeUsageIndex");
					this.AlternativeUsageIndexUpdated();
				}
			}
		}

		// Token: 0x170003A2 RID: 930
		// (get) Token: 0x06000B1C RID: 2844 RVA: 0x0002B3A5 File Offset: 0x000295A5
		// (set) Token: 0x06000B1D RID: 2845 RVA: 0x0002B3AD File Offset: 0x000295AD
		[DataSourceProperty]
		public MBBindingList<StringItemWithHintVM> AlternativeUsages
		{
			get
			{
				return this._alternativeUsages;
			}
			set
			{
				if (value != this._alternativeUsages)
				{
					this._alternativeUsages = value;
					base.OnPropertyChangedWithValue<MBBindingList<StringItemWithHintVM>>(value, "AlternativeUsages");
				}
			}
		}

		// Token: 0x06000B1E RID: 2846 RVA: 0x0002B3CC File Offset: 0x000295CC
		public void SetItem(SPItemVM item, ItemVM comparedItem = null, BasicCharacterObject character = null, int alternativeUsageIndex = 0)
		{
			this.IsInitializationOver = false;
			this._targetItem = item;
			this._comparedItem = comparedItem;
			ItemVM comparedItem2 = this._comparedItem;
			object obj;
			EquipmentElement equipmentElement;
			if (comparedItem2 == null)
			{
				obj = null;
			}
			else
			{
				equipmentElement = comparedItem2.ItemRosterElement.EquipmentElement;
				obj = equipmentElement.Item;
			}
			this.IsComparing = obj != null;
			this.IsPlayerItem = item.InventorySide == InventoryLogic.InventorySide.PlayerInventory;
			this._character = character;
			this.ImageIdentifier = item.ImageIdentifier;
			this.ComparedImageIdentifier = ((comparedItem != null) ? comparedItem.ImageIdentifier : null);
			this.ItemName = item.ItemDescription;
			string text;
			if (comparedItem == null)
			{
				text = null;
			}
			else
			{
				equipmentElement = comparedItem.ItemRosterElement.EquipmentElement;
				text = equipmentElement.GetModifiedItemName().ToString();
			}
			this.ComparedItemName = text;
			this.TargetItemProperties.Clear();
			this.ComparedItemProperties.Clear();
			this.TargetItemFlagList.Clear();
			this.ComparedItemFlagList.Clear();
			this.AlternativeUsages.Clear();
			this.SetGeneralComponentTooltip(true);
			Town town = this._inventoryLogic.CurrentSettlementComponent as Town;
			if (town != null && Game.Current.IsDevelopmentMode)
			{
				MBBindingList<ItemMenuTooltipPropertyVM> targetItemProperties = this.TargetItemProperties;
				string text2 = "Category:";
				equipmentElement = item.ItemRosterElement.EquipmentElement;
				this.CreateProperty(targetItemProperties, text2, equipmentElement.Item.ItemCategory.GetName().ToString(), 0, null);
				MBBindingList<ItemMenuTooltipPropertyVM> targetItemProperties2 = this.TargetItemProperties;
				string text3 = "Supply:";
				TownMarketData marketData = town.MarketData;
				equipmentElement = item.ItemRosterElement.EquipmentElement;
				this.CreateProperty(targetItemProperties2, text3, marketData.GetSupply(equipmentElement.Item.ItemCategory).ToString(), 0, null);
				MBBindingList<ItemMenuTooltipPropertyVM> targetItemProperties3 = this.TargetItemProperties;
				string text4 = "Demand:";
				TownMarketData marketData2 = town.MarketData;
				equipmentElement = item.ItemRosterElement.EquipmentElement;
				this.CreateProperty(targetItemProperties3, text4, marketData2.GetDemand(equipmentElement.Item.ItemCategory).ToString(), 0, null);
				MBBindingList<ItemMenuTooltipPropertyVM> targetItemProperties4 = this.TargetItemProperties;
				string text5 = "Price Index:";
				TownMarketData marketData3 = town.MarketData;
				equipmentElement = item.ItemRosterElement.EquipmentElement;
				this.CreateProperty(targetItemProperties4, text5, marketData3.GetPriceFactor(equipmentElement.Item.ItemCategory).ToString(), 0, null);
			}
			equipmentElement = item.ItemRosterElement.EquipmentElement;
			if (equipmentElement.Item.HasArmorComponent)
			{
				this.SetArmorComponentTooltip();
			}
			else
			{
				equipmentElement = item.ItemRosterElement.EquipmentElement;
				if (equipmentElement.Item.WeaponComponent != null)
				{
					equipmentElement = this._targetItem.ItemRosterElement.EquipmentElement;
					this.SetWeaponComponentTooltip(equipmentElement, alternativeUsageIndex, EquipmentElement.Invalid, -1, true);
				}
				else
				{
					equipmentElement = item.ItemRosterElement.EquipmentElement;
					if (equipmentElement.Item.HasHorseComponent)
					{
						this.SetHorseComponentTooltip();
					}
					else
					{
						equipmentElement = item.ItemRosterElement.EquipmentElement;
						if (equipmentElement.Item.IsFood)
						{
							this.SetFoodTooltip();
						}
					}
				}
			}
			equipmentElement = item.ItemRosterElement.EquipmentElement;
			if (InventoryManager.GetInventoryItemTypeOfItem(equipmentElement.Item) == InventoryItemType.Goods)
			{
				this.SetMerchandiseComponentTooltip();
			}
			if (this.IsComparing && !Input.IsGamepadActive)
			{
				for (EquipmentIndex equipmentIndex = this._comparedItem.ItemType + 1; equipmentIndex != this._comparedItem.ItemType; equipmentIndex = (equipmentIndex + 1) % EquipmentIndex.NumEquipmentSetSlots)
				{
					SPItemVM spitemVM = this._getEquipmentAtIndex(equipmentIndex);
					if (spitemVM != null)
					{
						equipmentElement = spitemVM.ItemRosterElement.EquipmentElement;
						ItemObject item2 = equipmentElement.Item;
						equipmentElement = comparedItem.ItemRosterElement.EquipmentElement;
						if (ItemHelper.CheckComparability(item2, equipmentElement.Item))
						{
							TextObject textObject = new TextObject("{=8fqFGxD9}Press {KEY} to compare with: {ITEM}", null);
							textObject.SetTextVariable("KEY", GameTexts.FindText("str_game_key_text", "anyalt"));
							textObject.SetTextVariable("ITEM", spitemVM.ItemDescription);
							this.CreateProperty(this.TargetItemProperties, "", textObject.ToString(), 0, null);
							this.CreateProperty(this.ComparedItemProperties, "", "", 0, null);
							break;
						}
					}
				}
			}
			this.IsInitializationOver = true;
			Game game = Game.Current;
			if (game == null)
			{
				return;
			}
			game.EventManager.TriggerEvent<InventoryItemInspectedEvent>(new InventoryItemInspectedEvent(item.ItemRosterElement, item.InventorySide));
		}

		// Token: 0x06000B1F RID: 2847 RVA: 0x0002B7C0 File Offset: 0x000299C0
		private int CompareValues(float currentValue, float comparedValue)
		{
			int num = (int)(currentValue * 10f);
			int num2 = (int)(comparedValue * 10f);
			if ((num != 0 && (float)MathF.Abs(num) <= MathF.Abs(currentValue)) || (num2 != 0 && (float)MathF.Abs(num2) <= MathF.Abs(comparedValue)))
			{
				return 0;
			}
			return this.CompareValues(num, num2);
		}

		// Token: 0x06000B20 RID: 2848 RVA: 0x0002B815 File Offset: 0x00029A15
		private int CompareValues(int currentValue, int comparedValue)
		{
			if (this._comparedItem == null || currentValue == comparedValue)
			{
				return 0;
			}
			if (currentValue <= comparedValue)
			{
				return -1;
			}
			return 1;
		}

		// Token: 0x06000B21 RID: 2849 RVA: 0x0002B82C File Offset: 0x00029A2C
		private void AlternativeUsageIndexUpdated()
		{
			if (this.AlternativeUsageIndex < 0 || !this.IsInitializationOver)
			{
				return;
			}
			EquipmentElement equipmentElement = this._targetItem.ItemRosterElement.EquipmentElement;
			if (equipmentElement.Item.WeaponComponent != null)
			{
				equipmentElement = this._targetItem.ItemRosterElement.EquipmentElement;
				WeaponComponentData weaponComponentData = equipmentElement.Item.Weapons[this.AlternativeUsageIndex];
				EquipmentElement equipmentElement2;
				int num;
				this.GetComparedWeapon(weaponComponentData.WeaponDescriptionId, out equipmentElement2, out num);
				if (!equipmentElement2.IsEmpty)
				{
					this.TargetItemProperties.Clear();
					this.ComparedItemProperties.Clear();
					this.SetGeneralComponentTooltip(false);
					equipmentElement = this._targetItem.ItemRosterElement.EquipmentElement;
					this.SetWeaponComponentTooltip(equipmentElement, this.AlternativeUsageIndex, equipmentElement2, num, false);
					this.TargetItemFlagList.Clear();
					this.ComparedItemFlagList.Clear();
					this.AddWeaponItemFlags(this.TargetItemFlagList, weaponComponentData);
					this.AddWeaponItemFlags(this.ComparedItemFlagList, weaponComponentData);
					return;
				}
				this._resetComparedItems(this._targetItem, this.AlternativeUsageIndex);
			}
		}

		// Token: 0x06000B22 RID: 2850 RVA: 0x0002B938 File Offset: 0x00029B38
		private void GetComparedWeapon(string weaponUsageId, out EquipmentElement comparedWeapon, out int comparedUsageIndex)
		{
			comparedWeapon = EquipmentElement.Invalid;
			comparedUsageIndex = -1;
			int num;
			if (this.IsComparing && this._comparedItem != null && ItemHelper.IsWeaponComparableWithUsage(this._comparedItem.ItemRosterElement.EquipmentElement.Item, weaponUsageId, out num))
			{
				comparedWeapon = this._comparedItem.ItemRosterElement.EquipmentElement;
				comparedUsageIndex = num;
			}
		}

		// Token: 0x06000B23 RID: 2851 RVA: 0x0002B9A0 File Offset: 0x00029BA0
		private void SetGeneralComponentTooltip(bool isInit = true)
		{
			if (this._targetItem.ItemCost >= 0)
			{
				if (this._targetItem.ItemRosterElement.EquipmentElement.Item.Type == ItemObject.ItemTypeEnum.Goods || this._targetItem.ItemRosterElement.EquipmentElement.Item.Type == ItemObject.ItemTypeEnum.Animal || this._targetItem.ItemRosterElement.EquipmentElement.Item.Type == ItemObject.ItemTypeEnum.Horse)
				{
					GameTexts.SetVariable("PERCENTAGE", (int)MathF.Abs((float)(this._targetItem.ItemCost - this._targetItem.ItemRosterElement.EquipmentElement.Item.Value) / (float)this._targetItem.ItemRosterElement.EquipmentElement.Item.Value * 100f));
					ItemCategory itemCategory = this._targetItem.ItemRosterElement.EquipmentElement.Item.ItemCategory;
					float num = 0f;
					float num2 = 0f;
					if (Town.AllTowns != null)
					{
						foreach (Town town in Town.AllTowns)
						{
							num += town.GetItemCategoryPriceIndex(itemCategory);
							num2 += 1f;
						}
					}
					num /= num2;
					if ((float)this._targetItem.ItemCost / (float)this._targetItem.ItemRosterElement.EquipmentElement.Item.Value > num * 1.3f)
					{
						this._costProperty = this.CreateColoredProperty(this.TargetItemProperties, "", this._targetItem.ItemCost + this.GoldIcon, UIColors.NegativeIndicator, 1, new HintViewModel(GameTexts.FindText("str_inventory_cost_higher", null), null), TooltipProperty.TooltipPropertyFlags.Cost);
					}
					else if ((float)this._targetItem.ItemCost / (float)this._targetItem.ItemRosterElement.EquipmentElement.Item.Value < num * 0.8f)
					{
						this._costProperty = this.CreateColoredProperty(this.TargetItemProperties, "", this._targetItem.ItemCost + this.GoldIcon, UIColors.PositiveIndicator, 1, new HintViewModel(GameTexts.FindText("str_inventory_cost_lower", null), null), TooltipProperty.TooltipPropertyFlags.Cost);
					}
					else
					{
						this._costProperty = this.CreateColoredProperty(this.TargetItemProperties, "", this._targetItem.ItemCost + this.GoldIcon, UIColors.Gold, 1, new HintViewModel(GameTexts.FindText("str_inventory_cost_normal", null), null), TooltipProperty.TooltipPropertyFlags.Cost);
					}
				}
				else
				{
					this._costProperty = this.CreateColoredProperty(this.TargetItemProperties, "", this._targetItem.ItemCost + this.GoldIcon, UIColors.Gold, 1, null, TooltipProperty.TooltipPropertyFlags.Cost);
				}
			}
			if (this.IsComparing)
			{
				this.CreateColoredProperty(this.ComparedItemProperties, "", this._comparedItem.ItemCost + this.GoldIcon, UIColors.Gold, 2, null, TooltipProperty.TooltipPropertyFlags.Cost);
			}
			if (Game.Current.IsDevelopmentMode)
			{
				if (this._targetItem.ItemRosterElement.EquipmentElement.Item.Culture != null)
				{
					this.CreateColoredProperty(this.TargetItemProperties, "Culture: ", this._targetItem.ItemRosterElement.EquipmentElement.Item.Culture.StringId, UIColors.Gold, 0, null, TooltipProperty.TooltipPropertyFlags.None);
				}
				else
				{
					this.CreateColoredProperty(this.TargetItemProperties, "Culture: ", "No Culture", UIColors.Gold, 0, null, TooltipProperty.TooltipPropertyFlags.None);
				}
				this.CreateColoredProperty(this.TargetItemProperties, "ID: ", this._targetItem.ItemRosterElement.EquipmentElement.Item.StringId, UIColors.Gold, 0, null, TooltipProperty.TooltipPropertyFlags.None);
			}
			float equipmentWeightMultiplier = 1f;
			CharacterObject characterObject;
			bool flag = (characterObject = this._character as CharacterObject) != null && characterObject.GetPerkValue(DefaultPerks.Athletics.FormFittingArmor);
			SPItemVM spitemVM = this._getEquipmentAtIndex(this._targetItem.ItemType);
			bool flag2 = spitemVM != null && spitemVM.ItemType != EquipmentIndex.None && spitemVM.ItemType != EquipmentIndex.HorseHarness && this._targetItem.ItemRosterElement.EquipmentElement.Item.HasArmorComponent;
			if (flag && flag2)
			{
				equipmentWeightMultiplier += DefaultPerks.Athletics.FormFittingArmor.PrimaryBonus / 100f;
			}
			this.AddFloatProperty(this._weightText, (EquipmentElement x) => x.GetEquipmentElementWeight() * equipmentWeightMultiplier, true);
			ItemObject item = this._targetItem.ItemRosterElement.EquipmentElement.Item;
			if (item.RelevantSkill != null && (item.Difficulty > 0 || (this.IsComparing && this._comparedItem.ItemRosterElement.EquipmentElement.Item.Difficulty > 0)))
			{
				this.AddSkillRequirement(this._targetItem, this.TargetItemProperties, false);
				if (this.IsComparing)
				{
					this.AddSkillRequirement(this._comparedItem, this.ComparedItemProperties, true);
				}
			}
			if (isInit)
			{
				this.AddGeneralItemFlags(this.TargetItemFlagList, item);
				if (this.IsComparing)
				{
					this.AddGeneralItemFlags(this.ComparedItemFlagList, this._comparedItem.ItemRosterElement.EquipmentElement.Item);
				}
			}
		}

		// Token: 0x06000B24 RID: 2852 RVA: 0x0002BF3C File Offset: 0x0002A13C
		private void AddSkillRequirement(ItemVM itemVm, MBBindingList<ItemMenuTooltipPropertyVM> itemProperties, bool isComparison)
		{
			ItemObject item = itemVm.ItemRosterElement.EquipmentElement.Item;
			string text = "";
			if (item.Difficulty > 0)
			{
				text = item.RelevantSkill.Name.ToString();
				text += " ";
				text += item.Difficulty.ToString();
			}
			string text2 = "";
			if (!isComparison)
			{
				text2 = this._requiresText.ToString();
			}
			this.CreateColoredProperty(itemProperties, text2, text, this.GetColorFromBool(this._character == null || CharacterHelper.CanUseItemBasedOnSkill(this._character, itemVm.ItemRosterElement.EquipmentElement)), 0, null, TooltipProperty.TooltipPropertyFlags.None);
		}

		// Token: 0x06000B25 RID: 2853 RVA: 0x0002BFE8 File Offset: 0x0002A1E8
		private void AddGeneralItemFlags(MBBindingList<ItemFlagVM> list, ItemObject item)
		{
			if (item.IsUniqueItem)
			{
				list.Add(new ItemFlagVM("GeneralFlagIcons\\unique", GameTexts.FindText("str_inventory_flag_unique", null)));
			}
			if (item.IsCivilian)
			{
				list.Add(new ItemFlagVM("GeneralFlagIcons\\civillian", GameTexts.FindText("str_inventory_flag_civillian", null)));
			}
			if (item.ItemFlags.HasAnyFlag(ItemFlags.NotUsableByFemale))
			{
				list.Add(new ItemFlagVM("GeneralFlagIcons\\male_only", GameTexts.FindText("str_inventory_flag_male_only", null)));
			}
			if (item.ItemFlags.HasAnyFlag(ItemFlags.NotUsableByMale))
			{
				list.Add(new ItemFlagVM("GeneralFlagIcons\\female_only", GameTexts.FindText("str_inventory_flag_female_only", null)));
			}
		}

		// Token: 0x06000B26 RID: 2854 RVA: 0x0002C095 File Offset: 0x0002A295
		private void AddFoodItemFlags(MBBindingList<ItemFlagVM> list, ItemObject item)
		{
			list.Add(new ItemFlagVM("GoodsFlagIcons\\consumable", GameTexts.FindText("str_inventory_flag_consumable", null)));
		}

		// Token: 0x06000B27 RID: 2855 RVA: 0x0002C0B4 File Offset: 0x0002A2B4
		private void AddWeaponItemFlags(MBBindingList<ItemFlagVM> list, WeaponComponentData weapon)
		{
			if (weapon == null)
			{
				Debug.FailedAssert("Trying to add flags for a null weapon", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem.ViewModelCollection\\Inventory\\ItemMenuVM.cs", "AddWeaponItemFlags", 640);
				return;
			}
			ItemObject.ItemUsageSetFlags itemUsageSetFlags = this._getItemUsageSetFlags(weapon);
			foreach (ValueTuple<string, TextObject> valueTuple in CampaignUIHelper.GetFlagDetailsForWeapon(weapon, itemUsageSetFlags, this._character as CharacterObject))
			{
				list.Add(new ItemFlagVM(valueTuple.Item1, valueTuple.Item2));
			}
		}

		// Token: 0x06000B28 RID: 2856 RVA: 0x0002C150 File Offset: 0x0002A350
		private Color GetColorFromBool(bool booleanValue)
		{
			if (!booleanValue)
			{
				return UIColors.NegativeIndicator;
			}
			return UIColors.PositiveIndicator;
		}

		// Token: 0x06000B29 RID: 2857 RVA: 0x0002C160 File Offset: 0x0002A360
		private void SetFoodTooltip()
		{
			this.CreateColoredProperty(this.TargetItemProperties, "", this._foodText.ToString(), this.ConsumableColor, 1, null, TooltipProperty.TooltipPropertyFlags.None);
			this.AddFoodItemFlags(this.TargetItemFlagList, this._targetItem.ItemRosterElement.EquipmentElement.Item);
		}

		// Token: 0x06000B2A RID: 2858 RVA: 0x0002C1B8 File Offset: 0x0002A3B8
		private void SetHorseComponentTooltip()
		{
			HorseComponent horseComponent = this._targetItem.ItemRosterElement.EquipmentElement.Item.HorseComponent;
			HorseComponent horseComponent2 = (this.IsComparing ? this._comparedItem.ItemRosterElement.EquipmentElement.Item.HorseComponent : null);
			this.CreateProperty(this.TargetItemProperties, this._typeText.ToString(), GameTexts.FindText("str_inventory_type_" + (int)this._targetItem.ItemRosterElement.EquipmentElement.Item.Type, null).ToString(), 0, null);
			this.AddHorseItemFlags(this.TargetItemFlagList, this._targetItem.ItemRosterElement.EquipmentElement.Item, horseComponent);
			if (this.IsComparing)
			{
				this.CreateProperty(this.ComparedItemProperties, " ", GameTexts.FindText("str_inventory_type_" + (int)this._comparedItem.ItemRosterElement.EquipmentElement.Item.Type, null).ToString(), 0, null);
				this.AddHorseItemFlags(this.ComparedItemFlagList, this._comparedItem.ItemRosterElement.EquipmentElement.Item, horseComponent2);
			}
			if (this._targetItem.ItemRosterElement.EquipmentElement.Item.IsMountable)
			{
				this.AddIntProperty(this._horseTierText, (int)(this._targetItem.ItemRosterElement.EquipmentElement.Item.Tier + 1), (this.IsComparing && this._comparedItem != null) ? new int?((int)(this._comparedItem.ItemRosterElement.EquipmentElement.Item.Tier + 1)) : null);
				this.AddIntProperty(this._chargeDamageText, this._targetItem.ItemRosterElement.EquipmentElement.GetModifiedMountCharge(EquipmentElement.Invalid), (this.IsComparing && this._comparedItem != null) ? new int?(this._comparedItem.ItemRosterElement.EquipmentElement.GetModifiedMountCharge(EquipmentElement.Invalid)) : null);
				this.AddIntProperty(this._speedText, this._targetItem.ItemRosterElement.EquipmentElement.GetModifiedMountSpeed(EquipmentElement.Invalid), (this.IsComparing && this._comparedItem != null) ? new int?(this._comparedItem.ItemRosterElement.EquipmentElement.GetModifiedMountSpeed(EquipmentElement.Invalid)) : null);
				this.AddIntProperty(this._maneuverText, this._targetItem.ItemRosterElement.EquipmentElement.GetModifiedMountManeuver(EquipmentElement.Invalid), (this.IsComparing && this._comparedItem != null) ? new int?(this._comparedItem.ItemRosterElement.EquipmentElement.GetModifiedMountManeuver(EquipmentElement.Invalid)) : null);
				this.AddIntProperty(this._hitPointsText, this._targetItem.ItemRosterElement.EquipmentElement.GetModifiedMountHitPoints(), (this.IsComparing && this._comparedItem != null) ? new int?(this._comparedItem.ItemRosterElement.EquipmentElement.GetModifiedMountHitPoints()) : null);
				if (this._targetItem.ItemRosterElement.EquipmentElement.Item.HasHorseComponent && this._targetItem.ItemRosterElement.EquipmentElement.Item.HorseComponent.IsMount)
				{
					this.AddComparableStringProperty(this._horseTypeText, (EquipmentElement x) => x.Item.ItemCategory.GetName().ToString(), (EquipmentElement x) => this.GetHorseCategoryValue(x.Item.ItemCategory));
				}
			}
		}

		// Token: 0x06000B2B RID: 2859 RVA: 0x0002C590 File Offset: 0x0002A790
		private void AddHorseItemFlags(MBBindingList<ItemFlagVM> list, ItemObject item, HorseComponent horse)
		{
			if (!horse.IsLiveStock)
			{
				if (item.ItemCategory == DefaultItemCategories.PackAnimal)
				{
					list.Add(new ItemFlagVM("MountFlagIcons\\weight_carrying_mount", GameTexts.FindText("str_inventory_flag_carrying_mount", null)));
				}
				else
				{
					list.Add(new ItemFlagVM("MountFlagIcons\\speed_mount", GameTexts.FindText("str_inventory_flag_speed_mount", null)));
				}
			}
			if (this._inventoryLogic.IsSlaughterable(item))
			{
				list.Add(new ItemFlagVM("MountFlagIcons\\slaughterable", GameTexts.FindText("str_inventory_flag_slaughterable", null)));
			}
		}

		// Token: 0x06000B2C RID: 2860 RVA: 0x0002C614 File Offset: 0x0002A814
		private void SetMerchandiseComponentTooltip()
		{
			if (Campaign.Current.GameMode == CampaignGameMode.Campaign)
			{
				if (this._tradeRumorsBehavior == null)
				{
					return;
				}
				IEnumerable<TradeRumor> tradeRumors = this._tradeRumorsBehavior.TradeRumors;
				bool flag = true;
				IMarketData marketData = this._inventoryLogic.MarketData;
				foreach (TradeRumor tradeRumor in from x in tradeRumors
					orderby x.SellPrice descending, x.BuyPrice descending
					select x)
				{
					bool flag2 = false;
					bool flag3 = false;
					if (this._targetItem.ItemRosterElement.EquipmentElement.Item == tradeRumor.ItemCategory)
					{
						if ((float)tradeRumor.BuyPrice < 0.9f * (float)marketData.GetPrice(tradeRumor.ItemCategory, MobileParty.MainParty, true, this._inventoryLogic.OtherParty))
						{
							flag3 = true;
						}
						if ((float)tradeRumor.SellPrice > 1.1f * (float)marketData.GetPrice(tradeRumor.ItemCategory, MobileParty.MainParty, false, this._inventoryLogic.OtherParty))
						{
							flag2 = true;
						}
						if ((Settlement.CurrentSettlement == null || Settlement.CurrentSettlement != tradeRumor.Settlement) && this._targetItem.ItemRosterElement.EquipmentElement.Item == tradeRumor.ItemCategory && (flag3 || flag2))
						{
							if (flag)
							{
								this.CreateColoredProperty(this.TargetItemProperties, "", this._tradeRumorsText.ToString(), this.TitleColor, 1, null, TooltipProperty.TooltipPropertyFlags.None);
								if (this.IsComparing)
								{
									this.CreateProperty(this.ComparedItemProperties, "", "", 0, null);
									this.CreateProperty(this.ComparedItemProperties, "", "", 0, null);
								}
								flag = false;
							}
							MBTextManager.SetTextVariable("SETTLEMENT_NAME", tradeRumor.Settlement.Name, false);
							MBTextManager.SetTextVariable("SELL_PRICE", tradeRumor.SellPrice);
							MBTextManager.SetTextVariable("BUY_PRICE", tradeRumor.BuyPrice);
							float num = this.CalculateTradeRumorOldnessFactor(tradeRumor);
							Color color = new Color(this.TitleColor.Red, this.TitleColor.Green, this.TitleColor.Blue, num);
							TextObject textObject = (flag3 ? GameTexts.FindText("str_trade_rumors_text_buy", null) : GameTexts.FindText("str_trade_rumors_text_sell", null));
							this.CreateColoredProperty(this.TargetItemProperties, "", textObject.ToString(), color, 0, null, TooltipProperty.TooltipPropertyFlags.None);
							if (this.IsComparing)
							{
								this.CreateProperty(this.ComparedItemProperties, "", "", 0, null);
							}
						}
					}
				}
			}
		}

		// Token: 0x06000B2D RID: 2861 RVA: 0x0002C8DC File Offset: 0x0002AADC
		private float CalculateTradeRumorOldnessFactor(TradeRumor rumor)
		{
			return MathF.Clamp((float)((int)rumor.RumorEndTime.RemainingDaysFromNow) / 5f, 0.5f, 1f);
		}

		// Token: 0x06000B2E RID: 2862 RVA: 0x0002C910 File Offset: 0x0002AB10
		private void SetWeaponComponentTooltip(in EquipmentElement targetWeapon, int targetWeaponUsageIndex, EquipmentElement comparedWeapon, int comparedWeaponUsageIndex, bool isInit)
		{
			EquipmentElement equipmentElement = targetWeapon;
			WeaponComponentData weaponWithUsageIndex = equipmentElement.Item.GetWeaponWithUsageIndex(targetWeaponUsageIndex);
			if (this.IsComparing && this._comparedItem != null && comparedWeapon.IsEmpty)
			{
				this.GetComparedWeapon(weaponWithUsageIndex.WeaponDescriptionId, out comparedWeapon, out comparedWeaponUsageIndex);
			}
			WeaponComponentData weaponComponentData = (comparedWeapon.IsEmpty ? null : comparedWeapon.Item.GetWeaponWithUsageIndex(comparedWeaponUsageIndex));
			if (isInit)
			{
				this.AddWeaponItemFlags(this.TargetItemFlagList, weaponWithUsageIndex);
				if (this.IsComparing)
				{
					this.AddWeaponItemFlags(this.ComparedItemFlagList, weaponComponentData);
				}
				if (targetWeaponUsageIndex == 0)
				{
					this.AlternativeUsageIndex = -1;
				}
				equipmentElement = targetWeapon;
				foreach (WeaponComponentData weaponComponentData2 in equipmentElement.Item.Weapons)
				{
					if (CampaignUIHelper.IsItemUsageApplicable(weaponComponentData2))
					{
						this.AlternativeUsages.Add(new StringItemWithHintVM(GameTexts.FindText("str_weapon_usage", weaponComponentData2.WeaponDescriptionId).ToString(), GameTexts.FindText("str_inventory_alternative_usage_hint", null)));
					}
				}
				this.AlternativeUsageIndex = targetWeaponUsageIndex;
			}
			this.CreateProperty(this.TargetItemProperties, this._classText.ToString(), GameTexts.FindText("str_inventory_weapon", ((int)weaponWithUsageIndex.WeaponClass).ToString()).ToString(), 0, null);
			if (!comparedWeapon.IsEmpty)
			{
				this.CreateProperty(this.ComparedItemProperties, " ", GameTexts.FindText("str_inventory_weapon", ((int)weaponWithUsageIndex.WeaponClass).ToString()).ToString(), 0, null);
			}
			else if (this.IsComparing)
			{
				this.CreateProperty(this.ComparedItemProperties, "", "", 0, null);
			}
			equipmentElement = targetWeapon;
			if (equipmentElement.Item.BannerComponent == null)
			{
				int num = 0;
				if (!comparedWeapon.IsEmpty)
				{
					num = (int)(comparedWeapon.Item.Tier + 1);
				}
				TextObject weaponTierText = this._weaponTierText;
				equipmentElement = targetWeapon;
				this.AddIntProperty(weaponTierText, (int)(equipmentElement.Item.Tier + 1), new int?(num));
			}
			ItemObject.ItemTypeEnum itemTypeFromWeaponClass = WeaponComponentData.GetItemTypeFromWeaponClass(weaponWithUsageIndex.WeaponClass);
			ItemObject.ItemTypeEnum itemTypeEnum = ((!comparedWeapon.IsEmpty) ? WeaponComponentData.GetItemTypeFromWeaponClass(weaponWithUsageIndex.WeaponClass) : ItemObject.ItemTypeEnum.Invalid);
			if (itemTypeFromWeaponClass == ItemObject.ItemTypeEnum.OneHandedWeapon || itemTypeFromWeaponClass == ItemObject.ItemTypeEnum.TwoHandedWeapon || itemTypeFromWeaponClass == ItemObject.ItemTypeEnum.Polearm || itemTypeEnum == ItemObject.ItemTypeEnum.OneHandedWeapon || itemTypeEnum == ItemObject.ItemTypeEnum.TwoHandedWeapon || itemTypeEnum == ItemObject.ItemTypeEnum.Polearm)
			{
				if (weaponWithUsageIndex.SwingDamageType != DamageTypes.Invalid)
				{
					TextObject swingSpeedText = this._swingSpeedText;
					equipmentElement = targetWeapon;
					this.AddIntProperty(swingSpeedText, equipmentElement.GetModifiedSwingSpeedForUsage(targetWeaponUsageIndex), comparedWeapon.IsEmpty ? null : new int?(comparedWeapon.GetModifiedSwingSpeedForUsage(comparedWeaponUsageIndex)));
					this.AddSwingDamageProperty(this._swingDamageText, targetWeapon, targetWeaponUsageIndex, comparedWeapon, comparedWeaponUsageIndex);
				}
				if (weaponWithUsageIndex.ThrustDamageType != DamageTypes.Invalid)
				{
					TextObject thrustSpeedText = this._thrustSpeedText;
					equipmentElement = targetWeapon;
					this.AddIntProperty(thrustSpeedText, equipmentElement.GetModifiedThrustSpeedForUsage(targetWeaponUsageIndex), comparedWeapon.IsEmpty ? null : new int?(comparedWeapon.GetModifiedThrustSpeedForUsage(comparedWeaponUsageIndex)));
					this.AddThrustDamageProperty(this._thrustDamageText, targetWeapon, targetWeaponUsageIndex, comparedWeapon, comparedWeaponUsageIndex);
				}
				this.AddIntProperty(this._lengthText, weaponWithUsageIndex.WeaponLength, (weaponComponentData != null) ? new int?(weaponComponentData.WeaponLength) : null);
				TextObject handlingText = this._handlingText;
				equipmentElement = targetWeapon;
				this.AddIntProperty(handlingText, equipmentElement.GetModifiedHandlingForUsage(targetWeaponUsageIndex), comparedWeapon.IsEmpty ? null : new int?(comparedWeapon.GetModifiedHandlingForUsage(comparedWeaponUsageIndex)));
			}
			if (itemTypeFromWeaponClass == ItemObject.ItemTypeEnum.Thrown || itemTypeEnum == ItemObject.ItemTypeEnum.Thrown)
			{
				this.AddIntProperty(this._weaponLengthText, weaponWithUsageIndex.WeaponLength, (weaponComponentData != null) ? new int?(weaponComponentData.WeaponLength) : null);
				this.AddMissileDamageProperty(this._damageText, targetWeapon, targetWeaponUsageIndex, comparedWeapon, comparedWeaponUsageIndex);
				TextObject missileSpeedText = this._missileSpeedText;
				equipmentElement = targetWeapon;
				this.AddIntProperty(missileSpeedText, equipmentElement.GetModifiedMissileSpeedForUsage(targetWeaponUsageIndex), comparedWeapon.IsEmpty ? null : new int?(comparedWeapon.GetModifiedMissileSpeedForUsage(comparedWeaponUsageIndex)));
				this.AddIntProperty(this._accuracyText, weaponWithUsageIndex.Accuracy, (weaponComponentData != null) ? new int?(weaponComponentData.Accuracy) : null);
				TextObject stackAmountText = this._stackAmountText;
				equipmentElement = targetWeapon;
				this.AddIntProperty(stackAmountText, (int)equipmentElement.GetModifiedStackCountForUsage(targetWeaponUsageIndex), comparedWeapon.IsEmpty ? null : new int?((int)comparedWeapon.GetModifiedStackCountForUsage(comparedWeaponUsageIndex)));
			}
			if (itemTypeFromWeaponClass == ItemObject.ItemTypeEnum.Shield || itemTypeEnum == ItemObject.ItemTypeEnum.Shield)
			{
				TextObject speedText = this._speedText;
				equipmentElement = targetWeapon;
				this.AddIntProperty(speedText, equipmentElement.GetModifiedSwingSpeedForUsage(targetWeaponUsageIndex), comparedWeapon.IsEmpty ? null : new int?(comparedWeapon.GetModifiedSwingSpeedForUsage(comparedWeaponUsageIndex)));
				TextObject hitPointsText = this._hitPointsText;
				equipmentElement = targetWeapon;
				this.AddIntProperty(hitPointsText, (int)equipmentElement.GetModifiedMaximumHitPointsForUsage(targetWeaponUsageIndex), comparedWeapon.IsEmpty ? null : new int?((int)comparedWeapon.GetModifiedMaximumHitPointsForUsage(comparedWeaponUsageIndex)));
			}
			if (itemTypeFromWeaponClass == ItemObject.ItemTypeEnum.Bow || itemTypeFromWeaponClass == ItemObject.ItemTypeEnum.Crossbow || itemTypeEnum == ItemObject.ItemTypeEnum.Bow || itemTypeEnum == ItemObject.ItemTypeEnum.Crossbow)
			{
				TextObject speedText2 = this._speedText;
				equipmentElement = targetWeapon;
				this.AddIntProperty(speedText2, equipmentElement.GetModifiedSwingSpeedForUsage(targetWeaponUsageIndex), comparedWeapon.IsEmpty ? null : new int?(comparedWeapon.GetModifiedSwingSpeedForUsage(comparedWeaponUsageIndex)));
				this.AddThrustDamageProperty(this._damageText, targetWeapon, targetWeaponUsageIndex, comparedWeapon, comparedWeaponUsageIndex);
				this.AddIntProperty(this._accuracyText, weaponWithUsageIndex.Accuracy, (weaponComponentData != null) ? new int?(weaponComponentData.Accuracy) : null);
				TextObject missileSpeedText2 = this._missileSpeedText;
				equipmentElement = targetWeapon;
				this.AddIntProperty(missileSpeedText2, equipmentElement.GetModifiedMissileSpeedForUsage(targetWeaponUsageIndex), comparedWeapon.IsEmpty ? null : new int?(comparedWeapon.GetModifiedMissileSpeedForUsage(comparedWeaponUsageIndex)));
				if (itemTypeFromWeaponClass == ItemObject.ItemTypeEnum.Crossbow || itemTypeEnum == ItemObject.ItemTypeEnum.Crossbow)
				{
					TextObject ammoLimitText = this._ammoLimitText;
					int maxDataValue = (int)weaponWithUsageIndex.MaxDataValue;
					short? num2 = ((weaponComponentData != null) ? new short?(weaponComponentData.MaxDataValue) : null);
					this.AddIntProperty(ammoLimitText, maxDataValue, (num2 != null) ? new int?((int)num2.GetValueOrDefault()) : null);
				}
			}
			if (weaponWithUsageIndex.IsAmmo || (weaponComponentData != null && weaponComponentData.IsAmmo))
			{
				if ((itemTypeFromWeaponClass != ItemObject.ItemTypeEnum.Arrows && itemTypeFromWeaponClass != ItemObject.ItemTypeEnum.Bolts) || (weaponComponentData != null && itemTypeEnum != ItemObject.ItemTypeEnum.Arrows && itemTypeEnum != ItemObject.ItemTypeEnum.Bolts))
				{
					this.AddIntProperty(this._accuracyText, weaponWithUsageIndex.Accuracy, (weaponComponentData != null) ? new int?(weaponComponentData.Accuracy) : null);
				}
				this.AddThrustDamageProperty(this._damageText, targetWeapon, targetWeaponUsageIndex, comparedWeapon, comparedWeaponUsageIndex);
				TextObject stackAmountText2 = this._stackAmountText;
				equipmentElement = targetWeapon;
				this.AddIntProperty(stackAmountText2, (int)equipmentElement.GetModifiedStackCountForUsage(targetWeaponUsageIndex), comparedWeapon.IsEmpty ? null : new int?((int)comparedWeapon.GetModifiedStackCountForUsage(comparedWeaponUsageIndex)));
			}
			equipmentElement = targetWeapon;
			ItemObject item = equipmentElement.Item;
			if (item == null || !item.HasBannerComponent)
			{
				ItemObject item2 = comparedWeapon.Item;
				if (item2 == null || !item2.HasBannerComponent)
				{
					goto IL_717;
				}
			}
			Func<EquipmentElement, string> func = delegate(EquipmentElement x)
			{
				ItemObject item3 = x.Item;
				bool flag;
				if (item3 == null)
				{
					flag = null != null;
				}
				else
				{
					BannerComponent bannerComponent = item3.BannerComponent;
					flag = ((bannerComponent != null) ? bannerComponent.BannerEffect : null) != null;
				}
				if (flag)
				{
					GameTexts.SetVariable("RANK", x.Item.BannerComponent.BannerEffect.Name);
					string text = string.Empty;
					if (x.Item.BannerComponent.BannerEffect.IncrementType == BannerEffect.EffectIncrementType.AddFactor)
					{
						GameTexts.FindText("str_NUMBER_percent", null).SetTextVariable("NUMBER", ((int)Math.Abs(x.Item.BannerComponent.GetBannerEffectBonus() * 100f)).ToString());
						object obj;
						text = obj.ToString();
					}
					else if (x.Item.BannerComponent.BannerEffect.IncrementType == BannerEffect.EffectIncrementType.Add)
					{
						text = x.Item.BannerComponent.GetBannerEffectBonus().ToString();
					}
					GameTexts.SetVariable("NUMBER", text);
					return GameTexts.FindText("str_RANK_with_NUM_between_parenthesis", null).ToString();
				}
				return this._noneText.ToString();
			};
			this.AddComparableStringProperty(this._bannerEffectText, func, (EquipmentElement x) => 0);
			IL_717:
			this.AddDonationXpTooltip();
		}

		// Token: 0x06000B2F RID: 2863 RVA: 0x0002D04C File Offset: 0x0002B24C
		private void AddIntProperty(TextObject description, int targetValue, int? comparedValue)
		{
			string text = targetValue.ToString();
			if (this.IsComparing && comparedValue != null)
			{
				string text2 = comparedValue.Value.ToString();
				int num = this.CompareValues(targetValue, comparedValue.Value);
				this.CreateColoredProperty(this.TargetItemProperties, description.ToString(), text, this.GetColorFromComparison(num, false), 0, null, TooltipProperty.TooltipPropertyFlags.None);
				this.CreateColoredProperty(this.ComparedItemProperties, " ", text2, this.GetColorFromComparison(num, true), 0, null, TooltipProperty.TooltipPropertyFlags.None);
				return;
			}
			this.CreateColoredProperty(this.TargetItemProperties, description.ToString(), text, this.GetColorFromComparison(0, false), 0, null, TooltipProperty.TooltipPropertyFlags.None);
		}

		// Token: 0x06000B30 RID: 2864 RVA: 0x0002D0F0 File Offset: 0x0002B2F0
		private void AddFloatProperty(TextObject description, Func<EquipmentElement, float> func, bool reversedCompare = false)
		{
			float num = func(this._targetItem.ItemRosterElement.EquipmentElement);
			float? num2 = null;
			if (this.IsComparing && this._comparedItem != null)
			{
				num2 = new float?(func(this._comparedItem.ItemRosterElement.EquipmentElement));
			}
			this.AddFloatProperty(description, num, num2, reversedCompare);
		}

		// Token: 0x06000B31 RID: 2865 RVA: 0x0002D154 File Offset: 0x0002B354
		private void AddFloatProperty(TextObject description, float targetValue, float? comparedValue, bool reversedCompare = false)
		{
			string text = targetValue.ToString("0.0");
			if (this.IsComparing && comparedValue != null)
			{
				string text2 = comparedValue.Value.ToString("0.0");
				int num = this.CompareValues(targetValue, comparedValue.Value);
				if (reversedCompare)
				{
					num *= -1;
				}
				this.CreateColoredProperty(this.TargetItemProperties, description.ToString(), text, this.GetColorFromComparison(num, false), 0, null, TooltipProperty.TooltipPropertyFlags.None);
				this.CreateColoredProperty(this.ComparedItemProperties, " ", text2, this.GetColorFromComparison(num, true), 0, null, TooltipProperty.TooltipPropertyFlags.None);
				return;
			}
			this.CreateColoredProperty(this.TargetItemProperties, description.ToString(), text, this.GetColorFromComparison(0, false), 0, null, TooltipProperty.TooltipPropertyFlags.None);
		}

		// Token: 0x06000B32 RID: 2866 RVA: 0x0002D208 File Offset: 0x0002B408
		private void AddComparableStringProperty(TextObject description, Func<EquipmentElement, string> valueAsStringFunc, Func<EquipmentElement, int> valueAsIntFunc)
		{
			string text = valueAsStringFunc(this._targetItem.ItemRosterElement.EquipmentElement);
			int num = valueAsIntFunc(this._targetItem.ItemRosterElement.EquipmentElement);
			if (this.IsComparing && this._comparedItem != null)
			{
				int num2 = valueAsIntFunc(this._comparedItem.ItemRosterElement.EquipmentElement);
				int num3 = this.CompareValues(num, num2);
				this.CreateColoredProperty(this.TargetItemProperties, description.ToString(), text, this.GetColorFromComparison(num3, false), 0, null, TooltipProperty.TooltipPropertyFlags.None);
				this.CreateColoredProperty(this.ComparedItemProperties, " ", valueAsStringFunc(this._comparedItem.ItemRosterElement.EquipmentElement), this.GetColorFromComparison(num3, true), 0, null, TooltipProperty.TooltipPropertyFlags.None);
				return;
			}
			this.CreateColoredProperty(this.TargetItemProperties, description.ToString(), text, this.GetColorFromComparison(0, false), 0, null, TooltipProperty.TooltipPropertyFlags.None);
		}

		// Token: 0x06000B33 RID: 2867 RVA: 0x0002D2E8 File Offset: 0x0002B4E8
		private void AddSwingDamageProperty(TextObject description, in EquipmentElement targetWeapon, int targetWeaponUsageIndex, in EquipmentElement comparedWeapon, int comparedWeaponUsageIndex)
		{
			EquipmentElement equipmentElement = targetWeapon;
			int modifiedSwingDamageForUsage = equipmentElement.GetModifiedSwingDamageForUsage(targetWeaponUsageIndex);
			equipmentElement = targetWeapon;
			WeaponComponentData weaponWithUsageIndex = equipmentElement.Item.GetWeaponWithUsageIndex(targetWeaponUsageIndex);
			equipmentElement = targetWeapon;
			string text = ItemHelper.GetSwingDamageText(weaponWithUsageIndex, equipmentElement.ItemModifier).ToString();
			if (this.IsComparing)
			{
				equipmentElement = comparedWeapon;
				if (!equipmentElement.IsEmpty)
				{
					equipmentElement = comparedWeapon;
					int modifiedSwingDamageForUsage2 = equipmentElement.GetModifiedSwingDamageForUsage(comparedWeaponUsageIndex);
					equipmentElement = comparedWeapon;
					WeaponComponentData weaponWithUsageIndex2 = equipmentElement.Item.GetWeaponWithUsageIndex(comparedWeaponUsageIndex);
					equipmentElement = comparedWeapon;
					string text2 = ItemHelper.GetSwingDamageText(weaponWithUsageIndex2, equipmentElement.ItemModifier).ToString();
					int num = this.CompareValues(modifiedSwingDamageForUsage, modifiedSwingDamageForUsage2);
					this.CreateColoredProperty(this.TargetItemProperties, description.ToString(), text, this.GetColorFromComparison(num, false), 0, null, TooltipProperty.TooltipPropertyFlags.None);
					this.CreateColoredProperty(this.ComparedItemProperties, " ", text2, this.GetColorFromComparison(num, true), 0, null, TooltipProperty.TooltipPropertyFlags.None);
					return;
				}
			}
			this.CreateColoredProperty(this.TargetItemProperties, description.ToString(), text, this.GetColorFromComparison(0, true), 0, null, TooltipProperty.TooltipPropertyFlags.None);
		}

		// Token: 0x06000B34 RID: 2868 RVA: 0x0002D400 File Offset: 0x0002B600
		private void AddMissileDamageProperty(TextObject description, in EquipmentElement targetWeapon, int targetWeaponUsageIndex, in EquipmentElement comparedWeapon, int comparedWeaponUsageIndex)
		{
			EquipmentElement equipmentElement = targetWeapon;
			int modifiedMissileDamageForUsage = equipmentElement.GetModifiedMissileDamageForUsage(targetWeaponUsageIndex);
			equipmentElement = targetWeapon;
			WeaponComponentData weaponWithUsageIndex = equipmentElement.Item.GetWeaponWithUsageIndex(targetWeaponUsageIndex);
			equipmentElement = targetWeapon;
			string text = ItemHelper.GetMissileDamageText(weaponWithUsageIndex, equipmentElement.ItemModifier).ToString();
			if (this.IsComparing)
			{
				equipmentElement = comparedWeapon;
				if (!equipmentElement.IsEmpty)
				{
					equipmentElement = comparedWeapon;
					int modifiedMissileDamageForUsage2 = equipmentElement.GetModifiedMissileDamageForUsage(comparedWeaponUsageIndex);
					equipmentElement = comparedWeapon;
					WeaponComponentData weaponWithUsageIndex2 = equipmentElement.Item.GetWeaponWithUsageIndex(comparedWeaponUsageIndex);
					equipmentElement = comparedWeapon;
					string text2 = ItemHelper.GetMissileDamageText(weaponWithUsageIndex2, equipmentElement.ItemModifier).ToString();
					int num = this.CompareValues(modifiedMissileDamageForUsage, modifiedMissileDamageForUsage2);
					this.CreateColoredProperty(this.TargetItemProperties, description.ToString(), text, this.GetColorFromComparison(num, false), 0, null, TooltipProperty.TooltipPropertyFlags.None);
					this.CreateColoredProperty(this.ComparedItemProperties, " ", text2, this.GetColorFromComparison(num, true), 0, null, TooltipProperty.TooltipPropertyFlags.None);
					return;
				}
			}
			this.CreateColoredProperty(this.TargetItemProperties, description.ToString(), text, this.GetColorFromComparison(0, true), 0, null, TooltipProperty.TooltipPropertyFlags.None);
		}

		// Token: 0x06000B35 RID: 2869 RVA: 0x0002D518 File Offset: 0x0002B718
		private void AddThrustDamageProperty(TextObject description, in EquipmentElement targetWeapon, int targetWeaponUsageIndex, in EquipmentElement comparedWeapon, int comparedWeaponUsageIndex)
		{
			EquipmentElement equipmentElement = targetWeapon;
			int modifiedThrustDamageForUsage = equipmentElement.GetModifiedThrustDamageForUsage(targetWeaponUsageIndex);
			equipmentElement = targetWeapon;
			WeaponComponentData weaponWithUsageIndex = equipmentElement.Item.GetWeaponWithUsageIndex(targetWeaponUsageIndex);
			equipmentElement = targetWeapon;
			string text = ItemHelper.GetThrustDamageText(weaponWithUsageIndex, equipmentElement.ItemModifier).ToString();
			if (this.IsComparing)
			{
				equipmentElement = comparedWeapon;
				if (!equipmentElement.IsEmpty)
				{
					equipmentElement = comparedWeapon;
					int modifiedThrustDamageForUsage2 = equipmentElement.GetModifiedThrustDamageForUsage(comparedWeaponUsageIndex);
					equipmentElement = comparedWeapon;
					WeaponComponentData weaponWithUsageIndex2 = equipmentElement.Item.GetWeaponWithUsageIndex(comparedWeaponUsageIndex);
					equipmentElement = comparedWeapon;
					string text2 = ItemHelper.GetThrustDamageText(weaponWithUsageIndex2, equipmentElement.ItemModifier).ToString();
					int num = this.CompareValues(modifiedThrustDamageForUsage, modifiedThrustDamageForUsage2);
					this.CreateColoredProperty(this.TargetItemProperties, description.ToString(), text, this.GetColorFromComparison(num, false), 0, null, TooltipProperty.TooltipPropertyFlags.None);
					this.CreateColoredProperty(this.ComparedItemProperties, " ", text2, this.GetColorFromComparison(num, true), 0, null, TooltipProperty.TooltipPropertyFlags.None);
					return;
				}
			}
			this.CreateColoredProperty(this.TargetItemProperties, description.ToString(), text, this.GetColorFromComparison(0, true), 0, null, TooltipProperty.TooltipPropertyFlags.None);
		}

		// Token: 0x06000B36 RID: 2870 RVA: 0x0002D630 File Offset: 0x0002B830
		private void SetArmorComponentTooltip()
		{
			int num = 0;
			if (this._comparedItem != null && this._comparedItem.ItemRosterElement.EquipmentElement.Item != null)
			{
				num = (int)(this._comparedItem.ItemRosterElement.EquipmentElement.Item.Tier + 1);
			}
			this.AddIntProperty(this._armorTierText, (int)(this._targetItem.ItemRosterElement.EquipmentElement.Item.Tier + 1), new int?(num));
			this.CreateProperty(this.TargetItemProperties, this._typeText.ToString(), GameTexts.FindText("str_inventory_type_" + (int)this._targetItem.ItemRosterElement.EquipmentElement.Item.Type, null).ToString(), 0, null);
			if (this.IsComparing)
			{
				this.CreateProperty(this.ComparedItemProperties, " ", GameTexts.FindText("str_inventory_type_" + (int)this._targetItem.ItemRosterElement.EquipmentElement.Item.Type, null).ToString(), 0, null);
			}
			ArmorComponent armorComponent = this._targetItem.ItemRosterElement.EquipmentElement.Item.ArmorComponent;
			ArmorComponent armorComponent2 = (this.IsComparing ? this._comparedItem.ItemRosterElement.EquipmentElement.Item.ArmorComponent : null);
			if (armorComponent.HeadArmor != 0 || (this.IsComparing && armorComponent2.HeadArmor != 0))
			{
				int num2 = (this.IsComparing ? this.CompareValues(this._targetItem.ItemRosterElement.EquipmentElement.GetModifiedHeadArmor(), this._comparedItem.ItemRosterElement.EquipmentElement.GetModifiedHeadArmor()) : 0);
				this.CreateColoredProperty(this.TargetItemProperties, this._headArmorText.ToString(), this._targetItem.ItemRosterElement.EquipmentElement.GetModifiedHeadArmor().ToString(), this.GetColorFromComparison(num2, false), 0, null, TooltipProperty.TooltipPropertyFlags.None);
				if (this.IsComparing)
				{
					this.CreateColoredProperty(this.ComparedItemProperties, " ", this._comparedItem.ItemRosterElement.EquipmentElement.GetModifiedHeadArmor().ToString(), this.GetColorFromComparison(num2, true), 0, null, TooltipProperty.TooltipPropertyFlags.None);
				}
			}
			if (armorComponent.BodyArmor != 0 || (this.IsComparing && this._comparedItem.ItemRosterElement.EquipmentElement.GetModifiedBodyArmor() != 0))
			{
				if (this._targetItem.ItemType == EquipmentIndex.HorseHarness)
				{
					int num2 = (this.IsComparing ? this.CompareValues(this._targetItem.ItemRosterElement.EquipmentElement.GetModifiedMountBodyArmor(), this._comparedItem.ItemRosterElement.EquipmentElement.GetModifiedMountBodyArmor()) : 0);
					this.CreateColoredProperty(this.TargetItemProperties, this._horseArmorText.ToString(), this._targetItem.ItemRosterElement.EquipmentElement.GetModifiedMountBodyArmor().ToString(), this.GetColorFromComparison(num2, false), 0, null, TooltipProperty.TooltipPropertyFlags.None);
					if (this.IsComparing)
					{
						this.CreateColoredProperty(this.ComparedItemProperties, " ", this._comparedItem.ItemRosterElement.EquipmentElement.GetModifiedMountBodyArmor().ToString(), this.GetColorFromComparison(num2, true), 0, null, TooltipProperty.TooltipPropertyFlags.None);
					}
				}
				else
				{
					int num2 = (this.IsComparing ? this.CompareValues(this._targetItem.ItemRosterElement.EquipmentElement.GetModifiedBodyArmor(), this._comparedItem.ItemRosterElement.EquipmentElement.GetModifiedBodyArmor()) : 0);
					this.CreateColoredProperty(this.TargetItemProperties, this._bodyArmorText.ToString(), this._targetItem.ItemRosterElement.EquipmentElement.GetModifiedBodyArmor().ToString(), this.GetColorFromComparison(num2, false), 0, null, TooltipProperty.TooltipPropertyFlags.None);
					if (this.IsComparing)
					{
						this.CreateColoredProperty(this.ComparedItemProperties, " ", this._comparedItem.ItemRosterElement.EquipmentElement.GetModifiedBodyArmor().ToString(), this.GetColorFromComparison(num2, true), 0, null, TooltipProperty.TooltipPropertyFlags.None);
					}
				}
			}
			if (this._targetItem.ItemRosterElement.EquipmentElement.GetModifiedLegArmor() != 0 || (this.IsComparing && this._comparedItem.ItemRosterElement.EquipmentElement.GetModifiedLegArmor() != 0))
			{
				int num2 = (this.IsComparing ? this.CompareValues(this._targetItem.ItemRosterElement.EquipmentElement.GetModifiedLegArmor(), this._comparedItem.ItemRosterElement.EquipmentElement.GetModifiedLegArmor()) : 0);
				this.CreateColoredProperty(this.TargetItemProperties, this._legArmorText.ToString(), this._targetItem.ItemRosterElement.EquipmentElement.GetModifiedLegArmor().ToString(), this.GetColorFromComparison(num2, false), 0, null, TooltipProperty.TooltipPropertyFlags.None);
				if (this.IsComparing)
				{
					this.CreateColoredProperty(this.ComparedItemProperties, " ", this._comparedItem.ItemRosterElement.EquipmentElement.GetModifiedLegArmor().ToString(), this.GetColorFromComparison(num2, true), 0, null, TooltipProperty.TooltipPropertyFlags.None);
				}
			}
			if (this._targetItem.ItemRosterElement.EquipmentElement.GetModifiedArmArmor() != 0 || (this.IsComparing && this._comparedItem.ItemRosterElement.EquipmentElement.GetModifiedArmArmor() != 0))
			{
				int num2 = (this.IsComparing ? this.CompareValues(this._targetItem.ItemRosterElement.EquipmentElement.GetModifiedArmArmor(), this._comparedItem.ItemRosterElement.EquipmentElement.GetModifiedArmArmor()) : 0);
				this.CreateColoredProperty(this.TargetItemProperties, this._armArmorText.ToString(), this._targetItem.ItemRosterElement.EquipmentElement.GetModifiedArmArmor().ToString(), this.GetColorFromComparison(num2, false), 0, null, TooltipProperty.TooltipPropertyFlags.None);
				if (this.IsComparing)
				{
					this.CreateColoredProperty(this.ComparedItemProperties, " ", this._comparedItem.ItemRosterElement.EquipmentElement.GetModifiedArmArmor().ToString(), this.GetColorFromComparison(num2, true), 0, null, TooltipProperty.TooltipPropertyFlags.None);
				}
			}
			this.AddDonationXpTooltip();
		}

		// Token: 0x06000B37 RID: 2871 RVA: 0x0002DC8C File Offset: 0x0002BE8C
		private void AddDonationXpTooltip()
		{
			ItemDiscardModel itemDiscardModel = Campaign.Current.Models.ItemDiscardModel;
			int xpBonusForDiscardingItem = itemDiscardModel.GetXpBonusForDiscardingItem(this._targetItem.ItemRosterElement.EquipmentElement.Item, 1);
			int num = (this.IsComparing ? itemDiscardModel.GetXpBonusForDiscardingItem(this._comparedItem.ItemRosterElement.EquipmentElement.Item, 1) : 0);
			if (xpBonusForDiscardingItem > 0 || (this.IsComparing && num > 0))
			{
				InventoryLogic inventoryLogic = this._inventoryLogic;
				if (inventoryLogic != null && inventoryLogic.IsDiscardDonating)
				{
					MBTextManager.SetTextVariable("LEFT", GameTexts.FindText("str_inventory_donation_item_hint", null).ToString(), false);
					int num2 = (this.IsComparing ? this.CompareValues(xpBonusForDiscardingItem, num) : 0);
					this.CreateColoredProperty(this.TargetItemProperties, GameTexts.FindText("str_LEFT_colon", null).ToString(), xpBonusForDiscardingItem.ToString(), this.GetColorFromComparison(num2, false), 0, null, TooltipProperty.TooltipPropertyFlags.None);
					if (this.IsComparing)
					{
						this.CreateColoredProperty(this.ComparedItemProperties, " ", num.ToString(), this.GetColorFromComparison(num2, true), 0, null, TooltipProperty.TooltipPropertyFlags.None);
					}
				}
			}
		}

		// Token: 0x06000B38 RID: 2872 RVA: 0x0002DDAE File Offset: 0x0002BFAE
		private Color GetColorFromComparison(int result, bool isCompared)
		{
			if (result != -1)
			{
				if (result != 1)
				{
					return Colors.Black;
				}
				if (!isCompared)
				{
					return UIColors.PositiveIndicator;
				}
				return UIColors.NegativeIndicator;
			}
			else
			{
				if (!isCompared)
				{
					return UIColors.NegativeIndicator;
				}
				return UIColors.PositiveIndicator;
			}
		}

		// Token: 0x06000B39 RID: 2873 RVA: 0x0002DDE0 File Offset: 0x0002BFE0
		private int GetHorseCategoryValue(ItemCategory itemCategory)
		{
			if (itemCategory.IsAnimal)
			{
				if (itemCategory == DefaultItemCategories.PackAnimal)
				{
					return 1;
				}
				if (itemCategory == DefaultItemCategories.Horse)
				{
					return 2;
				}
				if (itemCategory == DefaultItemCategories.WarHorse)
				{
					return 3;
				}
				if (itemCategory == DefaultItemCategories.NobleHorse)
				{
					return 4;
				}
			}
			Debug.FailedAssert("This horse item category is not defined", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem.ViewModelCollection\\Inventory\\ItemMenuVM.cs", "GetHorseCategoryValue", 1426);
			return -1;
		}

		// Token: 0x06000B3A RID: 2874 RVA: 0x0002DE38 File Offset: 0x0002C038
		private ItemMenuTooltipPropertyVM CreateProperty(MBBindingList<ItemMenuTooltipPropertyVM> targetList, string definition, string value, int textHeight = 0, HintViewModel hint = null)
		{
			ItemMenuTooltipPropertyVM itemMenuTooltipPropertyVM = new ItemMenuTooltipPropertyVM(definition, value, textHeight, false, hint);
			targetList.Add(itemMenuTooltipPropertyVM);
			return itemMenuTooltipPropertyVM;
		}

		// Token: 0x06000B3B RID: 2875 RVA: 0x0002DE5C File Offset: 0x0002C05C
		private ItemMenuTooltipPropertyVM CreateColoredProperty(MBBindingList<ItemMenuTooltipPropertyVM> targetList, string definition, string value, Color color, int textHeight = 0, HintViewModel hint = null, TooltipProperty.TooltipPropertyFlags propertyFlags = TooltipProperty.TooltipPropertyFlags.None)
		{
			if (color == Colors.Black)
			{
				this.CreateProperty(targetList, definition, value, textHeight, hint);
				return null;
			}
			ItemMenuTooltipPropertyVM itemMenuTooltipPropertyVM = new ItemMenuTooltipPropertyVM(definition, value, textHeight, color, false, hint, propertyFlags);
			targetList.Add(itemMenuTooltipPropertyVM);
			return itemMenuTooltipPropertyVM;
		}

		// Token: 0x06000B3C RID: 2876 RVA: 0x0002DEA0 File Offset: 0x0002C0A0
		public void SetTransactionCost(int getItemTotalPrice, int maxIndividualPrice)
		{
			this.TransactionTotalCost = getItemTotalPrice;
			if (this._targetItem.ItemCost == maxIndividualPrice)
			{
				this._costProperty.ValueLabel = this._targetItem.ItemCost + this.GoldIcon;
				return;
			}
			if (this._targetItem.ItemCost < maxIndividualPrice)
			{
				this._costProperty.ValueLabel = string.Concat(new object[]
				{
					this._targetItem.ItemCost,
					" - ",
					maxIndividualPrice,
					this.GoldIcon
				});
				return;
			}
			this._costProperty.ValueLabel = string.Concat(new object[]
			{
				maxIndividualPrice,
				" - ",
				this._targetItem.ItemCost,
				this.GoldIcon
			});
		}

		// Token: 0x040004F6 RID: 1270
		private readonly TextObject _swingDamageText = GameTexts.FindText("str_swing_damage", null);

		// Token: 0x040004F7 RID: 1271
		private readonly TextObject _swingSpeedText = new TextObject("{=345a87fcc69f626ae3916939ef2fc135}Swing Speed: ", null);

		// Token: 0x040004F8 RID: 1272
		private readonly TextObject _weaponTierText = new TextObject("{=weaponTier}Weapon Tier: ", null);

		// Token: 0x040004F9 RID: 1273
		private readonly TextObject _armorTierText = new TextObject("{=armorTier}Armor Tier: ", null);

		// Token: 0x040004FA RID: 1274
		private readonly TextObject _horseTierText = new TextObject("{=mountTier}Mount Tier: ", null);

		// Token: 0x040004FB RID: 1275
		private readonly TextObject _horseTypeText = new TextObject("{=9sxECG6e}Mount Type: ", null);

		// Token: 0x040004FC RID: 1276
		private readonly TextObject _chargeDamageText = new TextObject("{=c7638a0869219ae845de0f660fd57a9d}Charge Damage: ", null);

		// Token: 0x040004FD RID: 1277
		private readonly TextObject _hitPointsText = GameTexts.FindText("str_hit_points", null);

		// Token: 0x040004FE RID: 1278
		private readonly TextObject _speedText = new TextObject("{=74dc1908cb0b990e80fb977b5a0ef10d}Speed: ", null);

		// Token: 0x040004FF RID: 1279
		private readonly TextObject _maneuverText = new TextObject("{=3025020b83b218707499f0de3135ed0a}Maneuver: ", null);

		// Token: 0x04000500 RID: 1280
		private readonly TextObject _thrustSpeedText = GameTexts.FindText("str_thrust_speed", null);

		// Token: 0x04000501 RID: 1281
		private readonly TextObject _thrustDamageText = GameTexts.FindText("str_thrust_damage", null);

		// Token: 0x04000502 RID: 1282
		private readonly TextObject _lengthText = new TextObject("{=c6e4c8588ca9e42f6e1b47b11f0f367b}Length: ", null);

		// Token: 0x04000503 RID: 1283
		private readonly TextObject _weightText = GameTexts.FindText("str_weight_text", null);

		// Token: 0x04000504 RID: 1284
		private readonly TextObject _handlingText = new TextObject("{=ca8b1e8956057b831dfc665f54bae4b0}Handling: ", null);

		// Token: 0x04000505 RID: 1285
		private readonly TextObject _weaponLengthText = new TextObject("{=5fa36d2798479803b4518a64beb4d732}Weapon Length: ", null);

		// Token: 0x04000506 RID: 1286
		private readonly TextObject _damageText = new TextObject("{=c9c5dfed2ca6bcb7a73d905004c97b23}Damage: ", null);

		// Token: 0x04000507 RID: 1287
		private readonly TextObject _missileSpeedText = GameTexts.FindText("str_missile_speed", null);

		// Token: 0x04000508 RID: 1288
		private readonly TextObject _accuracyText = new TextObject("{=5dec16fa0be433ade3c4cb0074ef366d}Accuracy: ", null);

		// Token: 0x04000509 RID: 1289
		private readonly TextObject _stackAmountText = new TextObject("{=05fdfc6e238429753ef282f2ce97c1f8}Stack Amount: ", null);

		// Token: 0x0400050A RID: 1290
		private readonly TextObject _ammoLimitText = new TextObject("{=6adabc1f82216992571c3e22abc164d7}Ammo Limit: ", null);

		// Token: 0x0400050B RID: 1291
		private readonly TextObject _requiresText = new TextObject("{=154a34f8caccfc833238cc89d38861e8}Requires: ", null);

		// Token: 0x0400050C RID: 1292
		private readonly TextObject _foodText = new TextObject("{=qSi4DlT4}Food", null);

		// Token: 0x0400050D RID: 1293
		private readonly TextObject _partyMoraleText = new TextObject("{=a241aacb1780599430c79fd9f667b67f}Party Morale: ", null);

		// Token: 0x0400050E RID: 1294
		private readonly TextObject _typeText = new TextObject("{=08abd5af7774d311cadc3ed900b47754}Type: ", null);

		// Token: 0x0400050F RID: 1295
		private readonly TextObject _tradeRumorsText = new TextObject("{=f2971dc587a9777223ad2d7be236fb05}Trade Rumors", null);

		// Token: 0x04000510 RID: 1296
		private readonly TextObject _classText = new TextObject("{=8cad4a279770f269c4bb0dc7a357ee1e}Class: ", null);

		// Token: 0x04000511 RID: 1297
		private readonly TextObject _headArmorText = GameTexts.FindText("str_head_armor", null);

		// Token: 0x04000512 RID: 1298
		private readonly TextObject _horseArmorText = new TextObject("{=305cf7f98458b22e9af72b60a131714f}Horse Armor: ", null);

		// Token: 0x04000513 RID: 1299
		private readonly TextObject _bodyArmorText = GameTexts.FindText("str_body_armor", null);

		// Token: 0x04000514 RID: 1300
		private readonly TextObject _legArmorText = GameTexts.FindText("str_leg_armor", null);

		// Token: 0x04000515 RID: 1301
		private readonly TextObject _armArmorText = new TextObject("{=cf61cce254c7dca65be9bebac7fb9bf5}Arm Armor: ", null);

		// Token: 0x04000516 RID: 1302
		private readonly TextObject _bannerEffectText = new TextObject("{=DbXZjPdf}Banner Effect: ", null);

		// Token: 0x04000517 RID: 1303
		private readonly TextObject _noneText = new TextObject("{=koX9okuG}None", null);

		// Token: 0x04000518 RID: 1304
		private readonly string GoldIcon = "<img src=\"General\\Icons\\Coin@2x\" extend=\"8\"/>";

		// Token: 0x04000519 RID: 1305
		private readonly Color ConsumableColor = Color.FromUint(4290873921U);

		// Token: 0x0400051A RID: 1306
		private readonly Color TitleColor = Color.FromUint(4293446041U);

		// Token: 0x0400051B RID: 1307
		private TooltipProperty _costProperty;

		// Token: 0x0400051C RID: 1308
		private InventoryLogic _inventoryLogic;

		// Token: 0x0400051D RID: 1309
		private Action<ItemVM, int> _resetComparedItems;

		// Token: 0x0400051E RID: 1310
		private readonly Func<WeaponComponentData, ItemObject.ItemUsageSetFlags> _getItemUsageSetFlags;

		// Token: 0x0400051F RID: 1311
		private readonly Func<EquipmentIndex, SPItemVM> _getEquipmentAtIndex;

		// Token: 0x04000520 RID: 1312
		private bool _isComparing;

		// Token: 0x04000521 RID: 1313
		private bool _isPlayerItem;

		// Token: 0x04000522 RID: 1314
		private ImageIdentifierVM _imageIdentifier;

		// Token: 0x04000523 RID: 1315
		private ImageIdentifierVM _comparedImageIdentifier;

		// Token: 0x04000524 RID: 1316
		private string _itemName;

		// Token: 0x04000525 RID: 1317
		private string _comparedItemName;

		// Token: 0x04000526 RID: 1318
		private MBBindingList<ItemMenuTooltipPropertyVM> _comparedItemProperties;

		// Token: 0x04000527 RID: 1319
		private MBBindingList<ItemMenuTooltipPropertyVM> _targetItemProperties;

		// Token: 0x04000528 RID: 1320
		private bool _isInitializationOver;

		// Token: 0x04000529 RID: 1321
		private int _transactionTotalCost = -1;

		// Token: 0x0400052A RID: 1322
		private MBBindingList<ItemFlagVM> _targetItemFlagList;

		// Token: 0x0400052B RID: 1323
		private MBBindingList<ItemFlagVM> _comparedItemFlagList;

		// Token: 0x0400052C RID: 1324
		private int _alternativeUsageIndex;

		// Token: 0x0400052D RID: 1325
		private MBBindingList<StringItemWithHintVM> _alternativeUsages;

		// Token: 0x0400052E RID: 1326
		private ITradeRumorCampaignBehavior _tradeRumorsBehavior;

		// Token: 0x0400052F RID: 1327
		private ItemVM _targetItem;

		// Token: 0x04000530 RID: 1328
		private ItemVM _comparedItem;

		// Token: 0x04000531 RID: 1329
		private BasicCharacterObject _character;
	}
}
