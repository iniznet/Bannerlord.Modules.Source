using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class MultiplayerStrikeMagnitudeModel : StrikeMagnitudeCalculationModel
	{
		public override float CalculateStrikeMagnitudeForSwing(BasicCharacterObject attackerCharacter, BasicCharacterObject attackerCaptainCharacter, float swingSpeed, float impactPoint, float weaponWeight, WeaponComponentData weaponUsageComponent, float weaponLength, float weaponInertia, float weaponCoM, float extraLinearSpeed, bool doesAttackerHaveMount)
		{
			return CombatStatCalculator.CalculateStrikeMagnitudeForSwing(swingSpeed, impactPoint, weaponWeight, weaponLength, weaponInertia, weaponCoM, extraLinearSpeed);
		}

		public override float CalculateStrikeMagnitudeForThrust(BasicCharacterObject attackerCharacter, BasicCharacterObject attackerCaptainCharacter, float thrustWeaponSpeed, float weaponWeight, WeaponComponentData weaponUsageComponent, float extraLinearSpeed, bool doesAttackerHaveMount, bool isThrown = false)
		{
			return CombatStatCalculator.CalculateStrikeMagnitudeForThrust(thrustWeaponSpeed, weaponWeight, extraLinearSpeed, isThrown);
		}

		public override float CalculateSpeedBonusMultiplierForMissile(BasicCharacterObject attackerCharacter, WeaponClass ammoClass)
		{
			return 0f;
		}

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

		public override float CalculateHorseArcheryFactor(BasicCharacterObject characterObject)
		{
			return 100f;
		}
	}
}
