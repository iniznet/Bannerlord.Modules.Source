using System;
using TaleWorlds.DotNet;

namespace TaleWorlds.Engine
{
	[EngineStruct("rglCull_mode")]
	public enum MBMeshCullingMode : byte
	{
		None,
		Backfaces,
		Frontfaces,
		Count
	}
}
