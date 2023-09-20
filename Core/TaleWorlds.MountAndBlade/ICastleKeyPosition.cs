using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000350 RID: 848
	public interface ICastleKeyPosition
	{
		// Token: 0x17000828 RID: 2088
		// (get) Token: 0x06002D92 RID: 11666
		// (set) Token: 0x06002D93 RID: 11667
		IPrimarySiegeWeapon AttackerSiegeWeapon { get; set; }

		// Token: 0x17000829 RID: 2089
		// (get) Token: 0x06002D94 RID: 11668
		TacticalPosition MiddlePosition { get; }

		// Token: 0x1700082A RID: 2090
		// (get) Token: 0x06002D95 RID: 11669
		TacticalPosition WaitPosition { get; }

		// Token: 0x1700082B RID: 2091
		// (get) Token: 0x06002D96 RID: 11670
		WorldFrame MiddleFrame { get; }

		// Token: 0x1700082C RID: 2092
		// (get) Token: 0x06002D97 RID: 11671
		WorldFrame DefenseWaitFrame { get; }

		// Token: 0x1700082D RID: 2093
		// (get) Token: 0x06002D98 RID: 11672
		FormationAI.BehaviorSide DefenseSide { get; }

		// Token: 0x06002D99 RID: 11673
		Vec3 GetPosition();
	}
}
