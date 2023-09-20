using System;

namespace TaleWorlds.Engine
{
	// Token: 0x0200008E RID: 142
	public static class Time
	{
		// Token: 0x17000089 RID: 137
		// (get) Token: 0x06000ABC RID: 2748 RVA: 0x0000BCB1 File Offset: 0x00009EB1
		public static float ApplicationTime
		{
			get
			{
				return EngineApplicationInterface.ITime.GetApplicationTime();
			}
		}
	}
}
