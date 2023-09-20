using System;
using System.Collections.Generic;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	public class CharacterRelationCampaignBehavior : CampaignBehaviorBase
	{
		public override void RegisterEvents()
		{
			CampaignEvents.OnSettlementOwnerChangedEvent.AddNonSerializedListener(this, new Action<Settlement, bool, Hero, Hero, Hero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail>(this.OnSettlementOwnerChanged));
			CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new Action(this.DailyTick));
			CampaignEvents.OnNewGameCreatedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.UpdateFriendshipAndEnemies));
			CampaignEvents.RaidCompletedEvent.AddNonSerializedListener(this, new Action<BattleSideEnum, RaidEventComponent>(this.OnRaidCompleted));
			CampaignEvents.DailyTickPartyEvent.AddNonSerializedListener(this, new Action<MobileParty>(this.DailyTickParty));
			CampaignEvents.MapEventEnded.AddNonSerializedListener(this, new Action<MapEvent>(this.MapEventEnded));
			CampaignEvents.OnPrisonerDonatedToSettlementEvent.AddNonSerializedListener(this, new Action<MobileParty, FlattenedTroopRoster, Settlement>(this.OnPrisonerDonatedToSettlement));
			CampaignEvents.HeroRelationChanged.AddNonSerializedListener(this, new Action<Hero, Hero, int, bool, ChangeRelationAction.ChangeRelationDetail, Hero, Hero>(this.OnHeroRelationChanged));
			CampaignEvents.HeroesMarried.AddNonSerializedListener(this, new Action<Hero, Hero, bool>(CharacterRelationCampaignBehavior.OnHeroesMarried));
			CampaignEvents.OnHeroUnregisteredEvent.AddNonSerializedListener(this, new Action<Hero>(this.OnHeroUnregistered));
		}

		private void OnHeroUnregistered(Hero hero)
		{
			Campaign.Current.CharacterRelationManager.RemoveHero(hero);
		}

		private void OnHeroRelationChanged(Hero effectiveHero, Hero effectiveHeroGainedRelationWith, int relationChange, bool showNotification, ChangeRelationAction.ChangeRelationDetail detail, Hero originalHero, Hero originalGainedRelationWith)
		{
			if (relationChange > 0)
			{
				SkillLevelingManager.OnGainRelation(originalHero, effectiveHeroGainedRelationWith, (float)relationChange, detail);
			}
		}

		private void MapEventEnded(MapEvent mapEvent)
		{
			if ((mapEvent.EventType == MapEvent.BattleTypes.FieldBattle || mapEvent.EventType == MapEvent.BattleTypes.Siege || mapEvent.EventType == MapEvent.BattleTypes.SiegeOutside) && mapEvent.HasWinner)
			{
				MapEventSide winnerSide = mapEvent.Winner;
				MapEventSide otherSide = winnerSide.OtherSide;
				bool flag = false;
				foreach (MapEventParty mapEventParty in otherSide.Parties)
				{
					if (mapEventParty.Party.IsMobile && mapEventParty.Party.MobileParty.IsLordParty)
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					Hero leaderHero = winnerSide.LeaderParty.LeaderHero;
					if (leaderHero != null && leaderHero.GetPerkValue(DefaultPerks.Charm.Oratory))
					{
						Hero randomElementWithPredicate = Hero.AllAliveHeroes.GetRandomElementWithPredicate(delegate(Hero x)
						{
							if (x.IsActive && x.IsNotable)
							{
								Settlement currentSettlement = x.CurrentSettlement;
								return ((currentSettlement != null) ? currentSettlement.MapFaction : null) == winnerSide.LeaderParty.MapFaction;
							}
							return false;
						});
						if (randomElementWithPredicate != null)
						{
							ChangeRelationAction.ApplyRelationChangeBetweenHeroes(winnerSide.LeaderParty.LeaderHero, randomElementWithPredicate, (int)DefaultPerks.Charm.Oratory.SecondaryBonus, true);
						}
					}
					Hero leaderHero2 = winnerSide.LeaderParty.LeaderHero;
					if (leaderHero2 != null && leaderHero2.GetPerkValue(DefaultPerks.Charm.Warlord))
					{
						Hero randomElementWithPredicate2 = winnerSide.LeaderParty.MapFaction.Lords.GetRandomElementWithPredicate((Hero x) => x.IsAlive && x != winnerSide.LeaderParty.LeaderHero);
						if (randomElementWithPredicate2 != null)
						{
							ChangeRelationAction.ApplyRelationChangeBetweenHeroes(winnerSide.LeaderParty.LeaderHero, randomElementWithPredicate2, (int)DefaultPerks.Charm.Warlord.SecondaryBonus, true);
						}
					}
				}
			}
		}

		private void OnPrisonerDonatedToSettlement(MobileParty donatingParty, FlattenedTroopRoster donatedPrisoners, Settlement donatedSettlement)
		{
			if (donatingParty.IsMainParty)
			{
				foreach (FlattenedTroopRosterElement flattenedTroopRosterElement in donatedPrisoners)
				{
					if (flattenedTroopRosterElement.Troop.IsHero)
					{
						float num = Campaign.Current.Models.PrisonerDonationModel.CalculateRelationGainAfterHeroPrisonerDonate(donatingParty.Party, flattenedTroopRosterElement.Troop.HeroObject, donatedSettlement);
						if (num != 0f)
						{
							ChangeRelationAction.ApplyRelationChangeBetweenHeroes(Hero.MainHero, donatedSettlement.OwnerClan.Leader, (int)num, true);
						}
					}
				}
			}
		}

		public override void SyncData(IDataStore dataStore)
		{
		}

		private void UpdateFriendshipAndEnemies(CampaignGameStarter campaignGameStarter)
		{
			List<Hero> list = new List<Hero>(Campaign.Current.AliveHeroes.Count + Campaign.Current.DeadOrDisabledHeroes.Count);
			foreach (Hero hero in Campaign.Current.AliveHeroes)
			{
				if (hero.IsLord && hero != Hero.MainHero && hero.MapFaction != null)
				{
					IFaction mapFaction = hero.MapFaction;
					if (((mapFaction != null) ? mapFaction.Leader : null) != Hero.MainHero)
					{
						list.Add(hero);
					}
				}
			}
			foreach (Hero hero2 in Campaign.Current.DeadOrDisabledHeroes)
			{
				if (hero2.IsLord && hero2 != Hero.MainHero && hero2.MapFaction != null)
				{
					IFaction mapFaction2 = hero2.MapFaction;
					if (((mapFaction2 != null) ? mapFaction2.Leader : null) != Hero.MainHero)
					{
						list.Add(hero2);
					}
				}
			}
			for (int i = 0; i < list.Count; i++)
			{
				Hero hero3 = list[i];
				for (int j = i + 1; j < list.Count; j++)
				{
					Hero hero4 = list[j];
					float distance = Campaign.Current.Models.MapDistanceModel.GetDistance(hero3.MapFaction.FactionMidSettlement, hero4.MapFaction.FactionMidSettlement);
					float num = 1f / (2f + 5f * (distance / Campaign.MaximumDistanceBetweenTwoSettlements));
					if (hero3 == hero3.MapFaction.Leader || hero4 == hero4.MapFaction.Leader)
					{
						num = MathF.Sqrt(num);
					}
					if (MBRandom.RandomFloat < num)
					{
						float num2 = MBRandom.RandomFloat;
						float num3 = (float)HeroHelper.NPCPersonalityClashWithNPC(hero3, hero4) * 0.01f;
						float randomFloat = MBRandom.RandomFloat;
						num2 += num3 * randomFloat;
						num2 -= 0.04f;
						if (hero3.MapFaction == hero4.MapFaction)
						{
							float num4 = 1f - MBRandom.RandomFloat * MBRandom.RandomFloat;
							num2 *= num4;
							if (hero3 == hero3.MapFaction.Leader || hero4 == hero4.MapFaction.Leader)
							{
								num4 = 1f - MBRandom.RandomFloat * MBRandom.RandomFloat;
								num2 *= num4;
							}
						}
						else
						{
							float num5 = 1f + MBRandom.RandomFloat * MBRandom.RandomFloat * MBRandom.RandomFloat;
							num2 *= num5;
						}
						this.DetermineRelation(hero3, hero4, num2);
					}
				}
			}
		}

		private void DetermineRelation(Hero hero1, Hero hero2, float randomValue)
		{
			float num = 0.3f;
			float num2 = 0.3f;
			if (randomValue < num)
			{
				int num3 = (int)((num - randomValue) * (num - randomValue) / (num * num) * 100f);
				if (num3 > 0)
				{
					hero1.SetPersonalRelation(hero2, num3);
					return;
				}
			}
			else if (randomValue > 1f - num2)
			{
				int num4 = -(int)((randomValue - (1f - num2)) * (randomValue - (1f - num2)) / (num2 * num2) * 100f);
				if (num4 < 0)
				{
					hero1.SetPersonalRelation(hero2, num4);
				}
			}
		}

		private void DailyTickParty(MobileParty mobileParty)
		{
			if (mobileParty.LeaderHero != null)
			{
				Settlement currentSettlement = mobileParty.CurrentSettlement;
				if (currentSettlement != null && currentSettlement.IsTown && mobileParty.CurrentSettlement.SiegeEvent == null)
				{
					if (mobileParty.LeaderHero.GetPerkValue(DefaultPerks.Medicine.BestMedicine))
					{
						Hero randomElementWithPredicate = mobileParty.CurrentSettlement.Notables.GetRandomElementWithPredicate((Hero x) => x.Age >= 40f && x.IsAlive);
						if (randomElementWithPredicate != null)
						{
							ChangeRelationAction.ApplyRelationChangeBetweenHeroes(mobileParty.LeaderHero, randomElementWithPredicate, (int)DefaultPerks.Medicine.BestMedicine.SecondaryBonus, true);
						}
					}
					if (mobileParty.LeaderHero.GetPerkValue(DefaultPerks.Medicine.GoodLogdings))
					{
						Hero randomElement = TownHelpers.GetHeroesInSettlement(mobileParty.CurrentSettlement, (Hero x) => x.Age >= 40f && x != mobileParty.LeaderHero && x.IsLord).GetRandomElement<Hero>();
						if (randomElement != null)
						{
							ChangeRelationAction.ApplyRelationChangeBetweenHeroes(mobileParty.LeaderHero, randomElement, (int)DefaultPerks.Medicine.GoodLogdings.SecondaryBonus, true);
						}
					}
				}
				if (mobileParty.Army != null && mobileParty.LeaderHero.GetPerkValue(DefaultPerks.Charm.Parade) && MBRandom.RandomFloat < DefaultPerks.Charm.Parade.SecondaryBonus)
				{
					Func<TroopRosterElement, bool> <>9__3;
					MobileParty randomElementWithPredicate2 = mobileParty.Army.Parties.GetRandomElementWithPredicate(delegate(MobileParty x)
					{
						List<TroopRosterElement> troopRoster = x.MemberRoster.GetTroopRoster();
						Func<TroopRosterElement, bool> func;
						if ((func = <>9__3) == null)
						{
							func = (<>9__3 = (TroopRosterElement y) => y.Character.IsHero && y.Character.Occupation == Occupation.Lord && y.Character.HeroObject != mobileParty.LeaderHero);
						}
						return troopRoster.AnyQ(func);
					});
					if (randomElementWithPredicate2 != null)
					{
						CharacterObject character = randomElementWithPredicate2.MemberRoster.GetTroopRoster().GetRandomElementWithPredicate((TroopRosterElement x) => x.Character.IsHero && x.Character.Occupation == Occupation.Lord && x.Character.HeroObject != mobileParty.LeaderHero).Character;
						Hero hero = ((character != null) ? character.HeroObject : null);
						if (hero != null)
						{
							ChangeRelationAction.ApplyRelationChangeBetweenHeroes(mobileParty.LeaderHero, hero, 1, true);
						}
					}
				}
			}
		}

		private void DailyTick()
		{
			if (Settlement.CurrentSettlement != null && Hero.MainHero.GetPerkValue(DefaultPerks.Charm.ForgivableGrievances) && MBRandom.RandomFloat < DefaultPerks.Charm.ForgivableGrievances.SecondaryBonus)
			{
				MBList<Hero> mblist = new MBList<Hero>();
				foreach (Hero hero in SettlementHelper.GetAllHeroesOfSettlement(Settlement.CurrentSettlement, true))
				{
					if (!hero.IsHumanPlayerCharacter && hero.GetRelationWithPlayer() < 0f)
					{
						mblist.Add(hero);
					}
				}
				if (mblist.Count > 0)
				{
					ChangeRelationAction.ApplyPlayerRelation(mblist.GetRandomElement<Hero>(), 1, true, true);
				}
			}
			SettlementLoyaltyModel settlementLoyaltyModel = Campaign.Current.Models.SettlementLoyaltyModel;
			SettlementSecurityModel settlementSecurityModel = Campaign.Current.Models.SettlementSecurityModel;
			bool flag = false;
			bool flag2 = false;
			float num = 0.05f;
			foreach (Settlement settlement in Settlement.All)
			{
				if (settlement.IsTown)
				{
					if (settlement.Town.Security >= (float)settlementSecurityModel.ThresholdForNotableRelationBonus)
					{
						using (List<Hero>.Enumerator enumerator3 = settlement.Notables.GetEnumerator())
						{
							while (enumerator3.MoveNext())
							{
								Hero hero2 = enumerator3.Current;
								if ((hero2.IsArtisan || hero2.IsMerchant) && MBRandom.RandomFloat < num)
								{
									ChangeRelationAction.ApplyRelationChangeBetweenHeroes(settlement.OwnerClan.Leader, hero2, settlementSecurityModel.DailyNotableRelationBonus, false);
									flag2 = flag2 || settlement.OwnerClan.Leader.IsHumanPlayerCharacter;
								}
							}
							continue;
						}
					}
					if (settlement.Town.Security >= (float)settlementSecurityModel.ThresholdForNotableRelationPenalty)
					{
						continue;
					}
					foreach (Hero hero3 in settlement.Notables)
					{
						if ((hero3.IsArtisan || hero3.IsMerchant) && MBRandom.RandomFloat < num)
						{
							hero3.AddPower((float)settlementSecurityModel.DailyNotablePowerPenalty);
							ChangeRelationAction.ApplyRelationChangeBetweenHeroes(settlement.OwnerClan.Leader, hero3, settlementSecurityModel.DailyNotableRelationPenalty, false);
						}
					}
					using (List<Hero>.Enumerator enumerator3 = settlement.Notables.GetEnumerator())
					{
						while (enumerator3.MoveNext())
						{
							Hero hero4 = enumerator3.Current;
							if (hero4.IsGangLeader && MBRandom.RandomFloat < num)
							{
								hero4.AddPower((float)settlementSecurityModel.DailyNotablePowerBonus);
							}
						}
						continue;
					}
				}
				if (settlement.IsVillage && settlement.Village.Bound.Town.Loyalty >= settlementLoyaltyModel.ThresholdForNotableRelationBonus)
				{
					foreach (Hero hero5 in settlement.Notables)
					{
						if ((hero5.IsHeadman || hero5.IsRuralNotable) && MBRandom.RandomFloat < num)
						{
							ChangeRelationAction.ApplyRelationChangeBetweenHeroes(settlement.OwnerClan.Leader, hero5, settlementLoyaltyModel.DailyNotableRelationBonus, false);
							flag = flag || settlement.OwnerClan.Leader.IsHumanPlayerCharacter;
						}
					}
				}
			}
			if (flag2)
			{
				InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=ME5hmllb}Your relation with notables in some of your settlements increased due to high security", null).ToString()));
			}
			if (flag)
			{
				InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=0h5BrVdA}Your relation with notables in some of your settlements increased due to high loyalty", null).ToString()));
			}
		}

		public void OnSettlementOwnerChanged(Settlement settlement, bool openToClaim, Hero newOwner, Hero oldOwner, Hero capturerHero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail detail)
		{
			if ((detail == ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail.BySiege || detail == ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail.ByBarter || detail == ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail.ByRevolt) && oldOwner != null && oldOwner.MapFaction != null && oldOwner.MapFaction.Leader != oldOwner && oldOwner.IsAlive && oldOwner.MapFaction.Leader != Hero.MainHero)
			{
				float value = settlement.GetValue(null, true);
				int num = (int)((1f + MathF.Max(1f, MathF.Sqrt(value / 100000f))) * ((newOwner.MapFaction != oldOwner.MapFaction) ? 1f : 0.5f));
				ChangeRelationAction.ApplyRelationChangeBetweenHeroes(oldOwner, oldOwner.MapFaction.Leader, -num, false);
				if (capturerHero != null && capturerHero.Clan != capturerHero.MapFaction.Leader.Clan)
				{
					ChangeRelationAction.ApplyRelationChangeBetweenHeroes(capturerHero, capturerHero.MapFaction.Leader, num / 2, false);
				}
				if (oldOwner.Clan != null && settlement != null)
				{
					ChangeClanInfluenceAction.Apply(oldOwner.Clan, (float)(settlement.IsTown ? (-50) : (-25)));
				}
			}
		}

		private void OnRaidCompleted(BattleSideEnum winnerSide, RaidEventComponent raidEvent)
		{
			MapEvent mapEvent = raidEvent.MapEvent;
			PartyBase leaderParty = mapEvent.AttackerSide.LeaderParty;
			Hero hero = ((leaderParty != null) ? leaderParty.LeaderHero : null);
			PartyBase leaderParty2 = mapEvent.DefenderSide.LeaderParty;
			if (leaderParty == null || leaderParty.MapFaction == mapEvent.MapEventSettlement.MapFaction)
			{
				return;
			}
			if (winnerSide == BattleSideEnum.Attacker && hero != null && leaderParty2 != null && leaderParty2.IsSettlement && leaderParty2.Settlement.IsVillage && leaderParty2.Settlement.OwnerClan != Clan.PlayerClan)
			{
				int num = -MathF.Ceiling(6f * raidEvent.RaidDamage);
				int num2 = -MathF.Ceiling(6f * raidEvent.RaidDamage * 0.5f);
				if (num < 0)
				{
					ChangeRelationAction.ApplyRelationChangeBetweenHeroes(hero, leaderParty2.Settlement.OwnerClan.Leader, num, true);
				}
				if (num2 < 0)
				{
					foreach (Hero hero2 in leaderParty2.Settlement.Notables)
					{
						ChangeRelationAction.ApplyRelationChangeBetweenHeroes(hero, hero2, num2, true);
					}
				}
			}
		}

		private static void OnHeroesMarried(Hero firstHero, Hero secondHero, bool showNotification)
		{
			ChangeRelationAction.ApplyRelationChangeBetweenHeroes(firstHero, secondHero, 30, false);
		}

		private const int RelationPenaltyFactor = 6;

		private const int RelationIncreaseBetweenHeroesAfterMarriage = 30;
	}
}
