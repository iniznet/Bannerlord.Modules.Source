using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.ComponentInterfaces;

namespace TaleWorlds.MountAndBlade
{
	public struct AttackInformation
	{
		public AttackInformation(Agent attackerAgent, Agent victimAgent, GameEntity hitObject, in AttackCollisionData attackCollisionData, in MissionWeapon attackerWeapon)
		{
			this.AttackerAgent = attackerAgent;
			this.VictimAgent = victimAgent;
			this.IsAttackerAgentNull = attackerAgent == null;
			this.IsVictimAgentNull = victimAgent == null;
			this.ArmorAmountFloat = 0f;
			AttackCollisionData attackCollisionData2;
			if (!this.IsVictimAgentNull)
			{
				attackCollisionData2 = attackCollisionData;
				this.ArmorAmountFloat = victimAgent.GetBaseArmorEffectivenessForBodyPart(attackCollisionData2.VictimHitBodyPart);
			}
			this.ShieldOnBack = null;
			if (!this.IsVictimAgentNull && (victimAgent.GetAgentFlags() & AgentFlag.CanWieldWeapon) != AgentFlag.None)
			{
				EquipmentIndex wieldedItemIndex = victimAgent.GetWieldedItemIndex(Agent.HandIndex.OffHand);
				for (int i = 0; i < 4; i++)
				{
					WeaponComponentData currentUsageItem = victimAgent.Equipment[i].CurrentUsageItem;
					if (i != (int)wieldedItemIndex && currentUsageItem != null && currentUsageItem.IsShield)
					{
						this.ShieldOnBack = currentUsageItem;
						break;
					}
				}
			}
			this.VictimShield = MissionWeapon.Invalid;
			this.VictimMainHandWeapon = MissionWeapon.Invalid;
			if (!this.IsVictimAgentNull && (victimAgent.GetAgentFlags() & AgentFlag.CanWieldWeapon) != AgentFlag.None)
			{
				EquipmentIndex wieldedItemIndex2 = victimAgent.GetWieldedItemIndex(Agent.HandIndex.OffHand);
				if (wieldedItemIndex2 != EquipmentIndex.None)
				{
					this.VictimShield = victimAgent.Equipment[wieldedItemIndex2];
				}
				EquipmentIndex wieldedItemIndex3 = victimAgent.GetWieldedItemIndex(Agent.HandIndex.MainHand);
				if (wieldedItemIndex3 != EquipmentIndex.None)
				{
					this.VictimMainHandWeapon = victimAgent.Equipment[wieldedItemIndex3];
				}
			}
			this.AttackerAgentMountMovementDirection = default(Vec2);
			if (!this.IsAttackerAgentNull && attackerAgent.HasMount)
			{
				this.AttackerAgentMountMovementDirection = attackerAgent.MountAgent.GetMovementDirection();
			}
			this.VictimAgentMountMovementDirection = default(Vec2);
			if (!this.IsVictimAgentNull && victimAgent.HasMount)
			{
				this.VictimAgentMountMovementDirection = victimAgent.MountAgent.GetMovementDirection();
			}
			this.IsVictimAgentSameWithAttackerAgent = !this.IsAttackerAgentNull && attackerAgent == victimAgent;
			MissionWeapon missionWeapon = attackerWeapon;
			int num;
			if (missionWeapon.IsEmpty || this.IsAttackerAgentNull || !attackerAgent.IsHuman)
			{
				num = -1;
			}
			else
			{
				Monster monster = attackerAgent.Monster;
				missionWeapon = attackerWeapon;
				num = (int)monster.GetBoneToAttachForItemFlags(missionWeapon.Item.ItemFlags);
			}
			this.WeaponAttachBoneIndex = num;
			DestructableComponent destructableComponent = ((hitObject != null) ? hitObject.GetFirstScriptOfTypeInFamily<DestructableComponent>() : null);
			this.HitObjectDestructibleComponent = destructableComponent;
			bool flag;
			if (!this.IsAttackerAgentNull)
			{
				if (!this.IsVictimAgentSameWithAttackerAgent && (this.IsVictimAgentNull || !victimAgent.IsFriendOf(attackerAgent)))
				{
					if (destructableComponent != null)
					{
						BattleSideEnum battleSide = destructableComponent.BattleSide;
						Team team = attackerAgent.Team;
						BattleSideEnum? battleSideEnum = ((team != null) ? new BattleSideEnum?(team.Side) : null);
						flag = (battleSide == battleSideEnum.GetValueOrDefault()) & (battleSideEnum != null);
					}
					else
					{
						flag = false;
					}
				}
				else
				{
					flag = true;
				}
			}
			else
			{
				flag = false;
			}
			this.IsFriendlyFire = flag;
			this.OffHandItem = default(MissionWeapon);
			if (!this.IsAttackerAgentNull && (attackerAgent.GetAgentFlags() & AgentFlag.CanWieldWeapon) != AgentFlag.None)
			{
				EquipmentIndex wieldedItemIndex4 = attackerAgent.GetWieldedItemIndex(Agent.HandIndex.OffHand);
				if (wieldedItemIndex4 != EquipmentIndex.None)
				{
					this.OffHandItem = attackerAgent.Equipment[wieldedItemIndex4];
				}
			}
			attackCollisionData2 = attackCollisionData;
			this.IsHeadShot = attackCollisionData2.VictimHitBodyPart == BoneBodyPartType.Head;
			this.VictimAgentAbsorbedDamageRatio = 0f;
			this.DamageMultiplierOfBone = 0f;
			this.VictimMovementDirectionAsAngle = 0f;
			this.VictimAgentScale = 0f;
			this.VictimAgentHealth = 0f;
			this.VictimAgentMaxHealth = 0f;
			this.VictimAgentWeight = 0f;
			this.VictimAgentTotalEncumbrance = 0f;
			this.CombatDifficultyMultiplier = 1f;
			this.VictimHitPointRate = 0f;
			this.VictimAgentFlag = AgentFlag.CanAttack;
			this.IsVictimAgentLeftStance = false;
			this.DoesVictimHaveMountAgent = false;
			this.IsVictimAgentMine = false;
			this.DoesVictimHaveRiderAgent = false;
			this.IsVictimAgentRiderAgentMine = false;
			this.IsVictimAgentMount = false;
			this.IsVictimAgentHuman = false;
			this.IsVictimRiderAgentSameAsAttackerAgent = false;
			this.IsVictimPlayer = false;
			this.VictimAgentCharacter = null;
			this.VictimRiderAgentCharacter = null;
			this.VictimAgentMovementVelocity = default(Vec2);
			this.VictimAgentPosition = default(Vec3);
			this.VictimAgentVelocity = default(Vec3);
			this.VictimCaptainCharacter = null;
			this.VictimAgentOrigin = null;
			this.VictimRiderAgentOrigin = null;
			this.VictimFormation = null;
			if (!this.IsVictimAgentNull)
			{
				this.IsVictimAgentMount = victimAgent.IsMount;
				this.IsVictimAgentMine = victimAgent.IsMine;
				this.IsVictimAgentHuman = victimAgent.IsHuman;
				this.IsVictimAgentLeftStance = victimAgent.GetIsLeftStance();
				this.DoesVictimHaveMountAgent = victimAgent.HasMount;
				this.DoesVictimHaveRiderAgent = victimAgent.RiderAgent != null;
				this.IsVictimRiderAgentSameAsAttackerAgent = this.DoesVictimHaveRiderAgent && victimAgent.RiderAgent == attackerAgent;
				this.IsVictimPlayer = victimAgent.IsPlayerControlled;
				this.VictimAgentAbsorbedDamageRatio = victimAgent.Monster.AbsorbedDamageRatio;
				AgentApplyDamageModel agentApplyDamageModel = MissionGameModels.Current.AgentApplyDamageModel;
				attackCollisionData2 = attackCollisionData;
				BoneBodyPartType boneBodyPartType;
				if (attackCollisionData2.CollisionBoneIndex == -1)
				{
					boneBodyPartType = BoneBodyPartType.None;
				}
				else
				{
					MBAgentVisuals agentVisuals = victimAgent.AgentVisuals;
					attackCollisionData2 = attackCollisionData;
					boneBodyPartType = agentVisuals.GetBoneTypeData(attackCollisionData2.CollisionBoneIndex).BodyPartType;
				}
				attackCollisionData2 = attackCollisionData;
				DamageTypes damageType = (DamageTypes)attackCollisionData2.DamageType;
				bool isVictimAgentHuman = this.IsVictimAgentHuman;
				attackCollisionData2 = attackCollisionData;
				this.DamageMultiplierOfBone = agentApplyDamageModel.GetDamageMultiplierForBodyPart(boneBodyPartType, damageType, isVictimAgentHuman, attackCollisionData2.IsMissile);
				this.VictimMovementDirectionAsAngle = victimAgent.MovementDirectionAsAngle;
				this.VictimAgentScale = victimAgent.AgentScale;
				this.VictimAgentHealth = victimAgent.Health;
				this.VictimAgentMaxHealth = victimAgent.HealthLimit;
				this.VictimAgentWeight = (victimAgent.IsMount ? victimAgent.SpawnEquipment[EquipmentIndex.ArmorItemEndSlot].Weight : ((float)victimAgent.Monster.Weight));
				this.VictimAgentTotalEncumbrance = victimAgent.GetTotalEncumbrance();
				this.CombatDifficultyMultiplier = Mission.Current.GetDamageMultiplierOfCombatDifficulty(victimAgent, attackerAgent);
				this.VictimHitPointRate = victimAgent.Health / victimAgent.HealthLimit;
				this.VictimAgentMovementVelocity = victimAgent.MovementVelocity;
				this.VictimAgentVelocity = victimAgent.Velocity;
				this.VictimAgentPosition = victimAgent.Position;
				this.VictimAgentFlag = victimAgent.GetAgentFlags();
				this.VictimAgentCharacter = victimAgent.Character;
				this.VictimAgentOrigin = victimAgent.Origin;
				if (this.DoesVictimHaveRiderAgent)
				{
					Agent riderAgent = victimAgent.RiderAgent;
					this.IsVictimAgentRiderAgentMine = riderAgent.IsMine;
					this.VictimRiderAgentCharacter = riderAgent.Character;
					this.VictimRiderAgentOrigin = riderAgent.Origin;
					Formation formation = riderAgent.Formation;
					Agent agent = ((formation != null) ? formation.Captain : null);
					this.VictimCaptainCharacter = ((riderAgent != agent) ? ((agent != null) ? agent.Character : null) : null);
					this.VictimFormation = riderAgent.Formation;
				}
				else
				{
					Formation formation2 = victimAgent.Formation;
					Agent agent2 = ((formation2 != null) ? formation2.Captain : null);
					this.VictimCaptainCharacter = ((victimAgent != agent2) ? ((agent2 != null) ? agent2.Character : null) : null);
					this.VictimFormation = victimAgent.Formation;
				}
			}
			this.AttackerMovementDirectionAsAngle = 0f;
			this.AttackerAgentMountChargeDamageProperty = 0f;
			this.DoesAttackerHaveMountAgent = false;
			this.IsAttackerAgentMine = false;
			this.DoesAttackerHaveRiderAgent = false;
			this.IsAttackerAgentRiderAgentMine = false;
			this.IsAttackerAgentMount = false;
			this.IsAttackerAgentHuman = false;
			this.IsAttackerAgentActive = false;
			this.IsAttackerAgentDoingPassiveAttack = false;
			this.IsAttackerPlayer = false;
			this.AttackerAgentMovementVelocity = default(Vec2);
			this.AttackerAgentCharacter = null;
			this.AttackerRiderAgentCharacter = null;
			this.AttackerAgentOrigin = null;
			this.AttackerRiderAgentOrigin = null;
			this.AttackerAgentMovementDirection = default(Vec2);
			this.AttackerAgentVelocity = default(Vec3);
			this.AttackerAgentCurrentWeaponOffset = default(Vec3);
			this.IsAttackerAIControlled = false;
			this.AttackerCaptainCharacter = null;
			this.AttackerFormation = null;
			this.AttackerHitPointRate = 0f;
			if (!this.IsAttackerAgentNull)
			{
				this.DoesAttackerHaveMountAgent = attackerAgent.HasMount;
				this.IsAttackerAgentMine = attackerAgent.IsMine;
				this.IsAttackerAgentMount = attackerAgent.IsMount;
				this.IsAttackerAgentHuman = attackerAgent.IsHuman;
				this.IsAttackerAgentActive = attackerAgent.IsActive();
				this.IsAttackerAgentDoingPassiveAttack = attackerAgent.IsDoingPassiveAttack;
				this.DoesAttackerHaveRiderAgent = attackerAgent.RiderAgent != null;
				this.IsAttackerAIControlled = attackerAgent.IsAIControlled;
				this.IsAttackerPlayer = attackerAgent.IsPlayerControlled;
				this.AttackerMovementDirectionAsAngle = attackerAgent.MovementDirectionAsAngle;
				this.AttackerAgentMountChargeDamageProperty = attackerAgent.GetAgentDrivenPropertyValue(DrivenProperty.MountChargeDamage);
				this.AttackerHitPointRate = attackerAgent.Health / attackerAgent.HealthLimit;
				this.AttackerAgentMovementVelocity = attackerAgent.MovementVelocity;
				this.AttackerAgentMovementDirection = attackerAgent.GetMovementDirection();
				this.AttackerAgentVelocity = attackerAgent.Velocity;
				if (this.IsAttackerAgentActive)
				{
					this.AttackerAgentCurrentWeaponOffset = attackerAgent.GetCurWeaponOffset();
				}
				this.AttackerAgentCharacter = attackerAgent.Character;
				this.AttackerAgentOrigin = attackerAgent.Origin;
				if (this.DoesAttackerHaveRiderAgent)
				{
					Agent riderAgent2 = attackerAgent.RiderAgent;
					this.IsAttackerAgentRiderAgentMine = riderAgent2.IsMine;
					this.AttackerRiderAgentCharacter = riderAgent2.Character;
					this.AttackerRiderAgentOrigin = riderAgent2.Origin;
					Formation formation3 = riderAgent2.Formation;
					Agent agent3 = ((formation3 != null) ? formation3.Captain : null);
					this.AttackerCaptainCharacter = ((riderAgent2 != agent3) ? ((agent3 != null) ? agent3.Character : null) : null);
					this.AttackerFormation = riderAgent2.Formation;
				}
				else
				{
					Formation formation4 = attackerAgent.Formation;
					Agent agent4 = ((formation4 != null) ? formation4.Captain : null);
					this.AttackerCaptainCharacter = ((attackerAgent != agent4) ? ((agent4 != null) ? agent4.Character : null) : null);
					this.AttackerFormation = attackerAgent.Formation;
				}
			}
			this.CanGiveDamageToAgentShield = true;
			if (!this.IsVictimAgentSameWithAttackerAgent)
			{
				Mission mission = Mission.Current;
				missionWeapon = attackerWeapon;
				this.CanGiveDamageToAgentShield = mission.CanGiveDamageToAgentShield(attackerAgent, missionWeapon.CurrentUsageItem, victimAgent);
			}
		}

		public AttackInformation(Agent attackerAgent, Agent victimAgent, float armorAmountFloat, WeaponComponentData shieldOnBack, AgentFlag victimAgentFlag, float victimAgentAbsorbedDamageRatio, float damageMultiplierOfBone, float combatDifficultyMultiplier, MissionWeapon victimMainHandWeapon, MissionWeapon victimShield, bool canGiveDamageToAgentShield, bool isVictimAgentLeftStance, bool isFriendlyFire, bool doesAttackerHaveMountAgent, bool doesVictimHaveMountAgent, Vec2 attackerAgentMovementVelocity, Vec2 attackerAgentMountMovementDirection, float attackerMovementDirectionAsAngle, Vec2 victimAgentMovementVelocity, Vec2 victimAgentMountMovementDirection, float victimMovementDirectionAsAngle, bool isVictimAgentSameWithAttackerAgent, bool isAttackerAgentMine, bool doesAttackerHaveRiderAgent, bool isAttackerAgentRiderAgentMine, bool isAttackerAgentMount, bool isVictimAgentMine, bool doesVictimHaveRiderAgent, bool isVictimAgentRiderAgentMine, bool isVictimAgentMount, bool isAttackerAgentNull, bool isAttackerAIControlled, BasicCharacterObject attackerAgentCharacter, BasicCharacterObject attackerRiderAgentCharacter, IAgentOriginBase attackerAgentOrigin, IAgentOriginBase attackerRiderAgentOrigin, BasicCharacterObject victimAgentCharacter, BasicCharacterObject victimRiderAgentCharacter, IAgentOriginBase victimAgentOrigin, IAgentOriginBase victimRiderAgentOrigin, Vec2 attackerAgentMovementDirection, Vec3 attackerAgentVelocity, float attackerAgentMountChargeDamageProperty, Vec3 attackerAgentCurrentWeaponOffset, bool isAttackerAgentHuman, bool isAttackerAgentActive, bool isAttackerAgentDoingPassiveAttack, bool isVictimAgentNull, float victimAgentScale, float victimAgentHealth, float victimAgentMaxHealth, float victimAgentWeight, float victimAgentTotalEncumbrance, bool isVictimAgentHuman, Vec3 victimAgentVelocity, Vec3 victimAgentPosition, int weaponAttachBoneIndex, MissionWeapon offHandItem, bool isHeadShot, bool isVictimRiderAgentSameAsAttackerAgent, bool isAttackerPlayer, bool isVictimPlayer, DestructableComponent hitObjectDestructibleComponent)
		{
			this.AttackerAgent = attackerAgent;
			this.VictimAgent = victimAgent;
			this.ArmorAmountFloat = armorAmountFloat;
			this.ShieldOnBack = shieldOnBack;
			this.VictimAgentFlag = victimAgentFlag;
			this.VictimAgentAbsorbedDamageRatio = victimAgentAbsorbedDamageRatio;
			this.DamageMultiplierOfBone = damageMultiplierOfBone;
			this.CombatDifficultyMultiplier = combatDifficultyMultiplier;
			this.VictimMainHandWeapon = victimMainHandWeapon;
			this.VictimShield = victimShield;
			this.CanGiveDamageToAgentShield = canGiveDamageToAgentShield;
			this.IsVictimAgentLeftStance = isVictimAgentLeftStance;
			this.IsFriendlyFire = isFriendlyFire;
			this.DoesAttackerHaveMountAgent = doesAttackerHaveMountAgent;
			this.DoesVictimHaveMountAgent = doesVictimHaveMountAgent;
			this.AttackerAgentMovementVelocity = attackerAgentMovementVelocity;
			this.AttackerAgentMountMovementDirection = attackerAgentMountMovementDirection;
			this.AttackerMovementDirectionAsAngle = attackerMovementDirectionAsAngle;
			this.VictimAgentMovementVelocity = victimAgentMovementVelocity;
			this.VictimAgentMountMovementDirection = victimAgentMountMovementDirection;
			this.VictimMovementDirectionAsAngle = victimMovementDirectionAsAngle;
			this.IsVictimAgentSameWithAttackerAgent = isVictimAgentSameWithAttackerAgent;
			this.IsAttackerAgentMine = isAttackerAgentMine;
			this.DoesAttackerHaveRiderAgent = doesAttackerHaveRiderAgent;
			this.IsAttackerAgentRiderAgentMine = isAttackerAgentRiderAgentMine;
			this.IsAttackerAgentMount = isAttackerAgentMount;
			this.IsVictimAgentMine = isVictimAgentMine;
			this.DoesVictimHaveRiderAgent = doesVictimHaveRiderAgent;
			this.IsVictimAgentRiderAgentMine = isVictimAgentRiderAgentMine;
			this.IsVictimAgentMount = isVictimAgentMount;
			this.IsAttackerAgentNull = isAttackerAgentNull;
			this.IsAttackerAIControlled = isAttackerAIControlled;
			this.AttackerAgentCharacter = attackerAgentCharacter;
			this.AttackerRiderAgentCharacter = attackerRiderAgentCharacter;
			this.AttackerAgentOrigin = attackerAgentOrigin;
			this.AttackerRiderAgentOrigin = attackerRiderAgentOrigin;
			this.VictimAgentCharacter = victimAgentCharacter;
			this.VictimRiderAgentCharacter = victimRiderAgentCharacter;
			this.VictimAgentOrigin = victimAgentOrigin;
			this.VictimRiderAgentOrigin = victimRiderAgentOrigin;
			this.AttackerAgentMovementDirection = attackerAgentMovementDirection;
			this.AttackerAgentVelocity = attackerAgentVelocity;
			this.AttackerAgentMountChargeDamageProperty = attackerAgentMountChargeDamageProperty;
			this.AttackerAgentCurrentWeaponOffset = attackerAgentCurrentWeaponOffset;
			this.IsAttackerAgentHuman = isAttackerAgentHuman;
			this.IsAttackerAgentActive = isAttackerAgentActive;
			this.IsAttackerAgentDoingPassiveAttack = isAttackerAgentDoingPassiveAttack;
			this.VictimAgentScale = victimAgentScale;
			this.IsVictimAgentNull = isVictimAgentNull;
			this.VictimAgentHealth = victimAgentHealth;
			this.VictimAgentMaxHealth = victimAgentMaxHealth;
			this.VictimAgentWeight = victimAgentWeight;
			this.VictimAgentTotalEncumbrance = victimAgentTotalEncumbrance;
			this.IsVictimAgentHuman = isVictimAgentHuman;
			this.VictimAgentVelocity = victimAgentVelocity;
			this.VictimAgentPosition = victimAgentPosition;
			this.WeaponAttachBoneIndex = weaponAttachBoneIndex;
			this.OffHandItem = offHandItem;
			this.IsHeadShot = isHeadShot;
			this.IsVictimRiderAgentSameAsAttackerAgent = isVictimRiderAgentSameAsAttackerAgent;
			this.AttackerCaptainCharacter = null;
			this.VictimCaptainCharacter = null;
			this.VictimFormation = null;
			this.AttackerFormation = null;
			this.AttackerHitPointRate = 1f;
			this.VictimHitPointRate = 1f;
			this.IsAttackerPlayer = isAttackerPlayer;
			this.IsVictimPlayer = isVictimPlayer;
			this.HitObjectDestructibleComponent = hitObjectDestructibleComponent;
		}

		public Agent AttackerAgent;

		public Agent VictimAgent;

		public float ArmorAmountFloat;

		public WeaponComponentData ShieldOnBack;

		public AgentFlag VictimAgentFlag;

		public float VictimAgentAbsorbedDamageRatio;

		public float DamageMultiplierOfBone;

		public float CombatDifficultyMultiplier;

		public MissionWeapon VictimMainHandWeapon;

		public MissionWeapon VictimShield;

		public bool CanGiveDamageToAgentShield;

		public bool IsVictimAgentLeftStance;

		public bool IsFriendlyFire;

		public bool DoesAttackerHaveMountAgent;

		public bool DoesVictimHaveMountAgent;

		public Vec2 AttackerAgentMovementVelocity;

		public Vec2 AttackerAgentMountMovementDirection;

		public float AttackerMovementDirectionAsAngle;

		public Vec2 VictimAgentMovementVelocity;

		public Vec2 VictimAgentMountMovementDirection;

		public float VictimMovementDirectionAsAngle;

		public bool IsVictimAgentSameWithAttackerAgent;

		public bool IsAttackerAgentMine;

		public bool DoesAttackerHaveRiderAgent;

		public bool IsAttackerAgentRiderAgentMine;

		public bool IsAttackerAgentMount;

		public bool IsVictimAgentMine;

		public bool DoesVictimHaveRiderAgent;

		public bool IsVictimAgentRiderAgentMine;

		public bool IsVictimAgentMount;

		public bool IsAttackerAgentNull;

		public bool IsAttackerAIControlled;

		public BasicCharacterObject AttackerAgentCharacter;

		public BasicCharacterObject AttackerRiderAgentCharacter;

		public IAgentOriginBase AttackerAgentOrigin;

		public IAgentOriginBase AttackerRiderAgentOrigin;

		public BasicCharacterObject VictimAgentCharacter;

		public BasicCharacterObject VictimRiderAgentCharacter;

		public IAgentOriginBase VictimAgentOrigin;

		public IAgentOriginBase VictimRiderAgentOrigin;

		public Vec2 AttackerAgentMovementDirection;

		public Vec3 AttackerAgentVelocity;

		public float AttackerAgentMountChargeDamageProperty;

		public Vec3 AttackerAgentCurrentWeaponOffset;

		public bool IsAttackerAgentHuman;

		public bool IsAttackerAgentActive;

		public bool IsAttackerAgentDoingPassiveAttack;

		public bool IsVictimAgentNull;

		public float VictimAgentScale;

		public float VictimAgentWeight;

		public float VictimAgentHealth;

		public float VictimAgentMaxHealth;

		public float VictimAgentTotalEncumbrance;

		public bool IsVictimAgentHuman;

		public Vec3 VictimAgentVelocity;

		public Vec3 VictimAgentPosition;

		public int WeaponAttachBoneIndex;

		public MissionWeapon OffHandItem;

		public bool IsHeadShot;

		public bool IsVictimRiderAgentSameAsAttackerAgent;

		public BasicCharacterObject AttackerCaptainCharacter;

		public BasicCharacterObject VictimCaptainCharacter;

		public Formation AttackerFormation;

		public Formation VictimFormation;

		public float AttackerHitPointRate;

		public float VictimHitPointRate;

		public bool IsAttackerPlayer;

		public bool IsVictimPlayer;

		public DestructableComponent HitObjectDestructibleComponent;
	}
}
