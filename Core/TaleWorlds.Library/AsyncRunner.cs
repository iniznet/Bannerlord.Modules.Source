using System;

namespace TaleWorlds.Library
{
	// Token: 0x0200000C RID: 12
	public abstract class AsyncRunner
	{
		// Token: 0x0600002A RID: 42
		public abstract void Run();

		// Token: 0x0600002B RID: 43
		public abstract void SyncTick();

		// Token: 0x0600002C RID: 44
		public abstract void OnRemove();
	}
}
