using System;

namespace TaleWorlds.Library
{
	public struct TWSharedMutexWriteLock : IDisposable
	{
		public TWSharedMutexWriteLock(TWSharedMutex mtx)
		{
			mtx.EnterWriteLock();
			this._mtx = mtx;
		}

		public void Dispose()
		{
			this._mtx.ExitWriteLock();
		}

		private readonly TWSharedMutex _mtx;
	}
}
