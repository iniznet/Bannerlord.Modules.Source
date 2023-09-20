using System;
using System.Runtime.InteropServices;

namespace TaleWorlds.DotNet
{
	// Token: 0x0200000F RID: 15
	[EngineStruct("ftlObject_type_definition")]
	internal struct EngineClassTypeDefinition
	{
		// Token: 0x04000022 RID: 34
		public int TypeId;

		// Token: 0x04000023 RID: 35
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		public string TypeName;
	}
}
