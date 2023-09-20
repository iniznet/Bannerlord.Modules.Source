using System;
using System.Runtime.InteropServices;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	[EngineStruct("Attack_collision_data", false)]
	public struct AttackCollisionData
	{
		public bool AttackBlockedWithShield
		{
			get
			{
				return this._attackBlockedWithShield;
			}
		}

		public bool CorrectSideShieldBlock
		{
			get
			{
				return this._correctSideShieldBlock;
			}
		}

		public bool IsAlternativeAttack
		{
			get
			{
				return this._isAlternativeAttack;
			}
		}

		public bool IsColliderAgent
		{
			get
			{
				return this._isColliderAgent;
			}
		}

		public bool CollidedWithShieldOnBack
		{
			get
			{
				return this._collidedWithShieldOnBack;
			}
		}

		public bool IsMissile
		{
			get
			{
				return this._isMissile;
			}
		}

		public bool MissileBlockedWithWeapon
		{
			get
			{
				return this._missileBlockedWithWeapon;
			}
		}

		public bool MissileHasPhysics
		{
			get
			{
				return this._missileHasPhysics;
			}
		}

		public bool EntityExists
		{
			get
			{
				return this._entityExists;
			}
		}

		public bool ThrustTipHit
		{
			get
			{
				return this._thrustTipHit;
			}
		}

		public bool MissileGoneUnderWater
		{
			get
			{
				return this._missileGoneUnderWater;
			}
		}

		public bool MissileGoneOutOfBorder
		{
			get
			{
				return this._missileGoneOutOfBorder;
			}
		}

		public bool IsHorseCharge
		{
			get
			{
				return this.ChargeVelocity > 0f;
			}
		}

		public bool IsFallDamage
		{
			get
			{
				return this.FallSpeed > 0f;
			}
		}

		public CombatCollisionResult CollisionResult
		{
			get
			{
				return (CombatCollisionResult)this._collisionResult;
			}
		}

		public int AffectorWeaponSlotOrMissileIndex { get; }

		public int StrikeType { get; }

		public int DamageType { get; }

		public sbyte CollisionBoneIndex { get; private set; }

		public BoneBodyPartType VictimHitBodyPart { get; }

		public sbyte AttackBoneIndex { get; private set; }

		public Agent.UsageDirection AttackDirection { get; }

		public int PhysicsMaterialIndex { get; private set; }

		public CombatHitResultFlags CollisionHitResultFlags { get; private set; }

		public float AttackProgress { get; }

		public float CollisionDistanceOnWeapon { get; }

		public float AttackerStunPeriod { get; set; }

		public float DefenderStunPeriod { get; set; }

		public float MissileTotalDamage { get; }

		public float MissileStartingBaseSpeed { get; }

		public float ChargeVelocity { get; }

		public float FallSpeed { get; private set; }

		public Vec3 WeaponRotUp { get; }

		public Vec3 WeaponBlowDir
		{
			get
			{
				return this._weaponBlowDir;
			}
		}

		public Vec3 CollisionGlobalPosition { get; private set; }

		public Vec3 MissileVelocity { get; }

		public Vec3 MissileStartingPosition { get; }

		public Vec3 VictimAgentCurVelocity { get; }

		public Vec3 CollisionGlobalNormal { get; }

		public void SetCollisionBoneIndexForAreaDamage(sbyte boneIndex)
		{
			this.CollisionBoneIndex = boneIndex;
		}

		public void UpdateCollisionPositionAndBoneForReflect(int inflictedDamage, Vec3 position, sbyte boneIndex)
		{
			this.InflictedDamage = inflictedDamage;
			this.CollisionGlobalPosition = position;
			this.AttackBoneIndex = boneIndex;
		}

		private AttackCollisionData(bool attackBlockedWithShield, bool correctSideShieldBlock, bool isAlternativeAttack, bool isColliderAgent, bool collidedWithShieldOnBack, bool isMissile, bool missileBlockedWithWeapon, bool missileHasPhysics, bool entityExists, bool thrustTipHit, bool missileGoneUnderWater, bool missileGoneOutOfBorder, CombatCollisionResult collisionResult, int affectorWeaponSlotOrMissileIndex, int StrikeType, int DamageType, sbyte CollisionBoneIndex, BoneBodyPartType VictimHitBodyPart, sbyte AttackBoneIndex, Agent.UsageDirection AttackDirection, int PhysicsMaterialIndex, CombatHitResultFlags CollisionHitResultFlags, float AttackProgress, float CollisionDistanceOnWeapon, float AttackerStunPeriod, float DefenderStunPeriod, float MissileTotalDamage, float MissileStartingBaseSpeed, float ChargeVelocity, float FallSpeed, Vec3 WeaponRotUp, Vec3 weaponBlowDir, Vec3 CollisionGlobalPosition, Vec3 MissileVelocity, Vec3 MissileStartingPosition, Vec3 VictimAgentCurVelocity, Vec3 GroundNormal)
		{
			this._attackBlockedWithShield = attackBlockedWithShield;
			this._correctSideShieldBlock = correctSideShieldBlock;
			this._isAlternativeAttack = isAlternativeAttack;
			this._isColliderAgent = isColliderAgent;
			this._collidedWithShieldOnBack = collidedWithShieldOnBack;
			this._isMissile = isMissile;
			this._missileBlockedWithWeapon = missileBlockedWithWeapon;
			this._missileHasPhysics = missileHasPhysics;
			this._entityExists = entityExists;
			this._thrustTipHit = thrustTipHit;
			this._missileGoneUnderWater = missileGoneUnderWater;
			this._missileGoneOutOfBorder = missileGoneOutOfBorder;
			this._collisionResult = (int)collisionResult;
			this.AffectorWeaponSlotOrMissileIndex = affectorWeaponSlotOrMissileIndex;
			this.StrikeType = StrikeType;
			this.DamageType = DamageType;
			this.CollisionBoneIndex = CollisionBoneIndex;
			this.VictimHitBodyPart = VictimHitBodyPart;
			this.AttackBoneIndex = AttackBoneIndex;
			this.AttackDirection = AttackDirection;
			this.PhysicsMaterialIndex = PhysicsMaterialIndex;
			this.CollisionHitResultFlags = CollisionHitResultFlags;
			this.AttackProgress = AttackProgress;
			this.CollisionDistanceOnWeapon = CollisionDistanceOnWeapon;
			this.AttackerStunPeriod = AttackerStunPeriod;
			this.DefenderStunPeriod = DefenderStunPeriod;
			this.MissileTotalDamage = MissileTotalDamage;
			this.MissileStartingBaseSpeed = MissileStartingBaseSpeed;
			this.ChargeVelocity = ChargeVelocity;
			this.FallSpeed = FallSpeed;
			this.WeaponRotUp = WeaponRotUp;
			this._weaponBlowDir = weaponBlowDir;
			this.CollisionGlobalPosition = CollisionGlobalPosition;
			this.MissileVelocity = MissileVelocity;
			this.MissileStartingPosition = MissileStartingPosition;
			this.VictimAgentCurVelocity = VictimAgentCurVelocity;
			this.CollisionGlobalNormal = GroundNormal;
			this.BaseMagnitude = 0f;
			this.MovementSpeedDamageModifier = 0f;
			this.AbsorbedByArmor = 0;
			this.InflictedDamage = 0;
			this.SelfInflictedDamage = 0;
			this.IsShieldBroken = false;
		}

		public static AttackCollisionData GetAttackCollisionDataForDebugPurpose(bool _attackBlockedWithShield, bool _correctSideShieldBlock, bool _isAlternativeAttack, bool _isColliderAgent, bool _collidedWithShieldOnBack, bool _isMissile, bool _isMissileBlockedWithWeapon, bool _missileHasPhysics, bool _entityExists, bool _thrustTipHit, bool _missileGoneUnderWater, bool _missileGoneOutOfBorder, CombatCollisionResult collisionResult, int affectorWeaponSlotOrMissileIndex, int StrikeType, int DamageType, sbyte CollisionBoneIndex, BoneBodyPartType VictimHitBodyPart, sbyte AttackBoneIndex, Agent.UsageDirection AttackDirection, int PhysicsMaterialIndex, CombatHitResultFlags CollisionHitResultFlags, float AttackProgress, float CollisionDistanceOnWeapon, float AttackerStunPeriod, float DefenderStunPeriod, float MissileTotalDamage, float MissileInitialSpeed, float ChargeVelocity, float FallSpeed, Vec3 WeaponRotUp, Vec3 _weaponBlowDir, Vec3 CollisionGlobalPosition, Vec3 MissileVelocity, Vec3 MissileStartingPosition, Vec3 VictimAgentCurVelocity, Vec3 GroundNormal)
		{
			return new AttackCollisionData(_attackBlockedWithShield, _correctSideShieldBlock, _isAlternativeAttack, _isColliderAgent, _collidedWithShieldOnBack, _isMissile, _isMissileBlockedWithWeapon, _missileHasPhysics, _entityExists, _thrustTipHit, _missileGoneUnderWater, _missileGoneOutOfBorder, collisionResult, affectorWeaponSlotOrMissileIndex, StrikeType, DamageType, CollisionBoneIndex, VictimHitBodyPart, AttackBoneIndex, AttackDirection, PhysicsMaterialIndex, CollisionHitResultFlags, AttackProgress, CollisionDistanceOnWeapon, AttackerStunPeriod, DefenderStunPeriod, MissileTotalDamage, MissileInitialSpeed, ChargeVelocity, FallSpeed, WeaponRotUp, _weaponBlowDir, CollisionGlobalPosition, MissileVelocity, MissileStartingPosition, VictimAgentCurVelocity, GroundNormal);
		}

		[MarshalAs(UnmanagedType.U1)]
		private bool _attackBlockedWithShield;

		[MarshalAs(UnmanagedType.U1)]
		private bool _correctSideShieldBlock;

		[MarshalAs(UnmanagedType.U1)]
		private bool _isAlternativeAttack;

		[MarshalAs(UnmanagedType.U1)]
		private bool _isColliderAgent;

		[MarshalAs(UnmanagedType.U1)]
		private bool _collidedWithShieldOnBack;

		[MarshalAs(UnmanagedType.U1)]
		private bool _isMissile;

		[MarshalAs(UnmanagedType.U1)]
		private bool _missileBlockedWithWeapon;

		[MarshalAs(UnmanagedType.U1)]
		private bool _missileHasPhysics;

		[MarshalAs(UnmanagedType.U1)]
		private bool _entityExists;

		[MarshalAs(UnmanagedType.U1)]
		private bool _thrustTipHit;

		[MarshalAs(UnmanagedType.U1)]
		private bool _missileGoneUnderWater;

		[MarshalAs(UnmanagedType.U1)]
		private bool _missileGoneOutOfBorder;

		private int _collisionResult;

		private Vec3 _weaponBlowDir;

		[CustomEngineStructMemberData(true)]
		public float BaseMagnitude;

		[CustomEngineStructMemberData(true)]
		public float MovementSpeedDamageModifier;

		[CustomEngineStructMemberData(true)]
		public int AbsorbedByArmor;

		[CustomEngineStructMemberData(true)]
		public int InflictedDamage;

		[CustomEngineStructMemberData(true)]
		public int SelfInflictedDamage;

		[CustomEngineStructMemberData(true)]
		[MarshalAs(UnmanagedType.U1)]
		public bool IsShieldBroken;
	}
}
