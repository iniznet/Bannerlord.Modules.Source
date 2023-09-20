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
	// Token: 0x02000315 RID: 789
	public class LordNeedsGarrisonTroopsIssueQuestBehavior : CampaignBehaviorBase
	{
		// Token: 0x17000AD3 RID: 2771
		// (get) Token: 0x06002D0F RID: 11535 RVA: 0x000BBF4C File Offset: 0x000BA14C
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

		// Token: 0x06002D10 RID: 11536 RVA: 0x000BBFE4 File Offset: 0x000BA1E4
		public override void RegisterEvents()
		{
			CampaignEvents.OnCheckForIssueEvent.AddNonSerializedListener(this, new Action<Hero>(this.OnCheckForIssue));
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
		}

		// Token: 0x06002D11 RID: 11537 RVA: 0x000BC014 File Offset: 0x000BA214
		private void OnSessionLaunched(CampaignGameStarter gameStarter)
		{
			string text = "{=FirEOQaI}Talk to the garrison commander";
			gameStarter.AddGameMenuOption("town", "talk_to_garrison_commander_town", text, new GameMenuOption.OnConditionDelegate(this.talk_to_garrison_commander_on_condition), new GameMenuOption.OnConsequenceDelegate(this.talk_to_garrison_commander_on_consequence), false, 2, false, null);
			gameStarter.AddGameMenuOption("town_guard", "talk_to_garrison_commander_town", text, new GameMenuOption.OnConditionDelegate(this.talk_to_garrison_commander_on_condition), new GameMenuOption.OnConsequenceDelegate(this.talk_to_garrison_commander_on_consequence), false, 2, false, null);
			gameStarter.AddGameMenuOption("castle_guard", "talk_to_garrison_commander_castle", text, new GameMenuOption.OnConditionDelegate(this.talk_to_garrison_commander_on_condition), new GameMenuOption.OnConsequenceDelegate(this.talk_to_garrison_commander_on_consequence), false, 2, false, null);
		}

		// Token: 0x06002D12 RID: 11538 RVA: 0x000BC0B0 File Offset: 0x000BA2B0
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

		// Token: 0x06002D13 RID: 11539 RVA: 0x000BC12C File Offset: 0x000BA32C
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

		// Token: 0x06002D14 RID: 11540 RVA: 0x000BC22C File Offset: 0x000BA42C
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x06002D15 RID: 11541 RVA: 0x000BC230 File Offset: 0x000BA430
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

		// Token: 0x06002D16 RID: 11542 RVA: 0x000BC324 File Offset: 0x000BA524
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

		// Token: 0x06002D17 RID: 11543 RVA: 0x000BC38C File Offset: 0x000BA58C
		private IssueBase OnSelected(in PotentialIssueData pid, Hero issueOwner)
		{
			PotentialIssueData potentialIssueData = pid;
			return new LordNeedsGarrisonTroopsIssueQuestBehavior.LordNeedsGarrisonTroopsIssue(issueOwner, potentialIssueData.RelatedObject as Settlement);
		}

		// Token: 0x04000D94 RID: 3476
		private const IssueBase.IssueFrequency LordNeedsGarrisonTroopsIssueFrequency = IssueBase.IssueFrequency.Common;

		// Token: 0x04000D95 RID: 3477
		private LordNeedsGarrisonTroopsIssueQuestBehavior.LordNeedsGarrisonTroopsIssueQuest _cachedQuest;

		// Token: 0x02000642 RID: 1602
		public class LordNeedsGarrisonTroopsIssue : IssueBase
		{
			// Token: 0x06004DB7 RID: 19895 RVA: 0x0015926D File Offset: 0x0015746D
			internal static void AutoGeneratedStaticCollectObjectsLordNeedsGarrisonTroopsIssue(object o, List<object> collectedObjects)
			{
				((LordNeedsGarrisonTroopsIssueQuestBehavior.LordNeedsGarrisonTroopsIssue)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			// Token: 0x06004DB8 RID: 19896 RVA: 0x0015927B File Offset: 0x0015747B
			protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				base.AutoGeneratedInstanceCollectObjects(collectedObjects);
				collectedObjects.Add(this._settlement);
				collectedObjects.Add(this._neededTroopType);
			}

			// Token: 0x06004DB9 RID: 19897 RVA: 0x0015929C File Offset: 0x0015749C
			internal static object AutoGeneratedGetMemberValue_settlement(object o)
			{
				return ((LordNeedsGarrisonTroopsIssueQuestBehavior.LordNeedsGarrisonTroopsIssue)o)._settlement;
			}

			// Token: 0x06004DBA RID: 19898 RVA: 0x001592A9 File Offset: 0x001574A9
			internal static object AutoGeneratedGetMemberValue_neededTroopType(object o)
			{
				return ((LordNeedsGarrisonTroopsIssueQuestBehavior.LordNeedsGarrisonTroopsIssue)o)._neededTroopType;
			}

			// Token: 0x1700107A RID: 4218
			// (get) Token: 0x06004DBB RID: 19899 RVA: 0x001592B6 File Offset: 0x001574B6
			public override IssueBase.AlternativeSolutionScaleFlag AlternativeSolutionScaleFlags
			{
				get
				{
					return IssueBase.AlternativeSolutionScaleFlag.FailureRisk;
				}
			}

			// Token: 0x1700107B RID: 4219
			// (get) Token: 0x06004DBC RID: 19900 RVA: 0x001592B9 File Offset: 0x001574B9
			private int NumberOfTroopToBeRecruited
			{
				get
				{
					return 3 + (int)(base.IssueDifficultyMultiplier * 18f);
				}
			}

			// Token: 0x1700107C RID: 4220
			// (get) Token: 0x06004DBD RID: 19901 RVA: 0x001592CA File Offset: 0x001574CA
			public override int AlternativeSolutionBaseNeededMenCount
			{
				get
				{
					return 5 + MathF.Ceiling(8f * base.IssueDifficultyMultiplier);
				}
			}

			// Token: 0x1700107D RID: 4221
			// (get) Token: 0x06004DBE RID: 19902 RVA: 0x001592DF File Offset: 0x001574DF
			protected override int AlternativeSolutionBaseDurationInDaysInternal
			{
				get
				{
					return 3 + MathF.Ceiling(4f * base.IssueDifficultyMultiplier);
				}
			}

			// Token: 0x1700107E RID: 4222
			// (get) Token: 0x06004DBF RID: 19903 RVA: 0x001592F4 File Offset: 0x001574F4
			protected override int RewardGold
			{
				get
				{
					int num = Campaign.Current.Models.PartyWageModel.GetTroopRecruitmentCost(this._neededTroopType, Hero.MainHero, false) * this.NumberOfTroopToBeRecruited;
					return (int)(1500f + (float)num * 1.5f);
				}
			}

			// Token: 0x1700107F RID: 4223
			// (get) Token: 0x06004DC0 RID: 19904 RVA: 0x00159338 File Offset: 0x00157538
			public override TextObject IssueBriefByIssueGiver
			{
				get
				{
					return new TextObject("{=ZuTvTGsh}These wars have taken a toll on my men. The bravest often fall first, they say, and fewer and fewer families are willing to let their sons join my banner. But the wars don't stop because I have problems.", null);
				}
			}

			// Token: 0x17001080 RID: 4224
			// (get) Token: 0x06004DC1 RID: 19905 RVA: 0x00159348 File Offset: 0x00157548
			public override TextObject IssueAcceptByPlayer
			{
				get
				{
					TextObject textObject = new TextObject("{=tTM6nPul}What can I do for you, {?ISSUE_OWNER.GENDER}madam{?}sir{\\?}?", null);
					StringHelpers.SetCharacterProperties("ISSUE_OWNER", base.IssueOwner.CharacterObject, textObject, false);
					return textObject;
				}
			}

			// Token: 0x17001081 RID: 4225
			// (get) Token: 0x06004DC2 RID: 19906 RVA: 0x0015937C File Offset: 0x0015757C
			public override TextObject IssueQuestSolutionExplanationByIssueGiver
			{
				get
				{
					TextObject textObject = new TextObject("{=driH06vI}I need more recruits in {SETTLEMENT}'s garrison. Since I'll be elsewhere... maybe you can recruit {NUMBER_OF_TROOP_TO_BE_RECRUITED} {TROOP_TYPE} and bring them to the garrison for me?", null);
					textObject.SetTextVariable("SETTLEMENT", this._settlement.Name);
					textObject.SetTextVariable("TROOP_TYPE", this._neededTroopType.EncyclopediaLinkWithName);
					textObject.SetTextVariable("NUMBER_OF_TROOP_TO_BE_RECRUITED", this.NumberOfTroopToBeRecruited);
					return textObject;
				}
			}

			// Token: 0x17001082 RID: 4226
			// (get) Token: 0x06004DC3 RID: 19907 RVA: 0x001593D4 File Offset: 0x001575D4
			public override TextObject IssueAlternativeSolutionExplanationByIssueGiver
			{
				get
				{
					TextObject textObject = new TextObject("{=igXcCqdo}One of your trusted companions who knows how to lead men can go around with {ALTERNATIVE_SOLUTION_MAN_COUNT} horsemen and pick some up. One way or the other I will pay {REWARD_GOLD}{GOLD_ICON} denars in return for your services. What do you say?", null);
					textObject.SetTextVariable("ALTERNATIVE_SOLUTION_MAN_COUNT", base.GetTotalAlternativeSolutionNeededMenCount());
					textObject.SetTextVariable("REWARD_GOLD", this.RewardGold);
					textObject.SetTextVariable("GOLD_ICON", "{=!}<img src=\"General\\Icons\\Coin@2x\" extend=\"8\">");
					return textObject;
				}
			}

			// Token: 0x17001083 RID: 4227
			// (get) Token: 0x06004DC4 RID: 19908 RVA: 0x00159421 File Offset: 0x00157621
			public override TextObject IssueQuestSolutionAcceptByPlayer
			{
				get
				{
					TextObject textObject = new TextObject("{=YHSm72Ln}I'll find your recruits and bring them to {SETTLEMENT} garrison.", null);
					textObject.SetTextVariable("SETTLEMENT", this._settlement.Name);
					return textObject;
				}
			}

			// Token: 0x17001084 RID: 4228
			// (get) Token: 0x06004DC5 RID: 19909 RVA: 0x00159448 File Offset: 0x00157648
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

			// Token: 0x17001085 RID: 4229
			// (get) Token: 0x06004DC6 RID: 19910 RVA: 0x001594A0 File Offset: 0x001576A0
			public override TextObject IssueDiscussAlternativeSolution
			{
				get
				{
					TextObject textObject = new TextObject("{=lWrmxsYR}I haven't heard any news from {SETTLEMENT}, but I realize it might take some time for your men to deliver the recruits.", null);
					textObject.SetTextVariable("SETTLEMENT", this._settlement.Name);
					return textObject;
				}
			}

			// Token: 0x17001086 RID: 4230
			// (get) Token: 0x06004DC7 RID: 19911 RVA: 0x001594C4 File Offset: 0x001576C4
			public override TextObject IssueAlternativeSolutionResponseByIssueGiver
			{
				get
				{
					return new TextObject("{=WUWzyzWI}Thank you. Your help will be remembered.", null);
				}
			}

			// Token: 0x17001087 RID: 4231
			// (get) Token: 0x06004DC8 RID: 19912 RVA: 0x001594D4 File Offset: 0x001576D4
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

			// Token: 0x17001088 RID: 4232
			// (get) Token: 0x06004DC9 RID: 19913 RVA: 0x00159569 File Offset: 0x00157769
			public override bool IsThereAlternativeSolution
			{
				get
				{
					return true;
				}
			}

			// Token: 0x17001089 RID: 4233
			// (get) Token: 0x06004DCA RID: 19914 RVA: 0x0015956C File Offset: 0x0015776C
			public override bool IsThereLordSolution
			{
				get
				{
					return false;
				}
			}

			// Token: 0x1700108A RID: 4234
			// (get) Token: 0x06004DCB RID: 19915 RVA: 0x00159570 File Offset: 0x00157770
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

			// Token: 0x1700108B RID: 4235
			// (get) Token: 0x06004DCC RID: 19916 RVA: 0x001595BC File Offset: 0x001577BC
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

			// Token: 0x1700108C RID: 4236
			// (get) Token: 0x06004DCD RID: 19917 RVA: 0x00159608 File Offset: 0x00157808
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

			// Token: 0x06004DCE RID: 19918 RVA: 0x00159660 File Offset: 0x00157860
			public LordNeedsGarrisonTroopsIssue(Hero issueOwner, Settlement selectedSettlement)
				: base(issueOwner, CampaignTime.DaysFromNow(30f))
			{
				this._settlement = selectedSettlement;
				this._neededTroopType = CharacterHelper.GetTroopTree(base.IssueOwner.Culture.BasicTroop, 3f, 3f).GetRandomElementInefficiently<CharacterObject>();
			}

			// Token: 0x06004DCF RID: 19919 RVA: 0x001596AF File Offset: 0x001578AF
			protected override float GetIssueEffectAmountInternal(IssueEffect issueEffect)
			{
				if (issueEffect == DefaultIssueEffects.SettlementSecurity)
				{
					return -0.5f;
				}
				return 0f;
			}

			// Token: 0x06004DD0 RID: 19920 RVA: 0x001596C4 File Offset: 0x001578C4
			public override ValueTuple<SkillObject, int> GetAlternativeSolutionSkill(Hero hero)
			{
				return new ValueTuple<SkillObject, int>((hero.GetSkillValue(DefaultSkills.Leadership) >= hero.GetSkillValue(DefaultSkills.Steward)) ? DefaultSkills.Leadership : DefaultSkills.Steward, 120);
			}

			// Token: 0x06004DD1 RID: 19921 RVA: 0x001596F1 File Offset: 0x001578F1
			public override bool DoTroopsSatisfyAlternativeSolution(TroopRoster troopRoster, out TextObject explanation)
			{
				explanation = TextObject.Empty;
				return QuestHelper.CheckRosterForAlternativeSolution(troopRoster, base.GetTotalAlternativeSolutionNeededMenCount(), ref explanation, 0, true);
			}

			// Token: 0x06004DD2 RID: 19922 RVA: 0x00159709 File Offset: 0x00157909
			public override bool IsTroopTypeNeededByAlternativeSolution(CharacterObject character)
			{
				return character.IsMounted;
			}

			// Token: 0x06004DD3 RID: 19923 RVA: 0x00159711 File Offset: 0x00157911
			public override bool AlternativeSolutionCondition(out TextObject explanation)
			{
				explanation = TextObject.Empty;
				return QuestHelper.CheckRosterForAlternativeSolution(MobileParty.MainParty.MemberRoster, base.GetTotalAlternativeSolutionNeededMenCount(), ref explanation, 0, true);
			}

			// Token: 0x1700108D RID: 4237
			// (get) Token: 0x06004DD4 RID: 19924 RVA: 0x00159732 File Offset: 0x00157932
			protected override int CompanionSkillRewardXP
			{
				get
				{
					return (int)(800f + 900f * base.IssueDifficultyMultiplier);
				}
			}

			// Token: 0x06004DD5 RID: 19925 RVA: 0x00159747 File Offset: 0x00157947
			protected override void AlternativeSolutionEndWithSuccessConsequence()
			{
				GainRenownAction.Apply(Hero.MainHero, 1f, false);
				this.RelationshipChangeWithIssueOwner = 2;
			}

			// Token: 0x06004DD6 RID: 19926 RVA: 0x00159760 File Offset: 0x00157960
			public override IssueBase.IssueFrequency GetFrequency()
			{
				return IssueBase.IssueFrequency.Common;
			}

			// Token: 0x06004DD7 RID: 19927 RVA: 0x00159764 File Offset: 0x00157964
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

			// Token: 0x06004DD8 RID: 19928 RVA: 0x00159824 File Offset: 0x00157A24
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
				if (Hero.MainHero.MapFaction.IsKingdomFaction && Hero.MainHero.IsFactionLeader)
				{
					flags |= IssueBase.PreconditionFlags.MainHeroIsKingdomLeader;
				}
				if (issueGiver.MapFaction.IsAtWarWith(Hero.MainHero.MapFaction))
				{
					flags |= IssueBase.PreconditionFlags.AtWar;
				}
				return flags == IssueBase.PreconditionFlags.None;
			}

			// Token: 0x06004DD9 RID: 19929 RVA: 0x0015989B File Offset: 0x00157A9B
			protected override void OnGameLoad()
			{
			}

			// Token: 0x06004DDA RID: 19930 RVA: 0x0015989D File Offset: 0x00157A9D
			protected override QuestBase GenerateIssueQuest(string questId)
			{
				return new LordNeedsGarrisonTroopsIssueQuestBehavior.LordNeedsGarrisonTroopsIssueQuest(questId, base.IssueOwner, CampaignTime.DaysFromNow(30f), this.RewardGold, this._settlement, this.NumberOfTroopToBeRecruited, this._neededTroopType);
			}

			// Token: 0x06004DDB RID: 19931 RVA: 0x001598CD File Offset: 0x00157ACD
			protected override void AlternativeSolutionEndWithFailureConsequence()
			{
				this.RelationshipChangeWithIssueOwner = -5;
			}

			// Token: 0x06004DDC RID: 19932 RVA: 0x001598D7 File Offset: 0x00157AD7
			protected override void CompleteIssueWithTimedOutConsequences()
			{
			}

			// Token: 0x04001A3A RID: 6714
			private const int QuestDurationInDays = 30;

			// Token: 0x04001A3B RID: 6715
			private const int CompanionRequiredSkillLevel = 120;

			// Token: 0x04001A3C RID: 6716
			[SaveableField(60)]
			private Settlement _settlement;

			// Token: 0x04001A3D RID: 6717
			[SaveableField(30)]
			private CharacterObject _neededTroopType;
		}

		// Token: 0x02000643 RID: 1603
		public class LordNeedsGarrisonTroopsIssueQuest : QuestBase
		{
			// Token: 0x06004DDD RID: 19933 RVA: 0x001598D9 File Offset: 0x00157AD9
			internal static void AutoGeneratedStaticCollectObjectsLordNeedsGarrisonTroopsIssueQuest(object o, List<object> collectedObjects)
			{
				((LordNeedsGarrisonTroopsIssueQuestBehavior.LordNeedsGarrisonTroopsIssueQuest)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			// Token: 0x06004DDE RID: 19934 RVA: 0x001598E7 File Offset: 0x00157AE7
			protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				base.AutoGeneratedInstanceCollectObjects(collectedObjects);
				collectedObjects.Add(this._requestedTroopType);
				collectedObjects.Add(this._playerStartsQuestLog);
			}

			// Token: 0x06004DDF RID: 19935 RVA: 0x00159908 File Offset: 0x00157B08
			internal static object AutoGeneratedGetMemberValue_settlementStringID(object o)
			{
				return ((LordNeedsGarrisonTroopsIssueQuestBehavior.LordNeedsGarrisonTroopsIssueQuest)o)._settlementStringID;
			}

			// Token: 0x06004DE0 RID: 19936 RVA: 0x00159915 File Offset: 0x00157B15
			internal static object AutoGeneratedGetMemberValue_requestedTroopAmount(object o)
			{
				return ((LordNeedsGarrisonTroopsIssueQuestBehavior.LordNeedsGarrisonTroopsIssueQuest)o)._requestedTroopAmount;
			}

			// Token: 0x06004DE1 RID: 19937 RVA: 0x00159927 File Offset: 0x00157B27
			internal static object AutoGeneratedGetMemberValue_rewardGold(object o)
			{
				return ((LordNeedsGarrisonTroopsIssueQuestBehavior.LordNeedsGarrisonTroopsIssueQuest)o)._rewardGold;
			}

			// Token: 0x06004DE2 RID: 19938 RVA: 0x00159939 File Offset: 0x00157B39
			internal static object AutoGeneratedGetMemberValue_requestedTroopType(object o)
			{
				return ((LordNeedsGarrisonTroopsIssueQuestBehavior.LordNeedsGarrisonTroopsIssueQuest)o)._requestedTroopType;
			}

			// Token: 0x06004DE3 RID: 19939 RVA: 0x00159946 File Offset: 0x00157B46
			internal static object AutoGeneratedGetMemberValue_playerStartsQuestLog(object o)
			{
				return ((LordNeedsGarrisonTroopsIssueQuestBehavior.LordNeedsGarrisonTroopsIssueQuest)o)._playerStartsQuestLog;
			}

			// Token: 0x1700108E RID: 4238
			// (get) Token: 0x06004DE4 RID: 19940 RVA: 0x00159954 File Offset: 0x00157B54
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

			// Token: 0x1700108F RID: 4239
			// (get) Token: 0x06004DE5 RID: 19941 RVA: 0x0015999D File Offset: 0x00157B9D
			public override bool IsRemainingTimeHidden
			{
				get
				{
					return false;
				}
			}

			// Token: 0x17001090 RID: 4240
			// (get) Token: 0x06004DE6 RID: 19942 RVA: 0x001599A0 File Offset: 0x00157BA0
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

			// Token: 0x17001091 RID: 4241
			// (get) Token: 0x06004DE7 RID: 19943 RVA: 0x00159A38 File Offset: 0x00157C38
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

			// Token: 0x17001092 RID: 4242
			// (get) Token: 0x06004DE8 RID: 19944 RVA: 0x00159A7C File Offset: 0x00157C7C
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

			// Token: 0x17001093 RID: 4243
			// (get) Token: 0x06004DE9 RID: 19945 RVA: 0x00159AC8 File Offset: 0x00157CC8
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

			// Token: 0x17001094 RID: 4244
			// (get) Token: 0x06004DEA RID: 19946 RVA: 0x00159B14 File Offset: 0x00157D14
			private TextObject _playerDeclaredWarQuestLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=bqeWVVEE}Your actions have started a war with {QUEST_GIVER.LINK}'s faction. {?QUEST_GIVER.GENDER}She{?}He{\\?} cancels your agreement and the quest is a failure.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					return textObject;
				}
			}

			// Token: 0x17001095 RID: 4245
			// (get) Token: 0x06004DEB RID: 19947 RVA: 0x00159B46 File Offset: 0x00157D46
			private TextObject _timeOutLogText
			{
				get
				{
					return new TextObject("{=cnaxgN5b}You have failed to bring the troops in time.", null);
				}
			}

			// Token: 0x06004DEC RID: 19948 RVA: 0x00159B54 File Offset: 0x00157D54
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

			// Token: 0x06004DED RID: 19949 RVA: 0x00159BB8 File Offset: 0x00157DB8
			private bool DialogCondition()
			{
				return Hero.OneToOneConversationHero == base.QuestGiver;
			}

			// Token: 0x06004DEE RID: 19950 RVA: 0x00159BC8 File Offset: 0x00157DC8
			protected override void SetDialogs()
			{
				Campaign.Current.ConversationManager.AddDialogFlow(this.GetGarrisonCommanderDialogFlow(), this);
				this.OfferDialogFlow = DialogFlow.CreateDialogFlow("issue_classic_quest_start", 100).NpcLine(new TextObject("{=9iZg4vpz}Thank you. You will be rewarded when you are done.", null), null, null).Condition(new ConversationSentence.OnConditionDelegate(this.DialogCondition))
					.Consequence(new ConversationSentence.OnConsequenceDelegate(this.QuestAcceptedConsequences))
					.CloseDialog();
				this.DiscussDialogFlow = DialogFlow.CreateDialogFlow("quest_discuss", 100).NpcLine(new TextObject("{=o6BunhbE}Have you brought my troops?", null), null, null).Condition(new ConversationSentence.OnConditionDelegate(this.DialogCondition))
					.Consequence(delegate
					{
						Campaign.Current.ConversationManager.ConversationEndOneShot += MapEventHelper.OnConversationEnd;
					})
					.BeginPlayerOptions()
					.PlayerOption(new TextObject("{=eC4laxrj}I'm still out recruiting.", null), null)
					.NpcLine(new TextObject("{=TxxbCbUc}Good. I have faith in you...", null), null, null)
					.CloseDialog()
					.PlayerOption(new TextObject("{=DbraLcwM}I need more time to find proper men.", null), null)
					.NpcLine(new TextObject("{=Mw5bJ5Fb}Every day without a proper garrison is a day that we're vulnerable. Do hurry, if you can.", null), null, null)
					.CloseDialog()
					.EndPlayerOptions();
			}

			// Token: 0x06004DEF RID: 19951 RVA: 0x00159CE9 File Offset: 0x00157EE9
			private void QuestAcceptedConsequences()
			{
				base.StartQuest();
				this._playerStartsQuestLog = base.AddDiscreteLog(this._playerStartsQuestLogText, new TextObject("{=WIb9VvEM}Collected Troops", null), this._collectedTroopAmount, this._requestedTroopAmount, null, false);
			}

			// Token: 0x06004DF0 RID: 19952 RVA: 0x00159D1C File Offset: 0x00157F1C
			private DialogFlow GetGarrisonCommanderDialogFlow()
			{
				TextObject textObject = new TextObject("{=abda9slW}We were waiting for you, {?PLAYER.GENDER}madam{?}sir{\\?}. Have you brought the troops that our {?ISSUE_OWNER.GENDER}lady{?}lord{\\?} requested?", null);
				StringHelpers.SetCharacterProperties("ISSUE_OWNER", base.QuestGiver.CharacterObject, textObject, false);
				return DialogFlow.CreateDialogFlow("start", 300).NpcLine(textObject, null, null).Condition(() => CharacterObject.OneToOneConversationCharacter == this._selectedCharacterToTalk)
					.BeginPlayerOptions()
					.PlayerOption(new TextObject("{=ooHbl6JU}Here are your men.", null), null)
					.ClickableCondition(new ConversationSentence.OnClickableConditionDelegate(this.PlayerGiveTroopsToGarrisonCommanderCondition))
					.NpcLine(new TextObject("{=g8qb3Ame}Thank you.", null), null, null)
					.Consequence(delegate
					{
						Campaign.Current.ConversationManager.ConversationEndOneShot += this.PlayerTransferredTroopsToGarrisonCommander;
					})
					.CloseDialog()
					.PlayerOption(new TextObject("{=G5tyQj6N}Not yet.", null), null)
					.NpcLine(new TextObject("{=sjTpEzju}Very well. We'll keep waiting.", null), null, null)
					.CloseDialog()
					.EndPlayerOptions();
			}

			// Token: 0x06004DF1 RID: 19953 RVA: 0x00159DF4 File Offset: 0x00157FF4
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

			// Token: 0x06004DF2 RID: 19954 RVA: 0x00159EA8 File Offset: 0x001580A8
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

			// Token: 0x06004DF3 RID: 19955 RVA: 0x00159F38 File Offset: 0x00158138
			protected override void InitializeQuestOnGameLoad()
			{
				this._settlement = Settlement.Find(this._settlementStringID);
				this.CalculateTroopAmount();
				this.SetDialogs();
			}

			// Token: 0x06004DF4 RID: 19956 RVA: 0x00159F58 File Offset: 0x00158158
			protected override void RegisterEvents()
			{
				CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(this.OnHourlyTick));
				CampaignEvents.OnSettlementOwnerChangedEvent.AddNonSerializedListener(this, new Action<Settlement, bool, Hero, Hero, Hero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail>(this.OnSettlementOwnerChanged));
				CampaignEvents.WarDeclared.AddNonSerializedListener(this, new Action<IFaction, IFaction, DeclareWarAction.DeclareWarDetail>(this.OnWarDeclared));
				CampaignEvents.ClanChangedKingdom.AddNonSerializedListener(this, new Action<Clan, Kingdom, Kingdom, ChangeKingdomAction.ChangeKingdomActionDetail, bool>(this.OnClanChangedKingdom));
				CampaignEvents.MapEventStarted.AddNonSerializedListener(this, new Action<MapEvent, PartyBase, PartyBase>(this.OnMapEventStarted));
			}

			// Token: 0x06004DF5 RID: 19957 RVA: 0x00159FD8 File Offset: 0x001581D8
			private void OnMapEventStarted(MapEvent mapEvent, PartyBase attackerParty, PartyBase defenderParty)
			{
				if (QuestHelper.CheckMinorMajorCoercion(this, mapEvent, attackerParty))
				{
					QuestHelper.ApplyGenericMinorMajorCoercionConsequences(this, mapEvent);
				}
			}

			// Token: 0x06004DF6 RID: 19958 RVA: 0x00159FEB File Offset: 0x001581EB
			public void OnSettlementOwnerChanged(Settlement settlement, bool openToClaim, Hero newOwner, Hero oldOwner, Hero capturerHero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail detail)
			{
				if (settlement == this._settlement && this._settlement.OwnerClan != base.QuestGiver.Clan)
				{
					base.AddLog(this._questGiverLostTheSettlementLogText, false);
					base.CompleteQuestWithCancel(null);
				}
			}

			// Token: 0x06004DF7 RID: 19959 RVA: 0x0015A023 File Offset: 0x00158223
			public void OnHourlyTick()
			{
				if (base.IsOngoing)
				{
					this.CalculateTroopAmount();
					this._collectedTroopAmount = MBMath.ClampInt(this._collectedTroopAmount, 0, this._requestedTroopAmount);
					this._playerStartsQuestLog.UpdateCurrentProgress(this._collectedTroopAmount);
				}
			}

			// Token: 0x06004DF8 RID: 19960 RVA: 0x0015A05C File Offset: 0x0015825C
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

			// Token: 0x06004DF9 RID: 19961 RVA: 0x0015A0DC File Offset: 0x001582DC
			private void OnClanChangedKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail detail, bool showNotification = true)
			{
				if (base.QuestGiver.MapFaction.IsAtWarWith(Hero.MainHero.MapFaction))
				{
					base.CompleteQuestWithCancel(this._questFailedWarDeclaredLogText);
				}
			}

			// Token: 0x06004DFA RID: 19962 RVA: 0x0015A106 File Offset: 0x00158306
			private void OnWarDeclared(IFaction faction1, IFaction faction2, DeclareWarAction.DeclareWarDetail detail)
			{
				QuestHelper.CheckWarDeclarationAndFailOrCancelTheQuest(this, faction1, faction2, detail, this._playerDeclaredWarQuestLogText, this._questFailedWarDeclaredLogText);
			}

			// Token: 0x06004DFB RID: 19963 RVA: 0x0015A11D File Offset: 0x0015831D
			protected override void OnTimedOut()
			{
				base.AddLog(this._timeOutLogText, false);
				this.RelationshipChangeWithQuestGiver = -5;
			}

			// Token: 0x04001A3E RID: 6718
			internal Settlement _settlement;

			// Token: 0x04001A3F RID: 6719
			[SaveableField(10)]
			private string _settlementStringID;

			// Token: 0x04001A40 RID: 6720
			private int _collectedTroopAmount;

			// Token: 0x04001A41 RID: 6721
			[SaveableField(20)]
			private int _requestedTroopAmount;

			// Token: 0x04001A42 RID: 6722
			[SaveableField(30)]
			private int _rewardGold;

			// Token: 0x04001A43 RID: 6723
			[SaveableField(40)]
			private CharacterObject _requestedTroopType;

			// Token: 0x04001A44 RID: 6724
			internal CharacterObject _selectedCharacterToTalk;

			// Token: 0x04001A45 RID: 6725
			[SaveableField(50)]
			private JournalLog _playerStartsQuestLog;
		}

		// Token: 0x02000644 RID: 1604
		public class LordNeedsGarrisonTroopsIssueQuestTypeDefiner : SaveableTypeDefiner
		{
			// Token: 0x06004DFE RID: 19966 RVA: 0x0015A161 File Offset: 0x00158361
			public LordNeedsGarrisonTroopsIssueQuestTypeDefiner()
				: base(5080000)
			{
			}

			// Token: 0x06004DFF RID: 19967 RVA: 0x0015A16E File Offset: 0x0015836E
			protected override void DefineClassTypes()
			{
				base.AddClassDefinition(typeof(LordNeedsGarrisonTroopsIssueQuestBehavior.LordNeedsGarrisonTroopsIssue), 1, null);
				base.AddClassDefinition(typeof(LordNeedsGarrisonTroopsIssueQuestBehavior.LordNeedsGarrisonTroopsIssueQuest), 2, null);
			}
		}
	}
}
