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
	// Token: 0x02000386 RID: 902
	public class CraftingCampaignBehavior : CampaignBehaviorBase, ICraftingCampaignBehavior, ICampaignBehavior
	{
		// Token: 0x17000CBF RID: 3263
		// (get) Token: 0x060034DC RID: 13532 RVA: 0x000E29D2 File Offset: 0x000E0BD2
		public IReadOnlyDictionary<Town, CraftingCampaignBehavior.CraftingOrderSlots> CraftingOrders
		{
			get
			{
				return this._craftingOrders;
			}
		}

		// Token: 0x17000CC0 RID: 3264
		// (get) Token: 0x060034DD RID: 13533 RVA: 0x000E29DA File Offset: 0x000E0BDA
		public IReadOnlyCollection<WeaponDesign> CraftingHistory
		{
			get
			{
				return this._craftingHistory;
			}
		}

		// Token: 0x17000CC1 RID: 3265
		// (get) Token: 0x060034DE RID: 13534 RVA: 0x000E29E2 File Offset: 0x000E0BE2
		// (set) Token: 0x060034DF RID: 13535 RVA: 0x000E29E9 File Offset: 0x000E0BE9
		public static bool WeaponTypeDebugEnabled { get; private set; }

		// Token: 0x060034E0 RID: 13536 RVA: 0x000E29F4 File Offset: 0x000E0BF4
		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<ItemObject>("_latestCraftedItem", ref this._latestCraftedItem);
			dataStore.SyncData<Dictionary<ItemObject, CraftingCampaignBehavior.CraftedItemInitializationData>>("_craftedItemDictionary", ref this._craftedItemDictionary);
			dataStore.SyncData<Dictionary<Hero, CraftingCampaignBehavior.HeroCraftingRecord>>("_heroCraftingRecordsNew", ref this._heroCraftingRecords);
			dataStore.SyncData<Dictionary<Town, CraftingCampaignBehavior.CraftingOrderSlots>>("_craftingOrders", ref this._craftingOrders);
			dataStore.SyncData<List<WeaponDesign>>("_craftingHistory", ref this._craftingHistory);
			dataStore.SyncData<Dictionary<CraftingTemplate, List<CraftingPiece>>>("_openedPartsDictionary", ref this._openedPartsDictionary);
			dataStore.SyncData<Dictionary<CraftingTemplate, float>>("_openNewPartXpDictionary", ref this._openNewPartXpDictionary);
			if (dataStore.IsLoading && MBSaveLoad.IsUpdatingGameVersion && MBSaveLoad.LastLoadedGameVersion < ApplicationVersion.FromString("e1.8.0", 17949))
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

		// Token: 0x060034E1 RID: 13537 RVA: 0x000E2B98 File Offset: 0x000E0D98
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

		// Token: 0x060034E2 RID: 13538 RVA: 0x000E2C60 File Offset: 0x000E0E60
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

		// Token: 0x060034E3 RID: 13539 RVA: 0x000E2D70 File Offset: 0x000E0F70
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

		// Token: 0x060034E4 RID: 13540 RVA: 0x000E2E88 File Offset: 0x000E1088
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

		// Token: 0x060034E5 RID: 13541 RVA: 0x000E2F10 File Offset: 0x000E1110
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

		// Token: 0x060034E6 RID: 13542 RVA: 0x000E2FBC File Offset: 0x000E11BC
		private void OnHeroKilled(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail detail, bool showNotification = true)
		{
			this.RemoveOrdersOfHeroWithoutCompletionIfExists(victim);
		}

		// Token: 0x060034E7 RID: 13543 RVA: 0x000E2FC8 File Offset: 0x000E11C8
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

		// Token: 0x060034E8 RID: 13544 RVA: 0x000E3068 File Offset: 0x000E1268
		private int GetStaminaHourlyRecoveryRate(Hero hero)
		{
			int num = 5 + MathF.Round((float)hero.GetSkillValue(DefaultSkills.Crafting) * 0.025f);
			if (hero.GetPerkValue(DefaultPerks.Athletics.Stamina))
			{
				num += MathF.Round((float)num * DefaultPerks.Athletics.Stamina.PrimaryBonus);
			}
			return num;
		}

		// Token: 0x060034E9 RID: 13545 RVA: 0x000E30B4 File Offset: 0x000E12B4
		private void OnNewItemCrafted(ItemObject itemObject, Crafting.OverrideData overrideData, bool isCraftingOrderItem)
		{
			if (!this._craftedItemDictionary.ContainsKey(itemObject))
			{
				CultureObject @object = MBObjectManager.Instance.GetObject<CultureObject>(itemObject.Culture.StringId);
				CraftingCampaignBehavior.CraftedItemInitializationData craftedItemInitializationData = new CraftingCampaignBehavior.CraftedItemInitializationData(itemObject.WeaponDesign, itemObject.Name, @object);
				this._craftedItemDictionary.Add(itemObject, craftedItemInitializationData);
			}
		}

		// Token: 0x060034EA RID: 13546 RVA: 0x000E3108 File Offset: 0x000E1308
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

		// Token: 0x060034EB RID: 13547 RVA: 0x000E322C File Offset: 0x000E142C
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

		// Token: 0x060034EC RID: 13548 RVA: 0x000E32DC File Offset: 0x000E14DC
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

		// Token: 0x060034ED RID: 13549 RVA: 0x000E3344 File Offset: 0x000E1544
		public bool IsOpened(CraftingPiece craftingPiece, CraftingTemplate craftingTemplate)
		{
			return craftingPiece.IsGivenByDefault || this._openedPartsDictionary[craftingTemplate].Contains(craftingPiece);
		}

		// Token: 0x060034EE RID: 13550 RVA: 0x000E3364 File Offset: 0x000E1564
		public void InitializeCraftingElements()
		{
			List<ItemObject> list = new List<ItemObject>();
			foreach (KeyValuePair<ItemObject, CraftingCampaignBehavior.CraftedItemInitializationData> keyValuePair in this._craftedItemDictionary)
			{
				WeaponDesign weaponDesign = keyValuePair.Value.CraftedData;
				if (MBSaveLoad.IsUpdatingGameVersion && MBSaveLoad.LastLoadedGameVersion < ApplicationVersion.FromString("v1.1.0", 17949))
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

		// Token: 0x060034EF RID: 13551 RVA: 0x000E35B0 File Offset: 0x000E17B0
		public int GetCraftingDifficulty(WeaponDesign weaponDesign)
		{
			return Campaign.Current.Models.SmithingModel.CalculateWeaponDesignDifficulty(weaponDesign);
		}

		// Token: 0x060034F0 RID: 13552 RVA: 0x000E35C8 File Offset: 0x000E17C8
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

		// Token: 0x060034F1 RID: 13553 RVA: 0x000E3800 File Offset: 0x000E1A00
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

		// Token: 0x060034F2 RID: 13554 RVA: 0x000E399B File Offset: 0x000E1B9B
		public void OnSessionLaunched(CampaignGameStarter campaignGameStarter)
		{
			this.AddDialogs(campaignGameStarter);
		}

		// Token: 0x060034F3 RID: 13555 RVA: 0x000E39A4 File Offset: 0x000E1BA4
		private bool conversation_blacksmith_begin_on_condition()
		{
			return CharacterObject.OneToOneConversationCharacter.Occupation == Occupation.Blacksmith;
		}

		// Token: 0x060034F4 RID: 13556 RVA: 0x000E39B4 File Offset: 0x000E1BB4
		private void conversation_blacksmith_craft_items_on_consequence()
		{
			CraftingHelper.OpenCrafting(CraftingTemplate.All[0], null);
		}

		// Token: 0x060034F5 RID: 13557 RVA: 0x000E39C8 File Offset: 0x000E1BC8
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

		// Token: 0x060034F6 RID: 13558 RVA: 0x000E3A40 File Offset: 0x000E1C40
		private bool conversation_blacksmith_player_after_craft_accept_on_condition()
		{
			if (this._latestCraftedItem != null)
			{
				int value = this._latestCraftedItem.Value;
				return Hero.MainHero.Gold >= value;
			}
			return false;
		}

		// Token: 0x060034F7 RID: 13559 RVA: 0x000E3A73 File Offset: 0x000E1C73
		private void conversation_blacksmith_player_after_craft_accept_on_consequence()
		{
			ItemRoster itemRoster = new ItemRoster();
			itemRoster.AddToCounts(this._latestCraftedItem, 1);
			this._latestCraftedItem = null;
			InventoryManager.OpenScreenAsTrade(itemRoster, Settlement.CurrentSettlement.Town, InventoryManager.InventoryCategoryType.None, null);
		}

		// Token: 0x060034F8 RID: 13560 RVA: 0x000E3AA0 File Offset: 0x000E1CA0
		private bool conversation_blacksmith_player_after_craft_anything_else_on_condition()
		{
			this._latestCraftedItem = null;
			return true;
		}

		// Token: 0x060034F9 RID: 13561 RVA: 0x000E3AAA File Offset: 0x000E1CAA
		public bool CanHeroUsePart(Hero hero, CraftingPiece craftingPiece)
		{
			return true;
		}

		// Token: 0x060034FA RID: 13562 RVA: 0x000E3AAD File Offset: 0x000E1CAD
		public int GetHeroCraftingStamina(Hero hero)
		{
			return this.GetRecordForCompanion(hero).CraftingStamina;
		}

		// Token: 0x060034FB RID: 13563 RVA: 0x000E3ABC File Offset: 0x000E1CBC
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

		// Token: 0x060034FC RID: 13564 RVA: 0x000E3AF4 File Offset: 0x000E1CF4
		public void SetHeroCraftingStamina(Hero hero, int value)
		{
			this.GetRecordForCompanion(hero).CraftingStamina = MathF.Max(0, value);
		}

		// Token: 0x060034FD RID: 13565 RVA: 0x000E3B09 File Offset: 0x000E1D09
		public int GetMaxHeroCraftingStamina(Hero hero)
		{
			return 100 + MathF.Round((float)hero.GetSkillValue(DefaultSkills.Crafting) * 0.5f);
		}

		// Token: 0x060034FE RID: 13566 RVA: 0x000E3B28 File Offset: 0x000E1D28
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

		// Token: 0x060034FF RID: 13567 RVA: 0x000E3C58 File Offset: 0x000E1E58
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

		// Token: 0x06003500 RID: 13568 RVA: 0x000E3D4C File Offset: 0x000E1F4C
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

		// Token: 0x06003501 RID: 13569 RVA: 0x000E3E8C File Offset: 0x000E208C
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

		// Token: 0x06003502 RID: 13570 RVA: 0x000E3F74 File Offset: 0x000E2174
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

		// Token: 0x06003503 RID: 13571 RVA: 0x000E3FD2 File Offset: 0x000E21D2
		private void AddItemToHistory(WeaponDesign design)
		{
			while (this._craftingHistory.Count >= 10)
			{
				this._craftingHistory.RemoveAt(0);
			}
			this._craftingHistory.Add(design);
		}

		// Token: 0x06003504 RID: 13572 RVA: 0x000E4000 File Offset: 0x000E2200
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

		// Token: 0x06003505 RID: 13573 RVA: 0x000E40F8 File Offset: 0x000E22F8
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

		// Token: 0x06003506 RID: 13574 RVA: 0x000E4194 File Offset: 0x000E2394
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

		// Token: 0x06003507 RID: 13575 RVA: 0x000E420C File Offset: 0x000E240C
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

		// Token: 0x06003508 RID: 13576 RVA: 0x000E429C File Offset: 0x000E249C
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

		// Token: 0x06003509 RID: 13577 RVA: 0x000E4340 File Offset: 0x000E2540
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

		// Token: 0x0600350A RID: 13578 RVA: 0x000E44CC File Offset: 0x000E26CC
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

		// Token: 0x0600350B RID: 13579 RVA: 0x000E4584 File Offset: 0x000E2784
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

		// Token: 0x0600350C RID: 13580 RVA: 0x000E467C File Offset: 0x000E287C
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

		// Token: 0x0600350D RID: 13581 RVA: 0x000E4764 File Offset: 0x000E2964
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

		// Token: 0x0600350E RID: 13582 RVA: 0x000E4844 File Offset: 0x000E2A44
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

		// Token: 0x0600350F RID: 13583 RVA: 0x000E48E4 File Offset: 0x000E2AE4
		private void CancelOrder(Town town, CraftingOrder craftingOrder)
		{
			this._craftingOrders[town].RemoveTownOrder(craftingOrder);
		}

		// Token: 0x06003510 RID: 13584 RVA: 0x000E48F8 File Offset: 0x000E2AF8
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

		// Token: 0x06003511 RID: 13585 RVA: 0x000E4AE0 File Offset: 0x000E2CE0
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

		// Token: 0x06003512 RID: 13586 RVA: 0x000E4B2C File Offset: 0x000E2D2C
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

		// Token: 0x04001134 RID: 4404
		private const float WaitTargetHours = 8f;

		// Token: 0x04001135 RID: 4405
		private const float CraftingOrderReplaceChance = 0.05f;

		// Token: 0x04001136 RID: 4406
		private const float CreateCraftingOrderChance = 0.05f;

		// Token: 0x04001137 RID: 4407
		private const int TownCraftingOrderCount = 6;

		// Token: 0x04001138 RID: 4408
		private const int DefaultCraftingOrderPieceTier = 1;

		// Token: 0x04001139 RID: 4409
		private const int CraftingOrderTroopBonusAmount = 1;

		// Token: 0x0400113A RID: 4410
		private const int MinOrderDifficulty = 40;

		// Token: 0x0400113B RID: 4411
		private const int MaxOrderDifficulty = 240;

		// Token: 0x0400113C RID: 4412
		private const int MaxCraftingHistoryDesigns = 10;

		// Token: 0x0400113D RID: 4413
		private ItemObject _latestCraftedItem;

		// Token: 0x0400113E RID: 4414
		private Dictionary<CraftingTemplate, List<CraftingPiece>> _openedPartsDictionary = new Dictionary<CraftingTemplate, List<CraftingPiece>>();

		// Token: 0x0400113F RID: 4415
		private Dictionary<CraftingTemplate, float> _openNewPartXpDictionary = new Dictionary<CraftingTemplate, float>();

		// Token: 0x04001140 RID: 4416
		private Dictionary<ItemObject, CraftingCampaignBehavior.CraftedItemInitializationData> _craftedItemDictionary = new Dictionary<ItemObject, CraftingCampaignBehavior.CraftedItemInitializationData>();

		// Token: 0x04001141 RID: 4417
		private Dictionary<Hero, CraftingCampaignBehavior.HeroCraftingRecord> _heroCraftingRecords = new Dictionary<Hero, CraftingCampaignBehavior.HeroCraftingRecord>();

		// Token: 0x04001142 RID: 4418
		private List<WeaponDesign> _craftingHistory = new List<WeaponDesign>();

		// Token: 0x04001143 RID: 4419
		private Dictionary<Town, CraftingCampaignBehavior.CraftingOrderSlots> _craftingOrders = new Dictionary<Town, CraftingCampaignBehavior.CraftingOrderSlots>();

		// Token: 0x020006CB RID: 1739
		public class CraftingCampaignBehaviorTypeDefiner : CampaignBehaviorBase.SaveableCampaignBehaviorTypeDefiner
		{
			// Token: 0x0600545E RID: 21598 RVA: 0x0016ABF5 File Offset: 0x00168DF5
			public CraftingCampaignBehaviorTypeDefiner()
				: base(150000)
			{
			}

			// Token: 0x0600545F RID: 21599 RVA: 0x0016AC02 File Offset: 0x00168E02
			protected override void DefineClassTypes()
			{
				base.AddClassDefinition(typeof(CraftingCampaignBehavior.CraftedItemInitializationData), 10, null);
				base.AddClassDefinition(typeof(CraftingCampaignBehavior.HeroCraftingRecord), 20, null);
				base.AddClassDefinition(typeof(CraftingCampaignBehavior.CraftingOrderSlots), 30, null);
			}

			// Token: 0x06005460 RID: 21600 RVA: 0x0016AC3D File Offset: 0x00168E3D
			protected override void DefineContainerDefinitions()
			{
				base.ConstructContainerDefinition(typeof(Dictionary<ItemObject, CraftingCampaignBehavior.CraftedItemInitializationData>));
				base.ConstructContainerDefinition(typeof(Dictionary<Hero, CraftingCampaignBehavior.HeroCraftingRecord>));
				base.ConstructContainerDefinition(typeof(Dictionary<Town, CraftingCampaignBehavior.CraftingOrderSlots>));
			}
		}

		// Token: 0x020006CC RID: 1740
		internal class CraftedItemInitializationData
		{
			// Token: 0x06005461 RID: 21601 RVA: 0x0016AC6F File Offset: 0x00168E6F
			public CraftedItemInitializationData(WeaponDesign craftedData, TextObject itemName, CultureObject culture)
			{
				this.CraftedData = craftedData;
				this.ItemName = itemName;
				this.Culture = culture;
			}

			// Token: 0x06005462 RID: 21602 RVA: 0x0016AC8C File Offset: 0x00168E8C
			internal static void AutoGeneratedStaticCollectObjectsCraftedItemInitializationData(object o, List<object> collectedObjects)
			{
				((CraftingCampaignBehavior.CraftedItemInitializationData)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			// Token: 0x06005463 RID: 21603 RVA: 0x0016AC9A File Offset: 0x00168E9A
			protected virtual void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				collectedObjects.Add(this.CraftedData);
				collectedObjects.Add(this.ItemName);
				collectedObjects.Add(this.Culture);
			}

			// Token: 0x06005464 RID: 21604 RVA: 0x0016ACC0 File Offset: 0x00168EC0
			internal static object AutoGeneratedGetMemberValueCraftedData(object o)
			{
				return ((CraftingCampaignBehavior.CraftedItemInitializationData)o).CraftedData;
			}

			// Token: 0x06005465 RID: 21605 RVA: 0x0016ACCD File Offset: 0x00168ECD
			internal static object AutoGeneratedGetMemberValueItemName(object o)
			{
				return ((CraftingCampaignBehavior.CraftedItemInitializationData)o).ItemName;
			}

			// Token: 0x06005466 RID: 21606 RVA: 0x0016ACDA File Offset: 0x00168EDA
			internal static object AutoGeneratedGetMemberValueCulture(object o)
			{
				return ((CraftingCampaignBehavior.CraftedItemInitializationData)o).Culture;
			}

			// Token: 0x04001BFD RID: 7165
			[SaveableField(10)]
			public readonly WeaponDesign CraftedData;

			// Token: 0x04001BFE RID: 7166
			[SaveableField(20)]
			public readonly TextObject ItemName;

			// Token: 0x04001BFF RID: 7167
			[SaveableField(30)]
			public readonly CultureObject Culture;
		}

		// Token: 0x020006CD RID: 1741
		internal class HeroCraftingRecord
		{
			// Token: 0x06005467 RID: 21607 RVA: 0x0016ACE7 File Offset: 0x00168EE7
			public HeroCraftingRecord(int maxStamina)
			{
				this.CraftingStamina = maxStamina;
			}

			// Token: 0x06005468 RID: 21608 RVA: 0x0016ACF6 File Offset: 0x00168EF6
			internal static void AutoGeneratedStaticCollectObjectsHeroCraftingRecord(object o, List<object> collectedObjects)
			{
				((CraftingCampaignBehavior.HeroCraftingRecord)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			// Token: 0x06005469 RID: 21609 RVA: 0x0016AD04 File Offset: 0x00168F04
			protected virtual void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
			}

			// Token: 0x0600546A RID: 21610 RVA: 0x0016AD06 File Offset: 0x00168F06
			internal static object AutoGeneratedGetMemberValueCraftingStamina(object o)
			{
				return ((CraftingCampaignBehavior.HeroCraftingRecord)o).CraftingStamina;
			}

			// Token: 0x04001C00 RID: 7168
			[SaveableField(10)]
			public int CraftingStamina;
		}

		// Token: 0x020006CE RID: 1742
		public class CraftingOrderSlots
		{
			// Token: 0x0600546B RID: 21611 RVA: 0x0016AD18 File Offset: 0x00168F18
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

			// Token: 0x0600546C RID: 21612 RVA: 0x0016AD68 File Offset: 0x00168F68
			[LoadInitializationCallback]
			private void OnLoad(MetaData metaData)
			{
				this.QuestOrders = this._questOrders.GetReadOnlyDictionary<string, List<CraftingOrder>>();
			}

			// Token: 0x0600546D RID: 21613 RVA: 0x0016AD7C File Offset: 0x00168F7C
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

			// Token: 0x0600546E RID: 21614 RVA: 0x0016ADA4 File Offset: 0x00168FA4
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

			// Token: 0x0600546F RID: 21615 RVA: 0x0016ADCA File Offset: 0x00168FCA
			internal void AddTownOrder(CraftingOrder craftingOrder)
			{
				this.Slots[craftingOrder.DifficultyLevel] = craftingOrder;
			}

			// Token: 0x06005470 RID: 21616 RVA: 0x0016ADDA File Offset: 0x00168FDA
			internal void RemoveTownOrder(CraftingOrder craftingOrder)
			{
				this.Slots[craftingOrder.DifficultyLevel] = null;
			}

			// Token: 0x06005471 RID: 21617 RVA: 0x0016ADEA File Offset: 0x00168FEA
			internal void AddQuestOrder(string questId, CraftingOrder order)
			{
				if (!this._questOrders.ContainsKey(questId))
				{
					this._questOrders.Add(questId, new List<CraftingOrder>());
				}
				this._questOrders[questId].Add(order);
			}

			// Token: 0x06005472 RID: 21618 RVA: 0x0016AE1D File Offset: 0x0016901D
			internal void RemoveQuestOrder(string questId, CraftingOrder order)
			{
				this._questOrders[questId].Remove(order);
			}

			// Token: 0x06005473 RID: 21619 RVA: 0x0016AE32 File Offset: 0x00169032
			internal void RemoveAllQuestOrders(string questId)
			{
				this._questOrders.Remove(questId);
			}

			// Token: 0x06005474 RID: 21620 RVA: 0x0016AE41 File Offset: 0x00169041
			internal static void AutoGeneratedStaticCollectObjectsCraftingOrderSlots(object o, List<object> collectedObjects)
			{
				((CraftingCampaignBehavior.CraftingOrderSlots)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			// Token: 0x06005475 RID: 21621 RVA: 0x0016AE4F File Offset: 0x0016904F
			protected virtual void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				collectedObjects.Add(this.Slots);
				collectedObjects.Add(this._questOrders);
			}

			// Token: 0x06005476 RID: 21622 RVA: 0x0016AE69 File Offset: 0x00169069
			internal static object AutoGeneratedGetMemberValueSlots(object o)
			{
				return ((CraftingCampaignBehavior.CraftingOrderSlots)o).Slots;
			}

			// Token: 0x06005477 RID: 21623 RVA: 0x0016AE76 File Offset: 0x00169076
			internal static object AutoGeneratedGetMemberValue_questOrders(object o)
			{
				return ((CraftingCampaignBehavior.CraftingOrderSlots)o)._questOrders;
			}

			// Token: 0x04001C01 RID: 7169
			private const int SlotCount = 6;

			// Token: 0x04001C02 RID: 7170
			[SaveableField(10)]
			public CraftingOrder[] Slots;

			// Token: 0x04001C03 RID: 7171
			[SaveableField(20)]
			private readonly Dictionary<string, List<CraftingOrder>> _questOrders;

			// Token: 0x04001C04 RID: 7172
			public MBReadOnlyDictionary<string, List<CraftingOrder>> QuestOrders;
		}
	}
}
