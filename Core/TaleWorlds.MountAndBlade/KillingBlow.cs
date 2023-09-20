using System;
using System.Runtime.InteropServices;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	[EngineStruct("Killing_blow", false)]
	public struct KillingBlow
	{
		public KillingBlow(Blow b, Vec3 ragdollImpulsePoint, Vec3 ragdollImpulseAmount, int deathAction, int weaponItemKind, Agent.KillInfo overrideKillInfo = Agent.KillInfo.Invalid)
		{
			this.RagdollImpulseLocalPoint = ragdollImpulsePoint;
			this.RagdollImpulseAmount = ragdollImpulseAmount;
			this.DeathAction = deathAction;
			this.OverrideKillInfo = overrideKillInfo;
			this.DamageType = b.DamageType;
			this.AttackType = b.AttackType;
			this.OwnerId = b.OwnerId;
			this.VictimBodyPart = b.VictimBodyPart;
			this.WeaponClass = (int)b.WeaponRecord.WeaponClass;
			this.BlowPosition = b.GlobalPosition;
			this.WeaponRecordWeaponFlags = b.WeaponRecord.WeaponFlags;
			this.WeaponItemKind = weaponItemKind;
			this.InflictedDamage = b.InflictedDamage;
			this.IsMissile = b.IsMissile;
			this.IsValid = true;
		}

		public bool IsHeadShot()
		{
			return this.VictimBodyPart == BoneBodyPartType.Head;
		}

		public Vec3 RagdollImpulseLocalPoint;

		public Vec3 RagdollImpulseAmount;

		public int DeathAction;

		public DamageTypes DamageType;

		public AgentAttackType AttackType;

		public int OwnerId;

		public BoneBodyPartType VictimBodyPart;

		public int WeaponClass;

		public Agent.KillInfo OverrideKillInfo;

		public Vec3 BlowPosition;

		public WeaponFlags WeaponRecordWeaponFlags;

		public int WeaponItemKind;

		public int InflictedDamage;

		[MarshalAs(UnmanagedType.U1)]
		public bool IsMissile;

		[MarshalAs(UnmanagedType.U1)]
		public bool IsValid;
	}
}
