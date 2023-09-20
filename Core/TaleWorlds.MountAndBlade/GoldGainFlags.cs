using System;

namespace TaleWorlds.MountAndBlade
{
	[Flags]
	public enum GoldGainFlags : ushort
	{
		FirstRangedKill = 1,
		FirstMeleeKill = 2,
		FirstAssist = 4,
		SecondAssist = 8,
		ThirdAssist = 16,
		FifthKill = 32,
		TenthKill = 64,
		DefaultKill = 128,
		DefaultAssist = 256,
		ObjectiveCompleted = 512,
		ObjectiveDestroyed = 1024,
		PerkBonus = 2048
	}
}
