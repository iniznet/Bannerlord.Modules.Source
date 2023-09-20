using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using SandBox.Missions.MissionLogics;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Issues;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace SandBox.Issues
{
	public class RivalGangMovingInIssueBehavior : CampaignBehaviorBase
	{
		private static RivalGangMovingInIssueBehavior.RivalGangMovingInIssueQuest Instance
		{
			get
			{
				RivalGangMovingInIssueBehavior campaignBehavior = Campaign.Current.GetCampaignBehavior<RivalGangMovingInIssueBehavior>();
				if (campaignBehavior._cachedQuest != null && campaignBehavior._cachedQuest.IsOngoing)
				{
					return campaignBehavior._cachedQuest;
				}
				using (List<QuestBase>.Enumerator enumerator = Campaign.Current.QuestManager.Quests.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						RivalGangMovingInIssueBehavior.RivalGangMovingInIssueQuest rivalGangMovingInIssueQuest;
						if ((rivalGangMovingInIssueQuest = enumerator.Current as RivalGangMovingInIssueBehavior.RivalGangMovingInIssueQuest) != null)
						{
							campaignBehavior._cachedQuest = rivalGangMovingInIssueQuest;
							return campaignBehavior._cachedQuest;
						}
					}
				}
				return null;
			}
		}

		private void OnCheckForIssue(Hero hero)
		{
			if (this.ConditionsHold(hero))
			{
				Campaign.Current.IssueManager.AddPotentialIssueData(hero, new PotentialIssueData(new PotentialIssueData.StartIssueDelegate(this.OnStartIssue), typeof(RivalGangMovingInIssueBehavior.RivalGangMovingInIssue), 1, null));
				return;
			}
			Campaign.Current.IssueManager.AddPotentialIssueData(hero, new PotentialIssueData(typeof(RivalGangMovingInIssueBehavior.RivalGangMovingInIssue), 1));
		}

		private IssueBase OnStartIssue(in PotentialIssueData pid, Hero issueOwner)
		{
			Hero rivalGangLeader = this.GetRivalGangLeader(issueOwner);
			return new RivalGangMovingInIssueBehavior.RivalGangMovingInIssue(issueOwner, rivalGangLeader);
		}

		private static void rival_gang_wait_duration_is_over_menu_on_init(MenuCallbackArgs args)
		{
			Campaign.Current.TimeControlMode = 0;
			TextObject textObject = new TextObject("{=9Kr9pjGs}{QUEST_GIVER.LINK} has prepared {?QUEST_GIVER.GENDER}her{?}his{\\?} men and is waiting for you.", null);
			StringHelpers.SetCharacterProperties("QUEST_GIVER", RivalGangMovingInIssueBehavior.Instance.QuestGiver.CharacterObject, null, false);
			MBTextManager.SetTextVariable("MENU_TEXT", textObject, false);
		}

		private bool ConditionsHold(Hero issueGiver)
		{
			return issueGiver.IsGangLeader && issueGiver.CurrentSettlement != null && issueGiver.CurrentSettlement.IsTown && issueGiver.CurrentSettlement.Town.Security <= 60f && this.GetRivalGangLeader(issueGiver) != null;
		}

		private void rival_gang_quest_wait_duration_is_over_yes_consequence(MenuCallbackArgs args)
		{
			CampaignMapConversation.OpenConversation(new ConversationCharacterData(CharacterObject.PlayerCharacter, null, true, true, false, false, false, false), new ConversationCharacterData(RivalGangMovingInIssueBehavior.Instance.QuestGiver.CharacterObject, null, true, true, false, false, false, false));
		}

		private Hero GetRivalGangLeader(Hero issueOwner)
		{
			Hero hero = null;
			foreach (Hero hero2 in issueOwner.CurrentSettlement.Notables)
			{
				if (hero2 != issueOwner && hero2.IsGangLeader && hero2.CanHaveQuestsOrIssues())
				{
					hero = hero2;
					break;
				}
			}
			return hero;
		}

		private bool rival_gang_quest_wait_duration_is_over_yes_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = 17;
			return true;
		}

		private bool rival_gang_quest_wait_duration_is_over_no_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = 16;
			return true;
		}

		public override void RegisterEvents()
		{
			CampaignEvents.OnCheckForIssueEvent.AddNonSerializedListener(this, new Action<Hero>(this.OnCheckForIssue));
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
		}

		private void OnSessionLaunched(CampaignGameStarter gameStarter)
		{
			gameStarter.AddGameMenu("rival_gang_quest_before_fight", "", new OnInitDelegate(RivalGangMovingInIssueBehavior.rival_gang_quest_before_fight_init), 3, 0, null);
			gameStarter.AddGameMenu("rival_gang_quest_after_fight", "", new OnInitDelegate(RivalGangMovingInIssueBehavior.rival_gang_quest_after_fight_init), 3, 0, null);
			gameStarter.AddGameMenu("rival_gang_quest_wait_duration_is_over", "{MENU_TEXT}", new OnInitDelegate(RivalGangMovingInIssueBehavior.rival_gang_wait_duration_is_over_menu_on_init), 0, 0, null);
			gameStarter.AddGameMenuOption("rival_gang_quest_wait_duration_is_over", "rival_gang_quest_wait_duration_is_over_yes", "{=aka03VdU}Meet {?QUEST_GIVER.GENDER}her{?}him{\\?} now", new GameMenuOption.OnConditionDelegate(this.rival_gang_quest_wait_duration_is_over_yes_condition), new GameMenuOption.OnConsequenceDelegate(this.rival_gang_quest_wait_duration_is_over_yes_consequence), false, -1, false, null);
			gameStarter.AddGameMenuOption("rival_gang_quest_wait_duration_is_over", "rival_gang_quest_wait_duration_is_over_no", "{=NIzQb6nT}Leave and meet {?QUEST_GIVER.GENDER}her{?}him{\\?} later", new GameMenuOption.OnConditionDelegate(this.rival_gang_quest_wait_duration_is_over_no_condition), new GameMenuOption.OnConsequenceDelegate(this.rival_gang_quest_wait_duration_is_over_no_consequence), true, -1, false, null);
		}

		private void rival_gang_quest_wait_duration_is_over_no_consequence(MenuCallbackArgs args)
		{
			Campaign.Current.CurrentMenuContext.SwitchToMenu("town_wait_menus");
		}

		private static void rival_gang_quest_before_fight_init(MenuCallbackArgs args)
		{
			if (RivalGangMovingInIssueBehavior.Instance != null && RivalGangMovingInIssueBehavior.Instance._isFinalStage)
			{
				RivalGangMovingInIssueBehavior.Instance.StartAlleyBattle(RivalGangMovingInIssueBehavior.Instance._rivalGangLeader);
			}
		}

		private static void rival_gang_quest_after_fight_init(MenuCallbackArgs args)
		{
			if (RivalGangMovingInIssueBehavior.Instance != null && RivalGangMovingInIssueBehavior.Instance._isReadyToBeFinalized)
			{
				bool flag = PlayerEncounter.Battle.WinningSide == PlayerEncounter.Battle.PlayerSide;
				PlayerEncounter.Current.FinalizeBattle();
				RivalGangMovingInIssueBehavior.Instance.HandlePlayerEncounterResult(flag);
			}
		}

		public override void SyncData(IDataStore dataStore)
		{
		}

		[GameMenuInitializationHandler("rival_gang_quest_after_fight")]
		[GameMenuInitializationHandler("rival_gang_quest_wait_duration_is_over")]
		private static void game_menu_rival_gang_quest_end_on_init(MenuCallbackArgs args)
		{
			Settlement currentSettlement = Settlement.CurrentSettlement;
			if (currentSettlement != null)
			{
				args.MenuContext.SetBackgroundMeshName(currentSettlement.SettlementComponent.WaitMeshName);
			}
		}

		private const IssueBase.IssueFrequency RivalGangLeaderIssueFrequency = 1;

		private RivalGangMovingInIssueBehavior.RivalGangMovingInIssueQuest _cachedQuest;

		public class RivalGangMovingInIssueTypeDefiner : SaveableTypeDefiner
		{
			public RivalGangMovingInIssueTypeDefiner()
				: base(310000)
			{
			}

			protected override void DefineClassTypes()
			{
				base.AddClassDefinition(typeof(RivalGangMovingInIssueBehavior.RivalGangMovingInIssue), 1, null);
				base.AddClassDefinition(typeof(RivalGangMovingInIssueBehavior.RivalGangMovingInIssueQuest), 2, null);
			}
		}

		public class RivalGangMovingInIssue : IssueBase
		{
			public override IssueBase.AlternativeSolutionScaleFlag AlternativeSolutionScaleFlags
			{
				get
				{
					return 12;
				}
			}

			[SaveableProperty(207)]
			public Hero RivalGangLeader { get; private set; }

			public override int AlternativeSolutionBaseNeededMenCount
			{
				get
				{
					return 4 + MathF.Ceiling(6f * base.IssueDifficultyMultiplier);
				}
			}

			protected override int AlternativeSolutionBaseDurationInDaysInternal
			{
				get
				{
					return 3 + MathF.Ceiling(5f * base.IssueDifficultyMultiplier);
				}
			}

			protected override int RewardGold
			{
				get
				{
					return (int)(600f + 1700f * base.IssueDifficultyMultiplier);
				}
			}

			protected override int CompanionSkillRewardXP
			{
				get
				{
					return (int)(750f + 1000f * base.IssueDifficultyMultiplier);
				}
			}

			public override TextObject IssueBriefByIssueGiver
			{
				get
				{
					TextObject textObject = new TextObject("{=GXk6f9ah}I've got a problem... And {?TARGET_NOTABLE.GENDER}her{?}his{\\?} name is {TARGET_NOTABLE.LINK}. {?TARGET_NOTABLE.GENDER}Her{?}His{\\?} people have been coming around outside the walls, robbing the dice-players and the drinkers enjoying themselves under our protection. Me and my boys are eager to teach them a lesson but I figure some extra muscle wouldn't hurt.", null);
					if (RandomOwnerExtensions.RandomInt(base.IssueOwner, 2) == 0)
					{
						textObject = new TextObject("{=rgTGzfzI}Yeah. I have a problem all right. {?TARGET_NOTABLE.GENDER}Her{?}His{\\?} name is {TARGET_NOTABLE.LINK}. {?TARGET_NOTABLE.GENDER}Her{?}His{\\?} people have been bothering shop owners under our protection, demanding money and making threats. Let me tell you something - those shop owners are my cows, and no one else gets to milk them. We're ready to teach these interlopers a lesson, but I could use some help.", null);
					}
					if (this.RivalGangLeader != null)
					{
						StringHelpers.SetCharacterProperties("TARGET_NOTABLE", this.RivalGangLeader.CharacterObject, textObject, false);
					}
					return textObject;
				}
			}

			public override TextObject IssueAcceptByPlayer
			{
				get
				{
					return new TextObject("{=kc6vCycY}What exactly do you want me to do?", null);
				}
			}

			public override TextObject IssueQuestSolutionExplanationByIssueGiver
			{
				get
				{
					TextObject textObject = new TextObject("{=tyyAfWRR}We already had a small scuffle with them recently. They'll be waiting for us to come down hard. Instead, we'll hold off for {NUMBER} days. Let them think that we're backing off… Then, after {NUMBER} days, your men and mine will hit them in the middle of the night when they least expect it. I'll send you a messenger when the time comes and we'll strike them down together.", null);
					textObject.SetTextVariable("NUMBER", 4);
					return textObject;
				}
			}

			public override TextObject IssueAlternativeSolutionExplanationByIssueGiver
			{
				get
				{
					TextObject textObject = new TextObject("{=sSIjPCPO}If you'd rather not go into the fray yourself, you can leave me one of your companions together with {TROOP_COUNT} or so good men. If they stuck around for {RETURN_DAYS} days or so, I'd count it a very big favor.", null);
					textObject.SetTextVariable("TROOP_COUNT", base.GetTotalAlternativeSolutionNeededMenCount());
					textObject.SetTextVariable("RETURN_DAYS", base.GetTotalAlternativeSolutionDurationInDays());
					return textObject;
				}
			}

			protected override TextObject AlternativeSolutionStartLog
			{
				get
				{
					TextObject textObject = new TextObject("{=ymbVPod1}{ISSUE_GIVER.LINK}, a gang leader from {SETTLEMENT}, has told you about a new gang that is trying to get a hold on the town. You asked {COMPANION.LINK} to take {TROOP_COUNT} of your best men to stay with {ISSUE_GIVER.LINK} and help {?ISSUE_GIVER.GENDER}her{?}him{\\?} in the coming gang war. They should return to you in {RETURN_DAYS} days.", null);
					StringHelpers.SetCharacterProperties("ISSUE_GIVER", base.IssueOwner.CharacterObject, textObject, false);
					StringHelpers.SetCharacterProperties("COMPANION", base.AlternativeSolutionHero.CharacterObject, textObject, false);
					textObject.SetTextVariable("SETTLEMENT", base.IssueOwner.CurrentSettlement.EncyclopediaLinkWithName);
					textObject.SetTextVariable("TROOP_COUNT", this.AlternativeSolutionSentTroops.TotalManCount - 1);
					textObject.SetTextVariable("RETURN_DAYS", base.GetTotalAlternativeSolutionDurationInDays());
					return textObject;
				}
			}

			public override TextObject IssueQuestSolutionAcceptByPlayer
			{
				get
				{
					return new TextObject("{=LdCte9H0}I'll fight the other gang with you myself.", null);
				}
			}

			public override TextObject IssueAlternativeSolutionAcceptByPlayer
			{
				get
				{
					return new TextObject("{=AdbiUqtT}I'm busy, but I will leave a companion and some men.", null);
				}
			}

			public override TextObject IssueAlternativeSolutionResponseByIssueGiver
			{
				get
				{
					return new TextObject("{=0enbhess}Thank you. I'm sure your guys are worth their salt..", null);
				}
			}

			public override TextObject IssueDiscussAlternativeSolution
			{
				get
				{
					return new TextObject("{=QR0V8Ae5}Our lads are well hidden nearby, waiting for the signal to go get those bastards. I won't forget this little favor you're doing me.", null);
				}
			}

			public override bool IsThereAlternativeSolution
			{
				get
				{
					return true;
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
					TextObject textObject = new TextObject("{=vAjgn7yx}Rival Gang Moving In at {SETTLEMENT}", null);
					string text = "SETTLEMENT";
					Settlement issueSettlement = base.IssueSettlement;
					textObject.SetTextVariable(text, ((issueSettlement != null) ? issueSettlement.Name : null) ?? base.IssueOwner.HomeSettlement.Name);
					return textObject;
				}
			}

			public override TextObject Description
			{
				get
				{
					return new TextObject("{=H4EVfKAh}Gang leader needs help to beat the rival gang.", null);
				}
			}

			public override TextObject IssueAsRumorInSettlement
			{
				get
				{
					TextObject textObject = new TextObject("{=C9feTaca}I hear {QUEST_GIVER.LINK} is going to sort it out with {RIVAL_GANG_LEADER.LINK} once and for all.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.IssueOwner.CharacterObject, textObject, false);
					StringHelpers.SetCharacterProperties("RIVAL_GANG_LEADER", this.RivalGangLeader.CharacterObject, textObject, false);
					return textObject;
				}
			}

			protected override bool IssueQuestCanBeDuplicated
			{
				get
				{
					return false;
				}
			}

			public RivalGangMovingInIssue(Hero issueOwner, Hero rivalGangLeader)
				: base(issueOwner, CampaignTime.DaysFromNow(15f))
			{
				this.RivalGangLeader = rivalGangLeader;
			}

			public override void OnHeroCanHaveQuestOrIssueInfoIsRequested(Hero hero, ref bool result)
			{
				if (hero == this.RivalGangLeader)
				{
					result = false;
				}
			}

			protected override float GetIssueEffectAmountInternal(IssueEffect issueEffect)
			{
				if (issueEffect == DefaultIssueEffects.IssueOwnerPower)
				{
					return -0.2f;
				}
				if (issueEffect == DefaultIssueEffects.SettlementSecurity)
				{
					return -0.5f;
				}
				return 0f;
			}

			protected override void AlternativeSolutionEndWithSuccessConsequence()
			{
				this.RelationshipChangeWithIssueOwner = 5;
				ChangeRelationAction.ApplyPlayerRelation(this.RivalGangLeader, -5, true, true);
				base.IssueOwner.AddPower(10f);
			}

			protected override void AlternativeSolutionEndWithFailureConsequence()
			{
				this.RelationshipChangeWithIssueOwner = -5;
				base.IssueSettlement.Town.Security -= 10f;
				base.IssueOwner.AddPower(-10f);
			}

			public override ValueTuple<SkillObject, int> GetAlternativeSolutionSkill(Hero hero)
			{
				int skillValue = hero.GetSkillValue(DefaultSkills.OneHanded);
				int skillValue2 = hero.GetSkillValue(DefaultSkills.TwoHanded);
				int skillValue3 = hero.GetSkillValue(DefaultSkills.Polearm);
				int skillValue4 = hero.GetSkillValue(DefaultSkills.Roguery);
				if (skillValue >= skillValue2 && skillValue >= skillValue3 && skillValue >= skillValue4)
				{
					return new ValueTuple<SkillObject, int>(DefaultSkills.OneHanded, 150);
				}
				if (skillValue2 >= skillValue3 && skillValue2 >= skillValue4)
				{
					return new ValueTuple<SkillObject, int>(DefaultSkills.TwoHanded, 150);
				}
				if (skillValue3 < skillValue4)
				{
					return new ValueTuple<SkillObject, int>(DefaultSkills.Roguery, 120);
				}
				return new ValueTuple<SkillObject, int>(DefaultSkills.Polearm, 150);
			}

			public override bool AlternativeSolutionCondition(out TextObject explanation)
			{
				explanation = TextObject.Empty;
				return QuestHelper.CheckRosterForAlternativeSolution(MobileParty.MainParty.MemberRoster, base.GetTotalAlternativeSolutionNeededMenCount(), ref explanation, 2, false);
			}

			public override bool DoTroopsSatisfyAlternativeSolution(TroopRoster troopRoster, out TextObject explanation)
			{
				explanation = TextObject.Empty;
				return QuestHelper.CheckRosterForAlternativeSolution(troopRoster, base.GetTotalAlternativeSolutionNeededMenCount(), ref explanation, 2, false);
			}

			public override bool IsTroopTypeNeededByAlternativeSolution(CharacterObject character)
			{
				return character.Tier >= 2;
			}

			protected override void OnGameLoad()
			{
			}

			protected override QuestBase GenerateIssueQuest(string questId)
			{
				return new RivalGangMovingInIssueBehavior.RivalGangMovingInIssueQuest(questId, base.IssueOwner, this.RivalGangLeader, 8, this.RewardGold, base.IssueDifficultyMultiplier);
			}

			public override IssueBase.IssueFrequency GetFrequency()
			{
				return 1;
			}

			protected override bool CanPlayerTakeQuestConditions(Hero issueGiver, out IssueBase.PreconditionFlags flag, out Hero relationHero, out SkillObject skill)
			{
				flag = 0;
				relationHero = null;
				skill = null;
				if (Hero.MainHero.IsWounded)
				{
					flag = (int)(flag | 32U);
				}
				if (issueGiver.GetRelationWithPlayer() < -10f)
				{
					flag = (int)(flag | 1U);
					relationHero = issueGiver;
				}
				if (MobileParty.MainParty.MemberRoster.TotalHealthyCount < 5)
				{
					flag = (int)(flag | 256U);
				}
				if (base.IssueOwner.CurrentSettlement.OwnerClan == Clan.PlayerClan)
				{
					flag = (int)(flag | 32768U);
				}
				return flag == 0U;
			}

			public override bool IssueStayAliveConditions()
			{
				return this.RivalGangLeader.IsAlive && base.IssueOwner.CurrentSettlement.OwnerClan != Clan.PlayerClan && base.IssueOwner.CurrentSettlement.Town.Security <= 80f;
			}

			protected override void CompleteIssueWithTimedOutConsequences()
			{
			}

			internal static void AutoGeneratedStaticCollectObjectsRivalGangMovingInIssue(object o, List<object> collectedObjects)
			{
				((RivalGangMovingInIssueBehavior.RivalGangMovingInIssue)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				base.AutoGeneratedInstanceCollectObjects(collectedObjects);
				collectedObjects.Add(this.RivalGangLeader);
			}

			internal static object AutoGeneratedGetMemberValueRivalGangLeader(object o)
			{
				return ((RivalGangMovingInIssueBehavior.RivalGangMovingInIssue)o).RivalGangLeader;
			}

			private const int AlternativeSolutionRelationBonus = 5;

			private const int AlternativeSolutionFailRelationPenalty = -5;

			private const int AlternativeSolutionNotablePowerBonus = 10;

			private const int AlternativeSolutionFailNotablePowerPenalty = -10;

			private const int AlternativeSolutionRivalGangLeaderRelationPenalty = -5;

			private const int AlternativeSolutionMinimumTroopTier = 2;

			private const int IssueDuration = 15;

			private const int MinimumRequiredMenCount = 5;

			private const int IssueQuestDuration = 8;

			private const int MeleeSkillValueThreshold = 150;

			private const int RoguerySkillValueThreshold = 120;
		}

		public class RivalGangMovingInIssueQuest : QuestBase
		{
			private int PreparationDurationInDays
			{
				get
				{
					return (int)(this._timeoutDurationInDays / 2f);
				}
			}

			private TextObject _onQuestStartedLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=dav5rmDd}{QUEST_GIVER.LINK}, a gang leader from {SETTLEMENT} has told you about a rival that is trying to get a foothold in {?QUEST_GIVER.GENDER}her{?}his{\\?} town. {?QUEST_GIVER.GENDER}She{?}He{\\?} asked you to wait {DAY_COUNT} days so that the other gang lets its guard down.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					textObject.SetTextVariable("SETTLEMENT", this._questSettlement.EncyclopediaLinkWithName);
					textObject.SetTextVariable("DAY_COUNT", this.PreparationDurationInDays);
					return textObject;
				}
			}

			private TextObject _onQuestFailedWithRejectionLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=aXMg9M7t}You decided to stay out of the fight. {?QUEST_GIVER.GENDER}She{?}He{\\?} will certainly lose to the rival gang without your help.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					return textObject;
				}
			}

			private TextObject _onQuestFailedWithBetrayalLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=Rf0QqRIX}You have chosen to side with the rival gang leader, {RIVAL_GANG_LEADER.LINK}. {QUEST_GIVER.LINK} must be furious.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					StringHelpers.SetCharacterProperties("RIVAL_GANG_LEADER", this._rivalGangLeader.CharacterObject, textObject, false);
					return textObject;
				}
			}

			private TextObject _onQuestFailedWithDefeatLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=du3dpMaV}You were unable to defeat {RIVAL_GANG_LEADER.LINK}'s gang, and thus failed to fulfill your commitment to {QUEST_GIVER.LINK}.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					StringHelpers.SetCharacterProperties("RIVAL_GANG_LEADER", this._rivalGangLeader.CharacterObject, textObject, false);
					return textObject;
				}
			}

			private TextObject _onQuestSucceededLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=vpUl7xcy}You have defeated the rival gang and protected the interests of {QUEST_GIVER.LINK} in {SETTLEMENT}.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					textObject.SetTextVariable("SETTLEMENT", this._questSettlement.EncyclopediaLinkWithName);
					return textObject;
				}
			}

			private TextObject _onQuestPreperationsCompletedLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=OIBiRTRP}{QUEST_GIVER.LINK} is waiting for you at {SETTLEMENT}.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					textObject.SetTextVariable("SETTLEMENT", this._questSettlement.EncyclopediaLinkWithName);
					return textObject;
				}
			}

			private TextObject _onQuestCancelledDueToWarLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=vaUlAZba}Your clan is now at war with {QUEST_GIVER.LINK}. Your agreement with {QUEST_GIVER.LINK} was canceled.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
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

			private TextObject _onQuestCancelledDueToSiegeLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=s1GWSE9Y}{QUEST_GIVER.LINK} cancels your plans due to the siege of {SETTLEMENT}. {?QUEST_GIVER.GENDER}She{?}He{\\?} has worse troubles than {?QUEST_GIVER.GENDER}her{?}his{\\?} quarrel with the rival gang.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					textObject.SetTextVariable("SETTLEMENT", this._questSettlement.EncyclopediaLinkWithName);
					return textObject;
				}
			}

			private TextObject _playerStartedAlleyFightWithRivalGangLeader
			{
				get
				{
					TextObject textObject = new TextObject("{=OeKgpuAv}After your attack on the rival gang's alley, {QUEST_GIVER.LINK} decided to change {?QUEST_GIVER.GENDER}her{?}his{\\?} plans, and doesn't need your assistance anymore. Quest is canceled.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					return textObject;
				}
			}

			private TextObject _playerStartedAlleyFightWithQuestgiver
			{
				get
				{
					TextObject textObject = new TextObject("{=VPGkIqlh}Your attack on {QUEST_GIVER.LINK}'s gang has angered {?QUEST_GIVER.GENDER}her{?}him{\\?} and {?QUEST_GIVER.GENDER}she{?}he{\\?} broke off the agreement that you had.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					return textObject;
				}
			}

			private TextObject OwnerOfQuestSettlementIsPlayerClanLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=KxEnNEoD}Your clan is now owner of the settlement. As the {?PLAYER.GENDER}lady{?}lord{\\?} of the settlement you cannot get involved in gang wars anymore. Your agreement with the {QUEST_GIVER.LINK} has canceled.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					StringHelpers.SetCharacterProperties("PLAYER", CharacterObject.PlayerCharacter, textObject, false);
					return textObject;
				}
			}

			public RivalGangMovingInIssueQuest(string questId, Hero questGiver, Hero rivalGangLeader, int duration, int rewardGold, float issueDifficulty)
				: base(questId, questGiver, CampaignTime.DaysFromNow((float)duration), rewardGold)
			{
				this._rivalGangLeader = rivalGangLeader;
				this._rewardGold = rewardGold;
				this._issueDifficulty = issueDifficulty;
				this._timeoutDurationInDays = (float)duration;
				this._preparationCompletionTime = CampaignTime.DaysFromNow((float)this.PreparationDurationInDays);
				this._questTimeoutTime = CampaignTime.DaysFromNow(this._timeoutDurationInDays);
				this.InitializeQuestSettlement();
				this.SetDialogs();
				base.InitializeQuestOnCreation();
			}

			public override TextObject Title
			{
				get
				{
					TextObject textObject = new TextObject("{=WVorNMNc}Rival Gang Moving In At {SETTLEMENT}", null);
					textObject.SetTextVariable("SETTLEMENT", this._questSettlement.Name);
					return textObject;
				}
			}

			public override bool IsRemainingTimeHidden
			{
				get
				{
					return false;
				}
			}

			protected override void InitializeQuestOnGameLoad()
			{
				this.InitializeQuestSettlement();
				this.SetDialogs();
				Campaign.Current.ConversationManager.AddDialogFlow(this.GetRivalGangLeaderDialogFlow(), this);
				Campaign.Current.ConversationManager.AddDialogFlow(this.GetQuestGiverPreparationCompletedDialogFlow(), this);
				MobileParty rivalGangLeaderParty = this._rivalGangLeaderParty;
				if (rivalGangLeaderParty == null)
				{
					return;
				}
				rivalGangLeaderParty.SetPartyUsedByQuest(true);
			}

			private void InitializeQuestSettlement()
			{
				this._questSettlement = base.QuestGiver.CurrentSettlement;
			}

			protected override void SetDialogs()
			{
				this.OfferDialogFlow = DialogFlow.CreateDialogFlow("issue_classic_quest_start", 100).NpcLine("{=Fwm0PwVb}Great. As I said we need minimum of {NUMBER} days, so they'll let their guard down. I will let you know when it's time. Remember, we wait for the dark of the night to strike.", null, null).Condition(delegate
				{
					MBTextManager.SetTextVariable("SETTLEMENT", this._questSettlement.EncyclopediaLinkWithName, false);
					MBTextManager.SetTextVariable("NUMBER", this.PreparationDurationInDays);
					return Hero.OneToOneConversationHero == base.QuestGiver;
				})
					.Consequence(new ConversationSentence.OnConsequenceDelegate(this.OnQuestAccepted))
					.CloseDialog();
				this.DiscussDialogFlow = DialogFlow.CreateDialogFlow("quest_discuss", 100).NpcLine("{=z43j3Tzq}I'm still gathering my men for the fight. I'll send a runner for you when the time comes.", null, null).Condition(delegate
				{
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, null, false);
					return Hero.OneToOneConversationHero == base.QuestGiver && !this._isFinalStage && !this._preparationsComplete;
				})
					.BeginPlayerOptions()
					.PlayerOption("{=4IHRAmnA}All right. I am waiting for your runner.", null)
					.NpcLine("{=xEs830bT}You'll know right away once the preparations are complete. Just don't leave town.", null, null)
					.CloseDialog()
					.PlayerOption("{=6g8qvD2M}I can't just hang on here forever. Be quick about it.", null)
					.NpcLine("{=lM7AscLo}I'm getting this together as quickly as I can.", null, null)
					.CloseDialog()
					.EndPlayerOptions()
					.CloseDialog();
			}

			private DialogFlow GetRivalGangLeaderDialogFlow()
			{
				return DialogFlow.CreateDialogFlow("start", 125).NpcLine("{=IfeN8lYd}Coming to fight me, eh? Did {QUEST_GIVER.LINK} put you up to this? Look, there's no need for bloodshed. This town is big enough for all of us. But... if bloodshed is what you want, I will be happy to provide.", null, null).Condition(delegate
				{
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, null, false);
					return Hero.OneToOneConversationHero == this._rivalGangLeader && this._isReadyToBeFinalized;
				})
					.NpcLine("{=WSJxl2Hu}What I want to say is... You don't need to be a part of this. I'll double whatever {?QUEST_GIVER.GENDER}she{?}he{\\?} is paying you if you join us.", null, null)
					.BeginPlayerOptions()
					.PlayerOption("{=GPBja02V}I gave my word to {QUEST_GIVER.LINK}, and I won't be bought.", null)
					.Consequence(delegate
					{
						Campaign.Current.ConversationManager.ConversationEndOneShot += delegate
						{
							CombatMissionWithDialogueController missionBehavior = Mission.Current.GetMissionBehavior<CombatMissionWithDialogueController>();
							if (missionBehavior == null)
							{
								return;
							}
							missionBehavior.StartFight(false);
						};
					})
					.NpcLine("{=OSgBicif}You will regret this!", null, null)
					.CloseDialog()
					.PlayerOption("{=RB4uQpPV}You're going to pay me a lot then, {REWARD}{GOLD_ICON} to be exact. But at that price, I agree.", null)
					.Condition(delegate
					{
						MBTextManager.SetTextVariable("REWARD", this._rewardGold * 2);
						return true;
					})
					.Consequence(delegate
					{
						Campaign.Current.ConversationManager.ConversationEndOneShot += delegate
						{
							this._hasBetrayedQuestGiver = true;
							CombatMissionWithDialogueController missionBehavior2 = Mission.Current.GetMissionBehavior<CombatMissionWithDialogueController>();
							if (missionBehavior2 == null)
							{
								return;
							}
							missionBehavior2.StartFight(true);
						};
					})
					.NpcLine("{=5jW4FVDc}Welcome to our ranks then. Let's kill those bastards!", null, null)
					.CloseDialog()
					.EndPlayerOptions()
					.CloseDialog();
			}

			private DialogFlow GetQuestGiverPreparationCompletedDialogFlow()
			{
				return DialogFlow.CreateDialogFlow("start", 125).BeginNpcOptions().NpcOption(new TextObject("{=hM7LSuB1}Good to see you. But we still need to wait until after dusk. {HERO.LINK}'s men may be watching, so let's keep our distance from each other until night falls.", null), delegate
				{
					StringHelpers.SetCharacterProperties("HERO", this._rivalGangLeader.CharacterObject, null, false);
					return Hero.OneToOneConversationHero == base.QuestGiver && !this._isFinalStage && this._preparationCompletionTime.IsPast && (!this._preparationsComplete || !CampaignTime.Now.IsNightTime);
				}, null, null)
					.CloseDialog()
					.NpcOption("{=JxNlB547}Are you ready for the fight?", () => Hero.OneToOneConversationHero == base.QuestGiver && this._preparationsComplete && !this._isFinalStage && CampaignTime.Now.IsNightTime, null, null)
					.EndNpcOptions()
					.BeginPlayerOptions()
					.PlayerOption("{=NzMX0s21}I am ready.", null)
					.Condition(() => !Hero.MainHero.IsWounded)
					.NpcLine("{=dNjepcKu}Let's finish this!", null, null)
					.Consequence(delegate
					{
						Campaign.Current.ConversationManager.ConversationEndOneShot += this.rival_gang_start_fight_on_consequence;
					})
					.CloseDialog()
					.PlayerOption("{=B2Donbwz}I need more time.", null)
					.Condition(() => !Hero.MainHero.IsWounded)
					.NpcLine("{=advPT3WY}You'd better hurry up!", null, null)
					.Consequence(delegate
					{
						Campaign.Current.ConversationManager.ConversationEndOneShot += this.rival_gang_need_more_time_on_consequence;
					})
					.CloseDialog()
					.PlayerOption("{=QaN26CZ5}My wounds are still fresh. I need some time to recover.", null)
					.Condition(() => Hero.MainHero.IsWounded)
					.NpcLine("{=s0jKaYo0}We must attack before the rival gang hears about our plan. You'd better hurry up!", null, null)
					.CloseDialog()
					.EndPlayerOptions()
					.CloseDialog();
			}

			public override void OnHeroCanDieInfoIsRequested(Hero hero, KillCharacterAction.KillCharacterActionDetail causeOfDeath, ref bool result)
			{
				if (hero == base.QuestGiver || hero == this._rivalGangLeader)
				{
					result = false;
				}
			}

			private void rival_gang_start_fight_on_consequence()
			{
				this._isFinalStage = true;
				if (Mission.Current != null)
				{
					Mission.Current.EndMission();
				}
				Campaign.Current.GameMenuManager.SetNextMenu("rival_gang_quest_before_fight");
			}

			private void rival_gang_need_more_time_on_consequence()
			{
				if (Campaign.Current.CurrentMenuContext.GameMenu.StringId == "rival_gang_quest_wait_duration_is_over")
				{
					Campaign.Current.GameMenuManager.SetNextMenu("town_wait_menus");
				}
			}

			private void AddQuestGiverGangLeaderOnSuccessDialogFlow()
			{
				Campaign.Current.ConversationManager.AddDialogFlow(DialogFlow.CreateDialogFlow("start", 125).NpcLine("{=zNPzh5jO}Ah! Now that was as good a fight as any I've had. Here, take this purse, you deserve it.", null, null).Condition(delegate
				{
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, null, false);
					return base.IsOngoing && Hero.OneToOneConversationHero == base.QuestGiver;
				})
					.Consequence(delegate
					{
						Campaign.Current.ConversationManager.ConversationEndOneShot += this.OnQuestSucceeded;
					})
					.CloseDialog(), null);
			}

			private CharacterObject GetTroopTypeTemplateForDifficulty()
			{
				int difficultyRange = MBMath.ClampInt(MathF.Ceiling(this._issueDifficulty / 0.1f), 1, 10);
				CharacterObject characterObject;
				if (difficultyRange == 1)
				{
					characterObject = CharacterObject.All.FirstOrDefault((CharacterObject t) => t.StringId == "looter");
				}
				else if (difficultyRange == 10)
				{
					characterObject = CharacterObject.All.FirstOrDefault((CharacterObject t) => t.StringId == "mercenary_8");
				}
				else
				{
					characterObject = CharacterObject.All.FirstOrDefault((CharacterObject t) => t.StringId == "mercenary_" + (difficultyRange - 1));
				}
				if (characterObject == null)
				{
					Debug.FailedAssert("Can't find troop in rival gang leader quest", "C:\\Develop\\MB3\\Source\\Bannerlord\\SandBox\\Issues\\RivalGangMovingInIssueBehavior.cs", "GetTroopTypeTemplateForDifficulty", 756);
					characterObject = CharacterObject.All.First((CharacterObject t) => t.IsBasicTroop && t.IsSoldier);
				}
				return characterObject;
			}

			internal void StartAlleyBattle(Hero gangLeader)
			{
				this._rivalGangLeaderParty = MobileParty.CreateParty("rival_gang_leader_party", null, null);
				TextObject textObject = new TextObject("{=u4jhIFwG}{RIVAL_GANG_LEADER}'s Party", null);
				textObject.SetTextVariable("RIVAL_GANG_LEADER", this._rivalGangLeader.Name);
				this._rivalGangLeaderParty.InitializeMobilePartyAroundPosition(new TroopRoster(this._rivalGangLeaderParty.Party), new TroopRoster(this._rivalGangLeaderParty.Party), this._questSettlement.GatePosition, 1f, 0.5f);
				this._rivalGangLeaderParty.SetCustomName(textObject);
				EnterSettlementAction.ApplyForParty(this._rivalGangLeaderParty, this._questSettlement);
				this._rivalGangLeaderParty.SetPartyUsedByQuest(true);
				CharacterObject troopTypeTemplateForDifficulty = this.GetTroopTypeTemplateForDifficulty();
				this._rivalGangLeaderParty.MemberRoster.AddToCounts(troopTypeTemplateForDifficulty, 15, false, 0, 0, true, -1);
				ItemObject @object = MBObjectManager.Instance.GetObject<ItemObject>("cleaver_sword_t3");
				gangLeader.CharacterObject.FirstCivilianEquipment.AddEquipmentToSlotWithoutAgent(0, new EquipmentElement(@object, null, null, false));
				this._rivalGangLeaderParty.MemberRoster.AddToCounts(gangLeader.CharacterObject, 1, false, 0, 0, true, -1);
				foreach (TroopRosterElement troopRosterElement in PartyBase.MainParty.MemberRoster.GetTroopRoster())
				{
					if (!troopRosterElement.Character.IsPlayerCharacter)
					{
						this._playerTroops.Add(troopRosterElement);
					}
				}
				PartyBase.MainParty.MemberRoster.RemoveIf((TroopRosterElement t) => !t.Character.IsPlayerCharacter);
				PartyBase.MainParty.MemberRoster.AddToCounts(troopTypeTemplateForDifficulty, 20, false, 0, 0, true, -1);
				if (!Extensions.IsEmpty<TroopRosterElement>(this._playerTroops))
				{
					List<CharacterObject> list = new List<CharacterObject>();
					int num = 5;
					foreach (TroopRosterElement troopRosterElement2 in this._playerTroops.OrderByDescending((TroopRosterElement t) => t.Character.Level))
					{
						if (num <= 0)
						{
							break;
						}
						int num2 = 0;
						while (num2 < troopRosterElement2.Number - troopRosterElement2.WoundedNumber && num > 0)
						{
							list.Add(troopRosterElement2.Character);
							num--;
							num2++;
						}
					}
					foreach (CharacterObject characterObject in list)
					{
						PartyBase.MainParty.MemberRoster.AddToCounts(characterObject, 1, false, 0, 0, true, -1);
					}
				}
				PlayerEncounter.RestartPlayerEncounter(this._rivalGangLeaderParty.Party, PartyBase.MainParty, false);
				GameMenu.ActivateGameMenu("rival_gang_quest_after_fight");
				this._isReadyToBeFinalized = true;
				PlayerEncounter.StartBattle();
				PlayerEncounter.StartCombatMissionWithDialogueInTownCenter(gangLeader.CharacterObject, troopTypeTemplateForDifficulty);
			}

			internal void HandlePlayerEncounterResult(bool hasPlayerWon)
			{
				PlayerEncounter.Finish(false);
				EncounterManager.StartSettlementEncounter(MobileParty.MainParty, this._questSettlement);
				GameMenu.SwitchToMenu("town");
				PartyBase.MainParty.MemberRoster.RemoveIf((TroopRosterElement t) => !t.Character.IsPlayerCharacter);
				foreach (TroopRosterElement troopRosterElement in this._playerTroops)
				{
					PartyBase.MainParty.MemberRoster.AddToCounts(troopRosterElement.Character, troopRosterElement.Number, false, troopRosterElement.WoundedNumber, troopRosterElement.Xp, true, -1);
				}
				if (this._rivalGangLeader.PartyBelongedTo == this._rivalGangLeaderParty)
				{
					this._rivalGangLeaderParty.MemberRoster.AddToCounts(this._rivalGangLeader.CharacterObject, -1, false, 0, 0, true, -1);
				}
				if (hasPlayerWon)
				{
					if (!this._hasBetrayedQuestGiver)
					{
						this.AddQuestGiverGangLeaderOnSuccessDialogFlow();
						PlayerEncounter.LocationEncounter.CreateAndOpenMissionController(LocationComplex.Current.GetLocationOfCharacter(base.QuestGiver), null, base.QuestGiver.CharacterObject, null);
						return;
					}
					this.OnBattleWonWithBetrayal();
					return;
				}
				else
				{
					if (!this._hasBetrayedQuestGiver)
					{
						this.OnQuestFailedWithDefeat();
						return;
					}
					this.OnBattleLostWithBetrayal();
					return;
				}
			}

			protected override void RegisterEvents()
			{
				CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(this.HourlyTick));
				CampaignEvents.HeroKilledEvent.AddNonSerializedListener(this, new Action<Hero, Hero, KillCharacterAction.KillCharacterActionDetail, bool>(this.OnHeroKilled));
				CampaignEvents.OnPlayerBattleEndEvent.AddNonSerializedListener(this, new Action<MapEvent>(this.OnPlayerBattleEnd));
				CampaignEvents.WarDeclared.AddNonSerializedListener(this, new Action<IFaction, IFaction, DeclareWarAction.DeclareWarDetail>(this.OnWarDeclared));
				CampaignEvents.OnSiegeEventStartedEvent.AddNonSerializedListener(this, new Action<SiegeEvent>(this.OnSiegeEventStarted));
				CampaignEvents.ClanChangedKingdom.AddNonSerializedListener(this, new Action<Clan, Kingdom, Kingdom, ChangeKingdomAction.ChangeKingdomActionDetail, bool>(this.OnClanChangedKingdom));
				CampaignEvents.OnSettlementOwnerChangedEvent.AddNonSerializedListener(this, new Action<Settlement, bool, Hero, Hero, Hero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail>(this.OnSettlementOwnerChanged));
			}

			private void OnSettlementOwnerChanged(Settlement settlement, bool openToClaim, Hero newOwner, Hero oldOwner, Hero capturerHero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail detail)
			{
				if (settlement == base.QuestGiver.CurrentSettlement && newOwner == Hero.MainHero)
				{
					base.AddLog(this.OwnerOfQuestSettlementIsPlayerClanLogText, false);
					base.CompleteQuestWithCancel(null);
				}
			}

			public override void OnHeroCanHaveQuestOrIssueInfoIsRequested(Hero hero, ref bool result)
			{
				if (hero == this._rivalGangLeader)
				{
					result = false;
				}
			}

			private void OnClanChangedKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail detail, bool showNotification = true)
			{
				if (base.QuestGiver.CurrentSettlement.MapFaction.IsAtWarWith(Hero.MainHero.MapFaction))
				{
					base.CompleteQuestWithCancel(this._onQuestCancelledDueToWarLogText);
				}
			}

			private void OnWarDeclared(IFaction faction1, IFaction faction2, DeclareWarAction.DeclareWarDetail detail)
			{
				QuestHelper.CheckWarDeclarationAndFailOrCancelTheQuest(this, faction1, faction2, detail, this._playerDeclaredWarQuestLogText, this._onQuestCancelledDueToWarLogText);
			}

			private void OnSiegeEventStarted(SiegeEvent siegeEvent)
			{
				if (siegeEvent.BesiegedSettlement == this._questSettlement)
				{
					base.AddLog(this._onQuestCancelledDueToSiegeLogText, false);
					base.CompleteQuestWithCancel(null);
				}
			}

			private void HourlyTick()
			{
				if (RivalGangMovingInIssueBehavior.Instance != null && RivalGangMovingInIssueBehavior.Instance.IsOngoing && ((float)RivalGangMovingInIssueBehavior.Instance.PreparationDurationInDays - RivalGangMovingInIssueBehavior.Instance._preparationCompletionTime.RemainingDaysFromNow) / (float)RivalGangMovingInIssueBehavior.Instance.PreparationDurationInDays >= 1f && !this._preparationsComplete && CampaignTime.Now.IsNightTime)
				{
					this.OnGuestGiverPreparationsCompleted();
				}
			}

			private void OnHeroKilled(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail detail, bool showNotification = true)
			{
				if (victim == this._rivalGangLeader)
				{
					TextObject textObject = ((detail == 7) ? this.TargetHeroDisappearedLogText : this.TargetHeroDiedLogText);
					StringHelpers.SetCharacterProperties("QUEST_TARGET", this._rivalGangLeader.CharacterObject, textObject, false);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					base.AddLog(textObject, false);
					base.CompleteQuestWithCancel(null);
				}
			}

			private void OnPlayerBattleEnd(MapEvent mapEvent)
			{
				if (!this._isReadyToBeFinalized && mapEvent.AttackerSide.IsMainPartyAmongParties() && mapEvent.DefenderSide.LeaderParty.Owner != null)
				{
					if (mapEvent.DefenderSide.LeaderParty.Owner == this._rivalGangLeader)
					{
						this.OnPlayerAttackedRivalGangAlley();
						return;
					}
					if (mapEvent.DefenderSide.LeaderParty.Owner == base.QuestGiver)
					{
						this.OnPlayerAttackedQuestGiverAlley();
					}
				}
			}

			private void OnPlayerAttackedRivalGangAlley()
			{
				base.AddLog(this._playerStartedAlleyFightWithRivalGangLeader, false);
				base.CompleteQuestWithCancel(null);
			}

			private void OnPlayerAttackedQuestGiverAlley()
			{
				TraitLevelingHelper.OnIssueSolvedThroughQuest(base.QuestGiver, new Tuple<TraitObject, int>[]
				{
					new Tuple<TraitObject, int>(DefaultTraits.Honor, -150)
				});
				base.QuestGiver.AddPower(-10f);
				ChangeRelationAction.ApplyPlayerRelation(base.QuestGiver, -8, true, true);
				this._questSettlement.Town.Security += -10f;
				base.AddLog(this._playerStartedAlleyFightWithQuestgiver, false);
				base.CompleteQuestWithFail(null);
			}

			protected override void OnTimedOut()
			{
				this.OnQuestFailedWithRejectionOrTimeout();
			}

			private void OnGuestGiverPreparationsCompleted()
			{
				this._preparationsComplete = true;
				if (Settlement.CurrentSettlement != null && Settlement.CurrentSettlement == this._questSettlement && Campaign.Current.CurrentMenuContext != null && Campaign.Current.CurrentMenuContext.GameMenu.StringId == "town_wait_menus")
				{
					Campaign.Current.CurrentMenuContext.SwitchToMenu("rival_gang_quest_wait_duration_is_over");
				}
				TextObject textObject = new TextObject("{=DUKbtlNb}{QUEST_GIVER.LINK} has finally sent a messenger telling you it's time to meet {?QUEST_GIVER.GENDER}her{?}him{\\?} and join the fight.", null);
				StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
				base.AddLog(this._onQuestPreperationsCompletedLogText, false);
				MBInformationManager.AddQuickInformation(textObject, 0, null, "");
			}

			private void OnQuestAccepted()
			{
				base.StartQuest();
				this._onQuestStartedLog = base.AddLog(this._onQuestStartedLogText, false);
				Campaign.Current.ConversationManager.AddDialogFlow(this.GetRivalGangLeaderDialogFlow(), this);
				Campaign.Current.ConversationManager.AddDialogFlow(this.GetQuestGiverPreparationCompletedDialogFlow(), this);
			}

			private void OnQuestSucceeded()
			{
				this._onQuestSucceededLog = base.AddLog(this._onQuestSucceededLogText, false);
				GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, this._rewardGold, false);
				if (base.QuestGiver.IsAlive)
				{
					this.RelationshipChangeWithQuestGiver = 5;
					TraitLevelingHelper.OnIssueSolvedThroughQuest(base.QuestGiver, new Tuple<TraitObject, int>[]
					{
						new Tuple<TraitObject, int>(DefaultTraits.Honor, 50)
					});
					base.QuestGiver.AddPower(10f);
				}
				if (this._rivalGangLeader.IsAlive)
				{
					ChangeRelationAction.ApplyPlayerRelation(this._rivalGangLeader, -5, true, true);
				}
				if (this._rivalGangLeaderParty != null && this._rivalGangLeaderParty.IsActive)
				{
					DestroyPartyAction.Apply(null, this._rivalGangLeaderParty);
				}
				base.CompleteQuestWithSuccess();
			}

			private void OnQuestFailedWithRejectionOrTimeout()
			{
				base.AddLog(this._onQuestFailedWithRejectionLogText, false);
				TraitLevelingHelper.OnIssueFailed(base.QuestGiver, new Tuple<TraitObject, int>[]
				{
					new Tuple<TraitObject, int>(DefaultTraits.Honor, -20)
				});
				this.ApplyQuestFailConsequences();
			}

			private void OnBattleWonWithBetrayal()
			{
				base.AddLog(this._onQuestFailedWithBetrayalLogText, false);
				if (!this._rivalGangLeader.IsDead)
				{
					ChangeRelationAction.ApplyPlayerRelation(this._rivalGangLeader, 5, true, true);
				}
				GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, this._rewardGold * 2, false);
				TraitLevelingHelper.OnIssueSolvedThroughBetrayal(base.QuestGiver, new Tuple<TraitObject, int>[]
				{
					new Tuple<TraitObject, int>(DefaultTraits.Honor, -100)
				});
				this.ApplyQuestFailConsequences();
				base.CompleteQuestWithBetrayal(null);
			}

			private void OnBattleLostWithBetrayal()
			{
				base.AddLog(this._onQuestFailedWithBetrayalLogText, false);
				if (!this._rivalGangLeader.IsDead)
				{
					ChangeRelationAction.ApplyPlayerRelation(this._rivalGangLeader, 5, true, true);
				}
				TraitLevelingHelper.OnIssueSolvedThroughBetrayal(base.QuestGiver, new Tuple<TraitObject, int>[]
				{
					new Tuple<TraitObject, int>(DefaultTraits.Honor, -100)
				});
				this._questSettlement.Town.Security += -10f;
				this.ApplyQuestFailConsequences();
				base.CompleteQuestWithBetrayal(null);
			}

			private void OnQuestFailedWithDefeat()
			{
				base.AddLog(this._onQuestFailedWithDefeatLogText, false);
				this.ApplyQuestFailConsequences();
				base.CompleteQuestWithFail(null);
			}

			private void ApplyQuestFailConsequences()
			{
				this.RelationshipChangeWithQuestGiver = -15;
				base.QuestGiver.AddPower(-10f);
				this._questSettlement.Town.Security += -10f;
				if (this._rivalGangLeaderParty != null && this._rivalGangLeaderParty.IsActive)
				{
					DestroyPartyAction.Apply(null, this._rivalGangLeaderParty);
				}
			}

			protected override void OnFinalize()
			{
				if (this._rivalGangLeader != null && this._rivalGangLeader.IsAlive)
				{
					MobileParty partyBelongedTo = this._rivalGangLeader.PartyBelongedTo;
					if (partyBelongedTo != null)
					{
						partyBelongedTo.MemberRoster.RemoveTroop(this._rivalGangLeader.CharacterObject, 1, default(UniqueTroopDescriptor), 0);
					}
					EnterSettlementAction.ApplyForCharacterOnly(this._rivalGangLeader, this._questSettlement);
				}
			}

			internal static void AutoGeneratedStaticCollectObjectsRivalGangMovingInIssueQuest(object o, List<object> collectedObjects)
			{
				((RivalGangMovingInIssueBehavior.RivalGangMovingInIssueQuest)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				base.AutoGeneratedInstanceCollectObjects(collectedObjects);
				collectedObjects.Add(this._rivalGangLeader);
				collectedObjects.Add(this._rivalGangLeaderParty);
				CampaignTime.AutoGeneratedStaticCollectObjectsCampaignTime(this._preparationCompletionTime, collectedObjects);
				CampaignTime.AutoGeneratedStaticCollectObjectsCampaignTime(this._questTimeoutTime, collectedObjects);
				collectedObjects.Add(this._playerTroops);
			}

			internal static object AutoGeneratedGetMemberValue_rivalGangLeader(object o)
			{
				return ((RivalGangMovingInIssueBehavior.RivalGangMovingInIssueQuest)o)._rivalGangLeader;
			}

			internal static object AutoGeneratedGetMemberValue_timeoutDurationInDays(object o)
			{
				return ((RivalGangMovingInIssueBehavior.RivalGangMovingInIssueQuest)o)._timeoutDurationInDays;
			}

			internal static object AutoGeneratedGetMemberValue_isFinalStage(object o)
			{
				return ((RivalGangMovingInIssueBehavior.RivalGangMovingInIssueQuest)o)._isFinalStage;
			}

			internal static object AutoGeneratedGetMemberValue_isReadyToBeFinalized(object o)
			{
				return ((RivalGangMovingInIssueBehavior.RivalGangMovingInIssueQuest)o)._isReadyToBeFinalized;
			}

			internal static object AutoGeneratedGetMemberValue_hasBetrayedQuestGiver(object o)
			{
				return ((RivalGangMovingInIssueBehavior.RivalGangMovingInIssueQuest)o)._hasBetrayedQuestGiver;
			}

			internal static object AutoGeneratedGetMemberValue_rivalGangLeaderParty(object o)
			{
				return ((RivalGangMovingInIssueBehavior.RivalGangMovingInIssueQuest)o)._rivalGangLeaderParty;
			}

			internal static object AutoGeneratedGetMemberValue_preparationCompletionTime(object o)
			{
				return ((RivalGangMovingInIssueBehavior.RivalGangMovingInIssueQuest)o)._preparationCompletionTime;
			}

			internal static object AutoGeneratedGetMemberValue_questTimeoutTime(object o)
			{
				return ((RivalGangMovingInIssueBehavior.RivalGangMovingInIssueQuest)o)._questTimeoutTime;
			}

			internal static object AutoGeneratedGetMemberValue_playerTroops(object o)
			{
				return ((RivalGangMovingInIssueBehavior.RivalGangMovingInIssueQuest)o)._playerTroops;
			}

			internal static object AutoGeneratedGetMemberValue_preparationsComplete(object o)
			{
				return ((RivalGangMovingInIssueBehavior.RivalGangMovingInIssueQuest)o)._preparationsComplete;
			}

			internal static object AutoGeneratedGetMemberValue_rewardGold(object o)
			{
				return ((RivalGangMovingInIssueBehavior.RivalGangMovingInIssueQuest)o)._rewardGold;
			}

			internal static object AutoGeneratedGetMemberValue_issueDifficulty(object o)
			{
				return ((RivalGangMovingInIssueBehavior.RivalGangMovingInIssueQuest)o)._issueDifficulty;
			}

			private const int QuestGiverRelationBonusOnSuccess = 5;

			private const int RivalGangLeaderRelationPenaltyOnSuccess = -5;

			private const int QuestGiverNotablePowerBonusOnSuccess = 10;

			private const int QuestGiverRelationPenaltyOnFail = -15;

			private const int NotablePowerPenaltyOnFail = -10;

			private const int TownSecurityPenaltyOnFail = -10;

			private const int RivalGangLeaderRelationBonusOnBetrayal = 5;

			private const int PlayerAttackedQuestGiverHonorPenalty = -150;

			private const int PlayerAttackedQuestGiverPowerPenalty = -10;

			private const int PlayerAttackedQuestGiverRelationPenalty = -8;

			private const int PlayerAttackedQuestGiverSecurityPenalty = -10;

			private const int NumberofRegularEnemyTroops = 15;

			private const int NumberOfRegularAllyTroops = 20;

			private const int MaxNumberOfPlayerOwnedTroops = 5;

			[SaveableField(10)]
			internal readonly Hero _rivalGangLeader;

			[SaveableField(20)]
			private MobileParty _rivalGangLeaderParty;

			[SaveableField(30)]
			private readonly CampaignTime _preparationCompletionTime;

			[SaveableField(40)]
			private readonly CampaignTime _questTimeoutTime;

			[SaveableField(60)]
			internal readonly float _timeoutDurationInDays;

			[SaveableField(70)]
			internal bool _isFinalStage;

			[SaveableField(80)]
			internal bool _isReadyToBeFinalized;

			[SaveableField(90)]
			internal bool _hasBetrayedQuestGiver;

			[SaveableField(100)]
			private List<TroopRosterElement> _playerTroops = new List<TroopRosterElement>();

			[SaveableField(110)]
			private bool _preparationsComplete;

			[SaveableField(120)]
			private int _rewardGold;

			[SaveableField(130)]
			private float _issueDifficulty;

			private Settlement _questSettlement;

			private JournalLog _onQuestStartedLog;

			private JournalLog _onQuestSucceededLog;
		}
	}
}
