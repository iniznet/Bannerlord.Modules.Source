using System;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	[EngineStruct("ftlSphere_data")]
	public struct SphereData
	{
		public SphereData(float radius, Vec3 origin)
		{
			this.Radius = radius;
			this.Origin = origin;
		}

		public Vec3 Origin;

		public float Radius;
	}
}
