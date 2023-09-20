using System;
using TaleWorlds.DotNet;

namespace TaleWorlds.Engine
{
	// Token: 0x0200006B RID: 107
	[EngineClass("rglNative_script_component")]
	public sealed class NativeScriptComponent : ScriptComponent
	{
		// Token: 0x06000865 RID: 2149 RVA: 0x00008605 File Offset: 0x00006805
		internal NativeScriptComponent(UIntPtr pointer)
			: base(pointer)
		{
		}
	}
}
