using System;

namespace TaleWorlds.Library
{
	public struct TWSharedMutexUpgradeableReadLock : IDisposable
	{
		public TWSharedMutexUpgradeableReadLock(TWSharedMutex mtx)
		{
			mtx.EnterUpgradeableReadLock();
			this._mtx = mtx;
		}

		public void Dispose()
		{
			this._mtx.ExitUpgradeableReadLock();
		}

		private readonly TWSharedMutex _mtx;
	}
}
