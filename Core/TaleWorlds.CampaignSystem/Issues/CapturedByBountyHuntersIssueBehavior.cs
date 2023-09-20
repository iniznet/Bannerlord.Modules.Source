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
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.Issues
{
	public class CapturedByBountyHuntersIssueBehavior : CampaignBehaviorBase
	{
		public override void RegisterEvents()
		{
			CampaignEvents.OnCheckForIssueEvent.AddNonSerializedListener(this, new Action<Hero>(this.OnCheckForIssue));
		}

		public override void SyncData(IDataStore dataStore)
		{
		}

		private bool ConditionsHold(Hero issueGiver, out Settlement selectedHideout)
		{
			selectedHideout = null;
			if (issueGiver.IsLord || (issueGiver.IsNotable && issueGiver.CurrentSettlement == null))
			{
				return false;
			}
			if (issueGiver.IsGangLeader)
			{
				selectedHideout = this.FindSuitableHideout(issueGiver);
				CharacterObject @object = MBObjectManager.Instance.GetObject<CharacterObject>("looter");
				return selectedHideout != null && @object != null;
			}
			return false;
		}

		private Settlement FindSuitableHideout(Hero issueGiver)
		{
			Settlement settlement = null;
			float num = float.MaxValue;
			foreach (Hideout hideout in Campaign.Current.AllHideouts.Where((Hideout t) => t.IsInfested))
			{
				float num2;
				if (Campaign.Current.Models.MapDistanceModel.GetDistance(issueGiver.GetMapPoint(), hideout.Settlement, (55f < num) ? 55f : num, out num2) && num2 < num)
				{
					num = num2;
					settlement = hideout.Settlement;
				}
			}
			return settlement;
		}

		public void OnCheckForIssue(Hero hero)
		{
			Settlement settlement;
			if (this.ConditionsHold(hero, out settlement))
			{
				Campaign.Current.IssueManager.AddPotentialIssueData(hero, new PotentialIssueData(new PotentialIssueData.StartIssueDelegate(this.OnSelected), typeof(CapturedByBountyHuntersIssueBehavior.CapturedByBountyHuntersIssue), IssueBase.IssueFrequency.Common, settlement));
				return;
			}
			Campaign.Current.IssueManager.AddPotentialIssueData(hero, new PotentialIssueData(typeof(CapturedByBountyHuntersIssueBehavior.CapturedByBountyHuntersIssue), IssueBase.IssueFrequency.Common));
		}

		private IssueBase OnSelected(in PotentialIssueData pid, Hero issueOwner)
		{
			PotentialIssueData potentialIssueData = pid;
			return new CapturedByBountyHuntersIssueBehavior.CapturedByBountyHuntersIssue(issueOwner, potentialIssueData.RelatedObject as Settlement);
		}

		private const IssueBase.IssueFrequency CapturedByBountyHuntersIssueFrequency = IssueBase.IssueFrequency.Common;

		private const float ValidHideoutDistance = 55f;

		public class CapturedByBountyHuntersIssue : IssueBase
		{
			internal static void AutoGeneratedStaticCollectObjectsCapturedByBountyHuntersIssue(object o, List<object> collectedObjects)
			{
				((CapturedByBountyHuntersIssueBehavior.CapturedByBountyHuntersIssue)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				base.AutoGeneratedInstanceCollectObjects(collectedObjects);
				collectedObjects.Add(this._hideout);
			}

			internal static object AutoGeneratedGetMemberValue_hideout(object o)
			{
				return ((CapturedByBountyHuntersIssueBehavior.CapturedByBountyHuntersIssue)o)._hideout;
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
					return Math.Max(5, 4 + MathF.Ceiling(6f * base.IssueDifficultyMultiplier));
				}
			}

			protected override int RewardGold
			{
				get
				{
					return 3000;
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
					return new TextObject("{=QtmPWQ5a}Some of my lads have gone missing. I've got a witness who says they'd gotten themselves dead drunk drinking with another band in these parts who turned out to be filthy bounty hunters. Now my boys are all trussed up, and these treacherous animals aim to turn them in for the bounty.[if:convo_annoyed][ib:closed]", null);
				}
			}

			public override TextObject IssueAcceptByPlayer
			{
				get
				{
					return new TextObject("{=tZqbrlV9}How can I help you?", null);
				}
			}

			public override TextObject IssueQuestSolutionExplanationByIssueGiver
			{
				get
				{
					TextObject textObject = new TextObject("{=MiVYmiBc}Raid the bounty hunters' hideout and rescue my associates from them. I will make it worth your while, say {GOLD_AMOUNT} denars.[if:convo_mocking_revenge][ib:closed2]", null);
					textObject.SetTextVariable("GOLD_AMOUNT", this.RewardGold);
					return textObject;
				}
			}

			public override TextObject IssueAlternativeSolutionExplanationByIssueGiver
			{
				get
				{
					TextObject textObject = new TextObject("{=GIkvhuCC}Maybe one of your men who knows a thing or two about scouting, with {TROOP_AMOUNT} good men can deal with these scum. So what do you say?[if:convo_undecided_open]", null);
					textObject.SetTextVariable("TROOP_AMOUNT", base.GetTotalAlternativeSolutionNeededMenCount());
					return textObject;
				}
			}

			public override TextObject IssueQuestSolutionAcceptByPlayer
			{
				get
				{
					return new TextObject("{=cvWxXGo5}I can do the job.", null);
				}
			}

			public override TextObject IssueAlternativeSolutionAcceptByPlayer
			{
				get
				{
					return new TextObject("{=AvBNKK5y}Alright, I will have one of my companions go and rescue your associates.", null);
				}
			}

			public override TextObject IssueAlternativeSolutionResponseByIssueGiver
			{
				get
				{
					return new TextObject("{=9u9OEZ9Y}Splendid. My men will guide your companion to the hideout.", null);
				}
			}

			public override TextObject IssueDiscussAlternativeSolution
			{
				get
				{
					return new TextObject("{=zwNjgdbi}My boys are getting ready for the battle. I'm pretty sure your men will tip the balance of that fight in our favor. Thank you.", null);
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
					TextObject textObject = new TextObject("{=TQyB9rAs}{ISSUE_OWNER.NAME}'s associates captured by bounty hunters.", null);
					StringHelpers.SetCharacterProperties("ISSUE_OWNER", base.IssueOwner.CharacterObject, textObject, false);
					return textObject;
				}
			}

			public override TextObject Description
			{
				get
				{
					TextObject textObject = new TextObject("{=HhTTzgLj}{ISSUE_OWNER.LINK}, a gang leader in {ISSUE_SETTLEMENT}, wants us to raid some bounty hunters' hideout and rescue {?ISSUE_OWNER.GENDER}her{?}his{\\?} associates.", null);
					StringHelpers.SetCharacterProperties("ISSUE_OWNER", base.IssueOwner.CharacterObject, textObject, false);
					textObject.SetTextVariable("ISSUE_SETTLEMENT", base.IssueOwner.CurrentSettlement.EncyclopediaLinkWithName);
					return textObject;
				}
			}

			public CapturedByBountyHuntersIssue(Hero issueOwner, Settlement hideout)
				: base(issueOwner, CampaignTime.DaysFromNow(15f))
			{
				this._hideout = hideout;
			}

			protected override float GetIssueEffectAmountInternal(IssueEffect issueEffect)
			{
				if (issueEffect == DefaultIssueEffects.SettlementSecurity)
				{
					return 1f;
				}
				if (issueEffect == DefaultIssueEffects.IssueOwnerPower)
				{
					return -0.2f;
				}
				return 0f;
			}

			public override ValueTuple<SkillObject, int> GetAlternativeSolutionSkill(Hero hero)
			{
				return new ValueTuple<SkillObject, int>((hero.GetSkillValue(DefaultSkills.Scouting) >= hero.GetSkillValue(DefaultSkills.Riding)) ? DefaultSkills.Scouting : DefaultSkills.Riding, 120);
			}

			public override bool DoTroopsSatisfyAlternativeSolution(TroopRoster troopRoster, out TextObject explanation)
			{
				explanation = TextObject.Empty;
				return QuestHelper.CheckRosterForAlternativeSolution(troopRoster, base.GetTotalAlternativeSolutionNeededMenCount(), ref explanation, 0, false);
			}

			public override bool AlternativeSolutionCondition(out TextObject explanation)
			{
				explanation = TextObject.Empty;
				return QuestHelper.CheckRosterForAlternativeSolution(MobileParty.MainParty.MemberRoster, base.GetTotalAlternativeSolutionNeededMenCount(), ref explanation, 0, false);
			}

			protected override int CompanionSkillRewardXP
			{
				get
				{
					return (int)(750f + 1000f * base.IssueDifficultyMultiplier);
				}
			}

			protected override void AlternativeSolutionEndWithSuccessConsequence()
			{
				this.RelationshipChangeWithIssueOwner = 5;
				base.IssueOwner.AddPower(10f);
				base.IssueOwner.CurrentSettlement.Town.Security += 5f;
			}

			protected override void AlternativeSolutionEndWithFailureConsequence()
			{
				TraitLevelingHelper.OnIssueFailed(base.IssueOwner, new Tuple<TraitObject, int>[]
				{
					new Tuple<TraitObject, int>(DefaultTraits.Honor, -10)
				});
				this.RelationshipChangeWithIssueOwner = -5;
				base.IssueOwner.AddPower(-10f);
				base.IssueOwner.CurrentSettlement.Town.Security -= 5f;
			}

			protected override TextObject AlternativeSolutionStartLog
			{
				get
				{
					TextObject textObject = new TextObject("{=U7sTASN4}{ISSUE_OWNER.LINK}, a gang leader from {QUEST_SETTLEMENT}, has told you that some bounty hunters captured some of {?ISSUE_OWNER.GENDER}her{?}his{\\?} gang members and are holding them in their hideout. {?ISSUE_OWNER.GENDER}She{?}He{\\?} wants them found and rescued. You agreed to send {TROOP_COUNT} of your men along with a {COMPANION.LINK} to find these bounty hunters and rescue {?ISSUE_OWNER.GENDER}her{?}his{\\?} associates. They should be back in {RETURN_DAYS} days.", null);
					StringHelpers.SetCharacterProperties("ISSUE_OWNER", base.IssueOwner.CharacterObject, textObject, false);
					StringHelpers.SetCharacterProperties("COMPANION", base.AlternativeSolutionHero.CharacterObject, textObject, false);
					textObject.SetTextVariable("QUEST_SETTLEMENT", base.IssueOwner.CurrentSettlement.EncyclopediaLinkWithName);
					textObject.SetTextVariable("RETURN_DAYS", base.GetTotalAlternativeSolutionDurationInDays());
					textObject.SetTextVariable("TROOP_COUNT", this.AlternativeSolutionSentTroops.TotalManCount - 1);
					return textObject;
				}
			}

			public override IssueBase.IssueFrequency GetFrequency()
			{
				return IssueBase.IssueFrequency.Common;
			}

			public override bool IssueStayAliveConditions()
			{
				return this._hideout.Hideout.IsInfested;
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
				return new CapturedByBountyHuntersIssueBehavior.CapturedByBountyHuntersIssueQuest(questId, base.IssueOwner, CampaignTime.DaysFromNow(30f), this.RewardGold, this._hideout);
			}

			protected override void CompleteIssueWithTimedOutConsequences()
			{
			}

			private const int CompanionRequiredSkillLevel = 120;

			private const int IssueDuration = 15;

			private const int QuestTimeLimit = 30;

			[SaveableField(100)]
			private Settlement _hideout;
		}

		public class CapturedByBountyHuntersIssueQuest : QuestBase
		{
			internal static void AutoGeneratedStaticCollectObjectsCapturedByBountyHuntersIssueQuest(object o, List<object> collectedObjects)
			{
				((CapturedByBountyHuntersIssueBehavior.CapturedByBountyHuntersIssueQuest)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				base.AutoGeneratedInstanceCollectObjects(collectedObjects);
				collectedObjects.Add(this._questHideout);
			}

			internal static object AutoGeneratedGetMemberValue_questHideout(object o)
			{
				return ((CapturedByBountyHuntersIssueBehavior.CapturedByBountyHuntersIssueQuest)o)._questHideout;
			}

			public override TextObject Title
			{
				get
				{
					TextObject textObject = new TextObject("{=TQyB9rAs}{ISSUE_OWNER.NAME}'s associates captured by bounty hunters.", null);
					StringHelpers.SetCharacterProperties("ISSUE_OWNER", base.QuestGiver.CharacterObject, textObject, false);
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

			private TextObject _playerStartsQuestLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=P7MZ0hZb}{QUEST_GIVER.LINK}, a gang leader from {QUEST_SETTLEMENT}, has told you that some bounty hunters captured some of {?QUEST_GIVER.GENDER}her{?}his{\\?} gang members and are holding them in their hideout. You told {?QUEST_GIVER.GENDER}her{?}him{\\?} you would find them yourself.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					textObject.SetTextVariable("QUEST_SETTLEMENT", Settlement.CurrentSettlement.EncyclopediaLinkWithName);
					return textObject;
				}
			}

			private TextObject _successQuestLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=rNDRyFP4}You cleared the hideout and rescued the {QUEST_GIVER.LINK}'s associates. {?QUEST_GIVER.GENDER}She{?}He{\\?} sends you the following letter. \"Thank you for rescuing my men. I'll remember this.\"", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					return textObject;
				}
			}

			private TextObject _playerLostTheFightLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=nq0qLQ1x}You lost the fight against bounty hunters and failed to rescue the {QUEST_GIVER.LINK}'s men. {?QUEST_GIVER.GENDER}She{?}He{\\?} sends you the following letter. \"I appreciate your effort but it wasn't good enough...\"", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					return textObject;
				}
			}

			private TextObject _hideoutClearedBySomeoneElseLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=4Bub0GY6}Hideout was cleared by someone else. Your agreement with {QUEST_GIVER.LINK} is canceled.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					return textObject;
				}
			}

			private TextObject _timeOutLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=JPAGzEhe}You failed to rescue the {QUEST_GIVER.LINK}'s men in time. {?QUEST_GIVER.GENDER}She{?}He{\\?} sends you the following letter. \"You sat on your heels doing nothing and my men will pay the price. I won't forget this...\"", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					return textObject;
				}
			}

			public CapturedByBountyHuntersIssueQuest(string questId, Hero giverHero, CampaignTime duration, int rewardGold, Settlement hideout)
				: base(questId, giverHero, duration, rewardGold)
			{
				this._questHideout = hideout;
				this.SetDialogs();
				base.InitializeQuestOnCreation();
			}

			private bool DialogCondition()
			{
				return Hero.OneToOneConversationHero == base.QuestGiver;
			}

			protected override void SetDialogs()
			{
				this.OfferDialogFlow = DialogFlow.CreateDialogFlow("issue_classic_quest_start", 100).NpcLine(new TextObject("{=BUM63VJq}That's the spirit. My men will tell you how to find the hideout. Rescue those poor captives, and a large sack of silver will be on your way![if:convo_approving][ib:hip]", null), null, null).Condition(new ConversationSentence.OnConditionDelegate(this.DialogCondition))
					.Consequence(new ConversationSentence.OnConsequenceDelegate(this.QuestAcceptedConsequences))
					.CloseDialog();
				this.DiscussDialogFlow = DialogFlow.CreateDialogFlow("quest_discuss", 100).NpcLine(new TextObject("{=vYCY931w}Any news about my men?", null), null, null).Condition(new ConversationSentence.OnConditionDelegate(this.DialogCondition))
					.BeginPlayerOptions()
					.PlayerOption(new TextObject("{=DJcMau0U}Not yet. We are still looking for them.", null), null)
					.NpcLine(new TextObject("{=VZhs6rpG}Well, try to speed it up. Once the bounty hunters turn them in, it'll be too late.", null), null, null)
					.CloseDialog()
					.PlayerOption(new TextObject("{=LvNTjCtQ}We need more time.", null), null)
					.NpcLine(new TextObject("{=15wCjIBY}Take too much time, and my men will swing from the gallows. Speed it along, will you?", null), null, null)
					.CloseDialog()
					.EndPlayerOptions();
			}

			private void QuestAcceptedConsequences()
			{
				base.StartQuest();
				base.AddLog(this._playerStartsQuestLogText, false);
			}

			protected override void InitializeQuestOnGameLoad()
			{
				this.SetDialogs();
			}

			protected override void HourlyTick()
			{
			}

			protected override void RegisterEvents()
			{
				CampaignEvents.MapEventEnded.AddNonSerializedListener(this, new Action<MapEvent>(this.OnMapEventEnded));
				CampaignEvents.OnSettlementLeftEvent.AddNonSerializedListener(this, new Action<MobileParty, Settlement>(this.OnSettlementLeft));
			}

			private void OnSettlementLeft(MobileParty party, Settlement settlement)
			{
				if (party == MobileParty.MainParty && settlement == base.QuestGiver.CurrentSettlement)
				{
					this._questHideout.Hideout.IsSpotted = true;
					this._questHideout.IsVisible = true;
					base.AddTrackedObject(this._questHideout);
					TextObject textObject = new TextObject("{=R9R6imnU}Scouts working for {QUEST_GIVER.NAME} marked the hideout on your map", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					MBInformationManager.AddQuickInformation(textObject, 0, null, "");
				}
			}

			private void OnMapEventEnded(MapEvent mapEvent)
			{
				if (mapEvent.IsPlayerMapEvent)
				{
					if (mapEvent.MapEventSettlement == this._questHideout)
					{
						if (mapEvent.DefeatedSide == mapEvent.PlayerSide || mapEvent.DefeatedSide == BattleSideEnum.None)
						{
							base.AddLog(this._playerLostTheFightLogText, false);
							this.FailConsequences(false);
							return;
						}
						base.AddLog(this._successQuestLogText, false);
						this.SuccessConsequences();
						return;
					}
				}
				else if (this._questHideout.Parties.Count == 0)
				{
					base.AddLog(this._hideoutClearedBySomeoneElseLogText, false);
					base.CompleteQuestWithFail(null);
				}
			}

			private void SuccessConsequences()
			{
				GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, this.RewardGold, false);
				base.QuestGiver.AddPower(10f);
				this.RelationshipChangeWithQuestGiver = 5;
				if (base.QuestGiver.CurrentSettlement != null && base.QuestGiver.CurrentSettlement.Town != null)
				{
					base.QuestGiver.CurrentSettlement.Town.Security += 5f;
				}
				TraitLevelingHelper.OnIssueSolvedThroughQuest(base.QuestGiver, new Tuple<TraitObject, int>[]
				{
					new Tuple<TraitObject, int>(DefaultTraits.Honor, 100)
				});
				base.CompleteQuestWithSuccess();
			}

			private void FailConsequences(bool isTimedOut)
			{
				TraitLevelingHelper.OnIssueFailed(base.QuestGiver, new Tuple<TraitObject, int>[]
				{
					new Tuple<TraitObject, int>(DefaultTraits.Honor, -10)
				});
				this.RelationshipChangeWithQuestGiver = -5;
				base.QuestGiver.AddPower(-10f);
				if (base.QuestGiver.CurrentSettlement != null && base.QuestGiver.CurrentSettlement.Town != null)
				{
					base.QuestGiver.CurrentSettlement.Town.Security -= 5f;
				}
				if (!isTimedOut)
				{
					base.CompleteQuestWithFail(null);
				}
			}

			protected override void OnTimedOut()
			{
				base.AddLog(this._timeOutLogText, false);
				this.FailConsequences(true);
			}

			[SaveableField(102)]
			private Settlement _questHideout;
		}

		public class CapturedByBountyHuntersIssueTypeDefiner : SaveableTypeDefiner
		{
			public CapturedByBountyHuntersIssueTypeDefiner()
				: base(580000)
			{
			}

			protected override void DefineClassTypes()
			{
				base.AddClassDefinition(typeof(CapturedByBountyHuntersIssueBehavior.CapturedByBountyHuntersIssue), 1, null);
				base.AddClassDefinition(typeof(CapturedByBountyHuntersIssueBehavior.CapturedByBountyHuntersIssueQuest), 2, null);
			}
		}
	}
}
