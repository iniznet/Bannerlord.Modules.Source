using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	public class ViewDataTrackerCampaignBehavior : CampaignBehaviorBase, IViewDataTracker
	{
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

		public bool GetMapBarExtendedState()
		{
			return this._isMapBarExtended;
		}

		public void SetMapBarExtendedState(bool isExtended)
		{
			this._isMapBarExtended = isExtended;
		}

		public void SetInventoryLocks(IEnumerable<string> locks)
		{
			this._inventoryItemLocks = locks.ToList<string>();
		}

		public IEnumerable<string> GetInventoryLocks()
		{
			return this._inventoryItemLocks;
		}

		public void InventorySetSortPreference(int inventoryMode, int sortOption, int sortState)
		{
			this._inventorySortPreferences[inventoryMode] = new Tuple<int, int>(sortOption, sortState);
		}

		public Tuple<int, int> InventoryGetSortPreference(int inventoryMode)
		{
			Tuple<int, int> tuple;
			if (this._inventorySortPreferences.TryGetValue(inventoryMode, out tuple))
			{
				return tuple;
			}
			return new Tuple<int, int>(0, 0);
		}

		public void SetPartyTroopLocks(IEnumerable<string> locks)
		{
			this._partyTroopLocks = locks.ToList<string>();
		}

		public void SetPartyPrisonerLocks(IEnumerable<string> locks)
		{
			this._partyPrisonerLocks = locks.ToList<string>();
		}

		public void SetPartySortType(int sortType)
		{
			this._partySortType = sortType;
		}

		public void SetIsPartySortAscending(bool isAscending)
		{
			this._isPartySortAscending = isAscending;
		}

		public IEnumerable<string> GetPartyTroopLocks()
		{
			return this._partyTroopLocks;
		}

		public IEnumerable<string> GetPartyPrisonerLocks()
		{
			return this._partyPrisonerLocks;
		}

		public int GetPartySortType()
		{
			return this._partySortType;
		}

		public bool GetIsPartySortAscending()
		{
			return this._isPartySortAscending;
		}

		public void AddEncyclopediaBookmarkToItem(Hero item)
		{
			this._encyclopediaBookmarkedHeroes.Add(item);
		}

		public void AddEncyclopediaBookmarkToItem(Clan clan)
		{
			this._encyclopediaBookmarkedClans.Add(clan);
		}

		public void AddEncyclopediaBookmarkToItem(Concept concept)
		{
			this._encyclopediaBookmarkedConcepts.Add(concept);
		}

		public void AddEncyclopediaBookmarkToItem(Kingdom kingdom)
		{
			this._encyclopediaBookmarkedKingdoms.Add(kingdom);
		}

		public void AddEncyclopediaBookmarkToItem(Settlement settlement)
		{
			this._encyclopediaBookmarkedSettlements.Add(settlement);
		}

		public void AddEncyclopediaBookmarkToItem(CharacterObject unit)
		{
			this._encyclopediaBookmarkedUnits.Add(unit);
		}

		public void RemoveEncyclopediaBookmarkFromItem(Hero hero)
		{
			this._encyclopediaBookmarkedHeroes.Remove(hero);
		}

		public void RemoveEncyclopediaBookmarkFromItem(Clan clan)
		{
			this._encyclopediaBookmarkedClans.Remove(clan);
		}

		public void RemoveEncyclopediaBookmarkFromItem(Concept concept)
		{
			this._encyclopediaBookmarkedConcepts.Remove(concept);
		}

		public void RemoveEncyclopediaBookmarkFromItem(Kingdom kingdom)
		{
			this._encyclopediaBookmarkedKingdoms.Remove(kingdom);
		}

		public void RemoveEncyclopediaBookmarkFromItem(Settlement settlement)
		{
			this._encyclopediaBookmarkedSettlements.Remove(settlement);
		}

		public void RemoveEncyclopediaBookmarkFromItem(CharacterObject unit)
		{
			this._encyclopediaBookmarkedUnits.Remove(unit);
		}

		public bool IsEncyclopediaBookmarked(Hero hero)
		{
			return this._encyclopediaBookmarkedHeroes.Contains(hero);
		}

		public bool IsEncyclopediaBookmarked(Clan clan)
		{
			return this._encyclopediaBookmarkedClans.Contains(clan);
		}

		public bool IsEncyclopediaBookmarked(Concept concept)
		{
			return this._encyclopediaBookmarkedConcepts.Contains(concept);
		}

		public bool IsEncyclopediaBookmarked(Kingdom kingdom)
		{
			return this._encyclopediaBookmarkedKingdoms.Contains(kingdom);
		}

		public bool IsEncyclopediaBookmarked(Settlement settlement)
		{
			return this._encyclopediaBookmarkedSettlements.Contains(settlement);
		}

		public bool IsEncyclopediaBookmarked(CharacterObject unit)
		{
			return this._encyclopediaBookmarkedUnits.Contains(unit);
		}

		public void SetQuestSelection(QuestBase selection)
		{
			this._questSelection = selection;
		}

		public QuestBase GetQuestSelection()
		{
			return this._questSelection;
		}

		public override void RegisterEvents()
		{
		}

		public void SetQuestSortTypeSelection(int questSortTypeSelection)
		{
			this._questSortTypeSelection = questSortTypeSelection;
		}

		public int GetQuestSortTypeSelection()
		{
			return this._questSortTypeSelection;
		}

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

		private bool _isMapBarExtended;

		private List<string> _inventoryItemLocks;

		[SaveableField(21)]
		private Dictionary<int, Tuple<int, int>> _inventorySortPreferences;

		private int _partySortType;

		private bool _isPartySortAscending;

		private List<string> _partyTroopLocks;

		private List<string> _partyPrisonerLocks;

		private List<Hero> _encyclopediaBookmarkedHeroes;

		private List<Clan> _encyclopediaBookmarkedClans;

		private List<Concept> _encyclopediaBookmarkedConcepts;

		private List<Kingdom> _encyclopediaBookmarkedKingdoms;

		private List<Settlement> _encyclopediaBookmarkedSettlements;

		private List<CharacterObject> _encyclopediaBookmarkedUnits;

		private QuestBase _questSelection;

		[SaveableField(51)]
		private int _questSortTypeSelection;
	}
}
