using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem
{
	public static class HeroCreator
	{
		public static Hero CreateHeroAtOccupation(Occupation neededOccupation, Settlement forcedHomeSettlement = null)
		{
			Settlement settlement = forcedHomeSettlement ?? SettlementHelper.GetRandomTown(null);
			IEnumerable<CharacterObject> enumerable = settlement.Culture.NotableAndWandererTemplates.Where((CharacterObject x) => x.Occupation == neededOccupation);
			int num = 0;
			foreach (CharacterObject characterObject in enumerable)
			{
				int num2 = characterObject.GetTraitLevel(DefaultTraits.Frequency) * 10;
				num += ((num2 > 0) ? num2 : 100);
			}
			if (!enumerable.Any<CharacterObject>())
			{
				return null;
			}
			CharacterObject characterObject2 = null;
			int num3 = settlement.RandomIntWithSeed((uint)settlement.Notables.Count, 1, num);
			foreach (CharacterObject characterObject3 in enumerable)
			{
				int num4 = characterObject3.GetTraitLevel(DefaultTraits.Frequency) * 10;
				num3 -= ((num4 > 0) ? num4 : 100);
				if (num3 < 0)
				{
					characterObject2 = characterObject3;
					break;
				}
			}
			Hero hero = HeroCreator.CreateSpecialHero(characterObject2, settlement, null, null, -1);
			if (hero.HomeSettlement.IsVillage && hero.HomeSettlement.Village.Bound != null && hero.HomeSettlement.Village.Bound.IsCastle)
			{
				float num5 = MBRandom.RandomFloat * 20f;
				hero.AddPower(num5);
			}
			if (neededOccupation != Occupation.Wanderer)
			{
				hero.ChangeState(Hero.CharacterStates.Active);
			}
			if (neededOccupation != Occupation.Wanderer)
			{
				EnterSettlementAction.ApplyForCharacterOnly(hero, settlement);
			}
			if (neededOccupation != Occupation.Wanderer)
			{
				int num6 = 10000;
				GiveGoldAction.ApplyBetweenCharacters(null, hero, num6, true);
			}
			CharacterObject template = hero.Template;
			if (((template != null) ? template.HeroObject : null) != null && hero.Template.HeroObject.Clan != null && hero.Template.HeroObject.Clan.IsMinorFaction)
			{
				hero.SupporterOf = hero.Template.HeroObject.Clan;
			}
			else
			{
				hero.SupporterOf = HeroHelper.GetRandomClanForNotable(hero);
			}
			if (neededOccupation != Occupation.Wanderer)
			{
				HeroCreator.AddRandomVarianceToTraits(hero);
			}
			return hero;
		}

		private static Hero CreateNewHero(CharacterObject template, int age = -1)
		{
			Debug.Print("creating hero from template with id: " + template.StringId, 0, Debug.DebugColor.White, 17592186044416UL);
			CharacterObject newCharacter = CharacterObject.CreateFrom(template);
			Hero hero = Hero.CreateHero(newCharacter.StringId);
			hero.SetCharacterObject(newCharacter);
			newCharacter.HeroObject = hero;
			CampaignTime campaignTime;
			if (age == -1)
			{
				campaignTime = HeroHelper.GetRandomBirthDayForAge((float)(Campaign.Current.Models.AgeModel.HeroComesOfAge + MBRandom.RandomInt(30)));
			}
			else if (age == 0)
			{
				campaignTime = CampaignTime.Now;
			}
			else if (hero.IsWanderer)
			{
				age = (int)template.Age;
				if (age < 20)
				{
					foreach (TraitObject traitObject in TraitObject.All)
					{
						int num = 12 + 4 * template.GetTraitLevel(traitObject);
						if (age < num)
						{
							age = num;
						}
					}
				}
				campaignTime = HeroHelper.GetRandomBirthDayForAge((float)age);
			}
			else
			{
				campaignTime = HeroHelper.GetRandomBirthDayForAge((float)age);
			}
			newCharacter.HeroObject.SetBirthDay(campaignTime);
			Settlement settlement = SettlementHelper.FindRandomSettlement((Settlement x) => x.IsTown && (newCharacter.Culture.StringId == "neutral_culture" || x.Culture == newCharacter.Culture));
			if (settlement == null)
			{
				settlement = SettlementHelper.FindRandomSettlement((Settlement x) => x.IsTown);
			}
			newCharacter.HeroObject.BornSettlement = settlement;
			newCharacter.HeroObject.StaticBodyProperties = BodyProperties.GetRandomBodyProperties(template.Race, template.IsFemale, template.GetBodyPropertiesMin(false), template.GetBodyPropertiesMax(), 0, MBRandom.RandomInt(), newCharacter.HairTags, newCharacter.BeardTags, newCharacter.TattooTags).StaticProperties;
			newCharacter.HeroObject.Weight = 0f;
			newCharacter.HeroObject.Build = 0f;
			if (newCharacter.Age >= (float)Campaign.Current.Models.AgeModel.HeroComesOfAge)
			{
				newCharacter.HeroObject.HeroDeveloper.DeriveSkillsFromTraits(false, null);
			}
			hero.PreferredUpgradeFormation = HeroCreator.GetRandomPreferredUpgradeFormation();
			return newCharacter.HeroObject;
		}

		public static Hero CreateSpecialHero(CharacterObject template, Settlement bornSettlement = null, Clan faction = null, Clan supporterOfClan = null, int age = -1)
		{
			Hero hero = HeroCreator.CreateNewHero(template, age);
			CultureObject cultureObject = template.Culture;
			if (cultureObject == null && bornSettlement != null)
			{
				cultureObject = bornSettlement.Culture;
			}
			else if (cultureObject == null && faction != null)
			{
				cultureObject = faction.Culture;
			}
			if (faction != null)
			{
				hero.Clan = faction;
			}
			if (supporterOfClan != null)
			{
				hero.SupporterOf = supporterOfClan;
			}
			if (bornSettlement != null)
			{
				hero.BornSettlement = bornSettlement;
			}
			hero.CharacterObject.Culture = cultureObject;
			TextObject textObject;
			TextObject textObject2;
			NameGenerator.Current.GenerateHeroNameAndHeroFullName(hero, out textObject, out textObject2, false);
			hero.SetName(textObject2, textObject);
			HeroCreator.ModifyAppearanceByTraits(hero);
			CampaignEventDispatcher.Instance.OnHeroCreated(hero, false);
			return hero;
		}

		public static Hero CreateRelativeNotableHero(Hero relative)
		{
			Hero hero = HeroCreator.CreateHeroAtOccupation(relative.CharacterObject.Occupation, relative.HomeSettlement);
			hero.Culture = relative.Culture;
			BodyProperties bodyPropertiesMin = relative.CharacterObject.GetBodyPropertiesMin(false);
			BodyProperties bodyPropertiesMin2 = hero.CharacterObject.GetBodyPropertiesMin(false);
			int defaultFaceSeed = relative.CharacterObject.GetDefaultFaceSeed(1);
			string hairTags = relative.HairTags;
			string tattooTags = relative.TattooTags;
			hero.StaticBodyProperties = BodyProperties.GetRandomBodyProperties(hero.CharacterObject.Race, hero.IsFemale, bodyPropertiesMin, bodyPropertiesMin2, 1, defaultFaceSeed, hairTags, relative.BeardTags, tattooTags).StaticProperties;
			return hero;
		}

		public static bool CreateBasicHero(CharacterObject character, out Hero hero, string stringId = "")
		{
			if (string.IsNullOrEmpty(stringId))
			{
				hero = HeroCreator.CreateNewHero(character, -1);
				return true;
			}
			hero = Campaign.Current.CampaignObjectManager.Find<Hero>(stringId);
			if (hero != null)
			{
				return false;
			}
			hero = Hero.CreateHero(stringId);
			hero.SetCharacterObject(character);
			hero.StaticBodyProperties = character.GetBodyPropertiesMin(false).StaticProperties;
			hero.Weight = 0f;
			hero.Build = 0f;
			character.HeroObject = hero;
			hero.PreferredUpgradeFormation = HeroCreator.GetRandomPreferredUpgradeFormation();
			return true;
		}

		private static void ModifyAppearanceByTraits(Hero hero)
		{
			int num = MBRandom.RandomInt(0, 100);
			int num2 = MBRandom.RandomInt(0, 100);
			if (hero.Age >= 40f)
			{
				num -= 30;
				num2 += 30;
			}
			int num3 = -1;
			int num4 = -1;
			int num5 = -1;
			bool flag = hero.HairTags.IsEmpty<char>() && hero.BeardTags.IsEmpty<char>();
			if (hero.GetTraitLevel(DefaultTraits.RomanHair) > 0 && !hero.IsFemale && flag)
			{
				if (num < 0)
				{
					num3 = 0;
				}
				else if (num < 20)
				{
					num3 = 13;
				}
				else if (num < 70)
				{
					num3 = 8;
				}
				else
				{
					num3 = 6;
				}
				if (num2 < 60)
				{
					num4 = 0;
				}
				else if (num2 < 110)
				{
					num4 = 13;
				}
				else
				{
					num4 = 14;
				}
			}
			else if (hero.GetTraitLevel(DefaultTraits.CelticHair) > 0 && !hero.IsFemale && flag)
			{
				if (num < 0)
				{
					num3 = 0;
				}
				else if (num < 20)
				{
					num3 = 13;
				}
				else if (num < 40)
				{
					num3 = 6;
				}
				else if (num < 60)
				{
					num3 = 14;
				}
				else if (num < 85)
				{
					num3 = 2;
				}
				else
				{
					num3 = 7;
				}
				if (num2 < 40)
				{
					num4 = 1;
				}
				else if (num2 < 60)
				{
					num4 = 2;
				}
				else if (num2 < 110)
				{
					num4 = 3;
				}
				else
				{
					num4 = 5;
				}
			}
			else if (hero.GetTraitLevel(DefaultTraits.ArabianHair) > 0 && !hero.IsFemale && flag)
			{
				if (num < 0)
				{
					num3 = 0;
				}
				else if (num < 20)
				{
					num3 = 13;
				}
				else if (num < 40)
				{
					num3 = 6;
				}
				else if (num < 60)
				{
					num3 = 2;
				}
				else if (num < 85)
				{
					num3 = 11;
				}
				else
				{
					num3 = 7;
				}
				if (num2 < 40)
				{
					num4 = 0;
				}
				else if (num2 < 50)
				{
					num4 = 6;
				}
				else if (num2 < 60)
				{
					num4 = 12;
				}
				else if (num2 < 70)
				{
					num4 = 8;
				}
				else if (num2 < 80)
				{
					num4 = 15;
				}
				else if (num2 < 100)
				{
					num4 = 9;
				}
				else
				{
					num4 = 17;
				}
			}
			else if (hero.GetTraitLevel(DefaultTraits.RusHair) > 0 && !hero.IsFemale && flag)
			{
				if (num < 0)
				{
					num3 = 0;
				}
				else if (num < 40)
				{
					num3 = 6;
				}
				else if (num < 60)
				{
					num3 = 12;
				}
				else if (num < 85)
				{
					num3 = 11;
				}
				else
				{
					num3 = 2;
				}
				if (num2 < 30)
				{
					num4 = 0;
				}
				else if (num2 < 60)
				{
					num4 = 13;
				}
				else if (num2 < 70)
				{
					num4 = 17;
				}
				else if (num2 < 90)
				{
					num4 = 18;
				}
				else
				{
					num4 = 19;
				}
			}
			hero.ModifyHair(num3, num4, num5);
		}

		private static void AddRandomVarianceToTraits(Hero hero)
		{
			foreach (TraitObject traitObject in TraitObject.All)
			{
				if (traitObject == DefaultTraits.Honor || traitObject == DefaultTraits.Mercy || traitObject == DefaultTraits.Generosity || traitObject == DefaultTraits.Valor || traitObject == DefaultTraits.Calculating)
				{
					int num = hero.CharacterObject.GetTraitLevel(traitObject);
					float num2 = MBRandom.RandomFloat;
					if (hero.IsPreacher && traitObject == DefaultTraits.Generosity)
					{
						num2 = 0.5f;
					}
					if (hero.IsMerchant && traitObject == DefaultTraits.Calculating)
					{
						num2 = 0.5f;
					}
					if ((double)num2 < 0.25)
					{
						num--;
						if (num < -1)
						{
							num = -1;
						}
					}
					if ((double)num2 > 0.75)
					{
						num++;
						if (num > 1)
						{
							num = 1;
						}
					}
					if (hero.IsGangLeader && (traitObject == DefaultTraits.Mercy || traitObject == DefaultTraits.Honor) && num > 0)
					{
						num = 0;
					}
					num = MBMath.ClampInt(num, traitObject.MinValue, traitObject.MaxValue);
					hero.SetTraitLevel(traitObject, num);
				}
			}
		}

		public static Hero DeliverOffSpring(Hero mother, Hero father, bool isOffspringFemale)
		{
			Debug.SilentAssert(mother.CharacterObject.Race == father.CharacterObject.Race, "", false, "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\HeroCreator.cs", "DeliverOffSpring", 482);
			Hero hero = HeroCreator.CreateNewHero(isOffspringFemale ? mother.CharacterObject : father.CharacterObject, 0);
			hero.ClearTraits();
			float randomFloat = MBRandom.RandomFloat;
			int num;
			if ((double)randomFloat < 0.1)
			{
				num = 0;
			}
			else if ((double)randomFloat < 0.5)
			{
				num = 1;
			}
			else if ((double)randomFloat < 0.9)
			{
				num = 2;
			}
			else
			{
				num = 3;
			}
			List<TraitObject> list = DefaultTraits.Personality.ToList<TraitObject>();
			list.Shuffle<TraitObject>();
			for (int i = 0; i < Math.Min(list.Count, num); i++)
			{
				int num2 = (((double)MBRandom.RandomFloat < 0.5) ? MBRandom.RandomInt(list[i].MinValue, 0) : MBRandom.RandomInt(1, list[i].MaxValue + 1));
				hero.SetTraitLevel(list[i], num2);
			}
			foreach (TraitObject traitObject in TraitObject.All.Except(DefaultTraits.Personality))
			{
				hero.SetTraitLevel(traitObject, ((double)MBRandom.RandomFloat < 0.5) ? mother.GetTraitLevel(traitObject) : father.GetTraitLevel(traitObject));
			}
			hero.SetNewOccupation(isOffspringFemale ? mother.Occupation : father.Occupation);
			int becomeChildAge = Campaign.Current.Models.AgeModel.BecomeChildAge;
			hero.CharacterObject.IsFemale = isOffspringFemale;
			hero.Mother = mother;
			hero.Father = father;
			MBEquipmentRoster randomElementInefficiently = Campaign.Current.Models.EquipmentSelectionModel.GetEquipmentRostersForDeliveredOffspring(hero).GetRandomElementInefficiently<MBEquipmentRoster>();
			if (randomElementInefficiently != null)
			{
				Equipment randomElementInefficiently2 = randomElementInefficiently.GetCivilianEquipments().GetRandomElementInefficiently<Equipment>();
				EquipmentHelper.AssignHeroEquipmentFromEquipment(hero, randomElementInefficiently2);
				Equipment equipment = new Equipment(false);
				equipment.FillFrom(randomElementInefficiently2, false);
				EquipmentHelper.AssignHeroEquipmentFromEquipment(hero, equipment);
			}
			else
			{
				Debug.FailedAssert("Equipment template not found", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\HeroCreator.cs", "DeliverOffSpring", 547);
			}
			TextObject textObject;
			TextObject textObject2;
			NameGenerator.Current.GenerateHeroNameAndHeroFullName(hero, out textObject, out textObject2, false);
			hero.SetName(textObject2, textObject);
			hero.HeroDeveloper.DeriveSkillsFromTraits(true, null);
			BodyProperties bodyProperties = mother.BodyProperties;
			BodyProperties bodyProperties2 = father.BodyProperties;
			int num3 = MBRandom.RandomInt();
			string text = (isOffspringFemale ? mother.HairTags : father.HairTags);
			string text2 = (isOffspringFemale ? mother.TattooTags : father.TattooTags);
			hero.StaticBodyProperties = BodyProperties.GetRandomBodyProperties(mother.CharacterObject.Race, isOffspringFemale, bodyProperties, bodyProperties2, 1, num3, text, father.BeardTags, text2).StaticProperties;
			hero.BornSettlement = HeroCreator.DecideBornSettlement(hero);
			if (mother == Hero.MainHero || father == Hero.MainHero)
			{
				hero.Clan = Clan.PlayerClan;
				hero.Culture = Hero.MainHero.Culture;
			}
			else
			{
				hero.Clan = father.Clan;
				hero.Culture = (((double)MBRandom.RandomFloat < 0.5) ? father.Culture : mother.Culture);
			}
			CampaignEventDispatcher.Instance.OnHeroCreated(hero, true);
			int heroComesOfAge = Campaign.Current.Models.AgeModel.HeroComesOfAge;
			if (hero.Age > (float)becomeChildAge || (hero.Age == (float)becomeChildAge && hero.BirthDay.GetDayOfYear < CampaignTime.Now.GetDayOfYear))
			{
				CampaignEventDispatcher.Instance.OnHeroGrowsOutOfInfancy(hero);
			}
			if (hero.Age > (float)heroComesOfAge || (hero.Age == (float)heroComesOfAge && hero.BirthDay.GetDayOfYear < CampaignTime.Now.GetDayOfYear))
			{
				CampaignEventDispatcher.Instance.OnHeroComesOfAge(hero);
			}
			return hero;
		}

		private static Settlement DecideBornSettlement(Hero child)
		{
			Settlement settlement;
			if (child.Mother.CurrentSettlement != null && (child.Mother.CurrentSettlement.IsTown || child.Mother.CurrentSettlement.IsVillage))
			{
				settlement = child.Mother.CurrentSettlement;
			}
			else if (child.Mother.PartyBelongedTo != null || child.Mother.PartyBelongedToAsPrisoner != null)
			{
				IMapPoint mapPoint3;
				if (child.Mother.PartyBelongedToAsPrisoner != null)
				{
					IMapPoint mapPoint2;
					if (!child.Mother.PartyBelongedToAsPrisoner.IsMobile)
					{
						IMapPoint mapPoint = child.Mother.PartyBelongedToAsPrisoner.Settlement;
						mapPoint2 = mapPoint;
					}
					else
					{
						IMapPoint mapPoint = child.Mother.PartyBelongedToAsPrisoner.MobileParty;
						mapPoint2 = mapPoint;
					}
					mapPoint3 = mapPoint2;
				}
				else
				{
					mapPoint3 = child.Mother.PartyBelongedTo;
				}
				settlement = SettlementHelper.FindNearestTown(null, mapPoint3);
			}
			else
			{
				settlement = child.Mother.HomeSettlement;
			}
			if (settlement == null)
			{
				settlement = ((child.Mother.Clan.Settlements.Count > 0) ? child.Mother.Clan.Settlements.GetRandomElement<Settlement>() : Town.AllTowns.GetRandomElement<Town>().Settlement);
			}
			return settlement;
		}

		private static FormationClass GetRandomPreferredUpgradeFormation()
		{
			int num = MBRandom.RandomInt(10);
			if (num < 4)
			{
				return (FormationClass)num;
			}
			return FormationClass.NumberOfAllFormations;
		}
	}
}
