using System;
using System.Collections.Generic;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.GameMenus;
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
	public class LordNeedsGarrisonTroopsIssueQuestBehavior : CampaignBehaviorBase
	{
		private static LordNeedsGarrisonTroopsIssueQuestBehavior.LordNeedsGarrisonTroopsIssueQuest Instance
		{
			get
			{
				LordNeedsGarrisonTroopsIssueQuestBehavior campaignBehavior = Campaign.Current.GetCampaignBehavior<LordNeedsGarrisonTroopsIssueQuestBehavior>();
				if (campaignBehavior._cachedQuest != null && campaignBehavior._cachedQuest.IsOngoing)
				{
					return campaignBehavior._cachedQuest;
				}
				using (List<QuestBase>.Enumerator enumerator = Campaign.Current.QuestManager.Quests.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						LordNeedsGarrisonTroopsIssueQuestBehavior.LordNeedsGarrisonTroopsIssueQuest lordNeedsGarrisonTroopsIssueQuest;
						if ((lordNeedsGarrisonTroopsIssueQuest = enumerator.Current as LordNeedsGarrisonTroopsIssueQuestBehavior.LordNeedsGarrisonTroopsIssueQuest) != null)
						{
							campaignBehavior._cachedQuest = lordNeedsGarrisonTroopsIssueQuest;
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

		private void OnSessionLaunched(CampaignGameStarter gameStarter)
		{
			string text = "{=FirEOQaI}Talk to the garrison commander";
			gameStarter.AddGameMenuOption("town", "talk_to_garrison_commander_town", text, new GameMenuOption.OnConditionDelegate(this.talk_to_garrison_commander_on_condition), new GameMenuOption.OnConsequenceDelegate(this.talk_to_garrison_commander_on_consequence), false, 2, false, null);
			gameStarter.AddGameMenuOption("town_guard", "talk_to_garrison_commander_town", text, new GameMenuOption.OnConditionDelegate(this.talk_to_garrison_commander_on_condition), new GameMenuOption.OnConsequenceDelegate(this.talk_to_garrison_commander_on_consequence), false, 2, false, null);
			gameStarter.AddGameMenuOption("castle_guard", "talk_to_garrison_commander_castle", text, new GameMenuOption.OnConditionDelegate(this.talk_to_garrison_commander_on_condition), new GameMenuOption.OnConsequenceDelegate(this.talk_to_garrison_commander_on_consequence), false, 2, false, null);
		}

		private bool talk_to_garrison_commander_on_condition(MenuCallbackArgs args)
		{
			if (LordNeedsGarrisonTroopsIssueQuestBehavior.Instance != null)
			{
				if (Settlement.CurrentSettlement == LordNeedsGarrisonTroopsIssueQuestBehavior.Instance._settlement)
				{
					Town town = LordNeedsGarrisonTroopsIssueQuestBehavior.Instance._settlement.Town;
					if (((town != null) ? town.GarrisonParty : null) == null)
					{
						args.IsEnabled = false;
						args.Tooltip = new TextObject("{=JmoOJX4e}There is no one in the garrison to receive the troops requested. You should wait until someone arrives.", null);
					}
				}
				args.optionLeaveType = GameMenuOption.LeaveType.LeaveTroopsAndFlee;
				args.OptionQuestData = GameMenuOption.IssueQuestFlags.ActiveIssue;
				return Settlement.CurrentSettlement == LordNeedsGarrisonTroopsIssueQuestBehavior.Instance._settlement;
			}
			return false;
		}

		private void talk_to_garrison_commander_on_consequence(MenuCallbackArgs args)
		{
			CharacterObject characterObject = LordNeedsGarrisonTroopsIssueQuestBehavior.Instance._settlement.OwnerClan.Culture.EliteBasicTroop;
			foreach (TroopRosterElement troopRosterElement in LordNeedsGarrisonTroopsIssueQuestBehavior.Instance._settlement.Town.GarrisonParty.MemberRoster.GetTroopRoster())
			{
				if (troopRosterElement.Character.IsInfantry && characterObject.Level < troopRosterElement.Character.Level)
				{
					characterObject = troopRosterElement.Character;
				}
			}
			LordNeedsGarrisonTroopsIssueQuestBehavior.Instance._selectedCharacterToTalk = characterObject;
			ConversationCharacterData conversationCharacterData = new ConversationCharacterData(CharacterObject.PlayerCharacter, PartyBase.MainParty, false, false, false, false, false, false);
			CharacterObject selectedCharacterToTalk = LordNeedsGarrisonTroopsIssueQuestBehavior.Instance._selectedCharacterToTalk;
			Town town = LordNeedsGarrisonTroopsIssueQuestBehavior.Instance._settlement.Town;
			CampaignMapConversation.OpenConversation(conversationCharacterData, new ConversationCharacterData(selectedCharacterToTalk, (town != null) ? town.GarrisonParty.Party : null, false, false, false, false, false, false));
		}

		public override void SyncData(IDataStore dataStore)
		{
		}

		private bool ConditionsHold(Hero issueGiver, out Settlement selectedSettlement)
		{
			selectedSettlement = null;
			if (issueGiver.IsLord && issueGiver.Clan.Leader == issueGiver && !issueGiver.IsMinorFactionHero && issueGiver.Clan != Clan.PlayerClan)
			{
				foreach (Settlement settlement in issueGiver.Clan.Settlements)
				{
					if (settlement.IsCastle)
					{
						MobileParty garrisonParty = settlement.Town.GarrisonParty;
						if (garrisonParty != null && garrisonParty.MemberRoster.TotalHealthyCount < 120)
						{
							selectedSettlement = settlement;
							break;
						}
					}
					if (settlement.IsTown)
					{
						MobileParty garrisonParty2 = settlement.Town.GarrisonParty;
						if (garrisonParty2 != null && garrisonParty2.MemberRoster.TotalHealthyCount < 150)
						{
							selectedSettlement = settlement;
							break;
						}
					}
				}
				return selectedSettlement != null;
			}
			return false;
		}

		public void OnCheckForIssue(Hero hero)
		{
			Settlement settlement;
			if (this.ConditionsHold(hero, out settlement))
			{
				Campaign.Current.IssueManager.AddPotentialIssueData(hero, new PotentialIssueData(new PotentialIssueData.StartIssueDelegate(this.OnSelected), typeof(LordNeedsGarrisonTroopsIssueQuestBehavior.LordNeedsGarrisonTroopsIssue), IssueBase.IssueFrequency.Common, settlement));
				return;
			}
			Campaign.Current.IssueManager.AddPotentialIssueData(hero, new PotentialIssueData(typeof(LordNeedsGarrisonTroopsIssueQuestBehavior.LordNeedsGarrisonTroopsIssue), IssueBase.IssueFrequency.Common));
		}

		private IssueBase OnSelected(in PotentialIssueData pid, Hero issueOwner)
		{
			PotentialIssueData potentialIssueData = pid;
			return new LordNeedsGarrisonTroopsIssueQuestBehavior.LordNeedsGarrisonTroopsIssue(issueOwner, potentialIssueData.RelatedObject as Settlement);
		}

		private const IssueBase.IssueFrequency LordNeedsGarrisonTroopsIssueFrequency = IssueBase.IssueFrequency.Common;

		private LordNeedsGarrisonTroopsIssueQuestBehavior.LordNeedsGarrisonTroopsIssueQuest _cachedQuest;

		public class LordNeedsGarrisonTroopsIssue : IssueBase
		{
			internal static void AutoGeneratedStaticCollectObjectsLordNeedsGarrisonTroopsIssue(object o, List<object> collectedObjects)
			{
				((LordNeedsGarrisonTroopsIssueQuestBehavior.LordNeedsGarrisonTroopsIssue)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				base.AutoGeneratedInstanceCollectObjects(collectedObjects);
				collectedObjects.Add(this._settlement);
				collectedObjects.Add(this._neededTroopType);
			}

			internal static object AutoGeneratedGetMemberValue_settlement(object o)
			{
				return ((LordNeedsGarrisonTroopsIssueQuestBehavior.LordNeedsGarrisonTroopsIssue)o)._settlement;
			}

			internal static object AutoGeneratedGetMemberValue_neededTroopType(object o)
			{
				return ((LordNeedsGarrisonTroopsIssueQuestBehavior.LordNeedsGarrisonTroopsIssue)o)._neededTroopType;
			}

			public override IssueBase.AlternativeSolutionScaleFlag AlternativeSolutionScaleFlags
			{
				get
				{
					return IssueBase.AlternativeSolutionScaleFlag.FailureRisk;
				}
			}

			private int NumberOfTroopToBeRecruited
			{
				get
				{
					return 3 + (int)(base.IssueDifficultyMultiplier * 18f);
				}
			}

			public override int AlternativeSolutionBaseNeededMenCount
			{
				get
				{
					return 5 + MathF.Ceiling(8f * base.IssueDifficultyMultiplier);
				}
			}

			protected override int AlternativeSolutionBaseDurationInDaysInternal
			{
				get
				{
					return 3 + MathF.Ceiling(4f * base.IssueDifficultyMultiplier);
				}
			}

			protected override int RewardGold
			{
				get
				{
					int num = Campaign.Current.Models.PartyWageModel.GetTroopRecruitmentCost(this._neededTroopType, Hero.MainHero, false) * this.NumberOfTroopToBeRecruited;
					return (int)(1500f + (float)num * 1.5f);
				}
			}

			public override TextObject IssueBriefByIssueGiver
			{
				get
				{
					return new TextObject("{=ZuTvTGsh}These wars have taken a toll on my men. The bravest often fall first, they say, and fewer and fewer families are willing to let their sons join my banner. But the wars don't stop because I have problems.[if:convo_undecided_closed][ib:closed]", null);
				}
			}

			public override TextObject IssueAcceptByPlayer
			{
				get
				{
					TextObject textObject = new TextObject("{=tTM6nPul}What can I do for you, {?ISSUE_OWNER.GENDER}madam{?}sir{\\?}?", null);
					StringHelpers.SetCharacterProperties("ISSUE_OWNER", base.IssueOwner.CharacterObject, textObject, false);
					return textObject;
				}
			}

			public override TextObject IssueQuestSolutionExplanationByIssueGiver
			{
				get
				{
					TextObject textObject = new TextObject("{=driH06vI}I need more recruits in {SETTLEMENT}'s garrison. Since I'll be elsewhere... maybe you can recruit {NUMBER_OF_TROOP_TO_BE_RECRUITED} {TROOP_TYPE} and bring them to the garrison for me?[if:convo_undecided_open][ib:normal]", null);
					textObject.SetTextVariable("SETTLEMENT", this._settlement.Name);
					textObject.SetTextVariable("TROOP_TYPE", this._neededTroopType.EncyclopediaLinkWithName);
					textObject.SetTextVariable("NUMBER_OF_TROOP_TO_BE_RECRUITED", this.NumberOfTroopToBeRecruited);
					return textObject;
				}
			}

			public override TextObject IssueAlternativeSolutionExplanationByIssueGiver
			{
				get
				{
					TextObject textObject = new TextObject("{=igXcCqdo}One of your trusted companions who knows how to lead men can go around with {ALTERNATIVE_SOLUTION_MAN_COUNT} horsemen and pick some up. One way or the other I will pay {REWARD_GOLD}{GOLD_ICON} denars in return for your services. What do you say?[if:convo_thinking]", null);
					textObject.SetTextVariable("ALTERNATIVE_SOLUTION_MAN_COUNT", base.GetTotalAlternativeSolutionNeededMenCount());
					textObject.SetTextVariable("REWARD_GOLD", this.RewardGold);
					textObject.SetTextVariable("GOLD_ICON", "{=!}<img src=\"General\\Icons\\Coin@2x\" extend=\"8\">");
					return textObject;
				}
			}

			public override TextObject IssueQuestSolutionAcceptByPlayer
			{
				get
				{
					TextObject textObject = new TextObject("{=YHSm72Ln}I'll find your recruits and bring them to {SETTLEMENT} garrison.", null);
					textObject.SetTextVariable("SETTLEMENT", this._settlement.Name);
					return textObject;
				}
			}

			public override TextObject IssueAlternativeSolutionAcceptByPlayer
			{
				get
				{
					TextObject textObject = new TextObject("{=JPclWyyr}My companion can handle it... So, {NUMBER_OF_TROOP_TO_BE_RECRUITED} {TROOP_TYPE} to {SETTLEMENT}.", null);
					textObject.SetTextVariable("SETTLEMENT", this._settlement.Name);
					textObject.SetTextVariable("TROOP_TYPE", this._neededTroopType.EncyclopediaLinkWithName);
					textObject.SetTextVariable("NUMBER_OF_TROOP_TO_BE_RECRUITED", this.NumberOfTroopToBeRecruited);
					return textObject;
				}
			}

			public override TextObject IssueDiscussAlternativeSolution
			{
				get
				{
					TextObject textObject = new TextObject("{=lWrmxsYR}I haven't heard any news from {SETTLEMENT}, but I realize it might take some time for your men to deliver the recruits.", null);
					textObject.SetTextVariable("SETTLEMENT", this._settlement.Name);
					return textObject;
				}
			}

			public override TextObject IssueAlternativeSolutionResponseByIssueGiver
			{
				get
				{
					return new TextObject("{=WUWzyzWI}Thank you. Your help will be remembered.", null);
				}
			}

			protected override TextObject AlternativeSolutionStartLog
			{
				get
				{
					TextObject textObject = new TextObject("{=M560TDza}{ISSUE_OWNER.LINK}, the {?ISSUE_OWNER.GENDER}lady{?}lord{\\?} of {QUEST_SETTLEMENT}, told you that {?ISSUE_OWNER.GENDER}she{?}he{\\?} needs more troops in {?ISSUE_OWNER.GENDER}her{?}his{\\?} garrison. {?ISSUE_OWNER.GENDER}She{?}He{\\?} is willing to pay {REWARD}{GOLD_ICON} for your services. You asked your companion to deploy {NUMBER_OF_TROOP_TO_BE_RECRUITED} {TROOP_TYPE} troops to {QUEST_SETTLEMENT}'s garrison.", null);
					StringHelpers.SetCharacterProperties("ISSUE_OWNER", base.IssueOwner.CharacterObject, textObject, false);
					textObject.SetTextVariable("QUEST_SETTLEMENT", this._settlement.EncyclopediaLinkWithName);
					textObject.SetTextVariable("TROOP_TYPE", this._neededTroopType.EncyclopediaLinkWithName);
					textObject.SetTextVariable("REWARD", this.RewardGold);
					textObject.SetTextVariable("GOLD_ICON", "{=!}<img src=\"General\\Icons\\Coin@2x\" extend=\"8\">");
					textObject.SetTextVariable("NUMBER_OF_TROOP_TO_BE_RECRUITED", this.NumberOfTroopToBeRecruited);
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
					TextObject textObject = new TextObject("{=g6Ra6LUY}{ISSUE_OWNER.NAME} needs garrison troops in {SETTLEMENT}", null);
					StringHelpers.SetCharacterProperties("ISSUE_OWNER", base.IssueOwner.CharacterObject, textObject, false);
					textObject.SetTextVariable("SETTLEMENT", this._settlement.Name);
					return textObject;
				}
			}

			public override TextObject Description
			{
				get
				{
					TextObject textObject = new TextObject("{=BOAaF6x5}{ISSUE_OWNER.NAME} asks for help to increase troop levels in {SETTLEMENT}", null);
					StringHelpers.SetCharacterProperties("ISSUE_OWNER", base.IssueOwner.CharacterObject, textObject, false);
					textObject.SetTextVariable("SETTLEMENT", this._settlement.Name);
					return textObject;
				}
			}

			public override TextObject IssueAlternativeSolutionSuccessLog
			{
				get
				{
					TextObject textObject = new TextObject("{=sfFkYm0a}Your companion has successfully brought the troops {ISSUE_OWNER.LINK} requested. You received {REWARD}{GOLD_ICON}.", null);
					StringHelpers.SetCharacterProperties("ISSUE_OWNER", base.IssueOwner.CharacterObject, textObject, false);
					textObject.SetTextVariable("REWARD", this.RewardGold);
					textObject.SetTextVariable("GOLD_ICON", "{=!}<img src=\"General\\Icons\\Coin@2x\" extend=\"8\">");
					return textObject;
				}
			}

			public LordNeedsGarrisonTroopsIssue(Hero issueOwner, Settlement selectedSettlement)
				: base(issueOwner, CampaignTime.DaysFromNow(30f))
			{
				this._settlement = selectedSettlement;
				this._neededTroopType = CharacterHelper.GetTroopTree(base.IssueOwner.Culture.BasicTroop, 3f, 3f).GetRandomElementInefficiently<CharacterObject>();
			}

			protected override float GetIssueEffectAmountInternal(IssueEffect issueEffect)
			{
				if (issueEffect == DefaultIssueEffects.SettlementSecurity)
				{
					return -0.5f;
				}
				return 0f;
			}

			public override ValueTuple<SkillObject, int> GetAlternativeSolutionSkill(Hero hero)
			{
				return new ValueTuple<SkillObject, int>((hero.GetSkillValue(DefaultSkills.Leadership) >= hero.GetSkillValue(DefaultSkills.Steward)) ? DefaultSkills.Leadership : DefaultSkills.Steward, 120);
			}

			public override bool DoTroopsSatisfyAlternativeSolution(TroopRoster troopRoster, out TextObject explanation)
			{
				explanation = TextObject.Empty;
				return QuestHelper.CheckRosterForAlternativeSolution(troopRoster, base.GetTotalAlternativeSolutionNeededMenCount(), ref explanation, 0, true);
			}

			public override bool IsTroopTypeNeededByAlternativeSolution(CharacterObject character)
			{
				return character.IsMounted;
			}

			public override bool AlternativeSolutionCondition(out TextObject explanation)
			{
				explanation = TextObject.Empty;
				return QuestHelper.CheckRosterForAlternativeSolution(MobileParty.MainParty.MemberRoster, base.GetTotalAlternativeSolutionNeededMenCount(), ref explanation, 0, true);
			}

			protected override int CompanionSkillRewardXP
			{
				get
				{
					return (int)(800f + 900f * base.IssueDifficultyMultiplier);
				}
			}

			protected override void AlternativeSolutionEndWithSuccessConsequence()
			{
				GainRenownAction.Apply(Hero.MainHero, 1f, false);
				this.RelationshipChangeWithIssueOwner = 2;
			}

			public override IssueBase.IssueFrequency GetFrequency()
			{
				return IssueBase.IssueFrequency.Common;
			}

			public override bool IssueStayAliveConditions()
			{
				bool flag = false;
				if (this._settlement.IsTown)
				{
					MobileParty garrisonParty = this._settlement.Town.GarrisonParty;
					flag = garrisonParty != null && garrisonParty.MemberRoster.TotalRegulars < 200;
				}
				else if (this._settlement.IsCastle)
				{
					MobileParty garrisonParty2 = this._settlement.Town.GarrisonParty;
					flag = garrisonParty2 != null && garrisonParty2.MemberRoster.TotalRegulars < 160;
				}
				return this._settlement.OwnerClan == base.IssueOwner.Clan && flag && !base.IssueOwner.IsDead && base.IssueOwner.Clan != Clan.PlayerClan;
			}

			protected override bool CanPlayerTakeQuestConditions(Hero issueGiver, out IssueBase.PreconditionFlags flags, out Hero relationHero, out SkillObject skill)
			{
				skill = null;
				relationHero = null;
				flags = IssueBase.PreconditionFlags.None;
				if (issueGiver.GetRelationWithPlayer() < -10f)
				{
					flags |= IssueBase.PreconditionFlags.Relation;
					relationHero = issueGiver;
				}
				if (Hero.MainHero.IsKingdomLeader)
				{
					flags |= IssueBase.PreconditionFlags.MainHeroIsKingdomLeader;
				}
				if (issueGiver.MapFaction.IsAtWarWith(Hero.MainHero.MapFaction))
				{
					flags |= IssueBase.PreconditionFlags.AtWar;
				}
				return flags == IssueBase.PreconditionFlags.None;
			}

			protected override void OnGameLoad()
			{
			}

			protected override void HourlyTick()
			{
			}

			protected override QuestBase GenerateIssueQuest(string questId)
			{
				return new LordNeedsGarrisonTroopsIssueQuestBehavior.LordNeedsGarrisonTroopsIssueQuest(questId, base.IssueOwner, CampaignTime.DaysFromNow(30f), this.RewardGold, this._settlement, this.NumberOfTroopToBeRecruited, this._neededTroopType);
			}

			protected override void AlternativeSolutionEndWithFailureConsequence()
			{
				this.RelationshipChangeWithIssueOwner = -5;
			}

			protected override void CompleteIssueWithTimedOutConsequences()
			{
			}

			private const int QuestDurationInDays = 30;

			private const int CompanionRequiredSkillLevel = 120;

			[SaveableField(60)]
			private Settlement _settlement;

			[SaveableField(30)]
			private CharacterObject _neededTroopType;
		}

		public class LordNeedsGarrisonTroopsIssueQuest : QuestBase
		{
			internal static void AutoGeneratedStaticCollectObjectsLordNeedsGarrisonTroopsIssueQuest(object o, List<object> collectedObjects)
			{
				((LordNeedsGarrisonTroopsIssueQuestBehavior.LordNeedsGarrisonTroopsIssueQuest)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				base.AutoGeneratedInstanceCollectObjects(collectedObjects);
				collectedObjects.Add(this._requestedTroopType);
				collectedObjects.Add(this._playerStartsQuestLog);
			}

			internal static object AutoGeneratedGetMemberValue_settlementStringID(object o)
			{
				return ((LordNeedsGarrisonTroopsIssueQuestBehavior.LordNeedsGarrisonTroopsIssueQuest)o)._settlementStringID;
			}

			internal static object AutoGeneratedGetMemberValue_requestedTroopAmount(object o)
			{
				return ((LordNeedsGarrisonTroopsIssueQuestBehavior.LordNeedsGarrisonTroopsIssueQuest)o)._requestedTroopAmount;
			}

			internal static object AutoGeneratedGetMemberValue_rewardGold(object o)
			{
				return ((LordNeedsGarrisonTroopsIssueQuestBehavior.LordNeedsGarrisonTroopsIssueQuest)o)._rewardGold;
			}

			internal static object AutoGeneratedGetMemberValue_requestedTroopType(object o)
			{
				return ((LordNeedsGarrisonTroopsIssueQuestBehavior.LordNeedsGarrisonTroopsIssueQuest)o)._requestedTroopType;
			}

			internal static object AutoGeneratedGetMemberValue_playerStartsQuestLog(object o)
			{
				return ((LordNeedsGarrisonTroopsIssueQuestBehavior.LordNeedsGarrisonTroopsIssueQuest)o)._playerStartsQuestLog;
			}

			public override TextObject Title
			{
				get
				{
					TextObject textObject = new TextObject("{=g6Ra6LUY}{ISSUE_OWNER.NAME} needs garrison troops in {SETTLEMENT}", null);
					StringHelpers.SetCharacterProperties("ISSUE_OWNER", base.QuestGiver.CharacterObject, textObject, false);
					textObject.SetTextVariable("SETTLEMENT", this._settlement.Name);
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
					TextObject textObject = new TextObject("{=FViaQrbV}{QUEST_GIVER.LINK}, the {?QUEST_GIVER.GENDER}lady{?}lord{\\?} of {QUEST_SETTLEMENT}, told you that {?QUEST_GIVER.GENDER}she{?}he{\\?} needs more troops in {?QUEST_GIVER.GENDER}her{?}his{\\?} garrison. {?QUEST_GIVER.GENDER}She{?}He{\\?} is willing to pay {REWARD}{GOLD_ICON} for your services. {?QUEST_GIVER.GENDER}She{?}He{\\?} asked you to deliver {NUMBER_OF_TROOP_TO_BE_RECRUITED} {TROOP_TYPE} troops to garrison commander in {QUEST_SETTLEMENT}.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					textObject.SetTextVariable("TROOP_TYPE", this._requestedTroopType.Name);
					textObject.SetTextVariable("REWARD", this._rewardGold);
					textObject.SetTextVariable("GOLD_ICON", "{=!}<img src=\"General\\Icons\\Coin@2x\" extend=\"8\">");
					textObject.SetTextVariable("NUMBER_OF_TROOP_TO_BE_RECRUITED", this._requestedTroopAmount);
					textObject.SetTextVariable("QUEST_SETTLEMENT", this._settlement.EncyclopediaLinkWithName);
					return textObject;
				}
			}

			private TextObject _successQuestLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=UEn466Y6}You have successfully brought the troops {QUEST_GIVER.LINK} requested. You received {REWARD} gold in return for your service.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					textObject.SetTextVariable("REWARD", this._rewardGold);
					return textObject;
				}
			}

			private TextObject _questGiverLostTheSettlementLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=zS68eOsl}{QUEST_GIVER.LINK} has lost {SETTLEMENT} and your agreement with {?QUEST_GIVER.GENDER}her{?}his{\\?} canceled.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					textObject.SetTextVariable("SETTLEMENT", this._settlement.EncyclopediaLinkWithName);
					return textObject;
				}
			}

			private TextObject _questFailedWarDeclaredLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=JIWVeTMD}Your clan is now at war with {QUEST_GIVER.LINK}'s realm. Your agreement with {QUEST_GIVER.LINK} was canceled.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					textObject.SetTextVariable("SETTLEMENT", this._settlement.EncyclopediaLinkWithName);
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

			private TextObject _timeOutLogText
			{
				get
				{
					return new TextObject("{=cnaxgN5b}You have failed to bring the troops in time.", null);
				}
			}

			public LordNeedsGarrisonTroopsIssueQuest(string questId, Hero giverHero, CampaignTime duration, int rewardGold, Settlement selectedSettlement, int requestedTroopAmount, CharacterObject requestedTroopType)
				: base(questId, giverHero, duration, rewardGold)
			{
				this._settlement = selectedSettlement;
				this._settlementStringID = selectedSettlement.StringId;
				this._requestedTroopAmount = requestedTroopAmount;
				this._collectedTroopAmount = 0;
				this._requestedTroopType = requestedTroopType;
				this._rewardGold = rewardGold;
				this.SetDialogs();
				base.AddTrackedObject(this._settlement);
				base.InitializeQuestOnCreation();
			}

			private bool DialogCondition()
			{
				return Hero.OneToOneConversationHero == base.QuestGiver;
			}

			protected override void SetDialogs()
			{
				Campaign.Current.ConversationManager.AddDialogFlow(this.GetGarrisonCommanderDialogFlow(), this);
				this.OfferDialogFlow = DialogFlow.CreateDialogFlow("issue_classic_quest_start", 100).NpcLine(new TextObject("{=9iZg4vpz}Thank you. You will be rewarded when you are done.[if:convo_mocking_aristocratic]", null), null, null).Condition(new ConversationSentence.OnConditionDelegate(this.DialogCondition))
					.Consequence(new ConversationSentence.OnConsequenceDelegate(this.QuestAcceptedConsequences))
					.CloseDialog();
				this.DiscussDialogFlow = DialogFlow.CreateDialogFlow("quest_discuss", 100).NpcLine(new TextObject("{=o6BunhbE}Have you brought my troops?[if:convo_undecided_open]", null), null, null).Condition(new ConversationSentence.OnConditionDelegate(this.DialogCondition))
					.Consequence(delegate
					{
						Campaign.Current.ConversationManager.ConversationEndOneShot += MapEventHelper.OnConversationEnd;
					})
					.BeginPlayerOptions()
					.PlayerOption(new TextObject("{=eC4laxrj}I'm still out recruiting.", null), null)
					.NpcLine(new TextObject("{=TxxbCbUc}Good. I have faith in you...[if:convo_mocking_aristocratic]", null), null, null)
					.CloseDialog()
					.PlayerOption(new TextObject("{=DbraLcwM}I need more time to find proper men.", null), null)
					.NpcLine(new TextObject("{=Mw5bJ5Fb}Every day without a proper garrison is a day that we're vulnerable. Do hurry, if you can.[if:convo_normal]", null), null, null)
					.CloseDialog()
					.EndPlayerOptions();
			}

			private void QuestAcceptedConsequences()
			{
				base.StartQuest();
				this._playerStartsQuestLog = base.AddDiscreteLog(this._playerStartsQuestLogText, new TextObject("{=WIb9VvEM}Collected Troops", null), this._collectedTroopAmount, this._requestedTroopAmount, null, false);
			}

			private DialogFlow GetGarrisonCommanderDialogFlow()
			{
				TextObject textObject = new TextObject("{=abda9slW}We were waiting for you, {?PLAYER.GENDER}madam{?}sir{\\?}. Have you brought the troops that our {?ISSUE_OWNER.GENDER}lady{?}lord{\\?} requested?", null);
				StringHelpers.SetCharacterProperties("ISSUE_OWNER", base.QuestGiver.CharacterObject, textObject, false);
				return DialogFlow.CreateDialogFlow("start", 300).NpcLine(textObject, null, null).Condition(() => CharacterObject.OneToOneConversationCharacter == this._selectedCharacterToTalk)
					.BeginPlayerOptions()
					.PlayerOption(new TextObject("{=ooHbl6JU}Here are your men.", null), null)
					.ClickableCondition(new ConversationSentence.OnClickableConditionDelegate(this.PlayerGiveTroopsToGarrisonCommanderCondition))
					.NpcLine(new TextObject("{=Ouy4sN5b}Thank you.[if:convo_mocking_aristocratic]", null), null, null)
					.Consequence(delegate
					{
						Campaign.Current.ConversationManager.ConversationEndOneShot += this.PlayerTransferredTroopsToGarrisonCommander;
					})
					.CloseDialog()
					.PlayerOption(new TextObject("{=G5tyQj6N}Not yet.", null), null)
					.NpcLine(new TextObject("{=yPOZd1wb}Very well. We'll keep waiting.[if:convo_normal]", null), null, null)
					.CloseDialog()
					.EndPlayerOptions();
			}

			private void PlayerTransferredTroopsToGarrisonCommander()
			{
				using (List<TroopRosterElement>.Enumerator enumerator = MobileParty.MainParty.MemberRoster.GetTroopRoster().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.Character == this._requestedTroopType)
						{
							MobileParty.MainParty.MemberRoster.AddToCounts(this._requestedTroopType, -this._requestedTroopAmount, false, 0, 0, true, -1);
							break;
						}
					}
				}
				base.AddLog(this._successQuestLogText, false);
				this.RelationshipChangeWithQuestGiver = 2;
				GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, this._rewardGold, false);
				base.CompleteQuestWithSuccess();
			}

			private bool PlayerGiveTroopsToGarrisonCommanderCondition(out TextObject explanation)
			{
				int num = 0;
				foreach (TroopRosterElement troopRosterElement in MobileParty.MainParty.MemberRoster.GetTroopRoster())
				{
					if (troopRosterElement.Character == this._requestedTroopType)
					{
						num = troopRosterElement.Number;
						break;
					}
				}
				if (num < this._requestedTroopAmount)
				{
					explanation = new TextObject("{=VFO2aQ4l}You don't have enough men.", null);
					return false;
				}
				explanation = TextObject.Empty;
				return true;
			}

			protected override void InitializeQuestOnGameLoad()
			{
				this._settlement = Settlement.Find(this._settlementStringID);
				this.CalculateTroopAmount();
				this.SetDialogs();
			}

			protected override void RegisterEvents()
			{
				CampaignEvents.OnSettlementOwnerChangedEvent.AddNonSerializedListener(this, new Action<Settlement, bool, Hero, Hero, Hero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail>(this.OnSettlementOwnerChanged));
				CampaignEvents.WarDeclared.AddNonSerializedListener(this, new Action<IFaction, IFaction, DeclareWarAction.DeclareWarDetail>(this.OnWarDeclared));
				CampaignEvents.ClanChangedKingdom.AddNonSerializedListener(this, new Action<Clan, Kingdom, Kingdom, ChangeKingdomAction.ChangeKingdomActionDetail, bool>(this.OnClanChangedKingdom));
				CampaignEvents.MapEventStarted.AddNonSerializedListener(this, new Action<MapEvent, PartyBase, PartyBase>(this.OnMapEventStarted));
			}

			private void OnMapEventStarted(MapEvent mapEvent, PartyBase attackerParty, PartyBase defenderParty)
			{
				if (QuestHelper.CheckMinorMajorCoercion(this, mapEvent, attackerParty))
				{
					QuestHelper.ApplyGenericMinorMajorCoercionConsequences(this, mapEvent);
				}
			}

			private void OnSettlementOwnerChanged(Settlement settlement, bool openToClaim, Hero newOwner, Hero oldOwner, Hero capturerHero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail detail)
			{
				if (settlement == this._settlement && this._settlement.OwnerClan != base.QuestGiver.Clan)
				{
					base.AddLog(this._questGiverLostTheSettlementLogText, false);
					base.CompleteQuestWithCancel(null);
				}
			}

			protected override void HourlyTick()
			{
				if (base.IsOngoing)
				{
					this.CalculateTroopAmount();
					this._collectedTroopAmount = MBMath.ClampInt(this._collectedTroopAmount, 0, this._requestedTroopAmount);
					this._playerStartsQuestLog.UpdateCurrentProgress(this._collectedTroopAmount);
				}
			}

			private void CalculateTroopAmount()
			{
				foreach (TroopRosterElement troopRosterElement in MobileParty.MainParty.MemberRoster.GetTroopRoster())
				{
					if (troopRosterElement.Character == this._requestedTroopType)
					{
						this._collectedTroopAmount = MobileParty.MainParty.MemberRoster.GetTroopCount(troopRosterElement.Character);
						break;
					}
				}
			}

			private void OnClanChangedKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail detail, bool showNotification = true)
			{
				if (base.QuestGiver.MapFaction.IsAtWarWith(Hero.MainHero.MapFaction))
				{
					base.CompleteQuestWithCancel(this._questFailedWarDeclaredLogText);
				}
			}

			private void OnWarDeclared(IFaction faction1, IFaction faction2, DeclareWarAction.DeclareWarDetail detail)
			{
				QuestHelper.CheckWarDeclarationAndFailOrCancelTheQuest(this, faction1, faction2, detail, this._playerDeclaredWarQuestLogText, this._questFailedWarDeclaredLogText, false);
			}

			protected override void OnTimedOut()
			{
				base.AddLog(this._timeOutLogText, false);
				this.RelationshipChangeWithQuestGiver = -5;
			}

			internal Settlement _settlement;

			[SaveableField(10)]
			private string _settlementStringID;

			private int _collectedTroopAmount;

			[SaveableField(20)]
			private int _requestedTroopAmount;

			[SaveableField(30)]
			private int _rewardGold;

			[SaveableField(40)]
			private CharacterObject _requestedTroopType;

			internal CharacterObject _selectedCharacterToTalk;

			[SaveableField(50)]
			private JournalLog _playerStartsQuestLog;
		}

		public class LordNeedsGarrisonTroopsIssueQuestTypeDefiner : SaveableTypeDefiner
		{
			public LordNeedsGarrisonTroopsIssueQuestTypeDefiner()
				: base(5080000)
			{
			}

			protected override void DefineClassTypes()
			{
				base.AddClassDefinition(typeof(LordNeedsGarrisonTroopsIssueQuestBehavior.LordNeedsGarrisonTroopsIssue), 1, null);
				base.AddClassDefinition(typeof(LordNeedsGarrisonTroopsIssueQuestBehavior.LordNeedsGarrisonTroopsIssueQuest), 2, null);
			}
		}
	}
}
