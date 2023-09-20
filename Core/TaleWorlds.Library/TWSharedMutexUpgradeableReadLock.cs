using System;

namespace TaleWorlds.Library
{
	// Token: 0x02000094 RID: 148
	public struct TWSharedMutexUpgradeableReadLock : IDisposable
	{
		// Token: 0x06000502 RID: 1282 RVA: 0x0000FEA0 File Offset: 0x0000E0A0
		public TWSharedMutexUpgradeableReadLock(TWSharedMutex mtx)
		{
			mtx.EnterUpgradeableReadLock();
			this._mtx = mtx;
		}

		// Token: 0x06000503 RID: 1283 RVA: 0x0000FEAF File Offset: 0x0000E0AF
		public void Dispose()
		{
			this._mtx.ExitUpgradeableReadLock();
		}

		// Token: 0x0400017D RID: 381
		private readonly TWSharedMutex _mtx;
	}
}
