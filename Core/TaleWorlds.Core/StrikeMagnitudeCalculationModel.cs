using System;

namespace TaleWorlds.Core
{
	// Token: 0x0200006B RID: 107
	public abstract class StrikeMagnitudeCalculationModel : GameModel
	{
		// Token: 0x060006FC RID: 1788
		public abstract float CalculateStrikeMagnitudeForSwing(BasicCharacterObject attackerCharacter, BasicCharacterObject attackerCaptainCharacter, float swingSpeed, float impactPointAsPercent, float weaponWeight, WeaponComponentData weaponUsageComponent, float weaponLength, float weaponInertia, float weaponCoM, float extraLinearSpeed, bool doesAttackerHaveMount);

		// Token: 0x060006FD RID: 1789
		public abstract float CalculateStrikeMagnitudeForThrust(BasicCharacterObject attackerCharacter, BasicCharacterObject attackerCaptainCharacter, float thrustWeaponSpeed, float weaponWeight, WeaponComponentData weaponUsageComponent, float extraLinearSpeed, bool doesAtttackerHaveMount, bool isThrown = false);

		// Token: 0x060006FE RID: 1790
		public abstract float CalculateSpeedBonusMultiplierForMissile(BasicCharacterObject attackerCharacter, WeaponClass ammoClass);

		// Token: 0x060006FF RID: 1791
		public abstract float ComputeRawDamage(DamageTypes damageType, float magnitude, float armorEffectiveness, float absorbedDamageRatio);

		// Token: 0x06000700 RID: 1792
		public abstract float GetBluntDamageFactorByDamageType(DamageTypes damageType);

		// Token: 0x06000701 RID: 1793
		public abstract float CalculateHorseArcheryFactor(BasicCharacterObject characterObject);

		// Token: 0x06000702 RID: 1794 RVA: 0x00018403 File Offset: 0x00016603
		public virtual float CalculateAdjustedArmorForBlow(float baseArmor, BasicCharacterObject attackerCharacter, BasicCharacterObject attackerCaptainCharacter, BasicCharacterObject victimCharacter, BasicCharacterObject victimCaptainCharacter, WeaponComponentData weaponComponent)
		{
			return baseArmor;
		}
	}
}
