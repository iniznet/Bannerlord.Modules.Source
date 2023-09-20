using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Helpers
{
	public static class PerkHelper
	{
		public static IEnumerable<PerkObject> GetCaptainPerksForTroopClasses(TroopClassFlag troopClassFlags)
		{
			List<PerkObject> list = new List<PerkObject>();
			foreach (PerkObject perkObject in PerkObject.All)
			{
				TroopClassFlag troopClassFlag = perkObject.PrimaryTroopClassMask | perkObject.SecondaryTroopClassMask;
				if (troopClassFlag != TroopClassFlag.None && troopClassFlags.HasAllFlags(troopClassFlag))
				{
					list.Add(perkObject);
				}
			}
			return list;
		}

		public static bool PlayerHasAnyItemDonationPerk()
		{
			return MobileParty.MainParty.HasPerk(DefaultPerks.Steward.GivingHands, false) || MobileParty.MainParty.HasPerk(DefaultPerks.Steward.PaidInPromise, true);
		}

		public static void AddPerkBonusForParty(PerkObject perk, MobileParty party, bool isPrimaryBonus, ref ExplainedNumber stat)
		{
			Hero hero = ((party != null) ? party.LeaderHero : null);
			if (hero != null)
			{
				bool flag = isPrimaryBonus && perk.PrimaryRole == SkillEffect.PerkRole.PartyLeader;
				bool flag2 = !isPrimaryBonus && perk.SecondaryRole == SkillEffect.PerkRole.PartyLeader;
				if ((flag || flag2) && hero.GetPerkValue(perk))
				{
					float num = (flag ? perk.PrimaryBonus : perk.SecondaryBonus);
					if (hero.Clan != Clan.PlayerClan)
					{
						num *= 1.8f;
					}
					if (flag)
					{
						PerkHelper.AddToStat(ref stat, perk.PrimaryIncrementType, num, PerkHelper._textLeader);
					}
					else
					{
						PerkHelper.AddToStat(ref stat, perk.SecondaryIncrementType, num, PerkHelper._textLeader);
					}
				}
				flag = isPrimaryBonus && perk.PrimaryRole == SkillEffect.PerkRole.ClanLeader;
				flag2 = !isPrimaryBonus && perk.SecondaryRole == SkillEffect.PerkRole.ClanLeader;
				if ((flag || flag2) && hero.Clan.Leader != null && hero.Clan.Leader.GetPerkValue(perk))
				{
					if (flag)
					{
						PerkHelper.AddToStat(ref stat, perk.PrimaryIncrementType, perk.PrimaryBonus, PerkHelper._textLeader);
					}
					else
					{
						PerkHelper.AddToStat(ref stat, perk.SecondaryIncrementType, perk.SecondaryBonus, PerkHelper._textLeader);
					}
				}
				flag = isPrimaryBonus && perk.PrimaryRole == SkillEffect.PerkRole.PartyMember;
				flag2 = !isPrimaryBonus && perk.SecondaryRole == SkillEffect.PerkRole.PartyMember;
				if (flag || flag2)
				{
					if (hero.Clan != Clan.PlayerClan)
					{
						if (hero.GetPerkValue(perk))
						{
							PerkHelper.AddToStat(ref stat, flag ? perk.PrimaryIncrementType : perk.SecondaryIncrementType, flag ? perk.PrimaryBonus : perk.SecondaryBonus, PerkHelper._textMember);
						}
					}
					else
					{
						foreach (TroopRosterElement troopRosterElement in party.MemberRoster.GetTroopRoster())
						{
							if (troopRosterElement.Character.IsHero && troopRosterElement.Character.GetPerkValue(perk))
							{
								PerkHelper.AddToStat(ref stat, flag ? perk.PrimaryIncrementType : perk.SecondaryIncrementType, flag ? perk.PrimaryBonus : perk.SecondaryBonus, PerkHelper._textMember);
							}
						}
					}
				}
				if (hero.Clan == Clan.PlayerClan)
				{
					flag = isPrimaryBonus && perk.PrimaryRole == SkillEffect.PerkRole.Engineer;
					flag2 = !isPrimaryBonus && perk.SecondaryRole == SkillEffect.PerkRole.Engineer;
					if (flag || flag2)
					{
						Hero effectiveEngineer = party.EffectiveEngineer;
						if (effectiveEngineer != null && effectiveEngineer.GetPerkValue(perk))
						{
							if (flag)
							{
								PerkHelper.AddToStat(ref stat, perk.PrimaryIncrementType, perk.PrimaryBonus, PerkHelper._textEngineer);
							}
							else
							{
								PerkHelper.AddToStat(ref stat, perk.SecondaryIncrementType, perk.SecondaryBonus, PerkHelper._textEngineer);
							}
						}
					}
					flag = isPrimaryBonus && perk.PrimaryRole == SkillEffect.PerkRole.Scout;
					flag2 = !isPrimaryBonus && perk.SecondaryRole == SkillEffect.PerkRole.Scout;
					if (flag || flag2)
					{
						Hero effectiveScout = party.EffectiveScout;
						if (effectiveScout != null && effectiveScout.GetPerkValue(perk))
						{
							if (flag)
							{
								PerkHelper.AddToStat(ref stat, perk.PrimaryIncrementType, perk.PrimaryBonus, PerkHelper._textScout);
							}
							else
							{
								PerkHelper.AddToStat(ref stat, perk.SecondaryIncrementType, perk.SecondaryBonus, PerkHelper._textScout);
							}
						}
					}
					flag = isPrimaryBonus && perk.PrimaryRole == SkillEffect.PerkRole.Surgeon;
					flag2 = !isPrimaryBonus && perk.SecondaryRole == SkillEffect.PerkRole.Surgeon;
					if (flag || flag2)
					{
						Hero effectiveSurgeon = party.EffectiveSurgeon;
						if (effectiveSurgeon != null && effectiveSurgeon.GetPerkValue(perk))
						{
							if (flag)
							{
								PerkHelper.AddToStat(ref stat, perk.PrimaryIncrementType, perk.PrimaryBonus, PerkHelper._textSurgeon);
							}
							else
							{
								PerkHelper.AddToStat(ref stat, perk.SecondaryIncrementType, perk.SecondaryBonus, PerkHelper._textSurgeon);
							}
						}
					}
					flag = isPrimaryBonus && perk.PrimaryRole == SkillEffect.PerkRole.Quartermaster;
					flag2 = !isPrimaryBonus && perk.SecondaryRole == SkillEffect.PerkRole.Quartermaster;
					if (flag || flag2)
					{
						Hero effectiveQuartermaster = party.EffectiveQuartermaster;
						if (effectiveQuartermaster != null && effectiveQuartermaster.GetPerkValue(perk))
						{
							if (flag)
							{
								PerkHelper.AddToStat(ref stat, perk.PrimaryIncrementType, perk.PrimaryBonus, PerkHelper._textQuartermaster);
								return;
							}
							PerkHelper.AddToStat(ref stat, perk.SecondaryIncrementType, perk.SecondaryBonus, PerkHelper._textQuartermaster);
						}
					}
				}
			}
		}

		private static void AddToStat(ref ExplainedNumber stat, SkillEffect.EffectIncrementType effectIncrementType, float number, TextObject text)
		{
			if (effectIncrementType == SkillEffect.EffectIncrementType.Add)
			{
				stat.Add(number, text, null);
				return;
			}
			if (effectIncrementType == SkillEffect.EffectIncrementType.AddFactor)
			{
				stat.AddFactor(number, text);
			}
		}

		public static void AddPerkBonusForCharacter(PerkObject perk, CharacterObject character, bool isPrimaryBonus, ref ExplainedNumber bonuses)
		{
			if (isPrimaryBonus && perk.PrimaryRole == SkillEffect.PerkRole.Personal)
			{
				if (character.GetPerkValue(perk))
				{
					PerkHelper.AddToStat(ref bonuses, perk.PrimaryIncrementType, perk.PrimaryBonus, PerkHelper._textPerkBonuses);
				}
			}
			else if (!isPrimaryBonus && perk.SecondaryRole == SkillEffect.PerkRole.Personal && character.GetPerkValue(perk))
			{
				PerkHelper.AddToStat(ref bonuses, perk.SecondaryIncrementType, perk.SecondaryBonus, PerkHelper._textPerkBonuses);
			}
			if (isPrimaryBonus && perk.PrimaryRole == SkillEffect.PerkRole.ClanLeader)
			{
				if (character.IsHero)
				{
					Clan clan = character.HeroObject.Clan;
					if (((clan != null) ? clan.Leader : null) != null && character.HeroObject.Clan.Leader.GetPerkValue(perk))
					{
						PerkHelper.AddToStat(ref bonuses, perk.PrimaryIncrementType, perk.PrimaryBonus, PerkHelper._textPerkBonuses);
						return;
					}
				}
			}
			else if (!isPrimaryBonus && perk.SecondaryRole == SkillEffect.PerkRole.ClanLeader && character.IsHero && character.HeroObject.Clan.Leader != null && character.HeroObject.Clan.Leader.GetPerkValue(perk))
			{
				PerkHelper.AddToStat(ref bonuses, perk.SecondaryIncrementType, perk.SecondaryBonus, PerkHelper._textPerkBonuses);
			}
		}

		public static void AddEpicPerkBonusForCharacter(PerkObject perk, CharacterObject character, SkillObject skillType, bool applyPrimaryBonus, ref ExplainedNumber bonuses, int skillRequired = 250)
		{
			if (character.GetPerkValue(perk))
			{
				int skillValue = character.GetSkillValue(skillType);
				if (skillValue > skillRequired)
				{
					if (applyPrimaryBonus)
					{
						PerkHelper.AddToStat(ref bonuses, perk.PrimaryIncrementType, perk.PrimaryBonus * (float)(skillValue - skillRequired), PerkHelper._textPerkBonuses);
						return;
					}
					PerkHelper.AddToStat(ref bonuses, perk.SecondaryIncrementType, perk.SecondaryBonus * (float)(skillValue - skillRequired), PerkHelper._textPerkBonuses);
				}
			}
		}

		public static void AddPerkBonusFromCaptain(PerkObject perk, CharacterObject captainCharacter, ref ExplainedNumber bonuses)
		{
			if (perk.PrimaryRole == SkillEffect.PerkRole.Captain)
			{
				if (captainCharacter != null && captainCharacter.GetPerkValue(perk))
				{
					PerkHelper.AddToStat(ref bonuses, perk.PrimaryIncrementType, perk.PrimaryBonus, PerkHelper._textPerkBonuses);
					return;
				}
			}
			else if (perk.SecondaryRole == SkillEffect.PerkRole.Captain && captainCharacter != null && captainCharacter.GetPerkValue(perk))
			{
				PerkHelper.AddToStat(ref bonuses, perk.SecondaryIncrementType, perk.SecondaryBonus, PerkHelper._textPerkBonuses);
			}
		}

		public static void AddPerkBonusForTown(PerkObject perk, Town town, ref ExplainedNumber bonuses)
		{
			bool flag = perk.PrimaryRole == SkillEffect.PerkRole.Governor;
			bool flag2 = perk.SecondaryRole == SkillEffect.PerkRole.Governor;
			if (flag || flag2)
			{
				Hero governor = town.Governor;
				if (governor != null && governor.GetPerkValue(perk) && governor.CurrentSettlement != null && governor.CurrentSettlement == town.Settlement)
				{
					if (flag)
					{
						PerkHelper.AddToStat(ref bonuses, perk.PrimaryIncrementType, perk.PrimaryBonus, perk.Name);
						return;
					}
					PerkHelper.AddToStat(ref bonuses, perk.SecondaryIncrementType, perk.SecondaryBonus, perk.Name);
				}
			}
		}

		public static bool GetPerkValueForTown(PerkObject perk, Town town)
		{
			if (perk.PrimaryRole == SkillEffect.PerkRole.ClanLeader || perk.SecondaryRole == SkillEffect.PerkRole.ClanLeader)
			{
				Clan ownerClan = town.Owner.Settlement.OwnerClan;
				Hero hero = ((ownerClan != null) ? ownerClan.Leader : null);
				if (hero != null && hero.GetPerkValue(perk))
				{
					return true;
				}
			}
			if (perk.PrimaryRole == SkillEffect.PerkRole.Governor || perk.SecondaryRole == SkillEffect.PerkRole.Governor)
			{
				Hero governor = town.Governor;
				if (governor != null && governor.GetPerkValue(perk) && governor.CurrentSettlement != null && governor.CurrentSettlement == town.Settlement)
				{
					return true;
				}
			}
			return false;
		}

		public static List<PerkObject> GetGovernorPerksForHero(Hero hero)
		{
			List<PerkObject> list = new List<PerkObject>();
			foreach (PerkObject perkObject in PerkObject.All)
			{
				if ((perkObject.PrimaryRole == SkillEffect.PerkRole.Governor || perkObject.SecondaryRole == SkillEffect.PerkRole.Governor) && hero.GetPerkValue(perkObject))
				{
					list.Add(perkObject);
				}
			}
			return list;
		}

		public static ValueTuple<TextObject, TextObject> GetGovernorEngineeringSkillEffectForHero(Hero governor)
		{
			if (governor != null && governor.GetSkillValue(DefaultSkills.Engineering) > 0)
			{
				SkillEffect townProjectBuildingBonus = DefaultSkillEffects.TownProjectBuildingBonus;
				TextObject description = townProjectBuildingBonus.Description;
				float num = ((townProjectBuildingBonus.PrimaryRole == SkillEffect.PerkRole.Governor) ? townProjectBuildingBonus.PrimaryBonus : townProjectBuildingBonus.SecondaryBonus);
				description.SetTextVariable("a0", (float)governor.GetSkillValue(DefaultSkills.Engineering) * num);
				return new ValueTuple<TextObject, TextObject>(DefaultSkills.Engineering.Name, description);
			}
			return new ValueTuple<TextObject, TextObject>(TextObject.Empty, new TextObject("{=0rBsbw1T}No effect", null));
		}

		private static readonly TextObject _textLeader = new TextObject("{=fghweLqW}Leader Perks", null);

		private static readonly TextObject _textScout = new TextObject("{=3UDUqYv1}Scout Perks", null);

		private static readonly TextObject _textQuartermaster = new TextObject("{=AvaZUvnu}Quartermaster Perks", null);

		private static readonly TextObject _textEngineer = new TextObject("{=Gg5dJHjC}Engineer Perks", null);

		private static readonly TextObject _textSurgeon = new TextObject("{=aG7KXlu0}Surgeon Perks", null);

		private static readonly TextObject _textSergeant = new TextObject("{=TxbjTbZf}Sergeant Perks", null);

		private static readonly TextObject _textGovernor = new TextObject("{=Fa2nKXxI}Governor", null);

		private static readonly TextObject _textClanLeader = new TextObject("{=pqfz386V}Clan Leader", null);

		private static readonly TextObject _textPerkBonuses = new TextObject("{=Avy8Gua1}Perks", null);

		private static readonly TextObject _textFeatBonuses = new TextObject("{=snSBfQkV}Feats", null);

		private static readonly TextObject _textMember = new TextObject("{=7rxJWqby}Party members", null);
	}
}
