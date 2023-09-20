using System;
using TaleWorlds.Library;

namespace TaleWorlds.DotNet
{
	// Token: 0x0200001A RID: 26
	[LibraryInterfaceBase]
	internal interface INativeStringHelper
	{
		// Token: 0x06000063 RID: 99
		[EngineMethod("create_rglVarString", false)]
		UIntPtr CreateRglVarString(string text);

		// Token: 0x06000064 RID: 100
		[EngineMethod("get_thread_local_cached_rglVarString", false)]
		UIntPtr GetThreadLocalCachedRglVarString();

		// Token: 0x06000065 RID: 101
		[EngineMethod("set_rglVarString", false)]
		void SetRglVarString(UIntPtr pointer, string text);

		// Token: 0x06000066 RID: 102
		[EngineMethod("delete_rglVarString", false)]
		void DeleteRglVarString(UIntPtr pointer);
	}
}
