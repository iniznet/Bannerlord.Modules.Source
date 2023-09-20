using System;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020000F3 RID: 243
	[EngineStruct("Agent_spawn_data")]
	public struct AgentSpawnData
	{
		// Token: 0x04000299 RID: 665
		public int HitPoints;

		// Token: 0x0400029A RID: 666
		public int MonsterUsageIndex;

		// Token: 0x0400029B RID: 667
		public int Weight;

		// Token: 0x0400029C RID: 668
		public float StandingEyeHeight;

		// Token: 0x0400029D RID: 669
		public float CrouchEyeHeight;

		// Token: 0x0400029E RID: 670
		public float MountedEyeHeight;

		// Token: 0x0400029F RID: 671
		public float RiderEyeHeightAdder;

		// Token: 0x040002A0 RID: 672
		public float JumpAcceleration;

		// Token: 0x040002A1 RID: 673
		public Vec3 EyeOffsetWrtHead;

		// Token: 0x040002A2 RID: 674
		public Vec3 FirstPersonCameraOffsetWrtHead;

		// Token: 0x040002A3 RID: 675
		public float RiderCameraHeightAdder;

		// Token: 0x040002A4 RID: 676
		public float RiderBodyCapsuleHeightAdder;

		// Token: 0x040002A5 RID: 677
		public float RiderBodyCapsuleForwardAdder;

		// Token: 0x040002A6 RID: 678
		public float ArmLength;

		// Token: 0x040002A7 RID: 679
		public float ArmWeight;

		// Token: 0x040002A8 RID: 680
		public float JumpSpeedLimit;

		// Token: 0x040002A9 RID: 681
		public float RelativeSpeedLimitForCharge;
	}
}
