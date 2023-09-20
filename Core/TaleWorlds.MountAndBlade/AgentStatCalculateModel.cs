using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020001DB RID: 475
	public abstract class AgentStatCalculateModel : GameModel
	{
		// Token: 0x06001AFA RID: 6906
		public abstract void InitializeAgentStats(Agent agent, Equipment spawnEquipment, AgentDrivenProperties agentDrivenProperties, AgentBuildData agentBuildData);

		// Token: 0x06001AFB RID: 6907 RVA: 0x0005E13E File Offset: 0x0005C33E
		public virtual void InitializeMissionEquipment(Agent agent)
		{
		}

		// Token: 0x06001AFC RID: 6908
		public abstract void UpdateAgentStats(Agent agent, AgentDrivenProperties agentDrivenProperties);

		// Token: 0x06001AFD RID: 6909
		public abstract float GetDifficultyModifier();

		// Token: 0x06001AFE RID: 6910
		public abstract bool CanAgentRideMount(Agent agent, Agent targetMount);

		// Token: 0x06001AFF RID: 6911 RVA: 0x0005E140 File Offset: 0x0005C340
		public virtual bool HasHeavyArmor(Agent agent)
		{
			return agent.GetBaseArmorEffectivenessForBodyPart(BoneBodyPartType.Chest) >= 24f;
		}

		// Token: 0x06001B00 RID: 6912 RVA: 0x0005E153 File Offset: 0x0005C353
		public virtual float GetEffectiveMaxHealth(Agent agent)
		{
			return agent.BaseHealthLimit;
		}

		// Token: 0x06001B01 RID: 6913 RVA: 0x0005E15C File Offset: 0x0005C35C
		public virtual float GetEnvironmentSpeedFactor(Agent agent)
		{
			Scene scene = agent.Mission.Scene;
			float num = 1f;
			if (!scene.IsAtmosphereIndoor)
			{
				if (scene.GetRainDensity() > 0f)
				{
					num *= 0.9f;
				}
				if (!agent.IsHuman && !MBMath.IsBetween(scene.TimeOfDay, 4f, 20.01f))
				{
					num *= 0.9f;
				}
			}
			return num;
		}

		// Token: 0x06001B02 RID: 6914 RVA: 0x0005E1C0 File Offset: 0x0005C3C0
		public float CalculateAIAttackOnDecideMaxValue()
		{
			if (this.GetDifficultyModifier() < 0.5f)
			{
				return 0.32f;
			}
			return 0.96f;
		}

		// Token: 0x06001B03 RID: 6915 RVA: 0x0005E1DC File Offset: 0x0005C3DC
		public virtual float GetWeaponInaccuracy(Agent agent, WeaponComponentData weapon, int weaponSkill)
		{
			float num = 0f;
			if (weapon.IsRangedWeapon)
			{
				num = (100f - (float)weapon.Accuracy) * (1f - 0.002f * (float)weaponSkill) * 0.001f;
			}
			else if (weapon.WeaponFlags.HasAllFlags(WeaponFlags.WideGrip))
			{
				num = 1f - (float)weaponSkill * 0.01f;
			}
			return MathF.Max(num, 0f);
		}

		// Token: 0x06001B04 RID: 6916 RVA: 0x0005E246 File Offset: 0x0005C446
		public virtual float GetDetachmentCostMultiplierOfAgent(Agent agent, IDetachment detachment)
		{
			if (agent.Banner != null)
			{
				return 10f;
			}
			return 1f;
		}

		// Token: 0x06001B05 RID: 6917 RVA: 0x0005E25B File Offset: 0x0005C45B
		public virtual float GetInteractionDistance(Agent agent)
		{
			return 1.5f;
		}

		// Token: 0x06001B06 RID: 6918 RVA: 0x0005E262 File Offset: 0x0005C462
		public virtual float GetMaxCameraZoom(Agent agent)
		{
			return 1f;
		}

		// Token: 0x06001B07 RID: 6919 RVA: 0x0005E269 File Offset: 0x0005C469
		public virtual int GetEffectiveSkill(BasicCharacterObject agentCharacter, IAgentOriginBase agentOrigin, Formation agentFormation, SkillObject skill)
		{
			return agentCharacter.GetSkillValue(skill);
		}

		// Token: 0x06001B08 RID: 6920 RVA: 0x0005E273 File Offset: 0x0005C473
		public virtual int GetEffectiveSkillForWeapon(Agent agent, WeaponComponentData weapon)
		{
			return this.GetEffectiveSkill(agent.Character, agent.Origin, agent.Formation, weapon.RelevantSkill);
		}

		// Token: 0x06001B09 RID: 6921
		public abstract float GetWeaponDamageMultiplier(BasicCharacterObject agentCharacter, IAgentOriginBase agentOrigin, Formation agentFormation, WeaponComponentData weapon);

		// Token: 0x06001B0A RID: 6922
		public abstract float GetKnockBackResistance(Agent agent);

		// Token: 0x06001B0B RID: 6923
		public abstract float GetKnockDownResistance(Agent agent, StrikeType strikeType = StrikeType.Invalid);

		// Token: 0x06001B0C RID: 6924
		public abstract float GetDismountResistance(Agent agent);

		// Token: 0x06001B0D RID: 6925 RVA: 0x0005E293 File Offset: 0x0005C493
		public virtual string GetMissionDebugInfoForAgent(Agent agent)
		{
			return "Debug info not supported in this model";
		}

		// Token: 0x06001B0E RID: 6926 RVA: 0x0005E29C File Offset: 0x0005C49C
		protected int GetMeleeSkill(Agent agent, WeaponComponentData equippedItem, WeaponComponentData secondaryItem)
		{
			SkillObject skillObject = DefaultSkills.Athletics;
			if (equippedItem != null)
			{
				SkillObject relevantSkill = equippedItem.RelevantSkill;
				if (relevantSkill == DefaultSkills.OneHanded || relevantSkill == DefaultSkills.Polearm)
				{
					skillObject = relevantSkill;
				}
				else if (relevantSkill == DefaultSkills.TwoHanded)
				{
					skillObject = ((secondaryItem == null) ? DefaultSkills.TwoHanded : DefaultSkills.OneHanded);
				}
				else
				{
					skillObject = DefaultSkills.OneHanded;
				}
			}
			return this.GetEffectiveSkill(agent.Character, agent.Origin, agent.Formation, skillObject);
		}

		// Token: 0x06001B0F RID: 6927 RVA: 0x0005E308 File Offset: 0x0005C508
		protected float CalculateAILevel(Agent agent, int relevantSkillLevel)
		{
			float difficultyModifier = this.GetDifficultyModifier();
			return MBMath.ClampFloat((float)relevantSkillLevel / 350f * difficultyModifier, 0f, 1f);
		}

		// Token: 0x06001B10 RID: 6928 RVA: 0x0005E338 File Offset: 0x0005C538
		protected void SetAiRelatedProperties(Agent agent, AgentDrivenProperties agentDrivenProperties, WeaponComponentData equippedItem, WeaponComponentData secondaryItem)
		{
			int meleeSkill = this.GetMeleeSkill(agent, equippedItem, secondaryItem);
			SkillObject skillObject = ((equippedItem == null) ? DefaultSkills.Athletics : equippedItem.RelevantSkill);
			int effectiveSkill = this.GetEffectiveSkill(agent.Character, agent.Origin, agent.Formation, skillObject);
			float num = this.CalculateAILevel(agent, meleeSkill);
			float num2 = this.CalculateAILevel(agent, effectiveSkill);
			float num3 = num + agent.Defensiveness;
			agentDrivenProperties.AiRangedHorsebackMissileRange = 0.3f + 0.4f * num2;
			agentDrivenProperties.AiFacingMissileWatch = -0.96f + num * 0.06f;
			agentDrivenProperties.AiFlyingMissileCheckRadius = 8f - 6f * num;
			agentDrivenProperties.AiShootFreq = 0.3f + 0.7f * num2;
			agentDrivenProperties.AiWaitBeforeShootFactor = (agent.PropertyModifiers.resetAiWaitBeforeShootFactor ? 0f : (1f - 0.5f * num2));
			agentDrivenProperties.AIBlockOnDecideAbility = MBMath.Lerp(0.25f, 0.99f, MBMath.ClampFloat(num, 0f, 1f), 1E-05f);
			agentDrivenProperties.AIParryOnDecideAbility = MBMath.Lerp(0.01f, 0.95f, MBMath.ClampFloat(MathF.Pow(num, 1.5f), 0f, 1f), 1E-05f);
			agentDrivenProperties.AiTryChamberAttackOnDecide = (num - 0.15f) * 0.1f;
			agentDrivenProperties.AIAttackOnParryChance = 0.3f - 0.1f * agent.Defensiveness;
			agentDrivenProperties.AiAttackOnParryTiming = -0.2f + 0.3f * num;
			agentDrivenProperties.AIDecideOnAttackChance = 0.15f * agent.Defensiveness;
			agentDrivenProperties.AIParryOnAttackAbility = MBMath.ClampFloat(num * num * num, 0f, 1f);
			agentDrivenProperties.AiKick = -0.1f + ((num > 0.4f) ? 0.4f : num);
			agentDrivenProperties.AiAttackCalculationMaxTimeFactor = num;
			agentDrivenProperties.AiDecideOnAttackWhenReceiveHitTiming = -0.25f * (1f - num);
			agentDrivenProperties.AiDecideOnAttackContinueAction = -0.5f * (1f - num);
			agentDrivenProperties.AiDecideOnAttackingContinue = 0.1f * num;
			agentDrivenProperties.AIParryOnAttackingContinueAbility = MBMath.Lerp(0.05f, 0.95f, MBMath.ClampFloat(num * num * num, 0f, 1f), 1E-05f);
			agentDrivenProperties.AIDecideOnRealizeEnemyBlockingAttackAbility = 0.5f * MBMath.ClampFloat(MathF.Pow(num, 2.5f) - 0.1f, 0f, 1f);
			agentDrivenProperties.AIRealizeBlockingFromIncorrectSideAbility = 0.5f * MBMath.ClampFloat(MathF.Pow(num, 2.5f) - 0.1f, 0f, 1f);
			agentDrivenProperties.AiAttackingShieldDefenseChance = 0.2f + 0.3f * num;
			agentDrivenProperties.AiAttackingShieldDefenseTimer = -0.3f + 0.3f * num;
			agentDrivenProperties.AiRandomizedDefendDirectionChance = 1f - MathF.Log(num * 7f + 1f, 2f) * 0.33333f;
			agentDrivenProperties.AiShooterError = 0.008f;
			agentDrivenProperties.AISetNoAttackTimerAfterBeingHitAbility = MBMath.ClampFloat(num * num, 0.05f, 0.95f);
			agentDrivenProperties.AISetNoAttackTimerAfterBeingParriedAbility = MBMath.ClampFloat(num * num, 0.05f, 0.95f);
			agentDrivenProperties.AISetNoDefendTimerAfterHittingAbility = MBMath.ClampFloat(num * num, 0.05f, 0.95f);
			agentDrivenProperties.AISetNoDefendTimerAfterParryingAbility = MBMath.ClampFloat(num * num, 0.05f, 0.95f);
			agentDrivenProperties.AIEstimateStunDurationPrecision = 1f - MBMath.ClampFloat(num * num, 0.05f, 0.95f);
			agentDrivenProperties.AIHoldingReadyMaxDuration = MBMath.Lerp(0.25f, 0f, MathF.Min(1f, num * 1.2f), 1E-05f);
			agentDrivenProperties.AIHoldingReadyVariationPercentage = num;
			agentDrivenProperties.AiRaiseShieldDelayTimeBase = -0.75f + 0.5f * num;
			agentDrivenProperties.AiUseShieldAgainstEnemyMissileProbability = 0.1f + num * 0.6f + num3 * 0.2f;
			agentDrivenProperties.AiCheckMovementIntervalFactor = 0.005f * (1.1f - num);
			agentDrivenProperties.AiMovementDelayFactor = 4f / (3f + num2);
			agentDrivenProperties.AiParryDecisionChangeValue = 0.05f + 0.7f * num;
			agentDrivenProperties.AiDefendWithShieldDecisionChanceValue = MathF.Min(1f, 0.2f + 0.5f * num + 0.2f * num3);
			agentDrivenProperties.AiMoveEnemySideTimeValue = -2.5f + 0.5f * num;
			agentDrivenProperties.AiMinimumDistanceToContinueFactor = 2f + 0.3f * (3f - num);
			agentDrivenProperties.AiHearingDistanceFactor = 1f + num;
			agentDrivenProperties.AiChargeHorsebackTargetDistFactor = 1.5f * (3f - num);
			agentDrivenProperties.AiWaitBeforeShootFactor = (agent.PropertyModifiers.resetAiWaitBeforeShootFactor ? 0f : (1f - 0.5f * num2));
			float num4 = 1f - num2;
			agentDrivenProperties.AiRangerLeadErrorMin = -num4 * 0.35f;
			agentDrivenProperties.AiRangerLeadErrorMax = num4 * 0.2f;
			agentDrivenProperties.AiRangerVerticalErrorMultiplier = num4 * 0.1f;
			agentDrivenProperties.AiRangerHorizontalErrorMultiplier = num4 * 0.034906585f;
			agentDrivenProperties.AIAttackOnDecideChance = MathF.Clamp(0.23f * this.CalculateAIAttackOnDecideMaxValue() * (3f - agent.Defensiveness), 0.05f, 1f);
			agentDrivenProperties.SetStat(DrivenProperty.UseRealisticBlocking, (agent.Controller != Agent.ControllerType.Player) ? 1f : 0f);
		}

		// Token: 0x06001B11 RID: 6929 RVA: 0x0005E84D File Offset: 0x0005CA4D
		protected void SetAllWeaponInaccuracy(Agent agent, AgentDrivenProperties agentDrivenProperties, int equippedIndex, WeaponComponentData equippedWeaponComponent)
		{
			if (equippedWeaponComponent != null)
			{
				agentDrivenProperties.WeaponInaccuracy = this.GetWeaponInaccuracy(agent, equippedWeaponComponent, this.GetEffectiveSkillForWeapon(agent, equippedWeaponComponent));
				return;
			}
			agentDrivenProperties.WeaponInaccuracy = 0f;
		}

		// Token: 0x040008D8 RID: 2264
		protected const float MaxHorizontalErrorRadian = 0.034906585f;
	}
}
