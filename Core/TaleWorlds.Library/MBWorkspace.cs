using System;

namespace TaleWorlds.Library
{
	// Token: 0x02000069 RID: 105
	public class MBWorkspace<T> where T : IMBCollection, new()
	{
		// Token: 0x06000398 RID: 920 RVA: 0x0000B50D File Offset: 0x0000970D
		public T StartUsingWorkspace()
		{
			this._isBeingUsed = true;
			if (this._workspace == null)
			{
				this._workspace = new T();
			}
			return this._workspace;
		}

		// Token: 0x06000399 RID: 921 RVA: 0x0000B534 File Offset: 0x00009734
		public void StopUsingWorkspace()
		{
			this._isBeingUsed = false;
			this._workspace.Clear();
		}

		// Token: 0x0600039A RID: 922 RVA: 0x0000B54E File Offset: 0x0000974E
		public T GetWorkspace()
		{
			return this._workspace;
		}

		// Token: 0x04000113 RID: 275
		private bool _isBeingUsed;

		// Token: 0x04000114 RID: 276
		private T _workspace;
	}
}
