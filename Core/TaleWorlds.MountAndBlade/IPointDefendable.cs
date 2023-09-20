using System;
using System.Collections.Generic;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000143 RID: 323
	public interface IPointDefendable
	{
		// Token: 0x17000389 RID: 905
		// (get) Token: 0x06001078 RID: 4216
		IEnumerable<DefencePoint> DefencePoints { get; }

		// Token: 0x1700038A RID: 906
		// (get) Token: 0x06001079 RID: 4217
		FormationAI.BehaviorSide DefenseSide { get; }

		// Token: 0x1700038B RID: 907
		// (get) Token: 0x0600107A RID: 4218
		WorldFrame MiddleFrame { get; }

		// Token: 0x1700038C RID: 908
		// (get) Token: 0x0600107B RID: 4219
		WorldFrame DefenseWaitFrame { get; }
	}
}
