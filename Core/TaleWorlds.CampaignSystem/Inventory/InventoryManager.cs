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
	public class InventoryManager
	{
		public InventoryMode CurrentMode
		{
			get
			{
				return this._currentMode;
			}
		}

		public static InventoryManager Instance
		{
			get
			{
				return Campaign.Current.InventoryManager;
			}
		}

		public static InventoryLogic InventoryLogic
		{
			get
			{
				return InventoryManager.Instance._inventoryLogic;
			}
		}

		public void PlayerAcceptTradeOffer()
		{
			InventoryLogic inventoryLogic = this._inventoryLogic;
			if (inventoryLogic == null)
			{
				return;
			}
			inventoryLogic.SetPlayerAcceptTraderOffer();
		}

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

		public static void OpenScreenAsInventoryForCraftedItemDecomposition(MobileParty party, CharacterObject character, InventoryManager.DoneLogicExtrasDelegate doneLogicExtrasDelegate)
		{
			InventoryManager.Instance._inventoryLogic = new InventoryLogic(null);
			InventoryManager.Instance._inventoryLogic.Initialize(new ItemRoster(), party.ItemRoster, party.MemberRoster, false, false, character, InventoryManager.InventoryCategoryType.None, InventoryManager.GetCurrentMarketData(), false, null, null);
			InventoryManager.Instance._doneLogicExtrasDelegate = doneLogicExtrasDelegate;
			InventoryState inventoryState = Game.Current.GameStateManager.CreateState<InventoryState>();
			inventoryState.InitializeLogic(InventoryManager.Instance._inventoryLogic);
			Game.Current.GameStateManager.PushState(inventoryState, 0);
		}

		public static void OpenScreenAsInventoryOf(MobileParty party, CharacterObject character)
		{
			InventoryManager.Instance._inventoryLogic = new InventoryLogic(null);
			InventoryManager.Instance._inventoryLogic.Initialize(new ItemRoster(), party.ItemRoster, party.MemberRoster, false, true, character, InventoryManager.InventoryCategoryType.None, InventoryManager.GetCurrentMarketData(), false, null, null);
			InventoryState inventoryState = Game.Current.GameStateManager.CreateState<InventoryState>();
			inventoryState.InitializeLogic(InventoryManager.Instance._inventoryLogic);
			Game.Current.GameStateManager.PushState(inventoryState, 0);
		}

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

		public static void OpenScreenAsInventory(InventoryManager.DoneLogicExtrasDelegate doneLogicExtrasDelegate = null)
		{
			InventoryManager.Instance._currentMode = InventoryMode.Default;
			InventoryManager.Instance.OpenInventoryPresentation(new TextObject("{=02c5bQSM}Discard", null));
			InventoryManager.Instance._doneLogicExtrasDelegate = doneLogicExtrasDelegate;
		}

		public static void OpenCampaignBattleLootScreen()
		{
			InventoryManager.OpenScreenAsLoot(new Dictionary<PartyBase, ItemRoster> { 
			{
				PartyBase.MainParty,
				MapEvent.PlayerMapEvent.ItemRosterForPlayerLootShare(PartyBase.MainParty)
			} });
		}

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

		public static void OpenScreenAsStash(ItemRoster stash)
		{
			InventoryManager.Instance._currentMode = InventoryMode.Stash;
			InventoryManager.Instance._inventoryLogic = new InventoryLogic(null);
			InventoryManager.Instance._inventoryLogic.Initialize(stash, MobileParty.MainParty, false, false, CharacterObject.PlayerCharacter, InventoryManager.InventoryCategoryType.None, InventoryManager.GetCurrentMarketData(), false, new TextObject("{=nZbaYvVx}Stash", null), null);
			InventoryState inventoryState = Game.Current.GameStateManager.CreateState<InventoryState>();
			inventoryState.InitializeLogic(InventoryManager.Instance._inventoryLogic);
			Game.Current.GameStateManager.PushState(inventoryState, 0);
		}

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

		public static void ActivateTradeWithCurrentSettlement()
		{
			InventoryManager.OpenScreenAsTrade(Settlement.CurrentSettlement.ItemRoster, Settlement.CurrentSettlement.SettlementComponent, InventoryManager.InventoryCategoryType.None, null);
		}

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

		private InventoryMode _currentMode;

		private InventoryLogic _inventoryLogic;

		private InventoryManager.DoneLogicExtrasDelegate _doneLogicExtrasDelegate;

		public enum InventoryCategoryType
		{
			None = -1,
			All,
			Armors,
			Weapon,
			Shield,
			HorseCategory,
			Goods,
			CategoryTypeAmount
		}

		public delegate void DoneLogicExtrasDelegate();

		private class CaravanInventoryListener : InventoryListener
		{
			public CaravanInventoryListener(MobileParty caravan)
			{
				this._caravan = caravan;
			}

			public override int GetGold()
			{
				return this._caravan.PartyTradeGold;
			}

			public override TextObject GetTraderName()
			{
				if (this._caravan.LeaderHero == null)
				{
					return this._caravan.Name;
				}
				return this._caravan.LeaderHero.Name;
			}

			public override void SetGold(int gold)
			{
				this._caravan.PartyTradeGold = gold;
			}

			public override PartyBase GetOppositeParty()
			{
				return this._caravan.Party;
			}

			public override void OnTransaction()
			{
				throw new NotImplementedException();
			}

			private MobileParty _caravan;
		}

		private class MerchantInventoryListener : InventoryListener
		{
			public MerchantInventoryListener(SettlementComponent settlementComponent)
			{
				this._settlementComponent = settlementComponent;
			}

			public override TextObject GetTraderName()
			{
				return this._settlementComponent.Owner.Name;
			}

			public override PartyBase GetOppositeParty()
			{
				return this._settlementComponent.Owner;
			}

			public override int GetGold()
			{
				return this._settlementComponent.Gold;
			}

			public override void SetGold(int gold)
			{
				this._settlementComponent.ChangeGold(gold - this._settlementComponent.Gold);
			}

			public override void OnTransaction()
			{
				throw new NotImplementedException();
			}

			private SettlementComponent _settlementComponent;
		}
	}
}
