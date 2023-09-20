using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Issues;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.SaveSystem;

namespace SandBox.Issues
{
	// Token: 0x02000080 RID: 128
	public class SnareTheWealthyIssueBehavior : CampaignBehaviorBase
	{
		// Token: 0x06000573 RID: 1395 RVA: 0x00026B80 File Offset: 0x00024D80
		public override void RegisterEvents()
		{
			CampaignEvents.OnCheckForIssueEvent.AddNonSerializedListener(this, new Action<Hero>(this.OnCheckForIssue));
		}

		// Token: 0x06000574 RID: 1396 RVA: 0x00026B9C File Offset: 0x00024D9C
		private void OnCheckForIssue(Hero hero)
		{
			if (this.ConditionsHold(hero))
			{
				Campaign.Current.IssueManager.AddPotentialIssueData(hero, new PotentialIssueData(new PotentialIssueData.StartIssueDelegate(this.OnStartIssue), typeof(SnareTheWealthyIssueBehavior.SnareTheWealthyIssue), 2, null));
				return;
			}
			Campaign.Current.IssueManager.AddPotentialIssueData(hero, new PotentialIssueData(typeof(SnareTheWealthyIssueBehavior.SnareTheWealthyIssue), 2));
		}

		// Token: 0x06000575 RID: 1397 RVA: 0x00026C00 File Offset: 0x00024E00
		private bool ConditionsHold(Hero issueGiver)
		{
			return issueGiver.IsGangLeader && issueGiver.CurrentSettlement != null && issueGiver.CurrentSettlement.IsTown && issueGiver.CurrentSettlement.Town.Security <= 50f && this.GetTargetMerchant(issueGiver) != null;
		}

		// Token: 0x06000576 RID: 1398 RVA: 0x00026C50 File Offset: 0x00024E50
		private Hero GetTargetMerchant(Hero issueOwner)
		{
			Hero hero = null;
			foreach (Hero hero2 in issueOwner.CurrentSettlement.Notables)
			{
				if (hero2 != issueOwner && hero2.IsMerchant && hero2.Power >= 150f && hero2.GetTraitLevel(DefaultTraits.Mercy) + hero2.GetTraitLevel(DefaultTraits.Honor) < 0 && hero2.CanHaveQuestsOrIssues() && !Campaign.Current.IssueManager.HasIssueCoolDown(typeof(SnareTheWealthyIssueBehavior.SnareTheWealthyIssue), hero2) && !Campaign.Current.IssueManager.HasIssueCoolDown(typeof(EscortMerchantCaravanIssueBehavior), hero2) && !Campaign.Current.IssueManager.HasIssueCoolDown(typeof(CaravanAmbushIssueBehavior), hero2))
				{
					hero = hero2;
					break;
				}
			}
			return hero;
		}

		// Token: 0x06000577 RID: 1399 RVA: 0x00026D44 File Offset: 0x00024F44
		private IssueBase OnStartIssue(in PotentialIssueData pid, Hero issueOwner)
		{
			Hero targetMerchant = this.GetTargetMerchant(issueOwner);
			return new SnareTheWealthyIssueBehavior.SnareTheWealthyIssue(issueOwner, targetMerchant.CharacterObject);
		}

		// Token: 0x06000578 RID: 1400 RVA: 0x00026D65 File Offset: 0x00024F65
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x040002AB RID: 683
		private const IssueBase.IssueFrequency SnareTheWealthyIssueFrequency = 2;

		// Token: 0x0200015E RID: 350
		public class SnareTheWealthyIssueTypeDefiner : SaveableTypeDefiner
		{
			// Token: 0x06001038 RID: 4152 RVA: 0x0006F129 File Offset: 0x0006D329
			public SnareTheWealthyIssueTypeDefiner()
				: base(340000)
			{
			}

			// Token: 0x06001039 RID: 4153 RVA: 0x0006F136 File Offset: 0x0006D336
			protected override void DefineClassTypes()
			{
				base.AddClassDefinition(typeof(SnareTheWealthyIssueBehavior.SnareTheWealthyIssue), 1, null);
				base.AddClassDefinition(typeof(SnareTheWealthyIssueBehavior.SnareTheWealthyIssueQuest), 2, null);
			}

			// Token: 0x0600103A RID: 4154 RVA: 0x0006F15C File Offset: 0x0006D35C
			protected override void DefineEnumTypes()
			{
				base.AddEnumDefinition(typeof(SnareTheWealthyIssueBehavior.SnareTheWealthyIssueQuest.SnareTheWealthyQuestChoice), 3, null);
			}
		}

		// Token: 0x0200015F RID: 351
		public class SnareTheWealthyIssue : IssueBase
		{
			// Token: 0x170001A4 RID: 420
			// (get) Token: 0x0600103B RID: 4155 RVA: 0x0006F170 File Offset: 0x0006D370
			private int AlternativeSolutionReward
			{
				get
				{
					return MathF.Floor(1000f + 3000f * base.IssueDifficultyMultiplier);
				}
			}

			// Token: 0x0600103C RID: 4156 RVA: 0x0006F189 File Offset: 0x0006D389
			public SnareTheWealthyIssue(Hero issueOwner, CharacterObject targetMerchant)
				: base(issueOwner, CampaignTime.DaysFromNow(30f))
			{
				this._targetMerchantCharacter = targetMerchant;
			}

			// Token: 0x170001A5 RID: 421
			// (get) Token: 0x0600103D RID: 4157 RVA: 0x0006F1A4 File Offset: 0x0006D3A4
			public override TextObject IssueBriefByIssueGiver
			{
				get
				{
					TextObject textObject = new TextObject("{=bLigh8Sd}Well, let's just say there's an idea I've been mulling over. You may be able to help. Have you met {TARGET_MERCHANT.NAME}? {?TARGET_MERCHANT.GENDER}She{?}He{\\?} is a very rich merchant. Very rich indeed. But not very honest… It's not right that someone without morals should have so much wealth, is it? I have a plan to redistribute it a bit.", null);
					StringHelpers.SetCharacterProperties("TARGET_MERCHANT", this._targetMerchantCharacter, textObject, false);
					return textObject;
				}
			}

			// Token: 0x170001A6 RID: 422
			// (get) Token: 0x0600103E RID: 4158 RVA: 0x0006F1D1 File Offset: 0x0006D3D1
			public override TextObject IssueAcceptByPlayer
			{
				get
				{
					return new TextObject("{=keKEFagm}So what's the plan?", null);
				}
			}

			// Token: 0x170001A7 RID: 423
			// (get) Token: 0x0600103F RID: 4159 RVA: 0x0006F1E0 File Offset: 0x0006D3E0
			public override TextObject IssueQuestSolutionExplanationByIssueGiver
			{
				get
				{
					TextObject textObject = new TextObject("{=SliFGAX4}{TARGET_MERCHANT.NAME} is always looking for extra swords to protect {?TARGET_MERCHANT.GENDER}her{?}his{\\?} caravans. The wicked are the ones who fear wickedness the most, you might say. What if those guards turned out to be robbers? {TARGET_MERCHANT.NAME} wouldn't trust just anyone but I think {?TARGET_MERCHANT.GENDER}she{?}he{\\?} might hire a renowned warrior like yourself. And if that warrior were to lead the caravan into an ambush… Oh I suppose it's all a bit dishonorable, but I wouldn't worry too much about your reputation. {TARGET_MERCHANT.NAME} is known to defraud {?TARGET_MERCHANT.GENDER}her{?}his{\\?} partners. If something happened to one of {?TARGET_MERCHANT.GENDER}her{?}his{\\?} caravans - well, most people won't know who to believe, and won't really care either.", null);
					StringHelpers.SetCharacterProperties("TARGET_MERCHANT", this._targetMerchantCharacter, textObject, false);
					return textObject;
				}
			}

			// Token: 0x170001A8 RID: 424
			// (get) Token: 0x06001040 RID: 4160 RVA: 0x0006F20D File Offset: 0x0006D40D
			public override TextObject IssueQuestSolutionAcceptByPlayer
			{
				get
				{
					return new TextObject("{=4upBpsnb}All right. I am in.", null);
				}
			}

			// Token: 0x170001A9 RID: 425
			// (get) Token: 0x06001041 RID: 4161 RVA: 0x0006F21A File Offset: 0x0006D41A
			public override TextObject IssueAlternativeSolutionExplanationByIssueGiver
			{
				get
				{
					TextObject textObject = new TextObject("{=ivNVRP69}I prefer if you do this yourself, but one of your trusted companions with a strong sword-arm and enough brains to set an ambush can do the job with {TROOP_COUNT} fighters. We'll split the loot, and I'll throw in a little bonus on top of that for you..", null);
					textObject.SetTextVariable("TROOP_COUNT", base.GetTotalAlternativeSolutionNeededMenCount());
					return textObject;
				}
			}

			// Token: 0x170001AA RID: 426
			// (get) Token: 0x06001042 RID: 4162 RVA: 0x0006F239 File Offset: 0x0006D439
			public override TextObject IssueAlternativeSolutionAcceptByPlayer
			{
				get
				{
					return new TextObject("{=biqYiCnr}My companion can handle it. Do not worry.", null);
				}
			}

			// Token: 0x170001AB RID: 427
			// (get) Token: 0x06001043 RID: 4163 RVA: 0x0006F246 File Offset: 0x0006D446
			public override TextObject IssueAlternativeSolutionResponseByIssueGiver
			{
				get
				{
					return new TextObject("{=UURamhdC}Thank you. This should make both of us a pretty penny.", null);
				}
			}

			// Token: 0x170001AC RID: 428
			// (get) Token: 0x06001044 RID: 4164 RVA: 0x0006F253 File Offset: 0x0006D453
			public override TextObject IssueDiscussAlternativeSolution
			{
				get
				{
					return new TextObject("{=pmuEeFV8}We are still arranging with your men how we'll spring this ambush. Do not worry. Everything will go smoothly.", null);
				}
			}

			// Token: 0x170001AD RID: 429
			// (get) Token: 0x06001045 RID: 4165 RVA: 0x0006F260 File Offset: 0x0006D460
			protected override TextObject AlternativeSolutionStartLog
			{
				get
				{
					TextObject textObject = new TextObject("{=28lLrXOe}{ISSUE_GIVER.LINK} shared their plan for robbing {TARGET_MERCHANT.LINK} with you. You agreed to send your companion along with {TROOP_COUNT} men to lead the ambush for them. They will return after {RETURN_DAYS} days.", null);
					StringHelpers.SetCharacterProperties("ISSUE_GIVER", base.IssueOwner.CharacterObject, textObject, false);
					StringHelpers.SetCharacterProperties("TARGET_MERCHANT", this._targetMerchantCharacter, textObject, false);
					textObject.SetTextVariable("TROOP_COUNT", this.AlternativeSolutionSentTroops.TotalManCount - 1);
					textObject.SetTextVariable("RETURN_DAYS", base.GetTotalAlternativeSolutionDurationInDays());
					return textObject;
				}
			}

			// Token: 0x170001AE RID: 430
			// (get) Token: 0x06001046 RID: 4166 RVA: 0x0006F2D0 File Offset: 0x0006D4D0
			public override bool IsThereAlternativeSolution
			{
				get
				{
					return true;
				}
			}

			// Token: 0x170001AF RID: 431
			// (get) Token: 0x06001047 RID: 4167 RVA: 0x0006F2D3 File Offset: 0x0006D4D3
			public override bool IsThereLordSolution
			{
				get
				{
					return false;
				}
			}

			// Token: 0x170001B0 RID: 432
			// (get) Token: 0x06001048 RID: 4168 RVA: 0x0006F2D6 File Offset: 0x0006D4D6
			public override TextObject Title
			{
				get
				{
					return new TextObject("{=IeihUvCD}Snare The Wealthy", null);
				}
			}

			// Token: 0x170001B1 RID: 433
			// (get) Token: 0x06001049 RID: 4169 RVA: 0x0006F2E4 File Offset: 0x0006D4E4
			public override TextObject Description
			{
				get
				{
					TextObject textObject = new TextObject("{=8LghFfQO}Help {ISSUE_GIVER.NAME} to rob {TARGET_MERCHANT.NAME} by acting as their guard.", null);
					StringHelpers.SetCharacterProperties("TARGET_MERCHANT", this._targetMerchantCharacter, textObject, false);
					StringHelpers.SetCharacterProperties("ISSUE_GIVER", base.IssueOwner.CharacterObject, textObject, false);
					return textObject;
				}
			}

			// Token: 0x170001B2 RID: 434
			// (get) Token: 0x0600104A RID: 4170 RVA: 0x0006F329 File Offset: 0x0006D529
			protected override bool IssueQuestCanBeDuplicated
			{
				get
				{
					return false;
				}
			}

			// Token: 0x0600104B RID: 4171 RVA: 0x0006F32C File Offset: 0x0006D52C
			protected override float GetIssueEffectAmountInternal(IssueEffect issueEffect)
			{
				if (issueEffect == DefaultIssueEffects.SettlementLoyalty)
				{
					return -0.1f;
				}
				if (issueEffect == DefaultIssueEffects.SettlementSecurity)
				{
					return -0.5f;
				}
				return 0f;
			}

			// Token: 0x170001B3 RID: 435
			// (get) Token: 0x0600104C RID: 4172 RVA: 0x0006F34F File Offset: 0x0006D54F
			public override IssueBase.AlternativeSolutionScaleFlag AlternativeSolutionScaleFlags
			{
				get
				{
					return 4 | 8;
				}
			}

			// Token: 0x170001B4 RID: 436
			// (get) Token: 0x0600104D RID: 4173 RVA: 0x0006F354 File Offset: 0x0006D554
			public override int AlternativeSolutionBaseNeededMenCount
			{
				get
				{
					return 10 + MathF.Ceiling(16f * base.IssueDifficultyMultiplier);
				}
			}

			// Token: 0x170001B5 RID: 437
			// (get) Token: 0x0600104E RID: 4174 RVA: 0x0006F36A File Offset: 0x0006D56A
			protected override int AlternativeSolutionBaseDurationInDaysInternal
			{
				get
				{
					return 2 + MathF.Ceiling(4f * base.IssueDifficultyMultiplier);
				}
			}

			// Token: 0x170001B6 RID: 438
			// (get) Token: 0x0600104F RID: 4175 RVA: 0x0006F37F File Offset: 0x0006D57F
			protected override int CompanionSkillRewardXP
			{
				get
				{
					return (int)(800f + 1000f * base.IssueDifficultyMultiplier);
				}
			}

			// Token: 0x06001050 RID: 4176 RVA: 0x0006F394 File Offset: 0x0006D594
			public override ValueTuple<SkillObject, int> GetAlternativeSolutionSkill(Hero hero)
			{
				return new ValueTuple<SkillObject, int>((hero.GetSkillValue(DefaultSkills.Roguery) >= hero.GetSkillValue(DefaultSkills.Tactics)) ? DefaultSkills.Roguery : DefaultSkills.Tactics, 120);
			}

			// Token: 0x06001051 RID: 4177 RVA: 0x0006F3C1 File Offset: 0x0006D5C1
			public override bool AlternativeSolutionCondition(out TextObject explanation)
			{
				explanation = TextObject.Empty;
				return QuestHelper.CheckRosterForAlternativeSolution(MobileParty.MainParty.MemberRoster, base.GetTotalAlternativeSolutionNeededMenCount(), ref explanation, 2, false);
			}

			// Token: 0x06001052 RID: 4178 RVA: 0x0006F3E2 File Offset: 0x0006D5E2
			public override bool IsTroopTypeNeededByAlternativeSolution(CharacterObject character)
			{
				return character.Tier >= 2;
			}

			// Token: 0x06001053 RID: 4179 RVA: 0x0006F3F0 File Offset: 0x0006D5F0
			public override bool DoTroopsSatisfyAlternativeSolution(TroopRoster troopRoster, out TextObject explanation)
			{
				explanation = TextObject.Empty;
				return QuestHelper.CheckRosterForAlternativeSolution(troopRoster, base.GetTotalAlternativeSolutionNeededMenCount(), ref explanation, 2, false);
			}

			// Token: 0x06001054 RID: 4180 RVA: 0x0006F408 File Offset: 0x0006D608
			protected override void AlternativeSolutionEndWithSuccessConsequence()
			{
				TraitLevelingHelper.OnIssueSolvedThroughAlternativeSolution(Hero.MainHero, new Tuple<TraitObject, int>[]
				{
					new Tuple<TraitObject, int>(DefaultTraits.Honor, -50)
				});
				TraitLevelingHelper.OnIssueSolvedThroughAlternativeSolution(Hero.MainHero, new Tuple<TraitObject, int>[]
				{
					new Tuple<TraitObject, int>(DefaultTraits.Calculating, 50)
				});
				ChangeRelationAction.ApplyPlayerRelation(base.IssueOwner, 5, true, true);
				ChangeRelationAction.ApplyPlayerRelation(this._targetMerchantCharacter.HeroObject, -10, true, true);
				GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, this.AlternativeSolutionReward, false);
			}

			// Token: 0x06001055 RID: 4181 RVA: 0x0006F488 File Offset: 0x0006D688
			protected override void AlternativeSolutionEndWithFailureConsequence()
			{
				TraitLevelingHelper.OnIssueSolvedThroughAlternativeSolution(Hero.MainHero, new Tuple<TraitObject, int>[]
				{
					new Tuple<TraitObject, int>(DefaultTraits.Honor, -100)
				});
				TraitLevelingHelper.OnIssueSolvedThroughAlternativeSolution(Hero.MainHero, new Tuple<TraitObject, int>[]
				{
					new Tuple<TraitObject, int>(DefaultTraits.Calculating, 100)
				});
				ChangeRelationAction.ApplyPlayerRelation(base.IssueOwner, -10, true, true);
				ChangeRelationAction.ApplyPlayerRelation(this._targetMerchantCharacter.HeroObject, -10, true, true);
			}

			// Token: 0x06001056 RID: 4182 RVA: 0x0006F4F6 File Offset: 0x0006D6F6
			protected override void OnGameLoad()
			{
			}

			// Token: 0x06001057 RID: 4183 RVA: 0x0006F4F8 File Offset: 0x0006D6F8
			protected override QuestBase GenerateIssueQuest(string questId)
			{
				return new SnareTheWealthyIssueBehavior.SnareTheWealthyIssueQuest(questId, base.IssueOwner, this._targetMerchantCharacter, base.IssueDifficultyMultiplier, CampaignTime.DaysFromNow(10f));
			}

			// Token: 0x06001058 RID: 4184 RVA: 0x0006F51C File Offset: 0x0006D71C
			protected override void OnIssueFinalized()
			{
				if (base.IsSolvingWithQuest)
				{
					Campaign.Current.IssueManager.AddIssueCoolDownData(base.GetType(), new HeroRelatedIssueCoolDownData(this._targetMerchantCharacter.HeroObject, CampaignTime.DaysFromNow((float)Campaign.Current.Models.IssueModel.IssueOwnerCoolDownInDays)));
					Campaign.Current.IssueManager.AddIssueCoolDownData(typeof(EscortMerchantCaravanIssueBehavior.EscortMerchantCaravanIssueQuest), new HeroRelatedIssueCoolDownData(this._targetMerchantCharacter.HeroObject, CampaignTime.DaysFromNow((float)Campaign.Current.Models.IssueModel.IssueOwnerCoolDownInDays)));
					Campaign.Current.IssueManager.AddIssueCoolDownData(typeof(CaravanAmbushIssueBehavior.CaravanAmbushIssueQuest), new HeroRelatedIssueCoolDownData(this._targetMerchantCharacter.HeroObject, CampaignTime.DaysFromNow((float)Campaign.Current.Models.IssueModel.IssueOwnerCoolDownInDays)));
				}
			}

			// Token: 0x06001059 RID: 4185 RVA: 0x0006F5F9 File Offset: 0x0006D7F9
			public override IssueBase.IssueFrequency GetFrequency()
			{
				return 2;
			}

			// Token: 0x0600105A RID: 4186 RVA: 0x0006F5FC File Offset: 0x0006D7FC
			protected override bool CanPlayerTakeQuestConditions(Hero issueGiver, out IssueBase.PreconditionFlags flag, out Hero relationHero, out SkillObject skill)
			{
				flag = 0;
				relationHero = null;
				skill = null;
				if (MobileParty.MainParty.MemberRoster.TotalHealthyCount < 20)
				{
					flag = (int)(flag | 256U);
				}
				if (issueGiver.GetRelationWithPlayer() < -10f)
				{
					flag = (int)(flag | 1U);
					relationHero = issueGiver;
				}
				if (issueGiver.CurrentSettlement.OwnerClan == Clan.PlayerClan)
				{
					flag = (int)(flag | 32768U);
				}
				return flag == 0U;
			}

			// Token: 0x0600105B RID: 4187 RVA: 0x0006F667 File Offset: 0x0006D867
			public override bool IssueStayAliveConditions()
			{
				return base.IssueOwner.IsAlive && base.IssueOwner.CurrentSettlement.Town.Security <= 80f && this._targetMerchantCharacter.HeroObject.IsAlive;
			}

			// Token: 0x0600105C RID: 4188 RVA: 0x0006F6A4 File Offset: 0x0006D8A4
			protected override void CompleteIssueWithTimedOutConsequences()
			{
			}

			// Token: 0x0600105D RID: 4189 RVA: 0x0006F6A6 File Offset: 0x0006D8A6
			public override void OnHeroCanHaveQuestOrIssueInfoIsRequested(Hero hero, ref bool result)
			{
				if (hero == this._targetMerchantCharacter.HeroObject)
				{
					result = false;
				}
			}

			// Token: 0x0600105E RID: 4190 RVA: 0x0006F6B9 File Offset: 0x0006D8B9
			internal static void AutoGeneratedStaticCollectObjectsSnareTheWealthyIssue(object o, List<object> collectedObjects)
			{
				((SnareTheWealthyIssueBehavior.SnareTheWealthyIssue)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			// Token: 0x0600105F RID: 4191 RVA: 0x0006F6C7 File Offset: 0x0006D8C7
			protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				base.AutoGeneratedInstanceCollectObjects(collectedObjects);
				collectedObjects.Add(this._targetMerchantCharacter);
			}

			// Token: 0x06001060 RID: 4192 RVA: 0x0006F6DC File Offset: 0x0006D8DC
			internal static object AutoGeneratedGetMemberValue_targetMerchantCharacter(object o)
			{
				return ((SnareTheWealthyIssueBehavior.SnareTheWealthyIssue)o)._targetMerchantCharacter;
			}

			// Token: 0x04000688 RID: 1672
			private const int IssueDuration = 30;

			// Token: 0x04000689 RID: 1673
			private const int IssueQuestDuration = 10;

			// Token: 0x0400068A RID: 1674
			private const int MinimumRequiredMenCount = 20;

			// Token: 0x0400068B RID: 1675
			private const int MinimumRequiredRelationWithIssueGiver = -10;

			// Token: 0x0400068C RID: 1676
			private const int AlternativeSolutionMinimumTroopTier = 2;

			// Token: 0x0400068D RID: 1677
			private const int CompanionRoguerySkillValueThreshold = 120;

			// Token: 0x0400068E RID: 1678
			[SaveableField(1)]
			private readonly CharacterObject _targetMerchantCharacter;
		}

		// Token: 0x02000160 RID: 352
		public class SnareTheWealthyIssueQuest : QuestBase
		{
			// Token: 0x170001B7 RID: 439
			// (get) Token: 0x06001061 RID: 4193 RVA: 0x0006F6E9 File Offset: 0x0006D8E9
			private int CaravanPartyTroopCount
			{
				get
				{
					return 20 + MathF.Ceiling(40f * this._questDifficulty);
				}
			}

			// Token: 0x170001B8 RID: 440
			// (get) Token: 0x06001062 RID: 4194 RVA: 0x0006F6FF File Offset: 0x0006D8FF
			private int GangPartyTroopCount
			{
				get
				{
					return 10 + MathF.Ceiling(25f * this._questDifficulty);
				}
			}

			// Token: 0x170001B9 RID: 441
			// (get) Token: 0x06001063 RID: 4195 RVA: 0x0006F715 File Offset: 0x0006D915
			private int Reward1
			{
				get
				{
					return MathF.Floor(1000f + 3000f * this._questDifficulty);
				}
			}

			// Token: 0x170001BA RID: 442
			// (get) Token: 0x06001064 RID: 4196 RVA: 0x0006F72E File Offset: 0x0006D92E
			private int Reward2
			{
				get
				{
					return MathF.Floor((float)this.Reward1 * 0.4f);
				}
			}

			// Token: 0x06001065 RID: 4197 RVA: 0x0006F742 File Offset: 0x0006D942
			public SnareTheWealthyIssueQuest(string questId, Hero questGiver, CharacterObject targetMerchantCharacter, float questDifficulty, CampaignTime duration)
				: base(questId, questGiver, duration, 0)
			{
				this._targetMerchantCharacter = targetMerchantCharacter;
				this._targetSettlement = this.GetTargetSettlement();
				this._questDifficulty = questDifficulty;
				this.SetDialogs();
				base.InitializeQuestOnCreation();
			}

			// Token: 0x170001BB RID: 443
			// (get) Token: 0x06001066 RID: 4198 RVA: 0x0006F77D File Offset: 0x0006D97D
			public override TextObject Title
			{
				get
				{
					return new TextObject("{=IeihUvCD}Snare The Wealthy", null);
				}
			}

			// Token: 0x170001BC RID: 444
			// (get) Token: 0x06001067 RID: 4199 RVA: 0x0006F78A File Offset: 0x0006D98A
			public override bool IsRemainingTimeHidden
			{
				get
				{
					return false;
				}
			}

			// Token: 0x170001BD RID: 445
			// (get) Token: 0x06001068 RID: 4200 RVA: 0x0006F790 File Offset: 0x0006D990
			private TextObject _questStartedLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=Ba9nsfHc}{QUEST_GIVER.LINK} shared their plan for robbing {TARGET_MERCHANT.LINK} with you. You agreed to talk with {TARGET_MERCHANT.LINK} to convince {?TARGET_MERCHANT.GENDER}her{?}him{\\?} to guard {?TARGET_MERCHANT.GENDER}her{?}his{\\?} caravan and lead the caravan to ambush around {TARGET_SETTLEMENT}.", null);
					StringHelpers.SetCharacterProperties("TARGET_MERCHANT", this._targetMerchantCharacter, textObject, false);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					textObject.SetTextVariable("TARGET_SETTLEMENT", this._targetSettlement.EncyclopediaLinkWithName);
					return textObject;
				}
			}

			// Token: 0x170001BE RID: 446
			// (get) Token: 0x06001069 RID: 4201 RVA: 0x0006F7EC File Offset: 0x0006D9EC
			private TextObject _success1LogText
			{
				get
				{
					TextObject textObject = new TextObject("{=bblwaDi1}You have successfully robbed {TARGET_MERCHANT.LINK}'s caravan with {QUEST_GIVER.LINK}.", null);
					StringHelpers.SetCharacterProperties("TARGET_MERCHANT", this._targetMerchantCharacter, textObject, false);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					return textObject;
				}
			}

			// Token: 0x170001BF RID: 447
			// (get) Token: 0x0600106A RID: 4202 RVA: 0x0006F834 File Offset: 0x0006DA34
			private TextObject _sidedWithGangLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=lZjj3MZg}When {QUEST_GIVER.LINK} arrived, you kept your side of the bargain and attacked the caravan", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					return textObject;
				}
			}

			// Token: 0x170001C0 RID: 448
			// (get) Token: 0x0600106B RID: 4203 RVA: 0x0006F868 File Offset: 0x0006DA68
			private TextObject _timedOutWithoutTalkingToMerchantText
			{
				get
				{
					TextObject textObject = new TextObject("{=OMKgidoP}You have failed to convince the merchant to guard {?TARGET_MERCHANT.GENDER}her{?}his{\\?} caravan in time. {QUEST_GIVER.LINK} must be furious.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					StringHelpers.SetCharacterProperties("TARGET_MERCHANT", this._targetMerchantCharacter, textObject, false);
					return textObject;
				}
			}

			// Token: 0x170001C1 RID: 449
			// (get) Token: 0x0600106C RID: 4204 RVA: 0x0006F8AD File Offset: 0x0006DAAD
			private TextObject _fail1LogText
			{
				get
				{
					return new TextObject("{=DRpcqEMI}The caravan leader said your decisions were wasting their time and decided to go on his way. You have failed to uphold your part in the plan.", null);
				}
			}

			// Token: 0x170001C2 RID: 450
			// (get) Token: 0x0600106D RID: 4205 RVA: 0x0006F8BA File Offset: 0x0006DABA
			private TextObject _fail2LogText
			{
				get
				{
					return new TextObject("{=EFjas6hI}At the last moment, you decided to side with the caravan guard and defend them.", null);
				}
			}

			// Token: 0x170001C3 RID: 451
			// (get) Token: 0x0600106E RID: 4206 RVA: 0x0006F8C8 File Offset: 0x0006DAC8
			private TextObject _fail2OutcomeLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=JgrG0uoO}Having the {TARGET_MERCHANT.LINK} by your side, you were successful in protecting the caravan.", null);
					StringHelpers.SetCharacterProperties("TARGET_MERCHANT", this._targetMerchantCharacter, textObject, false);
					return textObject;
				}
			}

			// Token: 0x170001C4 RID: 452
			// (get) Token: 0x0600106F RID: 4207 RVA: 0x0006F8F5 File Offset: 0x0006DAF5
			private TextObject _fail3LogText
			{
				get
				{
					return new TextObject("{=0NxiTi8b}You didn't feel like splitting the loot, so you betrayed both the merchant and the gang leader.", null);
				}
			}

			// Token: 0x170001C5 RID: 453
			// (get) Token: 0x06001070 RID: 4208 RVA: 0x0006F902 File Offset: 0x0006DB02
			private TextObject _fail3OutcomeLogText
			{
				get
				{
					return new TextObject("{=KbMew14D}Although the gang leader and the caravaneer joined their forces, you have successfully defeated them and kept the loot for yourself.", null);
				}
			}

			// Token: 0x170001C6 RID: 454
			// (get) Token: 0x06001071 RID: 4209 RVA: 0x0006F910 File Offset: 0x0006DB10
			private TextObject _fail4LogText
			{
				get
				{
					TextObject textObject = new TextObject("{=22nahm29}You have lost the battle against the merchant's caravan and failed to help {QUEST_GIVER.LINK}.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					return textObject;
				}
			}

			// Token: 0x170001C7 RID: 455
			// (get) Token: 0x06001072 RID: 4210 RVA: 0x0006F944 File Offset: 0x0006DB44
			private TextObject _fail5LogText
			{
				get
				{
					TextObject textObject = new TextObject("{=QEgzLRnC}You have lost the battle against {QUEST_GIVER.LINK} and failed to help the merchant as you promised.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					return textObject;
				}
			}

			// Token: 0x170001C8 RID: 456
			// (get) Token: 0x06001073 RID: 4211 RVA: 0x0006F978 File Offset: 0x0006DB78
			private TextObject _fail6LogText
			{
				get
				{
					TextObject textObject = new TextObject("{=pGu2mcar}You have lost the battle against the combined forces of the {QUEST_GIVER.LINK} and the caravan.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					return textObject;
				}
			}

			// Token: 0x170001C9 RID: 457
			// (get) Token: 0x06001074 RID: 4212 RVA: 0x0006F9AA File Offset: 0x0006DBAA
			private TextObject _playerCapturedQuestSettlementLogText
			{
				get
				{
					return new TextObject("{=gPFfHluf}Your clan is now owner of the settlement. As the lord of the settlement you cannot be part of the criminal activities anymore. Your agreement with the questgiver has canceled.", null);
				}
			}

			// Token: 0x170001CA RID: 458
			// (get) Token: 0x06001075 RID: 4213 RVA: 0x0006F9B8 File Offset: 0x0006DBB8
			private TextObject _questSettlementWasCapturedLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=uVigJ3LP}{QUEST_GIVER.LINK} has lost the control of {SETTLEMENT} and the deal is now invalid.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					textObject.SetTextVariable("SETTLEMENT", base.QuestGiver.CurrentSettlement.EncyclopediaLinkWithName);
					return textObject;
				}
			}

			// Token: 0x170001CB RID: 459
			// (get) Token: 0x06001076 RID: 4214 RVA: 0x0006FA08 File Offset: 0x0006DC08
			private TextObject _warDeclaredBetweenPlayerAndQuestGiverLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=ojpW4WRD}Your clan is now at war with the {QUEST_GIVER.LINK}'s lord. Your agreement with {QUEST_GIVER.LINK} was canceled.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					return textObject;
				}
			}

			// Token: 0x170001CC RID: 460
			// (get) Token: 0x06001077 RID: 4215 RVA: 0x0006FA3C File Offset: 0x0006DC3C
			private TextObject _targetSettlementRaidedLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=QkbkesNJ}{QUEST_GIVER.LINK} called off the ambush after {TARGET_SETTLEMENT} was raided.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					textObject.SetTextVariable("TARGET_SETTLEMENT", this._targetSettlement.EncyclopediaLinkWithName);
					return textObject;
				}
			}

			// Token: 0x170001CD RID: 461
			// (get) Token: 0x06001078 RID: 4216 RVA: 0x0006FA88 File Offset: 0x0006DC88
			private TextObject _talkedToMerchantLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=N1ZiaLRL}You talked to {TARGET_MERCHANT.LINK} as {QUEST_GIVER.LINK} asked. The caravan is waiting for you outside the gates to be escorted to {TARGET_SETTLEMENT}.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					StringHelpers.SetCharacterProperties("TARGET_MERCHANT", this._targetMerchantCharacter, textObject, false);
					textObject.SetTextVariable("TARGET_SETTLEMENT", this._targetSettlement.EncyclopediaLinkWithName);
					return textObject;
				}
			}

			// Token: 0x06001079 RID: 4217 RVA: 0x0006FAE4 File Offset: 0x0006DCE4
			protected override void InitializeQuestOnGameLoad()
			{
				this.SetDialogs();
				Campaign.Current.ConversationManager.AddDialogFlow(this.GetEncounterDialogue(), this);
				Campaign.Current.ConversationManager.AddDialogFlow(this.GetDialogueWithMerchant(), this);
				Campaign.Current.ConversationManager.AddDialogFlow(this.GetDialogueWithCaravan(), this);
				Campaign.Current.ConversationManager.AddDialogFlow(this.GetDialogueWithGangWithoutCaravan(), this);
			}

			// Token: 0x0600107A RID: 4218 RVA: 0x0006FB50 File Offset: 0x0006DD50
			private Settlement GetTargetSettlement()
			{
				MapDistanceModel model = Campaign.Current.Models.MapDistanceModel;
				return Extensions.GetRandomElement<Village>((from t in Settlement.All
					where t != this.QuestGiver.CurrentSettlement && t.IsTown
					orderby model.GetDistance(t, this.QuestGiver.CurrentSettlement)
					select t).ElementAt(0).BoundVillages).Settlement;
			}

			// Token: 0x0600107B RID: 4219 RVA: 0x0006FBBC File Offset: 0x0006DDBC
			protected override void SetDialogs()
			{
				TextObject discussIntroDialogue = new TextObject("{=lOFR5sq6}Have you talked with {TARGET_MERCHANT.NAME}? It would be a damned waste if we waited too long and word of our plans leaked out.", null);
				TextObject textObject = new TextObject("{=cc4EEDMg}Splendid. Go have a word with {TARGET_MERCHANT.LINK}. If you can convince {?TARGET_MERCHANT.GENDER}her{?}him{\\?} to guide the caravan, we will wait in ambush along their route.", null);
				StringHelpers.SetCharacterProperties("TARGET_MERCHANT", this._targetMerchantCharacter, textObject, false);
				this.OfferDialogFlow = DialogFlow.CreateDialogFlow("issue_classic_quest_start", 100).NpcLine(textObject, null, null).Condition(() => Hero.OneToOneConversationHero == this.QuestGiver)
					.Consequence(new ConversationSentence.OnConsequenceDelegate(this.OnQuestAccepted))
					.CloseDialog();
				this.DiscussDialogFlow = DialogFlow.CreateDialogFlow("quest_discuss", 100).NpcLine(discussIntroDialogue, null, null).Condition(delegate
				{
					StringHelpers.SetCharacterProperties("TARGET_MERCHANT", this._targetMerchantCharacter, discussIntroDialogue, false);
					return Hero.OneToOneConversationHero == this.QuestGiver;
				})
					.BeginPlayerOptions()
					.PlayerOption("{=YuabHAbV}I'll take care of it shortly..", null)
					.NpcLine("{=CDXUehf0}Good, good.", null, null)
					.CloseDialog()
					.PlayerOption("{=2haJj9mp}I have but I need to deal with some other problems before leading the caravan.", null)
					.NpcLine("{=bSDIHQzO}Please do so. Hate to have word leak out.", null, null)
					.CloseDialog()
					.EndPlayerOptions()
					.CloseDialog();
			}

			// Token: 0x0600107C RID: 4220 RVA: 0x0006FCC0 File Offset: 0x0006DEC0
			private DialogFlow GetDialogueWithMerchant()
			{
				TextObject textObject = new TextObject("{=OJtUNAbN}Very well. You'll find the caravan getting ready outside the gates. You will get your payment after the job. Good luck, friend.", null);
				return DialogFlow.CreateDialogFlow("hero_main_options", 125).BeginPlayerOptions().PlayerOption(new TextObject("{=K1ICRis9}I have heard you are looking for extra swords to protect your caravan. I am here to offer my services.", null), null)
					.Condition(() => Hero.OneToOneConversationHero == this._targetMerchantCharacter.HeroObject && this._caravanParty == null)
					.NpcLine("{=ltbu3S63}Yes, you have heard correctly. I am looking for a capable leader with a good number of followers. You only need to escort the caravan until they reach {TARGET_SETTLEMENT}. A simple job, but the cargo is very important. I'm willing to pay {MERCHANT_REWARD} denars. And of course, if you betrayed me...", null, null)
					.Condition(delegate
					{
						MBTextManager.SetTextVariable("TARGET_SETTLEMENT", this._targetSettlement.EncyclopediaLinkWithName, false);
						MBTextManager.SetTextVariable("MERCHANT_REWARD", this.Reward2);
						return true;
					})
					.Consequence(new ConversationSentence.OnConsequenceDelegate(this.SpawnQuestParties))
					.BeginPlayerOptions()
					.PlayerOption("{=AGnd7nDb}Worry not. The outlaws in these parts know my name well, and fear it.", null)
					.NpcLine(textObject, null, null)
					.CloseDialog()
					.PlayerOption("{=RCsbpizl}If you have the denars we'll do the job,.", null)
					.NpcLine(textObject, null, null)
					.CloseDialog()
					.PlayerOption("{=TfDomerj}I think my men and I are more than enough to protect the caravan, good {?TARGET_MERCHANT.GENDER}madam{?}sir{\\?}.", null)
					.Condition(delegate
					{
						StringHelpers.SetCharacterProperties("TARGET_MERCHANT", this._targetMerchantCharacter, null, false);
						return true;
					})
					.NpcLine(textObject, null, null)
					.CloseDialog()
					.EndPlayerOptions()
					.EndPlayerOptions()
					.CloseDialog();
			}

			// Token: 0x0600107D RID: 4221 RVA: 0x0006FDA8 File Offset: 0x0006DFA8
			private DialogFlow GetDialogueWithCaravan()
			{
				return DialogFlow.CreateDialogFlow("start", 125).NpcLine("{=Xs7Qweuw}Lead the way, {PLAYER.NAME}.", null, null).Condition(() => MobileParty.ConversationParty == this._caravanParty && this._caravanParty != null && !this._canEncounterConversationStart)
					.Consequence(delegate
					{
						PlayerEncounter.LeaveEncounter = true;
					})
					.CloseDialog();
			}

			// Token: 0x0600107E RID: 4222 RVA: 0x0006FE08 File Offset: 0x0006E008
			private DialogFlow GetDialogueWithGangWithoutCaravan()
			{
				return DialogFlow.CreateDialogFlow("start", 125).NpcLine("{=F44s8kPB}Where is the caravan? My men can't wait here for too long.", null, null).Condition(() => MobileParty.ConversationParty == this._gangParty && this._gangParty != null && !this._canEncounterConversationStart)
					.BeginPlayerOptions()
					.PlayerOption("{=Yqv1jk7D}Don't worry, they are coming towards our trap.", null)
					.NpcLine("{=fHc6fwrb}Good, let's finish this.", null, null)
					.CloseDialog()
					.EndPlayerOptions()
					.CloseDialog();
			}

			// Token: 0x0600107F RID: 4223 RVA: 0x0006FE6C File Offset: 0x0006E06C
			private DialogFlow GetEncounterDialogue()
			{
				return DialogFlow.CreateDialogFlow("start", 125).NpcLine("{=vVH7wT07}Who are these men? Be on your guard {PLAYER.NAME}, I smell trouble!", null, null).Condition(() => MobileParty.ConversationParty == this._caravanParty && this._caravanParty != null && this._canEncounterConversationStart)
					.Consequence(delegate
					{
						StringHelpers.SetCharacterProperties("TARGET_MERCHANT", this._targetMerchantCharacter, null, false);
						AgentBuildData agentBuildData = new AgentBuildData(ConversationHelper.GetConversationCharacterPartyLeader(this._gangParty.Party));
						agentBuildData.TroopOrigin(new SimpleAgentOrigin(agentBuildData.AgentCharacter, -1, null, default(UniqueTroopDescriptor)));
						Vec3 vec = Agent.Main.LookDirection * 10f;
						vec.RotateAboutZ(1.3962634f);
						AgentBuildData agentBuildData2 = agentBuildData;
						Vec3 vec2 = Agent.Main.Position + vec;
						agentBuildData2.InitialPosition(ref vec2);
						AgentBuildData agentBuildData3 = agentBuildData;
						vec2 = Agent.Main.LookDirection;
						Vec2 vec3 = vec2.AsVec2;
						vec3 = -vec3.Normalized();
						agentBuildData3.InitialDirection(ref vec3);
						Agent agent = Mission.Current.SpawnAgent(agentBuildData, false);
						Campaign.Current.ConversationManager.AddConversationAgents(new List<IAgent> { agent }, true);
					})
					.NpcLine("{=LJ2AoQyS}Well, well. What do we have here? Must be one of our lucky days, huh? Release all the valuables you carry and nobody gets hurt.", new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsGangPartyLeader), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsCaravanMaster))
					.NpcLine("{=SdgDF4OZ}Hah! You're making a big mistake. See that group of men over there, led by the warrior {PLAYER.NAME}? They're with us, and they'll cut you open.", new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsCaravanMaster), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsGangPartyLeader))
					.NpcLine("{=LaHWB3r0}Oh… I'm afraid there's been a misunderstanding. {PLAYER.NAME} is with us, you see. Did {TARGET_MERCHANT.LINK} stuff you with lies and then send you out to your doom? Oh, shameful, shameful. {?TARGET_MERCHANT.GENDER}She{?}He{\\?} does that fairly often, unfortunately.", new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsGangPartyLeader), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsCaravanMaster))
					.NpcLine("{=EGC4BA4h}{PLAYER.NAME}! Is this true? Look, you're a smart {?PLAYER.GENDER}woman{?}man{\\?}. You know that {TARGET_MERCHANT.LINK} can pay more than these scum. Take the money and keep your reputation.", new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsCaravanMaster), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainHero))
					.NpcLine("{=zUKqWeUa}Come on, {PLAYER.NAME}. All this back-and-forth  is making me anxious. Let's finish this.", new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsGangPartyLeader), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainHero))
					.BeginPlayerOptions()
					.PlayerOption("{=UEY5aQ2l}I'm here to rob {TARGET_MERCHANT.NAME}, not be {?TARGET_MERCHANT.GENDER}her{?}his{\\?} lackey. Now, cough up the goods or fight.", new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsGangPartyLeader))
					.NpcLine("{=tHUHfe6C}You're with them? This is the basest treachery I have ever witnessed!", new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsCaravanMaster), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainHero))
					.Consequence(delegate
					{
						base.AddLog(this._sidedWithGangLogText, false);
					})
					.NpcLine("{=IKeZLbIK}No offense, captain, but if that's the case you need to get out more. Anyway, shall we go to it?", new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsGangPartyLeader), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainHero))
					.Consequence(delegate
					{
						this.StartBattle(SnareTheWealthyIssueBehavior.SnareTheWealthyIssueQuest.SnareTheWealthyQuestChoice.SidedWithGang);
					})
					.CloseDialog()
					.PlayerOption("{=W7TD4yTc}You know, {TARGET_MERCHANT.NAME}'s man makes a good point. I'm guarding this caravan.", new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsGangPartyLeader))
					.NpcLine("{=VXp0R7da}Heaven protect you! I knew you'd never be tempted by such a perfidious offer.", new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsCaravanMaster), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainHero))
					.Consequence(delegate
					{
						base.AddLog(this._fail2LogText, false);
					})
					.NpcLine("{=XJOqws2b}Hmf. A funny sense of honor you have… Anyway, I'm not going home empty handed, so let's do this.", new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsGangPartyLeader), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainHero))
					.Consequence(delegate
					{
						this.StartBattle(SnareTheWealthyIssueBehavior.SnareTheWealthyIssueQuest.SnareTheWealthyQuestChoice.SidedWithCaravan);
					})
					.CloseDialog()
					.PlayerOption("{=ILrYPvTV}You know, I think I'd prefer to take all the loot for myself.", new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsGangPartyLeader))
					.NpcLine("{=cpTMttNb}Is that so? Hey, caravan captain, whatever your name is… As long as we're all switching sides here, how about I join with you to defeat this miscreant who just betrayed both of us? Whichever of us comes out of this with the most men standing keeps your goods.", new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsGangPartyLeader), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainHero))
					.Consequence(delegate
					{
						base.AddLog(this._fail3LogText, false);
					})
					.NpcLine("{=15UCTrNA}I have no choice, do I? Well, better an honest robber than a traitor! Let's take {?PLAYER.GENDER}her{?}him{\\?} down.", new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsCaravanMaster), new ConversationSentence.OnMultipleConversationConsequenceDelegate(this.IsMainHero))
					.Consequence(delegate
					{
						this.StartBattle(SnareTheWealthyIssueBehavior.SnareTheWealthyIssueQuest.SnareTheWealthyQuestChoice.BetrayedBoth);
					})
					.CloseDialog()
					.EndPlayerOptions()
					.CloseDialog();
			}

			// Token: 0x06001080 RID: 4224 RVA: 0x000700F0 File Offset: 0x0006E2F0
			private void OnQuestAccepted()
			{
				base.StartQuest();
				base.AddLog(this._questStartedLogText, false);
				base.AddTrackedObject(this._targetMerchantCharacter.HeroObject);
				Campaign.Current.ConversationManager.AddDialogFlow(this.GetEncounterDialogue(), this);
				Campaign.Current.ConversationManager.AddDialogFlow(this.GetDialogueWithMerchant(), this);
				Campaign.Current.ConversationManager.AddDialogFlow(this.GetDialogueWithCaravan(), this);
				Campaign.Current.ConversationManager.AddDialogFlow(this.GetDialogueWithGangWithoutCaravan(), this);
			}

			// Token: 0x06001081 RID: 4225 RVA: 0x0007017C File Offset: 0x0006E37C
			public void GetMountAndHarnessVisualIdsForQuestCaravan(CultureObject culture, out string mountStringId, out string harnessStringId)
			{
				if (culture.StringId == "khuzait" || culture.StringId == "aserai")
				{
					mountStringId = "camel";
					harnessStringId = "camel_saddle_b";
					return;
				}
				mountStringId = "mule";
				harnessStringId = "mule_load_c";
			}

			// Token: 0x06001082 RID: 4226 RVA: 0x000701CC File Offset: 0x0006E3CC
			private void SpawnQuestParties()
			{
				TextObject textObject = new TextObject("{=Bh4sZo9o}Caravan of {TARGET_MERCHANT}", null);
				textObject.SetTextVariable("TARGET_MERCHANT", this._targetMerchantCharacter.HeroObject.Name);
				string text;
				string text2;
				this.GetMountAndHarnessVisualIdsForQuestCaravan(this._targetMerchantCharacter.Culture, out text, out text2);
				this._caravanParty = CustomPartyComponent.CreateQuestParty(this._targetMerchantCharacter.HeroObject.CurrentSettlement.GatePosition, 0.1f, this._targetMerchantCharacter.HeroObject.CurrentSettlement, textObject, this._targetMerchantCharacter.HeroObject.Clan, this._targetMerchantCharacter.HeroObject.Culture.CaravanPartyTemplate, this._targetMerchantCharacter.HeroObject, this.CaravanPartyTroopCount, text, text2, MobileParty.MainParty.Speed, false);
				this._caravanParty.MemberRoster.AddToCounts(this._targetMerchantCharacter.Culture.CaravanMaster, 1, false, 0, 0, true, -1);
				this._caravanParty.ItemRoster.AddToCounts(Game.Current.ObjectManager.GetObject<ItemObject>("grain"), 40);
				this._caravanParty.IgnoreByOtherPartiesTill(base.QuestDueTime);
				SetPartyAiAction.GetActionForEscortingParty(this._caravanParty, MobileParty.MainParty);
				this._caravanParty.Ai.SetDoNotMakeNewDecisions(true);
				this._caravanParty.SetPartyUsedByQuest(true);
				base.AddTrackedObject(this._caravanParty);
				MobilePartyHelper.TryMatchPartySpeedWithItemWeight(this._caravanParty, MobileParty.MainParty.Speed * 1.5f, null);
				Settlement closestHideout = SettlementHelper.FindNearestHideout((Settlement x) => x.IsActive, null);
				Clan clan = Clan.BanditFactions.FirstOrDefault((Clan t) => t.Culture == closestHideout.Culture);
				Vec2 gatePosition = this._targetSettlement.GatePosition;
				PartyTemplateObject partyTemplateObject = Campaign.Current.ObjectManager.GetObject<PartyTemplateObject>("kingdom_hero_party_caravan_ambushers") ?? base.QuestGiver.Culture.BanditBossPartyTemplate;
				this._gangParty = CustomPartyComponent.CreateQuestParty(gatePosition, 0.1f, this._targetSettlement, new TextObject("{=gJNdkwHV}Gang Party", null), CampaignData.NeutralFaction, partyTemplateObject, base.QuestGiver, this.GangPartyTroopCount, "", "", 0f, false);
				this._gangParty.MemberRoster.AddToCounts(clan.Culture.BanditBoss, 1, true, 0, 0, true, -1);
				this._gangParty.ItemRoster.AddToCounts(Game.Current.ObjectManager.GetObject<ItemObject>("grain"), 40);
				this._gangParty.SetPartyUsedByQuest(true);
				this._gangParty.IgnoreByOtherPartiesTill(base.QuestDueTime);
				this._gangParty.Ai.SetDoNotMakeNewDecisions(true);
				this._gangParty.Ai.DisableAi();
				MobilePartyHelper.TryMatchPartySpeedWithItemWeight(this._gangParty, 0.2f, null);
				this._gangParty.Ai.SetMoveGoToSettlement(this._targetSettlement);
				EnterSettlementAction.ApplyForParty(this._gangParty, this._targetSettlement);
				base.AddTrackedObject(this._targetSettlement);
				base.AddLog(this._talkedToMerchantLogText, false);
			}

			// Token: 0x06001083 RID: 4227 RVA: 0x000704E4 File Offset: 0x0006E6E4
			private void StartBattle(SnareTheWealthyIssueBehavior.SnareTheWealthyIssueQuest.SnareTheWealthyQuestChoice playerChoice)
			{
				this._playerChoice = playerChoice;
				if (this._caravanParty.MapEvent != null)
				{
					this._caravanParty.MapEvent.FinalizeEvent();
				}
				Settlement closestHideout = SettlementHelper.FindNearestHideout((Settlement x) => x.IsActive, null);
				Clan clan = Clan.BanditFactions.FirstOrDefault((Clan t) => t.Culture == closestHideout.Culture);
				Clan clan2 = ((playerChoice != SnareTheWealthyIssueBehavior.SnareTheWealthyIssueQuest.SnareTheWealthyQuestChoice.SidedWithCaravan) ? clan : this._caravanParty.Owner.SupporterOf);
				this._caravanParty.ActualClan = clan2;
				Clan clan3 = ((playerChoice == SnareTheWealthyIssueBehavior.SnareTheWealthyIssueQuest.SnareTheWealthyQuestChoice.SidedWithGang) ? base.QuestGiver.SupporterOf : clan);
				this._gangParty.ActualClan = clan3;
				PartyBase partyBase = ((playerChoice != SnareTheWealthyIssueBehavior.SnareTheWealthyIssueQuest.SnareTheWealthyQuestChoice.SidedWithGang) ? this._gangParty.Party : this._caravanParty.Party);
				PlayerEncounter.Start();
				PlayerEncounter.Current.SetupFields(partyBase, PartyBase.MainParty);
				PlayerEncounter.StartBattle();
				if (playerChoice == SnareTheWealthyIssueBehavior.SnareTheWealthyIssueQuest.SnareTheWealthyQuestChoice.BetrayedBoth)
				{
					this._caravanParty.MapEventSide = this._gangParty.MapEventSide;
					return;
				}
				if (playerChoice == SnareTheWealthyIssueBehavior.SnareTheWealthyIssueQuest.SnareTheWealthyQuestChoice.SidedWithCaravan)
				{
					this._caravanParty.MapEventSide = PartyBase.MainParty.MapEventSide;
					return;
				}
				this._gangParty.MapEventSide = PartyBase.MainParty.MapEventSide;
			}

			// Token: 0x06001084 RID: 4228 RVA: 0x00070624 File Offset: 0x0006E824
			private void StartEncounterDialogue()
			{
				LeaveSettlementAction.ApplyForParty(this._gangParty);
				PlayerEncounter.Finish(true);
				this._canEncounterConversationStart = true;
				ConversationCharacterData conversationCharacterData = new ConversationCharacterData(CharacterObject.PlayerCharacter, PartyBase.MainParty, true, false, false, false, false, false);
				ConversationCharacterData conversationCharacterData2;
				conversationCharacterData2..ctor(ConversationHelper.GetConversationCharacterPartyLeader(this._caravanParty.Party), this._caravanParty.Party, true, false, false, false, false, true);
				CampaignMission.OpenConversationMission(conversationCharacterData, conversationCharacterData2, "", "");
			}

			// Token: 0x06001085 RID: 4229 RVA: 0x00070698 File Offset: 0x0006E898
			private void StartDialogueWithoutCaravan()
			{
				PlayerEncounter.Finish(true);
				ConversationCharacterData conversationCharacterData = new ConversationCharacterData(CharacterObject.PlayerCharacter, PartyBase.MainParty, true, false, false, false, false, false);
				ConversationCharacterData conversationCharacterData2;
				conversationCharacterData2..ctor(ConversationHelper.GetConversationCharacterPartyLeader(this._gangParty.Party), this._gangParty.Party, true, false, false, false, false, false);
				CampaignMission.OpenConversationMission(conversationCharacterData, conversationCharacterData2, "", "");
			}

			// Token: 0x06001086 RID: 4230 RVA: 0x000706FC File Offset: 0x0006E8FC
			private void HourlyTick()
			{
				if (this._caravanParty != null)
				{
					if (this._caravanParty.Ai.DefaultBehavior != 14)
					{
						SetPartyAiAction.GetActionForEscortingParty(this._caravanParty, MobileParty.MainParty);
					}
					(this._caravanParty.PartyComponent as CustomPartyComponent).CustomPartyBaseSpeed = MobileParty.MainParty.Speed;
					if (MobileParty.MainParty.TargetParty == this._caravanParty)
					{
						this._caravanParty.Ai.SetMoveModeHold();
						this._isCaravanFollowing = false;
						return;
					}
					if (!this._isCaravanFollowing)
					{
						SetPartyAiAction.GetActionForEscortingParty(this._caravanParty, MobileParty.MainParty);
						this._isCaravanFollowing = true;
					}
				}
			}

			// Token: 0x06001087 RID: 4231 RVA: 0x000707A0 File Offset: 0x0006E9A0
			private void OnSettlementOwnerChanged(Settlement settlement, bool openToClaim, Hero newOwner, Hero oldOwner, Hero capturerHero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail detail)
			{
				if (settlement == base.QuestGiver.CurrentSettlement)
				{
					if (newOwner.Clan == Clan.PlayerClan)
					{
						this.OnCancel4();
						return;
					}
					this.OnCancel2();
				}
			}

			// Token: 0x06001088 RID: 4232 RVA: 0x000707CA File Offset: 0x0006E9CA
			public void OnWarDeclared(IFaction faction1, IFaction faction2, DeclareWarAction.DeclareWarDetail reason)
			{
				if ((faction1 == base.QuestGiver.MapFaction && faction2 == Hero.MainHero.MapFaction) || (faction2 == base.QuestGiver.MapFaction && faction1 == Hero.MainHero.MapFaction))
				{
					this.OnCancel1();
				}
			}

			// Token: 0x06001089 RID: 4233 RVA: 0x00070808 File Offset: 0x0006EA08
			public void OnVillageStateChanged(Village village, Village.VillageStates oldState, Village.VillageStates newState, MobileParty raiderParty)
			{
				if (village == this._targetSettlement.Village && newState != null)
				{
					this.OnCancel3();
				}
			}

			// Token: 0x0600108A RID: 4234 RVA: 0x00070824 File Offset: 0x0006EA24
			public void OnMapEventEnded(MapEvent mapEvent)
			{
				if (mapEvent.IsPlayerMapEvent && this._caravanParty != null)
				{
					if (mapEvent.InvolvedParties.Contains(this._caravanParty.Party))
					{
						if (!mapEvent.InvolvedParties.Contains(this._gangParty.Party))
						{
							this.OnFail1();
							return;
						}
						if (mapEvent.WinningSide == mapEvent.PlayerSide)
						{
							if (this._playerChoice == SnareTheWealthyIssueBehavior.SnareTheWealthyIssueQuest.SnareTheWealthyQuestChoice.SidedWithGang)
							{
								this.OnSuccess1();
								return;
							}
							if (this._playerChoice == SnareTheWealthyIssueBehavior.SnareTheWealthyIssueQuest.SnareTheWealthyQuestChoice.SidedWithCaravan)
							{
								this.OnFail2();
								return;
							}
							this.OnFail3();
							return;
						}
						else
						{
							if (this._playerChoice == SnareTheWealthyIssueBehavior.SnareTheWealthyIssueQuest.SnareTheWealthyQuestChoice.SidedWithGang)
							{
								this.OnFail4();
								return;
							}
							if (this._playerChoice == SnareTheWealthyIssueBehavior.SnareTheWealthyIssueQuest.SnareTheWealthyQuestChoice.SidedWithCaravan)
							{
								this.OnFail5();
								return;
							}
							this.OnFail6();
							return;
						}
					}
					else
					{
						this.OnFail1();
					}
				}
			}

			// Token: 0x0600108B RID: 4235 RVA: 0x000708E0 File Offset: 0x0006EAE0
			private void OnPartyJoinedArmy(MobileParty mobileParty)
			{
				if (mobileParty == MobileParty.MainParty && this._caravanParty != null)
				{
					this.OnFail1();
				}
			}

			// Token: 0x0600108C RID: 4236 RVA: 0x000708F8 File Offset: 0x0006EAF8
			private void OnGameMenuOpened(MenuCallbackArgs args)
			{
				if (this._startConversationDelegate != null && MobileParty.MainParty.CurrentSettlement == this._targetSettlement && this._caravanParty != null)
				{
					this._startConversationDelegate();
					this._startConversationDelegate = null;
				}
			}

			// Token: 0x0600108D RID: 4237 RVA: 0x00070930 File Offset: 0x0006EB30
			public void OnSettlementEntered(MobileParty party, Settlement settlement, Hero hero)
			{
				if (party == MobileParty.MainParty && settlement == this._targetSettlement && this._caravanParty != null)
				{
					if (this._caravanParty.Position2D.DistanceSquared(this._targetSettlement.Position2D) <= 20f)
					{
						this._startConversationDelegate = new SnareTheWealthyIssueBehavior.SnareTheWealthyIssueQuest.QuestEndDelegate(this.StartEncounterDialogue);
						return;
					}
					this._startConversationDelegate = new SnareTheWealthyIssueBehavior.SnareTheWealthyIssueQuest.QuestEndDelegate(this.StartDialogueWithoutCaravan);
				}
			}

			// Token: 0x0600108E RID: 4238 RVA: 0x000709A0 File Offset: 0x0006EBA0
			public void OnSettlementLeft(MobileParty party, Settlement settlement)
			{
				if (party == MobileParty.MainParty && this._caravanParty != null)
				{
					SetPartyAiAction.GetActionForEscortingParty(this._caravanParty, MobileParty.MainParty);
				}
			}

			// Token: 0x0600108F RID: 4239 RVA: 0x000709C2 File Offset: 0x0006EBC2
			private void CanHeroBecomePrisoner(Hero hero, ref bool result)
			{
				if (hero == Hero.MainHero && this._playerChoice != SnareTheWealthyIssueBehavior.SnareTheWealthyIssueQuest.SnareTheWealthyQuestChoice.None)
				{
					result = false;
				}
			}

			// Token: 0x06001090 RID: 4240 RVA: 0x000709D8 File Offset: 0x0006EBD8
			protected override void OnFinalize()
			{
				if (this._caravanParty != null && this._caravanParty.IsActive)
				{
					DestroyPartyAction.Apply(null, this._caravanParty);
				}
				if (this._gangParty != null && this._gangParty.IsActive)
				{
					DestroyPartyAction.Apply(null, this._gangParty);
				}
			}

			// Token: 0x06001091 RID: 4241 RVA: 0x00070A28 File Offset: 0x0006EC28
			private void OnSuccess1()
			{
				base.AddLog(this._success1LogText, false);
				TraitLevelingHelper.OnIssueSolvedThroughQuest(Hero.MainHero, new Tuple<TraitObject, int>[]
				{
					new Tuple<TraitObject, int>(DefaultTraits.Honor, -100)
				});
				TraitLevelingHelper.OnIssueSolvedThroughQuest(Hero.MainHero, new Tuple<TraitObject, int>[]
				{
					new Tuple<TraitObject, int>(DefaultTraits.Calculating, 50)
				});
				ChangeRelationAction.ApplyPlayerRelation(base.QuestGiver, 5, true, true);
				ChangeRelationAction.ApplyPlayerRelation(this._targetMerchantCharacter.HeroObject, -10, true, true);
				base.QuestGiver.AddPower(30f);
				GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, this.Reward1, false);
				base.CompleteQuestWithSuccess();
			}

			// Token: 0x06001092 RID: 4242 RVA: 0x00070ACB File Offset: 0x0006ECCB
			private void OnTimedOutWithoutTalkingToMerchant()
			{
				base.AddLog(this._timedOutWithoutTalkingToMerchantText, false);
				TraitLevelingHelper.OnIssueFailed(Hero.MainHero, new Tuple<TraitObject, int>[]
				{
					new Tuple<TraitObject, int>(DefaultTraits.Honor, -50)
				});
				ChangeRelationAction.ApplyPlayerRelation(base.QuestGiver, -5, true, true);
			}

			// Token: 0x06001093 RID: 4243 RVA: 0x00070B09 File Offset: 0x0006ED09
			private void OnFail1()
			{
				this.ApplyFail1Consequences();
				base.CompleteQuestWithFail(null);
			}

			// Token: 0x06001094 RID: 4244 RVA: 0x00070B18 File Offset: 0x0006ED18
			private void ApplyFail1Consequences()
			{
				base.AddLog(this._fail1LogText, false);
				TraitLevelingHelper.OnIssueFailed(Hero.MainHero, new Tuple<TraitObject, int>[]
				{
					new Tuple<TraitObject, int>(DefaultTraits.Honor, -50)
				});
				ChangeRelationAction.ApplyPlayerRelation(base.QuestGiver, -5, true, true);
				ChangeRelationAction.ApplyPlayerRelation(this._targetMerchantCharacter.HeroObject, -5, true, true);
			}

			// Token: 0x06001095 RID: 4245 RVA: 0x00070B78 File Offset: 0x0006ED78
			private void OnFail2()
			{
				base.AddLog(this._fail2OutcomeLogText, false);
				TraitLevelingHelper.OnIssueFailed(Hero.MainHero, new Tuple<TraitObject, int>[]
				{
					new Tuple<TraitObject, int>(DefaultTraits.Honor, 100)
				});
				ChangeRelationAction.ApplyPlayerRelation(base.QuestGiver, -10, true, true);
				ChangeRelationAction.ApplyPlayerRelation(this._targetMerchantCharacter.HeroObject, 5, true, true);
				GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, this.Reward2, false);
				base.CompleteQuestWithBetrayal(null);
			}

			// Token: 0x06001096 RID: 4246 RVA: 0x00070BF0 File Offset: 0x0006EDF0
			private void OnFail3()
			{
				base.AddLog(this._fail3OutcomeLogText, false);
				TraitLevelingHelper.OnIssueFailed(Hero.MainHero, new Tuple<TraitObject, int>[]
				{
					new Tuple<TraitObject, int>(DefaultTraits.Honor, -200)
				});
				TraitLevelingHelper.OnIssueFailed(Hero.MainHero, new Tuple<TraitObject, int>[]
				{
					new Tuple<TraitObject, int>(DefaultTraits.Calculating, 100)
				});
				ChangeRelationAction.ApplyPlayerRelation(base.QuestGiver, -15, true, true);
				ChangeRelationAction.ApplyPlayerRelation(this._targetMerchantCharacter.HeroObject, -20, true, true);
				base.CompleteQuestWithBetrayal(null);
			}

			// Token: 0x06001097 RID: 4247 RVA: 0x00070C78 File Offset: 0x0006EE78
			private void OnFail4()
			{
				base.AddLog(this._fail4LogText, false);
				TraitLevelingHelper.OnIssueFailed(Hero.MainHero, new Tuple<TraitObject, int>[]
				{
					new Tuple<TraitObject, int>(DefaultTraits.Honor, -100)
				});
				TraitLevelingHelper.OnIssueFailed(Hero.MainHero, new Tuple<TraitObject, int>[]
				{
					new Tuple<TraitObject, int>(DefaultTraits.Calculating, 100)
				});
				ChangeRelationAction.ApplyPlayerRelation(base.QuestGiver, -10, true, true);
				ChangeRelationAction.ApplyPlayerRelation(this._targetMerchantCharacter.HeroObject, -10, true, true);
				base.CompleteQuestWithFail(null);
			}

			// Token: 0x06001098 RID: 4248 RVA: 0x00070CFC File Offset: 0x0006EEFC
			private void OnFail5()
			{
				base.AddLog(this._fail5LogText, false);
				TraitLevelingHelper.OnIssueFailed(Hero.MainHero, new Tuple<TraitObject, int>[]
				{
					new Tuple<TraitObject, int>(DefaultTraits.Honor, -100)
				});
				TraitLevelingHelper.OnIssueFailed(Hero.MainHero, new Tuple<TraitObject, int>[]
				{
					new Tuple<TraitObject, int>(DefaultTraits.Calculating, 100)
				});
				ChangeRelationAction.ApplyPlayerRelation(base.QuestGiver, -10, true, true);
				ChangeRelationAction.ApplyPlayerRelation(this._targetMerchantCharacter.HeroObject, -10, true, true);
				base.CompleteQuestWithBetrayal(null);
			}

			// Token: 0x06001099 RID: 4249 RVA: 0x00070D80 File Offset: 0x0006EF80
			private void OnFail6()
			{
				base.AddLog(this._fail6LogText, false);
				TraitLevelingHelper.OnIssueFailed(Hero.MainHero, new Tuple<TraitObject, int>[]
				{
					new Tuple<TraitObject, int>(DefaultTraits.Honor, -200)
				});
				TraitLevelingHelper.OnIssueFailed(Hero.MainHero, new Tuple<TraitObject, int>[]
				{
					new Tuple<TraitObject, int>(DefaultTraits.Calculating, 100)
				});
				ChangeRelationAction.ApplyPlayerRelation(base.QuestGiver, -15, true, true);
				ChangeRelationAction.ApplyPlayerRelation(this._targetMerchantCharacter.HeroObject, -20, true, true);
				base.CompleteQuestWithBetrayal(null);
			}

			// Token: 0x0600109A RID: 4250 RVA: 0x00070E06 File Offset: 0x0006F006
			protected override void OnTimedOut()
			{
				if (this._caravanParty == null)
				{
					this.OnTimedOutWithoutTalkingToMerchant();
					return;
				}
				this.ApplyFail1Consequences();
			}

			// Token: 0x0600109B RID: 4251 RVA: 0x00070E1D File Offset: 0x0006F01D
			private void OnCancel1()
			{
				base.AddLog(this._warDeclaredBetweenPlayerAndQuestGiverLogText, false);
				base.CompleteQuestWithCancel(null);
			}

			// Token: 0x0600109C RID: 4252 RVA: 0x00070E34 File Offset: 0x0006F034
			private void OnCancel2()
			{
				base.AddLog(this._questSettlementWasCapturedLogText, false);
				base.CompleteQuestWithCancel(null);
			}

			// Token: 0x0600109D RID: 4253 RVA: 0x00070E4B File Offset: 0x0006F04B
			private void OnCancel3()
			{
				base.AddLog(this._targetSettlementRaidedLogText, false);
				base.CompleteQuestWithCancel(null);
			}

			// Token: 0x0600109E RID: 4254 RVA: 0x00070E62 File Offset: 0x0006F062
			private void OnCancel4()
			{
				base.AddLog(this._playerCapturedQuestSettlementLogText, false);
				base.QuestGiver.AddPower(-10f);
				ChangeRelationAction.ApplyPlayerRelation(base.QuestGiver, -5, true, true);
				base.CompleteQuestWithCancel(null);
			}

			// Token: 0x0600109F RID: 4255 RVA: 0x00070E98 File Offset: 0x0006F098
			private bool IsGangPartyLeader(IAgent agent)
			{
				return agent.Character == ConversationHelper.GetConversationCharacterPartyLeader(this._gangParty.Party);
			}

			// Token: 0x060010A0 RID: 4256 RVA: 0x00070EB2 File Offset: 0x0006F0B2
			private bool IsCaravanMaster(IAgent agent)
			{
				return agent.Character == ConversationHelper.GetConversationCharacterPartyLeader(this._caravanParty.Party);
			}

			// Token: 0x060010A1 RID: 4257 RVA: 0x00070ECC File Offset: 0x0006F0CC
			private bool IsMainHero(IAgent agent)
			{
				return agent.Character == CharacterObject.PlayerCharacter;
			}

			// Token: 0x060010A2 RID: 4258 RVA: 0x00070EDB File Offset: 0x0006F0DB
			public override void OnHeroCanHaveQuestOrIssueInfoIsRequested(Hero hero, ref bool result)
			{
				if (hero == this._targetMerchantCharacter.HeroObject)
				{
					result = false;
				}
			}

			// Token: 0x060010A3 RID: 4259 RVA: 0x00070EF0 File Offset: 0x0006F0F0
			protected override void RegisterEvents()
			{
				CampaignEvents.OnSettlementOwnerChangedEvent.AddNonSerializedListener(this, new Action<Settlement, bool, Hero, Hero, Hero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail>(this.OnSettlementOwnerChanged));
				CampaignEvents.WarDeclared.AddNonSerializedListener(this, new Action<IFaction, IFaction, DeclareWarAction.DeclareWarDetail>(this.OnWarDeclared));
				CampaignEvents.VillageStateChanged.AddNonSerializedListener(this, new Action<Village, Village.VillageStates, Village.VillageStates, MobileParty>(this.OnVillageStateChanged));
				CampaignEvents.MapEventEnded.AddNonSerializedListener(this, new Action<MapEvent>(this.OnMapEventEnded));
				CampaignEvents.OnPartyJoinedArmyEvent.AddNonSerializedListener(this, new Action<MobileParty>(this.OnPartyJoinedArmy));
				CampaignEvents.GameMenuOpened.AddNonSerializedListener(this, new Action<MenuCallbackArgs>(this.OnGameMenuOpened));
				CampaignEvents.SettlementEntered.AddNonSerializedListener(this, new Action<MobileParty, Settlement, Hero>(this.OnSettlementEntered));
				CampaignEvents.OnSettlementLeftEvent.AddNonSerializedListener(this, new Action<MobileParty, Settlement>(this.OnSettlementLeft));
				CampaignEvents.CanHeroBecomePrisonerEvent.AddNonSerializedListener(this, new ReferenceAction<Hero, bool>(this.CanHeroBecomePrisoner));
				CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(this.HourlyTick));
				CampaignEvents.CanHaveQuestsOrIssuesEvent.AddNonSerializedListener(this, new ReferenceAction<Hero, bool>(this.OnHeroCanHaveQuestOrIssueInfoIsRequested));
			}

			// Token: 0x060010A4 RID: 4260 RVA: 0x00070FFB File Offset: 0x0006F1FB
			internal static void AutoGeneratedStaticCollectObjectsSnareTheWealthyIssueQuest(object o, List<object> collectedObjects)
			{
				((SnareTheWealthyIssueBehavior.SnareTheWealthyIssueQuest)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			// Token: 0x060010A5 RID: 4261 RVA: 0x00071009 File Offset: 0x0006F209
			protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				base.AutoGeneratedInstanceCollectObjects(collectedObjects);
				collectedObjects.Add(this._targetMerchantCharacter);
				collectedObjects.Add(this._targetSettlement);
				collectedObjects.Add(this._caravanParty);
				collectedObjects.Add(this._gangParty);
			}

			// Token: 0x060010A6 RID: 4262 RVA: 0x00071042 File Offset: 0x0006F242
			internal static object AutoGeneratedGetMemberValue_targetMerchantCharacter(object o)
			{
				return ((SnareTheWealthyIssueBehavior.SnareTheWealthyIssueQuest)o)._targetMerchantCharacter;
			}

			// Token: 0x060010A7 RID: 4263 RVA: 0x0007104F File Offset: 0x0006F24F
			internal static object AutoGeneratedGetMemberValue_targetSettlement(object o)
			{
				return ((SnareTheWealthyIssueBehavior.SnareTheWealthyIssueQuest)o)._targetSettlement;
			}

			// Token: 0x060010A8 RID: 4264 RVA: 0x0007105C File Offset: 0x0006F25C
			internal static object AutoGeneratedGetMemberValue_caravanParty(object o)
			{
				return ((SnareTheWealthyIssueBehavior.SnareTheWealthyIssueQuest)o)._caravanParty;
			}

			// Token: 0x060010A9 RID: 4265 RVA: 0x00071069 File Offset: 0x0006F269
			internal static object AutoGeneratedGetMemberValue_gangParty(object o)
			{
				return ((SnareTheWealthyIssueBehavior.SnareTheWealthyIssueQuest)o)._gangParty;
			}

			// Token: 0x060010AA RID: 4266 RVA: 0x00071076 File Offset: 0x0006F276
			internal static object AutoGeneratedGetMemberValue_questDifficulty(object o)
			{
				return ((SnareTheWealthyIssueBehavior.SnareTheWealthyIssueQuest)o)._questDifficulty;
			}

			// Token: 0x060010AB RID: 4267 RVA: 0x00071088 File Offset: 0x0006F288
			internal static object AutoGeneratedGetMemberValue_playerChoice(object o)
			{
				return ((SnareTheWealthyIssueBehavior.SnareTheWealthyIssueQuest)o)._playerChoice;
			}

			// Token: 0x060010AC RID: 4268 RVA: 0x0007109A File Offset: 0x0006F29A
			internal static object AutoGeneratedGetMemberValue_canEncounterConversationStart(object o)
			{
				return ((SnareTheWealthyIssueBehavior.SnareTheWealthyIssueQuest)o)._canEncounterConversationStart;
			}

			// Token: 0x060010AD RID: 4269 RVA: 0x000710AC File Offset: 0x0006F2AC
			internal static object AutoGeneratedGetMemberValue_isCaravanFollowing(object o)
			{
				return ((SnareTheWealthyIssueBehavior.SnareTheWealthyIssueQuest)o)._isCaravanFollowing;
			}

			// Token: 0x0400068F RID: 1679
			private const float CaravanEncounterStartDistance = 20f;

			// Token: 0x04000690 RID: 1680
			private SnareTheWealthyIssueBehavior.SnareTheWealthyIssueQuest.QuestEndDelegate _startConversationDelegate;

			// Token: 0x04000691 RID: 1681
			[SaveableField(1)]
			private CharacterObject _targetMerchantCharacter;

			// Token: 0x04000692 RID: 1682
			[SaveableField(2)]
			private Settlement _targetSettlement;

			// Token: 0x04000693 RID: 1683
			[SaveableField(3)]
			private MobileParty _caravanParty;

			// Token: 0x04000694 RID: 1684
			[SaveableField(4)]
			private MobileParty _gangParty;

			// Token: 0x04000695 RID: 1685
			[SaveableField(5)]
			private readonly float _questDifficulty;

			// Token: 0x04000696 RID: 1686
			[SaveableField(6)]
			private SnareTheWealthyIssueBehavior.SnareTheWealthyIssueQuest.SnareTheWealthyQuestChoice _playerChoice;

			// Token: 0x04000697 RID: 1687
			[SaveableField(7)]
			private bool _canEncounterConversationStart;

			// Token: 0x04000698 RID: 1688
			[SaveableField(8)]
			private bool _isCaravanFollowing = true;

			// Token: 0x020001D6 RID: 470
			internal enum SnareTheWealthyQuestChoice
			{
				// Token: 0x0400086B RID: 2155
				None,
				// Token: 0x0400086C RID: 2156
				SidedWithCaravan,
				// Token: 0x0400086D RID: 2157
				SidedWithGang,
				// Token: 0x0400086E RID: 2158
				BetrayedBoth
			}

			// Token: 0x020001D7 RID: 471
			// (Invoke) Token: 0x06001300 RID: 4864
			private delegate void QuestEndDelegate();
		}
	}
}
