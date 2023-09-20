using System;
using System.Collections.Generic;

namespace TaleWorlds.DotNet
{
	// Token: 0x02000012 RID: 18
	public interface ICallbackManager
	{
		// Token: 0x0600003F RID: 63
		void Initialize();

		// Token: 0x06000040 RID: 64
		Delegate[] GetDelegates();

		// Token: 0x06000041 RID: 65
		Dictionary<string, object> GetScriptingInterfaceObjects();

		// Token: 0x06000042 RID: 66
		void SetFunctionPointer(int id, IntPtr pointer);

		// Token: 0x06000043 RID: 67
		void CheckSharedStructureSizes();
	}
}
