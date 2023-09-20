using System;
using Helpers;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	// Token: 0x02000122 RID: 290
	public class DefaultPartyHealingModel : PartyHealingModel
	{
		// Token: 0x06001657 RID: 5719 RVA: 0x0006B010 File Offset: 0x00069210
		public override float GetSurgeryChance(PartyBase party, CharacterObject character)
		{
			MobileParty mobileParty = party.MobileParty;
			int? num;
			if (mobileParty == null)
			{
				num = null;
			}
			else
			{
				Hero effectiveSurgeon = mobileParty.EffectiveSurgeon;
				num = ((effectiveSurgeon != null) ? new int?(effectiveSurgeon.GetSkillValue(DefaultSkills.Medicine)) : null);
			}
			int num2 = num ?? 0;
			return 0.0015f * (float)num2;
		}

		// Token: 0x06001658 RID: 5720 RVA: 0x0006B074 File Offset: 0x00069274
		public override float GetSiegeBombardmentHitSurgeryChance(PartyBase party)
		{
			float num = 0f;
			if (party != null && party.IsMobile && party.MobileParty.HasPerk(DefaultPerks.Medicine.SiegeMedic, false))
			{
				num += DefaultPerks.Medicine.SiegeMedic.PrimaryBonus;
			}
			return num;
		}

		// Token: 0x06001659 RID: 5721 RVA: 0x0006B0B4 File Offset: 0x000692B4
		public override float GetSurvivalChance(PartyBase party, CharacterObject character, DamageTypes damageType, PartyBase enemyParty = null)
		{
			if (damageType == DamageTypes.Blunt || (character.IsHero && CampaignOptions.BattleDeath == CampaignOptions.Difficulty.VeryEasy) || (character.IsPlayerCharacter && CampaignOptions.BattleDeath == CampaignOptions.Difficulty.Easy))
			{
				return 1f;
			}
			ExplainedNumber explainedNumber = new ExplainedNumber(1f, false, null);
			float num;
			if (((party != null) ? party.MobileParty : null) != null)
			{
				MobileParty mobileParty = party.MobileParty;
				SkillHelper.AddSkillBonusForParty(DefaultSkills.Medicine, DefaultSkillEffects.SurgeonSurvivalBonus, mobileParty, ref explainedNumber);
				if (((enemyParty != null) ? enemyParty.MobileParty : null) != null && enemyParty.MobileParty.HasPerk(DefaultPerks.Medicine.DoctorsOath, false))
				{
					SkillHelper.AddSkillBonusForParty(DefaultSkills.Medicine, DefaultSkillEffects.SurgeonSurvivalBonus, enemyParty.MobileParty, ref explainedNumber);
					SkillLevelingManager.OnSurgeryApplied(enemyParty.MobileParty, false, character.Tier);
				}
				explainedNumber.Add((float)character.Level * 0.02f, null, null);
				if (!character.IsHero && party.MapEvent != null && character.Tier < 3)
				{
					PerkHelper.AddPerkBonusForParty(DefaultPerks.Medicine.PhysicianOfPeople, party.MobileParty, false, ref explainedNumber);
				}
				if (character.IsHero)
				{
					explainedNumber.Add(character.GetTotalArmorSum(false) * 0.01f, null, null);
					explainedNumber.Add(character.Age * -0.01f, null, null);
					explainedNumber.AddFactor(50f, null);
				}
				ExplainedNumber explainedNumber2 = new ExplainedNumber(1f / explainedNumber.ResultNumber, false, null);
				if (character.IsHero)
				{
					if (party.IsMobile && party.MobileParty.HasPerk(DefaultPerks.Medicine.CheatDeath, true))
					{
						explainedNumber2.AddFactor(DefaultPerks.Medicine.CheatDeath.SecondaryBonus, DefaultPerks.Medicine.CheatDeath.Name);
					}
					if (character.HeroObject.Clan == Clan.PlayerClan)
					{
						float clanMemberDeathChanceMultiplier = Campaign.Current.Models.DifficultyModel.GetClanMemberDeathChanceMultiplier();
						if (!clanMemberDeathChanceMultiplier.ApproximatelyEqualsTo(0f, 1E-05f))
						{
							explainedNumber2.AddFactor(clanMemberDeathChanceMultiplier, GameTexts.FindText("str_game_difficulty", null));
						}
					}
				}
				num = 1f - MBMath.ClampFloat(explainedNumber2.ResultNumber, 0f, 1f);
			}
			else if (explainedNumber.ResultNumber.ApproximatelyEqualsTo(0f, 1E-05f))
			{
				num = 0f;
			}
			else
			{
				num = 1f - 1f / explainedNumber.ResultNumber;
			}
			return num;
		}

		// Token: 0x0600165A RID: 5722 RVA: 0x0006B2ED File Offset: 0x000694ED
		public override int GetSkillXpFromHealingTroop(PartyBase party)
		{
			return 5;
		}

		// Token: 0x0600165B RID: 5723 RVA: 0x0006B2F0 File Offset: 0x000694F0
		public override ExplainedNumber GetDailyHealingForRegulars(MobileParty party, bool includeDescriptions = false)
		{
			ExplainedNumber explainedNumber = new ExplainedNumber(0f, includeDescriptions, null);
			if (party.Party.IsStarving || (party.IsGarrison && party.CurrentSettlement.IsStarving))
			{
				if (party.IsGarrison)
				{
					if (SettlementHelper.IsGarrisonStarving(party.CurrentSettlement))
					{
						int num = MBRandom.RoundRandomized((float)party.MemberRoster.TotalRegulars * 0.1f);
						explainedNumber.Add((float)(-(float)num), DefaultPartyHealingModel._starvingText, null);
					}
				}
				else
				{
					int totalRegulars = party.MemberRoster.TotalRegulars;
					explainedNumber.Add((float)(-(float)totalRegulars) * 0.25f, DefaultPartyHealingModel._starvingText, null);
				}
			}
			else
			{
				explainedNumber = new ExplainedNumber(5f, includeDescriptions, null);
				if (party.IsGarrison)
				{
					if (party.CurrentSettlement.IsTown)
					{
						SkillHelper.AddSkillBonusForTown(DefaultSkills.Medicine, DefaultSkillEffects.GovernorHealingRateBonus, party.CurrentSettlement.Town, ref explainedNumber);
					}
				}
				else
				{
					SkillHelper.AddSkillBonusForParty(DefaultSkills.Medicine, DefaultSkillEffects.HealingRateBonusForRegulars, party, ref explainedNumber);
				}
				if (!party.IsGarrison && !party.IsMilitia)
				{
					if (!party.IsMoving)
					{
						PerkHelper.AddPerkBonusForParty(DefaultPerks.Medicine.TriageTent, party, true, ref explainedNumber);
						PerkHelper.AddPerkBonusForParty(DefaultPerks.Athletics.AGoodDaysRest, party, true, ref explainedNumber);
					}
					else
					{
						PerkHelper.AddPerkBonusForParty(DefaultPerks.Medicine.WalkItOff, party, true, ref explainedNumber);
						PerkHelper.AddPerkBonusForParty(DefaultPerks.Athletics.WalkItOff, party, true, ref explainedNumber);
					}
				}
				if (party.Morale >= Campaign.Current.Models.PartyMoraleModel.HighMoraleValue)
				{
					PerkHelper.AddPerkBonusForParty(DefaultPerks.Medicine.BestMedicine, party, true, ref explainedNumber);
				}
				if (party.CurrentSettlement != null && !party.CurrentSettlement.IsHideout)
				{
					if (party.CurrentSettlement.IsFortification)
					{
						explainedNumber.Add(10f, DefaultPartyHealingModel._settlementText, null);
					}
					if (party.SiegeEvent == null && !party.CurrentSettlement.IsUnderSiege && !party.CurrentSettlement.IsRaided && !party.CurrentSettlement.IsUnderRaid)
					{
						if (party.CurrentSettlement.IsTown)
						{
							PerkHelper.AddPerkBonusForParty(DefaultPerks.Medicine.PristineStreets, party, false, ref explainedNumber);
						}
						PerkHelper.AddPerkBonusForParty(DefaultPerks.Medicine.GoodLogdings, party, true, ref explainedNumber);
					}
				}
				else if (!party.IsMoving && party.LastVisitedSettlement != null && party.LastVisitedSettlement.IsVillage && party.LastVisitedSettlement.Position2D.DistanceSquared(party.Position2D) < 1f && !party.LastVisitedSettlement.IsUnderRaid && !party.LastVisitedSettlement.IsRaided)
				{
					PerkHelper.AddPerkBonusForParty(DefaultPerks.Medicine.BushDoctor, party, false, ref explainedNumber);
				}
				if (party.Army != null)
				{
					PerkHelper.AddPerkBonusForParty(DefaultPerks.Scouting.Rearguard, party, true, ref explainedNumber);
				}
				if (party.ItemRoster.FoodVariety > 0 && party.HasPerk(DefaultPerks.Medicine.PerfectHealth, false))
				{
					explainedNumber.AddFactor((float)party.ItemRoster.FoodVariety * DefaultPerks.Medicine.PerfectHealth.PrimaryBonus / 100f, DefaultPerks.Medicine.PerfectHealth.Name);
				}
				if (party.HasPerk(DefaultPerks.Medicine.HelpingHands, false))
				{
					float num2 = (float)MathF.Floor((float)party.MemberRoster.TotalManCount / 10f) * DefaultPerks.Medicine.HelpingHands.PrimaryBonus;
					explainedNumber.AddFactor(num2, DefaultPerks.Medicine.HelpingHands.Name);
				}
			}
			return explainedNumber;
		}

		// Token: 0x0600165C RID: 5724 RVA: 0x0006B614 File Offset: 0x00069814
		public override ExplainedNumber GetDailyHealingHpForHeroes(MobileParty party, bool includeDescriptions = false)
		{
			if (party.Party.IsStarving && party.CurrentSettlement == null)
			{
				return new ExplainedNumber(-19f, includeDescriptions, DefaultPartyHealingModel._starvingText);
			}
			ExplainedNumber explainedNumber = new ExplainedNumber(11f, includeDescriptions, null);
			if (!party.IsGarrison && !party.IsMilitia)
			{
				if (!party.IsMoving)
				{
					PerkHelper.AddPerkBonusForParty(DefaultPerks.Medicine.TriageTent, party, true, ref explainedNumber);
					PerkHelper.AddPerkBonusForParty(DefaultPerks.Athletics.AGoodDaysRest, party, true, ref explainedNumber);
				}
				else
				{
					PerkHelper.AddPerkBonusForParty(DefaultPerks.Medicine.WalkItOff, party, true, ref explainedNumber);
					PerkHelper.AddPerkBonusForParty(DefaultPerks.Athletics.WalkItOff, party, true, ref explainedNumber);
				}
			}
			if (party.Morale >= Campaign.Current.Models.PartyMoraleModel.HighMoraleValue)
			{
				PerkHelper.AddPerkBonusForParty(DefaultPerks.Medicine.BestMedicine, party, true, ref explainedNumber);
			}
			if (party.CurrentSettlement != null && !party.CurrentSettlement.IsHideout)
			{
				if (party.CurrentSettlement.IsFortification)
				{
					explainedNumber.Add(8f, DefaultPartyHealingModel._settlementText, null);
				}
				if (party.CurrentSettlement.IsTown)
				{
					PerkHelper.AddPerkBonusForParty(DefaultPerks.Medicine.PristineStreets, party, false, ref explainedNumber);
				}
				PerkHelper.AddPerkBonusForParty(DefaultPerks.Medicine.GoodLogdings, party, true, ref explainedNumber);
			}
			else if (!party.IsMoving && party.LastVisitedSettlement != null && party.LastVisitedSettlement.IsVillage && party.LastVisitedSettlement.Position2D.DistanceSquared(party.Position2D) < 1f && !party.LastVisitedSettlement.IsUnderRaid && !party.LastVisitedSettlement.IsRaided)
			{
				PerkHelper.AddPerkBonusForParty(DefaultPerks.Medicine.BushDoctor, party, false, ref explainedNumber);
			}
			SkillHelper.AddSkillBonusForParty(DefaultSkills.Medicine, DefaultSkillEffects.HealingRateBonusForHeroes, party, ref explainedNumber);
			return explainedNumber;
		}

		// Token: 0x0600165D RID: 5725 RVA: 0x0006B7A8 File Offset: 0x000699A8
		public override int GetHeroesEffectedHealingAmount(Hero hero, float healingRate)
		{
			ExplainedNumber explainedNumber = new ExplainedNumber(healingRate, false, null);
			PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Medicine.SelfMedication, hero.CharacterObject, true, ref explainedNumber);
			float resultNumber = explainedNumber.ResultNumber;
			if (resultNumber - (float)((int)resultNumber) > MBRandom.RandomFloat)
			{
				return (int)resultNumber + 1;
			}
			return (int)resultNumber;
		}

		// Token: 0x0600165E RID: 5726 RVA: 0x0006B7F0 File Offset: 0x000699F0
		public override int GetBattleEndHealingAmount(MobileParty party, CharacterObject character)
		{
			float num = 0f;
			if (character.IsHero)
			{
				Hero heroObject = character.HeroObject;
				if (heroObject.GetPerkValue(DefaultPerks.Medicine.PreventiveMedicine))
				{
					num += (float)(heroObject.MaxHitPoints - heroObject.HitPoints) * DefaultPerks.Medicine.PreventiveMedicine.SecondaryBonus;
				}
				if (party.MapEventSide == party.MapEvent.AttackerSide && heroObject.GetPerkValue(DefaultPerks.Medicine.WalkItOff))
				{
					num += DefaultPerks.Medicine.WalkItOff.SecondaryBonus;
				}
			}
			return MathF.Round(num);
		}

		// Token: 0x040007C6 RID: 1990
		private const int StarvingEffectHeroes = -19;

		// Token: 0x040007C7 RID: 1991
		private const int FortificationEffectForHeroes = 8;

		// Token: 0x040007C8 RID: 1992
		private const int FortificationEffectForRegulars = 10;

		// Token: 0x040007C9 RID: 1993
		private const int BaseDailyHealingForHeroes = 11;

		// Token: 0x040007CA RID: 1994
		private const int BaseDailyHealingForTroops = 5;

		// Token: 0x040007CB RID: 1995
		private const int SkillEXPFromHealingTroops = 5;

		// Token: 0x040007CC RID: 1996
		private const float StarvingWoundedEffectRatio = 0.25f;

		// Token: 0x040007CD RID: 1997
		private const float StarvingWoundedEffectRatioForGarrison = 0.1f;

		// Token: 0x040007CE RID: 1998
		private static readonly TextObject _starvingText = new TextObject("{=jZYUdkXF}Starving", null);

		// Token: 0x040007CF RID: 1999
		private static readonly TextObject _settlementText = new TextObject("{=M0Gpl0dH}In Settlement", null);
	}
}
