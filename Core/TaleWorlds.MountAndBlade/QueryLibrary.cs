using System;

namespace TaleWorlds.MountAndBlade
{
	public static class QueryLibrary
	{
		public static bool IsInfantry(Agent a)
		{
			return !a.HasMount && !a.IsRangedCached;
		}

		public static bool IsInfantryWithoutBanner(Agent a)
		{
			return a.Banner == null && !a.HasMount && !a.IsRangedCached;
		}

		public static bool HasShield(Agent a)
		{
			return a.HasShieldCached;
		}

		public static bool IsRanged(Agent a)
		{
			return !a.HasMount && a.IsRangedCached;
		}

		public static bool IsRangedWithoutBanner(Agent a)
		{
			return a.Banner == null && !a.HasMount && a.IsRangedCached;
		}

		public static bool IsCavalry(Agent a)
		{
			return a.HasMount && !a.IsRangedCached;
		}

		public static bool IsCavalryWithoutBanner(Agent a)
		{
			return a.Banner == null && a.HasMount && !a.IsRangedCached;
		}

		public static bool IsRangedCavalry(Agent a)
		{
			return a.HasMount && a.IsRangedCached;
		}

		public static bool IsRangedCavalryWithoutBanner(Agent a)
		{
			return a.Banner == null && a.HasMount && a.IsRangedCached;
		}

		public static bool HasSpear(Agent a)
		{
			return a.HasSpearCached;
		}

		public static bool HasThrown(Agent a)
		{
			return a.HasThrownCached;
		}

		public static bool IsHeavy(Agent a)
		{
			return MissionGameModels.Current.AgentStatCalculateModel.HasHeavyArmor(a);
		}
	}
}
