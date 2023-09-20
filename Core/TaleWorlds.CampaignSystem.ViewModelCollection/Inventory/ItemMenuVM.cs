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
	public class ItemMenuVM : ViewModel
	{
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

		private void AddFoodItemFlags(MBBindingList<ItemFlagVM> list, ItemObject item)
		{
			list.Add(new ItemFlagVM("GoodsFlagIcons\\consumable", GameTexts.FindText("str_inventory_flag_consumable", null)));
		}

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

		private Color GetColorFromBool(bool booleanValue)
		{
			if (!booleanValue)
			{
				return UIColors.NegativeIndicator;
			}
			return UIColors.PositiveIndicator;
		}

		private void SetFoodTooltip()
		{
			this.CreateColoredProperty(this.TargetItemProperties, "", this._foodText.ToString(), this.ConsumableColor, 1, null, TooltipProperty.TooltipPropertyFlags.None);
			this.AddFoodItemFlags(this.TargetItemFlagList, this._targetItem.ItemRosterElement.EquipmentElement.Item);
		}

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

		private float CalculateTradeRumorOldnessFactor(TradeRumor rumor)
		{
			return MathF.Clamp((float)((int)rumor.RumorEndTime.RemainingDaysFromNow) / 5f, 0.5f, 1f);
		}

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

		private ItemMenuTooltipPropertyVM CreateProperty(MBBindingList<ItemMenuTooltipPropertyVM> targetList, string definition, string value, int textHeight = 0, HintViewModel hint = null)
		{
			ItemMenuTooltipPropertyVM itemMenuTooltipPropertyVM = new ItemMenuTooltipPropertyVM(definition, value, textHeight, false, hint);
			targetList.Add(itemMenuTooltipPropertyVM);
			return itemMenuTooltipPropertyVM;
		}

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

		private readonly TextObject _swingDamageText = GameTexts.FindText("str_swing_damage", null);

		private readonly TextObject _swingSpeedText = new TextObject("{=345a87fcc69f626ae3916939ef2fc135}Swing Speed: ", null);

		private readonly TextObject _weaponTierText = new TextObject("{=weaponTier}Weapon Tier: ", null);

		private readonly TextObject _armorTierText = new TextObject("{=armorTier}Armor Tier: ", null);

		private readonly TextObject _horseTierText = new TextObject("{=mountTier}Mount Tier: ", null);

		private readonly TextObject _horseTypeText = new TextObject("{=9sxECG6e}Mount Type: ", null);

		private readonly TextObject _chargeDamageText = new TextObject("{=c7638a0869219ae845de0f660fd57a9d}Charge Damage: ", null);

		private readonly TextObject _hitPointsText = GameTexts.FindText("str_hit_points", null);

		private readonly TextObject _speedText = new TextObject("{=74dc1908cb0b990e80fb977b5a0ef10d}Speed: ", null);

		private readonly TextObject _maneuverText = new TextObject("{=3025020b83b218707499f0de3135ed0a}Maneuver: ", null);

		private readonly TextObject _thrustSpeedText = GameTexts.FindText("str_thrust_speed", null);

		private readonly TextObject _thrustDamageText = GameTexts.FindText("str_thrust_damage", null);

		private readonly TextObject _lengthText = new TextObject("{=c6e4c8588ca9e42f6e1b47b11f0f367b}Length: ", null);

		private readonly TextObject _weightText = GameTexts.FindText("str_weight_text", null);

		private readonly TextObject _handlingText = new TextObject("{=ca8b1e8956057b831dfc665f54bae4b0}Handling: ", null);

		private readonly TextObject _weaponLengthText = new TextObject("{=5fa36d2798479803b4518a64beb4d732}Weapon Length: ", null);

		private readonly TextObject _damageText = new TextObject("{=c9c5dfed2ca6bcb7a73d905004c97b23}Damage: ", null);

		private readonly TextObject _missileSpeedText = GameTexts.FindText("str_missile_speed", null);

		private readonly TextObject _accuracyText = new TextObject("{=5dec16fa0be433ade3c4cb0074ef366d}Accuracy: ", null);

		private readonly TextObject _stackAmountText = new TextObject("{=05fdfc6e238429753ef282f2ce97c1f8}Stack Amount: ", null);

		private readonly TextObject _ammoLimitText = new TextObject("{=6adabc1f82216992571c3e22abc164d7}Ammo Limit: ", null);

		private readonly TextObject _requiresText = new TextObject("{=154a34f8caccfc833238cc89d38861e8}Requires: ", null);

		private readonly TextObject _foodText = new TextObject("{=qSi4DlT4}Food", null);

		private readonly TextObject _partyMoraleText = new TextObject("{=a241aacb1780599430c79fd9f667b67f}Party Morale: ", null);

		private readonly TextObject _typeText = new TextObject("{=08abd5af7774d311cadc3ed900b47754}Type: ", null);

		private readonly TextObject _tradeRumorsText = new TextObject("{=f2971dc587a9777223ad2d7be236fb05}Trade Rumors", null);

		private readonly TextObject _classText = new TextObject("{=8cad4a279770f269c4bb0dc7a357ee1e}Class: ", null);

		private readonly TextObject _headArmorText = GameTexts.FindText("str_head_armor", null);

		private readonly TextObject _horseArmorText = new TextObject("{=305cf7f98458b22e9af72b60a131714f}Horse Armor: ", null);

		private readonly TextObject _bodyArmorText = GameTexts.FindText("str_body_armor", null);

		private readonly TextObject _legArmorText = GameTexts.FindText("str_leg_armor", null);

		private readonly TextObject _armArmorText = new TextObject("{=cf61cce254c7dca65be9bebac7fb9bf5}Arm Armor: ", null);

		private readonly TextObject _bannerEffectText = new TextObject("{=DbXZjPdf}Banner Effect: ", null);

		private readonly TextObject _noneText = new TextObject("{=koX9okuG}None", null);

		private readonly string GoldIcon = "<img src=\"General\\Icons\\Coin@2x\" extend=\"8\"/>";

		private readonly Color ConsumableColor = Color.FromUint(4290873921U);

		private readonly Color TitleColor = Color.FromUint(4293446041U);

		private TooltipProperty _costProperty;

		private InventoryLogic _inventoryLogic;

		private Action<ItemVM, int> _resetComparedItems;

		private readonly Func<WeaponComponentData, ItemObject.ItemUsageSetFlags> _getItemUsageSetFlags;

		private readonly Func<EquipmentIndex, SPItemVM> _getEquipmentAtIndex;

		private bool _isComparing;

		private bool _isPlayerItem;

		private ImageIdentifierVM _imageIdentifier;

		private ImageIdentifierVM _comparedImageIdentifier;

		private string _itemName;

		private string _comparedItemName;

		private MBBindingList<ItemMenuTooltipPropertyVM> _comparedItemProperties;

		private MBBindingList<ItemMenuTooltipPropertyVM> _targetItemProperties;

		private bool _isInitializationOver;

		private int _transactionTotalCost = -1;

		private MBBindingList<ItemFlagVM> _targetItemFlagList;

		private MBBindingList<ItemFlagVM> _comparedItemFlagList;

		private int _alternativeUsageIndex;

		private MBBindingList<StringItemWithHintVM> _alternativeUsages;

		private ITradeRumorCampaignBehavior _tradeRumorsBehavior;

		private ItemVM _targetItem;

		private ItemVM _comparedItem;

		private BasicCharacterObject _character;
	}
}
