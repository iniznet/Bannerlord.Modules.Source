﻿using System;
using System.Collections.Generic;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.Issues
{
	public class GangLeaderNeedsRecruitsIssueBehavior : CampaignBehaviorBase
	{
		public override void RegisterEvents()
		{
			CampaignEvents.OnCheckForIssueEvent.AddNonSerializedListener(this, new Action<Hero>(this.OnCheckForIssue));
		}

		public override void SyncData(IDataStore dataStore)
		{
		}

		private static bool ConditionsHold(Hero issueGiver)
		{
			return issueGiver.CurrentSettlement != null && issueGiver.IsGangLeader;
		}

		public void OnCheckForIssue(Hero hero)
		{
			if (GangLeaderNeedsRecruitsIssueBehavior.ConditionsHold(hero))
			{
				Campaign.Current.IssueManager.AddPotentialIssueData(hero, new PotentialIssueData(new PotentialIssueData.StartIssueDelegate(GangLeaderNeedsRecruitsIssueBehavior.OnSelected), typeof(GangLeaderNeedsRecruitsIssueBehavior.GangLeaderNeedsRecruitsIssue), IssueBase.IssueFrequency.VeryCommon, null));
				return;
			}
			Campaign.Current.IssueManager.AddPotentialIssueData(hero, new PotentialIssueData(typeof(GangLeaderNeedsRecruitsIssueBehavior.GangLeaderNeedsRecruitsIssue), IssueBase.IssueFrequency.VeryCommon));
		}

		private static IssueBase OnSelected(in PotentialIssueData pid, Hero issueOwner)
		{
			return new GangLeaderNeedsRecruitsIssueBehavior.GangLeaderNeedsRecruitsIssue(issueOwner);
		}

		private const IssueBase.IssueFrequency GangLeaderNeedsRecruitsIssueFrequency = IssueBase.IssueFrequency.VeryCommon;

		public class GangLeaderNeedsRecruitsIssue : IssueBase
		{
			internal static void AutoGeneratedStaticCollectObjectsGangLeaderNeedsRecruitsIssue(object o, List<object> collectedObjects)
			{
				((GangLeaderNeedsRecruitsIssueBehavior.GangLeaderNeedsRecruitsIssue)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			public override IssueBase.AlternativeSolutionScaleFlag AlternativeSolutionScaleFlags
			{
				get
				{
					return IssueBase.AlternativeSolutionScaleFlag.RequiredTroops;
				}
			}

			private int RequestedRecruitCount
			{
				get
				{
					return 6 + MathF.Ceiling(10f * base.IssueDifficultyMultiplier);
				}
			}

			public override int AlternativeSolutionBaseNeededMenCount
			{
				get
				{
					return 11 + MathF.Ceiling(9f * base.IssueDifficultyMultiplier);
				}
			}

			protected override int AlternativeSolutionBaseDurationInDaysInternal
			{
				get
				{
					return 6 + MathF.Ceiling(7f * base.IssueDifficultyMultiplier);
				}
			}

			protected override int RewardGold
			{
				get
				{
					return 2000 + this.RequestedRecruitCount * 100;
				}
			}

			public override TextObject IssueBriefByIssueGiver
			{
				get
				{
					return new TextObject("{=YxtiyxSf}Yes... As you no doubt know, this is rough work, and I've lost a lot of good lads recently. I haven't had much luck replacing them. I need men who understand how things work in our business, and that's not always easy to find. I could use bandits and looters. They usually know their stuff. But if I take them in as prisoners, they'll just slip away as soon as I get the chance. I need volunteers...", null);
				}
			}

			public override TextObject IssueAcceptByPlayer
			{
				get
				{
					return new TextObject("{=jGpBZDvC}I see. What do you want from me?", null);
				}
			}

			public override TextObject IssueQuestSolutionExplanationByIssueGiver
			{
				get
				{
					return new TextObject("{=Qh26ReAv}Look, I know that warriors like you can sometimes recruit bandits to your party. Some of those men might want to take their chances working for me. More comfortable in living in town, where there's always drink and women on hand, then roaming endlessly about the countryside, eh? For each one that signs up with me I'll give you a bounty, more if they have some experience.", null);
				}
			}

			public override TextObject IssueQuestSolutionAcceptByPlayer
			{
				get
				{
					return new TextObject("{=ekLDmgS7}I'll find your recruits.", null);
				}
			}

			public override TextObject IssueAlternativeSolutionExplanationByIssueGiver
			{
				get
				{
					return new TextObject("{=bKfaMFVK}You can also send me a recruiter: a trustworthy companion who is good at leading men, and also enough of a rogue to win the trust of other rogues...", null);
				}
			}

			public override TextObject IssueAlternativeSolutionAcceptByPlayer
			{
				get
				{
					return new TextObject("{=kxvnA811}All right, I will send you someone from my party who fits your bill.", null);
				}
			}

			public override TextObject IssueAlternativeSolutionResponseByIssueGiver
			{
				get
				{
					return new TextObject("{=8sDjwsnW}I'm sure your lieutenant will solve my problem. Thank you for your help.", null);
				}
			}

			public override TextObject IssueDiscussAlternativeSolution
			{
				get
				{
					return new TextObject("{=TkvsBd4H}Your companion seems to have a knack with the local never-do-wells. I hear a lot of fine lads have already signed up.", null);
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
					TextObject textObject = new TextObject("{=wX14wxqF}You asked {COMPANION.LINK} to deliver at least {WANTED_RECRUIT_AMOUNT} looters and bandits to {ISSUE_GIVER.LINK} in {SETTLEMENT}. They should rejoin your party in {RETURN_DAYS} days.", null);
					textObject.SetCharacterProperties("ISSUE_GIVER", base.IssueOwner.CharacterObject, false);
					textObject.SetCharacterProperties("COMPANION", base.AlternativeSolutionHero.CharacterObject, false);
					textObject.SetTextVariable("WANTED_RECRUIT_AMOUNT", this.RequestedRecruitCount);
					textObject.SetTextVariable("SETTLEMENT", base.IssueOwner.CurrentSettlement.EncyclopediaLinkWithName);
					textObject.SetTextVariable("RETURN_DAYS", base.GetTotalAlternativeSolutionDurationInDays());
					return textObject;
				}
			}

			public override TextObject Title
			{
				get
				{
					return new TextObject("{=rrh7rSLs}Gang Needs Recruits", null);
				}
			}

			public override TextObject Description
			{
				get
				{
					TextObject textObject = new TextObject("{=0kYaAb7c}A gang leader needs recruits for {?ISSUE_GIVER.GENDER}her{?}his{\\?} gang.", null);
					textObject.SetCharacterProperties("ISSUE_GIVER", base.IssueOwner.CharacterObject, false);
					return textObject;
				}
			}

			public GangLeaderNeedsRecruitsIssue(Hero issueOwner)
				: base(issueOwner, CampaignTime.DaysFromNow(30f))
			{
			}

			protected override float GetIssueEffectAmountInternal(IssueEffect issueEffect)
			{
				if (issueEffect == DefaultIssueEffects.IssueOwnerPower)
				{
					return -0.1f;
				}
				return 0f;
			}

			public override ValueTuple<SkillObject, int> GetAlternativeSolutionSkill(Hero hero)
			{
				return new ValueTuple<SkillObject, int>((hero.GetSkillValue(DefaultSkills.Leadership) >= hero.GetSkillValue(DefaultSkills.Roguery)) ? DefaultSkills.Leadership : DefaultSkills.Roguery, 120);
			}

			public override bool DoTroopsSatisfyAlternativeSolution(TroopRoster troopRoster, out TextObject explanation)
			{
				explanation = TextObject.Empty;
				return QuestHelper.CheckRosterForAlternativeSolution(troopRoster, base.GetTotalAlternativeSolutionNeededMenCount(), ref explanation, 2, false);
			}

			public override bool AlternativeSolutionCondition(out TextObject explanation)
			{
				explanation = TextObject.Empty;
				return QuestHelper.CheckRosterForAlternativeSolution(MobileParty.MainParty.MemberRoster, base.GetTotalAlternativeSolutionNeededMenCount(), ref explanation, 2, false);
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
				return new GangLeaderNeedsRecruitsIssueBehavior.GangLeaderNeedsRecruitsIssueQuest(questId, base.IssueOwner, CampaignTime.DaysFromNow(30f), this.RequestedRecruitCount);
			}

			public override IssueBase.IssueFrequency GetFrequency()
			{
				return IssueBase.IssueFrequency.VeryCommon;
			}

			protected override bool CanPlayerTakeQuestConditions(Hero issueGiver, out IssueBase.PreconditionFlags flag, out Hero relationHero, out SkillObject skill)
			{
				flag = IssueBase.PreconditionFlags.None;
				relationHero = null;
				skill = null;
				if (issueGiver.GetRelationWithPlayer() < -10f)
				{
					flag |= IssueBase.PreconditionFlags.Relation;
					relationHero = issueGiver;
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

			protected override int CompanionSkillRewardXP
			{
				get
				{
					return (int)(500f + 700f * base.IssueDifficultyMultiplier);
				}
			}

			protected override void AlternativeSolutionEndWithSuccessConsequence()
			{
				TraitLevelingHelper.OnIssueSolvedThroughQuest(base.IssueOwner, new Tuple<TraitObject, int>[]
				{
					new Tuple<TraitObject, int>(DefaultTraits.Honor, 30)
				});
				base.IssueOwner.AddPower(10f);
				this.RelationshipChangeWithIssueOwner = 5;
			}

			private const int IssueAndQuestDuration = 30;

			private const int AlternativeSolutionTroopTierRequirement = 2;

			private const int AlternativeSolutionRelationBonus = 5;

			private const int AlternativeSolutionNotablePowerBonus = 10;

			private const int AlternativeSolutionPlayerHonorBonus = 30;

			private const int AlternativeSolutionRewardPerRecruit = 100;

			private const int CompanionRequiredSkillLevel = 120;
		}

		public class GangLeaderNeedsRecruitsIssueQuest : QuestBase
		{
			internal static void AutoGeneratedStaticCollectObjectsGangLeaderNeedsRecruitsIssueQuest(object o, List<object> collectedObjects)
			{
				((GangLeaderNeedsRecruitsIssueBehavior.GangLeaderNeedsRecruitsIssueQuest)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				base.AutoGeneratedInstanceCollectObjects(collectedObjects);
				collectedObjects.Add(this._questProgressLogTest);
			}

			internal static object AutoGeneratedGetMemberValue_requestedRecruitCount(object o)
			{
				return ((GangLeaderNeedsRecruitsIssueBehavior.GangLeaderNeedsRecruitsIssueQuest)o)._requestedRecruitCount;
			}

			internal static object AutoGeneratedGetMemberValue_deliveredRecruitCount(object o)
			{
				return ((GangLeaderNeedsRecruitsIssueBehavior.GangLeaderNeedsRecruitsIssueQuest)o)._deliveredRecruitCount;
			}

			internal static object AutoGeneratedGetMemberValue_rewardGold(object o)
			{
				return ((GangLeaderNeedsRecruitsIssueBehavior.GangLeaderNeedsRecruitsIssueQuest)o)._rewardGold;
			}

			internal static object AutoGeneratedGetMemberValue_playerReachedRequestedAmount(object o)
			{
				return ((GangLeaderNeedsRecruitsIssueBehavior.GangLeaderNeedsRecruitsIssueQuest)o)._playerReachedRequestedAmount;
			}

			internal static object AutoGeneratedGetMemberValue_questProgressLogTest(object o)
			{
				return ((GangLeaderNeedsRecruitsIssueBehavior.GangLeaderNeedsRecruitsIssueQuest)o)._questProgressLogTest;
			}

			public override TextObject Title
			{
				get
				{
					return new TextObject("{=rrh7rSLs}Gang Needs Recruits", null);
				}
			}

			public override bool IsRemainingTimeHidden
			{
				get
				{
					return false;
				}
			}

			private TextObject QuestStartedLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=PZI9Smv3}{QUEST_GIVER.LINK}, a gang leader in {SETTLEMENT}, told you that {?QUEST_GIVER.GENDER}she{?}he{\\?} needs recruits for {?QUEST_GIVER.GENDER}her{?}his{\\?} gang. {?QUEST_GIVER.GENDER}She{?}He{\\?} asked you to recruit {NEEDED_RECRUIT_AMOUNT} looters or bandits into your party, then transfer them to {?QUEST_GIVER.GENDER}her{?}him{\\?}. You will be paid for the recruits depending on their experience.", null);
					textObject.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, false);
					textObject.SetTextVariable("SETTLEMENT", base.QuestGiver.CurrentSettlement.EncyclopediaLinkWithName);
					textObject.SetTextVariable("NEEDED_RECRUIT_AMOUNT", this._requestedRecruitCount);
					return textObject;
				}
			}

			private TextObject QuestSuccessLog
			{
				get
				{
					TextObject textObject = new TextObject("{=3ApJ6LaX}You have transferred the recruits to {QUEST_GIVER.LINK} as promised.", null);
					textObject.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, false);
					return textObject;
				}
			}

			private TextObject QuestFailedWithTimeOutLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=iUmWTmQz}You have failed to deliver enough recruits in time. {QUEST_GIVER.LINK} must be disappointed.", null);
					textObject.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, false);
					return textObject;
				}
			}

			public GangLeaderNeedsRecruitsIssueQuest(string questId, Hero questGiver, CampaignTime duration, int requestedRecruitCount)
				: base(questId, questGiver, duration, 0)
			{
				this._requestedRecruitCount = requestedRecruitCount;
				this._deliveredRecruitCount = 0;
				this._rewardGold = 2000;
				this._playerReachedRequestedAmount = false;
				this.SetDialogs();
				base.InitializeQuestOnCreation();
			}

			private void QuestAcceptedConsequences()
			{
				base.StartQuest();
				base.AddTrackedObject(base.QuestGiver.CurrentSettlement);
				this._questProgressLogTest = base.AddDiscreteLog(this.QuestStartedLogText, new TextObject("{=r8rwl9ZS}Delivered Recruits", null), this._deliveredRecruitCount, this._requestedRecruitCount, null, false);
			}

			protected override void SetDialogs()
			{
				this.OfferDialogFlow = DialogFlow.CreateDialogFlow("issue_classic_quest_start", 100).NpcLine(new TextObject("{=0QuAZ8YO}I'll be waiting. Good luck.", null), null, null).Condition(() => Hero.OneToOneConversationHero == this.QuestGiver)
					.Consequence(new ConversationSentence.OnConsequenceDelegate(this.QuestAcceptedConsequences))
					.CloseDialog();
				TextObject npcDiscussLine = new TextObject("{=!}{GANG_LEADER_NEEDS_RECRUITS_QUEST_NOTABLE_DISCUSS}", null);
				TextObject npcResponseLine = new TextObject("{=!}{GANG_LEADER_NEEDS_RECRUITS_QUEST_NOTABLE_RESPONSE}", null);
				bool changeDialogAfterTransfer = false;
				this.DiscussDialogFlow = DialogFlow.CreateDialogFlow("quest_discuss", 100).BeginNpcOptions().NpcOption(new TextObject("{=BGgDjRcW}I think that's enough. Here is your payment.", null), () => Hero.OneToOneConversationHero == this.QuestGiver && this._playerReachedRequestedAmount, null, null)
					.Consequence(delegate
					{
						this.ApplyQuestSuccessConsequences();
						this.CompleteQuestWithSuccess();
					})
					.CloseDialog()
					.NpcOption(npcDiscussLine, delegate
					{
						if (Hero.OneToOneConversationHero != this.QuestGiver)
						{
							return false;
						}
						if (!changeDialogAfterTransfer)
						{
							npcDiscussLine.SetTextVariable("GANG_LEADER_NEEDS_RECRUITS_QUEST_NOTABLE_DISCUSS", new TextObject("{=1hpeeCJD}Have you found any good men?", null));
							changeDialogAfterTransfer = true;
						}
						else
						{
							npcDiscussLine.SetTextVariable("GANG_LEADER_NEEDS_RECRUITS_QUEST_NOTABLE_DISCUSS", new TextObject("{=ds294zxi}Anything else?", null));
							changeDialogAfterTransfer = false;
						}
						return true;
					}, null, null)
					.BeginPlayerOptions()
					.PlayerOption(new TextObject("{=QbaOoilS}Yes, I have brought you a few men.", null), null)
					.Condition(() => (this.CheckIfThereIsSuitableRecruitInPlayer() && !this._playerReachedRequestedAmount) & changeDialogAfterTransfer)
					.NpcLine(npcResponseLine, null, null)
					.Condition(delegate
					{
						if (this._playerReachedRequestedAmount)
						{
							return false;
						}
						npcResponseLine.SetTextVariable("GANG_LEADER_NEEDS_RECRUITS_QUEST_NOTABLE_RESPONSE", new TextObject("{=70LnOZzo}Very good. Keep searching. We still need more men.", null));
						return true;
					})
					.Consequence(new ConversationSentence.OnConsequenceDelegate(this.OpenRecruitDeliveryScreen))
					.PlayerLine(new TextObject("{=IULW8h03}Sure.", null), null)
					.Consequence(delegate
					{
						if (this._playerReachedRequestedAmount && Campaign.Current.ConversationManager.IsConversationInProgress)
						{
							Campaign.Current.ConversationManager.ContinueConversation();
						}
					})
					.GotoDialogState("quest_discuss")
					.PlayerOption(new TextObject("{=PZqGagXt}No, not yet. I'm still looking for them.", null), null)
					.Condition(() => !this._playerReachedRequestedAmount & changeDialogAfterTransfer)
					.Consequence(delegate
					{
						changeDialogAfterTransfer = false;
					})
					.NpcLine(new TextObject("{=L1JyetPq}I am glad to hear that.", null), null, null)
					.CloseDialog()
					.PlayerOption(new TextObject("{=OlOhuO7X}No thank you. Good day to you.", null), null)
					.Condition(() => !this._playerReachedRequestedAmount && !changeDialogAfterTransfer)
					.CloseDialog()
					.EndPlayerOptions()
					.CloseDialog()
					.EndNpcOptions();
			}

			private void OpenRecruitDeliveryScreen()
			{
				PartyScreenManager.OpenScreenWithCondition(new IsTroopTransferableDelegate(this.IsTroopTransferable), new PartyPresentationDoneButtonConditionDelegate(this.DoneButtonCondition), new PartyPresentationDoneButtonDelegate(this.DoneClicked), null, PartyScreenLogic.TransferState.Transferable, PartyScreenLogic.TransferState.NotTransferable, base.QuestGiver.Name, this._requestedRecruitCount - this._deliveredRecruitCount, false, false, PartyScreenMode.TroopsManage);
			}

			private Tuple<bool, TextObject> DoneButtonCondition(TroopRoster leftMemberRoster, TroopRoster leftPrisonRoster, TroopRoster rightMemberRoster, TroopRoster rightPrisonRoster, int leftLimitNum, int rightLimitNum)
			{
				if (this._requestedRecruitCount - this._deliveredRecruitCount < leftMemberRoster.TotalManCount)
				{
					int num = this._requestedRecruitCount - this._deliveredRecruitCount;
					TextObject textObject = new TextObject("{=VOr3uoRZ}You can only transfer {X} recruit{?IS_PLURAL}s{?}{\\?}.", null);
					textObject.SetTextVariable("IS_PLURAL", (num > 1) ? 1 : 0);
					textObject.SetTextVariable("X", num);
					return new Tuple<bool, TextObject>(false, textObject);
				}
				return new Tuple<bool, TextObject>(true, null);
			}

			private bool DoneClicked(TroopRoster leftMemberRoster, TroopRoster leftPrisonRoster, TroopRoster rightMemberRoster, TroopRoster rightPrisonRoster, FlattenedTroopRoster takenPrisonerRoster, FlattenedTroopRoster releasedPrisonerRoster, bool isForced, PartyBase leftParty, PartyBase rightParty)
			{
				foreach (TroopRosterElement troopRosterElement in leftMemberRoster.GetTroopRoster())
				{
					this._rewardGold += this.RewardForEachRecruit(troopRosterElement.Character) * troopRosterElement.Number;
					this._deliveredRecruitCount += troopRosterElement.Number;
				}
				this._questProgressLogTest.UpdateCurrentProgress(this._deliveredRecruitCount);
				this._questProgressLogTest.TaskName.SetTextVariable("TOTAL_REWARD", this._rewardGold);
				if (this._deliveredRecruitCount == this._requestedRecruitCount)
				{
					this._playerReachedRequestedAmount = true;
					if (Campaign.Current.ConversationManager.IsConversationInProgress)
					{
						Campaign.Current.ConversationManager.ContinueConversation();
					}
				}
				return true;
			}

			private int RewardForEachRecruit(CharacterObject recruit)
			{
				return (int)(100f * ((recruit.Tier <= 1) ? 1f : ((recruit.Tier <= 3) ? 1.5f : 2f)));
			}

			private bool IsTroopTransferable(CharacterObject character, PartyScreenLogic.TroopType type, PartyScreenLogic.PartyRosterSide side, PartyBase leftOwnerParty)
			{
				return this._requestedRecruitCount - this._deliveredRecruitCount >= 0 && (side == PartyScreenLogic.PartyRosterSide.Left || (MobileParty.MainParty.MemberRoster.Contains(character) && character.Occupation == Occupation.Bandit));
			}

			private bool CheckIfThereIsSuitableRecruitInPlayer()
			{
				bool flag = false;
				using (List<TroopRosterElement>.Enumerator enumerator = MobileParty.MainParty.MemberRoster.GetTroopRoster().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.Character.Occupation == Occupation.Bandit)
						{
							flag = true;
							break;
						}
					}
				}
				return flag;
			}

			private void ApplyQuestSuccessConsequences()
			{
				base.AddLog(this.QuestSuccessLog, false);
				TraitLevelingHelper.OnIssueSolvedThroughQuest(base.QuestGiver, new Tuple<TraitObject, int>[]
				{
					new Tuple<TraitObject, int>(DefaultTraits.Honor, 30)
				});
				GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, this._rewardGold, false);
				base.QuestGiver.AddPower(10f);
				this.RelationshipChangeWithQuestGiver = 5;
			}

			protected override void OnBeforeTimedOut(ref bool completeWithSuccess, ref bool doNotResolveTheQuest)
			{
				if (this._deliveredRecruitCount >= this._requestedRecruitCount)
				{
					completeWithSuccess = true;
					this.ApplyQuestSuccessConsequences();
				}
			}

			protected override void OnTimedOut()
			{
				base.AddLog(this.QuestFailedWithTimeOutLogText, false);
				base.QuestGiver.AddPower(10f);
				this.RelationshipChangeWithQuestGiver = -5;
			}

			protected override void InitializeQuestOnGameLoad()
			{
				this.SetDialogs();
			}

			private const int QuestGiverRelationBonusOnSuccess = 5;

			private const int QuestGiverNotablePowerBonusOnSuccess = 10;

			private const int QuestGiverRelationPenaltyOnFail = -5;

			private const int NotablePowerPenaltyOnFail = -10;

			private const int PlayerHonorBonusOnSuccess = 30;

			[SaveableField(1)]
			private int _requestedRecruitCount;

			[SaveableField(5)]
			private int _deliveredRecruitCount;

			[SaveableField(6)]
			private int _rewardGold;

			[SaveableField(9)]
			private bool _playerReachedRequestedAmount;

			[SaveableField(7)]
			private JournalLog _questProgressLogTest;
		}

		public class GangLeaderNeedsRecruitsIssueBehaviorTypeDefiner : SaveableTypeDefiner
		{
			public GangLeaderNeedsRecruitsIssueBehaviorTypeDefiner()
				: base(820000)
			{
			}

			protected override void DefineClassTypes()
			{
				base.AddClassDefinition(typeof(GangLeaderNeedsRecruitsIssueBehavior.GangLeaderNeedsRecruitsIssue), 1, null);
				base.AddClassDefinition(typeof(GangLeaderNeedsRecruitsIssueBehavior.GangLeaderNeedsRecruitsIssueQuest), 2, null);
			}
		}
	}
}
