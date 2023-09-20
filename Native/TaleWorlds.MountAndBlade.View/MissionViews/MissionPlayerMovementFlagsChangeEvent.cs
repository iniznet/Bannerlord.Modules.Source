using System;
using TaleWorlds.Library.EventSystem;

namespace TaleWorlds.MountAndBlade.View.MissionViews
{
	// Token: 0x0200004D RID: 77
	public class MissionPlayerMovementFlagsChangeEvent : EventBase
	{
		// Token: 0x17000051 RID: 81
		// (get) Token: 0x06000365 RID: 869 RVA: 0x0001E18A File Offset: 0x0001C38A
		// (set) Token: 0x06000366 RID: 870 RVA: 0x0001E192 File Offset: 0x0001C392
		public Agent.MovementControlFlag MovementFlag { get; private set; }

		// Token: 0x06000367 RID: 871 RVA: 0x0001E19B File Offset: 0x0001C39B
		public MissionPlayerMovementFlagsChangeEvent(Agent.MovementControlFlag movementFlag)
		{
			this.MovementFlag = movementFlag;
		}
	}
}
