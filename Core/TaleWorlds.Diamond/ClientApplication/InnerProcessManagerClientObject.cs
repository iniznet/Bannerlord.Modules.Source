using System;
using TaleWorlds.Diamond.InnerProcess;

namespace TaleWorlds.Diamond.ClientApplication
{
	// Token: 0x0200005A RID: 90
	public class InnerProcessManagerClientObject : DiamondClientApplicationObject
	{
		// Token: 0x1700006F RID: 111
		// (get) Token: 0x06000214 RID: 532 RVA: 0x0000640F File Offset: 0x0000460F
		// (set) Token: 0x06000215 RID: 533 RVA: 0x00006417 File Offset: 0x00004617
		public InnerProcessManager InnerProcessManager { get; private set; }

		// Token: 0x06000216 RID: 534 RVA: 0x00006420 File Offset: 0x00004620
		public InnerProcessManagerClientObject(DiamondClientApplication application, InnerProcessManager innerProcessManager)
			: base(application)
		{
			this.InnerProcessManager = innerProcessManager;
		}
	}
}
