using System;
using System.Threading;

namespace TaleWorlds.Library
{
	// Token: 0x02000091 RID: 145
	public class TWSharedMutex
	{
		// Token: 0x060004F4 RID: 1268 RVA: 0x0000FDDF File Offset: 0x0000DFDF
		public TWSharedMutex()
		{
			this._mutex = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
		}

		// Token: 0x060004F5 RID: 1269 RVA: 0x0000FDF3 File Offset: 0x0000DFF3
		public void EnterReadLock()
		{
			this._mutex.EnterReadLock();
		}

		// Token: 0x060004F6 RID: 1270 RVA: 0x0000FE00 File Offset: 0x0000E000
		public void EnterUpgradeableReadLock()
		{
			this._mutex.EnterUpgradeableReadLock();
		}

		// Token: 0x060004F7 RID: 1271 RVA: 0x0000FE0D File Offset: 0x0000E00D
		public void EnterWriteLock()
		{
			this._mutex.EnterWriteLock();
		}

		// Token: 0x060004F8 RID: 1272 RVA: 0x0000FE1A File Offset: 0x0000E01A
		public void ExitReadLock()
		{
			this._mutex.ExitReadLock();
		}

		// Token: 0x060004F9 RID: 1273 RVA: 0x0000FE27 File Offset: 0x0000E027
		public void ExitUpgradeableReadLock()
		{
			this._mutex.ExitUpgradeableReadLock();
		}

		// Token: 0x060004FA RID: 1274 RVA: 0x0000FE34 File Offset: 0x0000E034
		public void ExitWriteLock()
		{
			this._mutex.ExitWriteLock();
		}

		// Token: 0x17000088 RID: 136
		// (get) Token: 0x060004FB RID: 1275 RVA: 0x0000FE41 File Offset: 0x0000E041
		public bool IsReadLockHeld
		{
			get
			{
				return this._mutex.IsReadLockHeld;
			}
		}

		// Token: 0x17000089 RID: 137
		// (get) Token: 0x060004FC RID: 1276 RVA: 0x0000FE4E File Offset: 0x0000E04E
		public bool IsUpgradeableReadLockHeld
		{
			get
			{
				return this._mutex.IsUpgradeableReadLockHeld;
			}
		}

		// Token: 0x1700008A RID: 138
		// (get) Token: 0x060004FD RID: 1277 RVA: 0x0000FE5B File Offset: 0x0000E05B
		public bool IsWriteLockHeld
		{
			get
			{
				return this._mutex.IsWriteLockHeld;
			}
		}

		// Token: 0x0400017A RID: 378
		private ReaderWriterLockSlim _mutex;
	}
}
