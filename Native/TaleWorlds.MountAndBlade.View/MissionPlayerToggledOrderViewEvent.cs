using System;
using TaleWorlds.Library.EventSystem;

namespace TaleWorlds.MountAndBlade.View
{
	// Token: 0x02000011 RID: 17
	public class MissionPlayerToggledOrderViewEvent : EventBase
	{
		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000071 RID: 113 RVA: 0x00005584 File Offset: 0x00003784
		// (set) Token: 0x06000072 RID: 114 RVA: 0x0000558C File Offset: 0x0000378C
		public bool IsOrderEnabled { get; private set; }

		// Token: 0x06000073 RID: 115 RVA: 0x00005595 File Offset: 0x00003795
		public MissionPlayerToggledOrderViewEvent(bool newIsEnabledState)
		{
			this.IsOrderEnabled = newIsEnabledState;
		}
	}
}
