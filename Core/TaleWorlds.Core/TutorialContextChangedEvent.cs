using System;
using TaleWorlds.Library.EventSystem;

namespace TaleWorlds.Core
{
	// Token: 0x020000C3 RID: 195
	public class TutorialContextChangedEvent : EventBase
	{
		// Token: 0x17000334 RID: 820
		// (get) Token: 0x0600098A RID: 2442 RVA: 0x0001FA2E File Offset: 0x0001DC2E
		// (set) Token: 0x0600098B RID: 2443 RVA: 0x0001FA36 File Offset: 0x0001DC36
		public TutorialContexts NewContext { get; private set; }

		// Token: 0x0600098C RID: 2444 RVA: 0x0001FA3F File Offset: 0x0001DC3F
		public TutorialContextChangedEvent(TutorialContexts newContext)
		{
			this.NewContext = newContext;
		}
	}
}
