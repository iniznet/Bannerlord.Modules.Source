using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.Core;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	// Token: 0x02000109 RID: 265
	public class DefaultEncounterModel : EncounterModel
	{
		// Token: 0x170005EA RID: 1514
		// (get) Token: 0x06001584 RID: 5508 RVA: 0x00065CCB File Offset: 0x00063ECB
		public override float EstimatedMaximumMobilePartySpeedExceptPlayer
		{
			get
			{
				return 10f;
			}
		}

		// Token: 0x170005EB RID: 1515
		// (get) Token: 0x06001585 RID: 5509 RVA: 0x00065CD2 File Offset: 0x00063ED2
		public override float NeededMaximumDistanceForEncounteringMobileParty
		{
			get
			{
				return 0.5f;
			}
		}

		// Token: 0x170005EC RID: 1516
		// (get) Token: 0x06001586 RID: 5510 RVA: 0x00065CD9 File Offset: 0x00063ED9
		public override float MaximumAllowedDistanceForEncounteringMobilePartyInArmy
		{
			get
			{
				return 1.5f;
			}
		}

		// Token: 0x170005ED RID: 1517
		// (get) Token: 0x06001587 RID: 5511 RVA: 0x00065CE0 File Offset: 0x00063EE0
		public override float NeededMaximumDistanceForEncounteringTown
		{
			get
			{
				return 0.05f;
			}
		}

		// Token: 0x170005EE RID: 1518
		// (get) Token: 0x06001588 RID: 5512 RVA: 0x00065CE7 File Offset: 0x00063EE7
		public override float NeededMaximumDistanceForEncounteringVillage
		{
			get
			{
				return 1f;
			}
		}

		// Token: 0x06001589 RID: 5513 RVA: 0x00065CF0 File Offset: 0x00063EF0
		public override bool IsEncounterExemptFromHostileActions(PartyBase side1, PartyBase side2)
		{
			if (side1 != null && side2 != null)
			{
				Hero owner = side1.Owner;
				if (((owner != null) ? owner.Clan : null) != CampaignData.NeutralFaction)
				{
					Hero owner2 = side2.Owner;
					if (((owner2 != null) ? owner2.Clan : null) != CampaignData.NeutralFaction && (!side1.IsMobile || side1.MobileParty.ActualClan != CampaignData.NeutralFaction) && (!side2.IsMobile || side2.MobileParty.ActualClan != CampaignData.NeutralFaction) && (!side1.IsMobile || !side1.MobileParty.IsCustomParty || !(side1.MobileParty.PartyComponent as CustomPartyComponent).AvoidHostileActions))
					{
						return side2.IsMobile && side2.MobileParty.IsCustomParty && (side2.MobileParty.PartyComponent as CustomPartyComponent).AvoidHostileActions;
					}
				}
			}
			return true;
		}

		// Token: 0x0600158A RID: 5514 RVA: 0x00065DCF File Offset: 0x00063FCF
		public override Hero GetLeaderOfSiegeEvent(SiegeEvent siegeEvent, BattleSideEnum side)
		{
			return this.GetLeaderOfEventInternal(siegeEvent.GetSiegeEventSide(side).GetInvolvedPartiesForEventType(MapEvent.BattleTypes.Siege).ToList<PartyBase>());
		}

		// Token: 0x0600158B RID: 5515 RVA: 0x00065DE9 File Offset: 0x00063FE9
		public override Hero GetLeaderOfMapEvent(MapEvent mapEvent, BattleSideEnum side)
		{
			return this.GetLeaderOfEventInternal(mapEvent.GetMapEventSide(side).Parties.Select((MapEventParty x) => x.Party).ToList<PartyBase>());
		}

		// Token: 0x0600158C RID: 5516 RVA: 0x00065E26 File Offset: 0x00064026
		private bool IsFactionLeader(Hero hero)
		{
			return hero.MapFaction.IsKingdomFaction && hero.IsFactionLeader;
		}

		// Token: 0x0600158D RID: 5517 RVA: 0x00065E3D File Offset: 0x0006403D
		private bool IsArmyLeader(Hero hero)
		{
			MobileParty partyBelongedTo = hero.PartyBelongedTo;
			return ((partyBelongedTo != null) ? partyBelongedTo.Army : null) != null && hero.PartyBelongedTo.Army.LeaderParty == hero.PartyBelongedTo;
		}

		// Token: 0x0600158E RID: 5518 RVA: 0x00065E6D File Offset: 0x0006406D
		private int GetLeadingScore(Hero hero)
		{
			if (!this.IsFactionLeader(hero) && !this.IsArmyLeader(hero))
			{
				return this.GetCharacterSergeantScore(hero);
			}
			return (int)hero.PartyBelongedTo.GetTotalStrengthWithFollowers(true);
		}

		// Token: 0x0600158F RID: 5519 RVA: 0x00065E98 File Offset: 0x00064098
		private Hero GetLeaderOfEventInternal(List<PartyBase> allPartiesThatBelongToASide)
		{
			Hero hero = null;
			int num = 0;
			foreach (PartyBase partyBase in allPartiesThatBelongToASide)
			{
				Hero leaderHero = partyBase.LeaderHero;
				if (leaderHero != null)
				{
					int leadingScore = this.GetLeadingScore(leaderHero);
					if (hero == null)
					{
						hero = leaderHero;
						num = leadingScore;
					}
					bool flag = this.IsFactionLeader(leaderHero);
					bool flag2 = this.IsArmyLeader(leaderHero);
					bool flag3 = this.IsFactionLeader(hero);
					bool flag4 = this.IsArmyLeader(hero);
					if (flag)
					{
						if (!flag3 || leadingScore > num)
						{
							hero = leaderHero;
							num = leadingScore;
						}
					}
					else if (flag2)
					{
						if ((!flag3 && !flag4) || (flag4 && !flag3 && leadingScore > num))
						{
							hero = leaderHero;
							num = leadingScore;
						}
					}
					else if (!flag3 && !flag4 && leadingScore > num)
					{
						hero = leaderHero;
						num = leadingScore;
					}
				}
			}
			return hero;
		}

		// Token: 0x06001590 RID: 5520 RVA: 0x00065F6C File Offset: 0x0006416C
		public override int GetCharacterSergeantScore(Hero hero)
		{
			int num = 0;
			Clan clan = hero.Clan;
			if (clan != null)
			{
				num += clan.Tier * ((hero == clan.Leader) ? 100 : 20);
				if (clan.Kingdom != null && clan.Kingdom.Leader == hero)
				{
					num += 2000;
				}
			}
			MobileParty partyBelongedTo = hero.PartyBelongedTo;
			if (partyBelongedTo != null)
			{
				if (partyBelongedTo.Army != null && partyBelongedTo.Army.LeaderParty == partyBelongedTo)
				{
					num += partyBelongedTo.Army.Parties.Count * 200;
				}
				num += partyBelongedTo.MemberRoster.TotalManCount - partyBelongedTo.MemberRoster.TotalWounded;
			}
			return num;
		}

		// Token: 0x06001591 RID: 5521 RVA: 0x00066010 File Offset: 0x00064210
		public override IEnumerable<PartyBase> GetDefenderPartiesOfSettlement(Settlement settlement, MapEvent.BattleTypes mapEventType)
		{
			if (settlement.IsFortification)
			{
				return settlement.Town.GetDefenderParties(mapEventType);
			}
			if (settlement.IsVillage)
			{
				return settlement.Village.GetDefenderParties(mapEventType);
			}
			if (settlement.IsHideout)
			{
				return settlement.Hideout.GetDefenderParties(mapEventType);
			}
			return null;
		}

		// Token: 0x06001592 RID: 5522 RVA: 0x00066060 File Offset: 0x00064260
		public override PartyBase GetNextDefenderPartyOfSettlement(Settlement settlement, ref int partyIndex, MapEvent.BattleTypes mapEventType)
		{
			if (settlement.IsFortification)
			{
				return settlement.Town.GetNextDefenderParty(ref partyIndex, mapEventType);
			}
			if (settlement.IsVillage)
			{
				return settlement.Village.GetNextDefenderParty(ref partyIndex, mapEventType);
			}
			if (settlement.IsHideout)
			{
				return settlement.Hideout.GetNextDefenderParty(ref partyIndex, mapEventType);
			}
			return null;
		}
	}
}
