using System;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x02000037 RID: 55
	[ApplicationInterfaceBase]
	internal interface IEngineSizeChecker
	{
		// Token: 0x06000500 RID: 1280
		[EngineMethod("get_engine_struct_size", false)]
		int GetEngineStructSize(string str);
	}
}
