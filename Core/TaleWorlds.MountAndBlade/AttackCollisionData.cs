using System;
using System.Runtime.InteropServices;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200017E RID: 382
	[EngineStruct("Attack_collision_data")]
	public struct AttackCollisionData
	{
		// Token: 0x17000440 RID: 1088
		// (get) Token: 0x06001376 RID: 4982 RVA: 0x0004CE13 File Offset: 0x0004B013
		public bool AttackBlockedWithShield
		{
			get
			{
				return this._attackBlockedWithShield;
			}
		}

		// Token: 0x17000441 RID: 1089
		// (get) Token: 0x06001377 RID: 4983 RVA: 0x0004CE1B File Offset: 0x0004B01B
		public bool CorrectSideShieldBlock
		{
			get
			{
				return this._correctSideShieldBlock;
			}
		}

		// Token: 0x17000442 RID: 1090
		// (get) Token: 0x06001378 RID: 4984 RVA: 0x0004CE23 File Offset: 0x0004B023
		public bool IsAlternativeAttack
		{
			get
			{
				return this._isAlternativeAttack;
			}
		}

		// Token: 0x17000443 RID: 1091
		// (get) Token: 0x06001379 RID: 4985 RVA: 0x0004CE2B File Offset: 0x0004B02B
		public bool IsColliderAgent
		{
			get
			{
				return this._isColliderAgent;
			}
		}

		// Token: 0x17000444 RID: 1092
		// (get) Token: 0x0600137A RID: 4986 RVA: 0x0004CE33 File Offset: 0x0004B033
		public bool CollidedWithShieldOnBack
		{
			get
			{
				return this._collidedWithShieldOnBack;
			}
		}

		// Token: 0x17000445 RID: 1093
		// (get) Token: 0x0600137B RID: 4987 RVA: 0x0004CE3B File Offset: 0x0004B03B
		public bool IsMissile
		{
			get
			{
				return this._isMissile;
			}
		}

		// Token: 0x17000446 RID: 1094
		// (get) Token: 0x0600137C RID: 4988 RVA: 0x0004CE43 File Offset: 0x0004B043
		public bool MissileBlockedWithWeapon
		{
			get
			{
				return this._missileBlockedWithWeapon;
			}
		}

		// Token: 0x17000447 RID: 1095
		// (get) Token: 0x0600137D RID: 4989 RVA: 0x0004CE4B File Offset: 0x0004B04B
		public bool MissileHasPhysics
		{
			get
			{
				return this._missileHasPhysics;
			}
		}

		// Token: 0x17000448 RID: 1096
		// (get) Token: 0x0600137E RID: 4990 RVA: 0x0004CE53 File Offset: 0x0004B053
		public bool EntityExists
		{
			get
			{
				return this._entityExists;
			}
		}

		// Token: 0x17000449 RID: 1097
		// (get) Token: 0x0600137F RID: 4991 RVA: 0x0004CE5B File Offset: 0x0004B05B
		public bool ThrustTipHit
		{
			get
			{
				return this._thrustTipHit;
			}
		}

		// Token: 0x1700044A RID: 1098
		// (get) Token: 0x06001380 RID: 4992 RVA: 0x0004CE63 File Offset: 0x0004B063
		public bool MissileGoneUnderWater
		{
			get
			{
				return this._missileGoneUnderWater;
			}
		}

		// Token: 0x1700044B RID: 1099
		// (get) Token: 0x06001381 RID: 4993 RVA: 0x0004CE6B File Offset: 0x0004B06B
		public bool MissileGoneOutOfBorder
		{
			get
			{
				return this._missileGoneOutOfBorder;
			}
		}

		// Token: 0x1700044C RID: 1100
		// (get) Token: 0x06001382 RID: 4994 RVA: 0x0004CE73 File Offset: 0x0004B073
		public bool IsHorseCharge
		{
			get
			{
				return this.ChargeVelocity > 0f;
			}
		}

		// Token: 0x1700044D RID: 1101
		// (get) Token: 0x06001383 RID: 4995 RVA: 0x0004CE82 File Offset: 0x0004B082
		public bool IsFallDamage
		{
			get
			{
				return this.FallSpeed > 0f;
			}
		}

		// Token: 0x1700044E RID: 1102
		// (get) Token: 0x06001384 RID: 4996 RVA: 0x0004CE91 File Offset: 0x0004B091
		public CombatCollisionResult CollisionResult
		{
			get
			{
				return (CombatCollisionResult)this._collisionResult;
			}
		}

		// Token: 0x1700044F RID: 1103
		// (get) Token: 0x06001385 RID: 4997 RVA: 0x0004CE99 File Offset: 0x0004B099
		public int AffectorWeaponSlotOrMissileIndex { get; }

		// Token: 0x17000450 RID: 1104
		// (get) Token: 0x06001386 RID: 4998 RVA: 0x0004CEA1 File Offset: 0x0004B0A1
		public int StrikeType { get; }

		// Token: 0x17000451 RID: 1105
		// (get) Token: 0x06001387 RID: 4999 RVA: 0x0004CEA9 File Offset: 0x0004B0A9
		public int DamageType { get; }

		// Token: 0x17000452 RID: 1106
		// (get) Token: 0x06001388 RID: 5000 RVA: 0x0004CEB1 File Offset: 0x0004B0B1
		// (set) Token: 0x06001389 RID: 5001 RVA: 0x0004CEB9 File Offset: 0x0004B0B9
		public sbyte CollisionBoneIndex { get; private set; }

		// Token: 0x17000453 RID: 1107
		// (get) Token: 0x0600138A RID: 5002 RVA: 0x0004CEC2 File Offset: 0x0004B0C2
		public BoneBodyPartType VictimHitBodyPart { get; }

		// Token: 0x17000454 RID: 1108
		// (get) Token: 0x0600138B RID: 5003 RVA: 0x0004CECA File Offset: 0x0004B0CA
		// (set) Token: 0x0600138C RID: 5004 RVA: 0x0004CED2 File Offset: 0x0004B0D2
		public sbyte AttackBoneIndex { get; private set; }

		// Token: 0x17000455 RID: 1109
		// (get) Token: 0x0600138D RID: 5005 RVA: 0x0004CEDB File Offset: 0x0004B0DB
		public Agent.UsageDirection AttackDirection { get; }

		// Token: 0x17000456 RID: 1110
		// (get) Token: 0x0600138E RID: 5006 RVA: 0x0004CEE3 File Offset: 0x0004B0E3
		// (set) Token: 0x0600138F RID: 5007 RVA: 0x0004CEEB File Offset: 0x0004B0EB
		public int PhysicsMaterialIndex { get; private set; }

		// Token: 0x17000457 RID: 1111
		// (get) Token: 0x06001390 RID: 5008 RVA: 0x0004CEF4 File Offset: 0x0004B0F4
		// (set) Token: 0x06001391 RID: 5009 RVA: 0x0004CEFC File Offset: 0x0004B0FC
		public CombatHitResultFlags CollisionHitResultFlags { get; private set; }

		// Token: 0x17000458 RID: 1112
		// (get) Token: 0x06001392 RID: 5010 RVA: 0x0004CF05 File Offset: 0x0004B105
		public float AttackProgress { get; }

		// Token: 0x17000459 RID: 1113
		// (get) Token: 0x06001393 RID: 5011 RVA: 0x0004CF0D File Offset: 0x0004B10D
		public float CollisionDistanceOnWeapon { get; }

		// Token: 0x1700045A RID: 1114
		// (get) Token: 0x06001394 RID: 5012 RVA: 0x0004CF15 File Offset: 0x0004B115
		// (set) Token: 0x06001395 RID: 5013 RVA: 0x0004CF1D File Offset: 0x0004B11D
		public float AttackerStunPeriod { get; set; }

		// Token: 0x1700045B RID: 1115
		// (get) Token: 0x06001396 RID: 5014 RVA: 0x0004CF26 File Offset: 0x0004B126
		// (set) Token: 0x06001397 RID: 5015 RVA: 0x0004CF2E File Offset: 0x0004B12E
		public float DefenderStunPeriod { get; set; }

		// Token: 0x1700045C RID: 1116
		// (get) Token: 0x06001398 RID: 5016 RVA: 0x0004CF37 File Offset: 0x0004B137
		public float MissileTotalDamage { get; }

		// Token: 0x1700045D RID: 1117
		// (get) Token: 0x06001399 RID: 5017 RVA: 0x0004CF3F File Offset: 0x0004B13F
		public float MissileStartingBaseSpeed { get; }

		// Token: 0x1700045E RID: 1118
		// (get) Token: 0x0600139A RID: 5018 RVA: 0x0004CF47 File Offset: 0x0004B147
		public float ChargeVelocity { get; }

		// Token: 0x1700045F RID: 1119
		// (get) Token: 0x0600139B RID: 5019 RVA: 0x0004CF4F File Offset: 0x0004B14F
		// (set) Token: 0x0600139C RID: 5020 RVA: 0x0004CF57 File Offset: 0x0004B157
		public float FallSpeed { get; private set; }

		// Token: 0x17000460 RID: 1120
		// (get) Token: 0x0600139D RID: 5021 RVA: 0x0004CF60 File Offset: 0x0004B160
		public Vec3 WeaponRotUp { get; }

		// Token: 0x17000461 RID: 1121
		// (get) Token: 0x0600139E RID: 5022 RVA: 0x0004CF68 File Offset: 0x0004B168
		public Vec3 WeaponBlowDir
		{
			get
			{
				return this._weaponBlowDir;
			}
		}

		// Token: 0x17000462 RID: 1122
		// (get) Token: 0x0600139F RID: 5023 RVA: 0x0004CF70 File Offset: 0x0004B170
		// (set) Token: 0x060013A0 RID: 5024 RVA: 0x0004CF78 File Offset: 0x0004B178
		public Vec3 CollisionGlobalPosition { get; private set; }

		// Token: 0x17000463 RID: 1123
		// (get) Token: 0x060013A1 RID: 5025 RVA: 0x0004CF81 File Offset: 0x0004B181
		public Vec3 MissileVelocity { get; }

		// Token: 0x17000464 RID: 1124
		// (get) Token: 0x060013A2 RID: 5026 RVA: 0x0004CF89 File Offset: 0x0004B189
		public Vec3 MissileStartingPosition { get; }

		// Token: 0x17000465 RID: 1125
		// (get) Token: 0x060013A3 RID: 5027 RVA: 0x0004CF91 File Offset: 0x0004B191
		public Vec3 VictimAgentCurVelocity { get; }

		// Token: 0x17000466 RID: 1126
		// (get) Token: 0x060013A4 RID: 5028 RVA: 0x0004CF99 File Offset: 0x0004B199
		public Vec3 CollisionGlobalNormal { get; }

		// Token: 0x060013A5 RID: 5029 RVA: 0x0004CFA1 File Offset: 0x0004B1A1
		public void SetCollisionBoneIndexForAreaDamage(sbyte boneIndex)
		{
			this.CollisionBoneIndex = boneIndex;
		}

		// Token: 0x060013A6 RID: 5030 RVA: 0x0004CFAA File Offset: 0x0004B1AA
		public void UpdateCollisionPositionAndBoneForReflect(int inflictedDamage, Vec3 position, sbyte boneIndex)
		{
			this.InflictedDamage = inflictedDamage;
			this.CollisionGlobalPosition = position;
			this.AttackBoneIndex = boneIndex;
		}

		// Token: 0x060013A7 RID: 5031 RVA: 0x0004CFC4 File Offset: 0x0004B1C4
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

		// Token: 0x060013A8 RID: 5032 RVA: 0x0004D128 File Offset: 0x0004B328
		public static AttackCollisionData GetAttackCollisionDataForDebugPurpose(bool _attackBlockedWithShield, bool _correctSideShieldBlock, bool _isAlternativeAttack, bool _isColliderAgent, bool _collidedWithShieldOnBack, bool _isMissile, bool _isMissileBlockedWithWeapon, bool _missileHasPhysics, bool _entityExists, bool _thrustTipHit, bool _missileGoneUnderWater, bool _missileGoneOutOfBorder, CombatCollisionResult collisionResult, int affectorWeaponSlotOrMissileIndex, int StrikeType, int DamageType, sbyte CollisionBoneIndex, BoneBodyPartType VictimHitBodyPart, sbyte AttackBoneIndex, Agent.UsageDirection AttackDirection, int PhysicsMaterialIndex, CombatHitResultFlags CollisionHitResultFlags, float AttackProgress, float CollisionDistanceOnWeapon, float AttackerStunPeriod, float DefenderStunPeriod, float MissileTotalDamage, float MissileInitialSpeed, float ChargeVelocity, float FallSpeed, Vec3 WeaponRotUp, Vec3 _weaponBlowDir, Vec3 CollisionGlobalPosition, Vec3 MissileVelocity, Vec3 MissileStartingPosition, Vec3 VictimAgentCurVelocity, Vec3 GroundNormal)
		{
			return new AttackCollisionData(_attackBlockedWithShield, _correctSideShieldBlock, _isAlternativeAttack, _isColliderAgent, _collidedWithShieldOnBack, _isMissile, _isMissileBlockedWithWeapon, _missileHasPhysics, _entityExists, _thrustTipHit, _missileGoneUnderWater, _missileGoneOutOfBorder, collisionResult, affectorWeaponSlotOrMissileIndex, StrikeType, DamageType, CollisionBoneIndex, VictimHitBodyPart, AttackBoneIndex, AttackDirection, PhysicsMaterialIndex, CollisionHitResultFlags, AttackProgress, CollisionDistanceOnWeapon, AttackerStunPeriod, DefenderStunPeriod, MissileTotalDamage, MissileInitialSpeed, ChargeVelocity, FallSpeed, WeaponRotUp, _weaponBlowDir, CollisionGlobalPosition, MissileVelocity, MissileStartingPosition, VictimAgentCurVelocity, GroundNormal);
		}

		// Token: 0x040005EB RID: 1515
		[MarshalAs(UnmanagedType.I1)]
		private bool _attackBlockedWithShield;

		// Token: 0x040005EC RID: 1516
		[MarshalAs(UnmanagedType.I1)]
		private bool _correctSideShieldBlock;

		// Token: 0x040005ED RID: 1517
		[MarshalAs(UnmanagedType.I1)]
		private bool _isAlternativeAttack;

		// Token: 0x040005EE RID: 1518
		[MarshalAs(UnmanagedType.I1)]
		private bool _isColliderAgent;

		// Token: 0x040005EF RID: 1519
		[MarshalAs(UnmanagedType.I1)]
		private bool _collidedWithShieldOnBack;

		// Token: 0x040005F0 RID: 1520
		[MarshalAs(UnmanagedType.I1)]
		private bool _isMissile;

		// Token: 0x040005F1 RID: 1521
		[MarshalAs(UnmanagedType.I1)]
		private bool _missileBlockedWithWeapon;

		// Token: 0x040005F2 RID: 1522
		[MarshalAs(UnmanagedType.I1)]
		private bool _missileHasPhysics;

		// Token: 0x040005F3 RID: 1523
		[MarshalAs(UnmanagedType.I1)]
		private bool _entityExists;

		// Token: 0x040005F4 RID: 1524
		[MarshalAs(UnmanagedType.I1)]
		private bool _thrustTipHit;

		// Token: 0x040005F5 RID: 1525
		[MarshalAs(UnmanagedType.I1)]
		private bool _missileGoneUnderWater;

		// Token: 0x040005F6 RID: 1526
		[MarshalAs(UnmanagedType.I1)]
		private bool _missileGoneOutOfBorder;

		// Token: 0x040005F7 RID: 1527
		private int _collisionResult;

		// Token: 0x0400060A RID: 1546
		private Vec3 _weaponBlowDir;

		// Token: 0x04000610 RID: 1552
		public float BaseMagnitude;

		// Token: 0x04000611 RID: 1553
		public float MovementSpeedDamageModifier;

		// Token: 0x04000612 RID: 1554
		public int AbsorbedByArmor;

		// Token: 0x04000613 RID: 1555
		public int InflictedDamage;

		// Token: 0x04000614 RID: 1556
		public int SelfInflictedDamage;

		// Token: 0x04000615 RID: 1557
		[MarshalAs(UnmanagedType.I1)]
		public bool IsShieldBroken;
	}
}
