using System;

namespace TaleWorlds.DotNet
{
	// Token: 0x0200000E RID: 14
	public class EngineClass : Attribute
	{
		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000035 RID: 53 RVA: 0x00002CD8 File Offset: 0x00000ED8
		// (set) Token: 0x06000036 RID: 54 RVA: 0x00002CE0 File Offset: 0x00000EE0
		public string EngineType { get; set; }

		// Token: 0x06000037 RID: 55 RVA: 0x00002CE9 File Offset: 0x00000EE9
		public EngineClass(string engineType)
		{
			this.EngineType = engineType;
		}
	}
}
