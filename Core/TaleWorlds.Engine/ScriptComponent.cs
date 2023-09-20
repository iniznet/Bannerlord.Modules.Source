using System;
using TaleWorlds.DotNet;

namespace TaleWorlds.Engine
{
	// Token: 0x02000080 RID: 128
	[EngineClass("rglScript_component")]
	public abstract class ScriptComponent : NativeObject
	{
		// Token: 0x060009D5 RID: 2517 RVA: 0x0000A908 File Offset: 0x00008B08
		protected ScriptComponent()
		{
		}

		// Token: 0x060009D6 RID: 2518 RVA: 0x0000A910 File Offset: 0x00008B10
		internal ScriptComponent(UIntPtr pointer)
		{
			base.Construct(pointer);
		}
	}
}
