using System;
using System.Runtime.InteropServices;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020001D0 RID: 464
	[EngineStruct("Blow")]
	public struct Blow
	{
		// Token: 0x06001A38 RID: 6712 RVA: 0x0005C764 File Offset: 0x0005A964
		public Blow(int ownerId)
		{
			this = default(Blow);
			this.OwnerId = ownerId;
		}

		// Token: 0x17000527 RID: 1319
		// (get) Token: 0x06001A39 RID: 6713 RVA: 0x0005C774 File Offset: 0x0005A974
		public bool IsMissile
		{
			get
			{
				return this.WeaponRecord.IsMissile;
			}
		}

		// Token: 0x06001A3A RID: 6714 RVA: 0x0005C781 File Offset: 0x0005A981
		public bool IsBlowCrit(int maxHitPointsOfVictim)
		{
			return (float)this.InflictedDamage > (float)maxHitPointsOfVictim * 0.5f;
		}

		// Token: 0x06001A3B RID: 6715 RVA: 0x0005C794 File Offset: 0x0005A994
		public bool IsBlowLow(int maxHitPointsOfVictim)
		{
			return (float)this.InflictedDamage <= (float)maxHitPointsOfVictim * 0.1f;
		}

		// Token: 0x06001A3C RID: 6716 RVA: 0x0005C7AA File Offset: 0x0005A9AA
		public bool IsHeadShot()
		{
			return this.VictimBodyPart == BoneBodyPartType.Head;
		}

		// Token: 0x04000831 RID: 2097
		public BlowWeaponRecord WeaponRecord;

		// Token: 0x04000832 RID: 2098
		public Vec3 Position;

		// Token: 0x04000833 RID: 2099
		public Vec3 Direction;

		// Token: 0x04000834 RID: 2100
		public Vec3 SwingDirection;

		// Token: 0x04000835 RID: 2101
		public int InflictedDamage;

		// Token: 0x04000836 RID: 2102
		public int SelfInflictedDamage;

		// Token: 0x04000837 RID: 2103
		public float BaseMagnitude;

		// Token: 0x04000838 RID: 2104
		public float DefenderStunPeriod;

		// Token: 0x04000839 RID: 2105
		public float AttackerStunPeriod;

		// Token: 0x0400083A RID: 2106
		public float AbsorbedByArmor;

		// Token: 0x0400083B RID: 2107
		public float MovementSpeedDamageModifier;

		// Token: 0x0400083C RID: 2108
		public StrikeType StrikeType;

		// Token: 0x0400083D RID: 2109
		public AgentAttackType AttackType;

		// Token: 0x0400083E RID: 2110
		public BlowFlags BlowFlag;

		// Token: 0x0400083F RID: 2111
		public int OwnerId;

		// Token: 0x04000840 RID: 2112
		public sbyte BoneIndex;

		// Token: 0x04000841 RID: 2113
		public BoneBodyPartType VictimBodyPart;

		// Token: 0x04000842 RID: 2114
		public DamageTypes DamageType;

		// Token: 0x04000843 RID: 2115
		[MarshalAs(UnmanagedType.I1)]
		public bool NoIgnore;

		// Token: 0x04000844 RID: 2116
		[MarshalAs(UnmanagedType.I1)]
		public bool DamageCalculated;

		// Token: 0x04000845 RID: 2117
		[MarshalAs(UnmanagedType.I1)]
		public bool IsFallDamage;

		// Token: 0x04000846 RID: 2118
		public float DamagedPercentage;
	}
}
