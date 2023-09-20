using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using SandBox.Conversation.MissionLogics;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.GameState;
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
	// Token: 0x02000081 RID: 129
	public class TheSpyPartyIssueQuestBehavior : CampaignBehaviorBase
	{
		// Token: 0x0600057A RID: 1402 RVA: 0x00026D6F File Offset: 0x00024F6F
		public override void RegisterEvents()
		{
			CampaignEvents.OnCheckForIssueEvent.AddNonSerializedListener(this, new Action<Hero>(this.OnCheckForIssue));
		}

		// Token: 0x0600057B RID: 1403 RVA: 0x00026D88 File Offset: 0x00024F88
		public void OnCheckForIssue(Hero hero)
		{
			Settlement settlement;
			if (this.ConditionsHold(hero, out settlement))
			{
				Campaign.Current.IssueManager.AddPotentialIssueData(hero, new PotentialIssueData(new PotentialIssueData.StartIssueDelegate(this.OnStartIssue), typeof(TheSpyPartyIssueQuestBehavior.TheSpyPartyIssue), 2, settlement));
				return;
			}
			Campaign.Current.IssueManager.AddPotentialIssueData(hero, new PotentialIssueData(typeof(TheSpyPartyIssueQuestBehavior.TheSpyPartyIssue), 2));
		}

		// Token: 0x0600057C RID: 1404 RVA: 0x00026DF0 File Offset: 0x00024FF0
		private bool ConditionsHold(Hero issueGiver, out Settlement selectedSettlement)
		{
			selectedSettlement = null;
			if (issueGiver.IsLord && issueGiver.Clan != Clan.PlayerClan)
			{
				if (issueGiver.Clan.Settlements.Any((Settlement x) => x.IsTown))
				{
					selectedSettlement = Extensions.GetRandomElementWithPredicate<Settlement>(issueGiver.Clan.Settlements, (Settlement x) => x.IsTown);
					string difficultySuffix = TheSpyPartyIssueQuestBehavior.GetDifficultySuffix(Campaign.Current.Models.IssueModel.GetIssueDifficultyMultiplier());
					bool flag = MBObjectManager.Instance.GetObject<CharacterObject>("bold_contender_" + difficultySuffix) != null && MBObjectManager.Instance.GetObject<CharacterObject>("confident_contender_" + difficultySuffix) != null && MBObjectManager.Instance.GetObject<CharacterObject>("dignified_contender_" + difficultySuffix) != null && MBObjectManager.Instance.GetObject<CharacterObject>("hardy_contender_" + difficultySuffix) != null;
					if (!flag)
					{
						CampaignEventDispatcher.Instance.RemoveListeners(this);
					}
					return flag;
				}
			}
			return false;
		}

		// Token: 0x0600057D RID: 1405 RVA: 0x00026F0A File Offset: 0x0002510A
		private static string GetDifficultySuffix(float difficulty)
		{
			if (difficulty <= 0.25f)
			{
				return "easy";
			}
			if (difficulty <= 0.5f)
			{
				return "normal";
			}
			if (difficulty <= 0.75f)
			{
				return "hard";
			}
			return "very_hard";
		}

		// Token: 0x0600057E RID: 1406 RVA: 0x00026F3C File Offset: 0x0002513C
		private IssueBase OnStartIssue(in PotentialIssueData pid, Hero issueOwner)
		{
			PotentialIssueData potentialIssueData = pid;
			return new TheSpyPartyIssueQuestBehavior.TheSpyPartyIssue(issueOwner, potentialIssueData.RelatedObject as Settlement);
		}

		// Token: 0x0600057F RID: 1407 RVA: 0x00026F62 File Offset: 0x00025162
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x040002AC RID: 684
		private const IssueBase.IssueFrequency TheSpyPartyIssueFrequency = 2;

		// Token: 0x040002AD RID: 685
		private const int IssueDuration = 5;

		// Token: 0x02000161 RID: 353
		public class TheSpyPartyIssueQuestTypeDefiner : CampaignBehaviorBase.SaveableCampaignBehaviorTypeDefiner
		{
			// Token: 0x060010BB RID: 4283 RVA: 0x000712AF File Offset: 0x0006F4AF
			public TheSpyPartyIssueQuestTypeDefiner()
				: base(121250)
			{
			}

			// Token: 0x060010BC RID: 4284 RVA: 0x000712BC File Offset: 0x0006F4BC
			protected override void DefineClassTypes()
			{
				base.AddClassDefinition(typeof(TheSpyPartyIssueQuestBehavior.TheSpyPartyIssue), 1, null);
				base.AddClassDefinition(typeof(TheSpyPartyIssueQuestBehavior.TheSpyPartyIssueQuest), 2, null);
			}

			// Token: 0x060010BD RID: 4285 RVA: 0x000712E2 File Offset: 0x0006F4E2
			protected override void DefineStructTypes()
			{
				base.AddStructDefinition(typeof(TheSpyPartyIssueQuestBehavior.SuspectNpc), 3, null);
			}
		}

		// Token: 0x02000162 RID: 354
		public struct SuspectNpc
		{
			// Token: 0x060010BE RID: 4286 RVA: 0x000712F6 File Offset: 0x0006F4F6
			public SuspectNpc(CharacterObject characterObject, bool hasHair, bool hasBigSword, bool withoutMarkings, bool hasBeard)
			{
				this.CharacterObject = characterObject;
				this.HasHair = hasHair;
				this.HasBigSword = hasBigSword;
				this.WithoutMarkings = withoutMarkings;
				this.HasBeard = hasBeard;
			}

			// Token: 0x060010BF RID: 4287 RVA: 0x00071320 File Offset: 0x0006F520
			public static void AutoGeneratedStaticCollectObjectsSuspectNpc(object o, List<object> collectedObjects)
			{
				((TheSpyPartyIssueQuestBehavior.SuspectNpc)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			// Token: 0x060010C0 RID: 4288 RVA: 0x0007133C File Offset: 0x0006F53C
			private void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				collectedObjects.Add(this.CharacterObject);
			}

			// Token: 0x060010C1 RID: 4289 RVA: 0x0007134A File Offset: 0x0006F54A
			internal static object AutoGeneratedGetMemberValueCharacterObject(object o)
			{
				return ((TheSpyPartyIssueQuestBehavior.SuspectNpc)o).CharacterObject;
			}

			// Token: 0x060010C2 RID: 4290 RVA: 0x00071357 File Offset: 0x0006F557
			internal static object AutoGeneratedGetMemberValueHasHair(object o)
			{
				return ((TheSpyPartyIssueQuestBehavior.SuspectNpc)o).HasHair;
			}

			// Token: 0x060010C3 RID: 4291 RVA: 0x00071369 File Offset: 0x0006F569
			internal static object AutoGeneratedGetMemberValueHasBigSword(object o)
			{
				return ((TheSpyPartyIssueQuestBehavior.SuspectNpc)o).HasBigSword;
			}

			// Token: 0x060010C4 RID: 4292 RVA: 0x0007137B File Offset: 0x0006F57B
			internal static object AutoGeneratedGetMemberValueWithoutMarkings(object o)
			{
				return ((TheSpyPartyIssueQuestBehavior.SuspectNpc)o).WithoutMarkings;
			}

			// Token: 0x060010C5 RID: 4293 RVA: 0x0007138D File Offset: 0x0006F58D
			internal static object AutoGeneratedGetMemberValueHasBeard(object o)
			{
				return ((TheSpyPartyIssueQuestBehavior.SuspectNpc)o).HasBeard;
			}

			// Token: 0x04000699 RID: 1689
			[SaveableField(10)]
			public readonly CharacterObject CharacterObject;

			// Token: 0x0400069A RID: 1690
			[SaveableField(20)]
			public readonly bool HasHair;

			// Token: 0x0400069B RID: 1691
			[SaveableField(30)]
			public readonly bool HasBigSword;

			// Token: 0x0400069C RID: 1692
			[SaveableField(40)]
			public readonly bool WithoutMarkings;

			// Token: 0x0400069D RID: 1693
			[SaveableField(50)]
			public readonly bool HasBeard;
		}

		// Token: 0x02000163 RID: 355
		public class TheSpyPartyIssue : IssueBase
		{
			// Token: 0x170001CE RID: 462
			// (get) Token: 0x060010C6 RID: 4294 RVA: 0x0007139F File Offset: 0x0006F59F
			public override IssueBase.AlternativeSolutionScaleFlag AlternativeSolutionScaleFlags
			{
				get
				{
					return 8;
				}
			}

			// Token: 0x170001CF RID: 463
			// (get) Token: 0x060010C7 RID: 4295 RVA: 0x000713A2 File Offset: 0x0006F5A2
			protected override int CompanionSkillRewardXP
			{
				get
				{
					return (int)(600f + 800f * base.IssueDifficultyMultiplier);
				}
			}

			// Token: 0x170001D0 RID: 464
			// (get) Token: 0x060010C8 RID: 4296 RVA: 0x000713B7 File Offset: 0x0006F5B7
			public override bool IsThereAlternativeSolution
			{
				get
				{
					return true;
				}
			}

			// Token: 0x170001D1 RID: 465
			// (get) Token: 0x060010C9 RID: 4297 RVA: 0x000713BA File Offset: 0x0006F5BA
			public override bool IsThereLordSolution
			{
				get
				{
					return false;
				}
			}

			// Token: 0x170001D2 RID: 466
			// (get) Token: 0x060010CA RID: 4298 RVA: 0x000713BD File Offset: 0x0006F5BD
			public override int AlternativeSolutionBaseNeededMenCount
			{
				get
				{
					return 1 + MathF.Ceiling(4f * base.IssueDifficultyMultiplier);
				}
			}

			// Token: 0x170001D3 RID: 467
			// (get) Token: 0x060010CB RID: 4299 RVA: 0x000713D2 File Offset: 0x0006F5D2
			protected override int AlternativeSolutionBaseDurationInDaysInternal
			{
				get
				{
					return 3 + MathF.Ceiling(3f * base.IssueDifficultyMultiplier);
				}
			}

			// Token: 0x170001D4 RID: 468
			// (get) Token: 0x060010CC RID: 4300 RVA: 0x000713E7 File Offset: 0x0006F5E7
			protected override int RewardGold
			{
				get
				{
					return (int)(500f + 3000f * base.IssueDifficultyMultiplier);
				}
			}

			// Token: 0x170001D5 RID: 469
			// (get) Token: 0x060010CD RID: 4301 RVA: 0x000713FC File Offset: 0x0006F5FC
			public override TextObject IssueBriefByIssueGiver
			{
				get
				{
					TextObject textObject = new TextObject("{=tFPJySG7}I am hosting a tournament at {SELECTED_SETTLEMENT}. I am expecting contenders to partake from all over the realm. I have my reasons to believe one of the attending warriors is actually a spy, sent to gather information about its defenses.", null);
					textObject.SetTextVariable("SELECTED_SETTLEMENT", this._selectedSettlement.EncyclopediaLinkWithName);
					return textObject;
				}
			}

			// Token: 0x170001D6 RID: 470
			// (get) Token: 0x060010CE RID: 4302 RVA: 0x00071420 File Offset: 0x0006F620
			public override TextObject IssueAcceptByPlayer
			{
				get
				{
					return new TextObject("{=EYT7b2J5}Any traveler can be asked by an enemy to spy on the places he travels. How can I track this one down?", null);
				}
			}

			// Token: 0x170001D7 RID: 471
			// (get) Token: 0x060010CF RID: 4303 RVA: 0x0007142D File Offset: 0x0006F62D
			public override TextObject IssueQuestSolutionExplanationByIssueGiver
			{
				get
				{
					TextObject textObject = new TextObject("{=2lgkL9db}Of course. I have employed spies myself. But if a tournament participant is asking questions about the state of the garrison and the walls, things which would concern no honest traveler - well, between that and the private information I've received, I think we'd have our man. The spy must be hiding inside {SELECTED_SETTLEMENT}. Once you are there start questioning the townsfolk.", null);
					textObject.SetTextVariable("SELECTED_SETTLEMENT", this._selectedSettlement.EncyclopediaLinkWithName);
					return textObject;
				}
			}

			// Token: 0x170001D8 RID: 472
			// (get) Token: 0x060010D0 RID: 4304 RVA: 0x00071451 File Offset: 0x0006F651
			public override TextObject IssuePlayerResponseAfterAlternativeExplanation
			{
				get
				{
					return new TextObject("{=2nFBTmao}Is there any other way to solve this other than asking around?", null);
				}
			}

			// Token: 0x170001D9 RID: 473
			// (get) Token: 0x060010D1 RID: 4305 RVA: 0x0007145E File Offset: 0x0006F65E
			public override TextObject IssueAlternativeSolutionExplanationByIssueGiver
			{
				get
				{
					TextObject textObject = new TextObject("{=avVno3H8}Well, you can assign a companion of yours with a knack for this kind of game and enough muscles to back him up. Judging from what I have heard, a group of {NEEDED_MEN_COUNT} should be enough.", null);
					textObject.SetTextVariable("NEEDED_MEN_COUNT", base.GetTotalAlternativeSolutionNeededMenCount());
					return textObject;
				}
			}

			// Token: 0x170001DA RID: 474
			// (get) Token: 0x060010D2 RID: 4306 RVA: 0x0007147D File Offset: 0x0006F67D
			public override TextObject IssueQuestSolutionAcceptByPlayer
			{
				get
				{
					return new TextObject("{=99OsuHGa}I will find the one you are looking for.", null);
				}
			}

			// Token: 0x170001DB RID: 475
			// (get) Token: 0x060010D3 RID: 4307 RVA: 0x0007148A File Offset: 0x0006F68A
			public override TextObject Title
			{
				get
				{
					return new TextObject("{=SJHtVaNa}The Spy Among Us", null);
				}
			}

			// Token: 0x170001DC RID: 476
			// (get) Token: 0x060010D4 RID: 4308 RVA: 0x00071498 File Offset: 0x0006F698
			public override TextObject Description
			{
				get
				{
					TextObject textObject = new TextObject("{=C6rbcpbi}{QUEST_GIVER.LINK} wants you to find a spy before he causes any harm...", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.IssueOwner.CharacterObject, textObject, false);
					return textObject;
				}
			}

			// Token: 0x170001DD RID: 477
			// (get) Token: 0x060010D5 RID: 4309 RVA: 0x000714CA File Offset: 0x0006F6CA
			public override TextObject IssueAlternativeSolutionAcceptByPlayer
			{
				get
				{
					TextObject textObject = new TextObject("{=FEcAwSfk}I will assign a companion with {NEEDED_MEN_COUNT} good men for {ALTERNATIVE_SOLUTION_DURATION} days.", null);
					textObject.SetTextVariable("NEEDED_MEN_COUNT", base.GetTotalAlternativeSolutionNeededMenCount());
					textObject.SetTextVariable("ALTERNATIVE_SOLUTION_DURATION", base.GetTotalAlternativeSolutionDurationInDays());
					return textObject;
				}
			}

			// Token: 0x170001DE RID: 478
			// (get) Token: 0x060010D6 RID: 4310 RVA: 0x000714FB File Offset: 0x0006F6FB
			public override TextObject IssueDiscussAlternativeSolution
			{
				get
				{
					return new TextObject("{=O0Cjam62}I hope your people are careful about how they proceed. It would be a pity if that spy got away.", null);
				}
			}

			// Token: 0x170001DF RID: 479
			// (get) Token: 0x060010D7 RID: 4311 RVA: 0x00071508 File Offset: 0x0006F708
			public override TextObject IssueAlternativeSolutionResponseByIssueGiver
			{
				get
				{
					TextObject textObject = new TextObject("{=ciXBiMMa}Thank you {PLAYER.NAME}, I hope your people will be successful.", null);
					StringHelpers.SetCharacterProperties("PLAYER", CharacterObject.PlayerCharacter, textObject, false);
					return textObject;
				}
			}

			// Token: 0x170001E0 RID: 480
			// (get) Token: 0x060010D8 RID: 4312 RVA: 0x00071534 File Offset: 0x0006F734
			protected override TextObject AlternativeSolutionStartLog
			{
				get
				{
					TextObject textObject = new TextObject("{=s5qs0bPs}{ISSUE_GIVER.LINK}, the {?ISSUE_GIVER.GENDER}lady{?}lord{\\?} of {QUEST_SETTLEMENT}, has told you about a spy that hides as a tournament attendee. You are asked to expose the spy and take care of him. You asked {COMPANION.LINK} to take {NEEDED_MEN_COUNT} of your best men to go and take care of it. They should report back to you in {ALTERNATIVE_SOLUTION_DURATION} days.", null);
					StringHelpers.SetCharacterProperties("ISSUE_GIVER", base.IssueOwner.CharacterObject, textObject, false);
					StringHelpers.SetCharacterProperties("COMPANION", base.AlternativeSolutionHero.CharacterObject, textObject, false);
					textObject.SetTextVariable("QUEST_SETTLEMENT", this._selectedSettlement.EncyclopediaLinkWithName);
					textObject.SetTextVariable("ALTERNATIVE_SOLUTION_DURATION", base.GetTotalAlternativeSolutionDurationInDays());
					textObject.SetTextVariable("NEEDED_MEN_COUNT", this.AlternativeSolutionSentTroops.TotalManCount - 1);
					return textObject;
				}
			}

			// Token: 0x060010D9 RID: 4313 RVA: 0x000715C0 File Offset: 0x0006F7C0
			public TheSpyPartyIssue(Hero issueOwner, Settlement selectedSettlement)
				: base(issueOwner, CampaignTime.DaysFromNow(5f))
			{
				this._selectedSettlement = selectedSettlement;
			}

			// Token: 0x060010DA RID: 4314 RVA: 0x000715DA File Offset: 0x0006F7DA
			protected override float GetIssueEffectAmountInternal(IssueEffect issueEffect)
			{
				if (issueEffect == DefaultIssueEffects.SettlementSecurity)
				{
					return -2f;
				}
				if (issueEffect == DefaultIssueEffects.SettlementLoyalty)
				{
					return -0.5f;
				}
				if (issueEffect == DefaultIssueEffects.ClanInfluence)
				{
					return -0.2f;
				}
				return 0f;
			}

			// Token: 0x060010DB RID: 4315 RVA: 0x0007160B File Offset: 0x0006F80B
			public override ValueTuple<SkillObject, int> GetAlternativeSolutionSkill(Hero hero)
			{
				return new ValueTuple<SkillObject, int>((hero.GetSkillValue(DefaultSkills.Charm) >= hero.GetSkillValue(DefaultSkills.Roguery)) ? DefaultSkills.Charm : DefaultSkills.Roguery, 150);
			}

			// Token: 0x060010DC RID: 4316 RVA: 0x0007163B File Offset: 0x0006F83B
			public override bool AlternativeSolutionCondition(out TextObject explanation)
			{
				explanation = TextObject.Empty;
				return QuestHelper.CheckRosterForAlternativeSolution(MobileParty.MainParty.MemberRoster, base.GetTotalAlternativeSolutionNeededMenCount(), ref explanation, 2, false);
			}

			// Token: 0x060010DD RID: 4317 RVA: 0x0007165C File Offset: 0x0006F85C
			public override bool IsTroopTypeNeededByAlternativeSolution(CharacterObject character)
			{
				return character.Tier >= 2;
			}

			// Token: 0x060010DE RID: 4318 RVA: 0x0007166A File Offset: 0x0006F86A
			public override bool DoTroopsSatisfyAlternativeSolution(TroopRoster troopRoster, out TextObject explanation)
			{
				explanation = TextObject.Empty;
				return QuestHelper.CheckRosterForAlternativeSolution(troopRoster, base.GetTotalAlternativeSolutionNeededMenCount(), ref explanation, 2, false);
			}

			// Token: 0x060010DF RID: 4319 RVA: 0x00071682 File Offset: 0x0006F882
			protected override void AlternativeSolutionEndWithSuccessConsequence()
			{
				GainRenownAction.Apply(Hero.MainHero, 1f, false);
				this.RelationshipChangeWithIssueOwner = 5;
				this._selectedSettlement.Prosperity += 5f;
			}

			// Token: 0x060010E0 RID: 4320 RVA: 0x000716B4 File Offset: 0x0006F8B4
			protected override void AlternativeSolutionEndWithFailureConsequence()
			{
				this.RelationshipChangeWithIssueOwner = -5;
				base.IssueOwner.AddPower(-5f);
				this._selectedSettlement.Town.Security -= 10f;
				this._selectedSettlement.Town.Loyalty -= 10f;
			}

			// Token: 0x060010E1 RID: 4321 RVA: 0x00071711 File Offset: 0x0006F911
			protected override void OnGameLoad()
			{
			}

			// Token: 0x060010E2 RID: 4322 RVA: 0x00071713 File Offset: 0x0006F913
			protected override QuestBase GenerateIssueQuest(string questId)
			{
				return new TheSpyPartyIssueQuestBehavior.TheSpyPartyIssueQuest(questId, base.IssueOwner, CampaignTime.DaysFromNow(16f), this.RewardGold, this._selectedSettlement, base.IssueDifficultyMultiplier);
			}

			// Token: 0x060010E3 RID: 4323 RVA: 0x0007173D File Offset: 0x0006F93D
			public override IssueBase.IssueFrequency GetFrequency()
			{
				return 2;
			}

			// Token: 0x060010E4 RID: 4324 RVA: 0x00071740 File Offset: 0x0006F940
			protected override bool CanPlayerTakeQuestConditions(Hero issueGiver, out IssueBase.PreconditionFlags flag, out Hero relationHero, out SkillObject skill)
			{
				flag = 0;
				relationHero = null;
				skill = null;
				if (issueGiver.GetRelationWithPlayer() < -10f)
				{
					flag = (int)(flag | 1U);
					relationHero = issueGiver;
				}
				if (issueGiver.MapFaction.IsAtWarWith(Hero.MainHero.MapFaction))
				{
					flag = (int)(flag | 64U);
				}
				return flag == 0U;
			}

			// Token: 0x060010E5 RID: 4325 RVA: 0x00071790 File Offset: 0x0006F990
			public override bool IssueStayAliveConditions()
			{
				return base.IssueOwner.IsAlive && this._selectedSettlement.OwnerClan == base.IssueOwner.Clan && base.IssueOwner.Clan != Clan.PlayerClan;
			}

			// Token: 0x060010E6 RID: 4326 RVA: 0x000717CE File Offset: 0x0006F9CE
			protected override void CompleteIssueWithTimedOutConsequences()
			{
			}

			// Token: 0x060010E7 RID: 4327 RVA: 0x000717D0 File Offset: 0x0006F9D0
			internal static void AutoGeneratedStaticCollectObjectsTheSpyPartyIssue(object o, List<object> collectedObjects)
			{
				((TheSpyPartyIssueQuestBehavior.TheSpyPartyIssue)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			// Token: 0x060010E8 RID: 4328 RVA: 0x000717DE File Offset: 0x0006F9DE
			protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				base.AutoGeneratedInstanceCollectObjects(collectedObjects);
				collectedObjects.Add(this._selectedSettlement);
			}

			// Token: 0x060010E9 RID: 4329 RVA: 0x000717F3 File Offset: 0x0006F9F3
			internal static object AutoGeneratedGetMemberValue_selectedSettlement(object o)
			{
				return ((TheSpyPartyIssueQuestBehavior.TheSpyPartyIssue)o)._selectedSettlement;
			}

			// Token: 0x0400069E RID: 1694
			private const int QuestDurationInDays = 16;

			// Token: 0x0400069F RID: 1695
			private const int RequiredSkillValue = 150;

			// Token: 0x040006A0 RID: 1696
			private const int AlternativeSolutionTroopTierRequirement = 2;

			// Token: 0x040006A1 RID: 1697
			[SaveableField(10)]
			private readonly Settlement _selectedSettlement;
		}

		// Token: 0x02000164 RID: 356
		public class TheSpyPartyIssueQuest : QuestBase
		{
			// Token: 0x170001E1 RID: 481
			// (get) Token: 0x060010EA RID: 4330 RVA: 0x00071800 File Offset: 0x0006FA00
			public override bool IsRemainingTimeHidden
			{
				get
				{
					return false;
				}
			}

			// Token: 0x170001E2 RID: 482
			// (get) Token: 0x060010EB RID: 4331 RVA: 0x00071803 File Offset: 0x0006FA03
			public override TextObject Title
			{
				get
				{
					return new TextObject("{=SJHtVaNa}The Spy Among Us", null);
				}
			}

			// Token: 0x170001E3 RID: 483
			// (get) Token: 0x060010EC RID: 4332 RVA: 0x00071810 File Offset: 0x0006FA10
			private TextObject _questStartedLog
			{
				get
				{
					TextObject textObject = new TextObject("{=94WRYoQp}{?QUEST_GIVER.GENDER}Lady{?}Lord{\\?} {QUEST_GIVER.LINK} from {QUEST_SETTLEMENT}, has told you about rumors of a spy disguised amongst the tournament attendees. You agreed to take care of the situation by yourself. {QUEST_GIVER.LINK} believes that the spy is posing as an tournament attendee in the city of {QUEST_SETTLEMENT}.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					textObject.SetTextVariable("QUEST_SETTLEMENT", this._selectedSettlement.EncyclopediaLinkWithName);
					return textObject;
				}
			}

			// Token: 0x170001E4 RID: 484
			// (get) Token: 0x060010ED RID: 4333 RVA: 0x0007185C File Offset: 0x0006FA5C
			private TextObject _questSuccessQuestLog
			{
				get
				{
					TextObject textObject = new TextObject("{=YIxpNP4k}You received a message from {QUEST_GIVER.LINK}. \"Thank you for killing the spy. Please accept these {REWARD_GOLD}{GOLD_ICON} denars with our gratitude.\"", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					textObject.SetTextVariable("REWARD_GOLD", this.RewardGold);
					textObject.SetTextVariable("GOLD_ICON", "{=!}<img src=\"General\\Icons\\Coin@2x\" extend=\"8\">");
					return textObject;
				}
			}

			// Token: 0x170001E5 RID: 485
			// (get) Token: 0x060010EE RID: 4334 RVA: 0x000718B4 File Offset: 0x0006FAB4
			private TextObject _questFailedKilledAnotherQuestLog
			{
				get
				{
					TextObject textObject = new TextObject("{=tTKpOFRK}You won the duel but your opponent was innocent. {QUEST_GIVER.LINK} is disappointed.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					return textObject;
				}
			}

			// Token: 0x170001E6 RID: 486
			// (get) Token: 0x060010EF RID: 4335 RVA: 0x000718E8 File Offset: 0x0006FAE8
			private TextObject _playerFoundTheSpyButLostTheFight
			{
				get
				{
					TextObject textObject = new TextObject("{=hJ1SFkmq}You managed to find the spy but lost the duel. {QUEST_GIVER.LINK} is disappointed.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					return textObject;
				}
			}

			// Token: 0x170001E7 RID: 487
			// (get) Token: 0x060010F0 RID: 4336 RVA: 0x0007191C File Offset: 0x0006FB1C
			private TextObject _playerCouldNotFoundTheSpyAndLostTheFight
			{
				get
				{
					TextObject textObject = new TextObject("{=dOdp1aKA}You couldn't find the spy and dueled the wrong warrior. {QUEST_GIVER.LINK} is disappointed.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					return textObject;
				}
			}

			// Token: 0x170001E8 RID: 488
			// (get) Token: 0x060010F1 RID: 4337 RVA: 0x00071950 File Offset: 0x0006FB50
			private TextObject _timedOutQuestLog
			{
				get
				{
					TextObject textObject = new TextObject("{=0dlDkkJV}You have failed to find the spy. {QUEST_GIVER.LINK} is disappointed.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					return textObject;
				}
			}

			// Token: 0x170001E9 RID: 489
			// (get) Token: 0x060010F2 RID: 4338 RVA: 0x00071984 File Offset: 0x0006FB84
			private TextObject _questGiverLostOwnershipQuestLog
			{
				get
				{
					TextObject textObject = new TextObject("{=2OmrHVjp}{QUEST_GIVER.LINK} has lost the ownership of {QUEST_SETTLEMENT}. Your contract with {?QUEST_GIVER.GENDER}her{?}him{\\?} has canceled.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					textObject.SetTextVariable("QUEST_SETTLEMENT", this._selectedSettlement.EncyclopediaLinkWithName);
					return textObject;
				}
			}

			// Token: 0x170001EA RID: 490
			// (get) Token: 0x060010F3 RID: 4339 RVA: 0x000719CD File Offset: 0x0006FBCD
			private TextObject _warDeclaredQuestLog
			{
				get
				{
					TextObject textObject = new TextObject("{=cKz1cyuM}Your clan is now at war with {QUEST_GIVER_SETTLEMENT_FACTION}. Quest is canceled.", null);
					textObject.SetTextVariable("QUEST_GIVER_SETTLEMENT_FACTION", base.QuestGiver.MapFaction.Name);
					return textObject;
				}
			}

			// Token: 0x170001EB RID: 491
			// (get) Token: 0x060010F4 RID: 4340 RVA: 0x000719F8 File Offset: 0x0006FBF8
			private TextObject _playerDeclaredWarQuestLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=bqeWVVEE}Your actions have started a war with {QUEST_GIVER.LINK}'s faction. {?QUEST_GIVER.GENDER}She{?}He{\\?} cancels your agreement and the quest is a failure.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					return textObject;
				}
			}

			// Token: 0x060010F5 RID: 4341 RVA: 0x00071A2C File Offset: 0x0006FC2C
			public TheSpyPartyIssueQuest(string questId, Hero questGiver, CampaignTime duration, int rewardGold, Settlement selectedSettlement, float issueDifficultyMultiplier)
				: base(questId, questGiver, duration, rewardGold)
			{
				this._selectedSettlement = selectedSettlement;
				this._alreadySpokenAgents = new List<Agent>();
				this._issueDifficultyMultiplier = issueDifficultyMultiplier;
				this._giveClueChange = 0.1f;
				this.SetDialogs();
				base.InitializeQuestOnCreation();
				this.InitializeSuspectNpcs();
				this._selectedSpy = Extensions.GetRandomElement<TheSpyPartyIssueQuestBehavior.SuspectNpc>(this._suspectList);
				if (!base.IsTracked(this._selectedSettlement))
				{
					base.AddTrackedObject(this._selectedSettlement);
				}
			}

			// Token: 0x060010F6 RID: 4342 RVA: 0x00071AA8 File Offset: 0x0006FCA8
			private void InitializeSuspectNpcs()
			{
				this._suspectList = new MBList<TheSpyPartyIssueQuestBehavior.SuspectNpc>();
				this._currentDifficultySuffix = TheSpyPartyIssueQuestBehavior.GetDifficultySuffix(this._issueDifficultyMultiplier);
				this._suspectList.Add(new TheSpyPartyIssueQuestBehavior.SuspectNpc(MBObjectManager.Instance.GetObject<CharacterObject>("bold_contender_" + this._currentDifficultySuffix), false, true, true, true));
				this._suspectList.Add(new TheSpyPartyIssueQuestBehavior.SuspectNpc(MBObjectManager.Instance.GetObject<CharacterObject>("confident_contender_" + this._currentDifficultySuffix), true, false, true, true));
				this._suspectList.Add(new TheSpyPartyIssueQuestBehavior.SuspectNpc(MBObjectManager.Instance.GetObject<CharacterObject>("dignified_contender_" + this._currentDifficultySuffix), true, true, false, true));
				this._suspectList.Add(new TheSpyPartyIssueQuestBehavior.SuspectNpc(MBObjectManager.Instance.GetObject<CharacterObject>("hardy_contender_" + this._currentDifficultySuffix), true, true, true, false));
			}

			// Token: 0x060010F7 RID: 4343 RVA: 0x00071B89 File Offset: 0x0006FD89
			protected override void InitializeQuestOnGameLoad()
			{
				this._alreadySpokenAgents = new List<Agent>();
				this._giveClueChange = 0.1f;
				this.InitializeSuspectNpcs();
				this.SetDialogs();
				if (Settlement.CurrentSettlement != null && Settlement.CurrentSettlement == this._selectedSettlement)
				{
					this._addSpyNpcsToSettlement = true;
				}
			}

			// Token: 0x060010F8 RID: 4344 RVA: 0x00071BC8 File Offset: 0x0006FDC8
			protected override void SetDialogs()
			{
				TextObject textObject = new TextObject("{=wql79Eta}Good! We understand the spy is going to {TARGET_SETTLEMENT}. If they're trying to gather information, they'll be wandering around the market asking questions in the guise of making small talk. Just go around talking to the townsfolk, and you should be able to figure out who it is.", null);
				textObject.SetTextVariable("TARGET_SETTLEMENT", this._selectedSettlement.EncyclopediaLinkWithName);
				TextObject textObject2 = new TextObject("{=aC0Fq6IE}Do not waste time, {PLAYER.NAME}. The spy probably won't linger any longer than he has to. Or she has to.", null);
				StringHelpers.SetCharacterProperties("PLAYER", CharacterObject.PlayerCharacter, textObject2, false);
				this.OfferDialogFlow = DialogFlow.CreateDialogFlow("issue_classic_quest_start", 100).NpcLine(textObject, null, null).Condition(() => Hero.OneToOneConversationHero == base.QuestGiver)
					.Consequence(new ConversationSentence.OnConsequenceDelegate(this.QuestAcceptedConsequences))
					.CloseDialog();
				this.DiscussDialogFlow = DialogFlow.CreateDialogFlow("quest_discuss", 100).NpcLine(new TextObject("{=yLRfb5zb}Any news? Have you managed to find him yet?", null), null, null).Condition(() => Hero.OneToOneConversationHero == base.QuestGiver)
					.BeginPlayerOptions()
					.PlayerOption(new TextObject("{=wErSpkjy}I'm still working on it.", null), null)
					.NpcLine(textObject2, null, null)
					.Consequence(new ConversationSentence.OnConsequenceDelegate(this.LeaveEncounter))
					.CloseDialog()
					.PlayerOption(new TextObject("{=I8raOMRH}Sorry. No progress yet.", null), null)
					.NpcLine(new TextObject("{=ajSm2FEU}I know spies are hard to catch but I tasked this to you for a reason. Do not let me down {PLAYER.NAME}.", null), null, null)
					.NpcLine(new TextObject("{=pW69nUp8}Finish this task before it is too late.", null), null, null)
					.Consequence(new ConversationSentence.OnConsequenceDelegate(this.LeaveEncounter))
					.CloseDialog()
					.EndPlayerOptions()
					.CloseDialog();
				Campaign.Current.ConversationManager.AddDialogFlow(this.GetTownsPeopleDialogFlow(), this);
				Campaign.Current.ConversationManager.AddDialogFlow(this.GetNotablesDialogFlow(), this);
				Campaign.Current.ConversationManager.AddDialogFlow(this.GetTradersDialogFlow(), this);
				Campaign.Current.ConversationManager.AddDialogFlow(this.GetSuspectsDialogFlow(), this);
			}

			// Token: 0x060010F9 RID: 4345 RVA: 0x00071D6D File Offset: 0x0006FF6D
			private void LeaveEncounter()
			{
				if (PlayerEncounter.Current != null)
				{
					PlayerEncounter.LeaveEncounter = true;
				}
			}

			// Token: 0x060010FA RID: 4346 RVA: 0x00071D7C File Offset: 0x0006FF7C
			private void QuestAcceptedConsequences()
			{
				base.StartQuest();
				base.AddLog(this._questStartedLog, false);
				if (Settlement.CurrentSettlement != null && Settlement.CurrentSettlement == this._selectedSettlement)
				{
					this._addSpyNpcsToSettlement = true;
				}
			}

			// Token: 0x060010FB RID: 4347 RVA: 0x00071DB0 File Offset: 0x0006FFB0
			private DialogFlow GetSuspectsDialogFlow()
			{
				return DialogFlow.CreateDialogFlow("start", 125).NpcLine(new TextObject("{=IqhGJ8Dy}Hello there friend. Are you here for the tournament.", null), null, null).Condition(() => this._suspectList.Any((TheSpyPartyIssueQuestBehavior.SuspectNpc x) => x.CharacterObject == CharacterObject.OneToOneConversationCharacter))
					.BeginPlayerOptions()
					.PlayerOption(new TextObject("{=SRa9NyP1}No, my friend. I am on a hunt.", null), null)
					.NpcLine(new TextObject("{=gYCSwLB2}Eh, what do you mean by that?[ib:closed][if:convo_annoyed]", null), null, null)
					.BeginPlayerOptions()
					.PlayerOption(new TextObject("{=oddzOnad}I'm hunting a spy. And now I have found him.", null), null)
					.NpcLine(new TextObject("{=MU8nbzwJ}You have nothing on me. If you try to take me anywhere I'll kill you, and it will be in self-defense.[ib:aggressive]", null), null, null)
					.PlayerLine(new TextObject("{=WDdlPUHw}Not if it's a duel. I challenge you. No true tournament fighter would refuse.", null), null)
					.NpcLine(new TextObject("{=Ll8q45h5}Hmf... Very well. I shall wipe out this insult with your blood.", null), null, null)
					.Consequence(delegate
					{
						Campaign.Current.ConversationManager.ConversationEndOneShot += this.StartFightWithSpy;
					})
					.CloseDialog()
					.PlayerOption(new TextObject("{=O5PDH9Bc}Nothing, nothing. Go on your way.", null), null)
					.CloseDialog()
					.EndPlayerOptions()
					.PlayerOption(new TextObject("{=O7j0uzcH}I should be on my way.", null), null)
					.CloseDialog()
					.EndPlayerOptions();
			}

			// Token: 0x060010FC RID: 4348 RVA: 0x00071EAC File Offset: 0x000700AC
			private void StartFightWithSpy()
			{
				this._playerManagedToFindSpy = this._selectedSpy.CharacterObject == CharacterObject.OneToOneConversationCharacter;
				this._duelCharacter = CharacterObject.OneToOneConversationCharacter;
				this._startFightWithSpy = true;
				Campaign.Current.GameMenuManager.NextLocation = LocationComplex.Current.GetLocationWithId("arena");
				Mission.Current.EndMission();
			}

			// Token: 0x060010FD RID: 4349 RVA: 0x00071F0C File Offset: 0x0007010C
			private DialogFlow GetNotablesDialogFlow()
			{
				TextObject textObject = new TextObject("{=0RTwaPBJ}I speak to many people. Of course, as I am loyal to {QUEST_GIVER.NAME}, I am always on the lookout for spies. But I've seen no one like this.", null);
				StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
				return DialogFlow.CreateDialogFlow("hero_main_options", 125).BeginPlayerOptions().PlayerOption(new TextObject("{=xPTxkzVM}I am looking for a spy. Have you seen any warriors in the tournament wandering about, asking too many suspicious questions?", null), null)
					.Condition(() => Settlement.CurrentSettlement == this._selectedSettlement && Hero.OneToOneConversationHero.IsNotable)
					.NpcLine(textObject, null, null)
					.CloseDialog()
					.EndPlayerOptions();
			}

			// Token: 0x060010FE RID: 4350 RVA: 0x00071F84 File Offset: 0x00070184
			private DialogFlow GetTradersDialogFlow()
			{
				return DialogFlow.CreateDialogFlow("weaponsmith_talk_player", 125).BeginPlayerOptions().PlayerOption(new TextObject("{=SHwlcdp3}I ask you because you are a trader here. Have you seen one of the warriors in the tournament walking around here, asking people a lot of suspicious questions?", null), null)
					.Condition(() => Settlement.CurrentSettlement == this._selectedSettlement && (CharacterObject.OneToOneConversationCharacter.Occupation == 12 || CharacterObject.OneToOneConversationCharacter.Occupation == 4 || CharacterObject.OneToOneConversationCharacter.Occupation == 28 || CharacterObject.OneToOneConversationCharacter.Occupation == 10))
					.NpcLine(new TextObject("{=ocoHNhNk}Hmm... I keep pretty busy with my own trade. I haven't heard anything like that.", null), null, null)
					.CloseDialog()
					.EndPlayerOptions();
			}

			// Token: 0x060010FF RID: 4351 RVA: 0x00071FE0 File Offset: 0x000701E0
			private DialogFlow GetTownsPeopleDialogFlow()
			{
				TextObject playerOption1 = new TextObject("{=A2oos2Uo}Listen to me. I'm on assignment from {QUEST_GIVER.NAME}. Have any strangers been around here, asking odd questions about the garrison?", null);
				StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, playerOption1, false);
				TextObject playerOption2 = new TextObject("{=RXhBl8e1}Act normal. Have any of the participants in the tournament come round, asking very odd questions?", null);
				TextObject playerOption3 = new TextObject("{=HF2GIpbI}Listen to me. Have any of the tournament participants spent long hours in the market and tavern? More than usual?", null);
				float dontGiveClueResponse = 0f;
				bool giveClue = false;
				return DialogFlow.CreateDialogFlow("town_or_village_player", 125).BeginPlayerOptions().PlayerOption(new TextObject("{=GtgGnMe1}{PLAYER_OPTION}", null), null)
					.Condition(delegate
					{
						if (Settlement.CurrentSettlement == this._selectedSettlement)
						{
							float randomFloat = MBRandom.RandomFloat;
							dontGiveClueResponse = MBRandom.RandomFloat;
							if (randomFloat < 0.33f)
							{
								MBTextManager.SetTextVariable("PLAYER_OPTION", playerOption1, false);
							}
							else if (randomFloat >= 0.33f && randomFloat <= 0.66f)
							{
								MBTextManager.SetTextVariable("PLAYER_OPTION", playerOption2, false);
							}
							else
							{
								MBTextManager.SetTextVariable("PLAYER_OPTION", playerOption3, false);
							}
							return true;
						}
						return false;
					})
					.Consequence(delegate
					{
						giveClue = this._giveClueChange >= MBRandom.RandomFloat;
						Campaign.Current.ConversationManager.ConversationEndOneShot += this.AddAgentToAlreadySpokenList;
					})
					.BeginNpcOptions()
					.NpcOption(new TextObject("{=8gmne3b9}Not to me sir, no. I did overhear someone talking to another merchant about such things. I remember him because he had this nasty looking sword by his side.", null), () => giveClue && this._selectedSpy.HasBigSword && !this._playerLearnedHasBigSword && this.CommonCondition(), null, null)
					.PlayerLine(new TextObject("{=VP6s1YFW}Many contenders have swords on their backs. Still this information might prove useful.", null), null)
					.Consequence(new ConversationSentence.OnConsequenceDelegate(this.PlayerLearnedSpyHasSword))
					.CloseDialog()
					.NpcOption(new TextObject("{=gHnMYU9n}Why yes... At the tavern last night... Cornered a drunk and kept pressing him for information about the gatehouse. Had a beard, that one did.", null), () => giveClue && this._selectedSpy.HasBeard && !this._playerLearnedHasBeard && this.CommonCondition(), null, null)
					.PlayerLine(new TextObject("{=QaAzicqA}Many men have beards. Still, that is something.", null), null)
					.Consequence(new ConversationSentence.OnConsequenceDelegate(this.PlayerLearnedSpyHasBeard))
					.CloseDialog()
					.NpcOption(new TextObject("{=DUVqJifX}Yeah. I've seen one like that around the arena, asking all matter of outlandish questions. Middle-aged, normal head of hair, that's really all I can remember though.", null), () => giveClue && this._selectedSpy.HasHair && !this._playerLearnedHasHair && this.CommonCondition(), null, null)
					.PlayerLine(new TextObject("{=JjtmptiD}More men have hair than not, but this is another tile in the mosaic.", null), null)
					.Consequence(new ConversationSentence.OnConsequenceDelegate(this.PlayerLearnedSpyHasHair))
					.CloseDialog()
					.NpcOption(new TextObject("{=tXpmCzoZ}Well, there was one warrior. A handsome young lad. Didn't have any of those scars that some fighters pick up in battle, nor any of those marks or tattoos or whatever that some of the hard cases like to show off.", null), () => giveClue && this._selectedSpy.WithoutMarkings && !this._playerLearnedHasNoMarkings && this.CommonCondition(), null, null)
					.PlayerLine(new TextObject("{=ZCbQvqqv}A face without scars and markings is usual for farmers and merchants but less so for warriors. This might be useful.", null), null)
					.Consequence(new ConversationSentence.OnConsequenceDelegate(this.PlayerLearnedSpyHasNoMarkings))
					.CloseDialog()
					.NpcOption(new TextObject("{=sfxfiWxl}{?PLAYER.GENDER}Madam{?}Sir{\\?}, people gossip. Everyone around here knows you've been asking those questions. Your quarry is going to slip away if you don't move quickly.", null), () => dontGiveClueResponse <= 0.2f, null, null)
					.PlayerLine(new TextObject("{=04gFKwY1}Well, if you see anyone like that, let me know.", null), null)
					.CloseDialog()
					.NpcOption(new TextObject("{=VWaNqkqJ}Can't say I've seen anyone around here like that, {?PLAYER.GENDER}madam{?}sir{\\?}.", null), () => dontGiveClueResponse > 0.2f && dontGiveClueResponse <= 0.4f, null, null)
					.PlayerLine(new TextObject("{=QbzsgawM}Okay, just keep your eyes open.", null), null)
					.CloseDialog()
					.NpcOption(new TextObject("{=ff5XEKPB}Afraid I can't recall anyone like that, {?PLAYER.GENDER}madam{?}sir{\\?}.", null), () => dontGiveClueResponse > 0.4f && dontGiveClueResponse <= 0.6f, null, null)
					.PlayerLine(new TextObject("{=ArseaKsm}Very well. Thanks for your time.", null), null)
					.CloseDialog()
					.NpcOption(new TextObject("{=C6EOT3yY}No, sorry. Haven't seen anything like that.", null), () => dontGiveClueResponse > 0.6f && dontGiveClueResponse <= 0.8f, null, null)
					.PlayerLine(new TextObject("{=3UX334MB}Hmm.. Very well. Sorry for interrupting.", null), null)
					.CloseDialog()
					.NpcOption(new TextObject("{=9DDWjL9Y}Hmm... Maybe, but I can't remember who. I didn't think it suspicious.", null), () => dontGiveClueResponse > 0.8f, null, null)
					.PlayerLine(new TextObject("{=QbzsgawM}Okay, just keep your eyes open.", null), null)
					.CloseDialog()
					.EndNpcOptions()
					.EndPlayerOptions();
			}

			// Token: 0x06001100 RID: 4352 RVA: 0x000722C7 File Offset: 0x000704C7
			private void AddAgentToAlreadySpokenList()
			{
				this._giveClueChange += 0.15f;
				this._alreadySpokenAgents.Add((Agent)MissionConversationLogic.Current.ConversationManager.ConversationAgents[0]);
			}

			// Token: 0x06001101 RID: 4353 RVA: 0x00072300 File Offset: 0x00070500
			private bool CommonCondition()
			{
				return Settlement.CurrentSettlement == this._selectedSettlement && !this._alreadySpokenAgents.Contains((Agent)MissionConversationLogic.Current.ConversationManager.ConversationAgents[0]) && CharacterObject.OneToOneConversationCharacter.Occupation == 8;
			}

			// Token: 0x06001102 RID: 4354 RVA: 0x00072350 File Offset: 0x00070550
			private void CheckIfPlayerLearnedEverything()
			{
				int num = 0;
				num = (this._playerLearnedHasBeard ? (num + 1) : num);
				num = (this._playerLearnedHasBigSword ? (num + 1) : num);
				num = (this._playerLearnedHasHair ? (num + 1) : num);
				num = (this._playerLearnedHasNoMarkings ? (num + 1) : num);
				if (num >= 3)
				{
					TextObject textObject = new TextObject("{=2LW2jWuG}You should now have enough evidence to identify the spy. You might be able to find the tournament participants hanging out in the alleys with local thugs. Find and speak with them.", null);
					base.AddLog(textObject, false);
				}
			}

			// Token: 0x06001103 RID: 4355 RVA: 0x000723BC File Offset: 0x000705BC
			private void PlayerLearnedSpyHasSword()
			{
				this._giveClueChange = 0f;
				this._playerLearnedHasBigSword = true;
				base.AddLog(new TextObject("{=awYMellZ}The spy is known to carry a sword.", null), false);
				this._alreadySpokenAgents.Add(MissionConversationLogic.Current.ConversationAgent);
				this.CheckIfPlayerLearnedEverything();
			}

			// Token: 0x06001104 RID: 4356 RVA: 0x0007240C File Offset: 0x0007060C
			private void PlayerLearnedSpyHasBeard()
			{
				this._giveClueChange = 0f;
				this._playerLearnedHasBeard = true;
				base.AddLog(new TextObject("{=5om6Wv1n}After questioning some folk in town, you learned that the spy has a beard.", null), false);
				this._alreadySpokenAgents.Add(MissionConversationLogic.Current.ConversationAgent);
				this.CheckIfPlayerLearnedEverything();
			}

			// Token: 0x06001105 RID: 4357 RVA: 0x0007245C File Offset: 0x0007065C
			private void PlayerLearnedSpyHasHair()
			{
				this._giveClueChange = 0f;
				this._playerLearnedHasHair = true;
				base.AddLog(new TextObject("{=PLgOm8tV}The townsfolk told you that the spy is not bald.", null), false);
				this._alreadySpokenAgents.Add(MissionConversationLogic.Current.ConversationAgent);
				this.CheckIfPlayerLearnedEverything();
			}

			// Token: 0x06001106 RID: 4358 RVA: 0x000724AC File Offset: 0x000706AC
			private void PlayerLearnedSpyHasNoMarkings()
			{
				this._giveClueChange = 0f;
				this._playerLearnedHasNoMarkings = true;
				base.AddLog(new TextObject("{=1ieLd5qq}The townsfolk told you that the spy has no distinctive scars or other facial markings.", null), false);
				this._alreadySpokenAgents.Add(MissionConversationLogic.Current.ConversationAgent);
				this.CheckIfPlayerLearnedEverything();
			}

			// Token: 0x06001107 RID: 4359 RVA: 0x000724FC File Offset: 0x000706FC
			protected override void RegisterEvents()
			{
				CampaignEvents.SettlementEntered.AddNonSerializedListener(this, new Action<MobileParty, Settlement, Hero>(this.OnSettlementEntered));
				CampaignEvents.OnSettlementLeftEvent.AddNonSerializedListener(this, new Action<MobileParty, Settlement>(this.OnSettlementLeft));
				CampaignEvents.AfterMissionStarted.AddNonSerializedListener(this, new Action<IMission>(this.OnMissionStarted));
				CampaignEvents.OnSettlementOwnerChangedEvent.AddNonSerializedListener(this, new Action<Settlement, bool, Hero, Hero, Hero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail>(this.OnSettlementOwnerChanged));
				CampaignEvents.WarDeclared.AddNonSerializedListener(this, new Action<IFaction, IFaction, DeclareWarAction.DeclareWarDetail>(this.OnWarDeclared));
				CampaignEvents.ClanChangedKingdom.AddNonSerializedListener(this, new Action<Clan, Kingdom, Kingdom, ChangeKingdomAction.ChangeKingdomActionDetail, bool>(this.OnClanChangedKingdom));
				CampaignEvents.BeforeGameMenuOpenedEvent.AddNonSerializedListener(this, new Action<MenuCallbackArgs>(this.BeforeGameMenuOpenedEvent));
				CampaignEvents.MapEventStarted.AddNonSerializedListener(this, new Action<MapEvent, PartyBase, PartyBase>(this.OnMapEventStarted));
			}

			// Token: 0x06001108 RID: 4360 RVA: 0x000725C1 File Offset: 0x000707C1
			private void OnMapEventStarted(MapEvent mapEvent, PartyBase attackerParty, PartyBase defenderParty)
			{
				if (QuestHelper.CheckMinorMajorCoercion(this, mapEvent, attackerParty))
				{
					QuestHelper.ApplyGenericMinorMajorCoercionConsequences(this, mapEvent);
				}
			}

			// Token: 0x06001109 RID: 4361 RVA: 0x000725D4 File Offset: 0x000707D4
			private void BeforeGameMenuOpenedEvent(MenuCallbackArgs args)
			{
				if (Settlement.CurrentSettlement == this._selectedSettlement && args.MenuContext.GameMenu.StringId == "town")
				{
					if (this._startFightWithSpy && Campaign.Current.GameMenuManager.NextLocation == LocationComplex.Current.GetLocationWithId("arena") && GameStateManager.Current.ActiveState is MapState)
					{
						this._startFightWithSpy = false;
						CampaignMission.OpenArenaDuelMission(LocationComplex.Current.GetLocationWithId("arena").GetSceneName(this._selectedSettlement.Town.GetWallLevel()), LocationComplex.Current.GetLocationWithId("arena"), this._duelCharacter, false, false, new Action<CharacterObject>(this.OnFightEnd), 225f);
						Campaign.Current.GameMenuManager.NextLocation = null;
					}
					if (this._checkForBattleResult)
					{
						if (this._playerWonTheFight)
						{
							if (this._playerManagedToFindSpy)
							{
								this.PlayerFoundTheSpyAndKilledHim();
								return;
							}
							this.PlayerCouldNotFoundTheSpyAndKilledAnotherSuspect();
							return;
						}
						else
						{
							if (this._playerManagedToFindSpy)
							{
								this.PlayerFoundTheSpyButLostTheFight();
								return;
							}
							this.PlayerCouldNotFoundTheSpyAndLostTheFight();
						}
					}
				}
			}

			// Token: 0x0600110A RID: 4362 RVA: 0x000726F0 File Offset: 0x000708F0
			private void PlayerFoundTheSpyAndKilledHim()
			{
				base.AddLog(this._questSuccessQuestLog, false);
				GainRenownAction.Apply(Hero.MainHero, 1f, false);
				GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, this.RewardGold, false);
				this.RelationshipChangeWithQuestGiver = 5;
				this._selectedSettlement.Prosperity += 5f;
				base.CompleteQuestWithSuccess();
			}

			// Token: 0x0600110B RID: 4363 RVA: 0x00072754 File Offset: 0x00070954
			private void PlayerCouldNotFoundTheSpyAndKilledAnotherSuspect()
			{
				base.AddLog(this._questFailedKilledAnotherQuestLog, false);
				ChangeCrimeRatingAction.Apply(base.QuestGiver.MapFaction, 10f, true);
				this.RelationshipChangeWithQuestGiver = -5;
				this._selectedSettlement.Town.Security -= 10f;
				this._selectedSettlement.Town.Loyalty -= 10f;
				base.CompleteQuestWithFail(null);
			}

			// Token: 0x0600110C RID: 4364 RVA: 0x000727CC File Offset: 0x000709CC
			private void PlayerFoundTheSpyButLostTheFight()
			{
				base.AddLog(this._playerFoundTheSpyButLostTheFight, false);
				this.RelationshipChangeWithQuestGiver = -5;
				this._selectedSettlement.Town.Security -= 10f;
				this._selectedSettlement.Town.Loyalty -= 10f;
				base.CompleteQuestWithFail(null);
			}

			// Token: 0x0600110D RID: 4365 RVA: 0x00072830 File Offset: 0x00070A30
			private void PlayerCouldNotFoundTheSpyAndLostTheFight()
			{
				base.AddLog(this._playerCouldNotFoundTheSpyAndLostTheFight, false);
				this.RelationshipChangeWithQuestGiver = -5;
				this._selectedSettlement.Town.Security -= 10f;
				this._selectedSettlement.Town.Loyalty -= 10f;
				base.CompleteQuestWithFail(null);
			}

			// Token: 0x0600110E RID: 4366 RVA: 0x00072892 File Offset: 0x00070A92
			private void OnFightEnd(CharacterObject winnerCharacterObject)
			{
				this._checkForBattleResult = true;
				this._playerWonTheFight = winnerCharacterObject == CharacterObject.PlayerCharacter;
			}

			// Token: 0x0600110F RID: 4367 RVA: 0x000728A9 File Offset: 0x00070AA9
			private void OnClanChangedKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail detail, bool showNotification = true)
			{
				if (base.QuestGiver.MapFaction.IsAtWarWith(Hero.MainHero.MapFaction))
				{
					base.CompleteQuestWithCancel(this._warDeclaredQuestLog);
				}
			}

			// Token: 0x06001110 RID: 4368 RVA: 0x000728D3 File Offset: 0x00070AD3
			private void OnWarDeclared(IFaction faction1, IFaction faction2, DeclareWarAction.DeclareWarDetail detail)
			{
				QuestHelper.CheckWarDeclarationAndFailOrCancelTheQuest(this, faction1, faction2, detail, this._playerDeclaredWarQuestLogText, this._warDeclaredQuestLog);
			}

			// Token: 0x06001111 RID: 4369 RVA: 0x000728EA File Offset: 0x00070AEA
			private void OnSettlementOwnerChanged(Settlement settlement, bool openToClaim, Hero newOwner, Hero oldOwner, Hero capturerHero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail detail)
			{
				if (settlement == this._selectedSettlement && oldOwner.Clan == base.QuestGiver.Clan)
				{
					base.AddLog(this._questGiverLostOwnershipQuestLog, false);
					base.CompleteQuestWithCancel(null);
				}
			}

			// Token: 0x06001112 RID: 4370 RVA: 0x0007291E File Offset: 0x00070B1E
			private void OnSettlementEntered(MobileParty party, Settlement settlement, Hero hero)
			{
				if (party != null && party.IsMainParty && settlement == this._selectedSettlement && hero == Hero.MainHero)
				{
					this._addSpyNpcsToSettlement = true;
				}
			}

			// Token: 0x06001113 RID: 4371 RVA: 0x00072943 File Offset: 0x00070B43
			public override GameMenuOption.IssueQuestFlags IsLocationTrackedByQuest(Location location)
			{
				if (PlayerEncounter.LocationEncounter.Settlement == this._selectedSettlement && location.StringId == "center")
				{
					return 2;
				}
				return 0;
			}

			// Token: 0x06001114 RID: 4372 RVA: 0x0007296C File Offset: 0x00070B6C
			private void OnSettlementLeft(MobileParty party, Settlement settlement)
			{
				if (party.IsMainParty && settlement == this._selectedSettlement)
				{
					this._addSpyNpcsToSettlement = false;
				}
			}

			// Token: 0x06001115 RID: 4373 RVA: 0x00072988 File Offset: 0x00070B88
			private void OnMissionStarted(IMission mission)
			{
				if (this._addSpyNpcsToSettlement)
				{
					Location locationWithId = Settlement.CurrentSettlement.LocationComplex.GetLocationWithId("center");
					if (locationWithId != null)
					{
						locationWithId.AddLocationCharacters(new CreateLocationCharacterDelegate(this.CreateBoldSpyLocationCharacter), Settlement.CurrentSettlement.Culture, 0, 1);
						locationWithId.AddLocationCharacters(new CreateLocationCharacterDelegate(this.CreateConfidentSpyLocationCharacter), Settlement.CurrentSettlement.Culture, 0, 1);
						locationWithId.AddLocationCharacters(new CreateLocationCharacterDelegate(this.CreateDignifiedSpyLocationCharacter), Settlement.CurrentSettlement.Culture, 0, 1);
						locationWithId.AddLocationCharacters(new CreateLocationCharacterDelegate(this.CreateHardySpyLocationCharacters), Settlement.CurrentSettlement.Culture, 0, 1);
					}
				}
			}

			// Token: 0x06001116 RID: 4374 RVA: 0x00072A30 File Offset: 0x00070C30
			private LocationCharacter CreateBoldSpyLocationCharacter(CultureObject culture, LocationCharacter.CharacterRelations relation)
			{
				CharacterObject @object = MBObjectManager.Instance.GetObject<CharacterObject>("bold_contender_" + this._currentDifficultySuffix);
				Monster monsterWithSuffix = FaceGen.GetMonsterWithSuffix(@object.Race, "_settlement");
				Tuple<string, Monster> tuple = new Tuple<string, Monster>(ActionSetCode.GenerateActionSetNameWithSuffix(monsterWithSuffix, @object.IsFemale, "_villain"), monsterWithSuffix);
				return new LocationCharacter(new AgentData(new SimpleAgentOrigin(@object, -1, null, default(UniqueTroopDescriptor))).Monster(tuple.Item2), new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddFixedCharacterBehaviors), "alley_1", true, relation, tuple.Item1, true, false, null, false, true, false);
			}

			// Token: 0x06001117 RID: 4375 RVA: 0x00072AD0 File Offset: 0x00070CD0
			private LocationCharacter CreateConfidentSpyLocationCharacter(CultureObject culture, LocationCharacter.CharacterRelations relation)
			{
				CharacterObject @object = MBObjectManager.Instance.GetObject<CharacterObject>("confident_contender_" + this._currentDifficultySuffix);
				Monster monsterWithSuffix = FaceGen.GetMonsterWithSuffix(@object.Race, "_settlement");
				Tuple<string, Monster> tuple = new Tuple<string, Monster>(ActionSetCode.GenerateActionSetNameWithSuffix(monsterWithSuffix, @object.IsFemale, "_villain"), monsterWithSuffix);
				return new LocationCharacter(new AgentData(new SimpleAgentOrigin(@object, -1, null, default(UniqueTroopDescriptor))).Monster(tuple.Item2), new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddFixedCharacterBehaviors), "alley_3", true, relation, tuple.Item1, true, false, null, false, true, false);
			}

			// Token: 0x06001118 RID: 4376 RVA: 0x00072B70 File Offset: 0x00070D70
			private LocationCharacter CreateDignifiedSpyLocationCharacter(CultureObject culture, LocationCharacter.CharacterRelations relation)
			{
				CharacterObject @object = MBObjectManager.Instance.GetObject<CharacterObject>("dignified_contender_" + this._currentDifficultySuffix);
				Monster monsterWithSuffix = FaceGen.GetMonsterWithSuffix(@object.Race, "_settlement");
				Tuple<string, Monster> tuple = new Tuple<string, Monster>(ActionSetCode.GenerateActionSetNameWithSuffix(monsterWithSuffix, @object.IsFemale, "_villain"), monsterWithSuffix);
				return new LocationCharacter(new AgentData(new SimpleAgentOrigin(@object, -1, null, default(UniqueTroopDescriptor))).Monster(tuple.Item2), new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddFixedCharacterBehaviors), "alley_3", true, relation, tuple.Item1, true, false, null, false, true, false);
			}

			// Token: 0x06001119 RID: 4377 RVA: 0x00072C10 File Offset: 0x00070E10
			private LocationCharacter CreateHardySpyLocationCharacters(CultureObject culture, LocationCharacter.CharacterRelations relation)
			{
				CharacterObject @object = MBObjectManager.Instance.GetObject<CharacterObject>("hardy_contender_" + this._currentDifficultySuffix);
				Monster monsterWithSuffix = FaceGen.GetMonsterWithSuffix(@object.Race, "_settlement");
				Tuple<string, Monster> tuple = new Tuple<string, Monster>(ActionSetCode.GenerateActionSetNameWithSuffix(monsterWithSuffix, @object.IsFemale, "_villain"), monsterWithSuffix);
				return new LocationCharacter(new AgentData(new SimpleAgentOrigin(@object, -1, null, default(UniqueTroopDescriptor))).Monster(tuple.Item2), new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddFixedCharacterBehaviors), "alley_2", true, relation, tuple.Item1, true, false, null, false, true, false);
			}

			// Token: 0x0600111A RID: 4378 RVA: 0x00072CB0 File Offset: 0x00070EB0
			protected override void OnTimedOut()
			{
				base.AddLog(this._timedOutQuestLog, false);
				base.QuestGiver.AddPower(-5f);
				this.RelationshipChangeWithQuestGiver = -5;
				this._selectedSettlement.Town.Security -= 10f;
				this._selectedSettlement.Town.Loyalty -= 10f;
			}

			// Token: 0x0600111B RID: 4379 RVA: 0x00072D1B File Offset: 0x00070F1B
			internal static void AutoGeneratedStaticCollectObjectsTheSpyPartyIssueQuest(object o, List<object> collectedObjects)
			{
				((TheSpyPartyIssueQuestBehavior.TheSpyPartyIssueQuest)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			// Token: 0x0600111C RID: 4380 RVA: 0x00072D29 File Offset: 0x00070F29
			protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				base.AutoGeneratedInstanceCollectObjects(collectedObjects);
				collectedObjects.Add(this._selectedSettlement);
				TheSpyPartyIssueQuestBehavior.SuspectNpc.AutoGeneratedStaticCollectObjectsSuspectNpc(this._selectedSpy, collectedObjects);
			}

			// Token: 0x0600111D RID: 4381 RVA: 0x00072D4F File Offset: 0x00070F4F
			internal static object AutoGeneratedGetMemberValue_selectedSettlement(object o)
			{
				return ((TheSpyPartyIssueQuestBehavior.TheSpyPartyIssueQuest)o)._selectedSettlement;
			}

			// Token: 0x0600111E RID: 4382 RVA: 0x00072D5C File Offset: 0x00070F5C
			internal static object AutoGeneratedGetMemberValue_selectedSpy(object o)
			{
				return ((TheSpyPartyIssueQuestBehavior.TheSpyPartyIssueQuest)o)._selectedSpy;
			}

			// Token: 0x0600111F RID: 4383 RVA: 0x00072D6E File Offset: 0x00070F6E
			internal static object AutoGeneratedGetMemberValue_playerLearnedHasHair(object o)
			{
				return ((TheSpyPartyIssueQuestBehavior.TheSpyPartyIssueQuest)o)._playerLearnedHasHair;
			}

			// Token: 0x06001120 RID: 4384 RVA: 0x00072D80 File Offset: 0x00070F80
			internal static object AutoGeneratedGetMemberValue_playerLearnedHasNoMarkings(object o)
			{
				return ((TheSpyPartyIssueQuestBehavior.TheSpyPartyIssueQuest)o)._playerLearnedHasNoMarkings;
			}

			// Token: 0x06001121 RID: 4385 RVA: 0x00072D92 File Offset: 0x00070F92
			internal static object AutoGeneratedGetMemberValue_playerLearnedHasBigSword(object o)
			{
				return ((TheSpyPartyIssueQuestBehavior.TheSpyPartyIssueQuest)o)._playerLearnedHasBigSword;
			}

			// Token: 0x06001122 RID: 4386 RVA: 0x00072DA4 File Offset: 0x00070FA4
			internal static object AutoGeneratedGetMemberValue_playerLearnedHasBeard(object o)
			{
				return ((TheSpyPartyIssueQuestBehavior.TheSpyPartyIssueQuest)o)._playerLearnedHasBeard;
			}

			// Token: 0x06001123 RID: 4387 RVA: 0x00072DB6 File Offset: 0x00070FB6
			internal static object AutoGeneratedGetMemberValue_issueDifficultyMultiplier(object o)
			{
				return ((TheSpyPartyIssueQuestBehavior.TheSpyPartyIssueQuest)o)._issueDifficultyMultiplier;
			}

			// Token: 0x06001124 RID: 4388 RVA: 0x00072DC8 File Offset: 0x00070FC8
			internal static object AutoGeneratedGetMemberValue_currentDifficultySuffix(object o)
			{
				return ((TheSpyPartyIssueQuestBehavior.TheSpyPartyIssueQuest)o)._currentDifficultySuffix;
			}

			// Token: 0x040006A2 RID: 1698
			public const float CustomAgentHealth = 225f;

			// Token: 0x040006A3 RID: 1699
			[SaveableField(10)]
			private Settlement _selectedSettlement;

			// Token: 0x040006A4 RID: 1700
			[SaveableField(20)]
			private TheSpyPartyIssueQuestBehavior.SuspectNpc _selectedSpy;

			// Token: 0x040006A5 RID: 1701
			private MBList<TheSpyPartyIssueQuestBehavior.SuspectNpc> _suspectList;

			// Token: 0x040006A6 RID: 1702
			private List<Agent> _alreadySpokenAgents;

			// Token: 0x040006A7 RID: 1703
			[SaveableField(30)]
			private bool _playerLearnedHasHair;

			// Token: 0x040006A8 RID: 1704
			[SaveableField(40)]
			private bool _playerLearnedHasNoMarkings;

			// Token: 0x040006A9 RID: 1705
			[SaveableField(50)]
			private bool _playerLearnedHasBigSword;

			// Token: 0x040006AA RID: 1706
			[SaveableField(60)]
			private bool _playerLearnedHasBeard;

			// Token: 0x040006AB RID: 1707
			private bool _playerWonTheFight;

			// Token: 0x040006AC RID: 1708
			private bool _addSpyNpcsToSettlement;

			// Token: 0x040006AD RID: 1709
			private bool _startFightWithSpy;

			// Token: 0x040006AE RID: 1710
			private bool _checkForBattleResult;

			// Token: 0x040006AF RID: 1711
			private bool _playerManagedToFindSpy;

			// Token: 0x040006B0 RID: 1712
			private float _giveClueChange;

			// Token: 0x040006B1 RID: 1713
			private CharacterObject _duelCharacter;

			// Token: 0x040006B2 RID: 1714
			[SaveableField(70)]
			private float _issueDifficultyMultiplier;

			// Token: 0x040006B3 RID: 1715
			[SaveableField(80)]
			private string _currentDifficultySuffix;
		}
	}
}
