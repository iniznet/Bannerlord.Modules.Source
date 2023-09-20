using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Issues;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;
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

		public bool IsPartyNotificationActive { get; private set; }

		public string GetPartyNotificationText()
		{
			this._recruitNotificationText.SetTextVariable("NUMBER", this._numOfRecruitablePrisoners);
			return this._recruitNotificationText.ToString();
		}

		public void ClearPartyNotification()
		{
			this.IsPartyNotificationActive = false;
			this._numOfRecruitablePrisoners = 0;
		}

		public void UpdatePartyNotification()
		{
			this.UpdatePrisonerRecruitValue();
		}

		private void UpdatePrisonerRecruitValue()
		{
			Dictionary<CharacterObject, int> dictionary = new Dictionary<CharacterObject, int>();
			foreach (TroopRosterElement troopRosterElement in MobileParty.MainParty.PrisonRoster.GetTroopRoster())
			{
				int num = Campaign.Current.Models.PrisonerRecruitmentCalculationModel.CalculateRecruitableNumber(PartyBase.MainParty, troopRosterElement.Character);
				int num2;
				if (this._examinedPrisonerCharacterList.TryGetValue(troopRosterElement.Character, out num2))
				{
					if (num2 != num)
					{
						this._examinedPrisonerCharacterList[troopRosterElement.Character] = num;
						if (num2 < num)
						{
							this.IsPartyNotificationActive = true;
							this._numOfRecruitablePrisoners += num - num2;
						}
					}
				}
				else
				{
					this._examinedPrisonerCharacterList.Add(troopRosterElement.Character, num);
					if (num > 0)
					{
						this.IsPartyNotificationActive = true;
						this._numOfRecruitablePrisoners += num;
					}
				}
				dictionary.Add(troopRosterElement.Character, num);
			}
			this._examinedPrisonerCharacterList = dictionary;
		}

		public bool IsQuestNotificationActive
		{
			get
			{
				return this._unExaminedQuestLogs.Count > 0;
			}
		}

		public List<JournalLog> UnExaminedQuestLogs
		{
			get
			{
				return this._unExaminedQuestLogs;
			}
		}

		public string GetQuestNotificationText()
		{
			this._questNotificationText.SetTextVariable("NUMBER", this._unExaminedQuestLogs.Count);
			return this._questNotificationText.ToString();
		}

		public void OnQuestLogExamined(JournalLog log)
		{
			if (this._unExaminedQuestLogs.Contains(log))
			{
				this._unExaminedQuestLogs.Remove(log);
			}
		}

		private void OnQuestLogAdded(QuestBase obj, bool hideInformation)
		{
			this._unExaminedQuestLogs.Add(obj.JournalEntries[obj.JournalEntries.Count - 1]);
		}

		private void OnIssueLogAdded(IssueBase obj, bool hideInformation)
		{
			this._unExaminedQuestLogs.Add(obj.JournalEntries[obj.JournalEntries.Count - 1]);
		}

		public List<Army> UnExaminedArmies
		{
			get
			{
				return this._unExaminedArmies;
			}
		}

		public int NumOfKingdomArmyNotifications
		{
			get
			{
				return this.UnExaminedArmies.Count;
			}
		}

		public void OnArmyExamined(Army army)
		{
			this._unExaminedArmies.Remove(army);
		}

		private void OnArmyDispersed(Army arg1, Army.ArmyDispersionReason arg2, bool isPlayersArmy)
		{
			Army army;
			if (isPlayersArmy && (army = this._unExaminedArmies.SingleOrDefault((Army a) => a == arg1)) != null)
			{
				this._unExaminedArmies.Remove(army);
			}
		}

		private void OnNewArmyCreated(Army army)
		{
			if (army.Kingdom == Hero.MainHero.MapFaction && army.LeaderParty != MobileParty.MainParty)
			{
				this._unExaminedArmies.Add(army);
			}
		}

		public bool IsCharacterNotificationActive
		{
			get
			{
				return this._isCharacterNotificationActive;
			}
		}

		public void ClearCharacterNotification()
		{
			this._isCharacterNotificationActive = false;
			this._numOfPerks = 0;
		}

		public string GetCharacterNotificationText()
		{
			this._characterNotificationText.SetTextVariable("NUMBER", this._numOfPerks);
			return this._characterNotificationText.ToString();
		}

		private void OnHeroGainedSkill(Hero hero, SkillObject skill, int change = 1, bool shouldNotify = true)
		{
			if ((hero == Hero.MainHero || hero.Clan == Clan.PlayerClan) && !hero.HeroDeveloper.GetOneAvailablePerkForEachPerkPair().IsEmpty<PerkObject>())
			{
				this._isCharacterNotificationActive = true;
				this._numOfPerks++;
			}
		}

		private void OnHeroLevelledUp(Hero hero, bool shouldNotify)
		{
			if (hero == Hero.MainHero)
			{
				this._isCharacterNotificationActive = true;
			}
		}

		private void OnGameLoaded(CampaignGameStarter campaignGameStarter)
		{
			this.UpdatePartyNotification();
			this.UpdatePrisonerRecruitValue();
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
			CampaignEvents.HeroGainedSkill.AddNonSerializedListener(this, new Action<Hero, SkillObject, int, bool>(this.OnHeroGainedSkill));
			CampaignEvents.HeroLevelledUp.AddNonSerializedListener(this, new Action<Hero, bool>(this.OnHeroLevelledUp));
			CampaignEvents.ArmyCreated.AddNonSerializedListener(this, new Action<Army>(this.OnNewArmyCreated));
			CampaignEvents.ArmyDispersed.AddNonSerializedListener(this, new Action<Army, Army.ArmyDispersionReason, bool>(this.OnArmyDispersed));
			CampaignEvents.QuestLogAddedEvent.AddNonSerializedListener(this, new Action<QuestBase, bool>(this.OnQuestLogAdded));
			CampaignEvents.IssueLogAddedEvent.AddNonSerializedListener(this, new Action<IssueBase, bool>(this.OnIssueLogAdded));
			CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnGameLoaded));
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
			dataStore.SyncData<List<JournalLog>>("_unExaminedQuestLogs", ref this._unExaminedQuestLogs);
			dataStore.SyncData<List<Army>>("_unExaminedArmies", ref this._unExaminedArmies);
			dataStore.SyncData<bool>("_isCharacterNotificationActive", ref this._isCharacterNotificationActive);
			dataStore.SyncData<int>("_numOfPerks", ref this._numOfPerks);
			dataStore.SyncData<Dictionary<CharacterObject, int>>("_examinedPrisonerCharacterList", ref this._examinedPrisonerCharacterList);
		}

		private readonly TextObject _characterNotificationText = new TextObject("{=rlqjkZ9Q}You have {NUMBER} new perks available for selection.", null);

		private readonly TextObject _questNotificationText = new TextObject("{=FAIYN0vN}You have {NUMBER} new updates to your quests.", null);

		private readonly TextObject _recruitNotificationText = new TextObject("{=PJMbfSPJ}You have {NUMBER} new prisoners to recruit.", null);

		private Dictionary<CharacterObject, int> _examinedPrisonerCharacterList = new Dictionary<CharacterObject, int>();

		private int _numOfRecruitablePrisoners;

		private List<JournalLog> _unExaminedQuestLogs = new List<JournalLog>();

		private List<Army> _unExaminedArmies = new List<Army>();

		private bool _isCharacterNotificationActive;

		private int _numOfPerks;

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
