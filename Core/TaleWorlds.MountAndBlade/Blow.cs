using System;
using System.Runtime.InteropServices;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	[EngineStruct("Blow", false)]
	public struct Blow
	{
		public Blow(int ownerId)
		{
			this = default(Blow);
			this.OwnerId = ownerId;
		}

		public bool IsMissile
		{
			get
			{
				return this.WeaponRecord.IsMissile;
			}
		}

		public bool IsBlowCrit(int maxHitPointsOfVictim)
		{
			return (float)this.InflictedDamage > (float)maxHitPointsOfVictim * 0.5f;
		}

		public bool IsBlowLow(int maxHitPointsOfVictim)
		{
			return (float)this.InflictedDamage <= (float)maxHitPointsOfVictim * 0.1f;
		}

		public bool IsHeadShot()
		{
			return this.VictimBodyPart == BoneBodyPartType.Head;
		}

		public BlowWeaponRecord WeaponRecord;

		public Vec3 GlobalPosition;

		public Vec3 Direction;

		public Vec3 SwingDirection;

		public int InflictedDamage;

		public int SelfInflictedDamage;

		public float BaseMagnitude;

		public float DefenderStunPeriod;

		public float AttackerStunPeriod;

		public float AbsorbedByArmor;

		public float MovementSpeedDamageModifier;

		public StrikeType StrikeType;

		public AgentAttackType AttackType;

		[CustomEngineStructMemberData("blow_flags")]
		public BlowFlags BlowFlag;

		public int OwnerId;

		public sbyte BoneIndex;

		public BoneBodyPartType VictimBodyPart;

		public DamageTypes DamageType;

		[MarshalAs(UnmanagedType.U1)]
		public bool NoIgnore;

		[MarshalAs(UnmanagedType.U1)]
		public bool DamageCalculated;

		[MarshalAs(UnmanagedType.U1)]
		public bool IsFallDamage;

		public float DamagedPercentage;
	}
}
