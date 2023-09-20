using System;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x0200000A RID: 10
	[EngineStruct("rglIntersection")]
	public struct Intersection
	{
		// Token: 0x0600002A RID: 42 RVA: 0x00002645 File Offset: 0x00000845
		public static bool DoSegmentsIntersect(Vec2 line1Start, Vec2 line1Direction, Vec2 line2Start, Vec2 line2Direction, ref Vec2 intersectionPoint)
		{
			return EngineApplicationInterface.IBodyPart.DoSegmentsIntersect(line1Start, line1Direction, line2Start, line2Direction, ref intersectionPoint);
		}

		// Token: 0x04000011 RID: 17
		internal UIntPtr doNotUse;

		// Token: 0x04000012 RID: 18
		internal UIntPtr doNotUse2;

		// Token: 0x04000013 RID: 19
		public float Penetration;

		// Token: 0x04000014 RID: 20
		public IntersectionType Type;

		// Token: 0x04000015 RID: 21
		public Vec3 IntersectionPoint;

		// Token: 0x04000016 RID: 22
		public Vec3 IntersectionNormal;
	}
}
