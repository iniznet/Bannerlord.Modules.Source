using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace Helpers
{
	public static class SkillHelper
	{
		public static void AddSkillBonusForParty(SkillObject skill, SkillEffect skillEffect, MobileParty party, ref ExplainedNumber stat)
		{
			Hero leaderHero = party.LeaderHero;
			if (leaderHero != null && skillEffect != null)
			{
				if (skillEffect.PrimaryRole == SkillEffect.PerkRole.PartyLeader || skillEffect.SecondaryRole == SkillEffect.PerkRole.PartyLeader)
				{
					int skillValue = leaderHero.GetSkillValue(skill);
					float num = ((skillEffect.PrimaryRole == SkillEffect.PerkRole.PartyLeader) ? skillEffect.PrimaryBonus : skillEffect.SecondaryBonus);
					num *= ((leaderHero.Clan != Clan.PlayerClan) ? 1.8f : 1f);
					SkillHelper.AddToStat(ref stat, skillEffect.IncrementType, num * (float)skillValue, SkillHelper._textLeader);
				}
				if (skillEffect.PrimaryRole == SkillEffect.PerkRole.Engineer || skillEffect.SecondaryRole == SkillEffect.PerkRole.Engineer)
				{
					Hero effectiveEngineer = party.EffectiveEngineer;
					if (effectiveEngineer != null)
					{
						int skillValue2 = effectiveEngineer.GetSkillValue(skill);
						float num2 = ((skillEffect.PrimaryRole == SkillEffect.PerkRole.Engineer) ? skillEffect.PrimaryBonus : skillEffect.SecondaryBonus);
						SkillHelper.AddToStat(ref stat, skillEffect.IncrementType, num2 * (float)skillValue2, SkillHelper._textEngineer);
					}
				}
				if (skillEffect.PrimaryRole == SkillEffect.PerkRole.Scout || skillEffect.SecondaryRole == SkillEffect.PerkRole.Scout)
				{
					Hero effectiveScout = party.EffectiveScout;
					if (effectiveScout != null)
					{
						int skillValue3 = effectiveScout.GetSkillValue(skill);
						float num3 = ((skillEffect.PrimaryRole == SkillEffect.PerkRole.Scout) ? skillEffect.PrimaryBonus : skillEffect.SecondaryBonus);
						SkillHelper.AddToStat(ref stat, skillEffect.IncrementType, num3 * (float)skillValue3, SkillHelper._textScout);
					}
				}
				if (skillEffect.PrimaryRole == SkillEffect.PerkRole.Surgeon || skillEffect.SecondaryRole == SkillEffect.PerkRole.Surgeon)
				{
					Hero effectiveSurgeon = party.EffectiveSurgeon;
					if (effectiveSurgeon != null)
					{
						int skillValue4 = effectiveSurgeon.GetSkillValue(skill);
						float num4 = ((skillEffect.PrimaryRole == SkillEffect.PerkRole.Surgeon) ? skillEffect.PrimaryBonus : skillEffect.SecondaryBonus);
						SkillHelper.AddToStat(ref stat, skillEffect.IncrementType, num4 * (float)skillValue4, SkillHelper._textSurgeon);
					}
				}
				if (skillEffect.PrimaryRole == SkillEffect.PerkRole.Quartermaster || skillEffect.SecondaryRole == SkillEffect.PerkRole.Quartermaster)
				{
					Hero effectiveQuartermaster = party.EffectiveQuartermaster;
					if (effectiveQuartermaster != null)
					{
						int skillValue5 = effectiveQuartermaster.GetSkillValue(skill);
						float num5 = ((skillEffect.PrimaryRole == SkillEffect.PerkRole.Quartermaster) ? skillEffect.PrimaryBonus : skillEffect.SecondaryBonus);
						SkillHelper.AddToStat(ref stat, skillEffect.IncrementType, num5 * (float)skillValue5, SkillHelper._textQuartermaster);
					}
				}
			}
		}

		public static void AddSkillBonusForTown(SkillObject skill, SkillEffect skillEffect, Town town, ref ExplainedNumber bonuses)
		{
			if (skillEffect.PrimaryRole == SkillEffect.PerkRole.ClanLeader || skillEffect.SecondaryRole == SkillEffect.PerkRole.ClanLeader)
			{
				Clan ownerClan = town.Owner.Settlement.OwnerClan;
				Hero hero = ((ownerClan != null) ? ownerClan.Leader : null);
				if (hero != null)
				{
					int skillValue = hero.GetSkillValue(skill);
					float num = ((skillEffect.PrimaryRole == SkillEffect.PerkRole.ClanLeader) ? skillEffect.PrimaryBonus : skillEffect.SecondaryBonus);
					SkillHelper.AddToStat(ref bonuses, skillEffect.IncrementType, num * (float)skillValue, SkillHelper._textClanLeader);
				}
			}
			if (skillEffect.PrimaryRole == SkillEffect.PerkRole.Governor || skillEffect.SecondaryRole == SkillEffect.PerkRole.Governor)
			{
				Hero governor = town.Governor;
				if (governor != null && governor.CurrentSettlement == town.Settlement)
				{
					int skillValue2 = governor.GetSkillValue(skill);
					float num2 = ((skillEffect.PrimaryRole == SkillEffect.PerkRole.Governor) ? skillEffect.PrimaryBonus : skillEffect.SecondaryBonus);
					SkillHelper.AddToStat(ref bonuses, skillEffect.IncrementType, num2 * (float)skillValue2, SkillHelper._textGovernor);
				}
			}
		}

		public static void AddSkillBonusForCharacter(SkillObject skill, SkillEffect skillEffect, CharacterObject character, ref ExplainedNumber stat, int baseSkillOverride = -1, bool isBonusPositive = true, int extraSkillValue = 0)
		{
			int num = ((baseSkillOverride >= 0) ? baseSkillOverride : character.GetSkillValue(skill)) + extraSkillValue;
			int num2 = (isBonusPositive ? 1 : (-1));
			if (skillEffect.PrimaryRole == SkillEffect.PerkRole.Personal || skillEffect.SecondaryRole == SkillEffect.PerkRole.Personal)
			{
				float num3 = ((skillEffect.PrimaryRole == SkillEffect.PerkRole.Personal) ? skillEffect.GetPrimaryValue(num) : skillEffect.GetSecondaryValue(num));
				SkillHelper.AddToStat(ref stat, skillEffect.IncrementType, (float)num2 * num3, SkillHelper._textPersonal);
			}
			Hero heroObject = character.HeroObject;
			if (heroObject == null)
			{
				return;
			}
			if ((skillEffect.PrimaryRole == SkillEffect.PerkRole.Engineer || skillEffect.SecondaryRole == SkillEffect.PerkRole.Engineer) && character.IsHero)
			{
				MobileParty partyBelongedTo = heroObject.PartyBelongedTo;
				if (((partyBelongedTo != null) ? partyBelongedTo.EffectiveEngineer : null) == heroObject)
				{
					float num4 = ((skillEffect.PrimaryRole == SkillEffect.PerkRole.Engineer) ? skillEffect.GetPrimaryValue(num) : skillEffect.GetSecondaryValue(num));
					SkillHelper.AddToStat(ref stat, skillEffect.IncrementType, (float)num2 * num4, SkillHelper._textEngineer);
				}
			}
			if ((skillEffect.PrimaryRole == SkillEffect.PerkRole.Quartermaster || skillEffect.SecondaryRole == SkillEffect.PerkRole.Quartermaster) && character.IsHero)
			{
				MobileParty partyBelongedTo2 = heroObject.PartyBelongedTo;
				if (((partyBelongedTo2 != null) ? partyBelongedTo2.EffectiveQuartermaster : null) == heroObject)
				{
					float num5 = ((skillEffect.PrimaryRole == SkillEffect.PerkRole.Quartermaster) ? skillEffect.GetPrimaryValue(num) : skillEffect.GetSecondaryValue(num));
					SkillHelper.AddToStat(ref stat, skillEffect.IncrementType, (float)num2 * num5, SkillHelper._textQuartermaster);
				}
			}
			if ((skillEffect.PrimaryRole == SkillEffect.PerkRole.Scout || skillEffect.SecondaryRole == SkillEffect.PerkRole.Scout) && character.IsHero)
			{
				MobileParty partyBelongedTo3 = heroObject.PartyBelongedTo;
				if (((partyBelongedTo3 != null) ? partyBelongedTo3.EffectiveScout : null) == heroObject)
				{
					float num6 = ((skillEffect.PrimaryRole == SkillEffect.PerkRole.Scout) ? skillEffect.GetPrimaryValue(num) : skillEffect.GetSecondaryValue(num));
					SkillHelper.AddToStat(ref stat, skillEffect.IncrementType, (float)num2 * num6, SkillHelper._textScout);
				}
			}
			if ((skillEffect.PrimaryRole == SkillEffect.PerkRole.Surgeon || skillEffect.SecondaryRole == SkillEffect.PerkRole.Surgeon) && character.IsHero)
			{
				MobileParty partyBelongedTo4 = heroObject.PartyBelongedTo;
				if (((partyBelongedTo4 != null) ? partyBelongedTo4.EffectiveSurgeon : null) == heroObject)
				{
					float num7 = ((skillEffect.PrimaryRole == SkillEffect.PerkRole.Surgeon) ? skillEffect.GetPrimaryValue(num) : skillEffect.GetSecondaryValue(num));
					SkillHelper.AddToStat(ref stat, skillEffect.IncrementType, (float)num2 * num7, SkillHelper._textSurgeon);
				}
			}
			if ((skillEffect.PrimaryRole == SkillEffect.PerkRole.PartyLeader || skillEffect.SecondaryRole == SkillEffect.PerkRole.PartyLeader) && character.IsHero)
			{
				MobileParty partyBelongedTo5 = heroObject.PartyBelongedTo;
				if (((partyBelongedTo5 != null) ? partyBelongedTo5.LeaderHero : null) == heroObject)
				{
					float num8 = ((skillEffect.PrimaryRole == SkillEffect.PerkRole.PartyLeader) ? skillEffect.GetPrimaryValue(num) : skillEffect.GetSecondaryValue(num));
					SkillHelper.AddToStat(ref stat, skillEffect.IncrementType, (float)num2 * num8, SkillHelper._textPartyLeader);
				}
			}
		}

		public static string GetEffectDescriptionForSkillLevel(SkillEffect effect, int level)
		{
			MBTextManager.SetTextVariable("a0", effect.GetPrimaryValue(level).ToString("0.0"), false);
			MBTextManager.SetTextVariable("a1", effect.GetSecondaryValue(level).ToString("0.0"), false);
			return effect.Description.ToString();
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
				stat.AddFactor(number * 0.01f, text);
			}
		}

		public static CharacterObject GetEffectivePartyLeaderForSkill(PartyBase party)
		{
			if (party == null)
			{
				return null;
			}
			if (party.LeaderHero != null)
			{
				return party.LeaderHero.CharacterObject;
			}
			TroopRoster memberRoster = party.MemberRoster;
			if (memberRoster == null || memberRoster.TotalManCount <= 0)
			{
				return null;
			}
			return party.MemberRoster.GetCharacterAtIndex(0);
		}

		private static readonly TextObject _textLeader = new TextObject("{=SrfYbg3x}Leader", null);

		private static readonly TextObject _textPersonal = new TextObject("{=UxAl9iyi}Personal", null);

		private static readonly TextObject _textScout = new TextObject("{=92M0Pb5T}Scout", null);

		private static readonly TextObject _textQuartermaster = new TextObject("{=redwEIlW}Quartermaster", null);

		private static readonly TextObject _textEngineer = new TextObject("{=7h6cXdW7}Engineer", null);

		private static readonly TextObject _textPartyLeader = new TextObject("{=ggpRTQQl}Party Leader", null);

		private static readonly TextObject _textSurgeon = new TextObject("{=QBPrRdQJ}Surgeon", null);

		private static readonly TextObject _textSergeant = new TextObject("{=g9VIbA9s}Sergeant", null);

		private static readonly TextObject _textGovernor = new TextObject("{=Fa2nKXxI}Governor", null);

		private static readonly TextObject _textClanLeader = new TextObject("{=pqfz386V}Clan Leader", null);
	}
}
