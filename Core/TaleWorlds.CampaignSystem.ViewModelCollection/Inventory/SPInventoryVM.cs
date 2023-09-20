using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
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
	// Token: 0x0200007E RID: 126
	public class SPInventoryVM : ViewModel, IInventoryStateHandler
	{
		// Token: 0x06000B67 RID: 2919 RVA: 0x0002E6BC File Offset: 0x0002C8BC
		public SPInventoryVM(InventoryLogic inventoryLogic, bool isInCivilianModeByDefault, Func<WeaponComponentData, ItemObject.ItemUsageSetFlags> getItemUsageSetFlags, string fiveStackShortcutkeyText, string entireStackShortcutkeyText)
		{
			this._usageType = InventoryManager.Instance.CurrentMode;
			this._inventoryLogic = inventoryLogic;
			this._playerUpdateTracker = Campaign.Current.PlayerUpdateTracker;
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
			Game.Current.EventManager.RegisterEvent<TutorialNotificationElementChangeEvent>(new Action<TutorialNotificationElementChangeEvent>(this.OnTutorialNotificationElementIDChange));
			this.RefreshValues();
		}

		// Token: 0x06000B68 RID: 2920 RVA: 0x0002ED74 File Offset: 0x0002CF74
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

		// Token: 0x06000B69 RID: 2921 RVA: 0x0002EE0C File Offset: 0x0002D00C
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
			this.CapacityExceededHint = new HintViewModel(GameTexts.FindText("str_capacity_exceeded_hint", null), null);
			this.CapacityExceededText = GameTexts.FindText("str_capacity_exceeded", null).ToString();
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

		// Token: 0x06000B6A RID: 2922 RVA: 0x0002F2B8 File Offset: 0x0002D4B8
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
			base.OnFinalize();
		}

		// Token: 0x06000B6B RID: 2923 RVA: 0x0002F3D8 File Offset: 0x0002D5D8
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

		// Token: 0x06000B6C RID: 2924 RVA: 0x0002F4A0 File Offset: 0x0002D6A0
		private bool CanSelectHero(Hero hero)
		{
			return hero.IsAlive && hero.CanHeroEquipmentBeChanged() && hero.Clan == Clan.PlayerClan && hero.HeroState != Hero.CharacterStates.Disabled && !hero.IsChild;
		}

		// Token: 0x06000B6D RID: 2925 RVA: 0x0002F4D3 File Offset: 0x0002D6D3
		private void SetPreviousCharacterHint()
		{
			this.PreviousCharacterHint = new BasicTooltipViewModel(delegate
			{
				GameTexts.SetVariable("HOTKEY", this.GetPreviousCharacterKeyText());
				GameTexts.SetVariable("TEXT", GameTexts.FindText("str_inventory_prev_char", null));
				return GameTexts.FindText("str_hotkey_with_hint", null).ToString();
			});
		}

		// Token: 0x06000B6E RID: 2926 RVA: 0x0002F4EC File Offset: 0x0002D6EC
		private void SetNextCharacterHint()
		{
			this.NextCharacterHint = new BasicTooltipViewModel(delegate
			{
				GameTexts.SetVariable("HOTKEY", this.GetNextCharacterKeyText());
				GameTexts.SetVariable("TEXT", GameTexts.FindText("str_inventory_next_char", null));
				return GameTexts.FindText("str_hotkey_with_hint", null).ToString();
			});
		}

		// Token: 0x06000B6F RID: 2927 RVA: 0x0002F508 File Offset: 0x0002D708
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

		// Token: 0x06000B70 RID: 2928 RVA: 0x0002F568 File Offset: 0x0002D768
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

		// Token: 0x06000B71 RID: 2929 RVA: 0x0002F5EC File Offset: 0x0002D7EC
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

		// Token: 0x170003B0 RID: 944
		// (get) Token: 0x06000B72 RID: 2930 RVA: 0x0002F69E File Offset: 0x0002D89E
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

		// Token: 0x06000B73 RID: 2931 RVA: 0x0002F6BF File Offset: 0x0002D8BF
		public void ExecuteShowRecap()
		{
			InformationManager.ShowTooltip(typeof(InventoryLogic), new object[] { this._inventoryLogic });
		}

		// Token: 0x06000B74 RID: 2932 RVA: 0x0002F6DF File Offset: 0x0002D8DF
		public void ExecuteCancelRecap()
		{
			MBInformationManager.HideInformations();
		}

		// Token: 0x06000B75 RID: 2933 RVA: 0x0002F6E8 File Offset: 0x0002D8E8
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

		// Token: 0x06000B76 RID: 2934 RVA: 0x0002F78B File Offset: 0x0002D98B
		private void ProcessPreviewItem(ItemVM item)
		{
			this._inventoryLogic.IsPreviewingItem = true;
			this.ItemPreview.Open(item.ItemRosterElement.EquipmentElement);
		}

		// Token: 0x06000B77 RID: 2935 RVA: 0x0002F7AF File Offset: 0x0002D9AF
		public void ClosePreview()
		{
			this.ItemPreview.Close();
		}

		// Token: 0x06000B78 RID: 2936 RVA: 0x0002F7BC File Offset: 0x0002D9BC
		private void OnPreviewClosed()
		{
			this._inventoryLogic.IsPreviewingItem = false;
		}

		// Token: 0x06000B79 RID: 2937 RVA: 0x0002F7CC File Offset: 0x0002D9CC
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

		// Token: 0x06000B7A RID: 2938 RVA: 0x0002F812 File Offset: 0x0002DA12
		private void ProcessUnequipItem(ItemVM draggedItem)
		{
			this.IsRefreshed = false;
			this.UnequipEquipment(draggedItem as SPItemVM);
			this.RefreshInformationValues();
			this.IsRefreshed = true;
		}

		// Token: 0x06000B7B RID: 2939 RVA: 0x0002F834 File Offset: 0x0002DA34
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

		// Token: 0x06000B7C RID: 2940 RVA: 0x0002F8FC File Offset: 0x0002DAFC
		private void ProcessSellItem(SPItemVM item, bool cameFromTradeData)
		{
			this.IsRefreshed = false;
			MBTextManager.SetTextVariable("ITEM_DESCRIPTION", item.ItemDescription, false);
			MBTextManager.SetTextVariable("ITEM_COST", item.ItemCost);
			if (this.IsEntireStackModifierActive && !cameFromTradeData)
			{
				ItemRosterElement? itemRosterElement;
				this.TransactionCount = ((this._inventoryLogic.FindItemFromSide(InventoryLogic.InventorySide.PlayerInventory, item.ItemRosterElement.EquipmentElement) != null) ? itemRosterElement.GetValueOrDefault().Amount : 0);
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

		// Token: 0x06000B7D RID: 2941 RVA: 0x0002F9B4 File Offset: 0x0002DBB4
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

		// Token: 0x06000B7E RID: 2942 RVA: 0x0002FA24 File Offset: 0x0002DC24
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

		// Token: 0x06000B7F RID: 2943 RVA: 0x0002FBD4 File Offset: 0x0002DDD4
		private void ResetComparedItems(ItemVM item, int alternativeUsageIndex)
		{
			ItemVM itemVM = this.ProcessCompareItem(item, alternativeUsageIndex);
			this.ItemMenu.SetItem(this._selectedItem, itemVM, this._currentCharacter, alternativeUsageIndex);
		}

		// Token: 0x06000B80 RID: 2944 RVA: 0x0002FC04 File Offset: 0x0002DE04
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

		// Token: 0x06000B81 RID: 2945 RVA: 0x0002FE90 File Offset: 0x0002E090
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

		// Token: 0x06000B82 RID: 2946 RVA: 0x0002FEEE File Offset: 0x0002E0EE
		public void ResetSelectedItem()
		{
			this._selectedItem = null;
		}

		// Token: 0x06000B83 RID: 2947 RVA: 0x0002FEF8 File Offset: 0x0002E0F8
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

		// Token: 0x06000B84 RID: 2948 RVA: 0x0002FF44 File Offset: 0x0002E144
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

		// Token: 0x06000B85 RID: 2949 RVA: 0x0002FF90 File Offset: 0x0002E190
		private void OnItemFocus(SPItemVM item)
		{
			this.CurrentFocusedItem = item;
		}

		// Token: 0x06000B86 RID: 2950 RVA: 0x0002FF99 File Offset: 0x0002E199
		private void ProcessItemSelect(ItemVM item)
		{
			this.ExecuteRemoveZeroCounts();
		}

		// Token: 0x06000B87 RID: 2951 RVA: 0x0002FFA4 File Offset: 0x0002E1A4
		private void RefreshTransactionCost(int transactionCount = 1)
		{
			if (this._selectedItem != null && this.IsTrading)
			{
				int num;
				int itemTotalPrice = this._inventoryLogic.GetItemTotalPrice(this._selectedItem.ItemRosterElement, transactionCount, out num, this._selectedItem.InventorySide == InventoryLogic.InventorySide.OtherInventory);
				this.ItemMenu.SetTransactionCost(itemTotalPrice, num);
			}
		}

		// Token: 0x06000B88 RID: 2952 RVA: 0x0002FFF8 File Offset: 0x0002E1F8
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

		// Token: 0x06000B89 RID: 2953 RVA: 0x00030080 File Offset: 0x0002E280
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

		// Token: 0x06000B8A RID: 2954 RVA: 0x000301AC File Offset: 0x0002E3AC
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

		// Token: 0x06000B8B RID: 2955 RVA: 0x000302BC File Offset: 0x0002E4BC
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

		// Token: 0x06000B8C RID: 2956 RVA: 0x00030328 File Offset: 0x0002E528
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
							spitemVM.ItemCost = this._inventoryLogic.GetItemPrice(spitemVM.ItemRosterElement, transferCommandResult.ResultSide == InventoryLogic.InventorySide.OtherInventory);
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

		// Token: 0x06000B8D RID: 2957 RVA: 0x00030858 File Offset: 0x0002EA58
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

		// Token: 0x06000B8E RID: 2958 RVA: 0x00030944 File Offset: 0x0002EB44
		private void CheckEquipAfterTransferStack()
		{
			while (this._equipAfterTransferStack.Count > 0)
			{
				SPItemVM spitemVM = new SPItemVM();
				spitemVM.RefreshWith(this._equipAfterTransferStack.Pop(), InventoryLogic.InventorySide.PlayerInventory);
				this.EquipEquipment(spitemVM);
			}
		}

		// Token: 0x06000B8F RID: 2959 RVA: 0x00030980 File Offset: 0x0002EB80
		private void RefreshInformationValues()
		{
			int inventoryCapacity = PartyBase.MainParty.InventoryCapacity;
			int num = MathF.Ceiling(this._equipmentCount);
			GameTexts.SetVariable("LEFT", num.ToString());
			GameTexts.SetVariable("RIGHT", inventoryCapacity.ToString());
			MBTextManager.SetTextVariable("newline", "\n", false);
			this.EquipmentCountText = GameTexts.FindText("str_LEFT_over_RIGHT", null).ToString();
			this.EquipmentCountWarned = num > inventoryCapacity;
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

		// Token: 0x06000B90 RID: 2960 RVA: 0x00030AC8 File Offset: 0x0002ECC8
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

		// Token: 0x06000B91 RID: 2961 RVA: 0x00030D60 File Offset: 0x0002EF60
		private bool CanCharacterUserItemBasedOnUsability(ItemRosterElement itemRosterElement)
		{
			return !itemRosterElement.EquipmentElement.Item.HasHorseComponent || itemRosterElement.EquipmentElement.Item.HorseComponent.IsRideable;
		}

		// Token: 0x06000B92 RID: 2962 RVA: 0x00030DA1 File Offset: 0x0002EFA1
		private bool CanCharacterUseItemBasedOnSkills(ItemRosterElement itemRosterElement)
		{
			return CharacterHelper.CanUseItemBasedOnSkill(this._currentCharacter, itemRosterElement.EquipmentElement);
		}

		// Token: 0x06000B93 RID: 2963 RVA: 0x00030DB8 File Offset: 0x0002EFB8
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

		// Token: 0x06000B94 RID: 2964 RVA: 0x00030FDC File Offset: 0x0002F1DC
		private void UnequipEquipment(SPItemVM itemVM)
		{
			if (itemVM == null || string.IsNullOrEmpty(itemVM.StringId))
			{
				return;
			}
			TransferCommand transferCommand = TransferCommand.Transfer(1, InventoryLogic.InventorySide.Equipment, InventoryLogic.InventorySide.PlayerInventory, itemVM.ItemRosterElement, itemVM.ItemType, itemVM.ItemType, this._currentCharacter, !this.IsInWarSet);
			this._inventoryLogic.AddTransferCommand(transferCommand);
		}

		// Token: 0x06000B95 RID: 2965 RVA: 0x00031030 File Offset: 0x0002F230
		private void UpdateEquipment(Equipment equipment, SPItemVM itemVM, EquipmentIndex itemType)
		{
			if (this.ActiveEquipment == equipment)
			{
				this.RefreshEquipment(itemVM, itemType);
			}
			equipment[itemType] = ((itemVM == null) ? default(EquipmentElement) : itemVM.ItemRosterElement.EquipmentElement);
		}

		// Token: 0x06000B96 RID: 2966 RVA: 0x00031070 File Offset: 0x0002F270
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

		// Token: 0x06000B97 RID: 2967 RVA: 0x00031174 File Offset: 0x0002F374
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

		// Token: 0x06000B98 RID: 2968 RVA: 0x00031268 File Offset: 0x0002F468
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

		// Token: 0x06000B99 RID: 2969 RVA: 0x0003138C File Offset: 0x0002F58C
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

		// Token: 0x06000B9A RID: 2970 RVA: 0x000313E8 File Offset: 0x0002F5E8
		private void UpdateLeftCharacter()
		{
			this.IsTradingWithSettlement = false;
			if (this._inventoryLogic.LeftRosterName != null)
			{
				this.LeftInventoryOwnerName = this._inventoryLogic.LeftRosterName.ToString();
				return;
			}
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

		// Token: 0x06000B9B RID: 2971 RVA: 0x0003151C File Offset: 0x0002F71C
		private void UpdateRightCharacter()
		{
			this.UpdateCharacterEquipment();
			this.UpdateCharacterArmorValues();
			this.RefreshCharacterTotalWeight();
			this.RefreshCharacterCanUseItem();
			this.CurrentCharacterName = this._currentCharacter.Name.ToString();
			this.RightInventoryOwnerGold = Hero.MainHero.Gold - this._inventoryLogic.TotalAmount;
		}

		// Token: 0x06000B9C RID: 2972 RVA: 0x00031574 File Offset: 0x0002F774
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

		// Token: 0x06000B9D RID: 2973 RVA: 0x000315E4 File Offset: 0x0002F7E4
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

		// Token: 0x06000B9E RID: 2974 RVA: 0x0003177C File Offset: 0x0002F97C
		private void UpdateCharacterArmorValues()
		{
			this.CurrentCharacterArmArmor = this._currentCharacter.GetArmArmorSum(!this.IsInWarSet);
			this.CurrentCharacterBodyArmor = this._currentCharacter.GetBodyArmorSum(!this.IsInWarSet);
			this.CurrentCharacterHeadArmor = this._currentCharacter.GetHeadArmorSum(!this.IsInWarSet);
			this.CurrentCharacterLegArmor = this._currentCharacter.GetLegArmorSum(!this.IsInWarSet);
			this.CurrentCharacterHorseArmor = this._currentCharacter.GetHorseArmorSum(!this.IsInWarSet);
		}

		// Token: 0x06000B9F RID: 2975 RVA: 0x0003180C File Offset: 0x0002FA0C
		private void RefreshCharacterTotalWeight()
		{
			CharacterObject currentCharacter = this._currentCharacter;
			float num = ((currentCharacter != null && currentCharacter.GetPerkValue(DefaultPerks.Athletics.FormFittingArmor)) ? (1f + DefaultPerks.Athletics.FormFittingArmor.PrimaryBonus / 100f) : 1f);
			this.CurrentCharacterTotalEncumbrance = MathF.Round(this.ActiveEquipment.GetTotalWeightOfWeapons() + this.ActiveEquipment.GetTotalWeightOfArmor(true) * num, 1).ToString("0.0");
		}

		// Token: 0x06000BA0 RID: 2976 RVA: 0x00031884 File Offset: 0x0002FA84
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

		// Token: 0x06000BA1 RID: 2977 RVA: 0x00031910 File Offset: 0x0002FB10
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

		// Token: 0x06000BA2 RID: 2978 RVA: 0x00031BCC File Offset: 0x0002FDCC
		private bool IsItemLocked(ItemRosterElement item)
		{
			string text = item.EquipmentElement.Item.StringId;
			if (item.EquipmentElement.ItemModifier != null)
			{
				text += item.EquipmentElement.ItemModifier.StringId;
			}
			return this._lockedItemIDs.Contains(text);
		}

		// Token: 0x06000BA3 RID: 2979 RVA: 0x00031C26 File Offset: 0x0002FE26
		public void CompareNextItem()
		{
			this.CycleBetweenWeaponSlots();
			this.RefreshComparedItem();
		}

		// Token: 0x06000BA4 RID: 2980 RVA: 0x00031C34 File Offset: 0x0002FE34
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

		// Token: 0x06000BA5 RID: 2981 RVA: 0x00031D58 File Offset: 0x0002FF58
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

		// Token: 0x06000BA6 RID: 2982 RVA: 0x00031DEC File Offset: 0x0002FFEC
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

		// Token: 0x06000BA7 RID: 2983 RVA: 0x00031E40 File Offset: 0x00030040
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

		// Token: 0x06000BA8 RID: 2984 RVA: 0x00031E94 File Offset: 0x00030094
		private void TransferAll(bool isBuy)
		{
			this.IsRefreshed = false;
			List<TransferCommand> list = new List<TransferCommand>(this.LeftItemListVM.Count);
			MBBindingList<SPItemVM> mbbindingList = (isBuy ? this.LeftItemListVM : this.RightItemListVM);
			List<SPItemVM> list2 = new List<SPItemVM>();
			object obj;
			if (!isBuy)
			{
				PartyBase oppositePartyFromListener = this._inventoryLogic.OppositePartyFromListener;
				obj = ((oppositePartyFromListener != null) ? oppositePartyFromListener.MobileParty : null);
			}
			else
			{
				obj = MobileParty.MainParty;
			}
			float num = 0f;
			object obj2 = obj;
			int? num2 = ((obj2 != null) ? new int?(obj2.InventoryCapacity) : null);
			float num3 = (((num2 != null) ? new float?((float)num2.GetValueOrDefault()) : null) - (isBuy ? this._equipmentCount : 0f)) ?? 0f;
			SPItemVM spitemVM = mbbindingList.FirstOrDefault((SPItemVM x) => !x.IsFiltered && !x.IsLocked);
			float num4 = ((spitemVM != null) ? spitemVM.ItemRosterElement.EquipmentElement.GetEquipmentElementWeight() : 0f);
			bool flag = !isBuy || num3 <= num4;
			InventoryLogic.InventorySide inventorySide = (isBuy ? InventoryLogic.InventorySide.OtherInventory : InventoryLogic.InventorySide.PlayerInventory);
			InventoryLogic.InventorySide inventorySide2 = (isBuy ? InventoryLogic.InventorySide.PlayerInventory : InventoryLogic.InventorySide.OtherInventory);
			for (int i = 0; i < mbbindingList.Count; i++)
			{
				SPItemVM spitemVM2 = mbbindingList[i];
				if (spitemVM2 != null && !spitemVM2.IsFiltered && spitemVM2 != null && !spitemVM2.IsLocked && spitemVM2 != null && spitemVM2.IsTransferable)
				{
					int num5 = spitemVM2.ItemRosterElement.Amount;
					if (!flag)
					{
						float equipmentElementWeight = spitemVM2.ItemRosterElement.EquipmentElement.GetEquipmentElementWeight();
						float num6 = equipmentElementWeight * (float)num5;
						if (spitemVM2.ItemRosterElement.EquipmentElement.Item.HasHorseComponent)
						{
							list2.Add(spitemVM2);
							goto IL_256;
						}
						num += num6;
						if (num5 > 0 && num > num3)
						{
							num5 = MBMath.ClampInt(num5, 0, num5 - MathF.Ceiling((num - num3) / equipmentElementWeight));
							i = mbbindingList.Count;
						}
					}
					if (num5 > 0)
					{
						TransferCommand transferCommand = TransferCommand.Transfer(num5, inventorySide, inventorySide2, spitemVM2.ItemRosterElement, EquipmentIndex.None, EquipmentIndex.None, this._currentCharacter, !this.IsInWarSet);
						list.Add(transferCommand);
					}
				}
				IL_256:;
			}
			if (num < num3)
			{
				foreach (SPItemVM spitemVM3 in list2)
				{
					TransferCommand transferCommand2 = TransferCommand.Transfer(spitemVM3.ItemRosterElement.Amount, inventorySide, inventorySide2, spitemVM3.ItemRosterElement, EquipmentIndex.None, EquipmentIndex.None, this._currentCharacter, !this.IsInWarSet);
					list.Add(transferCommand2);
				}
			}
			this._inventoryLogic.AddTransferCommands(list);
			this.RefreshInformationValues();
			this.ExecuteRemoveZeroCounts();
			this.IsRefreshed = true;
		}

		// Token: 0x06000BA9 RID: 2985 RVA: 0x000321A4 File Offset: 0x000303A4
		public void ExecuteBuyAllItems()
		{
			this.TransferAll(true);
		}

		// Token: 0x06000BAA RID: 2986 RVA: 0x000321AD File Offset: 0x000303AD
		public void ExecuteSellAllItems()
		{
			this.TransferAll(false);
		}

		// Token: 0x06000BAB RID: 2987 RVA: 0x000321B8 File Offset: 0x000303B8
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

		// Token: 0x06000BAC RID: 2988 RVA: 0x0003225C File Offset: 0x0003045C
		public void ExecuteResetTranstactions()
		{
			this._inventoryLogic.Reset(false);
			InformationManager.DisplayMessage(new InformationMessage(GameTexts.FindText("str_inventory_reset_message", null).ToString()));
			this.CurrentFocusedItem = null;
		}

		// Token: 0x06000BAD RID: 2989 RVA: 0x0003228C File Offset: 0x0003048C
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

		// Token: 0x06000BAE RID: 2990 RVA: 0x0003231C File Offset: 0x0003051C
		public void ExecuteCompleteTranstactions()
		{
			if (InventoryManager.Instance.CurrentMode == InventoryMode.Loot && !this._inventoryLogic.IsThereAnyChanges() && this._inventoryLogic.GetElementsInInitialRoster(InventoryLogic.InventorySide.OtherInventory).Any<ItemRosterElement>())
			{
				InformationManager.ShowInquiry(new InquiryData("", GameTexts.FindText("str_leaving_loot_behind", null).ToString(), true, true, GameTexts.FindText("str_yes", null).ToString(), GameTexts.FindText("str_no", null).ToString(), new Action(this.HandleDone), null, "", 0f, null, null, null), false, false);
				return;
			}
			this.HandleDone();
		}

		// Token: 0x06000BAF RID: 2991 RVA: 0x000323BC File Offset: 0x000305BC
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

		// Token: 0x06000BB0 RID: 2992 RVA: 0x00032555 File Offset: 0x00030755
		private void SaveItemLockStates()
		{
			this._viewDataTracker.SetInventoryLocks(this._lockedItemIDs);
		}

		// Token: 0x06000BB1 RID: 2993 RVA: 0x00032568 File Offset: 0x00030768
		private void SaveItemSortStates()
		{
			this._viewDataTracker.InventorySetSortPreference((int)this._usageType, (int)this.PlayerInventorySortController.CurrentSortOption.Value, (int)this.PlayerInventorySortController.CurrentSortState.Value);
		}

		// Token: 0x06000BB2 RID: 2994 RVA: 0x000325AC File Offset: 0x000307AC
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

		// Token: 0x06000BB3 RID: 2995 RVA: 0x000326EA File Offset: 0x000308EA
		private void UpdateIsDoneDisabled()
		{
			this.IsDoneDisabled = !this._inventoryLogic.CanPlayerCompleteTransaction();
		}

		// Token: 0x06000BB4 RID: 2996 RVA: 0x00032700 File Offset: 0x00030900
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

		// Token: 0x06000BB5 RID: 2997 RVA: 0x000327A4 File Offset: 0x000309A4
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

		// Token: 0x06000BB6 RID: 2998 RVA: 0x0003282F File Offset: 0x00030A2F
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

		// Token: 0x06000BB7 RID: 2999 RVA: 0x0003285B File Offset: 0x00030A5B
		public void ExecuteFilterNone()
		{
			this.ProcessFilter(SPInventoryVM.Filters.All);
			Game.Current.EventManager.TriggerEvent<InventoryFilterChangedEvent>(new InventoryFilterChangedEvent(SPInventoryVM.Filters.All));
		}

		// Token: 0x06000BB8 RID: 3000 RVA: 0x00032879 File Offset: 0x00030A79
		public void ExecuteFilterWeapons()
		{
			this.ProcessFilter(SPInventoryVM.Filters.Weapons);
			Game.Current.EventManager.TriggerEvent<InventoryFilterChangedEvent>(new InventoryFilterChangedEvent(SPInventoryVM.Filters.Weapons));
		}

		// Token: 0x06000BB9 RID: 3001 RVA: 0x00032897 File Offset: 0x00030A97
		public void ExecuteFilterArmors()
		{
			this.ProcessFilter(SPInventoryVM.Filters.Armors);
			Game.Current.EventManager.TriggerEvent<InventoryFilterChangedEvent>(new InventoryFilterChangedEvent(SPInventoryVM.Filters.Armors));
		}

		// Token: 0x06000BBA RID: 3002 RVA: 0x000328B5 File Offset: 0x00030AB5
		public void ExecuteFilterShieldsAndRanged()
		{
			this.ProcessFilter(SPInventoryVM.Filters.ShieldsAndRanged);
			Game.Current.EventManager.TriggerEvent<InventoryFilterChangedEvent>(new InventoryFilterChangedEvent(SPInventoryVM.Filters.ShieldsAndRanged));
		}

		// Token: 0x06000BBB RID: 3003 RVA: 0x000328D3 File Offset: 0x00030AD3
		public void ExecuteFilterMounts()
		{
			this.ProcessFilter(SPInventoryVM.Filters.Mounts);
			Game.Current.EventManager.TriggerEvent<InventoryFilterChangedEvent>(new InventoryFilterChangedEvent(SPInventoryVM.Filters.Mounts));
		}

		// Token: 0x06000BBC RID: 3004 RVA: 0x000328F1 File Offset: 0x00030AF1
		public void ExecuteFilterMisc()
		{
			this.ProcessFilter(SPInventoryVM.Filters.Miscellaneous);
			Game.Current.EventManager.TriggerEvent<InventoryFilterChangedEvent>(new InventoryFilterChangedEvent(SPInventoryVM.Filters.Miscellaneous));
		}

		// Token: 0x06000BBD RID: 3005 RVA: 0x00032910 File Offset: 0x00030B10
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

		// Token: 0x06000BBE RID: 3006 RVA: 0x0003297C File Offset: 0x00030B7C
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

		// Token: 0x06000BBF RID: 3007 RVA: 0x00032A18 File Offset: 0x00030C18
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
					if (obj.NewNotificationElementID != "InventoryOtherBannerItems" && this._isBannerItemsHighlightApplied)
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
					if (!this._isBannerItemsHighlightApplied && this._latestTutorialElementID == "InventoryOtherBannerItems")
					{
						this._isBannerItemsHighlightApplied = true;
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
					if (this._isBannerItemsHighlightApplied)
					{
						this.SetBannerItemsHighlightState(false);
						this._isBannerItemsHighlightApplied = false;
					}
				}
			}
		}

		// Token: 0x06000BC0 RID: 3008 RVA: 0x00032BD8 File Offset: 0x00030DD8
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

		// Token: 0x06000BC1 RID: 3009 RVA: 0x00032C2C File Offset: 0x00030E2C
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

		// Token: 0x170003B1 RID: 945
		// (get) Token: 0x06000BC2 RID: 3010 RVA: 0x00032C7D File Offset: 0x00030E7D
		// (set) Token: 0x06000BC3 RID: 3011 RVA: 0x00032C85 File Offset: 0x00030E85
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

		// Token: 0x170003B2 RID: 946
		// (get) Token: 0x06000BC4 RID: 3012 RVA: 0x00032CA3 File Offset: 0x00030EA3
		// (set) Token: 0x06000BC5 RID: 3013 RVA: 0x00032CAB File Offset: 0x00030EAB
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

		// Token: 0x170003B3 RID: 947
		// (get) Token: 0x06000BC6 RID: 3014 RVA: 0x00032CCE File Offset: 0x00030ECE
		// (set) Token: 0x06000BC7 RID: 3015 RVA: 0x00032CD6 File Offset: 0x00030ED6
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

		// Token: 0x170003B4 RID: 948
		// (get) Token: 0x06000BC8 RID: 3016 RVA: 0x00032CF9 File Offset: 0x00030EF9
		// (set) Token: 0x06000BC9 RID: 3017 RVA: 0x00032D01 File Offset: 0x00030F01
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

		// Token: 0x170003B5 RID: 949
		// (get) Token: 0x06000BCA RID: 3018 RVA: 0x00032D24 File Offset: 0x00030F24
		// (set) Token: 0x06000BCB RID: 3019 RVA: 0x00032D2C File Offset: 0x00030F2C
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

		// Token: 0x170003B6 RID: 950
		// (get) Token: 0x06000BCC RID: 3020 RVA: 0x00032D4A File Offset: 0x00030F4A
		// (set) Token: 0x06000BCD RID: 3021 RVA: 0x00032D52 File Offset: 0x00030F52
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

		// Token: 0x170003B7 RID: 951
		// (get) Token: 0x06000BCE RID: 3022 RVA: 0x00032D89 File Offset: 0x00030F89
		// (set) Token: 0x06000BCF RID: 3023 RVA: 0x00032D91 File Offset: 0x00030F91
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

		// Token: 0x170003B8 RID: 952
		// (get) Token: 0x06000BD0 RID: 3024 RVA: 0x00032DAF File Offset: 0x00030FAF
		// (set) Token: 0x06000BD1 RID: 3025 RVA: 0x00032DB7 File Offset: 0x00030FB7
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

		// Token: 0x170003B9 RID: 953
		// (get) Token: 0x06000BD2 RID: 3026 RVA: 0x00032DDA File Offset: 0x00030FDA
		// (set) Token: 0x06000BD3 RID: 3027 RVA: 0x00032DE2 File Offset: 0x00030FE2
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

		// Token: 0x170003BA RID: 954
		// (get) Token: 0x06000BD4 RID: 3028 RVA: 0x00032E05 File Offset: 0x00031005
		// (set) Token: 0x06000BD5 RID: 3029 RVA: 0x00032E0D File Offset: 0x0003100D
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

		// Token: 0x170003BB RID: 955
		// (get) Token: 0x06000BD6 RID: 3030 RVA: 0x00032E30 File Offset: 0x00031030
		// (set) Token: 0x06000BD7 RID: 3031 RVA: 0x00032E38 File Offset: 0x00031038
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

		// Token: 0x170003BC RID: 956
		// (get) Token: 0x06000BD8 RID: 3032 RVA: 0x00032E5B File Offset: 0x0003105B
		// (set) Token: 0x06000BD9 RID: 3033 RVA: 0x00032E63 File Offset: 0x00031063
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

		// Token: 0x170003BD RID: 957
		// (get) Token: 0x06000BDA RID: 3034 RVA: 0x00032E86 File Offset: 0x00031086
		// (set) Token: 0x06000BDB RID: 3035 RVA: 0x00032E8E File Offset: 0x0003108E
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

		// Token: 0x170003BE RID: 958
		// (get) Token: 0x06000BDC RID: 3036 RVA: 0x00032EB1 File Offset: 0x000310B1
		// (set) Token: 0x06000BDD RID: 3037 RVA: 0x00032EB9 File Offset: 0x000310B9
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

		// Token: 0x170003BF RID: 959
		// (get) Token: 0x06000BDE RID: 3038 RVA: 0x00032EDC File Offset: 0x000310DC
		// (set) Token: 0x06000BDF RID: 3039 RVA: 0x00032EE4 File Offset: 0x000310E4
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

		// Token: 0x170003C0 RID: 960
		// (get) Token: 0x06000BE0 RID: 3040 RVA: 0x00032F02 File Offset: 0x00031102
		// (set) Token: 0x06000BE1 RID: 3041 RVA: 0x00032F0A File Offset: 0x0003110A
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

		// Token: 0x170003C1 RID: 961
		// (get) Token: 0x06000BE2 RID: 3042 RVA: 0x00032F28 File Offset: 0x00031128
		// (set) Token: 0x06000BE3 RID: 3043 RVA: 0x00032F30 File Offset: 0x00031130
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

		// Token: 0x170003C2 RID: 962
		// (get) Token: 0x06000BE4 RID: 3044 RVA: 0x00032F4E File Offset: 0x0003114E
		// (set) Token: 0x06000BE5 RID: 3045 RVA: 0x00032F56 File Offset: 0x00031156
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

		// Token: 0x170003C3 RID: 963
		// (get) Token: 0x06000BE6 RID: 3046 RVA: 0x00032F74 File Offset: 0x00031174
		// (set) Token: 0x06000BE7 RID: 3047 RVA: 0x00032F7C File Offset: 0x0003117C
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

		// Token: 0x170003C4 RID: 964
		// (get) Token: 0x06000BE8 RID: 3048 RVA: 0x00032F9A File Offset: 0x0003119A
		// (set) Token: 0x06000BE9 RID: 3049 RVA: 0x00032FA2 File Offset: 0x000311A2
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

		// Token: 0x170003C5 RID: 965
		// (get) Token: 0x06000BEA RID: 3050 RVA: 0x00032FC0 File Offset: 0x000311C0
		// (set) Token: 0x06000BEB RID: 3051 RVA: 0x00032FC8 File Offset: 0x000311C8
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

		// Token: 0x170003C6 RID: 966
		// (get) Token: 0x06000BEC RID: 3052 RVA: 0x00032FE6 File Offset: 0x000311E6
		// (set) Token: 0x06000BED RID: 3053 RVA: 0x00032FEE File Offset: 0x000311EE
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

		// Token: 0x170003C7 RID: 967
		// (get) Token: 0x06000BEE RID: 3054 RVA: 0x0003300C File Offset: 0x0003120C
		// (set) Token: 0x06000BEF RID: 3055 RVA: 0x00033014 File Offset: 0x00031214
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

		// Token: 0x170003C8 RID: 968
		// (get) Token: 0x06000BF0 RID: 3056 RVA: 0x00033032 File Offset: 0x00031232
		// (set) Token: 0x06000BF1 RID: 3057 RVA: 0x0003303A File Offset: 0x0003123A
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

		// Token: 0x170003C9 RID: 969
		// (get) Token: 0x06000BF2 RID: 3058 RVA: 0x00033058 File Offset: 0x00031258
		// (set) Token: 0x06000BF3 RID: 3059 RVA: 0x00033060 File Offset: 0x00031260
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

		// Token: 0x170003CA RID: 970
		// (get) Token: 0x06000BF4 RID: 3060 RVA: 0x0003307E File Offset: 0x0003127E
		// (set) Token: 0x06000BF5 RID: 3061 RVA: 0x00033086 File Offset: 0x00031286
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

		// Token: 0x170003CB RID: 971
		// (get) Token: 0x06000BF6 RID: 3062 RVA: 0x000330A4 File Offset: 0x000312A4
		// (set) Token: 0x06000BF7 RID: 3063 RVA: 0x000330AC File Offset: 0x000312AC
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

		// Token: 0x170003CC RID: 972
		// (get) Token: 0x06000BF8 RID: 3064 RVA: 0x000330CA File Offset: 0x000312CA
		// (set) Token: 0x06000BF9 RID: 3065 RVA: 0x000330D2 File Offset: 0x000312D2
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

		// Token: 0x170003CD RID: 973
		// (get) Token: 0x06000BFA RID: 3066 RVA: 0x000330F0 File Offset: 0x000312F0
		// (set) Token: 0x06000BFB RID: 3067 RVA: 0x000330F8 File Offset: 0x000312F8
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

		// Token: 0x170003CE RID: 974
		// (get) Token: 0x06000BFC RID: 3068 RVA: 0x00033116 File Offset: 0x00031316
		// (set) Token: 0x06000BFD RID: 3069 RVA: 0x0003311E File Offset: 0x0003131E
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

		// Token: 0x170003CF RID: 975
		// (get) Token: 0x06000BFE RID: 3070 RVA: 0x0003313C File Offset: 0x0003133C
		// (set) Token: 0x06000BFF RID: 3071 RVA: 0x00033144 File Offset: 0x00031344
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

		// Token: 0x170003D0 RID: 976
		// (get) Token: 0x06000C00 RID: 3072 RVA: 0x00033162 File Offset: 0x00031362
		// (set) Token: 0x06000C01 RID: 3073 RVA: 0x0003316A File Offset: 0x0003136A
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

		// Token: 0x170003D1 RID: 977
		// (get) Token: 0x06000C02 RID: 3074 RVA: 0x00033188 File Offset: 0x00031388
		// (set) Token: 0x06000C03 RID: 3075 RVA: 0x00033190 File Offset: 0x00031390
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

		// Token: 0x170003D2 RID: 978
		// (get) Token: 0x06000C04 RID: 3076 RVA: 0x000331AE File Offset: 0x000313AE
		// (set) Token: 0x06000C05 RID: 3077 RVA: 0x000331B6 File Offset: 0x000313B6
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

		// Token: 0x170003D3 RID: 979
		// (get) Token: 0x06000C06 RID: 3078 RVA: 0x000331D4 File Offset: 0x000313D4
		// (set) Token: 0x06000C07 RID: 3079 RVA: 0x000331DC File Offset: 0x000313DC
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

		// Token: 0x170003D4 RID: 980
		// (get) Token: 0x06000C08 RID: 3080 RVA: 0x000331FA File Offset: 0x000313FA
		// (set) Token: 0x06000C09 RID: 3081 RVA: 0x00033202 File Offset: 0x00031402
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

		// Token: 0x170003D5 RID: 981
		// (get) Token: 0x06000C0A RID: 3082 RVA: 0x00033220 File Offset: 0x00031420
		// (set) Token: 0x06000C0B RID: 3083 RVA: 0x00033228 File Offset: 0x00031428
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

		// Token: 0x170003D6 RID: 982
		// (get) Token: 0x06000C0C RID: 3084 RVA: 0x00033246 File Offset: 0x00031446
		// (set) Token: 0x06000C0D RID: 3085 RVA: 0x0003324E File Offset: 0x0003144E
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

		// Token: 0x170003D7 RID: 983
		// (get) Token: 0x06000C0E RID: 3086 RVA: 0x0003326C File Offset: 0x0003146C
		// (set) Token: 0x06000C0F RID: 3087 RVA: 0x00033274 File Offset: 0x00031474
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

		// Token: 0x170003D8 RID: 984
		// (get) Token: 0x06000C10 RID: 3088 RVA: 0x00033292 File Offset: 0x00031492
		// (set) Token: 0x06000C11 RID: 3089 RVA: 0x0003329A File Offset: 0x0003149A
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

		// Token: 0x170003D9 RID: 985
		// (get) Token: 0x06000C12 RID: 3090 RVA: 0x000332B8 File Offset: 0x000314B8
		// (set) Token: 0x06000C13 RID: 3091 RVA: 0x000332C0 File Offset: 0x000314C0
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

		// Token: 0x170003DA RID: 986
		// (get) Token: 0x06000C14 RID: 3092 RVA: 0x000332DE File Offset: 0x000314DE
		// (set) Token: 0x06000C15 RID: 3093 RVA: 0x000332E6 File Offset: 0x000314E6
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

		// Token: 0x170003DB RID: 987
		// (get) Token: 0x06000C16 RID: 3094 RVA: 0x00033304 File Offset: 0x00031504
		// (set) Token: 0x06000C17 RID: 3095 RVA: 0x0003330C File Offset: 0x0003150C
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

		// Token: 0x170003DC RID: 988
		// (get) Token: 0x06000C18 RID: 3096 RVA: 0x0003332A File Offset: 0x0003152A
		// (set) Token: 0x06000C19 RID: 3097 RVA: 0x00033332 File Offset: 0x00031532
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

		// Token: 0x170003DD RID: 989
		// (get) Token: 0x06000C1A RID: 3098 RVA: 0x00033350 File Offset: 0x00031550
		// (set) Token: 0x06000C1B RID: 3099 RVA: 0x00033358 File Offset: 0x00031558
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

		// Token: 0x170003DE RID: 990
		// (get) Token: 0x06000C1C RID: 3100 RVA: 0x00033376 File Offset: 0x00031576
		// (set) Token: 0x06000C1D RID: 3101 RVA: 0x0003337E File Offset: 0x0003157E
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

		// Token: 0x170003DF RID: 991
		// (get) Token: 0x06000C1E RID: 3102 RVA: 0x0003339C File Offset: 0x0003159C
		// (set) Token: 0x06000C1F RID: 3103 RVA: 0x000333A4 File Offset: 0x000315A4
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

		// Token: 0x170003E0 RID: 992
		// (get) Token: 0x06000C20 RID: 3104 RVA: 0x000333C2 File Offset: 0x000315C2
		// (set) Token: 0x06000C21 RID: 3105 RVA: 0x000333CA File Offset: 0x000315CA
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

		// Token: 0x170003E1 RID: 993
		// (get) Token: 0x06000C22 RID: 3106 RVA: 0x000333E8 File Offset: 0x000315E8
		// (set) Token: 0x06000C23 RID: 3107 RVA: 0x000333F0 File Offset: 0x000315F0
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

		// Token: 0x170003E2 RID: 994
		// (get) Token: 0x06000C24 RID: 3108 RVA: 0x0003340E File Offset: 0x0003160E
		// (set) Token: 0x06000C25 RID: 3109 RVA: 0x00033416 File Offset: 0x00031616
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

		// Token: 0x170003E3 RID: 995
		// (get) Token: 0x06000C26 RID: 3110 RVA: 0x00033434 File Offset: 0x00031634
		// (set) Token: 0x06000C27 RID: 3111 RVA: 0x0003343C File Offset: 0x0003163C
		[DataSourceProperty]
		public HintViewModel CapacityExceededHint
		{
			get
			{
				return this._capacityExceededHint;
			}
			set
			{
				if (value != this._capacityExceededHint)
				{
					this._capacityExceededHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "CapacityExceededHint");
				}
			}
		}

		// Token: 0x170003E4 RID: 996
		// (get) Token: 0x06000C28 RID: 3112 RVA: 0x0003345A File Offset: 0x0003165A
		// (set) Token: 0x06000C29 RID: 3113 RVA: 0x00033462 File Offset: 0x00031662
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

		// Token: 0x170003E5 RID: 997
		// (get) Token: 0x06000C2A RID: 3114 RVA: 0x00033480 File Offset: 0x00031680
		// (set) Token: 0x06000C2B RID: 3115 RVA: 0x00033488 File Offset: 0x00031688
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

		// Token: 0x170003E6 RID: 998
		// (get) Token: 0x06000C2C RID: 3116 RVA: 0x000334A6 File Offset: 0x000316A6
		// (set) Token: 0x06000C2D RID: 3117 RVA: 0x000334AE File Offset: 0x000316AE
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

		// Token: 0x170003E7 RID: 999
		// (get) Token: 0x06000C2E RID: 3118 RVA: 0x000334CC File Offset: 0x000316CC
		// (set) Token: 0x06000C2F RID: 3119 RVA: 0x000334D4 File Offset: 0x000316D4
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

		// Token: 0x170003E8 RID: 1000
		// (get) Token: 0x06000C30 RID: 3120 RVA: 0x000334F2 File Offset: 0x000316F2
		// (set) Token: 0x06000C31 RID: 3121 RVA: 0x000334FA File Offset: 0x000316FA
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

		// Token: 0x170003E9 RID: 1001
		// (get) Token: 0x06000C32 RID: 3122 RVA: 0x00033518 File Offset: 0x00031718
		// (set) Token: 0x06000C33 RID: 3123 RVA: 0x00033520 File Offset: 0x00031720
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

		// Token: 0x170003EA RID: 1002
		// (get) Token: 0x06000C34 RID: 3124 RVA: 0x0003353E File Offset: 0x0003173E
		// (set) Token: 0x06000C35 RID: 3125 RVA: 0x00033546 File Offset: 0x00031746
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

		// Token: 0x170003EB RID: 1003
		// (get) Token: 0x06000C36 RID: 3126 RVA: 0x00033564 File Offset: 0x00031764
		// (set) Token: 0x06000C37 RID: 3127 RVA: 0x0003356C File Offset: 0x0003176C
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

		// Token: 0x170003EC RID: 1004
		// (get) Token: 0x06000C38 RID: 3128 RVA: 0x000335A5 File Offset: 0x000317A5
		// (set) Token: 0x06000C39 RID: 3129 RVA: 0x000335AD File Offset: 0x000317AD
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

		// Token: 0x170003ED RID: 1005
		// (get) Token: 0x06000C3A RID: 3130 RVA: 0x000335CB File Offset: 0x000317CB
		// (set) Token: 0x06000C3B RID: 3131 RVA: 0x000335D3 File Offset: 0x000317D3
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

		// Token: 0x170003EE RID: 1006
		// (get) Token: 0x06000C3C RID: 3132 RVA: 0x000335F1 File Offset: 0x000317F1
		// (set) Token: 0x06000C3D RID: 3133 RVA: 0x000335F9 File Offset: 0x000317F9
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

		// Token: 0x170003EF RID: 1007
		// (get) Token: 0x06000C3E RID: 3134 RVA: 0x00033617 File Offset: 0x00031817
		// (set) Token: 0x06000C3F RID: 3135 RVA: 0x0003361F File Offset: 0x0003181F
		[DataSourceProperty]
		public string CapacityExceededText
		{
			get
			{
				return this._capacityExceededText;
			}
			set
			{
				if (value != this._capacityExceededText)
				{
					this._capacityExceededText = value;
					base.OnPropertyChangedWithValue<string>(value, "CapacityExceededText");
				}
			}
		}

		// Token: 0x170003F0 RID: 1008
		// (get) Token: 0x06000C40 RID: 3136 RVA: 0x00033642 File Offset: 0x00031842
		// (set) Token: 0x06000C41 RID: 3137 RVA: 0x0003364A File Offset: 0x0003184A
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

		// Token: 0x170003F1 RID: 1009
		// (get) Token: 0x06000C42 RID: 3138 RVA: 0x00033674 File Offset: 0x00031874
		// (set) Token: 0x06000C43 RID: 3139 RVA: 0x0003367C File Offset: 0x0003187C
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

		// Token: 0x170003F2 RID: 1010
		// (get) Token: 0x06000C44 RID: 3140 RVA: 0x000336A6 File Offset: 0x000318A6
		// (set) Token: 0x06000C45 RID: 3141 RVA: 0x000336AE File Offset: 0x000318AE
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

		// Token: 0x170003F3 RID: 1011
		// (get) Token: 0x06000C46 RID: 3142 RVA: 0x000336CC File Offset: 0x000318CC
		// (set) Token: 0x06000C47 RID: 3143 RVA: 0x000336D4 File Offset: 0x000318D4
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

		// Token: 0x170003F4 RID: 1012
		// (get) Token: 0x06000C48 RID: 3144 RVA: 0x000336F2 File Offset: 0x000318F2
		// (set) Token: 0x06000C49 RID: 3145 RVA: 0x000336FA File Offset: 0x000318FA
		[DataSourceProperty]
		public bool EquipmentCountWarned
		{
			get
			{
				return this._equipmentCountWarned;
			}
			set
			{
				if (value != this._equipmentCountWarned)
				{
					this._equipmentCountWarned = value;
					base.OnPropertyChangedWithValue(value, "EquipmentCountWarned");
				}
			}
		}

		// Token: 0x170003F5 RID: 1013
		// (get) Token: 0x06000C4A RID: 3146 RVA: 0x00033718 File Offset: 0x00031918
		// (set) Token: 0x06000C4B RID: 3147 RVA: 0x00033720 File Offset: 0x00031920
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

		// Token: 0x170003F6 RID: 1014
		// (get) Token: 0x06000C4C RID: 3148 RVA: 0x0003373E File Offset: 0x0003193E
		// (set) Token: 0x06000C4D RID: 3149 RVA: 0x00033746 File Offset: 0x00031946
		[DataSourceProperty]
		public string EquipmentCountText
		{
			get
			{
				return this._equipmentCountText;
			}
			set
			{
				if (value != this._equipmentCountText)
				{
					this._equipmentCountText = value;
					base.OnPropertyChangedWithValue<string>(value, "EquipmentCountText");
				}
			}
		}

		// Token: 0x170003F7 RID: 1015
		// (get) Token: 0x06000C4E RID: 3150 RVA: 0x00033769 File Offset: 0x00031969
		// (set) Token: 0x06000C4F RID: 3151 RVA: 0x00033771 File Offset: 0x00031971
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

		// Token: 0x170003F8 RID: 1016
		// (get) Token: 0x06000C50 RID: 3152 RVA: 0x00033794 File Offset: 0x00031994
		// (set) Token: 0x06000C51 RID: 3153 RVA: 0x0003379C File Offset: 0x0003199C
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

		// Token: 0x170003F9 RID: 1017
		// (get) Token: 0x06000C52 RID: 3154 RVA: 0x000337BA File Offset: 0x000319BA
		// (set) Token: 0x06000C53 RID: 3155 RVA: 0x000337C2 File Offset: 0x000319C2
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

		// Token: 0x170003FA RID: 1018
		// (get) Token: 0x06000C54 RID: 3156 RVA: 0x000337DF File Offset: 0x000319DF
		// (set) Token: 0x06000C55 RID: 3157 RVA: 0x000337E7 File Offset: 0x000319E7
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

		// Token: 0x170003FB RID: 1019
		// (get) Token: 0x06000C56 RID: 3158 RVA: 0x0003380C File Offset: 0x00031A0C
		// (set) Token: 0x06000C57 RID: 3159 RVA: 0x00033814 File Offset: 0x00031A14
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

		// Token: 0x170003FC RID: 1020
		// (get) Token: 0x06000C58 RID: 3160 RVA: 0x00033832 File Offset: 0x00031A32
		// (set) Token: 0x06000C59 RID: 3161 RVA: 0x0003383A File Offset: 0x00031A3A
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

		// Token: 0x170003FD RID: 1021
		// (get) Token: 0x06000C5A RID: 3162 RVA: 0x00033858 File Offset: 0x00031A58
		// (set) Token: 0x06000C5B RID: 3163 RVA: 0x00033860 File Offset: 0x00031A60
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

		// Token: 0x170003FE RID: 1022
		// (get) Token: 0x06000C5C RID: 3164 RVA: 0x00033883 File Offset: 0x00031A83
		// (set) Token: 0x06000C5D RID: 3165 RVA: 0x0003388B File Offset: 0x00031A8B
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

		// Token: 0x170003FF RID: 1023
		// (get) Token: 0x06000C5E RID: 3166 RVA: 0x000338AE File Offset: 0x00031AAE
		// (set) Token: 0x06000C5F RID: 3167 RVA: 0x000338B6 File Offset: 0x00031AB6
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

		// Token: 0x17000400 RID: 1024
		// (get) Token: 0x06000C60 RID: 3168 RVA: 0x000338D9 File Offset: 0x00031AD9
		// (set) Token: 0x06000C61 RID: 3169 RVA: 0x000338E1 File Offset: 0x00031AE1
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

		// Token: 0x17000401 RID: 1025
		// (get) Token: 0x06000C62 RID: 3170 RVA: 0x00033904 File Offset: 0x00031B04
		// (set) Token: 0x06000C63 RID: 3171 RVA: 0x0003390C File Offset: 0x00031B0C
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

		// Token: 0x17000402 RID: 1026
		// (get) Token: 0x06000C64 RID: 3172 RVA: 0x0003392F File Offset: 0x00031B2F
		// (set) Token: 0x06000C65 RID: 3173 RVA: 0x00033937 File Offset: 0x00031B37
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

		// Token: 0x17000403 RID: 1027
		// (get) Token: 0x06000C66 RID: 3174 RVA: 0x00033955 File Offset: 0x00031B55
		// (set) Token: 0x06000C67 RID: 3175 RVA: 0x0003395D File Offset: 0x00031B5D
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

		// Token: 0x17000404 RID: 1028
		// (get) Token: 0x06000C68 RID: 3176 RVA: 0x0003397B File Offset: 0x00031B7B
		// (set) Token: 0x06000C69 RID: 3177 RVA: 0x00033983 File Offset: 0x00031B83
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

		// Token: 0x17000405 RID: 1029
		// (get) Token: 0x06000C6A RID: 3178 RVA: 0x000339A1 File Offset: 0x00031BA1
		// (set) Token: 0x06000C6B RID: 3179 RVA: 0x000339A9 File Offset: 0x00031BA9
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

		// Token: 0x17000406 RID: 1030
		// (get) Token: 0x06000C6C RID: 3180 RVA: 0x000339CC File Offset: 0x00031BCC
		// (set) Token: 0x06000C6D RID: 3181 RVA: 0x000339D4 File Offset: 0x00031BD4
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

		// Token: 0x17000407 RID: 1031
		// (get) Token: 0x06000C6E RID: 3182 RVA: 0x000339FD File Offset: 0x00031BFD
		// (set) Token: 0x06000C6F RID: 3183 RVA: 0x00033A05 File Offset: 0x00031C05
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

		// Token: 0x17000408 RID: 1032
		// (get) Token: 0x06000C70 RID: 3184 RVA: 0x00033A2E File Offset: 0x00031C2E
		// (set) Token: 0x06000C71 RID: 3185 RVA: 0x00033A36 File Offset: 0x00031C36
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

		// Token: 0x17000409 RID: 1033
		// (get) Token: 0x06000C72 RID: 3186 RVA: 0x00033A5F File Offset: 0x00031C5F
		// (set) Token: 0x06000C73 RID: 3187 RVA: 0x00033A67 File Offset: 0x00031C67
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

		// Token: 0x1700040A RID: 1034
		// (get) Token: 0x06000C74 RID: 3188 RVA: 0x00033A90 File Offset: 0x00031C90
		// (set) Token: 0x06000C75 RID: 3189 RVA: 0x00033A98 File Offset: 0x00031C98
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

		// Token: 0x1700040B RID: 1035
		// (get) Token: 0x06000C76 RID: 3190 RVA: 0x00033AC1 File Offset: 0x00031CC1
		// (set) Token: 0x06000C77 RID: 3191 RVA: 0x00033AC9 File Offset: 0x00031CC9
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

		// Token: 0x1700040C RID: 1036
		// (get) Token: 0x06000C78 RID: 3192 RVA: 0x00033AE7 File Offset: 0x00031CE7
		// (set) Token: 0x06000C79 RID: 3193 RVA: 0x00033AEF File Offset: 0x00031CEF
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

		// Token: 0x1700040D RID: 1037
		// (get) Token: 0x06000C7A RID: 3194 RVA: 0x00033B0D File Offset: 0x00031D0D
		// (set) Token: 0x06000C7B RID: 3195 RVA: 0x00033B15 File Offset: 0x00031D15
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

		// Token: 0x1700040E RID: 1038
		// (get) Token: 0x06000C7C RID: 3196 RVA: 0x00033B33 File Offset: 0x00031D33
		// (set) Token: 0x06000C7D RID: 3197 RVA: 0x00033B3B File Offset: 0x00031D3B
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

		// Token: 0x1700040F RID: 1039
		// (get) Token: 0x06000C7E RID: 3198 RVA: 0x00033B59 File Offset: 0x00031D59
		// (set) Token: 0x06000C7F RID: 3199 RVA: 0x00033B61 File Offset: 0x00031D61
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

		// Token: 0x17000410 RID: 1040
		// (get) Token: 0x06000C80 RID: 3200 RVA: 0x00033B7F File Offset: 0x00031D7F
		// (set) Token: 0x06000C81 RID: 3201 RVA: 0x00033B87 File Offset: 0x00031D87
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

		// Token: 0x17000411 RID: 1041
		// (get) Token: 0x06000C82 RID: 3202 RVA: 0x00033BA5 File Offset: 0x00031DA5
		// (set) Token: 0x06000C83 RID: 3203 RVA: 0x00033BAD File Offset: 0x00031DAD
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

		// Token: 0x17000412 RID: 1042
		// (get) Token: 0x06000C84 RID: 3204 RVA: 0x00033BCB File Offset: 0x00031DCB
		// (set) Token: 0x06000C85 RID: 3205 RVA: 0x00033BD3 File Offset: 0x00031DD3
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

		// Token: 0x17000413 RID: 1043
		// (get) Token: 0x06000C86 RID: 3206 RVA: 0x00033BF1 File Offset: 0x00031DF1
		// (set) Token: 0x06000C87 RID: 3207 RVA: 0x00033BF9 File Offset: 0x00031DF9
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

		// Token: 0x17000414 RID: 1044
		// (get) Token: 0x06000C88 RID: 3208 RVA: 0x00033C17 File Offset: 0x00031E17
		// (set) Token: 0x06000C89 RID: 3209 RVA: 0x00033C1F File Offset: 0x00031E1F
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

		// Token: 0x17000415 RID: 1045
		// (get) Token: 0x06000C8A RID: 3210 RVA: 0x00033C3D File Offset: 0x00031E3D
		// (set) Token: 0x06000C8B RID: 3211 RVA: 0x00033C45 File Offset: 0x00031E45
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

		// Token: 0x17000416 RID: 1046
		// (get) Token: 0x06000C8C RID: 3212 RVA: 0x00033C63 File Offset: 0x00031E63
		// (set) Token: 0x06000C8D RID: 3213 RVA: 0x00033C6B File Offset: 0x00031E6B
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

		// Token: 0x17000417 RID: 1047
		// (get) Token: 0x06000C8E RID: 3214 RVA: 0x00033C89 File Offset: 0x00031E89
		// (set) Token: 0x06000C8F RID: 3215 RVA: 0x00033C91 File Offset: 0x00031E91
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

		// Token: 0x17000418 RID: 1048
		// (get) Token: 0x06000C90 RID: 3216 RVA: 0x00033CAF File Offset: 0x00031EAF
		// (set) Token: 0x06000C91 RID: 3217 RVA: 0x00033CB7 File Offset: 0x00031EB7
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

		// Token: 0x17000419 RID: 1049
		// (get) Token: 0x06000C92 RID: 3218 RVA: 0x00033CD5 File Offset: 0x00031ED5
		// (set) Token: 0x06000C93 RID: 3219 RVA: 0x00033CDD File Offset: 0x00031EDD
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

		// Token: 0x1700041A RID: 1050
		// (get) Token: 0x06000C94 RID: 3220 RVA: 0x00033CFB File Offset: 0x00031EFB
		// (set) Token: 0x06000C95 RID: 3221 RVA: 0x00033D03 File Offset: 0x00031F03
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

		// Token: 0x1700041B RID: 1051
		// (get) Token: 0x06000C96 RID: 3222 RVA: 0x00033D21 File Offset: 0x00031F21
		// (set) Token: 0x06000C97 RID: 3223 RVA: 0x00033D29 File Offset: 0x00031F29
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

		// Token: 0x1700041C RID: 1052
		// (get) Token: 0x06000C98 RID: 3224 RVA: 0x00033D47 File Offset: 0x00031F47
		// (set) Token: 0x06000C99 RID: 3225 RVA: 0x00033D4F File Offset: 0x00031F4F
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

		// Token: 0x1700041D RID: 1053
		// (get) Token: 0x06000C9A RID: 3226 RVA: 0x00033D6D File Offset: 0x00031F6D
		// (set) Token: 0x06000C9B RID: 3227 RVA: 0x00033D75 File Offset: 0x00031F75
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

		// Token: 0x06000C9C RID: 3228 RVA: 0x00033D93 File Offset: 0x00031F93
		private TextObject GetPreviousCharacterKeyText()
		{
			if (this.PreviousCharacterInputKey == null || this._getKeyTextFromKeyId == null)
			{
				return TextObject.Empty;
			}
			return this._getKeyTextFromKeyId(this.PreviousCharacterInputKey.KeyID);
		}

		// Token: 0x06000C9D RID: 3229 RVA: 0x00033DC1 File Offset: 0x00031FC1
		private TextObject GetNextCharacterKeyText()
		{
			if (this.NextCharacterInputKey == null || this._getKeyTextFromKeyId == null)
			{
				return TextObject.Empty;
			}
			return this._getKeyTextFromKeyId(this.NextCharacterInputKey.KeyID);
		}

		// Token: 0x06000C9E RID: 3230 RVA: 0x00033DEF File Offset: 0x00031FEF
		private TextObject GetBuyAllKeyText()
		{
			if (this.BuyAllInputKey == null || this._getKeyTextFromKeyId == null)
			{
				return TextObject.Empty;
			}
			return this._getKeyTextFromKeyId(this.BuyAllInputKey.KeyID);
		}

		// Token: 0x06000C9F RID: 3231 RVA: 0x00033E1D File Offset: 0x0003201D
		private TextObject GetSellAllKeyText()
		{
			if (this.SellAllInputKey == null || this._getKeyTextFromKeyId == null)
			{
				return TextObject.Empty;
			}
			return this._getKeyTextFromKeyId(this.SellAllInputKey.KeyID);
		}

		// Token: 0x06000CA0 RID: 3232 RVA: 0x00033E4B File Offset: 0x0003204B
		public void SetResetInputKey(HotKey hotkey)
		{
			this.ResetInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
		}

		// Token: 0x06000CA1 RID: 3233 RVA: 0x00033E5A File Offset: 0x0003205A
		public void SetCancelInputKey(HotKey gameKey)
		{
			this.CancelInputKey = InputKeyItemVM.CreateFromHotKey(gameKey, true);
		}

		// Token: 0x06000CA2 RID: 3234 RVA: 0x00033E69 File Offset: 0x00032069
		public void SetDoneInputKey(HotKey hotKey)
		{
			this.DoneInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x06000CA3 RID: 3235 RVA: 0x00033E78 File Offset: 0x00032078
		public void SetPreviousCharacterInputKey(HotKey hotKey)
		{
			this.PreviousCharacterInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
			this.SetPreviousCharacterHint();
		}

		// Token: 0x06000CA4 RID: 3236 RVA: 0x00033E8D File Offset: 0x0003208D
		public void SetNextCharacterInputKey(HotKey hotKey)
		{
			this.NextCharacterInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
			this.SetNextCharacterHint();
		}

		// Token: 0x06000CA5 RID: 3237 RVA: 0x00033EA2 File Offset: 0x000320A2
		public void SetBuyAllInputKey(HotKey hotKey)
		{
			this.BuyAllInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
			this.SetBuyAllHint();
		}

		// Token: 0x06000CA6 RID: 3238 RVA: 0x00033EB7 File Offset: 0x000320B7
		public void SetSellAllInputKey(HotKey hotKey)
		{
			this.SellAllInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
			this.SetSellAllHint();
		}

		// Token: 0x06000CA7 RID: 3239 RVA: 0x00033ECC File Offset: 0x000320CC
		public void SetGetKeyTextFromKeyIDFunc(Func<string, TextObject> getKeyTextFromKeyId)
		{
			this._getKeyTextFromKeyId = getKeyTextFromKeyId;
		}

		// Token: 0x1700041E RID: 1054
		// (get) Token: 0x06000CA8 RID: 3240 RVA: 0x00033ED5 File Offset: 0x000320D5
		// (set) Token: 0x06000CA9 RID: 3241 RVA: 0x00033EDD File Offset: 0x000320DD
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

		// Token: 0x1700041F RID: 1055
		// (get) Token: 0x06000CAA RID: 3242 RVA: 0x00033EFB File Offset: 0x000320FB
		// (set) Token: 0x06000CAB RID: 3243 RVA: 0x00033F03 File Offset: 0x00032103
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

		// Token: 0x17000420 RID: 1056
		// (get) Token: 0x06000CAC RID: 3244 RVA: 0x00033F21 File Offset: 0x00032121
		// (set) Token: 0x06000CAD RID: 3245 RVA: 0x00033F29 File Offset: 0x00032129
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

		// Token: 0x17000421 RID: 1057
		// (get) Token: 0x06000CAE RID: 3246 RVA: 0x00033F47 File Offset: 0x00032147
		// (set) Token: 0x06000CAF RID: 3247 RVA: 0x00033F4F File Offset: 0x0003214F
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

		// Token: 0x17000422 RID: 1058
		// (get) Token: 0x06000CB0 RID: 3248 RVA: 0x00033F6D File Offset: 0x0003216D
		// (set) Token: 0x06000CB1 RID: 3249 RVA: 0x00033F75 File Offset: 0x00032175
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

		// Token: 0x17000423 RID: 1059
		// (get) Token: 0x06000CB2 RID: 3250 RVA: 0x00033F93 File Offset: 0x00032193
		// (set) Token: 0x06000CB3 RID: 3251 RVA: 0x00033F9B File Offset: 0x0003219B
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

		// Token: 0x17000424 RID: 1060
		// (get) Token: 0x06000CB4 RID: 3252 RVA: 0x00033FB9 File Offset: 0x000321B9
		// (set) Token: 0x06000CB5 RID: 3253 RVA: 0x00033FC1 File Offset: 0x000321C1
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

		// Token: 0x06000CB6 RID: 3254 RVA: 0x00033FDF File Offset: 0x000321DF
		void IInventoryStateHandler.ExecuteLootingScript()
		{
			this.ExecuteBuyAllItems();
		}

		// Token: 0x06000CB7 RID: 3255 RVA: 0x00033FE7 File Offset: 0x000321E7
		void IInventoryStateHandler.ExecuteBuyConsumableItem()
		{
			this.ExecuteBuyItemTest();
		}

		// Token: 0x06000CB8 RID: 3256 RVA: 0x00033FF0 File Offset: 0x000321F0
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

		// Token: 0x06000CB9 RID: 3257 RVA: 0x00034032 File Offset: 0x00032232
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

		// Token: 0x04000545 RID: 1349
		private readonly PlayerUpdateTracker _playerUpdateTracker;

		// Token: 0x04000546 RID: 1350
		private readonly IViewDataTracker _viewDataTracker;

		// Token: 0x04000547 RID: 1351
		public bool DoNotSync;

		// Token: 0x04000548 RID: 1352
		private TroopRoster _rightTroopRoster;

		// Token: 0x04000549 RID: 1353
		private TroopRoster _leftTroopRoster;

		// Token: 0x0400054A RID: 1354
		private bool _isFinalized;

		// Token: 0x0400054B RID: 1355
		public bool IsFiveStackModifierActive;

		// Token: 0x0400054C RID: 1356
		public bool IsEntireStackModifierActive;

		// Token: 0x0400054D RID: 1357
		private readonly int _donationMaxShareableXp;

		// Token: 0x0400054E RID: 1358
		private readonly Stack<SPItemVM> _equipAfterTransferStack;

		// Token: 0x0400054F RID: 1359
		private bool _isTrading;

		// Token: 0x04000550 RID: 1360
		private InventoryLogic _inventoryLogic;

		// Token: 0x04000551 RID: 1361
		private bool _isCharacterEquipmentDirty;

		// Token: 0x04000552 RID: 1362
		private CharacterObject _currentCharacter;

		// Token: 0x04000553 RID: 1363
		private int _currentInventoryCharacterIndex;

		// Token: 0x04000554 RID: 1364
		private string _selectedTooltipItemStringID = "";

		// Token: 0x04000555 RID: 1365
		private string _comparedTooltipItemStringID = "";

		// Token: 0x04000556 RID: 1366
		private SPItemVM _selectedItem;

		// Token: 0x04000557 RID: 1367
		private int _lastComparedItemIndex;

		// Token: 0x04000558 RID: 1368
		private float _equipmentCount;

		// Token: 0x04000559 RID: 1369
		private readonly Func<WeaponComponentData, ItemObject.ItemUsageSetFlags> _getItemUsageSetFlags;

		// Token: 0x0400055A RID: 1370
		private List<ItemVM> _comparedItemList;

		// Token: 0x0400055B RID: 1371
		private Func<string, TextObject> _getKeyTextFromKeyId;

		// Token: 0x0400055C RID: 1372
		private InventoryMode _usageType = InventoryMode.Trade;

		// Token: 0x0400055D RID: 1373
		private string _fiveStackShortcutkeyText;

		// Token: 0x0400055E RID: 1374
		private string _entireStackShortcutkeyText;

		// Token: 0x0400055F RID: 1375
		private List<string> _lockedItemIDs;

		// Token: 0x04000560 RID: 1376
		private readonly List<int> _everyItemType = new List<int>
		{
			1, 2, 3, 4, 5, 6, 7, 8, 9, 10,
			11, 12, 13, 14, 15, 16, 17, 18, 19, 20,
			21, 22, 23, 24
		};

		// Token: 0x04000561 RID: 1377
		private readonly List<int> _weaponItemTypes = new List<int> { 2, 3, 4 };

		// Token: 0x04000562 RID: 1378
		private readonly List<int> _armorItemTypes = new List<int> { 12, 13, 14, 15, 21, 22 };

		// Token: 0x04000563 RID: 1379
		private readonly List<int> _mountItemTypes = new List<int> { 1, 23 };

		// Token: 0x04000564 RID: 1380
		private readonly List<int> _shieldAndRangedItemTypes = new List<int> { 7, 5, 6, 8, 9, 10, 16, 17, 18 };

		// Token: 0x04000565 RID: 1381
		private readonly List<int> _miscellaneousItemTypes = new List<int> { 11, 19, 20, 24 };

		// Token: 0x04000566 RID: 1382
		private readonly Dictionary<SPInventoryVM.Filters, List<int>> _filters;

		// Token: 0x04000567 RID: 1383
		private int _selectedEquipmentIndex;

		// Token: 0x04000568 RID: 1384
		private bool _isFoodTransferButtonHighlightApplied;

		// Token: 0x04000569 RID: 1385
		private bool _isBannerItemsHighlightApplied;

		// Token: 0x0400056A RID: 1386
		private string _latestTutorialElementID;

		// Token: 0x0400056B RID: 1387
		private string _leftInventoryLabel;

		// Token: 0x0400056C RID: 1388
		private string _rightInventoryLabel;

		// Token: 0x0400056D RID: 1389
		private bool _isDoneDisabled;

		// Token: 0x0400056E RID: 1390
		private bool _isSearchAvailable;

		// Token: 0x0400056F RID: 1391
		private bool _isOtherInventoryGoldRelevant;

		// Token: 0x04000570 RID: 1392
		private string _doneLbl;

		// Token: 0x04000571 RID: 1393
		private string _cancelLbl;

		// Token: 0x04000572 RID: 1394
		private string _resetLbl;

		// Token: 0x04000573 RID: 1395
		private string _typeText;

		// Token: 0x04000574 RID: 1396
		private string _nameText;

		// Token: 0x04000575 RID: 1397
		private string _quantityText;

		// Token: 0x04000576 RID: 1398
		private string _costText;

		// Token: 0x04000577 RID: 1399
		private string _searchPlaceholderText;

		// Token: 0x04000578 RID: 1400
		private HintViewModel _resetHint;

		// Token: 0x04000579 RID: 1401
		private HintViewModel _filterAllHint;

		// Token: 0x0400057A RID: 1402
		private HintViewModel _filterWeaponHint;

		// Token: 0x0400057B RID: 1403
		private HintViewModel _filterArmorHint;

		// Token: 0x0400057C RID: 1404
		private HintViewModel _filterShieldAndRangedHint;

		// Token: 0x0400057D RID: 1405
		private HintViewModel _filterMountAndHarnessHint;

		// Token: 0x0400057E RID: 1406
		private HintViewModel _filterMiscHint;

		// Token: 0x0400057F RID: 1407
		private HintViewModel _civilianOutfitHint;

		// Token: 0x04000580 RID: 1408
		private HintViewModel _battleOutfitHint;

		// Token: 0x04000581 RID: 1409
		private HintViewModel _equipmentHelmSlotHint;

		// Token: 0x04000582 RID: 1410
		private HintViewModel _equipmentArmorSlotHint;

		// Token: 0x04000583 RID: 1411
		private HintViewModel _equipmentBootSlotHint;

		// Token: 0x04000584 RID: 1412
		private HintViewModel _equipmentCloakSlotHint;

		// Token: 0x04000585 RID: 1413
		private HintViewModel _equipmentGloveSlotHint;

		// Token: 0x04000586 RID: 1414
		private HintViewModel _equipmentHarnessSlotHint;

		// Token: 0x04000587 RID: 1415
		private HintViewModel _equipmentMountSlotHint;

		// Token: 0x04000588 RID: 1416
		private HintViewModel _equipmentWeaponSlotHint;

		// Token: 0x04000589 RID: 1417
		private HintViewModel _equipmentBannerSlotHint;

		// Token: 0x0400058A RID: 1418
		private BasicTooltipViewModel _buyAllHint;

		// Token: 0x0400058B RID: 1419
		private BasicTooltipViewModel _sellAllHint;

		// Token: 0x0400058C RID: 1420
		private BasicTooltipViewModel _previousCharacterHint;

		// Token: 0x0400058D RID: 1421
		private BasicTooltipViewModel _nextCharacterHint;

		// Token: 0x0400058E RID: 1422
		private HintViewModel _weightHint;

		// Token: 0x0400058F RID: 1423
		private HintViewModel _armArmorHint;

		// Token: 0x04000590 RID: 1424
		private HintViewModel _bodyArmorHint;

		// Token: 0x04000591 RID: 1425
		private HintViewModel _headArmorHint;

		// Token: 0x04000592 RID: 1426
		private HintViewModel _legArmorHint;

		// Token: 0x04000593 RID: 1427
		private HintViewModel _horseArmorHint;

		// Token: 0x04000594 RID: 1428
		private HintViewModel _previewHint;

		// Token: 0x04000595 RID: 1429
		private HintViewModel _equipHint;

		// Token: 0x04000596 RID: 1430
		private HintViewModel _unequipHint;

		// Token: 0x04000597 RID: 1431
		private HintViewModel _sellHint;

		// Token: 0x04000598 RID: 1432
		private HintViewModel _capacityExceededHint;

		// Token: 0x04000599 RID: 1433
		private HintViewModel _noSaddleHint;

		// Token: 0x0400059A RID: 1434
		private HintViewModel _donationLblHint;

		// Token: 0x0400059B RID: 1435
		private BasicTooltipViewModel _equipmentMaxCountHint;

		// Token: 0x0400059C RID: 1436
		private BasicTooltipViewModel _currentCharacterSkillsTooltip;

		// Token: 0x0400059D RID: 1437
		private BasicTooltipViewModel _productionTooltip;

		// Token: 0x0400059E RID: 1438
		private HeroViewModel _mainCharacter;

		// Token: 0x0400059F RID: 1439
		private bool _isExtendedEquipmentControlsEnabled;

		// Token: 0x040005A0 RID: 1440
		private bool _isFocusedOnItemList;

		// Token: 0x040005A1 RID: 1441
		private SPItemVM _currentFocusedItem;

		// Token: 0x040005A2 RID: 1442
		private bool _equipAfterBuy;

		// Token: 0x040005A3 RID: 1443
		private MBBindingList<SPItemVM> _leftItemListVM;

		// Token: 0x040005A4 RID: 1444
		private MBBindingList<SPItemVM> _rightItemListVM;

		// Token: 0x040005A5 RID: 1445
		private ItemMenuVM _itemMenu;

		// Token: 0x040005A6 RID: 1446
		private SPItemVM _characterHelmSlot;

		// Token: 0x040005A7 RID: 1447
		private SPItemVM _characterCloakSlot;

		// Token: 0x040005A8 RID: 1448
		private SPItemVM _characterTorsoSlot;

		// Token: 0x040005A9 RID: 1449
		private SPItemVM _characterGloveSlot;

		// Token: 0x040005AA RID: 1450
		private SPItemVM _characterBootSlot;

		// Token: 0x040005AB RID: 1451
		private SPItemVM _characterMountSlot;

		// Token: 0x040005AC RID: 1452
		private SPItemVM _characterMountArmorSlot;

		// Token: 0x040005AD RID: 1453
		private SPItemVM _characterWeapon1Slot;

		// Token: 0x040005AE RID: 1454
		private SPItemVM _characterWeapon2Slot;

		// Token: 0x040005AF RID: 1455
		private SPItemVM _characterWeapon3Slot;

		// Token: 0x040005B0 RID: 1456
		private SPItemVM _characterWeapon4Slot;

		// Token: 0x040005B1 RID: 1457
		private SPItemVM _characterBannerSlot;

		// Token: 0x040005B2 RID: 1458
		private EquipmentIndex _targetEquipmentIndex = EquipmentIndex.None;

		// Token: 0x040005B3 RID: 1459
		private int _transactionCount = -1;

		// Token: 0x040005B4 RID: 1460
		private bool _isRefreshed;

		// Token: 0x040005B5 RID: 1461
		private string _tradeLbl = "";

		// Token: 0x040005B6 RID: 1462
		private string _experienceLbl = "";

		// Token: 0x040005B7 RID: 1463
		private bool _hasGainedExperience;

		// Token: 0x040005B8 RID: 1464
		private bool _isDonationXpGainExceedsMax;

		// Token: 0x040005B9 RID: 1465
		private bool _equipmentCountWarned;

		// Token: 0x040005BA RID: 1466
		private bool _noSaddleWarned;

		// Token: 0x040005BB RID: 1467
		private bool _isTradingWithSettlement;

		// Token: 0x040005BC RID: 1468
		private string _equipmentCountText;

		// Token: 0x040005BD RID: 1469
		private string _noSaddleText;

		// Token: 0x040005BE RID: 1470
		private string _capacityExceededText;

		// Token: 0x040005BF RID: 1471
		private string _leftSearchText = "";

		// Token: 0x040005C0 RID: 1472
		private string _rightSearchText = "";

		// Token: 0x040005C1 RID: 1473
		private bool _isInWarSet = true;

		// Token: 0x040005C2 RID: 1474
		private bool _companionExists;

		// Token: 0x040005C3 RID: 1475
		private SPInventoryVM.Filters _activeFilterIndex;

		// Token: 0x040005C4 RID: 1476
		private bool _isMicsFilterHighlightEnabled;

		// Token: 0x040005C5 RID: 1477
		private bool _isCivilianFilterHighlightEnabled;

		// Token: 0x040005C6 RID: 1478
		private ItemPreviewVM _itemPreview;

		// Token: 0x040005C7 RID: 1479
		private SelectorVM<InventoryCharacterSelectorItemVM> _characterList;

		// Token: 0x040005C8 RID: 1480
		private SPInventorySortControllerVM _otherInventorySortController;

		// Token: 0x040005C9 RID: 1481
		private SPInventorySortControllerVM _playerInventorySortController;

		// Token: 0x040005CA RID: 1482
		private string _leftInventoryOwnerName;

		// Token: 0x040005CB RID: 1483
		private int _leftInventoryOwnerGold;

		// Token: 0x040005CC RID: 1484
		private string _rightInventoryOwnerName;

		// Token: 0x040005CD RID: 1485
		private string _currentCharacterName;

		// Token: 0x040005CE RID: 1486
		private int _rightInventoryOwnerGold;

		// Token: 0x040005CF RID: 1487
		private int _itemCountToBuy;

		// Token: 0x040005D0 RID: 1488
		private float _currentCharacterArmArmor;

		// Token: 0x040005D1 RID: 1489
		private float _currentCharacterBodyArmor;

		// Token: 0x040005D2 RID: 1490
		private float _currentCharacterHeadArmor;

		// Token: 0x040005D3 RID: 1491
		private float _currentCharacterLegArmor;

		// Token: 0x040005D4 RID: 1492
		private float _currentCharacterHorseArmor;

		// Token: 0x040005D5 RID: 1493
		private string _currentCharacterTotalEncumbrance;

		// Token: 0x040005D6 RID: 1494
		private InputKeyItemVM _resetInputKey;

		// Token: 0x040005D7 RID: 1495
		private InputKeyItemVM _cancelInputKey;

		// Token: 0x040005D8 RID: 1496
		private InputKeyItemVM _doneInputKey;

		// Token: 0x040005D9 RID: 1497
		private InputKeyItemVM _previousCharacterInputKey;

		// Token: 0x040005DA RID: 1498
		private InputKeyItemVM _nextCharacterInputKey;

		// Token: 0x040005DB RID: 1499
		private InputKeyItemVM _buyAllInputKey;

		// Token: 0x040005DC RID: 1500
		private InputKeyItemVM _sellAllInputKey;

		// Token: 0x020001BF RID: 447
		public enum Filters
		{
			// Token: 0x04000F85 RID: 3973
			All,
			// Token: 0x04000F86 RID: 3974
			Weapons,
			// Token: 0x04000F87 RID: 3975
			ShieldsAndRanged,
			// Token: 0x04000F88 RID: 3976
			Armors,
			// Token: 0x04000F89 RID: 3977
			Mounts,
			// Token: 0x04000F8A RID: 3978
			Miscellaneous
		}
	}
}
