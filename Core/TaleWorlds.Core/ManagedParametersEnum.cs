using System;

namespace TaleWorlds.Core
{
	// Token: 0x02000092 RID: 146
	public enum ManagedParametersEnum
	{
		// Token: 0x0400043A RID: 1082
		EnableCampaignTutorials,
		// Token: 0x0400043B RID: 1083
		ReducedMouseSensitivityMultiplier,
		// Token: 0x0400043C RID: 1084
		MeleeAddedElevationForCrosshair,
		// Token: 0x0400043D RID: 1085
		BipedalRadius,
		// Token: 0x0400043E RID: 1086
		QuadrupedalRadius,
		// Token: 0x0400043F RID: 1087
		BipedalCombatSpeedMinMultiplier,
		// Token: 0x04000440 RID: 1088
		BipedalCombatSpeedMaxMultiplier,
		// Token: 0x04000441 RID: 1089
		BipedalRangedReadySpeedMultiplier,
		// Token: 0x04000442 RID: 1090
		BipedalRangedReloadSpeedMultiplier,
		// Token: 0x04000443 RID: 1091
		DamageInterruptAttackThresholdPierce,
		// Token: 0x04000444 RID: 1092
		DamageInterruptAttackThresholdCut,
		// Token: 0x04000445 RID: 1093
		DamageInterruptAttackThresholdBlunt,
		// Token: 0x04000446 RID: 1094
		MakesRearAttackDamageThreshold,
		// Token: 0x04000447 RID: 1095
		MissileMinimumDamageToStick,
		// Token: 0x04000448 RID: 1096
		BreakableProjectileMinimumBreakSpeed,
		// Token: 0x04000449 RID: 1097
		FistFightDamageMultiplier,
		// Token: 0x0400044A RID: 1098
		FallDamageMultiplier,
		// Token: 0x0400044B RID: 1099
		FallDamageAbsorption,
		// Token: 0x0400044C RID: 1100
		FallSpeedReductionMultiplierForRiderDamage,
		// Token: 0x0400044D RID: 1101
		SwingHitWithArmDamageMultiplier,
		// Token: 0x0400044E RID: 1102
		ThrustHitWithArmDamageMultiplier,
		// Token: 0x0400044F RID: 1103
		NonTipThrustHitDamageMultiplier,
		// Token: 0x04000450 RID: 1104
		SwingCombatSpeedGraphZeroProgressValue,
		// Token: 0x04000451 RID: 1105
		SwingCombatSpeedGraphFirstMaximumPoint,
		// Token: 0x04000452 RID: 1106
		SwingCombatSpeedGraphSecondMaximumPoint,
		// Token: 0x04000453 RID: 1107
		SwingCombatSpeedGraphOneProgressValue,
		// Token: 0x04000454 RID: 1108
		OverSwingCombatSpeedGraphZeroProgressValue,
		// Token: 0x04000455 RID: 1109
		OverSwingCombatSpeedGraphFirstMaximumPoint,
		// Token: 0x04000456 RID: 1110
		OverSwingCombatSpeedGraphSecondMaximumPoint,
		// Token: 0x04000457 RID: 1111
		OverSwingCombatSpeedGraphOneProgressValue,
		// Token: 0x04000458 RID: 1112
		ThrustCombatSpeedGraphZeroProgressValue,
		// Token: 0x04000459 RID: 1113
		ThrustCombatSpeedGraphFirstMaximumPoint,
		// Token: 0x0400045A RID: 1114
		ThrustCombatSpeedGraphSecondMaximumPoint,
		// Token: 0x0400045B RID: 1115
		ThrustCombatSpeedGraphOneProgressValue,
		// Token: 0x0400045C RID: 1116
		StunPeriodAttackerSwing,
		// Token: 0x0400045D RID: 1117
		StunPeriodAttackerThrust,
		// Token: 0x0400045E RID: 1118
		StunDefendWeaponWeightOffsetShield,
		// Token: 0x0400045F RID: 1119
		StunDefendWeaponWeightMultiplierWeaponWeight,
		// Token: 0x04000460 RID: 1120
		StunDefendWeaponWeightBonusTwoHanded,
		// Token: 0x04000461 RID: 1121
		StunDefendWeaponWeightBonusPolearm,
		// Token: 0x04000462 RID: 1122
		StunMomentumTransferFactor,
		// Token: 0x04000463 RID: 1123
		StunDefendWeaponWeightParryMultiplier,
		// Token: 0x04000464 RID: 1124
		StunDefendWeaponWeightBonusRightStance,
		// Token: 0x04000465 RID: 1125
		StunDefendWeaponWeightBonusActiveBlocked,
		// Token: 0x04000466 RID: 1126
		StunDefendWeaponWeightBonusChamberBlocked,
		// Token: 0x04000467 RID: 1127
		StunPeriodAttackerFriendlyFire,
		// Token: 0x04000468 RID: 1128
		StunPeriodMax,
		// Token: 0x04000469 RID: 1129
		ProjectileMaxPenetrationSpeed,
		// Token: 0x0400046A RID: 1130
		ObjectMinPenetration,
		// Token: 0x0400046B RID: 1131
		ObjectMaxPenetration,
		// Token: 0x0400046C RID: 1132
		ProjectileMinPenetration,
		// Token: 0x0400046D RID: 1133
		ProjectileMaxPenetration,
		// Token: 0x0400046E RID: 1134
		RotatingProjectileMinPenetration,
		// Token: 0x0400046F RID: 1135
		RotatingProjectileMaxPenetration,
		// Token: 0x04000470 RID: 1136
		ShieldRightStanceBlockDamageMultiplier,
		// Token: 0x04000471 RID: 1137
		ShieldCorrectSideBlockDamageMultiplier,
		// Token: 0x04000472 RID: 1138
		AgentProjectileNormalWeight,
		// Token: 0x04000473 RID: 1139
		ProjectileNormalWeight,
		// Token: 0x04000474 RID: 1140
		ShieldPenetrationOffset,
		// Token: 0x04000475 RID: 1141
		ShieldPenetrationFactor,
		// Token: 0x04000476 RID: 1142
		AirFrictionJavelin,
		// Token: 0x04000477 RID: 1143
		AirFrictionArrow,
		// Token: 0x04000478 RID: 1144
		AirFrictionBallistaBolt,
		// Token: 0x04000479 RID: 1145
		AirFrictionBullet,
		// Token: 0x0400047A RID: 1146
		AirFrictionKnife,
		// Token: 0x0400047B RID: 1147
		AirFrictionAxe,
		// Token: 0x0400047C RID: 1148
		HeavyAttackMomentumMultiplier,
		// Token: 0x0400047D RID: 1149
		ActivateHeroTest,
		// Token: 0x0400047E RID: 1150
		Count
	}
}
