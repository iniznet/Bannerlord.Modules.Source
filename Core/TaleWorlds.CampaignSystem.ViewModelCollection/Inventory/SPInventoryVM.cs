using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Inventory;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.ViewModelCollection.Input;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.Core.ViewModelCollection.Tutorial;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ViewModelCollection.Inventory
{
	public class SPInventoryVM : ViewModel, IInventoryStateHandler
	{
		public SPInventoryVM(InventoryLogic inventoryLogic, bool isInCivilianModeByDefault, Func<WeaponComponentData, ItemObject.ItemUsageSetFlags> getItemUsageSetFlags, string fiveStackShortcutkeyText, string entireStackShortcutkeyText)
		{
			this._usageType = InventoryManager.Instance.CurrentMode;
			this._inventoryLogic = inventoryLogic;
			this._viewDataTracker = Campaign.Current.GetCampaignBehavior<IViewDataTracker>();
			this._getItemUsageSetFlags = getItemUsageSetFlags;
			this._fiveStackShortcutkeyText = fiveStackShortcutkeyText;
			this._entireStackShortcutkeyText = entireStackShortcutkeyText;
			this._filters = new Dictionary<SPInventoryVM.Filters, List<int>>();
			this._filters.Add(SPInventoryVM.Filters.All, this._everyItemType);
			this._filters.Add(SPInventoryVM.Filters.Weapons, this._weaponItemTypes);
			this._filters.Add(SPInventoryVM.Filters.Armors, this._armorItemTypes);
			this._filters.Add(SPInventoryVM.Filters.Mounts, this._mountItemTypes);
			this._filters.Add(SPInventoryVM.Filters.ShieldsAndRanged, this._shieldAndRangedItemTypes);
			this._filters.Add(SPInventoryVM.Filters.Miscellaneous, this._miscellaneousItemTypes);
			this._equipAfterTransferStack = new Stack<SPItemVM>();
			this._comparedItemList = new List<ItemVM>();
			this._donationMaxShareableXp = MobilePartyHelper.GetMaximumXpAmountPartyCanGet(MobileParty.MainParty);
			MBTextManager.SetTextVariable("XP_DONATION_LIMIT", this._donationMaxShareableXp);
			if (this._inventoryLogic != null)
			{
				this._currentCharacter = this._inventoryLogic.InitialEquipmentCharacter;
				this._isTrading = inventoryLogic.IsTrading;
				this._inventoryLogic.AfterReset += this.AfterReset;
				InventoryLogic inventoryLogic2 = this._inventoryLogic;
				inventoryLogic2.TotalAmountChange = (Action<int>)Delegate.Combine(inventoryLogic2.TotalAmountChange, new Action<int>(this.OnTotalAmountChange));
				InventoryLogic inventoryLogic3 = this._inventoryLogic;
				inventoryLogic3.DonationXpChange = (Action)Delegate.Combine(inventoryLogic3.DonationXpChange, new Action(this.OnDonationXpChange));
				this._inventoryLogic.AfterTransfer += this.AfterTransfer;
				this._rightTroopRoster = inventoryLogic.RightMemberRoster;
				this._leftTroopRoster = inventoryLogic.LeftMemberRoster;
				this._currentInventoryCharacterIndex = this._rightTroopRoster.FindIndexOfTroop(this._currentCharacter);
				this.OnDonationXpChange();
				this.CompanionExists = this.DoesCompanionExist();
			}
			this.MainCharacter = new HeroViewModel(CharacterViewModel.StanceTypes.None);
			this.MainCharacter.FillFrom(this._currentCharacter.HeroObject, -1, false, false);
			this.ItemMenu = new ItemMenuVM(new Action<ItemVM, int>(this.ResetComparedItems), this._inventoryLogic, this._getItemUsageSetFlags, new Func<EquipmentIndex, SPItemVM>(this.GetItemFromIndex));
			this.IsRefreshed = false;
			this.RightItemListVM = new MBBindingList<SPItemVM>();
			this.LeftItemListVM = new MBBindingList<SPItemVM>();
			this.CharacterHelmSlot = new SPItemVM();
			this.CharacterCloakSlot = new SPItemVM();
			this.CharacterTorsoSlot = new SPItemVM();
			this.CharacterGloveSlot = new SPItemVM();
			this.CharacterBootSlot = new SPItemVM();
			this.CharacterMountSlot = new SPItemVM();
			this.CharacterMountArmorSlot = new SPItemVM();
			this.CharacterWeapon1Slot = new SPItemVM();
			this.CharacterWeapon2Slot = new SPItemVM();
			this.CharacterWeapon3Slot = new SPItemVM();
			this.CharacterWeapon4Slot = new SPItemVM();
			this.CharacterBannerSlot = new SPItemVM();
			this.ProductionTooltip = new BasicTooltipViewModel();
			this.CurrentCharacterSkillsTooltip = new BasicTooltipViewModel(() => CampaignUIHelper.GetInventoryCharacterTooltip(this._currentCharacter.HeroObject));
			this.RefreshCallbacks();
			this._selectedEquipmentIndex = 0;
			this.EquipmentMaxCountHint = new BasicTooltipViewModel();
			this.IsInWarSet = !isInCivilianModeByDefault;
			if (this._inventoryLogic != null)
			{
				this.UpdateRightCharacter();
				this.UpdateLeftCharacter();
				this.InitializeInventory();
			}
			this.RightInventoryOwnerGold = Hero.MainHero.Gold;
			if (this._inventoryLogic.OtherSideCapacityData != null)
			{
				this.OtherSideHasCapacity = this._inventoryLogic.OtherSideCapacityData.GetCapacity() != -1;
			}
			this.IsOtherInventoryGoldRelevant = this._usageType != InventoryMode.Loot;
			this.PlayerInventorySortController = new SPInventorySortControllerVM(ref this._rightItemListVM);
			this.OtherInventorySortController = new SPInventorySortControllerVM(ref this._leftItemListVM);
			this.PlayerInventorySortController.SortByDefaultState();
			if (this._usageType == InventoryMode.Loot)
			{
				this.OtherInventorySortController.CostState = 1;
				this.OtherInventorySortController.ExecuteSortByCost();
			}
			else
			{
				this.OtherInventorySortController.SortByDefaultState();
			}
			Tuple<int, int> tuple = this._viewDataTracker.InventoryGetSortPreference((int)this._usageType);
			if (tuple != null)
			{
				this.PlayerInventorySortController.SortByOption((SPInventorySortControllerVM.InventoryItemSortOption)tuple.Item1, (SPInventorySortControllerVM.InventoryItemSortState)tuple.Item2);
			}
			this.ItemPreview = new ItemPreviewVM(new Action(this.OnPreviewClosed));
			this._characterList = new SelectorVM<InventoryCharacterSelectorItemVM>(0, new Action<SelectorVM<InventoryCharacterSelectorItemVM>>(this.OnCharacterSelected));
			this.AddApplicableCharactersToListFromRoster(this._rightTroopRoster.GetTroopRoster());
			if (this._inventoryLogic.IsOtherPartyFromPlayerClan && this._leftTroopRoster != null)
			{
				this.AddApplicableCharactersToListFromRoster(this._leftTroopRoster.GetTroopRoster());
			}
			if (this._characterList.SelectedIndex == -1 && this._characterList.ItemList.Count > 0)
			{
				this._characterList.SelectedIndex = 0;
			}
			this.BannerTypeCode = 24;
			InventoryTradeVM.RemoveZeroCounts += this.ExecuteRemoveZeroCounts;
			Game.Current.EventManager.RegisterEvent<TutorialNotificationElementChangeEvent>(new Action<TutorialNotificationElementChangeEvent>(this.OnTutorialNotificationElementIDChange));
			this.RefreshValues();
		}

		private void AddApplicableCharactersToListFromRoster(MBList<TroopRosterElement> roster)
		{
			for (int i = 0; i < roster.Count; i++)
			{
				CharacterObject character = roster[i].Character;
				if (character.IsHero && this.CanSelectHero(character.HeroObject))
				{
					this._characterList.AddItem(new InventoryCharacterSelectorItemVM(character.HeroObject.StringId, character.HeroObject, character.HeroObject.Name));
					if (character == this._currentCharacter)
					{
						this._characterList.SelectedIndex = this._characterList.ItemList.Count - 1;
					}
				}
			}
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.RightInventoryOwnerName = PartyBase.MainParty.Name.ToString();
			this.DoneLbl = GameTexts.FindText("str_done", null).ToString();
			this.CancelLbl = GameTexts.FindText("str_cancel", null).ToString();
			this.ResetLbl = GameTexts.FindText("str_reset", null).ToString();
			this.TypeText = GameTexts.FindText("str_sort_by_type_label", null).ToString();
			this.NameText = GameTexts.FindText("str_sort_by_name_label", null).ToString();
			this.QuantityText = GameTexts.FindText("str_quantity_sign", null).ToString();
			this.CostText = GameTexts.FindText("str_value", null).ToString();
			this.SearchPlaceholderText = new TextObject("{=tQOPRBFg}Search...", null).ToString();
			this.FilterAllHint = new HintViewModel(GameTexts.FindText("str_inventory_filter_all", null), null);
			this.FilterWeaponHint = new HintViewModel(GameTexts.FindText("str_inventory_filter_weapons", null), null);
			this.FilterArmorHint = new HintViewModel(GameTexts.FindText("str_inventory_filter_armors", null), null);
			this.FilterShieldAndRangedHint = new HintViewModel(GameTexts.FindText("str_inventory_filter_shields_ranged", null), null);
			this.FilterMountAndHarnessHint = new HintViewModel(GameTexts.FindText("str_inventory_filter_mounts", null), null);
			this.FilterMiscHint = new HintViewModel(GameTexts.FindText("str_inventory_filter_other", null), null);
			this.CivilianOutfitHint = new HintViewModel(GameTexts.FindText("str_inventory_civilian_outfit", null), null);
			this.BattleOutfitHint = new HintViewModel(GameTexts.FindText("str_inventory_battle_outfit", null), null);
			this.EquipmentHelmSlotHint = new HintViewModel(GameTexts.FindText("str_inventory_helm_slot", null), null);
			this.EquipmentArmorSlotHint = new HintViewModel(GameTexts.FindText("str_inventory_armor_slot", null), null);
			this.EquipmentBootSlotHint = new HintViewModel(GameTexts.FindText("str_inventory_boot_slot", null), null);
			this.EquipmentCloakSlotHint = new HintViewModel(GameTexts.FindText("str_inventory_cloak_slot", null), null);
			this.EquipmentGloveSlotHint = new HintViewModel(GameTexts.FindText("str_inventory_glove_slot", null), null);
			this.EquipmentHarnessSlotHint = new HintViewModel(GameTexts.FindText("str_inventory_mount_armor_slot", null), null);
			this.EquipmentMountSlotHint = new HintViewModel(GameTexts.FindText("str_inventory_mount_slot", null), null);
			this.EquipmentWeaponSlotHint = new HintViewModel(GameTexts.FindText("str_inventory_filter_weapons", null), null);
			this.EquipmentBannerSlotHint = new HintViewModel(GameTexts.FindText("str_inventory_banner_slot", null), null);
			this.WeightHint = new HintViewModel(GameTexts.FindText("str_inventory_weight_desc", null), null);
			this.ArmArmorHint = new HintViewModel(GameTexts.FindText("str_inventory_arm_armor", null), null);
			this.BodyArmorHint = new HintViewModel(GameTexts.FindText("str_inventory_body_armor", null), null);
			this.HeadArmorHint = new HintViewModel(GameTexts.FindText("str_inventory_head_armor", null), null);
			this.LegArmorHint = new HintViewModel(GameTexts.FindText("str_inventory_leg_armor", null), null);
			this.HorseArmorHint = new HintViewModel(GameTexts.FindText("str_inventory_horse_armor", null), null);
			this.DonationLblHint = new HintViewModel(GameTexts.FindText("str_inventory_donation_label_hint", null), null);
			this.SetPreviousCharacterHint();
			this.SetNextCharacterHint();
			this.PreviewHint = new HintViewModel(GameTexts.FindText("str_inventory_preview", null), null);
			this.EquipHint = new HintViewModel(GameTexts.FindText("str_inventory_equip", null), null);
			this.UnequipHint = new HintViewModel(GameTexts.FindText("str_inventory_unequip", null), null);
			this.ResetHint = new HintViewModel(GameTexts.FindText("str_reset", null), null);
			this.PlayerSideCapacityExceededText = GameTexts.FindText("str_capacity_exceeded", null).ToString();
			this.PlayerSideCapacityExceededHint = new HintViewModel(GameTexts.FindText("str_capacity_exceeded_hint", null), null);
			if (this._inventoryLogic.OtherSideCapacityData != null)
			{
				TextObject capacityExceededWarningText = this._inventoryLogic.OtherSideCapacityData.GetCapacityExceededWarningText();
				this.OtherSideCapacityExceededText = ((capacityExceededWarningText != null) ? capacityExceededWarningText.ToString() : null);
				this.OtherSideCapacityExceededHint = new HintViewModel(this._inventoryLogic.OtherSideCapacityData.GetCapacityExceededHintText(), null);
			}
			this.SetBuyAllHint();
			this.SetSellAllHint();
			if (this._usageType == InventoryMode.Loot || this._usageType == InventoryMode.Stash)
			{
				this.SellHint = new HintViewModel(GameTexts.FindText("str_inventory_give", null), null);
			}
			else if (this._usageType == InventoryMode.Default)
			{
				this.SellHint = new HintViewModel(GameTexts.FindText("str_inventory_discard", null), null);
			}
			else
			{
				this.SellHint = new HintViewModel(GameTexts.FindText("str_inventory_sell", null), null);
			}
			this.CharacterHelmSlot.RefreshValues();
			this.CharacterCloakSlot.RefreshValues();
			this.CharacterTorsoSlot.RefreshValues();
			this.CharacterGloveSlot.RefreshValues();
			this.CharacterBootSlot.RefreshValues();
			this.CharacterMountSlot.RefreshValues();
			this.CharacterMountArmorSlot.RefreshValues();
			this.CharacterWeapon1Slot.RefreshValues();
			this.CharacterWeapon2Slot.RefreshValues();
			this.CharacterWeapon3Slot.RefreshValues();
			this.CharacterWeapon4Slot.RefreshValues();
			this.CharacterBannerSlot.RefreshValues();
			SPInventorySortControllerVM playerInventorySortController = this.PlayerInventorySortController;
			if (playerInventorySortController != null)
			{
				playerInventorySortController.RefreshValues();
			}
			SPInventorySortControllerVM otherInventorySortController = this.OtherInventorySortController;
			if (otherInventorySortController == null)
			{
				return;
			}
			otherInventorySortController.RefreshValues();
		}

		public override void OnFinalize()
		{
			ItemVM.ProcessEquipItem = null;
			ItemVM.ProcessUnequipItem = null;
			ItemVM.ProcessPreviewItem = null;
			ItemVM.ProcessBuyItem = null;
			SPItemVM.ProcessSellItem = null;
			ItemVM.ProcessItemSelect = null;
			ItemVM.ProcessItemTooltip = null;
			SPItemVM.ProcessItemSlaughter = null;
			SPItemVM.ProcessItemDonate = null;
			SPItemVM.OnFocus = null;
			InventoryTradeVM.RemoveZeroCounts -= this.ExecuteRemoveZeroCounts;
			Game.Current.EventManager.UnregisterEvent<TutorialNotificationElementChangeEvent>(new Action<TutorialNotificationElementChangeEvent>(this.OnTutorialNotificationElementIDChange));
			this.ItemPreview.OnFinalize();
			this.ItemPreview = null;
			this.CancelInputKey.OnFinalize();
			this.DoneInputKey.OnFinalize();
			this.ResetInputKey.OnFinalize();
			this.PreviousCharacterInputKey.OnFinalize();
			this.NextCharacterInputKey.OnFinalize();
			this.BuyAllInputKey.OnFinalize();
			this.SellAllInputKey.OnFinalize();
			ItemVM.ProcessEquipItem = null;
			ItemVM.ProcessUnequipItem = null;
			ItemVM.ProcessPreviewItem = null;
			ItemVM.ProcessBuyItem = null;
			SPItemVM.ProcessLockItem = null;
			SPItemVM.ProcessSellItem = null;
			ItemVM.ProcessItemSelect = null;
			ItemVM.ProcessItemTooltip = null;
			SPItemVM.ProcessItemSlaughter = null;
			SPItemVM.ProcessItemDonate = null;
			SPItemVM.OnFocus = null;
			this.MainCharacter.OnFinalize();
			this._isFinalized = true;
			this._inventoryLogic = null;
			base.OnFinalize();
		}

		public void RefreshCallbacks()
		{
			ItemVM.ProcessEquipItem = new Action<ItemVM>(this.ProcessEquipItem);
			ItemVM.ProcessUnequipItem = new Action<ItemVM>(this.ProcessUnequipItem);
			ItemVM.ProcessPreviewItem = new Action<ItemVM>(this.ProcessPreviewItem);
			ItemVM.ProcessBuyItem = new Action<ItemVM, bool>(this.ProcessBuyItem);
			SPItemVM.ProcessLockItem = new Action<SPItemVM, bool>(this.ProcessLockItem);
			SPItemVM.ProcessSellItem = new Action<SPItemVM, bool>(this.ProcessSellItem);
			ItemVM.ProcessItemSelect = new Action<ItemVM>(this.ProcessItemSelect);
			ItemVM.ProcessItemTooltip = new Action<ItemVM>(this.ProcessItemTooltip);
			SPItemVM.ProcessItemSlaughter = new Action<SPItemVM>(this.ProcessItemSlaughter);
			SPItemVM.ProcessItemDonate = new Action<SPItemVM>(this.ProcessItemDonate);
			SPItemVM.OnFocus = new Action<SPItemVM>(this.OnItemFocus);
		}

		private bool CanSelectHero(Hero hero)
		{
			return hero.IsAlive && hero.CanHeroEquipmentBeChanged() && hero.Clan == Clan.PlayerClan && hero.HeroState != Hero.CharacterStates.Disabled && !hero.IsChild;
		}

		private void SetPreviousCharacterHint()
		{
			this.PreviousCharacterHint = new BasicTooltipViewModel(delegate
			{
				GameTexts.SetVariable("HOTKEY", this.GetPreviousCharacterKeyText());
				GameTexts.SetVariable("TEXT", GameTexts.FindText("str_inventory_prev_char", null));
				return GameTexts.FindText("str_hotkey_with_hint", null).ToString();
			});
		}

		private void SetNextCharacterHint()
		{
			this.NextCharacterHint = new BasicTooltipViewModel(delegate
			{
				GameTexts.SetVariable("HOTKEY", this.GetNextCharacterKeyText());
				GameTexts.SetVariable("TEXT", GameTexts.FindText("str_inventory_next_char", null));
				return GameTexts.FindText("str_hotkey_with_hint", null).ToString();
			});
		}

		private void SetBuyAllHint()
		{
			TextObject buyAllHintText;
			if (this._usageType == InventoryMode.Trade)
			{
				buyAllHintText = GameTexts.FindText("str_inventory_buy_all", null);
			}
			else
			{
				buyAllHintText = GameTexts.FindText("str_inventory_take_all", null);
			}
			this.BuyAllHint = new BasicTooltipViewModel(delegate
			{
				GameTexts.SetVariable("HOTKEY", this.GetBuyAllKeyText());
				GameTexts.SetVariable("TEXT", buyAllHintText);
				return GameTexts.FindText("str_hotkey_with_hint", null).ToString();
			});
		}

		private void SetSellAllHint()
		{
			TextObject sellAllHintText;
			if (this._usageType == InventoryMode.Loot || this._usageType == InventoryMode.Stash)
			{
				sellAllHintText = GameTexts.FindText("str_inventory_give_all", null);
			}
			else if (this._usageType == InventoryMode.Default)
			{
				sellAllHintText = GameTexts.FindText("str_inventory_discard_all", null);
			}
			else
			{
				sellAllHintText = GameTexts.FindText("str_inventory_sell_all", null);
			}
			this.SellAllHint = new BasicTooltipViewModel(delegate
			{
				GameTexts.SetVariable("HOTKEY", this.GetSellAllKeyText());
				GameTexts.SetVariable("TEXT", sellAllHintText);
				return GameTexts.FindText("str_hotkey_with_hint", null).ToString();
			});
		}

		private void OnCharacterSelected(SelectorVM<InventoryCharacterSelectorItemVM> selector)
		{
			if (this._inventoryLogic == null || selector.SelectedItem == null)
			{
				return;
			}
			for (int i = 0; i < this._rightTroopRoster.Count; i++)
			{
				if (this._rightTroopRoster.GetCharacterAtIndex(i).StringId == selector.SelectedItem.CharacterID)
				{
					this.UpdateCurrentCharacterIfPossible(i, true);
					return;
				}
			}
			if (this._leftTroopRoster != null)
			{
				for (int j = 0; j < this._leftTroopRoster.Count; j++)
				{
					if (this._leftTroopRoster.GetCharacterAtIndex(j).StringId == selector.SelectedItem.CharacterID)
					{
						this.UpdateCurrentCharacterIfPossible(j, false);
						return;
					}
				}
			}
		}

		private Equipment ActiveEquipment
		{
			get
			{
				if (!this.IsInWarSet)
				{
					return this._currentCharacter.FirstCivilianEquipment;
				}
				return this._currentCharacter.FirstBattleEquipment;
			}
		}

		public void ExecuteShowRecap()
		{
			InformationManager.ShowTooltip(typeof(InventoryLogic), new object[] { this._inventoryLogic });
		}

		public void ExecuteCancelRecap()
		{
			MBInformationManager.HideInformations();
		}

		public void ExecuteRemoveZeroCounts()
		{
			List<SPItemVM> list = this.LeftItemListVM.ToList<SPItemVM>();
			for (int i = list.Count - 1; i >= 0; i--)
			{
				if (list[i].ItemCount == 0 && i >= 0 && i < this.LeftItemListVM.Count)
				{
					this.LeftItemListVM.RemoveAt(i);
				}
			}
			List<SPItemVM> list2 = this.RightItemListVM.ToList<SPItemVM>();
			for (int j = list2.Count - 1; j >= 0; j--)
			{
				if (list2[j].ItemCount == 0 && j >= 0 && j < this.RightItemListVM.Count)
				{
					this.RightItemListVM.RemoveAt(j);
				}
			}
		}

		private void ProcessPreviewItem(ItemVM item)
		{
			this._inventoryLogic.IsPreviewingItem = true;
			this.ItemPreview.Open(item.ItemRosterElement.EquipmentElement);
		}

		public void ClosePreview()
		{
			this.ItemPreview.Close();
		}

		private void OnPreviewClosed()
		{
			this._inventoryLogic.IsPreviewingItem = false;
		}

		private void ProcessEquipItem(ItemVM draggedItem)
		{
			SPItemVM spitemVM = draggedItem as SPItemVM;
			if (!spitemVM.IsCivilianItem && !this.IsInWarSet)
			{
				return;
			}
			this.IsRefreshed = false;
			this.EquipEquipment(spitemVM);
			this.RefreshInformationValues();
			this.ExecuteRemoveZeroCounts();
			this.IsRefreshed = true;
		}

		private void ProcessUnequipItem(ItemVM draggedItem)
		{
			this.IsRefreshed = false;
			this.UnequipEquipment(draggedItem as SPItemVM);
			this.RefreshInformationValues();
			this.IsRefreshed = true;
		}

		private void ProcessBuyItem(ItemVM itemBase, bool cameFromTradeData)
		{
			this.IsRefreshed = false;
			MBTextManager.SetTextVariable("ITEM_DESCRIPTION", itemBase.ItemDescription, false);
			MBTextManager.SetTextVariable("ITEM_COST", itemBase.ItemCost);
			SPItemVM spitemVM = itemBase as SPItemVM;
			if (this.IsEntireStackModifierActive && !cameFromTradeData)
			{
				ItemRosterElement? itemRosterElement;
				this.TransactionCount = ((this._inventoryLogic.FindItemFromSide(InventoryLogic.InventorySide.OtherInventory, spitemVM.ItemRosterElement.EquipmentElement) != null) ? itemRosterElement.GetValueOrDefault().Amount : 0);
			}
			else if (this.IsFiveStackModifierActive && !cameFromTradeData)
			{
				this.TransactionCount = 5;
			}
			else
			{
				this.TransactionCount = ((spitemVM != null) ? spitemVM.TransactionCount : 0);
			}
			this.BuyItem(spitemVM);
			if (!cameFromTradeData)
			{
				this.ExecuteRemoveZeroCounts();
			}
			this.RefreshInformationValues();
			this.IsRefreshed = true;
		}

		private void ProcessSellItem(SPItemVM item, bool cameFromTradeData)
		{
			this.IsRefreshed = false;
			MBTextManager.SetTextVariable("ITEM_DESCRIPTION", item.ItemDescription, false);
			MBTextManager.SetTextVariable("ITEM_COST", item.ItemCost);
			if (this.IsEntireStackModifierActive && !cameFromTradeData)
			{
				ItemRosterElement? itemRosterElement = this._inventoryLogic.FindItemFromSide(InventoryLogic.InventorySide.PlayerInventory, item.ItemRosterElement.EquipmentElement);
				this.TransactionCount = ((itemRosterElement != null) ? itemRosterElement.GetValueOrDefault().Amount : 0);
			}
			else if (this.IsFiveStackModifierActive && !cameFromTradeData)
			{
				this.TransactionCount = 5;
			}
			else
			{
				this.TransactionCount = item.TransactionCount;
			}
			this.SellItem(item);
			if (!cameFromTradeData)
			{
				this.ExecuteRemoveZeroCounts();
			}
			this.RefreshInformationValues();
			this.IsRefreshed = true;
		}

		private void ProcessLockItem(SPItemVM item, bool isLocked)
		{
			if (isLocked && item.InventorySide == InventoryLogic.InventorySide.PlayerInventory && !this._lockedItemIDs.Contains(item.StringId))
			{
				this._lockedItemIDs.Add(item.StringId);
				return;
			}
			if (!isLocked && item.InventorySide == InventoryLogic.InventorySide.PlayerInventory && this._lockedItemIDs.Contains(item.StringId))
			{
				this._lockedItemIDs.Remove(item.StringId);
			}
		}

		private ItemVM ProcessCompareItem(ItemVM item, int alternativeUsageIndex = 0)
		{
			this._selectedEquipmentIndex = 0;
			this._comparedItemList.Clear();
			ItemVM itemVM = null;
			bool flag = false;
			EquipmentIndex equipmentIndex = EquipmentIndex.None;
			SPItemVM spitemVM = null;
			bool flag2 = item.ItemType >= EquipmentIndex.WeaponItemBeginSlot && item.ItemType < EquipmentIndex.ExtraWeaponSlot;
			if (((SPItemVM)item).InventorySide != InventoryLogic.InventorySide.Equipment)
			{
				if (flag2)
				{
					for (EquipmentIndex equipmentIndex2 = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex2 < EquipmentIndex.ExtraWeaponSlot; equipmentIndex2++)
					{
						EquipmentIndex equipmentIndex3 = equipmentIndex2;
						SPItemVM itemFromIndex = this.GetItemFromIndex(equipmentIndex3);
						if (itemFromIndex != null && itemFromIndex.ItemRosterElement.EquipmentElement.Item != null && ItemHelper.CheckComparability(item.ItemRosterElement.EquipmentElement.Item, itemFromIndex.ItemRosterElement.EquipmentElement.Item, alternativeUsageIndex))
						{
							this._comparedItemList.Add(itemFromIndex);
						}
					}
					if (!this._comparedItemList.IsEmpty<ItemVM>())
					{
						this.SortComparedItems(item);
						itemVM = this._comparedItemList[0];
						this._lastComparedItemIndex = 0;
					}
					if (itemVM != null)
					{
						equipmentIndex = itemVM.ItemType;
					}
				}
				else
				{
					equipmentIndex = item.ItemType;
				}
			}
			if (item.ItemType >= EquipmentIndex.WeaponItemBeginSlot && item.ItemType < EquipmentIndex.NumEquipmentSetSlots)
			{
				spitemVM = ((equipmentIndex != EquipmentIndex.None) ? this.GetItemFromIndex(equipmentIndex) : null);
				flag = spitemVM != null && !string.IsNullOrEmpty(spitemVM.StringId) && item.StringId != spitemVM.StringId;
			}
			if (!this._selectedTooltipItemStringID.Equals(item.StringId) || (flag && !this._comparedTooltipItemStringID.Equals(spitemVM.StringId)))
			{
				this._selectedTooltipItemStringID = item.StringId;
				if (flag)
				{
					this._comparedTooltipItemStringID = spitemVM.StringId;
				}
			}
			this._selectedEquipmentIndex = (int)equipmentIndex;
			if (spitemVM == null || spitemVM.ItemRosterElement.IsEmpty)
			{
				return null;
			}
			return spitemVM;
		}

		private void ResetComparedItems(ItemVM item, int alternativeUsageIndex)
		{
			ItemVM itemVM = this.ProcessCompareItem(item, alternativeUsageIndex);
			this.ItemMenu.SetItem(this._selectedItem, itemVM, this._currentCharacter, alternativeUsageIndex);
		}

		private void SortComparedItems(ItemVM selectedItem)
		{
			List<ItemVM> list = new List<ItemVM>();
			for (int i = 0; i < this._comparedItemList.Count; i++)
			{
				if (selectedItem.StringId == this._comparedItemList[i].StringId && !list.Contains(this._comparedItemList[i]))
				{
					list.Add(this._comparedItemList[i]);
				}
			}
			for (int j = 0; j < this._comparedItemList.Count; j++)
			{
				if (this._comparedItemList[j].ItemRosterElement.EquipmentElement.Item.Type == selectedItem.ItemRosterElement.EquipmentElement.Item.Type && !list.Contains(this._comparedItemList[j]))
				{
					list.Add(this._comparedItemList[j]);
				}
			}
			for (int k = 0; k < this._comparedItemList.Count; k++)
			{
				WeaponComponent weaponComponent = this._comparedItemList[k].ItemRosterElement.EquipmentElement.Item.WeaponComponent;
				WeaponComponent weaponComponent2 = selectedItem.ItemRosterElement.EquipmentElement.Item.WeaponComponent;
				if (((weaponComponent2.Weapons.Count > 1 && weaponComponent2.Weapons[1].WeaponClass == weaponComponent.Weapons[0].WeaponClass) || (weaponComponent.Weapons.Count > 1 && weaponComponent.Weapons[1].WeaponClass == weaponComponent2.Weapons[0].WeaponClass) || (weaponComponent2.Weapons.Count > 1 && weaponComponent.Weapons.Count > 1 && weaponComponent2.Weapons[1].WeaponClass == weaponComponent.Weapons[1].WeaponClass)) && !list.Contains(this._comparedItemList[k]))
				{
					list.Add(this._comparedItemList[k]);
				}
			}
			if (this._comparedItemList.Count != list.Count)
			{
				foreach (ItemVM itemVM in this._comparedItemList)
				{
					if (!list.Contains(itemVM))
					{
						list.Add(itemVM);
					}
				}
			}
			this._comparedItemList = list;
		}

		public void ProcessItemTooltip(ItemVM item)
		{
			if (item == null || string.IsNullOrEmpty(item.StringId))
			{
				return;
			}
			this._selectedItem = item as SPItemVM;
			ItemVM itemVM = this.ProcessCompareItem(item, 0);
			this.ItemMenu.SetItem(this._selectedItem, itemVM, this._currentCharacter, 0);
			this.RefreshTransactionCost(1);
			this._selectedItem.UpdateCanBeSlaughtered();
		}

		public void ResetSelectedItem()
		{
			this._selectedItem = null;
		}

		private void ProcessItemSlaughter(SPItemVM item)
		{
			this.IsRefreshed = false;
			if (string.IsNullOrEmpty(item.StringId) || !item.CanBeSlaughtered)
			{
				return;
			}
			this.SlaughterItem(item);
			this.RefreshInformationValues();
			if (item.ItemCount == 0)
			{
				this.ExecuteRemoveZeroCounts();
			}
			this.IsRefreshed = true;
		}

		private void ProcessItemDonate(SPItemVM item)
		{
			this.IsRefreshed = false;
			if (string.IsNullOrEmpty(item.StringId) || !item.CanBeDonated)
			{
				return;
			}
			this.DonateItem(item);
			this.RefreshInformationValues();
			if (item.ItemCount == 0)
			{
				this.ExecuteRemoveZeroCounts();
			}
			this.IsRefreshed = true;
		}

		private void OnItemFocus(SPItemVM item)
		{
			this.CurrentFocusedItem = item;
		}

		private void ProcessItemSelect(ItemVM item)
		{
			this.ExecuteRemoveZeroCounts();
		}

		private void RefreshTransactionCost(int transactionCount = 1)
		{
			if (this._selectedItem != null && this.IsTrading)
			{
				int num;
				int itemTotalPrice = this._inventoryLogic.GetItemTotalPrice(this._selectedItem.ItemRosterElement, transactionCount, out num, this._selectedItem.InventorySide == InventoryLogic.InventorySide.OtherInventory);
				this.ItemMenu.SetTransactionCost(itemTotalPrice, num);
			}
		}

		public void RefreshComparedItem()
		{
			this._lastComparedItemIndex++;
			if (this._lastComparedItemIndex > this._comparedItemList.Count - 1)
			{
				this._lastComparedItemIndex = 0;
			}
			if (!this._comparedItemList.IsEmpty<ItemVM>() && this._selectedItem != null && this._comparedItemList[this._lastComparedItemIndex] != null)
			{
				this.ItemMenu.SetItem(this._selectedItem, this._comparedItemList[this._lastComparedItemIndex], this._currentCharacter, 0);
			}
		}

		private void AfterReset(InventoryLogic itemRoster, bool fromCancel)
		{
			this._inventoryLogic = itemRoster;
			if (!fromCancel)
			{
				switch (this.ActiveFilterIndex)
				{
				case 1:
					this._inventoryLogic.MerchantItemType = InventoryManager.InventoryCategoryType.Weapon;
					break;
				case 2:
					this._inventoryLogic.MerchantItemType = InventoryManager.InventoryCategoryType.Shield;
					break;
				case 3:
					this._inventoryLogic.MerchantItemType = InventoryManager.InventoryCategoryType.Armors;
					break;
				case 4:
					this._inventoryLogic.MerchantItemType = InventoryManager.InventoryCategoryType.HorseCategory;
					break;
				case 5:
					this._inventoryLogic.MerchantItemType = InventoryManager.InventoryCategoryType.Goods;
					break;
				default:
					this._inventoryLogic.MerchantItemType = InventoryManager.InventoryCategoryType.All;
					break;
				}
				this.InitializeInventory();
				this.PlayerInventorySortController = new SPInventorySortControllerVM(ref this._rightItemListVM);
				this.OtherInventorySortController = new SPInventorySortControllerVM(ref this._leftItemListVM);
				this.PlayerInventorySortController.SortByDefaultState();
				this.OtherInventorySortController.SortByDefaultState();
				Tuple<int, int> tuple = this._viewDataTracker.InventoryGetSortPreference((int)this._usageType);
				if (tuple != null)
				{
					this.PlayerInventorySortController.SortByOption((SPInventorySortControllerVM.InventoryItemSortOption)tuple.Item1, (SPInventorySortControllerVM.InventoryItemSortState)tuple.Item2);
				}
				this.UpdateRightCharacter();
				this.UpdateLeftCharacter();
				this.RightInventoryOwnerName = PartyBase.MainParty.Name.ToString();
				this.RightInventoryOwnerGold = Hero.MainHero.Gold;
			}
		}

		private void OnTotalAmountChange(int newTotalAmount)
		{
			MBTextManager.SetTextVariable("PAY_OR_GET", (this._inventoryLogic.TotalAmount < 0) ? 1 : 0);
			int num = MathF.Min(-this._inventoryLogic.TotalAmount, this._inventoryLogic.InventoryListener.GetGold());
			MBTextManager.SetTextVariable("TRADE_AMOUNT", MathF.Abs(num));
			this.TradeLbl = ((this._inventoryLogic.TotalAmount == 0) ? "" : GameTexts.FindText("str_inventory_trade_label", null).ToString());
			this.RightInventoryOwnerGold = Hero.MainHero.Gold - this._inventoryLogic.TotalAmount;
			InventoryListener inventoryListener = this._inventoryLogic.InventoryListener;
			this.LeftInventoryOwnerGold = (((inventoryListener != null) ? new int?(inventoryListener.GetGold()) : null) + this._inventoryLogic.TotalAmount) ?? 0;
		}

		private void OnDonationXpChange()
		{
			int num = (int)this._inventoryLogic.XpGainFromDonations;
			bool flag = false;
			if (num > this._donationMaxShareableXp)
			{
				num = this._donationMaxShareableXp;
				flag = true;
			}
			this.IsDonationXpGainExceedsMax = flag;
			this.HasGainedExperience = num > 0;
			MBTextManager.SetTextVariable("XP_AMOUNT", num);
			this.ExperienceLbl = ((num == 0) ? "" : GameTexts.FindText("str_inventory_donation_label", null).ToString());
		}

		private void AfterTransfer(InventoryLogic inventoryLogic, List<TransferCommandResult> results)
		{
			this._isCharacterEquipmentDirty = false;
			List<SPItemVM> list = new List<SPItemVM>();
			HashSet<ItemCategory> hashSet = new HashSet<ItemCategory>();
			for (int num = 0; num != results.Count; num++)
			{
				TransferCommandResult transferCommandResult = results[num];
				if (transferCommandResult.ResultSide == InventoryLogic.InventorySide.OtherInventory || transferCommandResult.ResultSide == InventoryLogic.InventorySide.PlayerInventory)
				{
					if (transferCommandResult.ResultSide == InventoryLogic.InventorySide.PlayerInventory && transferCommandResult.EffectedItemRosterElement.EquipmentElement.Item != null && !transferCommandResult.EffectedItemRosterElement.EquipmentElement.Item.IsMountable && !transferCommandResult.EffectedItemRosterElement.EquipmentElement.Item.IsAnimal)
					{
						this._equipmentCount += (float)transferCommandResult.EffectedNumber * transferCommandResult.EffectedItemRosterElement.EquipmentElement.GetEquipmentElementWeight();
					}
					bool flag = false;
					MBBindingList<SPItemVM> mbbindingList = ((transferCommandResult.ResultSide == InventoryLogic.InventorySide.OtherInventory) ? this.LeftItemListVM : this.RightItemListVM);
					for (int i = 0; i < mbbindingList.Count; i++)
					{
						SPItemVM spitemVM = mbbindingList[i];
						if (spitemVM != null && spitemVM.ItemRosterElement.EquipmentElement.IsEqualTo(transferCommandResult.EffectedItemRosterElement.EquipmentElement))
						{
							spitemVM.ItemRosterElement.Amount = transferCommandResult.FinalNumber;
							spitemVM.ItemCount = transferCommandResult.FinalNumber;
							spitemVM.ItemCost = this._inventoryLogic.GetItemPrice(spitemVM.ItemRosterElement.EquipmentElement, transferCommandResult.ResultSide == InventoryLogic.InventorySide.OtherInventory);
							list.Add(spitemVM);
							if (!hashSet.Contains(spitemVM.ItemRosterElement.EquipmentElement.Item.GetItemCategory()))
							{
								hashSet.Add(spitemVM.ItemRosterElement.EquipmentElement.Item.GetItemCategory());
							}
							flag = true;
							break;
						}
					}
					if (!flag && transferCommandResult.EffectedNumber > 0 && this._inventoryLogic != null)
					{
						SPItemVM spitemVM2;
						if (transferCommandResult.ResultSide == InventoryLogic.InventorySide.OtherInventory)
						{
							spitemVM2 = new SPItemVM(this._inventoryLogic, this.MainCharacter.IsFemale, this.CanCharacterUseItemBasedOnSkills(transferCommandResult.EffectedItemRosterElement), this._usageType, transferCommandResult.EffectedItemRosterElement, InventoryLogic.InventorySide.OtherInventory, this._fiveStackShortcutkeyText, this._entireStackShortcutkeyText, this._inventoryLogic.GetCostOfItemRosterElement(transferCommandResult.EffectedItemRosterElement, transferCommandResult.ResultSide), null);
						}
						else
						{
							spitemVM2 = new SPItemVM(this._inventoryLogic, this.MainCharacter.IsFemale, this.CanCharacterUseItemBasedOnSkills(transferCommandResult.EffectedItemRosterElement), this._usageType, transferCommandResult.EffectedItemRosterElement, InventoryLogic.InventorySide.PlayerInventory, this._fiveStackShortcutkeyText, this._entireStackShortcutkeyText, this._inventoryLogic.GetCostOfItemRosterElement(transferCommandResult.EffectedItemRosterElement, transferCommandResult.ResultSide), null);
						}
						this.UpdateFilteredStatusOfItem(spitemVM2);
						spitemVM2.ItemCount = transferCommandResult.FinalNumber;
						spitemVM2.IsLocked = spitemVM2.InventorySide == InventoryLogic.InventorySide.PlayerInventory && this._lockedItemIDs.Contains(spitemVM2.StringId);
						spitemVM2.IsNew = true;
						mbbindingList.Add(spitemVM2);
					}
				}
				else if (transferCommandResult.ResultSide == InventoryLogic.InventorySide.Equipment)
				{
					SPItemVM spitemVM3 = null;
					if (transferCommandResult.FinalNumber > 0)
					{
						spitemVM3 = new SPItemVM(this._inventoryLogic, this.MainCharacter.IsFemale, this.CanCharacterUseItemBasedOnSkills(transferCommandResult.EffectedItemRosterElement), this._usageType, transferCommandResult.EffectedItemRosterElement, InventoryLogic.InventorySide.Equipment, this._fiveStackShortcutkeyText, this._entireStackShortcutkeyText, this._inventoryLogic.GetCostOfItemRosterElement(transferCommandResult.EffectedItemRosterElement, transferCommandResult.ResultSide), new EquipmentIndex?(transferCommandResult.EffectedEquipmentIndex));
						spitemVM3.IsNew = true;
					}
					this.UpdateEquipment(transferCommandResult.TransferEquipment, spitemVM3, transferCommandResult.EffectedEquipmentIndex);
					this._isCharacterEquipmentDirty = true;
				}
			}
			SPItemVM selectedItem = this._selectedItem;
			if (selectedItem != null && selectedItem.ItemCount > 1)
			{
				this.ProcessItemTooltip(this._selectedItem);
				this._selectedItem.UpdateCanBeSlaughtered();
			}
			this.CheckEquipAfterTransferStack();
			if (!this.ActiveEquipment[EquipmentIndex.HorseHarness].IsEmpty && this.ActiveEquipment[EquipmentIndex.ArmorItemEndSlot].IsEmpty)
			{
				this.UnequipEquipment(this.CharacterMountArmorSlot);
			}
			if (!this.ActiveEquipment[EquipmentIndex.ArmorItemEndSlot].IsEmpty && !this.ActiveEquipment[EquipmentIndex.HorseHarness].IsEmpty && this.ActiveEquipment[EquipmentIndex.ArmorItemEndSlot].Item.HorseComponent.Monster.FamilyType != this.ActiveEquipment[EquipmentIndex.HorseHarness].Item.ArmorComponent.FamilyType)
			{
				this.UnequipEquipment(this.CharacterMountArmorSlot);
			}
			foreach (SPItemVM spitemVM4 in list)
			{
				spitemVM4.UpdateTradeData(true);
				spitemVM4.UpdateCanBeSlaughtered();
			}
			this.UpdateCostOfItemsInCategory(hashSet);
			if (PartyBase.MainParty.IsMobile)
			{
				PartyBase.MainParty.MobileParty.MemberRoster.UpdateVersion();
				PartyBase.MainParty.MobileParty.PrisonRoster.UpdateVersion();
			}
		}

		private void UpdateCostOfItemsInCategory(HashSet<ItemCategory> categories)
		{
			foreach (SPItemVM spitemVM in this.LeftItemListVM)
			{
				if (categories.Contains(spitemVM.ItemRosterElement.EquipmentElement.Item.GetItemCategory()))
				{
					spitemVM.ItemCost = this._inventoryLogic.GetCostOfItemRosterElement(spitemVM.ItemRosterElement, InventoryLogic.InventorySide.OtherInventory);
				}
			}
			foreach (SPItemVM spitemVM2 in this.RightItemListVM)
			{
				if (categories.Contains(spitemVM2.ItemRosterElement.EquipmentElement.Item.GetItemCategory()))
				{
					spitemVM2.ItemCost = this._inventoryLogic.GetCostOfItemRosterElement(spitemVM2.ItemRosterElement, InventoryLogic.InventorySide.PlayerInventory);
				}
			}
		}

		private void CheckEquipAfterTransferStack()
		{
			while (this._equipAfterTransferStack.Count > 0)
			{
				SPItemVM spitemVM = new SPItemVM();
				spitemVM.RefreshWith(this._equipAfterTransferStack.Pop(), InventoryLogic.InventorySide.PlayerInventory);
				this.EquipEquipment(spitemVM);
			}
		}

		private void RefreshInformationValues()
		{
			TextObject textObject = GameTexts.FindText("str_LEFT_over_RIGHT", null);
			int inventoryCapacity = PartyBase.MainParty.InventoryCapacity;
			int num = MathF.Ceiling(this._equipmentCount);
			textObject.SetTextVariable("LEFT", num.ToString());
			textObject.SetTextVariable("RIGHT", inventoryCapacity.ToString());
			this.PlayerEquipmentCountText = textObject.ToString();
			this.PlayerEquipmentCountWarned = num > inventoryCapacity;
			if (this.OtherSideHasCapacity)
			{
				int num2 = MathF.Ceiling(this.LeftItemListVM.Sum((SPItemVM x) => x.ItemRosterElement.GetRosterElementWeight()));
				int capacity = this._inventoryLogic.OtherSideCapacityData.GetCapacity();
				textObject.SetTextVariable("LEFT", num2);
				textObject.SetTextVariable("RIGHT", capacity);
				this.OtherEquipmentCountText = textObject.ToString();
				this.OtherEquipmentCountWarned = num2 > capacity;
			}
			this.NoSaddleText = new TextObject("{=QSPrSsHv}No Saddle!", null).ToString();
			this.NoSaddleHint = new HintViewModel(new TextObject("{=VzCoqt8D}No sadle equipped. -10% penalty to mounted speed and maneuver.", null), null);
			SPItemVM characterMountSlot = this.CharacterMountSlot;
			bool flag;
			if (characterMountSlot != null && !characterMountSlot.ItemRosterElement.IsEmpty)
			{
				SPItemVM characterMountArmorSlot = this.CharacterMountArmorSlot;
				flag = characterMountArmorSlot != null && characterMountArmorSlot.ItemRosterElement.IsEmpty;
			}
			else
			{
				flag = false;
			}
			this.NoSaddleWarned = flag;
			if (Campaign.Current.GameMode == CampaignGameMode.Campaign)
			{
				this.EquipmentMaxCountHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetPartyInventoryCapacityTooltip(MobileParty.MainParty));
			}
			if (this._isCharacterEquipmentDirty)
			{
				this.MainCharacter.SetEquipment(this.ActiveEquipment);
				this.UpdateCharacterArmorValues();
				this.RefreshCharacterTotalWeight();
			}
			this._isCharacterEquipmentDirty = false;
			this.UpdateIsDoneDisabled();
		}

		public bool IsItemEquipmentPossible(SPItemVM itemVM)
		{
			if (itemVM == null)
			{
				return false;
			}
			if (this.TargetEquipmentType == EquipmentIndex.None)
			{
				this.TargetEquipmentType = itemVM.GetItemTypeWithItemObject();
				if (this.TargetEquipmentType == EquipmentIndex.None)
				{
					return false;
				}
				if (this.TargetEquipmentType == EquipmentIndex.WeaponItemBeginSlot)
				{
					EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot;
					bool flag = false;
					bool flag2 = false;
					SPItemVM[] array = new SPItemVM[] { this.CharacterWeapon1Slot, this.CharacterWeapon2Slot, this.CharacterWeapon3Slot, this.CharacterWeapon4Slot };
					for (int i = 0; i < array.Length; i++)
					{
						if (string.IsNullOrEmpty(array[i].StringId))
						{
							flag = true;
							equipmentIndex = EquipmentIndex.WeaponItemBeginSlot + i;
							break;
						}
						if (array[i].ItemRosterElement.EquipmentElement.Item.Type == itemVM.ItemRosterElement.EquipmentElement.Item.Type)
						{
							flag2 = true;
							equipmentIndex = EquipmentIndex.WeaponItemBeginSlot + i;
							break;
						}
					}
					if (flag || flag2)
					{
						this.TargetEquipmentType = equipmentIndex;
					}
					else
					{
						this.TargetEquipmentType = EquipmentIndex.WeaponItemBeginSlot;
					}
				}
			}
			else if (itemVM.ItemType != this.TargetEquipmentType && (this.TargetEquipmentType < EquipmentIndex.WeaponItemBeginSlot || this.TargetEquipmentType > EquipmentIndex.Weapon3 || itemVM.ItemType < EquipmentIndex.WeaponItemBeginSlot || itemVM.ItemType > EquipmentIndex.Weapon3))
			{
				return false;
			}
			if (!this.CanCharacterUseItemBasedOnSkills(itemVM.ItemRosterElement))
			{
				TextObject textObject = new TextObject("{=rgqA29b8}You don't have enough {SKILL_NAME} skill to equip this item", null);
				textObject.SetTextVariable("SKILL_NAME", itemVM.ItemRosterElement.EquipmentElement.Item.RelevantSkill.Name);
				MBInformationManager.AddQuickInformation(textObject, 0, null, "");
				return false;
			}
			if (!this.CanCharacterUserItemBasedOnUsability(itemVM.ItemRosterElement))
			{
				TextObject textObject2 = new TextObject("{=ITKb4cKv}{ITEM_NAME} is not equippable.", null);
				textObject2.SetTextVariable("ITEM_NAME", itemVM.ItemRosterElement.EquipmentElement.GetModifiedItemName());
				MBInformationManager.AddQuickInformation(textObject2, 0, null, "");
				return false;
			}
			if (!Equipment.IsItemFitsToSlot((EquipmentIndex)this.TargetEquipmentIndex, itemVM.ItemRosterElement.EquipmentElement.Item))
			{
				TextObject textObject3 = new TextObject("{=Omjlnsk3}{ITEM_NAME} cannot be equipped on this slot.", null);
				textObject3.SetTextVariable("ITEM_NAME", itemVM.ItemRosterElement.EquipmentElement.GetModifiedItemName());
				MBInformationManager.AddQuickInformation(textObject3, 0, null, "");
				return false;
			}
			if (this.TargetEquipmentType == EquipmentIndex.HorseHarness)
			{
				if (string.IsNullOrEmpty(this.CharacterMountSlot.StringId))
				{
					return false;
				}
				if (!this.ActiveEquipment[EquipmentIndex.ArmorItemEndSlot].IsEmpty && this.ActiveEquipment[EquipmentIndex.ArmorItemEndSlot].Item.HorseComponent.Monster.FamilyType != itemVM.ItemRosterElement.EquipmentElement.Item.ArmorComponent.FamilyType)
				{
					return false;
				}
			}
			return true;
		}

		private bool CanCharacterUserItemBasedOnUsability(ItemRosterElement itemRosterElement)
		{
			return !itemRosterElement.EquipmentElement.Item.HasHorseComponent || itemRosterElement.EquipmentElement.Item.HorseComponent.IsRideable;
		}

		private bool CanCharacterUseItemBasedOnSkills(ItemRosterElement itemRosterElement)
		{
			return CharacterHelper.CanUseItemBasedOnSkill(this._currentCharacter, itemRosterElement.EquipmentElement);
		}

		private void EquipEquipment(SPItemVM itemVM)
		{
			if (itemVM == null || string.IsNullOrEmpty(itemVM.StringId))
			{
				return;
			}
			SPItemVM spitemVM = new SPItemVM();
			spitemVM.RefreshWith(itemVM, InventoryLogic.InventorySide.Equipment);
			if (!this.IsItemEquipmentPossible(spitemVM))
			{
				return;
			}
			SPItemVM itemFromIndex = this.GetItemFromIndex(this.TargetEquipmentType);
			if (itemFromIndex != null && itemFromIndex.ItemRosterElement.EquipmentElement.Item == spitemVM.ItemRosterElement.EquipmentElement.Item && itemFromIndex.ItemRosterElement.EquipmentElement.ItemModifier == spitemVM.ItemRosterElement.EquipmentElement.ItemModifier)
			{
				return;
			}
			bool flag = itemFromIndex != null && itemFromIndex.ItemType != EquipmentIndex.None && itemVM.InventorySide == InventoryLogic.InventorySide.Equipment;
			if (!flag)
			{
				EquipmentIndex equipmentIndex = EquipmentIndex.None;
				if (itemVM.ItemRosterElement.EquipmentElement.Item.Type == ItemObject.ItemTypeEnum.Shield && itemVM.InventorySide != InventoryLogic.InventorySide.Equipment)
				{
					for (EquipmentIndex equipmentIndex2 = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex2 <= EquipmentIndex.NumAllWeaponSlots; equipmentIndex2++)
					{
						SPItemVM itemFromIndex2 = this.GetItemFromIndex(equipmentIndex2);
						bool flag2;
						if (itemFromIndex2 == null)
						{
							flag2 = false;
						}
						else
						{
							ItemObject item = itemFromIndex2.ItemRosterElement.EquipmentElement.Item;
							ItemObject.ItemTypeEnum? itemTypeEnum = ((item != null) ? new ItemObject.ItemTypeEnum?(item.Type) : null);
							ItemObject.ItemTypeEnum itemTypeEnum2 = ItemObject.ItemTypeEnum.Shield;
							flag2 = (itemTypeEnum.GetValueOrDefault() == itemTypeEnum2) & (itemTypeEnum != null);
						}
						if (flag2)
						{
							equipmentIndex = equipmentIndex2;
							break;
						}
					}
				}
				if (itemVM != null)
				{
					ItemObject item2 = itemVM.ItemRosterElement.EquipmentElement.Item;
					ItemObject.ItemTypeEnum? itemTypeEnum = ((item2 != null) ? new ItemObject.ItemTypeEnum?(item2.Type) : null);
					ItemObject.ItemTypeEnum itemTypeEnum2 = ItemObject.ItemTypeEnum.Shield;
					if (((itemTypeEnum.GetValueOrDefault() == itemTypeEnum2) & (itemTypeEnum != null)) && equipmentIndex != EquipmentIndex.None)
					{
						this.TargetEquipmentType = equipmentIndex;
					}
				}
			}
			List<TransferCommand> list = new List<TransferCommand>();
			TransferCommand transferCommand = TransferCommand.Transfer(1, itemVM.InventorySide, InventoryLogic.InventorySide.Equipment, spitemVM.ItemRosterElement, spitemVM.ItemType, this.TargetEquipmentType, this._currentCharacter, !this.IsInWarSet);
			list.Add(transferCommand);
			if (flag)
			{
				TransferCommand transferCommand2 = TransferCommand.Transfer(1, InventoryLogic.InventorySide.PlayerInventory, InventoryLogic.InventorySide.Equipment, itemFromIndex.ItemRosterElement, EquipmentIndex.None, spitemVM.ItemType, this._currentCharacter, !this.IsInWarSet);
				list.Add(transferCommand2);
			}
			this._inventoryLogic.AddTransferCommands(list);
		}

		private void UnequipEquipment(SPItemVM itemVM)
		{
			if (itemVM == null || string.IsNullOrEmpty(itemVM.StringId))
			{
				return;
			}
			TransferCommand transferCommand = TransferCommand.Transfer(1, InventoryLogic.InventorySide.Equipment, InventoryLogic.InventorySide.PlayerInventory, itemVM.ItemRosterElement, itemVM.ItemType, itemVM.ItemType, this._currentCharacter, !this.IsInWarSet);
			this._inventoryLogic.AddTransferCommand(transferCommand);
		}

		private void UpdateEquipment(Equipment equipment, SPItemVM itemVM, EquipmentIndex itemType)
		{
			if (this.ActiveEquipment == equipment)
			{
				this.RefreshEquipment(itemVM, itemType);
			}
			equipment[itemType] = ((itemVM == null) ? default(EquipmentElement) : itemVM.ItemRosterElement.EquipmentElement);
		}

		private void UnequipEquipmentWithEquipmentIndex(EquipmentIndex slotType)
		{
			switch (slotType)
			{
			case EquipmentIndex.None:
				return;
			case EquipmentIndex.WeaponItemBeginSlot:
				this.UnequipEquipment(this.CharacterWeapon1Slot);
				return;
			case EquipmentIndex.Weapon1:
				this.UnequipEquipment(this.CharacterWeapon2Slot);
				return;
			case EquipmentIndex.Weapon2:
				this.UnequipEquipment(this.CharacterWeapon3Slot);
				return;
			case EquipmentIndex.Weapon3:
				this.UnequipEquipment(this.CharacterWeapon4Slot);
				return;
			case EquipmentIndex.ExtraWeaponSlot:
				this.UnequipEquipment(this.CharacterBannerSlot);
				return;
			case EquipmentIndex.NumAllWeaponSlots:
				this.UnequipEquipment(this.CharacterHelmSlot);
				return;
			case EquipmentIndex.Body:
				this.UnequipEquipment(this.CharacterTorsoSlot);
				return;
			case EquipmentIndex.Leg:
				this.UnequipEquipment(this.CharacterBootSlot);
				return;
			case EquipmentIndex.Gloves:
				this.UnequipEquipment(this.CharacterGloveSlot);
				return;
			case EquipmentIndex.Cape:
				this.UnequipEquipment(this.CharacterCloakSlot);
				return;
			case EquipmentIndex.ArmorItemEndSlot:
				this.UnequipEquipment(this.CharacterMountSlot);
				if (!string.IsNullOrEmpty(this.CharacterMountArmorSlot.StringId))
				{
					this.UnequipEquipment(this.CharacterMountArmorSlot);
				}
				return;
			case EquipmentIndex.HorseHarness:
				this.UnequipEquipment(this.CharacterMountArmorSlot);
				return;
			default:
				return;
			}
		}

		protected void RefreshEquipment(SPItemVM itemVM, EquipmentIndex itemType)
		{
			switch (itemType)
			{
			case EquipmentIndex.None:
				return;
			case EquipmentIndex.WeaponItemBeginSlot:
				this.CharacterWeapon1Slot.RefreshWith(itemVM, InventoryLogic.InventorySide.Equipment);
				return;
			case EquipmentIndex.Weapon1:
				this.CharacterWeapon2Slot.RefreshWith(itemVM, InventoryLogic.InventorySide.Equipment);
				return;
			case EquipmentIndex.Weapon2:
				this.CharacterWeapon3Slot.RefreshWith(itemVM, InventoryLogic.InventorySide.Equipment);
				return;
			case EquipmentIndex.Weapon3:
				this.CharacterWeapon4Slot.RefreshWith(itemVM, InventoryLogic.InventorySide.Equipment);
				return;
			case EquipmentIndex.ExtraWeaponSlot:
				this.CharacterBannerSlot.RefreshWith(itemVM, InventoryLogic.InventorySide.Equipment);
				return;
			case EquipmentIndex.NumAllWeaponSlots:
				this.CharacterHelmSlot.RefreshWith(itemVM, InventoryLogic.InventorySide.Equipment);
				return;
			case EquipmentIndex.Body:
				this.CharacterTorsoSlot.RefreshWith(itemVM, InventoryLogic.InventorySide.Equipment);
				return;
			case EquipmentIndex.Leg:
				this.CharacterBootSlot.RefreshWith(itemVM, InventoryLogic.InventorySide.Equipment);
				return;
			case EquipmentIndex.Gloves:
				this.CharacterGloveSlot.RefreshWith(itemVM, InventoryLogic.InventorySide.Equipment);
				return;
			case EquipmentIndex.Cape:
				this.CharacterCloakSlot.RefreshWith(itemVM, InventoryLogic.InventorySide.Equipment);
				return;
			case EquipmentIndex.ArmorItemEndSlot:
				this.CharacterMountSlot.RefreshWith(itemVM, InventoryLogic.InventorySide.Equipment);
				return;
			case EquipmentIndex.HorseHarness:
				this.CharacterMountArmorSlot.RefreshWith(itemVM, InventoryLogic.InventorySide.Equipment);
				return;
			default:
				return;
			}
		}

		private bool UpdateCurrentCharacterIfPossible(int characterIndex, bool isFromRightSide)
		{
			CharacterObject character = (isFromRightSide ? this._rightTroopRoster : this._leftTroopRoster).GetElementCopyAtIndex(characterIndex).Character;
			if (character.IsHero)
			{
				if (!character.HeroObject.CanHeroEquipmentBeChanged())
				{
					Hero mainHero = Hero.MainHero;
					bool flag;
					if (mainHero == null)
					{
						flag = false;
					}
					else
					{
						Clan clan = mainHero.Clan;
						bool? flag2 = ((clan != null) ? new bool?(clan.Lords.Contains(character.HeroObject)) : null);
						bool flag3 = true;
						flag = (flag2.GetValueOrDefault() == flag3) & (flag2 != null);
					}
					if (!flag)
					{
						return false;
					}
				}
				this._currentInventoryCharacterIndex = characterIndex;
				this._currentCharacter = character;
				this.MainCharacter.FillFrom(this._currentCharacter.HeroObject, -1, false, false);
				if (this._currentCharacter.IsHero)
				{
					CharacterViewModel mainCharacter = this.MainCharacter;
					IFaction mapFaction = this._currentCharacter.HeroObject.MapFaction;
					mainCharacter.ArmorColor1 = ((mapFaction != null) ? mapFaction.Color : 0U);
					CharacterViewModel mainCharacter2 = this.MainCharacter;
					IFaction mapFaction2 = this._currentCharacter.HeroObject.MapFaction;
					mainCharacter2.ArmorColor2 = ((mapFaction2 != null) ? mapFaction2.Color2 : 0U);
				}
				this.UpdateRightCharacter();
				this.RefreshInformationValues();
				return true;
			}
			return false;
		}

		private bool DoesCompanionExist()
		{
			for (int i = 1; i < this._rightTroopRoster.Count; i++)
			{
				CharacterObject character = this._rightTroopRoster.GetElementCopyAtIndex(i).Character;
				if (character.IsHero && !character.HeroObject.CanHeroEquipmentBeChanged() && character.HeroObject != Hero.MainHero)
				{
					return true;
				}
			}
			return false;
		}

		private void UpdateLeftCharacter()
		{
			this.IsTradingWithSettlement = false;
			if (this._inventoryLogic.LeftRosterName != null)
			{
				this.LeftInventoryOwnerName = this._inventoryLogic.LeftRosterName.ToString();
				Settlement settlement2 = this._currentCharacter.HeroObject.CurrentSettlement;
				if (settlement2 != null && InventoryManager.Instance.CurrentMode == InventoryMode.Warehouse)
				{
					this.IsTradingWithSettlement = true;
					this.ProductionTooltip = new BasicTooltipViewModel(() => CampaignUIHelper.GetSettlementProductionTooltip(settlement2));
					return;
				}
			}
			else
			{
				Settlement settlement = this._currentCharacter.HeroObject.CurrentSettlement;
				if (settlement != null)
				{
					this.LeftInventoryOwnerName = settlement.Name.ToString();
					this.ProductionTooltip = new BasicTooltipViewModel(() => CampaignUIHelper.GetSettlementProductionTooltip(settlement));
					this.IsTradingWithSettlement = !settlement.IsHideout;
					if (this._inventoryLogic.InventoryListener != null)
					{
						this.LeftInventoryOwnerGold = this._inventoryLogic.InventoryListener.GetGold();
						return;
					}
				}
				else
				{
					PartyBase oppositePartyFromListener = this._inventoryLogic.OppositePartyFromListener;
					MobileParty mobileParty = ((oppositePartyFromListener != null) ? oppositePartyFromListener.MobileParty : null);
					if (mobileParty != null && (mobileParty.IsCaravan || mobileParty.IsVillager))
					{
						this.LeftInventoryOwnerName = mobileParty.Name.ToString();
						InventoryListener inventoryListener = this._inventoryLogic.InventoryListener;
						this.LeftInventoryOwnerGold = ((inventoryListener != null) ? inventoryListener.GetGold() : 0);
						return;
					}
					this.LeftInventoryOwnerName = GameTexts.FindText("str_loot", null).ToString();
				}
			}
		}

		private void UpdateRightCharacter()
		{
			this.UpdateCharacterEquipment();
			this.UpdateCharacterArmorValues();
			this.RefreshCharacterTotalWeight();
			this.RefreshCharacterCanUseItem();
			this.CurrentCharacterName = this._currentCharacter.Name.ToString();
			this.RightInventoryOwnerGold = Hero.MainHero.Gold - this._inventoryLogic.TotalAmount;
		}

		private SPItemVM InitializeCharacterEquipmentSlot(ItemRosterElement itemRosterElement, EquipmentIndex equipmentIndex)
		{
			SPItemVM spitemVM;
			if (!itemRosterElement.IsEmpty)
			{
				spitemVM = new SPItemVM(this._inventoryLogic, this.MainCharacter.IsFemale, this.CanCharacterUseItemBasedOnSkills(itemRosterElement), this._usageType, itemRosterElement, InventoryLogic.InventorySide.Equipment, this._fiveStackShortcutkeyText, this._entireStackShortcutkeyText, this._inventoryLogic.GetCostOfItemRosterElement(itemRosterElement, InventoryLogic.InventorySide.Equipment), new EquipmentIndex?(equipmentIndex));
			}
			else
			{
				spitemVM = new SPItemVM();
				spitemVM.RefreshWith(null, InventoryLogic.InventorySide.Equipment);
			}
			return spitemVM;
		}

		private void UpdateCharacterEquipment()
		{
			this.CharacterHelmSlot = this.InitializeCharacterEquipmentSlot(new ItemRosterElement(this.ActiveEquipment.GetEquipmentFromSlot(EquipmentIndex.NumAllWeaponSlots), 1), EquipmentIndex.NumAllWeaponSlots);
			this.CharacterCloakSlot = this.InitializeCharacterEquipmentSlot(new ItemRosterElement(this.ActiveEquipment.GetEquipmentFromSlot(EquipmentIndex.Cape), 1), EquipmentIndex.Cape);
			this.CharacterTorsoSlot = this.InitializeCharacterEquipmentSlot(new ItemRosterElement(this.ActiveEquipment.GetEquipmentFromSlot(EquipmentIndex.Body), 1), EquipmentIndex.Body);
			this.CharacterGloveSlot = this.InitializeCharacterEquipmentSlot(new ItemRosterElement(this.ActiveEquipment.GetEquipmentFromSlot(EquipmentIndex.Gloves), 1), EquipmentIndex.Gloves);
			this.CharacterBootSlot = this.InitializeCharacterEquipmentSlot(new ItemRosterElement(this.ActiveEquipment.GetEquipmentFromSlot(EquipmentIndex.Leg), 1), EquipmentIndex.Leg);
			this.CharacterMountSlot = this.InitializeCharacterEquipmentSlot(new ItemRosterElement(this.ActiveEquipment.GetEquipmentFromSlot(EquipmentIndex.ArmorItemEndSlot), 1), EquipmentIndex.ArmorItemEndSlot);
			this.CharacterMountArmorSlot = this.InitializeCharacterEquipmentSlot(new ItemRosterElement(this.ActiveEquipment.GetEquipmentFromSlot(EquipmentIndex.HorseHarness), 1), EquipmentIndex.HorseHarness);
			this.CharacterWeapon1Slot = this.InitializeCharacterEquipmentSlot(new ItemRosterElement(this.ActiveEquipment.GetEquipmentFromSlot(EquipmentIndex.WeaponItemBeginSlot), 1), EquipmentIndex.WeaponItemBeginSlot);
			this.CharacterWeapon2Slot = this.InitializeCharacterEquipmentSlot(new ItemRosterElement(this.ActiveEquipment.GetEquipmentFromSlot(EquipmentIndex.Weapon1), 1), EquipmentIndex.Weapon1);
			this.CharacterWeapon3Slot = this.InitializeCharacterEquipmentSlot(new ItemRosterElement(this.ActiveEquipment.GetEquipmentFromSlot(EquipmentIndex.Weapon2), 1), EquipmentIndex.Weapon2);
			this.CharacterWeapon4Slot = this.InitializeCharacterEquipmentSlot(new ItemRosterElement(this.ActiveEquipment.GetEquipmentFromSlot(EquipmentIndex.Weapon3), 1), EquipmentIndex.Weapon3);
			this.CharacterBannerSlot = this.InitializeCharacterEquipmentSlot(new ItemRosterElement(this.ActiveEquipment.GetEquipmentFromSlot(EquipmentIndex.ExtraWeaponSlot), 1), EquipmentIndex.ExtraWeaponSlot);
			this.MainCharacter.SetEquipment(this.ActiveEquipment);
		}

		private void UpdateCharacterArmorValues()
		{
			this.CurrentCharacterArmArmor = this._currentCharacter.GetArmArmorSum(!this.IsInWarSet);
			this.CurrentCharacterBodyArmor = this._currentCharacter.GetBodyArmorSum(!this.IsInWarSet);
			this.CurrentCharacterHeadArmor = this._currentCharacter.GetHeadArmorSum(!this.IsInWarSet);
			this.CurrentCharacterLegArmor = this._currentCharacter.GetLegArmorSum(!this.IsInWarSet);
			this.CurrentCharacterHorseArmor = this._currentCharacter.GetHorseArmorSum(!this.IsInWarSet);
		}

		private void RefreshCharacterTotalWeight()
		{
			CharacterObject currentCharacter = this._currentCharacter;
			float num = ((currentCharacter != null && currentCharacter.GetPerkValue(DefaultPerks.Athletics.FormFittingArmor)) ? (1f + DefaultPerks.Athletics.FormFittingArmor.PrimaryBonus) : 1f);
			this.CurrentCharacterTotalEncumbrance = MathF.Round(this.ActiveEquipment.GetTotalWeightOfWeapons() + this.ActiveEquipment.GetTotalWeightOfArmor(true) * num, 1).ToString("0.0");
		}

		private void RefreshCharacterCanUseItem()
		{
			for (int i = 0; i < this.RightItemListVM.Count; i++)
			{
				this.RightItemListVM[i].CanCharacterUseItem = this.CanCharacterUseItemBasedOnSkills(this.RightItemListVM[i].ItemRosterElement);
			}
			for (int j = 0; j < this.LeftItemListVM.Count; j++)
			{
				this.LeftItemListVM[j].CanCharacterUseItem = this.CanCharacterUseItemBasedOnSkills(this.LeftItemListVM[j].ItemRosterElement);
			}
		}

		private void InitializeInventory()
		{
			this.IsRefreshed = false;
			switch (this._inventoryLogic.MerchantItemType)
			{
			case InventoryManager.InventoryCategoryType.Armors:
				this.ActiveFilterIndex = 3;
				break;
			case InventoryManager.InventoryCategoryType.Weapon:
				this.ActiveFilterIndex = 1;
				break;
			case InventoryManager.InventoryCategoryType.Shield:
				this.ActiveFilterIndex = 2;
				break;
			case InventoryManager.InventoryCategoryType.HorseCategory:
				this.ActiveFilterIndex = 4;
				break;
			case InventoryManager.InventoryCategoryType.Goods:
				this.ActiveFilterIndex = 5;
				break;
			default:
				this.ActiveFilterIndex = 0;
				break;
			}
			this._equipmentCount = 0f;
			this.RightItemListVM.Clear();
			this.LeftItemListVM.Clear();
			int num = MathF.Max(this._inventoryLogic.GetElementCountOnSide(InventoryLogic.InventorySide.PlayerInventory), this._inventoryLogic.GetElementCountOnSide(InventoryLogic.InventorySide.OtherInventory));
			ItemRosterElement[] array = (from i in this._inventoryLogic.GetElementsInRoster(InventoryLogic.InventorySide.PlayerInventory)
				orderby i.EquipmentElement.GetModifiedItemName().ToString()
				select i).ToArray<ItemRosterElement>();
			ItemRosterElement[] array2 = (from i in this._inventoryLogic.GetElementsInRoster(InventoryLogic.InventorySide.OtherInventory)
				orderby i.EquipmentElement.GetModifiedItemName().ToString()
				select i).ToArray<ItemRosterElement>();
			this._lockedItemIDs = this._viewDataTracker.GetInventoryLocks().ToList<string>();
			for (int j = 0; j < num; j++)
			{
				if (j < array.Length)
				{
					ItemRosterElement itemRosterElement = array[j];
					SPItemVM spitemVM = new SPItemVM(this._inventoryLogic, this.MainCharacter.IsFemale, this.CanCharacterUseItemBasedOnSkills(itemRosterElement), this._usageType, itemRosterElement, InventoryLogic.InventorySide.PlayerInventory, this._fiveStackShortcutkeyText, this._entireStackShortcutkeyText, this._inventoryLogic.GetCostOfItemRosterElement(itemRosterElement, InventoryLogic.InventorySide.PlayerInventory), null);
					this.UpdateFilteredStatusOfItem(spitemVM);
					spitemVM.IsLocked = spitemVM.InventorySide == InventoryLogic.InventorySide.PlayerInventory && this.IsItemLocked(itemRosterElement);
					this.RightItemListVM.Add(spitemVM);
					if (!itemRosterElement.EquipmentElement.Item.IsMountable && !itemRosterElement.EquipmentElement.Item.IsAnimal)
					{
						this._equipmentCount += itemRosterElement.GetRosterElementWeight();
					}
				}
				if (j < array2.Length)
				{
					ItemRosterElement itemRosterElement2 = array2[j];
					SPItemVM spitemVM2 = new SPItemVM(this._inventoryLogic, this.MainCharacter.IsFemale, this.CanCharacterUseItemBasedOnSkills(itemRosterElement2), this._usageType, itemRosterElement2, InventoryLogic.InventorySide.OtherInventory, this._fiveStackShortcutkeyText, this._entireStackShortcutkeyText, this._inventoryLogic.GetCostOfItemRosterElement(itemRosterElement2, InventoryLogic.InventorySide.OtherInventory), null);
					this.UpdateFilteredStatusOfItem(spitemVM2);
					spitemVM2.IsLocked = spitemVM2.InventorySide == InventoryLogic.InventorySide.PlayerInventory && this.IsItemLocked(itemRosterElement2);
					this.LeftItemListVM.Add(spitemVM2);
				}
			}
			this.RefreshInformationValues();
			this.IsRefreshed = true;
		}

		private bool IsItemLocked(ItemRosterElement item)
		{
			string text = item.EquipmentElement.Item.StringId;
			if (item.EquipmentElement.ItemModifier != null)
			{
				text += item.EquipmentElement.ItemModifier.StringId;
			}
			return this._lockedItemIDs.Contains(text);
		}

		public void CompareNextItem()
		{
			this.CycleBetweenWeaponSlots();
			this.RefreshComparedItem();
		}

		private void BuyItem(SPItemVM item)
		{
			if (this.TargetEquipmentType != EquipmentIndex.None && item.ItemType != this.TargetEquipmentType && (this.TargetEquipmentType < EquipmentIndex.WeaponItemBeginSlot || this.TargetEquipmentType > EquipmentIndex.ExtraWeaponSlot || item.ItemType < EquipmentIndex.WeaponItemBeginSlot || item.ItemType > EquipmentIndex.ExtraWeaponSlot))
			{
				return;
			}
			if (this.TargetEquipmentType == EquipmentIndex.None)
			{
				this.TargetEquipmentType = item.ItemType;
				if (item.ItemType >= EquipmentIndex.WeaponItemBeginSlot && item.ItemType <= EquipmentIndex.ExtraWeaponSlot)
				{
					this.TargetEquipmentType = this.ActiveEquipment.GetWeaponPickUpSlotIndex(item.ItemRosterElement.EquipmentElement, false);
				}
			}
			int num = item.ItemCount;
			if (item.InventorySide == InventoryLogic.InventorySide.PlayerInventory)
			{
				ItemRosterElement? itemRosterElement = this._inventoryLogic.FindItemFromSide(InventoryLogic.InventorySide.OtherInventory, item.ItemRosterElement.EquipmentElement);
				if (itemRosterElement != null)
				{
					num = itemRosterElement.Value.Amount;
				}
			}
			TransferCommand transferCommand = TransferCommand.Transfer(MathF.Min(this.TransactionCount, num), InventoryLogic.InventorySide.OtherInventory, InventoryLogic.InventorySide.PlayerInventory, item.ItemRosterElement, item.ItemType, this.TargetEquipmentType, this._currentCharacter, !this.IsInWarSet);
			this._inventoryLogic.AddTransferCommand(transferCommand);
			if (this.EquipAfterBuy)
			{
				this._equipAfterTransferStack.Push(item);
			}
		}

		private void SellItem(SPItemVM item)
		{
			InventoryLogic.InventorySide inventorySide = item.InventorySide;
			int num = item.ItemCount;
			if (inventorySide == InventoryLogic.InventorySide.OtherInventory)
			{
				inventorySide = InventoryLogic.InventorySide.PlayerInventory;
				ItemRosterElement? itemRosterElement = this._inventoryLogic.FindItemFromSide(InventoryLogic.InventorySide.PlayerInventory, item.ItemRosterElement.EquipmentElement);
				if (itemRosterElement != null)
				{
					num = itemRosterElement.Value.Amount;
				}
			}
			TransferCommand transferCommand = TransferCommand.Transfer(MathF.Min(this.TransactionCount, num), inventorySide, InventoryLogic.InventorySide.OtherInventory, item.ItemRosterElement, item.ItemType, this.TargetEquipmentType, this._currentCharacter, !this.IsInWarSet);
			this._inventoryLogic.AddTransferCommand(transferCommand);
		}

		private void SlaughterItem(SPItemVM item)
		{
			int num = 1;
			if (this.IsFiveStackModifierActive)
			{
				num = MathF.Min(5, item.ItemCount);
			}
			else if (this.IsEntireStackModifierActive)
			{
				num = item.ItemCount;
			}
			for (int i = 0; i < num; i++)
			{
				this._inventoryLogic.SlaughterItem(item.ItemRosterElement);
			}
		}

		private void DonateItem(SPItemVM item)
		{
			if (this.IsFiveStackModifierActive)
			{
				int itemCount = item.ItemCount;
				for (int i = 0; i < MathF.Min(5, itemCount); i++)
				{
					this._inventoryLogic.DonateItem(item.ItemRosterElement);
				}
				return;
			}
			this._inventoryLogic.DonateItem(item.ItemRosterElement);
		}

		private float GetCapacityBudget(MobileParty party, bool isBuy)
		{
			if (isBuy)
			{
				int? num = ((party != null) ? new int?(party.InventoryCapacity) : null);
				float? num2 = ((num != null) ? new float?((float)num.GetValueOrDefault()) : null) - this._equipmentCount;
				if (num2 == null)
				{
					return 0f;
				}
				return num2.GetValueOrDefault();
			}
			else
			{
				if (this._inventoryLogic.OtherSideCapacityData != null)
				{
					return (float)this._inventoryLogic.OtherSideCapacityData.GetCapacity() - this.LeftItemListVM.Sum((SPItemVM x) => x.ItemRosterElement.GetRosterElementWeight());
				}
				return 0f;
			}
		}

		private void TransferAll(bool isBuy)
		{
			this.IsRefreshed = false;
			List<TransferCommand> list = new List<TransferCommand>(this.LeftItemListVM.Count);
			MBBindingList<SPItemVM> mbbindingList = (isBuy ? this.LeftItemListVM : this.RightItemListVM);
			MobileParty mobileParty;
			if (!isBuy)
			{
				PartyBase oppositePartyFromListener = this._inventoryLogic.OppositePartyFromListener;
				mobileParty = ((oppositePartyFromListener != null) ? oppositePartyFromListener.MobileParty : null);
			}
			else
			{
				mobileParty = MobileParty.MainParty;
			}
			MobileParty mobileParty2 = mobileParty;
			float num = 0f;
			float num2 = this.GetCapacityBudget(mobileParty2, isBuy);
			SPItemVM spitemVM = mbbindingList.FirstOrDefault((SPItemVM x) => !x.IsFiltered && !x.IsLocked);
			float num3 = ((spitemVM != null) ? spitemVM.ItemRosterElement.EquipmentElement.GetEquipmentElementWeight() : 0f);
			bool flag = num2 <= num3;
			InventoryLogic.InventorySide inventorySide = (isBuy ? InventoryLogic.InventorySide.OtherInventory : InventoryLogic.InventorySide.PlayerInventory);
			InventoryLogic.InventorySide inventorySide2 = (isBuy ? InventoryLogic.InventorySide.PlayerInventory : InventoryLogic.InventorySide.OtherInventory);
			List<SPItemVM> list2 = new List<SPItemVM>();
			bool flag2 = this._inventoryLogic.CanInventoryCapacityIncrease(inventorySide2);
			for (int i = 0; i < mbbindingList.Count; i++)
			{
				SPItemVM spitemVM2 = mbbindingList[i];
				if (spitemVM2 != null && !spitemVM2.IsFiltered && spitemVM2 != null && !spitemVM2.IsLocked && spitemVM2 != null && spitemVM2.IsTransferable)
				{
					int num4 = spitemVM2.ItemRosterElement.Amount;
					if (!flag)
					{
						float equipmentElementWeight = spitemVM2.ItemRosterElement.EquipmentElement.GetEquipmentElementWeight();
						float num5 = num + equipmentElementWeight * (float)num4;
						if (flag2)
						{
							if (this._inventoryLogic.GetCanItemIncreaseInventoryCapacity(mbbindingList[i].ItemRosterElement.EquipmentElement.Item))
							{
								list2.Add(mbbindingList[i]);
								goto IL_29E;
							}
							if (num5 >= num2 && list2.Count > 0)
							{
								List<TransferCommand> list3 = new List<TransferCommand>(list2.Count);
								for (int j = 0; j < list2.Count; j++)
								{
									SPItemVM spitemVM3 = list2[j];
									TransferCommand transferCommand = TransferCommand.Transfer(spitemVM3.ItemRosterElement.Amount, inventorySide, inventorySide2, spitemVM3.ItemRosterElement, EquipmentIndex.None, EquipmentIndex.None, this._currentCharacter, !this.IsInWarSet);
									list3.Add(transferCommand);
								}
								this._inventoryLogic.AddTransferCommands(list3);
								list3.Clear();
								list2.Clear();
								num2 = this.GetCapacityBudget(mobileParty2, isBuy);
							}
						}
						if (num4 > 0 && num5 > num2)
						{
							num4 = MBMath.ClampInt(num4, 0, MathF.Floor((num2 - num) / equipmentElementWeight));
							i = mbbindingList.Count;
						}
						num += (float)num4 * equipmentElementWeight;
					}
					if (num4 > 0)
					{
						TransferCommand transferCommand2 = TransferCommand.Transfer(num4, inventorySide, inventorySide2, spitemVM2.ItemRosterElement, EquipmentIndex.None, EquipmentIndex.None, this._currentCharacter, !this.IsInWarSet);
						list.Add(transferCommand2);
					}
				}
				IL_29E:;
			}
			if (num <= num2)
			{
				foreach (SPItemVM spitemVM4 in list2)
				{
					TransferCommand transferCommand3 = TransferCommand.Transfer(spitemVM4.ItemRosterElement.Amount, inventorySide, inventorySide2, spitemVM4.ItemRosterElement, EquipmentIndex.None, EquipmentIndex.None, this._currentCharacter, !this.IsInWarSet);
					list.Add(transferCommand3);
				}
			}
			this._inventoryLogic.AddTransferCommands(list);
			this.RefreshInformationValues();
			this.ExecuteRemoveZeroCounts();
			this.IsRefreshed = true;
		}

		public void ExecuteBuyAllItems()
		{
			this.TransferAll(true);
		}

		public void ExecuteSellAllItems()
		{
			this.TransferAll(false);
		}

		public void ExecuteBuyItemTest()
		{
			this.TransactionCount = 1;
			this.EquipAfterBuy = false;
			int totalGold = Hero.MainHero.Gold;
			foreach (SPItemVM spitemVM in this.LeftItemListVM.Where(delegate(SPItemVM i)
			{
				ItemObject item = i.ItemRosterElement.EquipmentElement.Item;
				return item != null && item.IsFood && i.ItemCost <= totalGold;
			}))
			{
				if (spitemVM.ItemCost <= totalGold)
				{
					this.ProcessBuyItem(spitemVM, false);
					totalGold -= spitemVM.ItemCost;
				}
			}
		}

		public void ExecuteResetTranstactions()
		{
			this._inventoryLogic.Reset(false);
			InformationManager.DisplayMessage(new InformationMessage(GameTexts.FindText("str_inventory_reset_message", null).ToString()));
			this.CurrentFocusedItem = null;
		}

		public void ExecuteResetAndCompleteTranstactions()
		{
			if (InventoryManager.Instance.CurrentMode == InventoryMode.Loot)
			{
				InformationManager.ShowInquiry(new InquiryData("", GameTexts.FindText("str_leaving_loot_behind", null).ToString(), true, true, GameTexts.FindText("str_yes", null).ToString(), GameTexts.FindText("str_no", null).ToString(), delegate
				{
					if (!this._isFinalized)
					{
						this._inventoryLogic.Reset(true);
						InventoryManager.Instance.CloseInventoryPresentation(true);
					}
				}, null, "", 0f, null, null, null), false, false);
				return;
			}
			this._inventoryLogic.Reset(true);
			InventoryManager.Instance.CloseInventoryPresentation(true);
		}

		public void ExecuteCompleteTranstactions()
		{
			if (InventoryManager.Instance.CurrentMode == InventoryMode.Loot && !this._inventoryLogic.IsThereAnyChanges() && this._inventoryLogic.GetElementsInInitialRoster(InventoryLogic.InventorySide.OtherInventory).Any<ItemRosterElement>())
			{
				InformationManager.ShowInquiry(new InquiryData("", GameTexts.FindText("str_leaving_loot_behind", null).ToString(), true, true, GameTexts.FindText("str_yes", null).ToString(), GameTexts.FindText("str_no", null).ToString(), new Action(this.HandleDone), null, "", 0f, null, null, null), false, false);
				return;
			}
			this.HandleDone();
		}

		private void HandleDone()
		{
			if (!this._isFinalized)
			{
				MBInformationManager.HideInformations();
				bool flag = this._inventoryLogic.TotalAmount < 0;
				InventoryListener inventoryListener = this._inventoryLogic.InventoryListener;
				bool flag2 = ((inventoryListener != null) ? inventoryListener.GetGold() : 0) >= MathF.Abs(this._inventoryLogic.TotalAmount);
				int num = (int)this._inventoryLogic.XpGainFromDonations;
				int num2 = ((this._usageType == InventoryMode.Default && num == 0 && !Game.Current.CheatMode) ? this._inventoryLogic.GetElementCountOnSide(InventoryLogic.InventorySide.OtherInventory) : 0);
				if (flag && !flag2)
				{
					InformationManager.ShowInquiry(new InquiryData("", GameTexts.FindText("str_trader_doesnt_have_enough_money", null).ToString(), true, true, GameTexts.FindText("str_yes", null).ToString(), GameTexts.FindText("str_no", null).ToString(), delegate
					{
						InventoryManager.Instance.CloseInventoryPresentation(false);
					}, null, "", 0f, null, null, null), false, false);
				}
				else if (num2 > 0)
				{
					InformationManager.ShowInquiry(new InquiryData("", GameTexts.FindText("str_discarding_items", null).ToString(), true, true, GameTexts.FindText("str_yes", null).ToString(), GameTexts.FindText("str_no", null).ToString(), delegate
					{
						InventoryManager.Instance.CloseInventoryPresentation(false);
					}, null, "", 0f, null, null, null), false, false);
				}
				else
				{
					InventoryManager.Instance.CloseInventoryPresentation(false);
				}
				this.SaveItemLockStates();
				this.SaveItemSortStates();
			}
		}

		private void SaveItemLockStates()
		{
			this._viewDataTracker.SetInventoryLocks(this._lockedItemIDs);
		}

		private void SaveItemSortStates()
		{
			this._viewDataTracker.InventorySetSortPreference((int)this._usageType, (int)this.PlayerInventorySortController.CurrentSortOption.Value, (int)this.PlayerInventorySortController.CurrentSortState.Value);
		}

		public void ExecuteTransferWithParameters(SPItemVM item, int index, string targetTag)
		{
			if (targetTag == "OverCharacter")
			{
				this.TargetEquipmentIndex = -1;
				if (item.InventorySide == InventoryLogic.InventorySide.OtherInventory)
				{
					item.TransactionCount = 1;
					this.TransactionCount = 1;
					this.ProcessEquipItem(item);
					return;
				}
				if (item.InventorySide == InventoryLogic.InventorySide.PlayerInventory)
				{
					this.ProcessEquipItem(item);
					return;
				}
			}
			else if (targetTag == "PlayerInventory")
			{
				this.TargetEquipmentIndex = -1;
				if (item.InventorySide == InventoryLogic.InventorySide.Equipment)
				{
					this.ProcessUnequipItem(item);
					return;
				}
				if (item.InventorySide == InventoryLogic.InventorySide.OtherInventory)
				{
					item.TransactionCount = item.ItemCount;
					this.TransactionCount = item.ItemCount;
					this.ProcessBuyItem(item, false);
					return;
				}
			}
			else if (targetTag == "OtherInventory")
			{
				if (item.InventorySide != InventoryLogic.InventorySide.OtherInventory)
				{
					item.TransactionCount = item.ItemCount;
					this.TransactionCount = item.ItemCount;
					this.ProcessSellItem(item, false);
					return;
				}
			}
			else if (targetTag.StartsWith("Equipment"))
			{
				this.TargetEquipmentIndex = int.Parse(targetTag.Substring("Equipment".Length + 1));
				if (item.InventorySide == InventoryLogic.InventorySide.OtherInventory)
				{
					item.TransactionCount = 1;
					this.TransactionCount = 1;
					this.ProcessEquipItem(item);
					return;
				}
				if (item.InventorySide == InventoryLogic.InventorySide.PlayerInventory || item.InventorySide == InventoryLogic.InventorySide.Equipment)
				{
					this.ProcessEquipItem(item);
				}
			}
		}

		private void UpdateIsDoneDisabled()
		{
			this.IsDoneDisabled = !this._inventoryLogic.CanPlayerCompleteTransaction();
		}

		private void ProcessFilter(SPInventoryVM.Filters filterIndex)
		{
			this.ActiveFilterIndex = (int)filterIndex;
			this.IsRefreshed = false;
			foreach (SPItemVM spitemVM in this.LeftItemListVM)
			{
				if (spitemVM != null)
				{
					this.UpdateFilteredStatusOfItem(spitemVM);
				}
			}
			foreach (SPItemVM spitemVM2 in this.RightItemListVM)
			{
				if (spitemVM2 != null)
				{
					this.UpdateFilteredStatusOfItem(spitemVM2);
				}
			}
			this.IsRefreshed = true;
		}

		private void UpdateFilteredStatusOfItem(SPItemVM item)
		{
			bool flag = !this._filters[this._activeFilterIndex].Contains(item.TypeId);
			bool flag2 = false;
			if (this.IsSearchAvailable && (item.InventorySide == InventoryLogic.InventorySide.OtherInventory || item.InventorySide == InventoryLogic.InventorySide.PlayerInventory))
			{
				string text = ((item.InventorySide == InventoryLogic.InventorySide.OtherInventory) ? this.LeftSearchText : this.RightSearchText);
				if (text.Length > 1)
				{
					flag2 = !item.ItemDescription.ToLower().Contains(text);
				}
			}
			item.IsFiltered = flag || flag2;
		}

		private void OnSearchTextChanged(bool isLeft)
		{
			if (this.IsSearchAvailable)
			{
				(isLeft ? this.LeftItemListVM : this.RightItemListVM).ApplyActionOnAllItems(delegate(SPItemVM x)
				{
					this.UpdateFilteredStatusOfItem(x);
				});
			}
		}

		public void ExecuteFilterNone()
		{
			this.ProcessFilter(SPInventoryVM.Filters.All);
			Game.Current.EventManager.TriggerEvent<InventoryFilterChangedEvent>(new InventoryFilterChangedEvent(SPInventoryVM.Filters.All));
		}

		public void ExecuteFilterWeapons()
		{
			this.ProcessFilter(SPInventoryVM.Filters.Weapons);
			Game.Current.EventManager.TriggerEvent<InventoryFilterChangedEvent>(new InventoryFilterChangedEvent(SPInventoryVM.Filters.Weapons));
		}

		public void ExecuteFilterArmors()
		{
			this.ProcessFilter(SPInventoryVM.Filters.Armors);
			Game.Current.EventManager.TriggerEvent<InventoryFilterChangedEvent>(new InventoryFilterChangedEvent(SPInventoryVM.Filters.Armors));
		}

		public void ExecuteFilterShieldsAndRanged()
		{
			this.ProcessFilter(SPInventoryVM.Filters.ShieldsAndRanged);
			Game.Current.EventManager.TriggerEvent<InventoryFilterChangedEvent>(new InventoryFilterChangedEvent(SPInventoryVM.Filters.ShieldsAndRanged));
		}

		public void ExecuteFilterMounts()
		{
			this.ProcessFilter(SPInventoryVM.Filters.Mounts);
			Game.Current.EventManager.TriggerEvent<InventoryFilterChangedEvent>(new InventoryFilterChangedEvent(SPInventoryVM.Filters.Mounts));
		}

		public void ExecuteFilterMisc()
		{
			this.ProcessFilter(SPInventoryVM.Filters.Miscellaneous);
			Game.Current.EventManager.TriggerEvent<InventoryFilterChangedEvent>(new InventoryFilterChangedEvent(SPInventoryVM.Filters.Miscellaneous));
		}

		public void CycleBetweenWeaponSlots()
		{
			EquipmentIndex selectedEquipmentIndex = (EquipmentIndex)this._selectedEquipmentIndex;
			if (selectedEquipmentIndex >= EquipmentIndex.WeaponItemBeginSlot && selectedEquipmentIndex < EquipmentIndex.NumAllWeaponSlots)
			{
				int selectedEquipmentIndex2 = this._selectedEquipmentIndex;
				do
				{
					if (this._selectedEquipmentIndex < 3)
					{
						this._selectedEquipmentIndex++;
					}
					else
					{
						this._selectedEquipmentIndex = 0;
					}
				}
				while (this._selectedEquipmentIndex != selectedEquipmentIndex2 && this.GetItemFromIndex((EquipmentIndex)this._selectedEquipmentIndex).ItemRosterElement.EquipmentElement.Item == null);
			}
		}

		private SPItemVM GetItemFromIndex(EquipmentIndex itemType)
		{
			switch (itemType)
			{
			case EquipmentIndex.WeaponItemBeginSlot:
				return this.CharacterWeapon1Slot;
			case EquipmentIndex.Weapon1:
				return this.CharacterWeapon2Slot;
			case EquipmentIndex.Weapon2:
				return this.CharacterWeapon3Slot;
			case EquipmentIndex.Weapon3:
				return this.CharacterWeapon4Slot;
			case EquipmentIndex.ExtraWeaponSlot:
				return this.CharacterBannerSlot;
			case EquipmentIndex.NumAllWeaponSlots:
				return this.CharacterHelmSlot;
			case EquipmentIndex.Body:
				return this.CharacterTorsoSlot;
			case EquipmentIndex.Leg:
				return this.CharacterBootSlot;
			case EquipmentIndex.Gloves:
				return this.CharacterGloveSlot;
			case EquipmentIndex.Cape:
				return this.CharacterCloakSlot;
			case EquipmentIndex.ArmorItemEndSlot:
				return this.CharacterMountSlot;
			case EquipmentIndex.HorseHarness:
				return this.CharacterMountArmorSlot;
			default:
				return null;
			}
		}

		private void OnTutorialNotificationElementIDChange(TutorialNotificationElementChangeEvent obj)
		{
			if (obj.NewNotificationElementID != this._latestTutorialElementID)
			{
				if (this._latestTutorialElementID != null)
				{
					if (obj.NewNotificationElementID != "TransferButtonOnlyFood" && this._isFoodTransferButtonHighlightApplied)
					{
						this.SetFoodTransferButtonHighlightState(false);
						this._isFoodTransferButtonHighlightApplied = false;
					}
					if (obj.NewNotificationElementID != "InventoryMicsFilter" && this.IsMicsFilterHighlightEnabled)
					{
						this.IsMicsFilterHighlightEnabled = false;
					}
					if (obj.NewNotificationElementID != "CivilianFilter" && this.IsCivilianFilterHighlightEnabled)
					{
						this.IsCivilianFilterHighlightEnabled = false;
					}
					if (obj.NewNotificationElementID != "InventoryOtherBannerItems" && this.IsBannerItemsHighlightApplied)
					{
						this.SetBannerItemsHighlightState(false);
						this.IsCivilianFilterHighlightEnabled = false;
					}
				}
				this._latestTutorialElementID = obj.NewNotificationElementID;
				if (!string.IsNullOrEmpty(this._latestTutorialElementID))
				{
					if (!this._isFoodTransferButtonHighlightApplied && this._latestTutorialElementID == "TransferButtonOnlyFood")
					{
						this.SetFoodTransferButtonHighlightState(true);
						this._isFoodTransferButtonHighlightApplied = true;
					}
					if (!this.IsMicsFilterHighlightEnabled && this._latestTutorialElementID == "InventoryMicsFilter")
					{
						this.IsMicsFilterHighlightEnabled = true;
					}
					if (!this.IsCivilianFilterHighlightEnabled && this._latestTutorialElementID == "CivilianFilter")
					{
						this.IsCivilianFilterHighlightEnabled = true;
					}
					if (!this.IsBannerItemsHighlightApplied && this._latestTutorialElementID == "InventoryOtherBannerItems")
					{
						this.IsBannerItemsHighlightApplied = true;
						this.ExecuteFilterMisc();
						this.SetBannerItemsHighlightState(true);
						return;
					}
				}
				else
				{
					if (this._isFoodTransferButtonHighlightApplied)
					{
						this.SetFoodTransferButtonHighlightState(false);
						this._isFoodTransferButtonHighlightApplied = false;
					}
					if (this.IsMicsFilterHighlightEnabled)
					{
						this.IsMicsFilterHighlightEnabled = false;
					}
					if (this.IsCivilianFilterHighlightEnabled)
					{
						this.IsCivilianFilterHighlightEnabled = false;
					}
					if (this.IsBannerItemsHighlightApplied)
					{
						this.SetBannerItemsHighlightState(false);
						this.IsBannerItemsHighlightApplied = false;
					}
				}
			}
		}

		private void SetFoodTransferButtonHighlightState(bool state)
		{
			for (int i = 0; i < this.LeftItemListVM.Count; i++)
			{
				SPItemVM spitemVM = this.LeftItemListVM[i];
				if (spitemVM.ItemRosterElement.EquipmentElement.Item.IsFood)
				{
					spitemVM.IsTransferButtonHighlighted = state;
				}
			}
		}

		private void SetBannerItemsHighlightState(bool state)
		{
			for (int i = 0; i < this.LeftItemListVM.Count; i++)
			{
				SPItemVM spitemVM = this.LeftItemListVM[i];
				if (spitemVM.ItemRosterElement.EquipmentElement.Item.IsBannerItem)
				{
					spitemVM.IsItemHighlightEnabled = state;
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel ResetHint
		{
			get
			{
				return this._resetHint;
			}
			set
			{
				if (value != this._resetHint)
				{
					this._resetHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "ResetHint");
				}
			}
		}

		[DataSourceProperty]
		public string LeftInventoryLabel
		{
			get
			{
				return this._leftInventoryLabel;
			}
			set
			{
				if (value != this._leftInventoryLabel)
				{
					this._leftInventoryLabel = value;
					base.OnPropertyChangedWithValue<string>(value, "LeftInventoryLabel");
				}
			}
		}

		[DataSourceProperty]
		public string RightInventoryLabel
		{
			get
			{
				return this._rightInventoryLabel;
			}
			set
			{
				if (value != this._rightInventoryLabel)
				{
					this._rightInventoryLabel = value;
					base.OnPropertyChangedWithValue<string>(value, "RightInventoryLabel");
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

		[DataSourceProperty]
		public bool IsDoneDisabled
		{
			get
			{
				return this._isDoneDisabled;
			}
			set
			{
				if (value != this._isDoneDisabled)
				{
					this._isDoneDisabled = value;
					base.OnPropertyChangedWithValue(value, "IsDoneDisabled");
				}
			}
		}

		[DataSourceProperty]
		public bool OtherSideHasCapacity
		{
			get
			{
				return this._otherSideHasCapacity;
			}
			set
			{
				if (value != this._otherSideHasCapacity)
				{
					this._otherSideHasCapacity = value;
					base.OnPropertyChangedWithValue(value, "OtherSideHasCapacity");
				}
			}
		}

		[DataSourceProperty]
		public bool IsSearchAvailable
		{
			get
			{
				return this._isSearchAvailable;
			}
			set
			{
				if (value != this._isSearchAvailable)
				{
					if (!value)
					{
						this.LeftSearchText = string.Empty;
						this.RightSearchText = string.Empty;
					}
					this._isSearchAvailable = value;
					base.OnPropertyChangedWithValue(value, "IsSearchAvailable");
				}
			}
		}

		[DataSourceProperty]
		public bool IsOtherInventoryGoldRelevant
		{
			get
			{
				return this._isOtherInventoryGoldRelevant;
			}
			set
			{
				if (value != this._isOtherInventoryGoldRelevant)
				{
					this._isOtherInventoryGoldRelevant = value;
					base.OnPropertyChangedWithValue(value, "IsOtherInventoryGoldRelevant");
				}
			}
		}

		[DataSourceProperty]
		public string CancelLbl
		{
			get
			{
				return this._cancelLbl;
			}
			set
			{
				if (value != this._cancelLbl)
				{
					this._cancelLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "CancelLbl");
				}
			}
		}

		[DataSourceProperty]
		public string ResetLbl
		{
			get
			{
				return this._resetLbl;
			}
			set
			{
				if (value != this._resetLbl)
				{
					this._resetLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "ResetLbl");
				}
			}
		}

		[DataSourceProperty]
		public string TypeText
		{
			get
			{
				return this._typeText;
			}
			set
			{
				if (value != this._typeText)
				{
					this._typeText = value;
					base.OnPropertyChangedWithValue<string>(value, "TypeText");
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
		public string QuantityText
		{
			get
			{
				return this._quantityText;
			}
			set
			{
				if (value != this._quantityText)
				{
					this._quantityText = value;
					base.OnPropertyChangedWithValue<string>(value, "QuantityText");
				}
			}
		}

		[DataSourceProperty]
		public string CostText
		{
			get
			{
				return this._costText;
			}
			set
			{
				if (value != this._costText)
				{
					this._costText = value;
					base.OnPropertyChangedWithValue<string>(value, "CostText");
				}
			}
		}

		[DataSourceProperty]
		public string SearchPlaceholderText
		{
			get
			{
				return this._searchPlaceholderText;
			}
			set
			{
				if (value != this._searchPlaceholderText)
				{
					this._searchPlaceholderText = value;
					base.OnPropertyChangedWithValue<string>(value, "SearchPlaceholderText");
				}
			}
		}

		[DataSourceProperty]
		public BasicTooltipViewModel ProductionTooltip
		{
			get
			{
				return this._productionTooltip;
			}
			set
			{
				if (value != this._productionTooltip)
				{
					this._productionTooltip = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "ProductionTooltip");
				}
			}
		}

		[DataSourceProperty]
		public BasicTooltipViewModel EquipmentMaxCountHint
		{
			get
			{
				return this._equipmentMaxCountHint;
			}
			set
			{
				if (value != this._equipmentMaxCountHint)
				{
					this._equipmentMaxCountHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "EquipmentMaxCountHint");
				}
			}
		}

		[DataSourceProperty]
		public BasicTooltipViewModel CurrentCharacterSkillsTooltip
		{
			get
			{
				return this._currentCharacterSkillsTooltip;
			}
			set
			{
				if (value != this._currentCharacterSkillsTooltip)
				{
					this._currentCharacterSkillsTooltip = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "CurrentCharacterSkillsTooltip");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel NoSaddleHint
		{
			get
			{
				return this._noSaddleHint;
			}
			set
			{
				if (value != this._noSaddleHint)
				{
					this._noSaddleHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "NoSaddleHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel DonationLblHint
		{
			get
			{
				return this._donationLblHint;
			}
			set
			{
				if (value != this._donationLblHint)
				{
					this._donationLblHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "DonationLblHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel ArmArmorHint
		{
			get
			{
				return this._armArmorHint;
			}
			set
			{
				if (value != this._armArmorHint)
				{
					this._armArmorHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "ArmArmorHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel BodyArmorHint
		{
			get
			{
				return this._bodyArmorHint;
			}
			set
			{
				if (value != this._bodyArmorHint)
				{
					this._bodyArmorHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "BodyArmorHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel HeadArmorHint
		{
			get
			{
				return this._headArmorHint;
			}
			set
			{
				if (value != this._headArmorHint)
				{
					this._headArmorHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "HeadArmorHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel LegArmorHint
		{
			get
			{
				return this._legArmorHint;
			}
			set
			{
				if (value != this._legArmorHint)
				{
					this._legArmorHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "LegArmorHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel HorseArmorHint
		{
			get
			{
				return this._horseArmorHint;
			}
			set
			{
				if (value != this._horseArmorHint)
				{
					this._horseArmorHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "HorseArmorHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel FilterAllHint
		{
			get
			{
				return this._filterAllHint;
			}
			set
			{
				if (value != this._filterAllHint)
				{
					this._filterAllHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "FilterAllHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel FilterWeaponHint
		{
			get
			{
				return this._filterWeaponHint;
			}
			set
			{
				if (value != this._filterWeaponHint)
				{
					this._filterWeaponHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "FilterWeaponHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel FilterArmorHint
		{
			get
			{
				return this._filterArmorHint;
			}
			set
			{
				if (value != this._filterArmorHint)
				{
					this._filterArmorHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "FilterArmorHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel FilterShieldAndRangedHint
		{
			get
			{
				return this._filterShieldAndRangedHint;
			}
			set
			{
				if (value != this._filterShieldAndRangedHint)
				{
					this._filterShieldAndRangedHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "FilterShieldAndRangedHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel FilterMountAndHarnessHint
		{
			get
			{
				return this._filterMountAndHarnessHint;
			}
			set
			{
				if (value != this._filterMountAndHarnessHint)
				{
					this._filterMountAndHarnessHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "FilterMountAndHarnessHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel FilterMiscHint
		{
			get
			{
				return this._filterMiscHint;
			}
			set
			{
				if (value != this._filterMiscHint)
				{
					this._filterMiscHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "FilterMiscHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel CivilianOutfitHint
		{
			get
			{
				return this._civilianOutfitHint;
			}
			set
			{
				if (value != this._civilianOutfitHint)
				{
					this._civilianOutfitHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "CivilianOutfitHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel BattleOutfitHint
		{
			get
			{
				return this._battleOutfitHint;
			}
			set
			{
				if (value != this._battleOutfitHint)
				{
					this._battleOutfitHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "BattleOutfitHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel EquipmentHelmSlotHint
		{
			get
			{
				return this._equipmentHelmSlotHint;
			}
			set
			{
				if (value != this._equipmentHelmSlotHint)
				{
					this._equipmentHelmSlotHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "EquipmentHelmSlotHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel EquipmentArmorSlotHint
		{
			get
			{
				return this._equipmentArmorSlotHint;
			}
			set
			{
				if (value != this._equipmentArmorSlotHint)
				{
					this._equipmentArmorSlotHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "EquipmentArmorSlotHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel EquipmentBootSlotHint
		{
			get
			{
				return this._equipmentBootSlotHint;
			}
			set
			{
				if (value != this._equipmentBootSlotHint)
				{
					this._equipmentBootSlotHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "EquipmentBootSlotHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel EquipmentCloakSlotHint
		{
			get
			{
				return this._equipmentCloakSlotHint;
			}
			set
			{
				if (value != this._equipmentCloakSlotHint)
				{
					this._equipmentCloakSlotHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "EquipmentCloakSlotHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel EquipmentGloveSlotHint
		{
			get
			{
				return this._equipmentGloveSlotHint;
			}
			set
			{
				if (value != this._equipmentGloveSlotHint)
				{
					this._equipmentGloveSlotHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "EquipmentGloveSlotHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel EquipmentHarnessSlotHint
		{
			get
			{
				return this._equipmentHarnessSlotHint;
			}
			set
			{
				if (value != this._equipmentHarnessSlotHint)
				{
					this._equipmentHarnessSlotHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "EquipmentHarnessSlotHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel EquipmentMountSlotHint
		{
			get
			{
				return this._equipmentMountSlotHint;
			}
			set
			{
				if (value != this._equipmentMountSlotHint)
				{
					this._equipmentMountSlotHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "EquipmentMountSlotHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel EquipmentWeaponSlotHint
		{
			get
			{
				return this._equipmentWeaponSlotHint;
			}
			set
			{
				if (value != this._equipmentWeaponSlotHint)
				{
					this._equipmentWeaponSlotHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "EquipmentWeaponSlotHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel EquipmentBannerSlotHint
		{
			get
			{
				return this._equipmentBannerSlotHint;
			}
			set
			{
				if (value != this._equipmentBannerSlotHint)
				{
					this._equipmentBannerSlotHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "EquipmentBannerSlotHint");
				}
			}
		}

		[DataSourceProperty]
		public BasicTooltipViewModel BuyAllHint
		{
			get
			{
				return this._buyAllHint;
			}
			set
			{
				if (value != this._buyAllHint)
				{
					this._buyAllHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "BuyAllHint");
				}
			}
		}

		[DataSourceProperty]
		public BasicTooltipViewModel SellAllHint
		{
			get
			{
				return this._sellAllHint;
			}
			set
			{
				if (value != this._sellAllHint)
				{
					this._sellAllHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "SellAllHint");
				}
			}
		}

		[DataSourceProperty]
		public BasicTooltipViewModel PreviousCharacterHint
		{
			get
			{
				return this._previousCharacterHint;
			}
			set
			{
				if (value != this._previousCharacterHint)
				{
					this._previousCharacterHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "PreviousCharacterHint");
				}
			}
		}

		[DataSourceProperty]
		public BasicTooltipViewModel NextCharacterHint
		{
			get
			{
				return this._nextCharacterHint;
			}
			set
			{
				if (value != this._nextCharacterHint)
				{
					this._nextCharacterHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "NextCharacterHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel WeightHint
		{
			get
			{
				return this._weightHint;
			}
			set
			{
				if (value != this._weightHint)
				{
					this._weightHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "WeightHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel PreviewHint
		{
			get
			{
				return this._previewHint;
			}
			set
			{
				if (value != this._previewHint)
				{
					this._previewHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "PreviewHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel EquipHint
		{
			get
			{
				return this._equipHint;
			}
			set
			{
				if (value != this._equipHint)
				{
					this._equipHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "EquipHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel UnequipHint
		{
			get
			{
				return this._unequipHint;
			}
			set
			{
				if (value != this._unequipHint)
				{
					this._unequipHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "UnequipHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel SellHint
		{
			get
			{
				return this._sellHint;
			}
			set
			{
				if (value != this._sellHint)
				{
					this._sellHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "SellHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel PlayerSideCapacityExceededHint
		{
			get
			{
				return this._playerSideCapacityExceededHint;
			}
			set
			{
				if (value != this._playerSideCapacityExceededHint)
				{
					this._playerSideCapacityExceededHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "PlayerSideCapacityExceededHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel OtherSideCapacityExceededHint
		{
			get
			{
				return this._otherSideCapacityExceededHint;
			}
			set
			{
				if (value != this._otherSideCapacityExceededHint)
				{
					this._otherSideCapacityExceededHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "OtherSideCapacityExceededHint");
				}
			}
		}

		[DataSourceProperty]
		public SelectorVM<InventoryCharacterSelectorItemVM> CharacterList
		{
			get
			{
				return this._characterList;
			}
			set
			{
				if (value != this._characterList)
				{
					this._characterList = value;
					base.OnPropertyChangedWithValue<SelectorVM<InventoryCharacterSelectorItemVM>>(value, "CharacterList");
				}
			}
		}

		[DataSourceProperty]
		public SPInventorySortControllerVM PlayerInventorySortController
		{
			get
			{
				return this._playerInventorySortController;
			}
			set
			{
				if (value != this._playerInventorySortController)
				{
					this._playerInventorySortController = value;
					base.OnPropertyChangedWithValue<SPInventorySortControllerVM>(value, "PlayerInventorySortController");
				}
			}
		}

		[DataSourceProperty]
		public SPInventorySortControllerVM OtherInventorySortController
		{
			get
			{
				return this._otherInventorySortController;
			}
			set
			{
				if (value != this._otherInventorySortController)
				{
					this._otherInventorySortController = value;
					base.OnPropertyChangedWithValue<SPInventorySortControllerVM>(value, "OtherInventorySortController");
				}
			}
		}

		[DataSourceProperty]
		public ItemPreviewVM ItemPreview
		{
			get
			{
				return this._itemPreview;
			}
			set
			{
				if (value != this._itemPreview)
				{
					this._itemPreview = value;
					base.OnPropertyChangedWithValue<ItemPreviewVM>(value, "ItemPreview");
				}
			}
		}

		[DataSourceProperty]
		public int ActiveFilterIndex
		{
			get
			{
				return (int)this._activeFilterIndex;
			}
			set
			{
				if (value != (int)this._activeFilterIndex)
				{
					this._activeFilterIndex = (SPInventoryVM.Filters)value;
					base.OnPropertyChangedWithValue(value, "ActiveFilterIndex");
				}
			}
		}

		[DataSourceProperty]
		public bool CompanionExists
		{
			get
			{
				return this._companionExists;
			}
			set
			{
				if (value != this._companionExists)
				{
					this._companionExists = value;
					base.OnPropertyChangedWithValue(value, "CompanionExists");
				}
			}
		}

		[DataSourceProperty]
		public bool IsTradingWithSettlement
		{
			get
			{
				return this._isTradingWithSettlement;
			}
			set
			{
				if (value != this._isTradingWithSettlement)
				{
					this._isTradingWithSettlement = value;
					base.OnPropertyChangedWithValue(value, "IsTradingWithSettlement");
				}
			}
		}

		[DataSourceProperty]
		public bool IsInWarSet
		{
			get
			{
				return this._isInWarSet;
			}
			set
			{
				if (value != this._isInWarSet)
				{
					this._isInWarSet = value;
					base.OnPropertyChangedWithValue(value, "IsInWarSet");
					this.UpdateRightCharacter();
					Game.Current.EventManager.TriggerEvent<InventoryEquipmentTypeChangedEvent>(new InventoryEquipmentTypeChangedEvent(value));
				}
			}
		}

		[DataSourceProperty]
		public bool IsMicsFilterHighlightEnabled
		{
			get
			{
				return this._isMicsFilterHighlightEnabled;
			}
			set
			{
				if (value != this._isMicsFilterHighlightEnabled)
				{
					this._isMicsFilterHighlightEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsMicsFilterHighlightEnabled");
				}
			}
		}

		[DataSourceProperty]
		public bool IsCivilianFilterHighlightEnabled
		{
			get
			{
				return this._isCivilianFilterHighlightEnabled;
			}
			set
			{
				if (value != this._isCivilianFilterHighlightEnabled)
				{
					this._isCivilianFilterHighlightEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsCivilianFilterHighlightEnabled");
				}
			}
		}

		[DataSourceProperty]
		public ItemMenuVM ItemMenu
		{
			get
			{
				return this._itemMenu;
			}
			set
			{
				if (value != this._itemMenu)
				{
					this._itemMenu = value;
					base.OnPropertyChangedWithValue<ItemMenuVM>(value, "ItemMenu");
				}
			}
		}

		[DataSourceProperty]
		public string PlayerSideCapacityExceededText
		{
			get
			{
				return this._playerSideCapacityExceededText;
			}
			set
			{
				if (value != this._playerSideCapacityExceededText)
				{
					this._playerSideCapacityExceededText = value;
					base.OnPropertyChangedWithValue<string>(value, "PlayerSideCapacityExceededText");
				}
			}
		}

		[DataSourceProperty]
		public string OtherSideCapacityExceededText
		{
			get
			{
				return this._otherSideCapacityExceededText;
			}
			set
			{
				if (value != this._otherSideCapacityExceededText)
				{
					this._otherSideCapacityExceededText = value;
					base.OnPropertyChangedWithValue<string>(value, "OtherSideCapacityExceededText");
				}
			}
		}

		[DataSourceProperty]
		public string LeftSearchText
		{
			get
			{
				return this._leftSearchText;
			}
			set
			{
				if (value != this._leftSearchText)
				{
					this._leftSearchText = value;
					base.OnPropertyChangedWithValue<string>(value, "LeftSearchText");
					this.OnSearchTextChanged(true);
				}
			}
		}

		[DataSourceProperty]
		public string RightSearchText
		{
			get
			{
				return this._rightSearchText;
			}
			set
			{
				if (value != this._rightSearchText)
				{
					this._rightSearchText = value;
					base.OnPropertyChangedWithValue<string>(value, "RightSearchText");
					this.OnSearchTextChanged(false);
				}
			}
		}

		[DataSourceProperty]
		public bool HasGainedExperience
		{
			get
			{
				return this._hasGainedExperience;
			}
			set
			{
				if (value != this._hasGainedExperience)
				{
					this._hasGainedExperience = value;
					base.OnPropertyChangedWithValue(value, "HasGainedExperience");
				}
			}
		}

		[DataSourceProperty]
		public bool IsDonationXpGainExceedsMax
		{
			get
			{
				return this._isDonationXpGainExceedsMax;
			}
			set
			{
				if (value != this._isDonationXpGainExceedsMax)
				{
					this._isDonationXpGainExceedsMax = value;
					base.OnPropertyChangedWithValue(value, "IsDonationXpGainExceedsMax");
				}
			}
		}

		[DataSourceProperty]
		public bool NoSaddleWarned
		{
			get
			{
				return this._noSaddleWarned;
			}
			set
			{
				if (value != this._noSaddleWarned)
				{
					this._noSaddleWarned = value;
					base.OnPropertyChangedWithValue(value, "NoSaddleWarned");
				}
			}
		}

		[DataSourceProperty]
		public bool PlayerEquipmentCountWarned
		{
			get
			{
				return this._playerEquipmentCountWarned;
			}
			set
			{
				if (value != this._playerEquipmentCountWarned)
				{
					this._playerEquipmentCountWarned = value;
					base.OnPropertyChangedWithValue(value, "PlayerEquipmentCountWarned");
				}
			}
		}

		[DataSourceProperty]
		public bool OtherEquipmentCountWarned
		{
			get
			{
				return this._otherEquipmentCountWarned;
			}
			set
			{
				if (value != this._otherEquipmentCountWarned)
				{
					this._otherEquipmentCountWarned = value;
					base.OnPropertyChangedWithValue(value, "OtherEquipmentCountWarned");
				}
			}
		}

		[DataSourceProperty]
		public string OtherEquipmentCountText
		{
			get
			{
				return this._otherEquipmentCountText;
			}
			set
			{
				if (value != this._otherEquipmentCountText)
				{
					this._otherEquipmentCountText = value;
					base.OnPropertyChangedWithValue<string>(value, "OtherEquipmentCountText");
				}
			}
		}

		[DataSourceProperty]
		public string PlayerEquipmentCountText
		{
			get
			{
				return this._playerEquipmentCountText;
			}
			set
			{
				if (value != this._playerEquipmentCountText)
				{
					this._playerEquipmentCountText = value;
					base.OnPropertyChangedWithValue<string>(value, "PlayerEquipmentCountText");
				}
			}
		}

		[DataSourceProperty]
		public string NoSaddleText
		{
			get
			{
				return this._noSaddleText;
			}
			set
			{
				if (value != this._noSaddleText)
				{
					this._noSaddleText = value;
					base.OnPropertyChangedWithValue<string>(value, "NoSaddleText");
				}
			}
		}

		[DataSourceProperty]
		public int TargetEquipmentIndex
		{
			get
			{
				return (int)this._targetEquipmentIndex;
			}
			set
			{
				if (value != (int)this._targetEquipmentIndex)
				{
					this._targetEquipmentIndex = (EquipmentIndex)value;
					base.OnPropertyChangedWithValue(value, "TargetEquipmentIndex");
				}
			}
		}

		public EquipmentIndex TargetEquipmentType
		{
			get
			{
				return this._targetEquipmentIndex;
			}
			set
			{
				if (value != this._targetEquipmentIndex)
				{
					this._targetEquipmentIndex = value;
					base.OnPropertyChanged("TargetEquipmentIndex");
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
				}
				this.RefreshTransactionCost(value);
			}
		}

		[DataSourceProperty]
		public bool IsTrading
		{
			get
			{
				return this._isTrading;
			}
			set
			{
				if (value != this._isTrading)
				{
					this._isTrading = value;
					base.OnPropertyChangedWithValue(value, "IsTrading");
				}
			}
		}

		[DataSourceProperty]
		public bool EquipAfterBuy
		{
			get
			{
				return this._equipAfterBuy;
			}
			set
			{
				if (value != this._equipAfterBuy)
				{
					this._equipAfterBuy = value;
					base.OnPropertyChangedWithValue(value, "EquipAfterBuy");
				}
			}
		}

		[DataSourceProperty]
		public string TradeLbl
		{
			get
			{
				return this._tradeLbl;
			}
			set
			{
				if (value != this._tradeLbl)
				{
					this._tradeLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "TradeLbl");
				}
			}
		}

		[DataSourceProperty]
		public string ExperienceLbl
		{
			get
			{
				return this._experienceLbl;
			}
			set
			{
				if (value != this._experienceLbl)
				{
					this._experienceLbl = value;
					base.OnPropertyChangedWithValue<string>(value, "ExperienceLbl");
				}
			}
		}

		[DataSourceProperty]
		public string CurrentCharacterName
		{
			get
			{
				return this._currentCharacterName;
			}
			set
			{
				if (value != this._currentCharacterName)
				{
					this._currentCharacterName = value;
					base.OnPropertyChangedWithValue<string>(value, "CurrentCharacterName");
				}
			}
		}

		[DataSourceProperty]
		public string RightInventoryOwnerName
		{
			get
			{
				return this._rightInventoryOwnerName;
			}
			set
			{
				if (value != this._rightInventoryOwnerName)
				{
					this._rightInventoryOwnerName = value;
					base.OnPropertyChangedWithValue<string>(value, "RightInventoryOwnerName");
				}
			}
		}

		[DataSourceProperty]
		public string LeftInventoryOwnerName
		{
			get
			{
				return this._leftInventoryOwnerName;
			}
			set
			{
				if (value != this._leftInventoryOwnerName)
				{
					this._leftInventoryOwnerName = value;
					base.OnPropertyChangedWithValue<string>(value, "LeftInventoryOwnerName");
				}
			}
		}

		[DataSourceProperty]
		public int RightInventoryOwnerGold
		{
			get
			{
				return this._rightInventoryOwnerGold;
			}
			set
			{
				if (value != this._rightInventoryOwnerGold)
				{
					this._rightInventoryOwnerGold = value;
					base.OnPropertyChangedWithValue(value, "RightInventoryOwnerGold");
				}
			}
		}

		[DataSourceProperty]
		public int LeftInventoryOwnerGold
		{
			get
			{
				return this._leftInventoryOwnerGold;
			}
			set
			{
				if (value != this._leftInventoryOwnerGold)
				{
					this._leftInventoryOwnerGold = value;
					base.OnPropertyChangedWithValue(value, "LeftInventoryOwnerGold");
				}
			}
		}

		[DataSourceProperty]
		public int ItemCountToBuy
		{
			get
			{
				return this._itemCountToBuy;
			}
			set
			{
				if (value != this._itemCountToBuy)
				{
					this._itemCountToBuy = value;
					base.OnPropertyChangedWithValue(value, "ItemCountToBuy");
				}
			}
		}

		[DataSourceProperty]
		public string CurrentCharacterTotalEncumbrance
		{
			get
			{
				return this._currentCharacterTotalEncumbrance;
			}
			set
			{
				if (value != this._currentCharacterTotalEncumbrance)
				{
					this._currentCharacterTotalEncumbrance = value;
					base.OnPropertyChangedWithValue<string>(value, "CurrentCharacterTotalEncumbrance");
				}
			}
		}

		[DataSourceProperty]
		public float CurrentCharacterLegArmor
		{
			get
			{
				return this._currentCharacterLegArmor;
			}
			set
			{
				if (MathF.Abs(value - this._currentCharacterLegArmor) > 0.01f)
				{
					this._currentCharacterLegArmor = value;
					base.OnPropertyChangedWithValue(value, "CurrentCharacterLegArmor");
				}
			}
		}

		[DataSourceProperty]
		public float CurrentCharacterHeadArmor
		{
			get
			{
				return this._currentCharacterHeadArmor;
			}
			set
			{
				if (MathF.Abs(value - this._currentCharacterHeadArmor) > 0.01f)
				{
					this._currentCharacterHeadArmor = value;
					base.OnPropertyChangedWithValue(value, "CurrentCharacterHeadArmor");
				}
			}
		}

		[DataSourceProperty]
		public float CurrentCharacterBodyArmor
		{
			get
			{
				return this._currentCharacterBodyArmor;
			}
			set
			{
				if (MathF.Abs(value - this._currentCharacterBodyArmor) > 0.01f)
				{
					this._currentCharacterBodyArmor = value;
					base.OnPropertyChangedWithValue(value, "CurrentCharacterBodyArmor");
				}
			}
		}

		[DataSourceProperty]
		public float CurrentCharacterArmArmor
		{
			get
			{
				return this._currentCharacterArmArmor;
			}
			set
			{
				if (MathF.Abs(value - this._currentCharacterArmArmor) > 0.01f)
				{
					this._currentCharacterArmArmor = value;
					base.OnPropertyChangedWithValue(value, "CurrentCharacterArmArmor");
				}
			}
		}

		[DataSourceProperty]
		public float CurrentCharacterHorseArmor
		{
			get
			{
				return this._currentCharacterHorseArmor;
			}
			set
			{
				if (MathF.Abs(value - this._currentCharacterHorseArmor) > 0.01f)
				{
					this._currentCharacterHorseArmor = value;
					base.OnPropertyChangedWithValue(value, "CurrentCharacterHorseArmor");
				}
			}
		}

		[DataSourceProperty]
		public bool IsRefreshed
		{
			get
			{
				return this._isRefreshed;
			}
			set
			{
				if (this._isRefreshed != value)
				{
					this._isRefreshed = value;
					base.OnPropertyChangedWithValue(value, "IsRefreshed");
				}
			}
		}

		[DataSourceProperty]
		public bool IsExtendedEquipmentControlsEnabled
		{
			get
			{
				return this._isExtendedEquipmentControlsEnabled;
			}
			set
			{
				if (value != this._isExtendedEquipmentControlsEnabled)
				{
					this._isExtendedEquipmentControlsEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsExtendedEquipmentControlsEnabled");
				}
			}
		}

		[DataSourceProperty]
		public bool IsFocusedOnItemList
		{
			get
			{
				return this._isFocusedOnItemList;
			}
			set
			{
				if (value != this._isFocusedOnItemList)
				{
					this._isFocusedOnItemList = value;
					base.OnPropertyChangedWithValue(value, "IsFocusedOnItemList");
				}
			}
		}

		[DataSourceProperty]
		public SPItemVM CurrentFocusedItem
		{
			get
			{
				return this._currentFocusedItem;
			}
			set
			{
				if (value != this._currentFocusedItem)
				{
					this._currentFocusedItem = value;
					base.OnPropertyChangedWithValue<SPItemVM>(value, "CurrentFocusedItem");
				}
			}
		}

		[DataSourceProperty]
		public SPItemVM CharacterHelmSlot
		{
			get
			{
				return this._characterHelmSlot;
			}
			set
			{
				if (value != this._characterHelmSlot)
				{
					this._characterHelmSlot = value;
					base.OnPropertyChangedWithValue<SPItemVM>(value, "CharacterHelmSlot");
				}
			}
		}

		[DataSourceProperty]
		public SPItemVM CharacterCloakSlot
		{
			get
			{
				return this._characterCloakSlot;
			}
			set
			{
				if (value != this._characterCloakSlot)
				{
					this._characterCloakSlot = value;
					base.OnPropertyChangedWithValue<SPItemVM>(value, "CharacterCloakSlot");
				}
			}
		}

		[DataSourceProperty]
		public SPItemVM CharacterTorsoSlot
		{
			get
			{
				return this._characterTorsoSlot;
			}
			set
			{
				if (value != this._characterTorsoSlot)
				{
					this._characterTorsoSlot = value;
					base.OnPropertyChangedWithValue<SPItemVM>(value, "CharacterTorsoSlot");
				}
			}
		}

		[DataSourceProperty]
		public SPItemVM CharacterGloveSlot
		{
			get
			{
				return this._characterGloveSlot;
			}
			set
			{
				if (value != this._characterGloveSlot)
				{
					this._characterGloveSlot = value;
					base.OnPropertyChangedWithValue<SPItemVM>(value, "CharacterGloveSlot");
				}
			}
		}

		[DataSourceProperty]
		public SPItemVM CharacterBootSlot
		{
			get
			{
				return this._characterBootSlot;
			}
			set
			{
				if (value != this._characterBootSlot)
				{
					this._characterBootSlot = value;
					base.OnPropertyChangedWithValue<SPItemVM>(value, "CharacterBootSlot");
				}
			}
		}

		[DataSourceProperty]
		public SPItemVM CharacterMountSlot
		{
			get
			{
				return this._characterMountSlot;
			}
			set
			{
				if (value != this._characterMountSlot)
				{
					this._characterMountSlot = value;
					base.OnPropertyChangedWithValue<SPItemVM>(value, "CharacterMountSlot");
				}
			}
		}

		[DataSourceProperty]
		public SPItemVM CharacterMountArmorSlot
		{
			get
			{
				return this._characterMountArmorSlot;
			}
			set
			{
				if (value != this._characterMountArmorSlot)
				{
					this._characterMountArmorSlot = value;
					base.OnPropertyChangedWithValue<SPItemVM>(value, "CharacterMountArmorSlot");
				}
			}
		}

		[DataSourceProperty]
		public SPItemVM CharacterWeapon1Slot
		{
			get
			{
				return this._characterWeapon1Slot;
			}
			set
			{
				if (value != this._characterWeapon1Slot)
				{
					this._characterWeapon1Slot = value;
					base.OnPropertyChangedWithValue<SPItemVM>(value, "CharacterWeapon1Slot");
				}
			}
		}

		[DataSourceProperty]
		public SPItemVM CharacterWeapon2Slot
		{
			get
			{
				return this._characterWeapon2Slot;
			}
			set
			{
				if (value != this._characterWeapon2Slot)
				{
					this._characterWeapon2Slot = value;
					base.OnPropertyChangedWithValue<SPItemVM>(value, "CharacterWeapon2Slot");
				}
			}
		}

		[DataSourceProperty]
		public SPItemVM CharacterWeapon3Slot
		{
			get
			{
				return this._characterWeapon3Slot;
			}
			set
			{
				if (value != this._characterWeapon3Slot)
				{
					this._characterWeapon3Slot = value;
					base.OnPropertyChangedWithValue<SPItemVM>(value, "CharacterWeapon3Slot");
				}
			}
		}

		[DataSourceProperty]
		public SPItemVM CharacterWeapon4Slot
		{
			get
			{
				return this._characterWeapon4Slot;
			}
			set
			{
				if (value != this._characterWeapon4Slot)
				{
					this._characterWeapon4Slot = value;
					base.OnPropertyChangedWithValue<SPItemVM>(value, "CharacterWeapon4Slot");
				}
			}
		}

		[DataSourceProperty]
		public SPItemVM CharacterBannerSlot
		{
			get
			{
				return this._characterBannerSlot;
			}
			set
			{
				if (value != this._characterBannerSlot)
				{
					this._characterBannerSlot = value;
					base.OnPropertyChangedWithValue<SPItemVM>(value, "CharacterBannerSlot");
				}
			}
		}

		[DataSourceProperty]
		public HeroViewModel MainCharacter
		{
			get
			{
				return this._mainCharacter;
			}
			set
			{
				if (value != this._mainCharacter)
				{
					this._mainCharacter = value;
					base.OnPropertyChangedWithValue<HeroViewModel>(value, "MainCharacter");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<SPItemVM> RightItemListVM
		{
			get
			{
				return this._rightItemListVM;
			}
			set
			{
				if (value != this._rightItemListVM)
				{
					this._rightItemListVM = value;
					base.OnPropertyChangedWithValue<MBBindingList<SPItemVM>>(value, "RightItemListVM");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<SPItemVM> LeftItemListVM
		{
			get
			{
				return this._leftItemListVM;
			}
			set
			{
				if (value != this._leftItemListVM)
				{
					this._leftItemListVM = value;
					base.OnPropertyChangedWithValue<MBBindingList<SPItemVM>>(value, "LeftItemListVM");
				}
			}
		}

		[DataSourceProperty]
		public bool IsBannerItemsHighlightApplied
		{
			get
			{
				return this._isBannerItemsHighlightApplied;
			}
			set
			{
				if (value != this._isBannerItemsHighlightApplied)
				{
					this._isBannerItemsHighlightApplied = value;
					base.OnPropertyChangedWithValue(value, "IsBannerItemsHighlightApplied");
				}
			}
		}

		[DataSourceProperty]
		public int BannerTypeCode
		{
			get
			{
				return this._bannerTypeCode;
			}
			set
			{
				if (value != this._bannerTypeCode)
				{
					this._bannerTypeCode = value;
					base.OnPropertyChangedWithValue(value, "BannerTypeCode");
				}
			}
		}

		private TextObject GetPreviousCharacterKeyText()
		{
			if (this.PreviousCharacterInputKey == null || this._getKeyTextFromKeyId == null)
			{
				return TextObject.Empty;
			}
			return this._getKeyTextFromKeyId(this.PreviousCharacterInputKey.KeyID);
		}

		private TextObject GetNextCharacterKeyText()
		{
			if (this.NextCharacterInputKey == null || this._getKeyTextFromKeyId == null)
			{
				return TextObject.Empty;
			}
			return this._getKeyTextFromKeyId(this.NextCharacterInputKey.KeyID);
		}

		private TextObject GetBuyAllKeyText()
		{
			if (this.BuyAllInputKey == null || this._getKeyTextFromKeyId == null)
			{
				return TextObject.Empty;
			}
			return this._getKeyTextFromKeyId(this.BuyAllInputKey.KeyID);
		}

		private TextObject GetSellAllKeyText()
		{
			if (this.SellAllInputKey == null || this._getKeyTextFromKeyId == null)
			{
				return TextObject.Empty;
			}
			return this._getKeyTextFromKeyId(this.SellAllInputKey.KeyID);
		}

		public void SetResetInputKey(HotKey hotkey)
		{
			this.ResetInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
		}

		public void SetCancelInputKey(HotKey gameKey)
		{
			this.CancelInputKey = InputKeyItemVM.CreateFromHotKey(gameKey, true);
		}

		public void SetDoneInputKey(HotKey hotKey)
		{
			this.DoneInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		public void SetPreviousCharacterInputKey(HotKey hotKey)
		{
			this.PreviousCharacterInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
			this.SetPreviousCharacterHint();
		}

		public void SetNextCharacterInputKey(HotKey hotKey)
		{
			this.NextCharacterInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
			this.SetNextCharacterHint();
		}

		public void SetBuyAllInputKey(HotKey hotKey)
		{
			this.BuyAllInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
			this.SetBuyAllHint();
		}

		public void SetSellAllInputKey(HotKey hotKey)
		{
			this.SellAllInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
			this.SetSellAllHint();
		}

		public void SetGetKeyTextFromKeyIDFunc(Func<string, TextObject> getKeyTextFromKeyId)
		{
			this._getKeyTextFromKeyId = getKeyTextFromKeyId;
		}

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
		public InputKeyItemVM PreviousCharacterInputKey
		{
			get
			{
				return this._previousCharacterInputKey;
			}
			set
			{
				if (value != this._previousCharacterInputKey)
				{
					this._previousCharacterInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "PreviousCharacterInputKey");
				}
			}
		}

		[DataSourceProperty]
		public InputKeyItemVM NextCharacterInputKey
		{
			get
			{
				return this._nextCharacterInputKey;
			}
			set
			{
				if (value != this._nextCharacterInputKey)
				{
					this._nextCharacterInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "NextCharacterInputKey");
				}
			}
		}

		[DataSourceProperty]
		public InputKeyItemVM BuyAllInputKey
		{
			get
			{
				return this._buyAllInputKey;
			}
			set
			{
				if (value != this._buyAllInputKey)
				{
					this._buyAllInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "BuyAllInputKey");
				}
			}
		}

		[DataSourceProperty]
		public InputKeyItemVM SellAllInputKey
		{
			get
			{
				return this._sellAllInputKey;
			}
			set
			{
				if (value != this._sellAllInputKey)
				{
					this._sellAllInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "SellAllInputKey");
				}
			}
		}

		void IInventoryStateHandler.ExecuteLootingScript()
		{
			this.ExecuteBuyAllItems();
		}

		void IInventoryStateHandler.ExecuteBuyConsumableItem()
		{
			this.ExecuteBuyItemTest();
		}

		void IInventoryStateHandler.ExecuteSellAllLoot()
		{
			for (int i = this.RightItemListVM.Count - 1; i >= 0; i--)
			{
				SPItemVM spitemVM = this.RightItemListVM[i];
				if (spitemVM.GetItemTypeWithItemObject() != EquipmentIndex.None)
				{
					this.SellItem(spitemVM);
				}
			}
		}

		void IInventoryStateHandler.FilterInventoryAtOpening(InventoryManager.InventoryCategoryType inventoryCategoryType)
		{
			switch (inventoryCategoryType)
			{
			case InventoryManager.InventoryCategoryType.Armors:
				this.ExecuteFilterArmors();
				return;
			case InventoryManager.InventoryCategoryType.Weapon:
				this.ExecuteFilterWeapons();
				return;
			case InventoryManager.InventoryCategoryType.Shield:
				break;
			case InventoryManager.InventoryCategoryType.HorseCategory:
				this.ExecuteFilterMounts();
				return;
			case InventoryManager.InventoryCategoryType.Goods:
				this.ExecuteFilterMisc();
				break;
			default:
				return;
			}
		}

		public bool DoNotSync;

		private readonly Func<WeaponComponentData, ItemObject.ItemUsageSetFlags> _getItemUsageSetFlags;

		private readonly IViewDataTracker _viewDataTracker;

		public bool IsFiveStackModifierActive;

		public bool IsEntireStackModifierActive;

		private readonly int _donationMaxShareableXp;

		private readonly TroopRoster _rightTroopRoster;

		private InventoryMode _usageType = InventoryMode.Trade;

		private readonly TroopRoster _leftTroopRoster;

		private int _lastComparedItemIndex;

		private readonly Stack<SPItemVM> _equipAfterTransferStack;

		private int _currentInventoryCharacterIndex;

		private bool _isTrading;

		private bool _isFinalized;

		private bool _isCharacterEquipmentDirty;

		private float _equipmentCount;

		private string _selectedTooltipItemStringID = "";

		private string _comparedTooltipItemStringID = "";

		private InventoryLogic _inventoryLogic;

		private CharacterObject _currentCharacter;

		private SPItemVM _selectedItem;

		private string _fiveStackShortcutkeyText;

		private string _entireStackShortcutkeyText;

		private List<ItemVM> _comparedItemList;

		private List<string> _lockedItemIDs;

		private Func<string, TextObject> _getKeyTextFromKeyId;

		private readonly List<int> _everyItemType = new List<int>
		{
			1, 2, 3, 4, 5, 6, 7, 8, 9, 10,
			11, 12, 13, 14, 15, 16, 17, 18, 19, 20,
			21, 22, 23, 24
		};

		private readonly List<int> _weaponItemTypes = new List<int> { 2, 3, 4 };

		private readonly List<int> _armorItemTypes = new List<int> { 12, 13, 14, 15, 21, 22 };

		private readonly List<int> _mountItemTypes = new List<int> { 1, 23 };

		private readonly List<int> _shieldAndRangedItemTypes = new List<int> { 7, 5, 6, 8, 9, 10, 16, 17, 18 };

		private readonly List<int> _miscellaneousItemTypes = new List<int> { 11, 19, 20, 24 };

		private readonly Dictionary<SPInventoryVM.Filters, List<int>> _filters;

		private int _selectedEquipmentIndex;

		private bool _isFoodTransferButtonHighlightApplied;

		private bool _isBannerItemsHighlightApplied;

		private string _latestTutorialElementID;

		private string _leftInventoryLabel;

		private string _rightInventoryLabel;

		private bool _otherSideHasCapacity;

		private bool _isDoneDisabled;

		private bool _isSearchAvailable;

		private bool _isOtherInventoryGoldRelevant;

		private string _doneLbl;

		private string _cancelLbl;

		private string _resetLbl;

		private string _typeText;

		private string _nameText;

		private string _quantityText;

		private string _costText;

		private string _searchPlaceholderText;

		private HintViewModel _resetHint;

		private HintViewModel _filterAllHint;

		private HintViewModel _filterWeaponHint;

		private HintViewModel _filterArmorHint;

		private HintViewModel _filterShieldAndRangedHint;

		private HintViewModel _filterMountAndHarnessHint;

		private HintViewModel _filterMiscHint;

		private HintViewModel _civilianOutfitHint;

		private HintViewModel _battleOutfitHint;

		private HintViewModel _equipmentHelmSlotHint;

		private HintViewModel _equipmentArmorSlotHint;

		private HintViewModel _equipmentBootSlotHint;

		private HintViewModel _equipmentCloakSlotHint;

		private HintViewModel _equipmentGloveSlotHint;

		private HintViewModel _equipmentHarnessSlotHint;

		private HintViewModel _equipmentMountSlotHint;

		private HintViewModel _equipmentWeaponSlotHint;

		private HintViewModel _equipmentBannerSlotHint;

		private BasicTooltipViewModel _buyAllHint;

		private BasicTooltipViewModel _sellAllHint;

		private BasicTooltipViewModel _previousCharacterHint;

		private BasicTooltipViewModel _nextCharacterHint;

		private HintViewModel _weightHint;

		private HintViewModel _armArmorHint;

		private HintViewModel _bodyArmorHint;

		private HintViewModel _headArmorHint;

		private HintViewModel _legArmorHint;

		private HintViewModel _horseArmorHint;

		private HintViewModel _previewHint;

		private HintViewModel _equipHint;

		private HintViewModel _unequipHint;

		private HintViewModel _sellHint;

		private HintViewModel _playerSideCapacityExceededHint;

		private HintViewModel _noSaddleHint;

		private HintViewModel _donationLblHint;

		private HintViewModel _otherSideCapacityExceededHint;

		private BasicTooltipViewModel _equipmentMaxCountHint;

		private BasicTooltipViewModel _currentCharacterSkillsTooltip;

		private BasicTooltipViewModel _productionTooltip;

		private HeroViewModel _mainCharacter;

		private bool _isExtendedEquipmentControlsEnabled;

		private bool _isFocusedOnItemList;

		private SPItemVM _currentFocusedItem;

		private bool _equipAfterBuy;

		private MBBindingList<SPItemVM> _leftItemListVM;

		private MBBindingList<SPItemVM> _rightItemListVM;

		private ItemMenuVM _itemMenu;

		private SPItemVM _characterHelmSlot;

		private SPItemVM _characterCloakSlot;

		private SPItemVM _characterTorsoSlot;

		private SPItemVM _characterGloveSlot;

		private SPItemVM _characterBootSlot;

		private SPItemVM _characterMountSlot;

		private SPItemVM _characterMountArmorSlot;

		private SPItemVM _characterWeapon1Slot;

		private SPItemVM _characterWeapon2Slot;

		private SPItemVM _characterWeapon3Slot;

		private SPItemVM _characterWeapon4Slot;

		private SPItemVM _characterBannerSlot;

		private EquipmentIndex _targetEquipmentIndex = EquipmentIndex.None;

		private int _transactionCount = -1;

		private bool _isRefreshed;

		private string _tradeLbl = "";

		private string _experienceLbl = "";

		private bool _hasGainedExperience;

		private bool _isDonationXpGainExceedsMax;

		private bool _noSaddleWarned;

		private bool _otherEquipmentCountWarned;

		private bool _playerEquipmentCountWarned;

		private bool _isTradingWithSettlement;

		private string _otherEquipmentCountText;

		private string _playerEquipmentCountText;

		private string _noSaddleText;

		private string _leftSearchText = "";

		private string _playerSideCapacityExceededText;

		private string _otherSideCapacityExceededText;

		private string _rightSearchText = "";

		private bool _isInWarSet = true;

		private bool _companionExists;

		private SPInventoryVM.Filters _activeFilterIndex;

		private bool _isMicsFilterHighlightEnabled;

		private bool _isCivilianFilterHighlightEnabled;

		private ItemPreviewVM _itemPreview;

		private SelectorVM<InventoryCharacterSelectorItemVM> _characterList;

		private SPInventorySortControllerVM _otherInventorySortController;

		private SPInventorySortControllerVM _playerInventorySortController;

		private int _bannerTypeCode;

		private string _leftInventoryOwnerName;

		private int _leftInventoryOwnerGold;

		private string _rightInventoryOwnerName;

		private string _currentCharacterName;

		private int _rightInventoryOwnerGold;

		private int _itemCountToBuy;

		private float _currentCharacterArmArmor;

		private float _currentCharacterBodyArmor;

		private float _currentCharacterHeadArmor;

		private float _currentCharacterLegArmor;

		private float _currentCharacterHorseArmor;

		private string _currentCharacterTotalEncumbrance;

		private InputKeyItemVM _resetInputKey;

		private InputKeyItemVM _cancelInputKey;

		private InputKeyItemVM _doneInputKey;

		private InputKeyItemVM _previousCharacterInputKey;

		private InputKeyItemVM _nextCharacterInputKey;

		private InputKeyItemVM _buyAllInputKey;

		private InputKeyItemVM _sellAllInputKey;

		public enum Filters
		{
			All,
			Weapons,
			ShieldsAndRanged,
			Armors,
			Mounts,
			Miscellaneous
		}
	}
}
