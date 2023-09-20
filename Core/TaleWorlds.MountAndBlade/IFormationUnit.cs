using System;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000131 RID: 305
	public interface IFormationUnit
	{
		// Token: 0x17000357 RID: 855
		// (get) Token: 0x06000ED6 RID: 3798
		IFormationArrangement Formation { get; }

		// Token: 0x17000358 RID: 856
		// (get) Token: 0x06000ED7 RID: 3799
		// (set) Token: 0x06000ED8 RID: 3800
		int FormationFileIndex { get; set; }

		// Token: 0x17000359 RID: 857
		// (get) Token: 0x06000ED9 RID: 3801
		// (set) Token: 0x06000EDA RID: 3802
		int FormationRankIndex { get; set; }

		// Token: 0x1700035A RID: 858
		// (get) Token: 0x06000EDB RID: 3803
		IFormationUnit FollowedUnit { get; }

		// Token: 0x1700035B RID: 859
		// (get) Token: 0x06000EDC RID: 3804
		bool IsShieldUsageEncouraged { get; }

		// Token: 0x1700035C RID: 860
		// (get) Token: 0x06000EDD RID: 3805
		bool IsPlayerUnit { get; }
	}
}
