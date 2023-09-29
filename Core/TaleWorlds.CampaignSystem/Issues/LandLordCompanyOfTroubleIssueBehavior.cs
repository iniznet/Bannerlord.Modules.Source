using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Conversation.Persuasion;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Overlay;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.Issues
{
	public class LandLordCompanyOfTroubleIssueBehavior : CampaignBehaviorBase
	{
		private static LandLordCompanyOfTroubleIssueBehavior.LandLordCompanyOfTroubleIssueQuest Instance
		{
			get
			{
				LandLordCompanyOfTroubleIssueBehavior campaignBehavior = Campaign.Current.GetCampaignBehavior<LandLordCompanyOfTroubleIssueBehavior>();
				if (campaignBehavior._cachedQuest != null && campaignBehavior._cachedQuest.IsOngoing)
				{
					return campaignBehavior._cachedQuest;
				}
				using (List<QuestBase>.Enumerator enumerator = Campaign.Current.QuestManager.Quests.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						LandLordCompanyOfTroubleIssueBehavior.LandLordCompanyOfTroubleIssueQuest landLordCompanyOfTroubleIssueQuest;
						if ((landLordCompanyOfTroubleIssueQuest = enumerator.Current as LandLordCompanyOfTroubleIssueBehavior.LandLordCompanyOfTroubleIssueQuest) != null)
						{
							campaignBehavior._cachedQuest = landLordCompanyOfTroubleIssueQuest;
							return campaignBehavior._cachedQuest;
						}
					}
				}
				return null;
			}
		}

		public override void RegisterEvents()
		{
			CampaignEvents.OnCheckForIssueEvent.AddNonSerializedListener(this, new Action<Hero>(this.OnCheckForIssue));
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
		}

		private void OnSessionLaunched(CampaignGameStarter gameStarter)
		{
			gameStarter.AddGameMenu("company_of_trouble_menu", "", new OnInitDelegate(this.company_of_trouble_menu_on_init), GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags.None, null);
		}

		private void company_of_trouble_menu_on_init(MenuCallbackArgs args)
		{
			if (LandLordCompanyOfTroubleIssueBehavior.Instance != null)
			{
				if (LandLordCompanyOfTroubleIssueBehavior.Instance._checkForBattleResults)
				{
					bool flag = PlayerEncounter.Battle.WinningSide == PlayerEncounter.Battle.PlayerSide;
					PlayerEncounter.Finish(true);
					if (LandLordCompanyOfTroubleIssueBehavior.Instance._companyOfTroubleParty != null && LandLordCompanyOfTroubleIssueBehavior.Instance._companyOfTroubleParty.IsActive)
					{
						DestroyPartyAction.Apply(null, LandLordCompanyOfTroubleIssueBehavior.Instance._companyOfTroubleParty);
					}
					LandLordCompanyOfTroubleIssueBehavior.Instance._checkForBattleResults = false;
					if (flag)
					{
						LandLordCompanyOfTroubleIssueBehavior.Instance.QuestSuccessWithPlayerDefeatedCompany();
						return;
					}
					LandLordCompanyOfTroubleIssueBehavior.Instance.QuestFailWithPlayerDefeatedAgainstCompany();
					return;
				}
				else
				{
					if (LandLordCompanyOfTroubleIssueBehavior.Instance._triggerCompanyOfTroubleConversation)
					{
						CampaignMapConversation.OpenConversation(new ConversationCharacterData(CharacterObject.PlayerCharacter, PartyBase.MainParty, false, false, false, false, false, false), new ConversationCharacterData(LandLordCompanyOfTroubleIssueBehavior.Instance._troubleCharacterObject, PartyBase.MainParty, false, false, false, false, false, false));
						LandLordCompanyOfTroubleIssueBehavior.Instance._triggerCompanyOfTroubleConversation = false;
						return;
					}
					if (LandLordCompanyOfTroubleIssueBehavior.Instance._battleWillStart)
					{
						PlayerEncounter.Start();
						PlayerEncounter.Current.SetupFields(PartyBase.MainParty, LandLordCompanyOfTroubleIssueBehavior.Instance._companyOfTroubleParty.Party);
						PlayerEncounter.StartBattle();
						CampaignMission.OpenBattleMission(PlayerEncounter.GetBattleSceneForMapPatch(Campaign.Current.MapSceneWrapper.GetMapPatchAtPosition(MobileParty.MainParty.Position2D)), false);
						LandLordCompanyOfTroubleIssueBehavior.Instance._battleWillStart = false;
						LandLordCompanyOfTroubleIssueBehavior.Instance._checkForBattleResults = true;
						return;
					}
					if (LandLordCompanyOfTroubleIssueBehavior.Instance._companyLeftQuestWillFail)
					{
						LandLordCompanyOfTroubleIssueBehavior.Instance.CompanyLeftQuestFail();
					}
				}
			}
		}

		public void OnCheckForIssue(Hero hero)
		{
			if (hero.IsLord && hero.Clan != Clan.PlayerClan && hero.PartyBelongedTo != null && !hero.IsMinorFactionHero && hero.GetTraitLevel(DefaultTraits.Mercy) <= 0)
			{
				Campaign.Current.IssueManager.AddPotentialIssueData(hero, new PotentialIssueData(new PotentialIssueData.StartIssueDelegate(this.OnStartIssue), typeof(LandLordCompanyOfTroubleIssueBehavior.LandLordCompanyOfTroubleIssue), IssueBase.IssueFrequency.Rare, null));
				return;
			}
			Campaign.Current.IssueManager.AddPotentialIssueData(hero, new PotentialIssueData(typeof(LandLordCompanyOfTroubleIssueBehavior.LandLordCompanyOfTroubleIssue), IssueBase.IssueFrequency.Rare));
		}

		private IssueBase OnStartIssue(in PotentialIssueData pid, Hero issueOwner)
		{
			return new LandLordCompanyOfTroubleIssueBehavior.LandLordCompanyOfTroubleIssue(issueOwner);
		}

		public override void SyncData(IDataStore dataStore)
		{
		}

		private const IssueBase.IssueFrequency LandLordCompanyOfTroubleIssueFrequency = IssueBase.IssueFrequency.Rare;

		private LandLordCompanyOfTroubleIssueBehavior.LandLordCompanyOfTroubleIssueQuest _cachedQuest;

		private const int IssueDuration = 25;

		public class LandLordCompanyOfTroubleIssue : IssueBase
		{
			internal static void AutoGeneratedStaticCollectObjectsLandLordCompanyOfTroubleIssue(object o, List<object> collectedObjects)
			{
				((LandLordCompanyOfTroubleIssueBehavior.LandLordCompanyOfTroubleIssue)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			private int CompanyTroopCount
			{
				get
				{
					return 5 + (int)(base.IssueDifficultyMultiplier * 30f);
				}
			}

			public override bool IsThereAlternativeSolution
			{
				get
				{
					return false;
				}
			}

			public override bool IsThereLordSolution
			{
				get
				{
					return false;
				}
			}

			public override TextObject IssueBriefByIssueGiver
			{
				get
				{
					return new TextObject("{=wrpsJM2u}Yes... I hired a band of mercenaries for a campaign some time back. But... normally mercenaries have their own peculiar kind of honor. You pay them, they fight for you, you don't, they go somewhere else. But these ones have made it pretty clear that if I don't keep renewing the contract, they'll turn bandit. I can't afford that right now.[if:convo_thinking][ib:closed]", null);
				}
			}

			public override TextObject IssueAcceptByPlayer
			{
				get
				{
					return new TextObject("{=VlbCFDWu}What do you want from me?", null);
				}
			}

			public override TextObject IssueQuestSolutionExplanationByIssueGiver
			{
				get
				{
					return new TextObject("{=wxDbPiNH}Well, you have the reputation of being able to manage ruffians. Maybe you can take them off my hands, find some other lord who has more need of them and more denars to pay them. I've paid their contract for a few months. I can give you a small reward and if you can find a buyer, you can transfer the rest of the contract to him and pocket the down payment.[if:convo_innocent_smile]", null);
				}
			}

			public override TextObject IssueQuestSolutionAcceptByPlayer
			{
				get
				{
					return new TextObject("{=6bvJSIqh}Yes. I can find a new lord to take them on.", null);
				}
			}

			public override TextObject Title
			{
				get
				{
					return new TextObject("{=PV7RHgUl}Company of Trouble", null);
				}
			}

			public override TextObject Description
			{
				get
				{
					TextObject textObject = new TextObject("{=zw7a9eIt}{ISSUE_GIVER.NAME} wants you to take {?ISSUE_GIVER.GENDER}her{?}his{\\?} mercenaries and transfer them to another lord before they cause any trouble.", null);
					StringHelpers.SetCharacterProperties("ISSUE_GIVER", base.IssueOwner.CharacterObject, textObject, false);
					return textObject;
				}
			}

			public override TextObject IssueAsRumorInSettlement
			{
				get
				{
					TextObject textObject = new TextObject("{=I022Z9Ub}Heh. {QUEST_GIVER.NAME} got in deeper than {?QUEST_GIVER.GENDER}she{?}he{\\?} could handle with those mercenaries.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.IssueOwner.CharacterObject, textObject, false);
					return textObject;
				}
			}

			public LandLordCompanyOfTroubleIssue(Hero issueOwner)
				: base(issueOwner, CampaignTime.DaysFromNow(25f))
			{
			}

			protected override float GetIssueEffectAmountInternal(IssueEffect issueEffect)
			{
				if (issueEffect == DefaultIssueEffects.ClanInfluence)
				{
					return -0.1f;
				}
				return 0f;
			}

			protected override void OnGameLoad()
			{
			}

			protected override void HourlyTick()
			{
			}

			protected override QuestBase GenerateIssueQuest(string questId)
			{
				return new LandLordCompanyOfTroubleIssueBehavior.LandLordCompanyOfTroubleIssueQuest(questId, base.IssueOwner, CampaignTime.Never, this.CompanyTroopCount);
			}

			public override IssueBase.IssueFrequency GetFrequency()
			{
				return IssueBase.IssueFrequency.Rare;
			}

			protected override bool CanPlayerTakeQuestConditions(Hero issueGiver, out IssueBase.PreconditionFlags flag, out Hero relationHero, out SkillObject skill)
			{
				skill = null;
				relationHero = null;
				flag = IssueBase.PreconditionFlags.None;
				if (issueGiver.GetRelationWithPlayer() < -10f)
				{
					flag |= IssueBase.PreconditionFlags.Relation;
					relationHero = issueGiver;
				}
				if (Clan.PlayerClan.Tier < 1)
				{
					flag |= IssueBase.PreconditionFlags.ClanTier;
				}
				if (MobileParty.MainParty.MemberRoster.TotalManCount < this.CompanyTroopCount)
				{
					flag |= IssueBase.PreconditionFlags.NotEnoughTroops;
				}
				if (MobileParty.MainParty.MemberRoster.TotalManCount + this.CompanyTroopCount > PartyBase.MainParty.PartySizeLimit)
				{
					flag |= IssueBase.PreconditionFlags.PartySizeLimit;
				}
				if (issueGiver.MapFaction.IsAtWarWith(Hero.MainHero.MapFaction))
				{
					flag |= IssueBase.PreconditionFlags.AtWar;
				}
				return flag == IssueBase.PreconditionFlags.None;
			}

			public override bool IssueStayAliveConditions()
			{
				return base.IssueOwner.Clan != Clan.PlayerClan;
			}

			protected override void CompleteIssueWithTimedOutConsequences()
			{
			}
		}

		public class LandLordCompanyOfTroubleIssueQuest : QuestBase
		{
			internal static void AutoGeneratedStaticCollectObjectsLandLordCompanyOfTroubleIssueQuest(object o, List<object> collectedObjects)
			{
				((LandLordCompanyOfTroubleIssueBehavior.LandLordCompanyOfTroubleIssueQuest)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				base.AutoGeneratedInstanceCollectObjects(collectedObjects);
				collectedObjects.Add(this._companyOfTroubleParty);
				collectedObjects.Add(this._persuationTriedHeroesList);
			}

			internal static object AutoGeneratedGetMemberValue_companyOfTroubleParty(object o)
			{
				return ((LandLordCompanyOfTroubleIssueBehavior.LandLordCompanyOfTroubleIssueQuest)o)._companyOfTroubleParty;
			}

			internal static object AutoGeneratedGetMemberValue_battleWillStart(object o)
			{
				return ((LandLordCompanyOfTroubleIssueBehavior.LandLordCompanyOfTroubleIssueQuest)o)._battleWillStart;
			}

			internal static object AutoGeneratedGetMemberValue_triggerCompanyOfTroubleConversation(object o)
			{
				return ((LandLordCompanyOfTroubleIssueBehavior.LandLordCompanyOfTroubleIssueQuest)o)._triggerCompanyOfTroubleConversation;
			}

			internal static object AutoGeneratedGetMemberValue_thieveryCount(object o)
			{
				return ((LandLordCompanyOfTroubleIssueBehavior.LandLordCompanyOfTroubleIssueQuest)o)._thieveryCount;
			}

			internal static object AutoGeneratedGetMemberValue_demandGold(object o)
			{
				return ((LandLordCompanyOfTroubleIssueBehavior.LandLordCompanyOfTroubleIssueQuest)o)._demandGold;
			}

			internal static object AutoGeneratedGetMemberValue_persuationTriedHeroesList(object o)
			{
				return ((LandLordCompanyOfTroubleIssueBehavior.LandLordCompanyOfTroubleIssueQuest)o)._persuationTriedHeroesList;
			}

			public override bool IsRemainingTimeHidden
			{
				get
				{
					return true;
				}
			}

			public override TextObject Title
			{
				get
				{
					return new TextObject("{=PV7RHgUl}Company of Trouble", null);
				}
			}

			private TextObject _playerStartsQuestLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=8nS3QgD7}{QUEST_GIVER.LINK} is a {?QUEST_GIVER.GENDER}lady{?}lord{\\?} who told you that {?QUEST_GIVER.GENDER}she{?}he{\\?} wants to sell {?QUEST_GIVER.GENDER}her{?}his{\\?} mercenaries to another lord's service. {?QUEST_GIVER.GENDER}She{?}He{\\?} asked you sell them for {?QUEST_GIVER.GENDER}her{?}him{\\?} without causing any trouble.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					return textObject;
				}
			}

			private TextObject _questSuccessPlayerSoldCompany
			{
				get
				{
					TextObject textObject = new TextObject("{=34MdCd6u}You have sold the mercenaries to another lord as you promised. {QUEST_GIVER.LINK} is grateful and sends {?QUEST_GIVER.GENDER}her{?}his{\\?} regards.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					return textObject;
				}
			}

			private TextObject _allCompanyDiedLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=RrTAX7QE}You got the troublesome mercenaries killed off. You get no extra money for the contract, but you did get rid of them as you promised. {QUEST_GIVER.LINK} is grateful and sends {?QUEST_GIVER.GENDER}her{?}his{\\?} regards.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					return textObject;
				}
			}

			private TextObject _playerDefeatedAgainstCompany
			{
				get
				{
					return new TextObject("{=7naLQmq1}You have lost the battle against the mercenaries. You have failed to get rid of them as you promised. Now they've turned bandit and are starting to plunder the countryside", null);
				}
			}

			private TextObject _questFailCompanyLeft
			{
				get
				{
					return new TextObject("{=k9SksaXg}The mercenaries left your party, as you failed to get rid of them as you promised. Now the mercenaries have turned bandit and start to plunder countryside.", null);
				}
			}

			private TextObject _questCanceledWarDeclared
			{
				get
				{
					TextObject textObject = new TextObject("{=ItueKmqd}Your clan is now at war with the {QUEST_GIVER_SETTLEMENT_FACTION}. You contract with {QUEST_GIVER.LINK} is canceled.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					textObject.SetTextVariable("QUEST_GIVER_SETTLEMENT_FACTION", base.QuestGiver.MapFaction.InformalName);
					return textObject;
				}
			}

			private TextObject _playerDeclaredWarQuestLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=bqeWVVEE}Your actions have started a war with {QUEST_GIVER.LINK}'s faction. {?QUEST_GIVER.GENDER}She{?}He{\\?} cancels your agreement and the quest is a failure.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					return textObject;
				}
			}

			public LandLordCompanyOfTroubleIssueQuest(string questId, Hero questGiver, CampaignTime duration, int companyTroopCount)
				: base(questId, questGiver, duration, 500)
			{
				this._troubleCharacterObject = MBObjectManager.Instance.GetObject<CharacterObject>("company_of_trouble_character");
				this._persuationTriedHeroesList = new List<Hero>();
				this._troubleCharacterObject.SetTransferableInPartyScreen(false);
				this._troubleCharacterObject.SetTransferableInHideouts(false);
				this._companyTroopCount = companyTroopCount;
				this._tasks = new PersuasionTask[3];
				this._battleWillStart = false;
				this._thieveryCount = 0;
				this._demandGold = 0;
				this.SetDialogs();
				base.InitializeQuestOnCreation();
			}

			protected override void InitializeQuestOnGameLoad()
			{
				this._troubleCharacterObject = MBObjectManager.Instance.GetObject<CharacterObject>("company_of_trouble_character");
				this._troubleCharacterObject.SetTransferableInPartyScreen(false);
				this._troubleCharacterObject.SetTransferableInHideouts(false);
				this._tasks = new PersuasionTask[3];
				this.UpdateCompanyTroopCount();
				this.SetDialogs();
			}

			protected override void SetDialogs()
			{
				this.OfferDialogFlow = DialogFlow.CreateDialogFlow("issue_classic_quest_start", 100).NpcLine(new TextObject("{=T6d7wtJX}Very well. I'll tell them to join your party. Good luck.[if:convo_mocking_aristocratic][ib:hip]", null), null, null).Condition(() => Hero.OneToOneConversationHero == base.QuestGiver)
					.Consequence(new ConversationSentence.OnConsequenceDelegate(this.QuestAcceptedConsequences))
					.CloseDialog();
				this.DiscussDialogFlow = DialogFlow.CreateDialogFlow("quest_discuss", 100).NpcLine(new TextObject("{=bWpLYiEg}Did you ever find a way to handle those mercenaries?[if:convo_astonished]", null), null, null).Condition(() => Hero.OneToOneConversationHero == base.QuestGiver)
					.Consequence(delegate
					{
						Campaign.Current.ConversationManager.ConversationEndOneShot += MapEventHelper.OnConversationEnd;
					})
					.BeginPlayerOptions()
					.PlayerOption(new TextObject("{=XzK4niIb}I'll find an employer soon.", null), null)
					.NpcLine(new TextObject("{=rOBRabQz}Good. I'm waiting for your good news.[if:convo_mocking_aristocratic]", null), null, null)
					.CloseDialog()
					.PlayerOption(new TextObject("{=Zb3EdxDT}That kind of lord is hard to find.", null), null)
					.NpcLine(new TextObject("{=yOfrb9Lu}Don't wait too long. These are dangerous men. Be careful.[if:convo_nonchalant]", null), null, null)
					.CloseDialog()
					.EndPlayerOptions()
					.CloseDialog();
				Campaign.Current.ConversationManager.AddDialogFlow(this.GetCompanyDialogFlow(), this);
				Campaign.Current.ConversationManager.AddDialogFlow(this.GetOtherLordsDialogFlow(), this);
			}

			private DialogFlow GetOtherLordsDialogFlow()
			{
				DialogFlow dialogFlow = DialogFlow.CreateDialogFlow("hero_main_options", 700).BeginPlayerOptions().PlayerOption(new TextObject("{=2E7s4L9R}Do you need mercenaries? I have a contract that I can transfer to you for {DEMAND_GOLD} denars.", null), null)
					.Condition(new ConversationSentence.OnConditionDelegate(this.PersuasionDialogForLordGeneralCondition))
					.BeginNpcOptions()
					.NpcOption(new TextObject("{=ZR4RJdYS}Hmm, that sounds interesting...[if:convo_thinking]", null), new ConversationSentence.OnConditionDelegate(this.PersuasionDialogSpecialCondition), null, null)
					.GotoDialogState("company_of_trouble_persuasion")
					.NpcOption(new TextObject("{=pmrjUNEz}As it happens, I already have a mercenary contract that I wish to sell. So, no thank you.[if:convo_calm_friendly]", null), new ConversationSentence.OnConditionDelegate(this.HasSameIssue), null, null)
					.GotoDialogState("hero_main_options")
					.NpcOption(new TextObject("{=bw0hEPN6}You already bought their contract from our clan. Why would I want to buy them back?[if:convo_confused_normal]", null), new ConversationSentence.OnConditionDelegate(this.IsSameClanMember), null, null)
					.GotoDialogState("hero_main_options")
					.NpcOption(new TextObject("{=64bH4bUo}No, thank you. But perhaps one of the other lords of our clan would be interested.[if:convo_undecided_closed]", null), () => !this.HasMobileParty(), null, null)
					.GotoDialogState("hero_main_options")
					.NpcOption(new TextObject("{=Zs6L1aBL}I'm sorry. I don't need mercenaries right now.[if:convo_normal]", null), null, null, null)
					.GotoDialogState("hero_main_options")
					.EndNpcOptions()
					.EndPlayerOptions();
				this.AddPersuasionDialogs(dialogFlow);
				return dialogFlow;
			}

			private bool PersuasionDialogSpecialCondition()
			{
				return !this.IsSameClanMember() && !this.HasSameIssue() && this.HasMobileParty() && !this.InSameSettlement();
			}

			private bool HasMobileParty()
			{
				return Hero.OneToOneConversationHero.PartyBelongedTo != null;
			}

			private bool IsSameClanMember()
			{
				return Hero.OneToOneConversationHero.Clan == base.QuestGiver.Clan;
			}

			private bool InSameSettlement()
			{
				return Hero.OneToOneConversationHero.CurrentSettlement != null && base.QuestGiver.CurrentSettlement != null && Hero.OneToOneConversationHero.CurrentSettlement == base.QuestGiver.CurrentSettlement;
			}

			private bool HasSameIssue()
			{
				IssueBase issue = Hero.OneToOneConversationHero.Issue;
				return ((issue != null) ? issue.GetType() : null) == base.GetType();
			}

			private bool PersuasionDialogForLordGeneralCondition()
			{
				if (Hero.OneToOneConversationHero != null && Hero.OneToOneConversationHero.IsLord && Hero.OneToOneConversationHero.Age >= (float)Campaign.Current.Models.AgeModel.HeroComesOfAge && Hero.OneToOneConversationHero != base.QuestGiver && !Hero.OneToOneConversationHero.MapFaction.IsAtWarWith(base.QuestGiver.MapFaction) && Hero.OneToOneConversationHero.Clan != Clan.PlayerClan && !this._persuationTriedHeroesList.Contains(Hero.OneToOneConversationHero))
				{
					this.UpdateCompanyTroopCount();
					this._demandGold = 1000 + this._companyTroopCount * 150;
					MBTextManager.SetTextVariable("DEMAND_GOLD", this._demandGold);
					this._tasks[0] = this.GetPersuasionTask1();
					this._tasks[1] = this.GetPersuasionTask2();
					this._tasks[2] = this.GetPersuasionTask3();
					this._selectedTask = this._tasks.GetRandomElement<PersuasionTask>();
					return true;
				}
				return false;
			}

			private void AddPersuasionDialogs(DialogFlow dialog)
			{
				dialog.AddDialogLine("company_of_trouble_persuasion_check_accepted", "company_of_trouble_persuasion", "company_of_trouble_persuasion_start_reservation", "{=GCH6RgIQ}How tough are they?", new ConversationSentence.OnConditionDelegate(this.persuasion_start_with_company_of_trouble_on_condition), new ConversationSentence.OnConsequenceDelegate(this.persuasion_start_with_company_of_trouble_on_consequence), this, 100, null, null, null);
				dialog.AddDialogLine("company_of_trouble_persuasion_rejected", "company_of_trouble_persuasion_start_reservation", "hero_main_options", "{=!}{FAILED_PERSUASION_LINE}", new ConversationSentence.OnConditionDelegate(this.persuasion_failed_with_company_of_trouble_on_condition), new ConversationSentence.OnConsequenceDelegate(this.persuasion_rejected_with_company_of_trouble_on_consequence), this, 100, null, null, null);
				dialog.AddDialogLine("company_of_trouble_persuasion_attempt", "company_of_trouble_persuasion_start_reservation", "company_of_trouble_persuasion_select_option", "{=K0Qtl5RZ}Tell me about the details...", () => !this.persuasion_failed_with_company_of_trouble_on_condition(), null, this, 100, null, null, null);
				dialog.AddDialogLine("company_of_trouble_persuasion_success", "company_of_trouble_persuasion_start_reservation", "close_window", "{=QlECaaHt}Hmm...They can be useful.", new ConversationSentence.OnConditionDelegate(ConversationManager.GetPersuasionProgressSatisfied), new ConversationSentence.OnConsequenceDelegate(this.persuasion_complete_with_company_of_trouble_on_consequence), this, 200, null, null, null);
				string text = "company_of_trouble_persuasion_select_option_1";
				string text2 = "company_of_trouble_persuasion_select_option";
				string text3 = "company_of_trouble_persuasion_selected_option_response";
				string text4 = "{=0AUZvSAq}{COMPANY_OF_TROUBLE_PERSUADE_ATTEMPT_1}";
				ConversationSentence.OnConditionDelegate onConditionDelegate = new ConversationSentence.OnConditionDelegate(this.company_of_trouble_persuasion_select_option_1_on_condition);
				ConversationSentence.OnConsequenceDelegate onConsequenceDelegate = new ConversationSentence.OnConsequenceDelegate(this.company_of_trouble_persuasion_select_option_1_on_consequence);
				ConversationSentence.OnPersuasionOptionDelegate onPersuasionOptionDelegate = new ConversationSentence.OnPersuasionOptionDelegate(this.company_of_trouble_persuasion_setup_option_1);
				ConversationSentence.OnClickableConditionDelegate onClickableConditionDelegate = new ConversationSentence.OnClickableConditionDelegate(this.company_of_trouble_persuasion_clickable_option_1_on_condition);
				dialog.AddPlayerLine(text, text2, text3, text4, onConditionDelegate, onConsequenceDelegate, this, 100, onClickableConditionDelegate, onPersuasionOptionDelegate, null, null);
				string text5 = "company_of_trouble_persuasion_select_option_2";
				string text6 = "company_of_trouble_persuasion_select_option";
				string text7 = "company_of_trouble_persuasion_selected_option_response";
				string text8 = "{=GG1W8qGd}{COMPANY_OF_TROUBLE_PERSUADE_ATTEMPT_2}";
				ConversationSentence.OnConditionDelegate onConditionDelegate2 = new ConversationSentence.OnConditionDelegate(this.company_of_trouble_persuasion_select_option_2_on_condition);
				ConversationSentence.OnConsequenceDelegate onConsequenceDelegate2 = new ConversationSentence.OnConsequenceDelegate(this.company_of_trouble_persuasion_select_option_2_on_consequence);
				onPersuasionOptionDelegate = new ConversationSentence.OnPersuasionOptionDelegate(this.company_of_trouble_persuasion_setup_option_2);
				onClickableConditionDelegate = new ConversationSentence.OnClickableConditionDelegate(this.company_of_trouble_persuasion_clickable_option_2_on_condition);
				dialog.AddPlayerLine(text5, text6, text7, text8, onConditionDelegate2, onConsequenceDelegate2, this, 100, onClickableConditionDelegate, onPersuasionOptionDelegate, null, null);
				string text9 = "company_of_trouble_persuasion_select_option_3";
				string text10 = "company_of_trouble_persuasion_select_option";
				string text11 = "company_of_trouble_persuasion_selected_option_response";
				string text12 = "{=kFs940kp}{COMPANY_OF_TROUBLE_PERSUADE_ATTEMPT_3}";
				ConversationSentence.OnConditionDelegate onConditionDelegate3 = new ConversationSentence.OnConditionDelegate(this.company_of_trouble_persuasion_select_option_3_on_condition);
				ConversationSentence.OnConsequenceDelegate onConsequenceDelegate3 = new ConversationSentence.OnConsequenceDelegate(this.company_of_trouble_persuasion_select_option_3_on_consequence);
				onPersuasionOptionDelegate = new ConversationSentence.OnPersuasionOptionDelegate(this.company_of_trouble_persuasion_setup_option_3);
				onClickableConditionDelegate = new ConversationSentence.OnClickableConditionDelegate(this.company_of_trouble_persuasion_clickable_option_3_on_condition);
				dialog.AddPlayerLine(text9, text10, text11, text12, onConditionDelegate3, onConsequenceDelegate3, this, 100, onClickableConditionDelegate, onPersuasionOptionDelegate, null, null);
				dialog.AddDialogLine("company_of_trouble_persuasion_select_option_reaction", "company_of_trouble_persuasion_selected_option_response", "company_of_trouble_persuasion_start_reservation", "{=D0xDRqvm}{PERSUASION_REACTION}", new ConversationSentence.OnConditionDelegate(this.company_of_trouble_persuasion_selected_option_response_on_condition), new ConversationSentence.OnConsequenceDelegate(this.company_of_trouble_persuasion_selected_option_response_on_consequence), this, 100, null, null, null);
			}

			private void persuasion_start_with_company_of_trouble_on_consequence()
			{
				this._persuationTriedHeroesList.Add(Hero.OneToOneConversationHero);
				ConversationManager.StartPersuasion(2f, 1f, 0f, 2f, 2f, 0f, PersuasionDifficulty.Hard);
			}

			private bool persuasion_start_with_company_of_trouble_on_condition()
			{
				return !this._persuationTriedHeroesList.Contains(Hero.OneToOneConversationHero);
			}

			private PersuasionTask GetPersuasionTask1()
			{
				PersuasionTask persuasionTask = new PersuasionTask(0);
				persuasionTask.FinalFailLine = new TextObject("{=1V9GeKr8}Fah...I don't need more men. Thank you.", null);
				persuasionTask.TryLaterLine = new TextObject("{=!}TODO", null);
				persuasionTask.SpokenLine = new TextObject("{=EvAubSxs}What kind of troops do they make?", null);
				PersuasionOptionArgs persuasionOptionArgs = new PersuasionOptionArgs(DefaultSkills.Trade, DefaultTraits.Calculating, TraitEffect.Positive, PersuasionArgumentStrength.Easy, false, new TextObject("{=sqMUtasn}Cheap, disposable and effective. What you say?", null), null, false, false, false);
				persuasionTask.AddOptionToTask(persuasionOptionArgs);
				PersuasionOptionArgs persuasionOptionArgs2 = new PersuasionOptionArgs(DefaultSkills.Tactics, DefaultTraits.Calculating, TraitEffect.Positive, PersuasionArgumentStrength.ExtremelyHard, true, new TextObject("{=Pcgqs9aX}Here's a quick run down of their training...", null), null, true, false, false);
				persuasionTask.AddOptionToTask(persuasionOptionArgs2);
				PersuasionOptionArgs persuasionOptionArgs3 = new PersuasionOptionArgs(DefaultSkills.Roguery, DefaultTraits.Valor, TraitEffect.Positive, PersuasionArgumentStrength.Normal, false, new TextObject("{=WvQDatMJ}I won't kid you, they're mean bastards, but that's good if you can manage them.", null), null, false, false, false);
				persuasionTask.AddOptionToTask(persuasionOptionArgs3);
				return persuasionTask;
			}

			private PersuasionTask GetPersuasionTask2()
			{
				PersuasionTask persuasionTask = new PersuasionTask(0);
				persuasionTask.FinalFailLine = new TextObject("{=UP0pMGDR}There are enough bandits around here already. I don't need more on retainer.", null);
				persuasionTask.TryLaterLine = new TextObject("{=!}TODO", null);
				persuasionTask.SpokenLine = new TextObject("{=zR356YDY}I have to say, they seem more like bandits than soldiers.", null);
				PersuasionOptionArgs persuasionOptionArgs = new PersuasionOptionArgs(DefaultSkills.Roguery, DefaultTraits.Valor, TraitEffect.Positive, PersuasionArgumentStrength.Easy, false, new TextObject("{=JI6Q9pQ7}Bandits can kill as well as any other kind of troops, if used correctly.", null), null, false, false, false);
				persuasionTask.AddOptionToTask(persuasionOptionArgs);
				PersuasionOptionArgs persuasionOptionArgs2 = new PersuasionOptionArgs(DefaultSkills.Trade, DefaultTraits.Calculating, TraitEffect.Positive, PersuasionArgumentStrength.ExtremelyHard, true, new TextObject("{=SqceZdzH}Of course. That's why they're cheap. You get what you pay for. ", null), null, true, false, false);
				persuasionTask.AddOptionToTask(persuasionOptionArgs2);
				PersuasionOptionArgs persuasionOptionArgs3 = new PersuasionOptionArgs(DefaultSkills.Scouting, DefaultTraits.Calculating, TraitEffect.Positive, PersuasionArgumentStrength.Normal, false, new TextObject("{=NWLH02KL}Bandits are good in the wilderness, having been both predator and prey.", null), null, false, false, false);
				persuasionTask.AddOptionToTask(persuasionOptionArgs3);
				return persuasionTask;
			}

			private PersuasionTask GetPersuasionTask3()
			{
				PersuasionTask persuasionTask = new PersuasionTask(0);
				persuasionTask.FinalFailLine = new TextObject("{=97pacK2l}Fah... I don't need more men. Thank you.", null);
				persuasionTask.TryLaterLine = new TextObject("{=!}TODO", null);
				persuasionTask.SpokenLine = new TextObject("{=A2ju7YTZ}I don't know... They look treacherous.", null);
				PersuasionOptionArgs persuasionOptionArgs = new PersuasionOptionArgs(DefaultSkills.Tactics, DefaultTraits.Mercy, TraitEffect.Negative, PersuasionArgumentStrength.Easy, false, new TextObject("{=z1mdQhDB}Of course. Send them in ahead of your other troops. If they die, you don't need to pay them.", null), null, false, false, false);
				persuasionTask.AddOptionToTask(persuasionOptionArgs);
				PersuasionOptionArgs persuasionOptionArgs2 = new PersuasionOptionArgs(DefaultSkills.Roguery, DefaultTraits.Calculating, TraitEffect.Positive, PersuasionArgumentStrength.ExtremelyHard, true, new TextObject("{=jWavM9AD}You've been around in the world. You know that mercenaries aren't saints.", null), null, true, false, false);
				persuasionTask.AddOptionToTask(persuasionOptionArgs2);
				PersuasionOptionArgs persuasionOptionArgs3 = new PersuasionOptionArgs(DefaultSkills.Roguery, DefaultTraits.Generosity, TraitEffect.Positive, PersuasionArgumentStrength.Normal, false, new TextObject("{=sLjGguGy}Sure, they're bastards. But they'll be loyal bastards if you treat them well.", null), null, false, false, false);
				persuasionTask.AddOptionToTask(persuasionOptionArgs3);
				return persuasionTask;
			}

			private bool company_of_trouble_persuasion_selected_option_response_on_condition()
			{
				PersuasionOptionResult item = ConversationManager.GetPersuasionChosenOptions().Last<Tuple<PersuasionOptionArgs, PersuasionOptionResult>>().Item2;
				MBTextManager.SetTextVariable("PERSUASION_REACTION", PersuasionHelper.GetDefaultPersuasionOptionReaction(item), false);
				if (item == PersuasionOptionResult.CriticalFailure)
				{
					this._selectedTask.BlockAllOptions();
				}
				return true;
			}

			private void company_of_trouble_persuasion_selected_option_response_on_consequence()
			{
				Tuple<PersuasionOptionArgs, PersuasionOptionResult> tuple = ConversationManager.GetPersuasionChosenOptions().Last<Tuple<PersuasionOptionArgs, PersuasionOptionResult>>();
				float difficulty = Campaign.Current.Models.PersuasionModel.GetDifficulty(PersuasionDifficulty.Hard);
				float num;
				float num2;
				Campaign.Current.Models.PersuasionModel.GetEffectChances(tuple.Item1, out num, out num2, difficulty);
				this._selectedTask.ApplyEffects(num, num2);
			}

			private bool company_of_trouble_persuasion_select_option_1_on_condition()
			{
				if (this._selectedTask.Options.Count > 0)
				{
					TextObject textObject = new TextObject("{=bSo9hKwr}{PERSUASION_OPTION_LINE} {SUCCESS_CHANCE}", null);
					textObject.SetTextVariable("SUCCESS_CHANCE", PersuasionHelper.ShowSuccess(this._selectedTask.Options.ElementAt(0), false));
					textObject.SetTextVariable("PERSUASION_OPTION_LINE", this._selectedTask.Options.ElementAt(0).Line);
					MBTextManager.SetTextVariable("COMPANY_OF_TROUBLE_PERSUADE_ATTEMPT_1", textObject, false);
					return true;
				}
				return false;
			}

			private bool company_of_trouble_persuasion_select_option_2_on_condition()
			{
				if (this._selectedTask.Options.Count > 1)
				{
					TextObject textObject = new TextObject("{=bSo9hKwr}{PERSUASION_OPTION_LINE} {SUCCESS_CHANCE}", null);
					textObject.SetTextVariable("SUCCESS_CHANCE", PersuasionHelper.ShowSuccess(this._selectedTask.Options.ElementAt(1), false));
					textObject.SetTextVariable("PERSUASION_OPTION_LINE", this._selectedTask.Options.ElementAt(1).Line);
					MBTextManager.SetTextVariable("COMPANY_OF_TROUBLE_PERSUADE_ATTEMPT_2", textObject, false);
					return true;
				}
				return false;
			}

			private bool company_of_trouble_persuasion_select_option_3_on_condition()
			{
				if (this._selectedTask.Options.Count > 2)
				{
					TextObject textObject = new TextObject("{=bSo9hKwr}{PERSUASION_OPTION_LINE} {SUCCESS_CHANCE}", null);
					textObject.SetTextVariable("SUCCESS_CHANCE", PersuasionHelper.ShowSuccess(this._selectedTask.Options.ElementAt(2), false));
					textObject.SetTextVariable("PERSUASION_OPTION_LINE", this._selectedTask.Options.ElementAt(2).Line);
					MBTextManager.SetTextVariable("COMPANY_OF_TROUBLE_PERSUADE_ATTEMPT_3", textObject, false);
					return true;
				}
				return false;
			}

			private void company_of_trouble_persuasion_select_option_1_on_consequence()
			{
				if (this._selectedTask.Options.Count > 0)
				{
					this._selectedTask.Options[0].BlockTheOption(true);
				}
			}

			private void company_of_trouble_persuasion_select_option_2_on_consequence()
			{
				if (this._selectedTask.Options.Count > 1)
				{
					this._selectedTask.Options[1].BlockTheOption(true);
				}
			}

			private void company_of_trouble_persuasion_select_option_3_on_consequence()
			{
				if (this._selectedTask.Options.Count > 2)
				{
					this._selectedTask.Options[2].BlockTheOption(true);
				}
			}

			private bool persuasion_failed_with_company_of_trouble_on_condition()
			{
				if (this._selectedTask.Options.All((PersuasionOptionArgs x) => x.IsBlocked) && !ConversationManager.GetPersuasionProgressSatisfied())
				{
					MBTextManager.SetTextVariable("FAILED_PERSUASION_LINE", this._selectedTask.FinalFailLine, false);
					return true;
				}
				return false;
			}

			private PersuasionOptionArgs company_of_trouble_persuasion_setup_option_1()
			{
				return this._selectedTask.Options.ElementAt(0);
			}

			private PersuasionOptionArgs company_of_trouble_persuasion_setup_option_2()
			{
				return this._selectedTask.Options.ElementAt(1);
			}

			private PersuasionOptionArgs company_of_trouble_persuasion_setup_option_3()
			{
				return this._selectedTask.Options.ElementAt(2);
			}

			private bool company_of_trouble_persuasion_clickable_option_1_on_condition(out TextObject hintText)
			{
				hintText = new TextObject("{=9ACJsI6S}Blocked", null);
				if (this._selectedTask.Options.Count > 0)
				{
					hintText = (this._selectedTask.Options.ElementAt(0).IsBlocked ? hintText : TextObject.Empty);
					return !this._selectedTask.Options.ElementAt(0).IsBlocked;
				}
				return false;
			}

			private bool company_of_trouble_persuasion_clickable_option_2_on_condition(out TextObject hintText)
			{
				hintText = new TextObject("{=9ACJsI6S}Blocked", null);
				if (this._selectedTask.Options.Count > 1)
				{
					hintText = (this._selectedTask.Options.ElementAt(1).IsBlocked ? hintText : TextObject.Empty);
					return !this._selectedTask.Options.ElementAt(1).IsBlocked;
				}
				return false;
			}

			private bool company_of_trouble_persuasion_clickable_option_3_on_condition(out TextObject hintText)
			{
				hintText = new TextObject("{=9ACJsI6S}Blocked", null);
				if (this._selectedTask.Options.Count > 2)
				{
					hintText = (this._selectedTask.Options.ElementAt(2).IsBlocked ? hintText : TextObject.Empty);
					return !this._selectedTask.Options.ElementAt(2).IsBlocked;
				}
				return false;
			}

			private void persuasion_rejected_with_company_of_trouble_on_consequence()
			{
				if (PlayerEncounter.Current != null)
				{
					PlayerEncounter.LeaveEncounter = true;
				}
				ConversationManager.EndPersuasion();
			}

			private void persuasion_complete_with_company_of_trouble_on_consequence()
			{
				if (PlayerEncounter.Current != null)
				{
					PlayerEncounter.LeaveEncounter = true;
				}
				ConversationManager.EndPersuasion();
				this.UpdateCompanyTroopCount();
				MobileParty.MainParty.MemberRoster.AddToCounts(this._troubleCharacterObject, -this._companyTroopCount, false, 0, 0, true, -1);
				GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, this._demandGold, false);
				this.RelationshipChangeWithQuestGiver = 5;
				base.AddLog(this._questSuccessPlayerSoldCompany, false);
				base.CompleteQuestWithSuccess();
			}

			private DialogFlow GetCompanyDialogFlow()
			{
				return DialogFlow.CreateDialogFlow("start", 125).NpcLine(new TextObject("{=8TCev3Qs}So, captain. We expect a bit of looting and plundering as compensation, in addition to the wages. You don't seem like you're going to provide it to us. So, farewell.[if:innocent_smile][ib:hip]", null), null, null).Condition(new ConversationSentence.OnConditionDelegate(this.CompanyDialogFromCondition))
					.BeginPlayerOptions()
					.PlayerOption(new TextObject("{=1aaoSpNf}Your contract with the {QUEST_GIVER.NAME} is still in force. I can't let you go without {?QUEST_GIVER.GENDER}her{?}his{\\?} permission.", null), null)
					.NpcLine(new TextObject("{=oI5H6Xo8}Don't think we won't fight you if you try and stop us.[if:convo_mocking_aristocratic]", null), null, null)
					.BeginPlayerOptions()
					.PlayerOption(new TextObject("{=hIFazIcK}So be it!", null), null)
					.NpcLine(new TextObject("{=KKeRi477}All right, lads. Let's kill the boss.[if:convo_predatory][ib:aggressive]", null), null, null)
					.Consequence(delegate
					{
						Campaign.Current.ConversationManager.ConversationEndOneShot += this.CreateCompanyEnemyParty;
					})
					.CloseDialog()
					.PlayerOption(new TextObject("{=bm7UcuQj}No! There is no need to fight. I don't want any bloodshed... Just leave.", null), null)
					.NpcLine(new TextObject("{=1vnaskLR}It was a pleasure to work with you, chief. Farewell...[if:convo_nonchalant][ib:normal2]", null), null, null)
					.Consequence(delegate
					{
						this._companyLeftQuestWillFail = true;
					})
					.CloseDialog()
					.EndPlayerOptions()
					.CloseDialog()
					.PlayerOption(new TextObject("{=hj4vfgxk}As you wish! Good luck. ", null), null)
					.NpcLine(new TextObject("{=1vnaskLR}It was a pleasure to work with you, chief. Farewell...[if:convo_nonchalant][ib:normal2]", null), null, null)
					.Consequence(delegate
					{
						this._companyLeftQuestWillFail = true;
					})
					.CloseDialog()
					.EndPlayerOptions();
			}

			private bool CompanyDialogFromCondition()
			{
				StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, null, false);
				return this._troubleCharacterObject == CharacterObject.OneToOneConversationCharacter;
			}

			private void CreateCompanyEnemyParty()
			{
				MobileParty.MainParty.MemberRoster.AddToCounts(this._troubleCharacterObject, -this._companyTroopCount, false, 0, 0, true, -1);
				Settlement settlement = SettlementHelper.FindRandomSettlement((Settlement x) => x.IsHideout);
				this._companyOfTroubleParty = BanditPartyComponent.CreateBanditParty("company_of_trouble_" + base.StringId, settlement.OwnerClan, settlement.Hideout, false);
				TextObject textObject = new TextObject("{=PV7RHgUl}Company of Trouble", null);
				this._companyOfTroubleParty.InitializeMobilePartyAtPosition(new TroopRoster(this._companyOfTroubleParty.Party), new TroopRoster(this._companyOfTroubleParty.Party), MobileParty.MainParty.Position2D);
				this._companyOfTroubleParty.SetCustomName(textObject);
				this._companyOfTroubleParty.SetPartyUsedByQuest(true);
				this._companyOfTroubleParty.MemberRoster.AddToCounts(this._troubleCharacterObject, this._companyTroopCount, false, 0, 0, true, -1);
				this._battleWillStart = true;
			}

			internal void CompanyLeftQuestFail()
			{
				this.UpdateCompanyTroopCount();
				MobileParty.MainParty.MemberRoster.AddToCounts(this._troubleCharacterObject, -this._companyTroopCount, false, 0, 0, true, -1);
				base.AddLog(this._questFailCompanyLeft, false);
				base.CompleteQuestWithFail(null);
				this._companyLeftQuestWillFail = false;
				GameMenu.ExitToLast();
			}

			private void QuestAcceptedConsequences()
			{
				base.StartQuest();
				base.AddLog(this._playerStartsQuestLogText, false);
				MobileParty.MainParty.MemberRoster.AddToCounts(this._troubleCharacterObject, this._companyTroopCount, false, 0, 0, true, -1);
				MBInformationManager.AddQuickInformation(new TextObject("{=jGIxKb99}Mercenaries have joined your party.", null), 0, null, "");
			}

			protected override void RegisterEvents()
			{
				CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new Action(this.DailyTick));
				CampaignEvents.MapEventEnded.AddNonSerializedListener(this, new Action<MapEvent>(this.OnMapEventEnded));
				CampaignEvents.WarDeclared.AddNonSerializedListener(this, new Action<IFaction, IFaction, DeclareWarAction.DeclareWarDetail>(this.OnWarDeclared));
				CampaignEvents.OnClanChangedKingdomEvent.AddNonSerializedListener(this, new Action<Clan, Kingdom, Kingdom, ChangeKingdomAction.ChangeKingdomActionDetail, bool>(this.OnClanChangedKingdom));
				CampaignEvents.MapEventStarted.AddNonSerializedListener(this, new Action<MapEvent, PartyBase, PartyBase>(this.OnMapEventStarted));
			}

			private void OnMapEventStarted(MapEvent mapEvent, PartyBase attackerParty, PartyBase defenderParty)
			{
				if (QuestHelper.CheckMinorMajorCoercion(this, mapEvent, attackerParty))
				{
					QuestHelper.ApplyGenericMinorMajorCoercionConsequences(this, mapEvent);
				}
			}

			private void OnClanChangedKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail detail, bool showNotification = true)
			{
				if (base.QuestGiver.MapFaction.IsAtWarWith(Hero.MainHero.MapFaction))
				{
					base.CompleteQuestWithCancel(this._questCanceledWarDeclared);
				}
			}

			private void OnWarDeclared(IFaction faction1, IFaction faction2, DeclareWarAction.DeclareWarDetail detail)
			{
				QuestHelper.CheckWarDeclarationAndFailOrCancelTheQuest(this, faction1, faction2, detail, this._playerDeclaredWarQuestLogText, this._questCanceledWarDeclared, false);
			}

			private void OnMapEventEnded(MapEvent mapEvent)
			{
				if ((mapEvent.IsPlayerMapEvent || mapEvent.IsPlayerSimulation) && !this._checkForBattleResults)
				{
					this.UpdateCompanyTroopCount();
					if (this._companyTroopCount == 0)
					{
						base.AddLog(this._allCompanyDiedLogText, false);
						this.RelationshipChangeWithQuestGiver = 5;
						GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, this.RewardGold, false);
						base.CompleteQuestWithSuccess();
					}
				}
			}

			protected override void HourlyTick()
			{
				if (base.IsOngoing)
				{
					this.UpdateCompanyTroopCount();
					if (MobileParty.MainParty.MemberRoster.TotalManCount - this._companyTroopCount <= this._companyTroopCount && MapEvent.PlayerMapEvent == null && Settlement.CurrentSettlement == null && PlayerEncounter.Current == null && !Hero.MainHero.IsWounded)
					{
						this._triggerCompanyOfTroubleConversation = true;
						GameMenu.ActivateGameMenu("company_of_trouble_menu");
					}
				}
			}

			private void TryToStealItemFromPlayer()
			{
				bool flag = false;
				for (int i = 0; i < MobileParty.MainParty.ItemRoster.Count; i++)
				{
					ItemRosterElement itemRosterElement = MobileParty.MainParty.ItemRoster[i];
					ItemObject item = itemRosterElement.EquipmentElement.Item;
					if (!itemRosterElement.IsEmpty && item.IsFood)
					{
						MobileParty.MainParty.ItemRoster.AddToCounts(item, -1);
						flag = true;
						break;
					}
				}
				if (flag)
				{
					if (this._thieveryCount == 0 || this._thieveryCount == 1)
					{
						InformationManager.ShowInquiry(new InquiryData(this.Title.ToString(), (this._thieveryCount == 0) ? new TextObject("{=OKpwA8Az}Your men have noticed some of the goods in the baggage train are missing.", null).ToString() : new TextObject("{=acu1wTeq}Your men are sure of that some of the goods were stolen from the baggage train.", null).ToString(), true, false, GameTexts.FindText("str_ok", null).ToString(), "", null, null, "", 0f, null, null, null), true, false);
					}
					else
					{
						MBInformationManager.AddQuickInformation(new TextObject("{=xlm8oYhM}Your men reported that some of the goods were stolen from the baggage train.", null), 0, null, "");
					}
					this._thieveryCount++;
				}
			}

			public void DailyTick()
			{
				if (MBRandom.RandomFloat > 0.5f)
				{
					this.TryToStealItemFromPlayer();
				}
			}

			private void UpdateCompanyTroopCount()
			{
				bool flag = false;
				foreach (TroopRosterElement troopRosterElement in MobileParty.MainParty.MemberRoster.GetTroopRoster())
				{
					if (troopRosterElement.Character == this._troubleCharacterObject)
					{
						flag = true;
						this._companyTroopCount = troopRosterElement.Number;
						break;
					}
				}
				if (!flag)
				{
					this._companyTroopCount = 0;
				}
			}

			internal void QuestSuccessWithPlayerDefeatedCompany()
			{
				base.AddLog(this._allCompanyDiedLogText, false);
				this.RelationshipChangeWithQuestGiver = 5;
				GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, this.RewardGold, false);
				base.CompleteQuestWithSuccess();
			}

			internal void QuestFailWithPlayerDefeatedAgainstCompany()
			{
				base.AddLog(this._playerDefeatedAgainstCompany, false);
				base.CompleteQuestWithFail(null);
			}

			protected override void OnFinalize()
			{
				this.UpdateCompanyTroopCount();
				if (this._companyTroopCount > 0)
				{
					MobileParty.MainParty.MemberRoster.AddToCounts(this._troubleCharacterObject, -this._companyTroopCount, false, 0, 0, true, -1);
				}
			}

			private const string TroubleCharacterObjectStringId = "company_of_trouble_character";

			private int _companyTroopCount;

			[SaveableField(20)]
			internal MobileParty _companyOfTroubleParty;

			[SaveableField(30)]
			internal bool _battleWillStart;

			internal bool _checkForBattleResults;

			[SaveableField(40)]
			private int _thieveryCount;

			[SaveableField(80)]
			internal bool _triggerCompanyOfTroubleConversation;

			[SaveableField(50)]
			private int _demandGold;

			internal CharacterObject _troubleCharacterObject;

			private PersuasionTask[] _tasks;

			private PersuasionTask _selectedTask;

			private const PersuasionDifficulty Difficulty = PersuasionDifficulty.Hard;

			[SaveableField(70)]
			private List<Hero> _persuationTriedHeroesList;

			internal bool _companyLeftQuestWillFail;
		}

		public class LandLordCompanyOfTroubleIssueTypeDefiner : SaveableTypeDefiner
		{
			public LandLordCompanyOfTroubleIssueTypeDefiner()
				: base(4800000)
			{
			}

			protected override void DefineClassTypes()
			{
				base.AddClassDefinition(typeof(LandLordCompanyOfTroubleIssueBehavior.LandLordCompanyOfTroubleIssue), 1, null);
				base.AddClassDefinition(typeof(LandLordCompanyOfTroubleIssueBehavior.LandLordCompanyOfTroubleIssueQuest), 2, null);
			}
		}
	}
}
