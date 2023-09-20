using System;
using TaleWorlds.Library;

namespace TaleWorlds.DotNet
{
	// Token: 0x0200001B RID: 27
	[LibraryInterfaceBase]
	internal interface INativeString
	{
		// Token: 0x06000067 RID: 103
		[EngineMethod("create", false)]
		NativeString Create();

		// Token: 0x06000068 RID: 104
		[EngineMethod("get_string", false)]
		string GetString(NativeString nativeString);

		// Token: 0x06000069 RID: 105
		[EngineMethod("set_string", false)]
		void SetString(NativeString nativeString, string newString);
	}
}
