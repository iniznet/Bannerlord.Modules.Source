using System;
using System.Collections.Generic;

namespace TaleWorlds.Library
{
	// Token: 0x02000070 RID: 112
	public abstract class PathFinder
	{
		// Token: 0x060003CC RID: 972 RVA: 0x0000C081 File Offset: 0x0000A281
		public PathFinder()
		{
		}

		// Token: 0x060003CD RID: 973 RVA: 0x0000C089 File Offset: 0x0000A289
		public virtual void Destroy()
		{
		}

		// Token: 0x060003CE RID: 974
		public abstract void Initialize(Vec3 bbSize);

		// Token: 0x060003CF RID: 975
		public abstract bool FindPath(Vec3 wSource, Vec3 wDestination, List<Vec3> path, float craftWidth = 5f);

		// Token: 0x04000123 RID: 291
		public static float BuildingCost = 5000f;

		// Token: 0x04000124 RID: 292
		public static float WaterCost = 400f;

		// Token: 0x04000125 RID: 293
		public static float ShallowWaterCost = 100f;
	}
}
