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
	// Token: 0x020000D3 RID: 211
	public class InventoryLogic
	{
		// Token: 0x17000551 RID: 1361
		// (get) Token: 0x060012BA RID: 4794 RVA: 0x000548ED File Offset: 0x00052AED
		// (set) Token: 0x060012BB RID: 4795 RVA: 0x000548F5 File Offset: 0x00052AF5
		public bool DisableNetwork { get; set; }

		// Token: 0x17000552 RID: 1362
		// (get) Token: 0x060012BC RID: 4796 RVA: 0x000548FE File Offset: 0x00052AFE
		// (set) Token: 0x060012BD RID: 4797 RVA: 0x00054906 File Offset: 0x00052B06
		public Action<int> TotalAmountChange { get; set; }

		// Token: 0x17000553 RID: 1363
		// (get) Token: 0x060012BE RID: 4798 RVA: 0x0005490F File Offset: 0x00052B0F
		// (set) Token: 0x060012BF RID: 4799 RVA: 0x00054917 File Offset: 0x00052B17
		public Action DonationXpChange { get; set; }

		// Token: 0x14000001 RID: 1
		// (add) Token: 0x060012C0 RID: 4800 RVA: 0x00054920 File Offset: 0x00052B20
		// (remove) Token: 0x060012C1 RID: 4801 RVA: 0x00054958 File Offset: 0x00052B58
		public event InventoryLogic.AfterResetDelegate AfterReset;

		// Token: 0x14000002 RID: 2
		// (add) Token: 0x060012C2 RID: 4802 RVA: 0x00054990 File Offset: 0x00052B90
		// (remove) Token: 0x060012C3 RID: 4803 RVA: 0x000549C8 File Offset: 0x00052BC8
		public event InventoryLogic.ProcessResultListDelegate AfterTransfer;

		// Token: 0x17000554 RID: 1364
		// (get) Token: 0x060012C4 RID: 4804 RVA: 0x000549FD File Offset: 0x00052BFD
		// (set) Token: 0x060012C5 RID: 4805 RVA: 0x00054A05 File Offset: 0x00052C05
		public TroopRoster RightMemberRoster { get; private set; }

		// Token: 0x17000555 RID: 1365
		// (get) Token: 0x060012C6 RID: 4806 RVA: 0x00054A0E File Offset: 0x00052C0E
		// (set) Token: 0x060012C7 RID: 4807 RVA: 0x00054A16 File Offset: 0x00052C16
		public TroopRoster LeftMemberRoster { get; private set; }

		// Token: 0x17000556 RID: 1366
		// (get) Token: 0x060012C8 RID: 4808 RVA: 0x00054A1F File Offset: 0x00052C1F
		// (set) Token: 0x060012C9 RID: 4809 RVA: 0x00054A27 File Offset: 0x00052C27
		public CharacterObject InitialEquipmentCharacter { get; private set; }

		// Token: 0x17000557 RID: 1367
		// (get) Token: 0x060012CA RID: 4810 RVA: 0x00054A30 File Offset: 0x00052C30
		// (set) Token: 0x060012CB RID: 4811 RVA: 0x00054A38 File Offset: 0x00052C38
		public bool IsTrading { get; private set; }

		// Token: 0x17000558 RID: 1368
		// (get) Token: 0x060012CC RID: 4812 RVA: 0x00054A41 File Offset: 0x00052C41
		// (set) Token: 0x060012CD RID: 4813 RVA: 0x00054A49 File Offset: 0x00052C49
		public bool IsSpecialActionsPermitted { get; private set; }

		// Token: 0x17000559 RID: 1369
		// (get) Token: 0x060012CE RID: 4814 RVA: 0x00054A52 File Offset: 0x00052C52
		// (set) Token: 0x060012CF RID: 4815 RVA: 0x00054A5A File Offset: 0x00052C5A
		public CharacterObject OwnerCharacter { get; private set; }

		// Token: 0x1700055A RID: 1370
		// (get) Token: 0x060012D0 RID: 4816 RVA: 0x00054A63 File Offset: 0x00052C63
		// (set) Token: 0x060012D1 RID: 4817 RVA: 0x00054A6B File Offset: 0x00052C6B
		public MobileParty OwnerParty { get; private set; }

		// Token: 0x1700055B RID: 1371
		// (get) Token: 0x060012D2 RID: 4818 RVA: 0x00054A74 File Offset: 0x00052C74
		// (set) Token: 0x060012D3 RID: 4819 RVA: 0x00054A7C File Offset: 0x00052C7C
		public PartyBase OtherParty { get; private set; }

		// Token: 0x1700055C RID: 1372
		// (get) Token: 0x060012D4 RID: 4820 RVA: 0x00054A85 File Offset: 0x00052C85
		// (set) Token: 0x060012D5 RID: 4821 RVA: 0x00054A8D File Offset: 0x00052C8D
		public IMarketData MarketData { get; private set; }

		// Token: 0x1700055D RID: 1373
		// (get) Token: 0x060012D6 RID: 4822 RVA: 0x00054A96 File Offset: 0x00052C96
		// (set) Token: 0x060012D7 RID: 4823 RVA: 0x00054A9E File Offset: 0x00052C9E
		public TextObject LeftRosterName { get; private set; }

		// Token: 0x1700055E RID: 1374
		// (get) Token: 0x060012D8 RID: 4824 RVA: 0x00054AA7 File Offset: 0x00052CA7
		// (set) Token: 0x060012D9 RID: 4825 RVA: 0x00054AAF File Offset: 0x00052CAF
		public bool IsDiscardDonating { get; private set; }

		// Token: 0x1700055F RID: 1375
		// (get) Token: 0x060012DA RID: 4826 RVA: 0x00054AB8 File Offset: 0x00052CB8
		// (set) Token: 0x060012DB RID: 4827 RVA: 0x00054AC0 File Offset: 0x00052CC0
		public bool IsOtherPartyFromPlayerClan { get; private set; }

		// Token: 0x17000560 RID: 1376
		// (get) Token: 0x060012DC RID: 4828 RVA: 0x00054AC9 File Offset: 0x00052CC9
		// (set) Token: 0x060012DD RID: 4829 RVA: 0x00054AD1 File Offset: 0x00052CD1
		public InventoryListener InventoryListener { get; private set; }

		// Token: 0x17000561 RID: 1377
		// (get) Token: 0x060012DE RID: 4830 RVA: 0x00054ADA File Offset: 0x00052CDA
		public int TotalAmount
		{
			get
			{
				return this.TransactionDebt;
			}
		}

		// Token: 0x17000562 RID: 1378
		// (get) Token: 0x060012DF RID: 4831 RVA: 0x00054AE2 File Offset: 0x00052CE2
		public PartyBase OppositePartyFromListener
		{
			get
			{
				return this.InventoryListener.GetOppositeParty();
			}
		}

		// Token: 0x17000563 RID: 1379
		// (get) Token: 0x060012E0 RID: 4832 RVA: 0x00054AEF File Offset: 0x00052CEF
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

		// Token: 0x17000564 RID: 1380
		// (get) Token: 0x060012E1 RID: 4833 RVA: 0x00054B04 File Offset: 0x00052D04
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

		// Token: 0x17000565 RID: 1381
		// (get) Token: 0x060012E2 RID: 4834 RVA: 0x00054B71 File Offset: 0x00052D71
		// (set) Token: 0x060012E3 RID: 4835 RVA: 0x00054B79 File Offset: 0x00052D79
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

		// Token: 0x17000566 RID: 1382
		// (get) Token: 0x060012E4 RID: 4836 RVA: 0x00054B9C File Offset: 0x00052D9C
		// (set) Token: 0x060012E5 RID: 4837 RVA: 0x00054BA4 File Offset: 0x00052DA4
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

		// Token: 0x060012E6 RID: 4838 RVA: 0x00054BE0 File Offset: 0x00052DE0
		public InventoryLogic(MobileParty ownerParty, CharacterObject ownerCharacter, PartyBase merchantParty)
		{
			this._rosters = new ItemRoster[2];
			this._rostersBackup = new ItemRoster[2];
			this.OwnerParty = ownerParty;
			this.OwnerCharacter = ownerCharacter;
			this.OtherParty = merchantParty;
		}

		// Token: 0x060012E7 RID: 4839 RVA: 0x00054C3D File Offset: 0x00052E3D
		public InventoryLogic(PartyBase merchantParty)
			: this(MobileParty.MainParty, CharacterObject.PlayerCharacter, merchantParty)
		{
		}

		// Token: 0x060012E8 RID: 4840 RVA: 0x00054C50 File Offset: 0x00052E50
		public void Initialize(ItemRoster leftItemRoster, MobileParty party, bool isTrading, bool isSpecialActionsPermitted, CharacterObject initialCharacterOfRightRoster, InventoryManager.InventoryCategoryType merchantItemType, IMarketData marketData, bool useBasePrices, TextObject leftRosterName = null, TroopRoster leftMemberRoster = null)
		{
			this.Initialize(leftItemRoster, party.ItemRoster, party.MemberRoster, isTrading, isSpecialActionsPermitted, initialCharacterOfRightRoster, merchantItemType, marketData, useBasePrices, leftRosterName, leftMemberRoster);
		}

		// Token: 0x060012E9 RID: 4841 RVA: 0x00054C80 File Offset: 0x00052E80
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

		// Token: 0x060012EA RID: 4842 RVA: 0x00054D52 File Offset: 0x00052F52
		private void InitializeRosters(ItemRoster leftItemRoster, ItemRoster rightItemRoster, TroopRoster rightMemberRoster, CharacterObject initialCharacterOfRightRoster, TroopRoster leftMemberRoster = null)
		{
			this._rosters[0] = leftItemRoster;
			this._rosters[1] = rightItemRoster;
			this.RightMemberRoster = rightMemberRoster;
			this.LeftMemberRoster = leftMemberRoster;
			this.InitialEquipmentCharacter = initialCharacterOfRightRoster;
			this.SetCurrentStateAsInitial();
		}

		// Token: 0x060012EB RID: 4843 RVA: 0x00054D83 File Offset: 0x00052F83
		public int GetItemTotalPrice(ItemRosterElement itemRosterElement, int absStockChange, out int lastPrice, bool isBuying)
		{
			lastPrice = this.GetItemPrice(itemRosterElement, isBuying);
			return lastPrice;
		}

		// Token: 0x060012EC RID: 4844 RVA: 0x00054D92 File Offset: 0x00052F92
		public void SetPlayerAcceptTraderOffer()
		{
			this._playerAcceptsTraderOffer = true;
		}

		// Token: 0x060012ED RID: 4845 RVA: 0x00054D9C File Offset: 0x00052F9C
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

		// Token: 0x060012EE RID: 4846 RVA: 0x0005511C File Offset: 0x0005331C
		public List<ValueTuple<ItemRosterElement, int>> GetBoughtItems()
		{
			return this._transactionHistory.GetBoughtItems();
		}

		// Token: 0x060012EF RID: 4847 RVA: 0x00055129 File Offset: 0x00053329
		public List<ValueTuple<ItemRosterElement, int>> GetSoldItems()
		{
			return this._transactionHistory.GetSoldItems();
		}

		// Token: 0x060012F0 RID: 4848 RVA: 0x00055138 File Offset: 0x00053338
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

		// Token: 0x060012F1 RID: 4849 RVA: 0x0005525C File Offset: 0x0005345C
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

		// Token: 0x060012F2 RID: 4850 RVA: 0x000552B0 File Offset: 0x000534B0
		private void HandleDonationOnTransferItem(ItemRosterElement rosterElement, int amount, bool isBuying, bool isSelling)
		{
			ItemObject item = rosterElement.EquipmentElement.Item;
			ItemDiscardModel itemDiscardModel = Campaign.Current.Models.ItemDiscardModel;
			if (this.IsDiscardDonating && (isSelling || isBuying) && item != null)
			{
				this.XpGainFromDonations += (float)(itemDiscardModel.GetXpBonusForDiscardingItem(item, amount) * (isSelling ? 1 : (-1)));
			}
		}

		// Token: 0x060012F3 RID: 4851 RVA: 0x0005530D File Offset: 0x0005350D
		public float GetAveragePriceFactorItemCategory(ItemCategory category)
		{
			if (this._itemCategoryAverages.ContainsKey(category))
			{
				return this._itemCategoryAverages[category];
			}
			return -99f;
		}

		// Token: 0x060012F4 RID: 4852 RVA: 0x0005532F File Offset: 0x0005352F
		public bool IsThereAnyChanges()
		{
			return this.IsThereAnyChangeBetweenRosters(this._rosters[1], this._rostersBackup[1]) || !this._partyInitialEquipment.IsEqual(new InventoryLogic.PartyEquipment(this.OwnerParty));
		}

		// Token: 0x060012F5 RID: 4853 RVA: 0x00055364 File Offset: 0x00053564
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

		// Token: 0x060012F6 RID: 4854 RVA: 0x000553DC File Offset: 0x000535DC
		public void Reset(bool fromCancel)
		{
			this.ResetLogic(fromCancel);
		}

		// Token: 0x060012F7 RID: 4855 RVA: 0x000553E8 File Offset: 0x000535E8
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

		// Token: 0x060012F8 RID: 4856 RVA: 0x00055488 File Offset: 0x00053688
		public bool CanPlayerCompleteTransaction()
		{
			return !this.IsPreviewingItem || !this.IsTrading || this.TotalAmount <= 0 || (this.TotalAmount >= 0 && this.OwnerCharacter.HeroObject.Gold - this.TotalAmount >= 0);
		}

		// Token: 0x060012F9 RID: 4857 RVA: 0x000554D8 File Offset: 0x000536D8
		public bool CanSlaughterItem(ItemRosterElement element, InventoryLogic.InventorySide sideOfItem)
		{
			return (!this.IsTrading || this._transactionHistory.IsEmpty) && (this.IsSpecialActionsPermitted && this.IsSlaughterable(element.EquipmentElement.Item) && sideOfItem == InventoryLogic.InventorySide.PlayerInventory && element.Amount > 0) && !this._transactionHistory.GetBoughtItems().Any((ValueTuple<ItemRosterElement, int> i) => i.Item1.EquipmentElement.Item == element.EquipmentElement.Item);
		}

		// Token: 0x060012FA RID: 4858 RVA: 0x0005555F File Offset: 0x0005375F
		public bool IsSlaughterable(ItemObject item)
		{
			return item.Type == ItemObject.ItemTypeEnum.Animal || item.Type == ItemObject.ItemTypeEnum.Horse;
		}

		// Token: 0x060012FB RID: 4859 RVA: 0x00055578 File Offset: 0x00053778
		public bool CanDonateItem(ItemRosterElement element, InventoryLogic.InventorySide sideOfItem)
		{
			return Game.Current.IsDevelopmentMode && this.IsSpecialActionsPermitted && element.Amount > 0 && this.IsDonatable(element.EquipmentElement.Item) && sideOfItem == InventoryLogic.InventorySide.PlayerInventory;
		}

		// Token: 0x060012FC RID: 4860 RVA: 0x000555C0 File Offset: 0x000537C0
		public bool IsDonatable(ItemObject item)
		{
			return item.Type == ItemObject.ItemTypeEnum.Arrows || item.Type == ItemObject.ItemTypeEnum.BodyArmor || item.Type == ItemObject.ItemTypeEnum.Bolts || item.Type == ItemObject.ItemTypeEnum.Bow || item.Type == ItemObject.ItemTypeEnum.Bullets || item.Type == ItemObject.ItemTypeEnum.Cape || item.Type == ItemObject.ItemTypeEnum.ChestArmor || item.Type == ItemObject.ItemTypeEnum.Crossbow || item.Type == ItemObject.ItemTypeEnum.HandArmor || item.Type == ItemObject.ItemTypeEnum.HeadArmor || item.Type == ItemObject.ItemTypeEnum.HorseHarness || item.Type == ItemObject.ItemTypeEnum.LegArmor || item.Type == ItemObject.ItemTypeEnum.Musket || item.Type == ItemObject.ItemTypeEnum.OneHandedWeapon || item.Type == ItemObject.ItemTypeEnum.Pistol || item.Type == ItemObject.ItemTypeEnum.Polearm || item.Type == ItemObject.ItemTypeEnum.Shield || item.Type == ItemObject.ItemTypeEnum.Thrown || item.Type == ItemObject.ItemTypeEnum.TwoHandedWeapon;
		}

		// Token: 0x060012FD RID: 4861 RVA: 0x00055695 File Offset: 0x00053895
		public void SetInventoryListener(InventoryListener inventoryListener)
		{
			this.InventoryListener = inventoryListener;
		}

		// Token: 0x060012FE RID: 4862 RVA: 0x000556A0 File Offset: 0x000538A0
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

		// Token: 0x060012FF RID: 4863 RVA: 0x00055710 File Offset: 0x00053910
		public int GetCostOfItemRosterElement(ItemRosterElement itemRosterElement, InventoryLogic.InventorySide side)
		{
			bool flag = side == InventoryLogic.InventorySide.OtherInventory && this.IsTrading;
			return this.GetItemPrice(itemRosterElement, flag);
		}

		// Token: 0x06001300 RID: 4864 RVA: 0x00055734 File Offset: 0x00053934
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

		// Token: 0x06001301 RID: 4865 RVA: 0x000557CC File Offset: 0x000539CC
		public void AddTransferCommand(TransferCommand command)
		{
			this.ProcessTransferCommand(command);
		}

		// Token: 0x06001302 RID: 4866 RVA: 0x000557D8 File Offset: 0x000539D8
		public void AddTransferCommands(IEnumerable<TransferCommand> commands)
		{
			foreach (TransferCommand transferCommand in commands)
			{
				this.ProcessTransferCommand(transferCommand);
			}
		}

		// Token: 0x06001303 RID: 4867 RVA: 0x00055820 File Offset: 0x00053A20
		public bool CheckItemRosterHasElement(InventoryLogic.InventorySide side, ItemRosterElement rosterElement, int number)
		{
			int num = this._rosters[(int)side].FindIndexOfElement(rosterElement.EquipmentElement);
			return num != -1 && this._rosters[(int)side].GetElementCopyAtIndex(num).Amount >= number;
		}

		// Token: 0x06001304 RID: 4868 RVA: 0x00055864 File Offset: 0x00053A64
		private void ProcessTransferCommand(TransferCommand command)
		{
			List<TransferCommandResult> list = this.TransferItem(ref command);
			this.OnAfterTransfer(list);
		}

		// Token: 0x06001305 RID: 4869 RVA: 0x00055884 File Offset: 0x00053A84
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

		// Token: 0x06001306 RID: 4870 RVA: 0x00055D90 File Offset: 0x00053F90
		private bool IsSell(InventoryLogic.InventorySide fromSide, InventoryLogic.InventorySide toSide)
		{
			return toSide == InventoryLogic.InventorySide.OtherInventory && (fromSide == InventoryLogic.InventorySide.Equipment || fromSide == InventoryLogic.InventorySide.PlayerInventory);
		}

		// Token: 0x06001307 RID: 4871 RVA: 0x00055DA1 File Offset: 0x00053FA1
		private bool IsBuy(InventoryLogic.InventorySide fromSide, InventoryLogic.InventorySide toSide)
		{
			return fromSide == InventoryLogic.InventorySide.OtherInventory && (toSide == InventoryLogic.InventorySide.Equipment || toSide == InventoryLogic.InventorySide.PlayerInventory);
		}

		// Token: 0x06001308 RID: 4872 RVA: 0x00055DB4 File Offset: 0x00053FB4
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

		// Token: 0x06001309 RID: 4873 RVA: 0x00055F30 File Offset: 0x00054130
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

		// Token: 0x0600130A RID: 4874 RVA: 0x0005607C File Offset: 0x0005427C
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

		// Token: 0x0600130B RID: 4875 RVA: 0x000561B4 File Offset: 0x000543B4
		private bool DoesTransferItemExist(ref TransferCommand transferCommand)
		{
			if (transferCommand.FromSide == InventoryLogic.InventorySide.OtherInventory || transferCommand.FromSide == InventoryLogic.InventorySide.PlayerInventory)
			{
				return this.CheckItemRosterHasElement(transferCommand.FromSide, transferCommand.ElementToTransfer, transferCommand.Amount);
			}
			return transferCommand.FromSide == InventoryLogic.InventorySide.Equipment && transferCommand.CharacterEquipment[(int)transferCommand.FromEquipmentIndex].Item != null && transferCommand.ElementToTransfer.EquipmentElement.IsEqualTo(transferCommand.CharacterEquipment[(int)transferCommand.FromEquipmentIndex]);
		}

		// Token: 0x0600130C RID: 4876 RVA: 0x0005623A File Offset: 0x0005443A
		public void TransferOne(ItemRosterElement itemRosterElement)
		{
		}

		// Token: 0x0600130D RID: 4877 RVA: 0x0005623C File Offset: 0x0005443C
		public int GetElementCountOnSide(InventoryLogic.InventorySide side)
		{
			return this._rosters[(int)side].Count;
		}

		// Token: 0x0600130E RID: 4878 RVA: 0x0005624B File Offset: 0x0005444B
		public IEnumerable<ItemRosterElement> GetElementsInInitialRoster(InventoryLogic.InventorySide side)
		{
			return this._rostersBackup[(int)side];
		}

		// Token: 0x0600130F RID: 4879 RVA: 0x00056255 File Offset: 0x00054455
		public IEnumerable<ItemRosterElement> GetElementsInRoster(InventoryLogic.InventorySide side)
		{
			return this._rosters[(int)side];
		}

		// Token: 0x06001310 RID: 4880 RVA: 0x00056260 File Offset: 0x00054460
		private void SetCurrentStateAsInitial()
		{
			for (int i = 0; i < this._rostersBackup.Length; i++)
			{
				this._rostersBackup[i] = new ItemRoster(this._rosters[i]);
			}
			this._partyInitialEquipment = new InventoryLogic.PartyEquipment(this.OwnerParty);
		}

		// Token: 0x06001311 RID: 4881 RVA: 0x000562A8 File Offset: 0x000544A8
		public ItemRosterElement? FindItemFromSide(InventoryLogic.InventorySide side, EquipmentElement item)
		{
			int num = this._rosters[(int)side].FindIndexOfElement(item);
			if (num >= 0)
			{
				return new ItemRosterElement?(this._rosters[(int)side].ElementAt(num));
			}
			return null;
		}

		// Token: 0x0400068F RID: 1679
		private ItemRoster[] _rosters;

		// Token: 0x04000690 RID: 1680
		private ItemRoster[] _rostersBackup;

		// Token: 0x04000696 RID: 1686
		public bool IsPreviewingItem;

		// Token: 0x04000697 RID: 1687
		private InventoryLogic.PartyEquipment _partyInitialEquipment;

		// Token: 0x0400069C RID: 1692
		private float _xpGainFromDonations;

		// Token: 0x0400069D RID: 1693
		private int _transactionDebt;

		// Token: 0x0400069E RID: 1694
		private bool _playerAcceptsTraderOffer;

		// Token: 0x0400069F RID: 1695
		private InventoryLogic.TransactionHistory _transactionHistory = new InventoryLogic.TransactionHistory();

		// Token: 0x040006A0 RID: 1696
		private Dictionary<ItemCategory, float> _itemCategoryAverages = new Dictionary<ItemCategory, float>();

		// Token: 0x040006A1 RID: 1697
		private bool _useBasePrices;

		// Token: 0x040006A2 RID: 1698
		public InventoryManager.InventoryCategoryType MerchantItemType = InventoryManager.InventoryCategoryType.None;

		// Token: 0x020004DF RID: 1247
		public enum TransferType
		{
			// Token: 0x04001511 RID: 5393
			Neutral,
			// Token: 0x04001512 RID: 5394
			Sell,
			// Token: 0x04001513 RID: 5395
			Buy
		}

		// Token: 0x020004E0 RID: 1248
		public enum InventorySide
		{
			// Token: 0x04001515 RID: 5397
			OtherInventory,
			// Token: 0x04001516 RID: 5398
			PlayerInventory,
			// Token: 0x04001517 RID: 5399
			Equipment,
			// Token: 0x04001518 RID: 5400
			None = -1
		}

		// Token: 0x020004E1 RID: 1249
		// (Invoke) Token: 0x060041A9 RID: 16809
		public delegate void AfterResetDelegate(InventoryLogic inventoryLogic, bool fromCancel);

		// Token: 0x020004E2 RID: 1250
		// (Invoke) Token: 0x060041AD RID: 16813
		public delegate void TotalAmountChangeDelegate(int newTotalAmount);

		// Token: 0x020004E3 RID: 1251
		// (Invoke) Token: 0x060041B1 RID: 16817
		public delegate void ProcessResultListDelegate(InventoryLogic inventoryLogic, List<TransferCommandResult> results);

		// Token: 0x020004E4 RID: 1252
		private class PartyEquipment
		{
			// Token: 0x17000D8E RID: 3470
			// (get) Token: 0x060041B4 RID: 16820 RVA: 0x001341A4 File Offset: 0x001323A4
			// (set) Token: 0x060041B5 RID: 16821 RVA: 0x001341AC File Offset: 0x001323AC
			public Dictionary<CharacterObject, Equipment[]> CharacterEquipments { get; private set; }

			// Token: 0x060041B6 RID: 16822 RVA: 0x001341B5 File Offset: 0x001323B5
			public PartyEquipment(MobileParty party)
			{
				this.CharacterEquipments = new Dictionary<CharacterObject, Equipment[]>();
				this.InitializeCopyFrom(party);
			}

			// Token: 0x060041B7 RID: 16823 RVA: 0x001341D0 File Offset: 0x001323D0
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

			// Token: 0x060041B8 RID: 16824 RVA: 0x00134248 File Offset: 0x00132448
			internal void ResetEquipment(MobileParty ownerParty)
			{
				foreach (KeyValuePair<CharacterObject, Equipment[]> keyValuePair in this.CharacterEquipments)
				{
					keyValuePair.Key.Equipment.FillFrom(keyValuePair.Value[0], true);
					keyValuePair.Key.FirstCivilianEquipment.FillFrom(keyValuePair.Value[1], true);
				}
			}

			// Token: 0x060041B9 RID: 16825 RVA: 0x001342CC File Offset: 0x001324CC
			public void SetReference(InventoryLogic.PartyEquipment partyEquipment)
			{
				this.CharacterEquipments.Clear();
				this.CharacterEquipments = partyEquipment.CharacterEquipments;
			}

			// Token: 0x060041BA RID: 16826 RVA: 0x001342E8 File Offset: 0x001324E8
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

		// Token: 0x020004E5 RID: 1253
		private class ItemLog : IReadOnlyCollection<int>, IEnumerable<int>, IEnumerable
		{
			// Token: 0x17000D8F RID: 3471
			// (get) Token: 0x060041BB RID: 16827 RVA: 0x001343D4 File Offset: 0x001325D4
			public bool IsSelling
			{
				get
				{
					return this._isSelling;
				}
			}

			// Token: 0x17000D90 RID: 3472
			// (get) Token: 0x060041BC RID: 16828 RVA: 0x001343DC File Offset: 0x001325DC
			public int Count
			{
				get
				{
					return ((IReadOnlyCollection<int>)this._transactions).Count;
				}
			}

			// Token: 0x060041BD RID: 16829 RVA: 0x001343E9 File Offset: 0x001325E9
			private void AddTransaction(int price, bool isSelling)
			{
				if (this._transactions.IsEmpty<int>())
				{
					this._isSelling = isSelling;
				}
				this._transactions.Add(price);
			}

			// Token: 0x060041BE RID: 16830 RVA: 0x0013440C File Offset: 0x0013260C
			private void RemoveLastTransaction()
			{
				if (!this._transactions.IsEmpty<int>())
				{
					this._transactions.RemoveAt(this._transactions.Count - 1);
					return;
				}
				Debug.FailedAssert("false", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\Inventory\\InventoryLogic.cs", "RemoveLastTransaction", 1137);
			}

			// Token: 0x060041BF RID: 16831 RVA: 0x00134458 File Offset: 0x00132658
			public void RecordTransaction(int price, bool isSelling)
			{
				if (!this._transactions.IsEmpty<int>() && isSelling != this._isSelling)
				{
					this.RemoveLastTransaction();
					return;
				}
				this.AddTransaction(price, isSelling);
			}

			// Token: 0x060041C0 RID: 16832 RVA: 0x0013447F File Offset: 0x0013267F
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

			// Token: 0x060041C1 RID: 16833 RVA: 0x001344B9 File Offset: 0x001326B9
			public IEnumerator<int> GetEnumerator()
			{
				return ((IEnumerable<int>)this._transactions).GetEnumerator();
			}

			// Token: 0x060041C2 RID: 16834 RVA: 0x001344C6 File Offset: 0x001326C6
			IEnumerator IEnumerable.GetEnumerator()
			{
				return ((IEnumerable<int>)this._transactions).GetEnumerator();
			}

			// Token: 0x0400151A RID: 5402
			private List<int> _transactions = new List<int>();

			// Token: 0x0400151B RID: 5403
			private bool _isSelling;
		}

		// Token: 0x020004E6 RID: 1254
		private class TransactionHistory
		{
			// Token: 0x060041C4 RID: 16836 RVA: 0x001344E8 File Offset: 0x001326E8
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

			// Token: 0x17000D91 RID: 3473
			// (get) Token: 0x060041C5 RID: 16837 RVA: 0x00134520 File Offset: 0x00132720
			public bool IsEmpty
			{
				get
				{
					return this._transactionLogs.IsEmpty<KeyValuePair<ItemRosterElement, InventoryLogic.ItemLog>>();
				}
			}

			// Token: 0x060041C6 RID: 16838 RVA: 0x0013452D File Offset: 0x0013272D
			public void Clear()
			{
				this._transactionLogs.Clear();
			}

			// Token: 0x060041C7 RID: 16839 RVA: 0x0013453C File Offset: 0x0013273C
			public bool GetLastTransfer(ItemRosterElement itemRosterElement, out int lastPrice, out bool lastIsSelling)
			{
				InventoryLogic.ItemLog itemLog;
				bool flag = this._transactionLogs.TryGetValue(itemRosterElement, out itemLog);
				lastPrice = 0;
				lastIsSelling = false;
				return flag && itemLog.GetLastTransaction(out lastPrice, out lastIsSelling);
			}

			// Token: 0x060041C8 RID: 16840 RVA: 0x0013456C File Offset: 0x0013276C
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

			// Token: 0x060041C9 RID: 16841 RVA: 0x00134644 File Offset: 0x00132844
			internal List<ValueTuple<ItemRosterElement, int>> GetBoughtItems()
			{
				return this.GetTransferredItems(true);
			}

			// Token: 0x060041CA RID: 16842 RVA: 0x0013464D File Offset: 0x0013284D
			internal List<ValueTuple<ItemRosterElement, int>> GetSoldItems()
			{
				return this.GetTransferredItems(false);
			}

			// Token: 0x0400151C RID: 5404
			private Dictionary<ItemRosterElement, InventoryLogic.ItemLog> _transactionLogs = new Dictionary<ItemRosterElement, InventoryLogic.ItemLog>();
		}
	}
}
