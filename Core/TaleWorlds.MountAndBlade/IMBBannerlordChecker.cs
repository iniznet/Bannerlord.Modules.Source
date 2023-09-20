using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020001A6 RID: 422
	[ScriptingInterfaceBase]
	internal interface IMBBannerlordChecker
	{
		// Token: 0x0600172F RID: 5935
		[EngineMethod("get_engine_struct_size", false)]
		int GetEngineStructSize(string str);
	}
}
