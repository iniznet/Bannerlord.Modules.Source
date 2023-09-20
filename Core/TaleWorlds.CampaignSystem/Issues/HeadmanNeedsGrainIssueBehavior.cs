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
	public class HeadmanNeedsGrainIssueBehavior : CampaignBehaviorBase
	{
		private static int AverageGrainPriceInCalradia
		{
			get
			{
				return Campaign.Current.GetCampaignBehavior<HeadmanNeedsGrainIssueBehavior>()._averageGrainPriceInCalradia;
			}
		}

		public override void RegisterEvents()
		{
			CampaignEvents.OnCheckForIssueEvent.AddNonSerializedListener(this, new Action<Hero>(this.OnCheckForIssue));
			CampaignEvents.WeeklyTickEvent.AddNonSerializedListener(this, new Action(this.WeeklyTick));
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
		}

		public override void SyncData(IDataStore dataStore)
		{
		}

		private bool ConditionsHold(Hero issueGiver)
		{
			return issueGiver.CurrentSettlement != null && issueGiver.IsNotable && issueGiver.CurrentSettlement.IsVillage && issueGiver.CurrentSettlement.Village.Bound.IsTown && (issueGiver.IsHeadman && issueGiver.CurrentSettlement.Village.VillageType == DefaultVillageTypes.WheatFarm) && (float)issueGiver.CurrentSettlement.Village.GetItemPrice(DefaultItems.Grain, null, false) > (float)this._averageGrainPriceInCalradia * 1.3f;
		}

		public void OnCheckForIssue(Hero hero)
		{
			if (this.ConditionsHold(hero))
			{
				Campaign.Current.IssueManager.AddPotentialIssueData(hero, new PotentialIssueData(new PotentialIssueData.StartIssueDelegate(this.OnSelected), typeof(HeadmanNeedsGrainIssueBehavior.HeadmanNeedsGrainIssue), IssueBase.IssueFrequency.Rare, null));
				return;
			}
			Campaign.Current.IssueManager.AddPotentialIssueData(hero, new PotentialIssueData(typeof(HeadmanNeedsGrainIssueBehavior.HeadmanNeedsGrainIssue), IssueBase.IssueFrequency.Rare));
		}

		private IssueBase OnSelected(in PotentialIssueData pid, Hero issueOwner)
		{
			return new HeadmanNeedsGrainIssueBehavior.HeadmanNeedsGrainIssue(issueOwner);
		}

		private void WeeklyTick()
		{
			this.CacheGrainPrice();
		}

		private void OnSessionLaunched(CampaignGameStarter campaignGameStarter)
		{
			this.CacheGrainPrice();
		}

		private void CacheGrainPrice()
		{
			this._averageGrainPriceInCalradia = QuestHelper.GetAveragePriceOfItemInTheWorld(DefaultItems.Grain);
		}

		private const IssueBase.IssueFrequency HeadmanNeedsGrainIssueFrequency = IssueBase.IssueFrequency.Rare;

		private const int NearbyTownMarketGrainLimit = 50;

		private int _averageGrainPriceInCalradia;

		public class HeadmanNeedsGrainIssue : IssueBase
		{
			internal static void AutoGeneratedStaticCollectObjectsHeadmanNeedsGrainIssue(object o, List<object> collectedObjects)
			{
				((HeadmanNeedsGrainIssueBehavior.HeadmanNeedsGrainIssue)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			public override IssueBase.AlternativeSolutionScaleFlag AlternativeSolutionScaleFlags
			{
				get
				{
					return IssueBase.AlternativeSolutionScaleFlag.Duration;
				}
			}

			private int NeededGrainAmount
			{
				get
				{
					return (int)(12f + 180f * base.IssueDifficultyMultiplier);
				}
			}

			private int AlternativeSolutionNeededGold
			{
				get
				{
					return this.NeededGrainAmount * HeadmanNeedsGrainIssueBehavior.AverageGrainPriceInCalradia;
				}
			}

			public override int AlternativeSolutionBaseNeededMenCount
			{
				get
				{
					return 3 + MathF.Ceiling(6f * base.IssueDifficultyMultiplier);
				}
			}

			protected override int AlternativeSolutionBaseDurationInDaysInternal
			{
				get
				{
					return 2 + MathF.Ceiling(6f * base.IssueDifficultyMultiplier);
				}
			}

			protected override int RewardGold
			{
				get
				{
					return 0;
				}
			}

			[CachedData]
			private Settlement NearbySuitableSettlement
			{
				get
				{
					if (this._nearbySuitableSettlementCache == null)
					{
						Settlement settlement = SettlementHelper.FindNearestSettlement(delegate(Settlement x)
						{
							if (x.Town != null && !x.Town.IsCastle && !x.MapFaction.IsAtWarWith(base.IssueOwner.MapFaction))
							{
								int price = x.Town.MarketData.GetPrice(DefaultItems.Grain, MobileParty.MainParty, false, null);
								return price > 0 && price < HeadmanNeedsGrainIssueBehavior.AverageGrainPriceInCalradia * 2;
							}
							return false;
						}, null);
						this._nearbySuitableSettlementCache = settlement;
					}
					return this._nearbySuitableSettlementCache;
				}
			}

			public override TextObject Title
			{
				get
				{
					TextObject textObject = new TextObject("{=sQBBOKDD}{ISSUE_SETTLEMENT} Needs Grain Seeds", null);
					textObject.SetTextVariable("ISSUE_SETTLEMENT", base.IssueSettlement.Name);
					return textObject;
				}
			}

			public override TextObject Description
			{
				get
				{
					TextObject textObject = new TextObject("{=OJObD61e}The headman of {ISSUE_SETTLEMENT} needs grain seeds for the coming sowing season.", null);
					textObject.SetTextVariable("ISSUE_SETTLEMENT", base.IssueSettlement.Name);
					return textObject;
				}
			}

			public override TextObject IssueBriefByIssueGiver
			{
				get
				{
					return new TextObject("{=p1buAbOQ}The harvest has been poor, and rats have eaten much of our stores. We can eat less and tighten our belts, but if we don't have seed grain left over to plant, we'll starve next year.", null);
				}
			}

			public override TextObject IssueAcceptByPlayer
			{
				get
				{
					return new TextObject("{=vKwndBbe}Is there a way to prevent this?", null);
				}
			}

			public override TextObject IssueQuestSolutionExplanationByIssueGiver
			{
				get
				{
					TextObject textObject = new TextObject("{=nG750jQB}Grain will solve our problems. If we had {GRAIN_AMOUNT} bushels, we could use it to sow our fields. But I doubt that {NEARBY_TOWN} has so much to sell at this time of the year. {GRAIN_AMOUNT} bushels of grain costs around {DENAR_AMOUNT}{GOLD_ICON} in the markets, and we don't have that!", null);
					int price = this.NearbySuitableSettlement.Town.MarketData.GetPrice(DefaultItems.Grain, MobileParty.MainParty, false, null);
					textObject.SetTextVariable("NEARBY_TOWN", this.NearbySuitableSettlement.Name);
					textObject.SetTextVariable("GRAIN_AMOUNT", this.NeededGrainAmount);
					textObject.SetTextVariable("DENAR_AMOUNT", price * this.NeededGrainAmount);
					textObject.SetTextVariable("GOLD_ICON", "{=!}<img src=\"General\\Icons\\Coin@2x\" extend=\"8\">");
					return textObject;
				}
			}

			public override TextObject IssueAlternativeSolutionExplanationByIssueGiver
			{
				get
				{
					TextObject textObject = new TextObject("{=5NYPqKBj}I know you're busy, but maybe you can ask some of your men to find us that grain? {MEN_COUNT} men should do the job along with {GOLD}{GOLD_ICON}, and I'd reckon the whole affair should take two weeks. \nI'm desperate here, {?PLAYER.GENDER}madam{?}sir{\\?}... Don't let our children starve!", null);
					textObject.SetTextVariable("MEN_COUNT", base.GetTotalAlternativeSolutionNeededMenCount());
					textObject.SetTextVariable("GOLD", this.AlternativeSolutionNeededGold);
					textObject.SetTextVariable("GOLD_ICON", "{=!}<img src=\"General\\Icons\\Coin@2x\" extend=\"8\">");
					textObject.SetCharacterProperties("PLAYER", Hero.MainHero.CharacterObject, false);
					return textObject;
				}
			}

			public override TextObject IssueQuestSolutionAcceptByPlayer
			{
				get
				{
					return new TextObject("{=ihfuqu2S}I will find that seed grain for you.", null);
				}
			}

			public override TextObject IssueAlternativeSolutionAcceptByPlayer
			{
				get
				{
					TextObject textObject = new TextObject("{=HCMsvAFv}I can order one of my companions and {MEN_COUNT} men to find grain for you.", null);
					textObject.SetTextVariable("MEN_COUNT", base.GetTotalAlternativeSolutionNeededMenCount());
					return textObject;
				}
			}

			public override TextObject IssueDiscussAlternativeSolution
			{
				get
				{
					return new TextObject("{=W6X5DffB}Thank you for sparing the men to bring us that seed grain, {?PLAYER.GENDER}madam{?}sir{\\?}. That should get us through the hard times ahead.", null);
				}
			}

			public override TextObject IssueAsRumorInSettlement
			{
				get
				{
					TextObject textObject = new TextObject("{=WVobv24n}Heaven save us if {QUEST_GIVER.NAME} can't get {?QUEST_GIVER.GENDER}her{?}his{\\?} hands on more grain.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.IssueOwner.CharacterObject, textObject, false);
					return textObject;
				}
			}

			public override TextObject IssueAlternativeSolutionResponseByIssueGiver
			{
				get
				{
					return new TextObject("{=k63ZKmXX}Thank you, {?PLAYER.GENDER}milady{?}sir{\\?}! You are a saviour.", null);
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
					TextObject textObject = new TextObject("{=a0UTO8tW}{ISSUE_OWNER.LINK}, the headman of {ISSUE_SETTLEMENT}, asked you to deliver {GRAIN_AMOUNT} bushels of grain to {?QUEST_GIVER.GENDER}her{?}him{\\?} to use as seeds. Otherwise the peasants cannot sow their fields and starve in the coming season. You have agreed to send your companion {COMPANION.NAME} along with {MEN_COUNT} men to find some grain and return to the village. Your men should return in {RETURN_DAYS} days.", null);
					StringHelpers.SetCharacterProperties("ISSUE_OWNER", base.IssueOwner.CharacterObject, textObject, false);
					StringHelpers.SetCharacterProperties("COMPANION", base.AlternativeSolutionHero.CharacterObject, textObject, false);
					textObject.SetTextVariable("ISSUE_SETTLEMENT", base.IssueSettlement.Name);
					textObject.SetTextVariable("GRAIN_AMOUNT", this.NeededGrainAmount);
					textObject.SetTextVariable("RETURN_DAYS", base.GetTotalAlternativeSolutionDurationInDays());
					textObject.SetTextVariable("MEN_COUNT", this.AlternativeSolutionSentTroops.TotalManCount - 1);
					return textObject;
				}
			}

			protected override int CompanionSkillRewardXP
			{
				get
				{
					return (int)(500f + 700f * base.IssueDifficultyMultiplier);
				}
			}

			public HeadmanNeedsGrainIssue(Hero issueOwner)
				: base(issueOwner, CampaignTime.DaysFromNow(30f))
			{
			}

			protected override float GetIssueEffectAmountInternal(IssueEffect issueEffect)
			{
				if (issueEffect == DefaultIssueEffects.SettlementProsperity)
				{
					return -0.2f;
				}
				if (issueEffect == DefaultIssueEffects.SettlementLoyalty)
				{
					return -0.5f;
				}
				return 0f;
			}

			public override ValueTuple<SkillObject, int> GetAlternativeSolutionSkill(Hero hero)
			{
				return new ValueTuple<SkillObject, int>((hero.GetSkillValue(DefaultSkills.Trade) >= hero.GetSkillValue(DefaultSkills.Medicine)) ? DefaultSkills.Trade : DefaultSkills.Medicine, 120);
			}

			public override bool DoTroopsSatisfyAlternativeSolution(TroopRoster troopRoster, out TextObject explanation)
			{
				explanation = TextObject.Empty;
				return QuestHelper.CheckRosterForAlternativeSolution(troopRoster, base.GetTotalAlternativeSolutionNeededMenCount(), ref explanation, 0, false);
			}

			public override bool AlternativeSolutionCondition(out TextObject explanation)
			{
				explanation = TextObject.Empty;
				return QuestHelper.CheckRosterForAlternativeSolution(MobileParty.MainParty.MemberRoster, base.GetTotalAlternativeSolutionNeededMenCount(), ref explanation, 0, false) && QuestHelper.CheckGoldForAlternativeSolution(this.AlternativeSolutionNeededGold, ref explanation);
			}

			public override void AlternativeSolutionStartConsequence()
			{
				GiveGoldAction.ApplyForCharacterToParty(Hero.MainHero, base.IssueSettlement.Party, this.AlternativeSolutionNeededGold, false);
				TextObject textObject = new TextObject("{=ex6ZhAAv}You gave {DENAR}{GOLD_ICON} to companion to buy {GRAIN_AMOUNT} units of grain for the {ISSUE_OWNER.NAME}.", null);
				textObject.SetTextVariable("GRAIN_AMOUNT", this.NeededGrainAmount);
				textObject.SetTextVariable("DENAR", this.AlternativeSolutionNeededGold);
				textObject.SetTextVariable("GOLD_ICON", "{=!}<img src=\"General\\Icons\\Coin@2x\" extend=\"8\">");
				StringHelpers.SetCharacterProperties("ISSUE_OWNER", base.IssueOwner.CharacterObject, textObject, false);
				MBInformationManager.AddQuickInformation(textObject, 0, null, "");
			}

			protected override void AlternativeSolutionEndWithSuccessConsequence()
			{
				TraitLevelingHelper.OnIssueSolvedThroughAlternativeSolution(base.IssueOwner, new Tuple<TraitObject, int>[]
				{
					new Tuple<TraitObject, int>(DefaultTraits.Generosity, 30)
				});
				base.IssueOwner.AddPower(10f);
				base.IssueSettlement.Prosperity += 50f;
				this.RelationshipChangeWithIssueOwner = 2;
				foreach (Hero hero in base.IssueOwner.CurrentSettlement.Notables)
				{
					if (hero != base.IssueOwner)
					{
						ChangeRelationAction.ApplyPlayerRelation(hero, 1, true, true);
					}
				}
			}

			protected override void AlternativeSolutionEndWithFailureConsequence()
			{
				base.IssueOwner.AddPower(-5f);
				foreach (Hero hero in base.IssueOwner.CurrentSettlement.Notables)
				{
					ChangeRelationAction.ApplyPlayerRelation(hero, -3, true, true);
				}
				base.IssueSettlement.Prosperity += -10f;
			}

			public override IssueBase.IssueFrequency GetFrequency()
			{
				return IssueBase.IssueFrequency.Rare;
			}

			public override bool IssueStayAliveConditions()
			{
				return this.NearbySuitableSettlement != null && this.NearbySuitableSettlement.Position2D.Distance(base.IssueOwner.CurrentSettlement.Position2D) < 75f && (float)base.IssueOwner.CurrentSettlement.Village.GetItemPrice(DefaultItems.Grain, null, false) > (float)HeadmanNeedsGrainIssueBehavior.AverageGrainPriceInCalradia * 1.05f && !base.IssueOwner.CurrentSettlement.IsRaided && !base.IssueOwner.CurrentSettlement.IsUnderRaid;
			}

			protected override void CompleteIssueWithTimedOutConsequences()
			{
			}

			protected override void OnGameLoad()
			{
			}

			protected override QuestBase GenerateIssueQuest(string questId)
			{
				return new HeadmanNeedsGrainIssueBehavior.HeadmanNeedsGrainIssueQuest(questId, base.IssueOwner, CampaignTime.DaysFromNow(18f), base.IssueDifficultyMultiplier, this.RewardGold, this.NeededGrainAmount);
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
				if (FactionManager.IsAtWarAgainstFaction(issueGiver.CurrentSettlement.MapFaction, Hero.MainHero.MapFaction))
				{
					flag |= IssueBase.PreconditionFlags.AtWar;
				}
				return flag == IssueBase.PreconditionFlags.None;
			}

			private const int IssueDuration = 30;

			private const int AlternativeSolutionSuccessGenerosityBonus = 30;

			private const int AlternativeSolutionFailPowerPenalty = -5;

			private const int QuestTimeLimit = 18;

			private const int AlternativeSolutionSuccessPowerBonus = 10;

			private const int AlternativeSolutionSuccessRelationBonusWithQuestGiver = 2;

			private const int AlternativeSolutionSuccessRelationBonusWithOtherNotables = 1;

			private const int AlternativeSolutionFailRelationPenaltyWithNotables = -3;

			private const int AlternativeSolutionSuccessProsperityBonus = 50;

			private const int AlternativeSolutionFailProsperityPenalty = -10;

			private const int CompanionTradeSkillLimit = 120;

			[CachedData]
			private Settlement _nearbySuitableSettlementCache;
		}

		public class HeadmanNeedsGrainIssueQuest : QuestBase
		{
			internal static void AutoGeneratedStaticCollectObjectsHeadmanNeedsGrainIssueQuest(object o, List<object> collectedObjects)
			{
				((HeadmanNeedsGrainIssueBehavior.HeadmanNeedsGrainIssueQuest)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				base.AutoGeneratedInstanceCollectObjects(collectedObjects);
				collectedObjects.Add(this._playerAcceptedQuestLog);
				collectedObjects.Add(this._playerHasNeededGrainsLog);
			}

			internal static object AutoGeneratedGetMemberValue_neededGrainAmount(object o)
			{
				return ((HeadmanNeedsGrainIssueBehavior.HeadmanNeedsGrainIssueQuest)o)._neededGrainAmount;
			}

			internal static object AutoGeneratedGetMemberValue_rewardGold(object o)
			{
				return ((HeadmanNeedsGrainIssueBehavior.HeadmanNeedsGrainIssueQuest)o)._rewardGold;
			}

			internal static object AutoGeneratedGetMemberValue_playerAcceptedQuestLog(object o)
			{
				return ((HeadmanNeedsGrainIssueBehavior.HeadmanNeedsGrainIssueQuest)o)._playerAcceptedQuestLog;
			}

			internal static object AutoGeneratedGetMemberValue_playerHasNeededGrainsLog(object o)
			{
				return ((HeadmanNeedsGrainIssueBehavior.HeadmanNeedsGrainIssueQuest)o)._playerHasNeededGrainsLog;
			}

			public override TextObject Title
			{
				get
				{
					TextObject textObject = new TextObject("{=apr2dH0n}{ISSUE_SETTLEMENT} Needs Grain Seeds", null);
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

			private TextObject _playerAcceptedQuestLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=5CokRxmL}{QUEST_GIVER.LINK}, the headman of the {QUEST_SETTLEMENT} asked you to deliver {GRAIN_AMOUNT} units of grain to {?QUEST_GIVER.GENDER}her{?}him{\\?} to use as seeds. Otherwise peasants cannot sow their fields and starve in the coming season. \n \n You have agreed to bring them {GRAIN_AMOUNT} units of grain as soon as possible.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					textObject.SetTextVariable("QUEST_SETTLEMENT", base.QuestGiver.CurrentSettlement.Name);
					textObject.SetTextVariable("GRAIN_AMOUNT", this._neededGrainAmount);
					return textObject;
				}
			}

			private TextObject _playerHasNeededGrainsLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=vOHc5dxC}You now have enough grain seeds to complete the quest. Return to {QUEST_SETTLEMENT} to hand them over.", null);
					textObject.SetTextVariable("QUEST_SETTLEMENT", base.QuestGiver.CurrentSettlement.Name);
					return textObject;
				}
			}

			private TextObject _questTimeoutLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=brDw7ewN}You have failed to deliver {GRAIN_AMOUNT} units of grain to the villagers. They won't be able to sow them before the coming winter. The Headman and the villagers are doomed.", null);
					textObject.SetTextVariable("GRAIN_AMOUNT", this._neededGrainAmount);
					return textObject;
				}
			}

			private TextObject _successLog
			{
				get
				{
					TextObject textObject = new TextObject("{=GGTxzAtn}You have delivered {GRAIN_AMOUNT} units of grain to the villagers. They will be able to sow them before the coming winter. You have saved a lot of lives today. The Headman and the villagers are grateful.", null);
					textObject.SetTextVariable("GRAIN_AMOUNT", this._neededGrainAmount);
					return textObject;
				}
			}

			private TextObject _cancelLogOnWarDeclared
			{
				get
				{
					TextObject textObject = new TextObject("{=8Z4vlcib}Your clan is now at war with the {ISSUE_GIVER.LINK}'s lord. Your agreement with {ISSUE_GIVER.LINK} was canceled.", null);
					StringHelpers.SetCharacterProperties("ISSUE_GIVER", base.QuestGiver.CharacterObject, textObject, false);
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

			private TextObject _cancelLogOnVillageRaided
			{
				get
				{
					TextObject textObject = new TextObject("{=PgFJLK85}{SETTLEMENT_NAME} is raided by someone else. Your agreement with {ISSUE_GIVER.LINK} was canceled.", null);
					textObject.SetTextVariable("SETTLEMENT_NAME", base.QuestGiver.CurrentSettlement.Name);
					StringHelpers.SetCharacterProperties("ISSUE_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					return textObject;
				}
			}

			public HeadmanNeedsGrainIssueQuest(string questId, Hero giverHero, CampaignTime duration, float difficultyMultiplier, int rewardGold, int neededGrainAmount)
				: base(questId, giverHero, duration, rewardGold)
			{
				this._neededGrainAmount = neededGrainAmount;
				this._rewardGold = rewardGold;
				this.SetDialogs();
				base.InitializeQuestOnCreation();
			}

			protected override void InitializeQuestOnGameLoad()
			{
				this.SetDialogs();
			}

			protected override void RegisterEvents()
			{
				CampaignEvents.PlayerInventoryExchangeEvent.AddNonSerializedListener(this, new Action<List<ValueTuple<ItemRosterElement, int>>, List<ValueTuple<ItemRosterElement, int>>, bool>(this.OnPlayerInventoryExchange));
				CampaignEvents.OnPartyConsumedFoodEvent.AddNonSerializedListener(this, new Action<MobileParty>(this.OnPartyConsumedFood));
				CampaignEvents.OnHeroSharedFoodWithAnotherHeroEvent.AddNonSerializedListener(this, new Action<Hero, Hero, float>(this.OnHeroSharedFoodWithAnotherHero));
				CampaignEvents.WarDeclared.AddNonSerializedListener(this, new Action<IFaction, IFaction, DeclareWarAction.DeclareWarDetail>(this.OnWarDeclared));
				CampaignEvents.ClanChangedKingdom.AddNonSerializedListener(this, new Action<Clan, Kingdom, Kingdom, ChangeKingdomAction.ChangeKingdomActionDetail, bool>(this.OnClanChangedKingdom));
				CampaignEvents.MapEventStarted.AddNonSerializedListener(this, new Action<MapEvent, PartyBase, PartyBase>(this.OnMapEventStarted));
				CampaignEvents.HourlyTickPartyEvent.AddNonSerializedListener(this, new Action<MobileParty>(this.HourlyTickParty));
				CampaignEvents.HeroPrisonerTaken.AddNonSerializedListener(this, new Action<PartyBase, Hero>(this.OnHeroPrisonerTaken));
				CampaignEvents.SettlementEntered.AddNonSerializedListener(this, new Action<MobileParty, Settlement, Hero>(this.OnSettlementEntered));
			}

			private void OnMapEventStarted(MapEvent mapEvent, PartyBase attackerParty, PartyBase defenderParty)
			{
				if (QuestHelper.CheckMinorMajorCoercion(this, mapEvent, attackerParty))
				{
					QuestHelper.ApplyGenericMinorMajorCoercionConsequences(this, mapEvent);
					return;
				}
				if (mapEvent.IsRaid || mapEvent.IsForcingSupplies || mapEvent.IsForcingVolunteers)
				{
					base.CompleteQuestWithCancel(this._cancelLogOnVillageRaided);
				}
			}

			private void HourlyTickParty(MobileParty mobileParty)
			{
				if (mobileParty == MobileParty.MainParty)
				{
					this._playerAcceptedQuestLog.UpdateCurrentProgress(this.GetRequiredGrainCountOnPlayer());
					this.CheckIfPlayerReadyToReturnGrains();
				}
			}

			private void OnHeroPrisonerTaken(PartyBase capturer, Hero prisoner)
			{
				if (prisoner == Hero.MainHero)
				{
					this._playerAcceptedQuestLog.UpdateCurrentProgress(this.GetRequiredGrainCountOnPlayer());
					this.CheckIfPlayerReadyToReturnGrains();
				}
			}

			private void OnSettlementEntered(MobileParty party, Settlement settlement, Hero hero)
			{
				if (party == MobileParty.MainParty)
				{
					this._playerAcceptedQuestLog.UpdateCurrentProgress(this.GetRequiredGrainCountOnPlayer());
					this.CheckIfPlayerReadyToReturnGrains();
				}
			}

			protected override void OnTimedOut()
			{
				base.AddLog(this._questTimeoutLogText, false);
				this.Fail();
			}

			protected override void SetDialogs()
			{
				TextObject textObject = new TextObject("{=nwIYsJRO}Have you brought our grain {?PLAYER.GENDER}milady{?}sir{\\?}?", null);
				TextObject textObject2 = new TextObject("{=k63ZKmXX}Thank you, {?PLAYER.GENDER}milady{?}sir{\\?}! You are a saviour.", null);
				TextObject textObject3 = new TextObject("{=0tB3VGE4}We await your success, {?PLAYER.GENDER}milady{?}sir{\\?}.", null);
				textObject.SetCharacterProperties("PLAYER", Hero.MainHero.CharacterObject, false);
				textObject2.SetCharacterProperties("PLAYER", Hero.MainHero.CharacterObject, false);
				textObject3.SetCharacterProperties("PLAYER", Hero.MainHero.CharacterObject, false);
				this.OfferDialogFlow = DialogFlow.CreateDialogFlow("issue_classic_quest_start", 100).NpcLine(textObject2, null, null).Condition(() => CharacterObject.OneToOneConversationCharacter == base.QuestGiver.CharacterObject)
					.Consequence(new ConversationSentence.OnConsequenceDelegate(this.QuestAcceptedConsequences))
					.CloseDialog();
				this.DiscussDialogFlow = DialogFlow.CreateDialogFlow("quest_discuss", 100).NpcLine(textObject, null, null).Condition(delegate
				{
					MBTextManager.SetTextVariable("GRAIN_AMOUNT", this._neededGrainAmount);
					return CharacterObject.OneToOneConversationCharacter == base.QuestGiver.CharacterObject;
				})
					.BeginPlayerOptions()
					.PlayerOption(new TextObject("{=9UABeRWO}Yes. Here is your grain.", null), null)
					.ClickableCondition(new ConversationSentence.OnClickableConditionDelegate(this.CompleteQuestClickableConditions))
					.NpcLine(textObject2, null, null)
					.Consequence(delegate
					{
						Campaign.Current.ConversationManager.ConversationEndOneShot += this.Success;
					})
					.CloseDialog()
					.PlayerOption(new TextObject("{=PI6ikMsc}I'm working on it.", null), null)
					.NpcLine(textObject3, null, null)
					.CloseDialog()
					.EndPlayerOptions()
					.CloseDialog();
			}

			private bool CompleteQuestClickableConditions(out TextObject explanation)
			{
				if (this._playerAcceptedQuestLog.CurrentProgress >= this._neededGrainAmount)
				{
					explanation = TextObject.Empty;
					return true;
				}
				explanation = new TextObject("{=mzabdwoh}You don't have enough grain.", null);
				return false;
			}

			private void QuestAcceptedConsequences()
			{
				base.StartQuest();
				int requiredGrainCountOnPlayer = this.GetRequiredGrainCountOnPlayer();
				this._playerAcceptedQuestLog = base.AddDiscreteLog(this._playerAcceptedQuestLogText, new TextObject("{=eEwI880g}Collect Grain", null), requiredGrainCountOnPlayer, this._neededGrainAmount, null, false);
			}

			private int GetRequiredGrainCountOnPlayer()
			{
				int itemNumber = PartyBase.MainParty.ItemRoster.GetItemNumber(DefaultItems.Grain);
				if (itemNumber >= this._neededGrainAmount)
				{
					TextObject textObject = new TextObject("{=Gtbfm10o}You have enough grain to complete the quest. Return to {QUEST_SETTLEMENT} to hand it over.", null);
					textObject.SetTextVariable("QUEST_SETTLEMENT", base.QuestGiver.CurrentSettlement.Name);
					MBInformationManager.AddQuickInformation(textObject, 0, null, "");
				}
				if (itemNumber <= this._neededGrainAmount)
				{
					return itemNumber;
				}
				return this._neededGrainAmount;
			}

			private void CheckIfPlayerReadyToReturnGrains()
			{
				if (this._playerHasNeededGrainsLog == null && this._playerAcceptedQuestLog.CurrentProgress >= this._neededGrainAmount)
				{
					this._playerHasNeededGrainsLog = base.AddLog(this._playerHasNeededGrainsLogText, false);
					return;
				}
				if (this._playerHasNeededGrainsLog != null && this._playerAcceptedQuestLog.CurrentProgress < this._neededGrainAmount)
				{
					base.RemoveLog(this._playerHasNeededGrainsLog);
					this._playerHasNeededGrainsLog = null;
				}
			}

			private void OnPlayerInventoryExchange(List<ValueTuple<ItemRosterElement, int>> purchasedItems, List<ValueTuple<ItemRosterElement, int>> soldItems, bool isTrading)
			{
				bool flag = false;
				foreach (ValueTuple<ItemRosterElement, int> valueTuple in purchasedItems)
				{
					ItemRosterElement itemRosterElement = valueTuple.Item1;
					if (itemRosterElement.EquipmentElement.Item == DefaultItems.Grain)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					foreach (ValueTuple<ItemRosterElement, int> valueTuple2 in soldItems)
					{
						ItemRosterElement itemRosterElement = valueTuple2.Item1;
						if (itemRosterElement.EquipmentElement.Item == DefaultItems.Grain)
						{
							flag = true;
							break;
						}
					}
				}
				if (flag)
				{
					this._playerAcceptedQuestLog.UpdateCurrentProgress(this.GetRequiredGrainCountOnPlayer());
					this.CheckIfPlayerReadyToReturnGrains();
				}
			}

			private void OnPartyConsumedFood(MobileParty party)
			{
				if (party.IsMainParty)
				{
					this._playerAcceptedQuestLog.UpdateCurrentProgress(this.GetRequiredGrainCountOnPlayer());
					this.CheckIfPlayerReadyToReturnGrains();
				}
			}

			private void OnHeroSharedFoodWithAnotherHero(Hero supporterHero, Hero supportedHero, float influence)
			{
				if (supporterHero == Hero.MainHero || supportedHero == Hero.MainHero)
				{
					this._playerAcceptedQuestLog.UpdateCurrentProgress(this.GetRequiredGrainCountOnPlayer());
					this.CheckIfPlayerReadyToReturnGrains();
				}
			}

			private void OnClanChangedKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail detail, bool showNotification = true)
			{
				if (base.QuestGiver.CurrentSettlement.MapFaction.IsAtWarWith(Hero.MainHero.MapFaction))
				{
					base.CompleteQuestWithCancel(this._cancelLogOnWarDeclared);
				}
			}

			private void OnWarDeclared(IFaction faction1, IFaction faction2, DeclareWarAction.DeclareWarDetail detail)
			{
				QuestHelper.CheckWarDeclarationAndFailOrCancelTheQuest(this, faction1, faction2, detail, this._playerDeclaredWarQuestLogText, this._cancelLogOnWarDeclared);
			}

			private void Success()
			{
				base.AddLog(this._successLog, false);
				TraitLevelingHelper.OnIssueSolvedThroughQuest(base.QuestGiver, new Tuple<TraitObject, int>[]
				{
					new Tuple<TraitObject, int>(DefaultTraits.Mercy, 70),
					new Tuple<TraitObject, int>(DefaultTraits.Generosity, 50)
				});
				GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, this._rewardGold, false);
				GiveItemAction.ApplyForParties(PartyBase.MainParty, Settlement.CurrentSettlement.Party, DefaultItems.Grain, this._neededGrainAmount);
				base.QuestGiver.AddPower(10f);
				base.QuestGiver.CurrentSettlement.Prosperity += 50f;
				this.RelationshipChangeWithQuestGiver = 2;
				foreach (Hero hero in base.QuestGiver.CurrentSettlement.Notables)
				{
					if (hero != base.QuestGiver)
					{
						ChangeRelationAction.ApplyPlayerRelation(hero, 1, true, true);
					}
				}
				base.CompleteQuestWithSuccess();
			}

			private void Fail()
			{
				base.QuestGiver.AddPower(-5f);
				base.QuestGiver.CurrentSettlement.Prosperity += -10f;
				this.RelationshipChangeWithQuestGiver = -5;
				foreach (Hero hero in base.QuestGiver.CurrentSettlement.Notables)
				{
					if (hero != base.QuestGiver)
					{
						ChangeRelationAction.ApplyPlayerRelation(hero, -3, true, true);
					}
				}
			}

			private const int SuccessMercyBonus = 70;

			private const int SuccessGenerosityBonus = 50;

			private const int SuccessRelationBonusWithQuestGiver = 2;

			private const int SuccessRelationBonusWithOtherNotables = 1;

			private const int SuccessPowerBonus = 10;

			private const int SuccessProsperityBonus = 50;

			private const int FailRelationPenalty = -5;

			private const int FailRelationPenaltyWithOtherNotables = -3;

			private const int TimeOutProsperityPenalty = -10;

			private const int TimeOutPowerPenalty = -5;

			[SaveableField(10)]
			private readonly int _neededGrainAmount;

			[SaveableField(20)]
			private int _rewardGold;

			[SaveableField(30)]
			private JournalLog _playerAcceptedQuestLog;

			[SaveableField(40)]
			private JournalLog _playerHasNeededGrainsLog;
		}

		public class HeadmanNeedsGrainIssueTypeDefiner : SaveableTypeDefiner
		{
			public HeadmanNeedsGrainIssueTypeDefiner()
				: base(440000)
			{
			}

			protected override void DefineClassTypes()
			{
				base.AddClassDefinition(typeof(HeadmanNeedsGrainIssueBehavior.HeadmanNeedsGrainIssue), 1, null);
				base.AddClassDefinition(typeof(HeadmanNeedsGrainIssueBehavior.HeadmanNeedsGrainIssueQuest), 2, null);
			}
		}
	}
}