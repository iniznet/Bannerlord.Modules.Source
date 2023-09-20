using System;
using System.Runtime.InteropServices;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200023F RID: 575
	[EngineStruct("Killing_blow")]
	public struct KillingBlow
	{
		// Token: 0x06001F5E RID: 8030 RVA: 0x0006F26C File Offset: 0x0006D46C
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
			this.BlowPosition = b.Position;
			this.WeaponRecordWeaponFlags = b.WeaponRecord.WeaponFlags;
			this.WeaponItemKind = weaponItemKind;
			this.InflictedDamage = b.InflictedDamage;
			this.IsMissile = b.IsMissile;
			this.IsValid = true;
		}

		// Token: 0x06001F5F RID: 8031 RVA: 0x0006F31D File Offset: 0x0006D51D
		public bool IsHeadShot()
		{
			return this.VictimBodyPart == BoneBodyPartType.Head;
		}

		// Token: 0x04000B76 RID: 2934
		public Vec3 RagdollImpulseLocalPoint;

		// Token: 0x04000B77 RID: 2935
		public Vec3 RagdollImpulseAmount;

		// Token: 0x04000B78 RID: 2936
		public int DeathAction;

		// Token: 0x04000B79 RID: 2937
		public DamageTypes DamageType;

		// Token: 0x04000B7A RID: 2938
		public AgentAttackType AttackType;

		// Token: 0x04000B7B RID: 2939
		public int OwnerId;

		// Token: 0x04000B7C RID: 2940
		public BoneBodyPartType VictimBodyPart;

		// Token: 0x04000B7D RID: 2941
		public int WeaponClass;

		// Token: 0x04000B7E RID: 2942
		public Agent.KillInfo OverrideKillInfo;

		// Token: 0x04000B7F RID: 2943
		public Vec3 BlowPosition;

		// Token: 0x04000B80 RID: 2944
		public WeaponFlags WeaponRecordWeaponFlags;

		// Token: 0x04000B81 RID: 2945
		public int WeaponItemKind;

		// Token: 0x04000B82 RID: 2946
		public int InflictedDamage;

		// Token: 0x04000B83 RID: 2947
		[MarshalAs(UnmanagedType.I1)]
		public bool IsMissile;

		// Token: 0x04000B84 RID: 2948
		[MarshalAs(UnmanagedType.I1)]
		public bool IsValid;
	}
}
