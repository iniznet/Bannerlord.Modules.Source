﻿using System;
using System.Collections.Generic;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Extensions;
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
	public class VillageNeedsCraftingMaterialsIssueBehavior : CampaignBehaviorBase
	{
		public override void RegisterEvents()
		{
			CampaignEvents.OnCheckForIssueEvent.AddNonSerializedListener(this, new Action<Hero>(this.OnCheckForIssue));
		}

		private void OnCheckForIssue(Hero hero)
		{
			Campaign.Current.IssueManager.AddPotentialIssueData(hero, this.ConditionsHold(hero) ? new PotentialIssueData(new PotentialIssueData.StartIssueDelegate(this.OnStartIssue), typeof(VillageNeedsCraftingMaterialsIssueBehavior.VillageNeedsCraftingMaterialsIssue), IssueBase.IssueFrequency.Rare, null) : new PotentialIssueData(typeof(VillageNeedsCraftingMaterialsIssueBehavior.VillageNeedsCraftingMaterialsIssue), IssueBase.IssueFrequency.Rare));
		}

		private bool ConditionsHold(Hero issueGiver)
		{
			return issueGiver.IsRuralNotable && !issueGiver.MapFaction.IsAtWarWith(Clan.PlayerClan);
		}

		private IssueBase OnStartIssue(in PotentialIssueData pid, Hero issueOwner)
		{
			return new VillageNeedsCraftingMaterialsIssueBehavior.VillageNeedsCraftingMaterialsIssue(issueOwner);
		}

		private static ItemObject SelectCraftingMaterial()
		{
			int num = MBRandom.RandomInt(0, 2);
			if (num == 0)
			{
				return DefaultItems.IronIngot1;
			}
			if (num != 1)
			{
				return DefaultItems.IronIngot1;
			}
			return DefaultItems.IronIngot2;
		}

		public override void SyncData(IDataStore dataStore)
		{
		}

		private const IssueBase.IssueFrequency VillageNeedsCraftingMaterialsIssueFrequency = IssueBase.IssueFrequency.Rare;

		public class VillageNeedsCraftingMaterialsIssue : IssueBase
		{
			internal static void AutoGeneratedStaticCollectObjectsVillageNeedsCraftingMaterialsIssue(object o, List<object> collectedObjects)
			{
				((VillageNeedsCraftingMaterialsIssueBehavior.VillageNeedsCraftingMaterialsIssue)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				base.AutoGeneratedInstanceCollectObjects(collectedObjects);
				collectedObjects.Add(this._requestedItem);
			}

			internal static object AutoGeneratedGetMemberValue_requestedItem(object o)
			{
				return ((VillageNeedsCraftingMaterialsIssueBehavior.VillageNeedsCraftingMaterialsIssue)o)._requestedItem;
			}

			internal static object AutoGeneratedGetMemberValue_promisedPayment(object o)
			{
				return ((VillageNeedsCraftingMaterialsIssueBehavior.VillageNeedsCraftingMaterialsIssue)o)._promisedPayment;
			}

			private int _numberOfRequestedItem
			{
				get
				{
					return MathF.Round((float)((int)(750f / (float)this._requestedItem.Value)) * base.IssueDifficultyMultiplier);
				}
			}

			protected override int CompanionSkillRewardXP
			{
				get
				{
					return 500 + (int)(700f * base.IssueDifficultyMultiplier);
				}
			}

			protected override bool IssueQuestCanBeDuplicated
			{
				get
				{
					return false;
				}
			}

			public override int AlternativeSolutionBaseNeededMenCount
			{
				get
				{
					return 4;
				}
			}

			protected override int AlternativeSolutionBaseDurationInDaysInternal
			{
				get
				{
					return (int)(2f + 4f * base.IssueDifficultyMultiplier);
				}
			}

			protected override float GetIssueEffectAmountInternal(IssueEffect issueEffect)
			{
				if (issueEffect == DefaultIssueEffects.VillageHearth)
				{
					return -0.2f;
				}
				if (issueEffect == DefaultIssueEffects.IssueOwnerPower)
				{
					return -0.1f;
				}
				return 0f;
			}

			public override ValueTuple<SkillObject, int> GetAlternativeSolutionSkill(Hero hero)
			{
				return new ValueTuple<SkillObject, int>(DefaultSkills.Crafting, 120);
			}

			public override bool AlternativeSolutionCondition(out TextObject explanation)
			{
				explanation = TextObject.Empty;
				return QuestHelper.CheckRosterForAlternativeSolution(MobileParty.MainParty.MemberRoster, base.GetTotalAlternativeSolutionNeededMenCount(), ref explanation, 0, false);
			}

			protected override void AlternativeSolutionEndWithSuccessConsequence()
			{
				GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, this.GetPayment(), false);
				this.RelationshipChangeWithIssueOwner = 5;
				base.IssueSettlement.Village.Hearth += 60f;
				base.IssueOwner.AddPower(10f);
			}

			public override bool DoTroopsSatisfyAlternativeSolution(TroopRoster troopRoster, out TextObject explanation)
			{
				explanation = TextObject.Empty;
				return QuestHelper.CheckRosterForAlternativeSolution(troopRoster, base.GetTotalAlternativeSolutionNeededMenCount(), ref explanation, 0, false);
			}

			public override TextObject Title
			{
				get
				{
					TextObject textObject = new TextObject("{=eR7P1cVA}{VILLAGE} Needs Crafting Materials", null);
					textObject.SetTextVariable("VILLAGE", base.IssueOwner.CurrentSettlement.Name);
					return textObject;
				}
			}

			public override TextObject Description
			{
				get
				{
					TextObject textObject = new TextObject("{=5CJrR0X3}{ISSUE_GIVER.LINK} in the village requested crafting materials for their ongoing project.", null);
					textObject.SetCharacterProperties("ISSUE_GIVER", base.IssueOwner.CharacterObject, false);
					return textObject;
				}
			}

			public override TextObject IssueBriefByIssueGiver
			{
				get
				{
					return new TextObject("{=095beaQ5}Yes, there's a lot of work we need to do around the village, and we're short on the materials that our smith needs to make us tools and fittings. Do you think you could get us some? We'll pay well.[ib:demure][if:convo_dismayed]", null);
				}
			}

			public override TextObject IssueAcceptByPlayer
			{
				get
				{
					return new TextObject("{=xmu89biL}Maybe I can help. What do you need exactly?", null);
				}
			}

			public override TextObject IssueQuestSolutionExplanationByIssueGiver
			{
				get
				{
					TextObject textObject = new TextObject("{=PftlaE0x}We need {REQUESTED_ITEM_COUNT} {?(REQUESTED_ITEM_COUNT > 1)}{PLURAL(REQUESTED_ITEM)}{?}{REQUESTED_ITEM}{\\?} in {NUMBER_OF_DAYS} days. We need to repair some roofs before the next big storms. I can offer {PAYMENT}{GOLD_ICON}. What do you say?", null);
					textObject.SetTextVariable("PAYMENT", this.GetPayment());
					textObject.SetTextVariable("REQUESTED_ITEM", this._requestedItem.Name);
					textObject.SetTextVariable("REQUESTED_ITEM_COUNT", this._numberOfRequestedItem);
					textObject.SetTextVariable("NUMBER_OF_DAYS", 30);
					textObject.SetTextVariable("GOLD_ICON", "{=!}<img src=\"General\\Icons\\Coin@2x\" extend=\"8\">");
					return textObject;
				}
			}

			public override TextObject IssuePlayerResponseAfterAlternativeExplanation
			{
				get
				{
					return new TextObject("{=i96OaGH3}Is there anything else I could do to help?", null);
				}
			}

			public override TextObject IssueAlternativeSolutionExplanationByIssueGiver
			{
				get
				{
					return new TextObject("{=WzdhPF7M}Well, if we had some extra skilled labor, we could probably melt down old tools and reforge them. That's too much work for just our smith by himself, but maybe he could do it with someone proficient in crafting to help him.[ib:demure2][if:convo_thinking]", null);
				}
			}

			public override TextObject IssueQuestSolutionAcceptByPlayer
			{
				get
				{
					return new TextObject("{=WsmH9Cfd}I will provide what you need.", null);
				}
			}

			public override TextObject IssueAlternativeSolutionAcceptByPlayer
			{
				get
				{
					return new TextObject("{=8DWTTnpP}My comrade will help your smith to produce what you need.", null);
				}
			}

			public override TextObject IssueAlternativeSolutionResponseByIssueGiver
			{
				get
				{
					return new TextObject("{=xlagNKZ2}Thank you. With their help, we should be able to make what we need.[if:convo_astonished]", null);
				}
			}

			public override TextObject IssueDiscussAlternativeSolution
			{
				get
				{
					return new TextObject("{=P3Uu0Ham}Your companion is still working with our smith. I hope they will finish the order in time.[if:convo_approving]", null);
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

			public override IssueBase.AlternativeSolutionScaleFlag AlternativeSolutionScaleFlags
			{
				get
				{
					return IssueBase.AlternativeSolutionScaleFlag.Duration;
				}
			}

			protected override TextObject AlternativeSolutionStartLog
			{
				get
				{
					TextObject textObject = new TextObject("{=1XuYGQcT}{ISSUE_GIVER.LINK} told you that {?QUEST_GIVER.GENDER}her{?}his{\\?} local smith needs {REQUESTED_ITEM} to forge more tools. You asked your companion {COMPANION.LINK} to help the local smith and craft {REQUESTED_ITEM_COUNT} {?(REQUESTED_ITEM_COUNT > 1)}{PLURAL(REQUESTED_ITEM)}{?}{REQUESTED_ITEM}{\\?} for the village. Your companion will rejoin your party in {RETURN_DAYS} days.", null);
					StringHelpers.SetCharacterProperties("ISSUE_GIVER", base.IssueOwner.CharacterObject, textObject, false);
					StringHelpers.SetCharacterProperties("COMPANION", base.AlternativeSolutionHero.CharacterObject, textObject, false);
					textObject.SetTextVariable("SETTLEMENT", base.IssueOwner.CurrentSettlement.EncyclopediaLinkWithName);
					textObject.SetTextVariable("REQUESTED_ITEM", this._requestedItem.Name);
					textObject.SetTextVariable("REQUESTED_ITEM_COUNT", this._numberOfRequestedItem);
					textObject.SetTextVariable("RETURN_DAYS", base.GetTotalAlternativeSolutionDurationInDays());
					return textObject;
				}
			}

			public override TextObject IssueAlternativeSolutionSuccessLog
			{
				get
				{
					TextObject textObject = new TextObject("{=n86jgG3m}Your companion {COMPANION.LINK} has helped the local smith and produced {REQUESTED_AMOUNT} {?(REQUESTED_AMOUNT > 1)}{PLURAL(REQUESTED_GOOD)}{?}{REQUESTED_GOOD}{\\?} as you promised.", null);
					StringHelpers.SetCharacterProperties("COMPANION", base.AlternativeSolutionHero.CharacterObject, textObject, false);
					textObject.SetTextVariable("REQUESTED_AMOUNT", this._numberOfRequestedItem);
					textObject.SetTextVariable("REQUESTED_GOOD", this._requestedItem.Name);
					return textObject;
				}
			}

			protected override void OnGameLoad()
			{
			}

			protected override void HourlyTick()
			{
			}

			protected override QuestBase GenerateIssueQuest(string questId)
			{
				return new VillageNeedsCraftingMaterialsIssueBehavior.VillageNeedsCraftingMaterialsIssueQuest(questId, base.IssueOwner, CampaignTime.DaysFromNow(30f), this.GetPayment(), this._requestedItem, this._numberOfRequestedItem);
			}

			public override IssueBase.IssueFrequency GetFrequency()
			{
				return IssueBase.IssueFrequency.Rare;
			}

			public override void AlternativeSolutionStartConsequence()
			{
				this._promisedPayment = this.GetPayment();
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

			protected override void CompleteIssueWithTimedOutConsequences()
			{
			}

			public override bool IssueStayAliveConditions()
			{
				return !base.IssueOwner.CurrentSettlement.IsRaided && !base.IssueOwner.CurrentSettlement.IsUnderRaid;
			}

			public VillageNeedsCraftingMaterialsIssue(Hero issueOwner)
				: base(issueOwner, CampaignTime.DaysFromNow(30f))
			{
				this._requestedItem = VillageNeedsCraftingMaterialsIssueBehavior.SelectCraftingMaterial();
			}

			private int GetPayment()
			{
				if (this._promisedPayment != 0)
				{
					return this._promisedPayment;
				}
				return 750 + (base.IssueSettlement.Village.Bound.Town.MarketData.GetPrice(this._requestedItem, null, false, null) + QuestHelper.GetAveragePriceOfItemInTheWorld(this._requestedItem) / 2) * this._numberOfRequestedItem;
			}

			private const int TimeLimit = 30;

			private const int PowerChangeForQuestGiver = 10;

			private const int RelationWithIssueOwnerRewardOnSuccess = 5;

			private const int VillageHeartChangeOnAlternativeSuccess = 60;

			private const int RequiredSkillValueForAlternativeSolution = 120;

			[SaveableField(1)]
			private readonly ItemObject _requestedItem;

			[SaveableField(4)]
			private int _promisedPayment;
		}

		public class VillageNeedsCraftingMaterialsIssueQuest : QuestBase
		{
			internal static void AutoGeneratedStaticCollectObjectsVillageNeedsCraftingMaterialsIssueQuest(object o, List<object> collectedObjects)
			{
				((VillageNeedsCraftingMaterialsIssueBehavior.VillageNeedsCraftingMaterialsIssueQuest)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				base.AutoGeneratedInstanceCollectObjects(collectedObjects);
				collectedObjects.Add(this._requestedItem);
				collectedObjects.Add(this._playerAcceptedQuestLog);
				collectedObjects.Add(this._playerHasNeededItemsLog);
			}

			internal static object AutoGeneratedGetMemberValue_requestedItemAmount(object o)
			{
				return ((VillageNeedsCraftingMaterialsIssueBehavior.VillageNeedsCraftingMaterialsIssueQuest)o)._requestedItemAmount;
			}

			internal static object AutoGeneratedGetMemberValue_requestedItem(object o)
			{
				return ((VillageNeedsCraftingMaterialsIssueBehavior.VillageNeedsCraftingMaterialsIssueQuest)o)._requestedItem;
			}

			internal static object AutoGeneratedGetMemberValue_playerAcceptedQuestLog(object o)
			{
				return ((VillageNeedsCraftingMaterialsIssueBehavior.VillageNeedsCraftingMaterialsIssueQuest)o)._playerAcceptedQuestLog;
			}

			internal static object AutoGeneratedGetMemberValue_playerHasNeededItemsLog(object o)
			{
				return ((VillageNeedsCraftingMaterialsIssueBehavior.VillageNeedsCraftingMaterialsIssueQuest)o)._playerHasNeededItemsLog;
			}

			private TextObject QuestStartedLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=YZeKScP5}{QUEST_GIVER.LINK} told you that {?QUEST_GIVER.GENDER}her{?}his{\\?} local smith needs {REQUESTED_ITEM} to forge more tools. {?QUEST_GIVER.GENDER}She{?}He{\\?} asked you to bring {REQUESTED_ITEM_AMOUNT} {?(REQUESTED_ITEM_AMOUNT > 1)}{PLURAL(REQUESTED_ITEM)}{?}{REQUESTED_ITEM}{\\?} to {?QUEST_GIVER.GENDER}her{?}him{\\?}.", null);
					textObject.SetTextVariable("REQUESTED_ITEM_AMOUNT", this._requestedItemAmount);
					textObject.SetTextVariable("REQUESTED_ITEM", this._requestedItem.Name);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					return textObject;
				}
			}

			private TextObject QuestSuccessLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=LiDSTrvV}You brought {REQUESTED_ITEM_AMOUNT} {?(REQUESTED_ITEM_AMOUNT > 1)}{PLURAL(REQUESTED_ITEM)}{?}{REQUESTED_ITEM}{\\?} to {?QUEST_GIVER.GENDER}her{?}him{\\?} as promised.", null);
					textObject.SetTextVariable("REQUESTED_ITEM_AMOUNT", this._requestedItemAmount);
					textObject.SetTextVariable("REQUESTED_ITEM", this._requestedItem.Name);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					return textObject;
				}
			}

			private TextObject QuestCanceledWarDeclaredLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=vW6kBki9}Your clan is now at war with {QUEST_GIVER.LINK}'s realm. Your agreement with {QUEST_GIVER.LINK} is canceled.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					return textObject;
				}
			}

			private TextObject QuestGiverVillageRaidedLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=gJG0xmAq}{QUEST_GIVER.LINK}'s village {QUEST_SETTLEMENT} was raided. Your agreement with {QUEST_GIVER.LINK} is canceled.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					textObject.SetTextVariable("QUEST_SETTLEMENT", base.QuestGiver.CurrentSettlement.EncyclopediaLinkWithName);
					return textObject;
				}
			}

			private TextObject QuestFailedWithTimeOutLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=nmz1ky2D}You failed to deliver {REQUESTED_ITEM_AMOUNT} {?(REQUESTED_ITEM_AMOUNT > 1)}{PLURAL(REQUESTED_ITEM)}{?}{REQUESTED_ITEM}{\\?} to {QUEST_GIVER.LINK} in time.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					textObject.SetTextVariable("REQUESTED_ITEM_AMOUNT", this._requestedItemAmount);
					textObject.SetTextVariable("REQUESTED_ITEM", this._requestedItem.Name);
					return textObject;
				}
			}

			private TextObject PlayerHasNeededItemsLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=MxpPkytG}You now have enough {ITEM} to complete the quest. Return to {QUEST_SETTLEMENT} to hand them over.", null);
					textObject.SetTextVariable("ITEM", this._requestedItem.Name);
					textObject.SetTextVariable("QUEST_SETTLEMENT", base.QuestGiver.CurrentSettlement.Name);
					return textObject;
				}
			}

			public VillageNeedsCraftingMaterialsIssueQuest(string questId, Hero questGiver, CampaignTime duration, int rewardGold, ItemObject requestedItem, int requestedItemAmount)
				: base(questId, questGiver, duration, rewardGold)
			{
				this._requestedItem = requestedItem;
				this._requestedItemAmount = requestedItemAmount;
				this.SetDialogs();
				base.InitializeQuestOnCreation();
			}

			public override TextObject Title
			{
				get
				{
					TextObject textObject = new TextObject("{=LgiRMbgE}{ISSUE_SETTLEMENT} Needs Crafting Materials", null);
					textObject.SetTextVariable("ISSUE_SETTLEMENT", base.QuestGiver.CurrentSettlement.Name);
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

			protected override void SetDialogs()
			{
				TextObject textObject = new TextObject("{=UbUokDyI}Thank you. We'd appreciate it if you got the goods to us as quickly as possible. Good luck![ib:nervous2][if:convo_excited]", null);
				TextObject textObject2 = new TextObject("{=4c9ySfVj}Did you find what we needed, {?PLAYER.GENDER}madam{?}sir{\\?}?", null);
				TextObject textObject3 = new TextObject("{=nEGe8rUd}Thank you for your help, {?PLAYER.GENDER}madam{?}sir{\\?}. Here is what we promised.", null);
				TextObject textObject4 = new TextObject("{=sTfr1C8H}Thank you. But if the storms come before you find them, well, that would be bad for us.[ib:nervous2][if:convo_nervous]", null);
				textObject2.SetCharacterProperties("PLAYER", Hero.MainHero.CharacterObject, false);
				textObject3.SetCharacterProperties("PLAYER", Hero.MainHero.CharacterObject, false);
				this.OfferDialogFlow = DialogFlow.CreateDialogFlow("issue_classic_quest_start", 100).NpcLine(textObject, null, null).Condition(() => CharacterObject.OneToOneConversationCharacter == base.QuestGiver.CharacterObject)
					.Consequence(new ConversationSentence.OnConsequenceDelegate(this.QuestAcceptedConsequences))
					.CloseDialog();
				this.DiscussDialogFlow = DialogFlow.CreateDialogFlow("quest_discuss", 100).NpcLine(textObject2, null, null).Condition(() => CharacterObject.OneToOneConversationCharacter == base.QuestGiver.CharacterObject)
					.BeginPlayerOptions()
					.PlayerOption(new TextObject("{=bLRGix1b}Yes, I have them with me.", null), null)
					.ClickableCondition(new ConversationSentence.OnClickableConditionDelegate(this.CompleteQuestClickableConditions))
					.NpcLine(textObject3, null, null)
					.Consequence(delegate
					{
						Campaign.Current.ConversationManager.ConversationEndOneShot += this.Success;
					})
					.CloseDialog()
					.PlayerOption(new TextObject("{=D8KFcE2i}Not yet, I am still working on it.", null), null)
					.NpcLine(textObject4, null, null)
					.CloseDialog()
					.EndPlayerOptions()
					.CloseDialog();
			}

			private bool CompleteQuestClickableConditions(out TextObject explanation)
			{
				if (this._playerAcceptedQuestLog.CurrentProgress >= this._requestedItemAmount)
				{
					explanation = TextObject.Empty;
					return true;
				}
				explanation = new TextObject("{=EmBla2xa}You don't have enough {ITEM}", null);
				explanation.SetTextVariable("ITEM", this._requestedItem.Name);
				return false;
			}

			protected override void InitializeQuestOnGameLoad()
			{
				this.SetDialogs();
			}

			protected override void HourlyTick()
			{
			}

			private void QuestAcceptedConsequences()
			{
				base.StartQuest();
				int requiredItemCountOnPlayer = this.GetRequiredItemCountOnPlayer();
				TextObject textObject = new TextObject("{=nAEhfGJk}Collect {ITEM}", null);
				textObject.SetTextVariable("ITEM", this._requestedItem.Name);
				this._playerAcceptedQuestLog = base.AddDiscreteLog(this.QuestStartedLogText, textObject, requiredItemCountOnPlayer, this._requestedItemAmount, null, false);
			}

			protected override void OnTimedOut()
			{
				this.Fail();
			}

			private void Success()
			{
				base.AddLog(this.QuestSuccessLogText, false);
				ItemRosterElement itemRosterElement = new ItemRosterElement(this._requestedItem, this._requestedItemAmount, null);
				GiveItemAction.ApplyForParties(PartyBase.MainParty, Settlement.CurrentSettlement.Party, itemRosterElement);
				GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, this.RewardGold, false);
				TraitLevelingHelper.OnIssueSolvedThroughQuest(Hero.MainHero, new Tuple<TraitObject, int>[]
				{
					new Tuple<TraitObject, int>(DefaultTraits.Honor, 30)
				});
				base.QuestGiver.AddPower(10f);
				this.RelationshipChangeWithQuestGiver = 5;
				base.QuestGiver.CurrentSettlement.Village.Hearth += 30f;
				base.CompleteQuestWithSuccess();
			}

			private void Fail()
			{
				base.AddLog(this.QuestFailedWithTimeOutLogText, false);
				base.QuestGiver.AddPower(-10f);
				this.RelationshipChangeWithQuestGiver = -5;
				base.QuestGiver.CurrentSettlement.Village.Hearth += -40f;
				base.CompleteQuestWithFail(null);
			}

			private int GetRequiredItemCountOnPlayer()
			{
				int itemNumber = PartyBase.MainParty.ItemRoster.GetItemNumber(this._requestedItem);
				if (itemNumber >= this._requestedItemAmount)
				{
					TextObject textObject = new TextObject("{=MTCrXEvj}You have enough {ITEM} to complete the quest. Return to {QUEST_SETTLEMENT} to hand it over.", null);
					textObject.SetTextVariable("QUEST_SETTLEMENT", base.QuestGiver.CurrentSettlement.Name);
					textObject.SetTextVariable("ITEM", this._requestedItem.Name);
					MBInformationManager.AddQuickInformation(textObject, 0, null, "");
				}
				if (itemNumber <= this._requestedItemAmount)
				{
					return itemNumber;
				}
				return this._requestedItemAmount;
			}

			protected override void RegisterEvents()
			{
				CampaignEvents.WarDeclared.AddNonSerializedListener(this, new Action<IFaction, IFaction, DeclareWarAction.DeclareWarDetail>(this.OnWarDeclared));
				CampaignEvents.ClanChangedKingdom.AddNonSerializedListener(this, new Action<Clan, Kingdom, Kingdom, ChangeKingdomAction.ChangeKingdomActionDetail, bool>(this.OnClanChangedKingdom));
				CampaignEvents.RaidCompletedEvent.AddNonSerializedListener(this, new Action<BattleSideEnum, RaidEventComponent>(this.OnRaidCompleted));
				CampaignEvents.PlayerInventoryExchangeEvent.AddNonSerializedListener(this, new Action<List<ValueTuple<ItemRosterElement, int>>, List<ValueTuple<ItemRosterElement, int>>, bool>(this.OnPlayerInventoryExchange));
				CampaignEvents.OnNewItemCraftedEvent.AddNonSerializedListener(this, new Action<ItemObject, ItemModifier, bool>(this.OnItemCrafted));
				CampaignEvents.MapEventStarted.AddNonSerializedListener(this, new Action<MapEvent, PartyBase, PartyBase>(this.OnMapEventStarted));
				CampaignEvents.OnEquipmentSmeltedByHeroEvent.AddNonSerializedListener(this, new Action<Hero, EquipmentElement>(this.OnEquipmentSmeltedByHero));
				CampaignEvents.OnItemsRefinedEvent.AddNonSerializedListener(this, new Action<Hero, Crafting.RefiningFormula>(this.OnItemsRefined));
			}

			private void OnItemsRefined(Hero hero, Crafting.RefiningFormula refiningFormula)
			{
				if (hero == Hero.MainHero)
				{
					this.UpdateQuestLog();
				}
			}

			private void OnEquipmentSmeltedByHero(Hero hero, EquipmentElement equipmentElement)
			{
				if (hero == Hero.MainHero)
				{
					this.UpdateQuestLog();
				}
			}

			private void OnWarDeclared(IFaction faction1, IFaction faction2, DeclareWarAction.DeclareWarDetail detail)
			{
				QuestHelper.CheckWarDeclarationAndFailOrCancelTheQuest(this, faction1, faction2, detail, this.QuestCanceledWarDeclaredLogText, this.QuestCanceledWarDeclaredLogText, false);
			}

			private void OnMapEventStarted(MapEvent mapEvent, PartyBase attackerParty, PartyBase defenderParty)
			{
				if (QuestHelper.CheckMinorMajorCoercion(this, mapEvent, attackerParty))
				{
					QuestHelper.ApplyGenericMinorMajorCoercionConsequences(this, mapEvent);
				}
			}

			private void OnItemCrafted(ItemObject item, ItemModifier overriddenItemModifier, bool isCraftingOrderItem)
			{
				this.UpdateQuestLog();
			}

			private void OnPlayerInventoryExchange(List<ValueTuple<ItemRosterElement, int>> purchasedItems, List<ValueTuple<ItemRosterElement, int>> soldItems, bool isTrading)
			{
				this.UpdateQuestLog();
			}

			private void OnClanChangedKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail detail, bool showNotification = true)
			{
				if (base.QuestGiver.CurrentSettlement.MapFaction.IsAtWarWith(Hero.MainHero.MapFaction))
				{
					base.CompleteQuestWithCancel(this.QuestCanceledWarDeclaredLogText);
				}
			}

			private void OnRaidCompleted(BattleSideEnum battleSide, RaidEventComponent mapEvent)
			{
				if (mapEvent.MapEventSettlement == base.QuestGiver.CurrentSettlement)
				{
					base.CompleteQuestWithCancel(this.QuestGiverVillageRaidedLogText);
				}
			}

			private void CheckIfPlayerReadyToReturnItems()
			{
				if (this._playerHasNeededItemsLog == null && this._playerAcceptedQuestLog.CurrentProgress >= this._requestedItemAmount)
				{
					this._playerHasNeededItemsLog = base.AddLog(this.PlayerHasNeededItemsLogText, false);
					return;
				}
				if (this._playerHasNeededItemsLog != null && this._playerAcceptedQuestLog.CurrentProgress < this._requestedItemAmount)
				{
					base.RemoveLog(this._playerHasNeededItemsLog);
					this._playerHasNeededItemsLog = null;
				}
			}

			private void UpdateQuestLog()
			{
				this._playerAcceptedQuestLog.UpdateCurrentProgress(this.GetRequiredItemCountOnPlayer());
				this.CheckIfPlayerReadyToReturnItems();
			}

			[SaveableField(10)]
			private readonly int _requestedItemAmount;

			[SaveableField(20)]
			private readonly ItemObject _requestedItem;

			[SaveableField(30)]
			private JournalLog _playerAcceptedQuestLog;

			[SaveableField(40)]
			private JournalLog _playerHasNeededItemsLog;

			private const int SuccessRelationBonus = 5;

			private const int FailRelationPenalty = -5;

			private const int SuccessPowerBonus = 10;

			private const int FailPowerPenalty = -10;

			private const int SuccessHonorBonus = 30;

			private const int FailWithCrimeHonorPenalty = -50;

			private const int SuccessHearthBonus = 30;

			private const int FailToDeliverInTimeHearthPenalty = -40;
		}

		public class VillageNeedsCraftingMaterialsIssueTypeDefiner : SaveableTypeDefiner
		{
			public VillageNeedsCraftingMaterialsIssueTypeDefiner()
				: base(601000)
			{
			}

			protected override void DefineClassTypes()
			{
				base.AddClassDefinition(typeof(VillageNeedsCraftingMaterialsIssueBehavior.VillageNeedsCraftingMaterialsIssue), 1, null);
				base.AddClassDefinition(typeof(VillageNeedsCraftingMaterialsIssueBehavior.VillageNeedsCraftingMaterialsIssueQuest), 2, null);
			}
		}
	}
}
