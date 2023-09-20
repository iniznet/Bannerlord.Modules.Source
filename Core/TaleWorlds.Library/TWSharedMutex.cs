using System;
using System.Threading;

namespace TaleWorlds.Library
{
	public class TWSharedMutex
	{
		public TWSharedMutex()
		{
			this._mutex = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
		}

		public void EnterReadLock()
		{
			this._mutex.EnterReadLock();
		}

		public void EnterUpgradeableReadLock()
		{
			this._mutex.EnterUpgradeableReadLock();
		}

		public void EnterWriteLock()
		{
			this._mutex.EnterWriteLock();
		}

		public void ExitReadLock()
		{
			this._mutex.ExitReadLock();
		}

		public void ExitUpgradeableReadLock()
		{
			this._mutex.ExitUpgradeableReadLock();
		}

		public void ExitWriteLock()
		{
			this._mutex.ExitWriteLock();
		}

		public bool IsReadLockHeld
		{
			get
			{
				return this._mutex.IsReadLockHeld;
			}
		}

		public bool IsUpgradeableReadLockHeld
		{
			get
			{
				return this._mutex.IsUpgradeableReadLockHeld;
			}
		}

		public bool IsWriteLockHeld
		{
			get
			{
				return this._mutex.IsWriteLockHeld;
			}
		}

		private ReaderWriterLockSlim _mutex;
	}
}
