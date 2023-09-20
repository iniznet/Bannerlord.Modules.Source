using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Issues;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	public class PlayerUpdateTracker
	{
		public static PlayerUpdateTracker Current
		{
			get
			{
				return Campaign.Current.PlayerUpdateTracker;
			}
		}

		public bool IsKingdomNotificationActive
		{
			get
			{
				return false;
			}
		}

		public bool IsClanNotificationActive
		{
			get
			{
				return false;
			}
		}

		public PlayerUpdateTracker()
		{
			this.RegisterEvents();
		}

		private void RegisterEvents()
		{
			CampaignEvents.HeroGainedSkill.AddNonSerializedListener(this, new Action<Hero, SkillObject, int, bool>(this.OnHeroGainedSkill));
			CampaignEvents.HeroLevelledUp.AddNonSerializedListener(this, new Action<Hero, bool>(this.OnHeroLevelledUp));
			CampaignEvents.ArmyCreated.AddNonSerializedListener(this, new Action<Army>(this.OnNewArmyCreated));
			CampaignEvents.ArmyDispersed.AddNonSerializedListener(this, new Action<Army, Army.ArmyDispersionReason, bool>(this.OnArmyDispersed));
			CampaignEvents.QuestLogAddedEvent.AddNonSerializedListener(this, new Action<QuestBase, bool>(this.OnQuestLogAdded));
			CampaignEvents.IssueLogAddedEvent.AddNonSerializedListener(this, new Action<IssueBase, bool>(this.OnIssueLogAdded));
		}

		public bool IsPartyNotificationActive { get; private set; }

		private void OnPlayerBattleEndEvent(MapEvent obj)
		{
			bool isPlayerMapEvent = obj.IsPlayerMapEvent;
		}

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
			Dictionary<BasicCharacterObject, int> dictionary = new Dictionary<BasicCharacterObject, int>();
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

		public int GetNumOfCategoryItemPartyHas(ItemRoster items, ItemCategory itemCategory)
		{
			int num = 0;
			foreach (ItemRosterElement itemRosterElement in items)
			{
				if (itemRosterElement.EquipmentElement.Item.ItemCategory == itemCategory)
				{
					num += itemRosterElement.Amount;
				}
			}
			return num;
		}

		public bool IsQuestNotificationActive { get; private set; }

		public List<JournalLog> UnExaminedQuestLogs { get; } = new List<JournalLog>();

		public void ClearQuestNotification()
		{
			this.IsQuestNotificationActive = false;
		}

		public string GetQuestNotificationText()
		{
			this._questNotificationText.SetTextVariable("NUMBER", this.UnExaminedQuestLogs.Count);
			return this._questNotificationText.ToString();
		}

		public void OnQuestlogExamined(JournalLog log)
		{
			if (this.UnExaminedQuestLogs.Contains(log))
			{
				this.UnExaminedQuestLogs.Remove(log);
			}
		}

		private void OnQuestLogAdded(QuestBase obj, bool hideInformation)
		{
			this.UnExaminedQuestLogs.Add(obj.JournalEntries[obj.JournalEntries.Count - 1]);
			this.IsQuestNotificationActive = true;
		}

		private void OnIssueLogAdded(IssueBase obj, bool hideInformation)
		{
			this.UnExaminedQuestLogs.Add(obj.JournalEntries[obj.JournalEntries.Count - 1]);
			this.IsQuestNotificationActive = true;
		}

		public List<Army> UnExaminedArmies { get; } = new List<Army>();

		public int NumKingdomArmyNotifications
		{
			get
			{
				return this.UnExaminedArmies.Count;
			}
		}

		public void OnArmyExamined(Army army)
		{
			this.UnExaminedArmies.Remove(army);
		}

		private void OnArmyDispersed(Army arg1, Army.ArmyDispersionReason arg2, bool isPlayersArmy)
		{
			Army army;
			if (isPlayersArmy && (army = this.UnExaminedArmies.SingleOrDefault((Army a) => a == arg1)) != null)
			{
				this.UnExaminedArmies.Remove(army);
			}
		}

		private void OnNewArmyCreated(Army army)
		{
			if (army.Kingdom == Hero.MainHero.MapFaction && army.LeaderParty != MobileParty.MainParty)
			{
				this.UnExaminedArmies.Add(army);
			}
		}

		public bool IsCharacterNotificationActive { get; private set; }

		public void ClearCharacterNotification()
		{
			this.IsCharacterNotificationActive = false;
			this._numOfPerks = 0;
			this._numOfFocusPoints = 0;
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
				this.IsCharacterNotificationActive = true;
				this._numOfPerks++;
			}
		}

		private void OnHeroLevelledUp(Hero hero, bool shouldNotify)
		{
			if (hero == Hero.MainHero)
			{
				this.IsCharacterNotificationActive = true;
				this._numOfFocusPoints++;
			}
		}

		private TextObject _characterNotificationText = new TextObject("{=rlqjkZ9Q}You have {NUMBER} new perks available for selection.", null);

		private TextObject _questNotificationText = new TextObject("{=FAIYN0vN}You have {NUMBER} new updates to your quests.", null);

		private TextObject _recruitNotificationText = new TextObject("{=PJMbfSPJ}You have {NUMBER} new prisoners to recruit.", null);

		private TextObject _upgradeNotificationText = new TextObject("{=Wbm4XGB6}You have {NUMBER} new troops to upgrade.", null);

		private Dictionary<BasicCharacterObject, int> _examinedTroopCharacterList = new Dictionary<BasicCharacterObject, int>();

		private Dictionary<BasicCharacterObject, int> _examinedPrisonerCharacterList = new Dictionary<BasicCharacterObject, int>();

		private int _numOfRecruitablePrisoners;

		private int _numOfPerks;

		private int _numOfFocusPoints;
	}
}
