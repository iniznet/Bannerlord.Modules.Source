using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x020001A8 RID: 424
	[ScriptingInterfaceBase]
	internal interface IMBBannerlordConfig
	{
		// Token: 0x06001733 RID: 5939
		[EngineMethod("validate_options", false)]
		void ValidateOptions();
	}
}
