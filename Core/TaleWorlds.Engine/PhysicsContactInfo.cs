using System;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	[EngineStruct("rglPhysics_contact_info")]
	public struct PhysicsContactInfo
	{
		public Vec3 Position;

		public Vec3 Normal;

		public float Penetration;

		public Vec3 Impulse;

		public PhysicsMaterial PhysicsMaterial0;

		public PhysicsMaterial PhysicsMaterial1;
	}
}
