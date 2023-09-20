using System;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x02000008 RID: 8
	[EngineStruct("ftlSphere_data")]
	public struct SphereData
	{
		// Token: 0x06000029 RID: 41 RVA: 0x00002635 File Offset: 0x00000835
		public SphereData(float radius, Vec3 origin)
		{
			this.Radius = radius;
			this.Origin = origin;
		}

		// Token: 0x0400000B RID: 11
		public Vec3 Origin;

		// Token: 0x0400000C RID: 12
		public float Radius;
	}
}
