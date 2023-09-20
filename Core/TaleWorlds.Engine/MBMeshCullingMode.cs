using System;
using TaleWorlds.DotNet;

namespace TaleWorlds.Engine
{
	// Token: 0x0200005F RID: 95
	[EngineStruct("rglCull_mode")]
	public enum MBMeshCullingMode : byte
	{
		// Token: 0x04000105 RID: 261
		None,
		// Token: 0x04000106 RID: 262
		Backfaces,
		// Token: 0x04000107 RID: 263
		Frontfaces,
		// Token: 0x04000108 RID: 264
		Count
	}
}
