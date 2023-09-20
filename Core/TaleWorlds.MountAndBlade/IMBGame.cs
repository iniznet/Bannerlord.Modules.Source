using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020001A2 RID: 418
	[ScriptingInterfaceBase]
	internal interface IMBGame
	{
		// Token: 0x060016FD RID: 5885
		[EngineMethod("start_new", false)]
		void StartNew();

		// Token: 0x060016FE RID: 5886
		[EngineMethod("load_module_data", false)]
		void LoadModuleData(bool isLoadGame);
	}
}
