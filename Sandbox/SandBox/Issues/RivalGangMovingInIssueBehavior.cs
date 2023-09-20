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
	// Token: 0x0200007E RID: 126
	public class RivalGangMovingInIssueBehavior : CampaignBehaviorBase
	{
		// Token: 0x1700006A RID: 106
		// (get) Token: 0x0600055C RID: 1372 RVA: 0x0002662C File Offset: 0x0002482C
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

		// Token: 0x0600055D RID: 1373 RVA: 0x000266C4 File Offset: 0x000248C4
		private void OnCheckForIssue(Hero hero)
		{
			if (this.ConditionsHold(hero))
			{
				Campaign.Current.IssueManager.AddPotentialIssueData(hero, new PotentialIssueData(new PotentialIssueData.StartIssueDelegate(this.OnStartIssue), typeof(RivalGangMovingInIssueBehavior.RivalGangMovingInIssue), 1, null));
				return;
			}
			Campaign.Current.IssueManager.AddPotentialIssueData(hero, new PotentialIssueData(typeof(RivalGangMovingInIssueBehavior.RivalGangMovingInIssue), 1));
		}

		// Token: 0x0600055E RID: 1374 RVA: 0x00026728 File Offset: 0x00024928
		private IssueBase OnStartIssue(in PotentialIssueData pid, Hero issueOwner)
		{
			Hero rivalGangLeader = this.GetRivalGangLeader(issueOwner);
			return new RivalGangMovingInIssueBehavior.RivalGangMovingInIssue(issueOwner, rivalGangLeader);
		}

		// Token: 0x0600055F RID: 1375 RVA: 0x00026744 File Offset: 0x00024944
		private static void rival_gang_wait_duration_is_over_menu_on_init(MenuCallbackArgs args)
		{
			Campaign.Current.TimeControlMode = 0;
			TextObject textObject = new TextObject("{=9Kr9pjGs}{QUEST_GIVER.LINK} has prepared {?QUEST_GIVER.GENDER}her{?}his{\\?} men and is waiting for you.", null);
			StringHelpers.SetCharacterProperties("QUEST_GIVER", RivalGangMovingInIssueBehavior.Instance.QuestGiver.CharacterObject, null, false);
			MBTextManager.SetTextVariable("MENU_TEXT", textObject, false);
		}

		// Token: 0x06000560 RID: 1376 RVA: 0x00026790 File Offset: 0x00024990
		private bool ConditionsHold(Hero issueGiver)
		{
			return issueGiver.IsGangLeader && issueGiver.CurrentSettlement != null && issueGiver.CurrentSettlement.IsTown && issueGiver.CurrentSettlement.Town.Security <= 60f && this.GetRivalGangLeader(issueGiver) != null;
		}

		// Token: 0x06000561 RID: 1377 RVA: 0x000267E0 File Offset: 0x000249E0
		private void rival_gang_quest_wait_duration_is_over_yes_consequence(MenuCallbackArgs args)
		{
			CampaignMapConversation.OpenConversation(new ConversationCharacterData(CharacterObject.PlayerCharacter, null, true, true, false, false, false, false), new ConversationCharacterData(RivalGangMovingInIssueBehavior.Instance.QuestGiver.CharacterObject, null, true, true, false, false, false, false));
		}

		// Token: 0x06000562 RID: 1378 RVA: 0x00026820 File Offset: 0x00024A20
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

		// Token: 0x06000563 RID: 1379 RVA: 0x0002688C File Offset: 0x00024A8C
		private bool rival_gang_quest_wait_duration_is_over_yes_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = 17;
			return true;
		}

		// Token: 0x06000564 RID: 1380 RVA: 0x00026897 File Offset: 0x00024A97
		private bool rival_gang_quest_wait_duration_is_over_no_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = 16;
			return true;
		}

		// Token: 0x06000565 RID: 1381 RVA: 0x000268A2 File Offset: 0x00024AA2
		public override void RegisterEvents()
		{
			CampaignEvents.OnCheckForIssueEvent.AddNonSerializedListener(this, new Action<Hero>(this.OnCheckForIssue));
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
		}

		// Token: 0x06000566 RID: 1382 RVA: 0x000268D4 File Offset: 0x00024AD4
		private void OnSessionLaunched(CampaignGameStarter gameStarter)
		{
			gameStarter.AddGameMenu("rival_gang_quest_before_fight", "", new OnInitDelegate(RivalGangMovingInIssueBehavior.rival_gang_quest_before_fight_init), 3, 0, null);
			gameStarter.AddGameMenu("rival_gang_quest_after_fight", "", new OnInitDelegate(RivalGangMovingInIssueBehavior.rival_gang_quest_after_fight_init), 3, 0, null);
			gameStarter.AddGameMenu("rival_gang_quest_wait_duration_is_over", "{MENU_TEXT}", new OnInitDelegate(RivalGangMovingInIssueBehavior.rival_gang_wait_duration_is_over_menu_on_init), 0, 0, null);
			gameStarter.AddGameMenuOption("rival_gang_quest_wait_duration_is_over", "rival_gang_quest_wait_duration_is_over_yes", "{=aka03VdU}Meet {?QUEST_GIVER.GENDER}her{?}him{\\?} now", new GameMenuOption.OnConditionDelegate(this.rival_gang_quest_wait_duration_is_over_yes_condition), new GameMenuOption.OnConsequenceDelegate(this.rival_gang_quest_wait_duration_is_over_yes_consequence), false, -1, false, null);
			gameStarter.AddGameMenuOption("rival_gang_quest_wait_duration_is_over", "rival_gang_quest_wait_duration_is_over_no", "{=NIzQb6nT}Leave and meet {?QUEST_GIVER.GENDER}her{?}him{\\?} later", new GameMenuOption.OnConditionDelegate(this.rival_gang_quest_wait_duration_is_over_no_condition), new GameMenuOption.OnConsequenceDelegate(this.rival_gang_quest_wait_duration_is_over_no_consequence), true, -1, false, null);
		}

		// Token: 0x06000567 RID: 1383 RVA: 0x000269A0 File Offset: 0x00024BA0
		private void rival_gang_quest_wait_duration_is_over_no_consequence(MenuCallbackArgs args)
		{
			Campaign.Current.CurrentMenuContext.SwitchToMenu("town_wait_menus");
		}

		// Token: 0x06000568 RID: 1384 RVA: 0x000269B6 File Offset: 0x00024BB6
		private static void rival_gang_quest_before_fight_init(MenuCallbackArgs args)
		{
			if (RivalGangMovingInIssueBehavior.Instance != null && RivalGangMovingInIssueBehavior.Instance._isFinalStage)
			{
				RivalGangMovingInIssueBehavior.Instance.StartAlleyBattle(RivalGangMovingInIssueBehavior.Instance._rivalGangLeader);
			}
		}

		// Token: 0x06000569 RID: 1385 RVA: 0x000269E0 File Offset: 0x00024BE0
		private static void rival_gang_quest_after_fight_init(MenuCallbackArgs args)
		{
			if (RivalGangMovingInIssueBehavior.Instance != null && RivalGangMovingInIssueBehavior.Instance._isReadyToBeFinalized)
			{
				bool flag = PlayerEncounter.Battle.WinningSide == PlayerEncounter.Battle.PlayerSide;
				PlayerEncounter.Current.FinalizeBattle();
				RivalGangMovingInIssueBehavior.Instance.HandlePlayerEncounterResult(flag);
			}
		}

		// Token: 0x0600056A RID: 1386 RVA: 0x00026A2C File Offset: 0x00024C2C
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x0600056B RID: 1387 RVA: 0x00026A30 File Offset: 0x00024C30
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

		// Token: 0x040002A6 RID: 678
		private const IssueBase.IssueFrequency RivalGangLeaderIssueFrequency = 1;

		// Token: 0x040002A7 RID: 679
		private RivalGangMovingInIssueBehavior.RivalGangMovingInIssueQuest _cachedQuest;

		// Token: 0x02000158 RID: 344
		public class RivalGangMovingInIssueTypeDefiner : SaveableTypeDefiner
		{
			// Token: 0x06000F72 RID: 3954 RVA: 0x0006C43A File Offset: 0x0006A63A
			public RivalGangMovingInIssueTypeDefiner()
				: base(310000)
			{
			}

			// Token: 0x06000F73 RID: 3955 RVA: 0x0006C447 File Offset: 0x0006A647
			protected override void DefineClassTypes()
			{
				base.AddClassDefinition(typeof(RivalGangMovingInIssueBehavior.RivalGangMovingInIssue), 1, null);
				base.AddClassDefinition(typeof(RivalGangMovingInIssueBehavior.RivalGangMovingInIssueQuest), 2, null);
			}
		}

		// Token: 0x02000159 RID: 345
		public class RivalGangMovingInIssue : IssueBase
		{
			// Token: 0x17000162 RID: 354
			// (get) Token: 0x06000F74 RID: 3956 RVA: 0x0006C46D File Offset: 0x0006A66D
			public override IssueBase.AlternativeSolutionScaleFlag AlternativeSolutionScaleFlags
			{
				get
				{
					return 12;
				}
			}

			// Token: 0x17000163 RID: 355
			// (get) Token: 0x06000F75 RID: 3957 RVA: 0x0006C471 File Offset: 0x0006A671
			// (set) Token: 0x06000F76 RID: 3958 RVA: 0x0006C479 File Offset: 0x0006A679
			[SaveableProperty(207)]
			public Hero RivalGangLeader { get; private set; }

			// Token: 0x17000164 RID: 356
			// (get) Token: 0x06000F77 RID: 3959 RVA: 0x0006C482 File Offset: 0x0006A682
			public override int AlternativeSolutionBaseNeededMenCount
			{
				get
				{
					return 4 + MathF.Ceiling(6f * base.IssueDifficultyMultiplier);
				}
			}

			// Token: 0x17000165 RID: 357
			// (get) Token: 0x06000F78 RID: 3960 RVA: 0x0006C497 File Offset: 0x0006A697
			protected override int AlternativeSolutionBaseDurationInDaysInternal
			{
				get
				{
					return 3 + MathF.Ceiling(5f * base.IssueDifficultyMultiplier);
				}
			}

			// Token: 0x17000166 RID: 358
			// (get) Token: 0x06000F79 RID: 3961 RVA: 0x0006C4AC File Offset: 0x0006A6AC
			protected override int RewardGold
			{
				get
				{
					return (int)(600f + 1700f * base.IssueDifficultyMultiplier);
				}
			}

			// Token: 0x17000167 RID: 359
			// (get) Token: 0x06000F7A RID: 3962 RVA: 0x0006C4C1 File Offset: 0x0006A6C1
			protected override int CompanionSkillRewardXP
			{
				get
				{
					return (int)(750f + 1000f * base.IssueDifficultyMultiplier);
				}
			}

			// Token: 0x17000168 RID: 360
			// (get) Token: 0x06000F7B RID: 3963 RVA: 0x0006C4D8 File Offset: 0x0006A6D8
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

			// Token: 0x17000169 RID: 361
			// (get) Token: 0x06000F7C RID: 3964 RVA: 0x0006C52C File Offset: 0x0006A72C
			public override TextObject IssueAcceptByPlayer
			{
				get
				{
					return new TextObject("{=kc6vCycY}What exactly do you want me to do?", null);
				}
			}

			// Token: 0x1700016A RID: 362
			// (get) Token: 0x06000F7D RID: 3965 RVA: 0x0006C539 File Offset: 0x0006A739
			public override TextObject IssueQuestSolutionExplanationByIssueGiver
			{
				get
				{
					TextObject textObject = new TextObject("{=tyyAfWRR}We already had a small scuffle with them recently. They'll be waiting for us to come down hard. Instead, we'll hold off for {NUMBER} days. Let them think that we're backing off… Then, after {NUMBER} days, your men and mine will hit them in the middle of the night when they least expect it. I'll send you a messenger when the time comes and we'll strike them down together.", null);
					textObject.SetTextVariable("NUMBER", 4);
					return textObject;
				}
			}

			// Token: 0x1700016B RID: 363
			// (get) Token: 0x06000F7E RID: 3966 RVA: 0x0006C553 File Offset: 0x0006A753
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

			// Token: 0x1700016C RID: 364
			// (get) Token: 0x06000F7F RID: 3967 RVA: 0x0006C584 File Offset: 0x0006A784
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

			// Token: 0x1700016D RID: 365
			// (get) Token: 0x06000F80 RID: 3968 RVA: 0x0006C615 File Offset: 0x0006A815
			public override TextObject IssueQuestSolutionAcceptByPlayer
			{
				get
				{
					return new TextObject("{=LdCte9H0}I'll fight the other gang with you myself.", null);
				}
			}

			// Token: 0x1700016E RID: 366
			// (get) Token: 0x06000F81 RID: 3969 RVA: 0x0006C622 File Offset: 0x0006A822
			public override TextObject IssueAlternativeSolutionAcceptByPlayer
			{
				get
				{
					return new TextObject("{=AdbiUqtT}I'm busy, but I will leave a companion and some men.", null);
				}
			}

			// Token: 0x1700016F RID: 367
			// (get) Token: 0x06000F82 RID: 3970 RVA: 0x0006C62F File Offset: 0x0006A82F
			public override TextObject IssueAlternativeSolutionResponseByIssueGiver
			{
				get
				{
					return new TextObject("{=0enbhess}Thank you. I'm sure your guys are worth their salt..", null);
				}
			}

			// Token: 0x17000170 RID: 368
			// (get) Token: 0x06000F83 RID: 3971 RVA: 0x0006C63C File Offset: 0x0006A83C
			public override TextObject IssueDiscussAlternativeSolution
			{
				get
				{
					return new TextObject("{=QR0V8Ae5}Our lads are well hidden nearby, waiting for the signal to go get those bastards. I won't forget this little favor you're doing me.", null);
				}
			}

			// Token: 0x17000171 RID: 369
			// (get) Token: 0x06000F84 RID: 3972 RVA: 0x0006C649 File Offset: 0x0006A849
			public override bool IsThereAlternativeSolution
			{
				get
				{
					return true;
				}
			}

			// Token: 0x17000172 RID: 370
			// (get) Token: 0x06000F85 RID: 3973 RVA: 0x0006C64C File Offset: 0x0006A84C
			public override bool IsThereLordSolution
			{
				get
				{
					return false;
				}
			}

			// Token: 0x17000173 RID: 371
			// (get) Token: 0x06000F86 RID: 3974 RVA: 0x0006C64F File Offset: 0x0006A84F
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

			// Token: 0x17000174 RID: 372
			// (get) Token: 0x06000F87 RID: 3975 RVA: 0x0006C68E File Offset: 0x0006A88E
			public override TextObject Description
			{
				get
				{
					return new TextObject("{=H4EVfKAh}Gang leader needs help to beat the rival gang.", null);
				}
			}

			// Token: 0x17000175 RID: 373
			// (get) Token: 0x06000F88 RID: 3976 RVA: 0x0006C69C File Offset: 0x0006A89C
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

			// Token: 0x17000176 RID: 374
			// (get) Token: 0x06000F89 RID: 3977 RVA: 0x0006C6E6 File Offset: 0x0006A8E6
			protected override bool IssueQuestCanBeDuplicated
			{
				get
				{
					return false;
				}
			}

			// Token: 0x06000F8A RID: 3978 RVA: 0x0006C6E9 File Offset: 0x0006A8E9
			public RivalGangMovingInIssue(Hero issueOwner, Hero rivalGangLeader)
				: base(issueOwner, CampaignTime.DaysFromNow(15f))
			{
				this.RivalGangLeader = rivalGangLeader;
			}

			// Token: 0x06000F8B RID: 3979 RVA: 0x0006C703 File Offset: 0x0006A903
			public override void OnHeroCanHaveQuestOrIssueInfoIsRequested(Hero hero, ref bool result)
			{
				if (hero == this.RivalGangLeader)
				{
					result = false;
				}
			}

			// Token: 0x06000F8C RID: 3980 RVA: 0x0006C711 File Offset: 0x0006A911
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

			// Token: 0x06000F8D RID: 3981 RVA: 0x0006C734 File Offset: 0x0006A934
			protected override void AlternativeSolutionEndWithSuccessConsequence()
			{
				this.RelationshipChangeWithIssueOwner = 5;
				ChangeRelationAction.ApplyPlayerRelation(this.RivalGangLeader, -5, true, true);
				base.IssueOwner.AddPower(10f);
			}

			// Token: 0x06000F8E RID: 3982 RVA: 0x0006C75C File Offset: 0x0006A95C
			protected override void AlternativeSolutionEndWithFailureConsequence()
			{
				this.RelationshipChangeWithIssueOwner = -5;
				base.IssueSettlement.Town.Security -= 10f;
				base.IssueOwner.AddPower(-10f);
			}

			// Token: 0x06000F8F RID: 3983 RVA: 0x0006C794 File Offset: 0x0006A994
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

			// Token: 0x06000F90 RID: 3984 RVA: 0x0006C825 File Offset: 0x0006AA25
			public override bool AlternativeSolutionCondition(out TextObject explanation)
			{
				explanation = TextObject.Empty;
				return QuestHelper.CheckRosterForAlternativeSolution(MobileParty.MainParty.MemberRoster, base.GetTotalAlternativeSolutionNeededMenCount(), ref explanation, 2, false);
			}

			// Token: 0x06000F91 RID: 3985 RVA: 0x0006C846 File Offset: 0x0006AA46
			public override bool DoTroopsSatisfyAlternativeSolution(TroopRoster troopRoster, out TextObject explanation)
			{
				explanation = TextObject.Empty;
				return QuestHelper.CheckRosterForAlternativeSolution(troopRoster, base.GetTotalAlternativeSolutionNeededMenCount(), ref explanation, 2, false);
			}

			// Token: 0x06000F92 RID: 3986 RVA: 0x0006C85E File Offset: 0x0006AA5E
			public override bool IsTroopTypeNeededByAlternativeSolution(CharacterObject character)
			{
				return character.Tier >= 2;
			}

			// Token: 0x06000F93 RID: 3987 RVA: 0x0006C86C File Offset: 0x0006AA6C
			protected override void OnGameLoad()
			{
			}

			// Token: 0x06000F94 RID: 3988 RVA: 0x0006C86E File Offset: 0x0006AA6E
			protected override QuestBase GenerateIssueQuest(string questId)
			{
				return new RivalGangMovingInIssueBehavior.RivalGangMovingInIssueQuest(questId, base.IssueOwner, this.RivalGangLeader, 8, this.RewardGold, base.IssueDifficultyMultiplier);
			}

			// Token: 0x06000F95 RID: 3989 RVA: 0x0006C88F File Offset: 0x0006AA8F
			public override IssueBase.IssueFrequency GetFrequency()
			{
				return 1;
			}

			// Token: 0x06000F96 RID: 3990 RVA: 0x0006C894 File Offset: 0x0006AA94
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

			// Token: 0x06000F97 RID: 3991 RVA: 0x0006C918 File Offset: 0x0006AB18
			public override bool IssueStayAliveConditions()
			{
				return this.RivalGangLeader.IsAlive && base.IssueOwner.CurrentSettlement.OwnerClan != Clan.PlayerClan && base.IssueOwner.CurrentSettlement.Town.Security <= 80f;
			}

			// Token: 0x06000F98 RID: 3992 RVA: 0x0006C96A File Offset: 0x0006AB6A
			protected override void CompleteIssueWithTimedOutConsequences()
			{
			}

			// Token: 0x06000F99 RID: 3993 RVA: 0x0006C96C File Offset: 0x0006AB6C
			internal static void AutoGeneratedStaticCollectObjectsRivalGangMovingInIssue(object o, List<object> collectedObjects)
			{
				((RivalGangMovingInIssueBehavior.RivalGangMovingInIssue)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			// Token: 0x06000F9A RID: 3994 RVA: 0x0006C97A File Offset: 0x0006AB7A
			protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				base.AutoGeneratedInstanceCollectObjects(collectedObjects);
				collectedObjects.Add(this.RivalGangLeader);
			}

			// Token: 0x06000F9B RID: 3995 RVA: 0x0006C98F File Offset: 0x0006AB8F
			internal static object AutoGeneratedGetMemberValueRivalGangLeader(object o)
			{
				return ((RivalGangMovingInIssueBehavior.RivalGangMovingInIssue)o).RivalGangLeader;
			}

			// Token: 0x04000653 RID: 1619
			private const int AlternativeSolutionRelationBonus = 5;

			// Token: 0x04000654 RID: 1620
			private const int AlternativeSolutionFailRelationPenalty = -5;

			// Token: 0x04000655 RID: 1621
			private const int AlternativeSolutionNotablePowerBonus = 10;

			// Token: 0x04000656 RID: 1622
			private const int AlternativeSolutionFailNotablePowerPenalty = -10;

			// Token: 0x04000657 RID: 1623
			private const int AlternativeSolutionRivalGangLeaderRelationPenalty = -5;

			// Token: 0x04000658 RID: 1624
			private const int AlternativeSolutionMinimumTroopTier = 2;

			// Token: 0x04000659 RID: 1625
			private const int IssueDuration = 15;

			// Token: 0x0400065A RID: 1626
			private const int MinimumRequiredMenCount = 5;

			// Token: 0x0400065B RID: 1627
			private const int IssueQuestDuration = 8;

			// Token: 0x0400065C RID: 1628
			private const int MeleeSkillValueThreshold = 150;

			// Token: 0x0400065D RID: 1629
			private const int RoguerySkillValueThreshold = 120;
		}

		// Token: 0x0200015A RID: 346
		public class RivalGangMovingInIssueQuest : QuestBase
		{
			// Token: 0x17000177 RID: 375
			// (get) Token: 0x06000F9C RID: 3996 RVA: 0x0006C99C File Offset: 0x0006AB9C
			private int PreparationDurationInDays
			{
				get
				{
					return (int)(this._timeoutDurationInDays / 2f);
				}
			}

			// Token: 0x17000178 RID: 376
			// (get) Token: 0x06000F9D RID: 3997 RVA: 0x0006C9AC File Offset: 0x0006ABAC
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

			// Token: 0x17000179 RID: 377
			// (get) Token: 0x06000F9E RID: 3998 RVA: 0x0006CA08 File Offset: 0x0006AC08
			private TextObject _onQuestFailedWithRejectionLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=aXMg9M7t}You decided to stay out of the fight. {?QUEST_GIVER.GENDER}She{?}He{\\?} will certainly lose to the rival gang without your help.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					return textObject;
				}
			}

			// Token: 0x1700017A RID: 378
			// (get) Token: 0x06000F9F RID: 3999 RVA: 0x0006CA3C File Offset: 0x0006AC3C
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

			// Token: 0x1700017B RID: 379
			// (get) Token: 0x06000FA0 RID: 4000 RVA: 0x0006CA88 File Offset: 0x0006AC88
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

			// Token: 0x1700017C RID: 380
			// (get) Token: 0x06000FA1 RID: 4001 RVA: 0x0006CAD4 File Offset: 0x0006ACD4
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

			// Token: 0x1700017D RID: 381
			// (get) Token: 0x06000FA2 RID: 4002 RVA: 0x0006CB20 File Offset: 0x0006AD20
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

			// Token: 0x1700017E RID: 382
			// (get) Token: 0x06000FA3 RID: 4003 RVA: 0x0006CB6C File Offset: 0x0006AD6C
			private TextObject _onQuestCancelledDueToWarLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=vaUlAZba}Your clan is now at war with {QUEST_GIVER.LINK}. Your agreement with {QUEST_GIVER.LINK} was canceled.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					return textObject;
				}
			}

			// Token: 0x1700017F RID: 383
			// (get) Token: 0x06000FA4 RID: 4004 RVA: 0x0006CBA0 File Offset: 0x0006ADA0
			private TextObject _playerDeclaredWarQuestLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=bqeWVVEE}Your actions have started a war with {QUEST_GIVER.LINK}'s faction. {?QUEST_GIVER.GENDER}She{?}He{\\?} cancels your agreement and the quest is a failure.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					return textObject;
				}
			}

			// Token: 0x17000180 RID: 384
			// (get) Token: 0x06000FA5 RID: 4005 RVA: 0x0006CBD4 File Offset: 0x0006ADD4
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

			// Token: 0x17000181 RID: 385
			// (get) Token: 0x06000FA6 RID: 4006 RVA: 0x0006CC20 File Offset: 0x0006AE20
			private TextObject _playerStartedAlleyFightWithRivalGangLeader
			{
				get
				{
					TextObject textObject = new TextObject("{=OeKgpuAv}After your attack on the rival gang's alley, {QUEST_GIVER.LINK} decided to change {?QUEST_GIVER.GENDER}her{?}his{\\?} plans, and doesn't need your assistance anymore. Quest is canceled.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					return textObject;
				}
			}

			// Token: 0x17000182 RID: 386
			// (get) Token: 0x06000FA7 RID: 4007 RVA: 0x0006CC54 File Offset: 0x0006AE54
			private TextObject _playerStartedAlleyFightWithQuestgiver
			{
				get
				{
					TextObject textObject = new TextObject("{=VPGkIqlh}Your attack on {QUEST_GIVER.LINK}'s gang has angered {?QUEST_GIVER.GENDER}her{?}him{\\?} and {?QUEST_GIVER.GENDER}she{?}he{\\?} broke off the agreement that you had.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					return textObject;
				}
			}

			// Token: 0x17000183 RID: 387
			// (get) Token: 0x06000FA8 RID: 4008 RVA: 0x0006CC88 File Offset: 0x0006AE88
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

			// Token: 0x06000FA9 RID: 4009 RVA: 0x0006CCCC File Offset: 0x0006AECC
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

			// Token: 0x17000184 RID: 388
			// (get) Token: 0x06000FAA RID: 4010 RVA: 0x0006CD4B File Offset: 0x0006AF4B
			public override TextObject Title
			{
				get
				{
					TextObject textObject = new TextObject("{=WVorNMNc}Rival Gang Moving In At {SETTLEMENT}", null);
					textObject.SetTextVariable("SETTLEMENT", this._questSettlement.Name);
					return textObject;
				}
			}

			// Token: 0x17000185 RID: 389
			// (get) Token: 0x06000FAB RID: 4011 RVA: 0x0006CD6F File Offset: 0x0006AF6F
			public override bool IsRemainingTimeHidden
			{
				get
				{
					return false;
				}
			}

			// Token: 0x06000FAC RID: 4012 RVA: 0x0006CD74 File Offset: 0x0006AF74
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

			// Token: 0x06000FAD RID: 4013 RVA: 0x0006CDCA File Offset: 0x0006AFCA
			private void InitializeQuestSettlement()
			{
				this._questSettlement = base.QuestGiver.CurrentSettlement;
			}

			// Token: 0x06000FAE RID: 4014 RVA: 0x0006CDE0 File Offset: 0x0006AFE0
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

			// Token: 0x06000FAF RID: 4015 RVA: 0x0006CEA8 File Offset: 0x0006B0A8
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

			// Token: 0x06000FB0 RID: 4016 RVA: 0x0006CF78 File Offset: 0x0006B178
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

			// Token: 0x06000FB1 RID: 4017 RVA: 0x0006D0C7 File Offset: 0x0006B2C7
			public override void OnHeroCanDieInfoIsRequested(Hero hero, KillCharacterAction.KillCharacterActionDetail causeOfDeath, ref bool result)
			{
				if (hero == base.QuestGiver || hero == this._rivalGangLeader)
				{
					result = false;
				}
			}

			// Token: 0x06000FB2 RID: 4018 RVA: 0x0006D0DE File Offset: 0x0006B2DE
			private void rival_gang_start_fight_on_consequence()
			{
				this._isFinalStage = true;
				if (Mission.Current != null)
				{
					Mission.Current.EndMission();
				}
				Campaign.Current.GameMenuManager.SetNextMenu("rival_gang_quest_before_fight");
			}

			// Token: 0x06000FB3 RID: 4019 RVA: 0x0006D10C File Offset: 0x0006B30C
			private void rival_gang_need_more_time_on_consequence()
			{
				if (Campaign.Current.CurrentMenuContext.GameMenu.StringId == "rival_gang_quest_wait_duration_is_over")
				{
					Campaign.Current.GameMenuManager.SetNextMenu("town_wait_menus");
				}
			}

			// Token: 0x06000FB4 RID: 4020 RVA: 0x0006D144 File Offset: 0x0006B344
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

			// Token: 0x06000FB5 RID: 4021 RVA: 0x0006D1A0 File Offset: 0x0006B3A0
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

			// Token: 0x06000FB6 RID: 4022 RVA: 0x0006D29C File Offset: 0x0006B49C
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

			// Token: 0x06000FB7 RID: 4023 RVA: 0x0006D59C File Offset: 0x0006B79C
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

			// Token: 0x06000FB8 RID: 4024 RVA: 0x0006D6F0 File Offset: 0x0006B8F0
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

			// Token: 0x06000FB9 RID: 4025 RVA: 0x0006D79E File Offset: 0x0006B99E
			private void OnSettlementOwnerChanged(Settlement settlement, bool openToClaim, Hero newOwner, Hero oldOwner, Hero capturerHero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail detail)
			{
				if (settlement == base.QuestGiver.CurrentSettlement && newOwner == Hero.MainHero)
				{
					base.AddLog(this.OwnerOfQuestSettlementIsPlayerClanLogText, false);
					base.CompleteQuestWithCancel(null);
				}
			}

			// Token: 0x06000FBA RID: 4026 RVA: 0x0006D7CB File Offset: 0x0006B9CB
			public override void OnHeroCanHaveQuestOrIssueInfoIsRequested(Hero hero, ref bool result)
			{
				if (hero == this._rivalGangLeader)
				{
					result = false;
				}
			}

			// Token: 0x06000FBB RID: 4027 RVA: 0x0006D7D9 File Offset: 0x0006B9D9
			private void OnClanChangedKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail detail, bool showNotification = true)
			{
				if (base.QuestGiver.CurrentSettlement.MapFaction.IsAtWarWith(Hero.MainHero.MapFaction))
				{
					base.CompleteQuestWithCancel(this._onQuestCancelledDueToWarLogText);
				}
			}

			// Token: 0x06000FBC RID: 4028 RVA: 0x0006D808 File Offset: 0x0006BA08
			private void OnWarDeclared(IFaction faction1, IFaction faction2, DeclareWarAction.DeclareWarDetail detail)
			{
				QuestHelper.CheckWarDeclarationAndFailOrCancelTheQuest(this, faction1, faction2, detail, this._playerDeclaredWarQuestLogText, this._onQuestCancelledDueToWarLogText);
			}

			// Token: 0x06000FBD RID: 4029 RVA: 0x0006D81F File Offset: 0x0006BA1F
			private void OnSiegeEventStarted(SiegeEvent siegeEvent)
			{
				if (siegeEvent.BesiegedSettlement == this._questSettlement)
				{
					base.AddLog(this._onQuestCancelledDueToSiegeLogText, false);
					base.CompleteQuestWithCancel(null);
				}
			}

			// Token: 0x06000FBE RID: 4030 RVA: 0x0006D844 File Offset: 0x0006BA44
			private void HourlyTick()
			{
				if (RivalGangMovingInIssueBehavior.Instance != null && RivalGangMovingInIssueBehavior.Instance.IsOngoing && ((float)RivalGangMovingInIssueBehavior.Instance.PreparationDurationInDays - RivalGangMovingInIssueBehavior.Instance._preparationCompletionTime.RemainingDaysFromNow) / (float)RivalGangMovingInIssueBehavior.Instance.PreparationDurationInDays >= 1f && !this._preparationsComplete && CampaignTime.Now.IsNightTime)
				{
					this.OnGuestGiverPreparationsCompleted();
				}
			}

			// Token: 0x06000FBF RID: 4031 RVA: 0x0006D8B4 File Offset: 0x0006BAB4
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

			// Token: 0x06000FC0 RID: 4032 RVA: 0x0006D920 File Offset: 0x0006BB20
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

			// Token: 0x06000FC1 RID: 4033 RVA: 0x0006D991 File Offset: 0x0006BB91
			private void OnPlayerAttackedRivalGangAlley()
			{
				base.AddLog(this._playerStartedAlleyFightWithRivalGangLeader, false);
				base.CompleteQuestWithCancel(null);
			}

			// Token: 0x06000FC2 RID: 4034 RVA: 0x0006D9A8 File Offset: 0x0006BBA8
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

			// Token: 0x06000FC3 RID: 4035 RVA: 0x0006DA28 File Offset: 0x0006BC28
			protected override void OnTimedOut()
			{
				this.OnQuestFailedWithRejectionOrTimeout();
			}

			// Token: 0x06000FC4 RID: 4036 RVA: 0x0006DA30 File Offset: 0x0006BC30
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

			// Token: 0x06000FC5 RID: 4037 RVA: 0x0006DAD8 File Offset: 0x0006BCD8
			private void OnQuestAccepted()
			{
				base.StartQuest();
				this._onQuestStartedLog = base.AddLog(this._onQuestStartedLogText, false);
				Campaign.Current.ConversationManager.AddDialogFlow(this.GetRivalGangLeaderDialogFlow(), this);
				Campaign.Current.ConversationManager.AddDialogFlow(this.GetQuestGiverPreparationCompletedDialogFlow(), this);
			}

			// Token: 0x06000FC6 RID: 4038 RVA: 0x0006DB2C File Offset: 0x0006BD2C
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

			// Token: 0x06000FC7 RID: 4039 RVA: 0x0006DBE5 File Offset: 0x0006BDE5
			private void OnQuestFailedWithRejectionOrTimeout()
			{
				base.AddLog(this._onQuestFailedWithRejectionLogText, false);
				TraitLevelingHelper.OnIssueFailed(base.QuestGiver, new Tuple<TraitObject, int>[]
				{
					new Tuple<TraitObject, int>(DefaultTraits.Honor, -20)
				});
				this.ApplyQuestFailConsequences();
			}

			// Token: 0x06000FC8 RID: 4040 RVA: 0x0006DC1C File Offset: 0x0006BE1C
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

			// Token: 0x06000FC9 RID: 4041 RVA: 0x0006DC94 File Offset: 0x0006BE94
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

			// Token: 0x06000FCA RID: 4042 RVA: 0x0006DD13 File Offset: 0x0006BF13
			private void OnQuestFailedWithDefeat()
			{
				base.AddLog(this._onQuestFailedWithDefeatLogText, false);
				this.ApplyQuestFailConsequences();
				base.CompleteQuestWithFail(null);
			}

			// Token: 0x06000FCB RID: 4043 RVA: 0x0006DD30 File Offset: 0x0006BF30
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

			// Token: 0x06000FCC RID: 4044 RVA: 0x0006DD94 File Offset: 0x0006BF94
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

			// Token: 0x06000FCD RID: 4045 RVA: 0x0006DDF8 File Offset: 0x0006BFF8
			internal static void AutoGeneratedStaticCollectObjectsRivalGangMovingInIssueQuest(object o, List<object> collectedObjects)
			{
				((RivalGangMovingInIssueBehavior.RivalGangMovingInIssueQuest)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			// Token: 0x06000FCE RID: 4046 RVA: 0x0006DE08 File Offset: 0x0006C008
			protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				base.AutoGeneratedInstanceCollectObjects(collectedObjects);
				collectedObjects.Add(this._rivalGangLeader);
				collectedObjects.Add(this._rivalGangLeaderParty);
				CampaignTime.AutoGeneratedStaticCollectObjectsCampaignTime(this._preparationCompletionTime, collectedObjects);
				CampaignTime.AutoGeneratedStaticCollectObjectsCampaignTime(this._questTimeoutTime, collectedObjects);
				collectedObjects.Add(this._playerTroops);
			}

			// Token: 0x06000FCF RID: 4047 RVA: 0x0006DE62 File Offset: 0x0006C062
			internal static object AutoGeneratedGetMemberValue_rivalGangLeader(object o)
			{
				return ((RivalGangMovingInIssueBehavior.RivalGangMovingInIssueQuest)o)._rivalGangLeader;
			}

			// Token: 0x06000FD0 RID: 4048 RVA: 0x0006DE6F File Offset: 0x0006C06F
			internal static object AutoGeneratedGetMemberValue_timeoutDurationInDays(object o)
			{
				return ((RivalGangMovingInIssueBehavior.RivalGangMovingInIssueQuest)o)._timeoutDurationInDays;
			}

			// Token: 0x06000FD1 RID: 4049 RVA: 0x0006DE81 File Offset: 0x0006C081
			internal static object AutoGeneratedGetMemberValue_isFinalStage(object o)
			{
				return ((RivalGangMovingInIssueBehavior.RivalGangMovingInIssueQuest)o)._isFinalStage;
			}

			// Token: 0x06000FD2 RID: 4050 RVA: 0x0006DE93 File Offset: 0x0006C093
			internal static object AutoGeneratedGetMemberValue_isReadyToBeFinalized(object o)
			{
				return ((RivalGangMovingInIssueBehavior.RivalGangMovingInIssueQuest)o)._isReadyToBeFinalized;
			}

			// Token: 0x06000FD3 RID: 4051 RVA: 0x0006DEA5 File Offset: 0x0006C0A5
			internal static object AutoGeneratedGetMemberValue_hasBetrayedQuestGiver(object o)
			{
				return ((RivalGangMovingInIssueBehavior.RivalGangMovingInIssueQuest)o)._hasBetrayedQuestGiver;
			}

			// Token: 0x06000FD4 RID: 4052 RVA: 0x0006DEB7 File Offset: 0x0006C0B7
			internal static object AutoGeneratedGetMemberValue_rivalGangLeaderParty(object o)
			{
				return ((RivalGangMovingInIssueBehavior.RivalGangMovingInIssueQuest)o)._rivalGangLeaderParty;
			}

			// Token: 0x06000FD5 RID: 4053 RVA: 0x0006DEC4 File Offset: 0x0006C0C4
			internal static object AutoGeneratedGetMemberValue_preparationCompletionTime(object o)
			{
				return ((RivalGangMovingInIssueBehavior.RivalGangMovingInIssueQuest)o)._preparationCompletionTime;
			}

			// Token: 0x06000FD6 RID: 4054 RVA: 0x0006DED6 File Offset: 0x0006C0D6
			internal static object AutoGeneratedGetMemberValue_questTimeoutTime(object o)
			{
				return ((RivalGangMovingInIssueBehavior.RivalGangMovingInIssueQuest)o)._questTimeoutTime;
			}

			// Token: 0x06000FD7 RID: 4055 RVA: 0x0006DEE8 File Offset: 0x0006C0E8
			internal static object AutoGeneratedGetMemberValue_playerTroops(object o)
			{
				return ((RivalGangMovingInIssueBehavior.RivalGangMovingInIssueQuest)o)._playerTroops;
			}

			// Token: 0x06000FD8 RID: 4056 RVA: 0x0006DEF5 File Offset: 0x0006C0F5
			internal static object AutoGeneratedGetMemberValue_preparationsComplete(object o)
			{
				return ((RivalGangMovingInIssueBehavior.RivalGangMovingInIssueQuest)o)._preparationsComplete;
			}

			// Token: 0x06000FD9 RID: 4057 RVA: 0x0006DF07 File Offset: 0x0006C107
			internal static object AutoGeneratedGetMemberValue_rewardGold(object o)
			{
				return ((RivalGangMovingInIssueBehavior.RivalGangMovingInIssueQuest)o)._rewardGold;
			}

			// Token: 0x06000FDA RID: 4058 RVA: 0x0006DF19 File Offset: 0x0006C119
			internal static object AutoGeneratedGetMemberValue_issueDifficulty(object o)
			{
				return ((RivalGangMovingInIssueBehavior.RivalGangMovingInIssueQuest)o)._issueDifficulty;
			}

			// Token: 0x0400065F RID: 1631
			private const int QuestGiverRelationBonusOnSuccess = 5;

			// Token: 0x04000660 RID: 1632
			private const int RivalGangLeaderRelationPenaltyOnSuccess = -5;

			// Token: 0x04000661 RID: 1633
			private const int QuestGiverNotablePowerBonusOnSuccess = 10;

			// Token: 0x04000662 RID: 1634
			private const int QuestGiverRelationPenaltyOnFail = -15;

			// Token: 0x04000663 RID: 1635
			private const int NotablePowerPenaltyOnFail = -10;

			// Token: 0x04000664 RID: 1636
			private const int TownSecurityPenaltyOnFail = -10;

			// Token: 0x04000665 RID: 1637
			private const int RivalGangLeaderRelationBonusOnBetrayal = 5;

			// Token: 0x04000666 RID: 1638
			private const int PlayerAttackedQuestGiverHonorPenalty = -150;

			// Token: 0x04000667 RID: 1639
			private const int PlayerAttackedQuestGiverPowerPenalty = -10;

			// Token: 0x04000668 RID: 1640
			private const int PlayerAttackedQuestGiverRelationPenalty = -8;

			// Token: 0x04000669 RID: 1641
			private const int PlayerAttackedQuestGiverSecurityPenalty = -10;

			// Token: 0x0400066A RID: 1642
			private const int NumberofRegularEnemyTroops = 15;

			// Token: 0x0400066B RID: 1643
			private const int NumberOfRegularAllyTroops = 20;

			// Token: 0x0400066C RID: 1644
			private const int MaxNumberOfPlayerOwnedTroops = 5;

			// Token: 0x0400066D RID: 1645
			[SaveableField(10)]
			internal readonly Hero _rivalGangLeader;

			// Token: 0x0400066E RID: 1646
			[SaveableField(20)]
			private MobileParty _rivalGangLeaderParty;

			// Token: 0x0400066F RID: 1647
			[SaveableField(30)]
			private readonly CampaignTime _preparationCompletionTime;

			// Token: 0x04000670 RID: 1648
			[SaveableField(40)]
			private readonly CampaignTime _questTimeoutTime;

			// Token: 0x04000671 RID: 1649
			[SaveableField(60)]
			internal readonly float _timeoutDurationInDays;

			// Token: 0x04000672 RID: 1650
			[SaveableField(70)]
			internal bool _isFinalStage;

			// Token: 0x04000673 RID: 1651
			[SaveableField(80)]
			internal bool _isReadyToBeFinalized;

			// Token: 0x04000674 RID: 1652
			[SaveableField(90)]
			internal bool _hasBetrayedQuestGiver;

			// Token: 0x04000675 RID: 1653
			[SaveableField(100)]
			private List<TroopRosterElement> _playerTroops = new List<TroopRosterElement>();

			// Token: 0x04000676 RID: 1654
			[SaveableField(110)]
			private bool _preparationsComplete;

			// Token: 0x04000677 RID: 1655
			[SaveableField(120)]
			private int _rewardGold;

			// Token: 0x04000678 RID: 1656
			[SaveableField(130)]
			private float _issueDifficulty;

			// Token: 0x04000679 RID: 1657
			private Settlement _questSettlement;

			// Token: 0x0400067A RID: 1658
			private JournalLog _onQuestStartedLog;

			// Token: 0x0400067B RID: 1659
			private JournalLog _onQuestSucceededLog;
		}
	}
}
