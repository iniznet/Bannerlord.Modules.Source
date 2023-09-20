using System;

namespace TaleWorlds.DotNet
{
	// Token: 0x02000011 RID: 17
	public class EngineStruct : Attribute
	{
		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000039 RID: 57 RVA: 0x00002D00 File Offset: 0x00000F00
		// (set) Token: 0x0600003A RID: 58 RVA: 0x00002D08 File Offset: 0x00000F08
		public string EngineType { get; set; }

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600003B RID: 59 RVA: 0x00002D11 File Offset: 0x00000F11
		// (set) Token: 0x0600003C RID: 60 RVA: 0x00002D19 File Offset: 0x00000F19
		public string AlternateDotNetType { get; set; }

		// Token: 0x0600003D RID: 61 RVA: 0x00002D22 File Offset: 0x00000F22
		public EngineStruct(string engineType)
		{
			this.EngineType = engineType;
			this.AlternateDotNetType = null;
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00002D38 File Offset: 0x00000F38
		public EngineStruct(string engineType, string alternateDotNetType)
		{
			this.EngineType = engineType;
			this.AlternateDotNetType = alternateDotNetType;
		}
	}
}
