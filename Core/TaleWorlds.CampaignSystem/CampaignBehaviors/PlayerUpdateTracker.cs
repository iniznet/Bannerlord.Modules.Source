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
	// Token: 0x020003C2 RID: 962
	public class PlayerUpdateTracker
	{
		// Token: 0x17000CDE RID: 3294
		// (get) Token: 0x060039A4 RID: 14756 RVA: 0x00109281 File Offset: 0x00107481
		public static PlayerUpdateTracker Current
		{
			get
			{
				return Campaign.Current.PlayerUpdateTracker;
			}
		}

		// Token: 0x17000CDF RID: 3295
		// (get) Token: 0x060039A5 RID: 14757 RVA: 0x0010928D File Offset: 0x0010748D
		public bool IsKingdomNotificationActive
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000CE0 RID: 3296
		// (get) Token: 0x060039A6 RID: 14758 RVA: 0x00109290 File Offset: 0x00107490
		public bool IsClanNotificationActive
		{
			get
			{
				return false;
			}
		}

		// Token: 0x060039A7 RID: 14759 RVA: 0x00109294 File Offset: 0x00107494
		public PlayerUpdateTracker()
		{
			this.RegisterEvents();
		}

		// Token: 0x060039A8 RID: 14760 RVA: 0x00109320 File Offset: 0x00107520
		private void RegisterEvents()
		{
			CampaignEvents.HeroGainedSkill.AddNonSerializedListener(this, new Action<Hero, SkillObject, int, bool>(this.OnHeroGainedSkill));
			CampaignEvents.HeroLevelledUp.AddNonSerializedListener(this, new Action<Hero, bool>(this.OnHeroLevelledUp));
			CampaignEvents.ArmyCreated.AddNonSerializedListener(this, new Action<Army>(this.OnNewArmyCreated));
			CampaignEvents.ArmyDispersed.AddNonSerializedListener(this, new Action<Army, Army.ArmyDispersionReason, bool>(this.OnArmyDispersed));
			CampaignEvents.QuestLogAddedEvent.AddNonSerializedListener(this, new Action<QuestBase, bool>(this.OnQuestLogAdded));
			CampaignEvents.IssueLogAddedEvent.AddNonSerializedListener(this, new Action<IssueBase, bool>(this.OnIssueLogAdded));
		}

		// Token: 0x17000CE1 RID: 3297
		// (get) Token: 0x060039A9 RID: 14761 RVA: 0x001093B7 File Offset: 0x001075B7
		// (set) Token: 0x060039AA RID: 14762 RVA: 0x001093BF File Offset: 0x001075BF
		public bool IsPartyNotificationActive { get; private set; }

		// Token: 0x060039AB RID: 14763 RVA: 0x001093C8 File Offset: 0x001075C8
		private void OnPlayerBattleEndEvent(MapEvent obj)
		{
			bool isPlayerMapEvent = obj.IsPlayerMapEvent;
		}

		// Token: 0x060039AC RID: 14764 RVA: 0x001093D1 File Offset: 0x001075D1
		public string GetPartyNotificationText()
		{
			this._recruitNotificationText.SetTextVariable("NUMBER", this._numOfRecruitablePrisoners);
			return this._recruitNotificationText.ToString();
		}

		// Token: 0x060039AD RID: 14765 RVA: 0x001093F5 File Offset: 0x001075F5
		public void ClearPartyNotification()
		{
			this.IsPartyNotificationActive = false;
			this._numOfRecruitablePrisoners = 0;
		}

		// Token: 0x060039AE RID: 14766 RVA: 0x00109405 File Offset: 0x00107605
		public void UpdatePartyNotification()
		{
			this.UpdatePrisonerRecruitValue();
		}

		// Token: 0x060039AF RID: 14767 RVA: 0x00109410 File Offset: 0x00107610
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

		// Token: 0x060039B0 RID: 14768 RVA: 0x0010951C File Offset: 0x0010771C
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

		// Token: 0x17000CE2 RID: 3298
		// (get) Token: 0x060039B1 RID: 14769 RVA: 0x00109584 File Offset: 0x00107784
		// (set) Token: 0x060039B2 RID: 14770 RVA: 0x0010958C File Offset: 0x0010778C
		public bool IsQuestNotificationActive { get; private set; }

		// Token: 0x17000CE3 RID: 3299
		// (get) Token: 0x060039B3 RID: 14771 RVA: 0x00109595 File Offset: 0x00107795
		public List<JournalLog> UnExaminedQuestLogs { get; } = new List<JournalLog>();

		// Token: 0x060039B4 RID: 14772 RVA: 0x0010959D File Offset: 0x0010779D
		public void ClearQuestNotification()
		{
			this.IsQuestNotificationActive = false;
		}

		// Token: 0x060039B5 RID: 14773 RVA: 0x001095A6 File Offset: 0x001077A6
		public string GetQuestNotificationText()
		{
			this._questNotificationText.SetTextVariable("NUMBER", this.UnExaminedQuestLogs.Count);
			return this._questNotificationText.ToString();
		}

		// Token: 0x060039B6 RID: 14774 RVA: 0x001095CF File Offset: 0x001077CF
		public void OnQuestlogExamined(JournalLog log)
		{
			if (this.UnExaminedQuestLogs.Contains(log))
			{
				this.UnExaminedQuestLogs.Remove(log);
			}
		}

		// Token: 0x060039B7 RID: 14775 RVA: 0x001095EC File Offset: 0x001077EC
		private void OnQuestLogAdded(QuestBase obj, bool hideInformation)
		{
			this.UnExaminedQuestLogs.Add(obj.JournalEntries[obj.JournalEntries.Count - 1]);
			this.IsQuestNotificationActive = true;
		}

		// Token: 0x060039B8 RID: 14776 RVA: 0x00109618 File Offset: 0x00107818
		private void OnIssueLogAdded(IssueBase obj, bool hideInformation)
		{
			this.UnExaminedQuestLogs.Add(obj.JournalEntries[obj.JournalEntries.Count - 1]);
			this.IsQuestNotificationActive = true;
		}

		// Token: 0x17000CE4 RID: 3300
		// (get) Token: 0x060039B9 RID: 14777 RVA: 0x00109644 File Offset: 0x00107844
		public List<Army> UnExaminedArmies { get; } = new List<Army>();

		// Token: 0x17000CE5 RID: 3301
		// (get) Token: 0x060039BA RID: 14778 RVA: 0x0010964C File Offset: 0x0010784C
		public int NumKingdomArmyNotifications
		{
			get
			{
				return this.UnExaminedArmies.Count;
			}
		}

		// Token: 0x060039BB RID: 14779 RVA: 0x00109659 File Offset: 0x00107859
		public void OnArmyExamined(Army army)
		{
			this.UnExaminedArmies.Remove(army);
		}

		// Token: 0x060039BC RID: 14780 RVA: 0x00109668 File Offset: 0x00107868
		private void OnArmyDispersed(Army arg1, Army.ArmyDispersionReason arg2, bool isPlayersArmy)
		{
			Army army;
			if (isPlayersArmy && (army = this.UnExaminedArmies.SingleOrDefault((Army a) => a == arg1)) != null)
			{
				this.UnExaminedArmies.Remove(army);
			}
		}

		// Token: 0x060039BD RID: 14781 RVA: 0x001096AD File Offset: 0x001078AD
		private void OnNewArmyCreated(Army army)
		{
			if (army.Kingdom == Hero.MainHero.MapFaction && army.LeaderParty != MobileParty.MainParty)
			{
				this.UnExaminedArmies.Add(army);
			}
		}

		// Token: 0x17000CE6 RID: 3302
		// (get) Token: 0x060039BE RID: 14782 RVA: 0x001096DA File Offset: 0x001078DA
		// (set) Token: 0x060039BF RID: 14783 RVA: 0x001096E2 File Offset: 0x001078E2
		public bool IsCharacterNotificationActive { get; private set; }

		// Token: 0x060039C0 RID: 14784 RVA: 0x001096EB File Offset: 0x001078EB
		public void ClearCharacterNotification()
		{
			this.IsCharacterNotificationActive = false;
			this._numOfPerks = 0;
			this._numOfFocusPoints = 0;
		}

		// Token: 0x060039C1 RID: 14785 RVA: 0x00109702 File Offset: 0x00107902
		public string GetCharacterNotificationText()
		{
			this._characterNotificationText.SetTextVariable("NUMBER", this._numOfPerks);
			return this._characterNotificationText.ToString();
		}

		// Token: 0x060039C2 RID: 14786 RVA: 0x00109726 File Offset: 0x00107926
		private void OnHeroGainedSkill(Hero hero, SkillObject skill, int change = 1, bool shouldNotify = true)
		{
			if ((hero == Hero.MainHero || hero.Clan == Clan.PlayerClan) && !hero.HeroDeveloper.GetOneAvailablePerkForEachPerkPair().IsEmpty<PerkObject>())
			{
				this.IsCharacterNotificationActive = true;
				this._numOfPerks++;
			}
		}

		// Token: 0x060039C3 RID: 14787 RVA: 0x00109764 File Offset: 0x00107964
		private void OnHeroLevelledUp(Hero hero, bool shouldNotify)
		{
			if (hero == Hero.MainHero)
			{
				this.IsCharacterNotificationActive = true;
				this._numOfFocusPoints++;
			}
		}

		// Token: 0x040011CC RID: 4556
		private TextObject _characterNotificationText = new TextObject("{=rlqjkZ9Q}You have {NUMBER} new perks available for selection.", null);

		// Token: 0x040011CD RID: 4557
		private TextObject _questNotificationText = new TextObject("{=FAIYN0vN}You have {NUMBER} new updates to your quests.", null);

		// Token: 0x040011CE RID: 4558
		private TextObject _recruitNotificationText = new TextObject("{=PJMbfSPJ}You have {NUMBER} new prisoners to recruit.", null);

		// Token: 0x040011CF RID: 4559
		private TextObject _upgradeNotificationText = new TextObject("{=Wbm4XGB6}You have {NUMBER} new troops to upgrade.", null);

		// Token: 0x040011D1 RID: 4561
		private Dictionary<BasicCharacterObject, int> _examinedTroopCharacterList = new Dictionary<BasicCharacterObject, int>();

		// Token: 0x040011D2 RID: 4562
		private Dictionary<BasicCharacterObject, int> _examinedPrisonerCharacterList = new Dictionary<BasicCharacterObject, int>();

		// Token: 0x040011D3 RID: 4563
		private int _numOfRecruitablePrisoners;

		// Token: 0x040011D8 RID: 4568
		private int _numOfPerks;

		// Token: 0x040011D9 RID: 4569
		private int _numOfFocusPoints;
	}
}
