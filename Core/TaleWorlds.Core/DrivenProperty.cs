using System;

namespace TaleWorlds.Core
{
	// Token: 0x02000007 RID: 7
	public enum DrivenProperty
	{
		// Token: 0x04000044 RID: 68
		None = -1,
		// Token: 0x04000045 RID: 69
		AiRangedHorsebackMissileRange,
		// Token: 0x04000046 RID: 70
		AiFacingMissileWatch,
		// Token: 0x04000047 RID: 71
		AiFlyingMissileCheckRadius,
		// Token: 0x04000048 RID: 72
		AiShootFreq,
		// Token: 0x04000049 RID: 73
		AiWaitBeforeShootFactor,
		// Token: 0x0400004A RID: 74
		AIBlockOnDecideAbility,
		// Token: 0x0400004B RID: 75
		AIParryOnDecideAbility,
		// Token: 0x0400004C RID: 76
		AiTryChamberAttackOnDecide,
		// Token: 0x0400004D RID: 77
		AIAttackOnParryChance,
		// Token: 0x0400004E RID: 78
		AiAttackOnParryTiming,
		// Token: 0x0400004F RID: 79
		AIDecideOnAttackChance,
		// Token: 0x04000050 RID: 80
		AIParryOnAttackAbility,
		// Token: 0x04000051 RID: 81
		AiKick,
		// Token: 0x04000052 RID: 82
		AiAttackCalculationMaxTimeFactor,
		// Token: 0x04000053 RID: 83
		AiDecideOnAttackWhenReceiveHitTiming,
		// Token: 0x04000054 RID: 84
		AiDecideOnAttackContinueAction,
		// Token: 0x04000055 RID: 85
		AiDecideOnAttackingContinue,
		// Token: 0x04000056 RID: 86
		AIParryOnAttackingContinueAbility,
		// Token: 0x04000057 RID: 87
		AIDecideOnRealizeEnemyBlockingAttackAbility,
		// Token: 0x04000058 RID: 88
		AIRealizeBlockingFromIncorrectSideAbility,
		// Token: 0x04000059 RID: 89
		AiAttackingShieldDefenseChance,
		// Token: 0x0400005A RID: 90
		AiAttackingShieldDefenseTimer,
		// Token: 0x0400005B RID: 91
		AiCheckMovementIntervalFactor,
		// Token: 0x0400005C RID: 92
		AiMovementDelayFactor,
		// Token: 0x0400005D RID: 93
		AiParryDecisionChangeValue,
		// Token: 0x0400005E RID: 94
		AiDefendWithShieldDecisionChanceValue,
		// Token: 0x0400005F RID: 95
		AiMoveEnemySideTimeValue,
		// Token: 0x04000060 RID: 96
		AiMinimumDistanceToContinueFactor,
		// Token: 0x04000061 RID: 97
		AiHearingDistanceFactor,
		// Token: 0x04000062 RID: 98
		AiChargeHorsebackTargetDistFactor,
		// Token: 0x04000063 RID: 99
		AiRangerLeadErrorMin,
		// Token: 0x04000064 RID: 100
		AiRangerLeadErrorMax,
		// Token: 0x04000065 RID: 101
		AiRangerVerticalErrorMultiplier,
		// Token: 0x04000066 RID: 102
		AiRangerHorizontalErrorMultiplier,
		// Token: 0x04000067 RID: 103
		AIAttackOnDecideChance,
		// Token: 0x04000068 RID: 104
		AiRaiseShieldDelayTimeBase,
		// Token: 0x04000069 RID: 105
		AiUseShieldAgainstEnemyMissileProbability,
		// Token: 0x0400006A RID: 106
		AiSpeciesIndex,
		// Token: 0x0400006B RID: 107
		AiRandomizedDefendDirectionChance,
		// Token: 0x0400006C RID: 108
		AiShooterError,
		// Token: 0x0400006D RID: 109
		AISetNoAttackTimerAfterBeingHitAbility,
		// Token: 0x0400006E RID: 110
		AISetNoAttackTimerAfterBeingParriedAbility,
		// Token: 0x0400006F RID: 111
		AISetNoDefendTimerAfterHittingAbility,
		// Token: 0x04000070 RID: 112
		AISetNoDefendTimerAfterParryingAbility,
		// Token: 0x04000071 RID: 113
		AIEstimateStunDurationPrecision,
		// Token: 0x04000072 RID: 114
		AIHoldingReadyMaxDuration,
		// Token: 0x04000073 RID: 115
		AIHoldingReadyVariationPercentage,
		// Token: 0x04000074 RID: 116
		MountChargeDamage,
		// Token: 0x04000075 RID: 117
		MountDifficulty,
		// Token: 0x04000076 RID: 118
		ArmorEncumbrance,
		// Token: 0x04000077 RID: 119
		ArmorHead,
		// Token: 0x04000078 RID: 120
		ArmorTorso,
		// Token: 0x04000079 RID: 121
		ArmorLegs,
		// Token: 0x0400007A RID: 122
		ArmorArms,
		// Token: 0x0400007B RID: 123
		UseRealisticBlocking,
		// Token: 0x0400007C RID: 124
		WeaponsEncumbrance,
		// Token: 0x0400007D RID: 125
		SwingSpeedMultiplier,
		// Token: 0x0400007E RID: 126
		ThrustOrRangedReadySpeedMultiplier,
		// Token: 0x0400007F RID: 127
		HandlingMultiplier,
		// Token: 0x04000080 RID: 128
		ReloadSpeed,
		// Token: 0x04000081 RID: 129
		MissileSpeedMultiplier,
		// Token: 0x04000082 RID: 130
		WeaponInaccuracy,
		// Token: 0x04000083 RID: 131
		WeaponWorstMobileAccuracyPenalty,
		// Token: 0x04000084 RID: 132
		WeaponWorstUnsteadyAccuracyPenalty,
		// Token: 0x04000085 RID: 133
		WeaponBestAccuracyWaitTime,
		// Token: 0x04000086 RID: 134
		WeaponUnsteadyBeginTime,
		// Token: 0x04000087 RID: 135
		WeaponUnsteadyEndTime,
		// Token: 0x04000088 RID: 136
		WeaponRotationalAccuracyPenaltyInRadians,
		// Token: 0x04000089 RID: 137
		AttributeRiding,
		// Token: 0x0400008A RID: 138
		AttributeShield,
		// Token: 0x0400008B RID: 139
		AttributeShieldMissileCollisionBodySizeAdder,
		// Token: 0x0400008C RID: 140
		ShieldBashStunDurationMultiplier,
		// Token: 0x0400008D RID: 141
		KickStunDurationMultiplier,
		// Token: 0x0400008E RID: 142
		ReloadMovementPenaltyFactor,
		// Token: 0x0400008F RID: 143
		TopSpeedReachDuration,
		// Token: 0x04000090 RID: 144
		MaxSpeedMultiplier,
		// Token: 0x04000091 RID: 145
		CombatMaxSpeedMultiplier,
		// Token: 0x04000092 RID: 146
		AttributeHorseArchery,
		// Token: 0x04000093 RID: 147
		AttributeCourage,
		// Token: 0x04000094 RID: 148
		MountManeuver,
		// Token: 0x04000095 RID: 149
		MountSpeed,
		// Token: 0x04000096 RID: 150
		MountDashAccelerationMultiplier,
		// Token: 0x04000097 RID: 151
		BipedalRangedReadySpeedMultiplier,
		// Token: 0x04000098 RID: 152
		BipedalRangedReloadSpeedMultiplier,
		// Token: 0x04000099 RID: 153
		Count,
		// Token: 0x0400009A RID: 154
		DrivenPropertiesCalculatedAtSpawnEnd = 55
	}
}
