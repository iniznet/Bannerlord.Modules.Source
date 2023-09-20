using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Helpers
{
	public static class HeroHelper
	{
		public static TextObject GetLastSeenText(Hero hero)
		{
			TextObject textObject = TextObject.Empty;
			if (hero.LastKnownClosestSettlement == null)
			{
				textObject = GameTexts.FindText("str_never_seen_encyclopedia_entry", null);
			}
			else
			{
				textObject = GameTexts.FindText("str_last_seen_encyclopedia_entry", null);
				textObject.SetTextVariable("SETTLEMENT", hero.LastKnownClosestSettlement.EncyclopediaLinkWithName);
				textObject.SetTextVariable("IS_IN_SETTLEMENT", (hero.LastKnownClosestSettlement == hero.CurrentSettlement) ? 1 : 0);
			}
			return textObject;
		}

		public static Settlement GetClosestSettlement(Hero hero)
		{
			Settlement settlement = null;
			if (hero.CurrentSettlement != null)
			{
				settlement = hero.CurrentSettlement;
			}
			else
			{
				MobileParty partyBelongedTo = hero.PartyBelongedTo;
				PartyBase partyBase = ((partyBelongedTo != null) ? partyBelongedTo.Party : null) ?? hero.PartyBelongedToAsPrisoner;
				if (partyBase != null)
				{
					if (partyBase.IsSettlement)
					{
						settlement = partyBase.Settlement;
					}
					else if (partyBase.IsMobile)
					{
						MobileParty mobileParty = partyBase.MobileParty;
						if (mobileParty.CurrentNavigationFace.IsValid())
						{
							settlement = SettlementHelper.FindNearestSettlement((Settlement x) => x.IsVillage || x.IsFortification, mobileParty);
						}
						else
						{
							Debug.FailedAssert("Mobileparty is nowhere to be found", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\Helpers.cs", "GetClosestSettlement", 1817);
						}
					}
				}
				else if (PlayerEncounter.Current != null && PlayerEncounter.Battle != null)
				{
					BattleSideEnum otherSide = PlayerEncounter.Battle.GetOtherSide(PlayerEncounter.Battle.PlayerSide);
					if (PlayerEncounter.Battle.PartiesOnSide(otherSide).Any((MapEventParty x) => x.Party.Owner == hero))
					{
						settlement = SettlementHelper.FindNearestSettlement((Settlement x) => x.IsVillage || x.IsFortification, MobileParty.MainParty);
					}
				}
			}
			if (settlement != null && !settlement.IsVillage && !settlement.IsFortification)
			{
				settlement = SettlementHelper.FindNearestSettlement((Settlement x) => x.IsVillage || x.IsFortification, settlement);
			}
			return settlement;
		}

		public static bool LordWillConspireWithLord(Hero lord, Hero otherLord, bool suggestingBetrayal)
		{
			Hero.OneToOneConversationHero.MapFaction.Leader.SetTextVariables();
			int num = 0;
			num += otherLord.RandomInt(-9, 11);
			num += lord.GetTraitLevel(DefaultTraits.Honor);
			if (suggestingBetrayal)
			{
				num--;
			}
			if (suggestingBetrayal && Hero.OneToOneConversationHero.Clan == Hero.OneToOneConversationHero.MapFaction.Leader.Clan)
			{
				TextObject textObject = new TextObject("{=0M6ApEr2}Surely you know that {FIRST_NAME} is {RELATIONSHIP} as well as my liege, and will always be able to count on my loyalty.", null);
				textObject.SetTextVariable("FIRST_NAME", Hero.OneToOneConversationHero.MapFaction.Leader.FirstName);
				textObject.SetTextVariable("RELATIONSHIP", ConversationHelper.HeroRefersToHero(Hero.OneToOneConversationHero, Hero.OneToOneConversationHero.MapFaction.Leader, true));
				MBTextManager.SetTextVariable("CONSPIRE_REFUSAL", textObject, false);
				return false;
			}
			if (num < 0)
			{
				if (suggestingBetrayal)
				{
					MBTextManager.SetTextVariable("CONSPIRE_REFUSAL", Campaign.Current.ConversationManager.FindMatchingTextOrNull("str_liege_support", lord.CharacterObject), false);
				}
				else
				{
					MBTextManager.SetTextVariable("CONSPIRE_REFUSAL", Campaign.Current.ConversationManager.FindMatchingTextOrNull("str_lord_intrigue_refuses", lord.CharacterObject), false);
				}
				return false;
			}
			return true;
		}

		public static bool UnderPlayerCommand(Hero hero)
		{
			return hero != null && ((hero.MapFaction != null && hero.MapFaction.Leader == Hero.MainHero) || (hero.IsNotable && hero.HomeSettlement.OwnerClan == Hero.MainHero.Clan) || hero.IsPlayerCompanion);
		}

		public static TextObject GetTitleInIndefiniteCase(Hero hero)
		{
			string text = hero.MapFaction.Culture.StringId;
			if (hero.IsFemale)
			{
				text += "_f";
			}
			if (hero.MapFaction.IsKingdomFaction && hero.MapFaction.Leader == hero)
			{
				return GameTexts.FindText("str_faction_ruler", text);
			}
			return GameTexts.FindText("str_faction_official", text);
		}

		public static TextObject GetCharacterTypeName(Hero hero)
		{
			if (hero.IsArtisan)
			{
				return GameTexts.FindText("str_charactertype_artisan", null);
			}
			if (hero.IsGangLeader)
			{
				return GameTexts.FindText("str_charactertype_gangleader", null);
			}
			if (hero.IsPreacher)
			{
				return GameTexts.FindText("str_charactertype_preacher", null);
			}
			if (hero.IsMerchant)
			{
				return GameTexts.FindText("str_charactertype_merchant", null);
			}
			if (hero.IsHeadman)
			{
				return GameTexts.FindText("str_charactertype_headman", null);
			}
			if (hero.IsRuralNotable)
			{
				return GameTexts.FindText("str_charactertype_ruralnotable", null);
			}
			if (hero.IsWanderer)
			{
				return GameTexts.FindText("str_charactertype_wanderer", null);
			}
			Clan clan = hero.Clan;
			if (clan != null && clan.IsClanTypeMercenary)
			{
				return GameTexts.FindText("str_charactertype_mercenary", null);
			}
			if (hero.IsMinorFactionHero)
			{
				return GameTexts.FindText("str_charactertype_minorfaction", null);
			}
			if (!hero.IsLord)
			{
				return GameTexts.FindText("str_charactertype_unknown", null);
			}
			if (hero.IsFemale)
			{
				return GameTexts.FindText("str_charactertype_lady", null);
			}
			return GameTexts.FindText("str_charactertype_lord", null);
		}

		public static TextObject GetOccupiedEventReasonText(Hero hero)
		{
			TextObject textObject;
			if (!hero.CanHaveQuestsOrIssues())
			{
				textObject = GameTexts.FindText("str_hero_busy_issue_quest", null);
			}
			else
			{
				textObject = GameTexts.FindText("str_hero_busy", null);
			}
			return textObject;
		}

		public static List<string> OrderHeroesOnPlayerSideByPriority()
		{
			List<Hero> list = new List<Hero>();
			foreach (MapEventParty mapEventParty in MobileParty.MainParty.MapEvent.PartiesOnSide(MobileParty.MainParty.MapEvent.PlayerSide))
			{
				if (mapEventParty.Party.LeaderHero != null)
				{
					MobileParty mobileParty = mapEventParty.Party.MobileParty;
					MobileParty mobileParty2;
					if (mobileParty == null)
					{
						mobileParty2 = null;
					}
					else
					{
						Army army = mobileParty.Army;
						mobileParty2 = ((army != null) ? army.LeaderParty : null);
					}
					if (mobileParty2 != mapEventParty.Party.MobileParty)
					{
						list.Add(mapEventParty.Party.LeaderHero);
					}
				}
			}
			return list.OrderByDescending((Hero t) => Campaign.Current.Models.EncounterModel.GetCharacterSergeantScore(t)).ToList<Hero>().ConvertAll<string>((Hero t) => t.CharacterObject.StringId);
		}

		public static bool WillLordAttack()
		{
			if (PlayerEncounter.Current != null && PlayerEncounter.Current.PlayerSide == BattleSideEnum.Defender && (PlayerEncounter.EncounteredMobileParty == null || PlayerEncounter.EncounteredMobileParty.Ai.DoNotAttackMainPartyUntil.IsPast))
			{
				PartyBase partyBase = ((Campaign.Current.ConversationManager.ConversationParty == null) ? PlayerEncounter.EncounteredParty : Campaign.Current.ConversationManager.ConversationParty.Party);
				if (partyBase.Owner != null && partyBase.LeaderHero != null && FactionManager.IsAtWarAgainstFaction(partyBase.MapFaction, Hero.MainHero.MapFaction))
				{
					return true;
				}
			}
			return false;
		}

		public static void SetPlayerSalutation()
		{
			if (Hero.OneToOneConversationHero.IsLord)
			{
				MBTextManager.SetTextVariable("PLAYER_SALUTATION", Hero.MainHero.Name, false);
				return;
			}
			if (Hero.OneToOneConversationHero.IsPlayerCompanion)
			{
				MBTextManager.SetTextVariable("PLAYER_SALUTATION", GameTexts.FindText("str_player_salutation_captain", null), false);
				return;
			}
			if (Hero.MainHero.IsFemale)
			{
				MBTextManager.SetTextVariable("PLAYER_SALUTATION", GameTexts.FindText("str_player_salutation_madame", null), false);
				return;
			}
			MBTextManager.SetTextVariable("PLAYER_SALUTATION", GameTexts.FindText("str_player_salutation_sir", null), false);
		}

		public static void DetermineInitialLevel(Hero hero)
		{
			hero.HeroDeveloper.CheckInitialLevel();
		}

		public static void SpawnHeroForTheFirstTime(Hero hero, Settlement spawnSettlement)
		{
			hero.BornSettlement = spawnSettlement;
			EnterSettlementAction.ApplyForCharacterOnly(hero, spawnSettlement);
			hero.ChangeState(Hero.CharacterStates.Active);
		}

		public static int DefaultRelation(Hero hero, Hero otherHero)
		{
			if (hero.Clan != null && hero.Clan.IsNoble && hero.Clan == otherHero.Clan)
			{
				return 40;
			}
			if (hero.MapFaction == otherHero.MapFaction && hero.CharacterObject.Culture == otherHero.CharacterObject.Culture && hero.Age > 35f && otherHero.Age > 35f && HeroHelper.NPCPersonalityClashWithNPC(hero, otherHero) > 40)
			{
				return -5;
			}
			if (hero.MapFaction == otherHero.MapFaction && hero.CharacterObject.Culture == otherHero.CharacterObject.Culture && hero.Age > 35f && otherHero.Age > 35f)
			{
				return 25;
			}
			if (hero.MapFaction == otherHero.MapFaction && hero.CharacterObject.Culture == otherHero.CharacterObject.Culture)
			{
				return 10;
			}
			return 0;
		}

		public static int CalculateTotalStrength(Hero hero)
		{
			float num = 1f;
			if (hero.PartyBelongedTo != null && hero.PartyBelongedTo.LeaderHero == hero)
			{
				num += hero.PartyBelongedTo.Party.TotalStrength;
			}
			if (hero.Clan != null && hero.Clan.Leader == hero)
			{
				foreach (Hero hero2 in hero.Clan.Companions)
				{
					if (hero2.PartyBelongedTo != null && hero2.PartyBelongedTo.LeaderHero == hero2)
					{
						num += hero2.PartyBelongedTo.Party.TotalStrength;
					}
				}
			}
			return MathF.Round(num);
		}

		public static bool IsCompanionInPlayerParty(Hero hero)
		{
			return hero != null && hero.IsPlayerCompanion && hero.PartyBelongedTo == MobileParty.MainParty;
		}

		public static bool NPCPoliticalDifferencesWithNPC(Hero firstNPC, Hero secondNPC)
		{
			bool flag = firstNPC.GetTraitLevel(DefaultTraits.Egalitarian) > 0;
			bool flag2 = firstNPC.GetTraitLevel(DefaultTraits.Oligarchic) > 0;
			bool flag3 = firstNPC.GetTraitLevel(DefaultTraits.Authoritarian) > 0;
			bool flag4 = secondNPC.GetTraitLevel(DefaultTraits.Egalitarian) > 0;
			bool flag5 = secondNPC.GetTraitLevel(DefaultTraits.Oligarchic) > 0;
			bool flag6 = secondNPC.GetTraitLevel(DefaultTraits.Authoritarian) > 0;
			return flag != flag4 || flag2 != flag5 || flag3 != flag6;
		}

		public static int NPCPersonalityClashWithNPC(Hero firstNPC, Hero secondNPC)
		{
			int num = 0;
			foreach (TraitObject traitObject in DefaultTraits.Personality)
			{
				if (traitObject != DefaultTraits.Calculating && traitObject != DefaultTraits.Generosity)
				{
					int traitLevel = firstNPC.CharacterObject.GetTraitLevel(traitObject);
					int traitLevel2 = secondNPC.CharacterObject.GetTraitLevel(traitObject);
					if (traitLevel > 0 && traitLevel2 < 0)
					{
						num += 2;
					}
					if (traitLevel2 > 0 && traitLevel < 0)
					{
						num += 2;
					}
					if (traitLevel == 0 && traitLevel2 < 0)
					{
						num++;
					}
					if (traitLevel2 == 0 && traitLevel < 0)
					{
						num++;
					}
				}
			}
			CharacterObject characterObject = firstNPC.CharacterObject;
			if (characterObject.GetTraitLevel(DefaultTraits.Generosity) == -1)
			{
				num++;
			}
			if (secondNPC.GetTraitLevel(DefaultTraits.Generosity) == -1)
			{
				num++;
			}
			if (characterObject.GetTraitLevel(DefaultTraits.Honor) == -1)
			{
				num++;
			}
			if (secondNPC.GetTraitLevel(DefaultTraits.Honor) == -1)
			{
				num++;
			}
			num *= 5;
			return num;
		}

		public static int TraitHarmony(Hero considerer, TraitObject trait, Hero consideree, bool sensitive)
		{
			int traitLevel = considerer.GetTraitLevel(trait);
			int traitLevel2 = consideree.GetTraitLevel(trait);
			if (traitLevel > 0 && traitLevel2 > 0)
			{
				return 3;
			}
			if (traitLevel == 0 && traitLevel2 > 0)
			{
				return 1;
			}
			if (traitLevel < 0 && traitLevel2 < 0)
			{
				return 1;
			}
			if (traitLevel > 0 && traitLevel2 < 0)
			{
				return -3;
			}
			if (traitLevel == 0 && traitLevel2 < 0)
			{
				return -1;
			}
			if (traitLevel < 0 && traitLevel2 > 0)
			{
				return -1;
			}
			return 0;
		}

		public static float CalculateReliabilityConstant(Hero hero, float maxValueConstant = 1f)
		{
			int traitLevel = hero.GetTraitLevel(DefaultTraits.Honor);
			return maxValueConstant * ((2.5f + (float)MathF.Min(2, MathF.Max(-2, traitLevel))) / 5f);
		}

		public static void SetPropertiesToTextObject(this Hero hero, TextObject textObject, string tagName)
		{
			StringHelpers.SetCharacterProperties(tagName, hero.CharacterObject, textObject, false);
		}

		public static void SetPropertiesToTextObject(this Settlement settlement, TextObject textObject, string tagName)
		{
			StringHelpers.SetSettlementProperties(tagName, settlement, textObject, false);
		}

		public static bool HeroCanRecruitFromHero(Hero buyerHero, Hero sellerHero, int index)
		{
			return index <= Campaign.Current.Models.VolunteerModel.MaximumIndexHeroCanRecruitFromHero(buyerHero, sellerHero, -101);
		}

		public static List<CharacterObject> GetVolunteerTroopsOfHeroForRecruitment(Hero hero)
		{
			List<CharacterObject> list = new List<CharacterObject>();
			for (int i = 0; i < 6; i++)
			{
				list.Add(hero.VolunteerTypes[i]);
			}
			return list;
		}

		public static Clan GetRandomClanForNotable(Hero notable)
		{
			float num = 0f;
			List<Clan> list = new List<Clan>();
			bool isWanderer = notable.IsWanderer;
			bool isMerchant = notable.IsMerchant;
			bool isRuralNotable = notable.IsRuralNotable;
			bool isArtisan = notable.IsArtisan;
			if (notable.IsPreacher)
			{
				num = 0.5f;
				list = Clan.NonBanditFactions.Where((Clan x) => x.IsSect).ToList<Clan>();
			}
			if (notable.IsGangLeader)
			{
				num = 0.5f;
				list = Clan.NonBanditFactions.Where((Clan x) => x.IsMafia).ToList<Clan>();
			}
			if (MBRandom.RandomFloat >= num)
			{
				return null;
			}
			foreach (Hero hero in notable.HomeSettlement.Notables)
			{
				if (list.Contains(hero.SupporterOf))
				{
					list.Remove(hero.SupporterOf);
				}
			}
			float num2 = 0f;
			ILookup<Clan, Settlement> lookup = Settlement.All.Where((Settlement x) => (x.IsVillage && x.Village.IsCastle) || x.IsTown || x.IsHideout).ToLookup((Settlement x) => x.OwnerClan);
			foreach (Clan clan in list)
			{
				num2 += HeroHelper.GetProbabilityForClan(clan, lookup[clan], notable);
			}
			num2 *= MBRandom.RandomFloat;
			foreach (Clan clan2 in list)
			{
				num2 -= HeroHelper.GetProbabilityForClan(clan2, lookup[clan2], notable);
				if (num2 <= 0f)
				{
					return clan2;
				}
			}
			return null;
		}

		public static float GetProbabilityForClan(Clan clan, IEnumerable<Settlement> applicableSettlements, Hero notable)
		{
			float num = 1f;
			if (clan.Culture == notable.Culture)
			{
				num *= 3f;
			}
			float num2 = float.MaxValue;
			foreach (Settlement settlement in applicableSettlements)
			{
				float num3 = settlement.Position2D.DistanceSquared(notable.HomeSettlement.Position2D);
				if (num3 < num2)
				{
					num2 = num3;
				}
			}
			num /= 50f + num2;
			return num;
		}

		public static CampaignTime GetRandomBirthDayForAge(float age)
		{
			float num = MBRandom.RandomFloatRanged(0f, (float)CampaignTime.Now.GetDayOfYear);
			float num2 = (float)CampaignTime.Now.GetYear - age;
			return CampaignTime.Days(num) + CampaignTime.Years(num2);
		}

		public static void GetRandomDeathDayAndBirthDay(int deathAge, out CampaignTime birthday, out CampaignTime deathday)
		{
			int num = 84;
			int num2 = MBRandom.RandomInt(num);
			birthday = CampaignTime.Years((float)(CampaignTime.Now.GetYear - deathAge - 1)) - CampaignTime.Days((float)num2);
			deathday = birthday + CampaignTime.Years((float)deathAge) + CampaignTime.Days((float)MBRandom.RandomInt(num - 1));
		}

		public static float StartRecruitingMoneyLimit(Hero hero)
		{
			if (hero.Clan == Clan.PlayerClan)
			{
				return 0f;
			}
			return 50f + ((hero.PartyBelongedTo != null) ? ((float)MathF.Min(150, hero.PartyBelongedTo.MemberRoster.TotalManCount) * 20f) : 0f);
		}

		public static float StartRecruitingMoneyLimitForClanLeader(Hero hero)
		{
			if (hero.Clan == Clan.PlayerClan)
			{
				return 0f;
			}
			return 50f + ((hero.Clan != null && hero.Clan.Leader != null && hero.Clan.Leader.PartyBelongedTo != null) ? ((float)hero.Clan.Leader.PartyBelongedTo.TotalWage + (float)hero.Clan.Leader.PartyBelongedTo.MemberRoster.TotalManCount * 40f) : 0f);
		}
	}
}
