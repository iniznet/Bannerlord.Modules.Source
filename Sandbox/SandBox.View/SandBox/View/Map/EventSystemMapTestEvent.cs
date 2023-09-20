using System;
using TaleWorlds.Library.EventSystem;

namespace SandBox.View.Map
{
	// Token: 0x02000051 RID: 81
	public class EventSystemMapTestEvent : EventBase
	{
		// Token: 0x1700005F RID: 95
		// (get) Token: 0x0600035D RID: 861 RVA: 0x0001CEA7 File Offset: 0x0001B0A7
		// (set) Token: 0x0600035E RID: 862 RVA: 0x0001CEAF File Offset: 0x0001B0AF
		public int TestNum { get; private set; }

		// Token: 0x0600035F RID: 863 RVA: 0x0001CEB8 File Offset: 0x0001B0B8
		public EventSystemMapTestEvent(int testNum)
		{
			this.TestNum = testNum;
		}
	}
}
