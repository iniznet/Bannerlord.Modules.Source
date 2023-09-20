using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000169 RID: 361
	public static class QueryLibrary
	{
		// Token: 0x0600125E RID: 4702 RVA: 0x000473CB File Offset: 0x000455CB
		public static bool IsInfantry(Agent a)
		{
			return !a.HasMount && !a.IsRangedCached;
		}

		// Token: 0x0600125F RID: 4703 RVA: 0x000473E0 File Offset: 0x000455E0
		public static bool IsInfantryWithoutBanner(Agent a)
		{
			return a.Banner == null && !a.HasMount && !a.IsRangedCached;
		}

		// Token: 0x06001260 RID: 4704 RVA: 0x000473FD File Offset: 0x000455FD
		public static bool HasShield(Agent a)
		{
			return a.HasShieldCached;
		}

		// Token: 0x06001261 RID: 4705 RVA: 0x00047405 File Offset: 0x00045605
		public static bool IsRanged(Agent a)
		{
			return !a.HasMount && a.IsRangedCached;
		}

		// Token: 0x06001262 RID: 4706 RVA: 0x00047417 File Offset: 0x00045617
		public static bool IsRangedWithoutBanner(Agent a)
		{
			return a.Banner == null && !a.HasMount && a.IsRangedCached;
		}

		// Token: 0x06001263 RID: 4707 RVA: 0x00047431 File Offset: 0x00045631
		public static bool IsCavalry(Agent a)
		{
			return a.HasMount && !a.IsRangedCached;
		}

		// Token: 0x06001264 RID: 4708 RVA: 0x00047446 File Offset: 0x00045646
		public static bool IsCavalryWithoutBanner(Agent a)
		{
			return a.Banner == null && a.HasMount && !a.IsRangedCached;
		}

		// Token: 0x06001265 RID: 4709 RVA: 0x00047463 File Offset: 0x00045663
		public static bool IsRangedCavalry(Agent a)
		{
			return a.HasMount && a.IsRangedCached;
		}

		// Token: 0x06001266 RID: 4710 RVA: 0x00047475 File Offset: 0x00045675
		public static bool IsRangedCavalryWithoutBanner(Agent a)
		{
			return a.Banner == null && a.HasMount && a.IsRangedCached;
		}

		// Token: 0x06001267 RID: 4711 RVA: 0x0004748F File Offset: 0x0004568F
		public static bool HasSpear(Agent a)
		{
			return a.HasSpearCached;
		}

		// Token: 0x06001268 RID: 4712 RVA: 0x00047497 File Offset: 0x00045697
		public static bool HasThrown(Agent a)
		{
			return a.HasThrownCached;
		}

		// Token: 0x06001269 RID: 4713 RVA: 0x0004749F File Offset: 0x0004569F
		public static bool IsHeavy(Agent a)
		{
			return MissionGameModels.Current.AgentStatCalculateModel.HasHeavyArmor(a);
		}
	}
}
