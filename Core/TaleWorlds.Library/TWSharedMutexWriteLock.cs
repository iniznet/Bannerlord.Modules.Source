using System;

namespace TaleWorlds.Library
{
	// Token: 0x02000093 RID: 147
	public struct TWSharedMutexWriteLock : IDisposable
	{
		// Token: 0x06000500 RID: 1280 RVA: 0x0000FE84 File Offset: 0x0000E084
		public TWSharedMutexWriteLock(TWSharedMutex mtx)
		{
			mtx.EnterWriteLock();
			this._mtx = mtx;
		}

		// Token: 0x06000501 RID: 1281 RVA: 0x0000FE93 File Offset: 0x0000E093
		public void Dispose()
		{
			this._mtx.ExitWriteLock();
		}

		// Token: 0x0400017C RID: 380
		private readonly TWSharedMutex _mtx;
	}
}
