using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public static class MissionCombatMechanicsHelper
	{
		public static bool DecideAgentShrugOffBlow(Agent victimAgent, AttackCollisionData collisionData, in Blow blow)
		{
			bool flag = false;
			if (victimAgent.Health - (float)collisionData.InflictedDamage >= 1f)
			{
				ManagedParametersEnum managedParametersEnum;
				if (blow.DamageType == DamageTypes.Cut)
				{
					managedParametersEnum = ManagedParametersEnum.DamageInterruptAttackThresholdCut;
				}
				else if (blow.DamageType == DamageTypes.Pierce)
				{
					managedParametersEnum = ManagedParametersEnum.DamageInterruptAttackThresholdPierce;
				}
				else
				{
					managedParametersEnum = ManagedParametersEnum.DamageInterruptAttackThresholdBlunt;
				}
				float num = MissionGameModels.Current.AgentApplyDamageModel.CalculateStaggerThresholdMultiplier(victimAgent);
				float num2 = ManagedParameters.Instance.GetManagedParameter(managedParametersEnum) * num;
				MPPerkObject.MPPerkHandler perkHandler = MPPerkObject.GetPerkHandler(victimAgent);
				float? num3 = ((perkHandler != null) ? new float?(perkHandler.GetDamageInterruptionThreshold()) : null);
				if (num3 != null && num3.Value > 0f)
				{
					num2 = num3.Value;
				}
				flag = (float)collisionData.InflictedDamage <= num2;
			}
			return flag;
		}

		public static bool DecideAgentDismountedByBlow(Agent attackerAgent, Agent victimAgent, in AttackCollisionData collisionData, WeaponComponentData attackerWeapon, in Blow blow)
		{
			bool flag = false;
			int inflictedDamage = collisionData.InflictedDamage;
			bool flag2 = victimAgent.Health - (float)inflictedDamage >= 1f;
			bool flag3 = (blow.BlowFlag & BlowFlags.ShrugOff) > BlowFlags.None;
			if (attackerWeapon != null && flag2 && !flag3)
			{
				int num = (int)victimAgent.HealthLimit;
				if (MissionGameModels.Current.AgentApplyDamageModel.CanWeaponDismount(attackerAgent, attackerWeapon, blow, collisionData))
				{
					float dismountPenetration = MissionGameModels.Current.AgentApplyDamageModel.GetDismountPenetration(attackerAgent, attackerWeapon, blow, collisionData);
					float dismountResistance = MissionGameModels.Current.AgentStatCalculateModel.GetDismountResistance(victimAgent);
					flag = MissionCombatMechanicsHelper.DecideCombatEffect((float)inflictedDamage, (float)num, dismountResistance, dismountPenetration);
				}
				if (!flag)
				{
					flag = MissionCombatMechanicsHelper.DecideWeaponKnockDown(attackerAgent, victimAgent, attackerWeapon, collisionData, blow);
				}
			}
			return flag;
		}

		public static bool DecideAgentKnockedBackByBlow(Agent attackerAgent, Agent victimAgent, in AttackCollisionData collisionData, WeaponComponentData attackerWeapon, in Blow blow)
		{
			bool flag = false;
			int num = (int)victimAgent.HealthLimit;
			int inflictedDamage = collisionData.InflictedDamage;
			bool flag2 = (blow.BlowFlag & BlowFlags.ShrugOff) > BlowFlags.None;
			AttackCollisionData attackCollisionData = collisionData;
			if (attackCollisionData.IsHorseCharge)
			{
				Vec3 position = victimAgent.Position;
				Vec2 movementDirection = attackerAgent.GetMovementDirection();
				attackCollisionData = collisionData;
				Vec3 collisionGlobalPosition = attackCollisionData.CollisionGlobalPosition;
				if (MissionCombatMechanicsHelper.ChargeDamageDotProduct(position, movementDirection, collisionGlobalPosition) >= 0.7f)
				{
					flag = true;
				}
			}
			else
			{
				attackCollisionData = collisionData;
				if (attackCollisionData.IsAlternativeAttack)
				{
					flag = true;
				}
				else if (attackerWeapon != null && !flag2 && MissionGameModels.Current.AgentApplyDamageModel.CanWeaponKnockback(attackerAgent, attackerWeapon, blow, collisionData))
				{
					float knockBackPenetration = MissionGameModels.Current.AgentApplyDamageModel.GetKnockBackPenetration(attackerAgent, attackerWeapon, blow, collisionData);
					float knockBackResistance = MissionGameModels.Current.AgentStatCalculateModel.GetKnockBackResistance(victimAgent);
					flag = MissionCombatMechanicsHelper.DecideCombatEffect((float)inflictedDamage, (float)num, knockBackResistance, knockBackPenetration);
				}
			}
			return flag;
		}

		public static bool DecideAgentKnockedDownByBlow(Agent attackerAgent, Agent victimAgent, in AttackCollisionData collisionData, WeaponComponentData attackerWeapon, in Blow blow)
		{
			bool flag = false;
			if ((blow.BlowFlag & BlowFlags.ShrugOff) <= BlowFlags.None)
			{
				int num = (int)victimAgent.HealthLimit;
				float num2 = (float)collisionData.InflictedDamage;
				bool flag2 = (blow.BlowFlag & BlowFlags.KnockBack) > BlowFlags.None;
				AttackCollisionData attackCollisionData = collisionData;
				if (attackCollisionData.IsHorseCharge && flag2)
				{
					float horseChargePenetration = MissionGameModels.Current.AgentApplyDamageModel.GetHorseChargePenetration();
					float knockDownResistance = MissionGameModels.Current.AgentStatCalculateModel.GetKnockDownResistance(victimAgent, StrikeType.Invalid);
					flag = MissionCombatMechanicsHelper.DecideCombatEffect(num2, (float)num, knockDownResistance, horseChargePenetration);
				}
				else if (attackerWeapon != null)
				{
					flag = MissionCombatMechanicsHelper.DecideWeaponKnockDown(attackerAgent, victimAgent, attackerWeapon, collisionData, blow);
				}
			}
			return flag;
		}

		public static bool DecideMountRearedByBlow(Agent attackerAgent, Agent victimAgent, in AttackCollisionData collisionData, WeaponComponentData attackerWeapon, in Blow blow)
		{
			float damageMultiplierOfCombatDifficulty = Mission.Current.GetDamageMultiplierOfCombatDifficulty(victimAgent, attackerAgent);
			if (attackerWeapon != null && attackerWeapon.WeaponFlags.HasAnyFlag(WeaponFlags.WideGrip) && attackerWeapon.WeaponLength > 120 && blow.StrikeType == StrikeType.Thrust)
			{
				AttackCollisionData attackCollisionData = collisionData;
				if (attackCollisionData.ThrustTipHit && attackerAgent != null && !attackerAgent.HasMount && victimAgent.GetAgentFlags().HasAnyFlag(AgentFlag.CanRear) && victimAgent.MovementVelocity.y > 5f && Vec3.DotProduct(blow.Direction, victimAgent.Frame.rotation.f) < -0.35f)
				{
					Vec3 globalPosition = blow.GlobalPosition;
					if (Vec2.DotProduct(globalPosition.AsVec2 - victimAgent.Position.AsVec2, victimAgent.GetMovementDirection()) > 0f)
					{
						return (float)collisionData.InflictedDamage >= ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.MakesRearAttackDamageThreshold) * damageMultiplierOfCombatDifficulty;
					}
				}
			}
			return false;
		}

		public static bool IsCollisionBoneDifferentThanWeaponAttachBone(in AttackCollisionData collisionData, int weaponAttachBoneIndex)
		{
			AttackCollisionData attackCollisionData = collisionData;
			if (attackCollisionData.AttackBoneIndex != -1 && weaponAttachBoneIndex != -1)
			{
				attackCollisionData = collisionData;
				return weaponAttachBoneIndex != (int)attackCollisionData.AttackBoneIndex;
			}
			return false;
		}

		public static bool DecideSweetSpotCollision(in AttackCollisionData collisionData)
		{
			AttackCollisionData attackCollisionData = collisionData;
			if (attackCollisionData.AttackProgress >= 0.22f)
			{
				attackCollisionData = collisionData;
				return attackCollisionData.AttackProgress <= 0.55f;
			}
			return false;
		}

		public static void GetAttackCollisionResults(in AttackInformation attackInformation, bool crushedThrough, float momentumRemaining, in MissionWeapon attackerWeapon, bool cancelDamage, ref AttackCollisionData attackCollisionData, out CombatLogData combatLog, out int speedBonus)
		{
			float num = 0f;
			if (attackCollisionData.IsMissile)
			{
				num = (attackCollisionData.MissileStartingPosition - attackCollisionData.CollisionGlobalPosition).Length;
			}
			combatLog = new CombatLogData(attackInformation.IsVictimAgentSameWithAttackerAgent, attackInformation.IsAttackerAgentHuman, attackInformation.IsAttackerAgentMine, attackInformation.DoesAttackerHaveRiderAgent, attackInformation.IsAttackerAgentRiderAgentMine, attackInformation.IsAttackerAgentMount, attackInformation.IsVictimAgentHuman, attackInformation.IsVictimAgentMine, false, attackInformation.DoesVictimHaveRiderAgent, attackInformation.IsVictimAgentRiderAgentMine, attackInformation.IsVictimAgentMount, false, attackInformation.IsVictimRiderAgentSameAsAttackerAgent, false, false, num);
			bool flag = MissionCombatMechanicsHelper.IsCollisionBoneDifferentThanWeaponAttachBone(attackCollisionData, attackInformation.WeaponAttachBoneIndex);
			Vec2 agentVelocityContribution = MissionCombatMechanicsHelper.GetAgentVelocityContribution(attackInformation.DoesAttackerHaveMountAgent, attackInformation.AttackerAgentMovementVelocity, attackInformation.AttackerAgentMountMovementDirection, attackInformation.AttackerMovementDirectionAsAngle);
			Vec2 agentVelocityContribution2 = MissionCombatMechanicsHelper.GetAgentVelocityContribution(attackInformation.DoesVictimHaveMountAgent, attackInformation.VictimAgentMovementVelocity, attackInformation.VictimAgentMountMovementDirection, attackInformation.VictimMovementDirectionAsAngle);
			if (attackCollisionData.IsColliderAgent)
			{
				combatLog.IsRangedAttack = attackCollisionData.IsMissile;
				combatLog.HitSpeed = (attackCollisionData.IsMissile ? (agentVelocityContribution2.ToVec3(0f) - attackCollisionData.MissileVelocity).Length : (agentVelocityContribution - agentVelocityContribution2).Length);
			}
			float baseMagnitude;
			MissionCombatMechanicsHelper.ComputeBlowMagnitude(attackCollisionData, attackInformation, attackerWeapon, momentumRemaining, cancelDamage, flag, agentVelocityContribution, agentVelocityContribution2, out attackCollisionData.BaseMagnitude, out baseMagnitude, out attackCollisionData.MovementSpeedDamageModifier, out speedBonus);
			MissionWeapon missionWeapon = attackerWeapon;
			DamageTypes damageTypes = (DamageTypes)((missionWeapon.IsEmpty || flag || attackCollisionData.IsAlternativeAttack || attackCollisionData.IsFallDamage || attackCollisionData.IsHorseCharge) ? 2 : attackCollisionData.DamageType);
			combatLog.DamageType = damageTypes;
			if (!attackCollisionData.IsColliderAgent && attackCollisionData.EntityExists)
			{
				string name = PhysicsMaterial.GetFromIndex(attackCollisionData.PhysicsMaterialIndex).Name;
				bool flag2 = name == "wood" || name == "wood_weapon" || name == "wood_shield";
				float baseMagnitude2 = attackCollisionData.BaseMagnitude;
				bool isAttackerAgentDoingPassiveAttack = attackInformation.IsAttackerAgentDoingPassiveAttack;
				missionWeapon = attackerWeapon;
				attackCollisionData.BaseMagnitude = baseMagnitude2 * MissionCombatMechanicsHelper.GetEntityDamageMultiplier(isAttackerAgentDoingPassiveAttack, missionWeapon.CurrentUsageItem, damageTypes, flag2);
				attackCollisionData.InflictedDamage = MBMath.ClampInt((int)attackCollisionData.BaseMagnitude, 0, 2000);
				combatLog.InflictedDamage = attackCollisionData.InflictedDamage;
			}
			if (attackCollisionData.IsColliderAgent && !attackInformation.IsVictimAgentNull)
			{
				if (attackCollisionData.IsAlternativeAttack)
				{
					baseMagnitude = attackCollisionData.BaseMagnitude;
				}
				if (attackCollisionData.AttackBlockedWithShield)
				{
					missionWeapon = attackerWeapon;
					MissionCombatMechanicsHelper.ComputeBlowDamageOnShield(attackInformation, attackCollisionData, missionWeapon.CurrentUsageItem, attackCollisionData.BaseMagnitude, out attackCollisionData.InflictedDamage);
					attackCollisionData.AbsorbedByArmor = attackCollisionData.InflictedDamage;
				}
				else if (attackCollisionData.MissileBlockedWithWeapon)
				{
					attackCollisionData.InflictedDamage = 0;
					attackCollisionData.AbsorbedByArmor = 0;
				}
				else
				{
					missionWeapon = attackerWeapon;
					MissionCombatMechanicsHelper.ComputeBlowDamage(attackInformation, attackCollisionData, missionWeapon.CurrentUsageItem, damageTypes, baseMagnitude, speedBonus, cancelDamage, out attackCollisionData.InflictedDamage, out attackCollisionData.AbsorbedByArmor);
				}
				combatLog.InflictedDamage = attackCollisionData.InflictedDamage;
				combatLog.AbsorbedDamage = attackCollisionData.AbsorbedByArmor;
				combatLog.AttackProgress = attackCollisionData.AttackProgress;
			}
		}

		internal static void GetDefendCollisionResults(Agent attackerAgent, Agent defenderAgent, CombatCollisionResult collisionResult, int attackerWeaponSlotIndex, bool isAlternativeAttack, StrikeType strikeType, Agent.UsageDirection attackDirection, float collisionDistanceOnWeapon, float attackProgress, bool attackIsParried, bool isPassiveUsageHit, bool isHeavyAttack, ref float defenderStunPeriod, ref float attackerStunPeriod, ref bool crushedThrough, ref bool chamber)
		{
			MissionWeapon missionWeapon = ((attackerWeaponSlotIndex >= 0) ? attackerAgent.Equipment[attackerWeaponSlotIndex] : MissionWeapon.Invalid);
			WeaponComponentData weaponComponentData = (missionWeapon.IsEmpty ? null : missionWeapon.CurrentUsageItem);
			EquipmentIndex equipmentIndex = defenderAgent.GetWieldedItemIndex(Agent.HandIndex.OffHand);
			if (equipmentIndex == EquipmentIndex.None)
			{
				equipmentIndex = defenderAgent.GetWieldedItemIndex(Agent.HandIndex.MainHand);
			}
			ItemObject itemObject = ((equipmentIndex != EquipmentIndex.None) ? defenderAgent.Equipment[equipmentIndex].Item : null);
			WeaponComponentData weaponComponentData2 = ((equipmentIndex != EquipmentIndex.None) ? defenderAgent.Equipment[equipmentIndex].CurrentUsageItem : null);
			float num = 10f;
			attackerStunPeriod = ((strikeType == StrikeType.Thrust) ? ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.StunPeriodAttackerThrust) : ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.StunPeriodAttackerSwing));
			chamber = false;
			if (!missionWeapon.IsEmpty)
			{
				float z = attackerAgent.GetCurWeaponOffset().z;
				float realWeaponLength = weaponComponentData.GetRealWeaponLength();
				float num2 = realWeaponLength + z;
				float num3 = MBMath.ClampFloat((0.2f + collisionDistanceOnWeapon) / num2, 0.1f, 0.98f);
				float num4 = MissionCombatMechanicsHelper.ComputeRelativeSpeedDiffOfAgents(attackerAgent, defenderAgent);
				float num5;
				if (strikeType == StrikeType.Thrust)
				{
					num5 = CombatStatCalculator.CalculateBaseBlowMagnitudeForThrust((float)missionWeapon.GetModifiedThrustSpeedForCurrentUsage() / 11.764706f * MissionCombatMechanicsHelper.SpeedGraphFunction(attackProgress, strikeType, attackDirection), missionWeapon.Item.Weight, num4);
				}
				else
				{
					num5 = CombatStatCalculator.CalculateBaseBlowMagnitudeForSwing((float)missionWeapon.GetModifiedSwingSpeedForCurrentUsage() / 4.5454545f * MissionCombatMechanicsHelper.SpeedGraphFunction(attackProgress, strikeType, attackDirection), realWeaponLength, missionWeapon.Item.Weight, weaponComponentData.Inertia, weaponComponentData.CenterOfMass, num3, num4);
				}
				if (strikeType == StrikeType.Thrust)
				{
					num5 *= 0.8f;
				}
				else if (attackDirection == Agent.UsageDirection.AttackUp)
				{
					num5 *= 1.25f;
				}
				else if (isHeavyAttack)
				{
					num5 *= ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.HeavyAttackMomentumMultiplier);
				}
				num += num5;
			}
			float num6 = 1f;
			defenderStunPeriod = num * ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.StunMomentumTransferFactor);
			if (weaponComponentData2 != null)
			{
				if (weaponComponentData2.IsShield)
				{
					float managedParameter = ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.StunDefendWeaponWeightOffsetShield);
					num6 += managedParameter * itemObject.Weight;
				}
				else
				{
					num6 = 0.9f;
					float num7 = ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.StunDefendWeaponWeightMultiplierWeaponWeight);
					num6 += num7 * itemObject.Weight;
					ItemObject.ItemTypeEnum itemType = itemObject.ItemType;
					if (itemType == ItemObject.ItemTypeEnum.TwoHandedWeapon)
					{
						num7 = ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.StunDefendWeaponWeightBonusTwoHanded);
					}
					else if (itemType == ItemObject.ItemTypeEnum.Polearm)
					{
						num6 += ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.StunDefendWeaponWeightBonusPolearm);
					}
				}
				if (collisionResult == CombatCollisionResult.Parried)
				{
					attackerStunPeriod += MathF.Min(0.15f, 0.12f * num6);
					num6 += ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.StunDefendWeaponWeightBonusActiveBlocked);
				}
				else if (collisionResult == CombatCollisionResult.ChamberBlocked)
				{
					attackerStunPeriod += MathF.Min(0.25f, 0.25f * num6);
					num6 += ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.StunDefendWeaponWeightBonusChamberBlocked);
					chamber = true;
				}
			}
			if (!defenderAgent.GetIsLeftStance())
			{
				num6 += ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.StunDefendWeaponWeightBonusRightStance);
			}
			defenderStunPeriod /= num6;
			float num8;
			float num9;
			MissionGameModels.Current.AgentApplyDamageModel.CalculateDefendedBlowStunMultipliers(attackerAgent, defenderAgent, collisionResult, weaponComponentData, weaponComponentData2, out num8, out num9);
			attackerStunPeriod *= num8;
			defenderStunPeriod *= num9;
			float managedParameter2 = ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.StunPeriodMax);
			attackerStunPeriod = MathF.Min(attackerStunPeriod, managedParameter2);
			defenderStunPeriod = MathF.Min(defenderStunPeriod, managedParameter2);
			crushedThrough = !chamber && MissionGameModels.Current.AgentApplyDamageModel.DecideCrushedThrough(attackerAgent, defenderAgent, num, attackDirection, strikeType, weaponComponentData2, isPassiveUsageHit);
		}

		private static bool DecideWeaponKnockDown(Agent attackerAgent, Agent victimAgent, WeaponComponentData attackerWeapon, in AttackCollisionData collisionData, in Blow blow)
		{
			if (MissionGameModels.Current.AgentApplyDamageModel.CanWeaponKnockDown(attackerAgent, victimAgent, attackerWeapon, blow, collisionData))
			{
				float knockDownPenetration = MissionGameModels.Current.AgentApplyDamageModel.GetKnockDownPenetration(attackerAgent, attackerWeapon, blow, collisionData);
				float knockDownResistance = MissionGameModels.Current.AgentStatCalculateModel.GetKnockDownResistance(victimAgent, blow.StrikeType);
				return MissionCombatMechanicsHelper.DecideCombatEffect((float)collisionData.InflictedDamage, victimAgent.HealthLimit, knockDownResistance, knockDownPenetration);
			}
			return false;
		}

		private static bool DecideCombatEffect(float inflictedDamage, float victimMaxHealth, float victimResistance, float attackPenetration)
		{
			float num = victimMaxHealth * Math.Max(0f, victimResistance - attackPenetration);
			return inflictedDamage >= num;
		}

		private static float ChargeDamageDotProduct(in Vec3 victimPosition, in Vec2 chargerMovementDirection, in Vec3 collisionPoint)
		{
			Vec3 vec = victimPosition;
			Vec2 asVec = vec.AsVec2;
			vec = collisionPoint;
			float num = Vec2.DotProduct((asVec - vec.AsVec2).Normalized(), chargerMovementDirection);
			return MathF.Max(0f, num);
		}

		private static float SpeedGraphFunction(float progress, StrikeType strikeType, Agent.UsageDirection attackDir)
		{
			bool flag = strikeType == StrikeType.Thrust;
			bool flag2 = attackDir == Agent.UsageDirection.AttackUp;
			ManagedParametersEnum managedParametersEnum;
			ManagedParametersEnum managedParametersEnum2;
			ManagedParametersEnum managedParametersEnum3;
			ManagedParametersEnum managedParametersEnum4;
			if (flag)
			{
				managedParametersEnum = ManagedParametersEnum.ThrustCombatSpeedGraphZeroProgressValue;
				managedParametersEnum2 = ManagedParametersEnum.ThrustCombatSpeedGraphFirstMaximumPoint;
				managedParametersEnum3 = ManagedParametersEnum.ThrustCombatSpeedGraphSecondMaximumPoint;
				managedParametersEnum4 = ManagedParametersEnum.ThrustCombatSpeedGraphOneProgressValue;
			}
			else if (flag2)
			{
				managedParametersEnum = ManagedParametersEnum.OverSwingCombatSpeedGraphZeroProgressValue;
				managedParametersEnum2 = ManagedParametersEnum.OverSwingCombatSpeedGraphFirstMaximumPoint;
				managedParametersEnum3 = ManagedParametersEnum.OverSwingCombatSpeedGraphSecondMaximumPoint;
				managedParametersEnum4 = ManagedParametersEnum.OverSwingCombatSpeedGraphOneProgressValue;
			}
			else
			{
				managedParametersEnum = ManagedParametersEnum.SwingCombatSpeedGraphZeroProgressValue;
				managedParametersEnum2 = ManagedParametersEnum.SwingCombatSpeedGraphFirstMaximumPoint;
				managedParametersEnum3 = ManagedParametersEnum.SwingCombatSpeedGraphSecondMaximumPoint;
				managedParametersEnum4 = ManagedParametersEnum.SwingCombatSpeedGraphOneProgressValue;
			}
			float managedParameter = ManagedParameters.Instance.GetManagedParameter(managedParametersEnum);
			float managedParameter2 = ManagedParameters.Instance.GetManagedParameter(managedParametersEnum2);
			float managedParameter3 = ManagedParameters.Instance.GetManagedParameter(managedParametersEnum3);
			float managedParameter4 = ManagedParameters.Instance.GetManagedParameter(managedParametersEnum4);
			float num;
			if (progress < managedParameter2)
			{
				num = (1f - managedParameter) / managedParameter2 * progress + managedParameter;
			}
			else if (managedParameter3 < progress)
			{
				num = (managedParameter4 - 1f) / (1f - managedParameter3) * (progress - managedParameter3) + 1f;
			}
			else
			{
				num = 1f;
			}
			return num;
		}

		private static float ConvertBaseAttackMagnitude(WeaponComponentData weapon, StrikeType strikeType, float baseMagnitude)
		{
			return baseMagnitude * ((strikeType == StrikeType.Thrust) ? weapon.ThrustDamageFactor : weapon.SwingDamageFactor);
		}

		private static Vec2 GetAgentVelocityContribution(bool hasAgentMountAgent, Vec2 agentMovementVelocity, Vec2 agentMountMovementDirection, float agentMovementDirectionAsAngle)
		{
			Vec2 vec = Vec2.Zero;
			if (hasAgentMountAgent)
			{
				vec = agentMovementVelocity.y * agentMountMovementDirection;
			}
			else
			{
				vec = agentMovementVelocity;
				vec.RotateCCW(agentMovementDirectionAsAngle);
			}
			return vec;
		}

		private static float GetEntityDamageMultiplier(bool isAttackerAgentDoingPassiveAttack, WeaponComponentData weapon, DamageTypes damageType, bool isWoodenBody)
		{
			float num = 1f;
			if (isAttackerAgentDoingPassiveAttack)
			{
				num *= 0.2f;
			}
			if (weapon != null)
			{
				if (weapon.WeaponFlags.HasAnyFlag(WeaponFlags.BonusAgainstShield))
				{
					num *= 1.2f;
				}
				switch (damageType)
				{
				case DamageTypes.Cut:
					num *= 0.8f;
					break;
				case DamageTypes.Pierce:
					num *= 0.1f;
					break;
				}
				if (isWoodenBody && weapon.WeaponFlags.HasAnyFlag(WeaponFlags.Burning))
				{
					num *= 1.5f;
				}
			}
			return num;
		}

		private static float ComputeSpeedBonus(float baseMagnitude, float baseMagnitudeWithoutSpeedBonus)
		{
			return baseMagnitude / baseMagnitudeWithoutSpeedBonus - 1f;
		}

		private static float ComputeRelativeSpeedDiffOfAgents(Agent agentA, Agent agentB)
		{
			Vec2 vec = Vec2.Zero;
			if (agentA.MountAgent != null)
			{
				vec = agentA.MountAgent.MovementVelocity.y * agentA.MountAgent.GetMovementDirection();
			}
			else
			{
				vec = agentA.MovementVelocity;
				vec.RotateCCW(agentA.MovementDirectionAsAngle);
			}
			Vec2 vec2 = Vec2.Zero;
			if (agentB.MountAgent != null)
			{
				vec2 = agentB.MountAgent.MovementVelocity.y * agentB.MountAgent.GetMovementDirection();
			}
			else
			{
				vec2 = agentB.MovementVelocity;
				vec2.RotateCCW(agentB.MovementDirectionAsAngle);
			}
			return (vec - vec2).Length;
		}

		private static void ComputeBlowDamage(in AttackInformation attackInformation, in AttackCollisionData attackCollisionData, WeaponComponentData attackerWeapon, DamageTypes damageType, float magnitude, int speedBonus, bool cancelDamage, out int inflictedDamage, out int absorbedByArmor)
		{
			float armorAmountFloat = attackInformation.ArmorAmountFloat;
			WeaponComponentData shieldOnBack = attackInformation.ShieldOnBack;
			AgentFlag victimAgentFlag = attackInformation.VictimAgentFlag;
			float victimAgentAbsorbedDamageRatio = attackInformation.VictimAgentAbsorbedDamageRatio;
			float damageMultiplierOfBone = attackInformation.DamageMultiplierOfBone;
			float combatDifficultyMultiplier = attackInformation.CombatDifficultyMultiplier;
			AttackCollisionData attackCollisionData2 = attackCollisionData;
			Vec3 collisionGlobalPosition = attackCollisionData2.CollisionGlobalPosition;
			attackCollisionData2 = attackCollisionData;
			bool attackBlockedWithShield = attackCollisionData2.AttackBlockedWithShield;
			attackCollisionData2 = attackCollisionData;
			bool collidedWithShieldOnBack = attackCollisionData2.CollidedWithShieldOnBack;
			attackCollisionData2 = attackCollisionData;
			bool isFallDamage = attackCollisionData2.IsFallDamage;
			BasicCharacterObject attackerAgentCharacter = attackInformation.AttackerAgentCharacter;
			BasicCharacterObject attackerCaptainCharacter = attackInformation.AttackerCaptainCharacter;
			BasicCharacterObject victimAgentCharacter = attackInformation.VictimAgentCharacter;
			BasicCharacterObject victimCaptainCharacter = attackInformation.VictimCaptainCharacter;
			float num = 0f;
			if (!isFallDamage)
			{
				num = Game.Current.BasicModels.StrikeMagnitudeModel.CalculateAdjustedArmorForBlow(armorAmountFloat, attackerAgentCharacter, attackerCaptainCharacter, victimAgentCharacter, victimCaptainCharacter, attackerWeapon);
			}
			if (collidedWithShieldOnBack && shieldOnBack != null)
			{
				num += 10f;
			}
			float num2 = victimAgentAbsorbedDamageRatio;
			float num3 = Game.Current.BasicModels.StrikeMagnitudeModel.ComputeRawDamage(damageType, magnitude, num, num2);
			float num4 = 1f;
			if (!attackBlockedWithShield && !isFallDamage)
			{
				num4 *= damageMultiplierOfBone;
				num4 *= combatDifficultyMultiplier;
			}
			num3 *= num4;
			inflictedDamage = MBMath.ClampInt(MathF.Ceiling(num3), 0, 2000);
			int num5 = MBMath.ClampInt(MathF.Ceiling(Game.Current.BasicModels.StrikeMagnitudeModel.ComputeRawDamage(damageType, magnitude, 0f, num2) * num4), 0, 2000);
			absorbedByArmor = num5 - inflictedDamage;
		}

		private static void ComputeBlowDamageOnShield(in AttackInformation attackInformation, in AttackCollisionData attackCollisionData, WeaponComponentData attackerWeapon, float blowMagnitude, out int inflictedDamage)
		{
			inflictedDamage = 0;
			MissionWeapon victimShield = attackInformation.VictimShield;
			if (victimShield.CurrentUsageItem.WeaponFlags.HasAnyFlag(WeaponFlags.CanBlockRanged) && attackInformation.CanGiveDamageToAgentShield)
			{
				AttackCollisionData attackCollisionData2 = attackCollisionData;
				DamageTypes damageType = (DamageTypes)attackCollisionData2.DamageType;
				int getModifiedArmorForCurrentUsage = victimShield.GetGetModifiedArmorForCurrentUsage();
				float num = 1f;
				float num2 = Game.Current.BasicModels.StrikeMagnitudeModel.ComputeRawDamage(damageType, blowMagnitude, (float)getModifiedArmorForCurrentUsage, num);
				attackCollisionData2 = attackCollisionData;
				if (attackCollisionData2.IsMissile)
				{
					if (attackerWeapon.WeaponClass == WeaponClass.ThrowingAxe)
					{
						num2 *= 0.3f;
					}
					else if (attackerWeapon.WeaponClass == WeaponClass.Javelin)
					{
						num2 *= 0.5f;
					}
					else if (attackerWeapon.WeaponFlags.HasAnyFlag(WeaponFlags.CanPenetrateShield) && attackerWeapon.WeaponFlags.HasAnyFlag(WeaponFlags.MultiplePenetration))
					{
						num2 *= 0.5f;
					}
					else
					{
						num2 *= 0.15f;
					}
				}
				else
				{
					attackCollisionData2 = attackCollisionData;
					switch (attackCollisionData2.DamageType)
					{
					case 0:
					case 2:
						num2 *= 0.7f;
						break;
					case 1:
						num2 *= 0.5f;
						break;
					}
				}
				if (attackerWeapon != null && attackerWeapon.WeaponFlags.HasAnyFlag(WeaponFlags.BonusAgainstShield))
				{
					num2 *= 2f;
				}
				if (num2 > 0f)
				{
					if (!attackInformation.IsVictimAgentLeftStance)
					{
						num2 *= ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.ShieldRightStanceBlockDamageMultiplier);
					}
					attackCollisionData2 = attackCollisionData;
					if (attackCollisionData2.CorrectSideShieldBlock)
					{
						num2 *= ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.ShieldCorrectSideBlockDamageMultiplier);
					}
					num2 = MissionGameModels.Current.AgentApplyDamageModel.CalculateShieldDamage(attackInformation, num2);
					inflictedDamage = (int)num2;
				}
			}
		}

		public static float CalculateBaseMeleeBlowMagnitude(in AttackInformation attackInformation, in MissionWeapon weapon, StrikeType strikeType, float progressEffect, float impactPointAsPercent, float exraLinearSpeed, bool doesAttackerHaveMount)
		{
			MissionWeapon missionWeapon = weapon;
			WeaponComponentData currentUsageItem = missionWeapon.CurrentUsageItem;
			BasicCharacterObject attackerAgentCharacter = attackInformation.AttackerAgentCharacter;
			BasicCharacterObject attackerCaptainCharacter = attackInformation.AttackerCaptainCharacter;
			float num = MathF.Sqrt(progressEffect);
			float num4;
			if (strikeType == StrikeType.Thrust)
			{
				exraLinearSpeed *= 0.5f;
				missionWeapon = weapon;
				float num2 = (float)missionWeapon.GetModifiedThrustSpeedForCurrentUsage() / 11.764706f * num;
				StrikeMagnitudeCalculationModel strikeMagnitudeModel = Game.Current.BasicModels.StrikeMagnitudeModel;
				BasicCharacterObject basicCharacterObject = attackerAgentCharacter;
				BasicCharacterObject basicCharacterObject2 = attackerCaptainCharacter;
				float num3 = num2;
				missionWeapon = weapon;
				float weight = missionWeapon.Item.Weight;
				missionWeapon = weapon;
				num4 = strikeMagnitudeModel.CalculateStrikeMagnitudeForThrust(basicCharacterObject, basicCharacterObject2, num3, weight, missionWeapon.Item, currentUsageItem, exraLinearSpeed, doesAttackerHaveMount, false);
			}
			else
			{
				exraLinearSpeed *= 0.7f;
				missionWeapon = weapon;
				float num5 = (float)missionWeapon.GetModifiedSwingSpeedForCurrentUsage() / 4.5454545f * num;
				float num6 = MBMath.ClampFloat(0.4f / currentUsageItem.GetRealWeaponLength(), 0f, 1f);
				float num7 = MathF.Min(0.93f, impactPointAsPercent);
				float num8 = MathF.Min(0.93f, impactPointAsPercent + num6);
				float num9 = 0f;
				for (int i = 0; i < 5; i++)
				{
					float num10 = num7 + (float)i / 4f * (num8 - num7);
					StrikeMagnitudeCalculationModel strikeMagnitudeModel2 = Game.Current.BasicModels.StrikeMagnitudeModel;
					BasicCharacterObject basicCharacterObject3 = attackerAgentCharacter;
					BasicCharacterObject basicCharacterObject4 = attackerCaptainCharacter;
					float num11 = num5;
					float num12 = num10;
					missionWeapon = weapon;
					float weight2 = missionWeapon.Item.Weight;
					missionWeapon = weapon;
					float num13 = strikeMagnitudeModel2.CalculateStrikeMagnitudeForSwing(basicCharacterObject3, basicCharacterObject4, num11, num12, weight2, missionWeapon.Item, currentUsageItem, currentUsageItem.GetRealWeaponLength(), currentUsageItem.Inertia, currentUsageItem.CenterOfMass, exraLinearSpeed, doesAttackerHaveMount);
					if (num9 < num13)
					{
						num9 = num13;
					}
				}
				num4 = num9;
			}
			return num4;
		}

		private static void ComputeBlowMagnitude(in AttackCollisionData acd, in AttackInformation attackInformation, MissionWeapon weapon, float momentumRemaining, bool cancelDamage, bool hitWithAnotherBone, Vec2 attackerVelocity, Vec2 victimVelocity, out float baseMagnitude, out float specialMagnitude, out float movementSpeedDamageModifier, out int speedBonusInt)
		{
			AttackCollisionData attackCollisionData = acd;
			StrikeType strikeType = (StrikeType)attackCollisionData.StrikeType;
			attackCollisionData = acd;
			Agent.UsageDirection attackDirection = attackCollisionData.AttackDirection;
			bool flag = !attackInformation.IsAttackerAgentNull && attackInformation.IsAttackerAgentHuman && attackInformation.IsAttackerAgentActive && attackInformation.IsAttackerAgentDoingPassiveAttack;
			movementSpeedDamageModifier = 0f;
			speedBonusInt = 0;
			attackCollisionData = acd;
			if (attackCollisionData.IsMissile)
			{
				MissionCombatMechanicsHelper.ComputeBlowMagnitudeMissile(attackInformation, acd, weapon, momentumRemaining, victimVelocity, out baseMagnitude, out specialMagnitude);
			}
			else
			{
				attackCollisionData = acd;
				if (attackCollisionData.IsFallDamage)
				{
					MissionCombatMechanicsHelper.ComputeBlowMagnitudeFromFall(attackInformation, acd, out baseMagnitude, out specialMagnitude);
				}
				else
				{
					attackCollisionData = acd;
					if (attackCollisionData.IsHorseCharge)
					{
						MissionCombatMechanicsHelper.ComputeBlowMagnitudeFromHorseCharge(attackInformation, acd, attackerVelocity, victimVelocity, out baseMagnitude, out specialMagnitude);
					}
					else
					{
						MissionCombatMechanicsHelper.ComputeBlowMagnitudeMelee(attackInformation, acd, momentumRemaining, cancelDamage, hitWithAnotherBone, strikeType, attackDirection, weapon, flag, attackerVelocity, victimVelocity, out baseMagnitude, out specialMagnitude, out movementSpeedDamageModifier, out speedBonusInt);
					}
				}
			}
			specialMagnitude = MBMath.ClampFloat(specialMagnitude, 0f, 500f);
		}

		private static void ComputeBlowMagnitudeMelee(in AttackInformation attackInformation, in AttackCollisionData acd, float momentumRemaining, bool cancelDamage, bool hitWithAnotherBone, StrikeType strikeType, Agent.UsageDirection attackDirection, in MissionWeapon weapon, bool attackerIsDoingPassiveAttack, Vec2 attackerVelocity, Vec2 victimVelocity, out float baseMagnitude, out float specialMagnitude, out float movementSpeedDamageModifier, out int speedBonusInt)
		{
			Vec3 attackerAgentCurrentWeaponOffset = attackInformation.AttackerAgentCurrentWeaponOffset;
			movementSpeedDamageModifier = 0f;
			speedBonusInt = 0;
			specialMagnitude = 0f;
			baseMagnitude = 0f;
			BasicCharacterObject attackerAgentCharacter = attackInformation.AttackerAgentCharacter;
			AttackCollisionData attackCollisionData = acd;
			MissionWeapon missionWeapon;
			if (attackCollisionData.IsAlternativeAttack)
			{
				missionWeapon = weapon;
				WeaponComponentData currentUsageItem = missionWeapon.CurrentUsageItem;
				baseMagnitude = MissionGameModels.Current.AgentApplyDamageModel.CalculateAlternativeAttackDamage(attackerAgentCharacter, currentUsageItem);
				baseMagnitude *= momentumRemaining;
				specialMagnitude = baseMagnitude;
				return;
			}
			attackCollisionData = acd;
			Vec3 weaponBlowDir = attackCollisionData.WeaponBlowDir;
			Vec2 vec = attackerVelocity - victimVelocity;
			float num = vec.Normalize();
			float num2 = Vec2.DotProduct(weaponBlowDir.AsVec2, vec);
			if (num2 > 0f)
			{
				num2 += 0.2f;
				num2 = MathF.Min(num2, 1f);
			}
			float num3 = num * num2;
			missionWeapon = weapon;
			if (missionWeapon.IsEmpty)
			{
				attackCollisionData = acd;
				baseMagnitude = MissionCombatMechanicsHelper.SpeedGraphFunction(attackCollisionData.AttackProgress, strikeType, attackDirection) * momentumRemaining * ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.FistFightDamageMultiplier);
				specialMagnitude = baseMagnitude;
				return;
			}
			float z = attackerAgentCurrentWeaponOffset.z;
			missionWeapon = weapon;
			WeaponComponentData currentUsageItem2 = missionWeapon.CurrentUsageItem;
			float num4 = currentUsageItem2.GetRealWeaponLength() + z;
			attackCollisionData = acd;
			float num5 = MBMath.ClampFloat(attackCollisionData.CollisionDistanceOnWeapon, -0.2f, num4) / num4;
			bool doesAttackerHaveMountAgent = attackInformation.DoesAttackerHaveMountAgent;
			if (attackerIsDoingPassiveAttack)
			{
				if (!doesAttackerHaveMountAgent && !attackInformation.DoesVictimHaveMountAgent && !attackInformation.IsVictimAgentMount)
				{
					baseMagnitude = 0f;
				}
				else
				{
					missionWeapon = weapon;
					baseMagnitude = CombatStatCalculator.CalculateBaseBlowMagnitudeForPassiveUsage(missionWeapon.Item.Weight, num3);
				}
				baseMagnitude = MissionGameModels.Current.AgentApplyDamageModel.CalculatePassiveAttackDamage(attackerAgentCharacter, acd, baseMagnitude);
			}
			else
			{
				attackCollisionData = acd;
				float num6 = MissionCombatMechanicsHelper.SpeedGraphFunction(attackCollisionData.AttackProgress, strikeType, attackDirection);
				baseMagnitude = MissionCombatMechanicsHelper.CalculateBaseMeleeBlowMagnitude(attackInformation, weapon, strikeType, num6, num5, num3, doesAttackerHaveMountAgent);
				if (baseMagnitude >= 0f && num6 > 0.7f)
				{
					float num7 = MissionCombatMechanicsHelper.CalculateBaseMeleeBlowMagnitude(attackInformation, weapon, strikeType, num6, num5, 0f, doesAttackerHaveMountAgent);
					movementSpeedDamageModifier = MissionCombatMechanicsHelper.ComputeSpeedBonus(baseMagnitude, num7);
					speedBonusInt = MathF.Round(100f * movementSpeedDamageModifier);
					speedBonusInt = MBMath.ClampInt(speedBonusInt, -1000, 1000);
				}
			}
			baseMagnitude *= momentumRemaining;
			float num8 = 1f;
			if (hitWithAnotherBone)
			{
				if (strikeType == StrikeType.Thrust)
				{
					num8 = ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.ThrustHitWithArmDamageMultiplier);
				}
				else
				{
					num8 = ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.SwingHitWithArmDamageMultiplier);
				}
			}
			else if (strikeType == StrikeType.Thrust)
			{
				attackCollisionData = acd;
				if (!attackCollisionData.ThrustTipHit)
				{
					attackCollisionData = acd;
					if (!attackCollisionData.AttackBlockedWithShield)
					{
						num8 = ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.NonTipThrustHitDamageMultiplier);
					}
				}
			}
			baseMagnitude *= num8;
			if (attackInformation.AttackerAgent != null)
			{
				float weaponDamageMultiplier = MissionGameModels.Current.AgentStatCalculateModel.GetWeaponDamageMultiplier(attackInformation.AttackerAgent, currentUsageItem2);
				baseMagnitude *= weaponDamageMultiplier;
			}
			specialMagnitude = MissionCombatMechanicsHelper.ConvertBaseAttackMagnitude(currentUsageItem2, strikeType, baseMagnitude);
		}

		private static void ComputeBlowMagnitudeFromHorseCharge(in AttackInformation attackInformation, in AttackCollisionData acd, Vec2 attackerAgentVelocity, Vec2 victimAgentVelocity, out float baseMagnitude, out float specialMagnitude)
		{
			Vec2 attackerAgentMovementDirection = attackInformation.AttackerAgentMovementDirection;
			Vec2 vec = attackerAgentMovementDirection * Vec2.DotProduct(victimAgentVelocity, attackerAgentMovementDirection);
			Vec2 vec2 = attackerAgentVelocity - vec;
			AttackCollisionData attackCollisionData = acd;
			Vec3 collisionGlobalPosition = attackCollisionData.CollisionGlobalPosition;
			float num = MissionCombatMechanicsHelper.ChargeDamageDotProduct(attackInformation.VictimAgentPosition, attackerAgentMovementDirection, collisionGlobalPosition);
			float num2 = vec2.Length * num;
			baseMagnitude = num2 * num2 * num * attackInformation.AttackerAgentMountChargeDamageProperty;
			specialMagnitude = baseMagnitude;
		}

		private static void ComputeBlowMagnitudeMissile(in AttackInformation attackInformation, in AttackCollisionData acd, in MissionWeapon weapon, float momentumRemaining, in Vec2 victimVelocity, out float baseMagnitude, out float specialMagnitude)
		{
			BasicCharacterObject attackerAgentCharacter = attackInformation.AttackerAgentCharacter;
			BasicCharacterObject attackerCaptainCharacter = attackInformation.AttackerCaptainCharacter;
			AttackCollisionData attackCollisionData;
			float num;
			if (!attackInformation.IsVictimAgentNull)
			{
				Vec2 vec = victimVelocity;
				Vec3 vec2 = vec.ToVec3(0f);
				attackCollisionData = acd;
				num = (vec2 - attackCollisionData.MissileVelocity).Length;
			}
			else
			{
				attackCollisionData = acd;
				num = attackCollisionData.MissileVelocity.Length;
			}
			StrikeMagnitudeCalculationModel strikeMagnitudeModel = Game.Current.BasicModels.StrikeMagnitudeModel;
			BasicCharacterObject basicCharacterObject = attackerAgentCharacter;
			BasicCharacterObject basicCharacterObject2 = attackerCaptainCharacter;
			attackCollisionData = acd;
			float missileTotalDamage = attackCollisionData.MissileTotalDamage;
			float num2 = num;
			attackCollisionData = acd;
			float missileStartingBaseSpeed = attackCollisionData.MissileStartingBaseSpeed;
			MissionWeapon missionWeapon = weapon;
			ItemObject item = missionWeapon.Item;
			missionWeapon = weapon;
			baseMagnitude = strikeMagnitudeModel.CalculateStrikeMagnitudeForMissile(basicCharacterObject, basicCharacterObject2, missileTotalDamage, num2, missileStartingBaseSpeed, item, missionWeapon.CurrentUsageItem);
			baseMagnitude *= momentumRemaining;
			if (attackInformation.AttackerAgent != null)
			{
				AgentStatCalculateModel agentStatCalculateModel = MissionGameModels.Current.AgentStatCalculateModel;
				Agent attackerAgent = attackInformation.AttackerAgent;
				missionWeapon = weapon;
				float weaponDamageMultiplier = agentStatCalculateModel.GetWeaponDamageMultiplier(attackerAgent, missionWeapon.CurrentUsageItem);
				baseMagnitude *= weaponDamageMultiplier;
			}
			specialMagnitude = baseMagnitude;
		}

		private static void ComputeBlowMagnitudeFromFall(in AttackInformation attackInformation, in AttackCollisionData acd, out float baseMagnitude, out float specialMagnitude)
		{
			float victimAgentScale = attackInformation.VictimAgentScale;
			float num = attackInformation.VictimAgentWeight * victimAgentScale * victimAgentScale;
			float num2 = MathF.Sqrt(1f + attackInformation.VictimAgentTotalEncumbrance / num);
			AttackCollisionData attackCollisionData = acd;
			float num3 = -attackCollisionData.VictimAgentCurVelocity.z;
			if (attackInformation.DoesVictimHaveMountAgent)
			{
				float managedParameter = ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.FallSpeedReductionMultiplierForRiderDamage);
				num3 *= managedParameter;
			}
			float num4;
			if (attackInformation.IsVictimAgentHuman)
			{
				num4 = 1f;
			}
			else
			{
				num4 = 1.41f;
			}
			float managedParameter2 = ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.FallDamageMultiplier);
			float managedParameter3 = ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.FallDamageAbsorption);
			baseMagnitude = (num3 * num3 * managedParameter2 - managedParameter3) * num2 * num4;
			baseMagnitude = MBMath.ClampFloat(baseMagnitude, 0f, 499.9f);
			specialMagnitude = baseMagnitude;
		}

		private const float SpeedBonusFactorForSwing = 0.7f;

		private const float SpeedBonusFactorForThrust = 0.5f;
	}
}
