using System;

namespace TaleWorlds.Engine
{
	[Flags]
	public enum PhysicsMaterialFlags : byte
	{
		None = 0,
		DontStickMissiles = 1,
		Flammable = 2,
		RainSplashesEnabled = 4,
		AttacksCanPassThrough = 8
	}
}
