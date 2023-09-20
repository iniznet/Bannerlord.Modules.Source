using System;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	[EngineStruct("rglBounding_box")]
	public struct BoundingBox
	{
		public Vec3 min;

		public Vec3 max;

		public Vec3 center;

		public float radius;
	}
}
