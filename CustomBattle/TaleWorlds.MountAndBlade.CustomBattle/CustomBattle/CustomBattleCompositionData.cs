using System;

namespace TaleWorlds.MountAndBlade.CustomBattle.CustomBattle
{
	public struct CustomBattleCompositionData
	{
		public CustomBattleCompositionData(float rangedPercentage, float cavalryPercentage, float rangedCavalryPercentage)
		{
			this.RangedPercentage = rangedPercentage;
			this.CavalryPercentage = cavalryPercentage;
			this.RangedCavalryPercentage = rangedCavalryPercentage;
			this.IsValid = true;
		}

		public readonly bool IsValid;

		public readonly float RangedPercentage;

		public readonly float CavalryPercentage;

		public readonly float RangedCavalryPercentage;
	}
}
