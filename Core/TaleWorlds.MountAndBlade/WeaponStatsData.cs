using System;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000395 RID: 917
	[EngineStruct("Weapon_stats_data")]
	public struct WeaponStatsData
	{
		// Token: 0x0400152B RID: 5419
		public MatrixFrame WeaponFrame;

		// Token: 0x0400152C RID: 5420
		public Vec3 RotationSpeed;

		// Token: 0x0400152D RID: 5421
		public ulong WeaponFlags;

		// Token: 0x0400152E RID: 5422
		public uint Properties;

		// Token: 0x0400152F RID: 5423
		public int WeaponClass;

		// Token: 0x04001530 RID: 5424
		public int AmmoClass;

		// Token: 0x04001531 RID: 5425
		public int ItemUsageIndex;

		// Token: 0x04001532 RID: 5426
		public int ThrustSpeed;

		// Token: 0x04001533 RID: 5427
		public int SwingSpeed;

		// Token: 0x04001534 RID: 5428
		public int MissileSpeed;

		// Token: 0x04001535 RID: 5429
		public int ShieldArmor;

		// Token: 0x04001536 RID: 5430
		public int ThrustDamage;

		// Token: 0x04001537 RID: 5431
		public int SwingDamage;

		// Token: 0x04001538 RID: 5432
		public int DefendSpeed;

		// Token: 0x04001539 RID: 5433
		public int Accuracy;

		// Token: 0x0400153A RID: 5434
		public int WeaponLength;

		// Token: 0x0400153B RID: 5435
		public float WeaponBalance;

		// Token: 0x0400153C RID: 5436
		public float SweetSpot;

		// Token: 0x0400153D RID: 5437
		public short MaxDataValue;

		// Token: 0x0400153E RID: 5438
		public short ReloadPhaseCount;

		// Token: 0x0400153F RID: 5439
		public int ThrustDamageType;

		// Token: 0x04001540 RID: 5440
		public int SwingDamageType;
	}
}
