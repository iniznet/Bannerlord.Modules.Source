﻿using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.Issues
{
	public class NearbyBanditBaseIssueBehavior : CampaignBehaviorBase
	{
		private Settlement FindSuitableHideout(Hero issueOwner)
		{
			Settlement settlement = null;
			float num = float.MaxValue;
			foreach (Hideout hideout in Campaign.Current.AllHideouts.Where((Hideout t) => t.IsInfested))
			{
				float num2 = hideout.Settlement.GatePosition.DistanceSquared(issueOwner.GetMapPoint().Position2D);
				if (num2 <= 1225f && num2 < num)
				{
					num = num2;
					settlement = hideout.Settlement;
				}
			}
			return settlement;
		}

		private void OnCheckForIssue(Hero hero)
		{
			if (hero.IsNotable)
			{
				Settlement settlement = this.FindSuitableHideout(hero);
				if (this.ConditionsHold(hero) && settlement != null)
				{
					Campaign.Current.IssueManager.AddPotentialIssueData(hero, new PotentialIssueData(new PotentialIssueData.StartIssueDelegate(this.OnIssueSelected), typeof(NearbyBanditBaseIssueBehavior.NearbyBanditBaseIssue), IssueBase.IssueFrequency.VeryCommon, settlement));
					return;
				}
				Campaign.Current.IssueManager.AddPotentialIssueData(hero, new PotentialIssueData(typeof(NearbyBanditBaseIssueBehavior.NearbyBanditBaseIssue), IssueBase.IssueFrequency.VeryCommon));
			}
		}

		private IssueBase OnIssueSelected(in PotentialIssueData pid, Hero issueOwner)
		{
			PotentialIssueData potentialIssueData = pid;
			return new NearbyBanditBaseIssueBehavior.NearbyBanditBaseIssue(issueOwner, potentialIssueData.RelatedObject as Settlement);
		}

		private bool ConditionsHold(Hero issueGiver)
		{
			return issueGiver.IsHeadman && issueGiver.CurrentSettlement != null && issueGiver.CurrentSettlement.Village.Bound.Town.Security <= 50f;
		}

		private void OnIssueUpdated(IssueBase issue, IssueBase.IssueUpdateDetails details, Hero issueSolver = null)
		{
			NearbyBanditBaseIssueBehavior.NearbyBanditBaseIssue nearbyBanditBaseIssue;
			if ((nearbyBanditBaseIssue = issue as NearbyBanditBaseIssueBehavior.NearbyBanditBaseIssue) != null && details == IssueBase.IssueUpdateDetails.IssueFinishedByAILord)
			{
				foreach (MobileParty mobileParty in nearbyBanditBaseIssue.TargetHideout.Parties)
				{
					mobileParty.Ai.SetMovePatrolAroundSettlement(nearbyBanditBaseIssue.TargetHideout);
				}
			}
		}

		public override void RegisterEvents()
		{
			CampaignEvents.OnCheckForIssueEvent.AddNonSerializedListener(this, new Action<Hero>(this.OnCheckForIssue));
			CampaignEvents.OnIssueUpdatedEvent.AddNonSerializedListener(this, new Action<IssueBase, IssueBase.IssueUpdateDetails, Hero>(this.OnIssueUpdated));
		}

		public override void SyncData(IDataStore dataStore)
		{
		}

		private const int NearbyHideoutMaxRange = 35;

		private const IssueBase.IssueFrequency NearbyHideoutIssueFrequency = IssueBase.IssueFrequency.VeryCommon;

		public class NearbyBanditBaseIssue : IssueBase
		{
			internal static void AutoGeneratedStaticCollectObjectsNearbyBanditBaseIssue(object o, List<object> collectedObjects)
			{
				((NearbyBanditBaseIssueBehavior.NearbyBanditBaseIssue)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				base.AutoGeneratedInstanceCollectObjects(collectedObjects);
				collectedObjects.Add(this._targetHideout);
				collectedObjects.Add(this._issueSettlement);
			}

			internal static object AutoGeneratedGetMemberValue_targetHideout(object o)
			{
				return ((NearbyBanditBaseIssueBehavior.NearbyBanditBaseIssue)o)._targetHideout;
			}

			internal static object AutoGeneratedGetMemberValue_issueSettlement(object o)
			{
				return ((NearbyBanditBaseIssueBehavior.NearbyBanditBaseIssue)o)._issueSettlement;
			}

			public override IssueBase.AlternativeSolutionScaleFlag AlternativeSolutionScaleFlags
			{
				get
				{
					return IssueBase.AlternativeSolutionScaleFlag.Casualties | IssueBase.AlternativeSolutionScaleFlag.FailureRisk;
				}
			}

			public override int AlternativeSolutionBaseNeededMenCount
			{
				get
				{
					return 10;
				}
			}

			protected override int AlternativeSolutionBaseDurationInDaysInternal
			{
				get
				{
					return 4 + MathF.Ceiling(6f * base.IssueDifficultyMultiplier);
				}
			}

			protected override int RewardGold
			{
				get
				{
					return 3000;
				}
			}

			internal Settlement TargetHideout
			{
				get
				{
					return this._targetHideout;
				}
			}

			public override TextObject IssueBriefByIssueGiver
			{
				get
				{
					return new TextObject("{=vw2Q9jJH}There's this old ruin, a place that offers a good view of the roads, and is yet hard to reach. Needless to say, it attracts bandits. A new gang has moved in and they have been giving hell to the caravans and travellers passing by.", null);
				}
			}

			public override TextObject IssueAcceptByPlayer
			{
				get
				{
					return new TextObject("{=IqH0jFdK}So you need someone to deal with these bastards?", null);
				}
			}

			public override TextObject IssueQuestSolutionExplanationByIssueGiver
			{
				get
				{
					return new TextObject("{=zstiYI49}Any bandits there can easily spot and evade a large army moving against them, but if you can enter the hideout with a small group of determined warriors you can catch them unaware.", null);
				}
			}

			public override TextObject IssueQuestSolutionAcceptByPlayer
			{
				get
				{
					return new TextObject("{=uhYprSnG}I will go to the hideout myself and ambush the bandits.", null);
				}
			}

			protected override int CompanionSkillRewardXP
			{
				get
				{
					return (int)(1000f + 1250f * base.IssueDifficultyMultiplier);
				}
			}

			public override bool CanBeCompletedByAI()
			{
				return Hero.MainHero.PartyBelongedToAsPrisoner != this._targetHideout.Party;
			}

			public override TextObject IssueAlternativeSolutionAcceptByPlayer
			{
				get
				{
					TextObject textObject = new TextObject("{=IFasMslv}I will assign a companion with {TROOP_COUNT} good men for {RETURN_DAYS} days.", null);
					textObject.SetTextVariable("TROOP_COUNT", base.GetTotalAlternativeSolutionNeededMenCount());
					textObject.SetTextVariable("RETURN_DAYS", base.GetTotalAlternativeSolutionDurationInDays());
					return textObject;
				}
			}

			public override TextObject IssueDiscussAlternativeSolution
			{
				get
				{
					return new TextObject("{=DgVU7owN}I pray for your warriors. The people here will be very glad to hear of their success.", null);
				}
			}

			public override TextObject IssueAlternativeSolutionResponseByIssueGiver
			{
				get
				{
					TextObject textObject = new TextObject("{=aXOgAKfj}Thank you, {?PLAYER.GENDER}madam{?}sir{\\?}. I hope your people will be successful.", null);
					StringHelpers.SetCharacterProperties("PLAYER", Hero.MainHero.CharacterObject, textObject, false);
					return textObject;
				}
			}

			public override TextObject IssueAlternativeSolutionExplanationByIssueGiver
			{
				get
				{
					TextObject textObject = new TextObject("{=VNXgZ8mt}Alternatively, if you can assign a companion with {TROOP_COUNT} or so men to this task, they can do the job.", null);
					textObject.SetTextVariable("TROOP_COUNT", base.GetTotalAlternativeSolutionNeededMenCount());
					return textObject;
				}
			}

			public override TextObject IssueAsRumorInSettlement
			{
				get
				{
					TextObject textObject = new TextObject("{=ctgihUte}I hope {QUEST_GIVER.NAME} has a plan to get rid of those bandits.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.IssueOwner.CharacterObject, textObject, false);
					return textObject;
				}
			}

			public override bool IsThereAlternativeSolution
			{
				get
				{
					return true;
				}
			}

			protected override TextObject AlternativeSolutionStartLog
			{
				get
				{
					TextObject textObject = new TextObject("{=G4kpabSf}{ISSUE_GIVER.LINK}, a headman from {ISSUE_SETTLEMENT}, has told you about recent bandit attacks on local villagers and asked you to clear out the outlaws' hideout. You asked {COMPANION.LINK} to take {TROOP_COUNT} of your best men to go and take care of it. They should report back to you in {RETURN_DAYS} days.", null);
					StringHelpers.SetCharacterProperties("PLAYER", Hero.MainHero.CharacterObject, textObject, false);
					StringHelpers.SetCharacterProperties("ISSUE_GIVER", base.IssueOwner.CharacterObject, textObject, false);
					StringHelpers.SetCharacterProperties("COMPANION", base.AlternativeSolutionHero.CharacterObject, textObject, false);
					textObject.SetTextVariable("ISSUE_SETTLEMENT", this._issueSettlement.EncyclopediaLinkWithName);
					textObject.SetTextVariable("TROOP_COUNT", this.AlternativeSolutionSentTroops.TotalManCount - 1);
					textObject.SetTextVariable("RETURN_DAYS", base.GetTotalAlternativeSolutionDurationInDays());
					return textObject;
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
					TextObject textObject = new TextObject("{=ENYbLO8r}Bandit Base Near {SETTLEMENT}", null);
					textObject.SetTextVariable("SETTLEMENT", this._issueSettlement.Name);
					return textObject;
				}
			}

			public override TextObject Description
			{
				get
				{
					TextObject textObject = new TextObject("{=vZ01a4cG}{QUEST_GIVER.LINK} wants you to clear the hideout that attracts more bandits to {?QUEST_GIVER.GENDER}her{?}his{\\?} region.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.IssueOwner.CharacterObject, textObject, false);
					return textObject;
				}
			}

			public override TextObject IssueAlternativeSolutionSuccessLog
			{
				get
				{
					TextObject textObject = new TextObject("{=SN3pjZiK}You received a message from {QUEST_GIVER.LINK}.\n\"Thank you for clearing out that bandits' nest. Please accept these {REWARD}{GOLD_ICON} denars with our gratitude.\"", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.IssueOwner.CharacterObject, textObject, false);
					textObject.SetTextVariable("REWARD", this.RewardGold);
					textObject.SetTextVariable("GOLD_ICON", "{=!}<img src=\"General\\Icons\\Coin@2x\" extend=\"8\">");
					return textObject;
				}
			}

			public override TextObject IssueAlternativeSolutionFailLog
			{
				get
				{
					TextObject textObject = new TextObject("{=qsMnnfQ3}You failed to clear the hideout in time to prevent further attacks. {QUEST_GIVER.LINK} is disappointed.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.IssueOwner.CharacterObject, textObject, false);
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

			public NearbyBanditBaseIssue(Hero issueOwner, Settlement targetHideout)
				: base(issueOwner, CampaignTime.DaysFromNow(15f))
			{
				this._targetHideout = targetHideout;
			}

			protected override float GetIssueEffectAmountInternal(IssueEffect issueEffect)
			{
				if (issueEffect == DefaultIssueEffects.SettlementProsperity)
				{
					return -0.2f;
				}
				if (issueEffect == DefaultIssueEffects.SettlementSecurity)
				{
					return -1f;
				}
				return 0f;
			}

			public override ValueTuple<SkillObject, int> GetAlternativeSolutionSkill(Hero hero)
			{
				int skillValue = hero.GetSkillValue(DefaultSkills.OneHanded);
				int skillValue2 = hero.GetSkillValue(DefaultSkills.TwoHanded);
				int skillValue3 = hero.GetSkillValue(DefaultSkills.Polearm);
				if (skillValue >= skillValue2 && skillValue >= skillValue3)
				{
					return new ValueTuple<SkillObject, int>(DefaultSkills.OneHanded, 120);
				}
				return new ValueTuple<SkillObject, int>((skillValue2 >= skillValue3) ? DefaultSkills.TwoHanded : DefaultSkills.Polearm, 120);
			}

			protected override void AfterIssueCreation()
			{
				this._issueSettlement = base.IssueOwner.CurrentSettlement;
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

			public override bool AlternativeSolutionCondition(out TextObject explanation)
			{
				explanation = TextObject.Empty;
				return QuestHelper.CheckRosterForAlternativeSolution(MobileParty.MainParty.MemberRoster, base.GetTotalAlternativeSolutionNeededMenCount(), ref explanation, 2, false);
			}

			protected override void AlternativeSolutionEndWithSuccessConsequence()
			{
				this.RelationshipChangeWithIssueOwner = 5;
				base.IssueOwner.AddPower(5f);
				this._issueSettlement.Village.Bound.Prosperity += 10f;
				TraitLevelingHelper.OnIssueSolvedThroughAlternativeSolution(base.IssueOwner, new Tuple<TraitObject, int>[]
				{
					new Tuple<TraitObject, int>(DefaultTraits.Honor, 50)
				});
				GainRenownAction.Apply(Hero.MainHero, 1f, false);
			}

			protected override void AlternativeSolutionEndWithFailureConsequence()
			{
				this.RelationshipChangeWithIssueOwner = -5;
				base.IssueOwner.AddPower(-5f);
				this._issueSettlement.Village.Bound.Prosperity += -10f;
			}

			protected override void OnGameLoad()
			{
			}

			protected override QuestBase GenerateIssueQuest(string questId)
			{
				return new NearbyBanditBaseIssueBehavior.NearbyBanditBaseIssueQuest(questId, base.IssueOwner, this._targetHideout, this._issueSettlement, this.RewardGold, CampaignTime.DaysFromNow(30f));
			}

			public override IssueBase.IssueFrequency GetFrequency()
			{
				return IssueBase.IssueFrequency.VeryCommon;
			}

			protected override bool CanPlayerTakeQuestConditions(Hero issueGiver, out IssueBase.PreconditionFlags flags, out Hero relationHero, out SkillObject skill)
			{
				flags = IssueBase.PreconditionFlags.None;
				relationHero = null;
				skill = null;
				if (issueGiver.GetRelationWithPlayer() < -10f)
				{
					flags |= IssueBase.PreconditionFlags.Relation;
					relationHero = issueGiver;
				}
				if (FactionManager.IsAtWarAgainstFaction(issueGiver.MapFaction, Hero.MainHero.MapFaction))
				{
					flags |= IssueBase.PreconditionFlags.AtWar;
				}
				return flags == IssueBase.PreconditionFlags.None;
			}

			public override bool IssueStayAliveConditions()
			{
				return this._targetHideout.Hideout.IsInfested && base.IssueOwner.CurrentSettlement.IsVillage && !base.IssueOwner.CurrentSettlement.IsRaided && !base.IssueOwner.CurrentSettlement.IsUnderRaid && base.IssueOwner.CurrentSettlement.Village.Bound.Town.Security <= 80f;
			}

			protected override void CompleteIssueWithTimedOutConsequences()
			{
			}

			private const int AlternativeSolutionFinalMenCount = 10;

			private const int AlternativeSolutionMinimumTroopTier = 2;

			private const int AlternativeSolutionCompanionSkillThreshold = 120;

			private const int AlternativeSolutionRelationRewardOnSuccess = 5;

			private const int AlternativeSolutionRelationPenaltyOnFail = -5;

			private const int IssueOwnerPowerBonusOnSuccess = 5;

			private const int IssueOwnerPowerPenaltyOnFail = -5;

			private const int SettlementProsperityBonusOnSuccess = 10;

			private const int SettlementProsperityPenaltyOnFail = -10;

			private const int IssueDuration = 15;

			private const int QuestTimeLimit = 30;

			[SaveableField(100)]
			private readonly Settlement _targetHideout;

			[SaveableField(101)]
			private Settlement _issueSettlement;
		}

		public class NearbyBanditBaseIssueQuest : QuestBase
		{
			internal static void AutoGeneratedStaticCollectObjectsNearbyBanditBaseIssueQuest(object o, List<object> collectedObjects)
			{
				((NearbyBanditBaseIssueBehavior.NearbyBanditBaseIssueQuest)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				base.AutoGeneratedInstanceCollectObjects(collectedObjects);
				collectedObjects.Add(this._targetHideout);
				collectedObjects.Add(this._questSettlement);
			}

			internal static object AutoGeneratedGetMemberValue_targetHideout(object o)
			{
				return ((NearbyBanditBaseIssueBehavior.NearbyBanditBaseIssueQuest)o)._targetHideout;
			}

			internal static object AutoGeneratedGetMemberValue_questSettlement(object o)
			{
				return ((NearbyBanditBaseIssueBehavior.NearbyBanditBaseIssueQuest)o)._questSettlement;
			}

			public override TextObject Title
			{
				get
				{
					TextObject textObject = new TextObject("{=ENYbLO8r}Bandit Base Near {SETTLEMENT}", null);
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

			private TextObject _onQuestStartedLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=ogsh3V6G}{QUEST_GIVER.LINK}, a headman from {QUEST_SETTLEMENT}, has told you about the hideout of some bandits who have recently been attacking local villagers. You told {?QUEST_GIVER.GENDER}her{?}him{\\?} that you will take care of the situation yourself. {QUEST_GIVER.LINK} also marked the location of the hideout on your map.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					textObject.SetTextVariable("QUEST_SETTLEMENT", this._questSettlement.EncyclopediaLinkWithName);
					return textObject;
				}
			}

			private TextObject _onQuestSucceededLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=SN3pjZiK}You received a message from {QUEST_GIVER.LINK}.\n\"Thank you for clearing out that bandits' nest. Please accept these {REWARD}{GOLD_ICON} denars with our gratitude.\"", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					textObject.SetTextVariable("REWARD", this.RewardGold);
					textObject.SetTextVariable("GOLD_ICON", "{=!}<img src=\"General\\Icons\\Coin@2x\" extend=\"8\">");
					return textObject;
				}
			}

			private TextObject _onQuestFailedLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=qsMnnfQ3}You failed to clear the hideout in time to prevent further attacks. {QUEST_GIVER.LINK} is disappointed.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					return textObject;
				}
			}

			private TextObject _onQuestCanceledLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=4Bub0GY6}Hideout was cleared by someone else. Your agreement with {QUEST_GIVER.LINK} is canceled.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					return textObject;
				}
			}

			public NearbyBanditBaseIssueQuest(string questId, Hero questGiver, Settlement targetHideout, Settlement questSettlement, int rewardGold, CampaignTime duration)
				: base(questId, questGiver, duration, rewardGold)
			{
				this._targetHideout = targetHideout;
				this._questSettlement = questSettlement;
				this.SetDialogs();
				base.InitializeQuestOnCreation();
			}

			protected override void InitializeQuestOnGameLoad()
			{
				this.SetDialogs();
			}

			protected override void SetDialogs()
			{
				this.OfferDialogFlow = DialogFlow.CreateDialogFlow("issue_classic_quest_start", 100).NpcLine("{=spj8bYVo}Good! I'll mark the hideout for you on a map.", null, null).Condition(() => Hero.OneToOneConversationHero == base.QuestGiver)
					.Consequence(new ConversationSentence.OnConsequenceDelegate(this.OnQuestAccepted))
					.CloseDialog();
				this.DiscussDialogFlow = DialogFlow.CreateDialogFlow("quest_discuss", 100).NpcLine("{=l9wYpIuV}Any news? Have you managed to clear out the hideout yet?", null, null).Condition(() => Hero.OneToOneConversationHero == base.QuestGiver)
					.BeginPlayerOptions()
					.PlayerOption("{=wErSpkjy}I'm still working on it.", null)
					.NpcLine("{=XTt6gZ7h}Do make haste, if you can. As long as those bandits are up there, no traveller is safe!", null, null)
					.CloseDialog()
					.PlayerOption("{=I8raOMRH}Sorry. No progress yet.", null)
					.NpcLine("{=kWruAXaF}Well... You know as long as those bandits remain there, no traveller is safe.", null, null)
					.CloseDialog()
					.EndPlayerOptions()
					.CloseDialog();
			}

			private void OnQuestAccepted()
			{
				base.StartQuest();
				this._targetHideout.Hideout.IsSpotted = true;
				this._targetHideout.IsVisible = true;
				base.AddTrackedObject(this._targetHideout);
				QuestHelper.AddMapArrowFromPointToTarget(new TextObject("{=xpsQyPaV}Direction to Bandits", null), this._questSettlement.Position2D, this._targetHideout.Position2D, 5f, 0.1f);
				TextObject textObject = new TextObject("{=XGa8MkbJ}{QUEST_GIVER.NAME} has marked the hideout on your map", null);
				StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
				MBInformationManager.AddQuickInformation(textObject, 0, null, "");
				base.AddLog(this._onQuestStartedLogText, false);
			}

			private void OnQuestSucceeded()
			{
				base.AddLog(this._onQuestSucceededLogText, false);
				GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, this.RewardGold, false);
				GainRenownAction.Apply(Hero.MainHero, 1f, false);
				TraitLevelingHelper.OnIssueSolvedThroughQuest(base.QuestGiver, new Tuple<TraitObject, int>[]
				{
					new Tuple<TraitObject, int>(DefaultTraits.Honor, 50)
				});
				base.QuestGiver.AddPower(5f);
				this.RelationshipChangeWithQuestGiver = 5;
				this._questSettlement.Village.Bound.Prosperity += 10f;
				base.CompleteQuestWithSuccess();
			}

			private void OnQuestFailed(bool isTimedOut)
			{
				base.AddLog(this._onQuestFailedLogText, false);
				this.RelationshipChangeWithQuestGiver = -5;
				base.QuestGiver.AddPower(-5f);
				this._questSettlement.Village.Bound.Prosperity += -10f;
				this._questSettlement.Village.Bound.Town.Security += -5f;
				if (!isTimedOut)
				{
					base.CompleteQuestWithFail(null);
				}
			}

			private void OnQuestCanceled()
			{
				base.AddLog(this._onQuestCanceledLogText, false);
				base.CompleteQuestWithFail(null);
			}

			protected override void OnTimedOut()
			{
				this.OnQuestFailed(true);
			}

			protected override void RegisterEvents()
			{
				CampaignEvents.MapEventEnded.AddNonSerializedListener(this, new Action<MapEvent>(this.OnMapEventEnded));
				CampaignEvents.OnHideoutDeactivatedEvent.AddNonSerializedListener(this, new Action<Settlement>(this.OnHideoutCleared));
				CampaignEvents.MapEventStarted.AddNonSerializedListener(this, new Action<MapEvent, PartyBase, PartyBase>(this.OnMapEventStarted));
			}

			private void OnMapEventStarted(MapEvent mapEvent, PartyBase attackerParty, PartyBase defenderParty)
			{
				if (QuestHelper.CheckMinorMajorCoercion(this, mapEvent, attackerParty))
				{
					QuestHelper.ApplyGenericMinorMajorCoercionConsequences(this, mapEvent);
				}
			}

			private void OnHideoutCleared(Settlement hideout)
			{
				if (this._targetHideout == hideout)
				{
					base.CompleteQuestWithCancel(null);
				}
			}

			private void OnMapEventEnded(MapEvent mapEvent)
			{
				if (mapEvent.IsHideoutBattle && mapEvent.MapEventSettlement == this._targetHideout)
				{
					if (mapEvent.InvolvedParties.Contains(PartyBase.MainParty))
					{
						if (mapEvent.BattleState == BattleState.DefenderVictory)
						{
							this.OnQuestFailed(false);
							return;
						}
						if (mapEvent.BattleState == BattleState.AttackerVictory)
						{
							this.OnQuestSucceeded();
							return;
						}
					}
					else if (mapEvent.BattleState == BattleState.AttackerVictory)
					{
						this.OnQuestCanceled();
					}
				}
			}

			private const int QuestGiverRelationBonus = 5;

			private const int QuestGiverRelationPenalty = -5;

			private const int QuestGiverPowerBonus = 5;

			private const int QuestGiverPowerPenalty = -5;

			private const int TownProsperityBonus = 10;

			private const int TownProsperityPenalty = -10;

			private const int TownSecurityPenalty = -5;

			private const int QuestGuid = 1056731;

			[SaveableField(100)]
			private readonly Settlement _targetHideout;

			[SaveableField(101)]
			private readonly Settlement _questSettlement;
		}

		public class NearbyBanditBaseIssueTypeDefiner : SaveableTypeDefiner
		{
			public NearbyBanditBaseIssueTypeDefiner()
				: base(400000)
			{
			}

			protected override void DefineClassTypes()
			{
				base.AddClassDefinition(typeof(NearbyBanditBaseIssueBehavior.NearbyBanditBaseIssue), 1, null);
				base.AddClassDefinition(typeof(NearbyBanditBaseIssueBehavior.NearbyBanditBaseIssueQuest), 2, null);
			}
		}
	}
}