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
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.Issues;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace SandBox.Issues
{
	// Token: 0x0200007D RID: 125
	public class ProdigalSonIssueBehavior : CampaignBehaviorBase
	{
		// Token: 0x06000555 RID: 1365 RVA: 0x00026321 File Offset: 0x00024521
		public override void RegisterEvents()
		{
			CampaignEvents.OnCheckForIssueEvent.AddNonSerializedListener(this, new Action<Hero>(this.CheckForIssue));
		}

		// Token: 0x06000556 RID: 1366 RVA: 0x0002633A File Offset: 0x0002453A
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x06000557 RID: 1367 RVA: 0x0002633C File Offset: 0x0002453C
		public void CheckForIssue(Hero hero)
		{
			Hero hero2;
			Hero hero3;
			if (this.ConditionsHold(hero, out hero2, out hero3))
			{
				Campaign.Current.IssueManager.AddPotentialIssueData(hero, new PotentialIssueData(new PotentialIssueData.StartIssueDelegate(this.OnStartIssue), typeof(ProdigalSonIssueBehavior.ProdigalSonIssue), 2, new Tuple<Hero, Hero>(hero2, hero3)));
				return;
			}
			Campaign.Current.IssueManager.AddPotentialIssueData(hero, new PotentialIssueData(typeof(ProdigalSonIssueBehavior.ProdigalSonIssue), 2));
		}

		// Token: 0x06000558 RID: 1368 RVA: 0x000263AC File Offset: 0x000245AC
		private bool ConditionsHoldForSettlement(Settlement settlement, Hero issueGiver)
		{
			if (settlement.IsTown && settlement != issueGiver.CurrentSettlement && settlement.OwnerClan != issueGiver.Clan && settlement.OwnerClan != Clan.PlayerClan)
			{
				if (settlement.HeroesWithoutParty.FirstOrDefault((Hero x) => x.CanHaveQuestsOrIssues() && x.IsGangLeader) != null)
				{
					return LinQuick.AnyQ<Location>(settlement.LocationComplex.GetListOfLocations(), (Location x) => x.CanBeReserved && !x.IsReserved);
				}
			}
			return false;
		}

		// Token: 0x06000559 RID: 1369 RVA: 0x00026448 File Offset: 0x00024648
		private bool ConditionsHold(Hero issueGiver, out Hero selectedHero, out Hero targetHero)
		{
			selectedHero = null;
			targetHero = null;
			if (issueGiver.IsLord && !issueGiver.IsPrisoner && issueGiver.Clan != Clan.PlayerClan && issueGiver.Age > 30f && issueGiver.GetTraitLevel(DefaultTraits.Mercy) <= 0 && (issueGiver.CurrentSettlement != null || issueGiver.PartyBelongedTo != null))
			{
				selectedHero = Extensions.GetRandomElementWithPredicate<Hero>(issueGiver.Clan.Lords, (Hero x) => x.IsActive && !x.IsFemale && x.Age < 35f && (int)x.Age + 10 <= (int)issueGiver.Age && !x.IsPrisoner && x.CanHaveQuestsOrIssues() && x.PartyBelongedTo == null && x.CurrentSettlement != null && x.GovernorOf == null && x.GetTraitLevel(DefaultTraits.Honor) + x.GetTraitLevel(DefaultTraits.Calculating) < 0);
				if (selectedHero != null)
				{
					IMapPoint currentSettlement = issueGiver.CurrentSettlement;
					IMapPoint mapPoint = currentSettlement ?? issueGiver.PartyBelongedTo;
					int num = 0;
					int num2 = -1;
					do
					{
						num2 = SettlementHelper.FindNextSettlementAroundMapPoint(mapPoint, 150f, num2);
						if (num2 >= 0 && this.ConditionsHoldForSettlement(Settlement.All[num2], issueGiver))
						{
							num++;
						}
					}
					while (num2 >= 0);
					if (num > 0)
					{
						int num3 = MBRandom.RandomInt(num);
						num2 = -1;
						for (;;)
						{
							num2 = SettlementHelper.FindNextSettlementAroundMapPoint(mapPoint, 150f, num2);
							if (num2 >= 0 && this.ConditionsHoldForSettlement(Settlement.All[num2], issueGiver))
							{
								num3--;
								if (num3 < 0)
								{
									break;
								}
							}
							if (num2 < 0)
							{
								goto IL_18E;
							}
						}
						targetHero = Settlement.All[num2].HeroesWithoutParty.FirstOrDefault((Hero x) => x.CanHaveQuestsOrIssues() && x.IsGangLeader);
					}
				}
			}
			IL_18E:
			return selectedHero != null && targetHero != null;
		}

		// Token: 0x0600055A RID: 1370 RVA: 0x000265F0 File Offset: 0x000247F0
		private IssueBase OnStartIssue(in PotentialIssueData pid, Hero issueOwner)
		{
			PotentialIssueData potentialIssueData = pid;
			Tuple<Hero, Hero> tuple = potentialIssueData.RelatedObject as Tuple<Hero, Hero>;
			return new ProdigalSonIssueBehavior.ProdigalSonIssue(issueOwner, tuple.Item1, tuple.Item2);
		}

		// Token: 0x040002A2 RID: 674
		private const IssueBase.IssueFrequency ProdigalSonIssueFrequency = 2;

		// Token: 0x040002A3 RID: 675
		private const int AgeLimitForSon = 35;

		// Token: 0x040002A4 RID: 676
		private const int AgeLimitForIssueOwner = 30;

		// Token: 0x040002A5 RID: 677
		private const int MinimumAgeDifference = 10;

		// Token: 0x02000153 RID: 339
		public class ProdigalSonIssueTypeDefiner : SaveableTypeDefiner
		{
			// Token: 0x06000EEC RID: 3820 RVA: 0x0006A39B File Offset: 0x0006859B
			public ProdigalSonIssueTypeDefiner()
				: base(345000)
			{
			}

			// Token: 0x06000EED RID: 3821 RVA: 0x0006A3A8 File Offset: 0x000685A8
			protected override void DefineClassTypes()
			{
				base.AddClassDefinition(typeof(ProdigalSonIssueBehavior.ProdigalSonIssue), 1, null);
				base.AddClassDefinition(typeof(ProdigalSonIssueBehavior.ProdigalSonIssueQuest), 2, null);
			}
		}

		// Token: 0x02000154 RID: 340
		public class ProdigalSonIssue : IssueBase
		{
			// Token: 0x17000140 RID: 320
			// (get) Token: 0x06000EEE RID: 3822 RVA: 0x0006A3CE File Offset: 0x000685CE
			public override IssueBase.AlternativeSolutionScaleFlag AlternativeSolutionScaleFlags
			{
				get
				{
					return 8;
				}
			}

			// Token: 0x17000141 RID: 321
			// (get) Token: 0x06000EEF RID: 3823 RVA: 0x0006A3D1 File Offset: 0x000685D1
			private Clan Clan
			{
				get
				{
					return base.IssueOwner.Clan;
				}
			}

			// Token: 0x17000142 RID: 322
			// (get) Token: 0x06000EF0 RID: 3824 RVA: 0x0006A3DE File Offset: 0x000685DE
			protected override int RewardGold
			{
				get
				{
					return 1200 + (int)(3000f * base.IssueDifficultyMultiplier);
				}
			}

			// Token: 0x17000143 RID: 323
			// (get) Token: 0x06000EF1 RID: 3825 RVA: 0x0006A3F4 File Offset: 0x000685F4
			public override TextObject IssueBriefByIssueGiver
			{
				get
				{
					TextObject textObject = new TextObject("{=5a6KlSXt}I have a problem. My young kinsman {PRODIGAL_SON.LINK} has gone to town to have fun, drinking, wenching and gambling. Many young men do that, but it seems he was a bit reckless. Now he sends news that he owes a large sum of money to {TARGET_HERO.LINK}, one of the local gang bosses in the city of {SETTLEMENT_LINK}. These ruffians are holding him as a “guest” in their house until someone pays his debt.", null);
					StringHelpers.SetCharacterProperties("PRODIGAL_SON", this._prodigalSon.CharacterObject, textObject, false);
					StringHelpers.SetCharacterProperties("TARGET_HERO", this._targetHero.CharacterObject, textObject, false);
					textObject.SetTextVariable("SETTLEMENT_LINK", this._targetSettlement.EncyclopediaLinkWithName);
					return textObject;
				}
			}

			// Token: 0x17000144 RID: 324
			// (get) Token: 0x06000EF2 RID: 3826 RVA: 0x0006A455 File Offset: 0x00068655
			public override TextObject IssueAcceptByPlayer
			{
				get
				{
					return new TextObject("{=YtS3cgto}What are you planning to do?", null);
				}
			}

			// Token: 0x17000145 RID: 325
			// (get) Token: 0x06000EF3 RID: 3827 RVA: 0x0006A462 File Offset: 0x00068662
			public override TextObject IssueQuestSolutionExplanationByIssueGiver
			{
				get
				{
					TextObject textObject = new TextObject("{=ZC1slXw1}I'm not inclined to pay the debt. I'm not going to reward this kind of lawlessness, when even the best families aren't safe. I've sent word to the lord of {SETTLEMENT_NAME} but I can't say I expect to hear back, what with the wars and all. I want someone to go there and free the lad. You could pay, I suppose, but I'd prefer it if you taught those bastards a lesson. I'll pay you either way but obviously you get to keep more if you use force.", null);
					textObject.SetTextVariable("SETTLEMENT_NAME", this._targetSettlement.EncyclopediaLinkWithName);
					return textObject;
				}
			}

			// Token: 0x17000146 RID: 326
			// (get) Token: 0x06000EF4 RID: 3828 RVA: 0x0006A486 File Offset: 0x00068686
			public override TextObject IssuePlayerResponseAfterAlternativeExplanation
			{
				get
				{
					return new TextObject("{=4zf1lg6L}I could go myself, or send a companion.", null);
				}
			}

			// Token: 0x17000147 RID: 327
			// (get) Token: 0x06000EF5 RID: 3829 RVA: 0x0006A493 File Offset: 0x00068693
			public override TextObject IssueAlternativeSolutionExplanationByIssueGiver
			{
				get
				{
					TextObject textObject = new TextObject("{=CWbAoGRu}Yes, I don't care how you solve it. Just solve it any way you like. I reckon {NEEDED_MEN_COUNT} led by someone who knows how to handle thugs could solve this in about {ALTERNATIVE_SOLUTION_DURATION} days. I'd send my own men but it could cause complications for us to go marching in wearing our clan colors in another lord's territory.", null);
					textObject.SetTextVariable("NEEDED_MEN_COUNT", base.GetTotalAlternativeSolutionNeededMenCount());
					textObject.SetTextVariable("ALTERNATIVE_SOLUTION_DURATION", base.GetTotalAlternativeSolutionDurationInDays());
					return textObject;
				}
			}

			// Token: 0x17000148 RID: 328
			// (get) Token: 0x06000EF6 RID: 3830 RVA: 0x0006A4C4 File Offset: 0x000686C4
			public override TextObject IssueQuestSolutionAcceptByPlayer
			{
				get
				{
					return new TextObject("{=aKbyJsho}I will free your kinsman myself.", null);
				}
			}

			// Token: 0x17000149 RID: 329
			// (get) Token: 0x06000EF7 RID: 3831 RVA: 0x0006A4D1 File Offset: 0x000686D1
			public override TextObject IssueAlternativeSolutionAcceptByPlayer
			{
				get
				{
					TextObject textObject = new TextObject("{=PuuVGOyM}I will send {NEEDED_MEN_COUNT} of my men with one of my lieutenants for {ALTERNATIVE_SOLUTION_DURATION} days to help you.", null);
					textObject.SetTextVariable("NEEDED_MEN_COUNT", base.GetTotalAlternativeSolutionNeededMenCount());
					textObject.SetTextVariable("ALTERNATIVE_SOLUTION_DURATION", base.GetTotalAlternativeSolutionDurationInDays());
					return textObject;
				}
			}

			// Token: 0x1700014A RID: 330
			// (get) Token: 0x06000EF8 RID: 3832 RVA: 0x0006A502 File Offset: 0x00068702
			public override TextObject IssueDiscussAlternativeSolution
			{
				get
				{
					return new TextObject("{=qxhMagyZ}I'm glad someone's on it. Just see that they do it quickly.", null);
				}
			}

			// Token: 0x1700014B RID: 331
			// (get) Token: 0x06000EF9 RID: 3833 RVA: 0x0006A50F File Offset: 0x0006870F
			public override TextObject IssueAlternativeSolutionResponseByIssueGiver
			{
				get
				{
					return new TextObject("{=mDXzDXKY}Very good. I'm sure you'll chose competent men to bring our boy back.", null);
				}
			}

			// Token: 0x1700014C RID: 332
			// (get) Token: 0x06000EFA RID: 3834 RVA: 0x0006A51C File Offset: 0x0006871C
			protected override TextObject AlternativeSolutionStartLog
			{
				get
				{
					TextObject textObject = new TextObject("{=Z9sp21rl}{QUEST_GIVER.LINK}, a lord from the {QUEST_GIVER_CLAN} clan, asked you to free {?QUEST_GIVER.GENDER}her{?}his{\\?} relative. The young man is currently held by {TARGET_HERO.LINK} a local gang leader because of his debts. {?QUEST_GIVER.GENDER}Lady{?}Lord{\\?} {QUEST_GIVER.LINK} has given you enough gold to settle {?QUEST_GIVER.GENDER}her{?}his{\\?} debts but {?QUEST_GIVER.GENDER}she{?}he{\\?} encourages you to keep the money to yourself and make an example of these criminals so no one would dare to hold a nobleman again. You have sent {COMPANION.LINK} and {NEEDED_MEN_COUNT} men to take care of the situation for you. They should be back in {ALTERNATIVE_SOLUTION_DURATION} days.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.IssueOwner.CharacterObject, textObject, false);
					StringHelpers.SetCharacterProperties("TARGET_HERO", this._targetHero.CharacterObject, textObject, false);
					StringHelpers.SetCharacterProperties("COMPANION", base.AlternativeSolutionHero.CharacterObject, textObject, false);
					textObject.SetTextVariable("QUEST_GIVER_CLAN", base.IssueOwner.Clan.EncyclopediaLinkWithName);
					textObject.SetTextVariable("SETTLEMENT", this._targetSettlement.EncyclopediaLinkWithName);
					textObject.SetTextVariable("NEEDED_MEN_COUNT", base.GetTotalAlternativeSolutionNeededMenCount());
					textObject.SetTextVariable("ALTERNATIVE_SOLUTION_DURATION", base.GetTotalAlternativeSolutionDurationInDays());
					return textObject;
				}
			}

			// Token: 0x1700014D RID: 333
			// (get) Token: 0x06000EFB RID: 3835 RVA: 0x0006A5D8 File Offset: 0x000687D8
			public override TextObject IssueAlternativeSolutionSuccessLog
			{
				get
				{
					TextObject textObject = new TextObject("{=IXnvQ8kG}{COMPANION.LINK} and the men you sent with {?COMPANION.GENDER}her{?}him{\\?} safely return with the news of success. {QUEST_GIVER.LINK} is happy and sends you {?QUEST_GIVER.GENDER}her{?}his{\\?} regards with {REWARD}{GOLD_ICON} the money he promised.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.IssueOwner.CharacterObject, textObject, false);
					StringHelpers.SetCharacterProperties("COMPANION", base.AlternativeSolutionHero.CharacterObject, textObject, false);
					textObject.SetTextVariable("REWARD", this.RewardGold);
					return textObject;
				}
			}

			// Token: 0x1700014E RID: 334
			// (get) Token: 0x06000EFC RID: 3836 RVA: 0x0006A634 File Offset: 0x00068834
			public override bool IsThereAlternativeSolution
			{
				get
				{
					return true;
				}
			}

			// Token: 0x1700014F RID: 335
			// (get) Token: 0x06000EFD RID: 3837 RVA: 0x0006A637 File Offset: 0x00068837
			public override int AlternativeSolutionBaseNeededMenCount
			{
				get
				{
					return 1 + MathF.Ceiling(3f * base.IssueDifficultyMultiplier);
				}
			}

			// Token: 0x17000150 RID: 336
			// (get) Token: 0x06000EFE RID: 3838 RVA: 0x0006A64C File Offset: 0x0006884C
			protected override int AlternativeSolutionBaseDurationInDaysInternal
			{
				get
				{
					return 7 + MathF.Ceiling(7f * base.IssueDifficultyMultiplier);
				}
			}

			// Token: 0x17000151 RID: 337
			// (get) Token: 0x06000EFF RID: 3839 RVA: 0x0006A661 File Offset: 0x00068861
			protected override int CompanionSkillRewardXP
			{
				get
				{
					return (int)(700f + 900f * base.IssueDifficultyMultiplier);
				}
			}

			// Token: 0x17000152 RID: 338
			// (get) Token: 0x06000F00 RID: 3840 RVA: 0x0006A676 File Offset: 0x00068876
			public override bool IsThereLordSolution
			{
				get
				{
					return false;
				}
			}

			// Token: 0x17000153 RID: 339
			// (get) Token: 0x06000F01 RID: 3841 RVA: 0x0006A679 File Offset: 0x00068879
			public override TextObject Title
			{
				get
				{
					TextObject textObject = new TextObject("{=Mr2rt8g8}Prodigal son of {CLAN_NAME}", null);
					textObject.SetTextVariable("CLAN_NAME", this.Clan.Name);
					return textObject;
				}
			}

			// Token: 0x17000154 RID: 340
			// (get) Token: 0x06000F02 RID: 3842 RVA: 0x0006A6A0 File Offset: 0x000688A0
			public override TextObject Description
			{
				get
				{
					TextObject textObject = new TextObject("{=5puy0Jle}{ISSUE_OWNER.NAME} asks the player to aid a young clan member. He is supposed to have huge gambling debts so the gang leaders holds him as a hostage. You are asked to retrieve him any way possible.", null);
					StringHelpers.SetCharacterProperties("ISSUE_OWNER", base.IssueOwner.CharacterObject, textObject, false);
					return textObject;
				}
			}

			// Token: 0x06000F03 RID: 3843 RVA: 0x0006A6D4 File Offset: 0x000688D4
			public ProdigalSonIssue(Hero issueOwner, Hero prodigalSon, Hero targetGangHero)
				: base(issueOwner, CampaignTime.DaysFromNow(50f))
			{
				this._prodigalSon = prodigalSon;
				this._targetHero = targetGangHero;
				this._targetSettlement = this._targetHero.CurrentSettlement;
				this._targetHouse = this._targetSettlement.LocationComplex.GetListOfLocations().FirstOrDefault((Location x) => x.CanBeReserved && !x.IsReserved);
				TextObject textObject = new TextObject("{=EZ19JOGj}{MENTOR.NAME}'s House", null);
				StringHelpers.SetCharacterProperties("MENTOR", this._targetHero.CharacterObject, textObject, false);
				this._targetHouse.ReserveLocation(textObject, textObject);
				DisableHeroAction.Apply(this._prodigalSon);
			}

			// Token: 0x06000F04 RID: 3844 RVA: 0x0006A787 File Offset: 0x00068987
			public override void OnHeroCanHaveQuestOrIssueInfoIsRequested(Hero hero, ref bool result)
			{
				if (hero == this._targetHero || hero == this._prodigalSon)
				{
					result = false;
				}
			}

			// Token: 0x06000F05 RID: 3845 RVA: 0x0006A79E File Offset: 0x0006899E
			protected override float GetIssueEffectAmountInternal(IssueEffect issueEffect)
			{
				if (issueEffect == DefaultIssueEffects.IssueOwnerPower)
				{
					return -0.2f;
				}
				return 0f;
			}

			// Token: 0x06000F06 RID: 3846 RVA: 0x0006A7B3 File Offset: 0x000689B3
			public override ValueTuple<SkillObject, int> GetAlternativeSolutionSkill(Hero hero)
			{
				return new ValueTuple<SkillObject, int>((hero.GetSkillValue(DefaultSkills.Charm) >= hero.GetSkillValue(DefaultSkills.Roguery)) ? DefaultSkills.Charm : DefaultSkills.Roguery, 120);
			}

			// Token: 0x06000F07 RID: 3847 RVA: 0x0006A7E0 File Offset: 0x000689E0
			protected override void OnGameLoad()
			{
				Town town = Town.AllTowns.FirstOrDefault((Town x) => x.Settlement.LocationComplex.GetListOfLocations().Contains(this._targetHouse));
				if (town != null)
				{
					this._targetSettlement = town.Settlement;
				}
			}

			// Token: 0x06000F08 RID: 3848 RVA: 0x0006A813 File Offset: 0x00068A13
			protected override QuestBase GenerateIssueQuest(string questId)
			{
				return new ProdigalSonIssueBehavior.ProdigalSonIssueQuest(questId, base.IssueOwner, this._targetHero, this._prodigalSon, this._targetHouse, base.IssueDifficultyMultiplier, CampaignTime.DaysFromNow(24f), this.RewardGold);
			}

			// Token: 0x06000F09 RID: 3849 RVA: 0x0006A849 File Offset: 0x00068A49
			public override IssueBase.IssueFrequency GetFrequency()
			{
				return 2;
			}

			// Token: 0x06000F0A RID: 3850 RVA: 0x0006A84C File Offset: 0x00068A4C
			protected override bool CanPlayerTakeQuestConditions(Hero issueGiver, out IssueBase.PreconditionFlags flag, out Hero relationHero, out SkillObject skill)
			{
				bool flag2 = issueGiver.GetRelationWithPlayer() >= -10f && !issueGiver.MapFaction.IsAtWarWith(Hero.MainHero.MapFaction) && Clan.PlayerClan.Tier >= 1;
				flag = (flag2 ? 0 : ((!issueGiver.MapFaction.IsAtWarWith(Hero.MainHero.MapFaction)) ? ((Clan.PlayerClan.Tier >= 1) ? 1 : 128) : 64));
				relationHero = issueGiver;
				skill = null;
				return flag2;
			}

			// Token: 0x06000F0B RID: 3851 RVA: 0x0006A8D1 File Offset: 0x00068AD1
			public override bool IssueStayAliveConditions()
			{
				return this._targetHero.IsActive;
			}

			// Token: 0x06000F0C RID: 3852 RVA: 0x0006A8DE File Offset: 0x00068ADE
			protected override void CompleteIssueWithTimedOutConsequences()
			{
			}

			// Token: 0x06000F0D RID: 3853 RVA: 0x0006A8E0 File Offset: 0x00068AE0
			public override bool DoTroopsSatisfyAlternativeSolution(TroopRoster troopRoster, out TextObject explanation)
			{
				explanation = TextObject.Empty;
				return QuestHelper.CheckRosterForAlternativeSolution(troopRoster, base.GetTotalAlternativeSolutionNeededMenCount(), ref explanation, 2, false);
			}

			// Token: 0x06000F0E RID: 3854 RVA: 0x0006A8F8 File Offset: 0x00068AF8
			public override bool IsTroopTypeNeededByAlternativeSolution(CharacterObject character)
			{
				return character.Tier >= 2;
			}

			// Token: 0x06000F0F RID: 3855 RVA: 0x0006A906 File Offset: 0x00068B06
			public override bool AlternativeSolutionCondition(out TextObject explanation)
			{
				explanation = TextObject.Empty;
				return QuestHelper.CheckRosterForAlternativeSolution(MobileParty.MainParty.MemberRoster, base.GetTotalAlternativeSolutionNeededMenCount(), ref explanation, 2, false);
			}

			// Token: 0x06000F10 RID: 3856 RVA: 0x0006A927 File Offset: 0x00068B27
			protected override void AlternativeSolutionEndWithSuccessConsequence()
			{
				base.AlternativeSolutionHero.AddSkillXp(DefaultSkills.Charm, (float)((int)(700f + 900f * base.IssueDifficultyMultiplier)));
				this.RelationshipChangeWithIssueOwner = 5;
				GainRenownAction.Apply(Hero.MainHero, 3f, false);
			}

			// Token: 0x06000F11 RID: 3857 RVA: 0x0006A964 File Offset: 0x00068B64
			protected override void AlternativeSolutionEndWithFailureConsequence()
			{
				this.RelationshipChangeWithIssueOwner = -5;
			}

			// Token: 0x06000F12 RID: 3858 RVA: 0x0006A96E File Offset: 0x00068B6E
			protected override void OnIssueFinalized()
			{
				if (this._prodigalSon.HeroState == 6)
				{
					this._prodigalSon.ChangeState(4);
				}
			}

			// Token: 0x06000F13 RID: 3859 RVA: 0x0006A98A File Offset: 0x00068B8A
			internal static void AutoGeneratedStaticCollectObjectsProdigalSonIssue(object o, List<object> collectedObjects)
			{
				((ProdigalSonIssueBehavior.ProdigalSonIssue)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			// Token: 0x06000F14 RID: 3860 RVA: 0x0006A998 File Offset: 0x00068B98
			protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				base.AutoGeneratedInstanceCollectObjects(collectedObjects);
				collectedObjects.Add(this._prodigalSon);
				collectedObjects.Add(this._targetHero);
				collectedObjects.Add(this._targetHouse);
			}

			// Token: 0x06000F15 RID: 3861 RVA: 0x0006A9C5 File Offset: 0x00068BC5
			internal static object AutoGeneratedGetMemberValue_prodigalSon(object o)
			{
				return ((ProdigalSonIssueBehavior.ProdigalSonIssue)o)._prodigalSon;
			}

			// Token: 0x06000F16 RID: 3862 RVA: 0x0006A9D2 File Offset: 0x00068BD2
			internal static object AutoGeneratedGetMemberValue_targetHero(object o)
			{
				return ((ProdigalSonIssueBehavior.ProdigalSonIssue)o)._targetHero;
			}

			// Token: 0x06000F17 RID: 3863 RVA: 0x0006A9DF File Offset: 0x00068BDF
			internal static object AutoGeneratedGetMemberValue_targetHouse(object o)
			{
				return ((ProdigalSonIssueBehavior.ProdigalSonIssue)o)._targetHouse;
			}

			// Token: 0x04000638 RID: 1592
			private const int IssueDurationInDays = 50;

			// Token: 0x04000639 RID: 1593
			private const int QuestDurationInDays = 24;

			// Token: 0x0400063A RID: 1594
			private const int TroopTierForAlternativeSolution = 2;

			// Token: 0x0400063B RID: 1595
			private const int RequiredSkillValueForAlternativeSolution = 120;

			// Token: 0x0400063C RID: 1596
			[SaveableField(10)]
			private readonly Hero _prodigalSon;

			// Token: 0x0400063D RID: 1597
			[SaveableField(20)]
			private readonly Hero _targetHero;

			// Token: 0x0400063E RID: 1598
			[SaveableField(30)]
			private readonly Location _targetHouse;

			// Token: 0x0400063F RID: 1599
			private Settlement _targetSettlement;
		}

		// Token: 0x02000155 RID: 341
		public class ProdigalSonIssueQuest : QuestBase
		{
			// Token: 0x17000155 RID: 341
			// (get) Token: 0x06000F19 RID: 3865 RVA: 0x0006AA09 File Offset: 0x00068C09
			public override TextObject Title
			{
				get
				{
					TextObject textObject = new TextObject("{=7kqz1LlI}Prodigal son of {CLAN}", null);
					textObject.SetTextVariable("CLAN", base.QuestGiver.Clan.Name);
					return textObject;
				}
			}

			// Token: 0x17000156 RID: 342
			// (get) Token: 0x06000F1A RID: 3866 RVA: 0x0006AA32 File Offset: 0x00068C32
			public override bool IsRemainingTimeHidden
			{
				get
				{
					return false;
				}
			}

			// Token: 0x17000157 RID: 343
			// (get) Token: 0x06000F1B RID: 3867 RVA: 0x0006AA35 File Offset: 0x00068C35
			private Settlement Settlement
			{
				get
				{
					return this._targetHero.CurrentSettlement;
				}
			}

			// Token: 0x17000158 RID: 344
			// (get) Token: 0x06000F1C RID: 3868 RVA: 0x0006AA42 File Offset: 0x00068C42
			private int DebtWithInterest
			{
				get
				{
					return (int)((float)this.RewardGold * 1.1f);
				}
			}

			// Token: 0x17000159 RID: 345
			// (get) Token: 0x06000F1D RID: 3869 RVA: 0x0006AA54 File Offset: 0x00068C54
			private TextObject QuestStartedLog
			{
				get
				{
					TextObject textObject = new TextObject("{=CXw9a1i5}{QUEST_GIVER.LINK}, a {?QUEST_GIVER.GENDER}lady{?}lord{\\?} from the {QUEST_GIVER_CLAN} clan, asked you to go to {SETTLEMENT} to free {?QUEST_GIVER.GENDER}her{?}his{\\?} relative. The young man is currently held by {TARGET_HERO.LINK}, a local gang leader, because of his debts. {QUEST_GIVER.LINK} has suggested that you make an example of the gang so no one would dare to hold a nobleman again. {?QUEST_GIVER.GENDER}She{?}He{\\?} said you can easily find the house in which the young nobleman is held in the town square.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					StringHelpers.SetCharacterProperties("TARGET_HERO", this._targetHero.CharacterObject, textObject, false);
					textObject.SetTextVariable("QUEST_GIVER_CLAN", base.QuestGiver.Clan.EncyclopediaLinkWithName);
					textObject.SetTextVariable("SETTLEMENT", this.Settlement.EncyclopediaLinkWithName);
					return textObject;
				}
			}

			// Token: 0x1700015A RID: 346
			// (get) Token: 0x06000F1E RID: 3870 RVA: 0x0006AAD4 File Offset: 0x00068CD4
			private TextObject PlayerDefeatsThugsQuestSuccessLog
			{
				get
				{
					TextObject textObject = new TextObject("{=axLR9bQo}You have defeated the thugs that held {PRODIGAL_SON.LINK} as {QUEST_GIVER.LINK} has asked you to. {?QUEST_GIVER.GENDER}Lady{?}Lord{\\?} {QUEST_GIVER.LINK} soon sends {?QUEST_GIVER.GENDER}her{?}his{\\?} best regards and a sum of {REWARD}{GOLD_ICON} as a reward.", null);
					StringHelpers.SetCharacterProperties("PRODIGAL_SON", this._prodigalSon.CharacterObject, textObject, false);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					textObject.SetTextVariable("REWARD", this.RewardGold);
					return textObject;
				}
			}

			// Token: 0x1700015B RID: 347
			// (get) Token: 0x06000F1F RID: 3871 RVA: 0x0006AB30 File Offset: 0x00068D30
			private TextObject PlayerPaysTheDebtQuestSuccessLog
			{
				get
				{
					TextObject textObject = new TextObject("{=skMoB7c6}You have paid the debt that {PRODIGAL_SON.LINK} owes. True to {?TARGET_HERO.GENDER}her{?}his{\\?} word {TARGET_HERO.LINK} releases the boy immediately. Soon after, {?QUEST_GIVER.GENDER}Lady{?}Lord{\\?} {QUEST_GIVER.LINK} sends {?QUEST_GIVER.GENDER}her{?}his{\\?} best regards and a sum of {REWARD}{GOLD_ICON} as a reward.", null);
					StringHelpers.SetCharacterProperties("PRODIGAL_SON", this._prodigalSon.CharacterObject, textObject, false);
					StringHelpers.SetCharacterProperties("TARGET_HERO", this._targetHero.CharacterObject, textObject, false);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					textObject.SetTextVariable("REWARD", this.RewardGold);
					return textObject;
				}
			}

			// Token: 0x1700015C RID: 348
			// (get) Token: 0x06000F20 RID: 3872 RVA: 0x0006ABA4 File Offset: 0x00068DA4
			private TextObject QuestTimeOutFailLog
			{
				get
				{
					TextObject textObject = new TextObject("{=dmijPqWn}You have failed to extract {QUEST_GIVER.LINK}'s relative captive in time. They have moved the boy to a more secure place. Its impossible to find him now. {QUEST_GIVER.LINK} will have to deal with {TARGET_HERO.LINK} himself now. {?QUEST_GIVER.GENDER}She{?}He{\\?} won't be happy to hear this.", null);
					StringHelpers.SetCharacterProperties("TARGET_HERO", this._targetHero.CharacterObject, textObject, false);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					return textObject;
				}
			}

			// Token: 0x1700015D RID: 349
			// (get) Token: 0x06000F21 RID: 3873 RVA: 0x0006ABF0 File Offset: 0x00068DF0
			private TextObject PlayerHasDefeatedQuestFailLog
			{
				get
				{
					TextObject textObject = new TextObject("{=d5a8xQos}You have failed to defeat the thugs that keep {QUEST_GIVER.LINK}'s relative captive. After your assault you learn that they move the boy to a more secure place. Now its impossible to find him. {QUEST_GIVER.LINK} will have to deal with {TARGET_HERO.LINK} himself now. {?QUEST_GIVER.GENDER}She{?}He{\\?} won't be happy to hear this.", null);
					StringHelpers.SetCharacterProperties("TARGET_HERO", this._targetHero.CharacterObject, textObject, false);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					return textObject;
				}
			}

			// Token: 0x1700015E RID: 350
			// (get) Token: 0x06000F22 RID: 3874 RVA: 0x0006AC3C File Offset: 0x00068E3C
			private TextObject PlayerConvincesGangLeaderQuestSuccessLog
			{
				get
				{
					TextObject textObject = new TextObject("{=Rb7g1U2s}You have convinced {TARGET_HERO.LINK} to release {PRODIGAL_SON.LINK}. Soon after, {?QUEST_GIVER.GENDER}Lady{?}Lord{\\?} {QUEST_GIVER.LINK} sends {?QUEST_GIVER.GENDER}her{?}his{\\?} best regards and a sum of {REWARD}{GOLD_ICON} as a reward.", null);
					StringHelpers.SetCharacterProperties("PRODIGAL_SON", this._prodigalSon.CharacterObject, textObject, false);
					StringHelpers.SetCharacterProperties("TARGET_HERO", this._targetHero.CharacterObject, textObject, false);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					textObject.SetTextVariable("REWARD", this.RewardGold);
					return textObject;
				}
			}

			// Token: 0x1700015F RID: 351
			// (get) Token: 0x06000F23 RID: 3875 RVA: 0x0006ACB0 File Offset: 0x00068EB0
			private TextObject WarDeclaredQuestCancelLog
			{
				get
				{
					TextObject textObject = new TextObject("{=VuqZuSe2}Your clan is now at war with the {QUEST_GIVER.LINK}'s faction. Your agreement has been canceled.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					return textObject;
				}
			}

			// Token: 0x17000160 RID: 352
			// (get) Token: 0x06000F24 RID: 3876 RVA: 0x0006ACE4 File Offset: 0x00068EE4
			private TextObject PlayerDeclaredWarQuestLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=bqeWVVEE}Your actions have started a war with {QUEST_GIVER.LINK}'s faction. {?QUEST_GIVER.GENDER}She{?}He{\\?} cancels your agreement and the quest is a failure.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					return textObject;
				}
			}

			// Token: 0x17000161 RID: 353
			// (get) Token: 0x06000F25 RID: 3877 RVA: 0x0006AD16 File Offset: 0x00068F16
			private TextObject CrimeRatingCancelLog
			{
				get
				{
					TextObject textObject = new TextObject("{=oulvvl52}You are accused in {SETTLEMENT} of a crime, and {QUEST_GIVER.LINK} no longer trusts you in this matter.", null);
					TextObjectExtensions.SetCharacterProperties(textObject, "QUEST_GIVER", base.QuestGiver.CharacterObject, false);
					textObject.SetTextVariable("SETTLEMENT", this.Settlement.EncyclopediaLinkWithName);
					return textObject;
				}
			}

			// Token: 0x06000F26 RID: 3878 RVA: 0x0006AD51 File Offset: 0x00068F51
			public ProdigalSonIssueQuest(string questId, Hero questGiver, Hero targetHero, Hero prodigalSon, Location targetHouse, float questDifficulty, CampaignTime duration, int rewardGold)
				: base(questId, questGiver, duration, rewardGold)
			{
				this._targetHero = targetHero;
				this._prodigalSon = prodigalSon;
				this._targetHouse = targetHouse;
				this._questDifficulty = questDifficulty;
				this.SetDialogs();
				base.InitializeQuestOnCreation();
			}

			// Token: 0x06000F27 RID: 3879 RVA: 0x0006AD8C File Offset: 0x00068F8C
			protected override void SetDialogs()
			{
				TextObject textObject = new TextObject("{=bQnVtegC}Good, even better. You can find the house easily when you go to {SETTLEMENT} and walk around the town square. Or you could just speak to this gang leader, {TARGET_HERO.LINK}, and make {?TARGET_HERO.GENDER}her{?}him{\\?} understand and get my boy released. Good luck. I await good news.", null);
				StringHelpers.SetCharacterProperties("TARGET_HERO", this._targetHero.CharacterObject, textObject, false);
				Settlement settlement = ((this._targetHero.CurrentSettlement != null) ? this._targetHero.CurrentSettlement : this._targetHero.PartyBelongedTo.HomeSettlement);
				textObject.SetTextVariable("SETTLEMENT", settlement.EncyclopediaLinkWithName);
				this.OfferDialogFlow = DialogFlow.CreateDialogFlow("issue_classic_quest_start", 100).NpcLine(textObject, null, null).Condition(new ConversationSentence.OnConditionDelegate(this.is_talking_to_quest_giver))
					.Consequence(new ConversationSentence.OnConsequenceDelegate(this.QuestAcceptedConsequences))
					.CloseDialog();
				this.DiscussDialogFlow = DialogFlow.CreateDialogFlow("quest_discuss", 100).NpcLine(new TextObject("{=TkYk5yxn}Yes? Go already. Get our boy back.", null), null, null).Condition(new ConversationSentence.OnConditionDelegate(this.is_talking_to_quest_giver))
					.BeginPlayerOptions()
					.PlayerOption(new TextObject("{=kqXxvtwQ}Don't worry I'll free him.", null), null)
					.NpcLine(new TextObject("{=ddEu5IFQ}I hope so.", null), null, null)
					.Consequence(new ConversationSentence.OnConsequenceDelegate(MapEventHelper.OnConversationEnd))
					.CloseDialog()
					.PlayerOption(new TextObject("{=Jss9UqZC}I'll go right away", null), null)
					.NpcLine(new TextObject("{=IdKG3IaS}Good to hear that.", null), null, null)
					.Consequence(new ConversationSentence.OnConsequenceDelegate(MapEventHelper.OnConversationEnd))
					.CloseDialog()
					.EndPlayerOptions()
					.CloseDialog();
				Campaign.Current.ConversationManager.AddDialogFlow(this.GetTargetHeroDialogFlow(), this);
				Campaign.Current.ConversationManager.AddDialogFlow(this.GetProdigalSonDialogFlow(), this);
			}

			// Token: 0x06000F28 RID: 3880 RVA: 0x0006AF1D File Offset: 0x0006911D
			protected override void InitializeQuestOnGameLoad()
			{
				this.SetDialogs();
			}

			// Token: 0x06000F29 RID: 3881 RVA: 0x0006AF28 File Offset: 0x00069128
			protected override void RegisterEvents()
			{
				CampaignEvents.BeforeMissionOpenedEvent.AddNonSerializedListener(this, new Action(this.BeforeMissionOpened));
				CampaignEvents.MissionTickEvent.AddNonSerializedListener(this, new Action<float>(this.OnMissionTick));
				CampaignEvents.WarDeclared.AddNonSerializedListener(this, new Action<IFaction, IFaction, DeclareWarAction.DeclareWarDetail>(this.OnWarDeclared));
				CampaignEvents.HeroKilledEvent.AddNonSerializedListener(this, new Action<Hero, Hero, KillCharacterAction.KillCharacterActionDetail, bool>(this.OnHeroKilled));
				CampaignEvents.MapEventStarted.AddNonSerializedListener(this, new Action<MapEvent, PartyBase, PartyBase>(this.OnMapEventStarted));
				CampaignEvents.OnMissionStartedEvent.AddNonSerializedListener(this, new Action<IMission>(this.OnMissionStarted));
			}

			// Token: 0x06000F2A RID: 3882 RVA: 0x0006AFBF File Offset: 0x000691BF
			private void OnMissionStarted(IMission mission)
			{
				ICampaignMission campaignMission = CampaignMission.Current;
				if (((campaignMission != null) ? campaignMission.Location : null) == this._targetHouse)
				{
					this._isFirstMissionTick = true;
				}
			}

			// Token: 0x06000F2B RID: 3883 RVA: 0x0006AFE1 File Offset: 0x000691E1
			public override void OnHeroCanHaveQuestOrIssueInfoIsRequested(Hero hero, ref bool result)
			{
				if (hero == this._prodigalSon || hero == this._targetHero)
				{
					result = false;
				}
			}

			// Token: 0x06000F2C RID: 3884 RVA: 0x0006AFF8 File Offset: 0x000691F8
			public override void OnHeroCanMoveToSettlementInfoIsRequested(Hero hero, ref bool result)
			{
				if (hero == this._prodigalSon)
				{
					result = false;
				}
			}

			// Token: 0x06000F2D RID: 3885 RVA: 0x0006B006 File Offset: 0x00069206
			private void OnMapEventStarted(MapEvent mapEvent, PartyBase attackerParty, PartyBase defenderParty)
			{
				if (QuestHelper.CheckMinorMajorCoercion(this, mapEvent, attackerParty))
				{
					QuestHelper.ApplyGenericMinorMajorCoercionConsequences(this, mapEvent);
				}
			}

			// Token: 0x06000F2E RID: 3886 RVA: 0x0006B01C File Offset: 0x0006921C
			private void OnHeroKilled(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail detail, bool showNotification = true)
			{
				if (victim == this._targetHero || victim == this._prodigalSon)
				{
					TextObject textObject = ((detail == 7) ? this.TargetHeroDisappearedLogText : this.TargetHeroDiedLogText);
					StringHelpers.SetCharacterProperties("QUEST_TARGET", victim.CharacterObject, textObject, false);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					base.AddLog(textObject, false);
					base.CompleteQuestWithCancel(null);
				}
			}

			// Token: 0x06000F2F RID: 3887 RVA: 0x0006B089 File Offset: 0x00069289
			protected override void OnTimedOut()
			{
				this.FinishQuestFail1();
			}

			// Token: 0x06000F30 RID: 3888 RVA: 0x0006B091 File Offset: 0x00069291
			protected override void OnFinalize()
			{
				this._targetHouse.RemoveReservation();
			}

			// Token: 0x06000F31 RID: 3889 RVA: 0x0006B0A0 File Offset: 0x000692A0
			private void BeforeMissionOpened()
			{
				if (Settlement.CurrentSettlement == this.Settlement && LocationComplex.Current != null)
				{
					if (LocationComplex.Current.GetLocationOfCharacter(this._prodigalSon) == null)
					{
						this.SpawnProdigalSonInHouse();
						if (!this._isHouseFightFinished)
						{
							this.SpawnThugsInHouse();
							this._isMissionFightInitialized = false;
						}
					}
					using (List<AccompanyingCharacter>.Enumerator enumerator = PlayerEncounter.LocationEncounter.CharactersAccompanyingPlayer.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							AccompanyingCharacter character = enumerator.Current;
							if (!character.CanEnterLocation(this._targetHouse))
							{
								character.AllowEntranceToLocations((Location x) => character.CanEnterLocation(x) || x == this._targetHouse);
							}
						}
					}
				}
			}

			// Token: 0x06000F32 RID: 3890 RVA: 0x0006B174 File Offset: 0x00069374
			private void OnMissionTick(float dt)
			{
				if (CampaignMission.Current.Location == this._targetHouse)
				{
					Mission mission = Mission.Current;
					if (this._isFirstMissionTick)
					{
						Mission.Current.Agents.First((Agent x) => x.Character == this._prodigalSon.CharacterObject).GetComponent<CampaignAgentComponent>().AgentNavigator.RemoveBehaviorGroup<AlarmedBehaviorGroup>();
						this._isFirstMissionTick = false;
					}
					if (!this._isMissionFightInitialized && !this._isHouseFightFinished && mission.Agents.Count > 0)
					{
						this._isMissionFightInitialized = true;
						MissionFightHandler missionBehavior = mission.GetMissionBehavior<MissionFightHandler>();
						List<Agent> list = new List<Agent>();
						List<Agent> list2 = new List<Agent>();
						foreach (Agent agent in mission.Agents)
						{
							if (agent.IsEnemyOf(Agent.Main))
							{
								list.Add(agent);
							}
							else if (agent.Team == Agent.Main.Team)
							{
								list2.Add(agent);
							}
						}
						missionBehavior.StartCustomFight(list2, list, false, false, new MissionFightHandler.OnFightEndDelegate(this.HouseFightFinished));
						foreach (Agent agent2 in list)
						{
							agent2.Defensiveness = 2f;
						}
					}
				}
			}

			// Token: 0x06000F33 RID: 3891 RVA: 0x0006B2E0 File Offset: 0x000694E0
			private void OnWarDeclared(IFaction faction1, IFaction faction2, DeclareWarAction.DeclareWarDetail detail)
			{
				if (base.QuestGiver.MapFaction.IsAtWarWith(Hero.MainHero.MapFaction))
				{
					if (detail == 4)
					{
						this.RelationshipChangeWithQuestGiver = -5;
						Tuple<TraitObject, int>[] array = new Tuple<TraitObject, int>[]
						{
							new Tuple<TraitObject, int>(DefaultTraits.Honor, -50)
						};
						TraitLevelingHelper.OnIssueSolvedThroughQuest(Hero.MainHero, array);
					}
					if (DiplomacyHelper.IsWarCausedByPlayer(faction1, faction2, detail))
					{
						base.CompleteQuestWithFail(this.PlayerDeclaredWarQuestLogText);
						return;
					}
					base.CompleteQuestWithCancel((detail == 4) ? this.CrimeRatingCancelLog : this.WarDeclaredQuestCancelLog);
				}
			}

			// Token: 0x06000F34 RID: 3892 RVA: 0x0006B368 File Offset: 0x00069568
			private void HouseFightFinished(bool isPlayerSideWon)
			{
				if (isPlayerSideWon)
				{
					Agent agent = Mission.Current.Agents.First((Agent x) => x.Character == this._prodigalSon.CharacterObject);
					if (agent.Position.Distance(Agent.Main.Position) > agent.GetInteractionDistanceToUsable(Agent.Main))
					{
						ScriptBehavior.AddTargetWithDelegate(agent, new ScriptBehavior.SelectTargetDelegate(this.SelectPlayerAsTarget), new ScriptBehavior.OnTargetReachedDelegate(this.OnTargetReached));
					}
					else
					{
						Agent agent2 = null;
						UsableMachine usableMachine = null;
						WorldFrame invalid = WorldFrame.Invalid;
						this.OnTargetReached(agent, ref agent2, ref usableMachine, ref invalid);
					}
				}
				else
				{
					this.FinishQuestFail2();
				}
				this._isHouseFightFinished = true;
			}

			// Token: 0x06000F35 RID: 3893 RVA: 0x0006B401 File Offset: 0x00069601
			private bool OnTargetReached(Agent agent, ref Agent targetAgent, ref UsableMachine targetUsableMachine, ref WorldFrame targetFrame)
			{
				Mission.Current.GetMissionBehavior<MissionConversationLogic>().StartConversation(agent, false, false);
				targetAgent = null;
				return false;
			}

			// Token: 0x06000F36 RID: 3894 RVA: 0x0006B41C File Offset: 0x0006961C
			private bool SelectPlayerAsTarget(Agent agent, ref Agent targetAgent, ref UsableMachine targetUsableMachine, ref WorldFrame targetFrame)
			{
				targetAgent = null;
				if (agent.Position.Distance(Agent.Main.Position) > agent.GetInteractionDistanceToUsable(Agent.Main))
				{
					targetAgent = Agent.Main;
				}
				return targetAgent != null;
			}

			// Token: 0x06000F37 RID: 3895 RVA: 0x0006B460 File Offset: 0x00069660
			private void SpawnProdigalSonInHouse()
			{
				Monster monsterWithSuffix = FaceGen.GetMonsterWithSuffix(this._prodigalSon.CharacterObject.Race, "_settlement");
				LocationCharacter locationCharacter = new LocationCharacter(new AgentData(new SimpleAgentOrigin(this._prodigalSon.CharacterObject, -1, null, default(UniqueTroopDescriptor))).Monster(monsterWithSuffix), new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddWandererBehaviors), "npc_common", true, 0, null, true, false, null, false, false, true);
				this._targetHouse.AddCharacter(locationCharacter);
			}

			// Token: 0x06000F38 RID: 3896 RVA: 0x0006B4E4 File Offset: 0x000696E4
			private void SpawnThugsInHouse()
			{
				CharacterObject @object = MBObjectManager.Instance.GetObject<CharacterObject>("gangster_1");
				CharacterObject object2 = MBObjectManager.Instance.GetObject<CharacterObject>("gangster_2");
				CharacterObject object3 = MBObjectManager.Instance.GetObject<CharacterObject>("gangster_3");
				List<CharacterObject> list = new List<CharacterObject>();
				if (this._questDifficulty < 0.4f)
				{
					list.Add(@object);
					list.Add(@object);
					if (this._questDifficulty >= 0.2f)
					{
						list.Add(object2);
					}
				}
				else if (this._questDifficulty < 0.6f)
				{
					list.Add(@object);
					list.Add(object2);
					list.Add(object2);
				}
				else
				{
					list.Add(object2);
					list.Add(object3);
					list.Add(object3);
				}
				foreach (CharacterObject characterObject in list)
				{
					Monster monsterWithSuffix = FaceGen.GetMonsterWithSuffix(characterObject.Race, "_settlement");
					LocationCharacter locationCharacter = new LocationCharacter(new AgentData(new SimpleAgentOrigin(characterObject, -1, null, default(UniqueTroopDescriptor))).Monster(monsterWithSuffix), new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddWandererBehaviors), "npc_common", true, 2, null, true, false, null, false, false, true);
					this._targetHouse.AddCharacter(locationCharacter);
				}
			}

			// Token: 0x06000F39 RID: 3897 RVA: 0x0006B634 File Offset: 0x00069834
			private void QuestAcceptedConsequences()
			{
				base.StartQuest();
				base.AddTrackedObject(this.Settlement);
				base.AddTrackedObject(this._targetHero);
				base.AddLog(this.QuestStartedLog, false);
			}

			// Token: 0x06000F3A RID: 3898 RVA: 0x0006B664 File Offset: 0x00069864
			private DialogFlow GetProdigalSonDialogFlow()
			{
				return DialogFlow.CreateDialogFlow("start", 125).NpcLine("{=DYq30shK}Thank you, {?PLAYER.GENDER}milady{?}sir{\\?}.", null, null).Condition(() => Hero.OneToOneConversationHero == this._prodigalSon)
					.NpcLine("{=K8TSoRSD}Did {?QUEST_GIVER.GENDER}Lady{?}Lord{\\?} {QUEST_GIVER.LINK} send you to rescue me?", null, null)
					.Condition(delegate
					{
						StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, null, false);
						return true;
					})
					.PlayerLine("{=ln3bGyIO}Yes, I'm here to take you back.", null)
					.NpcLine("{=evIohG6b}Thank you, but there's no need. Once we are out of here I can manage to return on my own. I appreciate your efforts. I'll tell everyone in my clan of your heroism.", null, null)
					.NpcLine("{=qsJxhNGZ}Safe travels {?PLAYER.GENDER}milady{?}sir{\\?}.", null, null)
					.Consequence(delegate
					{
						Mission.Current.Agents.First((Agent x) => x.Character == this._prodigalSon.CharacterObject).GetComponent<CampaignAgentComponent>().AgentNavigator.GetBehaviorGroup<DailyBehaviorGroup>().DisableScriptedBehavior();
						Campaign.Current.ConversationManager.ConversationEndOneShot += this.OnEndHouseMissionDialog;
					})
					.CloseDialog();
			}

			// Token: 0x06000F3B RID: 3899 RVA: 0x0006B6F0 File Offset: 0x000698F0
			private DialogFlow GetTargetHeroDialogFlow()
			{
				DialogFlow dialogFlow = DialogFlow.CreateDialogFlow("start", 125).BeginNpcOptions().NpcOption(new TextObject("{=M0vxXQGB}Yes? Do you have something to say?", null), () => Hero.OneToOneConversationHero == this._targetHero && !this._playerTalkedToTargetHero, null, null)
					.Consequence(delegate
					{
						StringHelpers.SetCharacterProperties("PRODIGAL_SON", this._prodigalSon.CharacterObject, null, false);
						this._playerTalkedToTargetHero = true;
					})
					.PlayerLine("{=K5DgDU2a}I am here for the boy. {PRODIGAL_SON.LINK}. You know who I mean.", null)
					.GotoDialogState("start")
					.NpcOption(new TextObject("{=I979VDEn}Yes, did you bring {GOLD_AMOUNT}{GOLD_ICON}? That's what he owes... With an interest of course.", null), delegate
					{
						bool flag = Hero.OneToOneConversationHero == this._targetHero && this._playerTalkedToTargetHero;
						if (flag)
						{
							MBTextManager.SetTextVariable("GOLD_AMOUNT", this.DebtWithInterest);
						}
						return flag;
					}, null, null)
					.BeginPlayerOptions()
					.PlayerOption("{=IboStvbL}Here is the money, now release him!", null)
					.ClickableCondition(delegate(out TextObject explanation)
					{
						bool flag2 = false;
						if (Hero.MainHero.Gold >= this.DebtWithInterest)
						{
							explanation = TextObject.Empty;
							flag2 = true;
						}
						else
						{
							explanation = new TextObject("{=YuLLsAUb}You don't have {GOLD_AMOUNT}{GOLD_ICON}.", null);
							explanation.SetTextVariable("GOLD_AMOUNT", this.DebtWithInterest);
						}
						return flag2;
					})
					.NpcLine("{=7k03GxZ1}It's great doing business with you. I'll order my men to release him immediately.", null, null)
					.Consequence(new ConversationSentence.OnConsequenceDelegate(this.FinishQuestSuccess4))
					.CloseDialog()
					.PlayerOption("{=9pTkQ5o2}It would be in your interest to let this young nobleman go...", null)
					.Condition(() => !this._playerTriedToPersuade)
					.Consequence(delegate
					{
						this._playerTriedToPersuade = true;
						this._task = this.GetPersuasionTask();
						this.persuasion_start_on_consequence();
					})
					.GotoDialogState("persuade_gang_start_reservation")
					.PlayerOption("{=AwZhx2tT}I will be back.", null)
					.NpcLine("{=0fp67gxl}Have a good day.", null, null)
					.CloseDialog()
					.EndPlayerOptions()
					.EndNpcOptions();
				this.AddPersuasionDialogs(dialogFlow);
				return dialogFlow;
			}

			// Token: 0x06000F3C RID: 3900 RVA: 0x0006B81C File Offset: 0x00069A1C
			private void AddPersuasionDialogs(DialogFlow dialog)
			{
				dialog.AddDialogLine("persuade_gang_introduction", "persuade_gang_start_reservation", "persuade_gang_player_option", "{=EIsQnfLP}Tell me how it's in my interest...", new ConversationSentence.OnConditionDelegate(this.persuasion_start_on_condition), null, this, 100, null, null, null);
				dialog.AddDialogLine("persuade_gang_success", "persuade_gang_start_reservation", "close_window", "{=alruamIW}Hmm... You may be right. It's not worth it. I'll release the boy immediately.", new ConversationSentence.OnConditionDelegate(ConversationManager.GetPersuasionProgressSatisfied), new ConversationSentence.OnConsequenceDelegate(this.persuasion_success_on_consequence), this, int.MaxValue, null, null, null);
				dialog.AddDialogLine("persuade_gang_failed", "persuade_gang_start_reservation", "start", "{=1YGgXOB7}Meh... Do you think ruling the streets of a city is easy? You underestimate us. Now, about the money.", null, new ConversationSentence.OnConsequenceDelegate(ConversationManager.EndPersuasion), this, 100, null, null, null);
				string text = "persuade_gang_player_option_1";
				string text2 = "persuade_gang_player_option";
				string text3 = "persuade_gang_player_option_response";
				string text4 = "{=!}{PERSUADE_GANG_ATTEMPT_1}";
				ConversationSentence.OnConditionDelegate onConditionDelegate = new ConversationSentence.OnConditionDelegate(this.persuasion_select_option_1_on_condition);
				ConversationSentence.OnConsequenceDelegate onConsequenceDelegate = new ConversationSentence.OnConsequenceDelegate(this.persuasion_select_option_1_on_consequence);
				ConversationSentence.OnPersuasionOptionDelegate onPersuasionOptionDelegate = new ConversationSentence.OnPersuasionOptionDelegate(this.persuasion_setup_option_1);
				ConversationSentence.OnClickableConditionDelegate onClickableConditionDelegate = new ConversationSentence.OnClickableConditionDelegate(this.persuasion_clickable_option_1_on_condition);
				dialog.AddPlayerLine(text, text2, text3, text4, onConditionDelegate, onConsequenceDelegate, this, 100, onClickableConditionDelegate, onPersuasionOptionDelegate, null, null);
				string text5 = "persuade_gang_player_option_2";
				string text6 = "persuade_gang_player_option";
				string text7 = "persuade_gang_player_option_response";
				string text8 = "{=!}{PERSUADE_GANG_ATTEMPT_2}";
				ConversationSentence.OnConditionDelegate onConditionDelegate2 = new ConversationSentence.OnConditionDelegate(this.persuasion_select_option_2_on_condition);
				ConversationSentence.OnConsequenceDelegate onConsequenceDelegate2 = new ConversationSentence.OnConsequenceDelegate(this.persuasion_select_option_2_on_consequence);
				onPersuasionOptionDelegate = new ConversationSentence.OnPersuasionOptionDelegate(this.persuasion_setup_option_2);
				onClickableConditionDelegate = new ConversationSentence.OnClickableConditionDelegate(this.persuasion_clickable_option_2_on_condition);
				dialog.AddPlayerLine(text5, text6, text7, text8, onConditionDelegate2, onConsequenceDelegate2, this, 100, onClickableConditionDelegate, onPersuasionOptionDelegate, null, null);
				string text9 = "persuade_gang_player_option_3";
				string text10 = "persuade_gang_player_option";
				string text11 = "persuade_gang_player_option_response";
				string text12 = "{=!}{PERSUADE_GANG_ATTEMPT_3}";
				ConversationSentence.OnConditionDelegate onConditionDelegate3 = new ConversationSentence.OnConditionDelegate(this.persuasion_select_option_3_on_condition);
				ConversationSentence.OnConsequenceDelegate onConsequenceDelegate3 = new ConversationSentence.OnConsequenceDelegate(this.persuasion_select_option_3_on_consequence);
				onPersuasionOptionDelegate = new ConversationSentence.OnPersuasionOptionDelegate(this.persuasion_setup_option_3);
				onClickableConditionDelegate = new ConversationSentence.OnClickableConditionDelegate(this.persuasion_clickable_option_3_on_condition);
				dialog.AddPlayerLine(text9, text10, text11, text12, onConditionDelegate3, onConsequenceDelegate3, this, 100, onClickableConditionDelegate, onPersuasionOptionDelegate, null, null);
				dialog.AddDialogLine("persuade_gang_option_reaction", "persuade_gang_player_option_response", "persuade_gang_start_reservation", "{=!}{PERSUASION_REACTION}", new ConversationSentence.OnConditionDelegate(this.persuasion_selected_option_response_on_condition), new ConversationSentence.OnConsequenceDelegate(this.persuasion_selected_option_response_on_consequence), this, 100, null, null, null);
			}

			// Token: 0x06000F3D RID: 3901 RVA: 0x0006B9F6 File Offset: 0x00069BF6
			private bool is_talking_to_quest_giver()
			{
				return Hero.OneToOneConversationHero == base.QuestGiver;
			}

			// Token: 0x06000F3E RID: 3902 RVA: 0x0006BA08 File Offset: 0x00069C08
			private bool persuasion_start_on_condition()
			{
				if (Hero.OneToOneConversationHero == this._targetHero && !ConversationManager.GetPersuasionIsFailure())
				{
					return this._task.Options.Any((PersuasionOptionArgs x) => !x.IsBlocked);
				}
				return false;
			}

			// Token: 0x06000F3F RID: 3903 RVA: 0x0006BA5C File Offset: 0x00069C5C
			private void persuasion_selected_option_response_on_consequence()
			{
				Tuple<PersuasionOptionArgs, PersuasionOptionResult> tuple = ConversationManager.GetPersuasionChosenOptions().Last<Tuple<PersuasionOptionArgs, PersuasionOptionResult>>();
				float difficulty = Campaign.Current.Models.PersuasionModel.GetDifficulty(5);
				float num;
				float num2;
				Campaign.Current.Models.PersuasionModel.GetEffectChances(tuple.Item1, ref num, ref num2, difficulty);
				this._task.ApplyEffects(num, num2);
			}

			// Token: 0x06000F40 RID: 3904 RVA: 0x0006BAB8 File Offset: 0x00069CB8
			private bool persuasion_selected_option_response_on_condition()
			{
				PersuasionOptionResult item = ConversationManager.GetPersuasionChosenOptions().Last<Tuple<PersuasionOptionArgs, PersuasionOptionResult>>().Item2;
				MBTextManager.SetTextVariable("PERSUASION_REACTION", PersuasionHelper.GetDefaultPersuasionOptionReaction(item), false);
				if (item == null)
				{
					this._task.BlockAllOptions();
				}
				return true;
			}

			// Token: 0x06000F41 RID: 3905 RVA: 0x0006BAF8 File Offset: 0x00069CF8
			private bool persuasion_select_option_1_on_condition()
			{
				if (this._task.Options.Count > 0)
				{
					TextObject textObject = new TextObject("{=bSo9hKwr}{PERSUASION_OPTION_LINE} {SUCCESS_CHANCE}", null);
					textObject.SetTextVariable("SUCCESS_CHANCE", PersuasionHelper.ShowSuccess(this._task.Options.ElementAt(0), false));
					textObject.SetTextVariable("PERSUASION_OPTION_LINE", this._task.Options.ElementAt(0).Line);
					MBTextManager.SetTextVariable("PERSUADE_GANG_ATTEMPT_1", textObject, false);
					return true;
				}
				return false;
			}

			// Token: 0x06000F42 RID: 3906 RVA: 0x0006BB78 File Offset: 0x00069D78
			private bool persuasion_select_option_2_on_condition()
			{
				if (this._task.Options.Count > 1)
				{
					TextObject textObject = new TextObject("{=bSo9hKwr}{PERSUASION_OPTION_LINE} {SUCCESS_CHANCE}", null);
					textObject.SetTextVariable("SUCCESS_CHANCE", PersuasionHelper.ShowSuccess(this._task.Options.ElementAt(1), false));
					textObject.SetTextVariable("PERSUASION_OPTION_LINE", this._task.Options.ElementAt(1).Line);
					MBTextManager.SetTextVariable("PERSUADE_GANG_ATTEMPT_2", textObject, false);
					return true;
				}
				return false;
			}

			// Token: 0x06000F43 RID: 3907 RVA: 0x0006BBF8 File Offset: 0x00069DF8
			private bool persuasion_select_option_3_on_condition()
			{
				if (this._task.Options.Count > 2)
				{
					TextObject textObject = new TextObject("{=bSo9hKwr}{PERSUASION_OPTION_LINE} {SUCCESS_CHANCE}", null);
					textObject.SetTextVariable("SUCCESS_CHANCE", PersuasionHelper.ShowSuccess(this._task.Options.ElementAt(2), false));
					textObject.SetTextVariable("PERSUASION_OPTION_LINE", this._task.Options.ElementAt(2).Line);
					MBTextManager.SetTextVariable("PERSUADE_GANG_ATTEMPT_3", textObject, false);
					return true;
				}
				return false;
			}

			// Token: 0x06000F44 RID: 3908 RVA: 0x0006BC78 File Offset: 0x00069E78
			private void persuasion_select_option_1_on_consequence()
			{
				if (this._task.Options.Count > 0)
				{
					this._task.Options[0].BlockTheOption(true);
				}
			}

			// Token: 0x06000F45 RID: 3909 RVA: 0x0006BCA4 File Offset: 0x00069EA4
			private void persuasion_select_option_2_on_consequence()
			{
				if (this._task.Options.Count > 1)
				{
					this._task.Options[1].BlockTheOption(true);
				}
			}

			// Token: 0x06000F46 RID: 3910 RVA: 0x0006BCD0 File Offset: 0x00069ED0
			private void persuasion_select_option_3_on_consequence()
			{
				if (this._task.Options.Count > 2)
				{
					this._task.Options[2].BlockTheOption(true);
				}
			}

			// Token: 0x06000F47 RID: 3911 RVA: 0x0006BCFC File Offset: 0x00069EFC
			private PersuasionOptionArgs persuasion_setup_option_1()
			{
				return this._task.Options.ElementAt(0);
			}

			// Token: 0x06000F48 RID: 3912 RVA: 0x0006BD0F File Offset: 0x00069F0F
			private PersuasionOptionArgs persuasion_setup_option_2()
			{
				return this._task.Options.ElementAt(1);
			}

			// Token: 0x06000F49 RID: 3913 RVA: 0x0006BD22 File Offset: 0x00069F22
			private PersuasionOptionArgs persuasion_setup_option_3()
			{
				return this._task.Options.ElementAt(2);
			}

			// Token: 0x06000F4A RID: 3914 RVA: 0x0006BD38 File Offset: 0x00069F38
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

			// Token: 0x06000F4B RID: 3915 RVA: 0x0006BDA4 File Offset: 0x00069FA4
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

			// Token: 0x06000F4C RID: 3916 RVA: 0x0006BE10 File Offset: 0x0006A010
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

			// Token: 0x06000F4D RID: 3917 RVA: 0x0006BE7B File Offset: 0x0006A07B
			private void persuasion_success_on_consequence()
			{
				ConversationManager.EndPersuasion();
				this.FinishQuestSuccess3();
			}

			// Token: 0x06000F4E RID: 3918 RVA: 0x0006BE88 File Offset: 0x0006A088
			private void OnEndHouseMissionDialog()
			{
				Campaign.Current.GameMenuManager.NextLocation = LocationComplex.Current.GetLocationWithId("center");
				Campaign.Current.GameMenuManager.PreviousLocation = CampaignMission.Current.Location;
				Mission.Current.EndMission();
				this.FinishQuestSuccess1();
			}

			// Token: 0x06000F4F RID: 3919 RVA: 0x0006BEDC File Offset: 0x0006A0DC
			private PersuasionTask GetPersuasionTask()
			{
				PersuasionTask persuasionTask = new PersuasionTask(0);
				persuasionTask.FinalFailLine = TextObject.Empty;
				persuasionTask.TryLaterLine = TextObject.Empty;
				persuasionTask.SpokenLine = new TextObject("{=6P1ruzsC}Maybe...", null);
				PersuasionOptionArgs persuasionOptionArgs = new PersuasionOptionArgs(DefaultSkills.Charm, DefaultTraits.Calculating, 0, -3, true, new TextObject("{=Lol4clzR}Look, it was a good try, but they're not going to pay. Releasing the kid is the only move that makes sense.", null), null, false, false, false);
				persuasionTask.AddOptionToTask(persuasionOptionArgs);
				PersuasionOptionArgs persuasionOptionArgs2 = new PersuasionOptionArgs(DefaultSkills.Roguery, DefaultTraits.Mercy, 1, -1, false, new TextObject("{=wJCVlVF7}These nobles aren't like you and me. They've kept their wealth by crushing people like you for generations. Don't mess with them.", null), null, false, false, false);
				persuasionTask.AddOptionToTask(persuasionOptionArgs2);
				PersuasionOptionArgs persuasionOptionArgs3 = new PersuasionOptionArgs(DefaultSkills.Roguery, DefaultTraits.Generosity, 0, 0, false, new TextObject("{=o1KOn4WZ}If you let this boy go, his family will remember you did them a favor. That's a better deal for you than a fight you can't hope to win.", null), null, false, false, false);
				persuasionTask.AddOptionToTask(persuasionOptionArgs3);
				return persuasionTask;
			}

			// Token: 0x06000F50 RID: 3920 RVA: 0x0006BF92 File Offset: 0x0006A192
			private void persuasion_start_on_consequence()
			{
				ConversationManager.StartPersuasion(2f, 1f, 1f, 2f, 2f, 0f, 5);
			}

			// Token: 0x06000F51 RID: 3921 RVA: 0x0006BFB8 File Offset: 0x0006A1B8
			private void FinishQuestSuccess1()
			{
				base.CompleteQuestWithSuccess();
				base.AddLog(this.PlayerDefeatsThugsQuestSuccessLog, false);
				ChangeRelationAction.ApplyPlayerRelation(base.QuestGiver, 5, true, true);
				GainRenownAction.Apply(Hero.MainHero, 3f, false);
				GiveGoldAction.ApplyForQuestBetweenCharacters(base.QuestGiver, Hero.MainHero, this.RewardGold, false);
			}

			// Token: 0x06000F52 RID: 3922 RVA: 0x0006C010 File Offset: 0x0006A210
			private void FinishQuestSuccess3()
			{
				base.CompleteQuestWithSuccess();
				base.AddLog(this.PlayerConvincesGangLeaderQuestSuccessLog, false);
				ChangeRelationAction.ApplyPlayerRelation(base.QuestGiver, 5, true, true);
				GainRenownAction.Apply(Hero.MainHero, 1f, false);
				GiveGoldAction.ApplyForQuestBetweenCharacters(base.QuestGiver, Hero.MainHero, this.RewardGold, false);
			}

			// Token: 0x06000F53 RID: 3923 RVA: 0x0006C068 File Offset: 0x0006A268
			private void FinishQuestSuccess4()
			{
				GainRenownAction.Apply(Hero.MainHero, 1f, false);
				GiveGoldAction.ApplyForQuestBetweenCharacters(Hero.MainHero, this._targetHero, this.DebtWithInterest, false);
				base.CompleteQuestWithSuccess();
				base.AddLog(this.PlayerPaysTheDebtQuestSuccessLog, false);
				ChangeRelationAction.ApplyPlayerRelation(base.QuestGiver, 5, true, true);
				GiveGoldAction.ApplyForQuestBetweenCharacters(base.QuestGiver, Hero.MainHero, this.RewardGold, false);
			}

			// Token: 0x06000F54 RID: 3924 RVA: 0x0006C0D5 File Offset: 0x0006A2D5
			private void FinishQuestFail1()
			{
				base.AddLog(this.QuestTimeOutFailLog, false);
				ChangeRelationAction.ApplyPlayerRelation(base.QuestGiver, -5, true, true);
			}

			// Token: 0x06000F55 RID: 3925 RVA: 0x0006C0F4 File Offset: 0x0006A2F4
			private void FinishQuestFail2()
			{
				base.CompleteQuestWithFail(null);
				base.AddLog(this.PlayerHasDefeatedQuestFailLog, false);
				ChangeRelationAction.ApplyPlayerRelation(base.QuestGiver, -5, true, true);
			}

			// Token: 0x06000F56 RID: 3926 RVA: 0x0006C11A File Offset: 0x0006A31A
			internal static void AutoGeneratedStaticCollectObjectsProdigalSonIssueQuest(object o, List<object> collectedObjects)
			{
				((ProdigalSonIssueBehavior.ProdigalSonIssueQuest)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			// Token: 0x06000F57 RID: 3927 RVA: 0x0006C128 File Offset: 0x0006A328
			protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				base.AutoGeneratedInstanceCollectObjects(collectedObjects);
				collectedObjects.Add(this._targetHero);
				collectedObjects.Add(this._prodigalSon);
				collectedObjects.Add(this._targetHouse);
			}

			// Token: 0x06000F58 RID: 3928 RVA: 0x0006C155 File Offset: 0x0006A355
			internal static object AutoGeneratedGetMemberValue_targetHero(object o)
			{
				return ((ProdigalSonIssueBehavior.ProdigalSonIssueQuest)o)._targetHero;
			}

			// Token: 0x06000F59 RID: 3929 RVA: 0x0006C162 File Offset: 0x0006A362
			internal static object AutoGeneratedGetMemberValue_prodigalSon(object o)
			{
				return ((ProdigalSonIssueBehavior.ProdigalSonIssueQuest)o)._prodigalSon;
			}

			// Token: 0x06000F5A RID: 3930 RVA: 0x0006C16F File Offset: 0x0006A36F
			internal static object AutoGeneratedGetMemberValue_playerTalkedToTargetHero(object o)
			{
				return ((ProdigalSonIssueBehavior.ProdigalSonIssueQuest)o)._playerTalkedToTargetHero;
			}

			// Token: 0x06000F5B RID: 3931 RVA: 0x0006C181 File Offset: 0x0006A381
			internal static object AutoGeneratedGetMemberValue_targetHouse(object o)
			{
				return ((ProdigalSonIssueBehavior.ProdigalSonIssueQuest)o)._targetHouse;
			}

			// Token: 0x06000F5C RID: 3932 RVA: 0x0006C18E File Offset: 0x0006A38E
			internal static object AutoGeneratedGetMemberValue_questDifficulty(object o)
			{
				return ((ProdigalSonIssueBehavior.ProdigalSonIssueQuest)o)._questDifficulty;
			}

			// Token: 0x06000F5D RID: 3933 RVA: 0x0006C1A0 File Offset: 0x0006A3A0
			internal static object AutoGeneratedGetMemberValue_isHouseFightFinished(object o)
			{
				return ((ProdigalSonIssueBehavior.ProdigalSonIssueQuest)o)._isHouseFightFinished;
			}

			// Token: 0x06000F5E RID: 3934 RVA: 0x0006C1B2 File Offset: 0x0006A3B2
			internal static object AutoGeneratedGetMemberValue_playerTriedToPersuade(object o)
			{
				return ((ProdigalSonIssueBehavior.ProdigalSonIssueQuest)o)._playerTriedToPersuade;
			}

			// Token: 0x04000640 RID: 1600
			private const PersuasionDifficulty Difficulty = 5;

			// Token: 0x04000641 RID: 1601
			private const int DistanceSquaredToStartConversation = 4;

			// Token: 0x04000642 RID: 1602
			private const int CrimeRatingCancelRelationshipPenalty = -5;

			// Token: 0x04000643 RID: 1603
			private const int CrimeRatingCancelHonorXpPenalty = -50;

			// Token: 0x04000644 RID: 1604
			[SaveableField(10)]
			private readonly Hero _targetHero;

			// Token: 0x04000645 RID: 1605
			[SaveableField(20)]
			private readonly Hero _prodigalSon;

			// Token: 0x04000646 RID: 1606
			[SaveableField(30)]
			private bool _playerTalkedToTargetHero;

			// Token: 0x04000647 RID: 1607
			[SaveableField(40)]
			private readonly Location _targetHouse;

			// Token: 0x04000648 RID: 1608
			[SaveableField(50)]
			private readonly float _questDifficulty;

			// Token: 0x04000649 RID: 1609
			[SaveableField(60)]
			private bool _isHouseFightFinished;

			// Token: 0x0400064A RID: 1610
			[SaveableField(70)]
			private bool _playerTriedToPersuade;

			// Token: 0x0400064B RID: 1611
			private PersuasionTask _task;

			// Token: 0x0400064C RID: 1612
			private bool _isMissionFightInitialized;

			// Token: 0x0400064D RID: 1613
			private bool _isFirstMissionTick;
		}
	}
}
