using System;

namespace TaleWorlds.Core
{
	[Flags]
	public enum SkinMask
	{
		NoneVisible = 0,
		HeadVisible = 1,
		BodyVisible = 32,
		UnderwearVisible = 64,
		HandsVisible = 128,
		LegsVisible = 256,
		AllVisible = 481
	}
}
