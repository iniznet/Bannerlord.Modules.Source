using System;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x02000007 RID: 7
	[EngineStruct("rglCapsule_data")]
	public struct CapsuleData
	{
		// Token: 0x06000026 RID: 38 RVA: 0x0000250A File Offset: 0x0000070A
		public CapsuleData(float radius, Vec3 p1, Vec3 p2)
		{
			this.Radius = radius;
			this.P1 = p1;
			this.P2 = p2;
			this.LocalRadius = radius;
			this.LocalP1 = p1;
			this.LocalP2 = p2;
		}

		// Token: 0x06000027 RID: 39 RVA: 0x00002538 File Offset: 0x00000738
		public Vec3 GetBoxMin()
		{
			return new Vec3(MathF.Min(this.P1.x, this.P2.x) - this.Radius, MathF.Min(this.P1.y, this.P2.y) - this.Radius, MathF.Min(this.P1.z, this.P2.z) - this.Radius, -1f);
		}

		// Token: 0x06000028 RID: 40 RVA: 0x000025B8 File Offset: 0x000007B8
		public Vec3 GetBoxMax()
		{
			return new Vec3(MathF.Max(this.P1.x, this.P2.x) + this.Radius, MathF.Max(this.P1.y, this.P2.y) + this.Radius, MathF.Max(this.P1.z, this.P2.z) + this.Radius, -1f);
		}

		// Token: 0x04000005 RID: 5
		public Vec3 P1;

		// Token: 0x04000006 RID: 6
		public Vec3 P2;

		// Token: 0x04000007 RID: 7
		public float Radius;

		// Token: 0x04000008 RID: 8
		internal float LocalRadius;

		// Token: 0x04000009 RID: 9
		internal Vec3 LocalP1;

		// Token: 0x0400000A RID: 10
		internal Vec3 LocalP2;
	}
}
