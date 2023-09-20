using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020001B6 RID: 438
	public static class MBItem
	{
		// Token: 0x0600196C RID: 6508 RVA: 0x0005B6B0 File Offset: 0x000598B0
		public static int GetItemUsageIndex(string itemUsageName)
		{
			return MBAPI.IMBItem.GetItemUsageIndex(itemUsageName);
		}

		// Token: 0x0600196D RID: 6509 RVA: 0x0005B6BD File Offset: 0x000598BD
		public static int GetItemHolsterIndex(string itemHolsterName)
		{
			return MBAPI.IMBItem.GetItemHolsterIndex(itemHolsterName);
		}

		// Token: 0x0600196E RID: 6510 RVA: 0x0005B6CA File Offset: 0x000598CA
		public static bool GetItemIsPassiveUsage(string itemUsageName)
		{
			return MBAPI.IMBItem.GetItemIsPassiveUsage(itemUsageName);
		}

		// Token: 0x0600196F RID: 6511 RVA: 0x0005B6D8 File Offset: 0x000598D8
		public static MatrixFrame GetHolsterFrameByIndex(int index)
		{
			MatrixFrame matrixFrame = default(MatrixFrame);
			MBAPI.IMBItem.GetHolsterFrameByIndex(index, ref matrixFrame);
			return matrixFrame;
		}

		// Token: 0x06001970 RID: 6512 RVA: 0x0005B6FB File Offset: 0x000598FB
		public static ItemObject.ItemUsageSetFlags GetItemUsageSetFlags(string ItemUsageName)
		{
			return (ItemObject.ItemUsageSetFlags)MBAPI.IMBItem.GetItemUsageSetFlags(ItemUsageName);
		}

		// Token: 0x06001971 RID: 6513 RVA: 0x0005B708 File Offset: 0x00059908
		public static ActionIndexValueCache GetItemUsageReloadActionCode(string itemUsageName, int usageDirection, bool isMounted, int leftHandUsageSetIndex, bool isLeftStance)
		{
			return new ActionIndexValueCache(MBAPI.IMBItem.GetItemUsageReloadActionCode(itemUsageName, usageDirection, isMounted, leftHandUsageSetIndex, isLeftStance));
		}

		// Token: 0x06001972 RID: 6514 RVA: 0x0005B71F File Offset: 0x0005991F
		public static int GetItemUsageStrikeType(string itemUsageName, int usageDirection, bool isMounted, int leftHandUsageSetIndex, bool isLeftStance)
		{
			return MBAPI.IMBItem.GetItemUsageStrikeType(itemUsageName, usageDirection, isMounted, leftHandUsageSetIndex, isLeftStance);
		}

		// Token: 0x06001973 RID: 6515 RVA: 0x0005B731 File Offset: 0x00059931
		public static float GetMissileRange(float shotSpeed, float zDiff)
		{
			return MBAPI.IMBItem.GetMissileRange(shotSpeed, zDiff);
		}
	}
}
