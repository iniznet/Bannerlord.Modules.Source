using System;
using System.Collections.Generic;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000351 RID: 849
	public interface IPrimarySiegeWeapon
	{
		// Token: 0x06002D9A RID: 11674
		bool HasCompletedAction();

		// Token: 0x1700082E RID: 2094
		// (get) Token: 0x06002D9B RID: 11675
		float SiegeWeaponPriority { get; }

		// Token: 0x1700082F RID: 2095
		// (get) Token: 0x06002D9C RID: 11676
		int OverTheWallNavMeshID { get; }

		// Token: 0x17000830 RID: 2096
		// (get) Token: 0x06002D9D RID: 11677
		bool HoldLadders { get; }

		// Token: 0x17000831 RID: 2097
		// (get) Token: 0x06002D9E RID: 11678
		bool SendLadders { get; }

		// Token: 0x17000832 RID: 2098
		// (get) Token: 0x06002D9F RID: 11679
		MissionObject TargetCastlePosition { get; }

		// Token: 0x17000833 RID: 2099
		// (get) Token: 0x06002DA0 RID: 11680
		FormationAI.BehaviorSide WeaponSide { get; }

		// Token: 0x06002DA1 RID: 11681
		bool GetNavmeshFaceIds(out List<int> navmeshFaceIds);
	}
}
