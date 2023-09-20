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
	// Token: 0x0200007F RID: 127
	public class RuralNotableInnAndOutIssueBehavior : CampaignBehaviorBase
	{
		// Token: 0x0600056D RID: 1389 RVA: 0x00026A64 File Offset: 0x00024C64
		public override void RegisterEvents()
		{
			CampaignEvents.OnCheckForIssueEvent.AddNonSerializedListener(this, new Action<Hero>(this.OnCheckForIssue));
		}

		// Token: 0x0600056E RID: 1390 RVA: 0x00026A7D File Offset: 0x00024C7D
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x0600056F RID: 1391 RVA: 0x00026A80 File Offset: 0x00024C80
		private bool ConditionsHold(Hero issueGiver)
		{
			return (issueGiver.IsRuralNotable || issueGiver.IsHeadman) && issueGiver.CurrentSettlement.Village != null && issueGiver.CurrentSettlement.Village.Bound.IsTown && issueGiver.GetTraitLevel(DefaultTraits.Mercy) + issueGiver.GetTraitLevel(DefaultTraits.Honor) < 0 && Campaign.Current.GetCampaignBehavior<BoardGameCampaignBehavior>() != null && issueGiver.CurrentSettlement.Village.Bound.Culture.BoardGame != -1;
		}

		// Token: 0x06000570 RID: 1392 RVA: 0x00026B0C File Offset: 0x00024D0C
		public void OnCheckForIssue(Hero hero)
		{
			if (this.ConditionsHold(hero))
			{
				Campaign.Current.IssueManager.AddPotentialIssueData(hero, new PotentialIssueData(new PotentialIssueData.StartIssueDelegate(this.OnSelected), typeof(RuralNotableInnAndOutIssueBehavior.RuralNotableInnAndOutIssue), 1, null));
				return;
			}
			Campaign.Current.IssueManager.AddPotentialIssueData(hero, new PotentialIssueData(typeof(RuralNotableInnAndOutIssueBehavior.RuralNotableInnAndOutIssue), 1));
		}

		// Token: 0x06000571 RID: 1393 RVA: 0x00026B70 File Offset: 0x00024D70
		private IssueBase OnSelected(in PotentialIssueData pid, Hero issueOwner)
		{
			return new RuralNotableInnAndOutIssueBehavior.RuralNotableInnAndOutIssue(issueOwner);
		}

		// Token: 0x040002A8 RID: 680
		private const IssueBase.IssueFrequency RuralNotableInnAndOutIssueFrequency = 1;

		// Token: 0x040002A9 RID: 681
		private const float IssueDuration = 30f;

		// Token: 0x040002AA RID: 682
		private const float QuestDuration = 14f;

		// Token: 0x0200015B RID: 347
		public class RuralNotableInnAndOutIssueTypeDefiner : SaveableTypeDefiner
		{
			// Token: 0x06000FE7 RID: 4071 RVA: 0x0006E145 File Offset: 0x0006C345
			public RuralNotableInnAndOutIssueTypeDefiner()
				: base(585900)
			{
			}

			// Token: 0x06000FE8 RID: 4072 RVA: 0x0006E152 File Offset: 0x0006C352
			protected override void DefineClassTypes()
			{
				base.AddClassDefinition(typeof(RuralNotableInnAndOutIssueBehavior.RuralNotableInnAndOutIssue), 1, null);
				base.AddClassDefinition(typeof(RuralNotableInnAndOutIssueBehavior.RuralNotableInnAndOutIssueQuest), 2, null);
			}
		}

		// Token: 0x0200015C RID: 348
		public class RuralNotableInnAndOutIssue : IssueBase
		{
			// Token: 0x17000186 RID: 390
			// (get) Token: 0x06000FE9 RID: 4073 RVA: 0x0006E178 File Offset: 0x0006C378
			public override IssueBase.AlternativeSolutionScaleFlag AlternativeSolutionScaleFlags
			{
				get
				{
					return 8;
				}
			}

			// Token: 0x17000187 RID: 391
			// (get) Token: 0x06000FEA RID: 4074 RVA: 0x0006E17B File Offset: 0x0006C37B
			protected override bool IssueQuestCanBeDuplicated
			{
				get
				{
					return false;
				}
			}

			// Token: 0x17000188 RID: 392
			// (get) Token: 0x06000FEB RID: 4075 RVA: 0x0006E17E File Offset: 0x0006C37E
			public override int AlternativeSolutionBaseNeededMenCount
			{
				get
				{
					return 1 + MathF.Ceiling(3f * base.IssueDifficultyMultiplier);
				}
			}

			// Token: 0x17000189 RID: 393
			// (get) Token: 0x06000FEC RID: 4076 RVA: 0x0006E193 File Offset: 0x0006C393
			protected override int AlternativeSolutionBaseDurationInDaysInternal
			{
				get
				{
					return 1 + MathF.Ceiling(3f * base.IssueDifficultyMultiplier);
				}
			}

			// Token: 0x1700018A RID: 394
			// (get) Token: 0x06000FED RID: 4077 RVA: 0x0006E1A8 File Offset: 0x0006C3A8
			protected override int RewardGold
			{
				get
				{
					return 1000;
				}
			}

			// Token: 0x1700018B RID: 395
			// (get) Token: 0x06000FEE RID: 4078 RVA: 0x0006E1AF File Offset: 0x0006C3AF
			public override TextObject Title
			{
				get
				{
					return new TextObject("{=uUhtKnfA}Inn and Out", null);
				}
			}

			// Token: 0x1700018C RID: 396
			// (get) Token: 0x06000FEF RID: 4079 RVA: 0x0006E1BC File Offset: 0x0006C3BC
			public override TextObject Description
			{
				get
				{
					TextObject textObject = new TextObject("{=swamqBRq}{ISSUE_OWNER.NAME} wants you to beat the game host", null);
					StringHelpers.SetCharacterProperties("ISSUE_OWNER", base.IssueOwner.CharacterObject, textObject, false);
					return textObject;
				}
			}

			// Token: 0x1700018D RID: 397
			// (get) Token: 0x06000FF0 RID: 4080 RVA: 0x0006E1EE File Offset: 0x0006C3EE
			public override TextObject IssueBriefByIssueGiver
			{
				get
				{
					TextObject textObject = new TextObject("{=T0zupcGB}Ah yes... It is a bit embarrassing to mention, but... Well, when I am in town, I often have a drink at the inn and perhaps play a round of {GAME_TYPE} or two. Normally I play for low stakes but let's just say that last time the wine went to my head, and I lost something I couldn't afford to lose.", null);
					textObject.SetTextVariable("GAME_TYPE", GameTexts.FindText("str_boardgame_name", this._boardGameType.ToString()));
					return textObject;
				}
			}

			// Token: 0x1700018E RID: 398
			// (get) Token: 0x06000FF1 RID: 4081 RVA: 0x0006E222 File Offset: 0x0006C422
			public override TextObject IssueAcceptByPlayer
			{
				get
				{
					return new TextObject("{=h2tMadtI}I've heard that story before. What did you lose?", null);
				}
			}

			// Token: 0x1700018F RID: 399
			// (get) Token: 0x06000FF2 RID: 4082 RVA: 0x0006E230 File Offset: 0x0006C430
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

			// Token: 0x17000190 RID: 400
			// (get) Token: 0x06000FF3 RID: 4083 RVA: 0x0006E282 File Offset: 0x0006C482
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

			// Token: 0x17000191 RID: 401
			// (get) Token: 0x06000FF4 RID: 4084 RVA: 0x0006E2B2 File Offset: 0x0006C4B2
			public override TextObject IssueQuestSolutionAcceptByPlayer
			{
				get
				{
					return new TextObject("{=KMThnMbt}I'll go to the tavern and win it back the same way you lost it.", null);
				}
			}

			// Token: 0x17000192 RID: 402
			// (get) Token: 0x06000FF5 RID: 4085 RVA: 0x0006E2C0 File Offset: 0x0006C4C0
			public override TextObject IssueAlternativeSolutionAcceptByPlayer
			{
				get
				{
					TextObject textObject = new TextObject("{=QdKWaabR}Worry not {ISSUE_OWNER.NAME}, my men will be back with your deed in no time.", null);
					StringHelpers.SetCharacterProperties("ISSUE_OWNER", base.IssueOwner.CharacterObject, textObject, false);
					return textObject;
				}
			}

			// Token: 0x17000193 RID: 403
			// (get) Token: 0x06000FF6 RID: 4086 RVA: 0x0006E2F2 File Offset: 0x0006C4F2
			public override TextObject IssueDiscussAlternativeSolution
			{
				get
				{
					return new TextObject("{=1yEyUHJe}I really hope your men can get my deed back. On my father's name, I will never gamble again.", null);
				}
			}

			// Token: 0x17000194 RID: 404
			// (get) Token: 0x06000FF7 RID: 4087 RVA: 0x0006E300 File Offset: 0x0006C500
			public override TextObject IssueAlternativeSolutionResponseByIssueGiver
			{
				get
				{
					TextObject textObject = new TextObject("{=kiaN39yb}Thank you, {PLAYER.NAME}. I'm sure your companion will be persuasive.", null);
					StringHelpers.SetCharacterProperties("PLAYER", CharacterObject.PlayerCharacter, textObject, false);
					return textObject;
				}
			}

			// Token: 0x17000195 RID: 405
			// (get) Token: 0x06000FF8 RID: 4088 RVA: 0x0006E32C File Offset: 0x0006C52C
			public override bool IsThereAlternativeSolution
			{
				get
				{
					return true;
				}
			}

			// Token: 0x17000196 RID: 406
			// (get) Token: 0x06000FF9 RID: 4089 RVA: 0x0006E32F File Offset: 0x0006C52F
			public override bool IsThereLordSolution
			{
				get
				{
					return false;
				}
			}

			// Token: 0x17000197 RID: 407
			// (get) Token: 0x06000FFA RID: 4090 RVA: 0x0006E334 File Offset: 0x0006C534
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

			// Token: 0x06000FFB RID: 4091 RVA: 0x0006E3C0 File Offset: 0x0006C5C0
			public RuralNotableInnAndOutIssue(Hero issueOwner)
				: base(issueOwner, CampaignTime.DaysFromNow(30f))
			{
				this.InitializeQuestVariables();
			}

			// Token: 0x06000FFC RID: 4092 RVA: 0x0006E3D9 File Offset: 0x0006C5D9
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

			// Token: 0x06000FFD RID: 4093 RVA: 0x0006E3FC File Offset: 0x0006C5FC
			public override ValueTuple<SkillObject, int> GetAlternativeSolutionSkill(Hero hero)
			{
				return new ValueTuple<SkillObject, int>((hero.GetSkillValue(DefaultSkills.Charm) >= hero.GetSkillValue(DefaultSkills.Tactics)) ? DefaultSkills.Charm : DefaultSkills.Tactics, 120);
			}

			// Token: 0x06000FFE RID: 4094 RVA: 0x0006E429 File Offset: 0x0006C629
			public override bool AlternativeSolutionCondition(out TextObject explanation)
			{
				explanation = TextObject.Empty;
				return QuestHelper.CheckRosterForAlternativeSolution(MobileParty.MainParty.MemberRoster, base.GetTotalAlternativeSolutionNeededMenCount(), ref explanation, 0, false) && QuestHelper.CheckGoldForAlternativeSolution(1000, ref explanation);
			}

			// Token: 0x17000198 RID: 408
			// (get) Token: 0x06000FFF RID: 4095 RVA: 0x0006E459 File Offset: 0x0006C659
			protected override int CompanionSkillRewardXP
			{
				get
				{
					return (int)(500f + 1000f * base.IssueDifficultyMultiplier);
				}
			}

			// Token: 0x06001000 RID: 4096 RVA: 0x0006E470 File Offset: 0x0006C670
			protected override void AlternativeSolutionEndWithSuccessConsequence()
			{
				this.RelationshipChangeWithIssueOwner = 5;
				GainRenownAction.Apply(Hero.MainHero, 5f, false);
				base.IssueOwner.CurrentSettlement.Village.Bound.Town.Loyalty += 5f;
			}

			// Token: 0x06001001 RID: 4097 RVA: 0x0006E4BF File Offset: 0x0006C6BF
			protected override void AlternativeSolutionEndWithFailureConsequence()
			{
				this.RelationshipChangeWithIssueOwner -= 5;
				base.IssueOwner.CurrentSettlement.Village.Bound.Town.Loyalty -= 5f;
			}

			// Token: 0x06001002 RID: 4098 RVA: 0x0006E4FA File Offset: 0x0006C6FA
			public override bool DoTroopsSatisfyAlternativeSolution(TroopRoster troopRoster, out TextObject explanation)
			{
				explanation = TextObject.Empty;
				return QuestHelper.CheckRosterForAlternativeSolution(troopRoster, base.GetTotalAlternativeSolutionNeededMenCount(), ref explanation, 0, false);
			}

			// Token: 0x06001003 RID: 4099 RVA: 0x0006E512 File Offset: 0x0006C712
			public override IssueBase.IssueFrequency GetFrequency()
			{
				return 1;
			}

			// Token: 0x06001004 RID: 4100 RVA: 0x0006E518 File Offset: 0x0006C718
			public override bool IssueStayAliveConditions()
			{
				BoardGameCampaignBehavior campaignBehavior = Campaign.Current.GetCampaignBehavior<BoardGameCampaignBehavior>();
				return campaignBehavior != null && !campaignBehavior.WonBoardGamesInOneWeekInSettlement.Contains(this._targetSettlement) && !base.IssueOwner.CurrentSettlement.IsRaided && !base.IssueOwner.CurrentSettlement.IsUnderRaid;
			}

			// Token: 0x06001005 RID: 4101 RVA: 0x0006E56D File Offset: 0x0006C76D
			protected override void CompleteIssueWithTimedOutConsequences()
			{
			}

			// Token: 0x06001006 RID: 4102 RVA: 0x0006E56F File Offset: 0x0006C76F
			private void InitializeQuestVariables()
			{
				this._targetSettlement = base.IssueOwner.CurrentSettlement.Village.Bound;
				this._boardGameType = this._targetSettlement.Culture.BoardGame;
			}

			// Token: 0x06001007 RID: 4103 RVA: 0x0006E5A2 File Offset: 0x0006C7A2
			protected override void OnGameLoad()
			{
				this.InitializeQuestVariables();
			}

			// Token: 0x06001008 RID: 4104 RVA: 0x0006E5AA File Offset: 0x0006C7AA
			protected override QuestBase GenerateIssueQuest(string questId)
			{
				return new RuralNotableInnAndOutIssueBehavior.RuralNotableInnAndOutIssueQuest(questId, base.IssueOwner, CampaignTime.DaysFromNow(14f), this.RewardGold);
			}

			// Token: 0x06001009 RID: 4105 RVA: 0x0006E5C8 File Offset: 0x0006C7C8
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

			// Token: 0x0600100A RID: 4106 RVA: 0x0006E634 File Offset: 0x0006C834
			internal static void AutoGeneratedStaticCollectObjectsRuralNotableInnAndOutIssue(object o, List<object> collectedObjects)
			{
				((RuralNotableInnAndOutIssueBehavior.RuralNotableInnAndOutIssue)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			// Token: 0x0600100B RID: 4107 RVA: 0x0006E642 File Offset: 0x0006C842
			protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			// Token: 0x0400067C RID: 1660
			private const int CompanionSkillLimit = 120;

			// Token: 0x0400067D RID: 1661
			private const int QuestMoneyLimit = 2000;

			// Token: 0x0400067E RID: 1662
			private const int AlternativeSolutionGoldCost = 1000;

			// Token: 0x0400067F RID: 1663
			private CultureObject.BoardGameType _boardGameType;

			// Token: 0x04000680 RID: 1664
			private Settlement _targetSettlement;
		}

		// Token: 0x0200015D RID: 349
		public class RuralNotableInnAndOutIssueQuest : QuestBase
		{
			// Token: 0x17000199 RID: 409
			// (get) Token: 0x0600100C RID: 4108 RVA: 0x0006E64C File Offset: 0x0006C84C
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

			// Token: 0x1700019A RID: 410
			// (get) Token: 0x0600100D RID: 4109 RVA: 0x0006E698 File Offset: 0x0006C898
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

			// Token: 0x1700019B RID: 411
			// (get) Token: 0x0600100E RID: 4110 RVA: 0x0006E6F0 File Offset: 0x0006C8F0
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

			// Token: 0x1700019C RID: 412
			// (get) Token: 0x0600100F RID: 4111 RVA: 0x0006E744 File Offset: 0x0006C944
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

			// Token: 0x1700019D RID: 413
			// (get) Token: 0x06001010 RID: 4112 RVA: 0x0006E788 File Offset: 0x0006C988
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

			// Token: 0x1700019E RID: 414
			// (get) Token: 0x06001011 RID: 4113 RVA: 0x0006E7D1 File Offset: 0x0006C9D1
			private TextObject _questCanceledWarDeclared
			{
				get
				{
					TextObject textObject = new TextObject("{=cKz1cyuM}Your clan is now at war with {QUEST_GIVER_SETTLEMENT_FACTION}. Quest is canceled.", null);
					textObject.SetTextVariable("QUEST_GIVER_SETTLEMENT_FACTION", base.QuestGiver.CurrentSettlement.MapFaction.Name);
					return textObject;
				}
			}

			// Token: 0x1700019F RID: 415
			// (get) Token: 0x06001012 RID: 4114 RVA: 0x0006E800 File Offset: 0x0006CA00
			private TextObject _playerDeclaredWarQuestLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=bqeWVVEE}Your actions have started a war with {QUEST_GIVER.LINK}'s faction. {?QUEST_GIVER.GENDER}She{?}He{\\?} cancels your agreement and the quest is a failure.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					return textObject;
				}
			}

			// Token: 0x170001A0 RID: 416
			// (get) Token: 0x06001013 RID: 4115 RVA: 0x0006E834 File Offset: 0x0006CA34
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

			// Token: 0x170001A1 RID: 417
			// (get) Token: 0x06001014 RID: 4116 RVA: 0x0006E880 File Offset: 0x0006CA80
			private TextObject _timeoutLog
			{
				get
				{
					TextObject textObject = new TextObject("{=XLy8anVr}You received a message from {QUEST_GIVER.LINK}. \"This may not have seemed like an important task, but I placed my trust in you. I guess I was wrong to do so.\"", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					return textObject;
				}
			}

			// Token: 0x170001A2 RID: 418
			// (get) Token: 0x06001015 RID: 4117 RVA: 0x0006E8B2 File Offset: 0x0006CAB2
			public override TextObject Title
			{
				get
				{
					return new TextObject("{=uUhtKnfA}Inn and Out", null);
				}
			}

			// Token: 0x170001A3 RID: 419
			// (get) Token: 0x06001016 RID: 4118 RVA: 0x0006E8BF File Offset: 0x0006CABF
			public override bool IsRemainingTimeHidden
			{
				get
				{
					return false;
				}
			}

			// Token: 0x06001017 RID: 4119 RVA: 0x0006E8C2 File Offset: 0x0006CAC2
			public RuralNotableInnAndOutIssueQuest(string questId, Hero giverHero, CampaignTime duration, int rewardGold)
				: base(questId, giverHero, duration, rewardGold)
			{
				this.InitializeQuestVariables();
				this.SetDialogs();
				base.InitializeQuestOnCreation();
			}

			// Token: 0x06001018 RID: 4120 RVA: 0x0006E8E1 File Offset: 0x0006CAE1
			private void InitializeQuestVariables()
			{
				this._targetSettlement = base.QuestGiver.CurrentSettlement.Village.Bound;
				this._boardGameType = this._targetSettlement.Culture.BoardGame;
			}

			// Token: 0x06001019 RID: 4121 RVA: 0x0006E914 File Offset: 0x0006CB14
			private void QuestAcceptedConsequences()
			{
				base.StartQuest();
				base.AddLog(this._questStartLog, false);
				base.AddTrackedObject(this._targetSettlement);
			}

			// Token: 0x0600101A RID: 4122 RVA: 0x0006E936 File Offset: 0x0006CB36
			protected override void InitializeQuestOnGameLoad()
			{
				this.InitializeQuestVariables();
				this.SetDialogs();
				if (Campaign.Current.GetCampaignBehavior<BoardGameCampaignBehavior>() == null)
				{
					base.CompleteQuestWithCancel(null);
				}
			}

			// Token: 0x0600101B RID: 4123 RVA: 0x0006E958 File Offset: 0x0006CB58
			protected override void RegisterEvents()
			{
				CampaignEvents.OnPlayerBoardGameOverEvent.AddNonSerializedListener(this, new Action<Hero, BoardGameHelper.BoardGameState>(this.OnBoardGameEnd));
				CampaignEvents.WarDeclared.AddNonSerializedListener(this, new Action<IFaction, IFaction, DeclareWarAction.DeclareWarDetail>(this.OnWarDeclared));
				CampaignEvents.ClanChangedKingdom.AddNonSerializedListener(this, new Action<Clan, Kingdom, Kingdom, ChangeKingdomAction.ChangeKingdomActionDetail, bool>(this.OnClanChangedKingdom));
				CampaignEvents.OnSiegeEventStartedEvent.AddNonSerializedListener(this, new Action<SiegeEvent>(this.OnSiegeStarted));
				CampaignEvents.MapEventStarted.AddNonSerializedListener(this, new Action<MapEvent, PartyBase, PartyBase>(this.OnMapEventStarted));
				CampaignEvents.VillageBeingRaided.AddNonSerializedListener(this, new Action<Village>(this.OnVillageBeingRaided));
			}

			// Token: 0x0600101C RID: 4124 RVA: 0x0006E9EF File Offset: 0x0006CBEF
			private void OnMapEventStarted(MapEvent mapEvent, PartyBase attackerParty, PartyBase defenderParty)
			{
				if (QuestHelper.CheckMinorMajorCoercion(this, mapEvent, attackerParty))
				{
					QuestHelper.ApplyGenericMinorMajorCoercionConsequences(this, mapEvent);
				}
			}

			// Token: 0x0600101D RID: 4125 RVA: 0x0006EA02 File Offset: 0x0006CC02
			private void OnVillageBeingRaided(Village village)
			{
				if (village == base.QuestGiver.CurrentSettlement.Village)
				{
					base.CompleteQuestWithCancel(this._questCanceledTargetVillageRaided);
				}
			}

			// Token: 0x0600101E RID: 4126 RVA: 0x0006EA23 File Offset: 0x0006CC23
			private void OnBoardGameEnd(Hero opposingHero, BoardGameHelper.BoardGameState state)
			{
				if (this._checkForBoardGameEnd)
				{
					this._playerWonTheGame = state == 1;
				}
			}

			// Token: 0x0600101F RID: 4127 RVA: 0x0006EA37 File Offset: 0x0006CC37
			private void OnSiegeStarted(SiegeEvent siegeEvent)
			{
				if (siegeEvent.BesiegedSettlement == this._targetSettlement)
				{
					base.CompleteQuestWithCancel(this._questCanceledSettlementIsUnderSiege);
				}
			}

			// Token: 0x06001020 RID: 4128 RVA: 0x0006EA54 File Offset: 0x0006CC54
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

			// Token: 0x06001021 RID: 4129 RVA: 0x0006EB80 File Offset: 0x0006CD80
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

			// Token: 0x06001022 RID: 4130 RVA: 0x0006ECBC File Offset: 0x0006CEBC
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

			// Token: 0x06001023 RID: 4131 RVA: 0x0006EDC8 File Offset: 0x0006CFC8
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

			// Token: 0x06001024 RID: 4132 RVA: 0x0006EDF4 File Offset: 0x0006CFF4
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

			// Token: 0x06001025 RID: 4133 RVA: 0x0006EE5F File Offset: 0x0006D05F
			private void PlayerPaid1000QuestSuccess()
			{
				base.AddLog(this._successWithPayingLog, false);
				this._applyLesserReward = true;
				GiveGoldAction.ApplyBetweenCharacters(Hero.MainHero, null, 1000, false);
				base.CompleteQuestWithSuccess();
			}

			// Token: 0x06001026 RID: 4134 RVA: 0x0006EE90 File Offset: 0x0006D090
			private void ApplySuccessRewards()
			{
				GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, this._applyLesserReward ? 800 : this.RewardGold, false);
				ChangeRelationAction.ApplyPlayerRelation(base.QuestGiver, 5, true, true);
				GainRenownAction.Apply(Hero.MainHero, 1f, false);
				base.QuestGiver.CurrentSettlement.Village.Bound.Town.Loyalty += 5f;
			}

			// Token: 0x06001027 RID: 4135 RVA: 0x0006EF07 File Offset: 0x0006D107
			protected override void OnCompleteWithSuccess()
			{
				this.ApplySuccessRewards();
			}

			// Token: 0x06001028 RID: 4136 RVA: 0x0006EF10 File Offset: 0x0006D110
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

			// Token: 0x06001029 RID: 4137 RVA: 0x0006EF6E File Offset: 0x0006D16E
			private void PlayerWonTheBoardGame()
			{
				base.AddLog(this._successLog, false);
				base.CompleteQuestWithSuccess();
			}

			// Token: 0x0600102A RID: 4138 RVA: 0x0006EF84 File Offset: 0x0006D184
			private void PlayerFailAfterBoardGame()
			{
				base.AddLog(this._lostLog, false);
				this.RelationshipChangeWithQuestGiver = -5;
				base.QuestGiver.CurrentSettlement.Village.Bound.Town.Loyalty -= 5f;
				base.CompleteQuestWithFail(null);
			}

			// Token: 0x0600102B RID: 4139 RVA: 0x0006EFD9 File Offset: 0x0006D1D9
			private void OnClanChangedKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail detail, bool showNotification = true)
			{
				if (base.QuestGiver.CurrentSettlement.MapFaction.IsAtWarWith(Hero.MainHero.MapFaction))
				{
					base.CompleteQuestWithCancel(this._questCanceledWarDeclared);
				}
			}

			// Token: 0x0600102C RID: 4140 RVA: 0x0006F008 File Offset: 0x0006D208
			private void OnWarDeclared(IFaction faction1, IFaction faction2, DeclareWarAction.DeclareWarDetail detail)
			{
				QuestHelper.CheckWarDeclarationAndFailOrCancelTheQuest(this, faction1, faction2, detail, this._playerDeclaredWarQuestLogText, this._questCanceledWarDeclared);
			}

			// Token: 0x0600102D RID: 4141 RVA: 0x0006F01F File Offset: 0x0006D21F
			public override GameMenuOption.IssueQuestFlags IsLocationTrackedByQuest(Location location)
			{
				if (PlayerEncounter.LocationEncounter.Settlement == this._targetSettlement && location.StringId == "tavern")
				{
					return 2;
				}
				return 0;
			}

			// Token: 0x0600102E RID: 4142 RVA: 0x0006F048 File Offset: 0x0006D248
			protected override void OnTimedOut()
			{
				this.RelationshipChangeWithQuestGiver = -5;
				base.QuestGiver.CurrentSettlement.Village.Bound.Town.Loyalty -= 5f;
				base.AddLog(this._timeoutLog, false);
			}

			// Token: 0x0600102F RID: 4143 RVA: 0x0006F096 File Offset: 0x0006D296
			internal static void AutoGeneratedStaticCollectObjectsRuralNotableInnAndOutIssueQuest(object o, List<object> collectedObjects)
			{
				((RuralNotableInnAndOutIssueBehavior.RuralNotableInnAndOutIssueQuest)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			// Token: 0x06001030 RID: 4144 RVA: 0x0006F0A4 File Offset: 0x0006D2A4
			protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			// Token: 0x06001031 RID: 4145 RVA: 0x0006F0AD File Offset: 0x0006D2AD
			internal static object AutoGeneratedGetMemberValue_tryCount(object o)
			{
				return ((RuralNotableInnAndOutIssueBehavior.RuralNotableInnAndOutIssueQuest)o)._tryCount;
			}

			// Token: 0x04000681 RID: 1665
			public const int LesserReward = 800;

			// Token: 0x04000682 RID: 1666
			private CultureObject.BoardGameType _boardGameType;

			// Token: 0x04000683 RID: 1667
			private Settlement _targetSettlement;

			// Token: 0x04000684 RID: 1668
			private bool _checkForBoardGameEnd;

			// Token: 0x04000685 RID: 1669
			private bool _playerWonTheGame;

			// Token: 0x04000686 RID: 1670
			private bool _applyLesserReward;

			// Token: 0x04000687 RID: 1671
			[SaveableField(1)]
			private int _tryCount;
		}
	}
}
