using System;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x0200002A RID: 42
	[ApplicationInterfaceBase]
	internal interface IAsyncTask
	{
		// Token: 0x060003FB RID: 1019
		[EngineMethod("create_with_function", false)]
		AsyncTask CreateWithDelegate(ManagedDelegate function, bool isBackground);

		// Token: 0x060003FC RID: 1020
		[EngineMethod("invoke", false)]
		void Invoke(UIntPtr Pointer);

		// Token: 0x060003FD RID: 1021
		[EngineMethod("wait", false)]
		void Wait(UIntPtr Pointer);
	}
}
