using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.CraftingSystem;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Inventory;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	public class CraftingCampaignBehavior : CampaignBehaviorBase, ICraftingCampaignBehavior, ICampaignBehavior
	{
		public IReadOnlyDictionary<Town, CraftingCampaignBehavior.CraftingOrderSlots> CraftingOrders
		{
			get
			{
				return this._craftingOrders;
			}
		}

		public IReadOnlyCollection<WeaponDesign> CraftingHistory
		{
			get
			{
				return this._craftingHistory;
			}
		}

		public static bool WeaponTypeDebugEnabled { get; private set; }

		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<ItemObject>("_latestCraftedItem", ref this._latestCraftedItem);
			dataStore.SyncData<Dictionary<ItemObject, CraftingCampaignBehavior.CraftedItemInitializationData>>("_craftedItemDictionary", ref this._craftedItemDictionary);
			dataStore.SyncData<Dictionary<Hero, CraftingCampaignBehavior.HeroCraftingRecord>>("_heroCraftingRecordsNew", ref this._heroCraftingRecords);
			dataStore.SyncData<Dictionary<Town, CraftingCampaignBehavior.CraftingOrderSlots>>("_craftingOrders", ref this._craftingOrders);
			dataStore.SyncData<List<WeaponDesign>>("_craftingHistory", ref this._craftingHistory);
			dataStore.SyncData<Dictionary<CraftingTemplate, List<CraftingPiece>>>("_openedPartsDictionary", ref this._openedPartsDictionary);
			dataStore.SyncData<Dictionary<CraftingTemplate, float>>("_openNewPartXpDictionary", ref this._openNewPartXpDictionary);
			if (dataStore.IsLoading && MBSaveLoad.IsUpdatingGameVersion && MBSaveLoad.LastLoadedGameVersion < ApplicationVersion.FromString("e1.8.0", 26219))
			{
				List<CraftingPiece> list = new List<CraftingPiece>();
				dataStore.SyncData<List<CraftingPiece>>("_openedParts", ref list);
				if (list != null)
				{
					this._openedPartsDictionary = new Dictionary<CraftingTemplate, List<CraftingPiece>>();
					foreach (CraftingTemplate craftingTemplate in CraftingTemplate.All)
					{
						this._openedPartsDictionary.Add(craftingTemplate, new List<CraftingPiece>());
						foreach (CraftingPiece craftingPiece in list)
						{
							if (craftingTemplate.Pieces.Contains(craftingPiece) && !this._openedPartsDictionary[craftingTemplate].Contains(craftingPiece))
							{
								this._openedPartsDictionary[craftingTemplate].Add(craftingPiece);
							}
						}
					}
				}
			}
		}

		public override void RegisterEvents()
		{
			CampaignEvents.OnNewGameCreatedPartialFollowUpEndEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnNewGameCreatedPartialFollowUpEnd));
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
			CampaignEvents.OnNewItemCraftedEvent.AddNonSerializedListener(this, new Action<ItemObject, Crafting.OverrideData, bool>(this.OnNewItemCrafted));
			CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(this.HourlyTick));
			CampaignEvents.DailyTickSettlementEvent.AddNonSerializedListener(this, new Action<Settlement>(this.DailyTickSettlement));
			CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new Action(this.DailyTick));
			CampaignEvents.HeroKilledEvent.AddNonSerializedListener(this, new Action<Hero, Hero, KillCharacterAction.KillCharacterActionDetail, bool>(this.OnHeroKilled));
			CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnGameLoaded));
		}

		private void OnNewGameCreatedPartialFollowUpEnd(CampaignGameStarter starter)
		{
			this.InitializeLists();
			MBList<Hero> mblist = new MBList<Hero>();
			foreach (Town town in Town.AllTowns)
			{
				Settlement settlement = town.Settlement;
				mblist.AddRange(settlement.HeroesWithoutParty);
				foreach (MobileParty mobileParty in settlement.Parties)
				{
					if (mobileParty.LeaderHero != null && !mobileParty.IsMainParty)
					{
						mblist.Add(mobileParty.LeaderHero);
					}
				}
				if (mblist.Count > 0)
				{
					for (int i = 0; i < 6; i++)
					{
						if (this.CraftingOrders[settlement.Town].GetAvailableSlot() > -1)
						{
							this.CreateTownOrder(mblist.GetRandomElement<Hero>(), i);
						}
					}
				}
				mblist.Clear();
			}
		}

		private void DailyTickSettlement(Settlement settlement)
		{
			if (settlement.IsTown && this.CraftingOrders[settlement.Town].IsThereAvailableSlot())
			{
				List<Hero> list = new List<Hero>();
				list.AddRange(settlement.HeroesWithoutParty);
				foreach (MobileParty mobileParty in settlement.Parties)
				{
					if (mobileParty.LeaderHero != null && !mobileParty.IsMainParty)
					{
						list.Add(mobileParty.LeaderHero);
					}
				}
				foreach (Hero hero in list)
				{
					if (hero != Hero.MainHero && MBRandom.RandomFloat <= 0.05f)
					{
						int availableSlot = this.CraftingOrders[settlement.Town].GetAvailableSlot();
						if (availableSlot > -1)
						{
							this.CreateTownOrder(hero, availableSlot);
						}
					}
				}
				list = null;
			}
		}

		private void DailyTick()
		{
			foreach (KeyValuePair<Town, CraftingCampaignBehavior.CraftingOrderSlots> keyValuePair in this.CraftingOrders)
			{
				foreach (CraftingOrder craftingOrder in keyValuePair.Value.Slots)
				{
					if (craftingOrder != null && MBRandom.RandomFloat <= 0.05f)
					{
						this.ReplaceCraftingOrder(keyValuePair.Key, craftingOrder);
					}
				}
			}
		}

		private void HourlyTick()
		{
			foreach (KeyValuePair<Hero, CraftingCampaignBehavior.HeroCraftingRecord> keyValuePair in this._heroCraftingRecords)
			{
				int maxHeroCraftingStamina = this.GetMaxHeroCraftingStamina(keyValuePair.Key);
				if (keyValuePair.Value.CraftingStamina < maxHeroCraftingStamina)
				{
					Hero key = keyValuePair.Key;
					MobileParty partyBelongedTo = key.PartyBelongedTo;
					if (((partyBelongedTo != null) ? partyBelongedTo.CurrentSettlement : null) != null)
					{
						keyValuePair.Value.CraftingStamina = MathF.Min(maxHeroCraftingStamina, keyValuePair.Value.CraftingStamina + this.GetStaminaHourlyRecoveryRate(key));
					}
				}
			}
		}

		private void OnHeroKilled(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail detail, bool showNotification = true)
		{
			this.RemoveOrdersOfHeroWithoutCompletionIfExists(victim);
		}

		private void OnGameLoaded(CampaignGameStarter campaignGameStarter)
		{
			this.InitializeLists();
			foreach (KeyValuePair<Town, CraftingCampaignBehavior.CraftingOrderSlots> keyValuePair in this._craftingOrders)
			{
				for (int i = 0; i < 6; i++)
				{
					CraftingOrder craftingOrder = keyValuePair.Value.Slots[i];
					if (craftingOrder != null && (craftingOrder.PreCraftedWeaponDesignItem == DefaultItems.Trash || craftingOrder.PreCraftedWeaponDesignItem == null || !craftingOrder.PreCraftedWeaponDesignItem.IsReady))
					{
						this.CancelOrder(keyValuePair.Key, craftingOrder);
					}
				}
			}
		}

		private int GetStaminaHourlyRecoveryRate(Hero hero)
		{
			int num = 5 + MathF.Round((float)hero.GetSkillValue(DefaultSkills.Crafting) * 0.025f);
			if (hero.GetPerkValue(DefaultPerks.Athletics.Stamina))
			{
				num += MathF.Round((float)num * DefaultPerks.Athletics.Stamina.PrimaryBonus);
			}
			return num;
		}

		private void OnNewItemCrafted(ItemObject itemObject, Crafting.OverrideData overrideData, bool isCraftingOrderItem)
		{
			if (!this._craftedItemDictionary.ContainsKey(itemObject))
			{
				CultureObject @object = MBObjectManager.Instance.GetObject<CultureObject>(itemObject.Culture.StringId);
				CraftingCampaignBehavior.CraftedItemInitializationData craftedItemInitializationData = new CraftingCampaignBehavior.CraftedItemInitializationData(itemObject.WeaponDesign, itemObject.Name, @object);
				this._craftedItemDictionary.Add(itemObject, craftedItemInitializationData);
			}
		}

		private void AddResearchPoints(CraftingTemplate craftingTemplate, int researchPoints)
		{
			Dictionary<CraftingTemplate, float> dictionary = this._openNewPartXpDictionary;
			CraftingTemplate craftingTemplate2 = craftingTemplate;
			dictionary[craftingTemplate2] += (float)researchPoints;
			int count = craftingTemplate.Pieces.Count;
			int num = craftingTemplate.Pieces.Count((CraftingPiece x) => this.IsOpened(x, craftingTemplate));
			float num2 = Campaign.Current.Models.SmithingModel.ResearchPointsNeedForNewPart(count, num);
			do
			{
				if (this._openNewPartXpDictionary[craftingTemplate] > num2)
				{
					dictionary = this._openNewPartXpDictionary;
					craftingTemplate2 = craftingTemplate;
					dictionary[craftingTemplate2] -= num2;
					if (this.OpenNewPart(craftingTemplate))
					{
						num++;
					}
				}
				num2 = Campaign.Current.Models.SmithingModel.ResearchPointsNeedForNewPart(count, craftingTemplate.Pieces.Count((CraftingPiece x) => this.IsOpened(x, craftingTemplate)));
			}
			while (this._openNewPartXpDictionary[craftingTemplate] > num2 && num < count);
		}

		private bool OpenNewPart(CraftingTemplate craftingTemplate)
		{
			int num = int.MaxValue;
			MBList<CraftingPiece> mblist = new MBList<CraftingPiece>();
			foreach (CraftingPiece craftingPiece in craftingTemplate.Pieces)
			{
				int pieceTier = craftingPiece.PieceTier;
				if (num >= pieceTier && !craftingPiece.IsHiddenOnDesigner && !this.IsOpened(craftingPiece, craftingTemplate))
				{
					if (num > craftingPiece.PieceTier)
					{
						mblist.Clear();
						num = pieceTier;
					}
					mblist.Add(craftingPiece);
				}
			}
			if (mblist.Count > 0)
			{
				CraftingPiece randomElement = mblist.GetRandomElement<CraftingPiece>();
				this.OpenPart(randomElement, craftingTemplate, true);
				return true;
			}
			return false;
		}

		private void OpenPart(CraftingPiece selectedPiece, CraftingTemplate craftingTemplate, bool showNotification = true)
		{
			this._openedPartsDictionary[craftingTemplate].Add(selectedPiece);
			CampaignEventDispatcher.Instance.CraftingPartUnlocked(selectedPiece);
			if (showNotification)
			{
				TextObject textObject = new TextObject("{=p9F90bc0}New Smithing Part Unlocked: {PART_NAME} for {WEAPON_TYPE}.", null);
				textObject.SetTextVariable("PART_NAME", selectedPiece.Name);
				textObject.SetTextVariable("WEAPON_TYPE", craftingTemplate.TemplateName);
				MBInformationManager.AddQuickInformation(textObject, 0, null, "");
			}
		}

		public bool IsOpened(CraftingPiece craftingPiece, CraftingTemplate craftingTemplate)
		{
			return craftingPiece.IsGivenByDefault || this._openedPartsDictionary[craftingTemplate].Contains(craftingPiece);
		}

		public void InitializeCraftingElements()
		{
			List<ItemObject> list = new List<ItemObject>();
			foreach (KeyValuePair<ItemObject, CraftingCampaignBehavior.CraftedItemInitializationData> keyValuePair in this._craftedItemDictionary)
			{
				WeaponDesign weaponDesign = keyValuePair.Value.CraftedData;
				if (MBSaveLoad.IsUpdatingGameVersion && MBSaveLoad.LastLoadedGameVersion < ApplicationVersion.FromString("v1.1.0", 26219))
				{
					WeaponDesignElement[] array = new WeaponDesignElement[keyValuePair.Value.CraftedData.UsedPieces.Length];
					for (int i = 0; i < keyValuePair.Value.CraftedData.UsedPieces.Length; i++)
					{
						array[i] = keyValuePair.Value.CraftedData.UsedPieces[i].GetCopy();
					}
					weaponDesign = new WeaponDesign(weaponDesign.Template, weaponDesign.WeaponName, array);
				}
				ItemObject itemObject = Crafting.InitializePreCraftedWeaponOnLoad(keyValuePair.Key, weaponDesign, keyValuePair.Value.ItemName, keyValuePair.Value.Culture, null);
				if (itemObject == DefaultItems.Trash || itemObject == null)
				{
					list.Add(keyValuePair.Key);
					if (MBObjectManager.Instance.GetObject(keyValuePair.Key.Id) != null)
					{
						MBObjectManager.Instance.UnregisterObject(keyValuePair.Key);
					}
				}
				else
				{
					ItemObject.InitAsPlayerCraftedItem(ref itemObject);
					itemObject.IsReady = true;
				}
			}
			foreach (ItemObject itemObject2 in list)
			{
				this._craftedItemDictionary.Remove(itemObject2);
			}
			foreach (KeyValuePair<Town, CraftingCampaignBehavior.CraftingOrderSlots> keyValuePair2 in this.CraftingOrders)
			{
				foreach (CraftingOrder craftingOrder in keyValuePair2.Value.Slots)
				{
					if (craftingOrder != null)
					{
						craftingOrder.InitializeCraftingOrderOnLoad();
					}
				}
			}
		}

		public int GetCraftingDifficulty(WeaponDesign weaponDesign)
		{
			return Campaign.Current.Models.SmithingModel.CalculateWeaponDesignDifficulty(weaponDesign);
		}

		private void InitializeLists()
		{
			if (this._craftingOrders.IsEmpty<KeyValuePair<Town, CraftingCampaignBehavior.CraftingOrderSlots>>())
			{
				foreach (Settlement settlement in Settlement.All)
				{
					if (settlement.IsTown)
					{
						this._craftingOrders.Add(settlement.Town, new CraftingCampaignBehavior.CraftingOrderSlots());
					}
				}
			}
			foreach (KeyValuePair<CraftingTemplate, List<CraftingPiece>> keyValuePair in this._openedPartsDictionary.ToList<KeyValuePair<CraftingTemplate, List<CraftingPiece>>>())
			{
				if (!CraftingTemplate.All.Contains(keyValuePair.Key))
				{
					this._openedPartsDictionary.Remove(keyValuePair.Key);
				}
			}
			foreach (KeyValuePair<CraftingTemplate, float> keyValuePair2 in this._openNewPartXpDictionary.ToList<KeyValuePair<CraftingTemplate, float>>())
			{
				if (!CraftingTemplate.All.Contains(keyValuePair2.Key))
				{
					this._openNewPartXpDictionary.Remove(keyValuePair2.Key);
				}
			}
			foreach (CraftingTemplate craftingTemplate in CraftingTemplate.All)
			{
				if (!this._openNewPartXpDictionary.ContainsKey(craftingTemplate))
				{
					this._openNewPartXpDictionary.Add(craftingTemplate, 0f);
				}
				if (!this._openedPartsDictionary.ContainsKey(craftingTemplate))
				{
					this._openedPartsDictionary.Add(craftingTemplate, new List<CraftingPiece>());
				}
				foreach (CraftingPiece craftingPiece in this._openedPartsDictionary[craftingTemplate].ToList<CraftingPiece>())
				{
					if (!craftingTemplate.Pieces.Contains(craftingPiece))
					{
						this._openedPartsDictionary[craftingTemplate].Remove(craftingPiece);
					}
				}
			}
		}

		private void AddDialogs(CampaignGameStarter campaignGameStarter)
		{
			campaignGameStarter.AddDialogLine("blacksmith_begin", "start", "blacksmith_player", "{=gYByVHQy}Good day, {?PLAYER.GENDER}madam{?}sir{\\?}. How may I help you?", new ConversationSentence.OnConditionDelegate(this.conversation_blacksmith_begin_on_condition), null, 100, null);
			campaignGameStarter.AddPlayerLine("blacksmith_craft_items", "blacksmith_player", "blacksmith_player_ok", "{=VXKGD0ta}I want to use your forge.", () => Campaign.Current.IsCraftingEnabled, new ConversationSentence.OnConsequenceDelegate(this.conversation_blacksmith_craft_items_on_consequence), 100, null, null);
			campaignGameStarter.AddPlayerLine("blacksmith_leave", "blacksmith_player", "close_window", "{=iW9iKbb8}Nothing.", null, null, 100, null, null);
			campaignGameStarter.AddDialogLine("blacksmith_player_ok", "blacksmith_player_ok", "blacksmith_player_after_craft", "{=FJ0uAo6p}{CRAFTING_END_TEXT}", new ConversationSentence.OnConditionDelegate(this.conversation_blacksmith_player_ok_on_condition), null, 100, null);
			campaignGameStarter.AddPlayerLine("blacksmith_player_after_craft_accept", "blacksmith_player_after_craft", "player_blacksmith_after_craft", "{=QUn2ugIX}Thank you. Here's your pay.", new ConversationSentence.OnConditionDelegate(this.conversation_blacksmith_player_after_craft_accept_on_condition), new ConversationSentence.OnConsequenceDelegate(this.conversation_blacksmith_player_after_craft_accept_on_consequence), 100, null, null);
			campaignGameStarter.AddDialogLine("blacksmith_player_after_craft_anything_else", "player_blacksmith_after_craft", "blacksmith_player_1", "{=IvY187PJ}No matter. Anything else?", new ConversationSentence.OnConditionDelegate(this.conversation_blacksmith_player_after_craft_anything_else_on_condition), null, 100, null);
			campaignGameStarter.AddPlayerLine("blacksmith_craft_items_1", "blacksmith_player_1", "blacksmith_player_ok", "{=hrn1Cdwo}There is something else I need you to make.", () => Campaign.Current.IsCraftingEnabled, new ConversationSentence.OnConsequenceDelegate(this.conversation_blacksmith_craft_items_on_consequence), 100, null, null);
			campaignGameStarter.AddPlayerLine("blacksmith_leave_1", "blacksmith_player_1", "close_window", "{=iW9iKbb8}Nothing.", null, null, 100, null, null);
		}

		public void OnSessionLaunched(CampaignGameStarter campaignGameStarter)
		{
			this.AddDialogs(campaignGameStarter);
		}

		private bool conversation_blacksmith_begin_on_condition()
		{
			return CharacterObject.OneToOneConversationCharacter.Occupation == Occupation.Blacksmith;
		}

		private void conversation_blacksmith_craft_items_on_consequence()
		{
			CraftingHelper.OpenCrafting(CraftingTemplate.All[0], null);
		}

		private bool conversation_blacksmith_player_ok_on_condition()
		{
			if (CharacterObject.OneToOneConversationCharacter.Occupation == Occupation.Blacksmith)
			{
				if (this._latestCraftedItem != null)
				{
					int value = this._latestCraftedItem.Value;
					TextObject textObject = new TextObject("{=zr80RMa6}This will cost you {AMOUNT}{GOLD_ICON}. Paid in advance, of course.", null);
					textObject.SetTextVariable("AMOUNT", value.ToString());
					MBTextManager.SetTextVariable("CRAFTING_END_TEXT", textObject, false);
				}
				else
				{
					MBTextManager.SetTextVariable("CRAFTING_END_TEXT", new TextObject("{=rrQx9jaV}It seems you're not interested.", null), false);
				}
				return true;
			}
			return false;
		}

		private bool conversation_blacksmith_player_after_craft_accept_on_condition()
		{
			if (this._latestCraftedItem != null)
			{
				int value = this._latestCraftedItem.Value;
				return Hero.MainHero.Gold >= value;
			}
			return false;
		}

		private void conversation_blacksmith_player_after_craft_accept_on_consequence()
		{
			ItemRoster itemRoster = new ItemRoster();
			itemRoster.AddToCounts(this._latestCraftedItem, 1);
			this._latestCraftedItem = null;
			InventoryManager.OpenScreenAsTrade(itemRoster, Settlement.CurrentSettlement.Town, InventoryManager.InventoryCategoryType.None, null);
		}

		private bool conversation_blacksmith_player_after_craft_anything_else_on_condition()
		{
			this._latestCraftedItem = null;
			return true;
		}

		public bool CanHeroUsePart(Hero hero, CraftingPiece craftingPiece)
		{
			return true;
		}

		public int GetHeroCraftingStamina(Hero hero)
		{
			return this.GetRecordForCompanion(hero).CraftingStamina;
		}

		private CraftingCampaignBehavior.HeroCraftingRecord GetRecordForCompanion(Hero hero)
		{
			CraftingCampaignBehavior.HeroCraftingRecord heroCraftingRecord;
			if (!this._heroCraftingRecords.TryGetValue(hero, out heroCraftingRecord))
			{
				heroCraftingRecord = new CraftingCampaignBehavior.HeroCraftingRecord(this.GetMaxHeroCraftingStamina(hero));
				this._heroCraftingRecords[hero] = heroCraftingRecord;
			}
			return heroCraftingRecord;
		}

		public void SetHeroCraftingStamina(Hero hero, int value)
		{
			this.GetRecordForCompanion(hero).CraftingStamina = MathF.Max(0, value);
		}

		public int GetMaxHeroCraftingStamina(Hero hero)
		{
			return 100 + MathF.Round((float)hero.GetSkillValue(DefaultSkills.Crafting) * 0.5f);
		}

		public void DoRefinement(Hero hero, Crafting.RefiningFormula refineFormula)
		{
			ItemRoster itemRoster = MobileParty.MainParty.ItemRoster;
			if (refineFormula.Input1Count > 0)
			{
				ItemObject craftingMaterialItem = Campaign.Current.Models.SmithingModel.GetCraftingMaterialItem(refineFormula.Input1);
				itemRoster.AddToCounts(craftingMaterialItem, -refineFormula.Input1Count);
			}
			if (refineFormula.Input2Count > 0)
			{
				ItemObject craftingMaterialItem2 = Campaign.Current.Models.SmithingModel.GetCraftingMaterialItem(refineFormula.Input2);
				itemRoster.AddToCounts(craftingMaterialItem2, -refineFormula.Input2Count);
			}
			if (refineFormula.OutputCount > 0)
			{
				ItemObject craftingMaterialItem3 = Campaign.Current.Models.SmithingModel.GetCraftingMaterialItem(refineFormula.Output);
				itemRoster.AddToCounts(craftingMaterialItem3, refineFormula.OutputCount);
			}
			if (refineFormula.Output2Count > 0)
			{
				ItemObject craftingMaterialItem4 = Campaign.Current.Models.SmithingModel.GetCraftingMaterialItem(refineFormula.Output2);
				itemRoster.AddToCounts(craftingMaterialItem4, refineFormula.Output2Count);
			}
			hero.AddSkillXp(DefaultSkills.Crafting, (float)Campaign.Current.Models.SmithingModel.GetSkillXpForRefining(ref refineFormula));
			int energyCostForRefining = Campaign.Current.Models.SmithingModel.GetEnergyCostForRefining(ref refineFormula, hero);
			this.SetHeroCraftingStamina(hero, this.GetHeroCraftingStamina(hero) - energyCostForRefining);
		}

		public void DoSmelting(Hero hero, EquipmentElement equipmentElement)
		{
			ItemRoster itemRoster = MobileParty.MainParty.ItemRoster;
			ItemObject item = equipmentElement.Item;
			int[] smeltingOutputForItem = Campaign.Current.Models.SmithingModel.GetSmeltingOutputForItem(item);
			for (int i = 8; i >= 0; i--)
			{
				if (smeltingOutputForItem[i] != 0)
				{
					itemRoster.AddToCounts(Campaign.Current.Models.SmithingModel.GetCraftingMaterialItem((CraftingMaterials)i), smeltingOutputForItem[i]);
				}
			}
			itemRoster.AddToCounts(equipmentElement, -1);
			hero.AddSkillXp(DefaultSkills.Crafting, (float)Campaign.Current.Models.SmithingModel.GetSkillXpForSmelting(item));
			int energyCostForSmelting = Campaign.Current.Models.SmithingModel.GetEnergyCostForSmelting(item, hero);
			this.SetHeroCraftingStamina(hero, this.GetHeroCraftingStamina(hero) - energyCostForSmelting);
			this.AddResearchPoints(item.WeaponDesign.Template, Campaign.Current.Models.SmithingModel.GetPartResearchGainForSmeltingItem(item, hero));
			CampaignEventDispatcher.Instance.OnEquipmentSmeltedByHero(hero, equipmentElement);
		}

		public ItemObject CreateCraftedWeaponInFreeBuildMode(Hero hero, WeaponDesign weaponDesign, int modifierTier, Crafting.OverrideData overrideData)
		{
			CraftingCampaignBehavior.SpendMaterials(weaponDesign);
			Campaign.Current.Models.SmithingModel.CalculateWeaponDesignDifficulty(weaponDesign);
			CraftingState craftingState;
			if ((craftingState = GameStateManager.Current.ActiveState as CraftingState) != null)
			{
				ItemObject currentCraftedItemObject = craftingState.CraftingLogic.GetCurrentCraftedItemObject(true, overrideData);
				ItemObject.InitAsPlayerCraftedItem(ref currentCraftedItemObject);
				MBObjectManager.Instance.RegisterObject<ItemObject>(currentCraftedItemObject);
				PartyBase.MainParty.ItemRoster.AddToCounts(currentCraftedItemObject, 1);
				CampaignEventDispatcher.Instance.OnNewItemCrafted(currentCraftedItemObject, overrideData, false);
				hero.AddSkillXp(DefaultSkills.Crafting, (float)Campaign.Current.Models.SmithingModel.GetSkillXpForSmithingInFreeBuildMode(currentCraftedItemObject));
				int energyCostForSmithing = Campaign.Current.Models.SmithingModel.GetEnergyCostForSmithing(currentCraftedItemObject, hero);
				this.SetHeroCraftingStamina(hero, this.GetHeroCraftingStamina(hero) - energyCostForSmithing);
				this.AddResearchPoints(weaponDesign.Template, Campaign.Current.Models.SmithingModel.GetPartResearchGainForSmithingItem(currentCraftedItemObject, hero, true));
				TextObject textObject = GameTexts.FindText("crafting_added_to_inventory", null);
				textObject.SetCharacterProperties("PLAYER", Hero.MainHero.CharacterObject, false);
				textObject.SetTextVariable("ITEM_NAME", currentCraftedItemObject.Name);
				MBInformationManager.AddQuickInformation(textObject, 0, null, "");
				this.AddItemToHistory(craftingState.CraftingLogic.CurrentWeaponDesign);
				return currentCraftedItemObject;
			}
			return null;
		}

		public ItemObject CreateCraftedWeaponInCraftingOrderMode(Hero crafterHero, CraftingOrder craftingOrder, WeaponDesign weaponDesign, int modifierTier, Crafting.OverrideData overrideData)
		{
			CraftingCampaignBehavior.SpendMaterials(weaponDesign);
			SmithingModel smithingModel = Campaign.Current.Models.SmithingModel;
			CraftingState craftingState;
			if ((craftingState = GameStateManager.Current.ActiveState as CraftingState) != null)
			{
				ItemObject currentCraftedItemObject = craftingState.CraftingLogic.GetCurrentCraftedItemObject(true, overrideData);
				ItemObject.InitAsPlayerCraftedItem(ref currentCraftedItemObject);
				MBObjectManager.Instance.RegisterObject<ItemObject>(currentCraftedItemObject);
				Campaign.Current.CampaignEvents.OnNewItemCrafted(currentCraftedItemObject, overrideData, true);
				float num = craftingOrder.GetOrderExperience(currentCraftedItemObject) + (float)Campaign.Current.Models.SmithingModel.GetSkillXpForSmithingInCraftingOrderMode(currentCraftedItemObject);
				crafterHero.AddSkillXp(DefaultSkills.Crafting, num);
				int energyCostForSmithing = Campaign.Current.Models.SmithingModel.GetEnergyCostForSmithing(currentCraftedItemObject, crafterHero);
				this.SetHeroCraftingStamina(crafterHero, this.GetHeroCraftingStamina(crafterHero) - energyCostForSmithing);
				this.AddResearchPoints(weaponDesign.Template, Campaign.Current.Models.SmithingModel.GetPartResearchGainForSmithingItem(currentCraftedItemObject, crafterHero, false));
				return currentCraftedItemObject;
			}
			return null;
		}

		private static void SpendMaterials(WeaponDesign weaponDesign)
		{
			ItemRoster itemRoster = MobileParty.MainParty.ItemRoster;
			int[] smithingCostsForWeaponDesign = Campaign.Current.Models.SmithingModel.GetSmithingCostsForWeaponDesign(weaponDesign);
			for (int i = 8; i >= 0; i--)
			{
				if (smithingCostsForWeaponDesign[i] != 0)
				{
					itemRoster.AddToCounts(Campaign.Current.Models.SmithingModel.GetCraftingMaterialItem((CraftingMaterials)i), smithingCostsForWeaponDesign[i]);
				}
			}
		}

		private void AddItemToHistory(WeaponDesign design)
		{
			while (this._craftingHistory.Count >= 10)
			{
				this._craftingHistory.RemoveAt(0);
			}
			this._craftingHistory.Add(design);
		}

		public void CreateTownOrder(Hero orderOwner, int orderSlot)
		{
			if (orderOwner.CurrentSettlement == null || !orderOwner.CurrentSettlement.IsTown)
			{
				Debug.Print(string.Concat(new string[]
				{
					"Order owner: ",
					orderOwner.StringId,
					" Settlement",
					(orderOwner.CurrentSettlement == null) ? "null" : orderOwner.CurrentSettlement.StringId,
					" Order owner party: ",
					(orderOwner.PartyBelongedTo == null) ? "null" : orderOwner.PartyBelongedTo.StringId
				}), 0, Debug.DebugColor.White, 17592186044416UL);
			}
			float townOrderDifficulty = this.GetTownOrderDifficulty(orderOwner.CurrentSettlement.Town, orderSlot);
			int num = (int)townOrderDifficulty / 50;
			CraftingTemplate randomElement = CraftingTemplate.All.GetRandomElement<CraftingTemplate>();
			WeaponDesign weaponDesign = new WeaponDesign(randomElement, TextObject.Empty, this.GetWeaponPieces(randomElement, num));
			this._craftingOrders[orderOwner.CurrentSettlement.Town].AddTownOrder(new CraftingOrder(orderOwner, townOrderDifficulty, weaponDesign, randomElement, orderSlot));
		}

		private float GetTownOrderDifficulty(Town town, int orderSlot)
		{
			int num = 0;
			switch (orderSlot)
			{
			case 0:
				num = MBRandom.RandomInt(40, 80);
				break;
			case 1:
				num = MBRandom.RandomInt(80, 120);
				break;
			case 2:
				num = MBRandom.RandomInt(120, 160);
				break;
			case 3:
				num = MBRandom.RandomInt(160, 200);
				break;
			case 4:
				num = MBRandom.RandomInt(200, 241);
				break;
			case 5:
				num = Hero.MainHero.GetSkillValue(DefaultSkills.Crafting);
				break;
			}
			return (float)num + town.Prosperity / 500f;
		}

		public CraftingOrder CreateRandomQuestOrderForHero(Hero orderOwner, string questId)
		{
			float randomOrderDifficulty = this.GetRandomOrderDifficulty(orderOwner.CurrentSettlement.Town);
			int num = (int)randomOrderDifficulty / 40;
			CraftingTemplate randomElement = CraftingTemplate.All.GetRandomElement<CraftingTemplate>();
			WeaponDesign weaponDesign = new WeaponDesign(randomElement, new TextObject("{=!}crafing order attempt 1", null), this.GetWeaponPieces(randomElement, num));
			CraftingOrder craftingOrder = new CraftingOrder(orderOwner, randomOrderDifficulty, weaponDesign, randomElement, -1);
			this._craftingOrders[orderOwner.CurrentSettlement.Town].AddQuestOrder(questId, craftingOrder);
			return craftingOrder;
		}

		public CraftingOrder CreateQuestOrderForHero(Hero orderOwner, string questId, float orderDifficulty = -1f, WeaponDesign weaponDesign = null, CraftingTemplate craftingTemplate = null)
		{
			if (orderDifficulty < 0f)
			{
				orderDifficulty = this.GetRandomOrderDifficulty(orderOwner.CurrentSettlement.Town);
			}
			if (craftingTemplate == null)
			{
				craftingTemplate = CraftingTemplate.All.GetRandomElement<CraftingTemplate>();
			}
			if (weaponDesign == null)
			{
				int num = (int)orderDifficulty / 40;
				weaponDesign = new WeaponDesign(craftingTemplate, new TextObject("{=!}crafing order attempt 1", null), this.GetWeaponPieces(craftingTemplate, num));
			}
			CraftingOrder craftingOrder = new CraftingOrder(orderOwner, orderDifficulty, weaponDesign, craftingTemplate, -1);
			this._craftingOrders[orderOwner.CurrentSettlement.Town].AddQuestOrder(questId, craftingOrder);
			return craftingOrder;
		}

		private float GetRandomOrderDifficulty(Town town)
		{
			int num = MBRandom.RandomInt(0, 6);
			int num2 = 0;
			switch (num)
			{
			case 0:
				num2 = MBRandom.RandomInt(40, 80);
				break;
			case 1:
				num2 = MBRandom.RandomInt(80, 120);
				break;
			case 2:
				num2 = MBRandom.RandomInt(120, 160);
				break;
			case 3:
				num2 = MBRandom.RandomInt(160, 200);
				break;
			case 4:
				num2 = MBRandom.RandomInt(200, 241);
				break;
			case 5:
				num2 = Hero.MainHero.GetSkillValue(DefaultSkills.Crafting);
				break;
			}
			return (float)num2 + town.Prosperity / 500f;
		}

		private WeaponDesignElement[] GetWeaponPieces(CraftingTemplate craftingTemplate, int pieceTier)
		{
			WeaponDesignElement[] array = new WeaponDesignElement[4];
			List<WeaponDesignElement>[] array2 = new List<WeaponDesignElement>[4];
			foreach (CraftingPiece craftingPiece in craftingTemplate.Pieces)
			{
				bool flag = false;
				foreach (PieceData pieceData in craftingTemplate.BuildOrders)
				{
					if (pieceData.PieceType == craftingPiece.PieceType)
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					int pieceType = (int)craftingPiece.PieceType;
					if (array2[pieceType] == null)
					{
						array2[pieceType] = new List<WeaponDesignElement>();
					}
					array2[pieceType].Add(WeaponDesignElement.CreateUsablePiece(craftingPiece, 100));
				}
			}
			Func<WeaponDesignElement, bool> <>9__0;
			for (int j = 0; j < array.Length; j++)
			{
				if (array2[j] != null)
				{
					WeaponDesignElement[] array3 = array;
					int num = j;
					List<WeaponDesignElement> list = array2[j];
					Func<WeaponDesignElement, bool> func;
					if ((func = <>9__0) == null)
					{
						func = (<>9__0 = (WeaponDesignElement p) => !p.CraftingPiece.IsHiddenOnDesigner && p.CraftingPiece.PieceTier == pieceTier);
					}
					WeaponDesignElement weaponDesignElement;
					if ((weaponDesignElement = list.FirstOrDefaultQ(func)) == null)
					{
						weaponDesignElement = array2[j].FirstOrDefaultQ((WeaponDesignElement p) => !p.CraftingPiece.IsHiddenOnDesigner && p.CraftingPiece.PieceTier == 1);
					}
					WeaponDesignElement weaponDesignElement2;
					if ((weaponDesignElement2 = weaponDesignElement) == null)
					{
						weaponDesignElement2 = array2[j].First((WeaponDesignElement p) => !p.CraftingPiece.IsHiddenOnDesigner);
					}
					array3[num] = weaponDesignElement2;
				}
				else
				{
					array[j] = WeaponDesignElement.GetInvalidPieceForType((CraftingPiece.PieceTypes)j);
				}
			}
			return array;
		}

		private void ReplaceCraftingOrder(Town town, CraftingOrder order)
		{
			MBList<Hero> mblist = new MBList<Hero>();
			Settlement settlement = town.Settlement;
			mblist.AddRange(settlement.HeroesWithoutParty);
			foreach (MobileParty mobileParty in settlement.Parties)
			{
				if (mobileParty.LeaderHero != null && !mobileParty.IsMainParty)
				{
					mblist.Add(mobileParty.LeaderHero);
				}
			}
			int difficultyLevel = order.DifficultyLevel;
			this._craftingOrders[town].RemoveTownOrder(order);
			if (mblist.Count > 0)
			{
				this.CreateTownOrder(mblist.GetRandomElement<Hero>(), difficultyLevel);
			}
			mblist = null;
		}

		public void GetOrderResult(CraftingOrder craftingOrder, ItemObject craftedItem, out bool isSucceed, out TextObject orderRemark, out TextObject orderResult, out int finalReward)
		{
			finalReward = this.CalculateOrderPriceDifference(craftingOrder, craftedItem);
			float num;
			float num2;
			bool flag;
			bool flag2;
			craftingOrder.CheckForBonusesAndPenalties(craftedItem, out num, out num2, out flag, out flag2);
			isSucceed = num >= num2 && flag && flag2;
			int num3 = finalReward - craftingOrder.BaseGoldReward;
			orderRemark = TextObject.Empty;
			if (isSucceed)
			{
				orderResult = new TextObject("{=Nn49hU2W}The client is satisfied.", null);
				if (num3 == 0)
				{
					orderRemark = new TextObject("{=FWHvvZFq}\"This is exactly what I wanted. Here is your money, you've earned it.\"", null);
					return;
				}
				if ((float)num3 > 0f)
				{
					orderRemark = new TextObject("{=raCa7QXj}\"This is even better than what I have imagined. Here is your money, and I'm putting a little extra for your effort.\"", null);
					return;
				}
			}
			else
			{
				orderResult = new TextObject("{=bC2jevlu}The client is displeased.", null);
				if (finalReward <= 0)
				{
					orderRemark = new TextObject("{=NZynd8vT}\"This weapon is worthless. I'm not giving you a dime!\"", null);
					return;
				}
				if (finalReward < craftingOrder.BaseGoldReward)
				{
					TextObject textObject = TextObject.Empty;
					if (!flag || !flag2)
					{
						textObject = new TextObject("{=WyuIksRB}\"This weapon does not have the damage type I wanted. I'm cutting {AMOUNT}{GOLD_ICON} from the price.\"", null);
					}
					else
					{
						textObject = new TextObject("{=wU76OPxM}\"This is worse than what I've asked for. I'm cutting {AMOUNT}{GOLD_ICON} from the price.\"", null);
					}
					textObject.SetTextVariable("AMOUNT", MathF.Abs(num3));
					orderRemark = textObject;
				}
			}
		}

		private int CalculateOrderPriceDifference(CraftingOrder craftingOrder, ItemObject craftedItem)
		{
			float num;
			float num2;
			bool flag;
			bool flag2;
			craftingOrder.CheckForBonusesAndPenalties(craftedItem, out num, out num2, out flag, out flag2);
			float num3 = (float)craftingOrder.BaseGoldReward;
			if (!num.ApproximatelyEqualsTo(0f, 1E-05f) && !num2.ApproximatelyEqualsTo(0f, 1E-05f))
			{
				if (num < num2 || !flag || !flag2)
				{
					float num4 = (float)Campaign.Current.Models.TradeItemPriceFactorModel.GetTheoreticalMaxItemMarketValue(craftedItem) / (float)Campaign.Current.Models.TradeItemPriceFactorModel.GetTheoreticalMaxItemMarketValue(craftingOrder.PreCraftedWeaponDesignItem);
					num3 = (float)craftingOrder.BaseGoldReward * 0.5f * MathF.Min(1f, num4);
					if (num3 > (float)craftingOrder.BaseGoldReward)
					{
						num3 = (float)craftingOrder.BaseGoldReward * 0.5f;
					}
				}
				else if (num > num2)
				{
					num3 = (float)craftingOrder.BaseGoldReward * (1f + (num - num2) / num2 * 0.1f);
				}
			}
			return (int)num3;
		}

		public void CompleteOrder(Town town, CraftingOrder craftingOrder, ItemObject craftedItem, Hero completerHero)
		{
			int num = this.CalculateOrderPriceDifference(craftingOrder, craftedItem);
			bool flag;
			TextObject textObject;
			TextObject textObject2;
			int num2;
			this.GetOrderResult(craftingOrder, craftedItem, out flag, out textObject, out textObject2, out num2);
			GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, num, false);
			if (craftingOrder.IsLordOrder)
			{
				this.ChangeCraftedOrderWithTheNoblesWeaponIfItIsBetter(craftedItem, craftingOrder);
				if (craftingOrder.OrderOwner.PartyBelongedTo != null)
				{
					this.GiveTroopToNobleAtWeaponTier((int)craftedItem.Tier, craftingOrder.OrderOwner);
				}
				if (flag && completerHero.GetPerkValue(DefaultPerks.Crafting.SteelMaker3))
				{
					ChangeRelationAction.ApplyRelationChangeBetweenHeroes(completerHero, craftingOrder.OrderOwner, (int)DefaultPerks.Crafting.SteelMaker3.SecondaryBonus, true);
				}
			}
			else
			{
				craftingOrder.OrderOwner.AddPower((float)(craftedItem.Tier + 1));
				if (flag && completerHero.GetPerkValue(DefaultPerks.Crafting.ExperiencedSmith))
				{
					ChangeRelationAction.ApplyRelationChangeBetweenHeroes(completerHero, craftingOrder.OrderOwner, (int)DefaultPerks.Crafting.ExperiencedSmith.SecondaryBonus, true);
				}
			}
			this._craftingOrders[town].RemoveTownOrder(craftingOrder);
		}

		private void RemoveOrdersOfHeroWithoutCompletionIfExists(Hero hero)
		{
			new List<CraftingOrder>();
			foreach (KeyValuePair<Town, CraftingCampaignBehavior.CraftingOrderSlots> keyValuePair in this._craftingOrders)
			{
				for (int i = 0; i < 6; i++)
				{
					if (keyValuePair.Value.Slots[i] != null && keyValuePair.Value.Slots[i].OrderOwner == hero)
					{
						keyValuePair.Value.RemoveTownOrder(keyValuePair.Value.Slots[i]);
					}
				}
			}
		}

		private void CancelOrder(Town town, CraftingOrder craftingOrder)
		{
			this._craftingOrders[town].RemoveTownOrder(craftingOrder);
		}

		private void ChangeCraftedOrderWithTheNoblesWeaponIfItIsBetter(ItemObject craftedItem, CraftingOrder craftingOrder)
		{
			Equipment battleEquipment = craftingOrder.OrderOwner.BattleEquipment;
			for (int i = 0; i < 12; i++)
			{
				if (!battleEquipment[i].IsEmpty)
				{
					WeaponClass weaponClass = craftedItem.PrimaryWeapon.WeaponClass;
					WeaponComponentData primaryWeapon = battleEquipment[i].Item.PrimaryWeapon;
					WeaponClass? weaponClass2 = ((primaryWeapon != null) ? new WeaponClass?(primaryWeapon.WeaponClass) : null);
					if ((weaponClass == weaponClass2.GetValueOrDefault()) & (weaponClass2 != null))
					{
						ItemObject item = battleEquipment[i].Item;
						int thrustSpeed = item.PrimaryWeapon.ThrustSpeed;
						int thrustSpeed2 = craftedItem.PrimaryWeapon.ThrustSpeed;
						int swingSpeed = item.PrimaryWeapon.SwingSpeed;
						int swingSpeed2 = craftedItem.PrimaryWeapon.SwingSpeed;
						int missileSpeed = item.PrimaryWeapon.MissileSpeed;
						int missileSpeed2 = craftedItem.PrimaryWeapon.MissileSpeed;
						float weaponBalance = item.PrimaryWeapon.WeaponBalance;
						float weaponBalance2 = craftedItem.PrimaryWeapon.WeaponBalance;
						int thrustDamage = item.PrimaryWeapon.ThrustDamage;
						int thrustDamage2 = craftedItem.PrimaryWeapon.ThrustDamage;
						DamageTypes thrustDamageType = item.PrimaryWeapon.ThrustDamageType;
						DamageTypes thrustDamageType2 = craftedItem.PrimaryWeapon.ThrustDamageType;
						int swingDamage = item.PrimaryWeapon.SwingDamage;
						int swingDamage2 = craftedItem.PrimaryWeapon.SwingDamage;
						DamageTypes swingDamageType = item.PrimaryWeapon.SwingDamageType;
						DamageTypes swingDamageType2 = craftedItem.PrimaryWeapon.SwingDamageType;
						int accuracy = item.PrimaryWeapon.Accuracy;
						int accuracy2 = craftedItem.PrimaryWeapon.Accuracy;
						float weight = item.Weight;
						float weight2 = craftedItem.Weight;
						if (thrustSpeed2 > thrustSpeed && swingSpeed2 > swingSpeed && missileSpeed2 > missileSpeed && weaponBalance2 > weaponBalance && thrustDamage2 > thrustDamage && thrustDamageType == thrustDamageType2 && swingDamage2 > swingDamage && swingDamageType2 == swingDamageType && accuracy2 > accuracy && weight2 < weight)
						{
							battleEquipment[i] = new EquipmentElement(craftedItem, null, null, false);
							return;
						}
					}
				}
			}
		}

		private void GiveTroopToNobleAtWeaponTier(int tier, Hero noble)
		{
			CharacterObject characterObject = noble.Culture.BasicTroop;
			for (int i = 0; i < tier; i++)
			{
				if (characterObject.UpgradeTargets.Length != 0)
				{
					characterObject = characterObject.UpgradeTargets.GetRandomElement<CharacterObject>();
				}
			}
			noble.PartyBelongedTo.AddElementToMemberRoster(characterObject, 1, false);
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("unlock_all_crafting_pieces", "campaign")]
		public static string UnlockCraftingPieces(List<string> strings)
		{
			string text = "";
			if (!CampaignCheats.CheckCheatUsage(ref text))
			{
				return text;
			}
			if (!CampaignCheats.CheckParameters(strings, 0) || CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"campaign.unlock_all_crafting_pieces\".";
			}
			CraftingCampaignBehavior campaignBehavior = Campaign.Current.GetCampaignBehavior<CraftingCampaignBehavior>();
			if (campaignBehavior == null)
			{
				return "Can not find Crafting Campaign Behavior!";
			}
			foreach (CraftingTemplate craftingTemplate in CraftingTemplate.All)
			{
				if (!campaignBehavior._openedPartsDictionary.ContainsKey(craftingTemplate))
				{
					campaignBehavior._openedPartsDictionary.Add(craftingTemplate, new List<CraftingPiece>());
				}
				if (!campaignBehavior._openNewPartXpDictionary.ContainsKey(craftingTemplate))
				{
					campaignBehavior._openNewPartXpDictionary.Add(craftingTemplate, 0f);
				}
				foreach (CraftingPiece craftingPiece in craftingTemplate.Pieces)
				{
					campaignBehavior.OpenPart(craftingPiece, craftingTemplate, false);
				}
			}
			return "Success";
		}

		private const float WaitTargetHours = 8f;

		private const float CraftingOrderReplaceChance = 0.05f;

		private const float CreateCraftingOrderChance = 0.05f;

		private const int TownCraftingOrderCount = 6;

		private const int DefaultCraftingOrderPieceTier = 1;

		private const int CraftingOrderTroopBonusAmount = 1;

		private const int MinOrderDifficulty = 40;

		private const int MaxOrderDifficulty = 240;

		private const int MaxCraftingHistoryDesigns = 10;

		private ItemObject _latestCraftedItem;

		private Dictionary<CraftingTemplate, List<CraftingPiece>> _openedPartsDictionary = new Dictionary<CraftingTemplate, List<CraftingPiece>>();

		private Dictionary<CraftingTemplate, float> _openNewPartXpDictionary = new Dictionary<CraftingTemplate, float>();

		private Dictionary<ItemObject, CraftingCampaignBehavior.CraftedItemInitializationData> _craftedItemDictionary = new Dictionary<ItemObject, CraftingCampaignBehavior.CraftedItemInitializationData>();

		private Dictionary<Hero, CraftingCampaignBehavior.HeroCraftingRecord> _heroCraftingRecords = new Dictionary<Hero, CraftingCampaignBehavior.HeroCraftingRecord>();

		private List<WeaponDesign> _craftingHistory = new List<WeaponDesign>();

		private Dictionary<Town, CraftingCampaignBehavior.CraftingOrderSlots> _craftingOrders = new Dictionary<Town, CraftingCampaignBehavior.CraftingOrderSlots>();

		public class CraftingCampaignBehaviorTypeDefiner : CampaignBehaviorBase.SaveableCampaignBehaviorTypeDefiner
		{
			public CraftingCampaignBehaviorTypeDefiner()
				: base(150000)
			{
			}

			protected override void DefineClassTypes()
			{
				base.AddClassDefinition(typeof(CraftingCampaignBehavior.CraftedItemInitializationData), 10, null);
				base.AddClassDefinition(typeof(CraftingCampaignBehavior.HeroCraftingRecord), 20, null);
				base.AddClassDefinition(typeof(CraftingCampaignBehavior.CraftingOrderSlots), 30, null);
			}

			protected override void DefineContainerDefinitions()
			{
				base.ConstructContainerDefinition(typeof(Dictionary<ItemObject, CraftingCampaignBehavior.CraftedItemInitializationData>));
				base.ConstructContainerDefinition(typeof(Dictionary<Hero, CraftingCampaignBehavior.HeroCraftingRecord>));
				base.ConstructContainerDefinition(typeof(Dictionary<Town, CraftingCampaignBehavior.CraftingOrderSlots>));
			}
		}

		internal class CraftedItemInitializationData
		{
			public CraftedItemInitializationData(WeaponDesign craftedData, TextObject itemName, CultureObject culture)
			{
				this.CraftedData = craftedData;
				this.ItemName = itemName;
				this.Culture = culture;
			}

			internal static void AutoGeneratedStaticCollectObjectsCraftedItemInitializationData(object o, List<object> collectedObjects)
			{
				((CraftingCampaignBehavior.CraftedItemInitializationData)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			protected virtual void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				collectedObjects.Add(this.CraftedData);
				collectedObjects.Add(this.ItemName);
				collectedObjects.Add(this.Culture);
			}

			internal static object AutoGeneratedGetMemberValueCraftedData(object o)
			{
				return ((CraftingCampaignBehavior.CraftedItemInitializationData)o).CraftedData;
			}

			internal static object AutoGeneratedGetMemberValueItemName(object o)
			{
				return ((CraftingCampaignBehavior.CraftedItemInitializationData)o).ItemName;
			}

			internal static object AutoGeneratedGetMemberValueCulture(object o)
			{
				return ((CraftingCampaignBehavior.CraftedItemInitializationData)o).Culture;
			}

			[SaveableField(10)]
			public readonly WeaponDesign CraftedData;

			[SaveableField(20)]
			public readonly TextObject ItemName;

			[SaveableField(30)]
			public readonly CultureObject Culture;
		}

		internal class HeroCraftingRecord
		{
			public HeroCraftingRecord(int maxStamina)
			{
				this.CraftingStamina = maxStamina;
			}

			internal static void AutoGeneratedStaticCollectObjectsHeroCraftingRecord(object o, List<object> collectedObjects)
			{
				((CraftingCampaignBehavior.HeroCraftingRecord)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			protected virtual void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
			}

			internal static object AutoGeneratedGetMemberValueCraftingStamina(object o)
			{
				return ((CraftingCampaignBehavior.HeroCraftingRecord)o).CraftingStamina;
			}

			[SaveableField(10)]
			public int CraftingStamina;
		}

		public class CraftingOrderSlots
		{
			public CraftingOrderSlots()
			{
				this.Slots = new CraftingOrder[6];
				for (int i = 0; i < 6; i++)
				{
					this.Slots[i] = null;
				}
				this._questOrders = new Dictionary<string, List<CraftingOrder>>();
				this.QuestOrders = this._questOrders.GetReadOnlyDictionary<string, List<CraftingOrder>>();
			}

			[LoadInitializationCallback]
			private void OnLoad(MetaData metaData)
			{
				this.QuestOrders = this._questOrders.GetReadOnlyDictionary<string, List<CraftingOrder>>();
			}

			public bool IsThereAvailableSlot()
			{
				for (int i = 0; i < 6; i++)
				{
					if (this.Slots[i] == null)
					{
						return true;
					}
				}
				return false;
			}

			public int GetAvailableSlot()
			{
				for (int i = 0; i < 6; i++)
				{
					if (this.Slots[i] == null)
					{
						return i;
					}
				}
				return -1;
			}

			internal void AddTownOrder(CraftingOrder craftingOrder)
			{
				this.Slots[craftingOrder.DifficultyLevel] = craftingOrder;
			}

			internal void RemoveTownOrder(CraftingOrder craftingOrder)
			{
				this.Slots[craftingOrder.DifficultyLevel] = null;
			}

			internal void AddQuestOrder(string questId, CraftingOrder order)
			{
				if (!this._questOrders.ContainsKey(questId))
				{
					this._questOrders.Add(questId, new List<CraftingOrder>());
				}
				this._questOrders[questId].Add(order);
			}

			internal void RemoveQuestOrder(string questId, CraftingOrder order)
			{
				this._questOrders[questId].Remove(order);
			}

			internal void RemoveAllQuestOrders(string questId)
			{
				this._questOrders.Remove(questId);
			}

			internal static void AutoGeneratedStaticCollectObjectsCraftingOrderSlots(object o, List<object> collectedObjects)
			{
				((CraftingCampaignBehavior.CraftingOrderSlots)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			protected virtual void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				collectedObjects.Add(this.Slots);
				collectedObjects.Add(this._questOrders);
			}

			internal static object AutoGeneratedGetMemberValueSlots(object o)
			{
				return ((CraftingCampaignBehavior.CraftingOrderSlots)o).Slots;
			}

			internal static object AutoGeneratedGetMemberValue_questOrders(object o)
			{
				return ((CraftingCampaignBehavior.CraftingOrderSlots)o)._questOrders;
			}

			private const int SlotCount = 6;

			[SaveableField(10)]
			public CraftingOrder[] Slots;

			[SaveableField(20)]
			private readonly Dictionary<string, List<CraftingOrder>> _questOrders;

			public MBReadOnlyDictionary<string, List<CraftingOrder>> QuestOrders;
		}
	}
}
