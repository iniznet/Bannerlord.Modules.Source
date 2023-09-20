using System;

namespace TaleWorlds.Library
{
	public struct TWSharedMutexReadLock : IDisposable
	{
		public TWSharedMutexReadLock(TWSharedMutex mtx)
		{
			mtx.EnterReadLock();
			this._mtx = mtx;
		}

		public void Dispose()
		{
			this._mtx.ExitReadLock();
		}

		private readonly TWSharedMutex _mtx;
	}
}
