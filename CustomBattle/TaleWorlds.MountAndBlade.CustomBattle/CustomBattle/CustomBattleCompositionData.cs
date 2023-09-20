using System;

namespace TaleWorlds.MountAndBlade.CustomBattle.CustomBattle
{
	// Token: 0x02000018 RID: 24
	public struct CustomBattleCompositionData
	{
		// Token: 0x06000109 RID: 265 RVA: 0x000083B3 File Offset: 0x000065B3
		public CustomBattleCompositionData(float rangedPercentage, float cavalryPercentage, float rangedCavalryPercentage)
		{
			this.RangedPercentage = rangedPercentage;
			this.CavalryPercentage = cavalryPercentage;
			this.RangedCavalryPercentage = rangedCavalryPercentage;
			this.IsValid = true;
		}

		// Token: 0x040000AF RID: 175
		public readonly bool IsValid;

		// Token: 0x040000B0 RID: 176
		public readonly float RangedPercentage;

		// Token: 0x040000B1 RID: 177
		public readonly float CavalryPercentage;

		// Token: 0x040000B2 RID: 178
		public readonly float RangedCavalryPercentage;
	}
}
