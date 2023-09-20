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
	// Token: 0x02000303 RID: 771
	public class BettingFraudIssueBehavior : CampaignBehaviorBase
	{
		// Token: 0x17000ACE RID: 2766
		// (get) Token: 0x06002C8D RID: 11405 RVA: 0x000BA42C File Offset: 0x000B862C
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

		// Token: 0x06002C8E RID: 11406 RVA: 0x000BA4C4 File Offset: 0x000B86C4
		public override void RegisterEvents()
		{
			CampaignEvents.OnCheckForIssueEvent.AddNonSerializedListener(this, new Action<Hero>(this.CheckForIssue));
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
		}

		// Token: 0x06002C8F RID: 11407 RVA: 0x000BA4F4 File Offset: 0x000B86F4
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

		// Token: 0x06002C90 RID: 11408 RVA: 0x000BA5BC File Offset: 0x000B87BC
		private void game_menu_tournament_join_on_init(MenuCallbackArgs args)
		{
			TournamentGame tournamentGame = Campaign.Current.TournamentManager.GetTournamentGame(Settlement.CurrentSettlement.Town);
			tournamentGame.UpdateTournamentPrize(true, false);
			GameTexts.SetVariable("MENU_TEXT", tournamentGame.GetMenuText());
		}

		// Token: 0x06002C91 RID: 11409 RVA: 0x000BA5FC File Offset: 0x000B87FC
		private void game_menu_tournament_join_current_game_on_consequence(MenuCallbackArgs args)
		{
			CampaignMapConversation.OpenConversation(new ConversationCharacterData(CharacterObject.PlayerCharacter, null, false, false, false, false, false, false), new ConversationCharacterData(BettingFraudIssueBehavior.Instance._thug, null, false, false, false, false, false, false));
		}

		// Token: 0x06002C92 RID: 11410 RVA: 0x000BA638 File Offset: 0x000B8838
		[GameMenuInitializationHandler("menu_town_tournament_join_betting_fraud")]
		private static void game_menu_ui_town_ui_on_init(MenuCallbackArgs args)
		{
			Settlement currentSettlement = Settlement.CurrentSettlement;
			args.MenuContext.SetBackgroundMeshName(currentSettlement.Town.WaitMeshName);
		}

		// Token: 0x06002C93 RID: 11411 RVA: 0x000BA661 File Offset: 0x000B8861
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x06002C94 RID: 11412 RVA: 0x000BA664 File Offset: 0x000B8864
		private void CheckForIssue(Hero hero)
		{
			if (this.ConditionsHold(hero))
			{
				Campaign.Current.IssueManager.AddPotentialIssueData(hero, new PotentialIssueData(new PotentialIssueData.StartIssueDelegate(this.OnStartIssue), typeof(BettingFraudIssueBehavior.BettingFraudIssue), IssueBase.IssueFrequency.Rare, null));
				return;
			}
			Campaign.Current.IssueManager.AddPotentialIssueData(hero, new PotentialIssueData(typeof(BettingFraudIssueBehavior.BettingFraudIssue), IssueBase.IssueFrequency.Rare));
		}

		// Token: 0x06002C95 RID: 11413 RVA: 0x000BA6C8 File Offset: 0x000B88C8
		private bool ConditionsHold(Hero issueGiver)
		{
			return issueGiver.IsGangLeader && issueGiver.CurrentSettlement != null && issueGiver.CurrentSettlement.Town != null && issueGiver.CurrentSettlement.Town.Security < 50f;
		}

		// Token: 0x06002C96 RID: 11414 RVA: 0x000BA700 File Offset: 0x000B8900
		private IssueBase OnStartIssue(in PotentialIssueData pid, Hero issueOwner)
		{
			return new BettingFraudIssueBehavior.BettingFraudIssue(issueOwner);
		}

		// Token: 0x04000D72 RID: 3442
		private const IssueBase.IssueFrequency BettingFraudIssueFrequency = IssueBase.IssueFrequency.Rare;

		// Token: 0x04000D73 RID: 3443
		private const string JoinTournamentMenuId = "menu_town_tournament_join";

		// Token: 0x04000D74 RID: 3444
		private const string JoinTournamentForBettingFraudQuestMenuId = "menu_town_tournament_join_betting_fraud";

		// Token: 0x04000D75 RID: 3445
		private const int SettlementSecurityLimit = 50;

		// Token: 0x04000D76 RID: 3446
		private BettingFraudIssueBehavior.BettingFraudQuest _cachedQuest;

		// Token: 0x02000605 RID: 1541
		public class BettingFraudIssue : IssueBase
		{
			// Token: 0x0600476D RID: 18285 RVA: 0x001407DC File Offset: 0x0013E9DC
			internal static void AutoGeneratedStaticCollectObjectsBettingFraudIssue(object o, List<object> collectedObjects)
			{
				((BettingFraudIssueBehavior.BettingFraudIssue)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			// Token: 0x0600476E RID: 18286 RVA: 0x001407EA File Offset: 0x0013E9EA
			protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			// Token: 0x17000E68 RID: 3688
			// (get) Token: 0x0600476F RID: 18287 RVA: 0x001407F3 File Offset: 0x0013E9F3
			public override TextObject IssueBriefByIssueGiver
			{
				get
				{
					return new TextObject("{=kru5Vpog}Yes. I'm glad to have the chance to talk to you. I've been thinking for a while about how you and I might work together... Interested?", null);
				}
			}

			// Token: 0x17000E69 RID: 3689
			// (get) Token: 0x06004770 RID: 18288 RVA: 0x00140800 File Offset: 0x0013EA00
			public override TextObject IssueAcceptByPlayer
			{
				get
				{
					return new TextObject("{=YWXkgDSd}What kind of a partnership are we talking about?", null);
				}
			}

			// Token: 0x17000E6A RID: 3690
			// (get) Token: 0x06004771 RID: 18289 RVA: 0x0014080D File Offset: 0x0013EA0D
			public override TextObject IssueQuestSolutionExplanationByIssueGiver
			{
				get
				{
					return new TextObject("{=vLaoZhkF}Well, you've made quite a name for yourself as a warrior. You may not know this, but I keep an eye on the careers of champions like yourself for professional reasons. I follow tournaments, you see, and like to both place and take bets. But of course I need someone who can not only win those tournaments but lose if necessary... if you understand what I mean. Not all the time. That would be too obvious. Here's what I propose. We enter into a partnership for five tournaments. Don't bother memorizing which ones you win and which ones you lose. Before each fight, an associate of my mine will let you know how you should place. Follow my instructions and I promise you will be rewarded handsomely. What do you say?", null);
				}
			}

			// Token: 0x17000E6B RID: 3691
			// (get) Token: 0x06004772 RID: 18290 RVA: 0x0014081A File Offset: 0x0013EA1A
			public override TextObject IssueQuestSolutionAcceptByPlayer
			{
				get
				{
					return new TextObject("{=cL9BX7ph}As long as the payment is good, I agree.", null);
				}
			}

			// Token: 0x17000E6C RID: 3692
			// (get) Token: 0x06004773 RID: 18291 RVA: 0x00140827 File Offset: 0x0013EA27
			public override bool IsThereAlternativeSolution
			{
				get
				{
					return false;
				}
			}

			// Token: 0x17000E6D RID: 3693
			// (get) Token: 0x06004774 RID: 18292 RVA: 0x0014082A File Offset: 0x0013EA2A
			public override bool IsThereLordSolution
			{
				get
				{
					return false;
				}
			}

			// Token: 0x17000E6E RID: 3694
			// (get) Token: 0x06004775 RID: 18293 RVA: 0x0014082D File Offset: 0x0013EA2D
			public override TextObject Title
			{
				get
				{
					return new TextObject("{=xhVrxgC4}Betting Fraud", null);
				}
			}

			// Token: 0x17000E6F RID: 3695
			// (get) Token: 0x06004776 RID: 18294 RVA: 0x0014083A File Offset: 0x0013EA3A
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

			// Token: 0x06004777 RID: 18295 RVA: 0x0014086B File Offset: 0x0013EA6B
			public BettingFraudIssue(Hero issueOwner)
				: base(issueOwner, CampaignTime.DaysFromNow(45f))
			{
			}

			// Token: 0x06004778 RID: 18296 RVA: 0x0014087E File Offset: 0x0013EA7E
			protected override float GetIssueEffectAmountInternal(IssueEffect issueEffect)
			{
				if (issueEffect == DefaultIssueEffects.IssueOwnerPower)
				{
					return -0.2f;
				}
				return 0f;
			}

			// Token: 0x06004779 RID: 18297 RVA: 0x00140893 File Offset: 0x0013EA93
			protected override void OnGameLoad()
			{
			}

			// Token: 0x0600477A RID: 18298 RVA: 0x00140895 File Offset: 0x0013EA95
			protected override QuestBase GenerateIssueQuest(string questId)
			{
				return new BettingFraudIssueBehavior.BettingFraudQuest(questId, base.IssueOwner, CampaignTime.DaysFromNow(45f), 0);
			}

			// Token: 0x0600477B RID: 18299 RVA: 0x001408AE File Offset: 0x0013EAAE
			public override IssueBase.IssueFrequency GetFrequency()
			{
				return IssueBase.IssueFrequency.Rare;
			}

			// Token: 0x0600477C RID: 18300 RVA: 0x001408B4 File Offset: 0x0013EAB4
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

			// Token: 0x0600477D RID: 18301 RVA: 0x00140A50 File Offset: 0x0013EC50
			public override bool IssueStayAliveConditions()
			{
				return true;
			}

			// Token: 0x0600477E RID: 18302 RVA: 0x00140A53 File Offset: 0x0013EC53
			protected override void CompleteIssueWithTimedOutConsequences()
			{
			}

			// Token: 0x040018E0 RID: 6368
			private const int NeededTournamentCount = 5;

			// Token: 0x040018E1 RID: 6369
			private const int IssueDuration = 45;

			// Token: 0x040018E2 RID: 6370
			private const int MainHeroSkillLimit = 50;

			// Token: 0x040018E3 RID: 6371
			private const int MainClanRenownLimit = 50;

			// Token: 0x040018E4 RID: 6372
			private const int RelationLimitWithIssueOwner = -10;

			// Token: 0x040018E5 RID: 6373
			private const float IssueOwnerPowerPenaltyForIssueEffect = -0.2f;
		}

		// Token: 0x02000606 RID: 1542
		public class BettingFraudQuest : QuestBase
		{
			// Token: 0x0600477F RID: 18303 RVA: 0x00140A55 File Offset: 0x0013EC55
			internal static void AutoGeneratedStaticCollectObjectsBettingFraudQuest(object o, List<object> collectedObjects)
			{
				((BettingFraudIssueBehavior.BettingFraudQuest)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			// Token: 0x06004780 RID: 18304 RVA: 0x00140A63 File Offset: 0x0013EC63
			protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				base.AutoGeneratedInstanceCollectObjects(collectedObjects);
				collectedObjects.Add(this._thug);
				collectedObjects.Add(this._startLog);
				collectedObjects.Add(this._counterOfferNotable);
			}

			// Token: 0x06004781 RID: 18305 RVA: 0x00140A90 File Offset: 0x0013EC90
			internal static object AutoGeneratedGetMemberValue_thug(object o)
			{
				return ((BettingFraudIssueBehavior.BettingFraudQuest)o)._thug;
			}

			// Token: 0x06004782 RID: 18306 RVA: 0x00140A9D File Offset: 0x0013EC9D
			internal static object AutoGeneratedGetMemberValue_startLog(object o)
			{
				return ((BettingFraudIssueBehavior.BettingFraudQuest)o)._startLog;
			}

			// Token: 0x06004783 RID: 18307 RVA: 0x00140AAA File Offset: 0x0013ECAA
			internal static object AutoGeneratedGetMemberValue_counterOfferNotable(object o)
			{
				return ((BettingFraudIssueBehavior.BettingFraudQuest)o)._counterOfferNotable;
			}

			// Token: 0x06004784 RID: 18308 RVA: 0x00140AB7 File Offset: 0x0013ECB7
			internal static object AutoGeneratedGetMemberValue_fixedTournamentCount(object o)
			{
				return ((BettingFraudIssueBehavior.BettingFraudQuest)o)._fixedTournamentCount;
			}

			// Token: 0x06004785 RID: 18309 RVA: 0x00140AC9 File Offset: 0x0013ECC9
			internal static object AutoGeneratedGetMemberValue_minorOffensiveCount(object o)
			{
				return ((BettingFraudIssueBehavior.BettingFraudQuest)o)._minorOffensiveCount;
			}

			// Token: 0x06004786 RID: 18310 RVA: 0x00140ADB File Offset: 0x0013ECDB
			internal static object AutoGeneratedGetMemberValue_counterOfferConversationDone(object o)
			{
				return ((BettingFraudIssueBehavior.BettingFraudQuest)o)._counterOfferConversationDone;
			}

			// Token: 0x17000E70 RID: 3696
			// (get) Token: 0x06004787 RID: 18311 RVA: 0x00140AED File Offset: 0x0013ECED
			public override TextObject Title
			{
				get
				{
					return new TextObject("{=xhVrxgC4}Betting Fraud", null);
				}
			}

			// Token: 0x17000E71 RID: 3697
			// (get) Token: 0x06004788 RID: 18312 RVA: 0x00140AFA File Offset: 0x0013ECFA
			public override bool IsRemainingTimeHidden
			{
				get
				{
					return false;
				}
			}

			// Token: 0x17000E72 RID: 3698
			// (get) Token: 0x06004789 RID: 18313 RVA: 0x00140AFD File Offset: 0x0013ECFD
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

			// Token: 0x17000E73 RID: 3699
			// (get) Token: 0x0600478A RID: 18314 RVA: 0x00140B3D File Offset: 0x0013ED3D
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

			// Token: 0x17000E74 RID: 3700
			// (get) Token: 0x0600478B RID: 18315 RVA: 0x00140B74 File Offset: 0x0013ED74
			private TextObject QuestFailedWithTimeOutLog
			{
				get
				{
					TextObject textObject = new TextObject("{=2brAaeFh}You failed to complete tournaments in time. {QUEST_GIVER.LINK} will certainly be disappointed.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					return textObject;
				}
			}

			// Token: 0x0600478C RID: 18316 RVA: 0x00140BA8 File Offset: 0x0013EDA8
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

			// Token: 0x0600478D RID: 18317 RVA: 0x00140C3A File Offset: 0x0013EE3A
			protected override void InitializeQuestOnGameLoad()
			{
				this.SetDialogs();
			}

			// Token: 0x0600478E RID: 18318 RVA: 0x00140C42 File Offset: 0x0013EE42
			private void SelectCounterOfferNotable(Settlement settlement)
			{
				this._counterOfferNotable = settlement.Notables.GetRandomElement<Hero>();
			}

			// Token: 0x0600478F RID: 18319 RVA: 0x00140C55 File Offset: 0x0013EE55
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

			// Token: 0x06004790 RID: 18320 RVA: 0x00140C84 File Offset: 0x0013EE84
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

			// Token: 0x06004791 RID: 18321 RVA: 0x00140CC4 File Offset: 0x0013EEC4
			private void SetCurrentDirective()
			{
				this._currentDirective = ((MBRandom.RandomFloat <= 0.33f) ? BettingFraudIssueBehavior.BettingFraudQuest.Directives.LoseAt3RdRound : ((MBRandom.RandomFloat < 0.5f) ? BettingFraudIssueBehavior.BettingFraudQuest.Directives.LoseAt4ThRound : BettingFraudIssueBehavior.BettingFraudQuest.Directives.WinTheTournament));
				base.AddLog(this.CurrentDirectiveLog, false);
			}

			// Token: 0x06004792 RID: 18322 RVA: 0x00140CFC File Offset: 0x0013EEFC
			private void StartTournamentMission()
			{
				TournamentGame tournamentGame = Campaign.Current.TournamentManager.GetTournamentGame(Settlement.CurrentSettlement.Town);
				GameMenu.SwitchToMenu("town");
				tournamentGame.PrepareForTournamentGame(true);
				Campaign.Current.TournamentManager.OnPlayerJoinTournament(tournamentGame.GetType(), Settlement.CurrentSettlement);
			}

			// Token: 0x06004793 RID: 18323 RVA: 0x00140D50 File Offset: 0x0013EF50
			protected override void RegisterEvents()
			{
				CampaignEvents.PlayerEliminatedFromTournament.AddNonSerializedListener(this, new Action<int, Town>(this.OnPlayerEliminatedFromTournament));
				CampaignEvents.TournamentFinished.AddNonSerializedListener(this, new Action<CharacterObject, MBReadOnlyList<CharacterObject>, Town, ItemObject>(this.OnTournamentFinished));
				CampaignEvents.GameMenuOpened.AddNonSerializedListener(this, new Action<MenuCallbackArgs>(this.OnGameMenuOpened));
			}

			// Token: 0x06004794 RID: 18324 RVA: 0x00140DA2 File Offset: 0x0013EFA2
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

			// Token: 0x06004795 RID: 18325 RVA: 0x00140DDC File Offset: 0x0013EFDC
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

			// Token: 0x06004796 RID: 18326 RVA: 0x00140E34 File Offset: 0x0013F034
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

			// Token: 0x06004797 RID: 18327 RVA: 0x00140F4A File Offset: 0x0013F14A
			protected override void OnTimedOut()
			{
				base.OnTimedOut();
				this.PlayerDidNotCompleteTournaments();
			}

			// Token: 0x06004798 RID: 18328 RVA: 0x00140F58 File Offset: 0x0013F158
			protected override void SetDialogs()
			{
				this.OfferDialogFlow = this.GetOfferDialogFlow();
				this.DiscussDialogFlow = this.GetDiscussDialogFlow();
				Campaign.Current.ConversationManager.AddDialogFlow(this.GetDialogWithThugStart(), this);
				Campaign.Current.ConversationManager.AddDialogFlow(this.GetDialogWithThugEnd(), this);
				Campaign.Current.ConversationManager.AddDialogFlow(this.GetCounterOfferDialog(), this);
			}

			// Token: 0x06004799 RID: 18329 RVA: 0x00140FC0 File Offset: 0x0013F1C0
			private DialogFlow GetOfferDialogFlow()
			{
				return DialogFlow.CreateDialogFlow("issue_classic_quest_start", 100).NpcLine(new TextObject("{=sp52g5AQ}Very good, very good. Try to enter five tournaments over the next 45 days or so. Right before the fight you'll hear from my associate how far I want you to go in the rankings before you lose.", null), null, null).Condition(() => Hero.OneToOneConversationHero == base.QuestGiver)
					.NpcLine(new TextObject("{=ADIYnC4u}Now, I know you can't win every fight, so if you underperform once or twice, I'd understand. But if you lose every time, or worse, if you overperform, well, then I'll be a bit angry.[if:convo_mocking_revenge][ib:confident]", null), null, null)
					.NpcLine(new TextObject("{=1hOPCf8I}But I'm sure you won't disappoint me. Enjoy your riches![if:happy][ib:demure]", null), null, null)
					.Consequence(new ConversationSentence.OnConsequenceDelegate(this.OfferDialogFlowConsequence))
					.CloseDialog();
			}

			// Token: 0x0600479A RID: 18330 RVA: 0x00141036 File Offset: 0x0013F236
			private void OfferDialogFlowConsequence()
			{
				base.StartQuest();
				this._startLog = base.AddDiscreteLog(this.StartLog, new TextObject("{=dLfWFa61}Fix 5 Tournaments", null), 0, 5, null, false);
			}

			// Token: 0x0600479B RID: 18331 RVA: 0x00141060 File Offset: 0x0013F260
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

			// Token: 0x0600479C RID: 18332 RVA: 0x00141150 File Offset: 0x0013F350
			private bool DiscussDialogCondition()
			{
				bool flag = Hero.OneToOneConversationHero == base.QuestGiver;
				if (flag)
				{
					if (this._minorOffensiveCount > 0)
					{
						MBTextManager.SetTextVariable("RESPONSE_TEXT", new TextObject("{=7SPwGYvf}I had expected better of you. But even the best can fail sometimes. Just make sure it does not happen again.[ib:confident][if:convo_nonchalant]", null), false);
						return flag;
					}
					MBTextManager.SetTextVariable("RESPONSE_TEXT", new TextObject("{=vo0uhUsZ}I have high hopes for you, friend. Just follow my directives and we will be rich.[if:happy][ib:demure]", null), false);
				}
				return flag;
			}

			// Token: 0x0600479D RID: 18333 RVA: 0x001411A4 File Offset: 0x0013F3A4
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

			// Token: 0x0600479E RID: 18334 RVA: 0x00141230 File Offset: 0x0013F430
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

			// Token: 0x0600479F RID: 18335 RVA: 0x001412EC File Offset: 0x0013F4EC
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
						TextObject textObject2 = new TextObject("{=cQE9tQOy}My friend! Good to see you. You did very well in that last fight. People definitely won't be expecting you to \"{DIRECTIVE}\". What a surprise that would be. Well, I should not keep you from your tournament. You know what to do.[if:happy][ib:demure]", null);
						textObject2.SetTextVariable("DIRECTIVE", this.GetDirectiveText());
						MBTextManager.SetTextVariable("GREETING_LINE", textObject2, false);
						return flag;
					}
					TextObject textObject3 = new TextObject("{=RVLPQ4rm}My friend. I am almost sad that these meetings are going to come to an end. Well, a deal is a deal. I won't beat around the bush. Here's your final message: {DIRECTIVE}. I wish you luck, right up until the moment that you have to go down.[if:happy][ib:demure]", null);
					textObject3.SetTextVariable("DIRECTIVE", this.GetDirectiveText());
					MBTextManager.SetTextVariable("GREETING_LINE", textObject3, false);
				}
				return flag;
			}

			// Token: 0x060047A0 RID: 18336 RVA: 0x001413C8 File Offset: 0x0013F5C8
			private bool PositiveOptionCondition()
			{
				if (this._fixedTournamentCount < 2)
				{
					MBTextManager.SetTextVariable("POSITIVE_OPTION", new TextObject("{=PrUauabl}As long as the payment is as we talked, you got nothing to worry about.", null), false);
				}
				else if (this._fixedTournamentCount < 4)
				{
					MBTextManager.SetTextVariable("POSITIVE_OPTION", new TextObject("{=TKRsPVMU}Yes, I do. Be around when the tournament is over.", null), false);
				}
				else
				{
					MBTextManager.SetTextVariable("POSITIVE_OPTION", new TextObject("{=26XPQw2v}I will miss this little deal we had. See you at the end", null), false);
				}
				return true;
			}

			// Token: 0x060047A1 RID: 18337 RVA: 0x0014142E File Offset: 0x0013F62E
			private void PositiveOptionConsequences()
			{
				this._readyToStartTournament = true;
			}

			// Token: 0x060047A2 RID: 18338 RVA: 0x00141437 File Offset: 0x0013F637
			private bool NegativeOptionCondition()
			{
				bool flag = this._fixedTournamentCount >= 4;
				if (flag)
				{
					MBTextManager.SetTextVariable("NEGATIVE_OPTION", new TextObject("{=vapdvRQO}This deal was a mistake. We will not talk again after this last tournament.", null), false);
				}
				return flag;
			}

			// Token: 0x060047A3 RID: 18339 RVA: 0x0014145E File Offset: 0x0013F65E
			private void NegativeOptionConsequence()
			{
				this._readyToStartTournament = true;
			}

			// Token: 0x060047A4 RID: 18340 RVA: 0x00141468 File Offset: 0x0013F668
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

			// Token: 0x060047A5 RID: 18341 RVA: 0x001414BC File Offset: 0x0013F6BC
			private DialogFlow GetDialogWithThugEnd()
			{
				return DialogFlow.CreateDialogFlow("start", 125).NpcLine(new TextObject("{=!}{GREETING_LINE}", null), null, null).Condition(new ConversationSentence.OnConditionDelegate(this.DialogWithThugEndCondition))
					.Consequence(new ConversationSentence.OnConsequenceDelegate(this.DialogWithThugEndConsequence))
					.CloseDialog();
			}

			// Token: 0x060047A6 RID: 18342 RVA: 0x00141510 File Offset: 0x0013F710
			private bool DialogWithThugEndCondition()
			{
				bool flag = CharacterObject.OneToOneConversationCharacter == this._thug && this._startTournamentEndConversation;
				if (flag)
				{
					TextObject textObject = TextObject.Empty;
					switch (this._afterTournamentConversationState)
					{
					case BettingFraudIssueBehavior.BettingFraudQuest.AfterTournamentConversationState.SmallReward:
						textObject = new TextObject("{=ZM8t4ZW2}We are very impressed, my friend. Here is the payment as promised. I hope we can continue this profitable partnership. See you at the next tournament.[if:happy][ib:demure]", null);
						GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, 250, false);
						break;
					case BettingFraudIssueBehavior.BettingFraudQuest.AfterTournamentConversationState.BigReward:
						textObject = new TextObject("{=9vOZWY25}What an exciting result! I will definitely miss these tournaments. Well, maybe after some time goes by and memories get a little hazy we can continue. Here is the last payment. Very well deserved.[if:happy][ib:demure]", null);
						break;
					case BettingFraudIssueBehavior.BettingFraudQuest.AfterTournamentConversationState.MinorOffense:
						textObject = new TextObject("{=d8bGHJnZ}This was not we were expecting. We lost some money. Well, Lady Fortune always casts her ballot too in these contests. But try to reassure us that this was her plan, and not yours, eh?[if:convo_mocking_revenge][ib:confident]", null);
						break;
					case BettingFraudIssueBehavior.BettingFraudQuest.AfterTournamentConversationState.SecondMinorOffense:
						textObject = new TextObject("{=bNAG2t8S}Well, my friend, either you're playing us false or you're just not very good at this. Either way, {QUEST_GIVER.LINK} wishes to tell you that {?QUEST_GIVER.GENDER}her{?}his{\\?} association with you is over.[if:idle_insulted][ib:closed]", null);
						textObject.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, false);
						break;
					case BettingFraudIssueBehavior.BettingFraudQuest.AfterTournamentConversationState.MajorOffense:
						textObject = new TextObject("{=Lyqx3NYE}Well... What happened back there... That wasn't bad luck or incompetence. {QUEST_GIVER.LINK} trusted in you and {?QUEST_GIVER.GENDER}She{?}He{\\?} doesn't take well to betrayal.[if:idle_angry][ib:warrior]", null);
						textObject.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, false);
						break;
					default:
						Debug.FailedAssert("After tournament conversation state is not set!", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\Issues\\BettingFraudIssueBehavior.cs", "DialogWithThugEndCondition", 713);
						break;
					}
					MBTextManager.SetTextVariable("GREETING_LINE", textObject, false);
				}
				return flag;
			}

			// Token: 0x060047A7 RID: 18343 RVA: 0x00141614 File Offset: 0x0013F814
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

			// Token: 0x060047A8 RID: 18344 RVA: 0x00141670 File Offset: 0x0013F870
			private DialogFlow GetCounterOfferDialog()
			{
				return DialogFlow.CreateDialogFlow("start", 125).NpcLine(new TextObject("{=bUfBHSsz}Hold on a moment, friend. I need to talk to you.", null), null, null).Condition(new ConversationSentence.OnConditionDelegate(this.CounterOfferConversationStartCondition))
					.Consequence(new ConversationSentence.OnConsequenceDelegate(this.CounterOfferConversationStartConsequence))
					.PlayerLine(new TextObject("{=PZfR7hEK}What do you want? I have a tournament to prepare for.", null), null)
					.NpcLine(new TextObject("{=GN9F316V}Oh of course you do. {QUEST_GIVER.LINK}'s people have been running around placing bets - we know all about your arrangement, you see. And let me tell you something: as these arrangements go, {QUEST_GIVER.LINK} is getting you cheap. Do you want to see real money? Win this tournament and I will pay you what you're worth. And isn't it better to win than to lose?", null), null, null)
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

			// Token: 0x060047A9 RID: 18345 RVA: 0x0014173D File Offset: 0x0013F93D
			private bool CounterOfferConversationStartCondition()
			{
				return this._counterOfferNotable != null && CharacterObject.OneToOneConversationCharacter == this._counterOfferNotable.CharacterObject;
			}

			// Token: 0x060047AA RID: 18346 RVA: 0x0014175B File Offset: 0x0013F95B
			private void CounterOfferConversationStartConsequence()
			{
				this._counterOfferConversationDone = true;
			}

			// Token: 0x060047AB RID: 18347 RVA: 0x00141764 File Offset: 0x0013F964
			private bool AccusationCondition()
			{
				StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, null, false);
				return true;
			}

			// Token: 0x060047AC RID: 18348 RVA: 0x0014177F File Offset: 0x0013F97F
			private void CounterOfferAcceptedConsequence()
			{
				this._counterOfferAccepted = true;
			}

			// Token: 0x060047AD RID: 18349 RVA: 0x00141788 File Offset: 0x0013F988
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

			// Token: 0x060047AE RID: 18350 RVA: 0x00141820 File Offset: 0x0013FA20
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

			// Token: 0x060047AF RID: 18351 RVA: 0x0014189C File Offset: 0x0013FA9C
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

			// Token: 0x060047B0 RID: 18352 RVA: 0x00141918 File Offset: 0x0013FB18
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

			// Token: 0x060047B1 RID: 18353 RVA: 0x001419C7 File Offset: 0x0013FBC7
			private void PlayerDidNotCompleteTournaments()
			{
				base.AddLog(this.QuestFailedWithTimeOutLog, false);
				ChangeRelationAction.ApplyPlayerRelation(base.QuestGiver, -5, true, true);
			}

			// Token: 0x040018E6 RID: 6374
			private const int TournamentFixCount = 5;

			// Token: 0x040018E7 RID: 6375
			private const int MinorOffensiveLimit = 2;

			// Token: 0x040018E8 RID: 6376
			private const int SmallReward = 250;

			// Token: 0x040018E9 RID: 6377
			private const int BigReward = 2500;

			// Token: 0x040018EA RID: 6378
			private const int CounterOfferReward = 4500;

			// Token: 0x040018EB RID: 6379
			private const string MaleThug = "betting_fraud_thug_male";

			// Token: 0x040018EC RID: 6380
			private const string FemaleThug = "betting_fraud_thug_female";

			// Token: 0x040018ED RID: 6381
			[SaveableField(100)]
			private JournalLog _startLog;

			// Token: 0x040018EE RID: 6382
			[SaveableField(1)]
			private Hero _counterOfferNotable;

			// Token: 0x040018EF RID: 6383
			[SaveableField(10)]
			internal readonly CharacterObject _thug;

			// Token: 0x040018F0 RID: 6384
			[SaveableField(20)]
			private int _fixedTournamentCount;

			// Token: 0x040018F1 RID: 6385
			[SaveableField(30)]
			private int _minorOffensiveCount;

			// Token: 0x040018F2 RID: 6386
			private BettingFraudIssueBehavior.BettingFraudQuest.Directives _currentDirective;

			// Token: 0x040018F3 RID: 6387
			private BettingFraudIssueBehavior.BettingFraudQuest.AfterTournamentConversationState _afterTournamentConversationState;

			// Token: 0x040018F4 RID: 6388
			private bool _counterOfferAccepted;

			// Token: 0x040018F5 RID: 6389
			private bool _readyToStartTournament;

			// Token: 0x040018F6 RID: 6390
			private bool _startTournamentEndConversation;

			// Token: 0x040018F7 RID: 6391
			[SaveableField(40)]
			private bool _counterOfferConversationDone;

			// Token: 0x0200078E RID: 1934
			private enum Directives
			{
				// Token: 0x04001EC6 RID: 7878
				None,
				// Token: 0x04001EC7 RID: 7879
				LoseAt3RdRound = 2,
				// Token: 0x04001EC8 RID: 7880
				LoseAt4ThRound,
				// Token: 0x04001EC9 RID: 7881
				WinTheTournament
			}

			// Token: 0x0200078F RID: 1935
			private enum AfterTournamentConversationState
			{
				// Token: 0x04001ECB RID: 7883
				None,
				// Token: 0x04001ECC RID: 7884
				SmallReward,
				// Token: 0x04001ECD RID: 7885
				BigReward,
				// Token: 0x04001ECE RID: 7886
				MinorOffense,
				// Token: 0x04001ECF RID: 7887
				SecondMinorOffense,
				// Token: 0x04001ED0 RID: 7888
				MajorOffense
			}
		}

		// Token: 0x02000607 RID: 1543
		public class BettingFraudIssueTypeDefiner : SaveableTypeDefiner
		{
			// Token: 0x060047B3 RID: 18355 RVA: 0x001419F5 File Offset: 0x0013FBF5
			public BettingFraudIssueTypeDefiner()
				: base(600327)
			{
			}

			// Token: 0x060047B4 RID: 18356 RVA: 0x00141A02 File Offset: 0x0013FC02
			protected override void DefineClassTypes()
			{
				base.AddClassDefinition(typeof(BettingFraudIssueBehavior.BettingFraudIssue), 1, null);
				base.AddClassDefinition(typeof(BettingFraudIssueBehavior.BettingFraudQuest), 2, null);
			}
		}
	}
}
