using System;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
	// Token: 0x02000039 RID: 57
	[ApplicationInterfaceBase]
	internal interface ITime
	{
		// Token: 0x0600051C RID: 1308
		[EngineMethod("get_application_time", false)]
		float GetApplicationTime();
	}
}
