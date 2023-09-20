using System;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	[EngineStruct("ftlCapsule_data", false)]
	internal struct FtlCapsuleData
	{
		public FtlCapsuleData(float radius, Vec3 p1, Vec3 p2)
		{
			this.P1 = p1;
			this.P2 = p2;
			this.Radius = radius;
		}

		public Vec3 GetBoxMin()
		{
			return new Vec3(MathF.Min(this.P1.x, this.P2.x) - this.Radius, MathF.Min(this.P1.y, this.P2.y) - this.Radius, MathF.Min(this.P1.z, this.P2.z) - this.Radius, -1f);
		}

		public Vec3 GetBoxMax()
		{
			return new Vec3(MathF.Max(this.P1.x, this.P2.x) + this.Radius, MathF.Max(this.P1.y, this.P2.y) + this.Radius, MathF.Max(this.P1.z, this.P2.z) + this.Radius, -1f);
		}

		public Vec3 P1;

		public Vec3 P2;

		public float Radius;
	}
}
