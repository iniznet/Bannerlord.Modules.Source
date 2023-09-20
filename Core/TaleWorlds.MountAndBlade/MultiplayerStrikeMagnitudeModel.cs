using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020001EE RID: 494
	public class MultiplayerStrikeMagnitudeModel : StrikeMagnitudeCalculationModel
	{
		// Token: 0x06001B98 RID: 7064 RVA: 0x00061D64 File Offset: 0x0005FF64
		public override float CalculateStrikeMagnitudeForSwing(BasicCharacterObject attackerCharacter, BasicCharacterObject attackerCaptainCharacter, float swingSpeed, float impactPoint, float weaponWeight, WeaponComponentData weaponUsageComponent, float weaponLength, float weaponInertia, float weaponCoM, float extraLinearSpeed, bool doesAttackerHaveMount)
		{
			return CombatStatCalculator.CalculateStrikeMagnitudeForSwing(swingSpeed, impactPoint, weaponWeight, weaponLength, weaponInertia, weaponCoM, extraLinearSpeed);
		}

		// Token: 0x06001B99 RID: 7065 RVA: 0x00061D78 File Offset: 0x0005FF78
		public override float CalculateStrikeMagnitudeForThrust(BasicCharacterObject attackerCharacter, BasicCharacterObject attackerCaptainCharacter, float thrustWeaponSpeed, float weaponWeight, WeaponComponentData weaponUsageComponent, float extraLinearSpeed, bool doesAttackerHaveMount, bool isThrown = false)
		{
			return CombatStatCalculator.CalculateStrikeMagnitudeForThrust(thrustWeaponSpeed, weaponWeight, extraLinearSpeed, isThrown);
		}

		// Token: 0x06001B9A RID: 7066 RVA: 0x00061D86 File Offset: 0x0005FF86
		public override float CalculateSpeedBonusMultiplierForMissile(BasicCharacterObject attackerCharacter, WeaponClass ammoClass)
		{
			return 0f;
		}

		// Token: 0x06001B9B RID: 7067 RVA: 0x00061D90 File Offset: 0x0005FF90
		public override float ComputeRawDamage(DamageTypes damageType, float magnitude, float armorEffectiveness, float absorbedDamageRatio)
		{
			float bluntDamageFactorByDamageType = this.GetBluntDamageFactorByDamageType(damageType);
			float num = 100f / (100f + armorEffectiveness);
			float num2 = magnitude * num;
			float num3 = bluntDamageFactorByDamageType * num2;
			if (damageType != DamageTypes.Blunt)
			{
				float num4;
				if (damageType != DamageTypes.Cut)
				{
					if (damageType != DamageTypes.Pierce)
					{
						Debug.FailedAssert("Given damage type is invalid.", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\ComponentInterfaces\\MultiplayerStrikeMagnitudeModel.cs", "ComputeRawDamage", 45);
						return 0f;
					}
					num4 = MathF.Max(0f, magnitude * (45f / (45f + armorEffectiveness)));
				}
				else
				{
					num4 = MathF.Max(0f, magnitude * (1f - 0.6f * armorEffectiveness / (20f + 0.4f * armorEffectiveness)));
				}
				num3 += (1f - bluntDamageFactorByDamageType) * num4;
			}
			return num3 * absorbedDamageRatio;
		}

		// Token: 0x06001B9C RID: 7068 RVA: 0x00061E40 File Offset: 0x00060040
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
				num = 1f;
				break;
			}
			return num;
		}

		// Token: 0x06001B9D RID: 7069 RVA: 0x00061E7E File Offset: 0x0006007E
		public override float CalculateHorseArcheryFactor(BasicCharacterObject characterObject)
		{
			return 100f;
		}
	}
}
