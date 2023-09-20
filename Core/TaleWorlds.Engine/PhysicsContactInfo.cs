using System;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x02000074 RID: 116
	[EngineStruct("rglPhysics_contact_info")]
	public struct PhysicsContactInfo
	{
		// Token: 0x04000156 RID: 342
		public Vec3 Position;

		// Token: 0x04000157 RID: 343
		public Vec3 Normal;

		// Token: 0x04000158 RID: 344
		public float Penetration;

		// Token: 0x04000159 RID: 345
		public Vec3 Impulse;

		// Token: 0x0400015A RID: 346
		public PhysicsMaterial PhysicsMaterial0;

		// Token: 0x0400015B RID: 347
		public PhysicsMaterial PhysicsMaterial1;
	}
}
