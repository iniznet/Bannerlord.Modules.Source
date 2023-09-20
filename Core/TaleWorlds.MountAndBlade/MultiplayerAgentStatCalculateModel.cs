using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class MultiplayerAgentStatCalculateModel : AgentStatCalculateModel
	{
		public override float GetDifficultyModifier()
		{
			return 0.5f;
		}

		public override bool CanAgentRideMount(Agent agent, Agent targetMount)
		{
			return agent.CheckSkillForMounting(targetMount);
		}

		public override void InitializeAgentStats(Agent agent, Equipment spawnEquipment, AgentDrivenProperties agentDrivenProperties, AgentBuildData agentBuildData)
		{
			agentDrivenProperties.ArmorEncumbrance = spawnEquipment.GetTotalWeightOfArmor(agent.IsHuman);
			if (!agent.IsHuman)
			{
				MultiplayerAgentStatCalculateModel.InitializeHorseAgentStats(agent, spawnEquipment, agentDrivenProperties);
				return;
			}
			agentDrivenProperties = this.InitializeHumanAgentStats(agent, agentDrivenProperties, agentBuildData);
		}

		public override float GetWeaponInaccuracy(Agent agent, WeaponComponentData weapon, int weaponSkill)
		{
			float num = 0f;
			if (weapon.IsRangedWeapon)
			{
				num = (100f - (float)weapon.Accuracy) * (1f - 0.002f * (float)weaponSkill) * 0.001f;
				if (weapon.WeaponClass == WeaponClass.ThrowingAxe)
				{
					num *= 5f;
				}
			}
			else if (weapon.WeaponFlags.HasAllFlags(WeaponFlags.WideGrip))
			{
				num = 1f - (float)weaponSkill * 0.01f;
			}
			return Math.Max(num, 0f);
		}

		private AgentDrivenProperties InitializeHumanAgentStats(Agent agent, AgentDrivenProperties agentDrivenProperties, AgentBuildData agentBuildData)
		{
			MultiplayerClassDivisions.MPHeroClass mpheroClassForCharacter = MultiplayerClassDivisions.GetMPHeroClassForCharacter(agent.Character);
			if (mpheroClassForCharacter != null)
			{
				this.FillAgentStatsFromData(ref agentDrivenProperties, agent, mpheroClassForCharacter, (agentBuildData != null) ? agentBuildData.AgentMissionPeer : null, (agentBuildData != null) ? agentBuildData.OwningAgentMissionPeer : null);
				agentDrivenProperties.SetStat(DrivenProperty.UseRealisticBlocking, MultiplayerOptions.OptionType.UseRealisticBlocking.GetBoolValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions) ? 1f : 0f);
			}
			if (mpheroClassForCharacter != null)
			{
				agent.BaseHealthLimit = (float)mpheroClassForCharacter.Health;
			}
			else
			{
				agent.BaseHealthLimit = 100f;
			}
			agent.HealthLimit = agent.BaseHealthLimit;
			agent.Health = agent.HealthLimit;
			return agentDrivenProperties;
		}

		private static void InitializeHorseAgentStats(Agent agent, Equipment spawnEquipment, AgentDrivenProperties agentDrivenProperties)
		{
			agentDrivenProperties.AiSpeciesIndex = agent.Monster.FamilyType;
			float num = 0.8f;
			EquipmentElement equipmentElement = spawnEquipment[EquipmentIndex.HorseHarness];
			agentDrivenProperties.AttributeRiding = num + ((equipmentElement.Item != null) ? 0.2f : 0f);
			float num2 = 0f;
			for (int i = 1; i < 12; i++)
			{
				equipmentElement = spawnEquipment[i];
				if (equipmentElement.Item != null)
				{
					float num3 = num2;
					equipmentElement = spawnEquipment[i];
					num2 = num3 + (float)equipmentElement.GetModifiedMountBodyArmor();
				}
			}
			agentDrivenProperties.ArmorTorso = num2;
			equipmentElement = spawnEquipment[EquipmentIndex.ArmorItemEndSlot];
			HorseComponent horseComponent = equipmentElement.Item.HorseComponent;
			EquipmentElement equipmentElement2 = spawnEquipment[EquipmentIndex.ArmorItemEndSlot];
			equipmentElement = spawnEquipment[EquipmentIndex.HorseHarness];
			agentDrivenProperties.MountChargeDamage = (float)equipmentElement2.GetModifiedMountCharge(equipmentElement) * 0.01f;
			agentDrivenProperties.MountDifficulty = (float)equipmentElement2.Item.Difficulty;
		}

		public override float GetWeaponDamageMultiplier(Agent agent, WeaponComponentData weapon)
		{
			return 1f;
		}

		public override float GetKnockBackResistance(Agent agent)
		{
			return 0.25f;
		}

		public override float GetKnockDownResistance(Agent agent, StrikeType strikeType = StrikeType.Invalid)
		{
			float num = 0.5f;
			if (agent.HasMount)
			{
				num += 0.1f;
			}
			else if (strikeType == StrikeType.Thrust)
			{
				num += 0.25f;
			}
			return num;
		}

		public override float GetDismountResistance(Agent agent)
		{
			return 0.5f;
		}

		public override void UpdateAgentStats(Agent agent, AgentDrivenProperties agentDrivenProperties)
		{
			if (agent.IsHuman)
			{
				this.UpdateHumanAgentStats(agent, agentDrivenProperties);
				return;
			}
			if (agent.IsMount)
			{
				this.UpdateMountAgentStats(agent, agentDrivenProperties);
			}
		}

		private void UpdateMountAgentStats(Agent agent, AgentDrivenProperties agentDrivenProperties)
		{
			MPPerkObject.MPPerkHandler perkHandler = MPPerkObject.GetPerkHandler(agent.RiderAgent);
			EquipmentElement equipmentElement = agent.SpawnEquipment[EquipmentIndex.ArmorItemEndSlot];
			EquipmentElement equipmentElement2 = agent.SpawnEquipment[EquipmentIndex.HorseHarness];
			agentDrivenProperties.MountManeuver = (float)equipmentElement.GetModifiedMountManeuver(equipmentElement2) * (1f + ((perkHandler != null) ? perkHandler.GetMountManeuver() : 0f));
			agentDrivenProperties.MountSpeed = (float)(equipmentElement.GetModifiedMountSpeed(equipmentElement2) + 1) * 0.22f * (1f + ((perkHandler != null) ? perkHandler.GetMountSpeed() : 0f));
			Agent riderAgent = agent.RiderAgent;
			int num = ((riderAgent != null) ? riderAgent.Character.GetSkillValue(DefaultSkills.Riding) : 100);
			agentDrivenProperties.TopSpeedReachDuration = Game.Current.BasicModels.RidingModel.CalculateAcceleration(equipmentElement, equipmentElement2, num);
			agentDrivenProperties.MountSpeed *= 1f + (float)num * 0.0032f;
			agentDrivenProperties.MountManeuver *= 1f + (float)num * 0.0035f;
			float num2 = equipmentElement.Weight / 2f + (equipmentElement2.IsEmpty ? 0f : equipmentElement2.Weight);
			agentDrivenProperties.MountDashAccelerationMultiplier = ((num2 > 200f) ? ((num2 < 300f) ? (1f - (num2 - 200f) / 111f) : 0.1f) : 1f);
		}

		public override int GetEffectiveSkillForWeapon(Agent agent, WeaponComponentData weapon)
		{
			int num = base.GetEffectiveSkillForWeapon(agent, weapon);
			if (num > 0 && weapon.IsRangedWeapon)
			{
				MPPerkObject.MPPerkHandler perkHandler = MPPerkObject.GetPerkHandler(agent);
				if (perkHandler != null)
				{
					num = MathF.Ceiling((float)num * (perkHandler.GetRangedAccuracy() + 1f));
				}
			}
			return num;
		}

		private void UpdateHumanAgentStats(Agent agent, AgentDrivenProperties agentDrivenProperties)
		{
			MPPerkObject.MPPerkHandler perkHandler = MPPerkObject.GetPerkHandler(agent);
			BasicCharacterObject character = agent.Character;
			MissionEquipment equipment = agent.Equipment;
			float num = equipment.GetTotalWeightOfWeapons();
			num *= 1f + ((perkHandler != null) ? perkHandler.GetEncumbrance(true) : 0f);
			EquipmentIndex wieldedItemIndex = agent.GetWieldedItemIndex(Agent.HandIndex.MainHand);
			EquipmentIndex wieldedItemIndex2 = agent.GetWieldedItemIndex(Agent.HandIndex.OffHand);
			if (wieldedItemIndex != EquipmentIndex.None)
			{
				ItemObject item = equipment[wieldedItemIndex].Item;
				WeaponComponent weaponComponent = item.WeaponComponent;
				if (weaponComponent != null)
				{
					float realWeaponLength = weaponComponent.PrimaryWeapon.GetRealWeaponLength();
					float num2 = ((weaponComponent.GetItemType() == ItemObject.ItemTypeEnum.Bow) ? 4f : 1.5f) * item.Weight * MathF.Sqrt(realWeaponLength);
					num2 *= 1f + ((perkHandler != null) ? perkHandler.GetEncumbrance(false) : 0f);
					num += num2;
				}
			}
			if (wieldedItemIndex2 != EquipmentIndex.None)
			{
				ItemObject item2 = equipment[wieldedItemIndex2].Item;
				float num3 = 1.5f * item2.Weight;
				num3 *= 1f + ((perkHandler != null) ? perkHandler.GetEncumbrance(false) : 0f);
				num += num3;
			}
			agentDrivenProperties.WeaponsEncumbrance = num;
			EquipmentIndex wieldedItemIndex3 = agent.GetWieldedItemIndex(Agent.HandIndex.MainHand);
			WeaponComponentData weaponComponentData = ((wieldedItemIndex3 != EquipmentIndex.None) ? equipment[wieldedItemIndex3].CurrentUsageItem : null);
			ItemObject itemObject = ((wieldedItemIndex3 != EquipmentIndex.None) ? equipment[wieldedItemIndex3].Item : null);
			EquipmentIndex wieldedItemIndex4 = agent.GetWieldedItemIndex(Agent.HandIndex.OffHand);
			WeaponComponentData weaponComponentData2 = ((wieldedItemIndex4 != EquipmentIndex.None) ? equipment[wieldedItemIndex4].CurrentUsageItem : null);
			agentDrivenProperties.SwingSpeedMultiplier = 0.93f + 0.0007f * (float)this.GetSkillValueForItem(character, itemObject);
			agentDrivenProperties.ThrustOrRangedReadySpeedMultiplier = agentDrivenProperties.SwingSpeedMultiplier;
			agentDrivenProperties.HandlingMultiplier = 1f;
			agentDrivenProperties.ShieldBashStunDurationMultiplier = 1f;
			agentDrivenProperties.KickStunDurationMultiplier = 1f;
			agentDrivenProperties.ReloadSpeed = 0.93f + 0.0007f * (float)this.GetSkillValueForItem(character, itemObject);
			agentDrivenProperties.MissileSpeedMultiplier = 1f;
			agentDrivenProperties.ReloadMovementPenaltyFactor = 1f;
			base.SetAllWeaponInaccuracy(agent, agentDrivenProperties, (int)wieldedItemIndex3, weaponComponentData);
			MultiplayerClassDivisions.MPHeroClass mpheroClassForCharacter = MultiplayerClassDivisions.GetMPHeroClassForCharacter(agent.Character);
			float num4 = (mpheroClassForCharacter.IsTroopCharacter(agent.Character) ? mpheroClassForCharacter.TroopMovementSpeedMultiplier : mpheroClassForCharacter.HeroMovementSpeedMultiplier);
			agentDrivenProperties.MaxSpeedMultiplier = 1.05f * (num4 * (100f / (100f + num)));
			int skillValue = character.GetSkillValue(DefaultSkills.Riding);
			bool flag = false;
			bool flag2 = false;
			if (weaponComponentData != null)
			{
				WeaponComponentData weaponComponentData3 = weaponComponentData;
				int effectiveSkillForWeapon = this.GetEffectiveSkillForWeapon(agent, weaponComponentData3);
				if (perkHandler != null)
				{
					agentDrivenProperties.MissileSpeedMultiplier *= perkHandler.GetThrowingWeaponSpeed(weaponComponentData) + 1f;
				}
				if (weaponComponentData3.IsRangedWeapon)
				{
					int thrustSpeed = weaponComponentData3.ThrustSpeed;
					if (!agent.HasMount)
					{
						float num5 = MathF.Max(0f, 1f - (float)effectiveSkillForWeapon / 500f);
						agentDrivenProperties.WeaponMaxMovementAccuracyPenalty = 0.125f * num5;
						agentDrivenProperties.WeaponMaxUnsteadyAccuracyPenalty = 0.1f * num5;
					}
					else
					{
						float num6 = MathF.Max(0f, (1f - (float)effectiveSkillForWeapon / 500f) * (1f - (float)skillValue / 1800f));
						agentDrivenProperties.WeaponMaxMovementAccuracyPenalty = 0.025f * num6;
						agentDrivenProperties.WeaponMaxUnsteadyAccuracyPenalty = 0.06f * num6;
					}
					agentDrivenProperties.WeaponMaxMovementAccuracyPenalty = MathF.Max(0f, agentDrivenProperties.WeaponMaxMovementAccuracyPenalty);
					agentDrivenProperties.WeaponMaxUnsteadyAccuracyPenalty = MathF.Max(0f, agentDrivenProperties.WeaponMaxUnsteadyAccuracyPenalty);
					if (weaponComponentData3.RelevantSkill == DefaultSkills.Bow)
					{
						float num7 = ((float)thrustSpeed - 60f) / 75f;
						num7 = MBMath.ClampFloat(num7, 0f, 1f);
						agentDrivenProperties.WeaponMaxMovementAccuracyPenalty *= 6f;
						agentDrivenProperties.WeaponMaxUnsteadyAccuracyPenalty *= 4.5f / MBMath.Lerp(0.75f, 2f, num7, 1E-05f);
					}
					else if (weaponComponentData3.RelevantSkill == DefaultSkills.Throwing)
					{
						float num8 = ((float)thrustSpeed - 85f) / 17f;
						num8 = MBMath.ClampFloat(num8, 0f, 1f);
						agentDrivenProperties.WeaponMaxUnsteadyAccuracyPenalty *= 3.5f * MBMath.Lerp(1.5f, 0.8f, num8, 1E-05f);
					}
					else if (weaponComponentData3.RelevantSkill == DefaultSkills.Crossbow)
					{
						agentDrivenProperties.WeaponMaxMovementAccuracyPenalty *= 2.5f;
						agentDrivenProperties.WeaponMaxUnsteadyAccuracyPenalty *= 1.2f;
					}
					if (weaponComponentData3.WeaponClass == WeaponClass.Bow)
					{
						flag = true;
						agentDrivenProperties.WeaponBestAccuracyWaitTime = 0.3f + (95.75f - (float)thrustSpeed) * 0.005f;
						float num9 = ((float)thrustSpeed - 60f) / 75f;
						num9 = MBMath.ClampFloat(num9, 0f, 1f);
						agentDrivenProperties.WeaponUnsteadyBeginTime = 0.1f + (float)effectiveSkillForWeapon * 0.01f * MBMath.Lerp(1f, 2f, num9, 1E-05f);
						if (agent.IsAIControlled)
						{
							agentDrivenProperties.WeaponUnsteadyBeginTime *= 4f;
						}
						agentDrivenProperties.WeaponUnsteadyEndTime = 2f + agentDrivenProperties.WeaponUnsteadyBeginTime;
						agentDrivenProperties.WeaponRotationalAccuracyPenaltyInRadians = 0.1f;
					}
					else if (weaponComponentData3.WeaponClass == WeaponClass.Javelin || weaponComponentData3.WeaponClass == WeaponClass.ThrowingAxe || weaponComponentData3.WeaponClass == WeaponClass.ThrowingKnife)
					{
						agentDrivenProperties.WeaponBestAccuracyWaitTime = 0.4f + (89f - (float)thrustSpeed) * 0.03f;
						agentDrivenProperties.WeaponUnsteadyBeginTime = 2.5f + (float)effectiveSkillForWeapon * 0.01f;
						agentDrivenProperties.WeaponUnsteadyEndTime = 10f + agentDrivenProperties.WeaponUnsteadyBeginTime;
						agentDrivenProperties.WeaponRotationalAccuracyPenaltyInRadians = 0.025f;
						if (weaponComponentData3.WeaponClass == WeaponClass.ThrowingAxe)
						{
							agentDrivenProperties.WeaponInaccuracy *= 6.6f;
						}
					}
					else
					{
						agentDrivenProperties.WeaponBestAccuracyWaitTime = 0.1f;
						agentDrivenProperties.WeaponUnsteadyBeginTime = 0f;
						agentDrivenProperties.WeaponUnsteadyEndTime = 0f;
						agentDrivenProperties.WeaponRotationalAccuracyPenaltyInRadians = 0.1f;
					}
				}
				else if (weaponComponentData3.WeaponFlags.HasAllFlags(WeaponFlags.WideGrip))
				{
					flag2 = true;
					agentDrivenProperties.WeaponUnsteadyBeginTime = 1f + (float)effectiveSkillForWeapon * 0.005f;
					agentDrivenProperties.WeaponUnsteadyEndTime = 3f + (float)effectiveSkillForWeapon * 0.01f;
				}
			}
			agentDrivenProperties.AttributeShieldMissileCollisionBodySizeAdder = 0.3f;
			Agent mountAgent = agent.MountAgent;
			float num10 = ((mountAgent != null) ? mountAgent.GetAgentDrivenPropertyValue(DrivenProperty.AttributeRiding) : 1f);
			agentDrivenProperties.AttributeRiding = (float)skillValue * num10;
			agentDrivenProperties.AttributeHorseArchery = Game.Current.BasicModels.StrikeMagnitudeModel.CalculateHorseArcheryFactor(character);
			agentDrivenProperties.BipedalRangedReadySpeedMultiplier = ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.BipedalRangedReadySpeedMultiplier);
			agentDrivenProperties.BipedalRangedReloadSpeedMultiplier = ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.BipedalRangedReloadSpeedMultiplier);
			if (perkHandler != null)
			{
				for (int i = 55; i < 84; i++)
				{
					DrivenProperty drivenProperty = (DrivenProperty)i;
					if (((drivenProperty != DrivenProperty.WeaponUnsteadyBeginTime && drivenProperty != DrivenProperty.WeaponUnsteadyEndTime) || flag || flag2) && (drivenProperty != DrivenProperty.WeaponRotationalAccuracyPenaltyInRadians || flag))
					{
						float stat = agentDrivenProperties.GetStat(drivenProperty);
						agentDrivenProperties.SetStat(drivenProperty, stat + perkHandler.GetDrivenPropertyBonus(drivenProperty, stat));
					}
				}
			}
			base.SetAiRelatedProperties(agent, agentDrivenProperties, weaponComponentData, weaponComponentData2);
		}

		private void FillAgentStatsFromData(ref AgentDrivenProperties agentDrivenProperties, Agent agent, MultiplayerClassDivisions.MPHeroClass heroClass, MissionPeer missionPeer, MissionPeer owningMissionPeer)
		{
			MissionPeer missionPeer2 = missionPeer ?? owningMissionPeer;
			if (missionPeer2 != null)
			{
				MPPerkObject.MPOnSpawnPerkHandler onSpawnPerkHandler = MPPerkObject.GetOnSpawnPerkHandler(missionPeer2);
				bool flag = missionPeer != null;
				for (int i = 0; i < 55; i++)
				{
					DrivenProperty drivenProperty = (DrivenProperty)i;
					float stat = agentDrivenProperties.GetStat(drivenProperty);
					if (drivenProperty == DrivenProperty.ArmorHead || drivenProperty == DrivenProperty.ArmorTorso || drivenProperty == DrivenProperty.ArmorLegs || drivenProperty == DrivenProperty.ArmorArms)
					{
						agentDrivenProperties.SetStat(drivenProperty, stat + (float)heroClass.ArmorValue + onSpawnPerkHandler.GetDrivenPropertyBonusOnSpawn(flag, drivenProperty, stat));
					}
					else
					{
						agentDrivenProperties.SetStat(drivenProperty, stat + onSpawnPerkHandler.GetDrivenPropertyBonusOnSpawn(flag, drivenProperty, stat));
					}
				}
			}
			float num = (heroClass.IsTroopCharacter(agent.Character) ? heroClass.TroopTopSpeedReachDuration : heroClass.HeroTopSpeedReachDuration);
			agentDrivenProperties.TopSpeedReachDuration = num;
			float managedParameter = ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.BipedalCombatSpeedMinMultiplier);
			float managedParameter2 = ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.BipedalCombatSpeedMaxMultiplier);
			float num2 = (heroClass.IsTroopCharacter(agent.Character) ? heroClass.TroopCombatMovementSpeedMultiplier : heroClass.HeroCombatMovementSpeedMultiplier);
			agentDrivenProperties.CombatMaxSpeedMultiplier = managedParameter + (managedParameter2 - managedParameter) * num2;
		}

		private int GetSkillValueForItem(BasicCharacterObject characterObject, ItemObject primaryItem)
		{
			return characterObject.GetSkillValue((primaryItem != null) ? primaryItem.RelevantSkill : DefaultSkills.Athletics);
		}

		public static float CalculateMaximumSpeedMultiplier(Agent agent)
		{
			MultiplayerClassDivisions.MPHeroClass mpheroClassForCharacter = MultiplayerClassDivisions.GetMPHeroClassForCharacter(agent.Character);
			if (!mpheroClassForCharacter.IsTroopCharacter(agent.Character))
			{
				return mpheroClassForCharacter.HeroMovementSpeedMultiplier;
			}
			return mpheroClassForCharacter.TroopMovementSpeedMultiplier;
		}
	}
}
