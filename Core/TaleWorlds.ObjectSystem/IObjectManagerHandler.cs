using System;

namespace TaleWorlds.ObjectSystem
{
	// Token: 0x02000002 RID: 2
	public interface IObjectManagerHandler
	{
		// Token: 0x06000001 RID: 1
		void AfterCreateObject(MBObjectBase objectBase);

		// Token: 0x06000002 RID: 2
		void AfterUnregisterObject(MBObjectBase objectBase);
	}
}
