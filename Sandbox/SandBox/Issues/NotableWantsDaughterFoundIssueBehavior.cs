using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using SandBox.Conversation.MissionLogics;
using SandBox.Missions.AgentBehaviors;
using SandBox.Missions.MissionLogics;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Conversation.Persuasion;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Issues;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace SandBox.Issues
{
	// Token: 0x0200007C RID: 124
	public class NotableWantsDaughterFoundIssueBehavior : CampaignBehaviorBase
	{
		// Token: 0x0600054F RID: 1359 RVA: 0x0002617B File Offset: 0x0002437B
		public override void RegisterEvents()
		{
			CampaignEvents.OnCheckForIssueEvent.AddNonSerializedListener(this, new Action<Hero>(this.OnCheckForIssue));
		}

		// Token: 0x06000550 RID: 1360 RVA: 0x00026194 File Offset: 0x00024394
		public void OnCheckForIssue(Hero hero)
		{
			if (this.ConditionsHold(hero))
			{
				Campaign.Current.IssueManager.AddPotentialIssueData(hero, new PotentialIssueData(new PotentialIssueData.StartIssueDelegate(this.OnStartIssue), typeof(NotableWantsDaughterFoundIssueBehavior.NotableWantsDaughterFoundIssue), 2, null));
				return;
			}
			Campaign.Current.IssueManager.AddPotentialIssueData(hero, new PotentialIssueData(typeof(NotableWantsDaughterFoundIssueBehavior.NotableWantsDaughterFoundIssue), 2));
		}

		// Token: 0x06000551 RID: 1361 RVA: 0x000261F8 File Offset: 0x000243F8
		private bool ConditionsHold(Hero issueGiver)
		{
			if (issueGiver.IsRuralNotable && issueGiver.CurrentSettlement.IsVillage && issueGiver.CurrentSettlement.Village.Bound != null && issueGiver.CurrentSettlement.Village.Bound.BoundVillages.Count > 2 && issueGiver.CanHaveQuestsOrIssues() && issueGiver.Age > (float)(Campaign.Current.Models.AgeModel.HeroComesOfAge * 2))
			{
				if (issueGiver.CurrentSettlement.Culture.NotableAndWandererTemplates.Any((CharacterObject x) => x.Occupation == 16 && x.IsFemale))
				{
					if (issueGiver.CurrentSettlement.Culture.NotableAndWandererTemplates.Any((CharacterObject x) => x.Occupation == 21 && !x.IsFemale) && issueGiver.GetTraitLevel(DefaultTraits.Mercy) <= 0)
					{
						return issueGiver.GetTraitLevel(DefaultTraits.Generosity) <= 0;
					}
				}
			}
			return false;
		}

		// Token: 0x06000552 RID: 1362 RVA: 0x0002630F File Offset: 0x0002450F
		private IssueBase OnStartIssue(in PotentialIssueData pid, Hero issueOwner)
		{
			return new NotableWantsDaughterFoundIssueBehavior.NotableWantsDaughterFoundIssue(issueOwner);
		}

		// Token: 0x06000553 RID: 1363 RVA: 0x00026317 File Offset: 0x00024517
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x0400029E RID: 670
		private const IssueBase.IssueFrequency NotableWantsDaughterFoundIssueFrequency = 2;

		// Token: 0x0400029F RID: 671
		private const int IssueDuration = 30;

		// Token: 0x040002A0 RID: 672
		private const int QuestTimeLimit = 19;

		// Token: 0x040002A1 RID: 673
		private const int BaseRewardGold = 500;

		// Token: 0x0200014F RID: 335
		public class NotableWantsDaughterFoundIssueTypeDefiner : CampaignBehaviorBase.SaveableCampaignBehaviorTypeDefiner
		{
			// Token: 0x06000E58 RID: 3672 RVA: 0x00067029 File Offset: 0x00065229
			public NotableWantsDaughterFoundIssueTypeDefiner()
				: base(1088000)
			{
			}

			// Token: 0x06000E59 RID: 3673 RVA: 0x00067036 File Offset: 0x00065236
			protected override void DefineClassTypes()
			{
				base.AddClassDefinition(typeof(NotableWantsDaughterFoundIssueBehavior.NotableWantsDaughterFoundIssue), 1, null);
				base.AddClassDefinition(typeof(NotableWantsDaughterFoundIssueBehavior.NotableWantsDaughterFoundIssueQuest), 2, null);
			}
		}

		// Token: 0x02000150 RID: 336
		public class NotableWantsDaughterFoundIssue : IssueBase
		{
			// Token: 0x17000123 RID: 291
			// (get) Token: 0x06000E5A RID: 3674 RVA: 0x0006705C File Offset: 0x0006525C
			public override IssueBase.AlternativeSolutionScaleFlag AlternativeSolutionScaleFlags
			{
				get
				{
					return 8;
				}
			}

			// Token: 0x17000124 RID: 292
			// (get) Token: 0x06000E5B RID: 3675 RVA: 0x0006705F File Offset: 0x0006525F
			public override bool IsThereAlternativeSolution
			{
				get
				{
					return true;
				}
			}

			// Token: 0x17000125 RID: 293
			// (get) Token: 0x06000E5C RID: 3676 RVA: 0x00067062 File Offset: 0x00065262
			public override bool IsThereLordSolution
			{
				get
				{
					return false;
				}
			}

			// Token: 0x17000126 RID: 294
			// (get) Token: 0x06000E5D RID: 3677 RVA: 0x00067065 File Offset: 0x00065265
			protected override int RewardGold
			{
				get
				{
					return 500 + MathF.Round(1200f * base.IssueDifficultyMultiplier);
				}
			}

			// Token: 0x17000127 RID: 295
			// (get) Token: 0x06000E5E RID: 3678 RVA: 0x0006707E File Offset: 0x0006527E
			public override int AlternativeSolutionBaseNeededMenCount
			{
				get
				{
					return 2 + MathF.Ceiling(4f * base.IssueDifficultyMultiplier);
				}
			}

			// Token: 0x17000128 RID: 296
			// (get) Token: 0x06000E5F RID: 3679 RVA: 0x00067093 File Offset: 0x00065293
			protected override int AlternativeSolutionBaseDurationInDaysInternal
			{
				get
				{
					return 4 + MathF.Ceiling(5f * base.IssueDifficultyMultiplier);
				}
			}

			// Token: 0x17000129 RID: 297
			// (get) Token: 0x06000E60 RID: 3680 RVA: 0x000670A8 File Offset: 0x000652A8
			protected override int CompanionSkillRewardXP
			{
				get
				{
					return (int)(500f + 1000f * base.IssueDifficultyMultiplier);
				}
			}

			// Token: 0x06000E61 RID: 3681 RVA: 0x000670BD File Offset: 0x000652BD
			public NotableWantsDaughterFoundIssue(Hero issueOwner)
				: base(issueOwner, CampaignTime.DaysFromNow(30f))
			{
			}

			// Token: 0x06000E62 RID: 3682 RVA: 0x000670D0 File Offset: 0x000652D0
			protected override float GetIssueEffectAmountInternal(IssueEffect issueEffect)
			{
				if (issueEffect == DefaultIssueEffects.IssueOwnerPower)
				{
					return -0.1f;
				}
				return 0f;
			}

			// Token: 0x1700012A RID: 298
			// (get) Token: 0x06000E63 RID: 3683 RVA: 0x000670E8 File Offset: 0x000652E8
			public override TextObject IssueBriefByIssueGiver
			{
				get
				{
					TextObject textObject = new TextObject("{=x9VgLEzi}Yes... I've suffered a great misfortune. My daughter, a headstrong girl, has been bewitched by this never-do-well. I told her to stop seeing him but she wouldn't listen! Now she's missing - I'm sure she's been abducted by him! I'm offering a bounty of {BASE_REWARD_GOLD}{GOLD_ICON} to anyone who brings her back. Please {?PLAYER.GENDER}ma'am{?}sir{\\?}! Don't let a father's heart be broken.", null);
					StringHelpers.SetCharacterProperties("PLAYER", CharacterObject.PlayerCharacter, textObject, false);
					textObject.SetTextVariable("GOLD_ICON", "{=!}<img src=\"General\\Icons\\Coin@2x\" extend=\"8\">");
					textObject.SetTextVariable("BASE_REWARD_GOLD", this.RewardGold);
					return textObject;
				}
			}

			// Token: 0x1700012B RID: 299
			// (get) Token: 0x06000E64 RID: 3684 RVA: 0x00067137 File Offset: 0x00065337
			public override TextObject IssueAcceptByPlayer
			{
				get
				{
					return new TextObject("{=35w6g8gM}Tell me more. What's wrong with the man? ", null);
				}
			}

			// Token: 0x1700012C RID: 300
			// (get) Token: 0x06000E65 RID: 3685 RVA: 0x00067144 File Offset: 0x00065344
			public override TextObject IssueQuestSolutionExplanationByIssueGiver
			{
				get
				{
					return new TextObject("{=IY5b9vZV}Everything is wrong. He is from a low family, the kind who is always involved in some land fraud scheme, or seen dealing with known bandits. Every village has a black sheep like that but I never imagined he would get his hooks into my daughter!", null);
				}
			}

			// Token: 0x1700012D RID: 301
			// (get) Token: 0x06000E66 RID: 3686 RVA: 0x00067151 File Offset: 0x00065351
			public override TextObject IssueAlternativeSolutionExplanationByIssueGiver
			{
				get
				{
					TextObject textObject = new TextObject("{=v0XsM7Zz}If you send your best tracker with a few men, I am sure they will find my girl and be back to you in no more than {ALTERNATIVE_SOLUTION_WAIT_DAYS} days.", null);
					textObject.SetTextVariable("ALTERNATIVE_SOLUTION_WAIT_DAYS", base.GetTotalAlternativeSolutionDurationInDays());
					return textObject;
				}
			}

			// Token: 0x1700012E RID: 302
			// (get) Token: 0x06000E67 RID: 3687 RVA: 0x00067170 File Offset: 0x00065370
			public override TextObject IssuePlayerResponseAfterAlternativeExplanation
			{
				get
				{
					return new TextObject("{=Ldp6ckgj}Don't worry, either I or one of my companions should be able to find her and see what's going on.", null);
				}
			}

			// Token: 0x1700012F RID: 303
			// (get) Token: 0x06000E68 RID: 3688 RVA: 0x0006717D File Offset: 0x0006537D
			public override TextObject IssueQuestSolutionAcceptByPlayer
			{
				get
				{
					return new TextObject("{=uYrxCtDa}I should be able to find her and see what's going on.", null);
				}
			}

			// Token: 0x17000130 RID: 304
			// (get) Token: 0x06000E69 RID: 3689 RVA: 0x0006718A File Offset: 0x0006538A
			public override TextObject IssueAlternativeSolutionAcceptByPlayer
			{
				get
				{
					TextObject textObject = new TextObject("{=WSrGHkal}I will have one of my trackers and {REQUIRED_TROOP_AMOUNT} of my men to find your daughter.", null);
					textObject.SetTextVariable("REQUIRED_TROOP_AMOUNT", base.GetTotalAlternativeSolutionNeededMenCount());
					return textObject;
				}
			}

			// Token: 0x17000131 RID: 305
			// (get) Token: 0x06000E6A RID: 3690 RVA: 0x000671A9 File Offset: 0x000653A9
			public override TextObject IssueDiscussAlternativeSolution
			{
				get
				{
					return new TextObject("{=mBPcZddA}{?PLAYER.GENDER}Madam{?}Sir{\\?}, we are still waiting for your men to bring my daughter back. I pray for their success.", null);
				}
			}

			// Token: 0x17000132 RID: 306
			// (get) Token: 0x06000E6B RID: 3691 RVA: 0x000671B8 File Offset: 0x000653B8
			public override TextObject IssueAlternativeSolutionResponseByIssueGiver
			{
				get
				{
					TextObject textObject = new TextObject("{=Hhd3KaKu}Thank you, my {?PLAYER.GENDER}lady{?}lord{\\?}. If your men can find my girl and bring her back to me, I will be so grateful. I will pay you {BASE_REWARD_GOLD}{GOLD_ICON} for your trouble.", null);
					textObject.SetTextVariable("GOLD_ICON", "{=!}<img src=\"General\\Icons\\Coin@2x\" extend=\"8\">");
					textObject.SetTextVariable("BASE_REWARD_GOLD", this.RewardGold);
					StringHelpers.SetCharacterProperties("PLAYER", CharacterObject.PlayerCharacter, textObject, false);
					return textObject;
				}
			}

			// Token: 0x17000133 RID: 307
			// (get) Token: 0x06000E6C RID: 3692 RVA: 0x00067208 File Offset: 0x00065408
			protected override TextObject AlternativeSolutionStartLog
			{
				get
				{
					TextObject textObject = new TextObject("{=6OmbzoBs}{ISSUE_GIVER.LINK}, a merchant from {ISSUE_GIVER_SETTLEMENT}, has told you that {?ISSUE_GIVER.GENDER}her{?}his{\\?} daughter has gone missing. You choose {COMPANION.LINK} and {REQUIRED_TROOP_AMOUNT} men to search for her and bring her back. You expect them to complete this task and return in {ALTERNATIVE_SOLUTION_DAYS} days.", null);
					textObject.SetTextVariable("GOLD_ICON", "{=!}<img src=\"General\\Icons\\Coin@2x\" extend=\"8\">");
					textObject.SetTextVariable("BASE_REWARD_GOLD", this.RewardGold);
					textObject.SetTextVariable("ISSUE_GIVER_SETTLEMENT", base.IssueOwner.CurrentSettlement.Name);
					textObject.SetTextVariable("REQUIRED_TROOP_AMOUNT", this.AlternativeSolutionSentTroops.TotalManCount - 1);
					textObject.SetTextVariable("ALTERNATIVE_SOLUTION_DAYS", base.GetTotalAlternativeSolutionDurationInDays());
					StringHelpers.SetCharacterProperties("PLAYER", CharacterObject.PlayerCharacter, textObject, false);
					StringHelpers.SetCharacterProperties("ISSUE_GIVER", base.IssueOwner.CharacterObject, textObject, false);
					StringHelpers.SetCharacterProperties("COMPANION", base.AlternativeSolutionHero.CharacterObject, textObject, false);
					return textObject;
				}
			}

			// Token: 0x06000E6D RID: 3693 RVA: 0x000672D0 File Offset: 0x000654D0
			protected override void AlternativeSolutionEndWithSuccessConsequence()
			{
				this.ApplySuccessRewards();
				float randomFloat = MBRandom.RandomFloat;
				SkillObject skillObject;
				if (randomFloat <= 0.33f)
				{
					skillObject = DefaultSkills.OneHanded;
				}
				else if (randomFloat <= 0.66f)
				{
					skillObject = DefaultSkills.TwoHanded;
				}
				else
				{
					skillObject = DefaultSkills.Polearm;
				}
				base.AlternativeSolutionHero.AddSkillXp(skillObject, (float)((int)(500f + 1000f * base.IssueDifficultyMultiplier)));
			}

			// Token: 0x06000E6E RID: 3694 RVA: 0x00067334 File Offset: 0x00065534
			protected override void AlternativeSolutionEndWithFailureConsequence()
			{
				this.RelationshipChangeWithIssueOwner = -10;
				if (base.IssueOwner.CurrentSettlement.Village.Bound != null)
				{
					base.IssueOwner.CurrentSettlement.Village.Bound.Prosperity -= 5f;
					base.IssueOwner.CurrentSettlement.Village.Bound.Town.Security -= 5f;
				}
			}

			// Token: 0x17000134 RID: 308
			// (get) Token: 0x06000E6F RID: 3695 RVA: 0x000673B4 File Offset: 0x000655B4
			public override TextObject IssueAlternativeSolutionSuccessLog
			{
				get
				{
					TextObject textObject = new TextObject("{=MaXA5HJi}Your companions report that the {ISSUE_GIVER.LINK}'s daughter returns to {?ISSUE_GIVER.GENDER}her{?}him{\\?} safe and sound. {?ISSUE_GIVER.GENDER}She{?}He{\\?} is happy and sends {?ISSUE_GIVER.GENDER}her{?}his{\\?} regards with a large pouch of {BASE_REWARD_GOLD}{GOLD_ICON}.", null);
					StringHelpers.SetCharacterProperties("ISSUE_GIVER", base.IssueOwner.CharacterObject, textObject, false);
					textObject.SetTextVariable("GOLD_ICON", "{=!}<img src=\"General\\Icons\\Coin@2x\" extend=\"8\">");
					textObject.SetTextVariable("BASE_REWARD_GOLD", this.RewardGold);
					return textObject;
				}
			}

			// Token: 0x06000E70 RID: 3696 RVA: 0x0006740C File Offset: 0x0006560C
			private void ApplySuccessRewards()
			{
				GainRenownAction.Apply(Hero.MainHero, 2f, false);
				base.IssueOwner.AddPower(10f);
				this.RelationshipChangeWithIssueOwner = 10;
				if (base.IssueOwner.CurrentSettlement.Village.Bound != null)
				{
					base.IssueOwner.CurrentSettlement.Village.Bound.Town.Security += 10f;
				}
			}

			// Token: 0x17000135 RID: 309
			// (get) Token: 0x06000E71 RID: 3697 RVA: 0x00067484 File Offset: 0x00065684
			public override TextObject Title
			{
				get
				{
					TextObject textObject = new TextObject("{=kr68V5pm}{ISSUE_GIVER.NAME} wants {?ISSUE_GIVER.GENDER}her{?}his{\\?} daughter found", null);
					StringHelpers.SetCharacterProperties("ISSUE_GIVER", base.IssueOwner.CharacterObject, textObject, false);
					return textObject;
				}
			}

			// Token: 0x17000136 RID: 310
			// (get) Token: 0x06000E72 RID: 3698 RVA: 0x000674B8 File Offset: 0x000656B8
			public override TextObject Description
			{
				get
				{
					TextObject textObject = new TextObject("{=SkzM5eSv}{ISSUE_GIVER.LINK}'s daughter is missing. {?ISSUE_GIVER.GENDER}She{?}He{\\?} is offering a substantial reward to find the young woman and bring her back safely.", null);
					StringHelpers.SetCharacterProperties("ISSUE_GIVER", base.IssueOwner.CharacterObject, textObject, false);
					return textObject;
				}
			}

			// Token: 0x17000137 RID: 311
			// (get) Token: 0x06000E73 RID: 3699 RVA: 0x000674EC File Offset: 0x000656EC
			public override TextObject IssueAsRumorInSettlement
			{
				get
				{
					TextObject textObject = new TextObject("{=7RyXSkEE}Wouldn't want to be the poor lovesick sap who ran off with {QUEST_GIVER.NAME}'s daughter.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.IssueOwner.CharacterObject, textObject, false);
					return textObject;
				}
			}

			// Token: 0x06000E74 RID: 3700 RVA: 0x0006751E File Offset: 0x0006571E
			protected override void OnGameLoad()
			{
			}

			// Token: 0x06000E75 RID: 3701 RVA: 0x00067520 File Offset: 0x00065720
			protected override QuestBase GenerateIssueQuest(string questId)
			{
				return new NotableWantsDaughterFoundIssueBehavior.NotableWantsDaughterFoundIssueQuest(questId, base.IssueOwner, CampaignTime.DaysFromNow(19f), this.RewardGold, base.IssueDifficultyMultiplier);
			}

			// Token: 0x06000E76 RID: 3702 RVA: 0x00067544 File Offset: 0x00065744
			public override IssueBase.IssueFrequency GetFrequency()
			{
				return 2;
			}

			// Token: 0x06000E77 RID: 3703 RVA: 0x00067547 File Offset: 0x00065747
			public override ValueTuple<SkillObject, int> GetAlternativeSolutionSkill(Hero hero)
			{
				return new ValueTuple<SkillObject, int>((hero.GetSkillValue(DefaultSkills.Charm) >= hero.GetSkillValue(DefaultSkills.Scouting)) ? DefaultSkills.Charm : DefaultSkills.Scouting, 120);
			}

			// Token: 0x06000E78 RID: 3704 RVA: 0x00067574 File Offset: 0x00065774
			public override bool AlternativeSolutionCondition(out TextObject explanation)
			{
				explanation = TextObject.Empty;
				return QuestHelper.CheckRosterForAlternativeSolution(MobileParty.MainParty.MemberRoster, base.GetTotalAlternativeSolutionNeededMenCount(), ref explanation, 2, false);
			}

			// Token: 0x06000E79 RID: 3705 RVA: 0x00067595 File Offset: 0x00065795
			public override bool DoTroopsSatisfyAlternativeSolution(TroopRoster troopRoster, out TextObject explanation)
			{
				explanation = TextObject.Empty;
				return QuestHelper.CheckRosterForAlternativeSolution(troopRoster, base.GetTotalAlternativeSolutionNeededMenCount(), ref explanation, 2, false);
			}

			// Token: 0x06000E7A RID: 3706 RVA: 0x000675AD File Offset: 0x000657AD
			public override bool IsTroopTypeNeededByAlternativeSolution(CharacterObject character)
			{
				return character.Tier >= 2;
			}

			// Token: 0x06000E7B RID: 3707 RVA: 0x000675BC File Offset: 0x000657BC
			protected override bool CanPlayerTakeQuestConditions(Hero issueGiver, out IssueBase.PreconditionFlags flag, out Hero relationHero, out SkillObject skill)
			{
				bool flag2 = issueGiver.GetRelationWithPlayer() >= -10f && !issueGiver.CurrentSettlement.MapFaction.IsAtWarWith(Hero.MainHero.MapFaction);
				flag = (flag2 ? 0 : ((!issueGiver.CurrentSettlement.MapFaction.IsAtWarWith(Hero.MainHero.MapFaction)) ? 1 : 64));
				relationHero = issueGiver;
				skill = null;
				return flag2;
			}

			// Token: 0x06000E7C RID: 3708 RVA: 0x00067628 File Offset: 0x00065828
			public override bool IssueStayAliveConditions()
			{
				return !base.IssueOwner.CurrentSettlement.IsRaided && !base.IssueOwner.CurrentSettlement.IsUnderRaid;
			}

			// Token: 0x06000E7D RID: 3709 RVA: 0x00067651 File Offset: 0x00065851
			protected override void CompleteIssueWithTimedOutConsequences()
			{
			}

			// Token: 0x06000E7E RID: 3710 RVA: 0x00067653 File Offset: 0x00065853
			internal static void AutoGeneratedStaticCollectObjectsNotableWantsDaughterFoundIssue(object o, List<object> collectedObjects)
			{
				((NotableWantsDaughterFoundIssueBehavior.NotableWantsDaughterFoundIssue)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			// Token: 0x06000E7F RID: 3711 RVA: 0x00067661 File Offset: 0x00065861
			protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			// Token: 0x04000620 RID: 1568
			private const int TroopTierForAlternativeSolution = 2;

			// Token: 0x04000621 RID: 1569
			private const int RequiredSkillLevelForAlternativeSolution = 120;
		}

		// Token: 0x02000151 RID: 337
		public class NotableWantsDaughterFoundIssueQuest : QuestBase
		{
			// Token: 0x17000138 RID: 312
			// (get) Token: 0x06000E80 RID: 3712 RVA: 0x0006766C File Offset: 0x0006586C
			public override TextObject Title
			{
				get
				{
					TextObject textObject = new TextObject("{=PDhmSieV}{QUEST_GIVER.NAME}'s Kidnapped Daughter at {SETTLEMENT}", null);
					textObject.SetTextVariable("SETTLEMENT", base.QuestGiver.CurrentSettlement.Name);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					return textObject;
				}
			}

			// Token: 0x17000139 RID: 313
			// (get) Token: 0x06000E81 RID: 3713 RVA: 0x000676BA File Offset: 0x000658BA
			public override bool IsRemainingTimeHidden
			{
				get
				{
					return false;
				}
			}

			// Token: 0x1700013A RID: 314
			// (get) Token: 0x06000E82 RID: 3714 RVA: 0x000676BD File Offset: 0x000658BD
			private bool DoesMainPartyHasEnoughScoutingSkill
			{
				get
				{
					return (float)MobilePartyHelper.GetMainPartySkillCounsellor(DefaultSkills.Scouting).GetSkillValue(DefaultSkills.Scouting) >= 150f * this._questDifficultyMultiplier;
				}
			}

			// Token: 0x1700013B RID: 315
			// (get) Token: 0x06000E83 RID: 3715 RVA: 0x000676E8 File Offset: 0x000658E8
			private TextObject _playerStartsQuestLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=1jExD58d}{QUEST_GIVER.LINK}, a merchant from {SETTLEMENT_NAME}, told you that {?QUEST_GIVER.GENDER}her{?}his{\\?} daughter {TARGET_HERO.NAME} has either been abducted or run off with a local rogue. You have agreed to search for her and bring her back to {SETTLEMENT_NAME}. If you cannot find their tracks when you exit settlement, you should visit the nearby villages of {SETTLEMENT_NAME} to look for clues and tracks of the kidnapper.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					StringHelpers.SetCharacterProperties("TARGET_HERO", this._daughterHero.CharacterObject, textObject, false);
					textObject.SetTextVariable("SETTLEMENT_NAME", base.QuestGiver.CurrentSettlement.EncyclopediaLinkWithName);
					textObject.SetTextVariable("BASE_REWARD_GOLD", this.RewardGold);
					textObject.SetTextVariable("GOLD_ICON", "{=!}<img src=\"General\\Icons\\Coin@2x\" extend=\"8\">");
					return textObject;
				}
			}

			// Token: 0x1700013C RID: 316
			// (get) Token: 0x06000E84 RID: 3716 RVA: 0x00067774 File Offset: 0x00065974
			private TextObject _successQuestLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=asVE53ac}Daughter returns to {QUEST_GIVER.LINK}. {?QUEST_GIVER.GENDER}She{?}He{\\?} is happy. Sends {?QUEST_GIVER.GENDER}her{?}his{\\?} regards with a large pouch of {BASE_REWARD}{GOLD_ICON}.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					textObject.SetTextVariable("GOLD_ICON", "{=!}<img src=\"General\\Icons\\Coin@2x\" extend=\"8\">");
					textObject.SetTextVariable("BASE_REWARD", this.RewardGold);
					return textObject;
				}
			}

			// Token: 0x1700013D RID: 317
			// (get) Token: 0x06000E85 RID: 3717 RVA: 0x000677CC File Offset: 0x000659CC
			private TextObject _failQuestLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=ak2EMWWR}You failed to bring the daughter back to her {?QUEST_GIVER.GENDER}mother{?}father{\\?} as promised to {QUEST_GIVER.LINK}. {QUEST_GIVER.LINK} is furious", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					return textObject;
				}
			}

			// Token: 0x1700013E RID: 318
			// (get) Token: 0x06000E86 RID: 3718 RVA: 0x00067800 File Offset: 0x00065A00
			private TextObject _questCanceledWarDeclaredLog
			{
				get
				{
					TextObject textObject = new TextObject("{=vW6kBki9}Your clan is now at war with {QUEST_GIVER.LINK}'s realm. Your agreement with {QUEST_GIVER.LINK} is canceled.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					return textObject;
				}
			}

			// Token: 0x1700013F RID: 319
			// (get) Token: 0x06000E87 RID: 3719 RVA: 0x00067834 File Offset: 0x00065A34
			private TextObject _playerDeclaredWarQuestLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=bqeWVVEE}Your actions have started a war with {QUEST_GIVER.LINK}'s faction. {?QUEST_GIVER.GENDER}She{?}He{\\?} cancels your agreement and the quest is a failure.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					return textObject;
				}
			}

			// Token: 0x06000E88 RID: 3720 RVA: 0x00067868 File Offset: 0x00065A68
			public NotableWantsDaughterFoundIssueQuest(string questId, Hero questGiver, CampaignTime duration, int baseReward, float issueDifficultyMultiplier)
				: base(questId, questGiver, duration, baseReward)
			{
				this._questDifficultyMultiplier = issueDifficultyMultiplier;
				this._targetVillage = Extensions.GetRandomElementWithPredicate<Village>(questGiver.CurrentSettlement.Village.Bound.BoundVillages, (Village x) => x != questGiver.CurrentSettlement.Village);
				Dictionary<string, CharacterObject> rogueCharacterBasedOnCulture = this._rogueCharacterBasedOnCulture;
				string text = "khuzait";
				Clan clan = Clan.BanditFactions.FirstOrDefault((Clan x) => x.StringId == "steppe_bandits");
				rogueCharacterBasedOnCulture.Add(text, (clan != null) ? clan.Culture.BanditBoss : null);
				Dictionary<string, CharacterObject> rogueCharacterBasedOnCulture2 = this._rogueCharacterBasedOnCulture;
				string text2 = "vlandia";
				Clan clan2 = Clan.BanditFactions.FirstOrDefault((Clan x) => x.StringId == "mountain_bandits");
				rogueCharacterBasedOnCulture2.Add(text2, (clan2 != null) ? clan2.Culture.BanditBoss : null);
				Dictionary<string, CharacterObject> rogueCharacterBasedOnCulture3 = this._rogueCharacterBasedOnCulture;
				string text3 = "aserai";
				Clan clan3 = Clan.BanditFactions.FirstOrDefault((Clan x) => x.StringId == "desert_bandits");
				rogueCharacterBasedOnCulture3.Add(text3, (clan3 != null) ? clan3.Culture.BanditBoss : null);
				Dictionary<string, CharacterObject> rogueCharacterBasedOnCulture4 = this._rogueCharacterBasedOnCulture;
				string text4 = "battania";
				Clan clan4 = Clan.BanditFactions.FirstOrDefault((Clan x) => x.StringId == "forest_bandits");
				rogueCharacterBasedOnCulture4.Add(text4, (clan4 != null) ? clan4.Culture.BanditBoss : null);
				Dictionary<string, CharacterObject> rogueCharacterBasedOnCulture5 = this._rogueCharacterBasedOnCulture;
				string text5 = "sturgia";
				Clan clan5 = Clan.BanditFactions.FirstOrDefault((Clan x) => x.StringId == "sea_raiders");
				rogueCharacterBasedOnCulture5.Add(text5, (clan5 != null) ? clan5.Culture.BanditBoss : null);
				Dictionary<string, CharacterObject> rogueCharacterBasedOnCulture6 = this._rogueCharacterBasedOnCulture;
				string text6 = "empire_w";
				Clan clan6 = Clan.BanditFactions.FirstOrDefault((Clan x) => x.StringId == "mountain_bandits");
				rogueCharacterBasedOnCulture6.Add(text6, (clan6 != null) ? clan6.Culture.BanditBoss : null);
				Dictionary<string, CharacterObject> rogueCharacterBasedOnCulture7 = this._rogueCharacterBasedOnCulture;
				string text7 = "empire_s";
				Clan clan7 = Clan.BanditFactions.FirstOrDefault((Clan x) => x.StringId == "mountain_bandits");
				rogueCharacterBasedOnCulture7.Add(text7, (clan7 != null) ? clan7.Culture.BanditBoss : null);
				Dictionary<string, CharacterObject> rogueCharacterBasedOnCulture8 = this._rogueCharacterBasedOnCulture;
				string text8 = "empire";
				Clan clan8 = Clan.BanditFactions.FirstOrDefault((Clan x) => x.StringId == "mountain_bandits");
				rogueCharacterBasedOnCulture8.Add(text8, (clan8 != null) ? clan8.Culture.BanditBoss : null);
				int heroComesOfAge = Campaign.Current.Models.AgeModel.HeroComesOfAge;
				int num = MBRandom.RandomInt(heroComesOfAge, 25);
				int num2 = MBRandom.RandomInt(heroComesOfAge, 25);
				CharacterObject randomElementWithPredicate = Extensions.GetRandomElementWithPredicate<CharacterObject>(questGiver.CurrentSettlement.Culture.NotableAndWandererTemplates, (CharacterObject x) => x.Occupation == 16 && x.IsFemale);
				this._daughterHero = HeroCreator.CreateSpecialHero(randomElementWithPredicate, questGiver.HomeSettlement, questGiver.Clan, null, num);
				this._daughterHero.CharacterObject.HiddenInEncylopedia = true;
				this._daughterHero.Father = questGiver;
				this._rogueHero = HeroCreator.CreateSpecialHero(this.GetRogueCharacterBasedOnCulture(questGiver.Culture), questGiver.HomeSettlement, questGiver.Clan, null, num2);
				this._rogueHero.CharacterObject.HiddenInEncylopedia = true;
				this.SetDialogs();
				base.InitializeQuestOnCreation();
			}

			// Token: 0x06000E89 RID: 3721 RVA: 0x00067C30 File Offset: 0x00065E30
			private CharacterObject GetRogueCharacterBasedOnCulture(CultureObject culture)
			{
				CharacterObject characterObject;
				if (this._rogueCharacterBasedOnCulture.ContainsKey(culture.StringId))
				{
					characterObject = this._rogueCharacterBasedOnCulture[culture.StringId];
				}
				else
				{
					characterObject = Extensions.GetRandomElementWithPredicate<CharacterObject>(base.QuestGiver.CurrentSettlement.Culture.NotableAndWandererTemplates, (CharacterObject x) => x.Occupation == 21 && !x.IsFemale);
				}
				characterObject.Culture = base.QuestGiver.Culture;
				return characterObject;
			}

			// Token: 0x06000E8A RID: 3722 RVA: 0x00067CB0 File Offset: 0x00065EB0
			protected override void SetDialogs()
			{
				TextObject textObject = new TextObject("{=PZq1EMcx}Thank you for your help. I am still very worried about my girl {TARGET_HERO.NAME}. Please find her and bring her back to me as soon as you can.", null);
				StringHelpers.SetCharacterProperties("TARGET_HERO", this._daughterHero.CharacterObject, textObject, false);
				TextObject textObject2 = new TextObject("{=sglD6abb}Please! Bring my daughter back.", null);
				TextObject textObject3 = new TextObject("{=ddEu5IFQ}I hope so.", null);
				TextObject textObject4 = new TextObject("{=IdKG3IaS}Good to hear that.", null);
				TextObject textObject5 = new TextObject("{=0hXofVLx}Don't worry I'll bring her.", null);
				TextObject textObject6 = new TextObject("{=zpqP5LsC}I'll go right away.", null);
				this.OfferDialogFlow = DialogFlow.CreateDialogFlow("issue_classic_quest_start", 100).NpcLine(textObject, null, null).Condition(() => Hero.OneToOneConversationHero == base.QuestGiver && !this._didPlayerBeatRouge)
					.Consequence(new ConversationSentence.OnConsequenceDelegate(this.QuestAcceptedConsequences))
					.CloseDialog();
				this.DiscussDialogFlow = DialogFlow.CreateDialogFlow("quest_discuss", 100).NpcLine(textObject2, null, null).Condition(() => Hero.OneToOneConversationHero == base.QuestGiver && !this._didPlayerBeatRouge)
					.BeginPlayerOptions()
					.PlayerOption(textObject5, null)
					.NpcLine(textObject3, null, null)
					.CloseDialog()
					.PlayerOption(textObject6, null)
					.NpcLine(textObject4, null, null)
					.CloseDialog();
				Campaign.Current.ConversationManager.AddDialogFlow(this.GetRougeDialogFlow(), this);
				Campaign.Current.ConversationManager.AddDialogFlow(this.GetDaughterAfterFightDialog(), this);
				Campaign.Current.ConversationManager.AddDialogFlow(this.GetDaughterAfterAcceptDialog(), this);
				Campaign.Current.ConversationManager.AddDialogFlow(this.GetDaughterAfterPersuadedDialog(), this);
				Campaign.Current.ConversationManager.AddDialogFlow(this.GetDaughterDialogWhenVillageRaid(), this);
				Campaign.Current.ConversationManager.AddDialogFlow(this.GetRougeAfterAcceptDialog(), this);
				Campaign.Current.ConversationManager.AddDialogFlow(this.GetRogueAfterPersuadedDialog(), this);
			}

			// Token: 0x06000E8B RID: 3723 RVA: 0x00067E54 File Offset: 0x00066054
			protected override void InitializeQuestOnGameLoad()
			{
				this.SetDialogs();
				if (this._daughterHero != null)
				{
					this._daughterHero.CharacterObject.HiddenInEncylopedia = true;
				}
				if (this._rogueHero != null)
				{
					this._rogueHero.CharacterObject.HiddenInEncylopedia = true;
				}
			}

			// Token: 0x06000E8C RID: 3724 RVA: 0x00067E8E File Offset: 0x0006608E
			private bool IsRougeHero(IAgent agent)
			{
				return agent.Character == this._rogueHero.CharacterObject;
			}

			// Token: 0x06000E8D RID: 3725 RVA: 0x00067EA3 File Offset: 0x000660A3
			private bool IsDaughterHero(IAgent agent)
			{
				return agent.Character == this._daughterHero.CharacterObject;
			}

			// Token: 0x06000E8E RID: 3726 RVA: 0x00067EB8 File Offset: 0x000660B8
			private bool IsMainHero(IAgent agent)
			{
				return agent.Character == CharacterObject.PlayerCharacter;
			}

			// Token: 0x06000E8F RID: 3727 RVA: 0x00067EC8 File Offset: 0x000660C8
			private bool multi_character_conversation_on_condition()
			{
				if (!this._villageIsRaidedTalkWithDaughter && !this._isDaughterPersuaded && !this._didPlayerBeatRouge && !this._acceptedDaughtersEscape && this._isQuestTargetMission && (CharacterObject.OneToOneConversationCharacter == this._daughterHero.CharacterObject || CharacterObject.OneToOneConversationCharacter == this._rogueHero.CharacterObject))
				{
					MBList<Agent> mblist = new MBList<Agent>();
					foreach (Agent agent in Mission.Current.GetNearbyAgents(Agent.Main.Position.AsVec2, 100f, mblist))
					{
						if (agent.Character == this._daughterHero.CharacterObject)
						{
							this._daughterAgent = agent;
							if (Mission.Current.GetMissionBehavior<MissionConversationLogic>() != null && Hero.OneToOneConversationHero != this._daughterHero)
							{
								Campaign.Current.ConversationManager.AddConversationAgents(new List<Agent> { this._daughterAgent }, true);
							}
						}
						else if (agent.Character == this._rogueHero.CharacterObject)
						{
							this._rogueAgent = agent;
							if (Mission.Current.GetMissionBehavior<MissionConversationLogic>() != null && Hero.OneToOneConversationHero != this._rogueHero)
							{
								Campaign.Current.ConversationManager.AddConversationAgents(new List<Agent> { this._rogueAgent }, true);
							}
						}
					}
					return this._daughterAgent != null && this._rogueAgent != null && this._daughterAgent.Health > 10f && this._rogueAgent.Health > 10f;
				}
				return false;
			}

			// Token: 0x06000E90 RID: 3728 RVA: 0x00068080 File Offset: 0x00066280
			private bool daughter_conversation_after_fight_on_condition()
			{
				return CharacterObject.OneToOneConversationCharacter == this._daughterHero.CharacterObject && this._didPlayerBeatRouge;
			}

			// Token: 0x06000E91 RID: 3729 RVA: 0x0006809C File Offset: 0x0006629C
			private void multi_agent_conversation_on_consequence()
			{
				this._task = this.GetPersuasionTask();
			}

			// Token: 0x06000E92 RID: 3730 RVA: 0x000680AC File Offset: 0x000662AC
			private DialogFlow GetRougeDialogFlow()
			{
				TextObject textObject = new TextObject("{=ovFbMMTJ}Who are you? Are you one of the bounty hunters sent by {QUEST_GIVER.LINK} to track us? Like we're animals or something? Look friend, we have done nothing wrong. As you may have figured out already, this woman and I, we love each other. I didn't force her to do anything.[ib:closed]", null);
				StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
				TextObject textObject2 = new TextObject("{=D25oY3j1}Thank you {?PLAYER.GENDER}lady{?}sir{\\?}. For your kindness and understanding. We won't forget this.", null);
				StringHelpers.SetCharacterProperties("PLAYER", CharacterObject.PlayerCharacter, textObject2, false);
				TextObject textObject3 = new TextObject("{=oL3amiu1}Come {DAUGHTER_NAME.NAME}, let's go before other hounds sniff our trail... I mean... No offense {?PLAYER.GENDER}madam{?}sir{\\?}.", null);
				StringHelpers.SetCharacterProperties("DAUGHTER_NAME", this._daughterHero.CharacterObject, textObject3, false);
				StringHelpers.SetCharacterProperties("PLAYER", CharacterObject.PlayerCharacter, textObject3, false);
				TextObject textObject4 = new TextObject("{=92sbq1YY}I'm no child, {?PLAYER.GENDER}lady{?}sir{\\?}! Draw your weapon! I challenge you to a duel!", null);
				StringHelpers.SetCharacterProperties("PLAYER", CharacterObject.PlayerCharacter, textObject4, false);
				TextObject textObject5 = new TextObject("{=jfzErupx}He is right! I ran away with him willingly. I love my {?QUEST_GIVER.GENDER}mother{?}father{\\?}, but {?QUEST_GIVER.GENDER}she{?}he{\\?} can be such a tyrant. Please {?PLAYER.GENDER}lady{?}sir{\\?}, if you believe in freedom and love, please leave us be.", null);
				StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject5, false);
				StringHelpers.SetCharacterProperties("PLAYER", CharacterObject.PlayerCharacter, textObject5, false);
				TextObject textObject6 = new TextObject("{=5NljlbLA}Thank you kind {?PLAYER.GENDER}lady{?}sir{\\?}, thank you.", null);
				StringHelpers.SetCharacterProperties("PLAYER", CharacterObject.PlayerCharacter, textObject6, false);
				TextObject textObject7 = new TextObject("{=i5fNZrhh}Please, {?PLAYER.GENDER}lady{?}sir{\\?}. I love him truly and I wish to spend the rest of my life with him. I beg of you, please don't stand in our way.", null);
				StringHelpers.SetCharacterProperties("PLAYER", CharacterObject.PlayerCharacter, textObject7, false);
				TextObject textObject8 = new TextObject("{=0RCdPKj2}Yes {?QUEST_GIVER.GENDER}she{?}he{\\?} would probably be sad. But not because of what you think. See, {QUEST_GIVER.LINK} promised me to one of {?QUEST_GIVER.GENDER}her{?}his{\\?} allies' sons and this will devastate {?QUEST_GIVER.GENDER}her{?}his{\\?} plans. That is true.", null);
				StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject8, false);
				TextObject textObject9 = new TextObject("{=5W7Kxfq9}I understand. If that is the case, I will let you go.", null);
				TextObject textObject10 = new TextObject("{=3XimdHOn}How do I know he's not forcing you to say that?", null);
				TextObject textObject11 = new TextObject("{=zNqDEuAw}But I've promised to find you and return you to your {?QUEST_GIVER.GENDER}mother{?}father{\\?}. {?QUEST_GIVER.GENDER}She{?}He{\\?} would be devastated.", null);
				StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject11, false);
				TextObject textObject12 = new TextObject("{=tuaQ5uU3}I guess the only way to free you from this pretty boy's spell is to kill him.", null);
				TextObject textObject13 = new TextObject("{=HDCmeGhG}I'm sorry but I gave a promise. I don't break my promises.", null);
				TextObject textObject14 = new TextObject("{=VGrHWxzf}This will be a massacre, not a duel, but I'm fine with that.", null);
				TextObject textObject15 = new TextObject("{=sytYViXb}I accept your duel.", null);
				DialogFlow dialogFlow = DialogFlow.CreateDialogFlow("start", 125).NpcLine(textObject, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsRougeHero), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainHero)).Condition(new ConversationSentence.OnConditionDelegate(this.multi_character_conversation_on_condition))
					.Consequence(new ConversationSentence.OnConsequenceDelegate(this.multi_agent_conversation_on_consequence))
					.NpcLine(textObject5, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsDaughterHero), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainHero))
					.BeginPlayerOptions()
					.PlayerOption(textObject9, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsDaughterHero))
					.NpcLine(textObject2, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsRougeHero), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainHero))
					.NpcLine(textObject3, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsRougeHero), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsDaughterHero))
					.NpcLine(textObject6, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsDaughterHero), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainHero))
					.Consequence(delegate
					{
						Campaign.Current.ConversationManager.ConversationEndOneShot += this.PlayerAcceptedDaughtersEscape;
					})
					.CloseDialog()
					.PlayerOption(textObject10, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsDaughterHero))
					.NpcLine(textObject7, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsDaughterHero), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainHero))
					.PlayerLine(textObject11, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsDaughterHero))
					.NpcLine(textObject8, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsDaughterHero), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainHero))
					.GotoDialogState("start_daughter_persuade_to_come_persuasion")
					.GoBackToDialogState("daughter_persuade_to_come_persuasion_finished")
					.PlayerLine((Hero.MainHero.GetTraitLevel(DefaultTraits.Mercy) < 0) ? textObject12 : textObject13, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsDaughterHero))
					.NpcLine(textObject4, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsRougeHero), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainHero))
					.BeginPlayerOptions()
					.PlayerOption(textObject14, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsRougeHero))
					.NpcLine(new TextObject("{=XWVW0oTB}You bastard![ib:aggressive][if:convo_furious]", null), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsRougeHero), null)
					.Consequence(delegate
					{
						Campaign.Current.ConversationManager.ConversationEndOneShot += this.PlayerRejectsDuelFight;
					})
					.CloseDialog()
					.PlayerOption(textObject15, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsRougeHero))
					.NpcLine(new TextObject("{=jqahxjWD}Heaven protect me![ib:aggressive][if:convo_astonished]", null), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsRougeHero), null)
					.Consequence(delegate
					{
						Campaign.Current.ConversationManager.ConversationEndOneShot += this.PlayerAcceptsDuelFight;
					})
					.CloseDialog()
					.EndPlayerOptions()
					.EndPlayerOptions()
					.CloseDialog();
				this.AddPersuasionDialogs(dialogFlow);
				return dialogFlow;
			}

			// Token: 0x06000E93 RID: 3731 RVA: 0x000684C4 File Offset: 0x000666C4
			private DialogFlow GetDaughterAfterFightDialog()
			{
				TextObject textObject = new TextObject("{=MN2v1AZQ}I hate you! You killed him! I can't believe it! I will hate you with all my heart till my dying days.[if:convo_angry]", null);
				TextObject textObject2 = new TextObject("{=TTkVcObg}What choice do I have, you heartless bastard?![if:convo_furious]", null);
				TextObject textObject3 = new TextObject("{=XqsrsjiL}I did what I had to do. Pack up, you need to go.", null);
				TextObject textObject4 = new TextObject("{=KQ3aYvp3}Some day you'll see I did you a favor. Pack up, you need to go.", null);
				return DialogFlow.CreateDialogFlow("start", 125).NpcLine(textObject, null, null).Condition(new ConversationSentence.OnConditionDelegate(this.daughter_conversation_after_fight_on_condition))
					.PlayerLine((Hero.MainHero.GetTraitLevel(DefaultTraits.Mercy) < 0) ? textObject3 : textObject4, null)
					.NpcLine(textObject2, null, null)
					.Consequence(delegate
					{
						Campaign.Current.ConversationManager.ConversationEndOneShot += this.PlayerWonTheFight;
					})
					.CloseDialog();
			}

			// Token: 0x06000E94 RID: 3732 RVA: 0x00068560 File Offset: 0x00066760
			private DialogFlow GetDaughterAfterAcceptDialog()
			{
				TextObject textObject = new TextObject("{=0Wg00sfN}Thank you, {?PLAYER.GENDER}madam{?}sir{\\?}. We will be moving immediately.", null);
				StringHelpers.SetCharacterProperties("PLAYER", CharacterObject.PlayerCharacter, textObject, false);
				TextObject textObject2 = new TextObject("{=kUReBc04}Good.", null);
				return DialogFlow.CreateDialogFlow("start", 125).NpcLine(textObject, null, null).Condition(new ConversationSentence.OnConditionDelegate(this.daughter_conversation_after_accept_on_condition))
					.PlayerLine(textObject2, null)
					.CloseDialog();
			}

			// Token: 0x06000E95 RID: 3733 RVA: 0x000685C8 File Offset: 0x000667C8
			private bool daughter_conversation_after_accept_on_condition()
			{
				return CharacterObject.OneToOneConversationCharacter == this._daughterHero.CharacterObject && this._acceptedDaughtersEscape;
			}

			// Token: 0x06000E96 RID: 3734 RVA: 0x000685E4 File Offset: 0x000667E4
			private DialogFlow GetDaughterAfterPersuadedDialog()
			{
				TextObject textObject = new TextObject("{=B8bHpJRP}You are right, {?PLAYER.GENDER}my lady{?}sir{\\?}. I should be moving immediately.", null);
				StringHelpers.SetCharacterProperties("PLAYER", CharacterObject.PlayerCharacter, textObject, false);
				TextObject textObject2 = new TextObject("{=kUReBc04}Good.", null);
				return DialogFlow.CreateDialogFlow("start", 125).NpcLine(textObject, null, null).Condition(new ConversationSentence.OnConditionDelegate(this.daughter_conversation_after_persuaded_on_condition))
					.PlayerLine(textObject2, null)
					.CloseDialog();
			}

			// Token: 0x06000E97 RID: 3735 RVA: 0x0006864C File Offset: 0x0006684C
			private DialogFlow GetDaughterDialogWhenVillageRaid()
			{
				return DialogFlow.CreateDialogFlow("start", 125).NpcLine(new TextObject("{=w0HPC53e}Who are you? What do you want from me?", null), null, null).Condition(() => this._villageIsRaidedTalkWithDaughter)
					.PlayerLine(new TextObject("{=iRupMGI0}Calm down! Your father has sent me to find you.", null), null)
					.NpcLine(new TextObject("{=dwNquUNr}My father? Oh, thank god! I saw terrible things. They took my beloved one and slew many innocents without hesitation.", null), null, null)
					.PlayerLine("{=HtAr22re}Try to forget all about these and return to your father's house.", null)
					.NpcLine("{=FgSIsasF}Yes, you are right. I shall be on my way...", null, null)
					.Consequence(delegate
					{
						Campaign.Current.ConversationManager.ConversationEndOneShot += delegate
						{
							this.ApplyDeliverySuccessConsequences();
							base.CompleteQuestWithSuccess();
							base.AddLog(this._successQuestLogText, false);
							this._villageIsRaidedTalkWithDaughter = false;
						};
					})
					.CloseDialog();
			}

			// Token: 0x06000E98 RID: 3736 RVA: 0x000686D8 File Offset: 0x000668D8
			private bool daughter_conversation_after_persuaded_on_condition()
			{
				return CharacterObject.OneToOneConversationCharacter == this._daughterHero.CharacterObject && this._isDaughterPersuaded;
			}

			// Token: 0x06000E99 RID: 3737 RVA: 0x000686F4 File Offset: 0x000668F4
			private DialogFlow GetRougeAfterAcceptDialog()
			{
				TextObject textObject = new TextObject("{=wlKtDR2z}Thank you, {?PLAYER.GENDER}my lady{?}sir{\\?}.", null);
				StringHelpers.SetCharacterProperties("PLAYER", CharacterObject.PlayerCharacter, textObject, false);
				return DialogFlow.CreateDialogFlow("start", 125).NpcLine(textObject, null, null).Condition(new ConversationSentence.OnConditionDelegate(this.rogue_conversation_after_accept_on_condition))
					.PlayerLine(new TextObject("{=0YJGvJ7o}You should leave now.", null), null)
					.NpcLine(new TextObject("{=6Q4cPOSG}Yes, we will.", null), null, null)
					.CloseDialog();
			}

			// Token: 0x06000E9A RID: 3738 RVA: 0x0006876C File Offset: 0x0006696C
			private bool rogue_conversation_after_accept_on_condition()
			{
				return CharacterObject.OneToOneConversationCharacter == this._rogueHero.CharacterObject && this._acceptedDaughtersEscape;
			}

			// Token: 0x06000E9B RID: 3739 RVA: 0x00068788 File Offset: 0x00066988
			private DialogFlow GetRogueAfterPersuadedDialog()
			{
				TextObject textObject = new TextObject("{=GFt9KiHP}You are right. Maybe we need to persuade {QUEST_GIVER.NAME}", null);
				StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
				TextObject textObject2 = new TextObject("{=btJkBTSF}I am sure you can solve it.", null);
				return DialogFlow.CreateDialogFlow("start", 125).NpcLine(textObject, null, null).Condition(new ConversationSentence.OnConditionDelegate(this.rogue_conversation_after_persuaded_on_condition))
					.PlayerLine(textObject2, null)
					.CloseDialog();
			}

			// Token: 0x06000E9C RID: 3740 RVA: 0x000687F6 File Offset: 0x000669F6
			private bool rogue_conversation_after_persuaded_on_condition()
			{
				return CharacterObject.OneToOneConversationCharacter == this._rogueHero.CharacterObject && this._isDaughterPersuaded;
			}

			// Token: 0x06000E9D RID: 3741 RVA: 0x00068814 File Offset: 0x00066A14
			protected override void OnTimedOut()
			{
				this.ApplyDeliveryRejectedFailConsequences();
				TextObject textObject = new TextObject("{=KAvwytDK}You didn't bring {DAUGHTER.NAME} to {QUEST_GIVER.LINK}. {?QUEST_GIVER.GENDER}she{?}he{\\?} must be furious.", null);
				StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
				StringHelpers.SetCharacterProperties("DAUGHTER", this._daughterHero.CharacterObject, textObject, false);
				base.AddLog(textObject, false);
			}

			// Token: 0x06000E9E RID: 3742 RVA: 0x0006886C File Offset: 0x00066A6C
			private void PlayerAcceptedDaughtersEscape()
			{
				this._acceptedDaughtersEscape = true;
			}

			// Token: 0x06000E9F RID: 3743 RVA: 0x00068875 File Offset: 0x00066A75
			private void PlayerWonTheFight()
			{
				this._isDaughterCaptured = true;
				Mission.Current.SetMissionMode(0, false);
			}

			// Token: 0x06000EA0 RID: 3744 RVA: 0x0006888C File Offset: 0x00066A8C
			private void ApplyDaughtersEscapeAcceptedFailConsequences()
			{
				this.RelationshipChangeWithQuestGiver = -10;
				if (base.QuestGiver.CurrentSettlement.Village.Bound != null)
				{
					base.QuestGiver.CurrentSettlement.Village.Bound.Town.Security -= 5f;
					base.QuestGiver.CurrentSettlement.Village.Bound.Prosperity -= 5f;
				}
			}

			// Token: 0x06000EA1 RID: 3745 RVA: 0x0006890C File Offset: 0x00066B0C
			private void ApplyDeliveryRejectedFailConsequences()
			{
				this.RelationshipChangeWithQuestGiver = -10;
				if (base.QuestGiver.CurrentSettlement.Village.Bound != null)
				{
					base.QuestGiver.CurrentSettlement.Village.Bound.Town.Security -= 5f;
					base.QuestGiver.CurrentSettlement.Village.Bound.Prosperity -= 5f;
				}
			}

			// Token: 0x06000EA2 RID: 3746 RVA: 0x0006898C File Offset: 0x00066B8C
			private void ApplyDeliverySuccessConsequences()
			{
				GainRenownAction.Apply(Hero.MainHero, 2f, false);
				base.QuestGiver.AddPower(10f);
				this.RelationshipChangeWithQuestGiver = 10;
				if (base.QuestGiver.CurrentSettlement.Village.Bound != null)
				{
					base.QuestGiver.CurrentSettlement.Village.Bound.Town.Security += 10f;
				}
				GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, this.RewardGold, false);
			}

			// Token: 0x06000EA3 RID: 3747 RVA: 0x00068A18 File Offset: 0x00066C18
			private void PlayerRejectsDuelFight()
			{
				this._rogueAgent = (Agent)MissionConversationLogic.Current.ConversationManager.ConversationAgents.First((IAgent x) => !x.Character.IsFemale);
				List<Agent> list = new List<Agent> { Agent.Main };
				List<Agent> list2 = new List<Agent> { this._rogueAgent };
				MBList<Agent> mblist = new MBList<Agent>();
				foreach (Agent agent in Mission.Current.GetNearbyAgents(Agent.Main.Position.AsVec2, 30f, mblist))
				{
					foreach (Hero hero in Hero.MainHero.CompanionsInParty)
					{
						if (agent.Character == hero.CharacterObject)
						{
							list.Add(agent);
							break;
						}
					}
				}
				this._rogueAgent.Health = (float)(150 + list.Count * 20);
				this._rogueAgent.Defensiveness = 1f;
				Mission.Current.GetMissionBehavior<MissionFightHandler>().StartCustomFight(list, list2, false, false, new MissionFightHandler.OnFightEndDelegate(this.StartConversationAfterFight));
			}

			// Token: 0x06000EA4 RID: 3748 RVA: 0x00068B8C File Offset: 0x00066D8C
			private void PlayerAcceptsDuelFight()
			{
				this._rogueAgent = (Agent)MissionConversationLogic.Current.ConversationManager.ConversationAgents.First((IAgent x) => !x.Character.IsFemale);
				List<Agent> list = new List<Agent> { Agent.Main };
				List<Agent> list2 = new List<Agent> { this._rogueAgent };
				MBList<Agent> mblist = new MBList<Agent>();
				foreach (Agent agent in Mission.Current.GetNearbyAgents(Agent.Main.Position.AsVec2, 30f, mblist))
				{
					foreach (Hero hero in Hero.MainHero.CompanionsInParty)
					{
						if (agent.Character == hero.CharacterObject)
						{
							agent.SetTeam(Mission.Current.SpectatorTeam, false);
							DailyBehaviorGroup behaviorGroup = agent.GetComponent<CampaignAgentComponent>().AgentNavigator.GetBehaviorGroup<DailyBehaviorGroup>();
							if (behaviorGroup.GetActiveBehavior() is FollowAgentBehavior)
							{
								behaviorGroup.GetBehavior<FollowAgentBehavior>().SetTargetAgent(null);
								break;
							}
							break;
						}
					}
				}
				this._rogueAgent.Health = 200f;
				Mission.Current.GetMissionBehavior<MissionFightHandler>().StartCustomFight(list, list2, false, false, new MissionFightHandler.OnFightEndDelegate(this.StartConversationAfterFight));
			}

			// Token: 0x06000EA5 RID: 3749 RVA: 0x00068D24 File Offset: 0x00066F24
			private void StartConversationAfterFight(bool isPlayerSideWon)
			{
				if (isPlayerSideWon)
				{
					this._didPlayerBeatRouge = true;
					Campaign.Current.ConversationManager.SetupAndStartMissionConversation(this._daughterAgent, Mission.Current.MainAgent, false);
					TraitLevelingHelper.OnHostileAction(-50);
					return;
				}
				TextObject textObject = new TextObject("{=i1sth9Ls}You were defeated by the rogue. He and {TARGET_HERO.NAME} ran off while you were unconscious. You failed to bring the daughter back to her {?QUEST_GIVER.GENDER}mother{?}father{\\?} as promised to {QUEST_GIVER.LINK}. {?QUEST_GIVER.GENDER}She{?}He{\\?} is furious.", null);
				StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
				StringHelpers.SetCharacterProperties("TARGET_HERO", this._daughterHero.CharacterObject, textObject, false);
				base.CompleteQuestWithFail(textObject);
				this._isQuestTargetMission = false;
			}

			// Token: 0x06000EA6 RID: 3750 RVA: 0x00068DB0 File Offset: 0x00066FB0
			private void AddPersuasionDialogs(DialogFlow dialog)
			{
				TextObject textObject = new TextObject("{=ob5SejgJ}I will not abandon my love, {?PLAYER.GENDER}lady{?}sir{\\?}!", null);
				StringHelpers.SetCharacterProperties("PLAYER", CharacterObject.PlayerCharacter, textObject, false);
				TextObject textObject2 = new TextObject("{=cqe8FU8M}{?QUEST_GIVER.GENDER}She{?}He{\\?} cares nothing about me! Only about {?QUEST_GIVER.GENDER}her{?}his{\\?} reputation in our district.", null);
				StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject2, false);
				dialog.AddDialogLine("daughter_persuade_to_come_introduction", "start_daughter_persuade_to_come_persuasion", "daughter_persuade_to_come_start_reservation", textObject2.ToString(), null, new ConversationSentence.OnConsequenceDelegate(this.persuasion_start_with_daughter_on_consequence), this, 100, null, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsDaughterHero), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainHero));
				dialog.AddDialogLine("daughter_persuade_to_come_rejected", "daughter_persuade_to_come_start_reservation", "daughter_persuade_to_come_persuasion_failed", "{=!}{FAILED_PERSUASION_LINE}", new ConversationSentence.OnConditionDelegate(this.daughter_persuade_to_come_persuasion_failed_on_condition), new ConversationSentence.OnConsequenceDelegate(this.daughter_persuade_to_come_persuasion_failed_on_consequence), this, 100, null, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsDaughterHero), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainHero));
				dialog.AddDialogLine("daughter_persuade_to_come_failed", "daughter_persuade_to_come_persuasion_failed", "daughter_persuade_to_come_persuasion_finished", textObject.ToString(), null, null, this, 100, null, null, null);
				dialog.AddDialogLine("daughter_persuade_to_come_start", "daughter_persuade_to_come_start_reservation", "daughter_persuade_to_come_persuasion_select_option", "{=9b2BETct}I have already decided. Don't expect me to change my mind.", () => !this.daughter_persuade_to_come_persuasion_failed_on_condition(), null, this, 100, null, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsDaughterHero), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainHero));
				dialog.AddDialogLine("daughter_persuade_to_come_success", "daughter_persuade_to_come_start_reservation", "close_window", "{=3tmXBpRH}You're right. I cannot do this. I will return to my family. ", new ConversationSentence.OnConditionDelegate(ConversationManager.GetPersuasionProgressSatisfied), new ConversationSentence.OnConsequenceDelegate(this.daughter_persuade_to_come_persuasion_success_on_consequence), this, int.MaxValue, null, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsDaughterHero), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainHero));
				string text = "daughter_persuade_to_come_select_option_1";
				string text2 = "daughter_persuade_to_come_persuasion_select_option";
				string text3 = "daughter_persuade_to_come_persuasion_selected_option_response";
				string text4 = "{=!}{DAUGHTER_PERSUADE_TO_COME_PERSUADE_ATTEMPT_1}";
				ConversationSentence.OnConditionDelegate onConditionDelegate = new ConversationSentence.OnConditionDelegate(this.persuasion_select_option_1_on_condition);
				ConversationSentence.OnConsequenceDelegate onConsequenceDelegate = new ConversationSentence.OnConsequenceDelegate(this.persuasion_select_option_1_on_consequence);
				ConversationSentence.OnPersuasionOptionDelegate onPersuasionOptionDelegate = new ConversationSentence.OnPersuasionOptionDelegate(this.persuasion_setup_option_1);
				ConversationSentence.OnClickableConditionDelegate onClickableConditionDelegate = new ConversationSentence.OnClickableConditionDelegate(this.persuasion_clickable_option_1_on_condition);
				dialog.AddPlayerLine(text, text2, text3, text4, onConditionDelegate, onConsequenceDelegate, this, 100, onClickableConditionDelegate, onPersuasionOptionDelegate, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainHero), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsDaughterHero));
				string text5 = "daughter_persuade_to_come_select_option_2";
				string text6 = "daughter_persuade_to_come_persuasion_select_option";
				string text7 = "daughter_persuade_to_come_persuasion_selected_option_response";
				string text8 = "{=!}{DAUGHTER_PERSUADE_TO_COME_PERSUADE_ATTEMPT_2}";
				ConversationSentence.OnConditionDelegate onConditionDelegate2 = new ConversationSentence.OnConditionDelegate(this.persuasion_select_option_2_on_condition);
				ConversationSentence.OnConsequenceDelegate onConsequenceDelegate2 = new ConversationSentence.OnConsequenceDelegate(this.persuasion_select_option_2_on_consequence);
				onPersuasionOptionDelegate = new ConversationSentence.OnPersuasionOptionDelegate(this.persuasion_setup_option_2);
				onClickableConditionDelegate = new ConversationSentence.OnClickableConditionDelegate(this.persuasion_clickable_option_2_on_condition);
				dialog.AddPlayerLine(text5, text6, text7, text8, onConditionDelegate2, onConsequenceDelegate2, this, 100, onClickableConditionDelegate, onPersuasionOptionDelegate, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainHero), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsDaughterHero));
				string text9 = "daughter_persuade_to_come_select_option_3";
				string text10 = "daughter_persuade_to_come_persuasion_select_option";
				string text11 = "daughter_persuade_to_come_persuasion_selected_option_response";
				string text12 = "{=!}{DAUGHTER_PERSUADE_TO_COME_PERSUADE_ATTEMPT_3}";
				ConversationSentence.OnConditionDelegate onConditionDelegate3 = new ConversationSentence.OnConditionDelegate(this.persuasion_select_option_3_on_condition);
				ConversationSentence.OnConsequenceDelegate onConsequenceDelegate3 = new ConversationSentence.OnConsequenceDelegate(this.persuasion_select_option_3_on_consequence);
				onPersuasionOptionDelegate = new ConversationSentence.OnPersuasionOptionDelegate(this.persuasion_setup_option_3);
				onClickableConditionDelegate = new ConversationSentence.OnClickableConditionDelegate(this.persuasion_clickable_option_3_on_condition);
				dialog.AddPlayerLine(text9, text10, text11, text12, onConditionDelegate3, onConsequenceDelegate3, this, 100, onClickableConditionDelegate, onPersuasionOptionDelegate, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainHero), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsDaughterHero));
				string text13 = "daughter_persuade_to_come_select_option_4";
				string text14 = "daughter_persuade_to_come_persuasion_select_option";
				string text15 = "daughter_persuade_to_come_persuasion_selected_option_response";
				string text16 = "{=!}{DAUGHTER_PERSUADE_TO_COME_PERSUADE_ATTEMPT_4}";
				ConversationSentence.OnConditionDelegate onConditionDelegate4 = new ConversationSentence.OnConditionDelegate(this.persuasion_select_option_4_on_condition);
				ConversationSentence.OnConsequenceDelegate onConsequenceDelegate4 = new ConversationSentence.OnConsequenceDelegate(this.persuasion_select_option_4_on_consequence);
				onPersuasionOptionDelegate = new ConversationSentence.OnPersuasionOptionDelegate(this.persuasion_setup_option_4);
				onClickableConditionDelegate = new ConversationSentence.OnClickableConditionDelegate(this.persuasion_clickable_option_4_on_condition);
				dialog.AddPlayerLine(text13, text14, text15, text16, onConditionDelegate4, onConsequenceDelegate4, this, 100, onClickableConditionDelegate, onPersuasionOptionDelegate, new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainHero), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsDaughterHero));
				dialog.AddDialogLine("daughter_persuade_to_come_select_option_reaction", "daughter_persuade_to_come_persuasion_selected_option_response", "daughter_persuade_to_come_start_reservation", "{=D0xDRqvm}{PERSUASION_REACTION}", new ConversationSentence.OnConditionDelegate(this.persuasion_selected_option_response_on_condition), new ConversationSentence.OnConsequenceDelegate(this.persuasion_selected_option_response_on_consequence), this, 100, null, null, null);
			}

			// Token: 0x06000EA7 RID: 3751 RVA: 0x00069130 File Offset: 0x00067330
			private void persuasion_selected_option_response_on_consequence()
			{
				Tuple<PersuasionOptionArgs, PersuasionOptionResult> tuple = ConversationManager.GetPersuasionChosenOptions().Last<Tuple<PersuasionOptionArgs, PersuasionOptionResult>>();
				float difficulty = Campaign.Current.Models.PersuasionModel.GetDifficulty(5);
				float num;
				float num2;
				Campaign.Current.Models.PersuasionModel.GetEffectChances(tuple.Item1, ref num, ref num2, difficulty);
				this._task.ApplyEffects(num, num2);
			}

			// Token: 0x06000EA8 RID: 3752 RVA: 0x0006918C File Offset: 0x0006738C
			private bool persuasion_selected_option_response_on_condition()
			{
				PersuasionOptionResult item = ConversationManager.GetPersuasionChosenOptions().Last<Tuple<PersuasionOptionArgs, PersuasionOptionResult>>().Item2;
				MBTextManager.SetTextVariable("PERSUASION_REACTION", PersuasionHelper.GetDefaultPersuasionOptionReaction(item), false);
				return true;
			}

			// Token: 0x06000EA9 RID: 3753 RVA: 0x000691BC File Offset: 0x000673BC
			private bool persuasion_select_option_1_on_condition()
			{
				if (this._task.Options.Count > 0)
				{
					TextObject textObject = new TextObject("{=bSo9hKwr}{PERSUASION_OPTION_LINE} {SUCCESS_CHANCE}", null);
					textObject.SetTextVariable("SUCCESS_CHANCE", PersuasionHelper.ShowSuccess(this._task.Options.ElementAt(0), false));
					textObject.SetTextVariable("PERSUASION_OPTION_LINE", this._task.Options.ElementAt(0).Line);
					MBTextManager.SetTextVariable("DAUGHTER_PERSUADE_TO_COME_PERSUADE_ATTEMPT_1", textObject, false);
					return true;
				}
				return false;
			}

			// Token: 0x06000EAA RID: 3754 RVA: 0x0006923C File Offset: 0x0006743C
			private bool persuasion_select_option_2_on_condition()
			{
				if (this._task.Options.Count > 1)
				{
					TextObject textObject = new TextObject("{=bSo9hKwr}{PERSUASION_OPTION_LINE} {SUCCESS_CHANCE}", null);
					textObject.SetTextVariable("SUCCESS_CHANCE", PersuasionHelper.ShowSuccess(this._task.Options.ElementAt(1), false));
					textObject.SetTextVariable("PERSUASION_OPTION_LINE", this._task.Options.ElementAt(1).Line);
					MBTextManager.SetTextVariable("DAUGHTER_PERSUADE_TO_COME_PERSUADE_ATTEMPT_2", textObject, false);
					return true;
				}
				return false;
			}

			// Token: 0x06000EAB RID: 3755 RVA: 0x000692BC File Offset: 0x000674BC
			private bool persuasion_select_option_3_on_condition()
			{
				if (this._task.Options.Count > 2)
				{
					TextObject textObject = new TextObject("{=bSo9hKwr}{PERSUASION_OPTION_LINE} {SUCCESS_CHANCE}", null);
					textObject.SetTextVariable("SUCCESS_CHANCE", PersuasionHelper.ShowSuccess(this._task.Options.ElementAt(2), false));
					textObject.SetTextVariable("PERSUASION_OPTION_LINE", this._task.Options.ElementAt(2).Line);
					MBTextManager.SetTextVariable("DAUGHTER_PERSUADE_TO_COME_PERSUADE_ATTEMPT_3", textObject, false);
					return true;
				}
				return false;
			}

			// Token: 0x06000EAC RID: 3756 RVA: 0x0006933C File Offset: 0x0006753C
			private bool persuasion_select_option_4_on_condition()
			{
				if (this._task.Options.Count > 3)
				{
					TextObject textObject = new TextObject("{=bSo9hKwr}{PERSUASION_OPTION_LINE} {SUCCESS_CHANCE}", null);
					textObject.SetTextVariable("SUCCESS_CHANCE", PersuasionHelper.ShowSuccess(this._task.Options.ElementAt(3), false));
					textObject.SetTextVariable("PERSUASION_OPTION_LINE", this._task.Options.ElementAt(3).Line);
					MBTextManager.SetTextVariable("DAUGHTER_PERSUADE_TO_COME_PERSUADE_ATTEMPT_4", textObject, false);
					return true;
				}
				return false;
			}

			// Token: 0x06000EAD RID: 3757 RVA: 0x000693BC File Offset: 0x000675BC
			private void persuasion_select_option_1_on_consequence()
			{
				if (this._task.Options.Count > 0)
				{
					this._task.Options[0].BlockTheOption(true);
				}
			}

			// Token: 0x06000EAE RID: 3758 RVA: 0x000693E8 File Offset: 0x000675E8
			private void persuasion_select_option_2_on_consequence()
			{
				if (this._task.Options.Count > 1)
				{
					this._task.Options[1].BlockTheOption(true);
				}
			}

			// Token: 0x06000EAF RID: 3759 RVA: 0x00069414 File Offset: 0x00067614
			private void persuasion_select_option_3_on_consequence()
			{
				if (this._task.Options.Count > 2)
				{
					this._task.Options[2].BlockTheOption(true);
				}
			}

			// Token: 0x06000EB0 RID: 3760 RVA: 0x00069440 File Offset: 0x00067640
			private void persuasion_select_option_4_on_consequence()
			{
				if (this._task.Options.Count > 3)
				{
					this._task.Options[3].BlockTheOption(true);
				}
			}

			// Token: 0x06000EB1 RID: 3761 RVA: 0x0006946C File Offset: 0x0006766C
			private PersuasionOptionArgs persuasion_setup_option_1()
			{
				return this._task.Options.ElementAt(0);
			}

			// Token: 0x06000EB2 RID: 3762 RVA: 0x0006947F File Offset: 0x0006767F
			private PersuasionOptionArgs persuasion_setup_option_2()
			{
				return this._task.Options.ElementAt(1);
			}

			// Token: 0x06000EB3 RID: 3763 RVA: 0x00069492 File Offset: 0x00067692
			private PersuasionOptionArgs persuasion_setup_option_3()
			{
				return this._task.Options.ElementAt(2);
			}

			// Token: 0x06000EB4 RID: 3764 RVA: 0x000694A5 File Offset: 0x000676A5
			private PersuasionOptionArgs persuasion_setup_option_4()
			{
				return this._task.Options.ElementAt(3);
			}

			// Token: 0x06000EB5 RID: 3765 RVA: 0x000694B8 File Offset: 0x000676B8
			private bool persuasion_clickable_option_1_on_condition(out TextObject hintText)
			{
				hintText = new TextObject("{=9ACJsI6S}Blocked", null);
				if (this._task.Options.Count > 0)
				{
					hintText = (this._task.Options.ElementAt(0).IsBlocked ? hintText : TextObject.Empty);
					return !this._task.Options.ElementAt(0).IsBlocked;
				}
				return false;
			}

			// Token: 0x06000EB6 RID: 3766 RVA: 0x00069524 File Offset: 0x00067724
			private bool persuasion_clickable_option_2_on_condition(out TextObject hintText)
			{
				hintText = new TextObject("{=9ACJsI6S}Blocked", null);
				if (this._task.Options.Count > 1)
				{
					hintText = (this._task.Options.ElementAt(1).IsBlocked ? hintText : TextObject.Empty);
					return !this._task.Options.ElementAt(1).IsBlocked;
				}
				return false;
			}

			// Token: 0x06000EB7 RID: 3767 RVA: 0x00069590 File Offset: 0x00067790
			private bool persuasion_clickable_option_3_on_condition(out TextObject hintText)
			{
				hintText = new TextObject("{=9ACJsI6S}Blocked", null);
				if (this._task.Options.Count > 2)
				{
					hintText = (this._task.Options.ElementAt(2).IsBlocked ? hintText : TextObject.Empty);
					return !this._task.Options.ElementAt(2).IsBlocked;
				}
				return false;
			}

			// Token: 0x06000EB8 RID: 3768 RVA: 0x000695FC File Offset: 0x000677FC
			private bool persuasion_clickable_option_4_on_condition(out TextObject hintText)
			{
				hintText = new TextObject("{=9ACJsI6S}Blocked", null);
				if (this._task.Options.Count > 3)
				{
					hintText = (this._task.Options.ElementAt(3).IsBlocked ? hintText : TextObject.Empty);
					return !this._task.Options.ElementAt(3).IsBlocked;
				}
				return false;
			}

			// Token: 0x06000EB9 RID: 3769 RVA: 0x00069668 File Offset: 0x00067868
			private PersuasionTask GetPersuasionTask()
			{
				PersuasionTask persuasionTask = new PersuasionTask(0);
				persuasionTask.FinalFailLine = new TextObject("{=5aDlmdmb}No... No. It does not make sense.", null);
				persuasionTask.TryLaterLine = TextObject.Empty;
				persuasionTask.SpokenLine = new TextObject("{=6P1ruzsC}Maybe...", null);
				PersuasionOptionArgs persuasionOptionArgs = new PersuasionOptionArgs(DefaultSkills.Leadership, DefaultTraits.Honor, 0, -1, true, new TextObject("{=Nhfl6tcM}Maybe, but that is your duty to your family.", null), null, false, false, false);
				persuasionTask.AddOptionToTask(persuasionOptionArgs);
				TextObject textObject = new TextObject("{=lustkZ7s}Perhaps {?QUEST_GIVER.GENDER}she{?}he{\\?} made those plans because {?QUEST_GIVER.GENDER}she{?}he{\\?} loves you.", null);
				StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
				PersuasionOptionArgs persuasionOptionArgs2 = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Mercy, 0, 2, false, textObject, null, false, false, false);
				persuasionTask.AddOptionToTask(persuasionOptionArgs2);
				PersuasionOptionArgs persuasionOptionArgs3 = new PersuasionOptionArgs(DefaultSkills.Roguery, DefaultTraits.Calculating, 0, -2, false, new TextObject("{=Ns6Svjsn}Do you think this one will be faithful to you over many years? I know a rogue when I see one.", null), null, false, false, false);
				persuasionTask.AddOptionToTask(persuasionOptionArgs3);
				PersuasionOptionArgs persuasionOptionArgs4 = new PersuasionOptionArgs(DefaultSkills.Roguery, DefaultTraits.Mercy, 1, -3, true, new TextObject("{=2dL6j8Hp}You want to marry a corpse? Because I'm going to kill your lover if you don't listen.", null), null, true, false, false);
				persuasionTask.AddOptionToTask(persuasionOptionArgs4);
				return persuasionTask;
			}

			// Token: 0x06000EBA RID: 3770 RVA: 0x0006976A File Offset: 0x0006796A
			private void persuasion_start_with_daughter_on_consequence()
			{
				ConversationManager.StartPersuasion(2f, 1f, 0f, 2f, 2f, 0f, 5);
			}

			// Token: 0x06000EBB RID: 3771 RVA: 0x00069790 File Offset: 0x00067990
			private void daughter_persuade_to_come_persuasion_success_on_consequence()
			{
				ConversationManager.EndPersuasion();
				this._isDaughterPersuaded = true;
			}

			// Token: 0x06000EBC RID: 3772 RVA: 0x000697A0 File Offset: 0x000679A0
			private bool daughter_persuade_to_come_persuasion_failed_on_condition()
			{
				if (this._task.Options.All((PersuasionOptionArgs x) => x.IsBlocked) && !ConversationManager.GetPersuasionProgressSatisfied())
				{
					MBTextManager.SetTextVariable("FAILED_PERSUASION_LINE", this._task.FinalFailLine, false);
					return true;
				}
				return false;
			}

			// Token: 0x06000EBD RID: 3773 RVA: 0x000697FE File Offset: 0x000679FE
			private void daughter_persuade_to_come_persuasion_failed_on_consequence()
			{
				ConversationManager.EndPersuasion();
			}

			// Token: 0x06000EBE RID: 3774 RVA: 0x00069808 File Offset: 0x00067A08
			private void OnSettlementLeft(MobileParty party, Settlement settlement)
			{
				if (party.IsMainParty && settlement == base.QuestGiver.CurrentSettlement && this._exitedQuestSettlementForTheFirstTime)
				{
					if (this.DoesMainPartyHasEnoughScoutingSkill)
					{
						QuestHelper.AddMapArrowFromPointToTarget(new TextObject("{=YdwLnWa1}Direction of daughter and rogue", null), settlement.Position2D, this._targetVillage.Settlement.Position2D, 5f, 0.1f);
						MBInformationManager.AddQuickInformation(new TextObject("{=O15PyNUK}With the help of your scouting skill, you were able to trace their tracks.", null), 0, null, "");
						MBInformationManager.AddQuickInformation(new TextObject("{=gOWebWiK}Their direction is marked with an arrow in the campaign map.", null), 0, null, "");
						base.AddTrackedObject(this._targetVillage.Settlement);
					}
					else
					{
						foreach (Village village in base.QuestGiver.CurrentSettlement.Village.Bound.BoundVillages)
						{
							if (village != base.QuestGiver.CurrentSettlement.Village)
							{
								this._villagesAndAlreadyVisitedBooleans.Add(village, false);
								base.AddTrackedObject(village.Settlement);
							}
						}
					}
					TextObject textObject = new TextObject("{=FvtAJE2Q}In order to find {QUEST_GIVER.LINK}'s daughter, you have decided to visit nearby villages.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					base.AddLog(textObject, this.DoesMainPartyHasEnoughScoutingSkill);
					this._exitedQuestSettlementForTheFirstTime = false;
				}
				if (party.IsMainParty && settlement == this._targetVillage.Settlement)
				{
					this._isQuestTargetMission = false;
				}
			}

			// Token: 0x06000EBF RID: 3775 RVA: 0x0006998C File Offset: 0x00067B8C
			public void OnBeforeMissionOpened()
			{
				if (this._isQuestTargetMission)
				{
					Location locationWithId = Settlement.CurrentSettlement.LocationComplex.GetLocationWithId("village_center");
					if (locationWithId != null)
					{
						ItemObject @object = MBObjectManager.Instance.GetObject<ItemObject>("short_sword_t3");
						this._rogueHero.CivilianEquipment.AddEquipmentToSlotWithoutAgent(0, new EquipmentElement(@object, null, null, false));
						locationWithId.AddCharacter(this.CreateQuestLocationCharacter(this._daughterHero.CharacterObject, 0));
						locationWithId.AddCharacter(this.CreateQuestLocationCharacter(this._rogueHero.CharacterObject, 0));
					}
				}
			}

			// Token: 0x06000EC0 RID: 3776 RVA: 0x00069A14 File Offset: 0x00067C14
			private void OnMissionEnded(IMission mission)
			{
				if (this._isQuestTargetMission)
				{
					this._daughterAgent = null;
					this._rogueAgent = null;
					if (this._isDaughterPersuaded)
					{
						this.ApplyDeliverySuccessConsequences();
						base.CompleteQuestWithSuccess();
						base.AddLog(this._successQuestLogText, false);
						this.RemoveQuestCharacters();
						return;
					}
					if (this._acceptedDaughtersEscape)
					{
						this.ApplyDaughtersEscapeAcceptedFailConsequences();
						base.CompleteQuestWithFail(this._failQuestLogText);
						this.RemoveQuestCharacters();
						return;
					}
					if (this._isDaughterCaptured)
					{
						this.ApplyDeliverySuccessConsequences();
						base.CompleteQuestWithSuccess();
						base.AddLog(this._successQuestLogText, false);
						this.RemoveQuestCharacters();
					}
				}
			}

			// Token: 0x06000EC1 RID: 3777 RVA: 0x00069AAC File Offset: 0x00067CAC
			private LocationCharacter CreateQuestLocationCharacter(CharacterObject character, LocationCharacter.CharacterRelations relation)
			{
				Monster monsterWithSuffix = FaceGen.GetMonsterWithSuffix(character.Race, "_settlement");
				Tuple<string, Monster> tuple = new Tuple<string, Monster>(ActionSetCode.GenerateActionSetNameWithSuffix(monsterWithSuffix, character.IsFemale, "_villager"), monsterWithSuffix);
				return new LocationCharacter(new AgentData(new SimpleAgentOrigin(character, -1, null, default(UniqueTroopDescriptor))).Monster(tuple.Item2), new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddCompanionBehaviors), "alley_2", true, relation, tuple.Item1, false, false, null, false, true, true);
			}

			// Token: 0x06000EC2 RID: 3778 RVA: 0x00069B34 File Offset: 0x00067D34
			private void RemoveQuestCharacters()
			{
				Location locationWithId = Settlement.CurrentSettlement.LocationComplex.GetLocationWithId("village_center");
				if (locationWithId != null)
				{
					locationWithId.RemoveCharacter(this._daughterHero);
					if (locationWithId.ContainsCharacter(this._rogueHero))
					{
						locationWithId.RemoveCharacter(this._rogueHero);
					}
				}
			}

			// Token: 0x06000EC3 RID: 3779 RVA: 0x00069B80 File Offset: 0x00067D80
			private void OnSettlementEntered(MobileParty party, Settlement settlement, Hero hero)
			{
				if (party != null && party.IsMainParty && settlement.IsVillage)
				{
					if (this._villagesAndAlreadyVisitedBooleans.ContainsKey(settlement.Village) && !this._villagesAndAlreadyVisitedBooleans[settlement.Village])
					{
						if (settlement.Village != this._targetVillage)
						{
							TextObject textObject = (settlement.IsRaided ? new TextObject("{=YTaM6G1E}It seems the village has been raided a short while ago. You found nothing but smoke, fire and crying people.", null) : new TextObject("{=2P3UJ8be}You ask around the village if anyone saw {TARGET_HERO.NAME} or some suspicious characters with a young woman.\n\nVillagers say that they saw a young man and woman ride in early in the morning. They bought some supplies and trotted off towards {TARGET_VILLAGE}.", null));
							textObject.SetTextVariable("TARGET_VILLAGE", this._targetVillage.Name);
							StringHelpers.SetCharacterProperties("TARGET_HERO", this._daughterHero.CharacterObject, textObject, false);
							InformationManager.ShowInquiry(new InquiryData(this.Title.ToString(), textObject.ToString(), true, false, new TextObject("{=yS7PvrTD}OK", null).ToString(), "", null, null, "", 0f, null, null, null), false, false);
							if (!this._isTrackerLogAdded)
							{
								TextObject textObject2 = new TextObject("{=WGi3Zuv7}You asked the villagers around {CURRENT_SETTLEMENT} if they saw a young woman matching the description of {QUEST_GIVER.LINK}'s daughter, {TARGET_HERO.NAME}.\n\nThey said a young woman and a young man dropped by early in the morning to buy some supplies and then rode off towards {TARGET_VILLAGE}.", null);
								textObject2.SetTextVariable("CURRENT_SETTLEMENT", Hero.MainHero.CurrentSettlement.Name);
								textObject2.SetTextVariable("TARGET_VILLAGE", this._targetVillage.Settlement.EncyclopediaLinkWithName);
								StringHelpers.SetCharacterProperties("TARGET_HERO", this._daughterHero.CharacterObject, textObject2, false);
								StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject2, false);
								base.AddLog(textObject2, false);
								this._isTrackerLogAdded = true;
							}
						}
						else
						{
							InquiryData inquiryData;
							if (settlement.IsRaided)
							{
								TextObject textObject3 = new TextObject("{=edoXFdmg}You have found {QUEST_GIVER.NAME}'s daughter.", null);
								StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject3, false);
								TextObject textObject4 = new TextObject("{=aYMW8bWi}Talk to her", null);
								inquiryData = new InquiryData(this.Title.ToString(), textObject3.ToString(), true, false, textObject4.ToString(), null, new Action(this.TalkWithDaughterAfterRaid), null, "", 0f, null, null, null);
							}
							else
							{
								TextObject textObject5 = new TextObject("{=bbwNIIKI}You ask around the village if anyone saw {TARGET_HERO.NAME} or some suspicious characters with a young woman.\n\nVillagers say that there was a young man and woman who arrived here exhausted. The villagers allowed them to stay for a while.\nYou can check the area, and see if they are still hiding here.", null);
								StringHelpers.SetCharacterProperties("TARGET_HERO", this._daughterHero.CharacterObject, textObject5, false);
								inquiryData = new InquiryData(this.Title.ToString(), textObject5.ToString(), true, true, new TextObject("{=bb6e8DoM}Search the village", null).ToString(), new TextObject("{=3CpNUnVl}Cancel", null).ToString(), new Action(this.SearchTheVillage), null, "", 0f, null, null, null);
							}
							InformationManager.ShowInquiry(inquiryData, false, false);
						}
						this._villagesAndAlreadyVisitedBooleans[settlement.Village] = true;
					}
					if (settlement == this._targetVillage.Settlement)
					{
						if (!base.IsTracked(this._daughterHero))
						{
							base.AddTrackedObject(this._daughterHero);
						}
						if (!base.IsTracked(this._rogueHero))
						{
							base.AddTrackedObject(this._rogueHero);
						}
						this._isQuestTargetMission = true;
					}
				}
			}

			// Token: 0x06000EC4 RID: 3780 RVA: 0x00069E61 File Offset: 0x00068061
			private void SearchTheVillage()
			{
				VillageEncounter villageEncounter = PlayerEncounter.LocationEncounter as VillageEncounter;
				if (villageEncounter == null)
				{
					return;
				}
				villageEncounter.CreateAndOpenMissionController(LocationComplex.Current.GetLocationWithId("village_center"), null, null, null);
			}

			// Token: 0x06000EC5 RID: 3781 RVA: 0x00069E8C File Offset: 0x0006808C
			private void TalkWithDaughterAfterRaid()
			{
				this._villageIsRaidedTalkWithDaughter = true;
				CampaignMapConversation.OpenConversation(new ConversationCharacterData(CharacterObject.PlayerCharacter, null, false, false, false, false, false, false), new ConversationCharacterData(this._daughterHero.CharacterObject, null, false, false, false, false, false, false));
			}

			// Token: 0x06000EC6 RID: 3782 RVA: 0x00069ECD File Offset: 0x000680CD
			private void QuestAcceptedConsequences()
			{
				base.StartQuest();
				base.AddLog(this._playerStartsQuestLogText, false);
			}

			// Token: 0x06000EC7 RID: 3783 RVA: 0x00069EE3 File Offset: 0x000680E3
			private void CanHeroDie(Hero victim, KillCharacterAction.KillCharacterActionDetail detail, ref bool result)
			{
				if (victim == Hero.MainHero && Settlement.CurrentSettlement == this._targetVillage.Settlement && Mission.Current != null)
				{
					result = false;
				}
			}

			// Token: 0x06000EC8 RID: 3784 RVA: 0x00069F0C File Offset: 0x0006810C
			protected override void RegisterEvents()
			{
				CampaignEvents.OnSettlementLeftEvent.AddNonSerializedListener(this, new Action<MobileParty, Settlement>(this.OnSettlementLeft));
				CampaignEvents.SettlementEntered.AddNonSerializedListener(this, new Action<MobileParty, Settlement, Hero>(this.OnSettlementEntered));
				CampaignEvents.BeforeMissionOpenedEvent.AddNonSerializedListener(this, new Action(this.OnBeforeMissionOpened));
				CampaignEvents.OnMissionEndedEvent.AddNonSerializedListener(this, new Action<IMission>(this.OnMissionEnded));
				CampaignEvents.WarDeclared.AddNonSerializedListener(this, new Action<IFaction, IFaction, DeclareWarAction.DeclareWarDetail>(this.OnWarDeclared));
				CampaignEvents.CanHeroDieEvent.AddNonSerializedListener(this, new ReferenceAction<Hero, KillCharacterAction.KillCharacterActionDetail, bool>(this.CanHeroDie));
				CampaignEvents.ClanChangedKingdom.AddNonSerializedListener(this, new Action<Clan, Kingdom, Kingdom, ChangeKingdomAction.ChangeKingdomActionDetail, bool>(this.OnClanChangedKingdom));
				CampaignEvents.MapEventStarted.AddNonSerializedListener(this, new Action<MapEvent, PartyBase, PartyBase>(this.OnMapEventStarted));
			}

			// Token: 0x06000EC9 RID: 3785 RVA: 0x00069FD1 File Offset: 0x000681D1
			public override void OnHeroCanHaveQuestOrIssueInfoIsRequested(Hero hero, ref bool result)
			{
				if (hero == this._rogueHero || hero == this._daughterHero)
				{
					result = false;
				}
			}

			// Token: 0x06000ECA RID: 3786 RVA: 0x00069FE8 File Offset: 0x000681E8
			public override void OnHeroCanMoveToSettlementInfoIsRequested(Hero hero, ref bool result)
			{
				if (hero == this._rogueHero || hero == this._daughterHero)
				{
					result = false;
				}
			}

			// Token: 0x06000ECB RID: 3787 RVA: 0x00069FFF File Offset: 0x000681FF
			private void OnMapEventStarted(MapEvent mapEvent, PartyBase attackerParty, PartyBase defenderParty)
			{
				if (QuestHelper.CheckMinorMajorCoercion(this, mapEvent, attackerParty))
				{
					QuestHelper.ApplyGenericMinorMajorCoercionConsequences(this, mapEvent);
				}
			}

			// Token: 0x06000ECC RID: 3788 RVA: 0x0006A012 File Offset: 0x00068212
			private void OnClanChangedKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail detail, bool showNotification = true)
			{
				if (base.QuestGiver.CurrentSettlement.MapFaction.IsAtWarWith(Hero.MainHero.MapFaction))
				{
					base.CompleteQuestWithCancel(this._questCanceledWarDeclaredLog);
				}
			}

			// Token: 0x06000ECD RID: 3789 RVA: 0x0006A041 File Offset: 0x00068241
			private void OnWarDeclared(IFaction faction1, IFaction faction2, DeclareWarAction.DeclareWarDetail detail)
			{
				QuestHelper.CheckWarDeclarationAndFailOrCancelTheQuest(this, faction1, faction2, detail, this._playerDeclaredWarQuestLogText, this._questCanceledWarDeclaredLog);
			}

			// Token: 0x06000ECE RID: 3790 RVA: 0x0006A058 File Offset: 0x00068258
			protected override void OnFinalize()
			{
				if (base.IsTracked(this._targetVillage.Settlement))
				{
					base.RemoveTrackedObject(this._targetVillage.Settlement);
				}
				if (!Hero.MainHero.IsPrisoner && !this.DoesMainPartyHasEnoughScoutingSkill)
				{
					foreach (Village village in base.QuestGiver.CurrentSettlement.BoundVillages)
					{
						if (base.IsTracked(village.Settlement))
						{
							base.RemoveTrackedObject(village.Settlement);
						}
					}
				}
				if (this._rogueHero != null && this._rogueHero.IsActive && this._rogueHero.IsAlive)
				{
					KillCharacterAction.ApplyByMurder(this._rogueHero, null, false);
				}
				if (this._daughterHero != null)
				{
					DisableHeroAction.Apply(this._daughterHero);
				}
			}

			// Token: 0x06000ECF RID: 3791 RVA: 0x0006A144 File Offset: 0x00068344
			internal static void AutoGeneratedStaticCollectObjectsNotableWantsDaughterFoundIssueQuest(object o, List<object> collectedObjects)
			{
				((NotableWantsDaughterFoundIssueBehavior.NotableWantsDaughterFoundIssueQuest)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			// Token: 0x06000ED0 RID: 3792 RVA: 0x0006A152 File Offset: 0x00068352
			protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				base.AutoGeneratedInstanceCollectObjects(collectedObjects);
				collectedObjects.Add(this._daughterHero);
				collectedObjects.Add(this._rogueHero);
				collectedObjects.Add(this._targetVillage);
				collectedObjects.Add(this._villagesAndAlreadyVisitedBooleans);
			}

			// Token: 0x06000ED1 RID: 3793 RVA: 0x0006A18B File Offset: 0x0006838B
			internal static object AutoGeneratedGetMemberValue_daughterHero(object o)
			{
				return ((NotableWantsDaughterFoundIssueBehavior.NotableWantsDaughterFoundIssueQuest)o)._daughterHero;
			}

			// Token: 0x06000ED2 RID: 3794 RVA: 0x0006A198 File Offset: 0x00068398
			internal static object AutoGeneratedGetMemberValue_rogueHero(object o)
			{
				return ((NotableWantsDaughterFoundIssueBehavior.NotableWantsDaughterFoundIssueQuest)o)._rogueHero;
			}

			// Token: 0x06000ED3 RID: 3795 RVA: 0x0006A1A5 File Offset: 0x000683A5
			internal static object AutoGeneratedGetMemberValue_isQuestTargetMission(object o)
			{
				return ((NotableWantsDaughterFoundIssueBehavior.NotableWantsDaughterFoundIssueQuest)o)._isQuestTargetMission;
			}

			// Token: 0x06000ED4 RID: 3796 RVA: 0x0006A1B7 File Offset: 0x000683B7
			internal static object AutoGeneratedGetMemberValue_didPlayerBeatRouge(object o)
			{
				return ((NotableWantsDaughterFoundIssueBehavior.NotableWantsDaughterFoundIssueQuest)o)._didPlayerBeatRouge;
			}

			// Token: 0x06000ED5 RID: 3797 RVA: 0x0006A1C9 File Offset: 0x000683C9
			internal static object AutoGeneratedGetMemberValue_exitedQuestSettlementForTheFirstTime(object o)
			{
				return ((NotableWantsDaughterFoundIssueBehavior.NotableWantsDaughterFoundIssueQuest)o)._exitedQuestSettlementForTheFirstTime;
			}

			// Token: 0x06000ED6 RID: 3798 RVA: 0x0006A1DB File Offset: 0x000683DB
			internal static object AutoGeneratedGetMemberValue_isTrackerLogAdded(object o)
			{
				return ((NotableWantsDaughterFoundIssueBehavior.NotableWantsDaughterFoundIssueQuest)o)._isTrackerLogAdded;
			}

			// Token: 0x06000ED7 RID: 3799 RVA: 0x0006A1ED File Offset: 0x000683ED
			internal static object AutoGeneratedGetMemberValue_isDaughterPersuaded(object o)
			{
				return ((NotableWantsDaughterFoundIssueBehavior.NotableWantsDaughterFoundIssueQuest)o)._isDaughterPersuaded;
			}

			// Token: 0x06000ED8 RID: 3800 RVA: 0x0006A1FF File Offset: 0x000683FF
			internal static object AutoGeneratedGetMemberValue_isDaughterCaptured(object o)
			{
				return ((NotableWantsDaughterFoundIssueBehavior.NotableWantsDaughterFoundIssueQuest)o)._isDaughterCaptured;
			}

			// Token: 0x06000ED9 RID: 3801 RVA: 0x0006A211 File Offset: 0x00068411
			internal static object AutoGeneratedGetMemberValue_acceptedDaughtersEscape(object o)
			{
				return ((NotableWantsDaughterFoundIssueBehavior.NotableWantsDaughterFoundIssueQuest)o)._acceptedDaughtersEscape;
			}

			// Token: 0x06000EDA RID: 3802 RVA: 0x0006A223 File Offset: 0x00068423
			internal static object AutoGeneratedGetMemberValue_targetVillage(object o)
			{
				return ((NotableWantsDaughterFoundIssueBehavior.NotableWantsDaughterFoundIssueQuest)o)._targetVillage;
			}

			// Token: 0x06000EDB RID: 3803 RVA: 0x0006A230 File Offset: 0x00068430
			internal static object AutoGeneratedGetMemberValue_villageIsRaidedTalkWithDaughter(object o)
			{
				return ((NotableWantsDaughterFoundIssueBehavior.NotableWantsDaughterFoundIssueQuest)o)._villageIsRaidedTalkWithDaughter;
			}

			// Token: 0x06000EDC RID: 3804 RVA: 0x0006A242 File Offset: 0x00068442
			internal static object AutoGeneratedGetMemberValue_villagesAndAlreadyVisitedBooleans(object o)
			{
				return ((NotableWantsDaughterFoundIssueBehavior.NotableWantsDaughterFoundIssueQuest)o)._villagesAndAlreadyVisitedBooleans;
			}

			// Token: 0x06000EDD RID: 3805 RVA: 0x0006A24F File Offset: 0x0006844F
			internal static object AutoGeneratedGetMemberValue_questDifficultyMultiplier(object o)
			{
				return ((NotableWantsDaughterFoundIssueBehavior.NotableWantsDaughterFoundIssueQuest)o)._questDifficultyMultiplier;
			}

			// Token: 0x04000622 RID: 1570
			[SaveableField(10)]
			private readonly Hero _daughterHero;

			// Token: 0x04000623 RID: 1571
			[SaveableField(20)]
			private readonly Hero _rogueHero;

			// Token: 0x04000624 RID: 1572
			private Agent _daughterAgent;

			// Token: 0x04000625 RID: 1573
			private Agent _rogueAgent;

			// Token: 0x04000626 RID: 1574
			[SaveableField(50)]
			private bool _isQuestTargetMission;

			// Token: 0x04000627 RID: 1575
			[SaveableField(60)]
			private bool _didPlayerBeatRouge;

			// Token: 0x04000628 RID: 1576
			[SaveableField(70)]
			private bool _exitedQuestSettlementForTheFirstTime = true;

			// Token: 0x04000629 RID: 1577
			[SaveableField(80)]
			private bool _isTrackerLogAdded;

			// Token: 0x0400062A RID: 1578
			[SaveableField(90)]
			private bool _isDaughterPersuaded;

			// Token: 0x0400062B RID: 1579
			[SaveableField(91)]
			private bool _isDaughterCaptured;

			// Token: 0x0400062C RID: 1580
			[SaveableField(100)]
			private bool _acceptedDaughtersEscape;

			// Token: 0x0400062D RID: 1581
			[SaveableField(110)]
			private readonly Village _targetVillage;

			// Token: 0x0400062E RID: 1582
			[SaveableField(120)]
			private bool _villageIsRaidedTalkWithDaughter;

			// Token: 0x0400062F RID: 1583
			[SaveableField(140)]
			private Dictionary<Village, bool> _villagesAndAlreadyVisitedBooleans = new Dictionary<Village, bool>();

			// Token: 0x04000630 RID: 1584
			private Dictionary<string, CharacterObject> _rogueCharacterBasedOnCulture = new Dictionary<string, CharacterObject>();

			// Token: 0x04000631 RID: 1585
			private PersuasionTask _task;

			// Token: 0x04000632 RID: 1586
			private const PersuasionDifficulty Difficulty = 5;

			// Token: 0x04000633 RID: 1587
			private const int MaxAgeForDaughterAndRogue = 25;

			// Token: 0x04000634 RID: 1588
			[SaveableField(130)]
			private readonly float _questDifficultyMultiplier;
		}
	}
}
