using System;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x0200000B RID: 11
	[EngineStruct("rglBounding_box")]
	public struct BoundingBox
	{
		// Token: 0x04000017 RID: 23
		public Vec3 min;

		// Token: 0x04000018 RID: 24
		public Vec3 max;

		// Token: 0x04000019 RID: 25
		public Vec3 center;

		// Token: 0x0400001A RID: 26
		public float radius;
	}
}
