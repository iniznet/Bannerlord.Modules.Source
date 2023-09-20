using System;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x02000098 RID: 152
	public static class EngineExtensions
	{
		// Token: 0x06000B88 RID: 2952 RVA: 0x0000CDCD File Offset: 0x0000AFCD
		public static WorldPosition ToWorldPosition(this Vec3 vec3, Scene scene)
		{
			return new WorldPosition(scene, UIntPtr.Zero, vec3, false);
		}
	}
}
