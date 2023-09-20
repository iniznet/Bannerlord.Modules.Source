using System;
using System.Collections.Generic;
using Helpers;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.Inventory
{
	// Token: 0x020000D7 RID: 215
	public class InventoryManager
	{
		// Token: 0x1700057B RID: 1403
		// (get) Token: 0x0600133C RID: 4924 RVA: 0x0005652E File Offset: 0x0005472E
		public InventoryMode CurrentMode
		{
			get
			{
				return this._currentMode;
			}
		}

		// Token: 0x1700057C RID: 1404
		// (get) Token: 0x0600133D RID: 4925 RVA: 0x00056536 File Offset: 0x00054736
		public static InventoryManager Instance
		{
			get
			{
				return Campaign.Current.InventoryManager;
			}
		}

		// Token: 0x1700057D RID: 1405
		// (get) Token: 0x0600133E RID: 4926 RVA: 0x00056542 File Offset: 0x00054742
		public static InventoryLogic InventoryLogic
		{
			get
			{
				return InventoryManager.Instance._inventoryLogic;
			}
		}

		// Token: 0x0600133F RID: 4927 RVA: 0x0005654E File Offset: 0x0005474E
		public void PlayerAcceptTradeOffer()
		{
			InventoryLogic inventoryLogic = this._inventoryLogic;
			if (inventoryLogic == null)
			{
				return;
			}
			inventoryLogic.SetPlayerAcceptTraderOffer();
		}

		// Token: 0x06001340 RID: 4928 RVA: 0x00056560 File Offset: 0x00054760
		public void CloseInventoryPresentation(bool fromCancel)
		{
			if (this._inventoryLogic.DoneLogic())
			{
				Game.Current.GameStateManager.PopState(0);
				InventoryManager.DoneLogicExtrasDelegate doneLogicExtrasDelegate = this._doneLogicExtrasDelegate;
				if (doneLogicExtrasDelegate != null)
				{
					doneLogicExtrasDelegate();
				}
				this._doneLogicExtrasDelegate = null;
				this._inventoryLogic = null;
			}
		}

		// Token: 0x06001341 RID: 4929 RVA: 0x000565A0 File Offset: 0x000547A0
		private void OpenInventoryPresentation(TextObject leftRosterName)
		{
			ItemRoster itemRoster = new ItemRoster();
			if (Game.Current.CheatMode)
			{
				TestCommonBase baseInstance = TestCommonBase.BaseInstance;
				if (baseInstance == null || !baseInstance.IsTestEnabled)
				{
					MBReadOnlyList<ItemObject> objectTypeList = Game.Current.ObjectManager.GetObjectTypeList<ItemObject>();
					for (int num = 0; num != objectTypeList.Count; num++)
					{
						ItemObject itemObject = objectTypeList[num];
						itemRoster.AddToCounts(itemObject, 10);
					}
				}
			}
			this._inventoryLogic = new InventoryLogic(null);
			this._inventoryLogic.Initialize(itemRoster, MobileParty.MainParty, false, true, CharacterObject.PlayerCharacter, InventoryManager.InventoryCategoryType.None, InventoryManager.GetCurrentMarketData(), false, leftRosterName, null);
			InventoryState inventoryState = Game.Current.GameStateManager.CreateState<InventoryState>();
			inventoryState.InitializeLogic(this._inventoryLogic);
			Game.Current.GameStateManager.PushState(inventoryState, 0);
		}

		// Token: 0x06001342 RID: 4930 RVA: 0x00056664 File Offset: 0x00054864
		private static IMarketData GetCurrentMarketData()
		{
			IMarketData marketData = null;
			if (Campaign.Current.GameMode == CampaignGameMode.Campaign)
			{
				Settlement settlement = MobileParty.MainParty.CurrentSettlement;
				if (settlement == null)
				{
					settlement = SettlementHelper.FindNearestTown(null, null);
				}
				if (settlement != null)
				{
					if (settlement.IsVillage)
					{
						marketData = settlement.Village.MarketData;
					}
					else if (settlement.IsTown)
					{
						marketData = settlement.Town.MarketData;
					}
				}
			}
			if (marketData == null)
			{
				marketData = new FakeMarketData();
			}
			return marketData;
		}

		// Token: 0x06001343 RID: 4931 RVA: 0x000566D0 File Offset: 0x000548D0
		public static void OpenScreenAsInventoryOfSubParty(MobileParty rightParty, MobileParty leftParty, InventoryManager.DoneLogicExtrasDelegate doneLogicExtrasDelegate)
		{
			Hero leaderHero = rightParty.LeaderHero;
			InventoryLogic inventoryLogic = new InventoryLogic(rightParty, (leaderHero != null) ? leaderHero.CharacterObject : null, leftParty.Party);
			InventoryLogic inventoryLogic2 = inventoryLogic;
			ItemRoster itemRoster = leftParty.ItemRoster;
			ItemRoster itemRoster2 = rightParty.ItemRoster;
			TroopRoster memberRoster = rightParty.MemberRoster;
			bool flag = false;
			bool flag2 = false;
			Hero leaderHero2 = rightParty.LeaderHero;
			inventoryLogic2.Initialize(itemRoster, itemRoster2, memberRoster, flag, flag2, (leaderHero2 != null) ? leaderHero2.CharacterObject : null, InventoryManager.InventoryCategoryType.None, InventoryManager.GetCurrentMarketData(), false, null, null);
			InventoryManager.Instance._doneLogicExtrasDelegate = doneLogicExtrasDelegate;
			InventoryState inventoryState = Game.Current.GameStateManager.CreateState<InventoryState>();
			inventoryState.InitializeLogic(inventoryLogic);
			Game.Current.GameStateManager.PushState(inventoryState, 0);
		}

		// Token: 0x06001344 RID: 4932 RVA: 0x00056764 File Offset: 0x00054964
		public static void OpenScreenAsInventoryForCraftedItemDecomposition(MobileParty party, CharacterObject character, InventoryManager.DoneLogicExtrasDelegate doneLogicExtrasDelegate)
		{
			InventoryManager.Instance._inventoryLogic = new InventoryLogic(null);
			InventoryManager.Instance._inventoryLogic.Initialize(new ItemRoster(), party.ItemRoster, party.MemberRoster, false, false, character, InventoryManager.InventoryCategoryType.None, InventoryManager.GetCurrentMarketData(), false, null, null);
			InventoryManager.Instance._doneLogicExtrasDelegate = doneLogicExtrasDelegate;
			InventoryState inventoryState = Game.Current.GameStateManager.CreateState<InventoryState>();
			inventoryState.InitializeLogic(InventoryManager.Instance._inventoryLogic);
			Game.Current.GameStateManager.PushState(inventoryState, 0);
		}

		// Token: 0x06001345 RID: 4933 RVA: 0x000567EC File Offset: 0x000549EC
		public static void OpenScreenAsInventoryOf(MobileParty party, CharacterObject character)
		{
			InventoryManager.Instance._inventoryLogic = new InventoryLogic(null);
			InventoryManager.Instance._inventoryLogic.Initialize(new ItemRoster(), party.ItemRoster, party.MemberRoster, false, true, character, InventoryManager.InventoryCategoryType.None, InventoryManager.GetCurrentMarketData(), false, null, null);
			InventoryState inventoryState = Game.Current.GameStateManager.CreateState<InventoryState>();
			inventoryState.InitializeLogic(InventoryManager.Instance._inventoryLogic);
			Game.Current.GameStateManager.PushState(inventoryState, 0);
		}

		// Token: 0x06001346 RID: 4934 RVA: 0x00056868 File Offset: 0x00054A68
		public static void OpenScreenAsInventoryOf(PartyBase rightParty, PartyBase leftParty)
		{
			InventoryManager.Instance._inventoryLogic = new InventoryLogic(leftParty);
			InventoryLogic inventoryLogic = InventoryManager.Instance._inventoryLogic;
			ItemRoster itemRoster = leftParty.ItemRoster;
			ItemRoster itemRoster2 = rightParty.ItemRoster;
			TroopRoster memberRoster = rightParty.MemberRoster;
			bool flag = false;
			bool flag2 = false;
			Hero leaderHero = rightParty.LeaderHero;
			inventoryLogic.Initialize(itemRoster, itemRoster2, memberRoster, flag, flag2, (leaderHero != null) ? leaderHero.CharacterObject : null, InventoryManager.InventoryCategoryType.None, InventoryManager.GetCurrentMarketData(), false, null, leftParty.MemberRoster);
			InventoryState inventoryState = Game.Current.GameStateManager.CreateState<InventoryState>();
			inventoryState.InitializeLogic(InventoryManager.Instance._inventoryLogic);
			Game.Current.GameStateManager.PushState(inventoryState, 0);
		}

		// Token: 0x06001347 RID: 4935 RVA: 0x000568F9 File Offset: 0x00054AF9
		public static void OpenScreenAsInventory(InventoryManager.DoneLogicExtrasDelegate doneLogicExtrasDelegate = null)
		{
			InventoryManager.Instance._currentMode = InventoryMode.Default;
			InventoryManager.Instance.OpenInventoryPresentation(new TextObject("{=02c5bQSM}Discard", null));
			InventoryManager.Instance._doneLogicExtrasDelegate = doneLogicExtrasDelegate;
		}

		// Token: 0x06001348 RID: 4936 RVA: 0x00056926 File Offset: 0x00054B26
		public static void OpenCampaignBattleLootScreen()
		{
			InventoryManager.OpenScreenAsLoot(new Dictionary<PartyBase, ItemRoster> { 
			{
				PartyBase.MainParty,
				MapEvent.PlayerMapEvent.ItemRosterForPlayerLootShare(PartyBase.MainParty)
			} });
		}

		// Token: 0x06001349 RID: 4937 RVA: 0x0005694C File Offset: 0x00054B4C
		public static void OpenScreenAsLoot(Dictionary<PartyBase, ItemRoster> itemRostersToLoot)
		{
			ItemRoster itemRoster = itemRostersToLoot[PartyBase.MainParty];
			InventoryManager.Instance._currentMode = InventoryMode.Loot;
			InventoryManager.Instance._inventoryLogic = new InventoryLogic(null);
			InventoryManager.Instance._inventoryLogic.Initialize(itemRoster, MobileParty.MainParty.ItemRoster, MobileParty.MainParty.MemberRoster, false, true, CharacterObject.PlayerCharacter, InventoryManager.InventoryCategoryType.None, InventoryManager.GetCurrentMarketData(), false, null, null);
			InventoryState inventoryState = Game.Current.GameStateManager.CreateState<InventoryState>();
			inventoryState.InitializeLogic(InventoryManager.Instance._inventoryLogic);
			Game.Current.GameStateManager.PushState(inventoryState, 0);
		}

		// Token: 0x0600134A RID: 4938 RVA: 0x000569E8 File Offset: 0x00054BE8
		public static void OpenScreenAsStash(ItemRoster stash)
		{
			InventoryManager.Instance._currentMode = InventoryMode.Stash;
			InventoryManager.Instance._inventoryLogic = new InventoryLogic(null);
			InventoryManager.Instance._inventoryLogic.Initialize(stash, MobileParty.MainParty, false, false, CharacterObject.PlayerCharacter, InventoryManager.InventoryCategoryType.None, InventoryManager.GetCurrentMarketData(), false, new TextObject("{=nZbaYvVx}Stash", null), null);
			InventoryState inventoryState = Game.Current.GameStateManager.CreateState<InventoryState>();
			inventoryState.InitializeLogic(InventoryManager.Instance._inventoryLogic);
			Game.Current.GameStateManager.PushState(inventoryState, 0);
		}

		// Token: 0x0600134B RID: 4939 RVA: 0x00056A70 File Offset: 0x00054C70
		public static void OpenScreenAsReceiveItems(ItemRoster items, TextObject leftRosterName, InventoryManager.DoneLogicExtrasDelegate doneLogicDelegate = null)
		{
			InventoryManager.Instance._currentMode = InventoryMode.Default;
			InventoryManager.Instance._inventoryLogic = new InventoryLogic(null);
			InventoryManager.Instance._inventoryLogic.Initialize(items, MobileParty.MainParty.ItemRoster, MobileParty.MainParty.MemberRoster, false, true, CharacterObject.PlayerCharacter, InventoryManager.InventoryCategoryType.None, InventoryManager.GetCurrentMarketData(), false, leftRosterName, null);
			InventoryManager.Instance._doneLogicExtrasDelegate = doneLogicDelegate;
			InventoryState inventoryState = Game.Current.GameStateManager.CreateState<InventoryState>();
			inventoryState.InitializeLogic(InventoryManager.Instance._inventoryLogic);
			Game.Current.GameStateManager.PushState(inventoryState, 0);
		}

		// Token: 0x0600134C RID: 4940 RVA: 0x00056B08 File Offset: 0x00054D08
		public static void OpenTradeWithCaravanOrAlleyParty(MobileParty caravan, InventoryManager.InventoryCategoryType merchantItemType = InventoryManager.InventoryCategoryType.None)
		{
			InventoryManager.Instance._currentMode = InventoryMode.Trade;
			InventoryManager.Instance._inventoryLogic = new InventoryLogic(caravan.Party);
			InventoryManager.Instance._inventoryLogic.Initialize(caravan.Party.ItemRoster, PartyBase.MainParty.ItemRoster, PartyBase.MainParty.MemberRoster, true, true, CharacterObject.PlayerCharacter, merchantItemType, InventoryManager.GetCurrentMarketData(), false, null, null);
			InventoryManager.Instance._inventoryLogic.SetInventoryListener(new InventoryManager.CaravanInventoryListener(caravan));
			InventoryState inventoryState = Game.Current.GameStateManager.CreateState<InventoryState>();
			inventoryState.InitializeLogic(InventoryManager.Instance._inventoryLogic);
			Game.Current.GameStateManager.PushState(inventoryState, 0);
		}

		// Token: 0x0600134D RID: 4941 RVA: 0x00056BB9 File Offset: 0x00054DB9
		public static void ActivateTradeWithCurrentSettlement()
		{
			InventoryManager.OpenScreenAsTrade(Settlement.CurrentSettlement.ItemRoster, Settlement.CurrentSettlement.SettlementComponent, InventoryManager.InventoryCategoryType.None, null);
		}

		// Token: 0x0600134E RID: 4942 RVA: 0x00056BD8 File Offset: 0x00054DD8
		public static void OpenScreenAsTrade(ItemRoster leftRoster, SettlementComponent settlementComponent, InventoryManager.InventoryCategoryType merchantItemType = InventoryManager.InventoryCategoryType.None, InventoryManager.DoneLogicExtrasDelegate doneLogicExtrasDelegate = null)
		{
			InventoryManager.Instance._currentMode = InventoryMode.Trade;
			InventoryManager.Instance._inventoryLogic = new InventoryLogic(settlementComponent.Owner);
			InventoryManager.Instance._inventoryLogic.Initialize(leftRoster, PartyBase.MainParty.ItemRoster, PartyBase.MainParty.MemberRoster, true, true, CharacterObject.PlayerCharacter, merchantItemType, InventoryManager.GetCurrentMarketData(), false, null, null);
			InventoryManager.Instance._inventoryLogic.SetInventoryListener(new InventoryManager.MerchantInventoryListener(settlementComponent));
			InventoryManager.Instance._doneLogicExtrasDelegate = doneLogicExtrasDelegate;
			InventoryState inventoryState = Game.Current.GameStateManager.CreateState<InventoryState>();
			inventoryState.InitializeLogic(InventoryManager.Instance._inventoryLogic);
			Game.Current.GameStateManager.PushState(inventoryState, 0);
			inventoryState.Handler.FilterInventoryAtOpening(merchantItemType);
		}

		// Token: 0x0600134F RID: 4943 RVA: 0x00056C98 File Offset: 0x00054E98
		public static InventoryItemType GetInventoryItemTypeOfItem(ItemObject item)
		{
			if (item != null)
			{
				switch (item.ItemType)
				{
				case ItemObject.ItemTypeEnum.Horse:
					return InventoryItemType.Horse;
				case ItemObject.ItemTypeEnum.OneHandedWeapon:
					return InventoryItemType.Weapon;
				case ItemObject.ItemTypeEnum.TwoHandedWeapon:
					return InventoryItemType.Weapon;
				case ItemObject.ItemTypeEnum.Polearm:
					return InventoryItemType.Weapon;
				case ItemObject.ItemTypeEnum.Arrows:
					return InventoryItemType.Weapon;
				case ItemObject.ItemTypeEnum.Bolts:
					return InventoryItemType.Weapon;
				case ItemObject.ItemTypeEnum.Shield:
					return InventoryItemType.Shield;
				case ItemObject.ItemTypeEnum.Bow:
					return InventoryItemType.Weapon;
				case ItemObject.ItemTypeEnum.Crossbow:
					return InventoryItemType.Weapon;
				case ItemObject.ItemTypeEnum.Thrown:
					return InventoryItemType.Weapon;
				case ItemObject.ItemTypeEnum.Goods:
					return InventoryItemType.Goods;
				case ItemObject.ItemTypeEnum.HeadArmor:
					return InventoryItemType.HeadArmor;
				case ItemObject.ItemTypeEnum.BodyArmor:
					return InventoryItemType.BodyArmor;
				case ItemObject.ItemTypeEnum.LegArmor:
					return InventoryItemType.LegArmor;
				case ItemObject.ItemTypeEnum.HandArmor:
					return InventoryItemType.HandArmor;
				case ItemObject.ItemTypeEnum.Pistol:
					return InventoryItemType.Weapon;
				case ItemObject.ItemTypeEnum.Musket:
					return InventoryItemType.Weapon;
				case ItemObject.ItemTypeEnum.Bullets:
					return InventoryItemType.Weapon;
				case ItemObject.ItemTypeEnum.Animal:
					return InventoryItemType.Animal;
				case ItemObject.ItemTypeEnum.Book:
					return InventoryItemType.Book;
				case ItemObject.ItemTypeEnum.Cape:
					return InventoryItemType.Cape;
				case ItemObject.ItemTypeEnum.HorseHarness:
					return InventoryItemType.HorseHarness;
				case ItemObject.ItemTypeEnum.Banner:
					return InventoryItemType.Banner;
				}
			}
			return InventoryItemType.None;
		}

		// Token: 0x040006B9 RID: 1721
		private InventoryMode _currentMode;

		// Token: 0x040006BA RID: 1722
		private InventoryLogic _inventoryLogic;

		// Token: 0x040006BB RID: 1723
		private InventoryManager.DoneLogicExtrasDelegate _doneLogicExtrasDelegate;

		// Token: 0x020004EB RID: 1259
		public enum InventoryCategoryType
		{
			// Token: 0x04001524 RID: 5412
			None = -1,
			// Token: 0x04001525 RID: 5413
			All,
			// Token: 0x04001526 RID: 5414
			Armors,
			// Token: 0x04001527 RID: 5415
			Weapon,
			// Token: 0x04001528 RID: 5416
			Shield,
			// Token: 0x04001529 RID: 5417
			HorseCategory,
			// Token: 0x0400152A RID: 5418
			Goods,
			// Token: 0x0400152B RID: 5419
			CategoryTypeAmount
		}

		// Token: 0x020004EC RID: 1260
		// (Invoke) Token: 0x060041D4 RID: 16852
		public delegate void DoneLogicExtrasDelegate();

		// Token: 0x020004ED RID: 1261
		private class CaravanInventoryListener : InventoryListener
		{
			// Token: 0x060041D7 RID: 16855 RVA: 0x001346F2 File Offset: 0x001328F2
			public CaravanInventoryListener(MobileParty caravan)
			{
				this._caravan = caravan;
			}

			// Token: 0x060041D8 RID: 16856 RVA: 0x00134701 File Offset: 0x00132901
			public override int GetGold()
			{
				return this._caravan.PartyTradeGold;
			}

			// Token: 0x060041D9 RID: 16857 RVA: 0x0013470E File Offset: 0x0013290E
			public override TextObject GetTraderName()
			{
				if (this._caravan.LeaderHero == null)
				{
					return this._caravan.Name;
				}
				return this._caravan.LeaderHero.Name;
			}

			// Token: 0x060041DA RID: 16858 RVA: 0x00134739 File Offset: 0x00132939
			public override void SetGold(int gold)
			{
				this._caravan.PartyTradeGold = gold;
			}

			// Token: 0x060041DB RID: 16859 RVA: 0x00134747 File Offset: 0x00132947
			public override PartyBase GetOppositeParty()
			{
				return this._caravan.Party;
			}

			// Token: 0x060041DC RID: 16860 RVA: 0x00134754 File Offset: 0x00132954
			public override void OnTransaction()
			{
				throw new NotImplementedException();
			}

			// Token: 0x0400152C RID: 5420
			private MobileParty _caravan;
		}

		// Token: 0x020004EE RID: 1262
		private class MerchantInventoryListener : InventoryListener
		{
			// Token: 0x060041DD RID: 16861 RVA: 0x0013475B File Offset: 0x0013295B
			public MerchantInventoryListener(SettlementComponent settlementComponent)
			{
				this._settlementComponent = settlementComponent;
			}

			// Token: 0x060041DE RID: 16862 RVA: 0x0013476A File Offset: 0x0013296A
			public override TextObject GetTraderName()
			{
				return this._settlementComponent.Owner.Name;
			}

			// Token: 0x060041DF RID: 16863 RVA: 0x0013477C File Offset: 0x0013297C
			public override PartyBase GetOppositeParty()
			{
				return this._settlementComponent.Owner;
			}

			// Token: 0x060041E0 RID: 16864 RVA: 0x00134789 File Offset: 0x00132989
			public override int GetGold()
			{
				return this._settlementComponent.Gold;
			}

			// Token: 0x060041E1 RID: 16865 RVA: 0x00134796 File Offset: 0x00132996
			public override void SetGold(int gold)
			{
				this._settlementComponent.ChangeGold(gold - this._settlementComponent.Gold);
			}

			// Token: 0x060041E2 RID: 16866 RVA: 0x001347B0 File Offset: 0x001329B0
			public override void OnTransaction()
			{
				throw new NotImplementedException();
			}

			// Token: 0x0400152D RID: 5421
			private SettlementComponent _settlementComponent;
		}
	}
}
