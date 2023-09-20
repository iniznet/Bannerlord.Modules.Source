using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200037C RID: 892
	[Flags]
	public enum TargetFlags
	{
		// Token: 0x0400141F RID: 5151
		None = 0,
		// Token: 0x04001420 RID: 5152
		IsMoving = 1,
		// Token: 0x04001421 RID: 5153
		IsFlammable = 2,
		// Token: 0x04001422 RID: 5154
		IsStructure = 4,
		// Token: 0x04001423 RID: 5155
		IsSiegeEngine = 8,
		// Token: 0x04001424 RID: 5156
		IsAttacker = 16,
		// Token: 0x04001425 RID: 5157
		IsSmall = 32,
		// Token: 0x04001426 RID: 5158
		NotAThreat = 64,
		// Token: 0x04001427 RID: 5159
		DebugThreat = 128,
		// Token: 0x04001428 RID: 5160
		IsSiegeTower = 256
	}
}
