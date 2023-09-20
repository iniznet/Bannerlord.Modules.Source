using System;

namespace TaleWorlds.Library
{
	// Token: 0x02000028 RID: 40
	public class EngineMethod : Attribute
	{
		// Token: 0x06000131 RID: 305 RVA: 0x000057C3 File Offset: 0x000039C3
		public EngineMethod(string engineMethodName, bool activateTelemetryProfiling = false)
		{
			this.EngineMethodName = engineMethodName;
			this.ActivateTelemetryProfiling = activateTelemetryProfiling;
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x06000132 RID: 306 RVA: 0x000057D9 File Offset: 0x000039D9
		// (set) Token: 0x06000133 RID: 307 RVA: 0x000057E1 File Offset: 0x000039E1
		public string EngineMethodName { get; private set; }

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x06000134 RID: 308 RVA: 0x000057EA File Offset: 0x000039EA
		// (set) Token: 0x06000135 RID: 309 RVA: 0x000057F2 File Offset: 0x000039F2
		public bool ActivateTelemetryProfiling { get; private set; }
	}
}
