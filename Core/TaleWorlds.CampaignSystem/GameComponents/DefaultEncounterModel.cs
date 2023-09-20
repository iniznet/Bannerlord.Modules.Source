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
	public class DefaultEncounterModel : EncounterModel
	{
		public override float EstimatedMaximumMobilePartySpeedExceptPlayer
		{
			get
			{
				return 10f;
			}
		}

		public override float NeededMaximumDistanceForEncounteringMobileParty
		{
			get
			{
				return 0.5f;
			}
		}

		public override float MaximumAllowedDistanceForEncounteringMobilePartyInArmy
		{
			get
			{
				return 1.5f;
			}
		}

		public override float NeededMaximumDistanceForEncounteringTown
		{
			get
			{
				return 0.05f;
			}
		}

		public override float NeededMaximumDistanceForEncounteringVillage
		{
			get
			{
				return 1f;
			}
		}

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

		public override Hero GetLeaderOfSiegeEvent(SiegeEvent siegeEvent, BattleSideEnum side)
		{
			return this.GetLeaderOfEventInternal(siegeEvent.GetSiegeEventSide(side).GetInvolvedPartiesForEventType(MapEvent.BattleTypes.Siege).ToList<PartyBase>());
		}

		public override Hero GetLeaderOfMapEvent(MapEvent mapEvent, BattleSideEnum side)
		{
			return this.GetLeaderOfEventInternal(mapEvent.GetMapEventSide(side).Parties.Select((MapEventParty x) => x.Party).ToList<PartyBase>());
		}

		private bool IsFactionLeader(Hero hero)
		{
			return hero.MapFaction.IsKingdomFaction && hero.IsFactionLeader;
		}

		private bool IsArmyLeader(Hero hero)
		{
			MobileParty partyBelongedTo = hero.PartyBelongedTo;
			return ((partyBelongedTo != null) ? partyBelongedTo.Army : null) != null && hero.PartyBelongedTo.Army.LeaderParty == hero.PartyBelongedTo;
		}

		private int GetLeadingScore(Hero hero)
		{
			if (!this.IsFactionLeader(hero) && !this.IsArmyLeader(hero))
			{
				return this.GetCharacterSergeantScore(hero);
			}
			return (int)hero.PartyBelongedTo.GetTotalStrengthWithFollowers(true);
		}

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
