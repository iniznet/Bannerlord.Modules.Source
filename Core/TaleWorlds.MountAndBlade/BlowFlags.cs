using System;

namespace TaleWorlds.MountAndBlade
{
	[Flags]
	public enum BlowFlags
	{
		None = 0,
		KnockBack = 16,
		KnockDown = 32,
		NoSound = 64,
		CrushThrough = 128,
		ShrugOff = 256,
		MakesRear = 512,
		NonTipThrust = 1024,
		CanDismount = 2048
	}
}
