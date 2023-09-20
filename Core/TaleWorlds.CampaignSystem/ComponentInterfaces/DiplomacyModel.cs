using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.BarterSystem;
using TaleWorlds.CampaignSystem.LogEntries;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.ComponentInterfaces
{
	// Token: 0x02000184 RID: 388
	public abstract class DiplomacyModel : GameModel
	{
		// Token: 0x17000690 RID: 1680
		// (get) Token: 0x06001970 RID: 6512
		public abstract int MaxRelationLimit { get; }

		// Token: 0x17000691 RID: 1681
		// (get) Token: 0x06001971 RID: 6513
		public abstract int MinRelationLimit { get; }

		// Token: 0x17000692 RID: 1682
		// (get) Token: 0x06001972 RID: 6514
		public abstract int MaxNeutralRelationLimit { get; }

		// Token: 0x17000693 RID: 1683
		// (get) Token: 0x06001973 RID: 6515
		public abstract int MinNeutralRelationLimit { get; }

		// Token: 0x17000694 RID: 1684
		// (get) Token: 0x06001974 RID: 6516
		public abstract int MinimumRelationWithConversationCharacterToJoinKingdom { get; }

		// Token: 0x17000695 RID: 1685
		// (get) Token: 0x06001975 RID: 6517
		public abstract int GiftingTownRelationshipBonus { get; }

		// Token: 0x17000696 RID: 1686
		// (get) Token: 0x06001976 RID: 6518
		public abstract int GiftingCastleRelationshipBonus { get; }

		// Token: 0x06001977 RID: 6519
		public abstract float GetStrengthThresholdForNonMutualWarsToBeIgnoredToJoinKingdom(Kingdom kingdomToJoin);

		// Token: 0x06001978 RID: 6520
		public abstract float GetRelationIncreaseFactor(Hero hero1, Hero hero2, float relationValue);

		// Token: 0x06001979 RID: 6521
		public abstract int GetInfluenceAwardForSettlementCapturer(Settlement settlement);

		// Token: 0x0600197A RID: 6522
		public abstract float GetHourlyInfluenceAwardForRaidingEnemyVillage(MobileParty mobileParty);

		// Token: 0x0600197B RID: 6523
		public abstract float GetHourlyInfluenceAwardForBesiegingEnemyFortification(MobileParty mobileParty);

		// Token: 0x0600197C RID: 6524
		public abstract float GetHourlyInfluenceAwardForBeingArmyMember(MobileParty mobileParty);

		// Token: 0x0600197D RID: 6525
		public abstract float GetScoreOfClanToJoinKingdom(Clan clan, Kingdom kingdom);

		// Token: 0x0600197E RID: 6526
		public abstract float GetScoreOfClanToLeaveKingdom(Clan clan, Kingdom kingdom);

		// Token: 0x0600197F RID: 6527
		public abstract float GetScoreOfKingdomToGetClan(Kingdom kingdom, Clan clan);

		// Token: 0x06001980 RID: 6528
		public abstract float GetScoreOfKingdomToSackClan(Kingdom kingdom, Clan clan);

		// Token: 0x06001981 RID: 6529
		public abstract float GetScoreOfMercenaryToJoinKingdom(Clan clan, Kingdom kingdom);

		// Token: 0x06001982 RID: 6530
		public abstract float GetScoreOfMercenaryToLeaveKingdom(Clan clan, Kingdom kingdom);

		// Token: 0x06001983 RID: 6531
		public abstract float GetScoreOfKingdomToHireMercenary(Kingdom kingdom, Clan mercenaryClan);

		// Token: 0x06001984 RID: 6532
		public abstract float GetScoreOfKingdomToSackMercenary(Kingdom kingdom, Clan mercenaryClan);

		// Token: 0x06001985 RID: 6533
		public abstract float GetScoreOfDeclaringPeace(IFaction factionDeclaresPeace, IFaction factionDeclaredPeace, IFaction evaluatingFaction, out TextObject reason);

		// Token: 0x06001986 RID: 6534
		public abstract float GetScoreOfDeclaringWar(IFaction factionDeclaresWar, IFaction factionDeclaredWar, IFaction evaluatingFaction, out TextObject reason);

		// Token: 0x06001987 RID: 6535
		public abstract float GetScoreOfLettingPartyGo(MobileParty party, MobileParty partyToLetGo);

		// Token: 0x06001988 RID: 6536
		public abstract float GetValueOfHeroForFaction(Hero examinedHero, IFaction targetFaction, bool forMarriage = false);

		// Token: 0x06001989 RID: 6537
		public abstract int GetRelationCostOfExpellingClanFromKingdom();

		// Token: 0x0600198A RID: 6538
		public abstract int GetInfluenceCostOfSupportingClan();

		// Token: 0x0600198B RID: 6539
		public abstract int GetInfluenceCostOfExpellingClan();

		// Token: 0x0600198C RID: 6540
		public abstract int GetInfluenceCostOfProposingPeace();

		// Token: 0x0600198D RID: 6541
		public abstract int GetInfluenceCostOfProposingWar(Kingdom proposingKingdom);

		// Token: 0x0600198E RID: 6542
		public abstract int GetInfluenceValueOfSupportingClan();

		// Token: 0x0600198F RID: 6543
		public abstract int GetRelationValueOfSupportingClan();

		// Token: 0x06001990 RID: 6544
		public abstract int GetInfluenceCostOfAnnexation(Kingdom proposingKingdom);

		// Token: 0x06001991 RID: 6545
		public abstract int GetInfluenceCostOfChangingLeaderOfArmy();

		// Token: 0x06001992 RID: 6546
		public abstract int GetInfluenceCostOfDisbandingArmy();

		// Token: 0x06001993 RID: 6547
		public abstract int GetRelationCostOfDisbandingArmy(bool isLeaderParty);

		// Token: 0x06001994 RID: 6548
		public abstract int GetInfluenceCostOfPolicyProposalAndDisavowal();

		// Token: 0x06001995 RID: 6549
		public abstract int GetInfluenceCostOfAbandoningArmy();

		// Token: 0x06001996 RID: 6550
		public abstract int GetEffectiveRelation(Hero hero, Hero hero1);

		// Token: 0x06001997 RID: 6551
		public abstract int GetBaseRelation(Hero hero, Hero hero1);

		// Token: 0x06001998 RID: 6552
		public abstract void GetHeroesForEffectiveRelation(Hero hero1, Hero hero2, out Hero effectiveHero1, out Hero effectiveHero2);

		// Token: 0x06001999 RID: 6553
		public abstract int GetRelationChangeAfterClanLeaderIsDead(Hero deadLeader, Hero relationHero);

		// Token: 0x0600199A RID: 6554
		public abstract int GetRelationChangeAfterVotingInSettlementOwnerPreliminaryDecision(Hero supporter, bool hasHeroVotedAgainstOwner);

		// Token: 0x0600199B RID: 6555
		public abstract float GetClanStrength(Clan clan);

		// Token: 0x0600199C RID: 6556
		public abstract float GetHeroCommandingStrengthForClan(Hero hero);

		// Token: 0x0600199D RID: 6557
		public abstract float GetHeroGoverningStrengthForClan(Hero hero);

		// Token: 0x0600199E RID: 6558
		public abstract uint GetNotificationColor(ChatNotificationType notificationType);

		// Token: 0x0600199F RID: 6559
		public abstract int GetValueOfDailyTribute(int dailyTributeAmount);

		// Token: 0x060019A0 RID: 6560
		public abstract int GetDailyTributeForValue(int value);

		// Token: 0x060019A1 RID: 6561
		public abstract bool CanSettlementBeGifted(Settlement settlement);

		// Token: 0x060019A2 RID: 6562
		public abstract bool IsClanEligibleToBecomeRuler(Clan clan);

		// Token: 0x060019A3 RID: 6563
		public abstract IEnumerable<BarterGroup> GetBarterGroups();

		// Token: 0x060019A4 RID: 6564
		public abstract int GetCharmExperienceFromRelationGain(Hero hero, float relationChange, ChangeRelationAction.ChangeRelationDetail detail);

		// Token: 0x060019A5 RID: 6565
		public abstract float DenarsToInfluence();
	}
}
