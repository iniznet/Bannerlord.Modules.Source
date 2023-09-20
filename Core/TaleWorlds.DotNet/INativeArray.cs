using System;
using TaleWorlds.Library;

namespace TaleWorlds.DotNet
{
	// Token: 0x02000018 RID: 24
	[LibraryInterfaceBase]
	internal interface INativeArray
	{
		// Token: 0x06000056 RID: 86
		[EngineMethod("get_data_pointer_offset", false)]
		int GetDataPointerOffset();

		// Token: 0x06000057 RID: 87
		[EngineMethod("create", false)]
		NativeArray Create();

		// Token: 0x06000058 RID: 88
		[EngineMethod("get_data_size", false)]
		int GetDataSize(UIntPtr pointer);

		// Token: 0x06000059 RID: 89
		[EngineMethod("get_data_pointer", false)]
		UIntPtr GetDataPointer(UIntPtr pointer);

		// Token: 0x0600005A RID: 90
		[EngineMethod("add_integer_element", false)]
		void AddIntegerElement(UIntPtr pointer, int value);

		// Token: 0x0600005B RID: 91
		[EngineMethod("add_float_element", false)]
		void AddFloatElement(UIntPtr pointer, float value);

		// Token: 0x0600005C RID: 92
		[EngineMethod("add_element", false)]
		void AddElement(UIntPtr pointer, IntPtr element, int elementSize);

		// Token: 0x0600005D RID: 93
		[EngineMethod("clear", false)]
		void Clear(UIntPtr pointer);
	}
}
