using System;

namespace TaleWorlds.MountAndBlade
{
	[Flags]
	public enum TargetFlags
	{
		None = 0,
		IsMoving = 1,
		IsFlammable = 2,
		IsStructure = 4,
		IsSiegeEngine = 8,
		IsAttacker = 16,
		IsSmall = 32,
		NotAThreat = 64,
		DebugThreat = 128,
		IsSiegeTower = 256
	}
}
