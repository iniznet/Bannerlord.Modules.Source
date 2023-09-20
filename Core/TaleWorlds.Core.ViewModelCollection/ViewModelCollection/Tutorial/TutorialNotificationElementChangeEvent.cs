using System;
using TaleWorlds.Library.EventSystem;

namespace TaleWorlds.Core.ViewModelCollection.Tutorial
{
	// Token: 0x02000010 RID: 16
	public class TutorialNotificationElementChangeEvent : EventBase
	{
		// Token: 0x17000040 RID: 64
		// (get) Token: 0x060000B7 RID: 183 RVA: 0x0000324A File Offset: 0x0000144A
		// (set) Token: 0x060000B8 RID: 184 RVA: 0x00003252 File Offset: 0x00001452
		public string NewNotificationElementID { get; private set; }

		// Token: 0x060000B9 RID: 185 RVA: 0x0000325B File Offset: 0x0000145B
		public TutorialNotificationElementChangeEvent(string newNotificationElementID)
		{
			this.NewNotificationElementID = newNotificationElementID;
		}
	}
}
