using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using SandBox.BoardGames.MissionLogics;
using SandBox.CampaignBehaviors;
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
using TaleWorlds.SaveSystem;

namespace SandBox.Issues
{
	public class RuralNotableInnAndOutIssueBehavior : CampaignBehaviorBase
	{
		public override void RegisterEvents()
		{
			CampaignEvents.OnCheckForIssueEvent.AddNonSerializedListener(this, new Action<Hero>(this.OnCheckForIssue));
		}

		public override void SyncData(IDataStore dataStore)
		{
		}

		private bool ConditionsHold(Hero issueGiver)
		{
			return (issueGiver.IsRuralNotable || issueGiver.IsHeadman) && issueGiver.CurrentSettlement.Village != null && issueGiver.CurrentSettlement.Village.Bound.IsTown && issueGiver.GetTraitLevel(DefaultTraits.Mercy) + issueGiver.GetTraitLevel(DefaultTraits.Honor) < 0 && Campaign.Current.GetCampaignBehavior<BoardGameCampaignBehavior>() != null && issueGiver.CurrentSettlement.Village.Bound.Culture.BoardGame != -1;
		}

		public void OnCheckForIssue(Hero hero)
		{
			if (this.ConditionsHold(hero))
			{
				Campaign.Current.IssueManager.AddPotentialIssueData(hero, new PotentialIssueData(new PotentialIssueData.StartIssueDelegate(this.OnSelected), typeof(RuralNotableInnAndOutIssueBehavior.RuralNotableInnAndOutIssue), 1, null));
				return;
			}
			Campaign.Current.IssueManager.AddPotentialIssueData(hero, new PotentialIssueData(typeof(RuralNotableInnAndOutIssueBehavior.RuralNotableInnAndOutIssue), 1));
		}

		private IssueBase OnSelected(in PotentialIssueData pid, Hero issueOwner)
		{
			return new RuralNotableInnAndOutIssueBehavior.RuralNotableInnAndOutIssue(issueOwner);
		}

		private const IssueBase.IssueFrequency RuralNotableInnAndOutIssueFrequency = 1;

		private const float IssueDuration = 30f;

		private const float QuestDuration = 14f;

		public class RuralNotableInnAndOutIssueTypeDefiner : SaveableTypeDefiner
		{
			public RuralNotableInnAndOutIssueTypeDefiner()
				: base(585900)
			{
			}

			protected override void DefineClassTypes()
			{
				base.AddClassDefinition(typeof(RuralNotableInnAndOutIssueBehavior.RuralNotableInnAndOutIssue), 1, null);
				base.AddClassDefinition(typeof(RuralNotableInnAndOutIssueBehavior.RuralNotableInnAndOutIssueQuest), 2, null);
			}
		}

		public class RuralNotableInnAndOutIssue : IssueBase
		{
			public override IssueBase.AlternativeSolutionScaleFlag AlternativeSolutionScaleFlags
			{
				get
				{
					return 8;
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
					return 1 + MathF.Ceiling(3f * base.IssueDifficultyMultiplier);
				}
			}

			protected override int AlternativeSolutionBaseDurationInDaysInternal
			{
				get
				{
					return 1 + MathF.Ceiling(3f * base.IssueDifficultyMultiplier);
				}
			}

			protected override int RewardGold
			{
				get
				{
					return 1000;
				}
			}

			public override TextObject Title
			{
				get
				{
					return new TextObject("{=uUhtKnfA}Inn and Out", null);
				}
			}

			public override TextObject Description
			{
				get
				{
					TextObject textObject = new TextObject("{=swamqBRq}{ISSUE_OWNER.NAME} wants you to beat the game host", null);
					StringHelpers.SetCharacterProperties("ISSUE_OWNER", base.IssueOwner.CharacterObject, textObject, false);
					return textObject;
				}
			}

			public override TextObject IssueBriefByIssueGiver
			{
				get
				{
					TextObject textObject = new TextObject("{=T0zupcGB}Ah yes... It is a bit embarrassing to mention, but... Well, when I am in town, I often have a drink at the inn and perhaps play a round of {GAME_TYPE} or two. Normally I play for low stakes but let's just say that last time the wine went to my head, and I lost something I couldn't afford to lose.", null);
					textObject.SetTextVariable("GAME_TYPE", GameTexts.FindText("str_boardgame_name", this._boardGameType.ToString()));
					return textObject;
				}
			}

			public override TextObject IssueAcceptByPlayer
			{
				get
				{
					return new TextObject("{=h2tMadtI}I've heard that story before. What did you lose?", null);
				}
			}

			public override TextObject IssueQuestSolutionExplanationByIssueGiver
			{
				get
				{
					TextObject textObject = new TextObject("{=LD4tGYCA}It's a deed to a plot of farmland. Not a big or valuable plot, mind you, but I'd rather not have to explain to my men why they won't be sowing it this year. You can find the man who took it from me at the tavern in {TARGET_SETTLEMENT}. They call him the \"Game Host\". Just be straight about what you're doing. He's in no position to work the land. I don't imagine that he'll turn down a chance to make more money off of it. Bring it back and {REWARD}{GOLD_ICON} is yours.", null);
					textObject.SetTextVariable("REWARD", this.RewardGold);
					textObject.SetTextVariable("GOLD_ICON", "{=!}<img src=\"General\\Icons\\Coin@2x\" extend=\"8\">");
					textObject.SetTextVariable("TARGET_SETTLEMENT", this._targetSettlement.Name);
					return textObject;
				}
			}

			public override TextObject IssueAlternativeSolutionExplanationByIssueGiver
			{
				get
				{
					TextObject textObject = new TextObject("{=urCXu9Fc}Well, I could try and buy it from him, but I would not really prefer that. I would be the joke of the tavern for months to come... If you choose to do that, I can only offer {REWARD}{GOLD_ICON} to compensate for your payment. If you have a man with a knack for such games he might do the trick.", null);
					textObject.SetTextVariable("REWARD", this.RewardGold);
					textObject.SetTextVariable("GOLD_ICON", "{=!}<img src=\"General\\Icons\\Coin@2x\" extend=\"8\">");
					return textObject;
				}
			}

			public override TextObject IssueQuestSolutionAcceptByPlayer
			{
				get
				{
					return new TextObject("{=KMThnMbt}I'll go to the tavern and win it back the same way you lost it.", null);
				}
			}

			public override TextObject IssueAlternativeSolutionAcceptByPlayer
			{
				get
				{
					TextObject textObject = new TextObject("{=QdKWaabR}Worry not {ISSUE_OWNER.NAME}, my men will be back with your deed in no time.", null);
					StringHelpers.SetCharacterProperties("ISSUE_OWNER", base.IssueOwner.CharacterObject, textObject, false);
					return textObject;
				}
			}

			public override TextObject IssueDiscussAlternativeSolution
			{
				get
				{
					return new TextObject("{=1yEyUHJe}I really hope your men can get my deed back. On my father's name, I will never gamble again.", null);
				}
			}

			public override TextObject IssueAlternativeSolutionResponseByIssueGiver
			{
				get
				{
					TextObject textObject = new TextObject("{=kiaN39yb}Thank you, {PLAYER.NAME}. I'm sure your companion will be persuasive.", null);
					StringHelpers.SetCharacterProperties("PLAYER", CharacterObject.PlayerCharacter, textObject, false);
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

			protected override TextObject AlternativeSolutionStartLog
			{
				get
				{
					TextObject textObject = new TextObject("{=MIxzaqzi}{QUEST_GIVER.LINK} told you that he lost a land deed in a wager in {TARGET_CITY}. He needs to buy it back, and he wants your companions to intimidate the seller into offering a reasonable price. You asked {COMPANION.LINK} to take {TROOP_COUNT} of your men to go and take care of it. They should report back to you in {RETURN_DAYS} days.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.IssueOwner.CharacterObject, textObject, false);
					StringHelpers.SetCharacterProperties("COMPANION", base.AlternativeSolutionHero.CharacterObject, textObject, false);
					textObject.SetTextVariable("TARGET_CITY", this._targetSettlement.EncyclopediaLinkWithName);
					textObject.SetTextVariable("RETURN_DAYS", base.GetTotalAlternativeSolutionDurationInDays());
					textObject.SetTextVariable("TROOP_COUNT", this.AlternativeSolutionSentTroops.TotalManCount - 1);
					return textObject;
				}
			}

			public RuralNotableInnAndOutIssue(Hero issueOwner)
				: base(issueOwner, CampaignTime.DaysFromNow(30f))
			{
				this.InitializeQuestVariables();
			}

			protected override float GetIssueEffectAmountInternal(IssueEffect issueEffect)
			{
				if (issueEffect == DefaultIssueEffects.VillageHearth)
				{
					return -0.1f;
				}
				if (issueEffect == DefaultIssueEffects.IssueOwnerPower)
				{
					return -0.1f;
				}
				return 0f;
			}

			public override ValueTuple<SkillObject, int> GetAlternativeSolutionSkill(Hero hero)
			{
				return new ValueTuple<SkillObject, int>((hero.GetSkillValue(DefaultSkills.Charm) >= hero.GetSkillValue(DefaultSkills.Tactics)) ? DefaultSkills.Charm : DefaultSkills.Tactics, 120);
			}

			public override bool AlternativeSolutionCondition(out TextObject explanation)
			{
				explanation = TextObject.Empty;
				return QuestHelper.CheckRosterForAlternativeSolution(MobileParty.MainParty.MemberRoster, base.GetTotalAlternativeSolutionNeededMenCount(), ref explanation, 0, false) && QuestHelper.CheckGoldForAlternativeSolution(1000, ref explanation);
			}

			protected override int CompanionSkillRewardXP
			{
				get
				{
					return (int)(500f + 1000f * base.IssueDifficultyMultiplier);
				}
			}

			protected override void AlternativeSolutionEndWithSuccessConsequence()
			{
				this.RelationshipChangeWithIssueOwner = 5;
				GainRenownAction.Apply(Hero.MainHero, 5f, false);
				base.IssueOwner.CurrentSettlement.Village.Bound.Town.Loyalty += 5f;
			}

			protected override void AlternativeSolutionEndWithFailureConsequence()
			{
				this.RelationshipChangeWithIssueOwner -= 5;
				base.IssueOwner.CurrentSettlement.Village.Bound.Town.Loyalty -= 5f;
			}

			public override bool DoTroopsSatisfyAlternativeSolution(TroopRoster troopRoster, out TextObject explanation)
			{
				explanation = TextObject.Empty;
				return QuestHelper.CheckRosterForAlternativeSolution(troopRoster, base.GetTotalAlternativeSolutionNeededMenCount(), ref explanation, 0, false);
			}

			public override IssueBase.IssueFrequency GetFrequency()
			{
				return 1;
			}

			public override bool IssueStayAliveConditions()
			{
				BoardGameCampaignBehavior campaignBehavior = Campaign.Current.GetCampaignBehavior<BoardGameCampaignBehavior>();
				return campaignBehavior != null && !campaignBehavior.WonBoardGamesInOneWeekInSettlement.Contains(this._targetSettlement) && !base.IssueOwner.CurrentSettlement.IsRaided && !base.IssueOwner.CurrentSettlement.IsUnderRaid;
			}

			protected override void CompleteIssueWithTimedOutConsequences()
			{
			}

			private void InitializeQuestVariables()
			{
				this._targetSettlement = base.IssueOwner.CurrentSettlement.Village.Bound;
				this._boardGameType = this._targetSettlement.Culture.BoardGame;
			}

			protected override void OnGameLoad()
			{
				this.InitializeQuestVariables();
			}

			protected override QuestBase GenerateIssueQuest(string questId)
			{
				return new RuralNotableInnAndOutIssueBehavior.RuralNotableInnAndOutIssueQuest(questId, base.IssueOwner, CampaignTime.DaysFromNow(14f), this.RewardGold);
			}

			protected override bool CanPlayerTakeQuestConditions(Hero issueGiver, out IssueBase.PreconditionFlags flag, out Hero relationHero, out SkillObject skill)
			{
				skill = null;
				relationHero = null;
				flag = 0;
				if (issueGiver.GetRelationWithPlayer() < -10f)
				{
					flag = (int)(flag | 1U);
					relationHero = issueGiver;
				}
				if (FactionManager.IsAtWarAgainstFaction(issueGiver.CurrentSettlement.MapFaction, Hero.MainHero.MapFaction))
				{
					flag = (int)(flag | 64U);
				}
				if (Hero.MainHero.Gold < 2000)
				{
					flag = (int)(flag | 4U);
				}
				return flag == 0U;
			}

			internal static void AutoGeneratedStaticCollectObjectsRuralNotableInnAndOutIssue(object o, List<object> collectedObjects)
			{
				((RuralNotableInnAndOutIssueBehavior.RuralNotableInnAndOutIssue)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			private const int CompanionSkillLimit = 120;

			private const int QuestMoneyLimit = 2000;

			private const int AlternativeSolutionGoldCost = 1000;

			private CultureObject.BoardGameType _boardGameType;

			private Settlement _targetSettlement;
		}

		public class RuralNotableInnAndOutIssueQuest : QuestBase
		{
			private TextObject _questStartLog
			{
				get
				{
					TextObject textObject = new TextObject("{=tirG1BB2}{QUEST_GIVER.LINK} told you that he lost a land deed while playing games in a tavern in {TARGET_SETTLEMENT}. He wants you to go find the game host and win it back for him. You told him that you will take care of the situation yourself.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					textObject.SetTextVariable("TARGET_SETTLEMENT", this._targetSettlement.EncyclopediaLinkWithName);
					return textObject;
				}
			}

			private TextObject _successLog
			{
				get
				{
					TextObject textObject = new TextObject("{=bvhWLb4C}You defeated the Game Host and got the deed back. {QUEST_GIVER.LINK}.{newline}\"Thank you for resolving this issue so neatly. Please accept these {GOLD}{GOLD_ICON} denars with our gratitude.\"", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					textObject.SetTextVariable("GOLD", this.RewardGold);
					textObject.SetTextVariable("GOLD_ICON", "{=!}<img src=\"General\\Icons\\Coin@2x\" extend=\"8\">");
					return textObject;
				}
			}

			private TextObject _successWithPayingLog
			{
				get
				{
					TextObject textObject = new TextObject("{=TIPxWsYW}You have bought the deed from the game host. {QUEST_GIVER.LINK}.{newline}\"I am happy that I got my land back. I'm not so happy that everyone knows I had to pay for it, but... Anyway, please accept these {GOLD}{GOLD_ICON} denars with my gratitude.\"", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					textObject.SetTextVariable("GOLD", 800);
					textObject.SetTextVariable("GOLD_ICON", "{=!}<img src=\"General\\Icons\\Coin@2x\" extend=\"8\">");
					return textObject;
				}
			}

			private TextObject _lostLog
			{
				get
				{
					TextObject textObject = new TextObject("{=ye4oqBFB}You lost the board game and failed to help {QUEST_GIVER.LINK}. \"Thank you for trying, {PLAYER.NAME}, but I guess I chose the wrong person for the job.\"", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					StringHelpers.SetCharacterProperties("PLAYER", CharacterObject.PlayerCharacter, textObject, false);
					return textObject;
				}
			}

			private TextObject _questCanceledTargetVillageRaided
			{
				get
				{
					TextObject textObject = new TextObject("{=YGVTXNrf}{SETTLEMENT} was raided, Your agreement with {QUEST_GIVER.LINK} is canceled.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					textObject.SetTextVariable("SETTLEMENT", this._targetSettlement.EncyclopediaLinkWithName);
					return textObject;
				}
			}

			private TextObject _questCanceledWarDeclared
			{
				get
				{
					TextObject textObject = new TextObject("{=cKz1cyuM}Your clan is now at war with {QUEST_GIVER_SETTLEMENT_FACTION}. Quest is canceled.", null);
					textObject.SetTextVariable("QUEST_GIVER_SETTLEMENT_FACTION", base.QuestGiver.CurrentSettlement.MapFaction.Name);
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

			private TextObject _questCanceledSettlementIsUnderSiege
			{
				get
				{
					TextObject textObject = new TextObject("{=b5LdBYpF}{SETTLEMENT} is under siege. Your agreement with {QUEST_GIVER.LINK} is canceled.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					textObject.SetTextVariable("SETTLEMENT", this._targetSettlement.EncyclopediaLinkWithName);
					return textObject;
				}
			}

			private TextObject _timeoutLog
			{
				get
				{
					TextObject textObject = new TextObject("{=XLy8anVr}You received a message from {QUEST_GIVER.LINK}. \"This may not have seemed like an important task, but I placed my trust in you. I guess I was wrong to do so.\"", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					return textObject;
				}
			}

			public override TextObject Title
			{
				get
				{
					return new TextObject("{=uUhtKnfA}Inn and Out", null);
				}
			}

			public override bool IsRemainingTimeHidden
			{
				get
				{
					return false;
				}
			}

			public RuralNotableInnAndOutIssueQuest(string questId, Hero giverHero, CampaignTime duration, int rewardGold)
				: base(questId, giverHero, duration, rewardGold)
			{
				this.InitializeQuestVariables();
				this.SetDialogs();
				base.InitializeQuestOnCreation();
			}

			private void InitializeQuestVariables()
			{
				this._targetSettlement = base.QuestGiver.CurrentSettlement.Village.Bound;
				this._boardGameType = this._targetSettlement.Culture.BoardGame;
			}

			private void QuestAcceptedConsequences()
			{
				base.StartQuest();
				base.AddLog(this._questStartLog, false);
				base.AddTrackedObject(this._targetSettlement);
			}

			protected override void InitializeQuestOnGameLoad()
			{
				this.InitializeQuestVariables();
				this.SetDialogs();
				if (Campaign.Current.GetCampaignBehavior<BoardGameCampaignBehavior>() == null)
				{
					base.CompleteQuestWithCancel(null);
				}
			}

			protected override void RegisterEvents()
			{
				CampaignEvents.OnPlayerBoardGameOverEvent.AddNonSerializedListener(this, new Action<Hero, BoardGameHelper.BoardGameState>(this.OnBoardGameEnd));
				CampaignEvents.WarDeclared.AddNonSerializedListener(this, new Action<IFaction, IFaction, DeclareWarAction.DeclareWarDetail>(this.OnWarDeclared));
				CampaignEvents.ClanChangedKingdom.AddNonSerializedListener(this, new Action<Clan, Kingdom, Kingdom, ChangeKingdomAction.ChangeKingdomActionDetail, bool>(this.OnClanChangedKingdom));
				CampaignEvents.OnSiegeEventStartedEvent.AddNonSerializedListener(this, new Action<SiegeEvent>(this.OnSiegeStarted));
				CampaignEvents.MapEventStarted.AddNonSerializedListener(this, new Action<MapEvent, PartyBase, PartyBase>(this.OnMapEventStarted));
				CampaignEvents.VillageBeingRaided.AddNonSerializedListener(this, new Action<Village>(this.OnVillageBeingRaided));
			}

			private void OnMapEventStarted(MapEvent mapEvent, PartyBase attackerParty, PartyBase defenderParty)
			{
				if (QuestHelper.CheckMinorMajorCoercion(this, mapEvent, attackerParty))
				{
					QuestHelper.ApplyGenericMinorMajorCoercionConsequences(this, mapEvent);
				}
			}

			private void OnVillageBeingRaided(Village village)
			{
				if (village == base.QuestGiver.CurrentSettlement.Village)
				{
					base.CompleteQuestWithCancel(this._questCanceledTargetVillageRaided);
				}
			}

			private void OnBoardGameEnd(Hero opposingHero, BoardGameHelper.BoardGameState state)
			{
				if (this._checkForBoardGameEnd)
				{
					this._playerWonTheGame = state == 1;
				}
			}

			private void OnSiegeStarted(SiegeEvent siegeEvent)
			{
				if (siegeEvent.BesiegedSettlement == this._targetSettlement)
				{
					base.CompleteQuestWithCancel(this._questCanceledSettlementIsUnderSiege);
				}
			}

			protected override void SetDialogs()
			{
				TextObject textObject = new TextObject("{=I6amLvVE}Good, good. That's the best way to do these things. Go to {TARGET_SETTLEMENT}, find this game host and wipe the smirk off of his face.", null);
				textObject.SetTextVariable("TARGET_SETTLEMENT", this._targetSettlement.Name);
				this.OfferDialogFlow = DialogFlow.CreateDialogFlow("issue_classic_quest_start", 100).NpcLine(textObject, null, null).Condition(() => Hero.OneToOneConversationHero == base.QuestGiver)
					.Consequence(new ConversationSentence.OnConsequenceDelegate(this.QuestAcceptedConsequences))
					.CloseDialog();
				this.DiscussDialogFlow = DialogFlow.CreateDialogFlow("quest_discuss", 100).NpcLine(new TextObject("{=HGRWs0zE}Have you met the man who took my deed? Did you get it back?", null), null, null).Condition(() => Hero.OneToOneConversationHero == base.QuestGiver)
					.BeginPlayerOptions()
					.PlayerOption(new TextObject("{=uJPAYUU7}I will be on my way soon enough.", null), null)
					.NpcLine(new TextObject("{=MOmePlJQ}Could you hurry this along? I don't want him to find another buyer. Thank you.", null), null, null)
					.CloseDialog()
					.PlayerOption(new TextObject("{=azVhRGik}I am waiting for the right moment.", null), null)
					.NpcLine(new TextObject("{=bRMLn0jj}Well, if he wanders off to another town, or gets his throat slit, or loses the deed, that would be the wrong moment, now wouldn't it?", null), null, null)
					.CloseDialog()
					.EndPlayerOptions();
				Campaign.Current.ConversationManager.AddDialogFlow(this.GetGameHostDialogFlow(), this);
				Campaign.Current.ConversationManager.AddDialogFlow(this.GetGameHostDialogueAfterFirstGame(), this);
			}

			private DialogFlow GetGameHostDialogFlow()
			{
				return DialogFlow.CreateDialogFlow("start", 125).NpcLine("{=dzWioKRa}Hello there, are you looking for a friendly match? A wager perhaps?", null, null).Condition(() => this.TavernHostDialogCondition(true))
					.PlayerLine(new TextObject("{=eOle8pYT}You won a deed of land from my associate. I'm here to win it back.", null), null)
					.NpcLine("{=bEipgE5E}Ah, yes, these are the most interesting kinds of games, aren't they? I won't deny myself the pleasure but clearly that deed is worth more to him than just the value of the land. I'll wager the deed, but you need to put up 1000 denars.", null, null)
					.BeginPlayerOptions()
					.PlayerOption("{=XvkSbY6N}I see your wager. Let's play.", null)
					.Condition(() => Hero.MainHero.Gold >= 1000)
					.Consequence(new ConversationSentence.OnConsequenceDelegate(this.StartBoardGame))
					.CloseDialog()
					.PlayerOption("{=89b5ao7P}As of now, I do not have 1000 denars to afford on gambling. I may get back to you once I get the required amount.", null)
					.Condition(() => Hero.MainHero.Gold < 1000)
					.NpcLine(new TextObject("{=ppi6eVos}As you wish.", null), null, null)
					.CloseDialog()
					.PlayerOption("{=WrnvRayQ}Let's just save ourselves some trouble, and I'll just pay you that amount.", null)
					.ClickableCondition(new ConversationSentence.OnClickableConditionDelegate(this.CheckPlayerHasEnoughDenarsClickableCondition))
					.NpcLine("{=pa3RY39w}Sure. I'm happy to turn paper into silver... 1000 denars it is.", null, null)
					.Consequence(new ConversationSentence.OnConsequenceDelegate(this.PlayerPaid1000QuestSuccess))
					.CloseDialog()
					.PlayerOption("{=BSeplVwe}That's too much. I will be back later.", null)
					.CloseDialog()
					.EndPlayerOptions()
					.CloseDialog();
			}

			private DialogFlow GetGameHostDialogueAfterFirstGame()
			{
				return DialogFlow.CreateDialogFlow("start", 125).BeginNpcOptions().NpcOption(new TextObject("{=dyhZUHao}Well, I thought you were here to be sheared, but it looks like the sheep bites back. Very well, nicely played, here's your man's land back.", null), () => this._playerWonTheGame && this.TavernHostDialogCondition(false), null, null)
					.Consequence(new ConversationSentence.OnConsequenceDelegate(this.PlayerWonTheBoardGame))
					.CloseDialog()
					.NpcOption("{=TdnD29Ax}Ah! You almost had me! Maybe you just weren't paying attention. Care to put another 1000 denars on the table and have another go?", () => !this._playerWonTheGame && this._tryCount < 2 && this.TavernHostDialogCondition(false), null, null)
					.BeginPlayerOptions()
					.PlayerOption("{=fiMZ696A}Yes, I'll play again.", null)
					.ClickableCondition(new ConversationSentence.OnClickableConditionDelegate(this.CheckPlayerHasEnoughDenarsClickableCondition))
					.Consequence(new ConversationSentence.OnConsequenceDelegate(this.StartBoardGame))
					.CloseDialog()
					.PlayerOption("{=zlFSIvD5}No, no. I know a trap when I see one. You win. Good-bye.", null)
					.NpcLine(new TextObject("{=ppi6eVos}As you wish.", null), null, null)
					.Consequence(new ConversationSentence.OnConsequenceDelegate(this.PlayerFailAfterBoardGame))
					.CloseDialog()
					.EndPlayerOptions()
					.NpcOption("{=hkNrC5d3}That was fun, but I've learned not to inflict too great a humiliation on those who carry a sword. I'll take my winnings and enjoy them now. Good-bye to you!", () => this._tryCount >= 2 && this.TavernHostDialogCondition(false), null, null)
					.Consequence(new ConversationSentence.OnConsequenceDelegate(this.PlayerFailAfterBoardGame))
					.CloseDialog()
					.EndNpcOptions();
			}

			private bool CheckPlayerHasEnoughDenarsClickableCondition(out TextObject explanation)
			{
				if (Hero.MainHero.Gold >= 1000)
				{
					explanation = TextObject.Empty;
					return true;
				}
				explanation = new TextObject("{=AMlaYbJv}You don't have 1000 denars.", null);
				return false;
			}

			private bool TavernHostDialogCondition(bool isInitialDialogue = false)
			{
				if ((!this._checkForBoardGameEnd || !isInitialDialogue) && Settlement.CurrentSettlement == this._targetSettlement && CharacterObject.OneToOneConversationCharacter.Occupation == 14)
				{
					LocationComplex locationComplex = LocationComplex.Current;
					if (((locationComplex != null) ? locationComplex.GetLocationWithId("tavern") : null) != null)
					{
						Mission.Current.GetMissionBehavior<MissionBoardGameLogic>().DetectOpposingAgent();
						return Mission.Current.GetMissionBehavior<MissionBoardGameLogic>().CheckIfBothSidesAreSitting();
					}
				}
				return false;
			}

			private void PlayerPaid1000QuestSuccess()
			{
				base.AddLog(this._successWithPayingLog, false);
				this._applyLesserReward = true;
				GiveGoldAction.ApplyBetweenCharacters(Hero.MainHero, null, 1000, false);
				base.CompleteQuestWithSuccess();
			}

			private void ApplySuccessRewards()
			{
				GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, this._applyLesserReward ? 800 : this.RewardGold, false);
				ChangeRelationAction.ApplyPlayerRelation(base.QuestGiver, 5, true, true);
				GainRenownAction.Apply(Hero.MainHero, 1f, false);
				base.QuestGiver.CurrentSettlement.Village.Bound.Town.Loyalty += 5f;
			}

			protected override void OnCompleteWithSuccess()
			{
				this.ApplySuccessRewards();
			}

			private void StartBoardGame()
			{
				MissionBoardGameLogic missionBehavior = Mission.Current.GetMissionBehavior<MissionBoardGameLogic>();
				Campaign.Current.GetCampaignBehavior<BoardGameCampaignBehavior>().SetBetAmount(1000);
				missionBehavior.DetectOpposingAgent();
				missionBehavior.SetCurrentDifficulty(1);
				missionBehavior.SetBoardGame(this._boardGameType);
				missionBehavior.StartBoardGame();
				this._checkForBoardGameEnd = true;
				this._tryCount++;
			}

			private void PlayerWonTheBoardGame()
			{
				base.AddLog(this._successLog, false);
				base.CompleteQuestWithSuccess();
			}

			private void PlayerFailAfterBoardGame()
			{
				base.AddLog(this._lostLog, false);
				this.RelationshipChangeWithQuestGiver = -5;
				base.QuestGiver.CurrentSettlement.Village.Bound.Town.Loyalty -= 5f;
				base.CompleteQuestWithFail(null);
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
				QuestHelper.CheckWarDeclarationAndFailOrCancelTheQuest(this, faction1, faction2, detail, this._playerDeclaredWarQuestLogText, this._questCanceledWarDeclared);
			}

			public override GameMenuOption.IssueQuestFlags IsLocationTrackedByQuest(Location location)
			{
				if (PlayerEncounter.LocationEncounter.Settlement == this._targetSettlement && location.StringId == "tavern")
				{
					return 2;
				}
				return 0;
			}

			protected override void OnTimedOut()
			{
				this.RelationshipChangeWithQuestGiver = -5;
				base.QuestGiver.CurrentSettlement.Village.Bound.Town.Loyalty -= 5f;
				base.AddLog(this._timeoutLog, false);
			}

			internal static void AutoGeneratedStaticCollectObjectsRuralNotableInnAndOutIssueQuest(object o, List<object> collectedObjects)
			{
				((RuralNotableInnAndOutIssueBehavior.RuralNotableInnAndOutIssueQuest)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			internal static object AutoGeneratedGetMemberValue_tryCount(object o)
			{
				return ((RuralNotableInnAndOutIssueBehavior.RuralNotableInnAndOutIssueQuest)o)._tryCount;
			}

			public const int LesserReward = 800;

			private CultureObject.BoardGameType _boardGameType;

			private Settlement _targetSettlement;

			private bool _checkForBoardGameEnd;

			private bool _playerWonTheGame;

			private bool _applyLesserReward;

			[SaveableField(1)]
			private int _tryCount;
		}
	}
}
