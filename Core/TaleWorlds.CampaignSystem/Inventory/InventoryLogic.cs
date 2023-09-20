using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.Inventory
{
	public class InventoryLogic
	{
		public bool DisableNetwork { get; set; }

		public Action<int> TotalAmountChange { get; set; }

		public Action DonationXpChange { get; set; }

		public event InventoryLogic.AfterResetDelegate AfterReset;

		public event InventoryLogic.ProcessResultListDelegate AfterTransfer;

		public TroopRoster RightMemberRoster { get; private set; }

		public TroopRoster LeftMemberRoster { get; private set; }

		public CharacterObject InitialEquipmentCharacter { get; private set; }

		public bool IsTrading { get; private set; }

		public bool IsSpecialActionsPermitted { get; private set; }

		public CharacterObject OwnerCharacter { get; private set; }

		public MobileParty OwnerParty { get; private set; }

		public PartyBase OtherParty { get; private set; }

		public IMarketData MarketData { get; private set; }

		public TextObject LeftRosterName { get; private set; }

		public bool IsDiscardDonating { get; private set; }

		public bool IsOtherPartyFromPlayerClan { get; private set; }

		public InventoryListener InventoryListener { get; private set; }

		public int TotalAmount
		{
			get
			{
				return this.TransactionDebt;
			}
		}

		public PartyBase OppositePartyFromListener
		{
			get
			{
				return this.InventoryListener.GetOppositeParty();
			}
		}

		public SettlementComponent CurrentSettlementComponent
		{
			get
			{
				Settlement currentSettlement = Settlement.CurrentSettlement;
				if (currentSettlement == null)
				{
					return null;
				}
				return currentSettlement.SettlementComponent;
			}
		}

		public MobileParty CurrentMobileParty
		{
			get
			{
				if (PlayerEncounter.Current != null)
				{
					return PlayerEncounter.EncounteredParty.MobileParty;
				}
				MapEvent mapEvent = PartyBase.MainParty.MapEvent;
				bool flag;
				if (mapEvent == null)
				{
					flag = null != null;
				}
				else
				{
					PartyBase leaderParty = mapEvent.GetLeaderParty(PartyBase.MainParty.OpponentSide);
					flag = ((leaderParty != null) ? leaderParty.MobileParty : null) != null;
				}
				if (flag)
				{
					return PartyBase.MainParty.MapEvent.GetLeaderParty(PartyBase.MainParty.OpponentSide).MobileParty;
				}
				return null;
			}
		}

		public int TransactionDebt
		{
			get
			{
				return this._transactionDebt;
			}
			private set
			{
				if (value != this._transactionDebt)
				{
					this._transactionDebt = value;
					this.TotalAmountChange(this._transactionDebt);
				}
			}
		}

		public float XpGainFromDonations
		{
			get
			{
				return this._xpGainFromDonations;
			}
			private set
			{
				if (value != this._xpGainFromDonations)
				{
					this._xpGainFromDonations = value;
					if (this._xpGainFromDonations < 0f)
					{
						this._xpGainFromDonations = 0f;
					}
					Action donationXpChange = this.DonationXpChange;
					if (donationXpChange == null)
					{
						return;
					}
					donationXpChange();
				}
			}
		}

		public InventoryLogic(MobileParty ownerParty, CharacterObject ownerCharacter, PartyBase merchantParty)
		{
			this._rosters = new ItemRoster[2];
			this._rostersBackup = new ItemRoster[2];
			this.OwnerParty = ownerParty;
			this.OwnerCharacter = ownerCharacter;
			this.OtherParty = merchantParty;
		}

		public InventoryLogic(PartyBase merchantParty)
			: this(MobileParty.MainParty, CharacterObject.PlayerCharacter, merchantParty)
		{
		}

		public void Initialize(ItemRoster leftItemRoster, MobileParty party, bool isTrading, bool isSpecialActionsPermitted, CharacterObject initialCharacterOfRightRoster, InventoryManager.InventoryCategoryType merchantItemType, IMarketData marketData, bool useBasePrices, TextObject leftRosterName = null, TroopRoster leftMemberRoster = null)
		{
			this.Initialize(leftItemRoster, party.ItemRoster, party.MemberRoster, isTrading, isSpecialActionsPermitted, initialCharacterOfRightRoster, merchantItemType, marketData, useBasePrices, leftRosterName, leftMemberRoster);
		}

		public void Initialize(ItemRoster leftItemRoster, ItemRoster rightItemRoster, TroopRoster rightMemberRoster, bool isTrading, bool isSpecialActionsPermitted, CharacterObject initialCharacterOfRightRoster, InventoryManager.InventoryCategoryType merchantItemType, IMarketData marketData, bool useBasePrices, TextObject leftRosterName = null, TroopRoster leftMemberRoster = null)
		{
			this.MarketData = marketData;
			this.TransactionDebt = 0;
			this.MerchantItemType = merchantItemType;
			this.InventoryListener = new FakeInventoryListener();
			this._useBasePrices = useBasePrices;
			this.LeftRosterName = leftRosterName;
			this.IsTrading = isTrading;
			this.IsSpecialActionsPermitted = isSpecialActionsPermitted;
			this.InitializeRosters(leftItemRoster, rightItemRoster, rightMemberRoster, initialCharacterOfRightRoster, leftMemberRoster);
			this._transactionHistory.Clear();
			this.InitializeCategoryAverages();
			this.IsDiscardDonating = (InventoryManager.Instance.CurrentMode == InventoryMode.Default && !Game.Current.CheatMode) || InventoryManager.Instance.CurrentMode == InventoryMode.Loot;
			this.InitializeXpGainFromDonations();
			PartyBase otherParty = this.OtherParty;
			Clan clan;
			if (otherParty == null)
			{
				clan = null;
			}
			else
			{
				MobileParty mobileParty = otherParty.MobileParty;
				clan = ((mobileParty != null) ? mobileParty.ActualClan : null);
			}
			if (clan == Hero.MainHero.Clan)
			{
				this.IsOtherPartyFromPlayerClan = true;
			}
		}

		private void InitializeRosters(ItemRoster leftItemRoster, ItemRoster rightItemRoster, TroopRoster rightMemberRoster, CharacterObject initialCharacterOfRightRoster, TroopRoster leftMemberRoster = null)
		{
			this._rosters[0] = leftItemRoster;
			this._rosters[1] = rightItemRoster;
			this.RightMemberRoster = rightMemberRoster;
			this.LeftMemberRoster = leftMemberRoster;
			this.InitialEquipmentCharacter = initialCharacterOfRightRoster;
			this.SetCurrentStateAsInitial();
		}

		public int GetItemTotalPrice(ItemRosterElement itemRosterElement, int absStockChange, out int lastPrice, bool isBuying)
		{
			lastPrice = this.GetItemPrice(itemRosterElement, isBuying);
			return lastPrice;
		}

		public void SetPlayerAcceptTraderOffer()
		{
			this._playerAcceptsTraderOffer = true;
		}

		public bool DoneLogic()
		{
			if (this.IsPreviewingItem)
			{
				return false;
			}
			SettlementComponent currentSettlementComponent = this.CurrentSettlementComponent;
			MobileParty currentMobileParty = this.CurrentMobileParty;
			PartyBase partyBase = null;
			if (currentMobileParty != null)
			{
				partyBase = currentMobileParty.Party;
			}
			else if (currentSettlementComponent != null)
			{
				partyBase = currentSettlementComponent.Owner;
			}
			if (!this._playerAcceptsTraderOffer)
			{
				InventoryListener inventoryListener = this.InventoryListener;
				int? num = ((inventoryListener != null) ? new int?(inventoryListener.GetGold()) : null) + this.TotalAmount;
				int num2 = 0;
				bool flag = (num.GetValueOrDefault() < num2) & (num != null);
			}
			if (this.InventoryListener != null && this.IsTrading && this.OwnerCharacter.HeroObject.Gold - this.TotalAmount < 0)
			{
				MBInformationManager.AddQuickInformation(GameTexts.FindText("str_warning_you_dont_have_enough_money", null), 0, null, "");
				return false;
			}
			if (this._playerAcceptsTraderOffer)
			{
				this._playerAcceptsTraderOffer = false;
				if (this.InventoryListener != null)
				{
					int gold = this.InventoryListener.GetGold();
					this.TransactionDebt = -gold;
				}
			}
			if (this.OwnerCharacter != null && this.OwnerCharacter.HeroObject != null && this.IsTrading)
			{
				GiveGoldAction.ApplyBetweenCharacters(null, this.OwnerCharacter.HeroObject, MathF.Min(-this.TotalAmount, this.InventoryListener.GetGold()), false);
				if (currentSettlementComponent != null && currentSettlementComponent.IsTown && this.OwnerCharacter.GetPerkValue(DefaultPerks.Trade.TrickleDown))
				{
					int num3 = 0;
					List<ValueTuple<ItemRosterElement, int>> boughtItems = this._transactionHistory.GetBoughtItems();
					int num4 = 0;
					while (boughtItems != null && num4 < boughtItems.Count)
					{
						ItemObject item = boughtItems[num4].Item1.EquipmentElement.Item;
						if (item != null && item.IsTradeGood)
						{
							num3 += boughtItems[num4].Item2;
						}
						num4++;
					}
					if (num3 >= 10000)
					{
						for (int i = 0; i < currentSettlementComponent.Settlement.Notables.Count; i++)
						{
							if (currentSettlementComponent.Settlement.Notables[i].IsMerchant)
							{
								ChangeRelationAction.ApplyRelationChangeBetweenHeroes(currentSettlementComponent.Settlement.Notables[i], this.OwnerCharacter.HeroObject, MathF.Floor(DefaultPerks.Trade.TrickleDown.PrimaryBonus), true);
							}
						}
					}
				}
			}
			if (this.IsDiscardDonating)
			{
				CampaignEventDispatcher.Instance.OnItemsDiscardedByPlayer(this._rosters[0]);
			}
			CampaignEventDispatcher.Instance.OnPlayerInventoryExchange(this._transactionHistory.GetBoughtItems(), this._transactionHistory.GetSoldItems(), this.IsTrading);
			if (currentSettlementComponent != null && this.InventoryListener != null && this.IsTrading)
			{
				this.InventoryListener.SetGold(this.InventoryListener.GetGold() + this.TotalAmount);
			}
			else if (((currentMobileParty != null) ? currentMobileParty.Party.LeaderHero : null) != null && this.IsTrading)
			{
				GiveGoldAction.ApplyBetweenCharacters(null, this.CurrentMobileParty.Party.LeaderHero, this.TotalAmount, false);
				if (this.CurrentMobileParty.Party.LeaderHero.CompanionOf != null)
				{
					this.CurrentMobileParty.AddTaxGold((int)((float)this.TotalAmount * 0.1f));
				}
			}
			else if (partyBase != null && partyBase.LeaderHero == null && this.IsTrading)
			{
				GiveGoldAction.ApplyForCharacterToParty(null, partyBase, this.TotalAmount, false);
			}
			this._partyInitialEquipment = new InventoryLogic.PartyEquipment(this.OwnerParty);
			return true;
		}

		public List<ValueTuple<ItemRosterElement, int>> GetBoughtItems()
		{
			return this._transactionHistory.GetBoughtItems();
		}

		public List<ValueTuple<ItemRosterElement, int>> GetSoldItems()
		{
			return this._transactionHistory.GetSoldItems();
		}

		private void InitializeCategoryAverages()
		{
			if (Campaign.Current != null && Settlement.CurrentSettlement != null)
			{
				Town town = (Settlement.CurrentSettlement.IsVillage ? Settlement.CurrentSettlement.Village.Bound.Town : Settlement.CurrentSettlement.Town);
				foreach (ItemCategory itemCategory in ItemCategories.All)
				{
					float num = 0f;
					for (int i = 0; i < Town.AllTowns.Count; i++)
					{
						if (Town.AllTowns[i] != town)
						{
							num += Town.AllTowns[i].MarketData.GetPriceFactor(itemCategory);
						}
					}
					float num2 = num / (float)(Town.AllTowns.Count - 1);
					this._itemCategoryAverages.Add(itemCategory, num2);
					Debug.Print(string.Format("Average value of {0} : {1}", itemCategory.GetName(), num2), 0, Debug.DebugColor.White, 17592186044416UL);
				}
			}
		}

		private void InitializeXpGainFromDonations()
		{
			this.XpGainFromDonations = 0f;
			bool flag = PerkHelper.PlayerHasAnyItemDonationPerk();
			bool flag2 = InventoryManager.Instance.CurrentMode == InventoryMode.Loot;
			if (flag && flag2)
			{
				this.XpGainFromDonations = (float)Campaign.Current.Models.ItemDiscardModel.GetXpBonusForDiscardingItems(this._rosters[0]);
			}
		}

		private void HandleDonationOnTransferItem(ItemRosterElement rosterElement, int amount, bool isBuying, bool isSelling)
		{
			ItemObject item = rosterElement.EquipmentElement.Item;
			ItemDiscardModel itemDiscardModel = Campaign.Current.Models.ItemDiscardModel;
			if (this.IsDiscardDonating && (isSelling || isBuying) && item != null)
			{
				this.XpGainFromDonations += (float)(itemDiscardModel.GetXpBonusForDiscardingItem(item, amount) * (isSelling ? 1 : (-1)));
			}
		}

		public float GetAveragePriceFactorItemCategory(ItemCategory category)
		{
			if (this._itemCategoryAverages.ContainsKey(category))
			{
				return this._itemCategoryAverages[category];
			}
			return -99f;
		}

		public bool IsThereAnyChanges()
		{
			return this.IsThereAnyChangeBetweenRosters(this._rosters[1], this._rostersBackup[1]) || !this._partyInitialEquipment.IsEqual(new InventoryLogic.PartyEquipment(this.OwnerParty));
		}

		private bool IsThereAnyChangeBetweenRosters(ItemRoster roster1, ItemRoster roster2)
		{
			if (roster1.Count != roster2.Count)
			{
				return true;
			}
			using (IEnumerator<ItemRosterElement> enumerator = roster1.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ItemRosterElement item = enumerator.Current;
					if (!roster2.Any((ItemRosterElement e) => e.IsEqualTo(item)))
					{
						return true;
					}
				}
			}
			return false;
		}

		public void Reset(bool fromCancel)
		{
			this.ResetLogic(fromCancel);
		}

		private void ResetLogic(bool fromCancel)
		{
			Debug.Print("InventoryLogic::Reset", 0, Debug.DebugColor.White, 17592186044416UL);
			for (int i = 0; i < 2; i++)
			{
				this._rosters[i].Clear();
				this._rosters[i].Add(this._rostersBackup[i]);
			}
			this.TransactionDebt = 0;
			this._transactionHistory.Clear();
			this.InitializeXpGainFromDonations();
			this._partyInitialEquipment.ResetEquipment(this.OwnerParty);
			InventoryLogic.AfterResetDelegate afterReset = this.AfterReset;
			if (afterReset != null)
			{
				afterReset(this, fromCancel);
			}
			List<TransferCommandResult> list = new List<TransferCommandResult>();
			if (!fromCancel)
			{
				this.OnAfterTransfer(list);
			}
		}

		public bool CanPlayerCompleteTransaction()
		{
			return !this.IsPreviewingItem || !this.IsTrading || this.TotalAmount <= 0 || (this.TotalAmount >= 0 && this.OwnerCharacter.HeroObject.Gold - this.TotalAmount >= 0);
		}

		public bool CanSlaughterItem(ItemRosterElement element, InventoryLogic.InventorySide sideOfItem)
		{
			return (!this.IsTrading || this._transactionHistory.IsEmpty) && (this.IsSpecialActionsPermitted && this.IsSlaughterable(element.EquipmentElement.Item) && sideOfItem == InventoryLogic.InventorySide.PlayerInventory && element.Amount > 0) && !this._transactionHistory.GetBoughtItems().Any((ValueTuple<ItemRosterElement, int> i) => i.Item1.EquipmentElement.Item == element.EquipmentElement.Item);
		}

		public bool IsSlaughterable(ItemObject item)
		{
			return item.Type == ItemObject.ItemTypeEnum.Animal || item.Type == ItemObject.ItemTypeEnum.Horse;
		}

		public bool CanDonateItem(ItemRosterElement element, InventoryLogic.InventorySide sideOfItem)
		{
			return Game.Current.IsDevelopmentMode && this.IsSpecialActionsPermitted && element.Amount > 0 && this.IsDonatable(element.EquipmentElement.Item) && sideOfItem == InventoryLogic.InventorySide.PlayerInventory;
		}

		public bool IsDonatable(ItemObject item)
		{
			return item.Type == ItemObject.ItemTypeEnum.Arrows || item.Type == ItemObject.ItemTypeEnum.BodyArmor || item.Type == ItemObject.ItemTypeEnum.Bolts || item.Type == ItemObject.ItemTypeEnum.Bow || item.Type == ItemObject.ItemTypeEnum.Bullets || item.Type == ItemObject.ItemTypeEnum.Cape || item.Type == ItemObject.ItemTypeEnum.ChestArmor || item.Type == ItemObject.ItemTypeEnum.Crossbow || item.Type == ItemObject.ItemTypeEnum.HandArmor || item.Type == ItemObject.ItemTypeEnum.HeadArmor || item.Type == ItemObject.ItemTypeEnum.HorseHarness || item.Type == ItemObject.ItemTypeEnum.LegArmor || item.Type == ItemObject.ItemTypeEnum.Musket || item.Type == ItemObject.ItemTypeEnum.OneHandedWeapon || item.Type == ItemObject.ItemTypeEnum.Pistol || item.Type == ItemObject.ItemTypeEnum.Polearm || item.Type == ItemObject.ItemTypeEnum.Shield || item.Type == ItemObject.ItemTypeEnum.Thrown || item.Type == ItemObject.ItemTypeEnum.TwoHandedWeapon;
		}

		public void SetInventoryListener(InventoryListener inventoryListener)
		{
			this.InventoryListener = inventoryListener;
		}

		public int GetItemPrice(ItemRosterElement itemRosterElement, bool isBuying = false)
		{
			bool flag = !isBuying;
			bool flag2 = false;
			int num = 0;
			int num2;
			bool flag3;
			if (this._transactionHistory.GetLastTransfer(itemRosterElement, out num2, out flag3) && flag3 != flag)
			{
				flag2 = true;
				num = num2;
			}
			if (this._useBasePrices)
			{
				return itemRosterElement.EquipmentElement.GetBaseValue();
			}
			if (flag2)
			{
				return num;
			}
			return this.MarketData.GetPrice(itemRosterElement.EquipmentElement, this.OwnerParty, flag, this.OtherParty);
		}

		public int GetCostOfItemRosterElement(ItemRosterElement itemRosterElement, InventoryLogic.InventorySide side)
		{
			bool flag = side == InventoryLogic.InventorySide.OtherInventory && this.IsTrading;
			return this.GetItemPrice(itemRosterElement, flag);
		}

		private void OnAfterTransfer(List<TransferCommandResult> resultList)
		{
			InventoryLogic.ProcessResultListDelegate afterTransfer = this.AfterTransfer;
			if (afterTransfer != null)
			{
				afterTransfer(this, resultList);
			}
			foreach (TransferCommandResult transferCommandResult in resultList)
			{
				if (transferCommandResult.EffectedNumber > 0)
				{
					Game.Current.EventManager.TriggerEvent<InventoryTransferItemEvent>(new InventoryTransferItemEvent(transferCommandResult.EffectedItemRosterElement.EquipmentElement.Item, transferCommandResult.ResultSide == InventoryLogic.InventorySide.PlayerInventory));
				}
			}
		}

		public void AddTransferCommand(TransferCommand command)
		{
			this.ProcessTransferCommand(command);
		}

		public void AddTransferCommands(IEnumerable<TransferCommand> commands)
		{
			foreach (TransferCommand transferCommand in commands)
			{
				this.ProcessTransferCommand(transferCommand);
			}
		}

		public bool CheckItemRosterHasElement(InventoryLogic.InventorySide side, ItemRosterElement rosterElement, int number)
		{
			int num = this._rosters[(int)side].FindIndexOfElement(rosterElement.EquipmentElement);
			return num != -1 && this._rosters[(int)side].GetElementCopyAtIndex(num).Amount >= number;
		}

		private void ProcessTransferCommand(TransferCommand command)
		{
			List<TransferCommandResult> list = this.TransferItem(ref command);
			this.OnAfterTransfer(list);
		}

		private List<TransferCommandResult> TransferItem(ref TransferCommand transferCommand)
		{
			List<TransferCommandResult> list = new List<TransferCommandResult>();
			Debug.Print(string.Format("TransferItem Name: {0} | From: {1} To: {2} | Amount: {3}", new object[]
			{
				transferCommand.ElementToTransfer.EquipmentElement.Item.Name.ToString(),
				transferCommand.FromSide,
				transferCommand.ToSide,
				transferCommand.Amount
			}), 0, Debug.DebugColor.White, 17592186044416UL);
			if (transferCommand.ElementToTransfer.EquipmentElement.Item != null && InventoryLogic.TransferIsMovementValid(ref transferCommand) && this.DoesTransferItemExist(ref transferCommand))
			{
				int num = 0;
				bool flag = false;
				if (transferCommand.FromSide != InventoryLogic.InventorySide.Equipment && transferCommand.FromSide != InventoryLogic.InventorySide.None)
				{
					int num2 = this._rosters[(int)transferCommand.FromSide].FindIndexOfElement(transferCommand.ElementToTransfer.EquipmentElement);
					ItemRosterElement elementCopyAtIndex = this._rosters[(int)transferCommand.FromSide].GetElementCopyAtIndex(num2);
					flag = transferCommand.Amount == elementCopyAtIndex.Amount;
				}
				bool flag2 = this.IsSell(transferCommand.FromSide, transferCommand.ToSide);
				bool flag3 = this.IsBuy(transferCommand.FromSide, transferCommand.ToSide);
				for (int i = 0; i < transferCommand.Amount; i++)
				{
					if (transferCommand.ToSide == InventoryLogic.InventorySide.Equipment && transferCommand.CharacterEquipment[(int)transferCommand.ToEquipmentIndex].Item != null)
					{
						TransferCommand transferCommand2 = TransferCommand.Transfer(1, InventoryLogic.InventorySide.Equipment, InventoryLogic.InventorySide.PlayerInventory, new ItemRosterElement(transferCommand.CharacterEquipment[(int)transferCommand.ToEquipmentIndex], 1), transferCommand.ToEquipmentIndex, EquipmentIndex.None, transferCommand.Character, transferCommand.IsCivilianEquipment);
						list.AddRange(this.TransferItem(ref transferCommand2));
					}
					ItemRosterElement elementToTransfer = transferCommand.ElementToTransfer;
					int itemPrice = this.GetItemPrice(transferCommand.ElementToTransfer, flag3);
					if (flag3 || flag2)
					{
						this._transactionHistory.RecordTransaction(transferCommand.ElementToTransfer, flag2, itemPrice);
					}
					if (this.IsTrading)
					{
						if (flag3)
						{
							num += itemPrice;
						}
						else if (flag2)
						{
							num -= itemPrice;
						}
					}
					if (transferCommand.FromSide == InventoryLogic.InventorySide.Equipment)
					{
						ItemRosterElement itemRosterElement = new ItemRosterElement(transferCommand.CharacterEquipment[(int)transferCommand.FromEquipmentIndex], transferCommand.Amount);
						itemRosterElement.Amount--;
						transferCommand.CharacterEquipment[(int)transferCommand.FromEquipmentIndex] = itemRosterElement.EquipmentElement;
					}
					else if (transferCommand.FromSide == InventoryLogic.InventorySide.PlayerInventory || transferCommand.FromSide == InventoryLogic.InventorySide.OtherInventory)
					{
						this._rosters[(int)transferCommand.FromSide].AddToCounts(transferCommand.ElementToTransfer.EquipmentElement, -1);
					}
					if (transferCommand.ToSide == InventoryLogic.InventorySide.Equipment)
					{
						ItemRosterElement elementToTransfer2 = transferCommand.ElementToTransfer;
						elementToTransfer2.Amount = 1;
						transferCommand.CharacterEquipment[(int)transferCommand.ToEquipmentIndex] = elementToTransfer2.EquipmentElement;
					}
					else if (transferCommand.ToSide == InventoryLogic.InventorySide.PlayerInventory || transferCommand.ToSide == InventoryLogic.InventorySide.OtherInventory)
					{
						this._rosters[(int)transferCommand.ToSide].AddToCounts(transferCommand.ElementToTransfer.EquipmentElement, 1);
					}
				}
				if (transferCommand.FromSide == InventoryLogic.InventorySide.Equipment)
				{
					ItemRosterElement itemRosterElement2 = new ItemRosterElement(transferCommand.CharacterEquipment[(int)transferCommand.FromEquipmentIndex], transferCommand.Amount);
					int amount = itemRosterElement2.Amount;
					itemRosterElement2.Amount = amount - 1;
					list.Add(new TransferCommandResult(transferCommand.FromSide, itemRosterElement2, -transferCommand.Amount, itemRosterElement2.Amount, transferCommand.FromEquipmentIndex, transferCommand.Character, transferCommand.IsCivilianEquipment));
				}
				else if (transferCommand.FromSide == InventoryLogic.InventorySide.PlayerInventory || transferCommand.FromSide == InventoryLogic.InventorySide.OtherInventory)
				{
					if (flag)
					{
						list.Add(new TransferCommandResult(transferCommand.FromSide, new ItemRosterElement(transferCommand.ElementToTransfer.EquipmentElement, 0), -transferCommand.Amount, 0, transferCommand.FromEquipmentIndex, transferCommand.Character, transferCommand.IsCivilianEquipment));
					}
					else
					{
						int num3 = this._rosters[(int)transferCommand.FromSide].FindIndexOfElement(transferCommand.ElementToTransfer.EquipmentElement);
						ItemRosterElement elementCopyAtIndex2 = this._rosters[(int)transferCommand.FromSide].GetElementCopyAtIndex(num3);
						list.Add(new TransferCommandResult(transferCommand.FromSide, elementCopyAtIndex2, -transferCommand.Amount, elementCopyAtIndex2.Amount, transferCommand.FromEquipmentIndex, transferCommand.Character, transferCommand.IsCivilianEquipment));
					}
				}
				if (transferCommand.ToSide == InventoryLogic.InventorySide.Equipment)
				{
					ItemRosterElement elementToTransfer3 = transferCommand.ElementToTransfer;
					elementToTransfer3.Amount = 1;
					list.Add(new TransferCommandResult(transferCommand.ToSide, elementToTransfer3, 1, 1, transferCommand.ToEquipmentIndex, transferCommand.Character, transferCommand.IsCivilianEquipment));
				}
				else if (transferCommand.ToSide == InventoryLogic.InventorySide.PlayerInventory || transferCommand.ToSide == InventoryLogic.InventorySide.OtherInventory)
				{
					int num4 = this._rosters[(int)transferCommand.ToSide].FindIndexOfElement(transferCommand.ElementToTransfer.EquipmentElement);
					ItemRosterElement elementCopyAtIndex3 = this._rosters[(int)transferCommand.ToSide].GetElementCopyAtIndex(num4);
					list.Add(new TransferCommandResult(transferCommand.ToSide, elementCopyAtIndex3, transferCommand.Amount, elementCopyAtIndex3.Amount, transferCommand.ToEquipmentIndex, transferCommand.Character, transferCommand.IsCivilianEquipment));
				}
				this.HandleDonationOnTransferItem(transferCommand.ElementToTransfer, transferCommand.Amount, flag3, flag2);
				this.TransactionDebt += num;
			}
			return list;
		}

		private bool IsSell(InventoryLogic.InventorySide fromSide, InventoryLogic.InventorySide toSide)
		{
			return toSide == InventoryLogic.InventorySide.OtherInventory && (fromSide == InventoryLogic.InventorySide.Equipment || fromSide == InventoryLogic.InventorySide.PlayerInventory);
		}

		private bool IsBuy(InventoryLogic.InventorySide fromSide, InventoryLogic.InventorySide toSide)
		{
			return fromSide == InventoryLogic.InventorySide.OtherInventory && (toSide == InventoryLogic.InventorySide.Equipment || toSide == InventoryLogic.InventorySide.PlayerInventory);
		}

		public void SlaughterItem(ItemRosterElement itemRosterElement)
		{
			List<TransferCommandResult> list = new List<TransferCommandResult>();
			EquipmentElement equipmentElement = itemRosterElement.EquipmentElement;
			int meatCount = equipmentElement.Item.HorseComponent.MeatCount;
			int hideCount = equipmentElement.Item.HorseComponent.HideCount;
			int num = this._rosters[1].AddToCounts(DefaultItems.Meat, meatCount);
			ItemRosterElement elementCopyAtIndex = this._rosters[1].GetElementCopyAtIndex(num);
			bool flag = itemRosterElement.Amount == 1;
			int num2 = this._rosters[1].AddToCounts(itemRosterElement.EquipmentElement, -1);
			if (flag)
			{
				list.Add(new TransferCommandResult(InventoryLogic.InventorySide.PlayerInventory, new ItemRosterElement(equipmentElement, 0), -1, 0, EquipmentIndex.None, null, equipmentElement.Item.IsCivilian));
			}
			else
			{
				ItemRosterElement elementCopyAtIndex2 = this._rosters[1].GetElementCopyAtIndex(num2);
				list.Add(new TransferCommandResult(InventoryLogic.InventorySide.PlayerInventory, elementCopyAtIndex2, -1, elementCopyAtIndex2.Amount, EquipmentIndex.None, null, elementCopyAtIndex2.EquipmentElement.Item.IsCivilian));
			}
			list.Add(new TransferCommandResult(InventoryLogic.InventorySide.PlayerInventory, elementCopyAtIndex, meatCount, elementCopyAtIndex.Amount, EquipmentIndex.None, null, elementCopyAtIndex.EquipmentElement.Item.IsCivilian));
			if (hideCount > 0)
			{
				int num3 = this._rosters[1].AddToCounts(DefaultItems.Hides, hideCount);
				ItemRosterElement elementCopyAtIndex3 = this._rosters[1].GetElementCopyAtIndex(num3);
				list.Add(new TransferCommandResult(InventoryLogic.InventorySide.PlayerInventory, elementCopyAtIndex3, hideCount, elementCopyAtIndex3.Amount, EquipmentIndex.None, null, elementCopyAtIndex3.EquipmentElement.Item.IsCivilian));
			}
			this.SetCurrentStateAsInitial();
			this.OnAfterTransfer(list);
		}

		public void DonateItem(ItemRosterElement itemRosterElement)
		{
			List<TransferCommandResult> list = new List<TransferCommandResult>();
			int tier = (int)itemRosterElement.EquipmentElement.Item.Tier;
			int num = 100 * (tier + 1);
			InventoryLogic.InventorySide inventorySide = InventoryLogic.InventorySide.PlayerInventory;
			int num2 = this._rosters[(int)inventorySide].AddToCounts(itemRosterElement.EquipmentElement, -1);
			ItemRosterElement elementCopyAtIndex = this._rosters[(int)inventorySide].GetElementCopyAtIndex(num2);
			list.Add(new TransferCommandResult(InventoryLogic.InventorySide.PlayerInventory, elementCopyAtIndex, -1, elementCopyAtIndex.Amount, EquipmentIndex.None, null, elementCopyAtIndex.EquipmentElement.Item.IsCivilian));
			if (num > 0)
			{
				TroopRosterElement randomElementWithPredicate = PartyBase.MainParty.MemberRoster.GetTroopRoster().GetRandomElementWithPredicate((TroopRosterElement m) => !m.Character.IsHero && m.Character.UpgradeTargets.Length != 0);
				if (randomElementWithPredicate.Character != null)
				{
					PartyBase.MainParty.MemberRoster.AddXpToTroop(num, randomElementWithPredicate.Character);
					TextObject textObject = new TextObject("{=Kwja0a4s}Added {XPAMOUNT} amount of xp to {TROOPNAME}", null);
					textObject.SetTextVariable("XPAMOUNT", num);
					textObject.SetTextVariable("TROOPNAME", randomElementWithPredicate.Character.Name.ToString());
					Debug.Print(textObject.ToString(), 0, Debug.DebugColor.White, 17592186044416UL);
					MBInformationManager.AddQuickInformation(textObject, 0, null, "");
				}
			}
			this.SetCurrentStateAsInitial();
			this.OnAfterTransfer(list);
		}

		private static bool TransferIsMovementValid(ref TransferCommand transferCommand)
		{
			if (transferCommand.ElementToTransfer.EquipmentElement.IsQuestItem)
			{
				BannerComponent bannerComponent = transferCommand.ElementToTransfer.EquipmentElement.Item.BannerComponent;
				if (((bannerComponent != null) ? bannerComponent.BannerEffect : null) == null || ((transferCommand.FromSide != InventoryLogic.InventorySide.PlayerInventory || transferCommand.ToSide != InventoryLogic.InventorySide.Equipment) && (transferCommand.FromSide != InventoryLogic.InventorySide.Equipment || transferCommand.ToSide != InventoryLogic.InventorySide.PlayerInventory)))
				{
					return false;
				}
			}
			bool flag = false;
			if (transferCommand.ToSide == InventoryLogic.InventorySide.Equipment)
			{
				InventoryItemType inventoryItemTypeOfItem = InventoryManager.GetInventoryItemTypeOfItem(transferCommand.ElementToTransfer.EquipmentElement.Item);
				switch (transferCommand.ToEquipmentIndex)
				{
				case EquipmentIndex.WeaponItemBeginSlot:
				case EquipmentIndex.Weapon1:
				case EquipmentIndex.Weapon2:
				case EquipmentIndex.Weapon3:
					flag = inventoryItemTypeOfItem == InventoryItemType.Weapon || inventoryItemTypeOfItem == InventoryItemType.Shield;
					break;
				case EquipmentIndex.ExtraWeaponSlot:
					flag = inventoryItemTypeOfItem == InventoryItemType.Banner;
					break;
				case EquipmentIndex.NumAllWeaponSlots:
					flag = inventoryItemTypeOfItem == InventoryItemType.HeadArmor;
					break;
				case EquipmentIndex.Body:
					flag = inventoryItemTypeOfItem == InventoryItemType.BodyArmor;
					break;
				case EquipmentIndex.Leg:
					flag = inventoryItemTypeOfItem == InventoryItemType.LegArmor;
					break;
				case EquipmentIndex.Gloves:
					flag = inventoryItemTypeOfItem == InventoryItemType.HandArmor;
					break;
				case EquipmentIndex.Cape:
					flag = inventoryItemTypeOfItem == InventoryItemType.Cape;
					break;
				case EquipmentIndex.ArmorItemEndSlot:
					flag = inventoryItemTypeOfItem == InventoryItemType.Horse;
					break;
				case EquipmentIndex.HorseHarness:
					flag = inventoryItemTypeOfItem == InventoryItemType.HorseHarness;
					break;
				}
			}
			else
			{
				flag = true;
			}
			return flag;
		}

		private bool DoesTransferItemExist(ref TransferCommand transferCommand)
		{
			if (transferCommand.FromSide == InventoryLogic.InventorySide.OtherInventory || transferCommand.FromSide == InventoryLogic.InventorySide.PlayerInventory)
			{
				return this.CheckItemRosterHasElement(transferCommand.FromSide, transferCommand.ElementToTransfer, transferCommand.Amount);
			}
			return transferCommand.FromSide == InventoryLogic.InventorySide.Equipment && transferCommand.CharacterEquipment[(int)transferCommand.FromEquipmentIndex].Item != null && transferCommand.ElementToTransfer.EquipmentElement.IsEqualTo(transferCommand.CharacterEquipment[(int)transferCommand.FromEquipmentIndex]);
		}

		public void TransferOne(ItemRosterElement itemRosterElement)
		{
		}

		public int GetElementCountOnSide(InventoryLogic.InventorySide side)
		{
			return this._rosters[(int)side].Count;
		}

		public IEnumerable<ItemRosterElement> GetElementsInInitialRoster(InventoryLogic.InventorySide side)
		{
			return this._rostersBackup[(int)side];
		}

		public IEnumerable<ItemRosterElement> GetElementsInRoster(InventoryLogic.InventorySide side)
		{
			return this._rosters[(int)side];
		}

		private void SetCurrentStateAsInitial()
		{
			for (int i = 0; i < this._rostersBackup.Length; i++)
			{
				this._rostersBackup[i] = new ItemRoster(this._rosters[i]);
			}
			this._partyInitialEquipment = new InventoryLogic.PartyEquipment(this.OwnerParty);
		}

		public ItemRosterElement? FindItemFromSide(InventoryLogic.InventorySide side, EquipmentElement item)
		{
			int num = this._rosters[(int)side].FindIndexOfElement(item);
			if (num >= 0)
			{
				return new ItemRosterElement?(this._rosters[(int)side].ElementAt(num));
			}
			return null;
		}

		private ItemRoster[] _rosters;

		private ItemRoster[] _rostersBackup;

		public bool IsPreviewingItem;

		private InventoryLogic.PartyEquipment _partyInitialEquipment;

		private float _xpGainFromDonations;

		private int _transactionDebt;

		private bool _playerAcceptsTraderOffer;

		private InventoryLogic.TransactionHistory _transactionHistory = new InventoryLogic.TransactionHistory();

		private Dictionary<ItemCategory, float> _itemCategoryAverages = new Dictionary<ItemCategory, float>();

		private bool _useBasePrices;

		public InventoryManager.InventoryCategoryType MerchantItemType = InventoryManager.InventoryCategoryType.None;

		public enum TransferType
		{
			Neutral,
			Sell,
			Buy
		}

		public enum InventorySide
		{
			OtherInventory,
			PlayerInventory,
			Equipment,
			None = -1
		}

		public delegate void AfterResetDelegate(InventoryLogic inventoryLogic, bool fromCancel);

		public delegate void TotalAmountChangeDelegate(int newTotalAmount);

		public delegate void ProcessResultListDelegate(InventoryLogic inventoryLogic, List<TransferCommandResult> results);

		private class PartyEquipment
		{
			public Dictionary<CharacterObject, Equipment[]> CharacterEquipments { get; private set; }

			public PartyEquipment(MobileParty party)
			{
				this.CharacterEquipments = new Dictionary<CharacterObject, Equipment[]>();
				this.InitializeCopyFrom(party);
			}

			public void InitializeCopyFrom(MobileParty party)
			{
				this.CharacterEquipments = new Dictionary<CharacterObject, Equipment[]>();
				for (int i = 0; i < party.MemberRoster.Count; i++)
				{
					CharacterObject character = party.MemberRoster.GetElementCopyAtIndex(i).Character;
					if (character.IsHero)
					{
						this.CharacterEquipments.Add(character, new Equipment[]
						{
							new Equipment(character.Equipment),
							new Equipment(character.FirstCivilianEquipment)
						});
					}
				}
			}

			internal void ResetEquipment(MobileParty ownerParty)
			{
				foreach (KeyValuePair<CharacterObject, Equipment[]> keyValuePair in this.CharacterEquipments)
				{
					keyValuePair.Key.Equipment.FillFrom(keyValuePair.Value[0], true);
					keyValuePair.Key.FirstCivilianEquipment.FillFrom(keyValuePair.Value[1], true);
				}
			}

			public void SetReference(InventoryLogic.PartyEquipment partyEquipment)
			{
				this.CharacterEquipments.Clear();
				this.CharacterEquipments = partyEquipment.CharacterEquipments;
			}

			public bool IsEqual(InventoryLogic.PartyEquipment partyEquipment)
			{
				if (partyEquipment.CharacterEquipments.Keys.Count != this.CharacterEquipments.Keys.Count)
				{
					return false;
				}
				foreach (CharacterObject characterObject in partyEquipment.CharacterEquipments.Keys)
				{
					if (!this.CharacterEquipments.Keys.Contains(characterObject))
					{
						return false;
					}
					Equipment[] array;
					if (!this.CharacterEquipments.TryGetValue(characterObject, out array))
					{
						return false;
					}
					Equipment[] array2;
					if (!partyEquipment.CharacterEquipments.TryGetValue(characterObject, out array2) || array2.Length != array.Length)
					{
						return false;
					}
					for (int i = 0; i < array.Length; i++)
					{
						if (!array[i].IsEquipmentEqualTo(array2[i]))
						{
							return false;
						}
					}
				}
				return true;
			}
		}

		private class ItemLog : IReadOnlyCollection<int>, IEnumerable<int>, IEnumerable
		{
			public bool IsSelling
			{
				get
				{
					return this._isSelling;
				}
			}

			public int Count
			{
				get
				{
					return ((IReadOnlyCollection<int>)this._transactions).Count;
				}
			}

			private void AddTransaction(int price, bool isSelling)
			{
				if (this._transactions.IsEmpty<int>())
				{
					this._isSelling = isSelling;
				}
				this._transactions.Add(price);
			}

			private void RemoveLastTransaction()
			{
				if (!this._transactions.IsEmpty<int>())
				{
					this._transactions.RemoveAt(this._transactions.Count - 1);
					return;
				}
				Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\Inventory\\InventoryLogic.cs", "RemoveLastTransaction", 1137);
			}

			public void RecordTransaction(int price, bool isSelling)
			{
				if (!this._transactions.IsEmpty<int>() && isSelling != this._isSelling)
				{
					this.RemoveLastTransaction();
					return;
				}
				this.AddTransaction(price, isSelling);
			}

			public bool GetLastTransaction(out int price, out bool isSelling)
			{
				if (this._transactions.IsEmpty<int>())
				{
					price = 0;
					isSelling = false;
					return false;
				}
				price = this._transactions[this._transactions.Count - 1];
				isSelling = this._isSelling;
				return true;
			}

			public IEnumerator<int> GetEnumerator()
			{
				return ((IEnumerable<int>)this._transactions).GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return ((IEnumerable<int>)this._transactions).GetEnumerator();
			}

			private List<int> _transactions = new List<int>();

			private bool _isSelling;
		}

		private class TransactionHistory
		{
			internal void RecordTransaction(ItemRosterElement elementToTransfer, bool isSelling, int price)
			{
				InventoryLogic.ItemLog itemLog;
				if (!this._transactionLogs.TryGetValue(elementToTransfer, out itemLog))
				{
					itemLog = new InventoryLogic.ItemLog();
					this._transactionLogs[elementToTransfer] = itemLog;
				}
				itemLog.RecordTransaction(price, isSelling);
			}

			public bool IsEmpty
			{
				get
				{
					return this._transactionLogs.IsEmpty<KeyValuePair<ItemRosterElement, InventoryLogic.ItemLog>>();
				}
			}

			public void Clear()
			{
				this._transactionLogs.Clear();
			}

			public bool GetLastTransfer(ItemRosterElement itemRosterElement, out int lastPrice, out bool lastIsSelling)
			{
				InventoryLogic.ItemLog itemLog;
				bool flag = this._transactionLogs.TryGetValue(itemRosterElement, out itemLog);
				lastPrice = 0;
				lastIsSelling = false;
				return flag && itemLog.GetLastTransaction(out lastPrice, out lastIsSelling);
			}

			internal List<ValueTuple<ItemRosterElement, int>> GetTransferredItems(bool isSelling)
			{
				List<ValueTuple<ItemRosterElement, int>> list = new List<ValueTuple<ItemRosterElement, int>>();
				foreach (KeyValuePair<ItemRosterElement, InventoryLogic.ItemLog> keyValuePair in this._transactionLogs)
				{
					if (keyValuePair.Value.Count > 0 && !keyValuePair.Value.IsSelling == isSelling)
					{
						int num = keyValuePair.Value.Sum();
						list.Add(new ValueTuple<ItemRosterElement, int>(new ItemRosterElement(keyValuePair.Key.EquipmentElement.Item, keyValuePair.Value.Count, keyValuePair.Key.EquipmentElement.ItemModifier), num));
					}
				}
				return list;
			}

			internal List<ValueTuple<ItemRosterElement, int>> GetBoughtItems()
			{
				return this.GetTransferredItems(true);
			}

			internal List<ValueTuple<ItemRosterElement, int>> GetSoldItems()
			{
				return this.GetTransferredItems(false);
			}

			private Dictionary<ItemRosterElement, InventoryLogic.ItemLog> _transactionLogs = new Dictionary<ItemRosterElement, InventoryLogic.ItemLog>();
		}
	}
}
