using System;
using TaleWorlds.Library;

namespace TaleWorlds.DotNet
{
	// Token: 0x02000019 RID: 25
	[LibraryInterfaceBase]
	internal interface INativeObjectArray
	{
		// Token: 0x0600005E RID: 94
		[EngineMethod("create", false)]
		NativeObjectArray Create();

		// Token: 0x0600005F RID: 95
		[EngineMethod("get_count", false)]
		int GetCount(UIntPtr pointer);

		// Token: 0x06000060 RID: 96
		[EngineMethod("add_element", false)]
		void AddElement(UIntPtr pointer, UIntPtr nativeObject);

		// Token: 0x06000061 RID: 97
		[EngineMethod("get_element_at_index", false)]
		NativeObject GetElementAtIndex(UIntPtr pointer, int index);

		// Token: 0x06000062 RID: 98
		[EngineMethod("clear", false)]
		void Clear(UIntPtr pointer);
	}
}
