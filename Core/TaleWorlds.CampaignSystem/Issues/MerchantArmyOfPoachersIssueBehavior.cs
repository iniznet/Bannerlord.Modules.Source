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
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Overlay;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.Issues
{
	public class MerchantArmyOfPoachersIssueBehavior : CampaignBehaviorBase
	{
		private void engage_poachers_consequence(MenuCallbackArgs args)
		{
			MerchantArmyOfPoachersIssueBehavior.Instance.StartQuestBattle();
		}

		private static MerchantArmyOfPoachersIssueBehavior.MerchantArmyOfPoachersIssueQuest Instance
		{
			get
			{
				MerchantArmyOfPoachersIssueBehavior campaignBehavior = Campaign.Current.GetCampaignBehavior<MerchantArmyOfPoachersIssueBehavior>();
				if (campaignBehavior._cachedQuest != null && campaignBehavior._cachedQuest.IsOngoing)
				{
					return campaignBehavior._cachedQuest;
				}
				using (List<QuestBase>.Enumerator enumerator = Campaign.Current.QuestManager.Quests.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						MerchantArmyOfPoachersIssueBehavior.MerchantArmyOfPoachersIssueQuest merchantArmyOfPoachersIssueQuest;
						if ((merchantArmyOfPoachersIssueQuest = enumerator.Current as MerchantArmyOfPoachersIssueBehavior.MerchantArmyOfPoachersIssueQuest) != null)
						{
							campaignBehavior._cachedQuest = merchantArmyOfPoachersIssueQuest;
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

		private bool poachers_menu_back_condition(MenuCallbackArgs args)
		{
			return Hero.MainHero.IsWounded;
		}

		private void OnSessionLaunched(CampaignGameStarter gameStarter)
		{
			gameStarter.AddGameMenu("army_of_poachers_village", "{=eaQxeRh6}A boy runs out of the village and asks you to talk to the leader of the poachers. The villagers want to avoid a fight outside their homes.", new OnInitDelegate(this.army_of_poachers_village_on_init), GameOverlays.MenuOverlayType.None, GameMenu.MenuFlags.None, null);
			gameStarter.AddGameMenuOption("army_of_poachers_village", "engage_the_poachers", "{=xF7he8fZ}Fight the poachers", new GameMenuOption.OnConditionDelegate(this.engage_poachers_condition), new GameMenuOption.OnConsequenceDelegate(this.engage_poachers_consequence), false, -1, false, null);
			gameStarter.AddGameMenuOption("army_of_poachers_village", "talk_to_the_poachers", "{=wwJGE28v}Negotiate with the poachers", new GameMenuOption.OnConditionDelegate(this.talk_to_leader_of_poachers_condition), new GameMenuOption.OnConsequenceDelegate(this.talk_to_leader_of_poachers_consequence), false, -1, false, null);
			gameStarter.AddGameMenuOption("army_of_poachers_village", "back_poachers", "{=E1OwmQFb}Back", new GameMenuOption.OnConditionDelegate(this.poachers_menu_back_condition), new GameMenuOption.OnConsequenceDelegate(this.poachers_menu_back_consequence), false, -1, false, null);
		}

		private void army_of_poachers_village_on_init(MenuCallbackArgs args)
		{
			if (MerchantArmyOfPoachersIssueBehavior.Instance != null && MerchantArmyOfPoachersIssueBehavior.Instance.IsOngoing)
			{
				args.MenuContext.SetBackgroundMeshName(MerchantArmyOfPoachersIssueBehavior.Instance._questVillage.Settlement.SettlementComponent.WaitMeshName);
				if (MerchantArmyOfPoachersIssueBehavior.Instance._poachersParty == null && !Hero.MainHero.IsWounded)
				{
					MerchantArmyOfPoachersIssueBehavior.Instance.CreatePoachersParty();
				}
				if (MerchantArmyOfPoachersIssueBehavior.Instance._isReadyToBeFinalized && PlayerEncounter.Current != null)
				{
					bool flag = PlayerEncounter.Battle.WinningSide == PlayerEncounter.Battle.PlayerSide;
					PlayerEncounter.Update();
					if (PlayerEncounter.Current == null)
					{
						MerchantArmyOfPoachersIssueBehavior.Instance._isReadyToBeFinalized = false;
						if (flag)
						{
							MerchantArmyOfPoachersIssueBehavior.Instance.QuestSuccessWithPlayerDefeatedPoachers();
						}
						else
						{
							MerchantArmyOfPoachersIssueBehavior.Instance.QuestFailWithPlayerDefeatedAgainstPoachers();
						}
					}
					else if (PlayerEncounter.Battle.WinningSide == BattleSideEnum.None)
					{
						PlayerEncounter.LeaveEncounter = true;
						PlayerEncounter.Update();
						MerchantArmyOfPoachersIssueBehavior.Instance.QuestFailWithPlayerDefeatedAgainstPoachers();
					}
					else if (flag && PlayerEncounter.Current != null && Game.Current.GameStateManager.ActiveState is MapState)
					{
						PlayerEncounter.Finish(true);
						MerchantArmyOfPoachersIssueBehavior.Instance.QuestSuccessWithPlayerDefeatedPoachers();
					}
				}
				if (MerchantArmyOfPoachersIssueBehavior.Instance != null && MerchantArmyOfPoachersIssueBehavior.Instance._talkedToPoachersBattleWillStart)
				{
					MerchantArmyOfPoachersIssueBehavior.Instance.StartQuestBattle();
				}
			}
		}

		private bool engage_poachers_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Mission;
			if (Hero.MainHero.IsWounded)
			{
				args.Tooltip = new TextObject("{=gEHEQazX}You're heavily wounded and not fit for the fight. Come back when you're ready.", null);
				args.IsEnabled = false;
			}
			return true;
		}

		public override void SyncData(IDataStore dataStore)
		{
		}

		private bool talk_to_leader_of_poachers_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = GameMenuOption.LeaveType.Conversation;
			if (Hero.MainHero.IsWounded)
			{
				args.Tooltip = new TextObject("{=gEHEQazX}You're heavily wounded and not fit for the fight. Come back when you're ready.", null);
				args.IsEnabled = false;
			}
			return true;
		}

		private void poachers_menu_back_consequence(MenuCallbackArgs args)
		{
			PlayerEncounter.LeaveSettlement();
			PlayerEncounter.Finish(true);
		}

		private bool ConditionsHold(Hero issueGiver, out Village questVillage)
		{
			questVillage = null;
			if (issueGiver.CurrentSettlement == null)
			{
				return false;
			}
			questVillage = issueGiver.CurrentSettlement.BoundVillages.GetRandomElementWithPredicate((Village x) => !x.Settlement.IsUnderRaid && !x.Settlement.IsRaided);
			if (questVillage != null && issueGiver.IsMerchant && issueGiver.GetTraitLevel(DefaultTraits.Mercy) + issueGiver.GetTraitLevel(DefaultTraits.Honor) < 0)
			{
				Town town = issueGiver.CurrentSettlement.Town;
				return town != null && town.Security <= (float)60;
			}
			return false;
		}

		private void talk_to_leader_of_poachers_consequence(MenuCallbackArgs args)
		{
			CampaignMapConversation.OpenConversation(new ConversationCharacterData(CharacterObject.PlayerCharacter, PartyBase.MainParty, false, false, false, false, false, false), new ConversationCharacterData(ConversationHelper.GetConversationCharacterPartyLeader(MerchantArmyOfPoachersIssueBehavior.Instance._poachersParty.Party), MerchantArmyOfPoachersIssueBehavior.Instance._poachersParty.Party, false, false, false, false, false, false));
		}

		public void OnCheckForIssue(Hero hero)
		{
			Village village;
			if (this.ConditionsHold(hero, out village))
			{
				Campaign.Current.IssueManager.AddPotentialIssueData(hero, new PotentialIssueData(new PotentialIssueData.StartIssueDelegate(this.OnSelected), typeof(MerchantArmyOfPoachersIssueBehavior.MerchantArmyOfPoachersIssue), IssueBase.IssueFrequency.Common, village));
				return;
			}
			Campaign.Current.IssueManager.AddPotentialIssueData(hero, new PotentialIssueData(typeof(MerchantArmyOfPoachersIssueBehavior.MerchantArmyOfPoachersIssue), IssueBase.IssueFrequency.Common));
		}

		private IssueBase OnSelected(in PotentialIssueData pid, Hero issueOwner)
		{
			PotentialIssueData potentialIssueData = pid;
			return new MerchantArmyOfPoachersIssueBehavior.MerchantArmyOfPoachersIssue(issueOwner, potentialIssueData.RelatedObject as Village);
		}

		private const IssueBase.IssueFrequency ArmyOfPoachersIssueFrequency = IssueBase.IssueFrequency.Common;

		private MerchantArmyOfPoachersIssueBehavior.MerchantArmyOfPoachersIssueQuest _cachedQuest;

		public class MerchantArmyOfPoachersIssue : IssueBase
		{
			internal static void AutoGeneratedStaticCollectObjectsMerchantArmyOfPoachersIssue(object o, List<object> collectedObjects)
			{
				((MerchantArmyOfPoachersIssueBehavior.MerchantArmyOfPoachersIssue)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				base.AutoGeneratedInstanceCollectObjects(collectedObjects);
				collectedObjects.Add(this._questVillage);
			}

			internal static object AutoGeneratedGetMemberValue_questVillage(object o)
			{
				return ((MerchantArmyOfPoachersIssueBehavior.MerchantArmyOfPoachersIssue)o)._questVillage;
			}

			public override int AlternativeSolutionBaseNeededMenCount
			{
				get
				{
					return 12 + MathF.Ceiling(28f * base.IssueDifficultyMultiplier);
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
					return (int)(500f + 3000f * base.IssueDifficultyMultiplier);
				}
			}

			public override IssueBase.AlternativeSolutionScaleFlag AlternativeSolutionScaleFlags
			{
				get
				{
					return IssueBase.AlternativeSolutionScaleFlag.Casualties | IssueBase.AlternativeSolutionScaleFlag.FailureRisk;
				}
			}

			public override TextObject IssueBriefByIssueGiver
			{
				get
				{
					return new TextObject("{=Jk3mDlU6}Yeah... I've got some problems. A few years ago, I needed hides for my tannery and I hired some hunters. I didn't ask too many questions about where they came by the skins they sold me. Well, that was a bit of mistake. Now they've banded together as a gang and are trying to muscle me out of the leather business.[ib:closed2][if:convo_thinking]", null);
				}
			}

			public override TextObject IssueAcceptByPlayer
			{
				get
				{
					return new TextObject("{=apuNQC2W}What can I do for you?", null);
				}
			}

			public override TextObject IssueQuestSolutionExplanationByIssueGiver
			{
				get
				{
					TextObject textObject = new TextObject("{=LbTETjZu}I want you to crush them. Go to {VILLAGE} and give them a lesson they won't forget.[ib:closed2][if:convo_grave]", null);
					textObject.SetTextVariable("VILLAGE", this._questVillage.Settlement.EncyclopediaLinkWithName);
					return textObject;
				}
			}

			public override TextObject IssueAlternativeSolutionExplanationByIssueGiver
			{
				get
				{
					TextObject textObject = new TextObject("{=2ELhox6C}If you don't want to get involved in this yourself, leave one of your capable companions and {NUMBER_OF_TROOPS} men for some days.[ib:closed][if:convo_grave]", null);
					textObject.SetTextVariable("NUMBER_OF_TROOPS", base.GetTotalAlternativeSolutionNeededMenCount());
					return textObject;
				}
			}

			public override TextObject IssueQuestSolutionAcceptByPlayer
			{
				get
				{
					return new TextObject("{=b6naGx6H}I'll rid you of those poachers myself.", null);
				}
			}

			public override TextObject IssueAlternativeSolutionAcceptByPlayer
			{
				get
				{
					return new TextObject("{=lA14Ubal}I can send a companion to hunt these poachers.", null);
				}
			}

			public override TextObject IssueAlternativeSolutionResponseByIssueGiver
			{
				get
				{
					return new TextObject("{=Xmtlrrmf}Thank you.[ib:normal][if:convo_normal]  Don't forget to warn your men. These poachers are not ordinary bandits. Good luck.", null);
				}
			}

			public override TextObject IssueDiscussAlternativeSolution
			{
				get
				{
					return new TextObject("{=51ahPi69}I understand that your men are still chasing those poachers. I realize that this mess might take a little time to clean up.[ib:normal2][if:convo_grave]", null);
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

			protected override TextObject AlternativeSolutionStartLog
			{
				get
				{
					TextObject textObject = new TextObject("{=428B377z}{ISSUE_GIVER.LINK}, a merchant of {QUEST_GIVER_SETTLEMENT}, told you that the poachers {?ISSUE_GIVER.GENDER}she{?}he{\\?} hired are now out of control. You asked {COMPANION.LINK} to take {NEEDED_MEN_COUNT} of your men to go to {QUEST_VILLAGE} and kill the poachers. They should rejoin your party in {RETURN_DAYS} days.", null);
					StringHelpers.SetCharacterProperties("ISSUE_GIVER", base.IssueOwner.CharacterObject, textObject, false);
					StringHelpers.SetCharacterProperties("COMPANION", base.AlternativeSolutionHero.CharacterObject, textObject, false);
					textObject.SetTextVariable("QUEST_GIVER_SETTLEMENT", base.IssueOwner.CurrentSettlement.EncyclopediaLinkWithName);
					textObject.SetTextVariable("NEEDED_MEN_COUNT", this.AlternativeSolutionSentTroops.TotalManCount - 1);
					textObject.SetTextVariable("QUEST_VILLAGE", this._questVillage.Settlement.EncyclopediaLinkWithName);
					textObject.SetTextVariable("RETURN_DAYS", base.GetTotalAlternativeSolutionDurationInDays());
					return textObject;
				}
			}

			public override TextObject Title
			{
				get
				{
					return new TextObject("{=iHFo2kjz}Army of Poachers", null);
				}
			}

			public override TextObject Description
			{
				get
				{
					TextObject textObject = new TextObject("{=NCC4VUOc}{ISSUE_GIVER.LINK} wants you to get rid of the poachers who once worked for {?ISSUE_GIVER.GENDER}her{?}him{\\?} but are now out of control.", null);
					StringHelpers.SetCharacterProperties("ISSUE_GIVER", base.IssueOwner.CharacterObject, null, false);
					return textObject;
				}
			}

			public MerchantArmyOfPoachersIssue(Hero issueOwner, Village questVillage)
				: base(issueOwner, CampaignTime.DaysFromNow(15f))
			{
				this._questVillage = questVillage;
			}

			protected override float GetIssueEffectAmountInternal(IssueEffect issueEffect)
			{
				if (issueEffect == DefaultIssueEffects.SettlementProsperity)
				{
					return 0.2f;
				}
				if (issueEffect == DefaultIssueEffects.SettlementSecurity)
				{
					return -1f;
				}
				if (issueEffect == DefaultIssueEffects.SettlementLoyalty)
				{
					return -0.2f;
				}
				if (issueEffect == DefaultIssueEffects.IssueOwnerPower)
				{
					return -0.2f;
				}
				return 0f;
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

			public override ValueTuple<SkillObject, int> GetAlternativeSolutionSkill(Hero hero)
			{
				int skillValue = hero.GetSkillValue(DefaultSkills.Bow);
				int skillValue2 = hero.GetSkillValue(DefaultSkills.Crossbow);
				int skillValue3 = hero.GetSkillValue(DefaultSkills.Throwing);
				if (skillValue >= skillValue2 && skillValue >= skillValue3)
				{
					return new ValueTuple<SkillObject, int>(DefaultSkills.Bow, 150);
				}
				return new ValueTuple<SkillObject, int>((skillValue2 >= skillValue3) ? DefaultSkills.Crossbow : DefaultSkills.Throwing, 150);
			}

			public override bool AlternativeSolutionCondition(out TextObject explanation)
			{
				explanation = TextObject.Empty;
				return QuestHelper.CheckRosterForAlternativeSolution(MobileParty.MainParty.MemberRoster, base.GetTotalAlternativeSolutionNeededMenCount(), ref explanation, 2, false);
			}

			public override IssueBase.IssueFrequency GetFrequency()
			{
				return IssueBase.IssueFrequency.Common;
			}

			public override bool IssueStayAliveConditions()
			{
				return !this._questVillage.Settlement.IsUnderRaid && !this._questVillage.Settlement.IsRaided && base.IssueOwner.CurrentSettlement.Town.Security <= 90f;
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
				if (MobileParty.MainParty.MemberRoster.TotalHealthyCount < 15)
				{
					flag |= IssueBase.PreconditionFlags.NotEnoughTroops;
				}
				if (issueGiver.MapFaction.IsAtWarWith(Hero.MainHero.MapFaction))
				{
					flag |= IssueBase.PreconditionFlags.AtWar;
				}
				return flag == IssueBase.PreconditionFlags.None;
			}

			protected override void OnGameLoad()
			{
			}

			protected override void HourlyTick()
			{
			}

			protected override QuestBase GenerateIssueQuest(string questId)
			{
				return new MerchantArmyOfPoachersIssueBehavior.MerchantArmyOfPoachersIssueQuest(questId, base.IssueOwner, CampaignTime.DaysFromNow(20f), this._questVillage, base.IssueDifficultyMultiplier, this.RewardGold);
			}

			protected override void CompleteIssueWithTimedOutConsequences()
			{
			}

			protected override int CompanionSkillRewardXP
			{
				get
				{
					return (int)(800f + 1000f * base.IssueDifficultyMultiplier);
				}
			}

			protected override void AlternativeSolutionEndWithSuccessConsequence()
			{
				this.RelationshipChangeWithIssueOwner = 5;
				base.IssueOwner.AddPower(30f);
				base.IssueOwner.CurrentSettlement.Town.Prosperity += 50f;
			}

			protected override void AlternativeSolutionEndWithFailureConsequence()
			{
				this.RelationshipChangeWithIssueOwner = -5;
				base.IssueOwner.AddPower(-50f);
				base.IssueOwner.CurrentSettlement.Town.Prosperity -= 30f;
				base.IssueOwner.CurrentSettlement.Town.Security -= 5f;
				TraitLevelingHelper.OnIssueFailed(base.IssueOwner, new Tuple<TraitObject, int>[]
				{
					new Tuple<TraitObject, int>(DefaultTraits.Honor, -30)
				});
			}

			private const int AlternativeSolutionTroopTierRequirement = 2;

			private const int CompanionRequiredSkillLevel = 150;

			private const int MinimumRequiredMenCount = 15;

			private const int IssueDuration = 15;

			private const int QuestTimeLimit = 20;

			[SaveableField(10)]
			private Village _questVillage;
		}

		public class MerchantArmyOfPoachersIssueQuest : QuestBase
		{
			internal static void AutoGeneratedStaticCollectObjectsMerchantArmyOfPoachersIssueQuest(object o, List<object> collectedObjects)
			{
				((MerchantArmyOfPoachersIssueBehavior.MerchantArmyOfPoachersIssueQuest)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				base.AutoGeneratedInstanceCollectObjects(collectedObjects);
				collectedObjects.Add(this._poachersParty);
				collectedObjects.Add(this._questVillage);
			}

			internal static object AutoGeneratedGetMemberValue_poachersParty(object o)
			{
				return ((MerchantArmyOfPoachersIssueBehavior.MerchantArmyOfPoachersIssueQuest)o)._poachersParty;
			}

			internal static object AutoGeneratedGetMemberValue_questVillage(object o)
			{
				return ((MerchantArmyOfPoachersIssueBehavior.MerchantArmyOfPoachersIssueQuest)o)._questVillage;
			}

			internal static object AutoGeneratedGetMemberValue_talkedToPoachersBattleWillStart(object o)
			{
				return ((MerchantArmyOfPoachersIssueBehavior.MerchantArmyOfPoachersIssueQuest)o)._talkedToPoachersBattleWillStart;
			}

			internal static object AutoGeneratedGetMemberValue_isReadyToBeFinalized(object o)
			{
				return ((MerchantArmyOfPoachersIssueBehavior.MerchantArmyOfPoachersIssueQuest)o)._isReadyToBeFinalized;
			}

			internal static object AutoGeneratedGetMemberValue_persuasionTriedOnce(object o)
			{
				return ((MerchantArmyOfPoachersIssueBehavior.MerchantArmyOfPoachersIssueQuest)o)._persuasionTriedOnce;
			}

			internal static object AutoGeneratedGetMemberValue_difficultyMultiplier(object o)
			{
				return ((MerchantArmyOfPoachersIssueBehavior.MerchantArmyOfPoachersIssueQuest)o)._difficultyMultiplier;
			}

			internal static object AutoGeneratedGetMemberValue_rewardGold(object o)
			{
				return ((MerchantArmyOfPoachersIssueBehavior.MerchantArmyOfPoachersIssueQuest)o)._rewardGold;
			}

			public override TextObject Title
			{
				get
				{
					return new TextObject("{=iHFo2kjz}Army of Poachers", null);
				}
			}

			public override bool IsRemainingTimeHidden
			{
				get
				{
					return false;
				}
			}

			private TextObject _questStartedLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=fk4ewfQh}{QUEST_GIVER.LINK}, a merchant of {SETTLEMENT}, told you that the poachers {?QUEST_GIVER.GENDER}she{?}he{\\?} hired before are now out of control. {?QUEST_GIVER.GENDER}She{?}He{\\?} asked you to go to {VILLAGE} around midnight and kill the poachers.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					textObject.SetTextVariable("SETTLEMENT", base.QuestGiver.CurrentSettlement.EncyclopediaLinkWithName);
					textObject.SetTextVariable("VILLAGE", this._questVillage.Settlement.EncyclopediaLinkWithName);
					return textObject;
				}
			}

			private TextObject _questCanceledTargetVillageRaided
			{
				get
				{
					TextObject textObject = new TextObject("{=etYq1Tky}{VILLAGE} was raided and the poachers scattered.", null);
					textObject.SetTextVariable("VILLAGE", this._questVillage.Settlement.EncyclopediaLinkWithName);
					return textObject;
				}
			}

			private TextObject _questCanceledWarDeclared
			{
				get
				{
					TextObject textObject = new TextObject("{=vW6kBki9}Your clan is now at war with {QUEST_GIVER.LINK}'s realm. Your agreement with {QUEST_GIVER.LINK} is canceled.", null);
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

			private TextObject _questFailedAfterTalkingWithProachers
			{
				get
				{
					TextObject textObject = new TextObject("{=PIukmFYA}You decided not to get involved and left the village. You have failed to help {QUEST_GIVER.LINK} as promised.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					return textObject;
				}
			}

			private TextObject _questSuccessPlayerComesToAnAgreementWithPoachers
			{
				get
				{
					return new TextObject("{=qPfJpwGa}You have persuaded the poachers to leave the district.", null);
				}
			}

			private TextObject _questFailWithPlayerDefeatedAgainstPoachers
			{
				get
				{
					TextObject textObject = new TextObject("{=p8Kfl5u6}You lost the battle against the poachers and failed to help {QUEST_GIVER.LINK} as promised.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					return textObject;
				}
			}

			private TextObject _questSuccessWithPlayerDefeatedPoachers
			{
				get
				{
					TextObject textObject = new TextObject("{=8gNqLqFl}You have defeated the poachers and helped {QUEST_GIVER.LINK} as promised.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					return textObject;
				}
			}

			private TextObject _questFailedWithTimeOutLogText
			{
				get
				{
					return new TextObject("{=HX7E09XJ}You failed to complete the quest in time.", null);
				}
			}

			public MerchantArmyOfPoachersIssueQuest(string questId, Hero giverHero, CampaignTime duration, Village questVillage, float difficultyMultiplier, int rewardGold)
				: base(questId, giverHero, duration, rewardGold)
			{
				this._questVillage = questVillage;
				this._talkedToPoachersBattleWillStart = false;
				this._isReadyToBeFinalized = false;
				this._difficultyMultiplier = difficultyMultiplier;
				this._rewardGold = rewardGold;
				this.SetDialogs();
				base.InitializeQuestOnCreation();
			}

			private bool SetStartDialogOnCondition()
			{
				if (this._poachersParty != null && CharacterObject.OneToOneConversationCharacter == ConversationHelper.GetConversationCharacterPartyLeader(this._poachersParty.Party))
				{
					MBTextManager.SetTextVariable("POACHER_PARTY_START_LINE", "{=j9MBwnWI}Well...Are you working for that merchant in the town ? So it's all fine when the rich folk trade in poached skins, but if we do it, armed men come to hunt us down.", false);
					if (this._persuasionTriedOnce)
					{
						MBTextManager.SetTextVariable("POACHER_PARTY_START_LINE", "{=Nn06TSq9}Anything else to say?", false);
					}
					return true;
				}
				return false;
			}

			private DialogFlow GetPoacherPartyDialogFlow()
			{
				DialogFlow dialogFlow = DialogFlow.CreateDialogFlow("start", 125).NpcLine("{=!}{POACHER_PARTY_START_LINE}", null, null).Condition(() => this.SetStartDialogOnCondition())
					.Consequence(delegate
					{
						this._task = this.GetPersuasionTask();
					})
					.BeginPlayerOptions()
					.PlayerOption("{=afbLOXbb}Maybe we can come to an agreement.", null)
					.Condition(() => !this._persuasionTriedOnce)
					.Consequence(delegate
					{
						this._persuasionTriedOnce = true;
					})
					.GotoDialogState("start_poachers_persuasion")
					.PlayerOption("{=mvw1ayGt}I'm here to do the job I agreed to do, outlaw. Give up or die.", null)
					.NpcLine("{=hOVr77fd}You will never see the sunrise again![ib:warrior][if:convo_furious]", null, null)
					.Consequence(delegate
					{
						this._talkedToPoachersBattleWillStart = true;
					})
					.CloseDialog()
					.PlayerOption("{=VJYEoOAc}Well... You have a point. Go on. We won't bother you any more.", null)
					.NpcLine("{=wglTyBbx}Thank you, friend. Go in peace.[ib:normal][if:convo_approving]", null, null)
					.Consequence(delegate
					{
						Campaign.Current.GameMenuManager.SetNextMenu("village");
						Campaign.Current.ConversationManager.ConversationEndOneShot += this.QuestFailedAfterTalkingWithPoachers;
					})
					.CloseDialog()
					.EndPlayerOptions()
					.CloseDialog();
				this.AddPersuasionDialogs(dialogFlow);
				return dialogFlow;
			}

			private void AddPersuasionDialogs(DialogFlow dialog)
			{
				dialog.AddDialogLine("poachers_persuasion_check_accepted", "start_poachers_persuasion", "poachers_persuasion_start_reservation", "{=6P1ruzsC}Maybe...", new ConversationSentence.OnConditionDelegate(this.persuasion_start_with_poachers_on_condition), new ConversationSentence.OnConsequenceDelegate(this.persuasion_start_with_poachers_on_consequence), this, 100, null, null, null);
				dialog.AddDialogLine("poachers_persuasion_rejected", "poachers_persuasion_start_reservation", "start", "{=!}{FAILED_PERSUASION_LINE}", new ConversationSentence.OnConditionDelegate(this.persuasion_failed_with_poachers_on_condition), new ConversationSentence.OnConsequenceDelegate(this.persuasion_rejected_with_poachers_on_consequence), this, 100, null, null, null);
				dialog.AddDialogLine("poachers_persuasion_attempt", "poachers_persuasion_start_reservation", "poachers_persuasion_select_option", "{=wM77S68a}What's there to discuss?", () => !this.persuasion_failed_with_poachers_on_condition(), null, this, 100, null, null, null);
				dialog.AddDialogLine("poachers_persuasion_success", "poachers_persuasion_start_reservation", "close_window", "{=JQKCPllJ}You've made your point.", new ConversationSentence.OnConditionDelegate(ConversationManager.GetPersuasionProgressSatisfied), new ConversationSentence.OnConsequenceDelegate(this.persuasion_complete_with_poachers_on_consequence), this, 200, null, null, null);
				string text = "poachers_persuasion_select_option_1";
				string text2 = "poachers_persuasion_select_option";
				string text3 = "poachers_persuasion_selected_option_response";
				string text4 = "{=!}{POACHERS_PERSUADE_ATTEMPT_1}";
				ConversationSentence.OnConditionDelegate onConditionDelegate = new ConversationSentence.OnConditionDelegate(this.poachers_persuasion_select_option_1_on_condition);
				ConversationSentence.OnConsequenceDelegate onConsequenceDelegate = new ConversationSentence.OnConsequenceDelegate(this.poachers_persuasion_select_option_1_on_consequence);
				ConversationSentence.OnPersuasionOptionDelegate onPersuasionOptionDelegate = new ConversationSentence.OnPersuasionOptionDelegate(this.poachers_persuasion_setup_option_1);
				ConversationSentence.OnClickableConditionDelegate onClickableConditionDelegate = new ConversationSentence.OnClickableConditionDelegate(this.poachers_persuasion_clickable_option_1_on_condition);
				dialog.AddPlayerLine(text, text2, text3, text4, onConditionDelegate, onConsequenceDelegate, this, 100, onClickableConditionDelegate, onPersuasionOptionDelegate, null, null);
				string text5 = "poachers_persuasion_select_option_2";
				string text6 = "poachers_persuasion_select_option";
				string text7 = "poachers_persuasion_selected_option_response";
				string text8 = "{=!}{POACHERS_PERSUADE_ATTEMPT_2}";
				ConversationSentence.OnConditionDelegate onConditionDelegate2 = new ConversationSentence.OnConditionDelegate(this.poachers_persuasion_select_option_2_on_condition);
				ConversationSentence.OnConsequenceDelegate onConsequenceDelegate2 = new ConversationSentence.OnConsequenceDelegate(this.poachers_persuasion_select_option_2_on_consequence);
				onPersuasionOptionDelegate = new ConversationSentence.OnPersuasionOptionDelegate(this.poachers_persuasion_setup_option_2);
				onClickableConditionDelegate = new ConversationSentence.OnClickableConditionDelegate(this.poachers_persuasion_clickable_option_2_on_condition);
				dialog.AddPlayerLine(text5, text6, text7, text8, onConditionDelegate2, onConsequenceDelegate2, this, 100, onClickableConditionDelegate, onPersuasionOptionDelegate, null, null);
				string text9 = "poachers_persuasion_select_option_3";
				string text10 = "poachers_persuasion_select_option";
				string text11 = "poachers_persuasion_selected_option_response";
				string text12 = "{=!}{POACHERS_PERSUADE_ATTEMPT_3}";
				ConversationSentence.OnConditionDelegate onConditionDelegate3 = new ConversationSentence.OnConditionDelegate(this.poachers_persuasion_select_option_3_on_condition);
				ConversationSentence.OnConsequenceDelegate onConsequenceDelegate3 = new ConversationSentence.OnConsequenceDelegate(this.poachers_persuasion_select_option_3_on_consequence);
				onPersuasionOptionDelegate = new ConversationSentence.OnPersuasionOptionDelegate(this.poachers_persuasion_setup_option_3);
				onClickableConditionDelegate = new ConversationSentence.OnClickableConditionDelegate(this.poachers_persuasion_clickable_option_3_on_condition);
				dialog.AddPlayerLine(text9, text10, text11, text12, onConditionDelegate3, onConsequenceDelegate3, this, 100, onClickableConditionDelegate, onPersuasionOptionDelegate, null, null);
				string text13 = "poachers_persuasion_select_option_4";
				string text14 = "poachers_persuasion_select_option";
				string text15 = "poachers_persuasion_selected_option_response";
				string text16 = "{=!}{POACHERS_PERSUADE_ATTEMPT_4}";
				ConversationSentence.OnConditionDelegate onConditionDelegate4 = new ConversationSentence.OnConditionDelegate(this.poachers_persuasion_select_option_4_on_condition);
				ConversationSentence.OnConsequenceDelegate onConsequenceDelegate4 = new ConversationSentence.OnConsequenceDelegate(this.poachers_persuasion_select_option_4_on_consequence);
				onPersuasionOptionDelegate = new ConversationSentence.OnPersuasionOptionDelegate(this.poachers_persuasion_setup_option_4);
				onClickableConditionDelegate = new ConversationSentence.OnClickableConditionDelegate(this.poachers_persuasion_clickable_option_4_on_condition);
				dialog.AddPlayerLine(text13, text14, text15, text16, onConditionDelegate4, onConsequenceDelegate4, this, 100, onClickableConditionDelegate, onPersuasionOptionDelegate, null, null);
				string text17 = "poachers_persuasion_select_option_5";
				string text18 = "poachers_persuasion_select_option";
				string text19 = "poachers_persuasion_selected_option_response";
				string text20 = "{=!}{POACHERS_PERSUADE_ATTEMPT_5}";
				ConversationSentence.OnConditionDelegate onConditionDelegate5 = new ConversationSentence.OnConditionDelegate(this.poachers_persuasion_select_option_5_on_condition);
				ConversationSentence.OnConsequenceDelegate onConsequenceDelegate5 = new ConversationSentence.OnConsequenceDelegate(this.poachers_persuasion_select_option_5_on_consequence);
				onPersuasionOptionDelegate = new ConversationSentence.OnPersuasionOptionDelegate(this.poachers_persuasion_setup_option_5);
				onClickableConditionDelegate = new ConversationSentence.OnClickableConditionDelegate(this.poachers_persuasion_clickable_option_5_on_condition);
				dialog.AddPlayerLine(text17, text18, text19, text20, onConditionDelegate5, onConsequenceDelegate5, this, 100, onClickableConditionDelegate, onPersuasionOptionDelegate, null, null);
				dialog.AddDialogLine("poachers_persuasion_select_option_reaction", "poachers_persuasion_selected_option_response", "poachers_persuasion_start_reservation", "{=!}{PERSUASION_REACTION}", new ConversationSentence.OnConditionDelegate(this.poachers_persuasion_selected_option_response_on_condition), new ConversationSentence.OnConsequenceDelegate(this.poachers_persuasion_selected_option_response_on_consequence), this, 100, null, null, null);
			}

			private void persuasion_start_with_poachers_on_consequence()
			{
				ConversationManager.StartPersuasion(2f, 1f, 0f, 2f, 2f, 0f, PersuasionDifficulty.MediumHard);
			}

			private bool persuasion_start_with_poachers_on_condition()
			{
				return this._poachersParty != null && CharacterObject.OneToOneConversationCharacter == ConversationHelper.GetConversationCharacterPartyLeader(this._poachersParty.Party);
			}

			private PersuasionTask GetPersuasionTask()
			{
				PersuasionTask persuasionTask = new PersuasionTask(0);
				persuasionTask.FinalFailLine = new TextObject("{=l7Jt5tvt}This is how I earn my living, and all your clever talk doesn't make it any different. Leave now!", null);
				persuasionTask.TryLaterLine = new TextObject("{=!}TODO", null);
				persuasionTask.SpokenLine = new TextObject("{=wM77S68a}What's there to discuss?", null);
				PersuasionOptionArgs persuasionOptionArgs = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Calculating, TraitEffect.Positive, PersuasionArgumentStrength.Easy, false, new TextObject("{=cQCs72U7}You're not bad people. You can easily ply your trade somewhere else, somewhere safe.", null), null, false, false, false);
				persuasionTask.AddOptionToTask(persuasionOptionArgs);
				PersuasionOptionArgs persuasionOptionArgs2 = new PersuasionOptionArgs(DefaultSkills.Roguery, DefaultTraits.Valor, TraitEffect.Positive, PersuasionArgumentStrength.ExtremelyHard, true, new TextObject("{=bioyMrUD}You are just a bunch of hunters. You don't stand a chance against us!", null), null, true, false, false);
				persuasionTask.AddOptionToTask(persuasionOptionArgs2);
				PersuasionOptionArgs persuasionOptionArgs3 = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Mercy, TraitEffect.Positive, PersuasionArgumentStrength.Normal, false, new TextObject("{=FO1oruNy}You talk about poor folk, but you think the people here like their village turned into a nest of outlaws?", null), null, false, false, false);
				persuasionTask.AddOptionToTask(persuasionOptionArgs3);
				TextObject textObject = new TextObject("{=S0NeQdLp}You had an agreement with {QUEST_GIVER.NAME}. Your word is your bond, no matter which side of the law you're on.", null);
				StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
				PersuasionOptionArgs persuasionOptionArgs4 = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Honor, TraitEffect.Positive, PersuasionArgumentStrength.Normal, false, textObject, null, false, false, false);
				persuasionTask.AddOptionToTask(persuasionOptionArgs4);
				TextObject textObject2 = new TextObject("{=brW4pjPQ}Flee while you can. An army is already on its way here to hang you all.", null);
				PersuasionOptionArgs persuasionOptionArgs5 = new PersuasionOptionArgs(DefaultSkills.Roguery, DefaultTraits.Calculating, TraitEffect.Positive, PersuasionArgumentStrength.Hard, true, textObject2, null, false, false, false);
				persuasionTask.AddOptionToTask(persuasionOptionArgs5);
				return persuasionTask;
			}

			private bool poachers_persuasion_selected_option_response_on_condition()
			{
				PersuasionOptionResult item = ConversationManager.GetPersuasionChosenOptions().Last<Tuple<PersuasionOptionArgs, PersuasionOptionResult>>().Item2;
				MBTextManager.SetTextVariable("PERSUASION_REACTION", PersuasionHelper.GetDefaultPersuasionOptionReaction(item), false);
				if (item == PersuasionOptionResult.CriticalFailure)
				{
					this._task.BlockAllOptions();
				}
				return true;
			}

			private void poachers_persuasion_selected_option_response_on_consequence()
			{
				Tuple<PersuasionOptionArgs, PersuasionOptionResult> tuple = ConversationManager.GetPersuasionChosenOptions().Last<Tuple<PersuasionOptionArgs, PersuasionOptionResult>>();
				float difficulty = Campaign.Current.Models.PersuasionModel.GetDifficulty(PersuasionDifficulty.MediumHard);
				float num;
				float num2;
				Campaign.Current.Models.PersuasionModel.GetEffectChances(tuple.Item1, out num, out num2, difficulty);
				this._task.ApplyEffects(num, num2);
			}

			private bool poachers_persuasion_select_option_1_on_condition()
			{
				if (this._task.Options.Count > 0)
				{
					TextObject textObject = new TextObject("{=bSo9hKwr}{PERSUASION_OPTION_LINE} {SUCCESS_CHANCE}", null);
					textObject.SetTextVariable("SUCCESS_CHANCE", PersuasionHelper.ShowSuccess(this._task.Options.ElementAt(0), false));
					textObject.SetTextVariable("PERSUASION_OPTION_LINE", this._task.Options.ElementAt(0).Line);
					MBTextManager.SetTextVariable("POACHERS_PERSUADE_ATTEMPT_1", textObject, false);
					return true;
				}
				return false;
			}

			private bool poachers_persuasion_select_option_2_on_condition()
			{
				if (this._task.Options.Count > 1)
				{
					TextObject textObject = new TextObject("{=bSo9hKwr}{PERSUASION_OPTION_LINE} {SUCCESS_CHANCE}", null);
					textObject.SetTextVariable("SUCCESS_CHANCE", PersuasionHelper.ShowSuccess(this._task.Options.ElementAt(1), false));
					textObject.SetTextVariable("PERSUASION_OPTION_LINE", this._task.Options.ElementAt(1).Line);
					MBTextManager.SetTextVariable("POACHERS_PERSUADE_ATTEMPT_2", textObject, false);
					return true;
				}
				return false;
			}

			private bool poachers_persuasion_select_option_3_on_condition()
			{
				if (this._task.Options.Count > 2)
				{
					TextObject textObject = new TextObject("{=bSo9hKwr}{PERSUASION_OPTION_LINE} {SUCCESS_CHANCE}", null);
					textObject.SetTextVariable("SUCCESS_CHANCE", PersuasionHelper.ShowSuccess(this._task.Options.ElementAt(2), false));
					textObject.SetTextVariable("PERSUASION_OPTION_LINE", this._task.Options.ElementAt(2).Line);
					MBTextManager.SetTextVariable("POACHERS_PERSUADE_ATTEMPT_3", textObject, false);
					return true;
				}
				return false;
			}

			private bool poachers_persuasion_select_option_4_on_condition()
			{
				if (this._task.Options.Count > 3)
				{
					TextObject textObject = new TextObject("{=bSo9hKwr}{PERSUASION_OPTION_LINE} {SUCCESS_CHANCE}", null);
					textObject.SetTextVariable("SUCCESS_CHANCE", PersuasionHelper.ShowSuccess(this._task.Options.ElementAt(3), false));
					textObject.SetTextVariable("PERSUASION_OPTION_LINE", this._task.Options.ElementAt(3).Line);
					MBTextManager.SetTextVariable("POACHERS_PERSUADE_ATTEMPT_4", textObject, false);
					return true;
				}
				return false;
			}

			private bool poachers_persuasion_select_option_5_on_condition()
			{
				if (this._task.Options.Count > 4)
				{
					TextObject textObject = new TextObject("{=bSo9hKwr}{PERSUASION_OPTION_LINE} {SUCCESS_CHANCE}", null);
					textObject.SetTextVariable("SUCCESS_CHANCE", PersuasionHelper.ShowSuccess(this._task.Options.ElementAt(4), false));
					textObject.SetTextVariable("PERSUASION_OPTION_LINE", this._task.Options.ElementAt(4).Line);
					MBTextManager.SetTextVariable("POACHERS_PERSUADE_ATTEMPT_5", textObject, false);
					return true;
				}
				return false;
			}

			private void poachers_persuasion_select_option_1_on_consequence()
			{
				if (this._task.Options.Count > 0)
				{
					this._task.Options[0].BlockTheOption(true);
				}
			}

			private void poachers_persuasion_select_option_2_on_consequence()
			{
				if (this._task.Options.Count > 1)
				{
					this._task.Options[1].BlockTheOption(true);
				}
			}

			private void poachers_persuasion_select_option_3_on_consequence()
			{
				if (this._task.Options.Count > 2)
				{
					this._task.Options[2].BlockTheOption(true);
				}
			}

			private void poachers_persuasion_select_option_4_on_consequence()
			{
				if (this._task.Options.Count > 3)
				{
					this._task.Options[3].BlockTheOption(true);
				}
			}

			private void poachers_persuasion_select_option_5_on_consequence()
			{
				if (this._task.Options.Count > 4)
				{
					this._task.Options[4].BlockTheOption(true);
				}
			}

			private bool persuasion_failed_with_poachers_on_condition()
			{
				if (this._task.Options.All((PersuasionOptionArgs x) => x.IsBlocked) && !ConversationManager.GetPersuasionProgressSatisfied())
				{
					MBTextManager.SetTextVariable("FAILED_PERSUASION_LINE", this._task.FinalFailLine, false);
					return true;
				}
				return false;
			}

			private PersuasionOptionArgs poachers_persuasion_setup_option_1()
			{
				return this._task.Options.ElementAt(0);
			}

			private PersuasionOptionArgs poachers_persuasion_setup_option_2()
			{
				return this._task.Options.ElementAt(1);
			}

			private PersuasionOptionArgs poachers_persuasion_setup_option_3()
			{
				return this._task.Options.ElementAt(2);
			}

			private PersuasionOptionArgs poachers_persuasion_setup_option_4()
			{
				return this._task.Options.ElementAt(3);
			}

			private PersuasionOptionArgs poachers_persuasion_setup_option_5()
			{
				return this._task.Options.ElementAt(4);
			}

			private bool poachers_persuasion_clickable_option_1_on_condition(out TextObject hintText)
			{
				hintText = new TextObject("{=9ACJsI6S}Blocked", null);
				if (this._task.Options.Count > 0)
				{
					hintText = (this._task.Options.ElementAt(0).IsBlocked ? hintText : TextObject.Empty);
					return !this._task.Options.ElementAt(0).IsBlocked;
				}
				return false;
			}

			private bool poachers_persuasion_clickable_option_2_on_condition(out TextObject hintText)
			{
				hintText = new TextObject("{=9ACJsI6S}Blocked", null);
				if (this._task.Options.Count > 1)
				{
					hintText = (this._task.Options.ElementAt(1).IsBlocked ? hintText : TextObject.Empty);
					return !this._task.Options.ElementAt(1).IsBlocked;
				}
				return false;
			}

			private bool poachers_persuasion_clickable_option_3_on_condition(out TextObject hintText)
			{
				hintText = new TextObject("{=9ACJsI6S}Blocked", null);
				if (this._task.Options.Count > 2)
				{
					hintText = (this._task.Options.ElementAt(2).IsBlocked ? hintText : TextObject.Empty);
					return !this._task.Options.ElementAt(2).IsBlocked;
				}
				return false;
			}

			private bool poachers_persuasion_clickable_option_4_on_condition(out TextObject hintText)
			{
				hintText = new TextObject("{=9ACJsI6S}Blocked", null);
				if (this._task.Options.Count > 3)
				{
					hintText = (this._task.Options.ElementAt(3).IsBlocked ? hintText : TextObject.Empty);
					return !this._task.Options.ElementAt(3).IsBlocked;
				}
				return false;
			}

			private bool poachers_persuasion_clickable_option_5_on_condition(out TextObject hintText)
			{
				hintText = new TextObject("{=9ACJsI6S}Blocked", null);
				if (this._task.Options.Count > 4)
				{
					hintText = (this._task.Options.ElementAt(4).IsBlocked ? hintText : TextObject.Empty);
					return !this._task.Options.ElementAt(4).IsBlocked;
				}
				return false;
			}

			private void persuasion_rejected_with_poachers_on_consequence()
			{
				PlayerEncounter.LeaveEncounter = false;
				ConversationManager.EndPersuasion();
			}

			private void persuasion_complete_with_poachers_on_consequence()
			{
				PlayerEncounter.LeaveEncounter = true;
				ConversationManager.EndPersuasion();
				Campaign.Current.GameMenuManager.SetNextMenu("village");
				Campaign.Current.ConversationManager.ConversationEndOneShot += this.QuestSuccessPlayerComesToAnAgreementWithPoachers;
			}

			internal void StartQuestBattle()
			{
				PlayerEncounter.RestartPlayerEncounter(PartyBase.MainParty, this._poachersParty.Party, false);
				PlayerEncounter.StartBattle();
				PlayerEncounter.Update();
				this._talkedToPoachersBattleWillStart = false;
				GameMenu.ActivateGameMenu("army_of_poachers_village");
				CampaignMission.OpenBattleMission(this._questVillage.Settlement.LocationComplex.GetScene("village_center", 1), false);
				this._isReadyToBeFinalized = true;
			}

			private bool DialogCondition()
			{
				return Hero.OneToOneConversationHero == base.QuestGiver;
			}

			protected override void SetDialogs()
			{
				this.OfferDialogFlow = DialogFlow.CreateDialogFlow("issue_classic_quest_start", 100).NpcLine(new TextObject("{=IefM6uAy}Thank you. You'll be paid well. Also you can keep their illegally obtained leather.[ib:normal2][if:convo_bemused]", null), null, null).Condition(new ConversationSentence.OnConditionDelegate(this.DialogCondition))
					.NpcLine(new TextObject("{=NC2VGafO}They skin their beasts in the woods, then go into the village after midnight to stash the hides. The villagers are terrified of them, I believe. If you go into the village late at night, you should be able to track them down.[ib:normal][if:convo_thinking]", null), null, null)
					.NpcLine(new TextObject("{=3pkVKMnA}Most poachers would probably run if they were surprised by armed men. But these ones are bold and desperate. Be ready for a fight.[ib:normal2][if:convo_undecided_closed]", null), null, null)
					.Consequence(new ConversationSentence.OnConsequenceDelegate(this.QuestAcceptedConsequences))
					.CloseDialog();
				this.DiscussDialogFlow = DialogFlow.CreateDialogFlow("quest_discuss", 100).NpcLine(new TextObject("{=QNV1b5s5}Are those poachers still in business?[ib:normal2][if:convo_undecided_open]", null), null, null).Condition(new ConversationSentence.OnConditionDelegate(this.DialogCondition))
					.BeginPlayerOptions()
					.PlayerOption(new TextObject("{=JhJBBWab}They will be gone soon.", null), null)
					.NpcLine(new TextObject("{=gjGb044I}I hope they will be...[ib:normal2][if:convo_dismayed]", null), null, null)
					.CloseDialog()
					.PlayerOption(new TextObject("{=Gu3jF88V}Any night battle can easily go wrong. I need more time to prepare.", null), null)
					.NpcLine(new TextObject("{=2EiC1YyZ}Well, if they get wind of what you're up to, things could go very wrong for me. Do be quick.[ib:nervous2][if:convo_dismayed]", null), null, null)
					.CloseDialog()
					.EndPlayerOptions();
				this.QuestCharacterDialogFlow = this.GetPoacherPartyDialogFlow();
			}

			internal void CreatePoachersParty()
			{
				this._poachersParty = MobileParty.CreateParty("poachers_party", null, null);
				TextObject textObject = new TextObject("{=WQa1R55u}Poachers Party", null);
				this._poachersParty.InitializeMobilePartyAtPosition(new TroopRoster(this._poachersParty.Party), new TroopRoster(this._poachersParty.Party), this._questVillage.Settlement.GetPosition2D);
				this._poachersParty.SetCustomName(textObject);
				ItemObject @object = MBObjectManager.Instance.GetObject<ItemObject>("leather");
				int num = MathF.Ceiling(this._difficultyMultiplier * 5f) + MBRandom.RandomInt(0, 2);
				this._poachersParty.ItemRoster.AddToCounts(@object, num * 2);
				CharacterObject characterObject = CharacterObject.All.FirstOrDefault((CharacterObject t) => t.StringId == "poacher");
				int num2 = 10 + MathF.Ceiling(40f * this._difficultyMultiplier);
				this._poachersParty.MemberRoster.AddToCounts(characterObject, num2, false, 0, 0, true, -1);
				this._poachersParty.SetPartyUsedByQuest(true);
				this._poachersParty.Ai.DisableAi();
				Settlement closestHideout = SettlementHelper.FindNearestHideout((Settlement x) => x.IsActive, null);
				Clan clan = Clan.BanditFactions.FirstOrDefaultQ((Clan t) => t.Culture == closestHideout.Culture);
				this._poachersParty.ActualClan = clan;
				EnterSettlementAction.ApplyForParty(this._poachersParty, Settlement.CurrentSettlement);
			}

			private void QuestAcceptedConsequences()
			{
				base.StartQuest();
				base.AddLog(this._questStartedLogText, false);
				base.AddTrackedObject(this._questVillage.Settlement);
			}

			internal void QuestFailedAfterTalkingWithPoachers()
			{
				base.AddLog(this._questFailedAfterTalkingWithProachers, false);
				TraitLevelingHelper.OnIssueFailed(base.QuestGiver, new Tuple<TraitObject, int>[]
				{
					new Tuple<TraitObject, int>(DefaultTraits.Honor, -50),
					new Tuple<TraitObject, int>(DefaultTraits.Mercy, 20)
				});
				this.RelationshipChangeWithQuestGiver = -5;
				base.QuestGiver.AddPower(-50f);
				base.QuestGiver.CurrentSettlement.Town.Security -= 5f;
				base.QuestGiver.CurrentSettlement.Town.Prosperity -= 30f;
				base.CompleteQuestWithFail(null);
			}

			internal void QuestSuccessPlayerComesToAnAgreementWithPoachers()
			{
				base.AddLog(this._questSuccessPlayerComesToAnAgreementWithPoachers, false);
				TraitLevelingHelper.OnIssueSolvedThroughQuest(base.QuestGiver, new Tuple<TraitObject, int>[]
				{
					new Tuple<TraitObject, int>(DefaultTraits.Honor, 10),
					new Tuple<TraitObject, int>(DefaultTraits.Mercy, 50)
				});
				GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, this._rewardGold, false);
				this.RelationshipChangeWithQuestGiver = 5;
				GainRenownAction.Apply(Hero.MainHero, 1f, false);
				base.QuestGiver.AddPower(30f);
				base.QuestGiver.CurrentSettlement.Town.Security -= 5f;
				base.QuestGiver.CurrentSettlement.Town.Prosperity += 50f;
				base.CompleteQuestWithSuccess();
			}

			internal void QuestFailWithPlayerDefeatedAgainstPoachers()
			{
				base.AddLog(this._questFailWithPlayerDefeatedAgainstPoachers, false);
				TraitLevelingHelper.OnIssueSolvedThroughQuest(base.QuestGiver, new Tuple<TraitObject, int>[]
				{
					new Tuple<TraitObject, int>(DefaultTraits.Honor, -30)
				});
				this.RelationshipChangeWithQuestGiver = -5;
				base.QuestGiver.AddPower(-50f);
				base.QuestGiver.CurrentSettlement.Town.Security -= 5f;
				base.QuestGiver.CurrentSettlement.Town.Prosperity -= 30f;
				base.CompleteQuestWithFail(null);
			}

			internal void QuestSuccessWithPlayerDefeatedPoachers()
			{
				base.AddLog(this._questSuccessWithPlayerDefeatedPoachers, false);
				TraitLevelingHelper.OnIssueSolvedThroughQuest(base.QuestGiver, new Tuple<TraitObject, int>[]
				{
					new Tuple<TraitObject, int>(DefaultTraits.Honor, 50)
				});
				GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, this._rewardGold, false);
				this.RelationshipChangeWithQuestGiver = 5;
				base.QuestGiver.AddPower(30f);
				base.QuestGiver.CurrentSettlement.Town.Prosperity += 50f;
				base.CompleteQuestWithSuccess();
			}

			protected override void OnTimedOut()
			{
				base.AddLog(this._questFailedWithTimeOutLogText, false);
				TraitLevelingHelper.OnIssueSolvedThroughQuest(base.QuestGiver, new Tuple<TraitObject, int>[]
				{
					new Tuple<TraitObject, int>(DefaultTraits.Honor, -30)
				});
				this.RelationshipChangeWithQuestGiver = -5;
				base.QuestGiver.AddPower(-50f);
				base.QuestGiver.CurrentSettlement.Town.Prosperity -= 30f;
				base.QuestGiver.CurrentSettlement.Town.Security -= 5f;
			}

			private void QuestCanceledTargetVillageRaided()
			{
				base.AddLog(this._questCanceledTargetVillageRaided, false);
				base.CompleteQuestWithFail(null);
			}

			protected override void RegisterEvents()
			{
				CampaignEvents.MapEventEnded.AddNonSerializedListener(this, new Action<MapEvent>(this.MapEventCheck));
				CampaignEvents.MapEventStarted.AddNonSerializedListener(this, new Action<MapEvent, PartyBase, PartyBase>(this.MapEventStarted));
				CampaignEvents.GameMenuOpened.AddNonSerializedListener(this, new Action<MenuCallbackArgs>(this.GameMenuOpened));
				CampaignEvents.WarDeclared.AddNonSerializedListener(this, new Action<IFaction, IFaction, DeclareWarAction.DeclareWarDetail>(this.OnWarDeclared));
				CampaignEvents.OnClanChangedKingdomEvent.AddNonSerializedListener(this, new Action<Clan, Kingdom, Kingdom, ChangeKingdomAction.ChangeKingdomActionDetail, bool>(this.OnClanChangedKingdom));
				CampaignEvents.CanHeroBecomePrisonerEvent.AddNonSerializedListener(this, new ReferenceAction<Hero, bool>(this.OnCanHeroBecomePrisonerInfoIsRequested));
			}

			private void OnCanHeroBecomePrisonerInfoIsRequested(Hero hero, ref bool result)
			{
				if (hero == Hero.MainHero && this._isReadyToBeFinalized)
				{
					result = false;
				}
			}

			protected override void HourlyTick()
			{
				if (PlayerEncounter.Current != null && PlayerEncounter.Current.IsPlayerWaiting && PlayerEncounter.EncounterSettlement == this._questVillage.Settlement && CampaignTime.Now.IsNightTime && !this._isReadyToBeFinalized && base.IsOngoing)
				{
					EnterSettlementAction.ApplyForParty(MobileParty.MainParty, this._questVillage.Settlement);
					GameMenu.SwitchToMenu("army_of_poachers_village");
				}
			}

			private void GameMenuOpened(MenuCallbackArgs obj)
			{
				if (obj.MenuContext.GameMenu.StringId == "village" && CampaignTime.Now.IsNightTime && Settlement.CurrentSettlement == this._questVillage.Settlement && !this._isReadyToBeFinalized)
				{
					GameMenu.SwitchToMenu("army_of_poachers_village");
				}
				if (obj.MenuContext.GameMenu.StringId == "army_of_poachers_village" && this._isReadyToBeFinalized && MapEvent.PlayerMapEvent != null && MapEvent.PlayerMapEvent.HasWinner && this._poachersParty != null)
				{
					this._poachersParty.IsVisible = false;
				}
			}

			private void MapEventStarted(MapEvent mapEvent, PartyBase attackerParty, PartyBase defenderParty)
			{
				this.MapEventCheck(mapEvent);
				if (QuestHelper.CheckMinorMajorCoercion(this, mapEvent, attackerParty))
				{
					QuestHelper.ApplyGenericMinorMajorCoercionConsequences(this, mapEvent);
				}
			}

			private void MapEventCheck(MapEvent mapEvent)
			{
				if (mapEvent.IsRaid && mapEvent.MapEventSettlement == this._questVillage.Settlement)
				{
					this.QuestCanceledTargetVillageRaided();
				}
			}

			private void OnClanChangedKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail detail, bool showNotification = true)
			{
				if (base.QuestGiver.CurrentSettlement.MapFaction.IsAtWarWith(Hero.MainHero.MapFaction))
				{
					base.CompleteQuestWithCancel(this._questCanceledWarDeclared);
				}
			}

			private void OnWarDeclared(IFaction faction1, IFaction faction2, DeclareWarAction.DeclareWarDetail detail)
			{
				QuestHelper.CheckWarDeclarationAndFailOrCancelTheQuest(this, faction1, faction2, detail, this._playerDeclaredWarQuestLogText, this._questCanceledWarDeclared, false);
			}

			protected override void OnFinalize()
			{
				if (this._poachersParty != null && this._poachersParty.IsActive)
				{
					DestroyPartyAction.Apply(null, this._poachersParty);
				}
				if (Hero.MainHero.IsPrisoner)
				{
					EndCaptivityAction.ApplyByPeace(Hero.MainHero, null);
				}
			}

			protected override void InitializeQuestOnGameLoad()
			{
				this.SetDialogs();
			}

			[SaveableField(10)]
			internal MobileParty _poachersParty;

			[SaveableField(20)]
			internal Village _questVillage;

			[SaveableField(30)]
			internal bool _talkedToPoachersBattleWillStart;

			[SaveableField(40)]
			internal bool _isReadyToBeFinalized;

			[SaveableField(50)]
			internal bool _persuasionTriedOnce;

			[SaveableField(60)]
			internal float _difficultyMultiplier;

			[SaveableField(70)]
			internal int _rewardGold;

			private PersuasionTask _task;

			private const PersuasionDifficulty Difficulty = PersuasionDifficulty.MediumHard;
		}

		public class MerchantArmyOfPoachersIssueBehaviorTypeDefiner : SaveableTypeDefiner
		{
			public MerchantArmyOfPoachersIssueBehaviorTypeDefiner()
				: base(800000)
			{
			}

			protected override void DefineClassTypes()
			{
				base.AddClassDefinition(typeof(MerchantArmyOfPoachersIssueBehavior.MerchantArmyOfPoachersIssue), 1, null);
				base.AddClassDefinition(typeof(MerchantArmyOfPoachersIssueBehavior.MerchantArmyOfPoachersIssueQuest), 2, null);
			}
		}
	}
}
