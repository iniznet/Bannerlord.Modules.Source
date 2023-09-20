using System;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	[EngineStruct("rglIntersection")]
	public struct Intersection
	{
		public static bool DoSegmentsIntersect(Vec2 line1Start, Vec2 line1Direction, Vec2 line2Start, Vec2 line2Direction, ref Vec2 intersectionPoint)
		{
			return EngineApplicationInterface.IBodyPart.DoSegmentsIntersect(line1Start, line1Direction, line2Start, line2Direction, ref intersectionPoint);
		}

		internal UIntPtr doNotUse;

		internal UIntPtr doNotUse2;

		public float Penetration;

		public IntersectionType Type;

		public Vec3 IntersectionPoint;

		public Vec3 IntersectionNormal;
	}
}
