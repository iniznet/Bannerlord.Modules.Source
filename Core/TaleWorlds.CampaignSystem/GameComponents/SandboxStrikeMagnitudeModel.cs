using System;
using Helpers;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.CampaignSystem.GameComponents
{
	// Token: 0x02000150 RID: 336
	public class SandboxStrikeMagnitudeModel : StrikeMagnitudeCalculationModel
	{
		// Token: 0x0600182B RID: 6187 RVA: 0x0007AB3E File Offset: 0x00078D3E
		public override float CalculateHorseArcheryFactor(BasicCharacterObject characterObject)
		{
			return 100f;
		}

		// Token: 0x0600182C RID: 6188 RVA: 0x0007AB48 File Offset: 0x00078D48
		public override float CalculateStrikeMagnitudeForSwing(BasicCharacterObject attackerCharacter, BasicCharacterObject attackerCaptainCharacter, float swingSpeed, float impactPointAsPercent, float weaponWeight, WeaponComponentData weaponUsageComponent, float weaponLength, float weaponInertia, float weaponCoM, float extraLinearSpeed, bool doesAttackerHaveMount)
		{
			CharacterObject characterObject = attackerCharacter as CharacterObject;
			ExplainedNumber explainedNumber = new ExplainedNumber(extraLinearSpeed, false, null);
			if (characterObject != null && extraLinearSpeed > 0f)
			{
				SkillObject relevantSkill = weaponUsageComponent.RelevantSkill;
				if (doesAttackerHaveMount)
				{
					if ((relevantSkill == DefaultSkills.OneHanded || relevantSkill == DefaultSkills.TwoHanded || relevantSkill == DefaultSkills.Polearm) && attackerCaptainCharacter != null)
					{
						PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Riding.NomadicTraditions, attackerCaptainCharacter as CharacterObject, ref explainedNumber);
					}
				}
				else
				{
					if (relevantSkill == DefaultSkills.TwoHanded)
					{
						PerkHelper.AddPerkBonusForCharacter(DefaultPerks.TwoHanded.RecklessCharge, characterObject, true, ref explainedNumber);
					}
					PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Roguery.DashAndSlash, characterObject, true, ref explainedNumber);
					PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Athletics.SurgingBlow, characterObject, true, ref explainedNumber);
					PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Athletics.SurgingBlow, attackerCaptainCharacter as CharacterObject, ref explainedNumber);
				}
				if (relevantSkill == DefaultSkills.Polearm)
				{
					PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Polearm.Lancer, attackerCaptainCharacter as CharacterObject, ref explainedNumber);
					if (doesAttackerHaveMount)
					{
						PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Polearm.Lancer, attackerCharacter as CharacterObject, true, ref explainedNumber);
						PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Polearm.UnstoppableForce, attackerCaptainCharacter as CharacterObject, ref explainedNumber);
					}
				}
			}
			return CombatStatCalculator.CalculateStrikeMagnitudeForSwing(swingSpeed, impactPointAsPercent, weaponWeight, weaponLength, weaponInertia, weaponCoM, explainedNumber.ResultNumber);
		}

		// Token: 0x0600182D RID: 6189 RVA: 0x0007AC58 File Offset: 0x00078E58
		public override float CalculateStrikeMagnitudeForThrust(BasicCharacterObject attackerCharacter, BasicCharacterObject attackerCaptainCharacter, float thrustWeaponSpeed, float weaponWeight, WeaponComponentData weaponUsageComponent, float extraLinearSpeed, bool doesAttackerHaveMount, bool isThrown = false)
		{
			CharacterObject characterObject = attackerCharacter as CharacterObject;
			ExplainedNumber explainedNumber = new ExplainedNumber(extraLinearSpeed, false, null);
			if (characterObject != null && extraLinearSpeed > 0f)
			{
				SkillObject relevantSkill = weaponUsageComponent.RelevantSkill;
				if (!doesAttackerHaveMount)
				{
					if (relevantSkill == DefaultSkills.TwoHanded)
					{
						PerkHelper.AddPerkBonusForCharacter(DefaultPerks.TwoHanded.RecklessCharge, characterObject, true, ref explainedNumber);
					}
					PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Roguery.DashAndSlash, characterObject, true, ref explainedNumber);
					PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Athletics.SurgingBlow, characterObject, true, ref explainedNumber);
					PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Athletics.SurgingBlow, attackerCaptainCharacter as CharacterObject, ref explainedNumber);
				}
				if (relevantSkill == DefaultSkills.Polearm)
				{
					PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Polearm.Lancer, attackerCharacter as CharacterObject, true, ref explainedNumber);
					PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Polearm.Lancer, attackerCaptainCharacter as CharacterObject, ref explainedNumber);
					if (doesAttackerHaveMount)
					{
						PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Polearm.UnstoppableForce, attackerCaptainCharacter as CharacterObject, ref explainedNumber);
					}
				}
			}
			return CombatStatCalculator.CalculateStrikeMagnitudeForThrust(thrustWeaponSpeed, weaponWeight, explainedNumber.ResultNumber, isThrown);
		}

		// Token: 0x0600182E RID: 6190 RVA: 0x0007AD30 File Offset: 0x00078F30
		public override float CalculateSpeedBonusMultiplierForMissile(BasicCharacterObject attackerCharacter, WeaponClass ammoClass)
		{
			float num = 0f;
			CharacterObject characterObject;
			if ((characterObject = attackerCharacter as CharacterObject) != null && characterObject.IsHero && (ammoClass == WeaponClass.Stone || ammoClass == WeaponClass.ThrowingAxe || ammoClass == WeaponClass.ThrowingKnife || ammoClass == WeaponClass.Javelin) && characterObject.GetPerkValue(DefaultPerks.Throwing.RunningThrow))
			{
				num += DefaultPerks.Throwing.RunningThrow.PrimaryBonus;
			}
			return num;
		}

		// Token: 0x0600182F RID: 6191 RVA: 0x0007AD8C File Offset: 0x00078F8C
		public override float ComputeRawDamage(DamageTypes damageType, float magnitude, float armorEffectiveness, float absorbedDamageRatio)
		{
			float bluntDamageFactorByDamageType = this.GetBluntDamageFactorByDamageType(damageType);
			float num = 50f / (50f + armorEffectiveness);
			float num2 = magnitude * num;
			float num3 = bluntDamageFactorByDamageType * num2;
			float num4;
			switch (damageType)
			{
			case DamageTypes.Cut:
				num4 = MathF.Max(0f, num2 - armorEffectiveness * 0.5f);
				break;
			case DamageTypes.Pierce:
				num4 = MathF.Max(0f, num2 - armorEffectiveness * 0.33f);
				break;
			case DamageTypes.Blunt:
				num4 = MathF.Max(0f, num2 - armorEffectiveness * 0.2f);
				break;
			default:
				Debug.FailedAssert("Given damage type is invalid.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.CampaignSystem\\GameComponents\\SandboxStrikeMagnitudeModel.cs", "ComputeRawDamage", 153);
				return 0f;
			}
			num3 += (1f - bluntDamageFactorByDamageType) * num4;
			return num3 * absorbedDamageRatio;
		}

		// Token: 0x06001830 RID: 6192 RVA: 0x0007AE44 File Offset: 0x00079044
		public override float GetBluntDamageFactorByDamageType(DamageTypes damageType)
		{
			float num = 0f;
			switch (damageType)
			{
			case DamageTypes.Cut:
				num = 0.1f;
				break;
			case DamageTypes.Pierce:
				num = 0.25f;
				break;
			case DamageTypes.Blunt:
				num = 0.6f;
				break;
			}
			return num;
		}

		// Token: 0x06001831 RID: 6193 RVA: 0x0007AE84 File Offset: 0x00079084
		public override float CalculateAdjustedArmorForBlow(float baseArmor, BasicCharacterObject attackerCharacter, BasicCharacterObject attackerCaptainCharacter, BasicCharacterObject victimCharacter, BasicCharacterObject victimCaptainCharacter, WeaponComponentData weaponComponent)
		{
			bool flag = false;
			float num = baseArmor;
			CharacterObject characterObject = attackerCharacter as CharacterObject;
			CharacterObject characterObject2 = attackerCaptainCharacter as CharacterObject;
			if (attackerCharacter == characterObject2)
			{
				characterObject2 = null;
			}
			if (num > 0f && characterObject != null)
			{
				if (weaponComponent != null && weaponComponent.RelevantSkill == DefaultSkills.Crossbow && characterObject.GetPerkValue(DefaultPerks.Crossbow.Piercer) && baseArmor < DefaultPerks.Crossbow.Piercer.PrimaryBonus)
				{
					flag = true;
				}
				if (flag)
				{
					num = 0f;
				}
				else
				{
					ExplainedNumber explainedNumber = new ExplainedNumber(baseArmor, false, null);
					PerkHelper.AddPerkBonusForCharacter(DefaultPerks.TwoHanded.Vandal, characterObject, true, ref explainedNumber);
					if (weaponComponent != null)
					{
						if (weaponComponent.RelevantSkill == DefaultSkills.OneHanded)
						{
							PerkHelper.AddPerkBonusForCharacter(DefaultPerks.OneHanded.ChinkInTheArmor, characterObject, true, ref explainedNumber);
						}
						else if (weaponComponent.RelevantSkill == DefaultSkills.Bow)
						{
							PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Bow.Bodkin, characterObject, true, ref explainedNumber);
							if (characterObject2 != null)
							{
								PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Bow.Bodkin, characterObject2, ref explainedNumber);
							}
						}
						else if (weaponComponent.RelevantSkill == DefaultSkills.Crossbow)
						{
							PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Crossbow.Puncture, characterObject, true, ref explainedNumber);
							if (characterObject2 != null)
							{
								PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Crossbow.Puncture, characterObject2, ref explainedNumber);
							}
						}
						else if (weaponComponent.RelevantSkill == DefaultSkills.Throwing)
						{
							PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Throwing.WeakSpot, characterObject, true, ref explainedNumber);
							if (characterObject2 != null)
							{
								PerkHelper.AddPerkBonusFromCaptain(DefaultPerks.Throwing.WeakSpot, characterObject2, ref explainedNumber);
							}
						}
					}
					num = MathF.Max(0f, explainedNumber.ResultNumber);
				}
			}
			return num;
		}
	}
}
