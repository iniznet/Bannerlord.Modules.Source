using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	// Token: 0x020003DE RID: 990
	public class ViewDataTrackerCampaignBehavior : CampaignBehaviorBase, IViewDataTracker
	{
		// Token: 0x06003BE6 RID: 15334 RVA: 0x0011C120 File Offset: 0x0011A320
		public ViewDataTrackerCampaignBehavior()
		{
			this._inventoryItemLocks = new List<string>();
			this._partyPrisonerLocks = new List<string>();
			this._partyTroopLocks = new List<string>();
			this._encyclopediaBookmarkedClans = new List<Clan>();
			this._encyclopediaBookmarkedConcepts = new List<Concept>();
			this._encyclopediaBookmarkedHeroes = new List<Hero>();
			this._encyclopediaBookmarkedKingdoms = new List<Kingdom>();
			this._encyclopediaBookmarkedSettlements = new List<Settlement>();
			this._encyclopediaBookmarkedUnits = new List<CharacterObject>();
			this._inventorySortPreferences = new Dictionary<int, Tuple<int, int>>();
		}

		// Token: 0x06003BE7 RID: 15335 RVA: 0x0011C1A1 File Offset: 0x0011A3A1
		public bool GetMapBarExtendedState()
		{
			return this._isMapBarExtended;
		}

		// Token: 0x06003BE8 RID: 15336 RVA: 0x0011C1A9 File Offset: 0x0011A3A9
		public void SetMapBarExtendedState(bool isExtended)
		{
			this._isMapBarExtended = isExtended;
		}

		// Token: 0x06003BE9 RID: 15337 RVA: 0x0011C1B2 File Offset: 0x0011A3B2
		public void SetInventoryLocks(IEnumerable<string> locks)
		{
			this._inventoryItemLocks = locks.ToList<string>();
		}

		// Token: 0x06003BEA RID: 15338 RVA: 0x0011C1C0 File Offset: 0x0011A3C0
		public IEnumerable<string> GetInventoryLocks()
		{
			return this._inventoryItemLocks;
		}

		// Token: 0x06003BEB RID: 15339 RVA: 0x0011C1C8 File Offset: 0x0011A3C8
		public void InventorySetSortPreference(int inventoryMode, int sortOption, int sortState)
		{
			this._inventorySortPreferences[inventoryMode] = new Tuple<int, int>(sortOption, sortState);
		}

		// Token: 0x06003BEC RID: 15340 RVA: 0x0011C1E0 File Offset: 0x0011A3E0
		public Tuple<int, int> InventoryGetSortPreference(int inventoryMode)
		{
			Tuple<int, int> tuple;
			if (this._inventorySortPreferences.TryGetValue(inventoryMode, out tuple))
			{
				return tuple;
			}
			return new Tuple<int, int>(0, 0);
		}

		// Token: 0x06003BED RID: 15341 RVA: 0x0011C206 File Offset: 0x0011A406
		public void SetPartyTroopLocks(IEnumerable<string> locks)
		{
			this._partyTroopLocks = locks.ToList<string>();
		}

		// Token: 0x06003BEE RID: 15342 RVA: 0x0011C214 File Offset: 0x0011A414
		public void SetPartyPrisonerLocks(IEnumerable<string> locks)
		{
			this._partyPrisonerLocks = locks.ToList<string>();
		}

		// Token: 0x06003BEF RID: 15343 RVA: 0x0011C222 File Offset: 0x0011A422
		public void SetPartySortType(int sortType)
		{
			this._partySortType = sortType;
		}

		// Token: 0x06003BF0 RID: 15344 RVA: 0x0011C22B File Offset: 0x0011A42B
		public void SetIsPartySortAscending(bool isAscending)
		{
			this._isPartySortAscending = isAscending;
		}

		// Token: 0x06003BF1 RID: 15345 RVA: 0x0011C234 File Offset: 0x0011A434
		public IEnumerable<string> GetPartyTroopLocks()
		{
			return this._partyTroopLocks;
		}

		// Token: 0x06003BF2 RID: 15346 RVA: 0x0011C23C File Offset: 0x0011A43C
		public IEnumerable<string> GetPartyPrisonerLocks()
		{
			return this._partyPrisonerLocks;
		}

		// Token: 0x06003BF3 RID: 15347 RVA: 0x0011C244 File Offset: 0x0011A444
		public int GetPartySortType()
		{
			return this._partySortType;
		}

		// Token: 0x06003BF4 RID: 15348 RVA: 0x0011C24C File Offset: 0x0011A44C
		public bool GetIsPartySortAscending()
		{
			return this._isPartySortAscending;
		}

		// Token: 0x06003BF5 RID: 15349 RVA: 0x0011C254 File Offset: 0x0011A454
		public void AddEncyclopediaBookmarkToItem(Hero item)
		{
			this._encyclopediaBookmarkedHeroes.Add(item);
		}

		// Token: 0x06003BF6 RID: 15350 RVA: 0x0011C262 File Offset: 0x0011A462
		public void AddEncyclopediaBookmarkToItem(Clan clan)
		{
			this._encyclopediaBookmarkedClans.Add(clan);
		}

		// Token: 0x06003BF7 RID: 15351 RVA: 0x0011C270 File Offset: 0x0011A470
		public void AddEncyclopediaBookmarkToItem(Concept concept)
		{
			this._encyclopediaBookmarkedConcepts.Add(concept);
		}

		// Token: 0x06003BF8 RID: 15352 RVA: 0x0011C27E File Offset: 0x0011A47E
		public void AddEncyclopediaBookmarkToItem(Kingdom kingdom)
		{
			this._encyclopediaBookmarkedKingdoms.Add(kingdom);
		}

		// Token: 0x06003BF9 RID: 15353 RVA: 0x0011C28C File Offset: 0x0011A48C
		public void AddEncyclopediaBookmarkToItem(Settlement settlement)
		{
			this._encyclopediaBookmarkedSettlements.Add(settlement);
		}

		// Token: 0x06003BFA RID: 15354 RVA: 0x0011C29A File Offset: 0x0011A49A
		public void AddEncyclopediaBookmarkToItem(CharacterObject unit)
		{
			this._encyclopediaBookmarkedUnits.Add(unit);
		}

		// Token: 0x06003BFB RID: 15355 RVA: 0x0011C2A8 File Offset: 0x0011A4A8
		public void RemoveEncyclopediaBookmarkFromItem(Hero hero)
		{
			this._encyclopediaBookmarkedHeroes.Remove(hero);
		}

		// Token: 0x06003BFC RID: 15356 RVA: 0x0011C2B7 File Offset: 0x0011A4B7
		public void RemoveEncyclopediaBookmarkFromItem(Clan clan)
		{
			this._encyclopediaBookmarkedClans.Remove(clan);
		}

		// Token: 0x06003BFD RID: 15357 RVA: 0x0011C2C6 File Offset: 0x0011A4C6
		public void RemoveEncyclopediaBookmarkFromItem(Concept concept)
		{
			this._encyclopediaBookmarkedConcepts.Remove(concept);
		}

		// Token: 0x06003BFE RID: 15358 RVA: 0x0011C2D5 File Offset: 0x0011A4D5
		public void RemoveEncyclopediaBookmarkFromItem(Kingdom kingdom)
		{
			this._encyclopediaBookmarkedKingdoms.Remove(kingdom);
		}

		// Token: 0x06003BFF RID: 15359 RVA: 0x0011C2E4 File Offset: 0x0011A4E4
		public void RemoveEncyclopediaBookmarkFromItem(Settlement settlement)
		{
			this._encyclopediaBookmarkedSettlements.Remove(settlement);
		}

		// Token: 0x06003C00 RID: 15360 RVA: 0x0011C2F3 File Offset: 0x0011A4F3
		public void RemoveEncyclopediaBookmarkFromItem(CharacterObject unit)
		{
			this._encyclopediaBookmarkedUnits.Remove(unit);
		}

		// Token: 0x06003C01 RID: 15361 RVA: 0x0011C302 File Offset: 0x0011A502
		public bool IsEncyclopediaBookmarked(Hero hero)
		{
			return this._encyclopediaBookmarkedHeroes.Contains(hero);
		}

		// Token: 0x06003C02 RID: 15362 RVA: 0x0011C310 File Offset: 0x0011A510
		public bool IsEncyclopediaBookmarked(Clan clan)
		{
			return this._encyclopediaBookmarkedClans.Contains(clan);
		}

		// Token: 0x06003C03 RID: 15363 RVA: 0x0011C31E File Offset: 0x0011A51E
		public bool IsEncyclopediaBookmarked(Concept concept)
		{
			return this._encyclopediaBookmarkedConcepts.Contains(concept);
		}

		// Token: 0x06003C04 RID: 15364 RVA: 0x0011C32C File Offset: 0x0011A52C
		public bool IsEncyclopediaBookmarked(Kingdom kingdom)
		{
			return this._encyclopediaBookmarkedKingdoms.Contains(kingdom);
		}

		// Token: 0x06003C05 RID: 15365 RVA: 0x0011C33A File Offset: 0x0011A53A
		public bool IsEncyclopediaBookmarked(Settlement settlement)
		{
			return this._encyclopediaBookmarkedSettlements.Contains(settlement);
		}

		// Token: 0x06003C06 RID: 15366 RVA: 0x0011C348 File Offset: 0x0011A548
		public bool IsEncyclopediaBookmarked(CharacterObject unit)
		{
			return this._encyclopediaBookmarkedUnits.Contains(unit);
		}

		// Token: 0x06003C07 RID: 15367 RVA: 0x0011C356 File Offset: 0x0011A556
		public void SetQuestSelection(QuestBase selection)
		{
			this._questSelection = selection;
		}

		// Token: 0x06003C08 RID: 15368 RVA: 0x0011C35F File Offset: 0x0011A55F
		public QuestBase GetQuestSelection()
		{
			return this._questSelection;
		}

		// Token: 0x06003C09 RID: 15369 RVA: 0x0011C367 File Offset: 0x0011A567
		public override void RegisterEvents()
		{
		}

		// Token: 0x06003C0A RID: 15370 RVA: 0x0011C369 File Offset: 0x0011A569
		public void SetQuestSortTypeSelection(int questSortTypeSelection)
		{
			this._questSortTypeSelection = questSortTypeSelection;
		}

		// Token: 0x06003C0B RID: 15371 RVA: 0x0011C372 File Offset: 0x0011A572
		public int GetQuestSortTypeSelection()
		{
			return this._questSortTypeSelection;
		}

		// Token: 0x06003C0C RID: 15372 RVA: 0x0011C37C File Offset: 0x0011A57C
		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<bool>("_isMapBarExtended", ref this._isMapBarExtended);
			dataStore.SyncData<List<string>>("_inventoryItemLocks", ref this._inventoryItemLocks);
			dataStore.SyncData<Dictionary<int, Tuple<int, int>>>("_inventorySortPreferences", ref this._inventorySortPreferences);
			dataStore.SyncData<int>("_partySortType", ref this._partySortType);
			dataStore.SyncData<bool>("_isPartySortAscending", ref this._isPartySortAscending);
			dataStore.SyncData<List<string>>("_partyTroopLocks", ref this._partyTroopLocks);
			dataStore.SyncData<List<string>>("_partyPrisonerLocks", ref this._partyPrisonerLocks);
			dataStore.SyncData<List<Hero>>("_encyclopediaBookmarkedHeroes", ref this._encyclopediaBookmarkedHeroes);
			dataStore.SyncData<List<Clan>>("_encyclopediaBookmarkedClans", ref this._encyclopediaBookmarkedClans);
			dataStore.SyncData<List<Concept>>("_encyclopediaBookmarkedConcepts", ref this._encyclopediaBookmarkedConcepts);
			dataStore.SyncData<List<Kingdom>>("_encyclopediaBookmarkedKingdoms", ref this._encyclopediaBookmarkedKingdoms);
			dataStore.SyncData<List<Settlement>>("_encyclopediaBookmarkedSettlements", ref this._encyclopediaBookmarkedSettlements);
			dataStore.SyncData<List<CharacterObject>>("_encyclopediaBookmarkedUnits", ref this._encyclopediaBookmarkedUnits);
			dataStore.SyncData<QuestBase>("_questSelection", ref this._questSelection);
		}

		// Token: 0x0400123B RID: 4667
		private bool _isMapBarExtended;

		// Token: 0x0400123C RID: 4668
		private List<string> _inventoryItemLocks;

		// Token: 0x0400123D RID: 4669
		[SaveableField(21)]
		private Dictionary<int, Tuple<int, int>> _inventorySortPreferences;

		// Token: 0x0400123E RID: 4670
		private int _partySortType;

		// Token: 0x0400123F RID: 4671
		private bool _isPartySortAscending;

		// Token: 0x04001240 RID: 4672
		private List<string> _partyTroopLocks;

		// Token: 0x04001241 RID: 4673
		private List<string> _partyPrisonerLocks;

		// Token: 0x04001242 RID: 4674
		private List<Hero> _encyclopediaBookmarkedHeroes;

		// Token: 0x04001243 RID: 4675
		private List<Clan> _encyclopediaBookmarkedClans;

		// Token: 0x04001244 RID: 4676
		private List<Concept> _encyclopediaBookmarkedConcepts;

		// Token: 0x04001245 RID: 4677
		private List<Kingdom> _encyclopediaBookmarkedKingdoms;

		// Token: 0x04001246 RID: 4678
		private List<Settlement> _encyclopediaBookmarkedSettlements;

		// Token: 0x04001247 RID: 4679
		private List<CharacterObject> _encyclopediaBookmarkedUnits;

		// Token: 0x04001248 RID: 4680
		private QuestBase _questSelection;

		// Token: 0x04001249 RID: 4681
		[SaveableField(51)]
		private int _questSortTypeSelection;
	}
}
