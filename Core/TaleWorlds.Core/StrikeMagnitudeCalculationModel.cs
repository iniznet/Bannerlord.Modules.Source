using System;

namespace TaleWorlds.Core
{
	public abstract class StrikeMagnitudeCalculationModel : GameModel
	{
		public abstract float CalculateStrikeMagnitudeForMissile(BasicCharacterObject attackerCharacter, BasicCharacterObject attackerCaptainCharacter, float missileDamage, float missileSpeed, float missileStartingSpeed, ItemObject weaponItem, WeaponComponentData weaponUsageComponent);

		public abstract float CalculateStrikeMagnitudeForSwing(BasicCharacterObject attackerCharacter, BasicCharacterObject attackerCaptainCharacter, float swingSpeed, float impactPointAsPercent, float weaponWeight, ItemObject weaponItem, WeaponComponentData weaponUsageComponent, float weaponLength, float weaponInertia, float weaponCoM, float extraLinearSpeed, bool doesAttackerHaveMount);

		public abstract float CalculateStrikeMagnitudeForThrust(BasicCharacterObject attackerCharacter, BasicCharacterObject attackerCaptainCharacter, float thrustWeaponSpeed, float weaponWeight, ItemObject weaponItem, WeaponComponentData weaponUsageComponent, float extraLinearSpeed, bool doesAtttackerHaveMount, bool isThrown = false);

		public abstract float ComputeRawDamage(DamageTypes damageType, float magnitude, float armorEffectiveness, float absorbedDamageRatio);

		public abstract float GetBluntDamageFactorByDamageType(DamageTypes damageType);

		public abstract float CalculateHorseArcheryFactor(BasicCharacterObject characterObject);

		public virtual float CalculateAdjustedArmorForBlow(float baseArmor, BasicCharacterObject attackerCharacter, BasicCharacterObject attackerCaptainCharacter, BasicCharacterObject victimCharacter, BasicCharacterObject victimCaptainCharacter, WeaponComponentData weaponComponent)
		{
			return baseArmor;
		}
	}
}
