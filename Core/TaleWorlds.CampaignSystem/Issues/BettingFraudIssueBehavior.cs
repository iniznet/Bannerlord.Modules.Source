using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Overlay;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.TournamentGames;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.Issues
{
	public class BettingFraudIssueBehavior : CampaignBehaviorBase
	{
		private static BettingFraudIssueBehavior.BettingFraudQuest Instance
		{
			get
			{
				BettingFraudIssueBehavior campaignBehavior = Campaign.Current.GetCampaignBehavior<BettingFraudIssueBehavior>();
				if (campaignBehavior._cachedQuest != null && campaignBehavior._cachedQuest.IsOngoing)
				{
					return campaignBehavior._cachedQuest;
				}
				using (List<QuestBase>.Enumerator enumerator = Campaign.Current.QuestManager.Quests.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						BettingFraudIssueBehavior.BettingFraudQuest bettingFraudQuest;
						if ((bettingFraudQuest = enumerator.Current as BettingFraudIssueBehavior.BettingFraudQuest) != null)
						{
							campaignBehavior._cachedQuest = bettingFraudQuest;
							return campaignBehavior._cachedQuest;
						}
					}
				}
				return null;
			}
		}

		public override void RegisterEvents()
		{
			CampaignEvents.OnCheckForIssueEvent.AddNonSerializedListener(this, new Action<Hero>(this.CheckForIssue));
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
		}

		private void OnSessionLaunched(CampaignGameStarter gameStarter)
		{
			gameStarter.AddGameMenu("menu_town_tournament_join_betting_fraud", "{=5Adr6toM}{MENU_TEXT}", new OnInitDelegate(this.game_menu_tournament_join_on_init), GameOverlays.MenuOverlayType.SettlementWithBoth, GameMenu.MenuFlags.None, null);
			gameStarter.AddGameMenuOption("menu_town_tournament_join_betting_fraud", "mno_tournament_event_1", "{=es0Y3Bxc}Join", delegate(MenuCallbackArgs args)
			{
				args.optionLeaveType = GameMenuOption.LeaveType.Mission;
				args.OptionQuestData = GameMenuOption.IssueQuestFlags.ActiveIssue;
				return true;
			}, new GameMenuOption.OnConsequenceDelegate(this.game_menu_tournament_join_current_game_on_consequence), false, -1, false, null);
			gameStarter.AddGameMenuOption("menu_town_tournament_join_betting_fraud", "mno_tournament_leave", "{=3sRdGQou}Leave", delegate(MenuCallbackArgs args)
			{
				args.optionLeaveType = GameMenuOption.LeaveType.Leave;
				return true;
			}, delegate(MenuCallbackArgs args)
			{
				GameMenu.SwitchToMenu("town_arena");
			}, true, -1, false, null);
		}

		private void game_menu_tournament_join_on_init(MenuCallbackArgs args)
		{
			TournamentGame tournamentGame = Campaign.Current.TournamentManager.GetTournamentGame(Settlement.CurrentSettlement.Town);
			tournamentGame.UpdateTournamentPrize(true, false);
			GameTexts.SetVariable("MENU_TEXT", tournamentGame.GetMenuText());
		}

		private void game_menu_tournament_join_current_game_on_consequence(MenuCallbackArgs args)
		{
			CampaignMapConversation.OpenConversation(new ConversationCharacterData(CharacterObject.PlayerCharacter, null, false, false, false, false, false, false), new ConversationCharacterData(BettingFraudIssueBehavior.Instance._thug, null, false, false, false, false, false, false));
		}

		[GameMenuInitializationHandler("menu_town_tournament_join_betting_fraud")]
		private static void game_menu_ui_town_ui_on_init(MenuCallbackArgs args)
		{
			Settlement currentSettlement = Settlement.CurrentSettlement;
			args.MenuContext.SetBackgroundMeshName(currentSettlement.Town.WaitMeshName);
		}

		public override void SyncData(IDataStore dataStore)
		{
		}

		private void CheckForIssue(Hero hero)
		{
			if (this.ConditionsHold(hero))
			{
				Campaign.Current.IssueManager.AddPotentialIssueData(hero, new PotentialIssueData(new PotentialIssueData.StartIssueDelegate(this.OnStartIssue), typeof(BettingFraudIssueBehavior.BettingFraudIssue), IssueBase.IssueFrequency.Rare, null));
				return;
			}
			Campaign.Current.IssueManager.AddPotentialIssueData(hero, new PotentialIssueData(typeof(BettingFraudIssueBehavior.BettingFraudIssue), IssueBase.IssueFrequency.Rare));
		}

		private bool ConditionsHold(Hero issueGiver)
		{
			return issueGiver.IsGangLeader && issueGiver.CurrentSettlement != null && issueGiver.CurrentSettlement.Town != null && issueGiver.CurrentSettlement.Town.Security < 50f;
		}

		private IssueBase OnStartIssue(in PotentialIssueData pid, Hero issueOwner)
		{
			return new BettingFraudIssueBehavior.BettingFraudIssue(issueOwner);
		}

		private const IssueBase.IssueFrequency BettingFraudIssueFrequency = IssueBase.IssueFrequency.Rare;

		private const string JoinTournamentMenuId = "menu_town_tournament_join";

		private const string JoinTournamentForBettingFraudQuestMenuId = "menu_town_tournament_join_betting_fraud";

		private const int SettlementSecurityLimit = 50;

		private BettingFraudIssueBehavior.BettingFraudQuest _cachedQuest;

		public class BettingFraudIssue : IssueBase
		{
			internal static void AutoGeneratedStaticCollectObjectsBettingFraudIssue(object o, List<object> collectedObjects)
			{
				((BettingFraudIssueBehavior.BettingFraudIssue)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			public override TextObject IssueBriefByIssueGiver
			{
				get
				{
					return new TextObject("{=kru5Vpog}Yes. I'm glad to have the chance to talk to you. I keep an eye on the careers of champions like yourself for professional reasons, and I have a proposal that might interest a good fighter like you. Interested?[ib:confident3][if:convo_bemused]", null);
				}
			}

			public override TextObject IssueAcceptByPlayer
			{
				get
				{
					return new TextObject("{=YWXkgDSd}What kind of a partnership are we talking about?", null);
				}
			}

			public override TextObject IssueQuestSolutionExplanationByIssueGiver
			{
				get
				{
					return new TextObject("{=vLaoZhkF}I follow tournaments, you see, and like to both place and take bets. But of course I need someone who can not only win those tournaments but lose if necessary... if you understand what I mean. Not all the time. That would be too obvious. Here's what I propose. We enter into a partnership for five tournaments. Don't bother memorizing which ones you win and which ones you lose. Before each fight, an associate of my mine will let you know how you should place. Follow my instructions and I promise you will be rewarded handsomely. What do you say?[if:convo_bemused][ib:demure2]", null);
				}
			}

			public override TextObject IssueQuestSolutionAcceptByPlayer
			{
				get
				{
					return new TextObject("{=cL9BX7ph}As long as the payment is good, I agree.", null);
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

			public override TextObject Title
			{
				get
				{
					return new TextObject("{=xhVrxgC4}Betting Fraud", null);
				}
			}

			public override TextObject Description
			{
				get
				{
					TextObject textObject = new TextObject("{=3j8pV58L}{ISSUE_GIVER.NAME} offers you a deal to fix {TOURNAMENT_COUNT} tournaments and share the profit from the bet winnings.", null);
					textObject.SetCharacterProperties("ISSUE_GIVER", base.IssueOwner.CharacterObject, false);
					textObject.SetTextVariable("TOURNAMENT_COUNT", 5);
					return textObject;
				}
			}

			public BettingFraudIssue(Hero issueOwner)
				: base(issueOwner, CampaignTime.DaysFromNow(45f))
			{
			}

			protected override float GetIssueEffectAmountInternal(IssueEffect issueEffect)
			{
				if (issueEffect == DefaultIssueEffects.IssueOwnerPower)
				{
					return -0.2f;
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
				return new BettingFraudIssueBehavior.BettingFraudQuest(questId, base.IssueOwner, CampaignTime.DaysFromNow(45f), 0);
			}

			public override IssueBase.IssueFrequency GetFrequency()
			{
				return IssueBase.IssueFrequency.Rare;
			}

			protected override bool CanPlayerTakeQuestConditions(Hero issueOwner, out IssueBase.PreconditionFlags flag, out Hero relationHero, out SkillObject skill)
			{
				relationHero = null;
				skill = null;
				flag = IssueBase.PreconditionFlags.None;
				if (Clan.PlayerClan.Renown < 50f)
				{
					flag |= IssueBase.PreconditionFlags.Renown;
				}
				if (issueOwner.GetRelationWithPlayer() < -10f)
				{
					flag |= IssueBase.PreconditionFlags.Relation;
					relationHero = issueOwner;
				}
				if (Hero.MainHero.GetSkillValue(DefaultSkills.OneHanded) < 50 && Hero.MainHero.GetSkillValue(DefaultSkills.TwoHanded) < 50 && Hero.MainHero.GetSkillValue(DefaultSkills.Polearm) < 50 && Hero.MainHero.GetSkillValue(DefaultSkills.Bow) < 50 && Hero.MainHero.GetSkillValue(DefaultSkills.Crossbow) < 50 && Hero.MainHero.GetSkillValue(DefaultSkills.Throwing) < 50)
				{
					if (Hero.MainHero.GetSkillValue(DefaultSkills.OneHanded) < 50)
					{
						flag |= IssueBase.PreconditionFlags.Skill;
						skill = DefaultSkills.OneHanded;
					}
					else if (Hero.MainHero.GetSkillValue(DefaultSkills.TwoHanded) < 50)
					{
						flag |= IssueBase.PreconditionFlags.Skill;
						skill = DefaultSkills.TwoHanded;
					}
					else if (Hero.MainHero.GetSkillValue(DefaultSkills.Polearm) < 50)
					{
						flag |= IssueBase.PreconditionFlags.Skill;
						skill = DefaultSkills.Polearm;
					}
					else if (Hero.MainHero.GetSkillValue(DefaultSkills.Bow) < 50)
					{
						flag |= IssueBase.PreconditionFlags.Skill;
						skill = DefaultSkills.Bow;
					}
					else if (Hero.MainHero.GetSkillValue(DefaultSkills.Crossbow) < 50)
					{
						flag |= IssueBase.PreconditionFlags.Skill;
						skill = DefaultSkills.Crossbow;
					}
					else if (Hero.MainHero.GetSkillValue(DefaultSkills.Throwing) < 50)
					{
						flag |= IssueBase.PreconditionFlags.Skill;
						skill = DefaultSkills.Throwing;
					}
				}
				return flag == IssueBase.PreconditionFlags.None;
			}

			public override bool IssueStayAliveConditions()
			{
				return true;
			}

			protected override void CompleteIssueWithTimedOutConsequences()
			{
			}

			private const int NeededTournamentCount = 5;

			private const int IssueDuration = 45;

			private const int MainHeroSkillLimit = 50;

			private const int MainClanRenownLimit = 50;

			private const int RelationLimitWithIssueOwner = -10;

			private const float IssueOwnerPowerPenaltyForIssueEffect = -0.2f;
		}

		public class BettingFraudQuest : QuestBase
		{
			internal static void AutoGeneratedStaticCollectObjectsBettingFraudQuest(object o, List<object> collectedObjects)
			{
				((BettingFraudIssueBehavior.BettingFraudQuest)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				base.AutoGeneratedInstanceCollectObjects(collectedObjects);
				collectedObjects.Add(this._thug);
				collectedObjects.Add(this._startLog);
				collectedObjects.Add(this._counterOfferNotable);
			}

			internal static object AutoGeneratedGetMemberValue_thug(object o)
			{
				return ((BettingFraudIssueBehavior.BettingFraudQuest)o)._thug;
			}

			internal static object AutoGeneratedGetMemberValue_startLog(object o)
			{
				return ((BettingFraudIssueBehavior.BettingFraudQuest)o)._startLog;
			}

			internal static object AutoGeneratedGetMemberValue_counterOfferNotable(object o)
			{
				return ((BettingFraudIssueBehavior.BettingFraudQuest)o)._counterOfferNotable;
			}

			internal static object AutoGeneratedGetMemberValue_fixedTournamentCount(object o)
			{
				return ((BettingFraudIssueBehavior.BettingFraudQuest)o)._fixedTournamentCount;
			}

			internal static object AutoGeneratedGetMemberValue_minorOffensiveCount(object o)
			{
				return ((BettingFraudIssueBehavior.BettingFraudQuest)o)._minorOffensiveCount;
			}

			internal static object AutoGeneratedGetMemberValue_counterOfferConversationDone(object o)
			{
				return ((BettingFraudIssueBehavior.BettingFraudQuest)o)._counterOfferConversationDone;
			}

			public override TextObject Title
			{
				get
				{
					return new TextObject("{=xhVrxgC4}Betting Fraud", null);
				}
			}

			public override bool IsRemainingTimeHidden
			{
				get
				{
					return false;
				}
			}

			private TextObject StartLog
			{
				get
				{
					TextObject textObject = new TextObject("{=6rweIvZS}{QUEST_GIVER.LINK}, a gang leader from {SETTLEMENT} offers you to fix 5 tournaments together and share the profit.\n {?QUEST_GIVER.GENDER}She{?}He{\\?} asked you to enter 5 tournaments and follow the instructions given by {?QUEST_GIVER.GENDER}her{?}his{\\?} associate.", null);
					textObject.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, false);
					textObject.SetTextVariable("SETTLEMENT", base.QuestGiver.CurrentSettlement.EncyclopediaLinkWithName);
					return textObject;
				}
			}

			private TextObject CurrentDirectiveLog
			{
				get
				{
					TextObject textObject = new TextObject("{=dnZekyZI}Directive from {QUEST_GIVER.LINK}: {DIRECTIVE}", null);
					textObject.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, false);
					textObject.SetTextVariable("DIRECTIVE", this.GetDirectiveText());
					return textObject;
				}
			}

			private TextObject QuestFailedWithTimeOutLog
			{
				get
				{
					TextObject textObject = new TextObject("{=2brAaeFh}You failed to complete tournaments in time. {QUEST_GIVER.LINK} will certainly be disappointed.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					return textObject;
				}
			}

			public BettingFraudQuest(string questId, Hero questGiver, CampaignTime duration, int rewardGold)
				: base(questId, questGiver, duration, rewardGold)
			{
				this._counterOfferNotable = null;
				this._fixedTournamentCount = 0;
				this._minorOffensiveCount = 0;
				this._counterOfferAccepted = false;
				this._readyToStartTournament = false;
				this._startTournamentEndConversation = false;
				this._counterOfferConversationDone = false;
				this._currentDirective = BettingFraudIssueBehavior.BettingFraudQuest.Directives.None;
				this._afterTournamentConversationState = BettingFraudIssueBehavior.BettingFraudQuest.AfterTournamentConversationState.None;
				this._thug = MBObjectManager.Instance.GetObject<CharacterObject>((MBRandom.RandomFloat > 0.5f) ? "betting_fraud_thug_male" : "betting_fraud_thug_female");
				this._startLog = null;
				this.SetDialogs();
				base.InitializeQuestOnCreation();
			}

			protected override void InitializeQuestOnGameLoad()
			{
				this.SetDialogs();
			}

			protected override void HourlyTick()
			{
			}

			private void SelectCounterOfferNotable(Settlement settlement)
			{
				this._counterOfferNotable = settlement.Notables.GetRandomElement<Hero>();
			}

			private void IncreaseMinorOffensive()
			{
				this._minorOffensiveCount++;
				this._currentDirective = BettingFraudIssueBehavior.BettingFraudQuest.Directives.None;
				if (this._minorOffensiveCount >= 2)
				{
					this._afterTournamentConversationState = BettingFraudIssueBehavior.BettingFraudQuest.AfterTournamentConversationState.SecondMinorOffense;
					return;
				}
				this._afterTournamentConversationState = BettingFraudIssueBehavior.BettingFraudQuest.AfterTournamentConversationState.MinorOffense;
			}

			private void IncreaseFixedTournamentCount()
			{
				this._fixedTournamentCount++;
				this._startLog.UpdateCurrentProgress(this._fixedTournamentCount);
				this._currentDirective = BettingFraudIssueBehavior.BettingFraudQuest.Directives.None;
				if (this._fixedTournamentCount >= 5)
				{
					this._afterTournamentConversationState = BettingFraudIssueBehavior.BettingFraudQuest.AfterTournamentConversationState.BigReward;
					return;
				}
				this._afterTournamentConversationState = BettingFraudIssueBehavior.BettingFraudQuest.AfterTournamentConversationState.SmallReward;
			}

			private void SetCurrentDirective()
			{
				this._currentDirective = ((MBRandom.RandomFloat <= 0.33f) ? BettingFraudIssueBehavior.BettingFraudQuest.Directives.LoseAt3RdRound : ((MBRandom.RandomFloat < 0.5f) ? BettingFraudIssueBehavior.BettingFraudQuest.Directives.LoseAt4ThRound : BettingFraudIssueBehavior.BettingFraudQuest.Directives.WinTheTournament));
				base.AddLog(this.CurrentDirectiveLog, false);
			}

			private void StartTournamentMission()
			{
				TournamentGame tournamentGame = Campaign.Current.TournamentManager.GetTournamentGame(Settlement.CurrentSettlement.Town);
				GameMenu.SwitchToMenu("town");
				tournamentGame.PrepareForTournamentGame(true);
				Campaign.Current.TournamentManager.OnPlayerJoinTournament(tournamentGame.GetType(), Settlement.CurrentSettlement);
			}

			protected override void RegisterEvents()
			{
				CampaignEvents.PlayerEliminatedFromTournament.AddNonSerializedListener(this, new Action<int, Town>(this.OnPlayerEliminatedFromTournament));
				CampaignEvents.TournamentFinished.AddNonSerializedListener(this, new Action<CharacterObject, MBReadOnlyList<CharacterObject>, Town, ItemObject>(this.OnTournamentFinished));
				CampaignEvents.GameMenuOpened.AddNonSerializedListener(this, new Action<MenuCallbackArgs>(this.OnGameMenuOpened));
			}

			private void OnPlayerEliminatedFromTournament(int round, Town town)
			{
				this._startTournamentEndConversation = true;
				if (round == (int)this._currentDirective)
				{
					this.IncreaseFixedTournamentCount();
					return;
				}
				if (round < (int)this._currentDirective)
				{
					this.IncreaseMinorOffensive();
					return;
				}
				if (round > (int)this._currentDirective)
				{
					this._afterTournamentConversationState = BettingFraudIssueBehavior.BettingFraudQuest.AfterTournamentConversationState.MajorOffense;
				}
			}

			private void OnTournamentFinished(CharacterObject winner, MBReadOnlyList<CharacterObject> participants, Town town, ItemObject prize)
			{
				if (participants.Contains(CharacterObject.PlayerCharacter) && this._currentDirective != BettingFraudIssueBehavior.BettingFraudQuest.Directives.None)
				{
					this._startTournamentEndConversation = true;
					if (this._currentDirective == BettingFraudIssueBehavior.BettingFraudQuest.Directives.WinTheTournament)
					{
						if (winner == CharacterObject.PlayerCharacter)
						{
							this.IncreaseFixedTournamentCount();
							return;
						}
						this.IncreaseMinorOffensive();
						return;
					}
					else if (winner == CharacterObject.PlayerCharacter)
					{
						this._afterTournamentConversationState = BettingFraudIssueBehavior.BettingFraudQuest.AfterTournamentConversationState.MajorOffense;
					}
				}
			}

			private void OnGameMenuOpened(MenuCallbackArgs args)
			{
				if (args.MenuContext.GameMenu.StringId == "menu_town_tournament_join")
				{
					GameMenu.SwitchToMenu("menu_town_tournament_join_betting_fraud");
				}
				if (args.MenuContext.GameMenu.StringId == "menu_town_tournament_join_betting_fraud")
				{
					if (this._readyToStartTournament)
					{
						if (this._fixedTournamentCount == 4 && !this._counterOfferConversationDone && this._counterOfferNotable != null && this._currentDirective != BettingFraudIssueBehavior.BettingFraudQuest.Directives.WinTheTournament)
						{
							CampaignMapConversation.OpenConversation(new ConversationCharacterData(CharacterObject.PlayerCharacter, null, false, false, false, false, false, false), new ConversationCharacterData(this._counterOfferNotable.CharacterObject, null, false, false, false, false, false, false));
						}
						else
						{
							this.StartTournamentMission();
							this._readyToStartTournament = false;
						}
					}
					if (this._fixedTournamentCount == 4 && (this._counterOfferNotable == null || this._counterOfferNotable.CurrentSettlement != Settlement.CurrentSettlement))
					{
						this.SelectCounterOfferNotable(Settlement.CurrentSettlement);
					}
				}
				if (this._startTournamentEndConversation)
				{
					CampaignMapConversation.OpenConversation(new ConversationCharacterData(CharacterObject.PlayerCharacter, null, false, false, false, false, false, false), new ConversationCharacterData(this._thug, null, false, false, false, false, false, false));
				}
			}

			protected override void OnTimedOut()
			{
				base.OnTimedOut();
				this.PlayerDidNotCompleteTournaments();
			}

			protected override void SetDialogs()
			{
				this.OfferDialogFlow = this.GetOfferDialogFlow();
				this.DiscussDialogFlow = this.GetDiscussDialogFlow();
				Campaign.Current.ConversationManager.AddDialogFlow(this.GetDialogWithThugStart(), this);
				Campaign.Current.ConversationManager.AddDialogFlow(this.GetDialogWithThugEnd(), this);
				Campaign.Current.ConversationManager.AddDialogFlow(this.GetCounterOfferDialog(), this);
			}

			private DialogFlow GetOfferDialogFlow()
			{
				return DialogFlow.CreateDialogFlow("issue_classic_quest_start", 100).NpcLine(new TextObject("{=sp52g5AQ}Very good, very good. Try to enter five tournaments over the next 45 days or so. Right before the fight you'll hear from my associate how far I want you to go in the rankings before you lose.[if:convo_delighted][ib:hip]", null), null, null).Condition(() => Hero.OneToOneConversationHero == base.QuestGiver)
					.NpcLine(new TextObject("{=ADIYnC4u}Now, I know you can't win every fight, so if you underperform once or twice, I'd understand. But if you lose every time, or worse, if you overperform, well, then I'll be a bit angry.[if:convo_nonchalant][ib:normal2]", null), null, null)
					.NpcLine(new TextObject("{=1hOPCf8I}But I'm sure you won't disappoint me. Enjoy your riches![if:convo_focused_happy][ib:confident]", null), null, null)
					.Consequence(new ConversationSentence.OnConsequenceDelegate(this.OfferDialogFlowConsequence))
					.CloseDialog();
			}

			private void OfferDialogFlowConsequence()
			{
				base.StartQuest();
				this._startLog = base.AddDiscreteLog(this.StartLog, new TextObject("{=dLfWFa61}Fix 5 Tournaments", null), 0, 5, null, false);
			}

			private DialogFlow GetDiscussDialogFlow()
			{
				return DialogFlow.CreateDialogFlow("quest_discuss", 100).NpcLine(new TextObject("{=!}{RESPONSE_TEXT}", null), null, null).Condition(new ConversationSentence.OnConditionDelegate(this.DiscussDialogCondition))
					.BeginPlayerOptions()
					.PlayerOption(new TextObject("{=abLgPWzf}I will continue to honor our deal. Do not forget to do your end, that's all.", null), null)
					.BeginNpcOptions()
					.NpcOption(new TextObject("{=ZLPEsMUx}Well, there are tournament happening in {NEARBY_TOURNAMENTS_LIST} right now. You can go there and do the job. Your denars will be waiting for you.", null), new ConversationSentence.OnConditionDelegate(this.NpcTournamentLocationCondition), null, null)
					.CloseDialog()
					.NpcDefaultOption("{=sUfSCLQx}Sadly, I've heard no news of an upcoming tournament. I am sure one will be held before too long.")
					.CloseDialog()
					.EndNpcOptions()
					.CloseDialog()
					.PlayerOption(new TextObject("{=XUS5wNsD}I feel like I do all the job and you get your denars.", null), null)
					.BeginNpcOptions()
					.NpcOption(new TextObject("{=ZLPEsMUx}Well, there are tournament happening in {NEARBY_TOURNAMENTS_LIST} right now. You can go there and do the job. Your denars will be waiting for you.", null), new ConversationSentence.OnConditionDelegate(this.NpcTournamentLocationCondition), null, null)
					.CloseDialog()
					.NpcDefaultOption("{=sUfSCLQx}Sadly, I've heard no news of an upcoming tournament. I am sure one will be held before too long.")
					.CloseDialog()
					.EndNpcOptions()
					.CloseDialog()
					.EndPlayerOptions()
					.CloseDialog();
			}

			private bool DiscussDialogCondition()
			{
				bool flag = Hero.OneToOneConversationHero == base.QuestGiver;
				if (flag)
				{
					if (this._minorOffensiveCount > 0)
					{
						MBTextManager.SetTextVariable("RESPONSE_TEXT", new TextObject("{=7SPwGYvf}I had expected better of you. But even the best can fail sometimes. Just make sure it does not happen again.[if:convo_bored][ib:closed2] ", null), false);
						return flag;
					}
					MBTextManager.SetTextVariable("RESPONSE_TEXT", new TextObject("{=vo0uhUsZ}I have high hopes for you, friend. Just follow my directives and we will be rich.[if:convo_relaxed_happy][ib:demure2]", null), false);
				}
				return flag;
			}

			private bool NpcTournamentLocationCondition()
			{
				List<Town> list = Town.AllTowns.Where((Town x) => Campaign.Current.TournamentManager.GetTournamentGame(x) != null && x != Settlement.CurrentSettlement.Town).ToList<Town>();
				list = list.OrderBy((Town x) => x.Settlement.Position2D.DistanceSquared(Settlement.CurrentSettlement.Position2D)).ToList<Town>();
				if (list.Count > 0)
				{
					MBTextManager.SetTextVariable("NEARBY_TOURNAMENTS_LIST", list[0].Name, false);
					return true;
				}
				return false;
			}

			private DialogFlow GetDialogWithThugStart()
			{
				return DialogFlow.CreateDialogFlow("start", 125).NpcLine(new TextObject("{=!}{GREETING_LINE}", null), null, null).Condition(new ConversationSentence.OnConditionDelegate(this.DialogWithThugStartCondition))
					.BeginPlayerOptions()
					.PlayerOption(new TextObject("{=!}{POSITIVE_OPTION}", null), null)
					.Condition(new ConversationSentence.OnConditionDelegate(this.PositiveOptionCondition))
					.Consequence(new ConversationSentence.OnConsequenceDelegate(this.PositiveOptionConsequences))
					.CloseDialog()
					.PlayerOption(new TextObject("{=!}{NEGATIVE_OPTION}", null), null)
					.Condition(new ConversationSentence.OnConditionDelegate(this.NegativeOptionCondition))
					.Consequence(new ConversationSentence.OnConsequenceDelegate(this.NegativeOptionConsequence))
					.CloseDialog()
					.EndPlayerOptions()
					.CloseDialog();
			}

			private bool DialogWithThugStartCondition()
			{
				bool flag = CharacterObject.OneToOneConversationCharacter == this._thug && !this._startTournamentEndConversation;
				if (flag)
				{
					this.SetCurrentDirective();
					if (this._fixedTournamentCount < 2)
					{
						TextObject textObject = new TextObject("{=xYu4yVRU}Hey there friend. So... You don't need to know my name, but suffice to say that we're both friends of {QUEST_GIVER.LINK}. Here's {?QUEST_GIVER.GENDER}her{?}his{\\?} message for you: {DIRECTIVE}.[ib:confident][if:convo_nonchalant]", null);
						textObject.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, false);
						textObject.SetTextVariable("DIRECTIVE", this.GetDirectiveText());
						MBTextManager.SetTextVariable("GREETING_LINE", textObject, false);
						return flag;
					}
					if (this._fixedTournamentCount < 4)
					{
						TextObject textObject2 = new TextObject("{=cQE9tQOy}My friend! Good to see you. You did very well in that last fight. People definitely won't be expecting you to \"{DIRECTIVE}\". What a surprise that would be. Well, I should not keep you from your tournament. You know what to do.[if:convo_happy][ib:closed2]", null);
						textObject2.SetTextVariable("DIRECTIVE", this.GetDirectiveText());
						MBTextManager.SetTextVariable("GREETING_LINE", textObject2, false);
						return flag;
					}
					TextObject textObject3 = new TextObject("{=RVLPQ4rm}My friend. I am almost sad that these meetings are going to come to an end. Well, a deal is a deal. I won't beat around the bush. Here's your final message: {DIRECTIVE}. I wish you luck, right up until the moment that you have to go down.[if:convo_mocking_teasing][ib:closed]", null);
					textObject3.SetTextVariable("DIRECTIVE", this.GetDirectiveText());
					MBTextManager.SetTextVariable("GREETING_LINE", textObject3, false);
				}
				return flag;
			}

			private bool PositiveOptionCondition()
			{
				if (this._fixedTournamentCount < 2)
				{
					MBTextManager.SetTextVariable("POSITIVE_OPTION", new TextObject("{=PrUauabl}As long as the payment is as we talked, you got nothing to worry about.", null), false);
				}
				else if (this._fixedTournamentCount < 4)
				{
					MBTextManager.SetTextVariable("POSITIVE_OPTION", new TextObject("{=TKRsPVMU}Yes, I did. Be around when the tournament is over.", null), false);
				}
				else
				{
					MBTextManager.SetTextVariable("POSITIVE_OPTION", new TextObject("{=26XPQw2v}I will miss this little deal we had. See you at the end", null), false);
				}
				return true;
			}

			private void PositiveOptionConsequences()
			{
				this._readyToStartTournament = true;
			}

			private bool NegativeOptionCondition()
			{
				bool flag = this._fixedTournamentCount >= 4;
				if (flag)
				{
					MBTextManager.SetTextVariable("NEGATIVE_OPTION", new TextObject("{=vapdvRQO}This deal was a mistake. We will not talk again after this last tournament.", null), false);
				}
				return flag;
			}

			private void NegativeOptionConsequence()
			{
				this._readyToStartTournament = true;
			}

			private TextObject GetDirectiveText()
			{
				if (this._currentDirective == BettingFraudIssueBehavior.BettingFraudQuest.Directives.LoseAt3RdRound)
				{
					return new TextObject("{=aHlcBLYB}Lose this tournament at 3rd round", null);
				}
				if (this._currentDirective == BettingFraudIssueBehavior.BettingFraudQuest.Directives.LoseAt4ThRound)
				{
					return new TextObject("{=hc1mnqOx}Lose this tournament at 4th round", null);
				}
				if (this._currentDirective == BettingFraudIssueBehavior.BettingFraudQuest.Directives.WinTheTournament)
				{
					return new TextObject("{=hl4pTsaO}Win this tournament", null);
				}
				return TextObject.Empty;
			}

			private DialogFlow GetDialogWithThugEnd()
			{
				return DialogFlow.CreateDialogFlow("start", 125).NpcLine(new TextObject("{=!}{GREETING_LINE}", null), null, null).Condition(new ConversationSentence.OnConditionDelegate(this.DialogWithThugEndCondition))
					.Consequence(new ConversationSentence.OnConsequenceDelegate(this.DialogWithThugEndConsequence))
					.CloseDialog();
			}

			private bool DialogWithThugEndCondition()
			{
				bool flag = CharacterObject.OneToOneConversationCharacter == this._thug && this._startTournamentEndConversation;
				if (flag)
				{
					TextObject textObject = TextObject.Empty;
					switch (this._afterTournamentConversationState)
					{
					case BettingFraudIssueBehavior.BettingFraudQuest.AfterTournamentConversationState.SmallReward:
						textObject = new TextObject("{=ZM8t4ZW2}We are very impressed, my friend. Here is the payment as promised. I hope we can continue this profitable partnership. See you at the next tournament.[if:convo_happy][ib:demure]", null);
						GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, 250, false);
						break;
					case BettingFraudIssueBehavior.BettingFraudQuest.AfterTournamentConversationState.BigReward:
						textObject = new TextObject("{=9vOZWY25}What an exciting result! I will definitely miss these tournaments. Well, maybe after some time goes by and memories get a little hazy we can continue. Here is the last payment. Very well deserved.[if:convo_happy][ib:demure]", null);
						break;
					case BettingFraudIssueBehavior.BettingFraudQuest.AfterTournamentConversationState.MinorOffense:
						textObject = new TextObject("{=d8bGHJnZ}This was not we were expecting. We lost some money. Well, Lady Fortune always casts her ballot too in these contests. But try to reassure us that this was her plan, and not yours, eh?[if:convo_grave][ib:closed2]", null);
						break;
					case BettingFraudIssueBehavior.BettingFraudQuest.AfterTournamentConversationState.SecondMinorOffense:
						textObject = new TextObject("{=bNAG2t8S}Well, my friend, either you're playing us false or you're just not very good at this. Either way, {QUEST_GIVER.LINK} wishes to tell you that {?QUEST_GIVER.GENDER}her{?}his{\\?} association with you is over.[if:convo_predatory][ib:closed2]", null);
						textObject.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, false);
						break;
					case BettingFraudIssueBehavior.BettingFraudQuest.AfterTournamentConversationState.MajorOffense:
						textObject = new TextObject("{=Lyqx3NYE}Well... What happened back there... That wasn't bad luck or incompetence. {QUEST_GIVER.LINK} trusted in you and {?QUEST_GIVER.GENDER}She{?}He{\\?} doesn't take well to betrayal.[if:convo_angry][ib:warrior]", null);
						textObject.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, false);
						break;
					default:
						Debug.FailedAssert("After tournament conversation state is not set!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\Issues\\BettingFraudIssueBehavior.cs", "DialogWithThugEndCondition", 722);
						break;
					}
					MBTextManager.SetTextVariable("GREETING_LINE", textObject, false);
				}
				return flag;
			}

			private void DialogWithThugEndConsequence()
			{
				this._startTournamentEndConversation = false;
				switch (this._afterTournamentConversationState)
				{
				case BettingFraudIssueBehavior.BettingFraudQuest.AfterTournamentConversationState.SmallReward:
				case BettingFraudIssueBehavior.BettingFraudQuest.AfterTournamentConversationState.MinorOffense:
					break;
				case BettingFraudIssueBehavior.BettingFraudQuest.AfterTournamentConversationState.BigReward:
					this.MainHeroSuccessfullyFixedTournaments();
					return;
				case BettingFraudIssueBehavior.BettingFraudQuest.AfterTournamentConversationState.SecondMinorOffense:
					this.MainHeroFailToFixTournaments();
					return;
				case BettingFraudIssueBehavior.BettingFraudQuest.AfterTournamentConversationState.MajorOffense:
					if (this._counterOfferAccepted)
					{
						this.MainHeroAcceptsCounterOffer();
						return;
					}
					this.MainHeroChooseNotToFixTournaments();
					break;
				default:
					return;
				}
			}

			private DialogFlow GetCounterOfferDialog()
			{
				return DialogFlow.CreateDialogFlow("start", 125).NpcLine(new TextObject("{=bUfBHSsz}Hold on a moment, friend. I need to talk to you.[ib:aggressive]", null), null, null).Condition(new ConversationSentence.OnConditionDelegate(this.CounterOfferConversationStartCondition))
					.Consequence(new ConversationSentence.OnConsequenceDelegate(this.CounterOfferConversationStartConsequence))
					.PlayerLine(new TextObject("{=PZfR7hEK}What do you want? I have a tournament to prepare for.", null), null)
					.NpcLine(new TextObject("{=GN9F316V}Oh of course you do. {QUEST_GIVER.LINK}'s people have been running around placing bets - we know all about your arrangement, you see. And let me tell you something: as these arrangements go, {QUEST_GIVER.LINK} is getting you cheap. Do you want to see real money? Win this tournament and I will pay you what you're worth. And isn't it better to win than to lose?[if:convo_mocking_aristocratic][ib:confident2]", null), null, null)
					.Condition(new ConversationSentence.OnConditionDelegate(this.AccusationCondition))
					.BeginPlayerOptions()
					.PlayerOption(new TextObject("{=MacG8ikN}I will think about it.", null), null)
					.Consequence(new ConversationSentence.OnConsequenceDelegate(this.CounterOfferAcceptedConsequence))
					.CloseDialog()
					.PlayerOption(new TextObject("{=bT279pk9}I have no idea what you talking about. Be on your way, friend.", null), null)
					.CloseDialog()
					.EndPlayerOptions()
					.CloseDialog();
			}

			private bool CounterOfferConversationStartCondition()
			{
				return this._counterOfferNotable != null && CharacterObject.OneToOneConversationCharacter == this._counterOfferNotable.CharacterObject;
			}

			private void CounterOfferConversationStartConsequence()
			{
				this._counterOfferConversationDone = true;
			}

			private bool AccusationCondition()
			{
				StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, null, false);
				return true;
			}

			private void CounterOfferAcceptedConsequence()
			{
				this._counterOfferAccepted = true;
			}

			private void MainHeroSuccessfullyFixedTournaments()
			{
				TextObject textObject = new TextObject("{=aCA83avL}You have placed in the tournaments as {QUEST_GIVER.LINK} wished.", null);
				textObject.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, false);
				base.AddLog(textObject, false);
				GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, 2500, false);
				Clan.PlayerClan.AddRenown(2f, true);
				base.QuestGiver.AddPower(10f);
				base.QuestGiver.CurrentSettlement.Town.Security += -20f;
				this.RelationshipChangeWithQuestGiver = 5;
				base.CompleteQuestWithSuccess();
			}

			private void MainHeroFailToFixTournaments()
			{
				TextObject textObject = new TextObject("{=ETbToaZC}You have failed to place in the tournaments as {QUEST_GIVER.LINK} wished.", null);
				textObject.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, false);
				base.AddLog(textObject, false);
				base.QuestGiver.AddPower(-10f);
				base.QuestGiver.CurrentSettlement.Town.Security += 10f;
				this.RelationshipChangeWithQuestGiver = -5;
				base.CompleteQuestWithFail(null);
			}

			private void MainHeroChooseNotToFixTournaments()
			{
				TextObject textObject = new TextObject("{=52smwnzz}You have chosen not to place in the tournaments as {QUEST_GIVER.LINK} wished.", null);
				textObject.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, false);
				base.AddLog(textObject, false);
				base.QuestGiver.AddPower(-15f);
				base.QuestGiver.CurrentSettlement.Town.Security += 15f;
				this.RelationshipChangeWithQuestGiver = -10;
				base.CompleteQuestWithFail(null);
			}

			private void MainHeroAcceptsCounterOffer()
			{
				TextObject textObject = new TextObject("{=nb0wqaGA}You have made a deal with {NOTABLE.LINK} to betray {QUEST_GIVER.LINK}.", null);
				textObject.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, false);
				textObject.SetCharacterProperties("NOTABLE", this._counterOfferNotable.CharacterObject, false);
				base.AddLog(textObject, false);
				GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, 4500, false);
				base.QuestGiver.AddPower(-15f);
				base.QuestGiver.CurrentSettlement.Town.Security += 15f;
				ChangeRelationAction.ApplyPlayerRelation(this._counterOfferNotable, 2, true, true);
				this.RelationshipChangeWithQuestGiver = -10;
				base.CompleteQuestWithFail(null);
			}

			private void PlayerDidNotCompleteTournaments()
			{
				base.AddLog(this.QuestFailedWithTimeOutLog, false);
				ChangeRelationAction.ApplyPlayerRelation(base.QuestGiver, -5, true, true);
			}

			private const int TournamentFixCount = 5;

			private const int MinorOffensiveLimit = 2;

			private const int SmallReward = 250;

			private const int BigReward = 2500;

			private const int CounterOfferReward = 4500;

			private const string MaleThug = "betting_fraud_thug_male";

			private const string FemaleThug = "betting_fraud_thug_female";

			[SaveableField(100)]
			private JournalLog _startLog;

			[SaveableField(1)]
			private Hero _counterOfferNotable;

			[SaveableField(10)]
			internal readonly CharacterObject _thug;

			[SaveableField(20)]
			private int _fixedTournamentCount;

			[SaveableField(30)]
			private int _minorOffensiveCount;

			private BettingFraudIssueBehavior.BettingFraudQuest.Directives _currentDirective;

			private BettingFraudIssueBehavior.BettingFraudQuest.AfterTournamentConversationState _afterTournamentConversationState;

			private bool _counterOfferAccepted;

			private bool _readyToStartTournament;

			private bool _startTournamentEndConversation;

			[SaveableField(40)]
			private bool _counterOfferConversationDone;

			private enum Directives
			{
				None,
				LoseAt3RdRound = 2,
				LoseAt4ThRound,
				WinTheTournament
			}

			private enum AfterTournamentConversationState
			{
				None,
				SmallReward,
				BigReward,
				MinorOffense,
				SecondMinorOffense,
				MajorOffense
			}
		}

		public class BettingFraudIssueTypeDefiner : SaveableTypeDefiner
		{
			public BettingFraudIssueTypeDefiner()
				: base(600327)
			{
			}

			protected override void DefineClassTypes()
			{
				base.AddClassDefinition(typeof(BettingFraudIssueBehavior.BettingFraudIssue), 1, null);
				base.AddClassDefinition(typeof(BettingFraudIssueBehavior.BettingFraudQuest), 2, null);
			}
		}
	}
}
