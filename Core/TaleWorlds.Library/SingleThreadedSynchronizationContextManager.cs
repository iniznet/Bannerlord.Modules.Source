using System;
using System.Threading;

namespace TaleWorlds.Library
{
	// Token: 0x02000084 RID: 132
	public static class SingleThreadedSynchronizationContextManager
	{
		// Token: 0x0600047D RID: 1149 RVA: 0x0000EC60 File Offset: 0x0000CE60
		public static void Initialize()
		{
			if (SingleThreadedSynchronizationContextManager._synchronizationContext == null)
			{
				SingleThreadedSynchronizationContextManager._synchronizationContext = new SingleThreadedSynchronizationContext();
				SynchronizationContext.SetSynchronizationContext(SingleThreadedSynchronizationContextManager._synchronizationContext);
			}
		}

		// Token: 0x0600047E RID: 1150 RVA: 0x0000EC7D File Offset: 0x0000CE7D
		public static void Tick()
		{
			SingleThreadedSynchronizationContextManager._synchronizationContext.Tick();
		}

		// Token: 0x0400015F RID: 351
		private static SingleThreadedSynchronizationContext _synchronizationContext;
	}
}
