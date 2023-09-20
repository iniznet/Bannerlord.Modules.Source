using System;

namespace TaleWorlds.TwoDimension.Standalone.Native.Windows
{
	[Flags]
	public enum BlurBehindConstraints : uint
	{
		Enable = 1U,
		BlurRegion = 2U,
		TransitionOnMaximized = 4U
	}
}
