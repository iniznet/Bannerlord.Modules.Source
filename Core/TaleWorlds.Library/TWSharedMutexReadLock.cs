using System;

namespace TaleWorlds.Library
{
	// Token: 0x02000092 RID: 146
	public struct TWSharedMutexReadLock : IDisposable
	{
		// Token: 0x060004FE RID: 1278 RVA: 0x0000FE68 File Offset: 0x0000E068
		public TWSharedMutexReadLock(TWSharedMutex mtx)
		{
			mtx.EnterReadLock();
			this._mtx = mtx;
		}

		// Token: 0x060004FF RID: 1279 RVA: 0x0000FE77 File Offset: 0x0000E077
		public void Dispose()
		{
			this._mtx.ExitReadLock();
		}

		// Token: 0x0400017B RID: 379
		private readonly TWSharedMutex _mtx;
	}
}
