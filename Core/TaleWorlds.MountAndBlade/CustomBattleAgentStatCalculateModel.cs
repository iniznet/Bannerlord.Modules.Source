using System;
using MBHelpers;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	public class CustomBattleAgentStatCalculateModel : AgentStatCalculateModel
	{
		public override float GetDifficultyModifier()
		{
			return 1f;
		}

		public override bool CanAgentRideMount(Agent agent, Agent targetMount)
		{
			return agent.CheckSkillForMounting(targetMount);
		}

		public override void InitializeAgentStats(Agent agent, Equipment spawnEquipment, AgentDrivenProperties agentDrivenProperties, AgentBuildData agentBuildData)
		{
			agentDrivenProperties.ArmorEncumbrance = spawnEquipment.GetTotalWeightOfArmor(agent.IsHuman);
			if (agent.IsHuman)
			{
				agentDrivenProperties.ArmorHead = spawnEquipment.GetHeadArmorSum();
				agentDrivenProperties.ArmorTorso = spawnEquipment.GetHumanBodyArmorSum();
				agentDrivenProperties.ArmorLegs = spawnEquipment.GetLegArmorSum();
				agentDrivenProperties.ArmorArms = spawnEquipment.GetArmArmorSum();
				return;
			}
			agentDrivenProperties.AiSpeciesIndex = (int)spawnEquipment[EquipmentIndex.ArmorItemEndSlot].Item.Id.InternalValue;
			agentDrivenProperties.AttributeRiding = 0.8f + ((spawnEquipment[EquipmentIndex.HorseHarness].Item != null) ? 0.2f : 0f);
			float num = 0f;
			for (int i = 1; i < 12; i++)
			{
				if (spawnEquipment[i].Item != null)
				{
					num += (float)spawnEquipment[i].GetModifiedMountBodyArmor();
				}
			}
			agentDrivenProperties.ArmorTorso = num;
			ItemObject item = spawnEquipment[EquipmentIndex.ArmorItemEndSlot].Item;
			if (item != null)
			{
				HorseComponent horseComponent = item.HorseComponent;
				EquipmentElement equipmentElement = spawnEquipment[EquipmentIndex.ArmorItemEndSlot];
				EquipmentElement equipmentElement2 = spawnEquipment[EquipmentIndex.HorseHarness];
				agentDrivenProperties.MountChargeDamage = (float)equipmentElement.GetModifiedMountCharge(equipmentElement2) * 0.01f;
				agentDrivenProperties.MountDifficulty = (float)equipmentElement.Item.Difficulty;
			}
		}

		public override void UpdateAgentStats(Agent agent, AgentDrivenProperties agentDrivenProperties)
		{
			if (agent.IsHuman)
			{
				this.UpdateHumanStats(agent, agentDrivenProperties);
				return;
			}
			this.UpdateHorseStats(agent, agentDrivenProperties);
		}

		public override float GetWeaponDamageMultiplier(BasicCharacterObject agentCharacter, IAgentOriginBase agentOrigin, Formation agentFormation, WeaponComponentData weapon)
		{
			float num = 1f;
			SkillObject skillObject = ((weapon != null) ? weapon.RelevantSkill : null);
			if (agentCharacter != null && skillObject != null)
			{
				if (skillObject == DefaultSkills.OneHanded)
				{
					int skillValue = agentCharacter.GetSkillValue(skillObject);
					num += (float)skillValue * 0.0015f;
				}
				else if (skillObject == DefaultSkills.TwoHanded)
				{
					int skillValue2 = agentCharacter.GetSkillValue(skillObject);
					num += (float)skillValue2 * 0.0016f;
				}
				else if (skillObject == DefaultSkills.Polearm)
				{
					int skillValue3 = agentCharacter.GetSkillValue(skillObject);
					num += (float)skillValue3 * 0.0007f;
				}
				else if (skillObject == DefaultSkills.Bow)
				{
					int skillValue4 = agentCharacter.GetSkillValue(skillObject);
					num += (float)skillValue4 * 0.0011f;
				}
				else if (skillObject == DefaultSkills.Throwing)
				{
					int skillValue5 = agentCharacter.GetSkillValue(skillObject);
					num += (float)skillValue5 * 0.0006f;
				}
			}
			return Math.Max(0f, num);
		}

		public override float GetKnockBackResistance(Agent agent)
		{
			BasicCharacterObject character;
			if (agent.IsHuman && (character = agent.Character) != null)
			{
				int effectiveSkill = this.GetEffectiveSkill(character, agent.Origin, agent.Formation, DefaultSkills.Athletics);
				float num = 0.15f + (float)effectiveSkill * 0.001f;
				return Math.Max(0f, num);
			}
			return float.MaxValue;
		}

		public override float GetKnockDownResistance(Agent agent, StrikeType strikeType = StrikeType.Invalid)
		{
			BasicCharacterObject character;
			if (agent.IsHuman && (character = agent.Character) != null)
			{
				int effectiveSkill = this.GetEffectiveSkill(character, agent.Origin, agent.Formation, DefaultSkills.Athletics);
				float num = 0.4f + (float)effectiveSkill * 0.001f;
				if (agent.HasMount)
				{
					num += 0.1f;
				}
				else if (strikeType == StrikeType.Thrust)
				{
					num += 0.15f;
				}
				return Math.Max(0f, num);
			}
			return float.MaxValue;
		}

		public override float GetDismountResistance(Agent agent)
		{
			BasicCharacterObject character;
			if (agent.IsHuman && (character = agent.Character) != null)
			{
				int effectiveSkill = this.GetEffectiveSkill(character, agent.Origin, agent.Formation, DefaultSkills.Riding);
				float num = 0.4f + (float)effectiveSkill * 0.001f;
				return Math.Max(0f, num);
			}
			return float.MaxValue;
		}

		private int GetSkillValueForItem(Agent agent, ItemObject primaryItem)
		{
			return this.GetEffectiveSkill(agent.Character, agent.Origin, agent.Formation, (primaryItem != null) ? primaryItem.RelevantSkill : DefaultSkills.Athletics);
		}

		private void UpdateHumanStats(Agent agent, AgentDrivenProperties agentDrivenProperties)
		{
			BasicCharacterObject character = agent.Character;
			MissionEquipment equipment = agent.Equipment;
			float num = equipment.GetTotalWeightOfWeapons();
			int weight = agent.Monster.Weight;
			float num2 = agentDrivenProperties.ArmorEncumbrance + num;
			EquipmentIndex wieldedItemIndex = agent.GetWieldedItemIndex(Agent.HandIndex.MainHand);
			EquipmentIndex wieldedItemIndex2 = agent.GetWieldedItemIndex(Agent.HandIndex.OffHand);
			if (wieldedItemIndex != EquipmentIndex.None)
			{
				ItemObject item = equipment[wieldedItemIndex].Item;
				WeaponComponent weaponComponent = item.WeaponComponent;
				if (weaponComponent != null)
				{
					float realWeaponLength = weaponComponent.PrimaryWeapon.GetRealWeaponLength();
					num += 1.5f * item.Weight * MathF.Sqrt(realWeaponLength);
				}
			}
			if (wieldedItemIndex2 != EquipmentIndex.None)
			{
				ItemObject item2 = equipment[wieldedItemIndex2].Item;
				num += 1.5f * item2.Weight;
			}
			agentDrivenProperties.WeaponsEncumbrance = num;
			EquipmentIndex wieldedItemIndex3 = agent.GetWieldedItemIndex(Agent.HandIndex.MainHand);
			WeaponComponentData weaponComponentData = ((wieldedItemIndex3 != EquipmentIndex.None) ? equipment[wieldedItemIndex3].CurrentUsageItem : null);
			ItemObject itemObject = ((wieldedItemIndex3 != EquipmentIndex.None) ? equipment[wieldedItemIndex3].Item : null);
			EquipmentIndex wieldedItemIndex4 = agent.GetWieldedItemIndex(Agent.HandIndex.OffHand);
			WeaponComponentData weaponComponentData2 = ((wieldedItemIndex4 != EquipmentIndex.None) ? equipment[wieldedItemIndex4].CurrentUsageItem : null);
			agentDrivenProperties.SwingSpeedMultiplier = 0.93f + 0.0007f * (float)this.GetSkillValueForItem(agent, itemObject);
			agentDrivenProperties.ThrustOrRangedReadySpeedMultiplier = agentDrivenProperties.SwingSpeedMultiplier;
			agentDrivenProperties.HandlingMultiplier = 1f;
			agentDrivenProperties.ShieldBashStunDurationMultiplier = 1f;
			agentDrivenProperties.KickStunDurationMultiplier = 1f;
			agentDrivenProperties.ReloadSpeed = 0.93f + 0.0007f * (float)this.GetSkillValueForItem(agent, itemObject);
			agentDrivenProperties.MissileSpeedMultiplier = 1f;
			agentDrivenProperties.ReloadMovementPenaltyFactor = 1f;
			base.SetAllWeaponInaccuracy(agent, agentDrivenProperties, (int)wieldedItemIndex3, weaponComponentData);
			IAgentOriginBase origin = agent.Origin;
			BasicCharacterObject character2 = agent.Character;
			Formation formation = agent.Formation;
			int effectiveSkill = this.GetEffectiveSkill(character2, origin, agent.Formation, DefaultSkills.Athletics);
			int effectiveSkill2 = this.GetEffectiveSkill(character2, origin, formation, DefaultSkills.Riding);
			if (weaponComponentData != null)
			{
				WeaponComponentData weaponComponentData3 = weaponComponentData;
				int effectiveSkillForWeapon = this.GetEffectiveSkillForWeapon(agent, weaponComponentData3);
				if (weaponComponentData3.IsRangedWeapon)
				{
					int thrustSpeed = weaponComponentData3.ThrustSpeed;
					if (!agent.HasMount)
					{
						float num3 = MathF.Max(0f, 1f - (float)effectiveSkillForWeapon / 500f);
						agentDrivenProperties.WeaponMaxMovementAccuracyPenalty = 0.125f * num3;
						agentDrivenProperties.WeaponMaxUnsteadyAccuracyPenalty = 0.1f * num3;
					}
					else
					{
						float num4 = MathF.Max(0f, (1f - (float)effectiveSkillForWeapon / 500f) * (1f - (float)effectiveSkill2 / 1800f));
						agentDrivenProperties.WeaponMaxMovementAccuracyPenalty = 0.025f * num4;
						agentDrivenProperties.WeaponMaxUnsteadyAccuracyPenalty = 0.12f * num4;
					}
					agentDrivenProperties.WeaponMaxMovementAccuracyPenalty = MathF.Max(0f, agentDrivenProperties.WeaponMaxMovementAccuracyPenalty);
					agentDrivenProperties.WeaponMaxUnsteadyAccuracyPenalty = MathF.Max(0f, agentDrivenProperties.WeaponMaxUnsteadyAccuracyPenalty);
					if (weaponComponentData3.RelevantSkill == DefaultSkills.Bow)
					{
						float num5 = ((float)thrustSpeed - 45f) / 90f;
						num5 = MBMath.ClampFloat(num5, 0f, 1f);
						agentDrivenProperties.WeaponMaxMovementAccuracyPenalty *= 6f;
						agentDrivenProperties.WeaponMaxUnsteadyAccuracyPenalty *= 4.5f / MBMath.Lerp(0.75f, 2f, num5, 1E-05f);
					}
					else if (weaponComponentData3.RelevantSkill == DefaultSkills.Throwing)
					{
						float num6 = ((float)thrustSpeed - 89f) / 13f;
						num6 = MBMath.ClampFloat(num6, 0f, 1f);
						agentDrivenProperties.WeaponMaxUnsteadyAccuracyPenalty *= 3.5f * MBMath.Lerp(1.5f, 0.8f, num6, 1E-05f);
					}
					else if (weaponComponentData3.RelevantSkill == DefaultSkills.Crossbow)
					{
						agentDrivenProperties.WeaponMaxMovementAccuracyPenalty *= 2.5f;
						agentDrivenProperties.WeaponMaxUnsteadyAccuracyPenalty *= 1.2f;
					}
					if (weaponComponentData3.WeaponClass == WeaponClass.Bow)
					{
						agentDrivenProperties.WeaponBestAccuracyWaitTime = 0.3f + (95.75f - (float)thrustSpeed) * 0.005f;
						float num7 = ((float)thrustSpeed - 45f) / 90f;
						num7 = MBMath.ClampFloat(num7, 0f, 1f);
						agentDrivenProperties.WeaponUnsteadyBeginTime = 0.6f + (float)effectiveSkillForWeapon * 0.01f * MBMath.Lerp(2f, 4f, num7, 1E-05f);
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
					agentDrivenProperties.WeaponUnsteadyBeginTime = 1f + (float)effectiveSkillForWeapon * 0.005f;
					agentDrivenProperties.WeaponUnsteadyEndTime = 3f + (float)effectiveSkillForWeapon * 0.01f;
				}
				if (agent.HasMount)
				{
					float num8 = 1f - MathF.Max(0f, 0.2f - (float)effectiveSkill2 * 0.002f);
					agentDrivenProperties.SwingSpeedMultiplier *= num8;
					agentDrivenProperties.ThrustOrRangedReadySpeedMultiplier *= num8;
					agentDrivenProperties.ReloadSpeed *= num8;
				}
			}
			agentDrivenProperties.TopSpeedReachDuration = 2f / MathF.Max((200f + (float)effectiveSkill) / 300f * ((float)weight / ((float)weight + num2)), 0.3f);
			float num9 = 0.7f + 0.00070000015f * (float)effectiveSkill;
			float num10 = MathF.Max(0.2f * (1f - (float)effectiveSkill * 0.001f), 0f) * num2 / (float)weight;
			float num11 = MBMath.ClampFloat(num9 - num10, 0f, 0.91f);
			agentDrivenProperties.MaxSpeedMultiplier = this.GetEnvironmentSpeedFactor(agent) * num11;
			float managedParameter = ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.BipedalCombatSpeedMinMultiplier);
			float managedParameter2 = ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.BipedalCombatSpeedMaxMultiplier);
			float num12 = MathF.Min(num2 / (float)weight, 1f);
			agentDrivenProperties.CombatMaxSpeedMultiplier = MathF.Min(MBMath.Lerp(managedParameter2, managedParameter, num12, 1E-05f), 1f);
			agentDrivenProperties.AttributeShieldMissileCollisionBodySizeAdder = 0.3f;
			Agent mountAgent = agent.MountAgent;
			float num13 = ((mountAgent != null) ? mountAgent.GetAgentDrivenPropertyValue(DrivenProperty.AttributeRiding) : 1f);
			agentDrivenProperties.AttributeRiding = (float)effectiveSkill2 * num13;
			agentDrivenProperties.AttributeHorseArchery = Game.Current.BasicModels.StrikeMagnitudeModel.CalculateHorseArcheryFactor(character);
			agentDrivenProperties.BipedalRangedReadySpeedMultiplier = ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.BipedalRangedReadySpeedMultiplier);
			agentDrivenProperties.BipedalRangedReloadSpeedMultiplier = ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.BipedalRangedReloadSpeedMultiplier);
			this.GetBannerEffectsOnAgent(agent, agentDrivenProperties, weaponComponentData);
			base.SetAiRelatedProperties(agent, agentDrivenProperties, weaponComponentData, weaponComponentData2);
			float num14 = 1f;
			if (!agent.Mission.Scene.IsAtmosphereIndoor)
			{
				float rainDensity = agent.Mission.Scene.GetRainDensity();
				float fog = agent.Mission.Scene.GetFog();
				if (rainDensity > 0f || fog > 0f)
				{
					num14 += MathF.Min(0.3f, rainDensity + fog);
				}
				if (!MBMath.IsBetween(agent.Mission.Scene.TimeOfDay, 4f, 20.01f))
				{
					num14 += 0.1f;
				}
			}
			agentDrivenProperties.AiShooterError *= num14;
		}

		private void UpdateHorseStats(Agent agent, AgentDrivenProperties agentDrivenProperties)
		{
			Equipment spawnEquipment = agent.SpawnEquipment;
			EquipmentElement equipmentElement = spawnEquipment[EquipmentIndex.ArmorItemEndSlot];
			EquipmentElement equipmentElement2 = spawnEquipment[EquipmentIndex.HorseHarness];
			ItemObject item = equipmentElement.Item;
			float num = (float)(equipmentElement.GetModifiedMountSpeed(equipmentElement2) + 1);
			int modifiedMountManeuver = equipmentElement.GetModifiedMountManeuver(equipmentElement2);
			int num2 = 0;
			float environmentSpeedFactor = this.GetEnvironmentSpeedFactor(agent);
			if (agent.RiderAgent != null)
			{
				num2 = this.GetEffectiveSkill(agent.RiderAgent.Character, agent.RiderAgent.Origin, agent.RiderAgent.Formation, DefaultSkills.Riding);
				FactoredNumber factoredNumber = new FactoredNumber(num);
				FactoredNumber factoredNumber2 = new FactoredNumber((float)modifiedMountManeuver);
				factoredNumber.AddFactor((float)num2 * 0.001f);
				factoredNumber2.AddFactor((float)num2 * 0.0004f);
				Formation formation = agent.RiderAgent.Formation;
				BannerComponent activeBanner = MissionGameModels.Current.BattleBannerBearersModel.GetActiveBanner(formation);
				if (activeBanner != null)
				{
					BannerHelper.AddBannerBonusForBanner(DefaultBannerEffects.IncreasedMountMovementSpeed, activeBanner, ref factoredNumber);
				}
				agentDrivenProperties.MountManeuver = factoredNumber2.ResultNumber;
				agentDrivenProperties.MountSpeed = environmentSpeedFactor * 0.22f * (1f + factoredNumber.ResultNumber);
			}
			else
			{
				agentDrivenProperties.MountManeuver = (float)modifiedMountManeuver;
				agentDrivenProperties.MountSpeed = environmentSpeedFactor * 0.22f * (1f + num);
			}
			float num3 = equipmentElement.Weight / 2f + (equipmentElement2.IsEmpty ? 0f : equipmentElement2.Weight);
			agentDrivenProperties.MountDashAccelerationMultiplier = ((num3 > 200f) ? ((num3 < 300f) ? (1f - (num3 - 200f) / 111f) : 0.1f) : 1f);
			agentDrivenProperties.TopSpeedReachDuration = Game.Current.BasicModels.RidingModel.CalculateAcceleration(equipmentElement, equipmentElement2, num2);
		}

		private void GetBannerEffectsOnAgent(Agent agent, AgentDrivenProperties agentDrivenProperties, WeaponComponentData rightHandEquippedItem)
		{
			BannerComponent activeBanner = MissionGameModels.Current.BattleBannerBearersModel.GetActiveBanner(agent.Formation);
			if (agent.Character != null && activeBanner != null)
			{
				bool flag = rightHandEquippedItem != null && rightHandEquippedItem.IsRangedWeapon;
				FactoredNumber factoredNumber = new FactoredNumber(agentDrivenProperties.MaxSpeedMultiplier);
				FactoredNumber factoredNumber2 = new FactoredNumber(agentDrivenProperties.WeaponInaccuracy);
				if (flag && rightHandEquippedItem != null)
				{
					BannerHelper.AddBannerBonusForBanner(DefaultBannerEffects.DecreasedRangedAccuracyPenalty, activeBanner, ref factoredNumber2);
				}
				BannerHelper.AddBannerBonusForBanner(DefaultBannerEffects.IncreasedTroopMovementSpeed, activeBanner, ref factoredNumber);
				agentDrivenProperties.MaxSpeedMultiplier = factoredNumber.ResultNumber;
				agentDrivenProperties.WeaponInaccuracy = factoredNumber2.ResultNumber;
			}
		}
	}
}
