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
	// Token: 0x02000017 RID: 23
	public static class PerkHelper
	{
		// Token: 0x060000D4 RID: 212 RVA: 0x0000B03C File Offset: 0x0000923C
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

		// Token: 0x060000D5 RID: 213 RVA: 0x0000B0B0 File Offset: 0x000092B0
		public static bool PlayerHasAnyItemDonationPerk()
		{
			return MobileParty.MainParty.HasPerk(DefaultPerks.Steward.GivingHands, false) || MobileParty.MainParty.HasPerk(DefaultPerks.Steward.PaidInPromise, true);
		}

		// Token: 0x060000D6 RID: 214 RVA: 0x0000B0D8 File Offset: 0x000092D8
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

		// Token: 0x060000D7 RID: 215 RVA: 0x0000B4C0 File Offset: 0x000096C0
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

		// Token: 0x060000D8 RID: 216 RVA: 0x0000B4DC File Offset: 0x000096DC
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

		// Token: 0x060000D9 RID: 217 RVA: 0x0000B600 File Offset: 0x00009800
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

		// Token: 0x060000DA RID: 218 RVA: 0x0000B664 File Offset: 0x00009864
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

		// Token: 0x060000DB RID: 219 RVA: 0x0000B6CC File Offset: 0x000098CC
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

		// Token: 0x060000DC RID: 220 RVA: 0x0000B750 File Offset: 0x00009950
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

		// Token: 0x060000DD RID: 221 RVA: 0x0000B7D8 File Offset: 0x000099D8
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

		// Token: 0x060000DE RID: 222 RVA: 0x0000B84C File Offset: 0x00009A4C
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

		// Token: 0x0400000E RID: 14
		private static readonly TextObject _textLeader = new TextObject("{=fghweLqW}Leader Perks", null);

		// Token: 0x0400000F RID: 15
		private static readonly TextObject _textScout = new TextObject("{=3UDUqYv1}Scout Perks", null);

		// Token: 0x04000010 RID: 16
		private static readonly TextObject _textQuartermaster = new TextObject("{=AvaZUvnu}Quartermaster Perks", null);

		// Token: 0x04000011 RID: 17
		private static readonly TextObject _textEngineer = new TextObject("{=Gg5dJHjC}Engineer Perks", null);

		// Token: 0x04000012 RID: 18
		private static readonly TextObject _textSurgeon = new TextObject("{=aG7KXlu0}Surgeon Perks", null);

		// Token: 0x04000013 RID: 19
		private static readonly TextObject _textSergeant = new TextObject("{=TxbjTbZf}Sergeant Perks", null);

		// Token: 0x04000014 RID: 20
		private static readonly TextObject _textGovernor = new TextObject("{=Fa2nKXxI}Governor", null);

		// Token: 0x04000015 RID: 21
		private static readonly TextObject _textClanLeader = new TextObject("{=pqfz386V}Clan Leader", null);

		// Token: 0x04000016 RID: 22
		private static readonly TextObject _textPerkBonuses = new TextObject("{=Avy8Gua1}Perks", null);

		// Token: 0x04000017 RID: 23
		private static readonly TextObject _textFeatBonuses = new TextObject("{=snSBfQkV}Feats", null);

		// Token: 0x04000018 RID: 24
		private static readonly TextObject _textMember = new TextObject("{=7rxJWqby}Party members", null);
	}
}
