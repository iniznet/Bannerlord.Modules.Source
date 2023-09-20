using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace Helpers
{
	// Token: 0x02000009 RID: 9
	public static class SettlementHelper
	{
		// Token: 0x06000027 RID: 39 RVA: 0x000039C4 File Offset: 0x00001BC4
		public static string GetRandomStuff(bool isFemale)
		{
			string text;
			if (isFemale)
			{
				text = SettlementHelper.StuffToCarryForWoman[SettlementHelper._stuffToCarryIndex % SettlementHelper.StuffToCarryForWoman.Length];
			}
			else
			{
				text = SettlementHelper.StuffToCarryForMan[SettlementHelper._stuffToCarryIndex % SettlementHelper.StuffToCarryForMan.Length];
			}
			SettlementHelper._stuffToCarryIndex++;
			return text;
		}

		// Token: 0x06000028 RID: 40 RVA: 0x00003A0C File Offset: 0x00001C0C
		public static Settlement FindNearestSettlement(Func<Settlement, bool> condition, IMapPoint toMapPoint = null)
		{
			Settlement settlement = null;
			float num = Campaign.MapDiagonal * 2f;
			IMapPoint mapPoint = toMapPoint ?? MobileParty.MainParty;
			foreach (Settlement settlement2 in Settlement.All)
			{
				float num2;
				if ((condition == null || condition(settlement2)) && Campaign.Current.Models.MapDistanceModel.GetDistance(mapPoint, settlement2, num, out num2))
				{
					settlement = settlement2;
					num = num2;
				}
			}
			return settlement;
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00003AA4 File Offset: 0x00001CA4
		public static Settlement FindNearestHideout(Func<Settlement, bool> condition = null, IMapPoint toMapPoint = null)
		{
			Settlement settlement = null;
			float num = 1E+09f;
			IMapPoint mapPoint = toMapPoint ?? MobileParty.MainParty;
			foreach (Hideout hideout in Hideout.All)
			{
				Settlement settlement2 = hideout.Settlement;
				float num2;
				if ((condition == null || condition(settlement2)) && Campaign.Current.Models.MapDistanceModel.GetDistance(mapPoint, settlement2, num, out num2))
				{
					settlement = settlement2;
					num = num2;
				}
			}
			return settlement;
		}

		// Token: 0x0600002A RID: 42 RVA: 0x00003B38 File Offset: 0x00001D38
		public static Settlement FindNearestTown(Func<Settlement, bool> condition = null, IMapPoint toMapPoint = null)
		{
			Settlement settlement = null;
			float num = 1E+09f;
			IMapPoint mapPoint = toMapPoint ?? MobileParty.MainParty;
			foreach (Town town in Town.AllTowns)
			{
				Settlement settlement2 = town.Settlement;
				float num2;
				if ((condition == null || condition(settlement2)) && Campaign.Current.Models.MapDistanceModel.GetDistance(mapPoint, settlement2, num, out num2))
				{
					settlement = settlement2;
					num = num2;
				}
			}
			return settlement;
		}

		// Token: 0x0600002B RID: 43 RVA: 0x00003BCC File Offset: 0x00001DCC
		public static Settlement FindNearestFortification(Func<Settlement, bool> condition = null, IMapPoint toMapPoint = null)
		{
			Settlement settlement = null;
			float num = 1E+09f;
			IMapPoint mapPoint = toMapPoint ?? MobileParty.MainParty;
			foreach (Town town in Town.AllTowns)
			{
				Settlement settlement2 = town.Settlement;
				float num2;
				if ((condition == null || condition(settlement2)) && Campaign.Current.Models.MapDistanceModel.GetDistance(mapPoint, settlement2, num, out num2))
				{
					settlement = settlement2;
					num = num2;
				}
			}
			foreach (Town town2 in Town.AllCastles)
			{
				Settlement settlement3 = town2.Settlement;
				float num3;
				if ((condition == null || condition(settlement3)) && Campaign.Current.Models.MapDistanceModel.GetDistance(mapPoint, settlement3, num, out num3))
				{
					settlement = settlement3;
					num = num3;
				}
			}
			return settlement;
		}

		// Token: 0x0600002C RID: 44 RVA: 0x00003CD0 File Offset: 0x00001ED0
		public static Settlement FindNearestCastle(Func<Settlement, bool> condition = null, IMapPoint toMapPoint = null)
		{
			Settlement settlement = null;
			float num = 1E+09f;
			IMapPoint mapPoint = toMapPoint ?? MobileParty.MainParty;
			foreach (Town town in Town.AllCastles)
			{
				Settlement settlement2 = town.Settlement;
				float num2;
				if ((condition == null || condition(settlement2)) && Campaign.Current.Models.MapDistanceModel.GetDistance(mapPoint, settlement2, num, out num2))
				{
					settlement = settlement2;
					num = num2;
				}
			}
			return settlement;
		}

		// Token: 0x0600002D RID: 45 RVA: 0x00003D64 File Offset: 0x00001F64
		public static Settlement FindNearestVillage(Func<Settlement, bool> condition = null, IMapPoint toMapPoint = null)
		{
			Settlement settlement = null;
			float num = 1E+09f;
			IMapPoint mapPoint = toMapPoint ?? MobileParty.MainParty;
			foreach (Village village in Village.All)
			{
				Settlement settlement2 = village.Settlement;
				float num2;
				if ((condition == null || condition(settlement2)) && Campaign.Current.Models.MapDistanceModel.GetDistance(mapPoint, settlement2, num, out num2))
				{
					settlement = settlement2;
					num = num2;
				}
			}
			return settlement;
		}

		// Token: 0x0600002E RID: 46 RVA: 0x00003DF8 File Offset: 0x00001FF8
		private static SettlementComponent FindNearestSettlementToMapPointInternal(IMapPoint mapPoint, IEnumerable<SettlementComponent> settlementsToIterate, Func<Settlement, bool> condition = null)
		{
			SettlementComponent settlementComponent = null;
			float num = Campaign.MapDiagonal * 2f;
			foreach (SettlementComponent settlementComponent2 in settlementsToIterate)
			{
				float num2;
				if ((condition == null || condition(settlementComponent2.Settlement)) && Campaign.Current.Models.MapDistanceModel.GetDistance(mapPoint, settlementComponent2.Settlement, num, out num2))
				{
					settlementComponent = settlementComponent2;
					num = num2;
				}
			}
			return settlementComponent;
		}

		// Token: 0x0600002F RID: 47 RVA: 0x00003E80 File Offset: 0x00002080
		public static int FindNextSettlementAroundMapPoint(IMapPoint mapPoint, float maxDistance, int lastIndex)
		{
			for (int i = lastIndex + 1; i < Settlement.All.Count; i++)
			{
				Settlement settlement = Settlement.All[i];
				float num;
				if (Campaign.Current.Models.MapDistanceModel.GetDistance(mapPoint, settlement, maxDistance, out num))
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00003ED0 File Offset: 0x000020D0
		private static Settlement FindRandomInternal(Func<Settlement, bool> condition, IEnumerable<Settlement> settlementsToIterate)
		{
			List<Settlement> list = new List<Settlement>();
			foreach (Settlement settlement in settlementsToIterate)
			{
				if (condition(settlement))
				{
					list.Add(settlement);
				}
			}
			if (list.Count > 0)
			{
				return list[MBRandom.RandomInt(list.Count)];
			}
			return null;
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00003F44 File Offset: 0x00002144
		public static Settlement FindRandomSettlement(Func<Settlement, bool> condition = null)
		{
			return SettlementHelper.FindRandomInternal(condition, Settlement.All);
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00003F51 File Offset: 0x00002151
		public static Settlement FindRandomHideout(Func<Settlement, bool> condition = null)
		{
			return SettlementHelper.FindRandomInternal(condition, Hideout.All.Select((Hideout x) => x.Settlement));
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00003F84 File Offset: 0x00002184
		public static void TakeEnemyVillagersOutsideSettlements(Settlement settlementWhichChangedFaction)
		{
			if (settlementWhichChangedFaction.IsFortification)
			{
				bool flag;
				do
				{
					flag = false;
					MobileParty mobileParty = null;
					foreach (MobileParty mobileParty2 in settlementWhichChangedFaction.Parties)
					{
						if (mobileParty2.IsVillager && mobileParty2.HomeSettlement.IsVillage && mobileParty2.HomeSettlement.Village.Bound == settlementWhichChangedFaction && mobileParty2.HomeSettlement.MapFaction != settlementWhichChangedFaction.MapFaction)
						{
							mobileParty = mobileParty2;
							flag = true;
							break;
						}
					}
					if (flag && mobileParty.MapEvent == null)
					{
						LeaveSettlementAction.ApplyForParty(mobileParty);
						mobileParty.Ai.SetMoveModeHold();
					}
				}
				while (flag);
				bool flag2;
				do
				{
					flag2 = false;
					MobileParty mobileParty3 = null;
					foreach (MobileParty mobileParty4 in settlementWhichChangedFaction.Parties)
					{
						if (mobileParty4.IsCaravan && FactionManager.IsAtWarAgainstFaction(mobileParty4.MapFaction, settlementWhichChangedFaction.MapFaction))
						{
							mobileParty3 = mobileParty4;
							flag2 = true;
							break;
						}
					}
					if (flag2 && mobileParty3.MapEvent == null)
					{
						LeaveSettlementAction.ApplyForParty(mobileParty3);
						mobileParty3.Ai.SetMoveModeHold();
					}
				}
				while (flag2);
				foreach (MobileParty mobileParty5 in MobileParty.All)
				{
					if ((mobileParty5.IsVillager || mobileParty5.IsCaravan) && mobileParty5.TargetSettlement == settlementWhichChangedFaction && mobileParty5.CurrentSettlement != settlementWhichChangedFaction)
					{
						mobileParty5.Ai.SetMoveModeHold();
					}
				}
			}
			if (settlementWhichChangedFaction.IsVillage)
			{
				foreach (MobileParty mobileParty6 in MobileParty.AllVillagerParties)
				{
					if (mobileParty6.HomeSettlement == settlementWhichChangedFaction && mobileParty6.CurrentSettlement != settlementWhichChangedFaction)
					{
						if (mobileParty6.CurrentSettlement != null && mobileParty6.MapEvent == null)
						{
							LeaveSettlementAction.ApplyForParty(mobileParty6);
							mobileParty6.Ai.SetMoveModeHold();
						}
						else
						{
							mobileParty6.Ai.SetMoveModeHold();
						}
					}
				}
			}
		}

		// Token: 0x06000034 RID: 52 RVA: 0x000041C8 File Offset: 0x000023C8
		public static Settlement GetRandomTown(Clan fromFaction = null)
		{
			int num = 0;
			foreach (Settlement settlement in Campaign.Current.Settlements)
			{
				if ((fromFaction == null || settlement.MapFaction == fromFaction) && (settlement.IsTown || settlement.IsVillage))
				{
					num++;
				}
			}
			int num2 = MBRandom.RandomInt(0, num - 1);
			foreach (Settlement settlement2 in Campaign.Current.Settlements)
			{
				if ((fromFaction == null || settlement2.MapFaction == fromFaction) && (settlement2.IsTown || settlement2.IsVillage))
				{
					num2--;
					if (num2 < 0)
					{
						return settlement2;
					}
				}
			}
			return null;
		}

		// Token: 0x06000035 RID: 53 RVA: 0x000042B8 File Offset: 0x000024B8
		public static Settlement GetBestSettlementToSpawnAround(Hero hero)
		{
			Settlement settlement = null;
			float num = -1f;
			uint num2 = 0U;
			using (List<Hero>.Enumerator enumerator = hero.Clan.Lords.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current == hero)
					{
						break;
					}
					num2 += 1U;
				}
			}
			IFaction mapFaction = hero.MapFaction;
			foreach (Settlement settlement2 in Settlement.All)
			{
				if (settlement2.Party.MapEvent == null)
				{
					IFaction mapFaction2 = settlement2.MapFaction;
					float num3 = 0.0001f;
					if (mapFaction2 == mapFaction)
					{
						num3 = 1f;
					}
					else if (FactionManager.IsAlliedWithFaction(mapFaction2, mapFaction))
					{
						num3 = 0.01f;
					}
					else if (FactionManager.IsNeutralWithFaction(mapFaction2, mapFaction))
					{
						num3 = 0.0005f;
					}
					float num4 = 0f;
					if (settlement2.IsTown)
					{
						num4 = 1f;
					}
					else if (settlement2.IsCastle)
					{
						num4 = 0.9f;
					}
					else if (settlement2.IsVillage)
					{
						num4 = 0.8f;
					}
					else if (settlement2.IsHideout)
					{
						num4 = ((mapFaction2 == mapFaction) ? 0.2f : 0f);
					}
					float num5 = ((settlement2.Town != null && settlement2.Town.GarrisonParty != null && settlement2.OwnerClan == hero.Clan) ? (settlement2.Town.GarrisonParty.Party.TotalStrength / (settlement2.IsTown ? 60f : 30f)) : 1f);
					float num6 = ((settlement2.IsUnderRaid || settlement2.IsUnderSiege) ? 0.1f : 1f);
					float num7 = ((settlement2.OwnerClan == hero.Clan) ? 1f : 0.25f);
					float num8 = settlement2.RandomFloatWithSeed(num2, 0.5f, 1f);
					float num9 = 1f - hero.MapFaction.InitialPosition.Distance(settlement2.Position2D) / Campaign.MapDiagonal;
					num9 *= num9;
					float num10 = num3 * num4 * num6 * num7 * num5 * num8 * num9;
					if (num10 > num)
					{
						num = num10;
						settlement = settlement2;
					}
				}
			}
			return settlement;
		}

		// Token: 0x06000036 RID: 54 RVA: 0x00004528 File Offset: 0x00002728
		public static IEnumerable<Hero> GetAllHeroesOfSettlement(Settlement settlement, bool includePrisoners)
		{
			foreach (MobileParty mobileParty in settlement.Parties)
			{
				if (mobileParty.LeaderHero != null)
				{
					yield return mobileParty.LeaderHero;
				}
			}
			List<MobileParty>.Enumerator enumerator = default(List<MobileParty>.Enumerator);
			foreach (Hero hero in settlement.HeroesWithoutParty)
			{
				yield return hero;
			}
			List<Hero>.Enumerator enumerator2 = default(List<Hero>.Enumerator);
			if (includePrisoners)
			{
				foreach (TroopRosterElement troopRosterElement in settlement.Party.PrisonRoster.GetTroopRoster())
				{
					if (troopRosterElement.Character.IsHero)
					{
						yield return troopRosterElement.Character.HeroObject;
					}
				}
				List<TroopRosterElement>.Enumerator enumerator3 = default(List<TroopRosterElement>.Enumerator);
			}
			yield break;
			yield break;
		}

		// Token: 0x06000037 RID: 55 RVA: 0x00004540 File Offset: 0x00002740
		public static int NumberOfVolunteersCanBeRecruitedForGarrison(Settlement settlement)
		{
			int num = 0;
			Hero leader = settlement.OwnerClan.Leader;
			foreach (Hero hero in settlement.Notables)
			{
				int num2 = Campaign.Current.Models.VolunteerModel.MaximumIndexHeroCanRecruitFromHero(leader, hero, -101);
				for (int i = 0; i < num2; i++)
				{
					if (hero.VolunteerTypes[i] != null)
					{
						num++;
					}
				}
			}
			foreach (Village village in settlement.BoundVillages)
			{
				if (village.VillageState == Village.VillageStates.Normal)
				{
					num += SettlementHelper.NumberOfVolunteersCanBeRecruitedForGarrison(village.Settlement);
				}
			}
			return num;
		}

		// Token: 0x06000038 RID: 56 RVA: 0x0000462C File Offset: 0x0000282C
		public static bool IsThereAnyVolunteerCanBeRecruitedForGarrison(Settlement settlement)
		{
			Hero leader = settlement.OwnerClan.Leader;
			foreach (Hero hero in settlement.Notables)
			{
				int num = Campaign.Current.Models.VolunteerModel.MaximumIndexHeroCanRecruitFromHero(leader, hero, -101);
				for (int i = 0; i < num; i++)
				{
					if (hero.VolunteerTypes[i] != null)
					{
						return true;
					}
				}
			}
			foreach (Village village in settlement.BoundVillages)
			{
				if (village.VillageState == Village.VillageStates.Normal && SettlementHelper.IsThereAnyVolunteerCanBeRecruitedForGarrison(village.Settlement))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000039 RID: 57 RVA: 0x0000471C File Offset: 0x0000291C
		public static bool IsGarrisonStarving(Settlement settlement)
		{
			bool flag = false;
			if (settlement.IsStarving)
			{
				flag = settlement.Town.FoodChange < -(settlement.Prosperity / (float)Campaign.Current.Models.SettlementFoodModel.NumberOfProsperityToEatOneFood);
			}
			return flag;
		}

		// Token: 0x0600003A RID: 58 RVA: 0x00004760 File Offset: 0x00002960
		public static void SpawnNotablesIfNeeded(Settlement settlement)
		{
			if (settlement.IsTown || settlement.IsVillage)
			{
				List<Occupation> list = new List<Occupation>();
				if (settlement.IsTown)
				{
					list = new List<Occupation>
					{
						Occupation.GangLeader,
						Occupation.Artisan,
						Occupation.Merchant
					};
				}
				else if (settlement.IsVillage)
				{
					list = new List<Occupation>
					{
						Occupation.RuralNotable,
						Occupation.Headman
					};
				}
				float randomFloat = MBRandom.RandomFloat;
				int num = 0;
				foreach (Occupation occupation in list)
				{
					num += Campaign.Current.Models.NotableSpawnModel.GetTargetNotableCountForSettlement(settlement, occupation);
				}
				float num2 = ((settlement.Notables.Count > 0) ? ((float)(num - settlement.Notables.Count) / (float)num) : 1f);
				num2 *= MathF.Pow(num2, 0.36f);
				if (randomFloat <= num2)
				{
					MBList<Occupation> mblist = new MBList<Occupation>();
					foreach (Occupation occupation2 in list)
					{
						int num3 = 0;
						using (List<Hero>.Enumerator enumerator2 = settlement.Notables.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								if (enumerator2.Current.CharacterObject.Occupation == occupation2)
								{
									num3++;
								}
							}
						}
						int targetNotableCountForSettlement = Campaign.Current.Models.NotableSpawnModel.GetTargetNotableCountForSettlement(settlement, occupation2);
						if (num3 < targetNotableCountForSettlement)
						{
							mblist.Add(occupation2);
						}
					}
					if (mblist.Count > 0)
					{
						EnterSettlementAction.ApplyForCharacterOnly(HeroCreator.CreateHeroAtOccupation(mblist.GetRandomElement<Occupation>(), settlement), settlement);
					}
				}
			}
		}

		// Token: 0x04000001 RID: 1
		private static readonly string[] StuffToCarryForMan = new string[] { "_to_carry_foods_basket_apple", "_to_carry_merchandise_hides_b", "_to_carry_bed_convolute_g", "_to_carry_bed_convolute_a", "_to_carry_bd_fabric_c", "_to_carry_bd_basket_a", "practice_spear_t1", "simple_sparth_axe_t2" };

		// Token: 0x04000002 RID: 2
		private static readonly string[] StuffToCarryForWoman = new string[] { "_to_carry_kitchen_pot_c", "_to_carry_arm_kitchen_pot_c", "_to_carry_foods_basket_apple", "_to_carry_bd_basket_a" };

		// Token: 0x04000003 RID: 3
		private static int _stuffToCarryIndex = MBRandom.NondeterministicRandomInt % 1024;
	}
}
