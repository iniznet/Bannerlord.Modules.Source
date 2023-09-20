using System;
using TaleWorlds.Library;

namespace TaleWorlds.Core
{
	// Token: 0x02000023 RID: 35
	public static class CombatStatCalculator
	{
		// Token: 0x060001B2 RID: 434 RVA: 0x000073D4 File Offset: 0x000055D4
		public static float CalculateStrikeMagnitudeForSwing(float swingSpeed, float impactPointAsPercent, float weaponWeight, float weaponLength, float weaponInertia, float weaponCoM, float extraLinearSpeed)
		{
			float num = weaponLength * impactPointAsPercent - weaponCoM;
			float num2 = swingSpeed * (0.5f + weaponCoM) + extraLinearSpeed;
			float num3 = 0.5f * weaponWeight * num2 * num2;
			float num4 = 0.5f * weaponInertia * swingSpeed * swingSpeed;
			float num5 = num3 + num4;
			float num6 = (num2 + swingSpeed * num) / (1f / weaponWeight + num * num / weaponInertia);
			float num7 = num2 - num6 / weaponWeight;
			float num8 = swingSpeed - num6 * num / weaponInertia;
			float num9 = 0.5f * weaponWeight * num7 * num7;
			float num10 = 0.5f * weaponInertia * num8 * num8;
			float num11 = num9 + num10;
			float num12 = num5 - num11 + 0.5f;
			return 0.067f * num12;
		}

		// Token: 0x060001B3 RID: 435 RVA: 0x00007470 File Offset: 0x00005670
		public static float CalculateStrikeMagnitudeForThrust(float thrustWeaponSpeed, float weaponWeight, float extraLinearSpeed, bool isThrown)
		{
			float num = thrustWeaponSpeed + extraLinearSpeed;
			if (num > 0f)
			{
				if (!isThrown)
				{
					weaponWeight += 2.5f;
				}
				float num2 = 0.5f * weaponWeight * num * num;
				return 0.125f * num2;
			}
			return 0f;
		}

		// Token: 0x060001B4 RID: 436 RVA: 0x000074B0 File Offset: 0x000056B0
		private static float CalculateStrikeMagnitudeForPassiveUsage(float weaponWeight, float extraLinearSpeed)
		{
			float num = 20f / ((extraLinearSpeed > 0f) ? MathF.Pow(extraLinearSpeed, 0.1f) : 1f) + weaponWeight;
			float num2 = CombatStatCalculator.CalculateStrikeMagnitudeForThrust(0f, num, extraLinearSpeed * 0.83f, false);
			if (num2 < 10f)
			{
				return 0f;
			}
			return num2;
		}

		// Token: 0x060001B5 RID: 437 RVA: 0x00007504 File Offset: 0x00005704
		public static float CalculateBaseBlowMagnitudeForSwing(float angularSpeed, float weaponReach, float weaponWeight, float weaponInertia, float weaponCoM, float impactPoint, float exraLinearSpeed)
		{
			impactPoint = MathF.Min(impactPoint, 0.93f);
			float num = MBMath.ClampFloat(0.4f / weaponReach, 0f, 1f);
			float num2 = 0f;
			for (int i = 0; i < 5; i++)
			{
				float num3 = impactPoint + (float)i / 4f * num;
				if (num3 >= 1f)
				{
					break;
				}
				float num4 = CombatStatCalculator.CalculateStrikeMagnitudeForSwing(angularSpeed, num3, weaponWeight, weaponReach, weaponInertia, weaponCoM, exraLinearSpeed);
				if (num2 < num4)
				{
					num2 = num4;
				}
			}
			return num2;
		}

		// Token: 0x060001B6 RID: 438 RVA: 0x00007577 File Offset: 0x00005777
		public static float CalculateBaseBlowMagnitudeForThrust(float linearSpeed, float weaponWeight, float exraLinearSpeed)
		{
			return CombatStatCalculator.CalculateStrikeMagnitudeForThrust(linearSpeed, weaponWeight, exraLinearSpeed, false);
		}

		// Token: 0x060001B7 RID: 439 RVA: 0x00007582 File Offset: 0x00005782
		public static float CalculateBaseBlowMagnitudeForPassiveUsage(float weaponWeight, float extraLinearSpeed)
		{
			return CombatStatCalculator.CalculateStrikeMagnitudeForPassiveUsage(weaponWeight, extraLinearSpeed);
		}

		// Token: 0x04000165 RID: 357
		public const float ReferenceSwingSpeed = 22f;

		// Token: 0x04000166 RID: 358
		public const float ReferenceThrustSpeed = 8.5f;

		// Token: 0x04000167 RID: 359
		public const float SwingSpeedConst = 4.5454545f;

		// Token: 0x04000168 RID: 360
		public const float ThrustSpeedConst = 11.764706f;

		// Token: 0x04000169 RID: 361
		public const float DefaultImpactDistanceFromTip = 0.07f;

		// Token: 0x0400016A RID: 362
		public const float ArmLength = 0.5f;

		// Token: 0x0400016B RID: 363
		public const float ArmWeight = 2.5f;
	}
}
