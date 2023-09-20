using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class AgentDrivenProperties
	{
		internal float[] Values
		{
			get
			{
				return this._statValues;
			}
		}

		public AgentDrivenProperties()
		{
			this._statValues = new float[84];
		}

		public float GetStat(DrivenProperty propertyEnum)
		{
			return this._statValues[(int)propertyEnum];
		}

		public void SetStat(DrivenProperty propertyEnum, float value)
		{
			this._statValues[(int)propertyEnum] = value;
		}

		public float SwingSpeedMultiplier
		{
			get
			{
				return this.GetStat(DrivenProperty.SwingSpeedMultiplier);
			}
			set
			{
				this.SetStat(DrivenProperty.SwingSpeedMultiplier, value);
			}
		}

		public float ThrustOrRangedReadySpeedMultiplier
		{
			get
			{
				return this.GetStat(DrivenProperty.ThrustOrRangedReadySpeedMultiplier);
			}
			set
			{
				this.SetStat(DrivenProperty.ThrustOrRangedReadySpeedMultiplier, value);
			}
		}

		public float HandlingMultiplier
		{
			get
			{
				return this.GetStat(DrivenProperty.HandlingMultiplier);
			}
			set
			{
				this.SetStat(DrivenProperty.HandlingMultiplier, value);
			}
		}

		public float ReloadSpeed
		{
			get
			{
				return this.GetStat(DrivenProperty.ReloadSpeed);
			}
			set
			{
				this.SetStat(DrivenProperty.ReloadSpeed, value);
			}
		}

		public float MissileSpeedMultiplier
		{
			get
			{
				return this.GetStat(DrivenProperty.MissileSpeedMultiplier);
			}
			set
			{
				this.SetStat(DrivenProperty.MissileSpeedMultiplier, value);
			}
		}

		public float WeaponInaccuracy
		{
			get
			{
				return this.GetStat(DrivenProperty.WeaponInaccuracy);
			}
			set
			{
				this.SetStat(DrivenProperty.WeaponInaccuracy, value);
			}
		}

		public float WeaponMaxMovementAccuracyPenalty
		{
			get
			{
				return this.GetStat(DrivenProperty.WeaponWorstMobileAccuracyPenalty);
			}
			set
			{
				this.SetStat(DrivenProperty.WeaponWorstMobileAccuracyPenalty, value);
			}
		}

		public float WeaponMaxUnsteadyAccuracyPenalty
		{
			get
			{
				return this.GetStat(DrivenProperty.WeaponWorstUnsteadyAccuracyPenalty);
			}
			set
			{
				this.SetStat(DrivenProperty.WeaponWorstUnsteadyAccuracyPenalty, value);
			}
		}

		public float WeaponBestAccuracyWaitTime
		{
			get
			{
				return this.GetStat(DrivenProperty.WeaponBestAccuracyWaitTime);
			}
			set
			{
				this.SetStat(DrivenProperty.WeaponBestAccuracyWaitTime, value);
			}
		}

		public float WeaponUnsteadyBeginTime
		{
			get
			{
				return this.GetStat(DrivenProperty.WeaponUnsteadyBeginTime);
			}
			set
			{
				this.SetStat(DrivenProperty.WeaponUnsteadyBeginTime, value);
			}
		}

		public float WeaponUnsteadyEndTime
		{
			get
			{
				return this.GetStat(DrivenProperty.WeaponUnsteadyEndTime);
			}
			set
			{
				this.SetStat(DrivenProperty.WeaponUnsteadyEndTime, value);
			}
		}

		public float WeaponRotationalAccuracyPenaltyInRadians
		{
			get
			{
				return this.GetStat(DrivenProperty.WeaponRotationalAccuracyPenaltyInRadians);
			}
			set
			{
				this.SetStat(DrivenProperty.WeaponRotationalAccuracyPenaltyInRadians, value);
			}
		}

		public float ArmorEncumbrance
		{
			get
			{
				return this.GetStat(DrivenProperty.ArmorEncumbrance);
			}
			set
			{
				this.SetStat(DrivenProperty.ArmorEncumbrance, value);
			}
		}

		public float WeaponsEncumbrance
		{
			get
			{
				return this.GetStat(DrivenProperty.WeaponsEncumbrance);
			}
			set
			{
				this.SetStat(DrivenProperty.WeaponsEncumbrance, value);
			}
		}

		public float ArmorHead
		{
			get
			{
				return this.GetStat(DrivenProperty.ArmorHead);
			}
			set
			{
				this.SetStat(DrivenProperty.ArmorHead, value);
			}
		}

		public float ArmorTorso
		{
			get
			{
				return this.GetStat(DrivenProperty.ArmorTorso);
			}
			set
			{
				this.SetStat(DrivenProperty.ArmorTorso, value);
			}
		}

		public float ArmorLegs
		{
			get
			{
				return this.GetStat(DrivenProperty.ArmorLegs);
			}
			set
			{
				this.SetStat(DrivenProperty.ArmorLegs, value);
			}
		}

		public float ArmorArms
		{
			get
			{
				return this.GetStat(DrivenProperty.ArmorArms);
			}
			set
			{
				this.SetStat(DrivenProperty.ArmorArms, value);
			}
		}

		public float AttributeRiding
		{
			get
			{
				return this.GetStat(DrivenProperty.AttributeRiding);
			}
			set
			{
				this.SetStat(DrivenProperty.AttributeRiding, value);
			}
		}

		public float AttributeShield
		{
			get
			{
				return this.GetStat(DrivenProperty.AttributeShield);
			}
			set
			{
				this.SetStat(DrivenProperty.AttributeShield, value);
			}
		}

		public float AttributeShieldMissileCollisionBodySizeAdder
		{
			get
			{
				return this.GetStat(DrivenProperty.AttributeShieldMissileCollisionBodySizeAdder);
			}
			set
			{
				this.SetStat(DrivenProperty.AttributeShieldMissileCollisionBodySizeAdder, value);
			}
		}

		public float ShieldBashStunDurationMultiplier
		{
			get
			{
				return this.GetStat(DrivenProperty.ShieldBashStunDurationMultiplier);
			}
			set
			{
				this.SetStat(DrivenProperty.ShieldBashStunDurationMultiplier, value);
			}
		}

		public float KickStunDurationMultiplier
		{
			get
			{
				return this.GetStat(DrivenProperty.KickStunDurationMultiplier);
			}
			set
			{
				this.SetStat(DrivenProperty.KickStunDurationMultiplier, value);
			}
		}

		public float ReloadMovementPenaltyFactor
		{
			get
			{
				return this.GetStat(DrivenProperty.ReloadMovementPenaltyFactor);
			}
			set
			{
				this.SetStat(DrivenProperty.ReloadMovementPenaltyFactor, value);
			}
		}

		public float TopSpeedReachDuration
		{
			get
			{
				return this.GetStat(DrivenProperty.TopSpeedReachDuration);
			}
			set
			{
				this.SetStat(DrivenProperty.TopSpeedReachDuration, value);
			}
		}

		public float MaxSpeedMultiplier
		{
			get
			{
				return this.GetStat(DrivenProperty.MaxSpeedMultiplier);
			}
			set
			{
				this.SetStat(DrivenProperty.MaxSpeedMultiplier, value);
			}
		}

		public float CombatMaxSpeedMultiplier
		{
			get
			{
				return this.GetStat(DrivenProperty.CombatMaxSpeedMultiplier);
			}
			set
			{
				this.SetStat(DrivenProperty.CombatMaxSpeedMultiplier, value);
			}
		}

		public float AttributeHorseArchery
		{
			get
			{
				return this.GetStat(DrivenProperty.AttributeHorseArchery);
			}
			set
			{
				this.SetStat(DrivenProperty.AttributeHorseArchery, value);
			}
		}

		public float AttributeCourage
		{
			get
			{
				return this.GetStat(DrivenProperty.AttributeCourage);
			}
			set
			{
				this.SetStat(DrivenProperty.AttributeCourage, value);
			}
		}

		public float MountManeuver
		{
			get
			{
				return this.GetStat(DrivenProperty.MountManeuver);
			}
			set
			{
				this.SetStat(DrivenProperty.MountManeuver, value);
			}
		}

		public float MountSpeed
		{
			get
			{
				return this.GetStat(DrivenProperty.MountSpeed);
			}
			set
			{
				this.SetStat(DrivenProperty.MountSpeed, value);
			}
		}

		public float MountDashAccelerationMultiplier
		{
			get
			{
				return this.GetStat(DrivenProperty.MountDashAccelerationMultiplier);
			}
			set
			{
				this.SetStat(DrivenProperty.MountDashAccelerationMultiplier, value);
			}
		}

		public float MountChargeDamage
		{
			get
			{
				return this.GetStat(DrivenProperty.MountChargeDamage);
			}
			set
			{
				this.SetStat(DrivenProperty.MountChargeDamage, value);
			}
		}

		public float MountDifficulty
		{
			get
			{
				return this.GetStat(DrivenProperty.MountDifficulty);
			}
			set
			{
				this.SetStat(DrivenProperty.MountDifficulty, value);
			}
		}

		public float BipedalRangedReadySpeedMultiplier
		{
			get
			{
				return this.GetStat(DrivenProperty.BipedalRangedReadySpeedMultiplier);
			}
			set
			{
				this.SetStat(DrivenProperty.BipedalRangedReadySpeedMultiplier, value);
			}
		}

		public float BipedalRangedReloadSpeedMultiplier
		{
			get
			{
				return this.GetStat(DrivenProperty.BipedalRangedReloadSpeedMultiplier);
			}
			set
			{
				this.SetStat(DrivenProperty.BipedalRangedReloadSpeedMultiplier, value);
			}
		}

		public float AiRangedHorsebackMissileRange
		{
			get
			{
				return this.GetStat(DrivenProperty.AiRangedHorsebackMissileRange);
			}
			set
			{
				this.SetStat(DrivenProperty.AiRangedHorsebackMissileRange, value);
			}
		}

		public float AiFacingMissileWatch
		{
			get
			{
				return this.GetStat(DrivenProperty.AiFacingMissileWatch);
			}
			set
			{
				this.SetStat(DrivenProperty.AiFacingMissileWatch, value);
			}
		}

		public float AiFlyingMissileCheckRadius
		{
			get
			{
				return this.GetStat(DrivenProperty.AiFlyingMissileCheckRadius);
			}
			set
			{
				this.SetStat(DrivenProperty.AiFlyingMissileCheckRadius, value);
			}
		}

		public float AiShootFreq
		{
			get
			{
				return this.GetStat(DrivenProperty.AiShootFreq);
			}
			set
			{
				this.SetStat(DrivenProperty.AiShootFreq, value);
			}
		}

		public float AiWaitBeforeShootFactor
		{
			get
			{
				return this.GetStat(DrivenProperty.AiWaitBeforeShootFactor);
			}
			set
			{
				this.SetStat(DrivenProperty.AiWaitBeforeShootFactor, value);
			}
		}

		public float AIBlockOnDecideAbility
		{
			get
			{
				return this.GetStat(DrivenProperty.AIBlockOnDecideAbility);
			}
			set
			{
				this.SetStat(DrivenProperty.AIBlockOnDecideAbility, value);
			}
		}

		public float AIParryOnDecideAbility
		{
			get
			{
				return this.GetStat(DrivenProperty.AIParryOnDecideAbility);
			}
			set
			{
				this.SetStat(DrivenProperty.AIParryOnDecideAbility, value);
			}
		}

		public float AiTryChamberAttackOnDecide
		{
			get
			{
				return this.GetStat(DrivenProperty.AiTryChamberAttackOnDecide);
			}
			set
			{
				this.SetStat(DrivenProperty.AiTryChamberAttackOnDecide, value);
			}
		}

		public float AIAttackOnParryChance
		{
			get
			{
				return this.GetStat(DrivenProperty.AIAttackOnParryChance);
			}
			set
			{
				this.SetStat(DrivenProperty.AIAttackOnParryChance, value);
			}
		}

		public float AiAttackOnParryTiming
		{
			get
			{
				return this.GetStat(DrivenProperty.AiAttackOnParryTiming);
			}
			set
			{
				this.SetStat(DrivenProperty.AiAttackOnParryTiming, value);
			}
		}

		public float AIDecideOnAttackChance
		{
			get
			{
				return this.GetStat(DrivenProperty.AIDecideOnAttackChance);
			}
			set
			{
				this.SetStat(DrivenProperty.AIDecideOnAttackChance, value);
			}
		}

		public float AIParryOnAttackAbility
		{
			get
			{
				return this.GetStat(DrivenProperty.AIParryOnAttackAbility);
			}
			set
			{
				this.SetStat(DrivenProperty.AIParryOnAttackAbility, value);
			}
		}

		public float AiKick
		{
			get
			{
				return this.GetStat(DrivenProperty.AiKick);
			}
			set
			{
				this.SetStat(DrivenProperty.AiKick, value);
			}
		}

		public float AiAttackCalculationMaxTimeFactor
		{
			get
			{
				return this.GetStat(DrivenProperty.AiAttackCalculationMaxTimeFactor);
			}
			set
			{
				this.SetStat(DrivenProperty.AiAttackCalculationMaxTimeFactor, value);
			}
		}

		public float AiDecideOnAttackWhenReceiveHitTiming
		{
			get
			{
				return this.GetStat(DrivenProperty.AiDecideOnAttackWhenReceiveHitTiming);
			}
			set
			{
				this.SetStat(DrivenProperty.AiDecideOnAttackWhenReceiveHitTiming, value);
			}
		}

		public float AiDecideOnAttackContinueAction
		{
			get
			{
				return this.GetStat(DrivenProperty.AiDecideOnAttackContinueAction);
			}
			set
			{
				this.SetStat(DrivenProperty.AiDecideOnAttackContinueAction, value);
			}
		}

		public float AiDecideOnAttackingContinue
		{
			get
			{
				return this.GetStat(DrivenProperty.AiDecideOnAttackingContinue);
			}
			set
			{
				this.SetStat(DrivenProperty.AiDecideOnAttackingContinue, value);
			}
		}

		public float AIParryOnAttackingContinueAbility
		{
			get
			{
				return this.GetStat(DrivenProperty.AIParryOnAttackingContinueAbility);
			}
			set
			{
				this.SetStat(DrivenProperty.AIParryOnAttackingContinueAbility, value);
			}
		}

		public float AIDecideOnRealizeEnemyBlockingAttackAbility
		{
			get
			{
				return this.GetStat(DrivenProperty.AIDecideOnRealizeEnemyBlockingAttackAbility);
			}
			set
			{
				this.SetStat(DrivenProperty.AIDecideOnRealizeEnemyBlockingAttackAbility, value);
			}
		}

		public float AIRealizeBlockingFromIncorrectSideAbility
		{
			get
			{
				return this.GetStat(DrivenProperty.AIRealizeBlockingFromIncorrectSideAbility);
			}
			set
			{
				this.SetStat(DrivenProperty.AIRealizeBlockingFromIncorrectSideAbility, value);
			}
		}

		public float AiAttackingShieldDefenseChance
		{
			get
			{
				return this.GetStat(DrivenProperty.AiAttackingShieldDefenseChance);
			}
			set
			{
				this.SetStat(DrivenProperty.AiAttackingShieldDefenseChance, value);
			}
		}

		public float AiAttackingShieldDefenseTimer
		{
			get
			{
				return this.GetStat(DrivenProperty.AiAttackingShieldDefenseTimer);
			}
			set
			{
				this.SetStat(DrivenProperty.AiAttackingShieldDefenseTimer, value);
			}
		}

		public float AiCheckMovementIntervalFactor
		{
			get
			{
				return this.GetStat(DrivenProperty.AiCheckMovementIntervalFactor);
			}
			set
			{
				this.SetStat(DrivenProperty.AiCheckMovementIntervalFactor, value);
			}
		}

		public float AiMovementDelayFactor
		{
			get
			{
				return this.GetStat(DrivenProperty.AiMovementDelayFactor);
			}
			set
			{
				this.SetStat(DrivenProperty.AiMovementDelayFactor, value);
			}
		}

		public float AiParryDecisionChangeValue
		{
			get
			{
				return this.GetStat(DrivenProperty.AiParryDecisionChangeValue);
			}
			set
			{
				this.SetStat(DrivenProperty.AiParryDecisionChangeValue, value);
			}
		}

		public float AiDefendWithShieldDecisionChanceValue
		{
			get
			{
				return this.GetStat(DrivenProperty.AiDefendWithShieldDecisionChanceValue);
			}
			set
			{
				this.SetStat(DrivenProperty.AiDefendWithShieldDecisionChanceValue, value);
			}
		}

		public float AiMoveEnemySideTimeValue
		{
			get
			{
				return this.GetStat(DrivenProperty.AiMoveEnemySideTimeValue);
			}
			set
			{
				this.SetStat(DrivenProperty.AiMoveEnemySideTimeValue, value);
			}
		}

		public float AiMinimumDistanceToContinueFactor
		{
			get
			{
				return this.GetStat(DrivenProperty.AiMinimumDistanceToContinueFactor);
			}
			set
			{
				this.SetStat(DrivenProperty.AiMinimumDistanceToContinueFactor, value);
			}
		}

		public float AiHearingDistanceFactor
		{
			get
			{
				return this.GetStat(DrivenProperty.AiHearingDistanceFactor);
			}
			set
			{
				this.SetStat(DrivenProperty.AiHearingDistanceFactor, value);
			}
		}

		public float AiChargeHorsebackTargetDistFactor
		{
			get
			{
				return this.GetStat(DrivenProperty.AiChargeHorsebackTargetDistFactor);
			}
			set
			{
				this.SetStat(DrivenProperty.AiChargeHorsebackTargetDistFactor, value);
			}
		}

		public float AiRangerLeadErrorMin
		{
			get
			{
				return this.GetStat(DrivenProperty.AiRangerLeadErrorMin);
			}
			set
			{
				this.SetStat(DrivenProperty.AiRangerLeadErrorMin, value);
			}
		}

		public float AiRangerLeadErrorMax
		{
			get
			{
				return this.GetStat(DrivenProperty.AiRangerLeadErrorMax);
			}
			set
			{
				this.SetStat(DrivenProperty.AiRangerLeadErrorMax, value);
			}
		}

		public float AiRangerVerticalErrorMultiplier
		{
			get
			{
				return this.GetStat(DrivenProperty.AiRangerVerticalErrorMultiplier);
			}
			set
			{
				this.SetStat(DrivenProperty.AiRangerVerticalErrorMultiplier, value);
			}
		}

		public float AiRangerHorizontalErrorMultiplier
		{
			get
			{
				return this.GetStat(DrivenProperty.AiRangerHorizontalErrorMultiplier);
			}
			set
			{
				this.SetStat(DrivenProperty.AiRangerHorizontalErrorMultiplier, value);
			}
		}

		public float AIAttackOnDecideChance
		{
			get
			{
				return this.GetStat(DrivenProperty.AIAttackOnDecideChance);
			}
			set
			{
				this.SetStat(DrivenProperty.AIAttackOnDecideChance, value);
			}
		}

		public float AiRaiseShieldDelayTimeBase
		{
			get
			{
				return this.GetStat(DrivenProperty.AiRaiseShieldDelayTimeBase);
			}
			set
			{
				this.SetStat(DrivenProperty.AiRaiseShieldDelayTimeBase, value);
			}
		}

		public float AiUseShieldAgainstEnemyMissileProbability
		{
			get
			{
				return this.GetStat(DrivenProperty.AiUseShieldAgainstEnemyMissileProbability);
			}
			set
			{
				this.SetStat(DrivenProperty.AiUseShieldAgainstEnemyMissileProbability, value);
			}
		}

		public int AiSpeciesIndex
		{
			get
			{
				return MathF.Round(this.GetStat(DrivenProperty.AiSpeciesIndex));
			}
			set
			{
				this.SetStat(DrivenProperty.AiSpeciesIndex, (float)value);
			}
		}

		public float AiRandomizedDefendDirectionChance
		{
			get
			{
				return this.GetStat(DrivenProperty.AiRandomizedDefendDirectionChance);
			}
			set
			{
				this.SetStat(DrivenProperty.AiRandomizedDefendDirectionChance, value);
			}
		}

		public float AiShooterError
		{
			get
			{
				return this.GetStat(DrivenProperty.AiShooterError);
			}
			set
			{
				this.SetStat(DrivenProperty.AiShooterError, value);
			}
		}

		public float AISetNoAttackTimerAfterBeingHitAbility
		{
			get
			{
				return this.GetStat(DrivenProperty.AISetNoAttackTimerAfterBeingHitAbility);
			}
			set
			{
				this.SetStat(DrivenProperty.AISetNoAttackTimerAfterBeingHitAbility, value);
			}
		}

		public float AISetNoAttackTimerAfterBeingParriedAbility
		{
			get
			{
				return this.GetStat(DrivenProperty.AISetNoAttackTimerAfterBeingParriedAbility);
			}
			set
			{
				this.SetStat(DrivenProperty.AISetNoAttackTimerAfterBeingParriedAbility, value);
			}
		}

		public float AISetNoDefendTimerAfterHittingAbility
		{
			get
			{
				return this.GetStat(DrivenProperty.AISetNoDefendTimerAfterHittingAbility);
			}
			set
			{
				this.SetStat(DrivenProperty.AISetNoDefendTimerAfterHittingAbility, value);
			}
		}

		public float AISetNoDefendTimerAfterParryingAbility
		{
			get
			{
				return this.GetStat(DrivenProperty.AISetNoDefendTimerAfterParryingAbility);
			}
			set
			{
				this.SetStat(DrivenProperty.AISetNoDefendTimerAfterParryingAbility, value);
			}
		}

		public float AIEstimateStunDurationPrecision
		{
			get
			{
				return this.GetStat(DrivenProperty.AIEstimateStunDurationPrecision);
			}
			set
			{
				this.SetStat(DrivenProperty.AIEstimateStunDurationPrecision, value);
			}
		}

		public float AIHoldingReadyMaxDuration
		{
			get
			{
				return this.GetStat(DrivenProperty.AIHoldingReadyMaxDuration);
			}
			set
			{
				this.SetStat(DrivenProperty.AIHoldingReadyMaxDuration, value);
			}
		}

		public float AIHoldingReadyVariationPercentage
		{
			get
			{
				return this.GetStat(DrivenProperty.AIHoldingReadyVariationPercentage);
			}
			set
			{
				this.SetStat(DrivenProperty.AIHoldingReadyVariationPercentage, value);
			}
		}

		internal float[] InitializeDrivenProperties(Agent agent, Equipment spawnEquipment, AgentBuildData agentBuildData)
		{
			MissionGameModels.Current.AgentStatCalculateModel.InitializeAgentStats(agent, spawnEquipment, this, agentBuildData);
			return this._statValues;
		}

		internal float[] UpdateDrivenProperties(Agent agent)
		{
			MissionGameModels.Current.AgentStatCalculateModel.UpdateAgentStats(agent, this);
			return this._statValues;
		}

		private readonly float[] _statValues;
	}
}
