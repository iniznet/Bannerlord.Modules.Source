using System;
using System.Collections.Generic;
using Helpers;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Buildings;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	public class DefaultPartySizeLimitModel : PartySizeLimitModel
	{
		public DefaultPartySizeLimitModel()
		{
			this._quarterMasterText = GameTexts.FindText("str_clan_role", "quartermaster");
		}

		public override ExplainedNumber GetPartyMemberSizeLimit(PartyBase party, bool includeDescriptions = false)
		{
			ExplainedNumber explainedNumber = new ExplainedNumber(0f, includeDescriptions, null);
			if (!party.IsMobile)
			{
				return explainedNumber;
			}
			if (party.MobileParty.IsGarrison)
			{
				return this.CalculateGarrisonPartySizeLimit(party.MobileParty, includeDescriptions);
			}
			return this.CalculateMobilePartyMemberSizeLimit(party.MobileParty, includeDescriptions);
		}

		public override ExplainedNumber GetPartyPrisonerSizeLimit(PartyBase party, bool includeDescriptions = false)
		{
			if (party.IsSettlement)
			{
				return this.CalculateSettlementPartyPrisonerSizeLimitInternal(party.Settlement, includeDescriptions);
			}
			return this.CalculateMobilePartyPrisonerSizeLimitInternal(party, includeDescriptions);
		}

		private ExplainedNumber CalculateMobilePartyMemberSizeLimit(MobileParty party, bool includeDescriptions = false)
		{
			ExplainedNumber explainedNumber = new ExplainedNumber(20f, includeDescriptions, this._baseSizeText);
			if (party.LeaderHero != null && party.LeaderHero.Clan != null && !party.IsCaravan)
			{
				if (party.MapFaction != null && party.MapFaction.IsKingdomFaction && party.LeaderHero.MapFaction.Leader == party.LeaderHero)
				{
					explainedNumber.Add(20f, this._factionLeaderText, null);
				}
				if (party.LeaderHero.GetPerkValue(DefaultPerks.OneHanded.Prestige))
				{
					explainedNumber.Add(DefaultPerks.OneHanded.Prestige.SecondaryBonus, DefaultPerks.OneHanded.Prestige.Name, null);
				}
				if (party.LeaderHero.GetPerkValue(DefaultPerks.TwoHanded.Hope))
				{
					explainedNumber.Add(DefaultPerks.TwoHanded.Hope.SecondaryBonus, DefaultPerks.TwoHanded.Hope.Name, null);
				}
				if (party.LeaderHero.GetPerkValue(DefaultPerks.Athletics.ImposingStature))
				{
					explainedNumber.Add(DefaultPerks.Athletics.ImposingStature.SecondaryBonus, DefaultPerks.Athletics.ImposingStature.Name, null);
				}
				if (party.LeaderHero.GetPerkValue(DefaultPerks.Bow.MerryMen))
				{
					explainedNumber.Add(DefaultPerks.Bow.MerryMen.PrimaryBonus, DefaultPerks.Bow.MerryMen.Name, null);
				}
				if (party.LeaderHero.GetPerkValue(DefaultPerks.Tactics.HordeLeader))
				{
					explainedNumber.Add(DefaultPerks.Tactics.HordeLeader.PrimaryBonus, DefaultPerks.Tactics.HordeLeader.Name, null);
				}
				if (party.LeaderHero.GetPerkValue(DefaultPerks.Scouting.MountedScouts))
				{
					explainedNumber.Add(DefaultPerks.Scouting.MountedScouts.SecondaryBonus, DefaultPerks.Scouting.MountedScouts.Name, null);
				}
				if (party.LeaderHero.GetPerkValue(DefaultPerks.Leadership.Authority))
				{
					explainedNumber.Add(DefaultPerks.Leadership.Authority.SecondaryBonus, DefaultPerks.Leadership.Authority.Name, null);
				}
				Clan actualClan = party.ActualClan;
				bool flag;
				if (actualClan == null)
				{
					flag = false;
				}
				else
				{
					Hero leader = actualClan.Leader;
					bool? flag2 = ((leader != null) ? new bool?(leader.GetPerkValue(DefaultPerks.Leadership.LeaderOfMasses)) : null);
					bool flag3 = true;
					flag = (flag2.GetValueOrDefault() == flag3) & (flag2 != null);
				}
				if (flag)
				{
					int num = 0;
					using (List<Settlement>.Enumerator enumerator = party.ActualClan.Settlements.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (enumerator.Current.IsTown)
							{
								num++;
							}
						}
					}
					float num2 = (float)num * DefaultPerks.Leadership.LeaderOfMasses.PrimaryBonus;
					if (num2 > 0f)
					{
						explainedNumber.Add(num2, DefaultPerks.Leadership.LeaderOfMasses.Name, null);
					}
				}
				if (party.LeaderHero.Clan.Leader == party.LeaderHero)
				{
					if (party.LeaderHero.Clan.Tier >= 5 && party.MapFaction.IsKingdomFaction && ((Kingdom)party.MapFaction).ActivePolicies.Contains(DefaultPolicies.NobleRetinues))
					{
						explainedNumber.Add(40f, DefaultPolicies.NobleRetinues.Name, null);
					}
					if (party.MapFaction.IsKingdomFaction && party.MapFaction.Leader == party.LeaderHero && ((Kingdom)party.MapFaction).ActivePolicies.Contains(DefaultPolicies.RoyalGuard))
					{
						explainedNumber.Add(60f, DefaultPolicies.RoyalGuard.Name, null);
					}
					if (party.LeaderHero.Clan.Tier > 0)
					{
						explainedNumber.Add((float)(party.LeaderHero.Clan.Tier * 25), this._clanTierText, null);
					}
				}
				else if (party.LeaderHero.Clan.Tier > 0)
				{
					explainedNumber.Add((float)(party.LeaderHero.Clan.Tier * 15), this._clanTierText, null);
				}
				explainedNumber.Add((float)(party.EffectiveQuartermaster.GetSkillValue(DefaultSkills.Steward) / 4), this._quarterMasterText, null);
				this.AddMobilePartyLeaderPartySizePerkEffects(party, ref explainedNumber);
				this.AddUltimateLeaderPerkEffectForPartySize(party, ref explainedNumber);
				if (DefaultPartySizeLimitModel._addAdditionalPartySizeAsCheat && party.IsMainParty && Game.Current.CheatMode)
				{
					explainedNumber.Add(5000f, new TextObject("{=!}Additional size from extra party cheat", null), null);
				}
			}
			else if (party.IsCaravan)
			{
				if (party.Party.Owner == Hero.MainHero)
				{
					explainedNumber.Add(10f, this._randomSizeBonusTemporary, null);
				}
				else
				{
					Hero owner = party.Party.Owner;
					if (owner != null && owner.IsNotable)
					{
						explainedNumber.Add((float)(10 * ((party.Party.Owner.Power < 100f) ? 1 : ((party.Party.Owner.Power < 200f) ? 2 : 3))), this._randomSizeBonusTemporary, null);
					}
				}
			}
			else if (party.IsVillager)
			{
				explainedNumber.Add(40f, this._randomSizeBonusTemporary, null);
			}
			return explainedNumber;
		}

		private ExplainedNumber CalculateGarrisonPartySizeLimit(MobileParty party, bool includeDescriptions = false)
		{
			ExplainedNumber explainedNumber = new ExplainedNumber(200f, includeDescriptions, this._baseSizeText);
			this.GetLeadershipSkillLevelEffect(party, DefaultPartySizeLimitModel.LimitType.GarrisonPartySizeLimit, ref explainedNumber);
			explainedNumber.Add((float)this.GetTownBonus(party), this._townBonusText, null);
			this.AddGarrisonOwnerPerkEffects(party.CurrentSettlement, ref explainedNumber);
			this.AddSettlementProjectBonuses(party.Party, ref explainedNumber);
			return explainedNumber;
		}

		private ExplainedNumber CalculateSettlementPartyPrisonerSizeLimitInternal(Settlement settlement, bool includeDescriptions = false)
		{
			ExplainedNumber explainedNumber = new ExplainedNumber(60f, includeDescriptions, this._baseSizeText);
			Town town = settlement.Town;
			int num = ((town != null) ? town.GetWallLevel() : 0);
			if (num > 0)
			{
				explainedNumber.Add((float)(num * 40), this._wallLevelBonusText, null);
			}
			return explainedNumber;
		}

		private ExplainedNumber CalculateMobilePartyPrisonerSizeLimitInternal(PartyBase party, bool includeDescriptions = false)
		{
			ExplainedNumber explainedNumber = new ExplainedNumber(10f, includeDescriptions, this._baseSizeText);
			explainedNumber.Add((float)this.GetCurrentPartySizeEffect(party), this._currentPartySizeBonusText, null);
			this.AddMobilePartyLeaderPrisonerSizePerkEffects(party, ref explainedNumber);
			if (DefaultPartySizeLimitModel._addAdditionalPrisonerSizeAsCheat && party.IsMobile && party.MobileParty.IsMainParty && Game.Current.CheatMode)
			{
				explainedNumber.Add(5000f, new TextObject("{=!}Additional size from extra prisoner cheat", null), null);
			}
			return explainedNumber;
		}

		private void GetLeadershipSkillLevelEffect(MobileParty party, DefaultPartySizeLimitModel.LimitType type, ref ExplainedNumber partySizeBonus)
		{
			Hero hero;
			if (!party.IsGarrison)
			{
				hero = party.LeaderHero;
			}
			else if (party == null)
			{
				hero = null;
			}
			else
			{
				Settlement currentSettlement = party.CurrentSettlement;
				if (currentSettlement == null)
				{
					hero = null;
				}
				else
				{
					Clan ownerClan = currentSettlement.OwnerClan;
					hero = ((ownerClan != null) ? ownerClan.Leader : null);
				}
			}
			Hero hero2 = hero;
			if (hero2 != null && type == DefaultPartySizeLimitModel.LimitType.GarrisonPartySizeLimit)
			{
				SkillHelper.AddSkillBonusForCharacter(DefaultSkills.Leadership, DefaultSkillEffects.LeadershipGarrisonSizeBonus, hero2.CharacterObject, ref partySizeBonus, -1, true, 0);
			}
		}

		private void AddMobilePartyLeaderPartySizePerkEffects(MobileParty party, ref ExplainedNumber result)
		{
			if (party.LeaderHero != null)
			{
				if (party.LeaderHero.GetPerkValue(DefaultPerks.Leadership.UpliftingSpirit))
				{
					result.Add(DefaultPerks.Leadership.UpliftingSpirit.SecondaryBonus, DefaultPerks.Leadership.UpliftingSpirit.Name, null);
				}
				if (party.LeaderHero.GetPerkValue(DefaultPerks.Leadership.TalentMagnet))
				{
					result.Add(DefaultPerks.Leadership.TalentMagnet.PrimaryBonus, DefaultPerks.Leadership.TalentMagnet.Name, null);
				}
			}
		}

		private void AddUltimateLeaderPerkEffectForPartySize(MobileParty party, ref ExplainedNumber result)
		{
			if (party.LeaderHero != null && party.LeaderHero.GetSkillValue(DefaultSkills.Leadership) > 250 && party.LeaderHero.GetPerkValue(DefaultPerks.Leadership.UltimateLeader))
			{
				result.Add((float)(party.LeaderHero.GetSkillValue(DefaultSkills.Leadership) - 250) * DefaultPerks.Leadership.UltimateLeader.PrimaryBonus, this._leadershipPerkUltimateLeaderBonusText, null);
			}
		}

		private void AddMobilePartyLeaderPrisonerSizePerkEffects(PartyBase party, ref ExplainedNumber result)
		{
			if (party.LeaderHero != null)
			{
				if (party.LeaderHero.GetPerkValue(DefaultPerks.TwoHanded.Terror))
				{
					result.Add(DefaultPerks.TwoHanded.Terror.SecondaryBonus, DefaultPerks.TwoHanded.Terror.Name, null);
				}
				if (party.LeaderHero.GetPerkValue(DefaultPerks.Athletics.Stamina))
				{
					result.Add(DefaultPerks.Athletics.Stamina.SecondaryBonus, DefaultPerks.Athletics.Stamina.Name, null);
				}
				if (party.LeaderHero.GetPerkValue(DefaultPerks.Roguery.Manhunter))
				{
					result.Add(DefaultPerks.Roguery.Manhunter.SecondaryBonus, DefaultPerks.Roguery.Manhunter.Name, null);
				}
				if (party.LeaderHero != null && party.LeaderHero.GetPerkValue(DefaultPerks.Scouting.VantagePoint))
				{
					result.Add(DefaultPerks.Scouting.VantagePoint.SecondaryBonus, DefaultPerks.Scouting.VantagePoint.Name, null);
				}
			}
		}

		private void AddGarrisonOwnerPerkEffects(Settlement currentSettlement, ref ExplainedNumber result)
		{
			if (currentSettlement != null && currentSettlement.IsTown)
			{
				PerkHelper.AddPerkBonusForTown(DefaultPerks.OneHanded.CorpsACorps, currentSettlement.Town, ref result);
				PerkHelper.AddPerkBonusForTown(DefaultPerks.Leadership.VeteransRespect, currentSettlement.Town, ref result);
			}
		}

		public override int GetTierPartySizeEffect(int tier)
		{
			if (tier >= 1)
			{
				return 15 * tier;
			}
			return 0;
		}

		public override int GetAssumedPartySizeForLordParty(Hero leaderHero, IFaction partyMapFaction, Clan actualClan)
		{
			ExplainedNumber explainedNumber = new ExplainedNumber(20f, false, this._baseSizeText);
			if (leaderHero != null && leaderHero.Clan != null)
			{
				if (partyMapFaction != null && partyMapFaction.IsKingdomFaction && leaderHero.MapFaction.Leader == leaderHero)
				{
					explainedNumber.Add(20f, this._factionLeaderText, null);
				}
				if (leaderHero.GetPerkValue(DefaultPerks.OneHanded.Prestige))
				{
					explainedNumber.Add(DefaultPerks.OneHanded.Prestige.SecondaryBonus, DefaultPerks.OneHanded.Prestige.Name, null);
				}
				if (leaderHero.GetPerkValue(DefaultPerks.TwoHanded.Hope))
				{
					explainedNumber.Add(DefaultPerks.TwoHanded.Hope.SecondaryBonus, DefaultPerks.TwoHanded.Hope.Name, null);
				}
				if (leaderHero.GetPerkValue(DefaultPerks.Athletics.ImposingStature))
				{
					explainedNumber.Add(DefaultPerks.Athletics.ImposingStature.SecondaryBonus, DefaultPerks.Athletics.ImposingStature.Name, null);
				}
				if (leaderHero.GetPerkValue(DefaultPerks.Bow.MerryMen))
				{
					explainedNumber.Add(DefaultPerks.Bow.MerryMen.PrimaryBonus, DefaultPerks.Bow.MerryMen.Name, null);
				}
				if (leaderHero.GetPerkValue(DefaultPerks.Tactics.HordeLeader))
				{
					explainedNumber.Add(DefaultPerks.Tactics.HordeLeader.PrimaryBonus, DefaultPerks.Tactics.HordeLeader.Name, null);
				}
				if (leaderHero.GetPerkValue(DefaultPerks.Scouting.MountedScouts))
				{
					explainedNumber.Add(DefaultPerks.Scouting.MountedScouts.SecondaryBonus, DefaultPerks.Scouting.MountedScouts.Name, null);
				}
				if (leaderHero.GetPerkValue(DefaultPerks.Leadership.Authority))
				{
					explainedNumber.Add(DefaultPerks.Leadership.Authority.SecondaryBonus, DefaultPerks.Leadership.Authority.Name, null);
				}
				if (actualClan != null)
				{
					Hero leader = actualClan.Leader;
					bool? flag = ((leader != null) ? new bool?(leader.GetPerkValue(DefaultPerks.Leadership.LeaderOfMasses)) : null);
					bool flag2 = true;
					if ((flag.GetValueOrDefault() == flag2) & (flag != null))
					{
						int num = 0;
						using (List<Settlement>.Enumerator enumerator = actualClan.Settlements.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								if (enumerator.Current.IsTown)
								{
									num++;
								}
							}
						}
						float num2 = (float)num * DefaultPerks.Leadership.LeaderOfMasses.PrimaryBonus;
						if (num2 > 0f)
						{
							explainedNumber.Add(num2, DefaultPerks.Leadership.LeaderOfMasses.Name, null);
						}
					}
				}
				if (leaderHero.Clan.Leader == leaderHero)
				{
					if (leaderHero.Clan.Tier >= 5 && partyMapFaction.IsKingdomFaction && ((Kingdom)partyMapFaction).ActivePolicies.Contains(DefaultPolicies.NobleRetinues))
					{
						explainedNumber.Add(40f, DefaultPolicies.NobleRetinues.Name, null);
					}
					if (partyMapFaction.IsKingdomFaction && partyMapFaction.Leader == leaderHero && ((Kingdom)partyMapFaction).ActivePolicies.Contains(DefaultPolicies.RoyalGuard))
					{
						explainedNumber.Add(60f, DefaultPolicies.RoyalGuard.Name, null);
					}
					if (leaderHero.Clan.Tier > 0)
					{
						explainedNumber.Add((float)(leaderHero.Clan.Tier * 25), this._clanTierText, null);
					}
				}
				else if (leaderHero.Clan.Tier > 0)
				{
					explainedNumber.Add((float)(leaderHero.Clan.Tier * 15), this._clanTierText, null);
				}
			}
			return (int)explainedNumber.BaseNumber;
		}

		private void AddSettlementProjectBonuses(PartyBase party, ref ExplainedNumber result)
		{
			if (party.Owner != null)
			{
				Settlement currentSettlement = party.MobileParty.CurrentSettlement;
				if (currentSettlement != null && (currentSettlement.IsTown || currentSettlement.IsCastle))
				{
					foreach (Building building in currentSettlement.Town.Buildings)
					{
						float buildingEffectAmount = building.GetBuildingEffectAmount(BuildingEffectEnum.GarrisonCapacity);
						if (buildingEffectAmount > 0f)
						{
							result.Add(buildingEffectAmount, building.Name, null);
						}
					}
				}
			}
		}

		private int GetTownBonus(MobileParty party)
		{
			Settlement currentSettlement = party.CurrentSettlement;
			if (currentSettlement != null && currentSettlement.IsFortification && currentSettlement.IsTown)
			{
				return 200;
			}
			return 0;
		}

		private int GetCurrentPartySizeEffect(PartyBase party)
		{
			return party.NumberOfHealthyMembers / 2;
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("show_party_size_limit_detail", "campaign")]
		public static string ShowPartySizeDetail(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (CampaignCheats.CheckParameters(strings, 0) || CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"campaign.show_party_size_limit_detail [PartyName]\".";
			}
			string text = CampaignCheats.ConcatenateString(strings);
			foreach (MobileParty mobileParty in MobileParty.All)
			{
				if (string.Equals(text, mobileParty.Name.ToString(), StringComparison.OrdinalIgnoreCase))
				{
					new ExplainedNumber(0f, true, null);
					return Campaign.Current.Models.PartySizeLimitModel.GetPartyMemberSizeLimit(mobileParty.Party, true).ToString() + "\nPrisoner size limit:\n" + Campaign.Current.Models.PartySizeLimitModel.GetPartyPrisonerSizeLimit(mobileParty.Party, true).ToString() + "\nGarrison size limit:\n" + Campaign.Current.Models.PartySizeLimitModel.GetPartyMemberSizeLimit(mobileParty.Party, true).ToString();
				}
			}
			return "Couldn't find the party: " + text;
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("fill_party", "campaign")]
		public static string FillParty(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (CampaignCheats.CheckHelp(strings) || CampaignCheats.CheckParameters(strings, 0))
			{
				return "Format is \"campaign.fill_party [Number] [Culture]\". Parameters are optional.";
			}
			if (MobileParty.MainParty.MapEvent == null)
			{
				int num;
				int.TryParse(strings[0], out num);
				strings.RemoveAt(0);
				CultureObject culture = null;
				if (strings.Count > 0)
				{
					culture = Campaign.Current.ObjectManager.GetObject<CultureObject>(CampaignCheats.ConcatenateString(strings));
					if (culture == null)
					{
						return "Culture couldn't be found!";
					}
				}
				Func<Clan, bool> <>9__0;
				for (int i = 0; i < num; i++)
				{
					MBReadOnlyList<Clan> all = Clan.All;
					Func<Clan, bool> func;
					if ((func = <>9__0) == null)
					{
						func = (<>9__0 = (Clan x) => x.Culture != null && (culture == null || culture == x.Culture) && !x.IsMinorFaction && !x.IsNeutralClan && !x.IsBanditFaction);
					}
					Clan randomElementWithPredicate = all.GetRandomElementWithPredicate(func);
					CharacterObject characterObject = randomElementWithPredicate.Culture.BasicTroop;
					if (MBRandom.RandomFloat < 0.15f && randomElementWithPredicate.Culture.EliteBasicTroop != null)
					{
						characterObject = randomElementWithPredicate.Culture.EliteBasicTroop;
					}
					CharacterObject randomElementInefficiently = CharacterHelper.GetTroopTree(characterObject, 1f, float.MaxValue).GetRandomElementInefficiently<CharacterObject>();
					DefaultPartySizeLimitModel.GiveTroopsToMainParty(new List<string>
					{
						(randomElementInefficiently ?? characterObject).StringId,
						"1"
					});
				}
				return "Success";
			}
			return "Main party shouldn't be in a map event.";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("give_troops", "campaign")]
		public static string GiveTroopsToMainParty(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (CampaignCheats.CheckParameters(strings, 0))
			{
				return "Write \"campaign.give_troops help\" for help";
			}
			if (CampaignCheats.CheckHelp(strings) || !CampaignCheats.CheckParameters(strings, 2))
			{
				string text = "";
				text += "Usage : campaign.give_troops [TroopId] [Number]";
				text += "\n";
				text += "\n";
				text += "Available troops";
				text += "\n";
				text += "==============================";
				text += "\n";
				foreach (CharacterObject characterObject in MBObjectManager.Instance.GetObjectTypeList<CharacterObject>())
				{
					if (characterObject.Occupation == Occupation.Soldier || characterObject.Occupation == Occupation.Gangster)
					{
						text = string.Concat(new object[] { text, "Id: ", characterObject.StringId, " Name: ", characterObject.Name, "\n" });
					}
				}
				return text;
			}
			string text2 = strings[0];
			CharacterObject characterObject2 = MBObjectManager.Instance.GetObject<CharacterObject>(text2);
			if (characterObject2 == null)
			{
				foreach (CharacterObject characterObject3 in MBObjectManager.Instance.GetObjectTypeList<CharacterObject>())
				{
					if ((characterObject3.Occupation == Occupation.Soldier || characterObject3.Occupation == Occupation.Gangster) && characterObject3.StringId == text2.ToLower())
					{
						characterObject2 = characterObject3;
						break;
					}
				}
			}
			if (characterObject2 == null)
			{
				return "Troop with id \"" + text2 + "\" could not be found.";
			}
			int num;
			if (int.TryParse(strings[1], out num))
			{
				DefaultPartySizeLimitModel._addAdditionalPartySizeAsCheat = true;
				PartyBase.MainParty.AddElementToMemberRoster(characterObject2, num, false);
				return string.Concat(new object[] { "Main hero gained ", num, " of ", characterObject2.Name, "." });
			}
			return "Enter a number.";
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("add_random_hero_to_party", "campaign")]
		public static string AddRandomHeroToParty(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"campaign.add_random_hero_to_party [PartyLeaderName]\".";
			}
			Hero hero = Hero.MainHero;
			string text = hero.Name.ToString();
			Hero hero2 = Hero.AllAliveHeroes.GetRandomElement<Hero>();
			if (!CampaignCheats.CheckParameters(strings, 0))
			{
				text = CampaignCheats.ConcatenateString(strings.GetRange(0, strings.Count));
				hero = CampaignCheats.GetHero(text);
			}
			if (hero == null)
			{
				return text + " is not found.";
			}
			if (!hero.IsPartyLeader)
			{
				return text + " is not a party leader.";
			}
			int num = 0;
			while (num < 1000 && (hero2.PartyBelongedTo != null || hero2.PartyBelongedToAsPrisoner != null))
			{
				hero2 = Hero.AllAliveHeroes.GetRandomElement<Hero>();
				num++;
			}
			if (hero2.PartyBelongedTo != null || hero2.PartyBelongedToAsPrisoner != null)
			{
				return "There is not any suitable hero right now.";
			}
			if (hero.Equals(Hero.MainHero))
			{
				DefaultPartySizeLimitModel._addAdditionalPartySizeAsCheat = true;
			}
			hero.PartyBelongedTo.AddElementToMemberRoster(hero2.CharacterObject, 1, false);
			return string.Format("{0} is added to {1}'s party.", hero2.Name, hero.Name);
		}

		[CommandLineFunctionality.CommandLineArgumentFunction("add_prisoner", "campaign")]
		public static string AddPrisoner(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			if (CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"campaign.add_prisoner [PositiveNumber] [TroopName]\".";
			}
			int num = 1;
			string text = "looter";
			CharacterObject characterObject = MBObjectManager.Instance.GetObject<CharacterObject>(text);
			bool flag = false;
			if (CampaignCheats.CheckParameters(strings, 1))
			{
				if (!int.TryParse(strings[0], out num))
				{
					num = 1;
					text = strings[0];
					characterObject = MBObjectManager.Instance.GetObject<CharacterObject>(text);
					flag = true;
				}
			}
			else if (!CampaignCheats.CheckParameters(strings, 0))
			{
				if (!int.TryParse(strings[0], out num))
				{
					num = 1;
					text = CampaignCheats.ConcatenateString(strings.GetRange(0, strings.Count));
					characterObject = MBObjectManager.Instance.GetObject<CharacterObject>(text);
				}
				else
				{
					text = CampaignCheats.ConcatenateString(strings.GetRange(1, strings.Count - 1));
					characterObject = MBObjectManager.Instance.GetObject<CharacterObject>(text);
				}
				flag = true;
			}
			if (characterObject == null)
			{
				foreach (CharacterObject characterObject2 in MBObjectManager.Instance.GetObjectTypeList<CharacterObject>())
				{
					if (characterObject2.Occupation == Occupation.Soldier && string.Equals(characterObject2.Name.ToString(), text, StringComparison.OrdinalIgnoreCase))
					{
						characterObject = characterObject2;
						break;
					}
				}
			}
			if (characterObject != null)
			{
				if (num > 0)
				{
					DefaultPartySizeLimitModel._addAdditionalPrisonerSizeAsCheat = true;
					PartyBase.MainParty.AddPrisoner(characterObject, num);
					return string.Concat(new object[] { "Main hero gained ", num, " of ", characterObject.Name, " as prisoner." });
				}
				return "Please enter a positive number\nFormat is \"campaign.add_prisoner [PositiveNumber] [TroopName]\".";
			}
			else
			{
				if (flag)
				{
					return "Troop: " + text + " not found.\nFormat is \"campaign.add_prisoner [PositiveNumber] [TroopName]\".";
				}
				return "Wrong input.\nFormat is \"campaign.add_prisoner [PositiveNumber] [TroopName]\".";
			}
		}

		private const int BaseMobilePartySize = 20;

		private const int BaseMobilePartyPrisonerSize = 10;

		private const int BaseSettlementPrisonerSize = 60;

		private const int SettlementPrisonerSizeBonusPerWallLevel = 40;

		private const int BaseGarrisonPartySize = 200;

		private const int TownGarrisonSizeBonus = 200;

		private const int AdditionalPartySizeForCheat = 5000;

		private readonly TextObject _leadershipSkillLevelBonusText = GameTexts.FindText("str_leadership_skill_level_bonus", null);

		private readonly TextObject _leadershipPerkUltimateLeaderBonusText = GameTexts.FindText("str_leadership_perk_bonus", null);

		private readonly TextObject _wallLevelBonusText = GameTexts.FindText("str_map_tooltip_wall_level", null);

		private readonly TextObject _baseSizeText = GameTexts.FindText("str_base_size", null);

		private readonly TextObject _clanTierText = GameTexts.FindText("str_clan_tier_bonus", null);

		private readonly TextObject _renownText = GameTexts.FindText("str_renown_bonus", null);

		private readonly TextObject _clanLeaderText = GameTexts.FindText("str_clan_leader_bonus", null);

		private readonly TextObject _factionLeaderText = GameTexts.FindText("str_faction_leader_bonus", null);

		private readonly TextObject _leaderLevelText = GameTexts.FindText("str_leader_level_bonus", null);

		private readonly TextObject _townBonusText = GameTexts.FindText("str_town_bonus", null);

		private readonly TextObject _minorFactionText = GameTexts.FindText("str_minor_faction_bonus", null);

		private readonly TextObject _currentPartySizeBonusText = GameTexts.FindText("str_current_party_size_bonus", null);

		private readonly TextObject _randomSizeBonusTemporary = new TextObject("{=hynFV8jC}Extra size bonus (Perk-like Effect)", null);

		private static bool _addAdditionalPartySizeAsCheat;

		private static bool _addAdditionalPrisonerSizeAsCheat;

		private TextObject _quarterMasterText;

		private enum LimitType
		{
			MobilePartySizeLimit,
			GarrisonPartySizeLimit,
			PrisonerSizeLimit
		}
	}
}
