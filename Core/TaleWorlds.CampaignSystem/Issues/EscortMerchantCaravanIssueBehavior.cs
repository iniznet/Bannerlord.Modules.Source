﻿using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.Inventory;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.CampaignSystem.Issues
{
	// Token: 0x02000306 RID: 774
	public class EscortMerchantCaravanIssueBehavior : CampaignBehaviorBase
	{
		// Token: 0x17000ACF RID: 2767
		// (get) Token: 0x06002CA6 RID: 11430 RVA: 0x000BA9EF File Offset: 0x000B8BEF
		private static EscortMerchantCaravanIssueBehavior Instance
		{
			get
			{
				return Campaign.Current.GetCampaignBehavior<EscortMerchantCaravanIssueBehavior>();
			}
		}

		// Token: 0x06002CA7 RID: 11431 RVA: 0x000BA9FC File Offset: 0x000B8BFC
		public override void RegisterEvents()
		{
			CampaignEvents.OnCheckForIssueEvent.AddNonSerializedListener(this, new Action<Hero>(this.OnCheckForIssue));
			CampaignEvents.OnNewGameCreatedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnNewGameCreated));
			CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnGameLoaded));
		}

		// Token: 0x06002CA8 RID: 11432 RVA: 0x000BAA50 File Offset: 0x000B8C50
		private void OnGameLoaded(CampaignGameStarter campaignGameStarter)
		{
			this.InitializeOnStart();
			if (MBSaveLoad.IsUpdatingGameVersion && MBSaveLoad.LastLoadedGameVersion < ApplicationVersion.FromString("e1.9.1", 17949))
			{
				for (int i = MobileParty.All.Count - 1; i >= 0; i--)
				{
					MobileParty mobileParty = MobileParty.All[i];
					if (mobileParty.StringId.Contains("defend_caravan_quest"))
					{
						if (mobileParty.MapEvent != null)
						{
							mobileParty.MapEvent.FinalizeEvent();
						}
						DestroyPartyAction.Apply(null, MobileParty.All[i]);
					}
				}
			}
		}

		// Token: 0x06002CA9 RID: 11433 RVA: 0x000BAAE0 File Offset: 0x000B8CE0
		private void InitializeOnStart()
		{
			if (MBObjectManager.Instance.GetObject<ItemObject>("hardwood") == null || MBObjectManager.Instance.GetObject<ItemObject>("sumpter_horse") == null)
			{
				CampaignEventDispatcher.Instance.RemoveListeners(this);
				using (List<KeyValuePair<Hero, IssueBase>>.Enumerator enumerator = Campaign.Current.IssueManager.Issues.Where((KeyValuePair<Hero, IssueBase> x) => x.Value.GetType() == typeof(EscortMerchantCaravanIssueBehavior.EscortMerchantCaravanIssue)).ToList<KeyValuePair<Hero, IssueBase>>().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<Hero, IssueBase> keyValuePair = enumerator.Current;
						keyValuePair.Value.CompleteIssueWithStayAliveConditionsFailed();
					}
					return;
				}
			}
			this.DefaultCaravanItems.Add(DefaultItems.Grain);
			foreach (string text in new string[] { "cotton", "velvet", "oil", "linen", "date_fruit" })
			{
				ItemObject @object = MBObjectManager.Instance.GetObject<ItemObject>(text);
				if (@object != null)
				{
					this.DefaultCaravanItems.Add(@object);
				}
			}
		}

		// Token: 0x06002CAA RID: 11434 RVA: 0x000BAC10 File Offset: 0x000B8E10
		private void OnNewGameCreated(CampaignGameStarter campaignGameStarter)
		{
			this.InitializeOnStart();
		}

		// Token: 0x06002CAB RID: 11435 RVA: 0x000BAC18 File Offset: 0x000B8E18
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x06002CAC RID: 11436 RVA: 0x000BAC1A File Offset: 0x000B8E1A
		private bool ConditionsHold(Hero issueGiver)
		{
			return issueGiver.IsMerchant && issueGiver.CurrentSettlement.Town.Security <= 50f && issueGiver.OwnedCaravans.Count < 2;
		}

		// Token: 0x06002CAD RID: 11437 RVA: 0x000BAC4C File Offset: 0x000B8E4C
		public void OnCheckForIssue(Hero hero)
		{
			if (this.ConditionsHold(hero))
			{
				Campaign.Current.IssueManager.AddPotentialIssueData(hero, new PotentialIssueData(new PotentialIssueData.StartIssueDelegate(this.OnSelected), typeof(EscortMerchantCaravanIssueBehavior.EscortMerchantCaravanIssue), IssueBase.IssueFrequency.VeryCommon, null));
				return;
			}
			Campaign.Current.IssueManager.AddPotentialIssueData(hero, new PotentialIssueData(typeof(EscortMerchantCaravanIssueBehavior.EscortMerchantCaravanIssue), IssueBase.IssueFrequency.VeryCommon));
		}

		// Token: 0x06002CAE RID: 11438 RVA: 0x000BACB0 File Offset: 0x000B8EB0
		private IssueBase OnSelected(in PotentialIssueData pid, Hero issueOwner)
		{
			return new EscortMerchantCaravanIssueBehavior.EscortMerchantCaravanIssue(issueOwner);
		}

		// Token: 0x04000D7A RID: 3450
		private const IssueBase.IssueFrequency EscortMerchantCaravanIssueFrequency = IssueBase.IssueFrequency.VeryCommon;

		// Token: 0x04000D7B RID: 3451
		internal readonly List<ItemObject> DefaultCaravanItems = new List<ItemObject>();

		// Token: 0x02000611 RID: 1553
		public class EscortMerchantCaravanIssue : IssueBase
		{
			// Token: 0x06004855 RID: 18517 RVA: 0x00143A22 File Offset: 0x00141C22
			internal static void AutoGeneratedStaticCollectObjectsEscortMerchantCaravanIssue(object o, List<object> collectedObjects)
			{
				((EscortMerchantCaravanIssueBehavior.EscortMerchantCaravanIssue)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			// Token: 0x06004856 RID: 18518 RVA: 0x00143A30 File Offset: 0x00141C30
			protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				base.AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			// Token: 0x06004857 RID: 18519 RVA: 0x00143A39 File Offset: 0x00141C39
			internal static object AutoGeneratedGetMemberValue_companionRewardRandom(object o)
			{
				return ((EscortMerchantCaravanIssueBehavior.EscortMerchantCaravanIssue)o)._companionRewardRandom;
			}

			// Token: 0x17000EAE RID: 3758
			// (get) Token: 0x06004858 RID: 18520 RVA: 0x00143A4B File Offset: 0x00141C4B
			public override IssueBase.AlternativeSolutionScaleFlag AlternativeSolutionScaleFlags
			{
				get
				{
					return IssueBase.AlternativeSolutionScaleFlag.Casualties | IssueBase.AlternativeSolutionScaleFlag.FailureRisk;
				}
			}

			// Token: 0x17000EAF RID: 3759
			// (get) Token: 0x06004859 RID: 18521 RVA: 0x00143A4F File Offset: 0x00141C4F
			public override int AlternativeSolutionBaseNeededMenCount
			{
				get
				{
					return 10 + MathF.Ceiling(16f * base.IssueDifficultyMultiplier);
				}
			}

			// Token: 0x17000EB0 RID: 3760
			// (get) Token: 0x0600485A RID: 18522 RVA: 0x00143A65 File Offset: 0x00141C65
			protected override int AlternativeSolutionBaseDurationInDaysInternal
			{
				get
				{
					return 6 + MathF.Ceiling(10f * base.IssueDifficultyMultiplier);
				}
			}

			// Token: 0x17000EB1 RID: 3761
			// (get) Token: 0x0600485B RID: 18523 RVA: 0x00143A7A File Offset: 0x00141C7A
			protected int DailyQuestRewardGold
			{
				get
				{
					return 250 + MathF.Ceiling(1000f * base.IssueDifficultyMultiplier);
				}
			}

			// Token: 0x17000EB2 RID: 3762
			// (get) Token: 0x0600485C RID: 18524 RVA: 0x00143A93 File Offset: 0x00141C93
			protected override int RewardGold
			{
				get
				{
					return Math.Min(this.DailyQuestRewardGold * this._companionRewardRandom, 8000);
				}
			}

			// Token: 0x17000EB3 RID: 3763
			// (get) Token: 0x0600485D RID: 18525 RVA: 0x00143AAC File Offset: 0x00141CAC
			public override TextObject IssueBriefByIssueGiver
			{
				get
				{
					TextObject textObject = new TextObject("{=CSqaF7tz}There's been a real surge of banditry around here recently. I don't know if it's because the lords are away fighting or something else, but it's a miracle if a traveler can make three leagues beyond the gates without being set upon by highwaymen.", null);
					if (base.IssueOwner.CharacterObject.GetPersona() == DefaultTraits.PersonaCurt || base.IssueOwner.CharacterObject.GetPersona() == DefaultTraits.PersonaSoftspoken)
					{
						textObject = new TextObject("{=xwc9mJdC}Things have gotten a lot worse recently with the brigands on the roads around town. My caravans get looted as soon as they're out of sight of the gates.", null);
					}
					return textObject;
				}
			}

			// Token: 0x17000EB4 RID: 3764
			// (get) Token: 0x0600485E RID: 18526 RVA: 0x00143B00 File Offset: 0x00141D00
			public override TextObject IssueAcceptByPlayer
			{
				get
				{
					return new TextObject("{=TGYJUUn0}Go on.", null);
				}
			}

			// Token: 0x17000EB5 RID: 3765
			// (get) Token: 0x0600485F RID: 18527 RVA: 0x00143B0D File Offset: 0x00141D0D
			public override TextObject IssueQuestSolutionExplanationByIssueGiver
			{
				get
				{
					return new TextObject("{=8ym6UvxE}I'm of a mind to send out a new caravan but I fear it will be plundered before it can turn a profit. So I am looking for some good fighters who can escort it until it finds its footing and visits a couple of settlements.", null);
				}
			}

			// Token: 0x17000EB6 RID: 3766
			// (get) Token: 0x06004860 RID: 18528 RVA: 0x00143B1C File Offset: 0x00141D1C
			public override TextObject IssueAlternativeSolutionExplanationByIssueGiver
			{
				get
				{
					TextObject textObject = new TextObject("{=ytdZutjw}I will be willing to pay generously {BASE_REWARD}{GOLD_ICON} for each day the caravan is on the road. It will be more than I usually pay for caravan guards, but you look like the type who send a message to these brigands, that my caravans aren't to be messed with.", null);
					if (base.IssueOwner.CharacterObject.GetPersona() == DefaultTraits.PersonaCurt || base.IssueOwner.CharacterObject.GetPersona() == DefaultTraits.PersonaSoftspoken)
					{
						textObject = new TextObject("{=YbbfaHqd}I will be willing to pay generously {BASE_REWARD}{GOLD_ICON} for each day the caravan is on the road. It will be more than I usually pay for guards, but figure maybe you can scare these bandits off. I'm sick of choosing between sending my men to the their deaths or letting them go because I've lost my goods and can't pay their wages.", null);
					}
					textObject.SetTextVariable("BASE_REWARD", this.DailyQuestRewardGold);
					return textObject;
				}
			}

			// Token: 0x17000EB7 RID: 3767
			// (get) Token: 0x06004861 RID: 18529 RVA: 0x00143B82 File Offset: 0x00141D82
			public override TextObject IssueQuestSolutionAcceptByPlayer
			{
				get
				{
					return new TextObject("{=a7fEPW5Y}Don't worry, I'll escort the caravan myself.", null);
				}
			}

			// Token: 0x17000EB8 RID: 3768
			// (get) Token: 0x06004862 RID: 18530 RVA: 0x00143B8F File Offset: 0x00141D8F
			public override TextObject IssueAlternativeSolutionAcceptByPlayer
			{
				get
				{
					TextObject textObject = new TextObject("{=N4p2GCsG}I'll assign one of my companions and {NEEDED_MEN_COUNT} of my men to protect your caravan for {RETURN_DAYS} days.", null);
					textObject.SetTextVariable("NEEDED_MEN_COUNT", base.GetTotalAlternativeSolutionNeededMenCount());
					textObject.SetTextVariable("RETURN_DAYS", base.GetTotalAlternativeSolutionDurationInDays());
					return textObject;
				}
			}

			// Token: 0x17000EB9 RID: 3769
			// (get) Token: 0x06004863 RID: 18531 RVA: 0x00143BC0 File Offset: 0x00141DC0
			public override TextObject IssueDiscussAlternativeSolution
			{
				get
				{
					return new TextObject("{=hU5j7b3e}I am sure your men are as capable as you are and will look after my caravan. Thanks again for your help, my friend.", null);
				}
			}

			// Token: 0x17000EBA RID: 3770
			// (get) Token: 0x06004864 RID: 18532 RVA: 0x00143BCD File Offset: 0x00141DCD
			public override TextObject IssueAlternativeSolutionResponseByIssueGiver
			{
				get
				{
					return new TextObject("{=iny76Ifh}Thank you, {?PLAYER.GENDER}madam{?}sir{\\?}, I think they will be enough.", null);
				}
			}

			// Token: 0x17000EBB RID: 3771
			// (get) Token: 0x06004865 RID: 18533 RVA: 0x00143BDA File Offset: 0x00141DDA
			public override bool IsThereAlternativeSolution
			{
				get
				{
					return true;
				}
			}

			// Token: 0x17000EBC RID: 3772
			// (get) Token: 0x06004866 RID: 18534 RVA: 0x00143BDD File Offset: 0x00141DDD
			public override bool IsThereLordSolution
			{
				get
				{
					return false;
				}
			}

			// Token: 0x17000EBD RID: 3773
			// (get) Token: 0x06004867 RID: 18535 RVA: 0x00143BE0 File Offset: 0x00141DE0
			protected override TextObject AlternativeSolutionStartLog
			{
				get
				{
					TextObject textObject = new TextObject("{=6y59FBgL}{ISSUEGIVER.LINK}, a merchant from {SETTLEMENT}, has told you about {?ISSUEGIVER.GENDER}her{?}his{\\?} recent problems with bandits. {?ISSUEGIVER.GENDER}She{?}he{\\?} asked you to guard {?ISSUEGIVER.GENDER}her{?}his{\\?} caravan for a while and deal with any attackers. In return {?ISSUEGIVER.GENDER}she{?}he{\\?} offered you {GOLD}{GOLD_ICON} for each day your troops spend on escort duty.{newline}You agreed to lend {?ISSUEGIVER.GENDER}her{?}him{\\?} {NEEDED_MEN_COUNT} men. They should be enough to turn away most of the bandits. Your troops should return after {RETURN_DAYS} days.", null);
					StringHelpers.SetCharacterProperties("ISSUEGIVER", base.IssueOwner.CharacterObject, textObject, false);
					textObject.SetTextVariable("SETTLEMENT", base.IssueOwner.CurrentSettlement.Name);
					textObject.SetTextVariable("NEEDED_MEN_COUNT", this.AlternativeSolutionSentTroops.TotalManCount);
					textObject.SetTextVariable("RETURN_DAYS", base.GetTotalAlternativeSolutionDurationInDays());
					textObject.SetTextVariable("GOLD", this.DailyQuestRewardGold);
					textObject.SetTextVariable("GOLD_ICON", "{=!}<img src=\"General\\Icons\\Coin@2x\" extend=\"8\">");
					return textObject;
				}
			}

			// Token: 0x17000EBE RID: 3774
			// (get) Token: 0x06004868 RID: 18536 RVA: 0x00143C7A File Offset: 0x00141E7A
			public override TextObject Title
			{
				get
				{
					return new TextObject("{=VpLzd69e}Escort Merchant Caravan", null);
				}
			}

			// Token: 0x17000EBF RID: 3775
			// (get) Token: 0x06004869 RID: 18537 RVA: 0x00143C87 File Offset: 0x00141E87
			public override TextObject Description
			{
				get
				{
					return new TextObject("{=8RNueEmy}A merchant caravan needs an escort for protection against bandits and brigands.", null);
				}
			}

			// Token: 0x17000EC0 RID: 3776
			// (get) Token: 0x0600486A RID: 18538 RVA: 0x00143C94 File Offset: 0x00141E94
			public override TextObject IssueAlternativeSolutionFailLog
			{
				get
				{
					return new TextObject("{=KLauwaRJ}The caravan was destroyed despite your companion's efforts. Quest failed.", null);
				}
			}

			// Token: 0x17000EC1 RID: 3777
			// (get) Token: 0x0600486B RID: 18539 RVA: 0x00143CA4 File Offset: 0x00141EA4
			public override TextObject IssueAlternativeSolutionSuccessLog
			{
				get
				{
					TextObject textObject = new TextObject("{=3NX8H4TJ}Your companion has protected the caravan that belongs to {ISSUE_GIVER.LINK} from {SETTLEMENT} as promised. {?ISSUE_GIVER.GENDER}She{?}He{\\?} was happy with your work.", null);
					StringHelpers.SetCharacterProperties("ISSUE_GIVER", base.IssueOwner.CharacterObject, textObject, false);
					textObject.SetTextVariable("SETTLEMENT", base.IssueSettlement.EncyclopediaLinkWithName);
					return textObject;
				}
			}

			// Token: 0x0600486C RID: 18540 RVA: 0x00143CED File Offset: 0x00141EED
			public EscortMerchantCaravanIssue(Hero issueOwner)
				: base(issueOwner, CampaignTime.DaysFromNow(30f))
			{
				this._companionRewardRandom = MBRandom.RandomInt(3, 10);
			}

			// Token: 0x0600486D RID: 18541 RVA: 0x00143D0E File Offset: 0x00141F0E
			protected override float GetIssueEffectAmountInternal(IssueEffect issueEffect)
			{
				if (issueEffect == DefaultIssueEffects.SettlementProsperity)
				{
					return -0.4f;
				}
				if (issueEffect == DefaultIssueEffects.IssueOwnerPower)
				{
					return -0.2f;
				}
				return 0f;
			}

			// Token: 0x0600486E RID: 18542 RVA: 0x00143D31 File Offset: 0x00141F31
			public override ValueTuple<SkillObject, int> GetAlternativeSolutionSkill(Hero hero)
			{
				return new ValueTuple<SkillObject, int>((hero.GetSkillValue(DefaultSkills.Scouting) >= hero.GetSkillValue(DefaultSkills.Riding)) ? DefaultSkills.Scouting : DefaultSkills.Riding, 120);
			}

			// Token: 0x0600486F RID: 18543 RVA: 0x00143D5E File Offset: 0x00141F5E
			public override bool DoTroopsSatisfyAlternativeSolution(TroopRoster troopRoster, out TextObject explanation)
			{
				explanation = TextObject.Empty;
				return QuestHelper.CheckRosterForAlternativeSolution(troopRoster, base.GetTotalAlternativeSolutionNeededMenCount(), ref explanation, 2, false);
			}

			// Token: 0x06004870 RID: 18544 RVA: 0x00143D76 File Offset: 0x00141F76
			public override bool AlternativeSolutionCondition(out TextObject explanation)
			{
				explanation = TextObject.Empty;
				return QuestHelper.CheckRosterForAlternativeSolution(MobileParty.MainParty.MemberRoster, base.GetTotalAlternativeSolutionNeededMenCount(), ref explanation, 2, false);
			}

			// Token: 0x17000EC2 RID: 3778
			// (get) Token: 0x06004871 RID: 18545 RVA: 0x00143D97 File Offset: 0x00141F97
			protected override int CompanionSkillRewardXP
			{
				get
				{
					return (int)(800f + 1000f * base.IssueDifficultyMultiplier);
				}
			}

			// Token: 0x06004872 RID: 18546 RVA: 0x00143DAC File Offset: 0x00141FAC
			public override bool IsTroopTypeNeededByAlternativeSolution(CharacterObject character)
			{
				return character.Tier >= 2;
			}

			// Token: 0x06004873 RID: 18547 RVA: 0x00143DBA File Offset: 0x00141FBA
			public override IssueBase.IssueFrequency GetFrequency()
			{
				return IssueBase.IssueFrequency.VeryCommon;
			}

			// Token: 0x06004874 RID: 18548 RVA: 0x00143DC0 File Offset: 0x00141FC0
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
				if (issueGiver.MapFaction.IsAtWarWith(Hero.MainHero.MapFaction))
				{
					flags |= IssueBase.PreconditionFlags.AtWar;
				}
				if (MobileParty.MainParty.MemberRoster.TotalHealthyCount < 20)
				{
					flags |= IssueBase.PreconditionFlags.NotEnoughTroops;
				}
				return flags == IssueBase.PreconditionFlags.None;
			}

			// Token: 0x06004875 RID: 18549 RVA: 0x00143E2D File Offset: 0x0014202D
			public override bool IssueStayAliveConditions()
			{
				return base.IssueOwner.OwnedCaravans.Count < 2 && base.IssueOwner.CurrentSettlement.Town.Security <= 80f;
			}

			// Token: 0x06004876 RID: 18550 RVA: 0x00143E63 File Offset: 0x00142063
			protected override void OnGameLoad()
			{
			}

			// Token: 0x06004877 RID: 18551 RVA: 0x00143E65 File Offset: 0x00142065
			protected override QuestBase GenerateIssueQuest(string questId)
			{
				return new EscortMerchantCaravanIssueBehavior.EscortMerchantCaravanIssueQuest(questId, base.IssueOwner, CampaignTime.DaysFromNow(30f), base.IssueDifficultyMultiplier, this.DailyQuestRewardGold);
			}

			// Token: 0x06004878 RID: 18552 RVA: 0x00143E8C File Offset: 0x0014208C
			protected override void AlternativeSolutionEndWithFailureConsequence()
			{
				base.IssueOwner.AddPower(-5f);
				this.RelationshipChangeWithIssueOwner = -5;
				TraitLevelingHelper.OnIssueFailed(base.IssueOwner, new Tuple<TraitObject, int>[]
				{
					new Tuple<TraitObject, int>(DefaultTraits.Honor, -20)
				});
				base.IssueSettlement.Prosperity -= 20f;
			}

			// Token: 0x06004879 RID: 18553 RVA: 0x00143EE8 File Offset: 0x001420E8
			protected override void AlternativeSolutionEndWithSuccessConsequence()
			{
				base.IssueOwner.AddPower(10f);
				this.RelationshipChangeWithIssueOwner = 5;
				base.IssueSettlement.Prosperity += 10f;
			}

			// Token: 0x0600487A RID: 18554 RVA: 0x00143F18 File Offset: 0x00142118
			protected override void CompleteIssueWithTimedOutConsequences()
			{
			}

			// Token: 0x04001927 RID: 6439
			private const int MinimumRequiredMenCount = 20;

			// Token: 0x04001928 RID: 6440
			private const int AlternativeSolutionTroopTierRequirement = 2;

			// Token: 0x04001929 RID: 6441
			private const int NeededCompanionSkillAmount = 120;

			// Token: 0x0400192A RID: 6442
			private const int QuestTimeLimit = 30;

			// Token: 0x0400192B RID: 6443
			private const int IssueDuration = 30;

			// Token: 0x0400192C RID: 6444
			[SaveableField(10)]
			private int _companionRewardRandom;
		}

		// Token: 0x02000612 RID: 1554
		public class EscortMerchantCaravanIssueQuest : QuestBase
		{
			// Token: 0x0600487B RID: 18555 RVA: 0x00143F1A File Offset: 0x0014211A
			internal static void AutoGeneratedStaticCollectObjectsEscortMerchantCaravanIssueQuest(object o, List<object> collectedObjects)
			{
				((EscortMerchantCaravanIssueBehavior.EscortMerchantCaravanIssueQuest)o).AutoGeneratedInstanceCollectObjects(collectedObjects);
			}

			// Token: 0x0600487C RID: 18556 RVA: 0x00143F28 File Offset: 0x00142128
			protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
			{
				base.AutoGeneratedInstanceCollectObjects(collectedObjects);
				collectedObjects.Add(this._visitedSettlements);
				collectedObjects.Add(this._questCaravanMobileParty);
				collectedObjects.Add(this._questBanditMobileParty);
				collectedObjects.Add(this._otherBanditParty);
				collectedObjects.Add(this._playerStartsQuestLog);
			}

			// Token: 0x0600487D RID: 18557 RVA: 0x00143F78 File Offset: 0x00142178
			internal static object AutoGeneratedGetMemberValue_requiredSettlementNumber(object o)
			{
				return ((EscortMerchantCaravanIssueBehavior.EscortMerchantCaravanIssueQuest)o)._requiredSettlementNumber;
			}

			// Token: 0x0600487E RID: 18558 RVA: 0x00143F8A File Offset: 0x0014218A
			internal static object AutoGeneratedGetMemberValue_visitedSettlements(object o)
			{
				return ((EscortMerchantCaravanIssueBehavior.EscortMerchantCaravanIssueQuest)o)._visitedSettlements;
			}

			// Token: 0x0600487F RID: 18559 RVA: 0x00143F97 File Offset: 0x00142197
			internal static object AutoGeneratedGetMemberValue_questCaravanMobileParty(object o)
			{
				return ((EscortMerchantCaravanIssueBehavior.EscortMerchantCaravanIssueQuest)o)._questCaravanMobileParty;
			}

			// Token: 0x06004880 RID: 18560 RVA: 0x00143FA4 File Offset: 0x001421A4
			internal static object AutoGeneratedGetMemberValue_questBanditMobileParty(object o)
			{
				return ((EscortMerchantCaravanIssueBehavior.EscortMerchantCaravanIssueQuest)o)._questBanditMobileParty;
			}

			// Token: 0x06004881 RID: 18561 RVA: 0x00143FB1 File Offset: 0x001421B1
			internal static object AutoGeneratedGetMemberValue_difficultyMultiplier(object o)
			{
				return ((EscortMerchantCaravanIssueBehavior.EscortMerchantCaravanIssueQuest)o)._difficultyMultiplier;
			}

			// Token: 0x06004882 RID: 18562 RVA: 0x00143FC3 File Offset: 0x001421C3
			internal static object AutoGeneratedGetMemberValue_isPlayerNotifiedForDanger(object o)
			{
				return ((EscortMerchantCaravanIssueBehavior.EscortMerchantCaravanIssueQuest)o)._isPlayerNotifiedForDanger;
			}

			// Token: 0x06004883 RID: 18563 RVA: 0x00143FD5 File Offset: 0x001421D5
			internal static object AutoGeneratedGetMemberValue_otherBanditParty(object o)
			{
				return ((EscortMerchantCaravanIssueBehavior.EscortMerchantCaravanIssueQuest)o)._otherBanditParty;
			}

			// Token: 0x06004884 RID: 18564 RVA: 0x00143FE2 File Offset: 0x001421E2
			internal static object AutoGeneratedGetMemberValue_questBanditPartyFollowDuration(object o)
			{
				return ((EscortMerchantCaravanIssueBehavior.EscortMerchantCaravanIssueQuest)o)._questBanditPartyFollowDuration;
			}

			// Token: 0x06004885 RID: 18565 RVA: 0x00143FF4 File Offset: 0x001421F4
			internal static object AutoGeneratedGetMemberValue_otherBanditPartyFollowDuration(object o)
			{
				return ((EscortMerchantCaravanIssueBehavior.EscortMerchantCaravanIssueQuest)o)._otherBanditPartyFollowDuration;
			}

			// Token: 0x06004886 RID: 18566 RVA: 0x00144006 File Offset: 0x00142206
			internal static object AutoGeneratedGetMemberValue_daysSpentForEscorting(object o)
			{
				return ((EscortMerchantCaravanIssueBehavior.EscortMerchantCaravanIssueQuest)o)._daysSpentForEscorting;
			}

			// Token: 0x06004887 RID: 18567 RVA: 0x00144018 File Offset: 0x00142218
			internal static object AutoGeneratedGetMemberValue_questBanditPartyAlreadyAttacked(object o)
			{
				return ((EscortMerchantCaravanIssueBehavior.EscortMerchantCaravanIssueQuest)o)._questBanditPartyAlreadyAttacked;
			}

			// Token: 0x06004888 RID: 18568 RVA: 0x0014402A File Offset: 0x0014222A
			internal static object AutoGeneratedGetMemberValue_playerStartsQuestLog(object o)
			{
				return ((EscortMerchantCaravanIssueBehavior.EscortMerchantCaravanIssueQuest)o)._playerStartsQuestLog;
			}

			// Token: 0x17000EC3 RID: 3779
			// (get) Token: 0x06004889 RID: 18569 RVA: 0x00144037 File Offset: 0x00142237
			public override TextObject Title
			{
				get
				{
					return new TextObject("{=VpLzd69e}Escort Merchant Caravan", null);
				}
			}

			// Token: 0x17000EC4 RID: 3780
			// (get) Token: 0x0600488A RID: 18570 RVA: 0x00144044 File Offset: 0x00142244
			public override bool IsRemainingTimeHidden
			{
				get
				{
					return false;
				}
			}

			// Token: 0x17000EC5 RID: 3781
			// (get) Token: 0x0600488B RID: 18571 RVA: 0x00144047 File Offset: 0x00142247
			private int BanditPartyTroopCount
			{
				get
				{
					return (int)MathF.Min(40f, (float)(MobileParty.MainParty.MemberRoster.TotalHealthyCount + this._questCaravanMobileParty.MemberRoster.TotalHealthyCount) * 0.7f);
				}
			}

			// Token: 0x17000EC6 RID: 3782
			// (get) Token: 0x0600488C RID: 18572 RVA: 0x0014407B File Offset: 0x0014227B
			private int CaravanPartyTroopCount
			{
				get
				{
					return MBRandom.RandomInt(10, 14);
				}
			}

			// Token: 0x17000EC7 RID: 3783
			// (get) Token: 0x0600488D RID: 18573 RVA: 0x00144086 File Offset: 0x00142286
			private bool CaravanIsInsideSettlement
			{
				get
				{
					return this._questCaravanMobileParty.CurrentSettlement != null;
				}
			}

			// Token: 0x17000EC8 RID: 3784
			// (get) Token: 0x0600488E RID: 18574 RVA: 0x00144096 File Offset: 0x00142296
			private int TotalRewardGold
			{
				get
				{
					return MathF.Min(8000, this.RewardGold * this._daysSpentForEscorting);
				}
			}

			// Token: 0x17000EC9 RID: 3785
			// (get) Token: 0x0600488F RID: 18575 RVA: 0x001440AF File Offset: 0x001422AF
			private CustomPartyComponent CaravanCustomPartyComponent
			{
				get
				{
					if (this._customPartyComponent == null)
					{
						this._customPartyComponent = this._questCaravanMobileParty.PartyComponent as CustomPartyComponent;
					}
					return this._customPartyComponent;
				}
			}

			// Token: 0x17000ECA RID: 3786
			// (get) Token: 0x06004890 RID: 18576 RVA: 0x001440D8 File Offset: 0x001422D8
			private TextObject _playerStartsQuestLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=YXbKXUDu}{ISSUE_GIVER.LINK}, a merchant from {SETTLEMENT}, has told you about {?ISSUE_GIVER.GENDER}her{?}his{\\?} recent problems with bandits. {?ISSUE_GIVER.GENDER}She{?}He{\\?} asked you to guard {?ISSUE_GIVER.GENDER}her{?}his{\\?} caravan for a while and deal with any attackers. In return {?ISSUE_GIVER.GENDER}she{?}he{\\?} offered you {GOLD}{GOLD_ICON} denars for each day you spend on escort duty.{newline}You have agreed to guard it yourself until it visits {NUMBER_OF_SETTLEMENTS} settlements.", null);
					StringHelpers.SetCharacterProperties("ISSUE_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					textObject.SetTextVariable("SETTLEMENT", Settlement.CurrentSettlement.Name);
					textObject.SetTextVariable("NUMBER_OF_SETTLEMENTS", this._requiredSettlementNumber);
					textObject.SetTextVariable("GOLD", this.RewardGold);
					textObject.SetTextVariable("GOLD_ICON", "{=!}<img src=\"General\\Icons\\Coin@2x\" extend=\"8\">");
					return textObject;
				}
			}

			// Token: 0x17000ECB RID: 3787
			// (get) Token: 0x06004891 RID: 18577 RVA: 0x00144155 File Offset: 0x00142355
			private TextObject _caravanDestroyedQuestLogText
			{
				get
				{
					return new TextObject("{=zk9QyKIz}The caravan was destroyed. Quest failed.", null);
				}
			}

			// Token: 0x17000ECC RID: 3788
			// (get) Token: 0x06004892 RID: 18578 RVA: 0x00144164 File Offset: 0x00142364
			private TextObject _caravanLostTheTrackLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=y62dyzH6}You have lost the track of caravan. Your agreement with {ISSUE_GIVER.LINK} is failed.", null);
					StringHelpers.SetCharacterProperties("ISSUE_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					return textObject;
				}
			}

			// Token: 0x17000ECD RID: 3789
			// (get) Token: 0x06004893 RID: 18579 RVA: 0x00144198 File Offset: 0x00142398
			private TextObject _caravanDestroyedByBanditsLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=MhvyTcrH}The caravan is destroyed by some bandits. Your agreement with {ISSUE_GIVER.LINK} is failed.", null);
					StringHelpers.SetCharacterProperties("ISSUE_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					return textObject;
				}
			}

			// Token: 0x17000ECE RID: 3790
			// (get) Token: 0x06004894 RID: 18580 RVA: 0x001441CA File Offset: 0x001423CA
			private TextObject _caravanDestroyedByPlayerQuestLogText
			{
				get
				{
					return new TextObject("{=Rd3m5kyk}You have attacked the caravan.", null);
				}
			}

			// Token: 0x17000ECF RID: 3791
			// (get) Token: 0x06004895 RID: 18581 RVA: 0x001441D8 File Offset: 0x001423D8
			private TextObject _successQuestLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=dKEADOhG}You have protected the caravan belonging to {QUEST_GIVER.LINK} from {SETTLEMENT} as promised. {?QUEST_GIVER.GENDER}She{?}He{\\?} was happy with your work.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					textObject.SetTextVariable("SETTLEMENT", base.QuestGiver.CurrentSettlement.Name);
					return textObject;
				}
			}

			// Token: 0x17000ED0 RID: 3792
			// (get) Token: 0x06004896 RID: 18582 RVA: 0x00144228 File Offset: 0x00142428
			private TextObject _cancelByWarQuestLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=KhNkBd9O}Your clan is now at war with the {QUEST_GIVER.LINK}’s lord. Your agreement with {QUEST_GIVER.LINK} was canceled.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					return textObject;
				}
			}

			// Token: 0x06004897 RID: 18583 RVA: 0x0014425C File Offset: 0x0014245C
			public EscortMerchantCaravanIssueQuest(string questId, Hero giverHero, CampaignTime duration, float difficultyMultiplier, int rewardGold)
				: base(questId, giverHero, duration, rewardGold)
			{
				this._difficultyMultiplier = difficultyMultiplier;
				this._requiredSettlementNumber = MathF.Round(2f + 4f * this._difficultyMultiplier);
				this._visitedSettlements = new List<Settlement>();
				this.SetDialogs();
				base.InitializeQuestOnCreation();
			}

			// Token: 0x06004898 RID: 18584 RVA: 0x001442B8 File Offset: 0x001424B8
			protected override void SetDialogs()
			{
				this.OfferDialogFlow = DialogFlow.CreateDialogFlow("issue_classic_quest_start", 100).NpcLine(new TextObject("{=TdwKwExD}Thank you. You can find the caravan just outside the settlement.", null), null, null).Condition(() => Hero.OneToOneConversationHero == base.QuestGiver)
					.Consequence(new ConversationSentence.OnConsequenceDelegate(this.QuestAcceptedConsequences))
					.CloseDialog();
				this.DiscussDialogFlow = DialogFlow.CreateDialogFlow("quest_discuss", 100).NpcLine(new TextObject("{=vtZYmAaR}I feel good knowing that you're looking after my caravan. Safe journeys, my friend!", null), null, null).Condition(() => Hero.OneToOneConversationHero == base.QuestGiver)
					.CloseDialog();
				Campaign.Current.ConversationManager.AddDialogFlow(this.GetCaravanPartyDialogFlow(), this);
				Campaign.Current.ConversationManager.AddDialogFlow(this.GetCaravanGreetingDialogFlow(), this);
				Campaign.Current.ConversationManager.AddDialogFlow(this.GetCaravanTradeDialogFlow(), this);
				Campaign.Current.ConversationManager.AddDialogFlow(this.GetCaravanLootDialogFlow(), this);
				Campaign.Current.ConversationManager.AddDialogFlow(this.GetCaravanFarewellDialogFlow(), this);
			}

			// Token: 0x17000ED1 RID: 3793
			// (get) Token: 0x06004899 RID: 18585 RVA: 0x001443B8 File Offset: 0x001425B8
			private TextObject _caravanNoTargetLogText
			{
				get
				{
					TextObject textObject = new TextObject("{=1FOmvEdf}All profitable trade routes of the caravan are blocked by recent wars. {QUEST_GIVER.LINK} decided to recall the caravan until the situation gets better. {?QUEST_GIVER.GENDER}She{?}He{\\?} was happy with your service and sent you {REWARD}{GOLD_ICON} as promised.", null);
					StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
					textObject.SetTextVariable("REWARD", this.TotalRewardGold);
					return textObject;
				}
			}

			// Token: 0x0600489A RID: 18586 RVA: 0x001443FC File Offset: 0x001425FC
			private DialogFlow GetCaravanPartyDialogFlow()
			{
				TextObject textObject = new TextObject("{=ZAqEJI9T}About the task {QUEST_GIVER.LINK} gave me.", null);
				StringHelpers.SetCharacterProperties("QUEST_GIVER", base.QuestGiver.CharacterObject, textObject, false);
				return DialogFlow.CreateDialogFlow("escort_caravan_talk", 125).BeginPlayerOptions().PlayerOption(textObject, null)
					.Condition(new ConversationSentence.OnConditionDelegate(this.caravan_talk_on_condition))
					.NpcLine("{=heWYa9Oq}I feel safe knowing that you're looking after us. Please continue to follow us my friend!", null, null)
					.Consequence(delegate
					{
						PlayerEncounter.LeaveEncounter = true;
					})
					.CloseDialog()
					.EndPlayerOptions();
			}

			// Token: 0x0600489B RID: 18587 RVA: 0x00144490 File Offset: 0x00142690
			private bool caravan_talk_on_condition()
			{
				return this._questCaravanMobileParty.MemberRoster.Contains(CharacterObject.OneToOneConversationCharacter) && this._questCaravanMobileParty == MobileParty.ConversationParty && MobileParty.ConversationParty != null && MobileParty.ConversationParty.IsCustomParty && !CharacterObject.OneToOneConversationCharacter.IsHero && MobileParty.ConversationParty.Party.Owner != Hero.MainHero;
			}

			// Token: 0x0600489C RID: 18588 RVA: 0x001444FC File Offset: 0x001426FC
			private DialogFlow GetCaravanFarewellDialogFlow()
			{
				TextObject textObject = new TextObject("{=1IJouNaM}Carry on, then. Farewell.", null);
				return DialogFlow.CreateDialogFlow("escort_caravan_talk", 125).BeginPlayerOptions().PlayerOption(textObject, null)
					.Condition(new ConversationSentence.OnConditionDelegate(this.caravan_talk_on_condition))
					.NpcLine("{=heWYa9Oq}I feel safe knowing that you're looking after us. Please continue to follow us my friend!", null, null)
					.Consequence(delegate
					{
						PlayerEncounter.LeaveEncounter = true;
					})
					.CloseDialog()
					.EndPlayerOptions();
			}

			// Token: 0x0600489D RID: 18589 RVA: 0x00144578 File Offset: 0x00142778
			private DialogFlow GetCaravanLootDialogFlow()
			{
				TextObject textObject = new TextObject("{=WOBy5UfY}Hand over your goods, or die!", null);
				return DialogFlow.CreateDialogFlow("escort_caravan_talk", 125).BeginPlayerOptions().PlayerOption(textObject, null)
					.Condition(new ConversationSentence.OnConditionDelegate(this.caravan_loot_on_condition))
					.NpcLine("{=QNaKmkt9}We're paid to guard this caravan. If you want to rob it, it's going to be over our dead bodies![rf:idle_angry][ib:aggressive]", null, null)
					.BeginPlayerOptions()
					.PlayerOption("{=EhxS7NQ4}So be it. Attack!", null)
					.Consequence(new ConversationSentence.OnConsequenceDelegate(this.conversation_caravan_fight_on_consequence))
					.CloseDialog()
					.PlayerOption("{=bfPsE9M1}You must have misunderstood me. Go in peace.", null)
					.Consequence(new ConversationSentence.OnConsequenceDelegate(this.caravan_talk_leave_on_consequence))
					.CloseDialog()
					.EndPlayerOptions()
					.EndPlayerOptions();
			}

			// Token: 0x0600489E RID: 18590 RVA: 0x00144617 File Offset: 0x00142817
			private void conversation_caravan_fight_on_consequence()
			{
				PlayerEncounter.Current.IsEnemy = true;
			}

			// Token: 0x0600489F RID: 18591 RVA: 0x00144624 File Offset: 0x00142824
			private void caravan_talk_leave_on_consequence()
			{
				if (PlayerEncounter.Current != null)
				{
					PlayerEncounter.LeaveEncounter = true;
				}
			}

			// Token: 0x060048A0 RID: 18592 RVA: 0x00144634 File Offset: 0x00142834
			private DialogFlow GetCaravanTradeDialogFlow()
			{
				TextObject textObject = new TextObject("{=t0UGXPV4}I'm interested in trading. What kind of products do you have?", null);
				return DialogFlow.CreateDialogFlow("escort_caravan_talk", 125).BeginPlayerOptions().PlayerOption(textObject, null)
					.Condition(new ConversationSentence.OnConditionDelegate(this.caravan_buy_products_on_condition))
					.NpcLine("{=tlLDHAIu}Very well. A pleasure doing business with you.[rf:convo_relaxed_happy][ib:demure]", null, null)
					.Condition(new ConversationSentence.OnConditionDelegate(this.conversation_caravan_player_trade_end_on_condition))
					.NpcLine("{=DQBaaC0e}Is there anything else?", null, null)
					.GotoDialogState("escort_caravan_talk")
					.EndPlayerOptions();
			}

			// Token: 0x060048A1 RID: 18593 RVA: 0x001446B0 File Offset: 0x001428B0
			private bool caravan_buy_products_on_condition()
			{
				if (MobileParty.ConversationParty != null && MobileParty.ConversationParty == this._questCaravanMobileParty && !MobileParty.ConversationParty.IsCaravan)
				{
					for (int i = 0; i < MobileParty.ConversationParty.ItemRoster.Count; i++)
					{
						if (MobileParty.ConversationParty.ItemRoster.GetElementNumber(i) > 0)
						{
							return true;
						}
					}
				}
				return false;
			}

			// Token: 0x060048A2 RID: 18594 RVA: 0x0014470D File Offset: 0x0014290D
			private bool conversation_caravan_player_trade_end_on_condition()
			{
				if (MobileParty.ConversationParty != null && MobileParty.ConversationParty == this._questCaravanMobileParty && !MobileParty.ConversationParty.IsCaravan)
				{
					InventoryManager.OpenTradeWithCaravanOrAlleyParty(MobileParty.ConversationParty, InventoryManager.InventoryCategoryType.None);
				}
				return true;
			}

			// Token: 0x060048A3 RID: 18595 RVA: 0x0014473C File Offset: 0x0014293C
			private DialogFlow GetCaravanGreetingDialogFlow()
			{
				TextObject textObject = new TextObject("{=FpUybbSk}Greetings. This caravan is owned by {MERCHANT.LINK}. We trade under the protection of {PROTECTOR.LINK}, master of {HOMETOWN}. How may we help you?[if:convo_normal]", null);
				return DialogFlow.CreateDialogFlow("start", 125).NpcLine(textObject, null, null).Condition(new ConversationSentence.OnConditionDelegate(this.caravan_talk_on_condition))
					.GotoDialogState("escort_caravan_talk");
			}

			// Token: 0x060048A4 RID: 18596 RVA: 0x00144784 File Offset: 0x00142984
			private void QuestAcceptedConsequences()
			{
				base.StartQuest();
				this.SpawnCaravan();
				this._playerStartsQuestLog = base.AddDiscreteLog(this._playerStartsQuestLogText, new TextObject("{=r2y3n7dR}Visited Settlements", null), this._visitedSettlements.Count, this._requiredSettlementNumber, null, false);
			}

			// Token: 0x060048A5 RID: 18597 RVA: 0x001447C4 File Offset: 0x001429C4
			private bool caravan_loot_on_condition()
			{
				bool flag = MobileParty.ConversationParty != null && MobileParty.ConversationParty.Party.MapFaction != Hero.MainHero.MapFaction && !MobileParty.ConversationParty.IsCaravan && MobileParty.ConversationParty == this._questCaravanMobileParty;
				if (flag)
				{
					MBTextManager.SetTextVariable("HOMETOWN", MobileParty.ConversationParty.HomeSettlement.EncyclopediaLinkWithName, false);
					StringHelpers.SetCharacterProperties("MERCHANT", MobileParty.ConversationParty.Party.Owner.CharacterObject, null, false);
					StringHelpers.SetCharacterProperties("PROTECTOR", MobileParty.ConversationParty.HomeSettlement.OwnerClan.Leader.CharacterObject, null, false);
				}
				return flag;
			}

			// Token: 0x060048A6 RID: 18598 RVA: 0x00144874 File Offset: 0x00142A74
			private void SpawnCaravan()
			{
				ItemRoster itemRoster = new ItemRoster();
				foreach (ItemObject itemObject in EscortMerchantCaravanIssueBehavior.Instance.DefaultCaravanItems)
				{
					itemRoster.AddToCounts(itemObject, 7);
				}
				string text;
				string text2;
				this.GetAdditionalVisualsForParty(base.QuestGiver.Culture, out text, out text2);
				TextObject textObject = GameTexts.FindText("str_caravan_party_name", null);
				textObject.SetCharacterProperties("OWNER", base.QuestGiver.CharacterObject, false);
				this._questCaravanMobileParty = CustomPartyComponent.CreateQuestParty(base.QuestGiver.CurrentSettlement.GatePosition, 0f, base.QuestGiver.CurrentSettlement, textObject, base.QuestGiver.Clan, TroopRoster.CreateDummyTroopRoster(), TroopRoster.CreateDummyTroopRoster(), base.QuestGiver, text, text2, 4f, false);
				this.InitializeCaravanOnCreation(this._questCaravanMobileParty, base.QuestGiver, base.QuestGiver.CurrentSettlement, itemRoster, this.CaravanPartyTroopCount, false);
				base.AddTrackedObject(this._questCaravanMobileParty);
				this._questCaravanMobileParty.SetPartyUsedByQuest(true);
				this._questCaravanMobileParty.Ai.SetDoNotMakeNewDecisions(true);
				this._questCaravanMobileParty.IgnoreByOtherPartiesTill(base.QuestDueTime);
				this._caravanWaitedInSettlementForHours = 4;
			}

			// Token: 0x060048A7 RID: 18599 RVA: 0x001449C4 File Offset: 0x00142BC4
			private bool ProperSettlementCondition(Settlement settlement)
			{
				return settlement != Settlement.CurrentSettlement && settlement.IsTown && !settlement.IsUnderSiege && !this._visitedSettlements.Contains(settlement);
			}

			// Token: 0x060048A8 RID: 18600 RVA: 0x001449F0 File Offset: 0x00142BF0
			private void InitializeCaravanOnCreation(MobileParty mobileParty, Hero owner, Settlement settlement, ItemRoster caravanItems, int troopToBeGiven, bool isElite)
			{
				mobileParty.Aggressiveness = 0f;
				if (troopToBeGiven == 0)
				{
					float num;
					if (MBRandom.RandomFloat < 0.67f)
					{
						num = (1f - MBRandom.RandomFloat * MBRandom.RandomFloat) * 0.5f + 0.5f;
					}
					else
					{
						num = 1f;
					}
					int num2 = (int)((float)mobileParty.Party.PartySizeLimit * num);
					if (num2 >= 10)
					{
						num2--;
					}
					troopToBeGiven = num2;
				}
				PartyTemplateObject partyTemplateObject = (isElite ? settlement.Culture.EliteCaravanPartyTemplate : settlement.Culture.CaravanPartyTemplate);
				mobileParty.InitializeMobilePartyAtPosition(partyTemplateObject, settlement.GatePosition, troopToBeGiven);
				CharacterObject characterObject = CharacterObject.All.First((CharacterObject character) => character.Occupation == Occupation.CaravanGuard && character.IsInfantry && character.Level == 26 && character.Culture == mobileParty.Party.Owner.Culture);
				mobileParty.MemberRoster.AddToCounts(characterObject, 1, true, 0, 0, true, -1);
				mobileParty.Party.Visuals.SetMapIconAsDirty();
				mobileParty.InitializePartyTrade(10000 + ((owner.Clan == Clan.PlayerClan) ? 5000 : 0));
				if (caravanItems != null)
				{
					mobileParty.ItemRoster.Add(caravanItems);
					return;
				}
				float num3 = 10000f;
				ItemObject itemObject = null;
				foreach (ItemObject itemObject2 in Items.All)
				{
					if (itemObject2.ItemCategory == DefaultItemCategories.PackAnimal && !itemObject2.NotMerchandise && (float)itemObject2.Value < num3)
					{
						itemObject = itemObject2;
						num3 = (float)itemObject2.Value;
					}
				}
				if (itemObject != null)
				{
					mobileParty.ItemRoster.Add(new ItemRosterElement(itemObject, (int)((float)mobileParty.MemberRoster.TotalManCount * 0.5f), null));
				}
			}

			// Token: 0x060048A9 RID: 18601 RVA: 0x00144BE0 File Offset: 0x00142DE0
			private void GetAdditionalVisualsForParty(CultureObject culture, out string mountStringId, out string harnessStringId)
			{
				if (culture.StringId == "aserai" || culture.StringId == "khuzait")
				{
					mountStringId = "camel";
					harnessStringId = ((MBRandom.RandomFloat > 0.5f) ? "camel_saddle_a" : "camel_saddle_b");
					return;
				}
				mountStringId = "mule";
				harnessStringId = ((MBRandom.RandomFloat > 0.5f) ? "mule_load_a" : ((MBRandom.RandomFloat > 0.5f) ? "mule_load_b" : "mule_load_c"));
			}

			// Token: 0x060048AA RID: 18602 RVA: 0x00144C68 File Offset: 0x00142E68
			protected override void RegisterEvents()
			{
				CampaignEvents.SettlementEntered.AddNonSerializedListener(this, new Action<MobileParty, Settlement, Hero>(this.OnSettlementEntered));
				CampaignEvents.OnSettlementLeftEvent.AddNonSerializedListener(this, new Action<MobileParty, Settlement>(this.OnSettlementLeft));
				CampaignEvents.MapEventEnded.AddNonSerializedListener(this, new Action<MapEvent>(this.OnMapEventEnded));
				CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new Action(this.OnDailyTick));
				CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(this.OnHourlyTick));
				CampaignEvents.WarDeclared.AddNonSerializedListener(this, new Action<IFaction, IFaction, DeclareWarAction.DeclareWarDetail>(this.OnWarDeclared));
				CampaignEvents.ClanChangedKingdom.AddNonSerializedListener(this, new Action<Clan, Kingdom, Kingdom, ChangeKingdomAction.ChangeKingdomActionDetail, bool>(this.OnClanChangedKingdom));
				CampaignEvents.HourlyTickPartyEvent.AddNonSerializedListener(this, new Action<MobileParty>(this.OnPartyHourlyTick));
				CampaignEvents.OnSettlementOwnerChangedEvent.AddNonSerializedListener(this, new Action<Settlement, bool, Hero, Hero, Hero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail>(this.OnSettlementOwnerChanged));
			}

			// Token: 0x060048AB RID: 18603 RVA: 0x00144D44 File Offset: 0x00142F44
			private void OnPartyHourlyTick(MobileParty mobileParty)
			{
				this.CheckPartyAndMakeItAttackTheCaravan(mobileParty);
				this.CheckEncounterForBanditParty(this._questBanditMobileParty);
				this.CheckEncounterForBanditParty(this._otherBanditParty);
				this.CheckOtherBanditPartyDistance();
			}

			// Token: 0x060048AC RID: 18604 RVA: 0x00144D6C File Offset: 0x00142F6C
			private void CheckOtherBanditPartyDistance()
			{
				if (this._otherBanditParty != null && this._otherBanditParty.IsActive && this._otherBanditParty.TargetParty == this._questCaravanMobileParty && this._otherBanditPartyFollowDuration < 0)
				{
					if (base.IsTracked(this._otherBanditParty))
					{
						base.RemoveTrackedObject(this._otherBanditParty);
					}
					this._otherBanditParty.Ai.SetMoveModeHold();
					this._otherBanditParty.Ai.SetDoNotMakeNewDecisions(false);
					this._otherBanditParty = null;
				}
				if (this._questBanditMobileParty != null && this._questBanditMobileParty.IsActive && this._questBanditMobileParty.MapEvent == null && this._questBanditMobileParty.TargetParty == this._questCaravanMobileParty && this._questBanditPartyFollowDuration < 0 && !this._questBanditMobileParty.IsVisible)
				{
					if (base.IsTracked(this._questBanditMobileParty))
					{
						base.RemoveTrackedObject(this._questBanditMobileParty);
					}
					this._questBanditMobileParty.Ai.SetMoveModeHold();
					this._questBanditMobileParty.Ai.SetDoNotMakeNewDecisions(false);
				}
			}

			// Token: 0x060048AD RID: 18605 RVA: 0x00144E74 File Offset: 0x00143074
			private void CheckEncounterForBanditParty(MobileParty mobileParty)
			{
				if (mobileParty != null && mobileParty.IsActive && mobileParty.MapEvent == null && this._questCaravanMobileParty.IsActive && this._questCaravanMobileParty.MapEvent == null && this._questCaravanMobileParty.CurrentSettlement == null && mobileParty.Position2D.DistanceSquared(this._questCaravanMobileParty.Position2D) <= 1f)
				{
					EncounterManager.StartPartyEncounter(mobileParty.Party, this._questCaravanMobileParty.Party);
					MBInformationManager.AddQuickInformation(new TextObject("{=o8uAzFaJ}The caravan you are protecting is ambushed by raiders!", null), 0, null, "");
					this._questCaravanMobileParty.MapEvent.IsInvulnerable = true;
				}
			}

			// Token: 0x060048AE RID: 18606 RVA: 0x00144F24 File Offset: 0x00143124
			private void CheckPartyAndMakeItAttackTheCaravan(MobileParty mobileParty)
			{
				if (this._otherBanditParty == null && mobileParty.MapEvent == null && mobileParty.IsBandit && mobileParty != this._questBanditMobileParty && mobileParty.Party.NumberOfHealthyMembers > this._questCaravanMobileParty.Party.NumberOfHealthyMembers && (mobileParty.Speed > this._questCaravanMobileParty.Speed || mobileParty.Position2D.DistanceSquared(this._questCaravanMobileParty.Position2D) < 9f))
				{
					Settlement settlement = this._visitedSettlements.LastOrDefault<Settlement>() ?? this._questCaravanMobileParty.HomeSettlement;
					Settlement targetSettlement = this._questCaravanMobileParty.TargetSettlement;
					if (targetSettlement == null)
					{
						this.TryToFindAndSetTargetToNextSettlement();
						return;
					}
					float distance = Campaign.Current.Models.MapDistanceModel.GetDistance(this._questCaravanMobileParty, targetSettlement);
					float distance2 = Campaign.Current.Models.MapDistanceModel.GetDistance(this._questCaravanMobileParty, settlement);
					float num = mobileParty.Position2D.DistanceSquared(this._questCaravanMobileParty.Position2D);
					if (distance > 5f && distance2 > 5f && num < 64f)
					{
						SetPartyAiAction.GetActionForEngagingParty(mobileParty, this._questCaravanMobileParty);
						mobileParty.Ai.SetDoNotMakeNewDecisions(true);
						if (!base.IsTracked(mobileParty))
						{
							base.AddTrackedObject(mobileParty);
						}
						float num2 = mobileParty.Speed + this._questCaravanMobileParty.Speed;
						this._otherBanditPartyFollowDuration = (int)(num / num2) + 5;
						this._otherBanditParty = mobileParty;
					}
				}
			}

			// Token: 0x060048AF RID: 18607 RVA: 0x001450A0 File Offset: 0x001432A0
			private void OnSettlementEntered(MobileParty party, Settlement settlement, Hero hero)
			{
				if (party == this._questCaravanMobileParty && settlement != this._questCaravanMobileParty.HomeSettlement && settlement.GatePosition.NearlyEquals(MobileParty.MainParty.Position2D, MobileParty.MainParty.SeeingRange + 2f) && settlement == this._questCaravanMobileParty.TargetSettlement)
				{
					this._visitedSettlements.Add(settlement);
					base.UpdateQuestTaskStage(this._playerStartsQuestLog, this._visitedSettlements.Count);
					TextObject textObject = new TextObject("{=0wj3HIbh}Caravan entered {SETTLEMENT_LINK}.", null);
					textObject.SetTextVariable("SETTLEMENT_LINK", settlement.EncyclopediaLinkWithName);
					base.AddLog(textObject, true);
					if (this._questBanditMobileParty != null && this._questBanditMobileParty.IsActive)
					{
						if (base.IsTracked(this._questBanditMobileParty))
						{
							base.RemoveTrackedObject(this._questBanditMobileParty);
						}
						this._questBanditMobileParty.Ai.SetDoNotMakeNewDecisions(false);
						this._questBanditMobileParty.IgnoreByOtherPartiesTill(CampaignTime.Now);
						if (this._questBanditMobileParty.MapEvent == null)
						{
							SetPartyAiAction.GetActionForPatrollingAroundSettlement(this._questBanditMobileParty, settlement);
						}
					}
					if (this._otherBanditParty != null)
					{
						if (base.IsTracked(this._otherBanditParty))
						{
							base.RemoveTrackedObject(this._otherBanditParty);
						}
						this._otherBanditParty.Ai.SetMoveModeHold();
						this._otherBanditParty.Ai.SetDoNotMakeNewDecisions(false);
						this._otherBanditParty = null;
					}
					if (this._visitedSettlements.Count == this._requiredSettlementNumber)
					{
						this.SuccessConsequences(false);
					}
				}
			}

			// Token: 0x060048B0 RID: 18608 RVA: 0x0014521F File Offset: 0x0014341F
			private void OnDailyTick()
			{
				this._daysSpentForEscorting++;
			}

			// Token: 0x060048B1 RID: 18609 RVA: 0x0014522F File Offset: 0x0014342F
			private void OnSettlementOwnerChanged(Settlement settlement, bool openToClaim, Hero newOwner, Hero oldOwner, Hero capturerHero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail detail)
			{
				this.CheckWarDeclaration();
			}

			// Token: 0x060048B2 RID: 18610 RVA: 0x00145237 File Offset: 0x00143437
			private void OnClanChangedKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail detail, bool showNotification = true)
			{
				this.CheckWarDeclaration();
			}

			// Token: 0x060048B3 RID: 18611 RVA: 0x0014523F File Offset: 0x0014343F
			private void CheckWarDeclaration()
			{
				if (base.QuestGiver.CurrentSettlement.MapFaction.IsAtWarWith(Hero.MainHero.MapFaction))
				{
					base.CompleteQuestWithCancel(this._cancelByWarQuestLogText);
				}
			}

			// Token: 0x060048B4 RID: 18612 RVA: 0x00145270 File Offset: 0x00143470
			private void OnWarDeclared(IFaction faction1, IFaction faction2, DeclareWarAction.DeclareWarDetail detail)
			{
				if (detail == DeclareWarAction.DeclareWarDetail.CausedByPlayerHostility && (faction1 == this._questCaravanMobileParty.MapFaction || faction2 == this._questCaravanMobileParty.MapFaction) && PlayerEncounter.Battle != null && this._questCaravanMobileParty.MapEvent == PlayerEncounter.Battle)
				{
					this.FailByPlayerHostileConsequences();
				}
				else
				{
					this.CheckWarDeclaration();
				}
				if (this._questCaravanMobileParty != null && (this._questCaravanMobileParty.TargetSettlement == null || this._questCaravanMobileParty.TargetSettlement.MapFaction.IsAtWarWith(this._questCaravanMobileParty.MapFaction)) && base.IsOngoing)
				{
					this.TryToFindAndSetTargetToNextSettlement();
				}
			}

			// Token: 0x060048B5 RID: 18613 RVA: 0x0014530C File Offset: 0x0014350C
			private void OnHourlyTick()
			{
				if (base.IsOngoing)
				{
					if (this._questCaravanMobileParty.TargetSettlement == null)
					{
						this.TryToFindAndSetTargetToNextSettlement();
					}
					if (this.CaravanIsInsideSettlement)
					{
						this.SimulateSettlementWaitForCaravan();
					}
					else if (this._questCaravanMobileParty.MapEvent == null)
					{
						this.AdjustCaravansSpeed();
					}
					this.NotifyPlayerOrCancelTheQuestIfCaravanIsFar();
					if (base.IsOngoing)
					{
						this.ThinkAboutSpawningBanditParty();
						this.CheckCaravanMapEvent();
						this._otherBanditPartyFollowDuration--;
						this._questBanditPartyFollowDuration--;
					}
				}
			}

			// Token: 0x060048B6 RID: 18614 RVA: 0x00145390 File Offset: 0x00143590
			private void CheckCaravanMapEvent()
			{
				if (this._questCaravanMobileParty.MapEvent != null && this._questCaravanMobileParty.MapEvent.IsInvulnerable && this._questCaravanMobileParty.MapEvent.BattleStartTime.ElapsedHoursUntilNow > 3f)
				{
					this._questCaravanMobileParty.MapEvent.IsInvulnerable = false;
				}
			}

			// Token: 0x060048B7 RID: 18615 RVA: 0x001453EC File Offset: 0x001435EC
			private void AdjustCaravansSpeed()
			{
				float num = MobileParty.MainParty.Speed;
				float num2 = this._questCaravanMobileParty.Speed;
				while (num < num2 || num - num2 > 1f)
				{
					if (num2 >= num)
					{
						this.CaravanCustomPartyComponent.SetBaseSpeed(this.CaravanCustomPartyComponent.BaseSpeed - 0.05f);
					}
					else if (num - num2 > 1f)
					{
						this.CaravanCustomPartyComponent.SetBaseSpeed(this.CaravanCustomPartyComponent.BaseSpeed + 0.05f);
					}
					num = MobileParty.MainParty.Speed;
					num2 = this._questCaravanMobileParty.Speed;
				}
			}

			// Token: 0x060048B8 RID: 18616 RVA: 0x00145480 File Offset: 0x00143680
			private void ThinkAboutSpawningBanditParty()
			{
				if (!this._questBanditPartyAlreadyAttacked && this._questBanditMobileParty == null)
				{
					Settlement targetSettlement = this._questCaravanMobileParty.TargetSettlement;
					if (targetSettlement != null)
					{
						float distance = Campaign.Current.Models.MapDistanceModel.GetDistance(this._questCaravanMobileParty, targetSettlement);
						if (distance > 10f && distance < 80f)
						{
							this.ActivateBanditParty();
							float num = this._questBanditMobileParty.Speed + this._questCaravanMobileParty.Speed;
							this._questBanditPartyFollowDuration = (int)(80f / num) + 5;
							this._questBanditPartyAlreadyAttacked = true;
						}
					}
				}
			}

			// Token: 0x060048B9 RID: 18617 RVA: 0x0014550D File Offset: 0x0014370D
			private void SimulateSettlementWaitForCaravan()
			{
				this._caravanWaitedInSettlementForHours++;
				if (this._caravanWaitedInSettlementForHours >= 5)
				{
					LeaveSettlementAction.ApplyForParty(this._questCaravanMobileParty);
					this._caravanWaitedInSettlementForHours = 0;
				}
			}

			// Token: 0x060048BA RID: 18618 RVA: 0x00145538 File Offset: 0x00143738
			private void NotifyPlayerOrCancelTheQuestIfCaravanIsFar()
			{
				if (this._questCaravanMobileParty.IsActive && !this._questCaravanMobileParty.IsVisible)
				{
					float num = this._questCaravanMobileParty.Position2D.Distance(MobileParty.MainParty.Position2D);
					if (!this._isPlayerNotifiedForDanger && num >= MobileParty.MainParty.SeeingRange + 3f)
					{
						MBInformationManager.AddQuickInformation(new TextObject("{=2y9DhzCR}You are about to lose sight of the caravan. Find the caravan before they are in danger!", null), 0, null, "");
						this._isPlayerNotifiedForDanger = true;
						return;
					}
					if (num >= MobileParty.MainParty.SeeingRange + 20f)
					{
						base.AddLog(this._caravanLostTheTrackLogText, false);
						this.FailConsequences(false);
					}
				}
			}

			// Token: 0x060048BB RID: 18619 RVA: 0x001455E8 File Offset: 0x001437E8
			private void OnSettlementLeft(MobileParty party, Settlement settlement)
			{
				if (party == this._questCaravanMobileParty)
				{
					this.AdjustCaravansSpeed();
					if (party.TargetSettlement == null || party.TargetSettlement == settlement)
					{
						this.TryToFindAndSetTargetToNextSettlement();
					}
					this._caravanWaitedInSettlementForHours = 0;
					this._questBanditPartyAlreadyAttacked = false;
					this._questCaravanMobileParty.Party.SetAsCameraFollowParty();
					if (base.IsTracked(settlement))
					{
						base.RemoveTrackedObject(settlement);
					}
				}
			}

			// Token: 0x060048BC RID: 18620 RVA: 0x0014564C File Offset: 0x0014384C
			private void TryToFindAndSetTargetToNextSettlement()
			{
				int num = 0;
				int num2 = -1;
				do
				{
					num2 = SettlementHelper.FindNextSettlementAroundMapPoint(this._questCaravanMobileParty, 150f, num2);
					if (num2 >= 0)
					{
						Settlement settlement = Settlement.All[num2];
						if (this.ProperSettlementCondition(settlement) && settlement != this._questCaravanMobileParty.HomeSettlement && (this._visitedSettlements.Count == 0 || settlement != this._visitedSettlements[this._visitedSettlements.Count - 1]) && !settlement.MapFaction.IsAtWarWith(this._questCaravanMobileParty.MapFaction))
						{
							num++;
						}
					}
				}
				while (num2 >= 0);
				if (num > 0)
				{
					int num3 = MBRandom.RandomInt(num);
					num2 = -1;
					Settlement settlement2;
					for (;;)
					{
						num2 = SettlementHelper.FindNextSettlementAroundMapPoint(this._questCaravanMobileParty, 150f, num2);
						if (num2 >= 0)
						{
							settlement2 = Settlement.All[num2];
							if (this.ProperSettlementCondition(settlement2) && settlement2 != this._questCaravanMobileParty.HomeSettlement && (this._visitedSettlements.Count == 0 || settlement2 != this._visitedSettlements[this._visitedSettlements.Count - 1]) && !settlement2.MapFaction.IsAtWarWith(this._questCaravanMobileParty.MapFaction))
							{
								num3--;
								if (num3 < 0)
								{
									break;
								}
							}
						}
						if (num2 < 0)
						{
							return;
						}
					}
					Settlement settlement3 = settlement2;
					SetPartyAiAction.GetActionForVisitingSettlement(this._questCaravanMobileParty, settlement3);
					this._questCaravanMobileParty.Ai.SetDoNotMakeNewDecisions(true);
					TextObject textObject = new TextObject("{=OjI8uGFa}We are traveling to {SETTLEMENT_NAME}.", null);
					textObject.SetTextVariable("SETTLEMENT_NAME", settlement3.Name);
					MBInformationManager.AddQuickInformation(textObject, 100, PartyBaseHelper.GetVisualPartyLeader(this._questCaravanMobileParty.Party), "");
					TextObject textObject2 = new TextObject("{=QDpfYm4c}The caravan is moving to {SETTLEMENT_NAME}.", null);
					textObject2.SetTextVariable("SETTLEMENT_NAME", settlement3.EncyclopediaLinkWithName);
					base.AddLog(textObject2, true);
					base.AddTrackedObject(settlement3);
					if (this._questBanditMobileParty != null && (this._questBanditMobileParty.Speed < this._questCaravanMobileParty.Speed || Campaign.Current.Models.MapDistanceModel.GetDistance(this._questCaravanMobileParty, this._questBanditMobileParty) > 10f))
					{
						this._questBanditMobileParty.Ai.SetDoNotMakeNewDecisions(false);
						this._questBanditMobileParty.IgnoreByOtherPartiesTill(CampaignTime.Now);
						if (base.IsTracked(this._questBanditMobileParty))
						{
							base.RemoveTrackedObject(this._questBanditMobileParty);
						}
						this._questBanditMobileParty = null;
						return;
					}
					return;
				}
				this.CaravanNoTargetQuestSuccess();
			}

			// Token: 0x060048BD RID: 18621 RVA: 0x001458B3 File Offset: 0x00143AB3
			private void CaravanNoTargetQuestSuccess()
			{
				this.SuccessConsequences(true);
			}

			// Token: 0x060048BE RID: 18622 RVA: 0x001458BC File Offset: 0x00143ABC
			private void OnMapEventEnded(MapEvent mapEvent)
			{
				if (this._questCaravanMobileParty != null && mapEvent.InvolvedParties.Contains(this._questCaravanMobileParty.Party))
				{
					if (mapEvent.HasWinner)
					{
						bool flag = this._questCaravanMobileParty.MapEventSide == MobileParty.MainParty.MapEventSide && mapEvent.IsPlayerMapEvent;
						bool flag2 = mapEvent.Winner == this._questCaravanMobileParty.MapEventSide;
						bool flag3 = mapEvent.InvolvedParties.Contains(PartyBase.MainParty);
						if (!flag2)
						{
							if (!flag3)
							{
								base.AddLog(this._caravanDestroyedByBanditsLogText, false);
								this.FailConsequences(true);
								return;
							}
							if (flag)
							{
								base.AddLog(this._caravanDestroyedQuestLogText, false);
								this.FailConsequences(true);
								return;
							}
							this.FailByPlayerHostileConsequences();
							return;
						}
						else
						{
							if (this._questBanditMobileParty != null && this._questBanditMobileParty.IsActive && mapEvent.InvolvedParties.Contains(this._questBanditMobileParty.Party))
							{
								DestroyPartyAction.Apply(MobileParty.MainParty.Party, this._questBanditMobileParty);
							}
							if (this._otherBanditParty != null && this._otherBanditParty.IsActive && mapEvent.InvolvedParties.Contains(this._otherBanditParty.Party))
							{
								DestroyPartyAction.Apply(MobileParty.MainParty.Party, this._otherBanditParty);
							}
							if (this._questCaravanMobileParty.MemberRoster.TotalManCount <= 0)
							{
								this.FailConsequences(true);
							}
							if (this._questCaravanMobileParty.Speed < 2f)
							{
								this._questCaravanMobileParty.ItemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("sumpter_horse"), 5);
								return;
							}
						}
					}
					else if (this._questCaravanMobileParty.MemberRoster.TotalManCount <= 0)
					{
						this.FailConsequences(true);
					}
				}
			}

			// Token: 0x060048BF RID: 18623 RVA: 0x00145A68 File Offset: 0x00143C68
			private void SuccessConsequences(bool isNoTargetLeftSuccess)
			{
				GiveGoldAction.ApplyBetweenCharacters(null, Hero.MainHero, this.TotalRewardGold, false);
				base.QuestGiver.AddPower(10f);
				this.RelationshipChangeWithQuestGiver = 5;
				TraitLevelingHelper.OnIssueSolvedThroughQuest(base.QuestGiver, new Tuple<TraitObject, int>[]
				{
					new Tuple<TraitObject, int>(DefaultTraits.Honor, 50)
				});
				base.QuestGiver.CurrentSettlement.Prosperity += 10f;
				if (isNoTargetLeftSuccess)
				{
					base.AddLog(this._caravanNoTargetLogText, false);
				}
				else
				{
					base.AddLog(this._successQuestLogText, true);
				}
				MobileParty questBanditMobileParty = this._questBanditMobileParty;
				if (questBanditMobileParty != null)
				{
					questBanditMobileParty.Ai.SetDoNotMakeNewDecisions(false);
				}
				base.CompleteQuestWithSuccess();
			}

			// Token: 0x060048C0 RID: 18624 RVA: 0x00145B18 File Offset: 0x00143D18
			private void FailConsequences(bool banditsWon = false)
			{
				base.QuestGiver.AddPower(-10f);
				this.RelationshipChangeWithQuestGiver = -5;
				TraitLevelingHelper.OnIssueFailed(base.QuestGiver, new Tuple<TraitObject, int>[]
				{
					new Tuple<TraitObject, int>(DefaultTraits.Honor, -20)
				});
				base.QuestGiver.CurrentSettlement.Prosperity -= 10f;
				if (this._questBanditMobileParty != null)
				{
					this._questBanditMobileParty.Ai.SetDoNotMakeNewDecisions(false);
					this._questBanditMobileParty.IgnoreByOtherPartiesTill(CampaignTime.Now);
					if (base.IsTracked(this._questBanditMobileParty))
					{
						base.RemoveTrackedObject(this._questBanditMobileParty);
					}
				}
				if (this._questCaravanMobileParty != null)
				{
					this._questCaravanMobileParty.Ai.SetDoNotMakeNewDecisions(false);
					this._questCaravanMobileParty.IgnoreByOtherPartiesTill(CampaignTime.Now);
				}
				if (this._questBanditMobileParty != null && !banditsWon)
				{
					if (base.IsTracked(this._questBanditMobileParty))
					{
						base.RemoveTrackedObject(this._questBanditMobileParty);
					}
					this._questBanditMobileParty.SetPartyUsedByQuest(false);
					this._questBanditMobileParty.IgnoreByOtherPartiesTill(CampaignTime.Now);
					if (this._questBanditMobileParty.IsActive && this._questBanditMobileParty.IsVisible)
					{
						DestroyPartyAction.Apply(null, this._questBanditMobileParty);
					}
				}
				base.CompleteQuestWithFail(null);
			}

			// Token: 0x060048C1 RID: 18625 RVA: 0x00145C54 File Offset: 0x00143E54
			private void FailByPlayerHostileConsequences()
			{
				base.QuestGiver.AddPower(-10f);
				this.RelationshipChangeWithQuestGiver = -10;
				TraitLevelingHelper.OnIssueFailed(base.QuestGiver, new Tuple<TraitObject, int>[]
				{
					new Tuple<TraitObject, int>(DefaultTraits.Honor, -80)
				});
				base.QuestGiver.CurrentSettlement.Prosperity -= 20f;
				base.AddLog(this._caravanDestroyedByPlayerQuestLogText, true);
				MobileParty questBanditMobileParty = this._questBanditMobileParty;
				if (questBanditMobileParty != null)
				{
					questBanditMobileParty.Ai.SetDoNotMakeNewDecisions(false);
				}
				base.CompleteQuestWithFail(null);
			}

			// Token: 0x060048C2 RID: 18626 RVA: 0x00145CE1 File Offset: 0x00143EE1
			protected override void InitializeQuestOnGameLoad()
			{
				MobileParty questCaravanMobileParty = this._questCaravanMobileParty;
				if (questCaravanMobileParty != null && questCaravanMobileParty.IsCaravan)
				{
					base.CompleteQuestWithCancel(null);
				}
				this.SetDialogs();
			}

			// Token: 0x060048C3 RID: 18627 RVA: 0x00145D04 File Offset: 0x00143F04
			private void ActivateBanditParty()
			{
				Settlement closestHideout = SettlementHelper.FindNearestHideout((Settlement x) => x.IsActive, null);
				Clan clan = Clan.BanditFactions.FirstOrDefault((Clan t) => t.Culture == closestHideout.Culture);
				this._questBanditMobileParty = BanditPartyComponent.CreateBanditParty("escort_caravan_quest_" + base.StringId, clan, closestHideout.Hideout, false);
				PartyTemplateObject partyTemplateObject = Campaign.Current.ObjectManager.GetObject<PartyTemplateObject>("kingdom_hero_party_caravan_ambushers") ?? clan.DefaultPartyTemplate;
				this._questBanditMobileParty.InitializeMobilePartyAroundPosition(partyTemplateObject, this._questCaravanMobileParty.TargetSettlement.GatePosition, 1f, 0.5f, -1);
				this._questBanditMobileParty.SetCustomName(new TextObject("{=u1Pkt4HC}Raiders", null));
				Campaign.Current.MobilePartyLocator.UpdateLocator(this._questBanditMobileParty);
				this._questBanditMobileParty.ActualClan = clan;
				this._questBanditMobileParty.MemberRoster.Clear();
				for (int i = 0; i < this.BanditPartyTroopCount; i++)
				{
					List<ValueTuple<PartyTemplateStack, float>> list = new List<ValueTuple<PartyTemplateStack, float>>();
					foreach (PartyTemplateStack partyTemplateStack in partyTemplateObject.Stacks)
					{
						list.Add(new ValueTuple<PartyTemplateStack, float>(partyTemplateStack, (float)(64 - partyTemplateStack.Character.Level)));
					}
					PartyTemplateStack partyTemplateStack2 = MBRandom.ChooseWeighted<PartyTemplateStack>(list);
					this._questBanditMobileParty.MemberRoster.AddToCounts(partyTemplateStack2.Character, 1, false, 0, 0, true, -1);
				}
				this._questBanditMobileParty.ItemRoster.AddToCounts(DefaultItems.Grain, this.BanditPartyTroopCount);
				this._questBanditMobileParty.ItemRoster.AddToCounts(MBObjectManager.Instance.GetObject<ItemObject>("sumpter_horse"), this.BanditPartyTroopCount);
				this._questBanditMobileParty.IgnoreByOtherPartiesTill(base.QuestDueTime);
				SetPartyAiAction.GetActionForEngagingParty(this._questBanditMobileParty, this._questCaravanMobileParty);
				this._questBanditMobileParty.Ai.SetDoNotMakeNewDecisions(true);
				base.AddTrackedObject(this._questBanditMobileParty);
			}

			// Token: 0x060048C4 RID: 18628 RVA: 0x00145F34 File Offset: 0x00144134
			protected override void OnFinalize()
			{
				if (this._questCaravanMobileParty != null && this._questCaravanMobileParty.IsActive && this._questCaravanMobileParty.IsCustomParty)
				{
					this._questCaravanMobileParty.PartyComponent = new CaravanPartyComponent(base.QuestGiver.CurrentSettlement, base.QuestGiver, null);
					this._questCaravanMobileParty.Ai.SetDoNotMakeNewDecisions(false);
					this._questCaravanMobileParty.IgnoreByOtherPartiesTill(CampaignTime.Now);
				}
				if (this._questCaravanMobileParty != null)
				{
					base.RemoveTrackedObject(this._questCaravanMobileParty);
				}
				if (this._otherBanditParty != null && this._otherBanditParty.IsActive)
				{
					this._otherBanditParty.Ai.SetDoNotMakeNewDecisions(false);
					this._otherBanditParty.IgnoreByOtherPartiesTill(CampaignTime.Now);
				}
			}

			// Token: 0x060048C5 RID: 18629 RVA: 0x00145FF0 File Offset: 0x001441F0
			protected override void OnTimedOut()
			{
				base.QuestGiver.AddPower(-5f);
				this.RelationshipChangeWithQuestGiver = -5;
				TraitLevelingHelper.OnIssueFailed(base.QuestGiver, new Tuple<TraitObject, int>[]
				{
					new Tuple<TraitObject, int>(DefaultTraits.Honor, -20)
				});
				base.QuestGiver.CurrentSettlement.Prosperity -= 20f;
				base.AddLog(new TextObject("{=pUrSIed8}You have failed to escort the caravan to its destination.", null), false);
			}

			// Token: 0x0400192D RID: 6445
			private const int BanditPartyAttackRadiusMin = 8;

			// Token: 0x0400192E RID: 6446
			private const int BattleFakeSimulationDuration = 3;

			// Token: 0x0400192F RID: 6447
			private const int QuestBanditPartySpawnDistance = 80;

			// Token: 0x04001930 RID: 6448
			private const string CustomPartyComponentTalkId = "escort_caravan_talk";

			// Token: 0x04001931 RID: 6449
			[SaveableField(2)]
			private readonly int _requiredSettlementNumber;

			// Token: 0x04001932 RID: 6450
			[SaveableField(3)]
			private List<Settlement> _visitedSettlements;

			// Token: 0x04001933 RID: 6451
			[SaveableField(4)]
			private MobileParty _questCaravanMobileParty;

			// Token: 0x04001934 RID: 6452
			[SaveableField(5)]
			private MobileParty _questBanditMobileParty;

			// Token: 0x04001935 RID: 6453
			[SaveableField(7)]
			private readonly float _difficultyMultiplier;

			// Token: 0x04001936 RID: 6454
			[SaveableField(12)]
			private bool _isPlayerNotifiedForDanger;

			// Token: 0x04001937 RID: 6455
			[SaveableField(26)]
			private MobileParty _otherBanditParty;

			// Token: 0x04001938 RID: 6456
			[SaveableField(30)]
			private int _questBanditPartyFollowDuration;

			// Token: 0x04001939 RID: 6457
			[SaveableField(31)]
			private int _otherBanditPartyFollowDuration;

			// Token: 0x0400193A RID: 6458
			[SaveableField(11)]
			private int _daysSpentForEscorting = 1;

			// Token: 0x0400193B RID: 6459
			private int _caravanWaitedInSettlementForHours;

			// Token: 0x0400193C RID: 6460
			[SaveableField(23)]
			private bool _questBanditPartyAlreadyAttacked;

			// Token: 0x0400193D RID: 6461
			private CustomPartyComponent _customPartyComponent;

			// Token: 0x0400193E RID: 6462
			[SaveableField(1)]
			private JournalLog _playerStartsQuestLog;
		}

		// Token: 0x02000613 RID: 1555
		public class EscortMerchantCaravanIssueTypeDefiner : SaveableTypeDefiner
		{
			// Token: 0x060048C8 RID: 18632 RVA: 0x00146082 File Offset: 0x00144282
			public EscortMerchantCaravanIssueTypeDefiner()
				: base(450000)
			{
			}

			// Token: 0x060048C9 RID: 18633 RVA: 0x0014608F File Offset: 0x0014428F
			protected override void DefineClassTypes()
			{
				base.AddClassDefinition(typeof(EscortMerchantCaravanIssueBehavior.EscortMerchantCaravanIssue), 1, null);
				base.AddClassDefinition(typeof(EscortMerchantCaravanIssueBehavior.EscortMerchantCaravanIssueQuest), 2, null);
			}
		}
	}
}
