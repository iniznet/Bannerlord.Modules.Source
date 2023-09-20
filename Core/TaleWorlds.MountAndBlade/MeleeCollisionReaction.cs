using System;

namespace TaleWorlds.MountAndBlade
{
	public enum MeleeCollisionReaction
	{
		Invalid = -1,
		SlicedThrough,
		ContinueChecking,
		Stuck,
		Bounced,
		Staggered
	}
}
