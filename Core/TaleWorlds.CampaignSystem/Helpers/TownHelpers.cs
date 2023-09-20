using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace Helpers
{
	public static class TownHelpers
	{
		public static ValueTuple<int, int> GetTownFoodAndMarketStocks(Town town)
		{
			float num = ((town != null) ? town.FoodStocks : 0f);
			float num2 = 0f;
			if (town != null && town.IsTown)
			{
				for (int i = town.Owner.ItemRoster.Count - 1; i >= 0; i--)
				{
					ItemRosterElement elementCopyAtIndex = town.Owner.ItemRoster.GetElementCopyAtIndex(i);
					if (elementCopyAtIndex.EquipmentElement.Item != null && elementCopyAtIndex.EquipmentElement.Item.ItemCategory.Properties == ItemCategory.Property.BonusToFoodStores)
					{
						num2 += (float)elementCopyAtIndex.Amount;
					}
				}
			}
			return new ValueTuple<int, int>((int)num, (int)num2);
		}

		public static bool IsThereAnyoneToMeetInTown(Settlement settlement)
		{
			foreach (MobileParty mobileParty in settlement.Parties.Where(new Func<MobileParty, bool>(TownHelpers.RequestAMeetingPartyCondition)))
			{
				using (List<TroopRosterElement>.Enumerator enumerator2 = mobileParty.MemberRoster.GetTroopRoster().GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						if (enumerator2.Current.Character.IsHero)
						{
							return true;
						}
					}
				}
			}
			using (IEnumerator<Hero> enumerator3 = settlement.HeroesWithoutParty.Where(new Func<Hero, bool>(TownHelpers.RequestAMeetingHeroWithoutPartyCondition)).GetEnumerator())
			{
				if (enumerator3.MoveNext())
				{
					Hero hero = enumerator3.Current;
					return true;
				}
			}
			return false;
		}

		public static List<Hero> GetHeroesToMeetInTown(Settlement settlement)
		{
			List<Hero> list = new List<Hero>();
			foreach (MobileParty mobileParty in settlement.Parties.Where(new Func<MobileParty, bool>(TownHelpers.RequestAMeetingPartyCondition)))
			{
				foreach (TroopRosterElement troopRosterElement in mobileParty.MemberRoster.GetTroopRoster())
				{
					if (troopRosterElement.Character.IsHero)
					{
						list.Add(troopRosterElement.Character.HeroObject);
					}
				}
			}
			foreach (Hero hero in settlement.HeroesWithoutParty.Where(new Func<Hero, bool>(TownHelpers.RequestAMeetingHeroWithoutPartyCondition)))
			{
				list.Add(hero);
			}
			return list;
		}

		public static MBList<Hero> GetHeroesInSettlement(Settlement settlement, Predicate<Hero> predicate = null)
		{
			MBList<Hero> mblist = new MBList<Hero>();
			foreach (MobileParty mobileParty in settlement.Parties)
			{
				foreach (TroopRosterElement troopRosterElement in mobileParty.MemberRoster.GetTroopRoster())
				{
					if (troopRosterElement.Character.IsHero && (predicate == null || predicate(troopRosterElement.Character.HeroObject)))
					{
						mblist.Add(troopRosterElement.Character.HeroObject);
					}
				}
			}
			foreach (Hero hero in settlement.HeroesWithoutParty)
			{
				if (predicate == null || predicate(hero))
				{
					mblist.Add(hero);
				}
			}
			return mblist;
		}

		public static bool RequestAMeetingPartyCondition(MobileParty party)
		{
			return party.IsLordParty && !party.IsMainParty && (party.Army == null || party.Army != MobileParty.MainParty.Army);
		}

		public static bool RequestAMeetingHeroWithoutPartyCondition(Hero hero)
		{
			return hero.CharacterObject.Occupation == Occupation.Lord && !hero.IsPrisoner && hero.Age >= (float)Campaign.Current.Models.AgeModel.HeroComesOfAge;
		}
	}
}
