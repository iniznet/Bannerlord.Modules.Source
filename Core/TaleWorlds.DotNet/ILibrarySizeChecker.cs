using System;
using TaleWorlds.Library;

namespace TaleWorlds.DotNet
{
	// Token: 0x02000017 RID: 23
	[LibraryInterfaceBase]
	internal interface ILibrarySizeChecker
	{
		// Token: 0x06000055 RID: 85
		[EngineMethod("get_engine_struct_size", false)]
		int GetEngineStructSize(string str);
	}
}
